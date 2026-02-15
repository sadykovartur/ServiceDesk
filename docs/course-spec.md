# Конструирование интернет-приложений — сквозной проект Service Desk

## 0. Цель курса
Студенты проектируют, разрабатывают и выпускают веб-приложение по модели "сквозного проекта":
- дизайн-спецификация (Figma)
- backend (ASP.NET Core Web API + PostgreSQL + Identity + JWT)
- frontend (React + TypeScript + MUI)
- качество (валидация, роли/права, тесты)
- поставка (Docker Compose, CI)

## 1. Тема проекта: Service Desk (Система заявок/тикетов)
Система для подачи и обработки заявок в рамках университета/кафедры.

## 2. Роли и права
Роли (MVP):
- Student
- Operator
- Admin

Права (MVP):
Student:
- регистрация/вход
- создание заявки
- просмотр своих заявок (список + фильтры)
- просмотр карточки заявки
- комментарии к заявке

Operator:
- просмотр очереди заявок (новые/назначенные на меня/все)
- назначить заявку на себя (Assign to me)
- смена статуса по правилам
- комментарии

Admin:
- CRUD категорий
- управление пользователями и ролями (минимально: смена роли)

## 3. Статусы и workflow
Статусы (MVP):
- New
- InProgress
- WaitingForStudent
- Resolved
- Closed
- Rejected

Правила:
- Operator может переводить New -> InProgress, InProgress -> WaitingForStudent/Resolved, WaitingForStudent -> InProgress/Resolved, Resolved -> Closed, а также переводить в Rejected (с обязательным reason).
- Student может добавлять комментарии в любом статусе.
- Rejected требует причины (rejectedReason).

## 4. Данные (минимальная модель)
Сущности (MVP):
Category:
- id
- name
- isActive

Ticket:
- id
- title
- description
- categoryId
- priority (Low/Medium/High)
- status
- authorId
- assigneeId (nullable)
- rejectedReason (nullable)
- createdAt, updatedAt

Comment:
- id
- ticketId
- authorId
- text
- isInternal (bool, optional)
- createdAt

Пользователи/роли:
- использовать ASP.NET Core Identity (IdentityUser/IdentityRole) с хранилищем в PostgreSQL (EF Core)
- роли: Student/Operator/Admin (seeding при старте)
- (опционально) seed admin user через переменные окружения

## 5. UI (набор экранов)
Публичные:
- /login
- /register (или combined /auth)

После входа (layout с AppBar + Drawer):
Student:
- /tickets (мои заявки)
- /tickets/new (создание)
- /tickets/:id (карточка)

Operator:
- /queue (новые)
- /assigned (назначенные на меня)
- /tickets/:id

Admin:
- /admin/categories
- /admin/users

Обязательные состояния UI для ключевых экранов:
- loading
- empty
- error
- validation errors (forms)

## 6. Технический стек
Frontend:
- Vite + React + TypeScript
- MUI
- React Router
- Server state: TanStack Query (рекомендуется)
- Forms: React Hook Form + Zod (рекомендуется)

Backend:
- ASP.NET Core Web API
- EF Core + Npgsql
- ASP.NET Core Identity Core (users + roles)
- JWT Bearer auth
- OpenAPI/Swagger
- Migration-based DB evolution

Quality & Delivery:
- Docker Compose (api + db)
- CI (GitHub Actions): build + tests + lint
- Healthcheck endpoint (/health)

## 7. Репозиторий и структура
Монорепо:
- /server
- /client
- /docs
- docker-compose.yml
- .github/workflows/ci.yml

## 8. API (минимальный набор)
Auth (Identity + JWT):
- POST /api/auth/register
- POST /api/auth/login
- GET  /api/auth/me

Categories (Admin):
- GET/POST/PUT/DELETE /api/categories

Tickets:
- GET  /api/tickets (пагинация + фильтры)
- POST /api/tickets
- GET  /api/tickets/{id}
- PUT  /api/tickets/{id} (ограниченно, по правилам)
- POST /api/tickets/{id}/assign-to-me (Operator)
- POST /api/tickets/{id}/status (Operator, body: status + optional reason)

Comments:
- GET  /api/tickets/{id}/comments
- POST /api/tickets/{id}/comments

Users (Admin):
- GET /api/users
- PUT /api/users/{id}/role

## 9. Нефункциональные требования (минимум)
- валидация входных данных (backend + frontend)
- корректные коды ошибок (401/403/404/400)
- единый формат ошибок (ProblemDetails или аналог)
- безопасность: доступ к данным согласно ролям
- работа через docker-compose
