FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["growth-planning-be/growth-planning-be.csproj", "growth-planning-be/"]
COPY ["dll/", "dll/"]
RUN dotnet restore "growth-planning-be/growth-planning-be.csproj"
COPY . .
WORKDIR "/src/growth-planning-be"
RUN dotnet build "./growth-planning-be.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./growth-planning-be.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "growth-planning-be.dll"]
