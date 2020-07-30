using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neura.Billing.Data;
using static Neura.Billing.GlobalVar;
using System.Windows.Forms;

namespace Neura.Billing.TariffCalcs
{
    class PreviousValues
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void GetPreviousTOU(int NodeId, DateTime receivedDate, int myMeteringInterval, int myInterval,
            out double cAcc, out double uAcc)
        {
            DateTime prevDate = receivedDate.AddMinutes(-myMeteringInterval);
            int count = UtilityConnections.GetPreviousTou(NodeId, prevDate, myInterval, out DataTable dtPrev);
            if (count == 0)
            {
                cAcc = uAcc = 0;
                if (bLogTest == true)
                {
                    Log.Info("No previous TOU values found");
                }

            }
            else if (count == 1)
            {
                if (receivedDate.Day == 1 && receivedDate.Hour == 0 && receivedDate.Minute == 30)  //First day of month
                {

                    cAcc = uAcc = 0;
                    if (bLogTest == true)
                    {
                        Log.Info("Start new month. Previous values set to 0. ======================");
                    }
                }
                else
                {
                    cAcc = Convert.ToDouble(dtPrev.Rows[0]["cAcc"]);
                    uAcc = Convert.ToDouble(dtPrev.Rows[0]["uAcc"]);
                }

            }
            else
            {
                //Problem
                MessageBox.Show("Multiple previous data found");
                cAcc = uAcc = 0;
            }

        }

        public static void GetPreviousValues(int NodeId, DateTime receivedDate, int myMeteringInterval,
            out double cFixed, out double cDemand, out double cAcc,
             out double uDemand, out double uAcc, out double uPeak)
        {
            DateTime prevDate = receivedDate.AddMinutes(-myMeteringInterval);
            int count = UtilityConnections.GetPreviousValues(NodeId, prevDate, out DataTable dtPrev);
            if (count == 0)
            {
                cFixed = cDemand = cAcc = uDemand = uAcc = uPeak = 0;
                if (bLogTest == true)
                {
                    Log.Info("No previous values found");
                }

            }
            else if (count == 1)
            {
                if (receivedDate.Day == 1 && receivedDate.Hour == 0 && receivedDate.Minute == 30)  //First day of month
                {

                    cFixed = cDemand = cAcc = uDemand = uAcc = uPeak = 0;
                    if (bLogTest == true)
                    {
                        Log.Info("Start new month. Previous values set to 0. ======================");
                    }
                }
                else
                {
                    cFixed = Convert.ToDouble(dtPrev.Rows[0]["cFixed"]);
                    cDemand = Convert.ToDouble(dtPrev.Rows[0]["cMaximum"]);
                    cAcc = Convert.ToDouble(dtPrev.Rows[0]["cAcc"]);
                    uPeak = Convert.ToDouble(dtPrev.Rows[0]["uPeak"]);
                    uDemand = Convert.ToDouble(dtPrev.Rows[0]["uMaximum"]);
                    uAcc = Convert.ToDouble(dtPrev.Rows[0]["uAcc"]);
                }

            }
            else
            {
                //Problem
                MessageBox.Show("Multiple previous data found");
                cFixed = cDemand = cAcc = uDemand = uAcc = uPeak = 0;
            }

        }
    }
}
