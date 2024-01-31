# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy the csproj file from the current directory where Dockerfile is located
COPY ["PushNotification.csproj", "./"]
RUN dotnet restore "PushNotification.csproj"
# Copy everything else from the current directory
COPY . .
WORKDIR "/src"
RUN dotnet build "PushNotification.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "PushNotification.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final base image
FROM base AS final
WORKDIR /app
# Copy from the publish stage to the final image
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PushNotification.dll"]
