# Employee Management

Employee management system built for a Full-Stack Developer (C# / PHP) coding challenge.
Two independent applications – a Windows Forms desktop app and a PHP web app – manage the
same employee data in a shared MariaDB database.

> **Status:** work in progress. Sections marked _TODO_ are filled in as the project evolves.

## Tech Stack

| Component | Technology |
|---|---|
| Desktop | C#, Windows Forms, .NET 10 |
| Web | PHP 8.5 (object-oriented), Twig, Bootstrap 5 |
| Database | MariaDB |
| Tests | xUnit v3 (desktop), PHPUnit (web) |
| Logging | Serilog, rolling log files (desktop) |

## Project Structure

```text
database/   SQL scripts: schema and sample data
desktop/    .NET solution (EmployeeManagement.slnx)
  EmployeeManagement.Desktop/      Windows Forms UI
  EmployeeManagement.Core/         Models, services, validation, data access
  EmployeeManagement.Core.Tests/   xUnit tests for the core layer
web/        PHP web application
docs/       Architecture documentation and decision records
```

## Prerequisites

- .NET 10 SDK (any 10.0.x feature band)
- PHP 8.5 with the `pdo_mysql` and `mbstring` extensions
- Composer 2
- MariaDB

## Setup

### Database

Run from the repository root:

```powershell
# 1. Database, tables and sample data (drops existing tables)
mariadb -u root -p -e "source database/setup.sql"

# 2. Restricted application user – set a password in the copy first (file is git-ignored)
Copy-Item database/create-user.example.sql database/create-user.sql
mariadb -u root -p -e "source database/create-user.sql"
```

Details on schema, indexes and sample data: [`database/README.md`](database/README.md).

### Desktop

1. Create the local settings file with the password of `employee_app` (git-ignored):

   ```powershell
   Copy-Item desktop/EmployeeManagement.Desktop/appsettings.Local.example.json `
             desktop/EmployeeManagement.Desktop/appsettings.Local.json
   # edit appsettings.Local.json and replace CHANGE_ME
   ```

2. Build and start – either open `desktop/EmployeeManagement.slnx` in Visual Studio and run
   `EmployeeManagement.Desktop`, or:

   ```powershell
   dotnet run --project desktop/EmployeeManagement.Desktop
   ```

Without a connection string the app shows a message and exits; details are written to the log.

**Logs** are written next to the executable, e.g.
`desktop/EmployeeManagement.Desktop/bin/Debug/net10.0-windows/logs/employee-management-<date>.log`
(one file per day, the last 14 are kept). The log level can be changed in `appsettings.json`
(`Serilog:MinimumLevel`). Logs contain employee ids only, no names or email addresses.

### Web

_TODO_

## Using the Desktop App

- **List:** search by the beginning of a first name, last name or email (several words narrow
  the result, e.g. `anna mü`), filter by department, sort by clicking a column header and
  page through the result. The page size can be set in the footer.
- **Add / edit:** required fields are marked with `*`. The save button stays disabled until
  something has changed and all required fields are filled. Invalid input is explained in red
  below the field.
- **Delete:** asks for confirmation first.
- **Concurrent changes:** if another user changed or deleted the employee in the meantime,
  nothing is overwritten. Edit offers to reload the current data, delete is cancelled and the
  list is refreshed.

| Shortcut | Action |
|---|---|
| `Ctrl+N` | Add employee |
| `Enter` / `F2` | Edit selected employee (in the list) |
| `Del` | Delete selected employee (in the list) |
| `F5` | Refresh list |
| `Ctrl+F` | Jump to the search field |
| `Esc` | Close the form (asks before discarding changes) |

## Running Tests

```powershell
dotnet test desktop/EmployeeManagement.slnx
```

The desktop tests use xUnit v3 on Microsoft.Testing.Platform (enabled in `global.json`) and
cover validation, the service layer and query building. They need no database.

## Design Decisions

Architecture decision records are located in [`docs/decisions/`](docs/decisions/).
