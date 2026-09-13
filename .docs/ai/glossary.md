# Domain glossary

Entities and events live in `Domain.Core`. Domain events implement MediatR `INotification` (`BaseEvent`) and are published on `SaveChanges` via `DomainEventDispatcher`.

## Entities

**Collar** — Physical QR tag (QR-адресник). Has a `SecretKey` used to activate it, `State` (`Unlinked` / `Linked`), and optional `UserId` once claimed. One-to-one with a `Questionnaire` (same `Id`); one-to-many with scan `Location`s.

**Questionnaire** — Public “QR passport” shown when the tag is scanned. Holds `OwnersName`, `PetsName`, `PhoneNumber`, and `State` (`WaitingFilling` → `Filling` → `Filled`). `LinkQuestionnaire` is the unique public id in `/quest/:id`. Created empty with the collar; the owner fills it after linking.

**Location** — One scan of a tag: `Latitude`, `Longitude`, `CreatedAt`, `CollarId`. Recorded when a finder shares coordinates.

**AppUser** — Identity user (`IdentityUser`): `FullName`, `CreatedAt`, optional `Avatar`. Owns refresh tokens. Collar `UserId` points here after linking. Does not raise domain events today.

**AppRefreshToken** — JWT refresh token for an `AppUser` (expiry, create/revoke timestamps and IPs). Auth plumbing, not a pet-domain concept.

## Events

**CollarCreatedEvent** (`CollarId`) — Raised when a collar (and empty questionnaire) is created. Handler is currently empty.

**CollarUpdatedEvent** (`CollarId`) — Raised when an owner links a collar (`Unlinked` → `Linked`). Handler moves the questionnaire from `WaitingFilling` to `Filling`.

**LocationCreatedEvent** (`CollarId`) — Raised when a scan location is stored. Handler emails the collar owner that the pet was found. (Constructor parameter is named `questionnaireId` but the value stored is `CollarId`.)

**QuestionnaireUpdatedEvent** — Raised when the owner saves questionnaire fields and state becomes `Filled`. No handler yet (cache invalidation is a TODO).
