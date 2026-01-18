# Ufas1Forms Project Overview

Ufas1Forms is an ASP.NET Core Razor Pages web application built with .NET 10.0. It implements user authentication and authorization using ASP.NET Core Identity with Entity Framework Core and SQLite as the database provider.

## Project Architecture

- **Framework**: ASP.NET Core 10.0 with Razor Pages
- **Authentication**: ASP.NET Core Identity with confirmed account requirement
- **Database**: SQLite with Entity Framework Core
- **UI**: Bootstrap-based responsive design
- **Structure**: Standard ASP.NET Core project with Pages, Areas (for Identity), Data, and wwwroot directories

## Key Features

- User registration and authentication system
- Role-based authorization (implied by Identity integration)
- SQLite database for storing user accounts and related data
- Error handling and logging configuration
- HTTPS redirection and security headers

## Building and Running

### Prerequisites
- .NET 10.0 SDK or later
- A code editor (Visual Studio, Visual Studio Code, or JetBrains Rider)

### Setup Instructions
1. Clone or navigate to the project directory
2. Restore dependencies: `dotnet restore`
3. Apply database migrations: `dotnet ef database update`
4. Run the application: `dotnet run`

### Development Commands
- **Build**: `dotnet build`
- **Run**: `dotnet run`
- **Test**: `dotnet test` (if tests exist)
- **Publish**: `dotnet publish -c Release`
- **Database Migrations**: 
  - Add migration: `dotnet ef migrations add MigrationName`
  - Update database: `dotnet ef database update`

## Project Structure

```
Ufas1Forms/
├── Areas/
│   └── Identity/           # ASP.NET Core Identity pages
├── Data/
│   ├── ApplicationDbContext.cs  # EF Core DbContext
│   └── Migrations/         # Database migration files
├── Pages/                  # Razor Pages views and models
├── Properties/             # Application properties
├── wwwroot/                # Static assets (CSS, JS, images)
├── app.db                  # SQLite database file
├── appsettings.json        # Configuration settings
├── Program.cs              # Application startup configuration
└── Ufas1Forms.csproj       # Project file with dependencies
```

## Configuration

The application uses the following configuration:
- Connection string points to a local SQLite file (`app.db`)
- Identity is configured to require confirmed accounts
- Logging level set to Information for default and Warning for ASP.NET Core components
- HSTS enabled for production environments

## Development Conventions

- Follow ASP.NET Core and C# coding conventions
- Use async/await for I/O-bound operations
- Leverage Entity Framework Core for data access
- Implement proper error handling and validation
- Use Tag Helpers for HTML generation in Razor Pages
- Follow security best practices for web applications