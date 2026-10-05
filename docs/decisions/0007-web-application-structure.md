# 0007 – Web application structure

- **Status:** Accepted
- **Date:** 2026-10-05

## Context

The web application has to be object-oriented PHP with a clear structure and a modern
frontend. It manages the same employees as the desktop app and must follow the same rules
(validation, search and paging, optimistic concurrency, unique email). A full framework such
as Symfony or Laravel would bring routing, DI and forms, but also far more code and
conventions than one CRUD screen needs.

## Decision

Plain PHP 8.5 without a framework, built from a few established libraries:

| Concern | Choice |
|---|---|
| Entry point | Front controller `public/index.php`; `.htaccess` for Apache |
| Routing | Small own `Router` (method + path pattern, e.g. `/employees/{id:\d+}/edit`) |
| Dependency injection | PHP-DI (PSR-11) with autowiring; only interfaces and configured objects are defined in `ContainerFactory` |
| Database | PDO with real prepared statements (`ATTR_EMULATE_PREPARES = false`) |
| Templates | Twig with auto-escaping |
| Texts | `lang/de.php`, accessed through `Translator` and the Twig function `t()` |
| Logging | Monolog, rotating files in `var/log/`, same rules as the desktop app ([ADR 0006](0006-logging-with-serilog.md)) |
| Frontend | Bootstrap 5.3 and Bootstrap Icons from jsDelivr with Subresource Integrity, small vanilla JavaScript |
| Quality | PHPUnit, PHPStan level 8 |

The layers mirror the desktop core: `Controllers` → `Services` (validation, `OperationResult`)
→ `Repositories` (SQL). Models, enums, validation codes and log messages carry the same names
as in C#, so both implementations can be compared side by side.

**Requests and forms**

- Only GET and POST are used, since HTML forms know no other methods:
  `POST /employees/{id}` updates, `POST /employees/{id}/delete` deletes. Nothing changes data
  on GET.
- Every POST carries a CSRF token from the session (synchronizer token, compared with
  `hash_equals`). The session cookie is `HttpOnly` and `SameSite=Lax`.
- After a successful POST the app redirects with `303 See Other` (Post/Redirect/Get);
  success and warning messages survive the redirect as flash messages in the session.
- Invalid input re-renders the form with status 422, a conflict with 409. The loaded `version`
  travels as a hidden field ([ADR 0003](0003-optimistic-concurrency.md)).
- The list state (search, department, sorting, page, page size) lives in the query string and
  is passed through the forms as hidden fields. Return URLs are rebuilt from these parsed values
  and never taken from the request, so they cannot point to another site.

**Validation in the browser** only controls the save button (enabled when something changed and
all required fields are filled) and asks before discarding changes. The form uses `novalidate`;
all messages come from the server, which validates every request.

### Alternatives considered

- **Symfony or Laravel:** well known and complete, but most of the framework would stay unused.
- **Manual wiring instead of a container:** works at this size, but every new class means
  editing the composition root; PHP-DI keeps that to interfaces and configuration.
- **Bootstrap served locally:** works offline, but adds third-party files to the repository.
  The CDN with integrity hashes keeps the repository free of them while the browser still
  rejects modified files.

## Consequences

- Few dependencies; the code that runs a request can be read in a few files.
- Routing, request handling and CSRF protection are own code and have their own tests.
- The styling needs internet access because of the CDN.
- Business rules exist twice, in C# and PHP. The shared names and the mirrored tests keep
  both versions aligned; a change to a rule has to be made in both apps.
