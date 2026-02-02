# TeamFlow

TeamFlow is an internal team visibility and operations app built with ASP.NET Core MVC.
It supports team management, employee status updates, internal messaging, and role-based dashboards.

## Project Tracking

- 📋 [Feature Checklist](./FEATURES_CHECKLIST.md)

## Tech Stack

- .NET 8
- ASP.NET Core MVC + Razor Pages (Identity)
- Entity Framework Core 8
- SQLite (local development)

## Current Features

- Authentication with ASP.NET Core Identity
- Role seeding (`Admin`, `Employee`)
- Optional admin bootstrap via environment variables
- Team CRUD (admin only)
- User management (assign team, activate/deactivate)
- Status updates with history
- Role-aware dashboard (admin vs employee visibility)
- Internal messaging (compose, inbox, sent)
- Admin settings page

## Prerequisites

- .NET SDK 8.0+

## Quick Start (Local)

1. Clone the repository.
2. From the solution root, run:

```bash
dotnet restore
dotnet build
dotnet run --project TeamFlow.Web
```

3. Open the app at:
   - `https://localhost:7140`
   - or `http://localhost:5166`

## Admin Seeding (Optional)

If you want an admin user automatically created on startup, set:

- `TF_ADMIN_EMAIL`
- `TF_ADMIN_PASSWORD`

Example:

```bash
export TF_ADMIN_EMAIL="admin@teamflow.local"
export TF_ADMIN_PASSWORD="ChangeMe123!"
dotnet run --project TeamFlow.Web
```

If these variables are not set, admin auto-seeding is skipped.

## Database Notes

- The app uses SQLite by default with:
  - `ConnectionStrings:ApplicationDbContextConnection = Data Source=teamflow.db`
- The local database file is created in `TeamFlow.Web/teamflow.db`.
- EF Core migrations are already included in `TeamFlow.Web/Migrations`.

## Useful Commands

```bash
# Apply migrations
dotnet ef database update --project TeamFlow.Web

# Add a new migration
dotnet ef migrations add <MigrationName> --project TeamFlow.Web
```

## Solution Structure

- `TeamFlow.sln` - Solution file
- `TeamFlow.Web/` - Web application project
- `TeamFlow.Web/Controllers/` - MVC controllers
- `TeamFlow.Web/Models/` - Domain + view models
- `TeamFlow.Web/Data/` - DbContext, seeders, settings service
- `TeamFlow.Web/Views/` - Razor views
- `TeamFlow.Web/Migrations/` - EF Core migrations

## Status

In active development. Production deployment tasks (Azure MySQL/App Service) are tracked in `FEATURES_CHECKLIST.md`.
