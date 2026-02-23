# Lab 04 — Categories & Admin (backend)

## План
Лаба 4 — Categories UI+API: Admin CRUD, Users/Roles API, ILogger
   Выход: Admin управляет категориями и ролями

## Как использовать в Cursor
Composer → вставить промпт ниже, добавить @.cursorrules и релевантные папки из server/.


## Prompt (Composer)
```text
Add to @ServiceDesk.API:

Category model: { Id, Name, IsActive } + migration
Endpoints:
  GET    /api/categories                        — authorized, isActive=true only
  GET    /api/categories?includeInactive=true   — Operator/Admin only
  POST   /api/categories          { name }      — Admin only
  PUT    /api/categories/{id}     { name }      — Admin only
  PATCH  /api/categories/{id}/active { isActive } — Admin only

Users/Roles:
  GET /api/users                                — Admin only, returns { id, displayName, email, role }
  PUT /api/users/{id}/role { role }             — Admin only, exactly one role from {Student,Operator,Admin}
  Note: role change takes effect after next login

ILogger<T>: add structured logs on role change, category enable/disable, 403/409 denials.
See @.cursorrules for conventions.
```
