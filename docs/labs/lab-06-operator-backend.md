# Lab 06 — Operator Workflow (backend)

## План
Лаба 6 — Operator Workflow UI+API: очереди, assign/status/reject, условные кнопки
   Выход: операторский workflow с корректными 400/403/409

## Как использовать в Cursor
Composer → вставить промпт ниже, добавить @.cursorrules и релевантные папки из server/.


## Prompt (Composer)
```text
Add to @ServiceDesk.API operator endpoints:

POST /api/tickets/{id}/assign-to-me
  Checks in order (see @.cursorrules assign-to-me section):
  1. status ∈ {New,InProgress,WaitingForStudent} else 400
  2. anti-steal → 409
  3. set assigneeId, if New→InProgress

POST /api/tickets/{id}/status { status }
  - assigneeId != currentOperator → 403
  - same status → 400
  - invalid workflow transition → 400
  - allowed transitions per @.cursorrules workflow

POST /api/tickets/{id}/reject { reason }
  Checks in order (see @.cursorrules reject section):
  1. status check → 400
  2. anti-steal → 409
  3. auto-assign if New+unassigned
  4. set Rejected + rejectedReason
```
