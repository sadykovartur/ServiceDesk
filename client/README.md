# client

Vite + React + TypeScript + MUI + React Router + TanStack Query.

## Запуск

```bash
npm install
npm run dev
```

## API base URL

Клиент читает адрес backend из переменной окружения `VITE_API_BASE_URL`.

Создайте файл `.env.local` в папке `client/`:

```env
VITE_API_BASE_URL=http://localhost:8080
```

Если переменная не задана, используется значение по умолчанию `http://localhost:8080`.
