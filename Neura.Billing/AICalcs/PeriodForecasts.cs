using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neura.Billing.Data;
using static Neura.Billing.GlobalVar;
using System.Data;

namespace Neura.Billing.AICalcs
{
    class PeriodForecasts
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void CalcValue(int nodeId, DateTime myDateTime, double W1, double W2, double W3, double W4, double W5, double W6, int myMeteringPeriod, double myAverage,
            DataTable dtPeriodValues, DataTable dtTemplate, double aveConsumption, out double myForecast)
        {
            myForecast = 0;
            //go back 1 period
            double[] wValue = new double[7];
            wValue[1] = Convert.ToDouble(dtPeriodValues.Rows[dtPeriodValues.Rows.Count - 1]["uAcc"]); // last value
            int periods = 60 * 24 / myMeteringPeriod; //periods per day
            DateTime newDate = myDateTime;
            //go back 1 day

            wValue[2] = Convert.ToDouble(dtPeriodValues.Rows[dtPeriodValues.Rows.Count - (periods + 1)]["uAcc"]);

            //go back 1 week
            if (dtPeriodValues.Rows.Count >= 7 * periods)
            {
                wValue[3] = Convert.ToDouble(dtPeriodValues.Rows[dtPeriodValues.Rows.Count - (7 * periods + 1)]["uAcc"]);
            }
            else
            {
                //Get template values
                newDate = newDate.AddDays(-7);
                string filter = "Timestamp = '" + newDate + "'";
                DataRow[] drf = dtTemplate.Select(filter);
                double templateConsumption = Convert.ToDouble(drf[0]["Consumption"]);
                wValue[3] = templateConsumption * (myAverage / aveConsumption);

                //wValue[3] = GetvW(newDate, dtTemplate,myAverage);
            }

            // go back 2 weeks
            if (dtPeriodValues.Rows.Count >= 14 * periods)
            {
                wValue[4] = Convert.ToDouble(dtPeriodValues.Rows[dtPeriodValues.Rows.Count - (14 * periods + 1)]["uAcc"]);
            }
            else
            {
                //Get template values
                newDate = newDate.AddDays(-14);
                string filter = "Timestamp = '" + newDate + "'";
                DataRow[] drf = dtTemplate.Select(filter);
                double templateConsumption = Convert.ToDouble(drf[0]["Consumption"]);
                wValue[4] = templateConsumption * (myAverage / aveConsumption);
                //wValue[4] = GetvW(newDate, dtTemplate, myAverage);
            }

            // go back 52 weeks
            if (dtPeriodValues.Rows.Count >= 52 * 7 * periods)
            {
                wValue[5] = Convert.ToDouble(dtPeriodValues.Rows[dtPeriodValues.Rows.Count - (52 * 7 * periods + 1)]["uAcc"]);
            }
            else
            {
                //Get template values
                newDate = newDate.AddDays(-(7 * 52));
                string filter = "Timestamp = '" + newDate + "'";
                DataRow[] drf = dtTemplate.Select(filter);
                double templateConsumption = Convert.ToDouble(drf[0]["Consumption"]);
                wValue[5] = templateConsumption * (myAverage / aveConsumption);
                //wValue[5] = GetvW(newDate, dtTemplate, myAverage);
            }

            // go back 104 weeks
            if (dtPeriodValues.Rows.Count >= 104 * 7 * periods)
            {
                wValue[6] = Convert.ToDouble(dtPeriodValues.Rows[dtPeriodValues.Rows.Count - (104 * 7 * periods + 1)]["uAcc"]);
            }
            else
            {
                //Get template values
                newDate = newDate.AddDays(-(7 * 104));
                string filter = "Timestamp = '" + newDate + "'";
                DataRow[] drf = dtTemplate.Select(filter);
                double templateConsumption = Convert.ToDouble(drf[0]["Consumption"]);
                wValue[6] = templateConsumption * (myAverage / aveConsumption);
                //wValue[6] = GetvW(newDate, dtTemplate, myAverage);
            }
            myForecast = W1 * wValue[1] + W2 * wValue[2] + W3 * wValue[3] + W4 * wValue[4] + W5 * wValue[5] + W6 * wValue[6];
        }
        private static double GetvW(DateTime myDate, DataTable dtTemplate, double myAverage)
        {

            double aveConsumption = 0;
            double templateConsumption = 0;
            string filter = "";
            int _month;
            int _year;
            int _daysInMonth;
            _month = myDate.Month;
            _year = myDate.Year;
            _daysInMonth = DateTime.DaysInMonth(_year, _month);
            aveConsumption = AIConnections.GetSumTemplate(_month, _year);
            aveConsumption = aveConsumption / _daysInMonth; //daily average from template
            filter = "Timestamp = '" + myDate + "'";
            DataRow[] drf = dtTemplate.Select(filter);
            templateConsumption = Convert.ToDouble(drf[0]["Consumption"]);
            templateConsumption = templateConsumption * (myAverage / aveConsumption);
            return templateConsumption;

        }
    }
}
