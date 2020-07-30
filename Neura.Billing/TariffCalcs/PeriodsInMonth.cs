using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neura.Billing.Data;
using System.Data;

namespace Neura.Billing.TariffCalcs
{
    class PeriodsInMonth
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void GetPeriodsInMonth(DateTime readDate, int lookupId, int myMeteringInterval,
            out int countPeak, out int countPeakStandard, out int countStandard, out int countOffPeak, bool toDate = false)
        {
            //Check for midnight
            string min = readDate.Minute.ToString();
            if (min.Length == 1) { min = "0" + min; }

            string hour = Convert.ToString(readDate.Hour);
            if (hour.Length == 1) { hour = "0" + hour; }
            hour = hour + ":" + min;
            if (hour == "00:00")
            {
                readDate = readDate.AddMinutes(-1);
                hour = "23:59";
            }


            int month = readDate.Month;
            string sMonth = month.ToString();
            if (sMonth.Length == 1) { sMonth = "0" + sMonth; }

            int year = readDate.Year;
            string sYear = year.ToString();

            DateTime myDate = Convert.ToDateTime(sYear + "/" + sMonth + "/01");
            countPeak = 0;
            countStandard = 0;
            countOffPeak = 0;
            countPeakStandard = 0;
            //Check if exists
            if (toDate == true) goto skipCount;
            int count = UtilityConnections.SelectPeriodInMonthPiM(myDate, lookupId, out DataTable dtPeriodInMonth);
            if (count > 0) { return; }
            string filter = "";
            skipCount:;
            int season = Seasons.GetSeason(month, lookupId);
            //int dayOfWeek = 0;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int periodsInDay = (24 * 60) / myMeteringInterval;

            UtilityConnections.SelectLookupTou(out DataTable dtLookupTou);

            filter = "TouLookupId = " + lookupId + " AND Season = " + season;
            string order = "DayofWeek ASC, Time ASC";
            DataRow[] dr = dtLookupTou.Select(filter, order);

            //Add the data
            DataTable myTable = new DataTable("myTable", "Neura.Billing2.TariffCalcs");
            myTable.Columns.Add("TouLookUpID", Type.GetType("System.Int16"));
            myTable.Columns.Add("Time", Type.GetType("System.String"));
            myTable.Columns.Add("Season", Type.GetType("System.Int16"));
            myTable.Columns.Add("DayOfWeek", Type.GetType("System.Int16"));
            myTable.Columns.Add("Category", Type.GetType("System.Int16"));
            DataRow newRow;
            for (int i = 0; i < dr.Length; i++)
            {
                newRow = myTable.NewRow();
                newRow["TouLookupId"] = dr[i]["TouLookupId"].ToString();
                newRow["time"] = dr[i]["time"].ToString();
                newRow["Season"] = dr[i]["Season"].ToString();
                newRow["DayOfWeek"] = dr[i]["DayOfWeek"].ToString();
                newRow["category"] = dr[i]["category"].ToString();
                //if (bLogTest == true)
                //{
                //    Log.Info("----- Time: " + newRow["time"] + "------");
                //    Log.Info("Season: " + newRow["Season"]);
                //    Log.Info("DayOfWeek: " + newRow["DayOfWeek"]);
                //    Log.Info("Category: " + newRow["category"]);
                //}
                myTable.Rows.Add(newRow);

            }
            DateTime myTime = new DateTime(year, month, 1, 0, 0, 0);
            DataRow[] drFilter;
            string sFilter = "";
            DateTime newTime;
            int thisDayOfWeek = 0;


            for (int i = 0; i < daysInMonth; i++)
            {
                for (int j = 0; j < periodsInDay; j++)
                {
                    myTime = myTime.AddMinutes(myMeteringInterval);
                    newTime = myTime;

                    min = newTime.Minute.ToString();
                    if (min.Length == 1) { min = "0" + min; }

                    hour = Convert.ToString(newTime.Hour);
                    if (hour.Length == 1) { hour = "0" + hour; }
                    hour = hour + ":" + min;
                    if (hour == "00:00")
                    {
                        newTime = newTime.AddMinutes(-1);
                        hour = "23:59";
                    }


                    switch ((int)newTime.DayOfWeek)
                    {
                        case 0: //Sunday
                            thisDayOfWeek = 1;
                            break;
                        case 6: //Saturday
                            thisDayOfWeek = 7;
                            break;
                        default:
                            thisDayOfWeek = 0;
                            break;
                    }

                    //if (bLogTest == true)
                    //{
                    //    Log.Info("----- Time: " + newTime + "------");
                    //    Log.Info("DayOfWeek: " + (int)newTime.DayOfWeek);
                    //    Log.Info("thisDayOfWeek: " + thisDayOfWeek);
                    //}
                    sFilter = "TouLookupId='" + lookupId + "' AND DayOfWeek=" + thisDayOfWeek +
                              " AND Time= '" + hour + "'";
                    drFilter = myTable.Select(sFilter);
                    int category = 0;
                    for (int k = 0; k < drFilter.Length; k++)
                    {
                        category = Convert.ToInt16(drFilter[k][4]);
                        //if (bLogTest == true)
                        //{
                        //    Log.Info("Category: " + category);
                        //}
                        switch (category)
                        {
                            case 0:
                                countPeak += 1;

                                break;
                            case 1:
                                countStandard += 1;

                                break;
                            case 2:
                                countOffPeak += 1;

                                break;
                            case 4:

                                countPeakStandard += 1;

                                break;
                        }
                    }
                    if (toDate == true)
                    {
                        //Check if to date reached
                        if (newTime == readDate)
                        {
                            return;
                        }
                    }
                }
            }
            SaveConnections.SavePeriodInMonth(lookupId, myDate, countPeak, countStandard, countOffPeak, countPeakStandard);
        }
    }
}
