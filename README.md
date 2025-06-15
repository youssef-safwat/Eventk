# Eventk - ASP.NET Core Application

A modern web application built with ASP.NET Core, featuring JWT authentication, Entity Framework Core with SQL Server, and interactive API documentation with Swagger.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Repository Structure](#repository-structure)
- [Building from Source Code](#option-2-building-from-source-code)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [Troubleshooting](#troubleshooting)
- [Additional Information](#additional-information)

## Prerequisites

### System Requirements

- **Operating System**: Windows 10/11 or macOS 10.15+ (Catalina or later)
- **Memory**: Minimum 4 GB RAM (8 GB recommended)
- **Storage**: At least 500 MB free disk space

### Software Dependencies

#### Source Code Compilation
- **.NET SDK 8.0 or later**
  - Windows: Download from [Microsoft .NET Downloads](https://dotnet.microsoft.com/download/dotnet/8.0)
  - macOS: Download from [Microsoft .NET Downloads](https://dotnet.microsoft.com/download/dotnet/8.0) or install via Homebrew: `brew install dotnet`
- **SQL Server** (for database functionality)

#### Package Dependencies (Automatically Restored)
The application uses the following NuGet packages:
- **Mapster (7.4.0)** - Object mapping library
- **Microsoft.AspNetCore.Authentication.JwtBearer (9.0.4)** - JWT authentication
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore (9.0.4)** - Identity management with EF Core
- **Microsoft.AspNetCore.Mvc.NewtonsoftJson (9.0.4)** - JSON serialization with Newtonsoft.Json
- **Microsoft.EntityFrameworkCore.SqlServer (9.0.4)** - Entity Framework Core SQL Server provider
- **Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite (9.0.4)** - Spatial data support
- **Microsoft.EntityFrameworkCore.Tools (9.0.4)** - EF Core tools for migrations
- **Refit (8.0.0)** - REST API client library
- **Refit.HttpClientFactory (8.0.0)** - Refit integration with HttpClientFactory
- **Swashbuckle.AspNetCore.SwaggerGen (8.1.1)** - Swagger documentation generator
- **Swashbuckle.AspNetCore.SwaggerUI (8.1.1)** - Swagger UI for API documentation

## Repository Structure

```
/
├── src/                    # Source code files
│   ├── Entities/          # Entity models and data structures
│   ├── Eventk/            # Main application project
│   │   ├── Controllers/   # API Controllers
│   │   ├── Helpers/       # Utility and helper classes
│   │   ├── Views/         # Razor views (if applicable)
│   │   ├── wwwroot/       # Static files (CSS, JS, images)
│   │   ├── appsettings.json # Application configuration
│   │   └── Program.cs     # Application entry point
│   ├── ServiceContracts/  # Service interface definitions
│   └── Services/          # Service implementations
├── exe/                   # Pre-built executables
│   ├── win-x64/          # Windows 64-bit executable
│   └── osx-x64/          # macOS 64-bit executable
└── README.md             # This file
```

## Setup Options

You can run this application in two ways: using pre-built executables or building from source code.

## Option 2: Building from Source Code

### Windows Build Process

1. **Install .NET SDK**:
   - Download and install from [Microsoft .NET Downloads](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Verify installation: `dotnet --version`

2. **Clone/Download the repository**:
   ```cmd
   git clone [your-repository-url]
   cd [repository-name]
   ```

3. **Navigate to source directory**:
   ```cmd
   cd src
   ```

4. **Restore dependencies**:
   ```cmd
   dotnet restore
   ```

5. **Build the application**:
   ```cmd
   # Debug build
   dotnet build
   
   # Release build (recommended for production)
   dotnet build --configuration Release
   ```

6. **Run the application**:
   ```cmd
   # Development mode
   dotnet run
   
   # Or run the built executable
   dotnet run --configuration Release
   ```

### macOS Build Process

1. **Install .NET SDK**:
   ```bash
   # Via Homebrew (recommended)
   brew install dotnet
   
   # Or download from Microsoft and follow installer instructions
   ```

2. **Verify installation**:
   ```bash
   dotnet --version
   ```

3. **Clone/Download the repository**:
   ```bash
   git clone [your-repository-url]
   cd [repository-name]
   ```

4. **Navigate to source directory**:
   ```bash
   cd src/Eventk
   ```

5. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

6. **Build the application**:
   ```bash
   # Debug build
   dotnet build
   
   # Release build (recommended for production)
   dotnet build --configuration Release
   ```

7. **Run the application**:
   ```bash
   # Development mode
   dotnet run
   
   # Or run the built executable
   dotnet run --configuration Release
   ```

## Configuration

### Application Settings

1. **Database Configuration**:
   - Edit `src/Eventk/appsettings.json`
   - Update the SQL Server connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=your-server-name; Database=your-database-name; User Id=your-username; Password=your-password; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;"
     }
   }
   ```

2. **Complete Configuration Template**:
   - Copy this template to your `src/Eventk/appsettings.json` and fill in your values:
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*",
     "ConnectionStrings": {
       "DefaultConnection": "Server=your-server-name; Database=your-database-name; User Id=your-username; Password=your-password; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;"
     },
     "EmailConfiguration": {
       "From": "your-email@domain.com",
       "SmtpServer": "smtp.gmail.com",
       "Port": "587",
       "Password": "your-app-password"
     },
     "JWT": {
       "ValidAudience": "http://localhost:5296/",
       "ValidIssuer": "http://localhost:5296/",
       "Secret": "your-jwt-secret-key-minimum-32-characters-long"
     },
     "Cloudinary": {
       "CloudName": "your-cloudinary-cloud-name",
       "ApiKey": "your-cloudinary-api-key",
       "ApiSecret": "your-cloudinary-api-secret"
     },
     "Paymob": {
       "PublicKey": "your-paymob-public-key",
       "SecretKey": "your-paymob-secret-key",
       "HmacSecret": "your-paymob-hmac-secret"
     },
     "GoogleMaps": {
       "ApiKey": "your-google-maps-api-key"
     }
   }
   ```

3. **Configuration Steps**:

   **Database Setup**:
   - Replace `your-server-name` with your SQL Server instance
   - Replace `your-database-name` with your database name
   - Replace `your-username` and `your-password` with your credentials

   **Email Configuration** (for notifications):
   - Replace `your-email@domain.com` with your sender email
   - Replace `your-app-password` with your Gmail App Password
   - Note: For Gmail, you need to enable 2FA and generate an App Password

   **JWT Authentication**:
   - Replace `your-jwt-secret-key-minimum-32-characters-long` with a secure secret key
   - Update `ValidAudience` and `ValidIssuer` URLs if deploying to different domains
   
   **Cloudinary** (for image/file uploads):
   - Replace placeholders with your Cloudinary account details
   - Sign up at [Cloudinary](https://cloudinary.com/) to get credentials

   **Paymob** (for payment processing):
   - Replace placeholders with your Paymob account credentials
   - Sign up at [Paymob](https://paymob.com/) to get API keys

   **Google Maps** (for location services):
   - Replace `your-google-maps-api-key` with your Google Maps API key
   - Get this from [Google Cloud Console](https://console.cloud.google.com/)

3. **Database Migration** (First-time setup):
   ```bash
   # Navigate to main project directory
   cd src/Eventk
   
   # Create and apply migrations
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

2. **Environment Variables**:
   - Create `src/appsettings.Development.json` for development settings
   - Set `ASPNETCORE_ENVIRONMENT` environment variable:
     - Windows: `set ASPNETCORE_ENVIRONMENT=Development`
     - macOS/Linux: `export ASPNETCORE_ENVIRONMENT=Development`

3. **Port Configuration**:
   - Default ports: HTTP (5000), HTTPS (5001)
   - To change ports, modify `src/Properties/launchSettings.json`

## Running the Application

### Development Mode

```bash
# Navigate to main project directory
cd src/Eventk

# Run in development mode
dotnet run
```

### Production Mode

```bash
# Build for production
dotnet build --configuration Release

# Run production build
dotnet run --configuration Release
```

### Accessing the Application

Once running, open your web browser and navigate to:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger API Documentation** (Local): `http://localhost:5000/swagger/index.html` or `https://localhost:5001/swagger/index.html`

### Accessing the Hosted API

If you want to access the live hosted API without running locally:
- **Dashboard**: `http://eventk.runasp.net/`
- **Hosted API Documentation**: `http://eventk.runasp.net/swagger/index.html`

The application will display startup information in the console, including the exact URLs.

## Troubleshooting

### Common Issues

#### 1. ".NET SDK not found" Error
**Problem**: `dotnet` command not recognized
**Solution**:
- **Windows**: Ensure .NET SDK is installed and added to PATH
- **macOS**: Try `brew install dotnet` or download from Microsoft

#### 2. Port Already in Use
**Problem**: "Address already in use" error
**Solution**:
- Change ports in `src/Properties/launchSettings.json`
- Or kill processes using the ports:
  - **Windows**: `netstat -ano | findstr :5000` then `taskkill /PID [PID] /F`
  - **macOS**: `lsof -ti:5000 | xargs kill -9`

#### 3. SSL Certificate Issues
**Problem**: HTTPS certificate warnings
**Solution**:
```bash
# Trust the development certificate
dotnet dev-certs https --trust
```

#### 4. Database Connection Issues
**Problem**: Cannot connect to SQL Server database
**Solution**:
- Verify connection string in `appsettings.json`
- Ensure SQL Server is running:
  - **Windows**: Check SQL Server services in Services.msc
  - **macOS**: Ensure Docker container is running (if using SQL Server in Docker)
- Run database migrations: `dotnet ef database update`
- Check firewall settings for SQL Server port (default 1433)

#### 5. Entity Framework Migration Issues
**Problem**: Database schema errors
**Solution**:
```bash
# Check migration status
dotnet ef migrations list

# Create new migration
dotnet ef migrations add [MigrationName]

# Apply migrations
dotnet ef database update

# Reset database (caution: this will delete all data)
dotnet ef database drop
dotnet ef database update
```

#### 6. JWT Authentication Issues
**Problem**: Authentication errors or invalid tokens
**Solution**:
- Verify JWT configuration in `appsettings.json`
- Ensure JWT secret key is properly configured
- Check token expiration settings
- Verify HTTPS is enabled for secure token transmission

#### 5. Permission Denied (macOS)
**Problem**: Cannot execute the application
**Solution**:
```bash
# Make executable runnable
chmod +x Eventk

# If still issues, check macOS security settings
# Go to System Preferences > Security & Privacy > General
```

#### 6. Swagger UI Not Loading
**Problem**: Cannot access Swagger documentation
**Solution**:
- Ensure you're running in Development environment
- Check that Swagger is enabled in `Program.cs`
- Try accessing directly: `http://localhost:5000/swagger/index.html`
- For hosted API, use: `http://eventk.runasp.net/swagger/index.html`

### Debugging Steps

1. **Check .NET Installation**:
   ```bash
   dotnet --info
   ```

2. **Verify Project Structure**:
   ```bash
   # Ensure you're in the correct directory
   ls -la  # macOS/Linux
   dir     # Windows
   ```

3. **Check Application Logs**:
   - Console output during startup
   - Log files (if configured)

4. **Port Conflicts**:
   ```bash
   # Check what's running on ports 5000/5001
   # Windows
   netstat -ano | findstr :5000
   
   # macOS
   lsof -i :5000
   ```

### Getting Help

If you encounter issues not covered here:

1. Check the console output for detailed error messages
2. Verify all prerequisites are installed correctly
3. Ensure you're using compatible versions of .NET
4. Check that no antivirus software is blocking the application

## Additional Information

### Development Tools (Optional)

For enhanced development experience, consider installing:

- **Visual Studio Code** with C# extension
- **Visual Studio Community** (Windows)
- **JetBrains Rider** (cross-platform)

### Building Self-Contained Executables

To create portable executables that don't require .NET Runtime:

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish/win-x64

# macOS
dotnet publish -c Release -r osx-x64 --self-contained true -o ./publish/osx-x64
```

### API Documentation

The application includes comprehensive Swagger/OpenAPI documentation:

#### Local Development
- **Swagger UI**: `http://localhost:5000/swagger/index.html` or `https://localhost:5001/swagger/index.html`
- **Features**: Interactive API testing, request/response examples
- **Authentication**: JWT Bearer token support in Swagger UI

#### Hosted API
- **Live API Documentation**: `http://eventk.runasp.net/swagger/index.html`
- **Base API URL**: `http://eventk.runasp.net`
- **Use Case**: Test API endpoints without local setup

### Project Architecture

The solution follows a clean architecture pattern with separate projects:
- **Entities**: Data models and entity definitions
- **ServiceContracts**: Interface definitions for services
- **Services**: Business logic implementations
- **Eventk**: Main web API project with controllers and configuration

### Database Features

- **Entity Framework Core** with SQL Server
- **Identity Management** for user authentication and authorization  
- **Spatial Data Support** via NetTopologySuite for geographic data
- **Code-First Migrations** for database schema management

---
