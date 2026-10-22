# Portfolio API

## Requirements

- .NET SDK 10.0 (the project targets `net10.0`)
- PostgreSQL (including Neon)
- A Resend API key and a verified Resend sending domain/address for notification delivery

## Restore and run

From `backend/Portfolio.Api`:

```powershell
dotnet restore
dotnet ef database update
dotnet run
```

The development profile uses `https://localhost:7149`. Swagger is available at `/swagger` in Development and health is available at `/health`.

## Database and email delivery

Configure PostgreSQL through user secrets or environment variables. Do not put production connection strings or email secrets in source-controlled JSON files.

Email notifications use Resend's HTTPS API; SMTP, MailKit, and MimeKit are not used. In production, the required Resend configuration is validated at startup. In development, contact messages are still stored if notification configuration is missing and the notification failure is logged.

## Environment variables

Set these for production (and override any local values as needed):

```text
ConnectionStrings__DefaultConnection=Host=...;Database=...;Username=...;Password=...;Ssl Mode=Require
Resend__ApiKey=re_...
Resend__FromEmail=no-reply@your-verified-domain.com
Resend__ToEmail=portfolio-owner@your-domain.com
AllowedOrigins__0=https://shahid-portfolio-web-application.vercel.app
```

`AllowedOrigins__1` can be used for another trusted frontend origin. Production values must be supplied by the deployment host's secret/environment-variable configuration.

For local Resend configuration without committing secrets, use user secrets:

```powershell
dotnet user-secrets init
dotnet user-secrets set "Resend:ApiKey" "re_your_api_key"
dotnet user-secrets set "Resend:FromEmail" "no-reply@your-verified-domain.com"
dotnet user-secrets set "Resend:ToEmail" "portfolio-owner@your-domain.com"
```

To send a local test after setting `ConnectionStrings:DefaultConnection` and the Resend values, start the API in one terminal:

```powershell
dotnet run
```

Then run this in a second terminal:

```powershell
Invoke-RestMethod -Method Post -Uri "https://localhost:7149/api/contact" -ContentType "application/json" -Body '{"name":"Local Test","email":"local.test@example.com","subject":"Resend test","message":"This is a local Resend notification test message."}'
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

Successful requests return `{ "success": true, "message": "Your message has been sent successfully." }`. The API persists the message first; a Resend notification failure is logged without exposing technical details to the visitor. Contact submissions are limited to five per minute per API instance.

## Frontend connection

In `Portfolio Web Application`, create `.env.local` (which is ignored) with:

```text
NEXT_PUBLIC_API_BASE_URL=https://localhost:7149
```

Set `NEXT_PUBLIC_API_BASE_URL` in Vercel to the deployed API URL. The existing contact form posts to `${NEXT_PUBLIC_API_BASE_URL}/api/contact`; no backend or frontend hostname is hardcoded into production code.
