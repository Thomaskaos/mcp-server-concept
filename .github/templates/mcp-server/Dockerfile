# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy Shared project files for restore
COPY Shared/Shared.csproj Shared/

# Copy project file for restore
COPY {{ServerName}}/{{ServerName}}.csproj {{ServerName}}/

# Restore dependencies (cached layer)
RUN dotnet restore "{{ServerName}}/{{ServerName}}.csproj"

# Copy remaining source files
COPY Shared/ Shared/
COPY {{ServerName}}/ {{ServerName}}/

# Build and publish
RUN dotnet publish "{{ServerName}}/{{ServerName}}.csproj" -c Release -o /app/publish --runtime linux-x64 --self-contained false 

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY {{ServerName}}/appsettings.json ./
EXPOSE {{Port}}
ENTRYPOINT ["dotnet", "{{ServerName}}.dll"]