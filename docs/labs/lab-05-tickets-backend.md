# Lab 05 — Tickets (Student) (backend)

## План
Лаба 5 — Tickets UI+API (Student): модели, миграции, GET/POST, TicketResponse, seed-данные
   Выход: Student create → list → details на реальных данных

## Как использовать в Cursor
Composer → вставить промпт ниже, добавить @.cursorrules и релевантные папки из server/.


## Prompt (Composer)
```text
Add to @ServiceDesk.API:

Ticket model: { Id, Title, Description, Priority(Low/Medium/High), Status, AuthorId, AssigneeId?,
  RejectedReason?, CreatedAt, UpdatedAt, CategoryId } + migration

Endpoints:
  POST /api/tickets { title, description, priority, categoryId }
    — Student only (Operator/Admin → 403)
    — validation: title required, priority valid enum, categoryId must be active category

  GET /api/tickets
    — Student: own tickets only (server enforced)
    — Operator/Admin: all tickets
    — filters: status, priority, categoryId, search (title contains), assignedToMe
    — pagination (page + pageSize)

  GET /api/tickets/{id}
    — Student: own only (else 404)
    — Operator/Admin: any

DTO TicketResponse: see @.cursorrules (nested category, author, assignee objects)
displayName from ApplicationUser.DisplayName (set in Lab 3)

Add seed data (DbInitializer):
  - 1 Admin, 2 Operators, 3 Students
  - 5 Categories (mix active/inactive)
  - 15+ Tickets covering all statuses, priorities, some with assignee

Important: For Student, server enforces own tickets only and MUST ignore assignedToMe query param.
```
