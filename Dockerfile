FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY codeduel-backend.csproj .
RUN dotnet restore "codeduel-backend.csproj"
COPY . .
RUN dotnet build "codeduel-backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "codeduel-backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

ENV ASPNETCORE_ENVIRONMENT=Docker

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "codeduel-backend.dll"]