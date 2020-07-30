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
    public static class UtilityConnections
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
           (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void SelectTOULookupLuT(int touLookUpId, int season, out DataTable dtTOULookup)
        {
            //string str = "Select * from vtoulookup " +
            //     "WHERE(( Category < 3 Or Category = 4) AND (TouSeasonName = " + season + ") AND " +
            //            "(TouLookupId = " + touLookUpId + ")) Order by DayOfWeek, TimeStart";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("SelectTOULookupLuT", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_season", season);
            cmd.Parameters.AddWithValue("_touLookUpId", touLookUpId);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtTOULookup = new DataTable();
            da.Fill(dtTOULookup);

        }
        public static void SelectLookupByIdSeason(int touLookUpId, int season, out DataTable dtTOULookupIdSeaon)
        {
            //string str = "Select TouLookupId, Time, Category, Season " +
            //             "From LookupTou " +
            //            "Where (TouLookupId ='" + touLookUpId + "' AND Season= " + season + ")";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("SelectLookupByIdSeason", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_season", season);
            cmd.Parameters.AddWithValue("_touLookUpId", touLookUpId);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtTOULookupIdSeaon = new DataTable();

            da.Fill(dtTOULookupIdSeaon);
        }

        public static void FindTouCategory(int TouLookupId, int Season, int DayOfWeek, string Time,
            out DataTable dtCategory)
        {
            //string str = "Select Category from LookupTOU WHERE TouLookupId = " + TouLookupId +
            //    " AND Season = " + Season + " AND DayOfWeek = " + DayOfWeek + " AND Time = '" + Time + "'";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("FindTouCategory", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_Season", Season);
            cmd.Parameters.AddWithValue("_TouLookupId", TouLookupId);
            cmd.Parameters.AddWithValue("_DayOfWeek", DayOfWeek);
            cmd.Parameters.AddWithValue("_Time", Time);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtCategory = new DataTable();
            da.Fill(dtCategory);


        }

        public static DateTime FindlLastReading(int NodeId, int ReadingsType)
        {
            //string sql = "Select NodeId, max(TempDate) as MyDate from ReadingIntermediate " +
            //             "Where NodeId = '" + NodeId + "' AND ReadingsType = " + ReadingsType + " Group by NodeId";
            ////string sql = "Select MeterNo, max(DateReceived) from Reading " +
            ////             "Where MeterNo = '" + meterNo + "' Group by MeterNo";

            //MySqlDataAdapter da = new MySqlDataAdapter(sql, mySqlConnection);
            MySqlCommand cmd = new MySqlCommand("FindlLastReading", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_NodeId", NodeId);
            cmd.Parameters.AddWithValue("_ReadingsType", ReadingsType);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            DataTable dt = new DataTable();

            da.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                //Can't find a previous reading
                return Convert.ToDateTime("1980/01/01");
            }
            DateTime myLast = Convert.ToDateTime(dt.Rows[0]["MyDate"]);
            return myLast;
        }

        public static void GetStartReading(int NodeId, int readingsType, out double StartReading)
        {


            //string sql = "Select * from vnode_readingtype where NodeId= " + NodeId + " AND ReadingsType = " + readingsType ;
            //MySqlDataAdapter da = new MySqlDataAdapter(sql, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("GetStartReading", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_nodeId", NodeId);
            cmd.Parameters.AddWithValue("_readingsType", readingsType);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);
            StartReading = Convert.ToDouble(dt.Rows[0]["StartValue"]);

        }



        public static void SelectIntermediateByReadingDate(int NodeId, DateTime readingDate, int ReadingsType,
            out DataTable dtSelectReading)


        {
            //string str = "Select * from ReadingIntermediate  Where NodeId = " + NodeId + " AND ReadingsType = " + ReadingsType +
            //             " AND TempDate = '" + readingDate + "' Order by NodeId, TempDate, ReadingsType";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("SelectIntermediateByReadingDate", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_NodeId", NodeId);
            cmd.Parameters.AddWithValue("_ReadingsType", ReadingsType);
            cmd.Parameters.AddWithValue("_readingDate", readingDate);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            dtSelectReading = new DataTable();
            da.Fill(dtSelectReading);


        }

        public static int CountReadingType(int NodeId)
        {
            //string str = "Select Node, ReadingsType from ReadingType Where Node = " + NodeId;
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            //MySqlCommand cmd = new MySqlCommand("CountReadingType", mySqlConnection);
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("_Node", NodeId);
            //MySqlDataAdapter da = new MySqlDataAdapter();
            //da.SelectCommand = cmd;

            //DataTable dt = new DataTable();
            //da.Fill(dt);
            //int count = dt.Rows.Count;
            //return count;

            MySqlCommand cmd = new MySqlCommand("CountReadingType", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_Node", NodeId);
            cmd.Parameters.Add("_count", MySqlDbType.Int16);
            cmd.Parameters["_count"].Direction = ParameterDirection.Output;
            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();
            return Convert.ToInt16(cmd.Parameters["_count"].Value);

        }

        public static void SelectReadingByReadingDate(int Nodeid, int ReadingType, DateTime readingDate, out double myPreviousReading)
        //public static void SelectReadingByReadingDate(int Nodeid, int ReadingType, DateTime readingDate, out DataTable dtPrevIntermediate)
        {
            //string str = "Select * from ReadingIntermediate  Where NodeId = " + Nodeid +
            //             " AND ReadingsType = " + ReadingType + " AND TempDate = '" + readingDate + "' Order by NodeId, TempDate, ReadingsType";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            //MySqlCommand cmd = new MySqlCommand("SelectReadingByReadingDate", mySqlConnection);
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("_NodeId", Nodeid);
            //cmd.Parameters.AddWithValue("_ReadingsType", ReadingType);
            //cmd.Parameters.AddWithValue("_readingDate", readingDate);
            //MySqlDataAdapter da = new MySqlDataAdapter();
            //da.SelectCommand = cmd;
            //dtPrevIntermediate = new DataTable();
            //da.Fill(dtPrevIntermediate);

            MySqlCommand cmd = new MySqlCommand("SelectReadingByReadingDate", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_NodeId", Nodeid);
            cmd.Parameters.AddWithValue("_ReadingsType", ReadingType);
            cmd.Parameters.AddWithValue("_readingDate", readingDate);
            cmd.Parameters.Add("_reading", MySqlDbType.Double);
            cmd.Parameters["_reading"].Direction = ParameterDirection.Output;
            if (mySqlConnection.State == ConnectionState.Closed) { mySqlConnection.Open(); }
            cmd.ExecuteNonQuery();
            mySqlConnection.Close();
            if (cmd.Parameters["_reading"].Value != System.DBNull.Value) { myPreviousReading = Convert.ToDouble(cmd.Parameters["_reading"].Value); }
            else { myPreviousReading = 0; }


        }

        public static int SelectUsageByNodeTariff(int nodeId, int tariffId, DateTime DateReceived, out DataTable dtUsage)
        {
            //string str = "Select * from vserviceusage_tariff Where (Node= " + nodeId + 
            //    " AND TariffId= " + tariffId + " And Status =0 " +
            //    "AND DateReceived = '" + DateReceived + "') Order by Node, DateReceived, ReadingsType";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("SelectUsageByNodeTariff", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_nodeId", nodeId);
            cmd.Parameters.AddWithValue("_tariffId", tariffId);
            cmd.Parameters.AddWithValue("_DateReceived", DateReceived);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtUsage = new DataTable();
            da.Fill(dtUsage);
            int count = dtUsage.Rows.Count;
            return count;
        }

        public static int SelectTariffComponents(int tariffId, DateTime myReadingDate, out DataTable dtTComponents)
        {
            //string str = "Select * from vtariff_tariffcomponent_rate " +
            //    "Where TariffOid = '" + tariffId + "' AND Start < '" + myReadingDate +
            //             "' AND End >= '" + myReadingDate + "' " +
            //            " Order by TariffOid ASC, Floor ASC"; 

            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("SelectTariffComponents", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("_tariffId", tariffId);
            cmd.Parameters.AddWithValue("_myReadingDate", myReadingDate);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtTComponents = new DataTable();
            da.Fill(dtTComponents);
            int count = dtTComponents.Rows.Count;
            return count;
        }

        public static int GetPreviousValues(int NodeId, DateTime receivedDate, out DataTable dtPrev)
        {
            //string str=  "Select * from MonthValues WHERE Node = " + NodeId + " AND DateReceived = '" + receivedDate + "'";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("GetPreviousValues", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("_NodeId", NodeId);
            cmd.Parameters.AddWithValue("_receivedDate", receivedDate);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtPrev = new DataTable();
            da.Fill(dtPrev);
            int count = dtPrev.Rows.Count;
            return count;
        }

        public static int GetPreviousTou(int NodeId, DateTime DateReceived, int myInterval, out DataTable dtPrev)
        {
            //string str = "Select * from TouValues WHERE NodeId = " + NodeId + " AND DateReceived = '" + DateReceived + "' AND iInterval = " + myInterval;
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("GetPreviousTou", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("_NodeId", NodeId);
            cmd.Parameters.AddWithValue("_DateReceived", DateReceived);
            cmd.Parameters.AddWithValue("_myInterval", myInterval);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtPrev = new DataTable();
            da.Fill(dtPrev);
            int count = dtPrev.Rows.Count;
            return count;
        }
        public static int GetGroupedUsage(out DataTable dtGrouped)
        {
            //string str = "Select * from vserviceusage_grouped Order by node, datereceived"; //Status=0
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("GetGroupedUsage", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtGrouped = new DataTable();
            da.Fill(dtGrouped);
            int count = dtGrouped.Rows.Count;
            return count;
        }

        public static int GetBlockComponents(int tariffId, DateTime myReadingDate, out DataTable dtBlocks)
        {

            //string str = "Select * from vtariff_tariffcomponent_block WHERE TariffOid = " + tariffId + " AND Start <= '" + myReadingDate +
            //             "' AND End >= '" + myReadingDate + "' Order  by Floor DESC" ;
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("GetBlockComponents", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("_tariffId", tariffId);
            cmd.Parameters.AddWithValue("_myReadingDate", myReadingDate);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            dtBlocks = new DataTable();
            da.Fill(dtBlocks);
            int count = dtBlocks.Rows.Count;
            return count;
        }

        public static int GetCustomerFromNode(int NodeId, out DataTable dtCustomerNode)
        {
            //string str = "Select * from customercustomergatewaynode Where NodeID=" + NodeId;
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("GetCustomerFromNode", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("_NodeId", NodeId);

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtCustomerNode = new DataTable();
            da.Fill(dtCustomerNode);
            int count = dtCustomerNode.Rows.Count;
            return count;
        }
        public static int SelectTouGroups(out DataTable dtTouGroups)
        {
            //string str = "Select TouLookUpId, TouSeasonName From vtoulookup Group by TouLookUpId, TouSeasonName";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("SelectTouGroups", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            dtTouGroups = new DataTable();
            da.Fill(dtTouGroups);
            int count = dtTouGroups.Rows.Count;
            return count;
        }

        public static int SelectPeriodInMonthPiM(DateTime myDate, int TouLookupId, out DataTable dtPeriodInMonth)
        {

            //string str = "Select * from PeriodInMonth " +
            //             "Where TOULookupId = " + TouLookupId + " And Month = '" + myDate + "'";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("SelectPeriodInMonthPiM", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("_myDate", myDate);
            cmd.Parameters.AddWithValue("_TouLookupId", TouLookupId);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            dtPeriodInMonth = new DataTable();
            da.Fill(dtPeriodInMonth);
            int count = dtPeriodInMonth.Rows.Count;
            return count;
        }
        public static int SelectTOULookupS(int touLookupId, out DataTable dtTOULookup)
        {
            //string str = "Select * from vtoulookup Where TouLookupId = " + touLookupId +
            //    " order by TouLookupId";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("SelectTOULookupS", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("_touLookupId", touLookupId);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            dtTOULookup = new DataTable();
            da.Fill(dtTOULookup);
            int count = dtTOULookup.Rows.Count;
            return count;

        }
        public static void SelectLookupTou(out DataTable dtLookupTou)
        {
            //string str = "Select * from LookupTou";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("SelectLookupTou", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            dtLookupTou = new DataTable();
            da.Fill(dtLookupTou);
        }

        public static void GetPeriodsInMonth(int TouLookupId, DateTime Month,
            out DataTable dtPeriodsInMonth)
        {
            string str = "Select * from PeriodInMonth WHERE TOULookupId = " + TouLookupId +
                " AND Month = '" + Month + "'";
            dtPeriodsInMonth = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);
            da.Fill(dtPeriodsInMonth);
        }

        public static int SelectTOULookup(int tariffId)
        {
            //string str = "Select TOULookup from Tariff Where Oid = " + tariffId;
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);


            MySqlCommand cmd = new MySqlCommand("SelectTOULookup", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("_tariffId", tariffId);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();

            da.Fill(dt);
            return Convert.ToInt16(dt.Rows[0]["TOULookup"]);
        }
        public static void SelectTOULookupTR(int myLookupId, int mySeason, int myInterval,
           string time, string date, DayOfWeek dow, out DataTable dtTOULookup)
        {
            string filter = "Where  Category= " + myInterval + " AND TouLookup.Oid = " +
                            myLookupId + " And TouSeasonName = " + mySeason + " And '" +
                            time + "' >TimeStart AND '" + time + "' <= TimeEnd AND ";

            //string str = "Select * from PublicHolidays Where DateHoliday = '" + date + "'";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("SelectPublicHoliday", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("_date", date);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;


            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                filter = filter + "DayofWeek = " + dt.Rows[0]["DayofWeek"];
                if (bLogTest == true)
                {
                    Log.Info("----- Date: " + date + " is public holiday ------");
                    Log.Info("DayOfWeek: " + dt.Rows[0]["DayofWeek"]);
                }
            }

            else if (dow == DayOfWeek.Sunday)
            {

                filter = filter + "DayofWeek = 1";
            }
            else if (dow == DayOfWeek.Saturday)
            {
                filter = filter + "DayofWeek = 7";
            }
            else
            {
                filter = filter + "DayofWeek = 0";
            }


            string str =
               "SELECT TouLookup.Publisher, TouSeason.TouSeasonName, TouDetail.Category, " +
               "TouLookup.Oid AS TouLookupId, " +
               "TouDetail.DayOfWeek, TouDetail.TimeStart, TouDetail.TimeEnd, TouSeason.MonthStart, " +
               "TouSeason.MonthEnd " +
               "FROM TouLookup INNER JOIN " +
               "TouSeason ON TouLookup.Oid = TouSeason.TouLookup INNER JOIN " +
               "TouDetail ON TouSeason.Oid = TouDetail.TouSeason " +
               filter +
               " Order by MonthStart";

            da = new MySqlDataAdapter(str, mySqlConnection);

            dtTOULookup = new DataTable();

            da.Fill(dtTOULookup);
        }
    }
}
