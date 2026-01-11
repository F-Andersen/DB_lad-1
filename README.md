# BD_LAB_TEST_PROJECT

REST API for managing travel plans and locations built with ASP.NET Core 9.0 and PostgreSQL.

## Technologies

- ASP.NET Core 9.0
- Entity Framework Core 9.0
- PostgreSQL 16
- Docker & Docker Compose
- Swagger/OpenAPI

## Project Structure

```
BD_LAB_TEST_PROJECT/
├── Controllers/          # API controllers
│   ├── LocationsController.cs
│   └── TravelPlansController.cs
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── EntityConfigurations/   # EF Core configurations
│   └── Repositories/           # Data access layer
├── DTOs/                 # Data transfer objects
├── Interfaces/           # Repository interfaces
├── Models/               # Domain models
│   ├── Location.cs
│   └── TravelPlan.cs
├── Migrations/           # EF Core migrations
├── Exceptions/           # Custom exception handlers
└── Extensions/           # Service extensions
```

## API Endpoints

### Travel Plans

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/TravelPlans | Get all travel plans (paginated) |
| GET | /api/TravelPlans/{id} | Get travel plan by ID |
| POST | /api/TravelPlans | Create new travel plan |
| PUT | /api/TravelPlans/{id} | Update travel plan |
| DELETE | /api/TravelPlans/{id} | Delete travel plan |

### Locations

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/Locations/{planId}/locations | Add location to travel plan |
| PUT | /api/Locations/{id} | Update location |
| DELETE | /api/Locations/{id} | Delete location |

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Docker & Docker Compose
- PostgreSQL (if running without Docker)

### Running with Docker

```bash
docker-compose up -d
```

The API will be available at:
- HTTP: http://localhost:6001
- HTTPS: https://localhost:6061

### Running Locally

1. Update connection string in `appsettings.json`
2. Apply migrations:
```bash
dotnet ef database update
```
3. Run the application:
```bash
dotnet run --project BD_LAB_TEST_PROJECT
```

## Configuration

### Database Connection

Configure PostgreSQL connection in `appsettings.json` or via environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=TravelerDb;Username=postgres;Password=postgres"
  }
}
```

### Docker Environment Variables

- `POSTGRES_USER` - Database user (default: postgres)
- `POSTGRES_PASSWORD` - Database password (default: postgres)
- `POSTGRES_DB` - Database name (default: TravelerDb)

## License

This project is for educational purposes.
