# 🚀 Быстрый старт

## 🕌 30 секунд

```bash
# 1. Клонирую
git clone https://github.com/20XMAS24/RAGEMP-NewServer.git
cd RAGEMP-NewServer

# 2. Запускаю в Docker
docker-compose up --build

# 3. Пожидаю когда высочитя
# " Server is ready! Waiting for RAGE:MP client connections..."
```

**Это оно!** Сервер работает на `localhost:22005`

## 💯 Что дальше?

- 📄 [Full Setup Guide](INSTALLATION.md)
- 📈 [API Reference](README.md#-api-services)
- 📄 [Project Structure](PROJECT_STRUCTURE.md)
- 🌱 [Examples](README.md#--%D0%BF%D1%80%D0%B8%D0%BC%D0%B5%D1%80%D1%8B-%D0%B8%D1%81%D0%BF%D0%BE%D0%BB%D1%8C%D0%B7%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D1%8F)

## 🚗 Тестирование

### Основные навыки

```csharp
// Регистрация
private async void TestRegister()
{
    var playerService = serviceProvider.GetRequiredService<IPlayerService>();
    
    var newPlayer = await playerService.RegisterAsync(new PlayerCreateDTO
    {
        Username = "testplayer",
        Password = "password123",
        Email = "test@example.com"
    });
    
    Console.WriteLine($"✅ Новый игрок: {newPlayer.Username}");
}
```

### Получить данные

```csharp
var player = await playerService.GetPlayerAsync(playerId);
Console.WriteLine($"📃 {player.CharacterName}: деньг: {player.Cash}");
```

### Экономика

```csharp
// Добавить деньги
await playerService.AddCashAsync(playerId, 10000);

// Опыт
var xpAdded = await playerService.AddExperienceAsync(playerId, 500);

// Начало работы
await playerService.SetJobAsync(playerId, "Taxi Driver");
```

## 💻 Напрявки архитектуры

### Красные прапор

❌ NEVER открыт данные напрямую из Entity

```csharp
// 💣 WRONG!
public async Task<Player> GetPlayer(uint id)
{
    var player = await _dbContext.Players.FindAsync(id);
    return player; // Никогда!
}
```

✅ Когда Service -> DTO

```csharp
// ✅ CORRECT!
public async Task<PlayerDTO> GetPlayer(uint id)
{
    var player = await _unitOfWork.Players.GetByIdAsync(id);
    return MapToDTO(player); // Always return DTO!
}
```

### Данные флоу

```
Client -> Server 
          |
          ↓ (PlayerCreateDTO)
          |
      PlayerService (Business Logic)
      /  |
     /   ↓
Validation
     \   ↓
      \ Repository (CRUD)
       \↓
        Database
```

### Правила очки

1. 📄 **Entity** = Pure Data (нет логики)
2. 🔧 **Service** = Business Logic (не работает с DB напрямую)
3. 💿 **Repository** = Обэкт доступа (КРУД)
4. 🔖 **DTO** = Меняю (Отправляю) данные
5. 🔒 **Security** = Всегда по МОРЕ

## 📃 Полезные моменты

| У меня есть | Мне нужно | Пишу в |
|---------|----------|--------|
| Новая Entity | Repo Interface | `Core/Entities/` |
| Новая Service | Business Logic | `Application/Services/` |
| Новые DTO | Наружные данные | `Application/DTOs/` |
| Крипто нужна | Финн основы | `Infrastructure/Security/` |

## 🏗️ Общие ошибки

| Ошибка | Почему | Ответ |
|-------|--------|------|
| `ArgumentNullException` | Forgetting `??` or null checks | Null checks before access |
| `InvalidOperationException` | No await on async | Use `await Task` |
| `DbUpdateException` | FK constraint | Check navigation properties |
| `JsonException` | DTO mismatch | Check DTO structure |

## 📚 Доступ к данным

### Правильно (через Service)

```csharp
private readonly IPlayerService _playerService;

public async Task<PlayerDTO?> GetPlayer(uint id)
{
    return await _playerService.GetPlayerAsync(id);
}
```

### НЕПравильно (напрямую из DbContext)

```csharp
private readonly AppDbContext _dbContext;

public Player GetPlayer(uint id) // ❌ BAD!
{
    return _dbContext.Players.FirstOrDefault(p => p.Id == id);
}
```

## 🔗 Линки

- [README](README.md)
- [Installation](INSTALLATION.md)
- [Project Structure](PROJECT_STRUCTURE.md)
- [GitHub](https://github.com/20XMAS24/RAGEMP-NewServer)

---

**Мы готовы! Час начать разработку?** 🚀