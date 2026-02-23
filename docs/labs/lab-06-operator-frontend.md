# Lab 06 — Operator Workflow (frontend)

## План
Лаба 6 — Operator Workflow UI+API: очереди, assign/status/reject, условные кнопки
   Выход: операторский workflow с корректными 400/403/409

## Как использовать в Cursor
Composer → вставить промпт ниже, добавить @.cursorrules и релевантные файлы из client/src/.


## Prompt (Composer)
```text
Implement Operator flow in @client:

Queue pages (all use useQuery):
- /queue/new: tickets where status=New
- /queue/assigned: tickets where assignedToMe=true
- /queue/resolved: tickets where status=Resolved, read-only (no actions)
Each queue: filters + AntD Table + click row → /tickets/:id

Ticket Details — Operator actions (useMutation + invalidate query):
Show buttons ONLY when allowed per @.cursorrules rules:
- Assign-to-me: Operator + status∈{New,InProgress,WaitingForStudent} + (assigneeId==null OR ==me)
- Change status (Segmented/Dropdown): Operator + assigneeId==me; show only valid next statuses
- Reject (Button → Modal with required reason textarea): Operator + status active + not assigned to another
- Internal toggle: show ONLY if Operator && assigneeId == currentOperatorId
If action not allowed → hide or disabled + Tooltip explanation

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```
