using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neura.Billing.Data;
using static Neura.Billing.GlobalVar;
using System.Data;

namespace Neura.Billing.TariffCalcs
{
    public class TOURate
    {
        public static bool CheckTouRate(DateTime myReadingDate, int myLookupId, int mySeason, int myInterval)
        {

            int day = myReadingDate.Day;
            int hour = myReadingDate.Hour;
            int minute = myReadingDate.Minute;
            DateTime tempDateTime = myReadingDate;
            //if (day == 1 && hour == 0 && minute == 0) { tempDateTime = tempDateTime.AddMinutes(-1); }
            if (hour == 0 && minute == 0) { tempDateTime = tempDateTime.AddMinutes(-1); }
            int month = tempDateTime.Month;
            DayOfWeek dow = tempDateTime.DayOfWeek;
            int monthStart = 0;
            int monthEnd = tempDateTime.Month;

            string time = tempDateTime.ToString("HH:mm");
            string date = tempDateTime.ToShortDateString();
            UtilityConnections.SelectTOULookupTR(myLookupId, mySeason, myInterval, time, date, dow, out DataTable dtTOULookup);
            string filter = "";
            string order = "";
            DataRow[] dr = dtTOULookup.Select(filter, order);
            int componentCount = 0;
            componentCount = dr.Length;
            if (componentCount == 0)
            {
                return false;
            }

            for (int i = 0; i < componentCount; i++)
            {
                monthStart = Convert.ToInt32(dr[i]["monthStart"]);
                monthEnd = Convert.ToInt32(dr[i]["monthEnd"]);
            }
            //Check for month
            if (monthEnd > monthStart)
            {
                if (month >= monthStart && month <= monthEnd)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (month >= monthStart || month <= monthEnd)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
