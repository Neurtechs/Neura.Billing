using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Neura.Billing
{
    class GlobalVar
    {
        //public static string ConnectionString { get; set; }
        public static MySqlConnection mySqlConnection { get; set; }
        public static bool bLogTest { get; set; }
        public static bool bLogResult { get; set; }
        public static int MeteringInterval { get; set; }
    }
}
