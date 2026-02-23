# Lab 05 — Tickets (Student) (frontend)

## План
Лаба 5 — Tickets UI+API (Student): модели, миграции, GET/POST, TicketResponse, seed-данные
   Выход: Student create → list → details на реальных данных

## Как использовать в Cursor
Composer → вставить промпт ниже, добавить @.cursorrules и релевантные файлы из client/src/.


## Prompt (Composer)
```text
Implement Student flow in @client:
- /tickets: AntD Table with filters (Status Select + Priority Select minimum)
  + columns: Title, Category, Priority Tag, Status Tag, CreatedAt
  + click row → /tickets/:id
  + useQuery, show PageLoading/PageEmpty/PageError states
- /tickets/new: AntD Form (Title, Description TextArea, Priority Select, Category Select)
  + useMutation, on success → /tickets
  + show validation errors under fields
- /tickets/:id: ticket details (title, category, priority tag, status tag, author, assignee, createdAt/updatedAt)
  + no operator actions yet
  + useQuery, show PageNotFound if 404

Status Tags: New=blue, InProgress=processing, WaitingForStudent=orange, Resolved=green, Closed=default, Rejected=red
Priority Tags: Low=default, Medium=orange, High=red

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```
