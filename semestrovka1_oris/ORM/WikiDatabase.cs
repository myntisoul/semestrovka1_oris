using Microsoft.Data.SqlClient;

namespace ORM;

internal static class WikiDatabase
{
    public static OrmContext Connect()
    {
        var connection = new SqlConnectionStringBuilder
        {
            DataSource = Environment.GetEnvironmentVariable("ORIS_SQL_SERVER") ?? "localhost,1433",
            InitialCatalog = "OrisDb",
            UserID = "sa",
            Password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD") ?? "YourStrong!Passw0rd",
            Encrypt = true,
            TrustServerCertificate = true
        };
        return new OrmContext(connection.ConnectionString);
    }
}
