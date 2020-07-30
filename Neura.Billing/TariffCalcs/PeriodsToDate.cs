using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Neura.Billing.GlobalVar;
using Neura.Billing.Data;
using System.Data;

namespace Neura.Billing.TariffCalcs
{
    class PeriodsTDate
    {
        public static void PeriodsToDate(DateTime myReadingDate, int myMeteringInterval, out int periodsInMonth, out int periodsPast)
        {

            int month = myReadingDate.Month;
            int year = myReadingDate.Year;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int minutesInMonth = 24 * 60 * daysInMonth;
            periodsInMonth = minutesInMonth / myMeteringInterval;
            int periodsInDay = (24 * 60) / myMeteringInterval;
            int daysPast = myReadingDate.Day - 1;
            DateTime newTime = myReadingDate;

            string min = newTime.Minute.ToString();
            if (min.Length == 1) { min = "0" + min; }

            string hour = Convert.ToString(newTime.Hour);
            if (hour.Length == 1) { hour = "0" + hour; }
            hour = hour + ":" + min;
            periodsPast = 0;
            if (myReadingDate.Day == 1 && hour == "00:00")
            {
                myReadingDate = myReadingDate.AddMinutes(-1);
                month = myReadingDate.Month;
                year = myReadingDate.Year;
                daysInMonth = DateTime.DaysInMonth(year, month);
                minutesInMonth = 24 * 60 * daysInMonth;
                periodsInMonth = minutesInMonth / myMeteringInterval;
                periodsPast = periodsInMonth;
            }
            else
            {
                if (daysPast > 0)
                {
                    periodsPast = daysPast * periodsInDay;
                }
                if (myReadingDate.Minute == 59) { myReadingDate = myReadingDate.AddMinutes(1); }
                int minutesPast = myReadingDate.Hour * 60 + myReadingDate.Minute;
                periodsPast = periodsPast + minutesPast / myMeteringInterval;
            }




        }
    }
}
