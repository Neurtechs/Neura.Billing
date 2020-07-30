using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using static Neura.Billing.GlobalVar;
using Neura.Billing.TariffCalcs;
using System.Collections;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Neura.Billing.Data;
using Neura.Billing.AICalcs;
using Neura.Billing.Calls;

namespace Neura.Billing
{
    public partial class Main : DevExpress.XtraEditors.XtraForm
    {

        public static List<string> listItems { get; set; }
        int tickCount = 1;
        private DateTime maxDate;
        private DataTable dtNodesWithData;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            timer1.Interval = Convert.ToInt32(textEdit1.Text) * 1000;
            w1.Text = "0.3";
            w2.Text = "0.1";
            w3.Text = "0.25";
            w4.Text = "0.2";
            w5.Text = "0.1";
            w6.Text = "0.05";
            total.Text = "1";
            lWarning.Visible = false;
            MeteringInterval = 30;
            AIConnections.GetNodesWithData(out dtNodesWithData);
            foreach (DataRow dr in dtNodesWithData.Rows)
            {
                cbNode.Properties.Items.Add(dr["Node"]);
            }

            try
            {
                maxDate = Convert.ToDateTime(dtNodesWithData.Rows[0]["MaxDate"]);
                maxDate = maxDate.AddMinutes(MeteringInterval);
                cbNode.SelectedIndex = 0;
                txtStart.Text = Convert.ToString(maxDate);
                cmbInterval.SelectedIndex = 0;
                comboBoxEditType.SelectedIndex = 0;
                comboBoxEditOnOff.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }


        }

        private void simpleButtonStart_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            bLogTest = false;
            bLogResult = false;
            if (checkEditLogTest.Checked == true) { bLogTest = true; }
            if (checkEditResult.Checked == true) { bLogResult = true; }

            if (bLogTest == true || bLogResult == true)
            {

                Log.Info("Running Project Neura.Billing Test");
                Log.Info("------------------------------------------------------------------------");
                Log.Info(DateTime.Now.ToString());
                Log.Info("");

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

            listItems = new List<string>();

            listItems.Add("Started at " + DateTime.Now.ToString());
            listItems.Add("-------------------------------");
            if (bLogTest == true)
            {
                Log.Info("Process started at " + DateTime.Now.ToString());
                Log.Info("==========================");
            }

            int incomming = Verify.VerifyIncoming(MeteringInterval);
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

            listBoxControl1.DataSource = listItems;
            listBoxControl1.Refresh();

            if (bLogTest == true)
            {
                Log.Info("Number of tariff processed groups: " + processedGroups);
                Log.Info("----------------------------------------------");
            }

            if (mySqlConnection.State == ConnectionState.Open)
            {
                mySqlConnection.Close();
            }

            Cursor.Current = Cursors.Default;
            timer1.Start();
            ExitHere:;
        }

        private void simpleButtonStop_Click(object sender, EventArgs e)
        {
            listItems.Add("Stopped by User at " + DateTime.Now);
            listBoxControl1.Refresh();
            if (bLogTest == true)
            {
                Log.Info("Stopped by User at " + DateTime.Now);
                Log.Info("----------------------------------------------");
            }
            timer1.Stop();
        }

