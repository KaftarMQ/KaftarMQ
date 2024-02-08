# Base image for runtime
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

# SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Since the Dockerfile is in the same directory as the Producer.csproj, the path is '.'
COPY ["Producer.csproj", "./"]
RUN dotnet restore "Producer.csproj"
# The context is the root of the solution, so copy the Producer directory
COPY ["./", "./"]
WORKDIR "/src"
RUN dotnet build "Producer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Producer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final base image
FROM base AS final
WORKDIR /app
# Copy from the publish stage to the final image
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Producer.dll"]
