FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY OptimizelyIntegration.sln .
COPY src/OptimizelyIntegration.Core/OptimizelyIntegration.Core.csproj src/OptimizelyIntegration.Core/
COPY src/OptimizelyIntegration.Web/OptimizelyIntegration.Web.csproj src/OptimizelyIntegration.Web/
RUN dotnet restore

COPY src/ src/
RUN dotnet publish src/OptimizelyIntegration.Web/OptimizelyIntegration.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "OptimizelyIntegration.Web.dll"]
