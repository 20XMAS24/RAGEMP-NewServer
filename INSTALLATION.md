# 🔧 Установка и настройка

## Минимальные требования

- **OS**: Windows 10+ / Linux / macOS
- **.NET SDK**: 8.0 или выше
- **MySQL**: 8.0+ (или используйте Docker)
- **RAM**: 2GB+
- **HDD**: 500MB+

## 📥 Установка

### Вариант 1: Docker (РЕКОМЕНДУЕТСЯ)

#### Windows

1. Установите [Docker Desktop](https://www.docker.com/products/docker-desktop)
2. Откройте PowerShell в корне проекта
3. Выполните:

```powershell
docker-compose up --build
```

#### macOS / Linux

```bash
# Установите Docker если не установлен
sudo apt-get install docker.io docker-compose

# Запустите
docker-compose up --build
```

### Вариант 2: Локальная установка

#### Windows

1. Установите [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
2. Установите [MySQL Community Server](https://dev.mysql.com/downloads/mysql/)
3. Откройте cmd и выполните:

```cmd
cd src
dotnet restore
dotnet ef database update
dotnet run
```

#### macOS

```bash
# Установка через Homebrew
brew install dotnet mysql

# Запуск
cd src
dotnet restore
dotnet ef database update
dotnet run
```

#### Linux (Ubuntu/Debian)

```bash
# Установка .NET
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
sudo chmod +x dotnet-install.sh
./dotnet-install.sh --version 8.0

# Установка MySQL
sudo apt-get install mysql-server

# Запуск
cd src
dotnet restore
dotnet ef database update
dotnet run
```

## ⚙️ Конфигурация

### appsettings.json

Отредактируйте файл `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "Database": {
    "Host": "localhost",
    "Port": 3306,
    "User": "ragemp_user",
    "Password": "your_secure_password",
    "Database": "ragemp_db"
  },
  "Server": {
    "Port": 22005,
    "MaxPlayers": 500,
    "Name": "Your Server Name",
    "Language": "ru"
  },
  "Security": {
    "PasswordHashIterations": 12,
    "JwtSecret": "change-this-in-production"
  }
}
```

### MySQL Setup (если не используете Docker)

```sql
-- Создание пользователя
CREATE USER 'ragemp_user'@'localhost' IDENTIFIED BY 'ragemp_password';

-- Создание базы
CREATE DATABASE ragemp_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Выдача прав
GRANT ALL PRIVILEGES ON ragemp_db.* TO 'ragemp_user'@'localhost';
FLUSH PRIVILEGES;
```

## ✅ Проверка установки

### Через Docker

```bash
# Проверка статуса контейнеров
docker ps

# Логи сервера
docker logs ragemp_server

# Логи БД
docker logs ragemp_db
```

### Локально

```bash
# Проверка .NET
dotnet --version

# Проверка MySQL
mysql -u ragemp_user -p

# Проверка БД
SHOW DATABASES;
```

## 🔌 Тестирование подключения

### Powershell

```powershell
# Проверка доступности сервера
Test-NetConnection localhost -Port 22005

# Проверка БД
mysql -h localhost -u ragemp_user -p ragemp_db -e "SELECT 1"
```

### Bash

```bash
# Проверка порта
netstat -an | grep 22005

# Проверка MySQL
mysql -h localhost -u ragemp_user -pragemp_password ragemp_db -e "SELECT 1;"
```

## 🐛 Решение проблем

### Ошибка подключения к БД

```
Error: Access denied for user 'ragemp_user'@'localhost'
```

**Решение**:
```bash
# Проверьте пароль в appsettings.json
# Проверьте что MySQL запущен
mysql -u root -p

# Пересоздайте пользователя
DROP USER 'ragemp_user'@'localhost';
CREATE USER 'ragemp_user'@'localhost' IDENTIFIED BY 'ragemp_password';
GRANT ALL ON ragemp_db.* TO 'ragemp_user'@'localhost';
```

### Порт 22005 занят

```
Error: Address already in use
```

**Решение**:
```bash
# Windows
netstat -ano | findstr :22005
taskkill /PID <PID> /F

# Linux/macOS
lsof -i :22005
kill -9 <PID>
```

### Миграции не применяются

```bash
# Удалить старые миграции
rm -r src/Migrations

# Создать новые
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Docker контейнер не запускается

```bash
# Проверить логи
docker-compose logs

# Пересобрать образы
docker-compose up --build --force-recreate

# Очистить все
docker-compose down -v
docker-compose up
```

## 🎯 Важные порты

| Сервис | Порт | Протокол | Описание |
|--------|------|----------|----------|
| RAGE:MP | 22005 | TCP/UDP | Game server |
| MySQL | 3306 | TCP | Database |
| HTTP API | 8080 | TCP | Future REST API |

## 🔐 Безопасность

### Перед production

- [ ] Измените JWT Secret в appsettings.json
- [ ] Используйте сильный пароль для БД
- [ ] Включите SSL/TLS
- [ ] Настройте firewall
- [ ] Включите резервные копии БД
- [ ] Включите логирование
- [ ] Настройте rate limiting

## 📚 Дальше

- Читайте [README.md](README.md) для примеров использования
- Смотрите [документацию API](./docs/API.md)
- Присоединяйтесь к Discord сообществу

---

**Поддержка**: Если у вас есть вопросы, откройте issue на GitHub