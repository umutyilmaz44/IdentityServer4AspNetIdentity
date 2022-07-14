namespace NetShop.IdentityService.Settings
{
    public class PostgresqlSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string Database { get; set; }

        public string ConnectionString
        {
            get
            {
                return $"Host={Host};Port={Port};Username={Username};Password={Password};Database={Database};";
            }
        }
    }
}