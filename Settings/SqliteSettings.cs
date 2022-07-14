namespace NetShop.IdentityService.Settings
{
    public class SqliteSettings
    {
        public string DataSource { get; set; }

        public string ConnectionString
        {
            get
            {
                return $"Data Source={DataSource};";
            }
        }
    }
}