        private void btnTest_Click(object sender, EventArgs e)
        { ////mySqlConnection.Open();

            //string str = "Select * from MonthValues";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);
            //DataTable dt = new DataTable();
            //da.Fill(dt);
            //double data = Convert.ToDouble(dt.Rows[0]["cAcc"]);
            //mySqlConnection.Close();

            //For Insert
            //MySqlCommand cmd = new MySqlCommand("SelectNode", mySqlConnection);

            //cmd.Parameters.AddWithValue("nodeId", 1);
            //cmd.Parameters.AddWithValue("readingsType", 1);
            //cmd.Parameters.AddWithValue("reading", 100);
            //cmd.Parameters.AddWithValue("timestamp", DateTime.Now );
            //cmd.Parameters.AddWithValue("UTC", 1);
            //mySqlConnection.Open();
            //cmd.ExecuteNonQuery();
            //mySqlConnection.Close();

            //MySqlCommand cmd = new MySqlCommand("SelectNode", mySqlConnection);
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("NodeId", 1);
            //cmd.Parameters.AddWithValue("ReadingsType", 1);

            //MySqlDataAdapter da = new MySqlDataAdapter();
            //da.SelectCommand = cmd;
            //DataTable dt = new DataTable();
            //da.Fill(dt);
            //MessageBox.Show(Convert.ToString(dt.Rows[0]["NodeName"]));

            //MySqlCommand cmd = new MySqlCommand("CountMonthValues", mySqlConnection);
            //cmd.CommandType = CommandType.StoredProcedure;

            //int node = 10;
            //DateTime monthStart = Convert.ToDateTime("2014/09/01");
            //DateTime monthEnd = Convert.ToDateTime("2014/10/01");
            //cmd.Parameters.AddWithValue("_node", node);
            //cmd.Parameters.AddWithValue("_monthEnd", monthEnd);
            //cmd.Parameters.AddWithValue("_monthStart", monthStart);
            //cmd.Parameters.Add("_count", MySqlDbType.Int16);
            //cmd.Parameters["_count"].Direction = ParameterDirection.Output;
            //cmd.ExecuteNonQuery();
            //string myStr =Convert.ToString( cmd.Parameters["_count"].Value);
            //MessageBox.Show(myStr );


            //MySqlCommand cmd = new MySqlCommand("SelectIntermediateByReadingDate", mySqlConnection);
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("_NodeId", 10);
            //cmd.Parameters.AddWithValue("_ReadingsType", 1);
            //cmd.Parameters.AddWithValue("_readingDate", Convert.ToDateTime("2014/09/16 17:00:00"));
            //cmd.Parameters.Add("_reading", MySqlDbType.Decimal);
            //cmd.Parameters.Add("_tempDate", MySqlDbType.DateTime);
            //cmd.Parameters["_reading"].Direction = ParameterDirection.Output;
            //cmd.Parameters["_tempDate"].Direction = ParameterDirection.Output;
            //cmd.ExecuteNonQuery();
            //string myReading = Convert.ToString(cmd.Parameters["_reading"].Value);
            //string myTempDate = Convert.ToString(cmd.Parameters["_tempDate"].Value);
            //MessageBox.Show(myReading + "," + myTempDate);

            //string str = "Update Readings1 Set Comment = 1 WHERE TimeStamp < '2015/01/01'";
            ////    "AND Node = " + Node;
            //MySqlCommand cmd = new MySqlCommand(str, mySqlConnection);
            //cmd.ExecuteNonQuery();

            //MySqlCommand cmd = new MySqlCommand("GNodeChangeStatus", mySqlConnection);
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("_serialNo", 10);
            //cmd.Parameters.AddWithValue("_timeStamp", DateTime.Now);
            //cmd.Parameters.AddWithValue("_value", 2.5);
            //cmd.Parameters.AddWithValue("_source", 0);
            //cmd.ExecuteNonQuery();

            decimal myHeating = 0;
            decimal myCooling = 0;
            DateTime myTimestamp = DateTime.Now;
            DateTime _timeStamp = DateTime.Now;
            
            decimal _value = 10;
            int myOid = 10;
            int _source = 1;
            string str = "Select Timestamp into " + myTimestamp + " from DEHWStatus Where Node = " + myOid;
            MySqlCommand cmd = new MySqlCommand(str, mySqlConnection);
            cmd.ExecuteNonQuery();

            if (_value > 0)
            {
                var xVal = (_timeStamp - myTimestamp).TotalSeconds;
                myCooling = Convert.ToDecimal(-5 / xVal);
            }
            else
            {
                var xVal = (_timeStamp - myTimestamp).TotalSeconds;
                myCooling = Convert.ToDecimal(5 / xVal);
            }
            str = "	update DEHWStatus Set TimeStamp = '" + _timeStamp + "', Value = " + _value + ", HeatingGrad = " + myHeating +
                    ", CoolingGrad = " + myCooling + ", Source = " + _source +
                    " WHERE Oid = " + myOid;
            cmd = new MySqlCommand(str, mySqlConnection);
            cmd.ExecuteNonQuery();

        }

