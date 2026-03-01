# Lab 03 — Auth UI+API (backend)

## План
Лаба 3 — Auth UI+API: register/login, AuthContext, JWT, Swagger, Global Exception Middleware, GET /health
   Выход: login/register end-to-end, Swagger с Bearer и кодами 400/401/403/404/409, GET /health → 200, соглашение по displayName зафиксировано

## Как использовать в Cursor
Composer → вставить промпт ниже, добавить @.cursorrules и релевантные папки из server/.


## Prompt (Composer)
```text
Create ASP.NET Core 8 Web API project in server/ServiceDesk.API with:
- ASP.NET Core Identity + EF Core 8 + PostgreSQL (Npgsql), connection string via env
- ApplicationUser extends IdentityUser; add field string DisplayName
- JWT access token (no refresh); include userId and role claims (claim type "role")
- Ensure roles exist on app start (dev): Student, Operator, Admin
- On register: create user + assign default role Student

Endpoints:
POST /api/auth/register body: { email, password, displayName } → 200 { accessToken } or 400 (ProblemDetails)
POST /api/auth/login    body: { email, password } → 200 { accessToken } or 400/401 (ProblemDetails)
GET  /api/auth/me       (Bearer) → 200 { id, email, displayName, role } or 401
GET  /health            → 200 "Healthy"

Error handling:
- Global exception middleware: return ProblemDetails for unhandled exceptions
- Use ProblemDetails for validation/business errors as well

Swagger:
- Add Bearer auth
- Document relevant codes on auth endpoints: 200/400/401 (+ ProblemDetails on errors)

EF Core:
- Create initial migration (Identity tables + DisplayName)

See @.cursorrules for ProblemDetails and JWT conventions.
```
