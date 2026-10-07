# Database

MariaDB schema and sample data shared by the desktop and the web application.

| File | Purpose |
|---|---|
| `setup.sql` | Creates the database `employee_management`, both tables, indexes, sample data and the application user `employee_app` |

## Setup

Run from the repository root as `root`. `-e "source …"` works in PowerShell, cmd and bash
alike (PowerShell does not support `<` input redirection).

```powershell
mariadb -u root -p -e "source database/setup.sql"
```

> **Warning:** the script drops and recreates the tables. Running it again resets all data
> to the sample data.

If `mariadb` is not on the `PATH`, use the full path, e.g.
`& "C:\Program Files\MariaDB 13.0\bin\mariadb.exe"`.

## Application user

The applications never connect as `root`. The script (re)creates `employee_app` for
`localhost`, `127.0.0.1` and `::1` with the password `employee_app_dev`, which the committed
configuration of both apps uses. It is a development password for this local demo only
([ADR 0008](../docs/decisions/0008-database-application-user.md)).

`employee_app` may read, insert, update and delete employees, but only read departments.

## Schema

```text
departments                          employees
  id    INT PK                         id             INT PK
  name  VARCHAR(100) UNIQUE   <──┐     first_name     VARCHAR(100)
                                 │     last_name      VARCHAR(100)
                                 │     email          VARCHAR(255) UNIQUE
                                 └──── department_id  INT FK (ON DELETE RESTRICT)
                                       hire_date      DATE
                                       version        INT UNSIGNED, starts at 1
                                       created_at     TIMESTAMP
                                       updated_at     TIMESTAMP (auto-updated)
```

- All text columns are required and must not be blank (`CHECK` constraints).
- Character set `utf8mb4`, collation `utf8mb4_uca1400_ai_ci`: comparisons ignore case and
  accents, so searching for `muller` finds `Müller` and `Max@example.com` counts as a
  duplicate of `max@example.com`.
- Departments are a fixed list; there is no UI to maintain them
  ([ADR 0002](../docs/decisions/0002-departments-table.md)).
- `version` protects against lost updates when two users edit the same employee
  ([ADR 0003](../docs/decisions/0003-optimistic-concurrency.md)).

### Indexes

| Index | Used for |
|---|---|
| `idx_employees_last_first (last_name, first_name)` | Prefix search by last name, sorting by name |
| `idx_employees_first_name (first_name)` | Prefix search by first name |
| `uq_employees_email (email)` | Unique email, prefix search by email |
| `idx_employees_department_name (department_id, last_name, first_name)` | Department filter sorted by name, foreign key |
| `idx_employees_hire_date (hire_date)` | Sorting by hire date with paging |

Search uses prefix matching (`LIKE 'abc%'`) so that these indexes can be used; a leading
wildcard (`'%abc%'`) would force a full table scan.

## Sample data

8 departments and 20 employees with hire dates between 2010 and 2026. The data includes
umlauts, `ß`, an apostrophe (`O'Connor`), hyphenated and long names and two employees with
the same last name. The department *Finanzen* has no employees on purpose, to show an empty
filter result.
