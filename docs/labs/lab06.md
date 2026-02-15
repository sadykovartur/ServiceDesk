# Lab 06 — Аутентификация и роли (Student/Operator/Admin)

## Цель
Реализовать login/register, защитить маршруты и эндпоинты, включить роли.

## Что сделать
Backend:
1) Реализовать регистрацию и вход (JWT или cookie-based).
2) Добавить роли и политики доступа:
   - Student: только свои тикеты
   - Operator: очередь и смена статусов
   - Admin: категории и пользователи
3) Добавить /me endpoint.

Frontend:
4) Страницы Login/Register.
5) Хранение сессии (JWT): аккуратно, с logout.
6) Protected routes и роль-зависимое меню.

## Критерии приёмки
- 401/403 работают корректно
- Student не видит чужие тикеты
- Operator может назначать на себя и менять статус (в UI и API)

## Команды проверки
- npm run build
- dotnet build
- docker compose up
