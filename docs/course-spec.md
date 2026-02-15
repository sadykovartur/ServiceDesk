# Конструирование интернет-приложений — сквозной проект Service Desk

## 0. Цель курса
Студенты проектируют, разрабатывают и выпускают веб-приложение (интернет-приложение) по модели "сквозного проекта":
- дизайн-спецификация (Figma)
- backend (ASP.NET Core Web API + PostgreSQL)
- frontend (React + TypeScript + MUI)
- качество (валидация, роли/права, тесты)
- поставка (Docker Compose, CI)

## 1. Тема проекта: Service Desk (Система заявок/тикетов)
Система для подачи и обработки заявок (проблем/запросов) в рамках университета/кафедры:
- студент создает заявку и ведет переписку
- оператор берет в работу и меняет статус
- администратор управляет справочниками и ролями

## 2. Роли и права
### Роли
- Student
- Operator
- Admin

### Права (MVP)
Student:
- регистрация/вход
- создание заявки
- просмотр своих заявок (список + фильтры)
- просмотр карточки заявки
- комментарии к заявке
- (опционально) редактирование/отмена заявки только в статусе New

Operator:
- просмотр очереди заявок (все/новые/назначенные на меня)
- назначить заявку на себя (Assign to me)
- смена статуса по правилам
- комментарии
- (опционально) внутренние комментарии (видны только Operator/Admin)

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
Сущности:
User:
- id (GUID)
- email (unique)
- passwordHash
- fullName
- role
- createdAt

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
- EF Core + provider PostgreSQL (Npgsql)
- Migration-based DB evolution
- OpenAPI/Swagger

Quality & Delivery:
- Docker Compose (api + db)
- CI (GitHub Actions): build + tests + lint
- Minimal healthcheck endpoint

## 7. Репозиторий и структура
Монорепо:
- /server
- /client
- /docs
- docker-compose.yml
- .github/workflows/ci.yml

Требования к репо:
- README с командами запуска
- миграции в коммите
- единый стиль именования и структура папок
- сдача через Pull Request

## 8. API (минимальный набор)
Auth:
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

## 10. Формат сдачи
Для каждой лабораторной:
- PR в репозиторий
- чек-лист критериев выполнен
- короткая защита: показать UI + API + данные (по теме лабы)

Курсовая работа:
- итоговый работающий проект + демонстрация
- отчёт/презентация: архитектура, модель данных, ключевые сценарии, безопасность