        private void updateTotal()
        {
            double t = Convert.ToDouble(w1.Text) + Convert.ToDouble(w2.Text) +
                       Convert.ToDouble(w3.Text) + Convert.ToDouble(w4.Text) + Convert.ToDouble(w5.Text) + Convert.ToDouble(w6.Text);
            total.Text = Convert.ToString(t);
            if (t == 1)
            {
                total.ForeColor = Color.DarkGreen;
                lWarning.Visible = false;
            }
            else
            {
                total.ForeColor = Color.Red;
                lWarning.Visible = true;
            }
        }


        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            DataTable dtGetLastPeriodValues;
            DataTable dtTemplate;
            DateTime myTime = Convert.ToDateTime(txtStart.Text);
            int myNodeId = Convert.ToInt32(cbNode.Text);
            AIConnections.GetLastPeriodValues(myNodeId, myTime, out dtGetLastPeriodValues);
            AIConnections.GetTemplate(out dtTemplate);
            listItems = new List<string>();
            listBoxControl1.Items.Clear();
            listBoxControl1.Refresh();
            listItems.Add("Started at " + DateTime.Now.ToString());
            listItems.Add("-------------------------------");
            listItems.Add("");

            int myCount = dtGetLastPeriodValues.Rows.Count;
            listItems.Add("Node " + myNodeId + " previous records found = " + myCount);
            listBoxControl1.DataSource = listItems;

            decimal days = (myCount / MeteringInterval);
            int myDays = Convert.ToInt32(Math.Truncate(days));
            if (myDays < 1)
            {
                listItems.Add("Cannot proceed with less than 1 days worth of data");
                goto ExitHere;
            }
            listBoxControl1.Refresh();

            if (myDays > 14) { myDays = 14; }
            int myPeriods = myDays * MeteringInterval;

            double totUAcc = 0;
            DataTable dtLastX = dtGetLastPeriodValues.AsEnumerable().Reverse().Take(myPeriods).CopyToDataTable();
            double myAverage = 0;
            foreach (DataRow dr in dtLastX.Rows) { myAverage += Convert.ToDouble(dr["uAcc"]); }
            myAverage = myAverage / myDays;
            listItems.Add("Average daily consumption =  " + myAverage);
            //listBoxControl1.Refresh();

            MonthStartEnd.GetMonthStartEnd(myTime, out DateTime monthStart, out DateTime monthEnd);
            MonthStartEnd.GetDayStartEnd(myTime, out DateTime dayStart, out DateTime dayEnd);
            MonthStartEnd.GetWeekStartEnd(myTime, out DateTime weekStart, out DateTime weekEnd);
            AIConnections.GetMonthUAcc(myNodeId, monthStart, out DataTable dtMonthValues); //Consumption to date

            double cPreviousFixed = Convert.ToDouble(dtMonthValues.Rows[0]["cFixed"]);
            double cPreviousMaximum = Convert.ToDouble(dtMonthValues.Rows[0]["cMaximum"]);
            double cPreviousAcc = Convert.ToDouble(dtMonthValues.Rows[0]["cAcc"]);
            double uPreviousMaximum = Convert.ToDouble(dtMonthValues.Rows[0]["uMaximum"]);
            double uPreviousPeak = Convert.ToDouble(dtMonthValues.Rows[0]["uPeak"]);
            double uPreviousAcc = Convert.ToDouble(dtMonthValues.Rows[0]["uAcc"]);


            int periodsToGo = 0;
            TimeSpan duration;
            if (cmbInterval.SelectedIndex == 0) //rest of day
            {
                //periodsToGo = ((myTime - dayEnd).Minutes) / MeteringInterval;
                duration = dayEnd - myTime;
                periodsToGo = Convert.ToInt32(duration.TotalMinutes / MeteringInterval) + 1;
            }
            else if (cmbInterval.SelectedIndex == 1) // rest of week
            {
                //periodsToGo = (( weekEnd-myTime).Minutes) / MeteringInterval;
                duration = weekEnd - myTime;
                periodsToGo = Convert.ToInt32(duration.TotalMinutes / MeteringInterval) + 1;
            }
            else //Rest if month
            {
                //periodsToGo = ((myTime - monthEnd).Minutes) / MeteringInterval;
                duration = monthEnd - myTime;
                periodsToGo = Convert.ToInt32(duration.TotalMinutes / MeteringInterval) + 1;
            }

            totUAcc = uPreviousAcc;

