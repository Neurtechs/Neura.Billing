using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using static Neura.Billing.GlobalVar;

namespace Neura.Billing.DEHWData
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

        public static void ThermostatChangeStatus(string serialNo, DateTime timeStamp, double value)
        {
            MySqlCommand cmd = new MySqlCommand("ThermostatChangeStatus", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_serialNo", serialNo);
            cmd.Parameters.AddWithValue("_timeStamp", timeStamp);
            cmd.Parameters.AddWithValue("_value", value);

            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();

        }

        public static void ThermosStatSetStatus(string serialNo, DateTime timeStamp, double value,
            double heatingGrad, double coolingGrad)
        {
            MySqlCommand cmd = new MySqlCommand("ThermostatSetStatus", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_serialNo", serialNo);
            cmd.Parameters.AddWithValue("_timeStamp", timeStamp);
            cmd.Parameters.AddWithValue("_value", value);
            cmd.Parameters.AddWithValue("_heatingGrad", heatingGrad);
            cmd.Parameters.AddWithValue("_coolingGrad", coolingGrad);

            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();
        }

        public static void ReadThermostats(out DataTable dtThermostat)
        {
            string str = "Select * from DEHWStatus";
            MySqlDataAdapter da = new MySqlDataAdapter(str,mySqlConnection);
            dtThermostat=new DataTable();
            da.Fill(dtThermostat);
        }
    }
}
