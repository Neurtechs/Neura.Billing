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
            //string str = "Select * from DEHWStatus";
            //MySqlDataAdapter da = new MySqlDataAdapter(str,mySqlConnection);
            //dtThermostat=new DataTable();
            //da.Fill(dtThermostat);

            MySqlCommand cmd = new MySqlCommand("GetThermostatStatus", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            dtThermostat = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dtThermostat);
        }

        public static void SwitchStatusChange(string serialNo, DateTime timeStamp, int status)
        {
            MySqlCommand cmd = new MySqlCommand("SwitchStatusChange", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_serialNo", serialNo);
            cmd.Parameters.AddWithValue("_timeStamp", timeStamp);
            cmd.Parameters.AddWithValue("_status", status);
          
            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();
        }
        public static void UpdateHeatGrad (int _nodeId,  double _heatGrad)
        {
            MySqlCommand cmd = new MySqlCommand("UpdateHeatGrad", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_nodeId", _nodeId);
            cmd.Parameters.AddWithValue("_heatGrad", _heatGrad);
            
            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();
        }

        public static void UpdateCoolGrad(int _nodeId, double _coolGrad)
        {
            MySqlCommand cmd = new MySqlCommand("UpdateCoolGrad", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_nodeId", _nodeId);
            cmd.Parameters.AddWithValue("_coolGrad", _coolGrad);

            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();
        }
        public static void RetailerTariff(out DataTable dtRetailerTariff)
        {
            MySqlCommand cmd = new MySqlCommand("GetRetailerTariff", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            dtRetailerTariff=new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dtRetailerTariff);
        }

        public static void Retailers(out DataTable dtRetailers)
        {
            MySqlCommand cmd = new MySqlCommand("GetRetailers", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            dtRetailers = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dtRetailers);
        }

        public static void TariffsByRetailer(int _retailerOid, out DataTable dtTariffs)
        {
            MySqlCommand cmd = new MySqlCommand("GetTariffByRetailer", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_retailerOid", _retailerOid);
            dtTariffs = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dtTariffs);
        }

        public static int GetNodeBasicData(int _nodeId)
        {
            
            MySqlCommand cmd = new MySqlCommand("GetNodeStatus", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_nodeId",_nodeId);
            cmd.Parameters.Add("_status", MySqlDbType.Int16);
            cmd.Parameters["_status"].Direction = ParameterDirection.Output;
            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();
            return Convert.ToInt16(cmd.Parameters["_status"].Value);


        }

    }
}
