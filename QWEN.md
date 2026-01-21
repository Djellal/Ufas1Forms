# Ufas1Forms - Dynamic Form Builder Application

## Project Overview

Ufas1Forms is an ASP.NET Core web application designed as a dynamic form builder system for educational institutions. It allows users to create, manage, and submit various types of forms including registration forms, surveys, and data collection forms. The application features role-based access control with three user roles: admin, facadmin (faculty administrator), and student.

### Key Features
- **Multiple Form Types**: Support for Registration, Survey, and Data Collection forms
- **14 Field Types**: Including text, email, number, textarea, select, radio, checkbox, date, time, phone, URL, password, hidden, and file upload fields
- **Visual Form Builder**: Easy-to-use interface to create and edit forms without coding
- **Response Collection**: Store and view all form submissions in one place
- **CSV Export**: Export responses to CSV for analysis in Excel or other tools
- **Responsive Design**: Bootstrap 5 UI that works on desktop and mobile devices
- **Role-Based Access Control**: Different permissions for admin, facadmin, and student users
- **Cascading Dropdowns**: Support for dependent dropdown fields
- **File Upload Support**: Ability to upload files as part of form submissions

### Architecture
- **Backend**: ASP.NET Core 10.0 with Entity Framework Core
- **Frontend**: Razor Pages with Bootstrap 5
- **Database**: SQLite (with option to switch to other providers)
- **Authentication**: ASP.NET Core Identity with role management
- **ORM**: Entity Framework Core with SQLite provider

### Data Model
The application includes several key entities:
- **Form**: Represents a form with title, description, type, status, and fields
- **FormField**: Individual fields within a form with type, validation, and options
- **FormSubmission**: Records of form submissions with associated answers
- **FormAnswer**: Individual answers to form fields in a submission
- **UploadedFile**: Files uploaded as part of form submissions
- **Etablissement/Faculte/Domaine**: Educational institution hierarchy

## Building and Running

### Prerequisites
- .NET 10.0 SDK or later
- SQLite (for the default database configuration)

### Setup Instructions
1. Clone the repository
2. Navigate to the project directory
3. Run the following commands:

```bash
dotnet restore
dotnet build
dotnet run
```

### Database Setup
The application automatically creates and seeds the database on first run:
- Creates necessary tables using Entity Framework migrations
- Seeds default roles: "admin", "facadmin", "student"
- Creates a default admin user with email `djellal@univ-setif.dz`
- Creates sample forms and submissions for demonstration

### Configuration
The application uses `appsettings.json` for configuration:
- Connection string for SQLite database (`DataSource=app.db;Cache=Shared`)
- Logging levels
- Allowed hosts

For development, you can use user secrets to store sensitive configuration values.

## Development Conventions

### Coding Standards
- Follow standard C# naming conventions and coding style
- Use nullable reference types (enabled in project)
- Use implicit usings (enabled in project)
- Follow ASP.NET Core best practices for dependency injection and middleware

### Project Structure
- `/Areas`: Contains admin and identity areas with specialized pages
- `/Data`: Entity Framework DbContext and migrations
- `/Helpers`: Utility classes and helper methods
- `/Models`: Data models and enums
- `/Pages`: Main Razor Pages for the application
- `/Properties`: Application properties and launch settings
- `/Services`: Service classes including database seeding
- `/wwwroot`: Static assets (CSS, JS, images)

### Security Considerations
- Authentication and authorization using ASP.NET Core Identity
- Role-based access control for different user types
- Input validation and sanitization
- Secure password storage using Identity's built-in hashing

### Testing
While no explicit test files were found in the initial scan, the application follows patterns that would support unit and integration testing. Consider adding:
- Unit tests for service layer logic
- Integration tests for database operations
- UI tests for critical user flows

## Key Components

### Database Seeding
The `DbSeeder` class handles initialization of:
- Default roles (admin, facadmin, student)
- Admin user account
- Sample users for each role
- Sample forms with various field types
- Sample form submissions for demonstration

### Form Management
The application supports complex form configurations:
- Multiple field types with validation options
- Field ordering and grouping
- Cascading dropdowns with parent-child relationships
- File upload constraints and validation

### User Roles and Permissions
- **Admin**: Full access to all forms and system administration
- **Facadmin**: Access to forms within their faculty/department
- **Student**: Can view and submit published forms

## Deployment

The application is configured as a standard ASP.NET Core web application and can be deployed to:
- Azure App Service
- AWS Elastic Beanstalk
- Self-hosted servers
- Containerized environments (Docker)

For production deployment, consider:
- Using a production-ready database (SQL Server, PostgreSQL, MySQL)
- Configuring HTTPS certificates
- Setting up proper logging and monitoring
- Implementing backup strategies for the database