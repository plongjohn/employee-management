# 0006 – Logging with Serilog

- **Status:** Accepted
- **Date:** 2026-10-05

## Context

The desktop application runs on the users' own machines. If something goes wrong, for
example a missing connection string or an unreachable database, there has to be a place to
read what happened. The built-in providers of `Microsoft.Extensions.Logging` only write to the
console, the debugger or the Windows event log; none of them writes files.

## Decision

The desktop app logs to files with **Serilog** (`Serilog.Extensions.Hosting`,
`Serilog.Sinks.File`, `Serilog.Settings.Configuration`).

- Files: `logs/employee-management-<date>.log` next to the executable, one file per day, the
  last 14 are kept. The path is set in code, because a relative path in the configuration
  would depend on the working directory.
- Log levels are configured in `appsettings.json` (`Information` by default, framework
  messages from `Warning`), so they can be changed without rebuilding.
- A bootstrap logger records failures that happen before the configuration is loaded.
- Core only depends on `ILogger<T>` from `Microsoft.Extensions.Logging.Abstractions`; Serilog
  is wired up in the desktop project alone.
- **No personal data in logs:** create, update and delete are logged with the employee id
  only, never with names or email addresses. Conflicts and duplicate emails are warnings,
  unexpected errors are logged with their stack trace.

## Consequences

- The log is found next to the `.exe`, e.g. `bin/Debug/net10.0-windows/logs/`.
- Installing the app under `Program Files` would need a writable log location instead
  (e.g. `%LOCALAPPDATA%`) – not relevant for this project.
- Three additional NuGet packages in the desktop project.
