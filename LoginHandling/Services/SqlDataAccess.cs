namespace LoginHandling.Services
{
    public class SqlDataAccess : IDataAccess
    {
        private readonly string _connectionString;

        public SqlDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("azuredb");
        }
    }
}
