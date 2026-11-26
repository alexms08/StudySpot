# StudySpot 2.0

**StudySpot** is an ASP.NET Core MVC (.NET 8) site for discovering, reviewing and suggesting study-friendly locations.

## Features
- Search by name/address/city/category and amenities
- Rating and price sorting, minimum rating filter and pagination
- Registration/login via ASP.NET Core Identity
- Favorites and one review per user/place, with edit and deletion
- User-submitted place suggestions with admin moderation
- Verified place indicators; places are not labeled verified automatically
- Accessible, responsive interface and OpenStreetMap embeds
- PostgreSQL for Render, SQLite for local development
- Health endpoint `/health`, rate limiting for suggestions and review POSTs
- Docker build and Render deployment infrastructure

## Local development

Install .NET 8 SDK then run:

```bash
dotnet restore
dotnet run
```

Local runs use SQLite (`studyspot.db`). For production you must provide a private `DATABASE_URL` (either PostgreSQL URI or Npgsql connection string). The app refuses to start in production without one.

## Render deployment

Use the `studyspot-v2` branch, Docker runtime and Frankfurt region for the service and PostgreSQL. `DATABASE_URL` should be the **internal** Render database URL. Do not commit credentials. Set `ASPNETCORE_ENVIRONMENT=Production`. The app listens on port 10000 by default (see Dockerfile). Free PostgreSQL expires after 30 days: back up or upgrade before expiry.

To create the *first* administrator, temporarily set both `ADMIN_BOOTSTRAP_EMAIL` and `ADMIN_BOOTSTRAP_PASSWORD` as private service environment variables. A **new** admin is created at first start; an existing account is never promoted automatically. Choose a long unique password, then **remove these variables** after success. Do not set these values in Git or chat.

## Database lifecycle

This version uses Entity Framework Core `EnsureCreatedAsync()` to initialize a **fresh** database without fake data. **This is an initial deployment mechanism, not a migrations strategy.** For production changes to an existing database, create and review EF Core migrations, test them on a backup, then switch to `MigrateAsync()`. Do not mix `EnsureCreated` with migrations against the same database.

No fabricated places are inserted: add verified entries through Admin, or approve user suggestions and then verify details. An unverified entry is clearly marked as such.

## Security / remaining production requirements

Authentication uses Identity with lockout and password complexity. Forms use antiforgery protection. Login and suggestion endpoints are rate-limited. For a full public launch add email verification/password recovery delivery, incident monitoring, privacy policy and data-retention controls, managed database backup and migration strategy. Perform live deployment and end-to-end testing before accepting real users.

## Source structure

- `Models/` + `Data/`: EF entities and database
- `Controllers/`: MVC actions, moderation and access rules
- `ViewModels/`: validated user input
- `Views/`: Razor templates
- `wwwroot/css/`: responsive styling
- `Dockerfile`: container definition
- `.github/workflows/`: build checks
