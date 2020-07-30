using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neura.Billing.TariffCalcs
{
    class MonthStartEnd
    {
        public static void GetMonthStartEnd(DateTime myDate, out DateTime monthStart, out DateTime monthEnd)
        {

            int day = myDate.Day;
            int minute = myDate.Minute;
            int hour = myDate.Hour;
            if (day == 1 && hour == 0 && minute == 0)
            {
                // Still in previous time
                myDate = myDate.AddMinutes(-1);
            }
            int month = myDate.Month;
            int year = myDate.Year;
            string lastDay = DateTime.DaysInMonth(year, month).ToString();
            string sYear = year.ToString();
            string sMonth = month.ToString();
            string sDay = day.ToString();
            if (sMonth.Length == 1) { sMonth = "0" + sMonth; }
            if (sDay.Length == 1) { sDay = "0" + sDay; }

            string sFirst = (sYear + "/" + sMonth + "/01 00:00:00");
            monthStart = DateTime.Parse(sFirst);
            monthEnd = monthStart.AddMonths(1);
            //monthEnd = monthEnd.AddMinutes(-1);
        }
        public static void GetDayStartEnd(DateTime myDate, out DateTime dayStart, out DateTime dayEnd)
        {
            int day = myDate.Day;
            int day2 = myDate.AddDays(1).Day;
            int minute = myDate.Minute;
            int hour = myDate.Hour;
            if (day == 1 && hour == 0 && minute == 0)
            {
                // Still in previous time
                myDate = myDate.AddMinutes(-1);
            }
            int month = myDate.Month;
            int year = myDate.Year;
            string lastDay = DateTime.DaysInMonth(year, month).ToString();
            string sYear = year.ToString();
            string sMonth = month.ToString();
            string sDay = day.ToString();
            string sDay2 = (day2).ToString();
            if (sMonth.Length == 1) { sMonth = "0" + sMonth; }
            if (sDay.Length == 1) { sDay = "0" + sDay; }
            if (sDay2.Length == 1) { sDay2 = "0" + sDay2; }
            string sFirst = (sYear + "/" + sMonth + "/" + sDay + " 00:30:00");
            string sLast = (sYear + "/" + sMonth + "/" + sDay2 + " 00:00:00");
            dayStart = DateTime.Parse(sFirst);
            dayEnd = DateTime.Parse(sLast);
        }
        public static void GetWeekStartEnd(DateTime myDate, out DateTime weekStart, out DateTime weekEnd)
        {
            int day = myDate.Day;

            int minute = myDate.Minute;
            int hour = myDate.Hour;
            if (day == 1 && hour == 0 && minute == 0)
            {
                // Still in previous time
                myDate = myDate.AddMinutes(-1);
            }


            DateTime Monday = myDate.AddDays(-(int)myDate.DayOfWeek + (int)DayOfWeek.Monday);
            DateTime Sunday = myDate.AddDays(-(int)myDate.DayOfWeek + 8); //Acually the Monday 

            string MondayY = Monday.Year.ToString();
            string MondayM = Monday.Month.ToString();
            string MondayD = Monday.Day.ToString();
            string sMondayY = MondayY.ToString();

            if (MondayM.Length == 1) { MondayM = "0" + MondayM; }
            if (MondayD.Length == 1) { MondayD = "0" + MondayD; }
            string sWeekStart = (MondayY + "/" + MondayM + "/" + MondayD + " 00:30:00");

            string SundayY = Sunday.Year.ToString();
            string SundayM = Sunday.Month.ToString();
            string SundayD = Sunday.Day.ToString();
            string sSundayY = SundayY.ToString();

            if (SundayM.Length == 1) { SundayM = "0" + SundayM; }
            if (SundayD.Length == 1) { SundayD = "0" + SundayD; }

            string sWeekEnd = (SundayY + "/" + SundayM + "/" + SundayD + " 00:00:00");

            weekStart = DateTime.Parse(sWeekStart);
            weekEnd = DateTime.Parse(sWeekEnd);
        }
    }
}
