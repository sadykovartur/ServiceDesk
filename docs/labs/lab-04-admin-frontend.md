# Lab 04 — Categories & Admin (frontend)

## План
Лаба 4 — Categories UI+API: Admin CRUD, Users/Roles API, ILogger
   Выход: Admin управляет категориями и ролями

## Как использовать в Cursor
Composer → вставить промпт ниже, добавить @.cursorrules и релевантные файлы из client/src/.


## Prompt (Composer)
```text
Implement admin pages in @client:
- /admin/categories: AntD Table (Name, IsActive switch, Edit button)
  + Create/Edit Modal (name field)
  + toggle active via PATCH
  + useQuery for list, useMutation for create/edit/toggle
- /admin/users: AntD Table (displayName, email, role)
  + Change role: Select in row + Save button
  + Note under table: "Роль вступает в силу после следующего логина"
  + useMutation for role change

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```
