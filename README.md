# BoardGameApp

BoardGameApp is a local ASP.NET Core MVC application for tracking board game sessions and results. It is designed for personal or small-group use, without authentication, and stores data in SQL Server.

The application lets you manage:

- Players, including Brazilian WhatsApp formatting and soft deactivation.
- Games, including genre, publisher, author, max players, and times played.
- Matches, including selected game, participating players, scores, winner calculation, and creation date.
- Authors, with CRUD screens and delete protection when an author is already used by a game.
- Supporting data such as genres and publishers seeded through EF Core migrations.

## Tech Stack

- .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server / LocalDB
- xUnit
- Bootstrap, JavaScript, and server-rendered Razor views

## Solution Structure

```text
BoardGameApp.sln
src/
  BoardGameApp.Domain/          Domain entities
  BoardGameApp.Application/     DTOs, services, and repository contracts
  BoardGameApp.Infrastructure/  EF Core DbContext, repositories, and migrations
  BoardGameApp.Web/             ASP.NET Core MVC web app
tests/
  BoardGameApp.Tests/           Unit and integration-style tests
Documentation/
  docs/                         Project specs and SDD rules
  tasks/                        Task backlog and acceptance criteria
  commands/                     Agent command documentation
```

## Requirements

Install these before running the project:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server LocalDB, usually included with Visual Studio on Windows
- EF Core CLI tool, if it is not already installed

To install or update the EF Core CLI tool:

```powershell
dotnet tool install --global dotnet-ef
```

If `dotnet-ef` is already installed:

```powershell
dotnet tool update --global dotnet-ef
```

## Installation

1. Clone the repository.

```powershell
git clone <repository-url>
cd Projeto1
```

2. Restore dependencies.

```powershell
dotnet restore BoardGameApp.sln
```

3. Check that the solution builds.

```powershell
dotnet build BoardGameApp.sln
```

4. Create or update the local database.

```powershell
dotnet ef database update --project src\BoardGameApp.Infrastructure\BoardGameApp.Infrastructure.csproj --startup-project src\BoardGameApp.Web\BoardGameApp.Web.csproj
```

The default connection string uses SQL Server LocalDB:

```text
Server=(localdb)\mssqllocaldb;Database=BoardGameApp;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

To use another SQL Server instance, update `src/BoardGameApp.Web/appsettings.json`.

5. Run the web application.

```powershell
dotnet run --project src\BoardGameApp.Web\BoardGameApp.Web.csproj
```

By default, the app is configured to run at:

- `https://localhost:51631`
- `http://localhost:51632`

Open one of those URLs in your browser.

## Running Tests

Run the full test suite with:

```powershell
dotnet test BoardGameApp.sln
```

## Development Notes

This project follows Spec-Driven Development. Before implementing new behavior, check:

- `Documentation/docs/spec.md`
- `Documentation/docs/agent.md`
- `Documentation/tasks/task.md`
- `Documentation/docs/testing.md`

For UI work, use the visual references in `Documentation/visual-reference` as the layout and interaction baseline.
