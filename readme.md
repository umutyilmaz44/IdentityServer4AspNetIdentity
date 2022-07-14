# netShop.IdentityService

## Microservices Architecture (IdentityServer4) on .NET 5.0 with applying IdentityServer4, AspNetIdentity, {Postgresql, InMemory, Sqlite}

**NOTICE:** 
* This project is for authentication and authorization.

## For Migrations
**NOTICE:** 
> When application is starting auto migration will start, so you don't need to run following commands  


Creating Migration For ConfigurationDbContext (Client, ApiScope, vs tables)
>add-migration InitialConfigurationDbContext -c ConfigurationDbContext
dotnet ef migrations add InitialConfigurationDbContext -c ConfigurationDbContext -o Data/Migrations/ConfigDbContext

Creating Migration For PersistedGrantDbContext (DeviceCodes, PersistedGrants tables)
>add-migration InitialPersistedGrantDbContext -c PersistedGrantDbContext
dotnet ef migrations add InitialPersistedGrantDbContext -c PersistedGrantDbContext -o Data/Migrations/PersistGrantDbContext

Creating Migration For ApplicationDbContext (AspNetRoles, AspNetUsers, vs tables)
>add-migration Initial -c ApplicationDbContext
dotnet ef migrations add InitialApplicationDbContext -c ApplicationDbContext -o Data/Migrations/AppDbContext

Creating DB Tables For ConfigurationDbContext By Migration
>dotnet ef database update ConfigurationDbContextMigration -c ConfigurationDbContext

Creating DB Tables For PersistedGrantDbContext By Migration
>dotnet ef database update PersistedGrantDbContextMigration -c PersistedGrantDbContext

Creating DB Tables For ApplicationDbContext By Migration
>dotnet ef database update ApplicationDbContextMigration -c ApplicationDbContext


## Moreover, 
> ***```postgresqlAddress```*** word in the this document means that is a postgresql url or service name in cloud network. (ex: htttp://192.168.0.20 OR dbService in docker swarm, kubernetes, openshift, vs...)

> ***```postgresqlDataPath```*** word in the this document means that is your persist postgresql data's path (ex: ${HOME}/netShop/productService/db/data)

## To Work With Postgresql
 * cd project-path
 * docker run -d --rm --network netshop-network -p 5432:5432 --name c_netshop_identity_service_db \
-e POSTGRES_PASSWORD=netshop -e POSTGRES_USER=postgres -e POSTGRES_DB=NetShopIdentityDb \
-v ***```postgresqlDataPath```***:/var/lib/postgresql/data \
postgres

## To build docker image:
 * cd projectPath/
 * docker image build -t netshop_identity_service .

## Create Developer Certification to run image with Https:
 For MacOS
 * dotnet dev-certs https -ep ${HOME}/.aspnet/https/netShop.IdentityService.pfx -p netProduct123.
 * dotnet dev-certs https --trust

## To run local docker image with HTTPS:
 * docker container run --rm --network netshop-network -p 5000:80 -p 5001:443 --name c_netshop_identity_service \
-e DbSettings__PostgresqlSettings__Host=***```postgresqlAddress```*** -e UseHttps=yes \
-e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=5001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="netProduct123." \
-e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/netShop.IdentityService.pfx -v ${HOME}/.aspnet/https:/https/ \
netshop_identity_service

## To run docker hub image with HTTPS:
 * docker container run --rm --network netshop-network -p 5000:80 -p 5001:443 --name c_netshop_identity_service \
-e DbSettings__PostgresqlSettings__Host=***```postgresqlAddress```*** -e UseHttps=yes \
-e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=5001 \
-e ASPNETCORE_Kestrel__Certificates__Default__Password="netProduct123." \
-e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/netShop.IdentityService.pfx -v ${HOME}/.aspnet/https:/https/ \
uyilmaz/netshop_identity_service

## To run docker app with local docker-compose:
* docker-compose up
```yml
version: '3.7'

services:
  c_netshop_identity_service_db:
    image: postgres
    restart: on-failure
    environment:
      - POSTGRES_PASSWORD=password
      - POSTGRES_USER=postgres 
      - POSTGRES_DB=NetShopIdentityDb
    ports:
      - "5432:5432"
    volumes:
      - postgresqlDataPath:/var/lib/postgresql/data
    networks:
      - netshop-network
      
  c_netshop_identity_service:
    image: uyilmaz/netshop_identity_service
    depends_on:
      - "c_netshop_identity_service_db"
    restart: on-failure
    environment:
      - DbSettings__DatabaseType=Postgresql
      - DbSettings__PostgresqlSettings__Host=c_netshop_identity_service_db
      - DbSettings__PostgresqlSettings__Port=5432
      - DbSettings__PostgresqlSettings__Username=postgres
      - DbSettings__PostgresqlSettings__Password=password
      - DbSettings__PostgresqlSettings__Database=NetShopIdentityDb
      - UseHttps=yes
      - ASPNETCORE_URLS=https://+;http://+
      - ASPNETCORE_HTTPS_PORT=5001
      - ASPNETCORE_Kestrel__Certificates__Default__Password=netProduct123.
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/netShop.IdentityService.pfx
    ports:
      - "5000:80"
      - "5001:443"
    volumes:
      - ${HOME}/.aspnet/https:/https/
    networks:
      - netshop-network

networks:
  netshop-network: {}
```

## NOTICE :
* You can change certifitaion passsword {netProduct123.} and 5000, 5001 ports what you want.

* ALL PARAMETERS OF THE APPLICATION HAS DESCRIBED ON MY DOCKER HUB IMAGE. FOR DETAIL, YOU CAN VISIT [uyilmaz/netshop_identity_service](https://hub.docker.com/r/uyilmaz/netshop_identity_service).