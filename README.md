# 🎮 RAGE:MP NewServer

![Status](https://img.shields.io/badge/Status-In%20Development-yellow?style=flat-square)
![C#](https://img.shields.io/badge/Language-C%23-239120?style=flat-square)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square)
![MySQL](https://img.shields.io/badge/Database-MySQL-4479A1?style=flat-square)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

Уперминалистичная, но полнофункциональная сборка RAGE:MP сервера на **C# .NET 8** с чистой архитектурой (Clean Architecture), асинхронными операциями, полной типизацией и безопасностью.

## 📋 Особенности

✅ **Clean Architecture** - разделение на слои (Core, Application, Infrastructure)
✅ **Entity Framework Core** - ORM с миграциями для MySQL
✅ **Асинхронные операции** - полностью async/await
✅ **Безопасность** - BCrypt хеширование, валидация входных данных
✅ **Repository Pattern** - единообразный доступ к данным
✅ **Dependency Injection** - встроенный в .NET
✅ **Docker поддержка** - локальная разработка в контейнерах
✅ **Логирование** - встроенное в .NET

## 🏗️ Архитектура

```
Server/
├── Core/                      # Бизнес-логика (независимо от фреймворка)
│   └── Entities/              # Доменные модели
├── Application/               # Промежуточный слой
│   ├── Services/              # Бизнес-сервисы
│   └── DTOs/                  # Data Transfer Objects
├── Infrastructure/            # Реализация инфраструктуры
│   ├── Data/                  # Database Context
│   ├── Persistence/           # Repositories
│   └── Security/              # Криптография
├── Program.cs                 # Точка входа
└── GameServer.cs              # Основной сервер
```

## 🗄️ Доменные модели

### Player (Игрок)
- 👤 Профиль: username, email, character name
- 💰 Экономика: cash, bank money, salary
- 📊 Статистика: experience, playtime, admin level
- 🔒 Безопасность: password hash, ban system

### Vehicle (Транспорт)
- 🚗 Характеристики: model hash, plate, colors
- ⚙️ Механика: engine health, body health, fuel, mileage
- 🛠️ Модификации: paint, engine, suspension
- 💥 Повреждения: window/door state, repair cost

### Property (Недвижимость)
- 🏠 Тип: дом, квартира, бизнес
- 💵 Торговля: цена, аренда, статус продажи
- 🏪 Хранилище: сейф деньги
- 📍 Локация: entrance, interior spawn

### Bank (Банк)
- 💳 Счета: account number, balance, PIN
- 📋 Операции: deposit, withdrawal, transfer
- 📊 История: transaction logs

### Job (Работа)
- 👨‍💼 Название: taxi driver, mechanic, etc.
- 💵 Оплата: base salary, level requirements
- ✅ Статус: active/inactive

## 🚀 Быстрый старт

### Требования
- .NET 8 SDK
- Docker & Docker Compose (рекомендуется)
- MySQL 8.0+ (если не использовать Docker)

### Вариант 1: Docker (Рекомендуется)

```bash
# Клонирование
git clone https://github.com/20XMAS24/RAGEMP-NewServer.git
cd RAGEMP-NewServer

# Запуск
docker-compose up --build

# Сервер будет доступен на localhost:22005
```

### Вариант 2: Локальная разработка

```bash
# Установка зависимостей
cd src
dotnet restore

# Запуск миграций
dotnet ef database update

# Запуск сервера
dotnet run
```

## 📝 Конфигурация

### appsettings.json
```json
{
  "Database": {
    "Host": "localhost",
    "Port": 3306,
    "User": "ragemp_user",
    "Password": "ragemp_password",
    "Database": "ragemp_db"
  },
  "Server": {
    "Port": 22005,
    "MaxPlayers": 500,
    "Name": "[ALPHA] RAGEMP Server",
    "Language": "ru"
  }
}
```

## 💻 API Services

### PlayerService

```csharp
// Регистрация
await playerService.RegisterAsync(new PlayerCreateDTO 
{ 
    Username = "player1", 
    Password = "secret123",
    Email = "player@example.com"
});

// Вход
var player = await playerService.LoginAsync(new PlayerLoginDTO
{
    Username = "player1",
    Password = "secret123"
});

// Добавить деньги
await playerService.AddCashAsync(playerId, 5000);

// Получить опыт
await playerService.AddExperienceAsync(playerId, 100);

// Устроиться на работу
await playerService.SetJobAsync(playerId, "Taxi Driver");
```

### VehicleService

```csharp
// Создать машину
var vehicle = await vehicleService.CreateVehicleAsync(new VehicleCreateDTO
{
    ModelHash = 0x20B5E0DB, // Adder
    Plate = "ABC123",
    OwnerId = playerId,
    Price = 100000,
    LocationX = 100f,
    LocationY = 200f,
    LocationZ = 50f
});

// Получить машины игрока
var vehicles = await vehicleService.GetPlayerVehiclesAsync(playerId);

// Повредить машину
await vehicleService.DamageVehicleAsync(vehicleId, 300, 120.5f);

// Отремонтировать
await vehicleService.RepairVehicleAsync(vehicleId);
```

### PropertyService

```csharp
// Создать недвижимость
var property = await propertyService.CreatePropertyAsync(new PropertyCreateDTO
{
    Address = "Downtown Apartment",
    PropertyType = "apartment",
    Price = 500000,
    RentCost = 10000,
    EntranceX = 150f,
    EntranceY = 250f,
    EntranceZ = 20f,
    InteriorRoom = 1
});

// Купить
await propertyService.BuyPropertyAsync(propertyId, playerId, playerCash);

// Выставить на продажу
await propertyService.SellPropertyAsync(propertyId);

// Получить доступные
var available = await propertyService.GetAvailablePropertiesAsync();
```

### BankService

```csharp
// Открыть счет
var account = await bankService.CreateAccountAsync(playerId, "1234", "personal");

// Пополнить
await bankService.DepositAsync(accountId, 50000, "Salary deposit");

// Снять деньги
await bankService.WithdrawAsync(accountId, 20000, "1234", "ATM withdrawal");

// Перевод
await bankService.TransferAsync(fromAccountId, toAccountId, 15000, "1234");

// История
var transactions = await bankService.GetTransactionHistoryAsync(accountId, limit: 50);
```

### JobService

```csharp
// Создать работу
await jobService.CreateJobAsync("Bus Driver", 6000, requiredLevel: 5);

// Получить все работы
var allJobs = await jobService.GetAllJobsAsync();

// Обновить
await jobService.UpdateJobAsync(jobId, baseSalary: 7000);

// Удалить
await jobService.DeleteJobAsync(jobId);
```

## 🔐 Безопасность

### Хеширование паролей
- ✅ BCrypt с работающим фактором 12
- ✅ Сравнение с enhanced verification
- ✅ Защита от timing attacks

### Валидация
- ✅ Проверка уникальности username/email
- ✅ Проверка баланса перед операциями
- ✅ PIN-коды для банка
- ✅ Ban система

### Database
- ✅ Параметризованные запросы (EF Core)
- ✅ Иностранные ключи
- ✅ Индексы для производительности

## 📊 Database Schema

### Players Table
```sql
- id (PK)
- username (UNIQUE)
- password_hash
- email (UNIQUE)
- character_name
- cash
- bank_money
- job
- experience
- admin_level
- is_banned
- ban_reason
- ban_expires
- created_at
- updated_at
```

### Vehicles Table
```sql
- id (PK)
- model_hash
- plate (UNIQUE)
- owner_id (FK Players)
- engine_health
- body_health
- fuel
- mileage
- location_x, y, z
- repair_cost
- window_state (bitmask)
- door_state (bitmask)
- created_at
- updated_at
```

### Properties Table
```sql
- id (PK)
- address (UNIQUE)
- owner_id (FK Players)
- property_type
- price
- rent_cost
- entrance_x, y, z
- interior_room, x, y, z
- for_sale
- safe_money
- created_at
- updated_at
```

### BankAccounts Table
```sql
- id (PK)
- owner_id (FK Players)
- account_number (UNIQUE)
- account_type
- pin (hashed)
- balance
- is_locked
- created_at
- updated_at
```

### BankTransactions Table
```sql
- id (PK)
- account_id (FK BankAccounts)
- amount
- transaction_type (deposit/withdrawal/transfer)
- description
- previous_balance
- new_balance
- created_at
```

## 📈 Примеры использования

### Регистрация и вход
```csharp
// In your RAGE:MP client code (CEF/JavaScript)
// Send event to server
mp.events.callRemote('auth:register', {
    username: 'newplayer',
    password: 'password123',
    email: 'player@example.com'
});

// Server receives and processes
private async void OnPlayerRegister(Player player, string username, string password, string email)
{
    try
    {
        var result = await _playerService.RegisterAsync(new PlayerCreateDTO
        {
            Username = username,
            Password = password,
            Email = email
        });
        // Send success to client
        player.SendChatMessage($"Welcome {result.Username}!");
    }
    catch (Exception ex)
    {
        player.SendChatMessage($"Error: {ex.Message}");
    }
}
```

### Система зарплаты
```csharp
// Pay jobs every 10 minutes
var paymentTimer = new Timer(async (state) => 
{
    var players = await _playerService.GetAllPlayersAsync();
    foreach (var player in players)
    {
        if (!string.IsNullOrEmpty(player.Job))
        {
            var job = await _jobService.GetJobByNameAsync(player.Job);
            var salary = job.BaseSalary + (player.JobLevel * 100);
            await _playerService.AddCashAsync(player.Id, salary);
        }
    }
}, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
```

### Экономия топлива
```csharp
// Called every server tick
var drivingVehicles = GetAllDrivingVehicles();
foreach (var vehicle in drivingVehicles)
{
    if (vehicle.Fuel > 0)
    {
        vehicle.Fuel = Math.Max(0, vehicle.Fuel - 0.01f); // Small decrease per tick
        vehicle.Mileage += vehicle.Speed * 0.001f;
        await _vehicleService.UpdateVehicleAsync(vehicle.Id, vehicleDTO);
    }
    else
    {
        // Engine stops
        vehicle.EngineHealth = 0;
    }
}
```

## 🔄 Интеграция с RAGE:MP

### Client-side Event (JavaScript/CEF)
```javascript
// client.js
mp.events.add('playerQuit', () => {
    mp.events.callRemote('server:playerQuit', playerId);
});

mp.events.add('vehicleCollision', (vehicle, speed) => {
    mp.events.callRemote('server:vehicleDamage', vehicle.id, 50, speed);
});
```

### Server-side Handler (C#)
```csharp
[RemoteEvent("server:playerQuit")]
public async void OnPlayerQuit(Player player, uint playerId)
{
    var playerData = await _playerService.GetPlayerAsync(playerId);
    // Save player data
    playerData.PlaytimeMinutes += (int)(DateTime.UtcNow - playerData.LastLogin).TotalMinutes;
    // ...
}
```

## 🧪 Тестирование

```bash
# Unit tests
dotnet test

# Specific test
dotnet test --filter PlayerServiceTests

# With coverage
dotnet test /p:CollectCoverage=true
```

## 📦 Структура папок

```
src/
├── Core/
│   └── Entities/                    # Domain models
│       ├── BaseEntity.cs
│       ├── Player.cs
│       ├── Vehicle.cs
│       ├── Property.cs
│       ├── BankAccount.cs
│       └── Job.cs
├── Application/
│   ├── Services/                    # Business logic
│   │   ├── PlayerService.cs
│   │   ├── VehicleService.cs
│   │   ├── PropertyService.cs
│   │   ├── BankService.cs
│   │   └── JobService.cs
│   └── DTOs/                        # Data transfer objects
│       ├── PlayerDTO.cs
│       ├── VehicleDTO.cs
│       ├── PropertyDTO.cs
│       └── ...
├── Infrastructure/
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Persistence/
│   │   ├── Repository.cs
│   │   └── UnitOfWork.cs
│   └── Security/
│       └── PasswordHasher.cs
├── Program.cs                       # Entry point
├── GameServer.cs                    # Main server
└── Server.csproj                    # Project file

appsettings.json
docker-compose.yml
Dockerfile
.env.example
README.md
```

## 🚀 Следующие шаги для разработки

- [ ] WebSocket API для real-time updates
- [ ] Admin panel (CEF UI)
- [ ] Anti-cheat система
- [ ] Job-based commands
- [ ] Faction/Gang система
- [ ] Prison система
- [ ] Crafting система
- [ ] Vehicle painting/tuning
- [ ] Business ownership
- [ ] Teleportation system
- [ ] Event система
- [ ] Logging в базу
- [ ] Performance metrics

## 🤝 Вклад

Приветствуются pull requests! Для больших изменений сначала откройте issue.

## 📄 Лицензия

MIT License - смотрите LICENSE файл

## ❓ Помощь

- 📖 [Документация](./docs/)
- 🐛 [Issues](https://github.com/20XMAS24/RAGEMP-NewServer/issues)
- 💬 Обсуждения в Discord

---

**Разработано с ❤️ для RAGE:MP сообщества**

**Версия**: 1.0.0  
**Последнее обновление**: 2026-01-13