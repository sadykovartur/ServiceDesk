# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# После генерации решения Cursor/Codex добавит ServiceDesk.sln и проекты в src/ и tests/.
COPY . ./

RUN dotnet restore
RUN dotnet publish src/ServiceDesk.Api/ServiceDesk.Api.csproj -c Release -o /app/publish

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "ServiceDesk.Api.dll"]
