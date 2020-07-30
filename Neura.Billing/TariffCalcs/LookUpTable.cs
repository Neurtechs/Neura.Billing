using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neura.Billing.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using static Neura.Billing.GlobalVar;

namespace Neura.Billing.TariffCalcs
{
    public class LookUpTable
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void GenerateLookUp(int touLookUpId, int season, int myMeteringInterval)
        {
            //Check if data available for this lookup and season
            UtilityConnections.SelectLookupByIdSeason(touLookUpId, season,
                out DataTable dtTOULookupIdSeaon);
            int countExisting = dtTOULookupIdSeaon.Rows.Count;
            if (countExisting > 0) { return; }

            //Get the TOU Data
            UtilityConnections.SelectTOULookupLuT(touLookUpId, season, out DataTable dtTOULookup);
            string filter = "";
            string sort = "DayOfWeek, TimeStart";
            DataRow[] dr = dtTOULookup.Select(filter, sort);


            //Create a datatable and add the data
            DataTable myTable = new DataTable("myTable", "Neura.Billing2.TariffCalcs");
            myTable.Columns.Add("touLookUpID", Type.GetType("System.Int16"));
            myTable.Columns.Add("timeStart", Type.GetType("System.String"));
            myTable.Columns.Add("timeEnd", Type.GetType("System.String"));
            myTable.Columns.Add("DayOfWeek", Type.GetType("System.Int16"));
            myTable.Columns.Add("Category", Type.GetType("System.Int16"));
            myTable.Columns.Add("season", Type.GetType("System.Int16"));

            DataRow newRow;
            if (dr.Length == 0) { return; }
            for (int i = 0; i < dr.Length; i++)
            {
                newRow = myTable.NewRow();
                newRow["TouLookupId"] = dr[i]["TouLookUpId"].ToString();
                newRow["timeStart"] = dr[i]["timeStart"].ToString();
                newRow["timeEnd"] = dr[i]["timeEnd"].ToString();
                newRow["DayOfWeek"] = dr[i]["DayOfWeek"].ToString();
                newRow["category"] = dr[i]["category"].ToString();
                newRow["season"] = dr[i]["TOUSeasonName"].ToString();
                myTable.Rows.Add(newRow);
                if (bLogTest == true)
                {
                    Log.Info("Time Start: " + dr[i]["timeStart"].ToString());
                    Log.Info("Time End: " + dr[i]["timeEnd"].ToString());
                    Log.Info("DayofWeek: " + dr[i]["DayOfWeek"].ToString());
                    Log.Info("Category: " + dr[i]["category"].ToString());
                    Log.Info("Season: " + dr[i]["TOUSeasonName"].ToString());
                }
            }
            DateTime myTime = new DateTime(2006, 1, 1, 0, 0, 0);

            int dayOfWeek = 0;
            DataRow[] drFilter;
            string sFilter = "";

            int myPeriods = (60 * 24) / myMeteringInterval;


            NextDay:;
            DateTime newTime = myTime;
            int category = 0;

            for (int i = 0; i < myPeriods; i++)
            {
                newTime = newTime.AddMinutes(myMeteringInterval);

                string min = newTime.Minute.ToString();
                if (min.Length == 1) { min = "0" + min; }

                string hour = Convert.ToString(newTime.Hour);
                if (hour.Length == 1) { hour = "0" + hour; }
                hour = hour + ":" + min;
                if (hour == "00:00")
                {
                    hour = "23:59";
                }
                sFilter = "TouLookupId=" + touLookUpId + " AND DayOfWeek=" + dayOfWeek + " AND TimeStart< '" + hour + "' AND TimeEnd>= '" + hour + "'  AND Season = " + season;
                drFilter = myTable.Select(sFilter);

                for (int j = 0; j < drFilter.Length; j++)
                {
                    try
                    {
                        category = Convert.ToInt16(drFilter[j][4]);
                    }
                    catch (Exception)
                    {
                        goto SkipNext;
                    }

                    if (bLogTest == true)
                    {
                        Log.Info("Count for Time: " + hour.ToString() + " = " + drFilter.Length);
                    }
                    if (bLogTest == true)
                    {
                        Log.Info("Time: " + hour.ToString());
                        Log.Info("Season: " + season.ToString());
                        Log.Info("DayofWeek: " + dayOfWeek.ToString());
                        Log.Info("Category: " + category.ToString());
                    }
                    SaveConnections.SaveLookUp(touLookUpId, hour, category, season, dayOfWeek);
                    SkipNext:;


                }
            }
            if (dayOfWeek == 0)
            {
                dayOfWeek = 1;
                goto NextDay;
            }
            else if (dayOfWeek == 1)
            {
                dayOfWeek = 7;
                goto NextDay;
            }
            else
            {
                dayOfWeek = 0;
            }

        }
    }
}
