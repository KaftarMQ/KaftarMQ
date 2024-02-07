# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy the csproj file from the current directory where Dockerfile is located
COPY ["Syncer.csproj", "./"]
RUN dotnet restore "Syncer.csproj"
# Copy everything else from the current directory
COPY . .
WORKDIR "/src"
RUN dotnet build "Syncer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Syncer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final base image
FROM base AS final
WORKDIR /app
# Copy from the publish stage to the final image
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Syncer.dll"]