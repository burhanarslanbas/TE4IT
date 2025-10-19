# Multi-stage build
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/TE4IT.API/TE4IT.API.csproj", "src/TE4IT.API/"]
COPY ["src/TE4IT.Application/TE4IT.Application.csproj", "src/TE4IT.Application/"]
COPY ["src/TE4IT.Domain/TE4IT.Domain.csproj", "src/TE4IT.Domain/"]
COPY ["src/TE4IT.Infrastructure/TE4IT.Infrastructure.csproj", "src/TE4IT.Infrastructure/"]
COPY ["src/TE4IT.Persistence/TE4IT.Persistence.csproj", "src/TE4IT.Persistence/"]

# Restore packages
RUN dotnet restore "src/TE4IT.API/TE4IT.API.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/src/TE4IT.API"
RUN dotnet build "TE4IT.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "TE4IT.API.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TE4IT.API.dll"]
