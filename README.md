# Service Desk — учебный шаблон (React + TS + MUI / ASP.NET Core Identity + JWT / PostgreSQL)

Монорепо для курса "Конструирование интернет-приложений".
Сквозной проект: система заявок (тикетов) с ролями Student/Operator/Admin.

> Этот архив — стартовый каркас (docs + infra). Код server/client предполагается
> сгенерировать через Cursor/Codex по спецификации в `docs/course-spec.md` и лабораторным.

## Быстрый старт (после генерации кода)

### Предпосылки
- .NET SDK (LTS)
- Node.js LTS + npm
- Docker + Docker Compose
- Git

### 1) Запуск базы данных
```bash
docker compose up -d db
```

### 2) Запуск backend (локально)
```bash
cd server
dotnet restore
dotnet build
dotnet run --project src/ServiceDesk.Api
```

Swagger: http://localhost:8080/swagger

### 3) Запуск frontend (локально)
```bash
cd client
npm ci
npm run dev
```

Frontend: http://localhost:5173

## Запуск всего через Docker Compose (опционально)
```bash
docker compose up --build
```

## Лабораторные
См. `docs/labs/lab01.md` ... `docs/labs/lab08.md`.
Сдача каждой лабы: Pull Request + демонстрация по критериям приёмки.
