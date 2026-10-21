# Portfolio API

## Requirements

- .NET SDK 10.0 (the project targets `net10.0`)
- SQL Server, SQL Server Express, LocalDB, or a hosted SQL Server instance
- SMTP credentials for notification delivery

## Restore and run

From `backend/Portfolio.Api`:

```powershell
dotnet restore
dotnet ef database update
dotnet run
```

The development profile uses `https://localhost:7149`. Swagger is available at `/swagger` in Development and health is available at `/health`.

## Local SQL Server

`appsettings.Development.json` contains a password-free LocalDB example. Replace it through user secrets or environment variables if you use SQL Server Express or another local instance. Do not put production connection strings in source-controlled JSON files.

The API can start without SMTP configured so that `/health` remains available. Contact messages are still stored if notification configuration is missing; the notification failure is logged. Configure the email variables below before production use.

## Environment variables

Set these for production (and override any local values as needed):

```text
ConnectionStrings__DefaultConnection=Server=...;Database=PortfolioDb;User Id=...;Password=...;Encrypt=True;TrustServerCertificate=False
EmailSettings__Host=smtp.example.com
EmailSettings__Port=587
EmailSettings__Username=your-smtp-username
EmailSettings__Password=your-smtp-password
EmailSettings__FromEmail=no-reply@your-domain.com
EmailSettings__ToEmail=portfolio-owner@your-domain.com
EmailSettings__UseSsl=true
AllowedOrigins__0=https://shahid-portfolio-web-application.vercel.app
```

`AllowedOrigins__1` can be used for another trusted frontend origin. Production values must be supplied by the deployment host's secret/environment-variable configuration.

For local SMTP configuration without committing secrets, use user secrets:

```powershell
dotnet user-secrets init
dotnet user-secrets set "EmailSettings:Password" "your-password"
```

## EF Core migrations

Create a migration after model changes, then apply it:

```powershell
dotnet ef migrations add DescriptiveMigrationName
dotnet ef database update
```

The initial migration is included. The API intentionally uses migrations rather than `EnsureCreated()`.

## Contact API

`POST /api/contact`

```json
{
  "name": "Ali Khan",
  "email": "ali@example.com",
  "subject": "Job Opportunity",
  "message": "I would like to discuss an opportunity with you."
}
```

Successful requests return `{ "success": true, "message": "Your message has been sent successfully." }`. The API persists the message first; an SMTP failure is logged without exposing technical details to the visitor. Contact submissions are limited to five per minute per API instance.

## Frontend connection

In `Portfolio Web Application`, create `.env.local` (which is ignored) with:

```text
NEXT_PUBLIC_API_BASE_URL=https://localhost:7149
```

Set `NEXT_PUBLIC_API_BASE_URL` in Vercel to the deployed API URL. The existing contact form posts to `${NEXT_PUBLIC_API_BASE_URL}/api/contact`; no backend or frontend hostname is hardcoded into production code.
