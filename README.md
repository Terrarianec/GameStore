# GameStore

[![.NET 8](https://img.shields.io/badge/.NET-8-512BD4?style=for-the-badge&logo=.net)](https://dotnet.microsoft.com)
[![WPF](https://img.shields.io/badge/WPF-2-2D2D30?style=for-the-badge&logo=windows)](https://learn.microsoft.com/dotnet/desktop/wpf)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?style=for-the-badge&logo=entity-framework)](https://learn.microsoft.com/ef/core)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019-CC2927?style=for-the-badge&logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![C#](https://img.shields.io/badge/C%23-2-239120?style=for-the-badge&logo=csharp)](https://learn.microsoft.com/dotnet/csharp)

<br/>

![GameStore Logo](docs/logo.png)

[Полный отчёт](https://drive.google.com/file/d/1w5Xuyxq0U9rk5yr_9ukOFnEnZII7rm3n/view?usp=sharing)

<!-- toc -->

<!-- tocstop -->

---

## 🎮 О проекте

**GameStore** — десктопное приложение для магазина игр, разработанное в рамках курсовой работы. Приложение представляет собой онлайн-сервис цифрового распространения компьютерных игр, который исполняет роль средства технической защиты авторских прав и платформы для многопользовательских игр.

Приложение позволяет просматривать каталог игр с фильтрацией по меткам, изучать детальную информацию о каждой игре, читать отзывы и приобретать покупки. Пользователи могут создавать учётные записи, управлять личными данными и историей покупок, а также [запускать приобретённые игры](https://github.com/Terrarianec/GameStore/commit/b24ec29) из собственной библиотеки. Также реализован функционал для команд-разработчиков: создание и редактирование команд, публикация собственных игр и управление составом участников.

База данных спроектирована с учётом связей между пользователями, играми, метками, командами и отзывами, а для контроля прав доступа реализованы определяемые пользователем функции.

---

## 🏗️ Архитектура

Проект состоит из трёх слоёв, разделённых по назначению:

| Слой | Проект | Описание |
|---|---|---|
| 💾 **Данные** | `GameStore.DB` | EF Core-модели, контекст БД, отображение сущностей |
| 🔧 **Утилиты** | `GameStore.Utilities` | Вспомогательные функции и классы |
| 🖥️ **Презентация** | `GameStore.Presentation` | WPF-клиент с окнами, элементами управления и стилями |

---

## 📊 Модели данных

### ER-диаграмма

![ER-диаграмма](docs/er-diagram.png)

### Диаграмма базы данных

![Схема данных](docs/data-sheme.png)

### Основные сущности

| Сущность | Описание |
|---|---|
| 👤 **User** | Пользователь: `Id`, `Username`, `Avatar`, `Login`, `PasswordHash`, `DateOfBirth`, `Balance` |
| 🎮 **Game** | Игра: `Id`, `Name`, `Logo`, `Description`, `Price`, `RequiredFreeSpace`, `PublishDate`, `TeamId` |
| 👥 **Team** | Команда-разработчик: `Id`, `Name`, `Logo`, `OwnerId` |
| 🤝 **Member** | Связь «пользователь ↔ команда» (таблица-пересечение) |
| 🏷️ **Tag** | Метка игры |
| 💬 **GameReview** | Отзыв: `UserId`, `GameId`, `Rate`, `Content`, `PublishDate` |

---

## 💻 Экраны приложения

### 🏠 Главное окно

Рабочая область приложения. Содержит список всех игр, панель навигации и быстрый доступ к профилю.

![Главное окно](docs/main-page.png)

### 🎮 Карточка игры

Детальная информация о конкретной игре. Содержит описание, цену, обложку, отзывы и возможность покупки.

![Страница игры](docs/game-page.png)

### 👤 Профиль пользователя

Управление личными данными пользователя: редактирование имени, загрузка изображения профиля, просмотр истории покупок и баланса.

![Страница пользователя](docs/user-page.png)

### ✏️ Редактирование игры

Форма создания и редактирования информации об игре: название, описание, цена, размер, обложка, привязка к команде.

![Страница редактирования игры](docs/edit-game-page.png)

### ➕ Создание игры

Форма добавления новой игры в каталог с заполнением всех обязательных полей и привязкой к команде-разработчику.

![Создание игры](docs/create-game-page.png)

### 👥 Команда-разработчик

Управление командой-разработчиком: название, логотип, владелец.

![Страница команды](docs/team-page.png)

### 🏷️ Метки игр

Интерфейс выбора и управления метками для игр.

![Выбор меток](docs/select-tags-menu.png)

---

## 🗄️ База данных

### 🔗 Подключение

Строка подключения настроена в `GameStoreContext.OnConfiguring()`:

```
Server=localhost\SQLEXPRESS;encrypt=false;database=GameStore;user=исп-31;password=1234567890;
```

### 📥 Инициализация

Запустите скрипт **`sql/db.sql`** в **SSMS** или **Azure Data Studio** — он создаст базу `GameStore`, все таблицы, представления и определяемые пользователем функции, а также заполнит данными 16 игр и отзывы.

### ⚙️ SQL Server функции

Для проверок прав доступа используются пользовательские функции:

| Функция | Назначение |
|---|---|
| `IsOwnerOfTeam(@TeamId, @UserId)` | Является ли пользователь владельцем команды |
| `IsMemberOfTeam(@TeamId, @UserId)` | Является ли пользователь участником команды |
| `IsOwnerOfGame(@GameId, @UserId)` | Является ли пользователь владельцем игры |
| `IsDeveloperOfGame(@GameId, @UserId)` | Является ли пользователь разработчиком игры |
| `IsGamePurchased(@GameId, @UserId)` | Приобрёл ли пользователь игру |

### 📜 Дополнительные скрипты

| Скрипт | Описание |
|---|---|
| `BuyGame.sql` | Покупка игры |
| `TransferMoney.sql` | Перевод средств |
| `BalanceReplenishment.sql` | Пополнение баланса |
| `CreateTag.sql` / `DeleteTag.sql` | Добавление / удаление меток |
| `InsertGameTags.sql` | Привязка меток к играм |
| `ResetTags.sql` | Полный сброс меток |

---

## 🏃 Как запустить

### 📦 Требования

| Требование | Описание |
|---|---|
| **.NET 8.0 SDK** | Платформа для сборки и запуска |
| **SQL Server 2019** | СУБД для хранения данных |
| **Visual Studio 2022** / **VS Code + C# Dev Kit** | IDE для разработки |
| **SSMS 2018** | SQL Server Management Studio для работы с БД |

### 🚀 Запуск

```bash
dotnet build
dotnet run --project GameStore.Presentation
```

> ⚠️ Строка подключения в `GameStore.DB/GameStoreContext.cs` рассчитана на локальный SQL Server с именем `исп-31`. Перед запуском убедитесь, что БД создана через `sql/db.sql`, а параметры подключения соответствуют вашему окружению.

---

## 📂 Структура решения

```
GameStore/
├── GameStore.sln
├── GameStore.DB/           # слой данных и модели
├── GameStore.Utilities/    # вспомогательные функции
└── GameStore.Presentation/ # слой представления (WPF)
    ├── Windows/            # окна приложения
    │   ├── LoginWindow           # вход в систему (запуск приложения; «Выйти» в MainWindow)
    │   ├── RegisterWindow        # создание учётной записи («Ещё нет учётной записи?» в LoginWindow)
    │   ├── MainWindow            # главная страница приложения (открывается после входа)
    │   ├── GameWindow            # просмотр запущенной игры («Открыть» в GamePage)
    │   ├── EditProfileWindow     # редактирование профиля («Редактировать» в UserProfile)
    │   ├── EditGameWindow        # создание / редактирование игры («Редактировать» в GamePage, «Создать игру» в TeamPage)
    │   ├── EditTeamWindow        # создание / редактирование команды («Моя команда» в MainWindow, «Редактировать» в TeamPage)
    │   ├── AddMembersWindow      # добавление участников в команду («Добавить участников» в TeamPage)
    │   └── SelectGameTagsWindow  # выбор меток для игры («Выбрать метки» в EditGameWindow)
    ├── Pages/            # встраиваемые страницы в MainWindow
    │   ├── MainStorePage       # каталог игр («Магазин» в MainWindow)
    │   ├── GamePage            # страница игры (выбор игры из MainStorePage или UserProfile)
    │   ├── UserProfile         # страница пользователя («Профиль» в MainWindow, выбор пользователя в TeamPage)
    │   └── TeamPage            # страница команды («Моя команда» в MainWindow, выбор команды в GamePage или UserProfile)
    ├── Controls/           # пользовательские элементы управления
    │   ├── UploadImageArea     # загрузка изображения с превью (используется в RegisterWindow, EditProfileWindow, EditGameWindow)
    │   ├── CaptchaBox          # простая капча (используется в LoginWindow, RegisterWindow)
    │   └── RateView            # интерактивная оценка в виде лягушек (оценки в GamePage)
    └── Resources/          # изображения и другие ресурсы
```