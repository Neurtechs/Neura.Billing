using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Neura.Billing.GlobalVar;
using System.Data;
using Neura.Billing.Data;

namespace Neura.Billing.TariffCalcs
{
    class BlockTariff
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void GetBlockTariff(DateTime myReadingDate, double cPrevAcc, double uPrevAcc, double myConsumed, int myReadingsType,
            int myFloor, int myCeiling, double myRate, int tariffId, int nodeId, out double cAcc, out double uAcc, bool bSave = true)
        {
            //Check if the current block component applies
            double uToDateAcc = uPrevAcc + myConsumed;
            double cToDateAcc = 0;

            if (myCeiling == 0)
            {
                //last block
                if (uToDateAcc <= myFloor)
                {
                    //Not in block
                    cAcc = 0;
                    uAcc = 0;
                    return;
                }

            }
            else if (!((uToDateAcc <= myCeiling) && (uToDateAcc > myFloor)))
            {
                //Not in block
                cAcc = 0;
                uAcc = 0;
                return;
            }

            if (bLogTest == true)
            {
                Log.Info("");
                Log.Info("--------------Block tariff calculation--------------");
                Log.Info("Previous usage = " + uPrevAcc);
                Log.Info("Additional usage = " + myConsumed);
                Log.Info("Previous cost = " + cPrevAcc);
            }
            //Get block count

            int minute = myReadingDate.Minute;

            int day = myReadingDate.Day;
            int hour = myReadingDate.Hour;
            DateTime tempDateTime = myReadingDate;

            if (day == 1 && hour == 0 && minute == 0) { tempDateTime = tempDateTime.AddMinutes(-1); }
            int countBlock = UtilityConnections.GetBlockComponents(tariffId, tempDateTime, out DataTable dtBlocks);
            double[] cons = new double[countBlock];
            double[] cost = new double[countBlock];
            double[] rate = new double[countBlock];
            int[] floor = new int[countBlock];
            int[] ceiling = new int[countBlock];
            int newFloor = 0;
            double myValue = uToDateAcc;
            MonthStartEnd.GetMonthStartEnd(myReadingDate, out DateTime monthStart, out DateTime monthEnd);
            for (int i = 0; i < countBlock; i++)
            {
                newFloor = Convert.ToInt16(dtBlocks.Rows[i]["Floor"]);
                floor[i] = Convert.ToInt16(dtBlocks.Rows[i]["Floor"]);
                ceiling[i] = Convert.ToInt16(dtBlocks.Rows[i]["Ceiling"]);
                if (myValue > newFloor)
                {
                    cons[i] = myValue - newFloor;
                    myValue = newFloor;
                    rate[i] = Convert.ToDouble(dtBlocks.Rows[i]["Rate"]);
                    cost[i] = cons[i] * rate[i];
                    cToDateAcc += cost[i];
                    if (bLogTest == true)
                    {
                        Log.Info("Updating block with floor " + newFloor);
                        Log.Info("Rate = " + rate[i]);
                        Log.Info("Usage = " + cons[i]);
                        Log.Info("Cost = " + cost[i]);
                    }

                    //Save the values to the block tariff
                    if (bSave == true)
                    {
                        SaveConnections.SaveBlockValues(nodeId, floor[i], ceiling[i], myReadingDate, monthStart,
                            monthEnd, cost[i], cons[i]);
                    }

                }

            }
            cAcc = cToDateAcc - cPrevAcc;
            uAcc = uToDateAcc - uPrevAcc;
            if (bLogTest == true)
            {

                Log.Info("Cost to date = " + cToDateAcc);
            }
        }
    }
}
