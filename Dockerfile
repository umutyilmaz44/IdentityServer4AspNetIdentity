# BASE Stage
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim as base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# BUILD Stage
FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /app
# Copy csproj and restore as distinct layers
# csproj files
COPY *.csproj ./

# Copy everything else, except specified into dockerignore
RUN dotnet restore ./*.csproj
COPY . .
RUN dotnet publish -c Release ./*.csproj -o /app/dist

# FINAL Stage
FROM base AS final
WORKDIR /app

LABEL maintainer="Ümit YILMAZ <umutyilmaz44@gmail.com>" name="netShop.IdentityService" description="Microservices Architecture (IdentityService) on .NET 5.0 with applying IdentityServer4, AspNetIdentity, {Postgresql, InMemory, Sqlite}"

ENV ASPNETCORE_ENVIRONMENT=Development
ENV UseHttps=YES
ENV ASPNETCORE_HTTPS_PORT=5001
ENV DbSettings__Host=127.0.0.1
ENV DbSettings__Port=5432
ENV DbSettings__Username=postgres
ENV DbSettings__Password=netshop
ENV DbSettings__Database=NetShopIdentityDb
ENV DbSettings__DatabaseType=Postgresql

COPY --from=build /app/dist .
ENTRYPOINT ["dotnet", "netShop.IdentityService.dll"]