# 0003 – Optimistic concurrency with a version column

- **Status:** Accepted
- **Date:** 2026-10-05

## Context

The desktop and the web application work on the same database, and several users can use
them at the same time. Without protection, a *lost update* can happen:

1. A and B open the same employee.
2. A changes the department and saves.
3. B changes the email and saves – and silently overwrites A's change.

Locking rows while a form is open (pessimistic locking) does not fit: forms stay open for
minutes, and the web application is stateless between requests.

## Decision

`employees.version` (`INT UNSIGNED`, starts at 1) is checked and incremented on every write:

```sql
UPDATE employees
SET first_name = ?, ..., version = version + 1
WHERE id = ? AND version = ?;

DELETE FROM employees WHERE id = ? AND version = ?;
```

The applications keep the version they loaded. If the statement affects 0 rows, the employee
was changed or deleted in the meantime: the user gets a clear message and can reload the
current data. Changes are never overwritten silently.

### Alternatives considered

- **Compare `updated_at`:** needs no extra column, but `TIMESTAMP` has one-second precision,
  so two changes within the same second are not detected. An update that changes no values
  also does not touch `updated_at`; drivers then report 0 affected rows unless they are set
  to return *found* rows instead.
- **Compare all original values in the `WHERE` clause:** detects conflicts, but every update
  statement grows with the number of columns and is easy to get wrong.

## Consequences

- Reliable conflict detection independent of timestamp precision; because `version` always
  changes, 0 affected rows always means a conflict.
- Both applications must pass the loaded version through their forms (hidden field in the web
  app) and handle the conflict case.
- Deleting with an outdated version is reported as a conflict as well and is never forced.
