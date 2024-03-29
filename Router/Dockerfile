﻿# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

# Use the SDK image to build the project files
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./Router.csproj", "./"]
COPY ["./RoutingAlgorithm/RoutingAlgorithm.csproj", "./RoutingAlgorithm/"]
RUN dotnet restore "./Router.csproj"

# Copy the source code and build the application
COPY . .
WORKDIR "/src/"
RUN dotnet build "Router.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Router.csproj" -c Release -o /app/publish

# Copy the build output to the base image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Router.dll"]