# Lab 03 — Auth UI+API (frontend)

## План
Лаба 3 — Auth UI+API: register/login, AuthContext, JWT, Swagger, Global Exception Middleware, GET /health
   Выход: login/register end-to-end, Swagger с Bearer и кодами 400/401/403/404/409, GET /health → 200, соглашение по displayName зафиксировано

## Как использовать в Cursor
Composer → вставить промпт ниже, добавить @.cursorrules и релевантные файлы из client/src/.


## Prompt (Composer)
```text
Implement auth in @client using @.cursorrules conventions:
- /login page: AntD Form, fields: Email + Password, Submit, link to /register
- /register page: AntD Form, fields: DisplayName + Email + Password + Confirm Password, Submit, link to /login
- Complete AuthContext:
  init(): if token in localStorage → GET /api/auth/me → set user/role/isAuthenticated
  login(email, password): POST /api/auth/login → save token → set state
  logout(): clear localStorage → reset state
- Hook up RequireAuth and RequireRole to real AuthContext
- After login: Student → /tickets, Operator → /queue/new, Admin → /admin/categories

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```
