FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore Comments.Server/Comments.Server.csproj

RUN dotnet publish Comments.Server/Comments.Server.csproj -c Release -o /app/publish

RUN apt-get update && apt-get install -y fonts-dejavu-core


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 7092

ENTRYPOINT ["dotnet", "Comments.Server.dll"]
