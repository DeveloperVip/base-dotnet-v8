# Growth Planning Backend

Base backend for Growth Planning System built with ASP.NET Core 9.

## Included

- JWT authentication
- User login and SSO login endpoints
- Permission lookup and role claims
- Swagger/OpenAPI setup
- SQL Server EF Core setup
- Dockerfile and compose service

## Run

```bash
dotnet restore growth-planning-be.sln
dotnet run --project growth-planning-be/growth-planning-be.csproj
```

## Build

```bash
dotnet build growth-planning-be.sln
```
