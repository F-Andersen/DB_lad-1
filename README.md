# BD_LAB_TEST_PROJECT

REST API for managing travel plans and locations built with ASP.NET Core 9.0 and PostgreSQL.

## Technologies

- ASP.NET Core 9.0
- Entity Framework Core 9.0
- PostgreSQL 16
- Docker & Docker Compose
- Swagger/OpenAPI
- K6 (Performance Testing)

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

tests/
└── performance-tests/    # K6 performance tests
    ├── config/
    │   └── endpoints.js
    ├── utils/
    │   ├── api-client.js
    │   ├── data-generator.js
    │   └── response-utils.js
    ├── smoke-test.js
    ├── load-test.js
    ├── stress-test.js
    ├── spike-test.js
    └── endurance-test.js
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

## Performance Testing (K6)

The project includes K6 performance tests located in `tests/performance-tests/`.

### Test Types

| Test | Description | Command |
|------|-------------|---------|
| Smoke Test | Basic functionality check with 1 VU | `k6 run smoke-test.js` |
| Load Test | Normal load simulation (50 VUs) | `k6 run load-test.js` |
| Stress Test | Gradually increasing load (up to 300 VUs) | `k6 run stress-test.js` |
| Spike Test | Sudden traffic spike (500 VUs) | `k6 run spike-test.js` |
| Endurance Test | Extended duration test (40 min) | `k6 run endurance-test.js` |

### Running Tests

1. Install K6: https://k6.io/docs/get-started/installation/
2. Start the API with Docker:
```bash
docker-compose up -d
```
3. Run tests:
```bash
cd tests/performance-tests
k6 run smoke-test.js
```

### Custom API URL

```bash
k6 run -e API_URL=http://localhost:6001 smoke-test.js
```

## License

This project is for educational purposes.
