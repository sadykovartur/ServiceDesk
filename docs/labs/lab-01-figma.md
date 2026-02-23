# Lab 01 — Figma (экраны, состояния, flow, Rules)

## План
Лаба 1 — Figma: экраны, состояния, flow, роли/действия
   Выход: Figma-файл + страница Rules

## Как использовать в Cursor
Используйте Figma Make. Cursor не нужен.


## Prompt (Composer)
```text
Запрос 1/2:

Ты — Figma Make. Сгенерируй визуальный концепт (wireframe + базовый UI-стиль) для веб-приложения Service Desk на базе Ant Design и подготовь набор компонентов (UI Kit).

A) Общие требования
Стиль: чистый B2B SaaS, light theme.
Компоненты AntD: Layout, Menu, Table, Form, Button, Tag/Badge, Alert, Result, Spin, Modal, Tooltip, Dropdown/Segmented.
Добавь Header контент: слева “Service Desk”, справа user (displayName + роль) + Logout.

B) UI Kit (отдельная страница/секция)
Сделай компоненты/варианты:

1) Status Tags (6) с цветами:
- New (blue)
- InProgress (processing/blue)
- WaitingForStudent (orange/warning)
- Resolved (green)
- Closed (default/grey)
- Rejected (red/error)

2) Priority Tags (3) с цветами:
- Low (default/grey)
- Medium (orange/warning)
- High (red/error)

3) Page states как мини-фреймы (унифицированные):
- Loading (Spin)
- Empty
- Error (Alert/Result)
- Validation errors (пример: поля формы с ошибками + сообщение сверху)
- NotFound (Result 404)

4) Table row / list item (тикет в таблице)
5) Modal template (общий)
6) Form field patterns (Input/Password/Select/TextArea)

C) Экраны (фреймы)
Сделай фреймы:
- Login (/login) — Email, Password, Submit, link на Register
- Register (/register) — DisplayName, Email, Password, Confirm Password, Submit, link на Login
- Admin: Categories (/admin/categories) — таблица + Create/Edit modal + Active switch
- Admin: Users/Roles (/admin/users) — таблица + смена роли (Select+Save) + note “роль вступает в силу после следующего логина”

Для каждого экрана выше добавь рядом мини-фреймы UI-состояний:
[ScreenName]/Loading, /Empty, /Error, /Validation, /NotFound.

D) Навигация (Layout)
Показать общий Layout (Header+Sider+Content).
Для Login/Register — отдельный Public layout без сайдбара.
Показать 3 варианта Sider Menu (отдельными мини-фреймами или вариантами компонента):
- Student menu: “My Tickets” (/tickets), “Create Ticket” (/tickets/new)
- Operator menu: “Queue New” (/queue/new), “Queue Assigned” (/queue/assigned), “Queue Resolved” (/queue/resolved)
- Admin menu: “Categories” (/admin/categories), “Users” (/admin/users)

E) Rules page (ОБЯЗАТЕЛЬНО, 1 страница)
Сделай отдельную страницу/фрейм “Rules”.
Формат: таблица-матрица «Роль × Статус → какие действия доступны».
Строки (Роли): Student, Operator, Admin.
Колонки (Статусы): New, InProgress, WaitingForStudent, Resolved, Closed, Rejected.
Действия в ячейках (иконки/текст): View details, Comment, Comment internal, Assign-to-me, Change status, Reject, Close.
Добавь Legend:
- ✅ Allowed
- ⛔ Forbidden
- ⚠️ Conditional (с пометкой условия, например “только если assigneeId == currentOperatorId”)

F) Выход
Один Figma файл/страница с UI Kit + перечисленные фреймы + отдельная страница/фрейм Rules.
Prototype connections:
- Register → Login
- (опционально) Login → Admin: Categories (для быстрого просмотра админки)


---

Запрос 2/2:

Ты — Figma Make. На основе предыдущего концепта (Ant Design стиль + UI Kit) сгенерируй основные рабочие экраны Tickets/Queues и Ticket Details для приложения Service Desk и сделай кликабельный прототип.

A) Роли и маршруты (используй как подписи фреймов)
Student:
- Tickets List (/tickets)
- Create Ticket (/tickets/new)
- Ticket Details (/tickets/[id])

Operator:
- Queue New (/queue/new)
- Queue Assigned (/queue/assigned)
- Queue Resolved (/queue/resolved) — read-only
- Ticket Details (/tickets/[id])

B) Экраны (фреймы) — минимум 7
- Student: Tickets List (/tickets)
- Student: Create Ticket (/tickets/new)
- Operator: Queue New (/queue/new)
- Operator: Queue Assigned (/queue/assigned)
- Operator: Queue Resolved (/queue/resolved, read-only)
- Ticket Details (/tickets/[id])
- Ticket Details — WaitingForStudent → InProgress (variant after auto-transition)

C) Содержимое экранов

C1) List/Queue screens
Заголовок + описание.
Фильтры (минимум 2): Status / Priority / Category / Search (любой набор, но минимум 2).
Таблица: Title, Category, Priority (Tag), Status (Tag), CreatedAt, Assignee (если актуально).

Для каждого list/queue экрана добавь мини-фреймы UI-состояний рядом:
[ScreenName]/Loading, /Empty, /Error, /Validation, /NotFound.

Queue Resolved (важно): показать, что это “read-only мониторинг” — на экране НЕТ действий Operator (нет assign/status/reject). Можно добавить небольшую подсказку/Alert “Read-only”.

C2) Create Ticket (Student)
Form: Title, Description (TextArea), Priority (Select), Category (Select).
Submit/Cancel.
Validation errors: пример под полями + общее сообщение сверху.

C3) Ticket Details (обязательный)
Верх:
- Title, Category, Priority Tag, Status Tag, CreatedAt/UpdatedAt
- Author (displayName), Assignee (если есть)
- Если Rejected — Alert с rejectedReason

Действия:
Operator:
- Assign-to-me
- Change status (Dropdown/Segmented)
- Reject (Modal с обязательным reason)
Student:
- Close

Если действие недоступно — скрыть или disabled + Tooltip (в зависимости от варианта/сценария).

Комментарии:
- список комментариев (обычный vs internal: серый фон + “Internal” Tag)
- важно: Student не видит internal (можно добавить подсказку/комментарий в макете)

Форма комментария:
- скрыть/disabled при Closed/Rejected (всем)
- скрыть/disabled при Resolved (только Student; у Student в Resolved единственное действие — Close)
- Operator: internal toggle показывать СТРОГО только если assigneeId == currentOperatorId
  (если оператор не назначен на тикет — toggle скрыть или disabled + Tooltip “Только для назначенного оператора”)

Sticky note на Ticket Details (правила видимости/доступности):
- Assign-to-me: Operator + статус ∈ {New, InProgress, WaitingForStudent} + (assigneeId==null или ==я)
- Change status: Operator + assigneeId==я + переход строго по workflow
- Reject: Operator + статус ∈ {New, InProgress, WaitingForStudent} + тикет не назначен другому оператору
- Close: Student + status==Resolved + я автор
- Comment form: скрыть Closed/Rejected всем; + скрыть Resolved только Student
- Internal toggle: только если assigneeId==currentOperatorId
- Auto-transition: WaitingForStudent + Student submit comment → статус становится InProgress и переход на фрейм “Ticket Details — WaitingForStudent → InProgress”

Фрейм “Ticket Details — WaitingForStudent → InProgress” (обязательно визуально отличается):
- Status Tag изменён с WaitingForStudent (orange) на InProgress (blue)
- В список комментариев добавлен новый комментарий от Student
- Форма комментария снова активна (не disabled)

D) Prototype connections (обязательно)
- Tickets List → Ticket Details (/tickets/[id]) (клик по строке)
- Create Ticket → Tickets List (после Submit)
- Queue New/Assigned/Resolved → Ticket Details (/tickets/[id]) (клик по строке)
- Ticket Details (Student, статус WaitingForStudent) submit comment → перейти на “Ticket Details — WaitingForStudent → InProgress”
- (опционально) меню: переходы между очередями Operator

E) Выход
Фреймы готовы к использованию как кликабельный прототип.
Сохрани единый стиль и компоненты из UI Kit.
```
