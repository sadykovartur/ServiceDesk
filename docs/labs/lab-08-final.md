# Lab 08 — Final

## План
Лаба 8 — Финал: тесты (3), Docker, CI, README, ADR, Postman/.http, демо
   Выход: проект запускается по README, CI зелёный

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## Лаба 1 — Figma: экраны, состояния, flow, роли/действия

Цель: зафиксировать дизайн, правила видимости действий и пользовательские сценарии до кода.

Делается (Figma)

Карта маршрутов:
  Public: /login, /register
  Student: /tickets, /tickets/new, /tickets/:id
  Operator: /queue/new, /queue/assigned, /queue/resolved, /tickets/:id
  Admin: /admin/categories, /admin/users

Экранные макеты (wireframe достаточно):
  Login / Register
  Tickets List + фильтры
  Create Ticket
  Ticket Details (обязательный)
  Queue New / Assigned / Resolved (Operator)
  Admin Categories
  Admin Users

UI-состояния для каждого экрана:
  Loading (Spin), Empty, Error (Alert/Result), Validation errors, Not Found (Result 404)

Ticket Details (строго по ТЗ):
  Если Rejected → Alert с rejectedReason
  Комментарии: форма скрыта/disabled при Closed/Rejected; для Student также при Resolved
  Internal toggle у Operator: показывать только если assigneeId == currentOperatorId
  Кнопка Close (Student): только если status=Resolved и тикет принадлежит текущему Student
  Действия Operator: assign/status/reject — условные

Rules page (1 страница):
  Матрица «роль × статус → какие действия доступны» (assign/status/reject/comment/close)

Выход: Figma-файл + страница Rules.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## Лаба 2 — Frontend каркас

Цель: создать каркас UI, чтобы на каждой следующей лабе «экран → реальный API» замыкался в рамках занятия.

Делается (Frontend):

- Инициализация проекта: Vite + React + TypeScript + Ant Design
- ESLint + Prettier:
    установить и настроить ESLint (react, typescript-eslint)
    установить и настроить Prettier (интеграция через eslint-config-prettier)
    добавить скрипты lint и format в package.json
- TanStack Query:
    установить @tanstack/react-query
    обернуть приложение в QueryClientProvider
- Роутинг:
    PublicLayout / PrivateLayout
    Заглушки всех маршрутов
- Guards-заглушки:
    RequireAuth (нет token → redirect /login)
    RequireRole (роль не совпадает → 403/redirect)
- Навигация по ролям (меню/сайдбар по роли)
- Компоненты состояний: PageLoading, PageEmpty, PageError, PageNotFound

Выход: все маршруты работают, приватные защищены, ESLint/Prettier настроены, TanStack Query подключён.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## Лаба 3 — Auth UI+API

Цель: полноценный auth end-to-end, единый формат ошибок ProblemDetails, тестирование через Swagger.

Делается (Backend):
- ASP.NET Core Identity + EF Core + PostgreSQL, миграции
- ApplicationUser: добавить кастомное поле DisplayName (+ миграция)
- JWT (access token), роли в claim role
- Endpoints:
    POST /api/auth/register
    POST /api/auth/login → { accessToken }
    GET  /api/auth/me
    GET  /health → 200
- Swagger: Bearer авторизация
    Auth endpoints: 400, 401, 403
    Tickets/Comments/Operator ops: 400, 401, 403, 404, 409
    ProblemDetails на всех кодах ошибок
- Global Exception Middleware → ProblemDetails

Делается (Frontend):
- /login: AntD Form (Email, Password, Submit, ссылка на /register)
- /register: AntD Form (DisplayName, Email, Password, Confirm Password, Submit, ссылка на /login)
- AuthContext:
    init(): token в localStorage → GET /api/auth/me → set user/role/isAuthenticated
    login(), logout()
- JWT в localStorage (MVP)
- Решение по displayName фиксируется здесь и применяется везде:
    кастомное ApplicationUser.DisplayName — добавить поле и миграцию в этой лабе
    во всех DTO author.displayName / assignee.displayName — одно и то же поле

Выход: login/register end-to-end, Swagger с Bearer и кодами 400/401/403/404/409, GET /health → 200, соглашение по displayName зафиксировано.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## Лаба 4 — Categories UI+API: Admin CRUD, Users/Roles API, ILogger

Цель: закрыть админский контур (категории + роли) и ввести логирование через ILogger.

Делается (Backend):

