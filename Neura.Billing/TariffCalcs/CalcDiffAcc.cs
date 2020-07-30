using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Neura.Billing.GlobalVar;
using Neura.Billing.Data;

namespace Neura.Billing.TariffCalcs
{
    class CalcDiffAcc
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static double usageDelta(int NodeId, int readingsType, DateTime readingDate, double myReading, int myMeteringInterval)
        {
            double usage = 0;
            double readingPrev = 0;
            DateTime prevReading = readingDate.AddMinutes(-myMeteringInterval);
            //UtilityConnections.SelectReadingByReadingDate(NodeId, readingsType, prevReading, out DataTable dtPrevIntermediale);
            UtilityConnections.SelectReadingByReadingDate(NodeId, readingsType, prevReading, out readingPrev);
            if (readingPrev == 0)
                //if (dtPrevIntermediale.Rows.Count == 0)
            {
                //Get Start Reading
                //Find Start Readings
                UtilityConnections.GetStartReading(NodeId, readingsType, out double myStartReading);
                readingPrev = myStartReading;
                if (bLogTest == true)
                {
                    Log.Info("For Node = " + NodeId + " ReadingsType = " + readingsType + " ===========");
                    Log.Info("No previous readings found. Attempting to get start readings ------");
                    Log.Info("Start Reading= " + myStartReading);

                }
            }
            else
            {
                //readingPrev = Convert.ToDouble(dtPrevIntermediale.Rows[0]["Reading"]);
            }
            usage = myReading - readingPrev;
            return usage;

        }
    }
}
