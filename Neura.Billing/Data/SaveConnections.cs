using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using static Neura.Billing.GlobalVar;
using Neura.Billing.TariffCalcs;


namespace Neura.Billing.Data
{
    public class SaveConnections
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Save Intermediate values
        /// </summary>
        /// <param name="NodeId"></param>
        /// <param name="reading"></param>
        /// <param name="tempDate"></param>
        /// <param name="timeStamp"></param>
        public static void SaveIntermediateReadings(int NodeId, double reading, DateTime tempDate, int readingsType, int meterType,
           int status = 0, DateTime? timeStamp = null)
        {
            //string sql = "Insert into ReadingIntermediate (NodeId,Reading,TimeStamp,tempDate,ReadingsType,Status,MeterType) " +
            //    "Values (" + NodeId + "," + reading + ",'" + timeStamp + "','" + tempDate + "'," + readingsType + "," + status + "," + meterType + ")";

            MySqlCommand cmd = new MySqlCommand("SaveIntermediateReadings", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_NodeId", NodeId);
            cmd.Parameters.AddWithValue("_reading", reading);
            cmd.Parameters.AddWithValue("_timeStamp", timeStamp);
            cmd.Parameters.AddWithValue("_tempDate", tempDate);
            cmd.Parameters.AddWithValue("_readingsType", readingsType);
            cmd.Parameters.AddWithValue("_status", status);
            cmd.Parameters.AddWithValue("_meterType", meterType);
            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();



            if (timeStamp == null)
            {
                //sql = "Insert into ReadingIntermediate (NodeId,Reading,tempDate,ReadingsType,Status,MeterType) " +
                //"Values (" + NodeId + "," + reading + ",'"  + tempDate + "'," + readingsType + "," + status + "," + meterType + ")";
                cmd = new MySqlCommand("SaveIntermediateReadingsNull", mySqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_NodeId", NodeId);
                cmd.Parameters.AddWithValue("_reading", reading);
                //cmd.Parameters.AddWithValue("_timeStamp", timeStamp);
                cmd.Parameters.AddWithValue("_tempDate", tempDate);
                cmd.Parameters.AddWithValue("_readingsType", readingsType);
                cmd.Parameters.AddWithValue("_status", status);
                cmd.Parameters.AddWithValue("_meterType", meterType);
                if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
                cmd.ExecuteNonQuery();
                mySqlConnection.Close();

            }
            //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
            //cmd.ExecuteNonQuery();

        }
        /// <summary>
        /// Update readingtable comment
        /// </summary>
        /// <param name="myRecordId"></param>
        /// <param name="myDateReceived"></param>
        /// <param name="comment"></param>
        public static void UpdateReading(Int32 myRecordId, int comment)
        {

            ////string sql = "Update Reading Set comment = " + comment + " " +
            ////             "Where Oid =" + myRecordId;
            //string sql = "Update Readings1 Set comment = " + comment + " " +
            //             "Where Oid =" + myRecordId;
            //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
            //cmd.ExecuteNonQuery();

            MySqlCommand cmd = new MySqlCommand("UpdateReading", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_myRecordId", myRecordId);
            cmd.Parameters.AddWithValue("_comment", comment);

            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();

        }
        public static void UpdateIntermediateStats(Int32 myRecordId)
        {
            //string sql = "Update ReadingIntermediate Set Status = 2 Where Oid= " + myRecordId;
            //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
            //cmd.ExecuteNonQuery();

            MySqlCommand cmd = new MySqlCommand("UpdateIntermediateStats", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_myRecordId", myRecordId);

            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();
        }

        public static void DeleteIntermediate(int myNodeId, int myReadingType, DateTime dateTime)
        {
            //string sql = "Delete from ReadingIntermediate WHERE NodeId = " + myNodeId + 
            //    " AND ReadingsType= " + myReadingType + " AND Status = 3";
            //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
            //cmd.ExecuteNonQuery();

            MySqlCommand cmd = new MySqlCommand("DeleteIntermediate", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_myNodeId", myNodeId);
            cmd.Parameters.AddWithValue("_myReadingType", myReadingType);
            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();


        }

        public static void SaveUsage(int myReadingsType, double myUsage,
            DateTime myDateReceived, int myNodeId, double max, int myStatus = 0)
        {

            //string sql = "Insert into ServiceUsage (ReadingsType,Consumed,DateReceived,Status,Node, Max) " +
            //    "Values (" + myReadingsType + "," + myUsage + ",'" + myDateReceived + "'," +
            //    myStatus + "," + myNodeId + "," + max + ")";
            //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
            //cmd.ExecuteNonQuery();

            MySqlCommand cmd = new MySqlCommand("SaveUsage", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_myNodeId", myNodeId);
            cmd.Parameters.AddWithValue("_myReadingsType", myReadingsType);
            cmd.Parameters.AddWithValue("_myUsage", myUsage);
            cmd.Parameters.AddWithValue("_myDateReceived", myDateReceived);
            cmd.Parameters.AddWithValue("_myStatus", myStatus);
            cmd.Parameters.AddWithValue("_max", max);

            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();

        }
        public static void SavePeriodValues(int node, DateTime DateReceived, double cFixed, double cMaximum,
            double cAcc, double uMaximum, double uAcc, int customer)
        {
            //string sql = " Insert into PeriodValues (DateReceived, Node, cFixed, cMaximum, cAcc,  uMaximum, uAcc, Customer) " +
            //       "values ('" + DateReceived + "'," + node + "," + cFixed + "," + cMaximum + "," + cAcc + "," 
            //       + uMaximum + "" +
            //       "," + uAcc + "," + customer + ")";
            //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
            //cmd.ExecuteNonQuery();

            MySqlCommand cmd = new MySqlCommand("SavePeriodValues", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_node", node);
            cmd.Parameters.AddWithValue("_cFixed", cFixed);
            cmd.Parameters.AddWithValue("_cMaximum", cMaximum);
            cmd.Parameters.AddWithValue("_DateReceived", DateReceived);
            cmd.Parameters.AddWithValue("_cAcc", cAcc);
            cmd.Parameters.AddWithValue("_uMaximum", uMaximum);
            cmd.Parameters.AddWithValue("_uAcc", uAcc);
            cmd.Parameters.AddWithValue("_customer", customer);

            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();

        }

        public static void SaveCustomerValues(DateTime DateReceived, double Cost, int customerId)
        {
            //string sql = "Insert into CustomerValues (DateReceived, Cost, Customer) " +
            //    "values ('" + DateReceived + "', " + Cost + "," + customerId + ")";
            //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
            //cmd.ExecuteNonQuery();

            MySqlCommand cmd = new MySqlCommand("SaveCustomerValues", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_Cost", Cost);
            cmd.Parameters.AddWithValue("_DateReceived", DateReceived);
            cmd.Parameters.AddWithValue("_customerId", customerId);

            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();
        }

        public static void SaveTouValues(int NodeId, int Interval, DateTime DateReceived, DateTime MonthStart,
            DateTime MonthEnd, double cAccToDate, double uAccToDate)
        {
            //string sql = "Select * from TouValues WHERE  ((MonthStart <= '" + MonthStart +
            //            "') AND (MonthEnd >= '" + MonthEnd + "') AND (NodeId = " + NodeId +
            //            ") AND (iInterval = " + Interval + "))";
            //MySqlDataAdapter da = new MySqlDataAdapter(sql,mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("SelectTOUValues", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_MonthStart", MonthStart);
            cmd.Parameters.AddWithValue("_MonthEnd", MonthEnd);
            cmd.Parameters.AddWithValue("_NodeId", NodeId);
            cmd.Parameters.AddWithValue("_Interval", Interval);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows.Count;
            if (count > 0)
            {
                //Existing record
                int Oid = Convert.ToInt16(dt.Rows[0]["Oid"]);
                //sql = "Update TouValues Set DateReceived = '" + DateReceived + "', " +
                //    "cAcc = " + cAccToDate + ", uAcc = " + uAccToDate +
                //    " Where Oid = " + Oid;
                //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
                //cmd.ExecuteNonQuery();
                cmd = new MySqlCommand("UpdateTOUValues", mySqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_DateReceived", DateReceived);
                cmd.Parameters.AddWithValue("_cAccToDate", cAccToDate);
                cmd.Parameters.AddWithValue("_uAccToDate", uAccToDate);
                cmd.Parameters.AddWithValue("_Oid", Oid);



                if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
                cmd.ExecuteNonQuery();
                mySqlConnection.Close();
            }
            else
            {
                //new record
                //sql = " Insert into TouValues (DateReceived, NodeId, cAcc, uAcc, MonthStart, MonthEnd, iInterval) " +
                //   "values ('" + DateReceived + "'," + NodeId + "," + cAccToDate + "," + uAccToDate + ",'" + MonthStart +
                //   "','" + MonthEnd + "',"  + Interval + ")";
                //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);

                cmd = new MySqlCommand("InsertTOUValues", mySqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_DateReceived", DateReceived);
                cmd.Parameters.AddWithValue("_cAccToDate", cAccToDate);
                cmd.Parameters.AddWithValue("_uAccToDate", uAccToDate);
                cmd.Parameters.AddWithValue("_Interval", Interval);
                cmd.Parameters.AddWithValue("_MonthStart", MonthStart);
                cmd.Parameters.AddWithValue("_MonthEnd", MonthEnd);
                cmd.Parameters.AddWithValue("_NodeId", NodeId);

                if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
                cmd.ExecuteNonQuery();
                mySqlConnection.Close();

            }
        }
        public static void SaveBlockValues(int NodeId, int Floor, int Ceiling, DateTime ReadingDate,
            DateTime MonthStart, DateTime MonthEnd, double cAcc, double uAcc)
        {
            //string sql = "Select * from BlockValues WHERE  MonthStart <= '" + MonthStart +
            //            "' AND MonthEnd >= '" + MonthEnd + "' AND NodeId = " + NodeId + 
            //            " AND Floor = " + Floor;
            //MySqlDataAdapter da = new MySqlDataAdapter(sql, mySqlConnection);


            MySqlCommand cmd = new MySqlCommand("SelectBlockValues", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_NodeId", NodeId);
            cmd.Parameters.AddWithValue("_Floor", Floor);
            cmd.Parameters.AddWithValue("_MonthEnd", MonthEnd);
            cmd.Parameters.AddWithValue("_MonthStart", MonthStart);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            DataTable dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows.Count;
            if (count > 0)
            {
                //Existing record
                int Oid = Convert.ToInt16(dt.Rows[0]["Oid"]);

                //sql = "Update BlockValues Set DateReceived = '" + ReadingDate + "', " +
                //    "cAcc = " + cAcc + ", uAcc = " + uAcc +
                //    " Where Oid = " + Oid;
                //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
                //cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("UpdateBlockValues", mySqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_ReadingDate", ReadingDate);
                cmd.Parameters.AddWithValue("_cAcc", cAcc);
                cmd.Parameters.AddWithValue("_uAcc", uAcc);

                //cmd.Parameters.AddWithValue("_MonthStart", MonthStart);
                //cmd.Parameters.AddWithValue("_MonthEnd", MonthEnd);
                cmd.Parameters.AddWithValue("_Oid", Oid);

                if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
                cmd.ExecuteNonQuery();
                mySqlConnection.Close();

            }
            else
            {
                //sql = " Insert into BlockValues (DateReceived, NodeId, cAcc, uAcc, MonthStart, MonthEnd, Floor, Ceiling) " +
                //    "values ('" + ReadingDate + "'," + NodeId + ","  + cAcc + "," + uAcc + ",'" + MonthStart + 
                //    "','" + MonthEnd + "'," + Floor + "," + Ceiling + ")";
                //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
                //cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("InsertBlockValues", mySqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_ReadingDate", ReadingDate);
                cmd.Parameters.AddWithValue("_cAcc", cAcc);
                cmd.Parameters.AddWithValue("_uAcc", uAcc);
                cmd.Parameters.AddWithValue("_MonthStart", MonthStart);
                cmd.Parameters.AddWithValue("_MonthEnd", MonthEnd);
                cmd.Parameters.AddWithValue("_NodeId", NodeId);
                cmd.Parameters.AddWithValue("_Floor", Floor);
                cmd.Parameters.AddWithValue("_Ceiling", Ceiling);
                if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
                cmd.ExecuteNonQuery();
                mySqlConnection.Close();
            }
        }
        public static void SaveMonthValues(int node, DateTime DateReceived, double cFixed, double cMaximum,
            double cAcc, double uMaximum, double uAcc, int Customer, double uPeak)
        {
            MonthStartEnd.GetMonthStartEnd(DateReceived, out DateTime monthStart, out DateTime monthEnd);

            //string sql = "Select * from MonthValues WHERE  Start <= '" + monthStart +
            //             "' AND End >= '" + monthEnd + "' AND Node = " + node;
            //MySqlDataAdapter da = new MySqlDataAdapter(sql, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("SelectMonthValues", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_node", node);
            cmd.Parameters.AddWithValue("_monthEnd", monthEnd);
            cmd.Parameters.AddWithValue("_monthStart", monthStart);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            DataTable dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows.Count;




            if (count > 0)
            {
                //Existing record
                int Oid = Convert.ToInt16(dt.Rows[0]["Oid"]);
                //sql = "Update MonthValues Set DateReceived = '" + DateReceived + 
                //    "', cFixed = " + cFixed + ", cMaximum = " + cMaximum + ", " +
                //    "cAcc = " + cAcc + ", uMaximum = " + uMaximum + ", uAcc = " + uAcc + 
                //    ", Customer = " + Customer + ", uPeak = " + uPeak +
                //    " Where Oid = " + Oid;
                //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
                //cmd.ExecuteNonQuery();
                cmd = new MySqlCommand("UpdateMonthValues", mySqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_DateReceived", DateReceived);
                cmd.Parameters.AddWithValue("_cAcc", cAcc);
                cmd.Parameters.AddWithValue("_uAcc", uAcc);
                cmd.Parameters.AddWithValue("_uPeak", uPeak);
                cmd.Parameters.AddWithValue("_cFixed", cFixed);
                cmd.Parameters.AddWithValue("_cMaximum", cMaximum);
                cmd.Parameters.AddWithValue("_Oid", Oid);
                cmd.Parameters.AddWithValue("_uMaximum", uMaximum);
                cmd.Parameters.AddWithValue("_Customer", Customer);

                if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
                cmd.ExecuteNonQuery();
                mySqlConnection.Close();

            }
            else
            {
                //sql = " Insert into MonthValues (DateReceived, Node, cFixed, cMaximum, cAcc,  uMaximum, uAcc, Start, End, Customer, uPeak) " +
                //    "values ('" + DateReceived + "'," + node + "," + cFixed + "," + cMaximum + "," + cAcc +  "," + uMaximum + "" +
                //    "," + uAcc + ",'" + monthStart + "','" + monthEnd + "'," + Customer + "," + uPeak + ")";
                //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
                //cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("InsertMonthValues", mySqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_DateReceived", DateReceived);
                cmd.Parameters.AddWithValue("_cAcc", cAcc);
                cmd.Parameters.AddWithValue("_uAcc", uAcc);
                cmd.Parameters.AddWithValue("_uPeak", uPeak);
                cmd.Parameters.AddWithValue("_cFixed", cFixed);
                cmd.Parameters.AddWithValue("_cMaximum", cMaximum);
                cmd.Parameters.AddWithValue("_node", node);
                cmd.Parameters.AddWithValue("_uMaximum", uMaximum);
                cmd.Parameters.AddWithValue("_Customer", Customer);
                cmd.Parameters.AddWithValue("_monthStart", monthStart);
                cmd.Parameters.AddWithValue("_monthEnd", monthEnd);

                if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
                cmd.ExecuteNonQuery();
                mySqlConnection.Close();
            }


        }
        public static void UpdateServiceUsageStatus(int Node, DateTime DateReceived)
        {
            //string str = "Update ServiceUsage Set Status = 1 WHERE DateReceived = '" + DateReceived + "' " +
            //    "AND Node = " + Node;
            //MySqlCommand cmd = new MySqlCommand(str, mySqlConnection);
            //cmd.ExecuteNonQuery();

            MySqlCommand cmd = new MySqlCommand("UpdateServiceUsageStatus", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_DateReceived", DateReceived);
            cmd.Parameters.AddWithValue("_Node", Node);

            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();
        }
        public static void SaveLookUp(int touLookUpId, string hour, int category, int season,
            int dayOfWeek)
        {
            //string sql = "Insert into LookupTou (TouLookupID, Time, Category, Season, DayOfWeek) " +
            //    "Values ('" + touLookUpId + "','" + hour + "'," + category + ","
            //    + season + "," + dayOfWeek + ")";
            //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
            //cmd.ExecuteNonQuery();

            MySqlCommand cmd = new MySqlCommand("SaveLookUp", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_touLookUpId", touLookUpId);
            cmd.Parameters.AddWithValue("_hour", hour);
            cmd.Parameters.AddWithValue("_category", category);
            cmd.Parameters.AddWithValue("_season", season);
            cmd.Parameters.AddWithValue("_dayOfWeek", dayOfWeek);

            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();
        }

        public static void SavePeriodInMonth(int TouLookupId,
            DateTime myDate, int countPeak,
            int countStandard, int countOffPeak, int countPeakStandard)
        {
            //string sql = "Insert into PeriodInMonth (TouLookupId, Month, Peak, " +
            //             "Standard, OffPeak, PeakStandard) " +
            //             "Values ("  + TouLookupId + ",'" + myDate + "'," +
            //             countPeak + "," + countStandard + ","
            //             + countOffPeak + "," + countPeakStandard + ")";

            //MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
            //cmd.ExecuteNonQuery();

            MySqlCommand cmd = new MySqlCommand("SavePeriodInMonth", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_TouLookupId", TouLookupId);
            cmd.Parameters.AddWithValue("_myDate", myDate);
            cmd.Parameters.AddWithValue("_countPeak", countPeak);
            cmd.Parameters.AddWithValue("_countOffPeak", countOffPeak);
            cmd.Parameters.AddWithValue("_countStandard", countStandard);
            cmd.Parameters.AddWithValue("_countPeakStandard", countPeakStandard);

            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();
        }
    }
}
