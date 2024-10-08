FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PlanningPoker.Website/PlanningPoker.Website.csproj", "PlanningPoker.Website/"]
RUN dotnet restore "PlanningPoker.Website/PlanningPoker.Website.csproj"
COPY . .
WORKDIR "/src/PlanningPoker.Website"
RUN dotnet build "PlanningPoker.Website.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PlanningPoker.Website.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlanningPoker.Website.dll"]
