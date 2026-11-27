# StudySpot 2.0 – дипломен проект

ASP.NET Core MVC / .NET 8. Приложението се стартира само на локалния компютър, на http://localhost:5080. Не публикувайте в Render или друга публична хостинг услуга.

## Стартиране
Клонирайте хранилището и изберете branch studyspot-v2. Отворете StudySpot.csproj във Visual Studio 2022 с .NET 8 SDK. Стартирайте с профил Project (не IIS Express), или от терминал:
```powershell
dotnet restore StudySpot.csproj
dotnet run --project StudySpot.csproj --no-launch-profile
```
Отворете http://localhost:5080. Без настройки се използва локална SQLite база за демонстрация.

## Облачна PostgreSQL (по желание)
В Render > studyspot-db > Connect копирайте External Database URL. В локалния терминал:
```powershell
dotnet user-secrets set "DATABASE_URL" "YOUR_EXTERNAL_DATABASE_URL" --project StudySpot.csproj
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project StudySpot.csproj --no-launch-profile
```
Заменете примерната стойност само локално. Не качвайте пароли в GitHub. Ограничете външния достъп до базата до IP адресите на упълномощените компютри. Безплатната база изтича на 26.12.2026.

## Функции
Регистрация, вход, любими, оценки, филтри, предложения за места и администраторско одобрение. EF Core създава таблиците при първото стартиране на празна база. Начални библиотечни записи: DATA_SOURCES.md. За съществуваща база използвайте миграции, не изтривайте данни.

## Администратор
В Development задайте ADMIN_BOOTSTRAP_EMAIL и ADMIN_BOOTSTRAP_PASSWORD чрез dotnet user-secrets. Стартирайте веднъж, после премахнете двете настройки. Не се повишава автоматично съществуващ потребител.

## Тестове
```powershell
dotnet test StudySpot.Tests/StudySpot.Tests.csproj
```
