# Lab 02 — Frontend каркас

## План
Лаба 2 — Frontend каркас: Vite+TS+AntD, ESLint/Prettier, TanStack Query, роуты, Layout, guards, UI-состояния
   Выход: все маршруты работают, приватные защищены, ESLint/Prettier настроены, TanStack Query подключён

## Как использовать в Cursor
Composer → вставить промпт ниже, добавить @.cursorrules.


## Prompt (Composer)
```text
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Create a React frontend project scaffold in client/ with:
- Vite + React 18 + TypeScript
- Ant Design 5
- React Router v6 (all routes as stubs)
- TanStack Query v5 (QueryClientProvider in main.tsx)
- ESLint with typescript-eslint + react plugin
- Prettier with eslint-config-prettier integration
- Scripts in package.json: "lint", "format", "build"

Routes:
Public (no layout): /login, /register
Private (with Layout):
  Student: /tickets, /tickets/new, /tickets/:id
  Operator: /queue/new, /queue/assigned, /queue/resolved, /tickets/:id
  Admin: /admin/categories, /admin/users

Components to create:
- AppLayout: Header (left: "Service Desk", right: displayName + role + Logout), Sider (role-based menu)
- RequireAuth: if no token in localStorage → redirect /login
- RequireRole(roles[]): if role not in list → show 403 page
- PageLoading, PageEmpty, PageError, PageNotFound (use Ant Design Spin / Result / Alert)
- AuthContext (stub): { user, role, isAuthenticated, login, logout, init } — все заглушки

All page components are stubs returning <div>PageName</div>.
See @.cursorrules for full conventions.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```
