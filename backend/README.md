# Application Urls  
## Http  
OpenAPI URL: http://localhost:5141/openapi/v1.json  
Scalar URL: http://localhost:5141/scalar  
## Https  
OpenAPI URL: https://localhost:7035/openapi/v1.json  
Scalar URL: https://localhost:7035/scalar  

## Persistence

The application now uses PostgreSQL via Entity Framework Core.

### Local development

By default, the connection string `ConnectionStrings:AppDb` is configured in `appsettings.Development.json` as:

`Host=localhost;Port=5432;Database=language_proficiency_blockchain_dev;Username=postgres;Password=postgres`

Override it via environment variable if needed:

`ConnectionStrings__AppDb="Host=localhost;Port=5432;Database=language_proficiency_blockchain_dev;Username=...;Password=..."`

### Docker / Docker Compose

`compose.yaml` defines a `db` service using the `postgres:16` image and wires the app service `language-proficiency-blockchain` to it using the connection string:

`Host=db;Port=5432;Database=language_proficiency_blockchain;Username=postgres;Password=postgres`

To start the stack:

```bash
docker compose up --build
```
