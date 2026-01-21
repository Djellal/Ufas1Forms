# Ufas1Forms - Dynamic Form Builder

An ASP.NET web application for creating dynamic forms for registration, surveys, and data collection.

## Features

- **Multiple Form Types**: Registration, Survey, and Data Collection forms
- **Dynamic Fields**: 14 field types including text, email, select, radio, checkbox, date, file upload
- **Form Builder**: Visual interface to create and edit forms
- **Response Collection**: Store and view all form submissions
- **CSV Export**: Export responses to CSV for analysis
- **Bootstrap 5 UI**: Modern, responsive design

## Project Overview

University forms management application built with ASP.NET Core Razor Pages, targeting .NET 10.0. Uses ASP.NET Core Identity for authentication with role-based authorization.

## Tech Stack

- **Framework**: ASP.NET Core 10.0 (Razor Pages)
- **Database**: SQLite with Entity Framework Core
- **Authentication**: ASP.NET Core Identity with roles (admin, facadmin, student)
- **Frontend**: Razor Pages with Bootstrap (via wwwroot/lib)

## Commands

```bash
# Build
dotnet build

# Run (development)
dotnet run

# Run with watch (hot reload)
dotnet watch run

# Restore packages
dotnet restore

# Add EF Core migration
dotnet ef migrations add <MigrationName>

# Apply migrations
dotnet ef database update

# Clean build artifacts
dotnet clean
```

## Project Structure

```
Ufas1Forms/
├── Areas/
│   ├── Admin/Pages/Forms/  # Admin form management
│   │   ├── Index           # List all forms
│   │   ├── Create          # Create new form
│   │   ├── Edit            # Edit form metadata
│   │   ├── Build           # Visual field builder
│   │   ├── Delete          # Delete confirmation
│   │   ├── Submissions     # View form submissions
│   │   ├── SubmissionDetails # View single submission
│   │   └── ExportCsv       # CSV export handler
│   └── Identity/           # ASP.NET Identity UI
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Migrations/
├── Helpers/
│   ├── SelectOption.cs     # Option model for select/radio/checkbox
│   └── OptionsJsonParser.cs # Parse field options JSON
├── Models/
│   ├── Form.cs             # Form entity with Title, Type, Status, Slug
│   ├── FormField.cs        # Dynamic field with 14 types
│   ├── FormSubmission.cs   # User submission record
│   ├── FormAnswer.cs       # Answer for each field
│   ├── UploadedFile.cs     # File upload metadata
│   ├── Domaine.cs          # Academic domain
│   ├── Etablissement.cs    # Institution
│   └── Faculte.cs          # Faculty
├── Pages/
│   ├── Forms/              # Public form pages
│   │   ├── Index           # List published forms
│   │   ├── Fill            # Render and submit form
│   │   └── Thanks          # Submission confirmation
│   └── Shared/             # Layouts and partials
├── Services/
│   └── DbSeeder.cs         # Seeds roles and admin user
├── wwwroot/                # Static assets
├── Program.cs              # Application entry point
└── appsettings.json        # Configuration
```

## Domain Model

### Dynamic Forms
- **Form**: Id, Title, Description, Type (Registration/Survey/DataCollection), Status (Draft/Published/Archived), Slug, CreatedByUserId
- **FormField**: Id, FormId, Name, Label, FieldType (14 types), IsRequired, Order, Placeholder, DefaultValue, OptionsJson, ValidationJson
- **FormSubmission**: Id, FormId, SubmittedAt, SubmittedByUserId, IpAddress, UserAgent, Status
- **FormAnswer**: Id, SubmissionId, FieldId, ValueText, ValueJson
- **UploadedFile**: Id, SubmissionId, FieldId, OriginalFileName, StoredFileName, ContentType, SizeBytes

### Academic Structure
- **Etablissement**: Institution with Id, Nom, Adresse
- **Faculte**: Faculty with Id, Nom
- **Domaine**: Academic domain with Id, Nom, FaculteId (FK to Faculte)

## Field Types

Text, Email, Number, Textarea, Select, Radio, Checkbox, Date, Time, Phone, Url, Password, Hidden, File

## Roles

- `admin` - System administrator
- `facadmin` - Faculty administrator
- `student` - Student user

## Coding Conventions

- Use file-scoped namespaces (`namespace X;`)
- Use primary constructors for classes where appropriate
- Use nullable reference types (enabled in project)
- Use Data Annotations for model validation (`[Required]`, `[MaxLength]`)
- Use `[ForeignKey]` attribute for navigation properties
- Initialize string properties with `string.Empty` to avoid null warnings

## Database

- SQLite database stored as `app.db` in project root
- Connection string in `appsettings.json`
- Migrations located in `Data/Migrations/`

## Security Notes

- Never commit sensitive credentials to source control
- Admin credentials are seeded via DbSeeder (review before production)
- Use User Secrets for local development: `dotnet user-secrets set "Key" "Value"`
