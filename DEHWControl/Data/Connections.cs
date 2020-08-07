using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using static Neura.Billing.GlobalVar;

namespace DEHWControl.Data
{
    public static class Connections
    {
        public static void GetGeyserNodes(out DataTable dtGeyserNodes)
        {
            MySqlCommand cmd = new MySqlCommand("GetGeyserNodes", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtGeyserNodes = new DataTable();
            da.Fill(dtGeyserNodes);
        }
    }
}
