# Используем официальный образ .NET для сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем все файлы проекта
COPY . .

# Восстанавливаем зависимости
RUN dotnet restore "./Comments.Server.csproj"

# Сборка проекта
RUN dotnet publish "./Comments/Comments.Server.csproj" -c Release -o /app/publish

# Базовый образ для запуска
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Переменные среды
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Точка входа
ENTRYPOINT ["dotnet", "Comments.Server.dll"]
