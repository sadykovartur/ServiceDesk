# Lab 03 — Auth UI+API (backend)

## План
Лаба 3 — Auth UI+API: register/login, AuthContext, JWT, Swagger, Global Exception Middleware, GET /health
   Выход: login/register end-to-end, Swagger с Bearer и кодами 400/401/403/404/409, GET /health → 200, соглашение по displayName зафиксировано

## Как использовать в Cursor
Composer → вставить промпт ниже, добавить @.cursorrules и релевантные папки из server/.


## Prompt (Composer)
```text
Create ASP.NET Core 8 Web API project in server/ServiceDesk.API with:
- ASP.NET Core Identity + EF Core 8, PostgreSQL (Npgsql), connection string via env
- ApplicationUser extends IdentityUser, add custom field: string DisplayName
- JWT: access token, role in claim "role", no refresh token
- Endpoints:
  POST /api/auth/register  body: { email, password, displayName } → 200 { accessToken } or 400
  POST /api/auth/login     body: { email, password } → 200 { accessToken } or 400/401
  GET  /api/auth/me        → 200 { id, email, displayName, role } or 401
  GET  /health             → 200 "Healthy"
- Global Exception Middleware → ProblemDetails for all unhandled exceptions
- Swagger with Bearer authorization
  Auth: document 400/401/403
  Tickets/Comments/Operator ops: document 400/401/403/404/409
  ProblemDetails on all error codes
- EF Core migration: initial (Identity tables + DisplayName field)

See @.cursorrules for ProblemDetails and JWT conventions.
```
