# 0008 – Database application user with a development password

- **Status:** Accepted
- **Date:** 2026-10-07

## Context

Both applications need a database account. Connecting as `root` would give them rights to
drop tables or read other databases. A separate setup step for the user, with a password the
reviewer chooses and enters in two configuration files, makes the first start error-prone.
The project is a local demo; there is no shared or public server.

## Decision

- `database/setup.sql` also creates the user `employee_app` with the fixed development
  password `employee_app_dev`, for `localhost`, `127.0.0.1` and `::1` (TCP connections
  arrive with one of the addresses, depending on the client and the server's name
  resolution).
- Minimal rights: `SELECT, INSERT, UPDATE, DELETE` on `employees`, only `SELECT` on
  `departments` (a fixed list, [ADR 0002](0002-departments-table.md)).
- `CREATE OR REPLACE USER` keeps the script rerunnable.
- The committed configuration of both apps (`appsettings.json`, `web/config/config.php`)
  contains these credentials, so both apps run right after the setup script without any
  copying or editing.
- The desktop app can still override the connection string in the git-ignored
  `appsettings.Local.json`.

## Consequences

- One script and no manual configuration from a fresh database to two running apps.
- The password is public. This is acceptable for a local demo only; any shared deployment
  needs its own password, kept out of Git.
- Running the setup script resets the user's password and rights together with the data.
