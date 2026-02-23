# Lab 07 — Comments & Close (backend)

## План
Лаба 7 — Comments UI+API: internal, anti-necropost, auto InProgress, Close
   Выход: ветка WaitingForStudent + закрытие тикета

## Как использовать в Cursor
Composer → вставить промпт ниже, добавить @.cursorrules и релевантные папки из server/.


## Prompt (Composer)
```text
Add to @ServiceDesk.API:

Comment model: { Id, TicketId, AuthorId, Text, IsInternal, CreatedAt } + migration

GET /api/tickets/{id}/comments
  Student: own ticket only (else 404), filter out isInternal=true
  Operator/Admin: any ticket, all comments

POST /api/tickets/{id}/comments { text, isInternal? }
  Anti-necropost + role checks per @.cursorrules
  Auto-transition: if WaitingForStudent + Student comment → set InProgress

POST /api/tickets/{id}/close
  Student only (Operator/Admin → 403)
  Foreign ticket → 404
  Status != Resolved → 400
  Else → Closed

DTO CommentResponse: see @.cursorrules
```
