# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project files
COPY ["CrimeStatistics/CrimeStatistics.csproj", "CrimeStatistics/"]
RUN dotnet restore "CrimeStatistics/CrimeStatistics.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "CrimeStatistics/CrimeStatistics.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "CrimeStatistics/CrimeStatistics.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["dotnet", "CrimeStatistics.dll"]
