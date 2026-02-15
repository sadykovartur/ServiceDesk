# Lab 08 — CI + мини-релиз (Ship)

## Цель
Подключить CI, собрать клиент и сервер, подготовить проект к демонстрации/стенду.

## Что сделать
1) GitHub Actions:
   - server: build + test
   - client: install + lint + build
2) Добавить healthcheck endpoint.
3) Добавить базовое логирование.
4) (Опционально) сделать deploy-инструкцию (staging):
   - Docker Compose на сервере или другой выбранный способ

## Критерии приёмки
- pipeline проходит
- проект собирается с нуля
- есть инструкции запуска (README)
- (опционально) ссылка на стенд

## Команды проверки
- CI должен проходить на PR
- локально: docker compose up + npm run build + dotnet test
