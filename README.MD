```markdown
# NewsPortal — мини-инструкция по запуску

## Требования
- .NET SDK 9.0
- SQL Server (подойдёт LocalDB на Windows или контейнер Docker)
- Доступ к интернету для восстановления пакетов NuGet

## 1) Клонирование и вход в каталог
```
bash
# Bash/PowerShell
git clone <url-репозитория>
cd NewsPortal
```
## 2) Настройка строки подключения
Укажите/проверьте строку подключения `ConnectionStrings:DefaultConnection` в `appsettings.json` (или в `appsettings.Development.json` для локальной разработки).

Примеры:
- Для LocalDB (Windows):
```
json
"ConnectionStrings": {
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=NewsPortal;Trusted_Connection=True;TrustServerCertificate=True"
}
```
- Для SQL Server в Docker:
```
json
"ConnectionStrings": {
"DefaultConnection": "Server=localhost,1433;Database=NewsPortal;User Id=sa;Password=<YourStrong!Passw0rd>;TrustServerCertificate=True"
}
```
Быстрый запуск SQL Server в Docker (опционально):
```
bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<YourStrong!Passw0rd>" \
-p 1433:1433 --name sql2022 -d mcr.microsoft.com/mssql/server:2022-latest
```
## 3) Подготовка базы данных (миграции)
В проект добавлены EF Core Tools, но для CLI установите глобальный инструмент:
```
bash
dotnet tool install --global dotnet-ef
```
Создайте и примените миграции (если папка Migrations отсутствует):
```
bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
Повторное применение миграций при изменениях модели:
```
bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```
## 4) Сборка и запуск
```
bash
dotnet restore
dotnet build
dotnet run
```
По умолчанию приложение будет доступно по адресам:
- http://localhost:5000
- https://localhost:5001

Актуальные URL выводятся в консоль при запуске.

Быстрый запуск с Hot Reload:
```
bash
dotnet watch run
```
## 5) Доступ и роли
- Страница новостей по умолчанию: `/News/Index`
- Страница входа в админку: `/Admin/Account/Login`

При первом старте автоматически создаются роли (например, Admin/… если предусмотрено). Пользователь по умолчанию не создаётся: зарегистрируйте пользователя и назначьте ему нужную роль (через БД/скрипт/отдельную админ-панель, если она предусмотрена).

## 6) Локализация
Приложение поддерживает ru/en, язык по умолчанию — ru. Переключение осуществляется через локализационный cookie (UI/ссылки — по реализации).

```