            listItems.Add("Consumption to date for month = " + totUAcc + " kWh");
            listItems.Add("Forecasting for  = " + periodsToGo + " periods");

            //Get Aveage consumption from template
            double aveConsumption = 0;


            int _month;
            int _year;
            int _daysInMonth;
            _month = myTime.Month;
            _year = myTime.Year;
            _daysInMonth = DateTime.DaysInMonth(_year, _month);
            aveConsumption = AIConnections.GetSumTemplate(_month, _year);
            aveConsumption = aveConsumption / _daysInMonth; //daily average from template

            AIConnections.GetNodeBasic(myNodeId, out int myTariff, out int myMeterType, out int ReadingsType);

            //double cAccToDate = 0;
            //double cFixedToDate = 0;
            //double cMaxToDate = 0;

            int componentCount = TariffComponents.GetTariffComponents(myTariff, myTime, out DataTable dtComponents);
            int TOULookupId = UtilityConnections.SelectTOULookup(myTariff);

            //Deal with reading types
            int readingTypesCount = UtilityConnections.SelectUsageByNodeTariff(myNodeId, myTariff,
                myTime, out DataTable dtUsage);

            double runningTotUacc = 0;
            double runningTotCost = 0;
            double totCAcc = cPreviousAcc + cPreviousFixed + cPreviousMaximum;

            double cAcc = 0;
            double cFixed = 0;
            double cMax = 0;
            DataRow newRow;

            for (int i = 0; i < periodsToGo; i++)
            {
                //Forecast Consumption
                PeriodForecasts.CalcValue(myNodeId, myTime, Convert.ToDouble(w1.Text), Convert.ToDouble(w2.Text), Convert.ToDouble(w3.Text),
                     Convert.ToDouble(w4.Text), Convert.ToDouble(w5.Text), Convert.ToDouble(w6.Text), MeteringInterval, myAverage, dtGetLastPeriodValues,
                     dtTemplate, aveConsumption, out double myForecast);
                listItems.Add("Forecast energy for period " + myTime + " = " + Math.Round(myForecast, 4) + " kWh");


                //Forecast costs
                CalcCosts.GetCosts(dtComponents, MeteringInterval, myForecast, cPreviousAcc, cPreviousFixed, cPreviousMaximum, uPreviousAcc, myTariff, ReadingsType, uPreviousMaximum,
                    uPreviousMaximum, uPreviousPeak, myNodeId, TOULookupId, myTime, out double cAccToDate, out double cFixedToDate,
                    out double cMaxToDate, out double uAccToDate, out double uToDatePeak);
                cAcc = cAccToDate - cPreviousAcc;
                cFixed = cFixedToDate - cPreviousFixed;
                cMax = cMaxToDate - cPreviousMaximum;

                runningTotUacc += myForecast;
                totUAcc += myForecast;
                runningTotCost += cAcc + cFixed + cMax;
                totCAcc += cAcc + cFixed + cMax;

                // update table
                newRow = dtMonthValues.NewRow();
                newRow["uAcc"] = cAccToDate;
                newRow["Node"] = myNodeId;
                newRow["DateReceived"] = myTime;
                newRow["cFixed"] = cFixedToDate;
                newRow["cMaximum"] = cMaxToDate;
                newRow["cAcc"] = cAccToDate;


                dtMonthValues.Rows.Add(newRow);


                listItems.Add("Forecast cost for period " + myTime + " = " + string.Format("{0:c}", Math.Round(cAcc + cFixed + cMax, 4)));
                myTime = myTime.AddMinutes(MeteringInterval);
                cPreviousAcc = cAccToDate;
                cPreviousFixed = cFixedToDate;
                cPreviousMaximum = cMaxToDate;
                uPreviousAcc = uAccToDate;
                uPreviousPeak = uToDatePeak;

            }
            listItems.Add("");
            listItems.Add("Forecast Addtional energy in " + cmbInterval.Text + " = " + string.Format("{0:n}", Math.Round(runningTotUacc, 4)) + " kWh");
            listItems.Add("Forecast Total month-to-date energy at end of  " + cmbInterval.Text + " = " + string.Format("{0:n}", Math.Round(totUAcc, 4)) + " kWh");
            listItems.Add("");
            listItems.Add("Forecast Addtional cost in " + cmbInterval.Text + " = " + string.Format("{0:c}", runningTotCost));
            listItems.Add("Forecast Total month-to-date cost at end of  " + cmbInterval.Text + " = " + string.Format("{0:c}", totCAcc));
            ExitHere:;

