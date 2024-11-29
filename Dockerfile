# Use .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything
COPY . .

# Restore
RUN dotnet restore

# Publish
RUN dotnet publish -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
COPY --from=build /app/Migrations ./Migrations

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set ASP.NET Core URLs
ENV ASPNETCORE_URLS=http://+:5000

# Run app
ENTRYPOINT ["dotnet", "UrlShortener.dll"]