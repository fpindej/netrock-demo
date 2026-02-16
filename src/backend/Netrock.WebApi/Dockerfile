# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution-wide config files
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]

# Copy csproj files
COPY ["Netrock.Shared/Netrock.Shared.csproj", "Netrock.Shared/"]
COPY ["Netrock.Domain/Netrock.Domain.csproj", "Netrock.Domain/"]
COPY ["Netrock.Application/Netrock.Application.csproj", "Netrock.Application/"]
COPY ["Netrock.Infrastructure/Netrock.Infrastructure.csproj", "Netrock.Infrastructure/"]
COPY ["Netrock.WebApi/Netrock.WebApi.csproj", "Netrock.WebApi/"]

# Restore dependencies
RUN dotnet restore "Netrock.WebApi/Netrock.WebApi.csproj"

# Copy the entire source code
COPY . .

# Build the application
WORKDIR "/src/Netrock.WebApi"
RUN dotnet build "Netrock.WebApi.csproj" -c Release

# Publish stage
FROM build AS publish

# StripDevConfig controls whether appsettings.Development/Testing.json are excluded.
# Default: true (production). docker-compose.local overrides to false for local dev.
ARG STRIP_DEV_CONFIG=true

RUN dotnet publish "Netrock.WebApi.csproj" -c Release -o /app/publish \
    /p:UseAppHost=false /p:DebugType=None /p:DebugSymbols=false \
    /p:StripDevConfig=${STRIP_DEV_CONFIG}

# Defense-in-depth: strip non-production config even if .csproj exclusion is bypassed
RUN if [ "$STRIP_DEV_CONFIG" = "true" ]; then \
        rm -f /app/publish/appsettings.Development.json /app/publish/appsettings.Testing.json; \
    fi

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Netrock.WebApi.dll"]
