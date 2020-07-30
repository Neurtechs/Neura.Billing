using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neura.Billing.Data;
using static Neura.Billing.GlobalVar;

namespace Neura.Billing.TariffCalcs
{
    public class Maximum
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void GetMaximumDemand(Double myRate, int myFlow, int mySeason,
            int myInterval, int myMeasurement, int myMeteringInterval,
            DateTime myReadingDate, int myLookupId, double netDemand, double cPreviousMax,
            double uPreviousMax, double uPreviousPeak, out double cMax, out double uMax, out double uToDateMax,
            out double cToDateMax, out double uPeak)
        {
            if (bLogTest == true)
            {
                Log.Info("");
                Log.Info("--------------Maximum rate calculation--------------");
            }

            if (netDemand == 0)
            {
                if (bLogTest == true) { Log.Info("Maximum demand == 0"); }

                uMax = 0; //current marginal max
                cMax = 0;
                uPeak = 0; //current absolute peak
                uToDateMax = uPreviousMax;
                cToDateMax = cPreviousMax;

                return;
            }


            Double value = 0;
            uMax = 0;
            cMax = 0;
            uPeak = 0; //current absolute peak
            uToDateMax = uPreviousMax;
            cToDateMax = cPreviousMax;

            int myDayOfWeek = 0;
            string hour = Convert.ToString(myReadingDate.Hour);
            if (hour.Length == 1) { hour = "0" + hour; }
            string minute = Convert.ToString(myReadingDate.Minute);
            if (minute.Length == 1) { minute = "0" + minute; }
            string myTime = hour + ":" + minute;

            switch (myReadingDate.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    myDayOfWeek = 7;
                    break;
                case DayOfWeek.Sunday:
                    myDayOfWeek = 1;
                    break;
                default:
                    myDayOfWeek = 0;
                    break;
            }
            //Interval: 0=Peak,1=Standard,2=Offpeak,3=All,4=PeakAndStandard,5=NonEnergy
            if (myInterval != 3)
            {
                //Check if period applicable
                UtilityConnections.FindTouCategory(myLookupId, mySeason, myDayOfWeek, myTime,
                    out DataTable dtCategory);
                foreach (DataRow dr in dtCategory.Rows)
                {
                    if (Convert.ToInt16(dr["Category"]) == myInterval) { goto Continue; }
                }
                //Not applicble
                if (bLogTest == true) { Log.Info("Maximum Demand not appicable this interval"); }
                uMax = 0;
                cMax = 0;
                uPeak = 0;

                uToDateMax = uPreviousMax;
                cToDateMax = cPreviousMax;

                return;

                Continue:;

            }


            if (myMeasurement == 1) //Maximum
            {
                if (netDemand > uPreviousMax) //higher that perviuos
                {
                    value = netDemand * myRate;
                    uMax = netDemand - uPreviousMax;
                    cMax = value - cPreviousMax;
                    uToDateMax = netDemand;
                    cToDateMax = value;
                    uPeak = uMax;

                    if (bLogTest == true)
                    {
                        Log.Info("Maximum demand type: Once-off Maximum");
                        Log.Info("Maximum demand updated from: " + uPreviousMax);
                        Log.Info("Maximum demand updated to: " + netDemand);
                        Log.Info("Maximum demand cost updated from: " + cPreviousMax);
                        Log.Info("Maximum demand cost updated to: " + cToDateMax);
                        Log.Info("Rate: " + myRate);
                        Log.Info("Marginal MD: " + uMax);
                        Log.Info("Marinal Cost: " + cMax);
                    }
                    if (bLogResult == true)
                    {
                        Log.Info("Rate for maximum component: " + myRate);
                    }

                }

                else
                {
                    if (bLogTest == true)
                    {
                        Log.Info("Maximum demand type: Once-off Maximum");
                        Log.Info("Maximum demand of : " + netDemand + " lower than previous");
                        Log.Info("Rate: " + myRate);
                        Log.Info("MD to date: " + uPreviousMax);
                        Log.Info("Cost to date: " + cPreviousMax);
                    }
                    if (bLogResult == true)
                    {
                        Log.Info("Rate for maximum component: " + myRate);
                    }
                    uMax = 0; //current max
                    cMax = 0;
                    uPeak = 0;
                    uToDateMax = uPreviousMax;
                    cToDateMax = cPreviousMax;

                    return;
                }
            }
            else //max progressive
            {

                ////Get Periods in month
                //int month = myReadingDate.Month;
                //string sMonth = month.ToString();
                //if (sMonth.Length == 1) { sMonth = "0" + sMonth; }

                //int year = myReadingDate.Year;
                //string sYear = year.ToString();

                //DateTime myDate = Convert.ToDateTime(sYear + "/" + sMonth + "/01");
                //UtilityConnections.GetPeriodsInMonth(myLookupId, myDate, out DataTable dtPeriodsInMonth);

                //Get Periods to date and in month
                PeriodsTDate.PeriodsToDate(myReadingDate, myMeteringInterval, out int periodsInMonth, out int periodsPast);

                int periodsRemaining = periodsInMonth - periodsPast + 1;

                //PeriodsInMonth.GetPeriodsInMonth(myReadingDate, myLookupId, myMeteringInterval, out int peak, out int peakAndStandard,
                //    out int standard, out int offPeak, true);
                //int periodsInMonth = 0;
                //int periodsRemaining = 0;

                ////Interval: 0=Peak,1=Standard,2=Offpeak,3=All,4=PeakAndStandard,5=NonEnergy

                //switch (myInterval)
                //{
                //    case 0:
                //        periodsInMonth = Convert.ToInt16(dtPeriodsInMonth.Rows[0]["Peak"]);
                //        periodsRemaining = periodsInMonth - peak;
                //        break;
                //    case 1:
                //        periodsInMonth = Convert.ToInt16(dtPeriodsInMonth.Rows[0]["Standard"]);
                //        periodsRemaining = periodsInMonth - standard;
                //        break;
                //    case 2:
                //        periodsInMonth = Convert.ToInt16(dtPeriodsInMonth.Rows[0]["OffPeak"]);
                //        periodsRemaining = periodsInMonth - offPeak;
                //        break;
                //    case 4:
                //        periodsInMonth = Convert.ToInt16(dtPeriodsInMonth.Rows[0]["PeakStandard"]);
                //        periodsRemaining = periodsInMonth - peakAndStandard;
                //        break;
                //    default:
                //        periodsInMonth = Convert.ToInt16(dtPeriodsInMonth.Rows[0]["PeakStandard"]) +
                //            Convert.ToInt16(dtPeriodsInMonth.Rows[0]["OffPeak"]);
                //        periodsRemaining = periodsInMonth - (peakAndStandard + offPeak);
                //        break;
                //}

                if (periodsInMonth == periodsRemaining)
                {
                    //First Period
                    uPreviousMax = uPreviousPeak = 0;
                    uMax = netDemand / periodsRemaining;
                    uToDateMax = uMax;
                    cMax = uMax * myRate;
                    cToDateMax = cMax;
                    uPeak = netDemand;

                    if (bLogTest == true)
                    {
                        Log.Info("Maximum demand First Period of Month =============");
                        Log.Info("Maximum demand type: Maximum Progressive");
                        Log.Info("Maximum demand : " + netDemand);
                        Log.Info("Periods remaining : " + periodsRemaining);
                        Log.Info("Marinal cost this period : " + cMax);
                        Log.Info("Rate: " + myRate);
                        Log.Info("Cost to date: " + cToDateMax);
                    }
                    if (bLogResult == true)
                    {
                        Log.Info("Rate for maximum component: " + myRate);
                    }

                }
                else if (periodsRemaining == 1)
                //Last period
                {
                    if (netDemand <= uPreviousMax)
                    {
                        //no need to update
                        uToDateMax = uPreviousMax;
                        uPeak = uPreviousPeak;
                        cToDateMax = uToDateMax * myRate;
                        cMax = cToDateMax - cPreviousMax;

                        if (bLogTest == true)
                        {
                            Log.Info("Maximum demand Last Period of Month =============");
                            Log.Info("Maximum demand type: Maximum Progressive");
                            Log.Info("Maximum demand of : " + netDemand + " lower than previous");
                            Log.Info("Rate: " + myRate);
                            Log.Info("MD to date: " + uToDateMax);
                            Log.Info("Cost to date: " + cToDateMax);
                        }
                        if (bLogResult == true)
                        {
                            Log.Info("Rate for maximum component: " + myRate);
                        }

                    }
                    else
                    {
                        //Value higher than before
                        uPeak = netDemand; //new peak
                        uMax = uPeak - uPreviousPeak;
                        uToDateMax = uPeak;
                        cToDateMax = uToDateMax * myRate;
                        cMax = cToDateMax - cPreviousMax; //final additional cost

                        if (bLogTest == true)
                        {
                            Log.Info("Maximum demand Last Period of Month =============");
                            Log.Info("Maximum demand type: Maximum Progressive");
                            Log.Info("Maximum demand of : " + netDemand + " higher than previous");
                            Log.Info("Rate: " + myRate);

                            Log.Info("Marinal cost this period : " + cMax);
                            Log.Info("MD to date: " + uToDateMax);
                            Log.Info("Cost to date: " + cToDateMax);
                        }
                        if (bLogResult == true)
                        {
                            Log.Info("Rate for maximum component: " + myRate);
                        }
                    }
                }
                else
                {
                    //intermediate period
                    if (netDemand <= uPreviousPeak)
                    {
                        //no recalc necessary
                        //uMax= uPreviousPeak / periodsRemaining;
                        //uToDateMax = uPreviousMax + uMax;
                        //cMax = uMax  * myRate;
                        //cToDateMax = cPreviousMax + cMax;
                        //uPeak = uPreviousPeak;
                        uPeak = uPreviousPeak;
                        double costRequired = uPeak * myRate;
                        double demandRequired = uPeak - uPreviousMax;
                        uMax = demandRequired / periodsRemaining;
                        costRequired = costRequired - cPreviousMax; //Still to be charged
                        cMax = costRequired / periodsRemaining; //cost required per remaining period
                        cToDateMax = cPreviousMax + cMax;
                        uToDateMax = uPreviousMax + uMax;

                        if (bLogTest == true)
                        {
                            Log.Info("Maximum demand Intermediate period");
                            Log.Info("Maximum demand type: Maximum Progressive");
                            Log.Info("Maximum demand of : " + netDemand + " lower than previous");
                            Log.Info("Rate: " + myRate);
                            Log.Info("Remaining Cost Required: " + costRequired);
                            Log.Info("Remaining Periods: " + periodsRemaining);
                            Log.Info("Cost this period: " + cMax);
                            Log.Info("MD to date: " + uToDateMax);
                            Log.Info("Cost to date: " + cToDateMax);
                        }
                        if (bLogResult == true)
                        {
                            Log.Info("Rate for maximum component: " + myRate);
                        }
                    }
                    else
                    {
                        uPeak = netDemand; //required for month
                        double costRequired = uPeak * myRate;
                        costRequired = costRequired - cPreviousMax; //Still to be charged
                        cMax = costRequired / periodsRemaining; //cost required per remaining period
                        cToDateMax = cPreviousMax + cMax;
                        double demandRequired = uPeak - uPreviousMax;
                        uMax = demandRequired / periodsRemaining;

                        uToDateMax = uPreviousMax + uMax;

                        if (bLogTest == true)
                        {
                            Log.Info("Maximum demand Intermediate Period");
                            Log.Info("Maximum demand type: Maximum Progressive");
                            Log.Info("Peak demand of : " + netDemand + " higher than previous");
                            Log.Info("Periods remaining : " + periodsRemaining);
                            Log.Info("Remaining Cost Required: " + costRequired);
                            Log.Info("Marginal demand updated by: " + uMax);
                            Log.Info("Marginal cost this period : " + cMax);

                            Log.Info("Maximum demand updated from: " + uPreviousMax);
                            Log.Info("Maximum demand updated to: " + uToDateMax);

                            Log.Info("Maximum demand cost updated from: " + cPreviousMax);
                            Log.Info("Maximum demand cost updated to: " + cToDateMax);
                            Log.Info("Rate: " + myRate);
                            Log.Info("MD to date: " + uToDateMax);

                        }
                        if (bLogResult == true)
                        {
                            Log.Info("Rate for maximum component: " + myRate);
                        }
                    }
                }
            }
        }
    }
}
