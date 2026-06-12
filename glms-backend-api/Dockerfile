FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["glms-backend-api/glms-backend-api.csproj", "glms-backend-api/"]
RUN dotnet restore "glms-backend-api/glms-backend-api.csproj"

COPY . .
WORKDIR "/src/glms-backend-api"
RUN dotnet build "glms-backend-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "glms-backend-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "glms-backend-api.dll"]