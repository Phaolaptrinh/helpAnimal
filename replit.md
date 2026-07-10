# Petlor – Pet Shop & Clinic App

An ASP.NET Core 8 MVC web application for a Vietnamese pet shop and clinic ("Bán Hàng" = Sales). Features include product listings, cart/checkout, pet management, appointment booking, and an AI vet chat powered by Groq.

## Stack

- **Framework:** ASP.NET Core 8 MVC (C#)
- **ORM:** Entity Framework Core 8 with SQL Server
- **Auth:** ASP.NET Core Identity
- **AI:** Groq API (llama-3.3-70b-versatile) for the VetAi chat feature
- **Session:** Server-side sessions for cart

## How to run

The workflow `Start application` runs the app:

```
cd 'Ban hang' && dotnet run --no-launch-profile --urls http://0.0.0.0:5000
```

The app listens on port 5000. Replit handles HTTPS at the proxy level.

## Environment / Secrets

| Key | Where | Purpose |
|---|---|---|
| `GroqSettings:ApiKey` | Secret `GROQ_API_KEY` (read via appsettings / env) | AI vet chat |
| `ConnectionStrings:DefaultConnection` | `appsettings.json` | SQL Server connection |

## Database

Uses an external SQL Server hosted on somee.com. Connection string is in `appsettings.json`. EF Core migrations are in `Ban hang/Migrations/`. The DB is seeded on startup via `DbSeeder.SeedAsync`.

## Project structure

```
Ban hang/
  Controllers/   – MVC controllers (Home, Product, Cart, Checkout, Account, Admin, Pet, VetAi)
  Models/        – Entity and view models
  Views/         – Razor views
  Data/          – AppDbContext, DbSeeder, GroqChatService
  Migrations/    – EF Core migrations
  wwwroot/       – Static assets
```

## User preferences

- Keep existing project structure and stack
- HTTPS redirection disabled in app code (handled by Replit proxy)
