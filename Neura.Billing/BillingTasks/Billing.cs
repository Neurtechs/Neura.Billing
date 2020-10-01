using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data.Helpers;
using Neura.Billing.AICalcs;
using Neura.Billing.TariffCalcs;
using static Neura.Billing.GlobalVar;

namespace Neura.Billing.BillingTasks
{
    public class Billing
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void RunCalcs(bool bCheckLimit, int limit, bool bLogTest, bool bLogResult, CancellationToken ct, out List<string> listItems)
        {
            listItems = new List<string>();
            if (ct.IsCancellationRequested)
            {
                //MessageBox.Show("Task " + taskNum + " was cancelled before it got started.");
                listItems.Add("Task  was cancelled before it got started.");

                ct.ThrowIfCancellationRequested();
            }
            if (mySqlConnection.State == ConnectionState.Closed)
            {
                try
                {
                    mySqlConnection.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Check you connection string and comms to DB");
                    MessageBox.Show(ex.Message);
                    goto ExitHere;
                }
            }
            listItems.Add("Started at " + DateTime.Now.ToString());
            listItems.Add("-------------------------------");
            if (bLogTest == true)
            {
                Log.Info("Process started at " + DateTime.Now.ToString());
                Log.Info("==========================");
            }

            int incomming;
            if (bCheckLimit == true)
            {
                incomming = Verify.VerifyIncoming(MeteringInterval, limit);
            }
            else
            {
                incomming = Verify.VerifyIncoming(MeteringInterval, 1000);
            }

            listItems.Add("Number of new readings: " + incomming);

            if (bLogTest == true)
            {
                Log.Info("");
                Log.Info("Number of new readings this loop: " + incomming);
                Log.Info("----------------------------------------------");
            }

            int inreadings = ManageIncoming.GetIncomingReadings(MeteringInterval);
            listItems.Add("Number of new usage records: " + inreadings);

            if (bLogTest == true)
            {
                Log.Info("Number of new usage records: " + inreadings);
                Log.Info("----------------------------------------------");
            }

            TariffMain.GetCosts(MeteringInterval, out int processedGroups);
            listItems.Add("Number of tariff processed groups: " + processedGroups);
            listItems.Add("");

            if (bLogTest == true)
            {
                Log.Info("Number of tariff processed groups: " + processedGroups);
                Log.Info("----------------------------------------------");
            }

            if (mySqlConnection.State == ConnectionState.Open)
            {
                mySqlConnection.Close();
            }

            ExitHere: ;

        }
    }
}
