# 0002 – Departments as a separate table

- **Status:** Accepted
- **Date:** 2026-10-05

## Context

The requirements list the department as one of the employee's fields, without prescribing how
it is stored. Both applications need to assign a department to an employee and filter the
employee list by department, including an "all departments" view.

Stored as free text, the same department can end up in several spellings
(`Logistik`, `Logisitk`, `logistik `), which breaks filtering and grouping.

## Decision

Departments live in their own table `departments (id, name UNIQUE)`. `employees.department_id`
references it with a foreign key (`ON DELETE RESTRICT`).

- The department list is fixed and created by `database/setup.sql`. There is no UI to
  maintain departments; the application user only has `SELECT` on this table.
- Both UIs pick the department from a dropdown filled from this table.
- "All departments" is **not** a row in the table but a UI option meaning "no department
  filter". The repositories add the department condition to the query only when a department
  is selected, so the filter stays parameterized and can use the index
  `idx_employees_department_name`.

## Consequences

- Consistent department names; typos are impossible.
- A department that still has employees cannot be deleted.
- Changing the list requires editing `setup.sql` (acceptable for a fixed list in this scope).
- Reading employees needs a join to show the department name.
