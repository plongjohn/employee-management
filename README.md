# Employee Management

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

1. Install the dependencies:

   ```powershell
   cd web
   composer install
   ```

2. Create the local configuration with the password of `employee_app` (git-ignored):

   ```powershell
   Copy-Item config/config.example.php config/config.php
   # edit config.php and replace CHANGE_ME
   ```

3. Start the built-in PHP server and open <http://localhost:8000>:

   ```powershell
   php -S localhost:8000 -t public
   ```

   For Apache, point the document root to `web/public`; `.htaccess` routes all requests to
   `index.php` (requires `mod_rewrite`).

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

## Design Decisions

Architecture decision records are located in [`docs/decisions/`](docs/decisions/).
