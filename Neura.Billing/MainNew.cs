using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Neura.Billing
{
    public partial class MainNew : DevExpress.XtraEditors.XtraForm
    {
        private static bool bStopBilling = false;
        private static ListBoxControl lb;
        private static CancellationTokenSource cSource;
        private static bool bCheckLimit; 
        private static int limit;
        private static bool bLogTest;
        private static bool bLogResult;
        public static List<string> listItems { get; set; }

        public MainNew()
        {
            InitializeComponent();
        }
        private void MainNew_Load(object sender, EventArgs e)
        {
            lb = listBoxControl1;
            cSource = new CancellationTokenSource();
            listItems = new List<string>();
        }
        private void simpleButtonStartBilling_Click(object sender, EventArgs e)
        {
            //cSource = new CancellationTokenSource();
            //bStopBilling = false;
            limit = Convert.ToInt32(textEditLimit.Text);
            lb = listBoxControl1;
            listItems = new List<string>();

            lb.Items.Clear();
            lb.Refresh();
            //BillingMain();
            lb.DataSource = listItems;
            BillingMain();
            lb.Refresh();
            timerBilling.Interval = 10*1000;
            timerBilling.Start();
           
           
        }

        public static async Task BillingMain()
        {
            cSource = new CancellationTokenSource();
            var token = cSource.Token;
            int countCalcs = 0;
            // Store references to the tasks so that we can wait on them and
            // observe their status after cancellation.
            Task t;
            var tasks = new ConcurrentBag<Task>();

            t = Task.Run(() => RunCalcs(1,token, out countCalcs), token);
            //MessageBox.Show("Task " + t.Id + " executing");
            listItems.Add("Task " + t.Id + " executing");
            
            tasks.Add(t);
          
            t = Task.Run(() =>
            {
                // Create some cancelable child tasks.
                Task tc;
                for (int i = 3; i <= 10; i++)
                {
                    // For each child task, pass the same token
                    // to each user delegate and to Task.Run.
                    tc = Task.Run(() => RunCalcs(2,token, out countCalcs), token);
                    listItems.Add("Task " + tc.Id + " executing");
                   
                    //MessageBox.Show("Task " + tc.Id + " executing");
                    tasks.Add(tc);
                    // Pass the same token again to do work on the parent task.
                    // All will be signaled by the call to tokenSource.Cancel below.
                    RunCalcs(2,token, out  countCalcs);
                }
            }, token);
            tasks.Add(t);
            if (bStopBilling == true)
            {
                cSource.Cancel();
                listItems.Add("\nTask cancellation requested.");
               
                //MessageBox.Show("\nTask cancellation requested.");
            }
            try
            {
                await Task.WhenAll(tasks.ToArray());
            }
            catch (Exception e)
            {
                //MessageBox.Show($"\n{nameof(OperationCanceledException)} thrown\n");
                //listItems.Add($"\n{nameof(OperationCanceledException)} thrown\n");
                MessageBox.Show(e.Message);
                
            }
            finally
            {
                cSource.Dispose();
            }
        }

        static void RunCalcs(int taskNum, CancellationToken ct, out int countCalcs)
        {
            countCalcs = 5;
            // Was cancellation already requested?
            if (ct.IsCancellationRequested)
            {
                //MessageBox.Show("Task " + taskNum + " was cancelled before it got started.");
                listItems.Add("Task " + taskNum + " was cancelled before it got started.");
              
                ct.ThrowIfCancellationRequested();
            }

            int maxIterations = 100;

            // NOTE!!! A "TaskCanceledException was unhandled
            // by user code" error will be raised here if "Just My Code"
            // is enabled on your computer. On Express editions JMC is
            // enabled and cannot be disabled. The exception is benign.
            // Just press F5 to continue executing your code.
            for (int i = 0; i <= maxIterations; i++)
            {
                // Do a bit of work. Not too much.
                var sw = new SpinWait();
                for (int j = 0; j <= 100; j++)
                {
                    sw.SpinOnce();
                    
                }

                if (ct.IsCancellationRequested)
                {
                    //MessageBox.Show("Task " + taskNum + " cancelled");
                   listItems.Add("Task " + taskNum + " cancelled.");
                  
                    ct.ThrowIfCancellationRequested();
                }
            }
        }

        private void simpleButtonStopBilling_Click(object sender, EventArgs e)
        {
            try
            {
                cSource.Cancel();
            }
            catch (Exception exception)
            {
              
            }
           timerBilling.Stop();
            listItems.Add("Stopped by user");
           lb.Refresh();
        }

        private void timerBilling_Tick(object sender, EventArgs e)
        {
           
            RunBilling();
            lb.Refresh();
        }

        public static async Task RunBilling()
        {
            cSource = new CancellationTokenSource();
            var token = cSource.Token;
            Task t;
            //var tasks = new ConcurrentBag<Task>();
            
            //t = Task.Run(() => RunTest( token), token);
            t = Task.Run(() => BillingTasks.Billing.RunCalcs(bCheckLimit,
                limit,bLogTest,bLogResult,token,out List<string> listItems), token);
            //tasks.Add(t);

            try
            {
                await t;
            }
            catch (Exception e)
            {
                //MessageBox.Show($"\n{nameof(OperationCanceledException)} thrown\n");
                //listItems.Add($"\n{nameof(OperationCanceledException)} thrown\n");
                MessageBox.Show(e.Message);

            }
            finally
            {
                cSource.Dispose();
            }
        }

        static void RunTest(CancellationToken ct)
        {
            
            if (ct.IsCancellationRequested)
            {
                //MessageBox.Show("Task " + taskNum + " was cancelled before it got started.");
                listItems.Add("Task  was cancelled before it got started.");

                ct.ThrowIfCancellationRequested();
            }
            int maxIterations = 10;
           
            for (int i = 0; i <= maxIterations; i++)
            {
                // Do a bit of work. Not too much.
                var sw = new SpinWait();
                for (int j = 0; j <= 100; j++)
                {
                    sw.SpinOnce();

                }
                listItems.Add("Iteration " + i);
                
                if (ct.IsCancellationRequested)
                {
                    //MessageBox.Show("Task " + taskNum + " cancelled");
                    listItems.Add("Task  cancelled.");

                    ct.ThrowIfCancellationRequested();
                }
            }
        }

        private void checkLimit_CheckedChanged(object sender, EventArgs e)
        {
            if (checkLimit.Checked == true)
            {
                bCheckLimit = true;
            }
            else
            {
                bCheckLimit = false;
            }
        }
    }
}