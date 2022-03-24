// Client, api, vs db de tablolarını oluşturacak migration oluşturur
add-migration Inıtial -c ConfigurationDbContext
dotnet ef migrations add InitialConfigurationDbContext -c ConfigurationDbContext -o Data/Migrations/ConfigDbContext

/ PersistedGrant tablolarını oluşturacak migration oluşturur
add-migration Inıtial -c PersistedGrantDbContext
dotnet ef migrations add InitialPersistedGrantDbContext -c PersistedGrantDbContext -o Data/Migrations/PersistGrantDbContext

// Identity tablolarını vs db de kayıtlarını oluşturacak migration oluşturur
add-migration Inıtial -c ApplicationDbContext
dotnet ef migrations add InitialApplicationDbContext -c ApplicationDbContext -o Data/Migrations/AppDbContext

// Client, api, vs db de tablolarını oluşturacak migration db de calıştırır
dotnet ef database update ConfigurationDbContextMigration -c ConfigurationDbContext

// PersistedGrant tablolarını oluşturacak migration db de calıştırır
dotnet ef database update PersistedGrantDbContextMigration -c PersistedGrantDbContext

// Identity tablolarını oluşturacak migration db de calıştırır
dotnet ef database update ApplicationDbContextMigration -c ApplicationDbContext



