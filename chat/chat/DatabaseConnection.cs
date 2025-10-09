using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat
{
    public static class DatabaseConnection
    {
        public static string ConnectionString { get; set; }
        public static void Initialize(string server, string database, string user, string password)
        {
            ConnectionString = $"server={server};database={database};uid={user};pwd={password};";
        }
    }
}
