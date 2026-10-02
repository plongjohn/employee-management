# 0001 – Desktop project structure

- **Status:** Accepted
- **Date:** 2026-10-02

## Context

The challenge asks for a Windows Forms application on .NET 10 with a clean separation
of UI and data access. The initial project outline used a single Windows Forms project
with folders (`Forms/`, `Models/`, `Repositories/`, `Services/`) and a classic `.sln` file.

Business logic and validation must be covered by unit tests. With a single project, the
test project would have to reference the Windows Forms application, pulling the UI and its
Windows-only target framework into the tests.

## Decision

The desktop solution consists of three projects in a flat layout under `desktop/`:

| Project | Target | Responsibility |
|---|---|---|
| `EmployeeManagement.Desktop` | `net10.0-windows` | Windows Forms UI, DI host, configuration |
| `EmployeeManagement.Core` | `net10.0` | Models, services, validation, repositories (Dapper) |
| `EmployeeManagement.Core.Tests` | `net10.0` | xUnit tests for the core layer |

References: `Desktop → Core`, `Core.Tests → Core`.

The solution uses the `.slnx` format, the default for new solutions in .NET 10.
Shared build settings (`Nullable`, `ImplicitUsings`) live in `desktop/Directory.Build.props`.
A root `global.json` requires a .NET 10 SDK but allows any 10.0.x feature band.

## Consequences

- Core targets plain `net10.0` and therefore cannot depend on Windows Forms – the business
  logic is UI-independent by construction, and tests run without any UI dependency.
- Folder names inside the projects follow the original outline (`Models/`, `Repositories/`,
  `Services/`, `Forms/`), so the layering is still recognisable.
- Repositories live in Core, so the UI project can technically reach them. That the UI only
  talks to services is a convention, not enforced by the compiler. A separate data-access
  project would enforce it, but is not worth the extra project at this scale.
- `.slnx` requires a current toolchain (Visual Studio 2022 17.13+ / .NET SDK 9.0.200+);
  both are given with the .NET 10 requirement.
