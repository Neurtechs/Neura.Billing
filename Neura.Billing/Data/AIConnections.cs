using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DevExpress.Charts.Native;
using MySql.Data.MySqlClient;
using static Neura.Billing.GlobalVar;

namespace Neura.Billing.Data
{
    public static class AIConnections
    {
        public static void GetNodesWithData(int enodesOnly, out DataTable dtNodesWithData)
        {
            // enodes only = 1, else 0

            MySqlCommand cmd = new MySqlCommand("GetNodesWithData", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_enodesonly", enodesOnly);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtNodesWithData = new DataTable();
            da.Fill(dtNodesWithData);

        }
        public static void GetLastPeriodValues(int myNodeId, DateTime myStart, out DataTable dtLastPeriodValues)
        {
            MySqlCommand cmd = new MySqlCommand("GetLastPeriodValues", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_NodeId", myNodeId);
            cmd.Parameters.AddWithValue("_maxDate", myStart);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            dtLastPeriodValues = new DataTable();
            da.Fill(dtLastPeriodValues);
        }

        public static void GetTemplate(out DataTable dtTemplate)
        {
            MySqlCommand cmd = new MySqlCommand("GetTemplate", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            dtTemplate = new DataTable();
            da.Fill(dtTemplate);
        }

        public static double GetSumTemplate(int month, int year)
        {
            MySqlCommand cmd = new MySqlCommand("GetSumTemplate", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_month", month);
            cmd.Parameters.AddWithValue("_year", year);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dtTemplate = new DataTable();
            da.Fill(dtTemplate);

            double consumption = Convert.ToDouble(dtTemplate.Rows[0]["sumConsumption"]);
            return consumption;
        }

        public static void GetMonthUAcc(int nodeId, DateTime monthStart, out DataTable dtMonthValues)
        {
            MySqlCommand cmd = new MySqlCommand("GetMonthUAcc", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_node", nodeId);
            cmd.Parameters.AddWithValue("_start", monthStart);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            dtMonthValues = new DataTable();
            da.Fill(dtMonthValues);

        }
        public static void GetNodeBasic(int nodeId, out int tariff, out int meterType, out int readingsType)
        {
            MySqlCommand cmd = new MySqlCommand("GetNodeReadingsType", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_node", nodeId);

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);

            tariff = Convert.ToInt32(dt.Rows[0]["Tariff"]);
            meterType = Convert.ToInt16(dt.Rows[0]["MeterType"]);
            readingsType = Convert.ToInt16(dt.Rows[0]["ReadingsType"]);
        }

        public static void UpdateForecast(int nodeId, double dayk, double dayc, double weekk, double weekc,
            double monthk, double monthc, double todateDk, double todateDc,
            double todateWk, double todateWc, double todateMk, double todateMc)
        {
            MySqlCommand cmd = new MySqlCommand("UpdateForecast", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_nodeId", nodeId);
            cmd.Parameters.AddWithValue("_weekk", weekk);
            cmd.Parameters.AddWithValue("_week$", weekc);
            cmd.Parameters.AddWithValue("_monthk", monthk);
            cmd.Parameters.AddWithValue("_month$", monthc);
            cmd.Parameters.AddWithValue("_dayk", dayk);
            cmd.Parameters.AddWithValue("_day$", dayc);
            cmd.Parameters.AddWithValue("_todateDk", todateDk);
            cmd.Parameters.AddWithValue("_todateD$", todateDc);
            cmd.Parameters.AddWithValue("_todateWk", todateWk);
            cmd.Parameters.AddWithValue("_todateW$", todateWc);
            cmd.Parameters.AddWithValue("_todateMk", todateMk);
            cmd.Parameters.AddWithValue("_todateM$", todateMc);
            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();
        }
    }
}
