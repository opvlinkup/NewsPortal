

## NewsPortal
```
Требования
- .NET SDK 9.0
- SQL Server

Установка

git clone <url-репозитория>
cd NewsPortal
dotnet restore
```
## Настройка подключения к БД 
Cтрокf подключения в appsettings.json (или appsettings.Development.json):
```
json
{
"ConnectionStrings": {
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=NewsPortal;Trusted_Connection=True;TrustServerCertificate=True"
}
}
```
## Миграции БД
Установите инструмент EF и примените миграции:
```
dotnet tool install --global dotnet-ef
dotnet ef migrations add Init
dotnet ef database update
```
## Сборка и запуск
```
dotnet build
dotnet run
```
Приложение будет доступно по адресам: http://localhost:5000 и https://localhost:5001.

## Дополнительно
- Горячий перезапуск: `dotnet watch run`
- Главная страница: `/News/Index`
- Вход в админку: `/Admin/Account/Login`
```
