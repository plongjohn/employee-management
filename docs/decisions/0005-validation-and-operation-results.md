# 0005 – Validation error codes and operation results

- **Status:** Accepted
- **Date:** 2026-10-05

## Context

`EmployeeManagement.Core` contains the business rules but must not depend on the UI. The UI
language is German, an English switch may follow, and the tests should not break when a
message text changes.

Saving an employee can fail for expected reasons: invalid input, an email address already in
use, a conflicting change by another user or an employee that was deleted in the meantime.

## Decision

**Validation returns codes, not texts.** `EmployeeValidator` returns a list of
`ValidationError` values (e.g. `FirstNameRequired`, `EmailInvalid`). Each code identifies the
field and the rule; the desktop app maps it to a text from its `.resx` resources.

Rules: first and last name required, at most 100 characters; email required, at most 255
characters, simple format `x@y.z`; department required; hire date between 1950-01-01 and one
year from today. Text fields are trimmed before validation and storage. The email pattern is
the same in both applications.

**Write operations return an `OperationResult`** with a status: `Success`, `ValidationFailed`
(with the error codes), `DuplicateEmail`, `Conflict` or `NotFound`. Create and update return
the employee as reloaded from the database, including its new version.

The repository translates MariaDB errors into `DuplicateEmailException` (error 1062 on
`uq_employees_email`) and `DepartmentNotFoundException` (error 1452); the service turns them
into results. Exceptions only leave the service for unexpected errors such as an unreachable
database.

If an update or delete matches no row, the service checks whether the employee still exists:
`NotFound` if not, `Conflict` if another user changed it. A conflicting delete is never
forced.

## Consequences

- Core stays language-neutral; texts and translations live in the UI only.
- Tests assert codes and statuses, not strings.
- The UI must handle every status explicitly; the compiler does not enforce this.
