using Microsoft.Data.SqlClient;

namespace ProductInventorySystem.Data
{
    public class DatabaseHelper
    {
        public static string ConnectionString =
            "Server=localhost;Database=DESKTOP-UTJOGUO;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}