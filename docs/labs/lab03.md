# Lab 03 — Backend: ASP.NET Core Web API + PostgreSQL + EF Core + миграции

## Цель
Поднять сервер, подключить PostgreSQL, реализовать CRUD для Ticket и Category, Swagger.

## Что сделать
1) Создать /server: ASP.NET Core Web API.
2) Подключить EF Core + Npgsql.
3) Описать сущности: Category, Ticket (минимум полей), миграции.
4) Реализовать эндпоинты:
   - GET/POST/GET{id} для tickets
   - GET для categories (пока достаточно)
5) Подключить Swagger/OpenAPI.
6) Сделать единый формат ошибок (ProblemDetails или единый ErrorResponse).

## Критерии приёмки
- миграции присутствуют в репозитории
- API работает с PostgreSQL
- Swagger показывает эндпоинты
- ошибки 400/404 возвращаются корректно

## Команды проверки
- dotnet build
- dotnet test (если тестов пока нет — подготовить проект tests)
- docker compose up (db)
