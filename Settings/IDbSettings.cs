namespace NetShop.IdentityService.Settings
{
    public interface IDbSettings
    {
        public string DatabaseType { get; set; }
        
        public PostgresqlSettings PostgresqlSettings { get; set; }

         public SqliteSettings SqliteSettings { get; set; }
    }
}