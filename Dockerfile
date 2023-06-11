# Base image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the entire project and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Set the connection string as an environment variable
ENV ConnectionStrings__Default "Server=db;Port=5432;Database=basket-service;User Id=hanordev;Password=hanordevs;"

# Expose port
EXPOSE 80
ENTRYPOINT ["dotnet", "BasketService.dll"]