Categories:
  Модель Category(id, name, isActive) + миграции
  GET  /api/categories                       — всем авторизованным, isActive=true
  GET  /api/categories?includeInactive=true  — Operator/Admin
  POST /api/categories                       — Admin
  PUT  /api/categories/{id}                  — Admin
  PATCH /api/categories/{id}/active          — Admin

Users/Roles:
  GET /api/users                             — Admin
  PUT /api/users/{id}/role { role }          — Admin, ровно 1 роль
  Правило: новая роль действует после следующего логина

ILogger:
  смена роли, enable/disable категории
  причины отказов (403/409/400) — кратко, без утечки данных

Делается (Frontend):
  /admin/categories: таблица + create/edit (Modal) + toggle active
  /admin/users: таблица + смена роли (Select) + подсказка «после следующего логина»

Выход: Admin управляет категориями и ролями.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## Лаба 5 — Tickets UI+API (Student)

Цель: реализовать старт бизнес-процесса Student: create → list → details на реальных данных.

Делается (Backend):
- Модель Ticket + связи с Category и Identity user (author/assignee), миграции
- POST /api/tickets — Student only (Operator/Admin → 403)
- GET  /api/tickets — фильтры min 2 (status/priority/search/categoryId/assignedToMe) + пагинация
- GET  /api/tickets/{id} — Student только свой (иначе 404)
- Безопасность: Student принудительно получает только свои тикеты; чужой тикет → 404
- DTO TicketResponse (вложенные объекты):
    category: { id, name, isActive }
    author: { id, displayName }  — поле из Лабы 3
    assignee?: { id, displayName }

Seed-данные:
  1 Admin, 2 Operator, 3 Student
  5 Categories (mix active/inactive)
  15+ Tickets во всех статусах, часть с assignee

Делается (Frontend):
  /tickets: список + фильтры (Status, Priority) — useQuery + PageLoading/PageEmpty/PageError
  /tickets/new: форма + валидация — useMutation, после submit → /tickets
  /tickets/:id: детали тикета (без операторских действий пока) — useQuery + PageNotFound при 404

Status Tags: New=blue, InProgress=processing, WaitingForStudent=orange, Resolved=green, Closed=default, Rejected=red
Priority Tags: Low=default, Medium=orange, High=red

Выход: Student create → list → details на реальных данных + готовые seed.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## Лаба 6 — Operator Workflow UI+API

Цель: реализовать operator workflow с правильными кодами 400/403/409 и видимостью действий в UI.

Делается (Backend):

POST /api/tickets/{id}/assign-to-me
  Порядок проверок:
  1. status ∈ {New, InProgress, WaitingForStudent} иначе 400
  2. anti-steal: assigneeId != null && != currentOperator → 409
  3. assigneeId = currentOperatorId
  4. если был New → InProgress

POST /api/tickets/{id}/status { status }
  - assigneeId != currentOperator → 403
  - same-status → 400
  - невалидный переход по workflow → 400

POST /api/tickets/{id}/reject { reason }
  1. status ∈ {New,InProgress,WaitingForStudent} иначе 400
  2. anti-steal → 409
  3. если New && assigneeId==null → auto-assign
  4. Rejected + rejectedReason

GET /api/tickets для Operator: assignedToMe, status и др. фильтры

Делается (Frontend):
  /queue/new    (status=New)
  /queue/assigned (assignedToMe=true)
  /queue/resolved (status=Resolved, read-only)
  — все через useQuery

Ticket Details — Operator actions (useMutation + инвалидация):
  Assign-to-me: Operator + status∈{New,InProgress,WaitingForStudent} + (assigneeId==null OR ==me)
  Change status (Segmented/Dropdown): Operator + assigneeId==me
  Reject (Button → Modal с обязательным reason): Operator + статус active + не назначен на другого
  Internal toggle: показывать ТОЛЬКО если assigneeId == currentOperatorId
  Если действие не разрешено → скрыть или disabled + Tooltip

Выход: операторский workflow работает, ошибки 400/403/409 корректны.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## Лаба 7 — Comments UI+API

Цель: закрыть Ticket Details: комментарии, internal, auto-transition, Close.

Делается (Backend):

GET /api/tickets/{id}/comments
  Student: только свой тикет (иначе 404); только isInternal=false
  Operator/Admin: любой тикет, все комментарии

