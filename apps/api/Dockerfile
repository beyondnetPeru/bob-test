# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore as distinct layers
COPY ["apps/api/Api.csproj", "apps/api/"]
COPY ["libs/backend/application/Application.csproj", "libs/backend/application/"]
COPY ["libs/backend/domain/Domain.csproj", "libs/backend/domain/"]
COPY ["libs/backend/infrastructure/Infrastructure.csproj", "libs/backend/infrastructure/"]

RUN dotnet restore "apps/api/Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/apps/api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

# Stage 3: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
