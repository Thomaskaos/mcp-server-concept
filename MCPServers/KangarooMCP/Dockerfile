# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy Shared project files for restore
COPY Shared/Shared.csproj Shared/

# Copy project file for restore
COPY KangarooMCP/KangarooMCP.csproj KangarooMCP/

# Restore dependencies (cached layer)
RUN dotnet restore "KangarooMCP/KangarooMCP.csproj"

# Copy remaining source files
COPY Shared/ Shared/
COPY KangarooMCP/ KangarooMCP/

# Build and publish
RUN dotnet publish "KangarooMCP/KangarooMCP.csproj" -c Release -o /app/publish --runtime linux-x64 --self-contained false 

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY KangarooMCP/appsettings.json ./
EXPOSE 4547
ENTRYPOINT ["dotnet", "KangarooMCP.dll"]