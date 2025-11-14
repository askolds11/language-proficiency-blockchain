FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["language-proficiency-blockchain/language-proficiency-blockchain.csproj", "language-proficiency-blockchain/"]
RUN dotnet restore "language-proficiency-blockchain/language-proficiency-blockchain.csproj"
COPY . .
WORKDIR "/src/language-proficiency-blockchain"
RUN dotnet build "./language-proficiency-blockchain.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./language-proficiency-blockchain.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "language-proficiency-blockchain.dll"]
