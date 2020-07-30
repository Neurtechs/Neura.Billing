using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Neura.Billing.GlobalVar;

namespace Neura.Billing.TariffCalcs
{
    class Fixed
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void GetFixed(int mySeason, int myInterval, Double myRate, bool byPeriod,
           DateTime myReadingDate, double cPreviousFixed, int myMeteringInterval, out double cFixed)
        {
            //Meaning of values: 0, 1, 2 etc.
            //public enum TouSeasonName { High, Low, All }
            //public enum Category { Peak, Standard, OffPeak, All, PeakAndStandard }
            //public enum Service { Electricity, Water, Gas }
            //public enum Season { High, Low, All, NotApplicable }
            //public enum Measurement { Accumulated, Maximum, Fixed, MaxProgressive }
            //public enum Interval { Peak, Standard, OffPeak, All, PeakAndStandard, NonEnergy }
            //public enum Flow { NotApplicable, Import, Export, Consumption, Generate, NetImportExport, NetImportExportGenerate }
            //public enum Period { Account, Day, Month }

            //if calculate by 1/2 hour byPeriod = true. Else over account period e.g. month
            Double calc = myRate;  //Default fixed charge over month
            int hour = myReadingDate.Hour;
            int day = myReadingDate.Day;
            int minute = myReadingDate.Minute;
            if (bLogTest == true)
            {
                Log.Info("");
                Log.Info("--------------Fixed rate calculation--------------");
            }

            if (byPeriod == true)
            {
                if (hour == 0 && minute == 0 && day == 1)  //Last period
                {
                    myReadingDate = myReadingDate.AddMinutes(-1);
                    int year = myReadingDate.Year;
                    int month = myReadingDate.Month;
                    int days = DateTime.DaysInMonth(year, month);
                    int minutes = days * 24 * 60;
                    int periods = minutes / myMeteringInterval;
                    Double toDate = cPreviousFixed;
                    cPreviousFixed = myRate; //Final accurate amount
                    calc = myRate - toDate;
                    cFixed = calc;
                    if (bLogTest == true)
                    {
                        Log.Info("----Final Period in month----------");
                        Log.Info("Rate: " + myRate);
                        Log.Info("Days in month: " + days);
                        Log.Info("calc (cost per period): " + calc);
                        Log.Info("Recovered to date: " + cPreviousFixed);
                    }

                    if (bLogResult == true)
                    {
                        Log.Info("Rate for fixed component: " + myRate);
                    }


                }
                else
                {
                    int year = myReadingDate.Year;
                    int month = myReadingDate.Month;
                    int days = DateTime.DaysInMonth(year, month);
                    int minutes = days * 24 * 60;
                    int periods = minutes / myMeteringInterval;

                    calc = calc / (periods);
                    cPreviousFixed = cPreviousFixed + calc;
                    cFixed = calc;
                    if (bLogTest == true)
                    {
                        if (hour == 0 && minute == 30 && day == 1) { Log.Info("----First Period in month----------"); }
                        Log.Info("Rate: " + myRate);
                        Log.Info("Days in month: " + days);
                        Log.Info("calc (cost per period): " + calc);
                        Log.Info("Recovered to date: " + cPreviousFixed);
                    }


                }
            }
            else
            {
                calc = calc - cPreviousFixed;
                cPreviousFixed = calc + cPreviousFixed;
                cFixed = cPreviousFixed;

            }

        }

    }
}
