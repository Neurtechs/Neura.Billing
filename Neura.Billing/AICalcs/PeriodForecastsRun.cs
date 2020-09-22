using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.Reflection;
using Neura.Billing.Data;
using Neura.Billing.TariffCalcs;

namespace Neura.Billing.AICalcs
{
    class PeriodForecastsRun
    {
        private static DataTable dtGetLastPeriodValues;
        private static DataTable dtTemplate;
        private static DateTime myTime;
        private static DataTable dtNodes;
        private static DataTable dtNodesWithData;
        private static int myNodeId;
        public static void RunForecasts(int meteringInterval,double w1, double w2, double w3,
             double w4, double w5, double w6)
        {
            AIConnections.GetNodesWithData(1, out dtNodesWithData);
            int nodeCount = dtNodesWithData.Rows.Count;
            AIConnections.GetTemplate(out dtTemplate);
            myTime =DateTime.Now;
            if (myTime.Minute>30)
            {
                myTime = myTime.AddMinutes(-(myTime.Minute - 30));
                myTime = myTime.AddSeconds(-myTime.Second);
            }
            else
            {
                myTime = myTime.AddMinutes(-myTime.Minute);
                myTime = myTime.AddSeconds(-myTime.Second);
            }
            for (int i = 0; i < nodeCount; i++)
            {
                myNodeId =Convert.ToInt32( dtNodesWithData.Rows[i]["Node"]);
                AIConnections.GetLastPeriodValues(myNodeId, myTime, out dtGetLastPeriodValues);
                int myCount = dtGetLastPeriodValues.Rows.Count;
                decimal days = (myCount / meteringInterval);
                int myDays = Convert.ToInt32(Math.Truncate(days));
                if (myDays < 1)
                {
                    //listItems.Add("Cannot proceed with less than 1 days worth of data");
                    goto NextNode;
                }
                if (myDays > 14) { myDays = 14; }
                int myPeriods = myDays * meteringInterval;

                double[] totUAcc = new double[3];
                DataTable dtLastX = dtGetLastPeriodValues.AsEnumerable().Reverse().Take(myPeriods).CopyToDataTable();
                double myAverage = 0;
                foreach (DataRow dr in dtLastX.Rows) { myAverage += Convert.ToDouble(dr["uAcc"]); }
                myAverage = myAverage / myDays;
                //listItems.Add("Average daily consumption =  " + myAverage);
             

                MonthStartEnd.GetMonthStartEnd(myTime, out DateTime monthStart, out DateTime monthEnd);
                MonthStartEnd.GetDayStartEnd(myTime, out DateTime dayStart, out DateTime dayEnd);
                MonthStartEnd.GetWeekStartEnd(myTime, out DateTime weekStart, out DateTime weekEnd);
                AIConnections.GetMonthUAcc(myNodeId, monthStart, out DataTable dtMonthValues); //Consumption to date

                double cPreviousFixed = Convert.ToDouble(dtMonthValues.Rows[0]["cFixed"]);
                double cPreviousMaximum = Convert.ToDouble(dtMonthValues.Rows[0]["cMaximum"]);
                double cPreviousAcc = Convert.ToDouble(dtMonthValues.Rows[0]["cAcc"]);
                double uPreviousMaximum = Convert.ToDouble(dtMonthValues.Rows[0]["uMaximum"]);
                double uPreviousPeak = Convert.ToDouble(dtMonthValues.Rows[0]["uPeak"]);
                double uPreviousAcc = Convert.ToDouble(dtMonthValues.Rows[0]["uAcc"]);

                int[]periodsToGo=new int[3];
               
                TimeSpan duration;
               
                duration = dayEnd - myTime;
                periodsToGo[0] = Convert.ToInt32(duration.TotalMinutes / meteringInterval) + 1;
            
                duration = weekEnd - myTime;
                periodsToGo[1] = Convert.ToInt32(duration.TotalMinutes / meteringInterval) + 1;
            
                duration = monthEnd - myTime;
                periodsToGo[2] = Convert.ToInt32(duration.TotalMinutes / meteringInterval) + 1;
                
                totUAcc[2] = uPreviousAcc;

                //listItems.Add("Consumption to date for month = " + totUAcc + " kWh");
                //listItems.Add("Forecasting for  = " + periodsToGo + " periods");

                //Get Aveage consumption from template
                double aveConsumption = 0;
                int _month;
                int _year;
                int _daysInMonth;
                _month = myTime.Month;
                _year = myTime.Year;
                _daysInMonth = DateTime.DaysInMonth(_year, _month);
                aveConsumption = AIConnections.GetSumTemplate(_month, _year);
                aveConsumption = aveConsumption / _daysInMonth; //daily average from template

                AIConnections.GetNodeBasic(myNodeId, out int myTariff, out int myMeterType, out int ReadingsType);
                int componentCount = TariffComponents.GetTariffComponents(myTariff, myTime, out DataTable dtComponents);
                int TOULookupId = UtilityConnections.SelectTOULookup(myTariff);

                //Deal with reading types
                int readingTypesCount = UtilityConnections.SelectUsageByNodeTariff(myNodeId, myTariff,
                    myTime, out DataTable dtUsage);
                double[] totCAcc = new double[3];
                totCAcc[2] = cPreviousAcc + cPreviousFixed + cPreviousMaximum;
                double[] runningTotUacc = new double[3];
                double[] runningTotCost = new double[3];
               
                double cAcc = 0;
                double cFixed = 0;
                double cMax = 0;
                DataRow newRow;
                DateTime fDate=myTime;
                for (int j = 0; j < periodsToGo[2]; j++)
                {
                    //Forecast Consumption
                   
                    PeriodForecasts.CalcValue(myNodeId, fDate, w1, w2, w3,
                        w4, w5, w6, meteringInterval, myAverage, dtGetLastPeriodValues,
                        dtTemplate, aveConsumption, out double myForecast);
                    //listItems.Add("Forecast energy for period " + myTime + " = " + Math.Round(myForecast, 4) + " kWh");


                    //Forecast costs
                    CalcCosts.GetCosts(dtComponents, meteringInterval, 
                        myForecast, cPreviousAcc, cPreviousFixed, 
                        cPreviousMaximum, uPreviousAcc, myTariff, 
                        ReadingsType, uPreviousMaximum,
                        uPreviousMaximum, uPreviousPeak, myNodeId, TOULookupId, 
                        fDate, out double cAccToDate, 
                        out double cFixedToDate,
                        out double cMaxToDate, out double uAccToDate, 
                        out double uToDatePeak);
                    cAcc = cAccToDate - cPreviousAcc;
                    cFixed = cFixedToDate - cPreviousFixed;
                    cMax = cMaxToDate - cPreviousMaximum;

                    runningTotUacc[2] += myForecast;
                    totUAcc[2] += myForecast;
                    runningTotCost[2] += cAcc + cFixed + cMax;
                    totCAcc[2] += cAcc + cFixed + cMax;
                    if (j == periodsToGo[0])
                    {
                        runningTotUacc[0] = runningTotUacc[2];
                        totUAcc[0] = totUAcc[2];
                        runningTotCost[0] = runningTotCost[2];
                        totCAcc[0] = totCAcc[2];
                    }
                    if (j == periodsToGo[1])
                    {
                        runningTotUacc[1] = runningTotUacc[2];
                        totUAcc[1] = totUAcc[2];
                        runningTotCost[1] = runningTotCost[2];
                        totCAcc[1] = totCAcc[2];
                    }
                    fDate = fDate.AddMinutes(meteringInterval);
                    cPreviousAcc = cAccToDate;
                    cPreviousFixed = cFixedToDate;
                    cPreviousMaximum = cMaxToDate;
                    uPreviousAcc = uAccToDate;
                    uPreviousPeak = uToDatePeak;

                }
                //Update values
                AIConnections.UpdateForecast(myNodeId,totUAcc[0],totCAcc[0],
                    totUAcc[1], totCAcc[1], totUAcc[2], totCAcc[2],
                       runningTotUacc[0],runningTotCost[0],
                    runningTotUacc[1], runningTotCost[1],
                    runningTotUacc[2], runningTotCost[2]);

                NextNode: ;
            }
        }
    }
}
