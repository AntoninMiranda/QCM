# Stage de développement avec hot reload
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS development
WORKDIR /src
EXPOSE 5000
# Copier le projet et restaurer les dépendances
COPY ["backend.csproj", "./"]
RUN dotnet restore "backend.csproj"
COPY . .
# Installer dotnet-ef en tant que package local si nécessaire
ENV PATH="${PATH}:/root/.dotnet/tools"
CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:5000"]

# Stage de base pour la production
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

# Stage de build pour la production
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend.csproj", "./"]
RUN dotnet restore "backend.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage de publication pour la production
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage final pour la production
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "backend.dll"]
