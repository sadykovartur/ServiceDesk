# Service Desk (учебный проект)

Этот репозиторий содержит учебный проект Service Desk + материалы по 8 лабораторным.

## Структура
- `server/` — ASP.NET Core Web API + тесты
- `client/` — React (Vite) frontend
- `docs/` — планы и промпты для Cursor/Figma Make
- `http/` — .http запросы (финальная лаба)

## Быстрый старт (после генерации кода лабами)
### Backend
```bash
cd server
dotnet restore
dotnet test
dotnet run --project ServiceDesk.API
```

### Frontend
```bash
cd client
npm i
npm run dev
```

## Переменные окружения
Скопируйте `.env.example` в `.env` и заполните значения по комментариям.

## Документация по лабам
См. `docs/README.md`.
