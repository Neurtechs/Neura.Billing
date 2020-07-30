using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neura.Billing.Data;
using static Neura.Billing.GlobalVar;

namespace Neura.Billing.TariffCalcs
{
    public class Energy
    {
        private static readonly log4net.ILog Log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void GetEnergy(Double myRate, int myFlow, int mySeason, int myInterval, int myMeasurement,
            int myReadingType, double myNetUsage, int nodeId,
            DateTime myReadingDate, int myLookupId, double cPreviousAcc, double uPreviousAcc, int myMeteringInterval,
            out double uAcc, out double cAcc, out double uAccToDate, out double cAccToDate)
        {

            //Meaning of values: 0, 1, 2 etc.
            //public enum Measurement { Accumulated, Maximum, Fixed, MaxProgressive }
            //public enum Interval { Peak, Standard, OffPeak, All, PeakAndStandard, NonEnergy }
            //public enum Flow { NotApplicable, Import, Export, Consumption, Generate, NetImportExport, NetImportExportGenerate }
            //public enum Period { Account, Day, Month }
            //public enum Season { High, Low, All, NotApplicable }
            //public enum DayOfWeek { Weekdays, Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }
            uAcc = myNetUsage;
            cAcc = uAcc * myRate;
            uAccToDate = uPreviousAcc + uAcc;
            cAccToDate = cPreviousAcc + cAcc;



            if (bLogTest == true)
            {
                Log.Info("");
                Log.Info("--------------Accumulative tariff calculation--------------");
                Log.Info("Usage Previous = " + uPreviousAcc);
                Log.Info("Cost Previous = " + cPreviousAcc);
                Log.Info("Usage = " + uAcc);
                Log.Info("Rate = " + myRate);
                Log.Info("Cost = " + cAcc);

                Log.Info("Usage to Date = " + uAccToDate);
                Log.Info("Cost to Date = " + cAccToDate);
            }

            if (myInterval < 3) //TOU
            {
                PreviousValues.GetPreviousTOU(nodeId, myReadingDate, myMeteringInterval, myInterval,
                    out double prevcAcc, out double prevuAcc);
                double cTouToDate = prevcAcc + cAcc;
                double uTouToDate = prevuAcc + uAcc;

                if (bLogTest == true) { Log.Info("Saving to TOU Table for interval " + myInterval); }
                MonthStartEnd.GetMonthStartEnd(myReadingDate, out DateTime monthStart, out DateTime monthEnd);
                //Save the values to the TOU Table
                SaveConnections.SaveTouValues(nodeId, myInterval, myReadingDate, monthStart, monthEnd, cTouToDate, uTouToDate);
            }
        }
    }
}
