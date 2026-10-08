# 0010 – Data access with Dapper instead of Entity Framework Core

- **Status:** Accepted
- **Date:** 2026-10-08

## Context

The desktop app needs data access for two tables (`employees`, `departments`). The schema is
defined by `database/setup.sql`, which also provides the sample data, and the PHP web app
uses the same tables through PDO. Two kinds of tools are available:

- **Entity Framework Core**, a full ORM: queries are written in LINQ and translated to SQL,
  loaded entities are change-tracked, and the schema can be generated from the C# model
  through migrations.
- **Dapper**, a micro-ORM: the SQL is written by hand; Dapper fills parameters from objects
  and maps result rows to objects.

Some queries depend on the exact SQL: the prefix search must stay index-friendly, the sort
order must let MariaDB read an index in order, and updates and deletes must carry the
version condition ([ADR 0003](0003-optimistic-concurrency.md),
[ADR 0004](0004-search-sorting-paging.md)).

## Decision

- The repositories use **Dapper** with **MySqlConnector** as the ADO.NET driver.
- All SQL is written in the repositories and parameterized; column names in `ORDER BY` come
  from a fixed mapping, never from user input.
- Database errors are translated in the repository (`DuplicateEmailException`,
  `DepartmentNotFoundException`), so services never see driver-specific exceptions.

Reasons against EF Core here:

- Migrations, one of its main strengths, do not fit: the schema is owned by `setup.sql` and
  shared with the PHP app, so the C# model must follow the script rather than define it.
- With two tables there is little to gain from LINQ and change tracking, while the SQL that
  matters for indexes and concurrency is easier to control and review when written directly.
  It also stays close to the web app's SQL, so both apps behave the same.
- The EF Core provider for MariaDB is a community project (Pomelo), an additional dependency
  for the database access.

## Consequences

- SQL lives in strings that the compiler does not check. A renamed column fails only when
  the statement runs; a column alias that matches no property is ignored by Dapper and leaves
  the property at its default value.
- The repositories have no automated tests – the unit tests use in-memory fakes, and the CI
  database job checks `setup.sql` and the user's rights, not the application's queries.
  Integration tests against MariaDB in CI would close this gap.
- Dynamic parts (search terms, department filter, sorting) are built by hand in
  `EmployeeRepository`.
- Schema changes are made in `setup.sql` and in the SQL of both apps together.
- For a larger application that owns its schema, EF Core would be the better default; the
  optimistic concurrency pattern carries over (`ExecuteUpdateAsync` with the version in the
  `Where`, or a concurrency token).
