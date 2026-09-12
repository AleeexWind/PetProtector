# PetProtector

PetProtector helps pet owners recover lost animals using a physical QR tag (QR-адресник) attached to the pet’s collar. An owner registers, activates a tag, and fills in a short questionnaire with the pet’s name, the owner’s name, and a phone number. Anyone who finds the pet can scan the tag, open the public questionnaire, contact the owner, and (when the scanner allows it) share coordinates so the owner can see where the pet was found.

The product is a React (Vite) client and an ASP.NET Core Web API with JWT auth, email confirmation, and optional Yandex/VK login. Domain concepts are collars, questionnaires, and scan locations; data lives in SQL Server with Redis for caching.
