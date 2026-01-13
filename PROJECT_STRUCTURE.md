# 📄 Структура проекта

```
RAGEMP-NewServer/
├── src/                           # Основной код приложения
│   ├── Core/                      # Слой бизнес-логики
│   │   └── Entities/              # Доменные модели (ГЭННАЁ!лав)
│   │       ├── BaseEntity.cs      # Базовые поля (ид, время)
│   │       ├── Player.cs          # Модель игрока
│   │       ├── Vehicle.cs         # Модель транспорта
│   │       ├── VehicleModification.cs # Модификации
│   │       ├── Property.cs        # Недвижимость
│   │       ├── BankAccount.cs     # Банковские счета
│   │       ├── BankTransaction.cs # Трансакции
│   │       └── Job.cs             # Работы/Профессии
│   ├── Application/                # Промежуточный слой
│   │   ├── Services/              # Бизнес-логика
│   │   │   ├── PlayerService.cs    # Логика для игроков
│   │   │   ├── VehicleService.cs   # Логика транспорта
│   │   │   ├── PropertyService.cs  # Логика недвижимости
│   │   │   ├── BankService.cs      # Логика банка
│   │   │   └── JobService.cs       # Логика работ
│   │   └── DTOs/                  # Объекты передачи данных
│   │       ├── PlayerDTO.cs
│   │       ├── VehicleDTO.cs
│   │       ├── PropertyDTO.cs
│   │       └── ...
│   ├── Infrastructure/             # Инфраструктура аппликации
│   │   ├── Data/                  # работа с грунтом данных
│   │   │   └── AppDbContext.cs    # Entity Framework Core контекст
│   │   ├── Persistence/           # репозитории
│   │   │   ├── Repository.cs      # Обычный репозиторий
│   │   │   └── UnitOfWork.cs      # Unit of Work паттерн
│   │   └── Security/              # Безопасность
│   │       └── PasswordHasher.cs  # BCrypt хеширование
│   ├── Program.cs                  # Точка входа
│   ├── GameServer.cs               # Основной класс сервера
│   └── Server.csproj               # C# проект файл
├── appsettings.json            # Сервер конфигурация
├── .env.example                # Пример переменных окружения
├── Dockerfile                  # Docker образ сервера
├── docker-compose.yml          # Docker Compose оркестрация
├── .gitignore                  # Git игнорирование
├── README.md                   # Основная документация
├── INSTALLATION.md             # Проводник о установке
├── PROJECT_STRUCTURE.md        # Этот файл 🐈
└── LICENSE                     # MIT Лицензия
```

## 💡 Подключателю таблица

### Код добавляется в **Application/Services/**

Новая бизнес-логика горит здесь. Она не зависит от данных.

**Пример**: При отправке машины на ромонт - наделается `VehicleService.RepairVehicleAsync()`

### Данные страницы в **Core/Entities/**

Доменные модели - пуре данные классы без логики.

**Пример**: `Vehicle` граница ни сама не ромонтируется - это работа для `VehicleService`

### База данных в **Infrastructure/Data/**

EF Core хранилище. Миграции сочиняются с моделями автоматически.

### репозитории в **Infrastructure/Persistence/**

Обычные репозитории абстрактные исполнение `CRUD` операций.

## 🚀 Как добавить новую фичу

### 1. Создать модель (Entity)

```csharp
// src/Core/Entities/Garage.cs
public class Garage : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public float LocationX { get; set; }
    public float LocationY { get; set; }
    public float LocationZ { get; set; }
    
    // Navigation
    public virtual ICollection<Vehicle> StoredVehicles { get; set; } = new List<Vehicle>();
}
```

### 2. Обновить DbContext

```csharp
// src/Infrastructure/Data/AppDbContext.cs
public DbSet<Garage> Garages { get; set; }

// In OnModelCreating:
modelBuilder.Entity<Garage>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
});
```

### 3. Создать DTO

```csharp
// src/Application/DTOs/GarageDTO.cs
public class GarageDTO
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
```

### 4. Написать Service

```csharp
// src/Application/Services/GarageService.cs
public interface IGarageService
{
    Task<GarageDTO?> GetGarageAsync(uint id);
    Task<GarageDTO> CreateGarageAsync(string name, float x, float y, float z);
}

public class GarageService : IGarageService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<GarageDTO?> GetGarageAsync(uint id)
    {
        var garage = await _unitOfWork.Garages.GetByIdAsync(id);
        return garage == null ? null : MapToDTO(garage);
    }
    
    // ... интеграция
}
```

### 5. Обрегистрировать Service

```csharp
// src/Program.cs
services.AddScoped<IGarageService, GarageService>();
```

### 6. Корректировать Infrastructure

```csharp
// src/Infrastructure/Persistence/UnitOfWork.cs
public IRepository<Garage> Garages => GetRepository<Garage>();
```

## 📂 Овервю

| Шаг | Где | Что | Почему |
|-------|-------|------|--------|
| 1 | Core | Адд Entity | Определить структуру данных |
| 2 | Infrastructure | Обновить DbContext | Конфигурировать ИМ МАППИНГ |
| 3 | Application | Создать DTO | Мешать внутренние данные |
| 4 | Application | Написать Service | Гбизнес-ориентированная логика |
| 5 | Program.cs | Обрегистрир | Охватить DI |
| 6 | UnitOfWork | Адд Repo | улисти доступа |

## 📄 Ключевые файлы

| Файл | Дескриптион |
|--------|--------|
| `Server.csproj` | Все NuGet депенденции |
| `appsettings.json` | Конфигурация БД и сервера |
| `Program.cs` | DI и инициализация |
| `AppDbContext.cs` | EF Core контекст |
| `docker-compose.yml` | Docker оркестрация |

## 🧪 Тестирование

Когда добавляете новую фичу:

```bash
# 1. Построить
dotnet build

# 2. Попробуйте
dotnet run

# 3. Проверьте в числовых тестах
dotnet test
```
