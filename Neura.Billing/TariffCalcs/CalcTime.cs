using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neura.Billing.TariffCalcs
{
    public class CalcTime
    {
        public static DateTime ToNearest(DateTime timeStamp, int myMeteringInterval, bool getNearest, bool UTC = false)
        {
            DateTime result;

            //Convert UTC to local time
            if (UTC == true)
            {
                try
                {
                    timeStamp = timeStamp.ToLocalTime();
                    if (getNearest == false) { return timeStamp; }
                }
                catch (Exception)
                {

                }
            }

            //Get nearest metering interval

            int minute = timeStamp.Minute;
            int year = timeStamp.Year;
            int month = timeStamp.Month;
            string sMonth = month.ToString();
            if (sMonth.Length == 1) { sMonth = "0" + sMonth; }

            sMonth = year.ToString() + "/" + sMonth + "/01 00:00:00";
            string sMinute = "";
            bool upHour = false;

            double variance = Convert.ToDouble(myMeteringInterval) / 2;

            int intervalsPerHour = 60 / myMeteringInterval;

            for (int i = 0; i < intervalsPerHour; i++)
            {
                if (minute >= (i * myMeteringInterval - variance) && minute < (i * myMeteringInterval + variance))
                {
                    sMinute = Convert.ToString(i * myMeteringInterval);
                    break;
                }
            }
            if (sMinute.Length == 1) { sMinute = "0" + sMinute; }


            if (minute >= 0 && minute <= variance)
            {
                sMinute = "00";
            } //Between 0 and variance, then 0
            else if (minute > variance && minute <= 3 * myMeteringInterval)
            {
                sMinute = Convert.ToString(myMeteringInterval);
            }
            else
            {
                sMinute = "00";
                upHour = true;
            }

            string test = timeStamp.Year.ToString().Trim();
            string sDateReceived = test;
            test = timeStamp.Month.ToString().Trim();
            if (test.Length == 1)
            {
                test = "0" + test;
            }

            sDateReceived += "/" + test;
            test = timeStamp.Day.ToString().Trim();
            if (test.Length == 1)
            {
                test = "0" + test;
            }

            sDateReceived += "/" + test;
            int hour = timeStamp.Hour;
            if (upHour == true)
            {
                hour += 1;
            }

            if (hour == 24)
            {
                hour = 0;
            }

            test = hour.ToString();
            if (test.Length == 1)
            {
                test = "0" + test;
            }

            sDateReceived += " " + test + ":" + sMinute + ":00";
            result = DateTime.Parse(sDateReceived);
            int day = result.Day;
            hour = result.Hour;
            minute = result.Minute;
            DateTime tempDateTime = result;

            // if (day == 1 && hour == 0 && minute == 0) { tempDateTime = tempDateTime.AddMinutes(-1); }
            return tempDateTime;
        }
    }
}
