# Lab 06 — ASP.NET Core Identity + JWT + роли

## Цель
Реализовать регистрацию/вход на основе Identity и защитить API/маршруты JWT + ролями.

## Backend (минимум)
- IdentityUser/IdentityRole + EF Core + PostgreSQL
- Seed ролей Student/Operator/Admin
- /api/auth/register, /api/auth/login, /api/auth/me
- JWT Bearer auth (issuer/audience/key из config)

## Frontend (минимум)
- Login/Register + хранение access token
- Protected routes + роль-зависимое меню

## Критерии приёмки
- 401/403 корректно
- Student не видит чужие тикеты
- Operator меняет статус/assign-to-me