POST /api/tickets/{id}/comments { text, isInternal? }
  Anti-necropost:
    Closed/Rejected → 400 для всех
    Resolved → Student не может (400); только /close
  Student: только свой тикет (иначе 404); isInternal=false принудительно
  Operator — порядок проверок:
    1. Closed/Rejected → 400
    2. assigneeId != currentOperator → 403
    3. создать (isInternal разрешён)
  Admin → 403
  Auto: WaitingForStudent + Student comment → InProgress

DTO CommentResponse:
  { id, ticketId, text, isInternal, createdAt, author: { id, displayName } }

POST /api/tickets/{id}/close
  Student only; Operator/Admin → 403
  Чужой тикет → 404
  Статус != Resolved → 400
  Иначе → Closed

Делается (Frontend):
  Список комментариев: author, createdAt, text — useQuery
    Internal: серый фон + "Internal" Tag; скрыть для Student
  Форма комментария (useMutation):
    Скрыть/disabled при Closed/Rejected (всем)
    Скрыть/disabled при Resolved (только Student)
    Internal toggle: показывать ТОЛЬКО если assigneeId == currentOperatorId
  Кнопка Close (useMutation):
    Только Student-владелец + status=Resolved
  Rejected: Alert с rejectedReason

Выход: ветка WaitingForStudent реализована + закрытие тикета.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## Лаба 8 — Финал

Цель: сделать проект проверяемым «одной кнопкой» и готовым к сдаче.

Делается (Backend):

Тесты (3 обязательных):
  1. Student не может GET чужой тикет → 404
  2. Operator assign-to-me: New→InProgress + anti-steal 409
  3. POST /api/tickets с пустым title → 400 ProblemDetails

Инженерка:
  docker-compose.yml: db (postgres:16) + api
  docker-compose.full.yml (опц.): db + api + client
  README:
    команды запуска
    env-переменные
    миграции/seed
    как прогонять тесты и клиент
  CI (.github/workflows/ci.yml):
    server: dotnet build + dotnet test
    client: npm ci + npm run lint + npm run build

Артефакты сдачи:
  ADR.md:
    почему JWT, а не cookie
    почему isActive (soft delete) вместо удаления категорий
    почему отдельный /reject, а не общий /status
    решение по displayName (кастомное поле vs UserName)
  Postman коллекция или .http файлы:
    сценарии: auth / tickets / operator / comments / close

Демо (как в ТЗ):
  Student: register → login → create → list → details → comment → (Operator resolves) → Close
  Operator: queue new → assign → InProgress → WaitingForStudent → Student comment → InProgress → Resolved/Reject
  Admin: categories enable/disable + users change role

Выход: проект запускается по README, CI зелёный, демо-сценарий проходит.

## Как использовать в Cursor
Composer → используйте промпт ниже, добавив @.cursorrules и нужные файлы server/client/http.


## Prompt (Composer)
```text
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Finalize @ServiceDesk.API and @client:

1. Add xUnit test project /src/ServiceDesk.API.Tests
   Write exactly 3 tests:
   - Test 1: Student cannot GET another student's ticket → 404
   - Test 2: Operator assign-to-me New→InProgress; second operator assign-to-me → 409
   - Test 3: POST /api/tickets with missing title → 400 ProblemDetails
   Use WebApplicationFactory with test PostgreSQL or SQLite

2. docker-compose.yml: services db (postgres:16) + api, env vars, depends_on
3. docker-compose.full.yml: db + api + client (optional)

4. GitHub Actions CI (.github/workflows/ci.yml):
   server job: dotnet build + dotnet test
   client job: npm ci + npm run lint + npm run build

5. README.md:
   - Prerequisites
   - Run with docker-compose
   - DB migrations & seed commands
   - How to run tests
   - How to run client dev server
   - Env variables reference

6. ADR.md:
   - JWT vs cookie
   - isActive (soft delete) vs hard delete for categories
   - separate /reject endpoint vs generic /status
   - DisplayName as custom field vs UserName

7. Create /http or /postman folder:
   register, login, me, categories CRUD, tickets CRUD, assign, status, reject, close, comments

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## Советы по работе в Cursor

- Composer (Ctrl+I) — создание нескольких файлов за раз
- Chat (Ctrl+L) — уточнения и дебаг
- Inline (Ctrl+K) — правка конкретного метода
- Перед каждым промтом добавлять @.cursorrules
- После генерации: "Check @TicketsController.cs against @.cursorrules — are all error codes and check orders correct?"
- Для каркасов/новых файлов — claude-sonnet-4-5 (быстрее)
- Для бизнес-логики/тестов — claude-code (точнее)
- Не давать весь план сразу — короткие фокусированные промты
```
