# Employee Management

[![CI](https://github.com/plongjohn/employee-management/actions/workflows/ci.yml/badge.svg)](https://github.com/plongjohn/employee-management/actions/workflows/ci.yml)

Employee management system built for a Full-Stack Developer (C# / PHP) coding challenge.
Two independent applications – a Windows Forms desktop app and a PHP web app – manage the
same employee data in a shared MariaDB database.

> **Status:** work in progress. Sections marked _TODO_ are filled in as the project evolves.

## Tech Stack

| Component | Technology |
|---|---|
| Desktop | C#, Windows Forms, .NET 10 |
| Web | PHP 8.5 (object-oriented), PHP-DI, Twig, Bootstrap 5 |
| Database | MariaDB |
| Tests | xUnit v3 (desktop), PHPUnit and PHPStan (web) |
| Logging | Serilog (desktop), Monolog (web) – rolling log files |

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

The setup script creates everything both apps need, including the database user. The
committed configuration of both apps already contains its credentials, so there is nothing
to copy or edit.

### 1. Database

Run from the repository root as `root` (or another user that may create databases and
users):

```powershell
mariadb -u root -p -e "source database/setup.sql"
```

The script creates the database `employee_management`, both tables, the sample data and the
restricted user `employee_app` with the password `employee_app_dev`.

> **Warning:** the script drops and recreates the tables. Running it again resets all data
> to the sample data.

If `mariadb` is not found, it is not on the `PATH`; use the full path instead, e.g.
`& "C:\Program Files\MariaDB 13.0\bin\mariadb.exe" -u root -p -e "source database/setup.sql"`.

`employee_app_dev` is a development password for this local demo only
([ADR 0008](docs/decisions/0008-database-application-user.md)). Details on schema, indexes
and sample data: [`database/README.md`](database/README.md).

### 2. Desktop

Open `desktop/EmployeeManagement.slnx` in Visual Studio and run `EmployeeManagement.Desktop`,
or:

```powershell
dotnet run --project desktop/EmployeeManagement.Desktop
```

The connection string is in `desktop/EmployeeManagement.Desktop/appsettings.json`. To use
other credentials without changing that file, create `appsettings.Local.json` next to it
(git-ignored); its values take precedence:

```json
{
  "Database": {
    "ConnectionString": "Server=localhost;Port=3306;Database=employee_management;User ID=employee_app;Password=..."
  }
}
```

Without a connection string the app shows a message and exits; details are written to the log.

**Logs** are written next to the executable, e.g.
`desktop/EmployeeManagement.Desktop/bin/Debug/net10.0-windows/logs/employee-management-<date>.log`
(one file per day, the last 14 are kept). The log level can be changed in `appsettings.json`
(`Serilog:MinimumLevel`). Logs contain employee ids only, no names or email addresses.

### 3. Web

Install the dependencies, start the built-in PHP server and open <http://localhost:8000>:

```powershell
cd web
composer install
php -S localhost:8000 -t public
```

The database settings are in `web/config/config.php`. For Apache, point the document root to
`web/public`; `.htaccess` routes all requests to `index.php` (requires `mod_rewrite`).

Without a database configuration the app shows an error page; details are written to the log.
Bootstrap and Bootstrap Icons are loaded from the jsDelivr CDN, so the browser needs internet
access for the styling.

**Logs** are written to `web/var/log/employee-management-<date>.log` (one file per day, the
last 14 are kept). The log level is set with `logLevel` in `config/config.php`. Logs contain
employee ids only, no names or email addresses. Compiled templates are cached in
`web/var/cache/`; the folder `web/var/` must be writable.

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

## Using the Web App

The web app follows the same rules as the desktop app: the same search, filter, sorting and
paging, the same validation and the same handling of concurrent changes.

- **List:** search with `Enter` or the magnifier button; changing the department or the page
  size updates the list right away. Search, filter, sorting and page are part of the URL, so a
  view can be bookmarked, and saving, cancelling or deleting returns to the same view.
- **Add / edit:** the save button behaves as in the desktop app. Leaving the form with
  unsaved changes asks for confirmation.
- **Concurrent changes:** if another user changed the employee in the meantime, the form keeps
  your input and offers to reload the current data.

## Running Tests

```powershell
dotnet test desktop/EmployeeManagement.slnx
```

The desktop tests use xUnit v3 on Microsoft.Testing.Platform (enabled in `global.json`) and
cover validation, the service layer and query building. They need no database.

```powershell
cd web
composer test      # PHPUnit
composer analyse   # PHPStan, level 8
```

The web tests cover the same rules plus routing, CSRF protection and the parsing of list
parameters. They need no database either.

### Continuous Integration

GitHub Actions ([`.github/workflows/ci.yml`](.github/workflows/ci.yml)) runs on every push to
`development` and `main` and on pull requests to `main`, in three parallel jobs:

| Job | Runner | Checks |
|---|---|---|
| Desktop | Windows | Build with warnings as errors, xUnit tests |
| Web | Linux | `composer validate`, `composer audit`, PHPUnit, PHPStan |
| Database | Linux, MariaDB 13 | `setup.sql` runs twice without errors, `employee_app` reads the sample data but cannot change departments |

Pull requests to `main` can only be merged when all three jobs pass.

## Design Decisions

Architecture decision records are located in [`docs/decisions/`](docs/decisions/).
