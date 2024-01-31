# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the SDK image to build the project files
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Broker/Broker.csproj", "Broker/"]
RUN dotnet restore "Broker/Broker.csproj"

# Copy the source code and build the application
COPY . .
WORKDIR "/src/Broker"
RUN dotnet build "Broker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Broker.csproj" -c Release -o /app/publish

# Copy the build output to the base image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Broker.dll"]
