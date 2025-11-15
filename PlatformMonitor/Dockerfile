# Use .NET 8.0 image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Add sourcepath as argument
ARG source_path=./PlatformMonitor/

# Copy files
COPY $source_path ./

# Restore and build web with NuGet secrets
RUN --mount=type=secret,id=nuget_username \
    --mount=type=secret,id=nuget_token \
    export NUGET_USERNAME=$(cat /run/secrets/nuget_username) && \
    export NUGET_TOKEN=$(cat /run/secrets/nuget_token) && \
    dotnet restore PlatformMonitor.csproj

# Publish the application
RUN dotnet publish PlatformMonitor.csproj -c Release -o out 
    
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "PlatformMonitor.dll"]

EXPOSE 8080