            listBoxControl1.Refresh();
            this.Cursor = Cursors.Default;
        }

        private void simpleButtonSwitch_Click(object sender, EventArgs e)
        {
            //Switch.nodeSwitch(comboBoxEditType.Text,textEditGateway.Text,textEditNode.Text,out string result,comboBoxEditOnOff.Text);

            //listBoxControl1.Items.Add(result);
            timer2.Interval = 600000;
            timer2.Start();
        }

        private void simpleButtonEnd_Click(object sender, EventArgs e)
        {
            timer2.Stop();
        }

        private void cbNode_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cbNode.SelectedIndex;
            DateTime myTime = Convert.ToDateTime(dtNodesWithData.Rows[index]["MaxDate"]);
            myTime = myTime.AddMinutes(MeteringInterval);
            txtStart.Text = Convert.ToString(myTime);
        }

        private void cmbInterval_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void w1_Validated(object sender, EventArgs e)
        {
            updateTotal();
        }

        private void w2_Validated(object sender, EventArgs e)
        {
            updateTotal();
        }

        private void w3_Validated(object sender, EventArgs e)
        {
            updateTotal();
        }

        private void w4_Validated(object sender, EventArgs e)
        {
            updateTotal();
        }

        private void w5_Validated(object sender, EventArgs e)
        {
            updateTotal();
        }

        private void w6_Validated(object sender, EventArgs e)
        {
            updateTotal();
        }

        private void btnForecast(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
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
                    timer1.Stop();

                }
            }
            Cursor.Current = Cursors.WaitCursor;

            listItems.Add(DateTime.Now + " Count since started = " + tickCount);
            listItems.Add("-------------------------------");
            if (bLogTest == true)
            {
                Log.Info("Process started at " + DateTime.Now.ToString());
                Log.Info("==========================");
            }

            //int incomming = Verify.VerifyIncoming(MeteringInterval);
            //listItems.Add("Number of new readings: " + incomming);


            //if (bLogTest == true)
            //{
            //    Log.Info("");
            //    Log.Info("Number of new readings this loop: " + incomming);
            //    Log.Info("----------------------------------------------");
            //}

            //int inreadings = ManageIncoming.GetIncomingReadings(MeteringInterval);
            //listItems.Add("Number of new usage records: " + inreadings);


            //if (bLogTest == true)
            //{
            //    Log.Info("Number of new usage records: " + inreadings);
            //    Log.Info("----------------------------------------------");
            //}


            TariffMain.GetCosts(MeteringInterval, out int processedGroups);
            listItems.Add("Number of tariff processed groups: " + processedGroups);
            listItems.Add("");
            listBoxControl1.Refresh();
            if (bLogTest == true)
            {
                Log.Info("Number of tariff processed groups: " + processedGroups);
                Log.Info("----------------------------------------------");
            }

            if (mySqlConnection.State == ConnectionState.Open) { mySqlConnection.Close(); }
            Cursor.Current = Cursors.Default;

            tickCount += 1;
            if (tickCount % 10 == 0) { listItems.Clear(); }
        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = Convert.ToInt32(textEdit1.Text) * 1000;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Neura.Billing.Calls.Switch.nodeSwitch("electricity", textEditGateway.Text, "enode", out string resulte, "off");
            Neura.Billing.Calls.Switch.nodeSwitch("electricity", textEditGateway.Text, "gnode", out string resultg, "off");
            Neura.Billing.Calls.Switch.nodeSwitch("water", textEditGateway.Text, "wnode", out string resultw, "off");

            System.Threading.Thread.Sleep(60000);
            Neura.Billing.Calls.Switch.nodeSwitch("electricity", textEditGateway.Text, "enode", out string resulteo, "on");
            Neura.Billing.Calls.Switch.nodeSwitch("electricity", textEditGateway.Text, "gnode", out string resultgo, "on");
            Neura.Billing.Calls.Switch.nodeSwitch("water", textEditGateway.Text, "wnode", out string resultwo, "on");
        }
    }
}