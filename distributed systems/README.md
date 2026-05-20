# BookingDemo

Навчальний проект з розподілених систем на .NET 10 — система бронювання готелів.

## Сервіси

| Сервіс | Порт | Відповідальність |
|--------|------|-----------------|
| HotelService | 5001 | Готелі та номери |
| BookingService | 5002 | Бронювання |
| GuestService | 5003 | Гості |

## Запуск

```bash
docker compose up --build
```

Swagger UI:
- HotelService: http://localhost:5001/swagger
- BookingService: http://localhost:5002/swagger
- GuestService: http://localhost:5003/swagger

## Міграції

```bash
dotnet ef migrations add InitialCreate --project src/Shared --startup-project src/HotelService
dotnet ef database update --project src/Shared --startup-project src/HotelService
```

## Структура

```
src/
├── Shared/                    # Моделі, DbContext, міграції
├── HotelService/              # Web API: готелі та номери
├── HotelService.Application/  # Сервіси та інтерфейси репозиторіїв
├── BookingService/            # Web API: бронювання
├── BookingService.Application/
├── GuestService/              # Web API: гості
└── GuestService.Application/
```

> ⚠️ Week 1: Shared database — навмисний антипатерн для навчання. У Week 3 замінимо на окремі БД.
