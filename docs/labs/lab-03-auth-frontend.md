# Lab 03 — Auth UI+API (frontend)

## План
Лаба 3 — Auth UI+API: register/login, AuthContext, JWT, Swagger, Global Exception Middleware, GET /health
   Выход: login/register end-to-end, Swagger с Bearer и кодами 400/401/403/404/409, GET /health → 200, соглашение по displayName зафиксировано

## Как использовать в Cursor
Composer → вставить промпт ниже, добавить @.cursorrules и релевантные файлы из client/src/.


## Prompt (Composer)
```text
Implement real auth in @client using @.cursorrules conventions. Replace any demo-login.

1) API client
- Create a small API helper (fetch or axios) with base URL (Vite env or relative /api)
- Attach Authorization: Bearer <token> when token exists
- Handle 401 by clearing token and redirecting to /login when appropriate

2) Pages
- /login: AntD Form (Email, Password). Submit calls auth.login(email,password). Link to /register.
- /register: AntD Form (DisplayName, Email, Password, Confirm Password). Submit calls /api/auth/register and stores token. Link to /login.
- Show errors:
  - 400 validation → PageValidation (or Form.Item errors + Alert)
  - network/500 → PageError

3) AuthContext
- token is source of truth (already in Lab 2): token state synced with localStorage
- init(): if token exists → GET /api/auth/me → set user/role; if fails 401 → logout
- login(email,password): POST /api/auth/login → save token → await init()
- register(displayName,email,password): POST /api/auth/register → save token → await init()
- logout(): clear token from state + localStorage + reset user/role

4) Guards + redirect
- RequireAuth uses isAuthenticated (or token) and redirects to /login
- After successful login/register:
  Student → /tickets
  Operator → /queue/new
  Admin → /admin/categories

Keep existing routes/layout; do not implement tickets/admin pages yet beyond stubs.
At the end ensure: npm run build passes (0 TS errors).

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```
