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
| Tests | xUnit (desktop), PHPUnit (web) |

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

_TODO_

### Desktop

_TODO_

### Web

_TODO_

## Running Tests

_TODO_

## Design Decisions

Architecture decision records are located in [`docs/decisions/`](docs/decisions/).
