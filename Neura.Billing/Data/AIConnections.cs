using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using static Neura.Billing.GlobalVar;

namespace Neura.Billing.Data
{
    public static class AIConnections
    {
        public static void GetNodesWithData(out DataTable dtNodesWithData)
        {


            MySqlCommand cmd = new MySqlCommand("GetNodesWithData", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

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
    }
}
