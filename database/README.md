# Database

MariaDB schema and sample data shared by the desktop and the web application.

| File | Purpose |
|---|---|
| `setup.sql` | Creates the database `employee_management`, both tables, indexes and sample data |
| `create-user.example.sql` | Template for the restricted application user `employee_app` |

## Setup

Run the commands from the repository root. `-e "source …"` works in PowerShell, cmd and bash
alike (PowerShell does not support `<` input redirection).

1. Create the database, tables and sample data:

   ```powershell
   mariadb -u root -p -e "source database/setup.sql"
   ```

   > **Warning:** the script drops and recreates the tables. Running it again resets all
   > data to the sample data.

2. Create the application user. Copy the template, replace `CHANGE_ME` with a password of
   your choice and run it. `create-user.sql` is git-ignored, so the password stays local.

   ```powershell
   Copy-Item database/create-user.example.sql database/create-user.sql
   # edit database/create-user.sql and set the password
   mariadb -u root -p -e "source database/create-user.sql"
   ```

3. Use `employee_app` and this password in the desktop and web configuration.

The applications never connect as `root`. `employee_app` may read, insert, update and delete
employees, but only read departments.

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
