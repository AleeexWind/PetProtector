# Architecture

PetProtector is a Clean Architecture / CQRS web app: a React SPA talks to a .NET 9 API. Docker Compose runs nginx (SPA + `/api` proxy), the API, SQL Server, and Redis.

```
ClientApp  →  nginx /api  →  WebApi  →  Application (MediatR)  →  Domain.Core
                                      ↘ Infrastructure → SQL Server, Redis, SMTP
```

## Backend

| Module | Role |
| --- | --- |
| **Domain.Core** | Entities (`Collar`, `Questionnaire`, `Location`, `AppUser`), enums, domain events. No infrastructure. |
| **Application** | CQRS: MediatR commands/queries, FluentValidation, AutoMapper, ports (`IAppDbContext`, `IRedisCache`, `IEmailSender`, `IJwtTokenManager`). Features: Authentication, Users, OAuth, Collars, Questionnaires, Locations. |
| **Infrastructure** | EF Core (`AppDbContext` for domain, `AuthDbContext` for Identity), JWT, Redis, MailKit SMTP, Yandex OAuth. SaveChanges dispatches domain events. |
| **WebApi** | Thin `api/[controller]` endpoints: Accounts, Users, Collars, Questionnaries, Locations. JWT (`UserIdPolicy`), IP rate limits, Swagger in Development. |

Flow: controller → MediatR → validation/exception behaviors → handler → EF/Redis/email.

## Frontend (`ClientApp`)

Vite + React 18 + MUI. Routes: Home, Login/Register (email, VK ID, Yandex), Profile (JWT, private), public Questionnaire (`/quest/:id`) for QR scans. Axios clients with cookie credentials and token refresh (`AuthProvider`). Yandex Maps for scan locations.

## Runtime

GitHub Actions on `main` build and deploy frontend and backend. Target: `https://petprotector.ru`.
