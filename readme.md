# IdentityService

Microservices Architecture (IdentityService) on .NET 3.1 with applying IdentityServer4, AspNetIdentity, Postgresql, InMemory

* Creating Migration For ConfigurationDbContext (Client, ApiScope, vs tables)
add-migration Inıtial -c ConfigurationDbContext
dotnet ef migrations add InitialConfigurationDbContext -c ConfigurationDbContext -o Data/Migrations/ConfigDbContext

* Creating Migration For PersistedGrantDbContext (DeviceCodes, PersistedGrants tables)
add-migration Inıtial -c PersistedGrantDbContext
dotnet ef migrations add InitialPersistedGrantDbContext -c PersistedGrantDbContext -o Data/Migrations/PersistGrantDbContext

* Creating Migration For ApplicationDbContext (AspNetRoles, AspNetUsers, vs tables)
add-migration Inıtial -c ApplicationDbContext
dotnet ef migrations add InitialApplicationDbContext -c ApplicationDbContext -o Data/Migrations/AppDbContext

* Creating DB Tables For ConfigurationDbContext By Migration
dotnet ef database update ConfigurationDbContextMigration -c ConfigurationDbContext

* Creating DB Tables For PersistedGrantDbContext By Migration
dotnet ef database update PersistedGrantDbContextMigration -c PersistedGrantDbContext

* Creating DB Tables For ApplicationDbContext By Migration
dotnet ef database update ApplicationDbContextMigration -c ApplicationDbContext



