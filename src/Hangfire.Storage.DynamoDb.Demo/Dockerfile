FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Hangfire.Storage.DynamoDb.Demo/Hangfire.Storage.DynamoDb.Demo.csproj", "src/Hangfire.Storage.DynamoDb.Demo/"]
RUN dotnet restore "src/Hangfire.Storage.DynamoDb.Demo/Hangfire.Storage.DynamoDb.Demo.csproj"
COPY . .
WORKDIR "/src/src/Hangfire.Storage.DynamoDb.Demo"
RUN dotnet build "Hangfire.Storage.DynamoDb.Demo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Hangfire.Storage.DynamoDb.Demo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hangfire.Storage.DynamoDb.Demo.dll"]
