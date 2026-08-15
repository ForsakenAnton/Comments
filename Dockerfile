FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore Comments.Server/Comments.Server.csproj

RUN dotnet publish Comments.Server/Comments.Server.csproj -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# The aspnet image listens on 8080 by default. Hosted platforms (Render, Cloud Run, ...)
# inject the port they expect through PORT, so honour it when it is set.
EXPOSE 8080

ENTRYPOINT ["sh", "-c", "ASPNETCORE_HTTP_PORTS=${PORT:-8080} exec dotnet Comments.Server.dll"]
