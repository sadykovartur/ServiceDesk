# Lab 07 — Comments & Close (frontend)

## План
Лаба 7 — Comments UI+API: internal, anti-necropost, auto InProgress, Close
   Выход: ветка WaitingForStudent + закрытие тикета

## Как использовать в Cursor
Composer → вставить промпт ниже, добавить @.cursorrules и релевантные файлы из client/src/.


## Prompt (Composer)
```text
Complete Ticket Details in @client per @.cursorrules:

Comments section (useQuery):
  - List of comment cards: author displayName, createdAt, text
  - Internal comments: grey background + "Internal" Tag; hidden for Student

Comment form (useMutation):
  - Hidden/disabled if Closed or Rejected (everyone)
  - Hidden/disabled if Resolved (Student only)
  - Internal toggle (Operator only): show ONLY if assigneeId == currentOperatorId

Close button (useMutation):
  - Show only if Student + status==Resolved + currentUser==author
  - On success → invalidate ticket query

Rejected status: show Alert with rejectedReason

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```
