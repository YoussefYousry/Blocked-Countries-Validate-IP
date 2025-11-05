# 🛰️ Blocked Countries & IP Validation API

A lightweight ASP.NET Core 8 Web API that manages blocked countries, validates IP addresses using third‑party geolocation APIs, and logs blocked access attempts — without a database (in‑memory only).

## 🚀 Overview

This project demonstrates clean architecture, in‑memory data, and external API integration with HttpClient. It allows you to:

- Block or unblock countries permanently
- Temporarily block countries for a specified duration
- Lookup the country of an IP address
- Check if the caller’s IP is from a blocked country
- View logs of blocked attempts

## 🧩 Architecture

Solution layout (simplified):

```
BlockedCountries.sln
│
├── BlockedCountries.API                     # Presentation: Controllers, Program, Swagger
├── BlockedCountries.Application             # Application services (business logic)
├── BlockedCountries.Infrastructure          # In-memory repositories, DI, HttpClient
├── BlockedCountries.Infrastructure.Logging  # Logging helpers/middleware
└── BlockedCountries.Domain                  # Core interfaces and models
```

Key choices:
- In‑memory, thread‑safe stores via ConcurrentDictionary/ConcurrentBag
- HttpClientFactory for third‑party API calls
- Background service cleans expired temporal blocks every 5 minutes
- Swagger/OpenAPI with XML documentation summaries

## 🧱 Tech Stack

| Component        | Technology                                          |
| ---------------- | --------------------------------------------------- |
| Framework        | .NET 8 (ASP.NET Core Web API)                       |
| Language         | C#                                                  |
| Data Storage     | In‑memory (ConcurrentDictionary, ConcurrentBag)     |
| Logging          | Serilog (via Infrastructure.Logging)                |
| API Docs         | Swagger / OpenAPI                                   |
| HTTP Integration | HttpClient + Factory                                |
| Background Tasks | IHostedService                                      |

## ⚙️ Setup

1) Restore dependencies
```bash
dotnet restore
```

2) Configure geolocation provider

The API is configurable via the `GeoApi` section in `BlockedCountries/BlockedCountries/appsettings.json`:

```json
"GeoApi": {
  "BaseUrl": "https://api.ipgeolocation.io/",
  "ApiKey": "<your-key-or-empty>",
  "ApiKeyQueryName": "apiKey"
}
```

Notes:
- ipapi.co (no key required): set `BaseUrl` to `https://ipapi.co/` and leave `ApiKey` empty; `ApiKeyQueryName` can be `key`.
- IPGeolocation.io (requires key): set `BaseUrl` to `https://api.ipgeolocation.io/`, provide your key, and use `apiKey` as the query name.
- You can also store the key securely using user‑secrets or environment variables (e.g., `GeoApi__ApiKey`).

3) Run the API
```bash
dotnet run --project BlockedCountries/BlockedCountries
```

Open Swagger UI at:
- https://localhost:5001/swagger

## 🧠 Features & Endpoints

Countries
- POST `/api/countries/block`
- DELETE `/api/countries/block/{countryCode}`
- GET `/api/countries/blocked?q=&page=&pageSize=`
- POST `/api/countries/temporal-block` (durationMinutes 1–1440)

IP
- GET `/api/ip/lookup?ipAddress={ip}` (if omitted, uses caller IP)
- GET `/api/ip/check-block` (checks caller IP, logs attempt)

Logs
- GET `/api/logs/blocked-attempts?page=&pageSize=`

## 📦 In‑Memory Storage

- Blocked countries: `ConcurrentDictionary<string, BlockedCountry>`
- Blocked attempts logs: `ConcurrentBag<BlockedAttempt>`
- Temporal blocks use `ExpiresAt` on `BlockedCountry`; a hosted service purges expired entries every 5 minutes

## 🧾 Responses

- All controllers return `Task<IResponseModel>`
- Success and error shapes use `ResponseModel`
- Pagination payloads use `PagainationModel<T>` from the Common layer
- Status codes use `StatusCodesEnum` (e.g., Ok, BadRequest, Conflict, NotFound)

## 🧪 Testing (Optional)

You can create a test project and reference the Application layer to test business logic.

```bash
dotnet new xunit -n BlockedCountries.Tests
dotnet add BlockedCountries.Tests reference BlockedCountries/BlockedCountries.Application/BlockedCountries.Application.csproj
```

Suggested tests:
- Add/Remove blocked countries
- Temporal blocking and expiration
- Geolocation API handling (success/error)

## 📧 Contact

Developed by **Youssef Yousry**
- Email: yousef.yosry82@gmail.com
- LinkedIn: https://www.linkedin.com/in/youssef-yousry9/
