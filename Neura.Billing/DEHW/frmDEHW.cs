using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils.Extensions;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraTab.Drawing;
using Neura.Billing.DEHWData;

namespace Neura.Billing.DEHW
{
    public partial class frmDEHW : DevExpress.XtraEditors.XtraForm
    {
        private double[] power; //element size
        private string[] serial; //node serial no
        private Random rand;
        private Random rand2;
        private DataTable dt;  //main datatable
        private DataTable dtGeyserNodes; //List of nodes

        private double[] timeCooling;
        private double[] timeHeating;
        private DateTime[] timeStartCooling;
        private DateTime[] timeStartHeating;
        private double[] gradHeating;
        private double[] gradCooling;
        private double[] baseCooling; //50 minutes
        private double[] baseHeating;  //10 minutes
        private double[] HWIndex; //current HW index
        private double[] index;  //c in y=mx+c

        private int[] on_off;
        private int[] sw_on_off;
        private double[] energy;
        private DateTime myTime;
        //private DateTime myTimeMon;
        private DateTime myBaseTime;
        private DateTime prevTime;
        private int nowSec;
        private int prevSec;
        private Series[] seriesC;
        private Series series1;
        private Series series2;
        private Series series3;
        private Series series4;
        private Series series5;

        private List<double> xValues;
        private List<double> yValues;
        private List<double> xValues2;
        private List<double> yValues2;
        private int thermoStart;
        private int thermoEnd;
        private int timeMultiplier;
        private int[] NodeId;
        private int recCount;
        private int rowsToShow = 0;
        private int points = 0;
        private int i = 0;
        public frmDEHW()
        {
            InitializeComponent();
        }

        private void frmDEHW_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 4;

            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy/MM/dd HH:mm:ss";

            int sec = dateTimePicker1.Value.Second;
            dateTimePicker1.Value = dateTimePicker1.Value.AddSeconds(-sec);
            int min = dateTimePicker1.Value.Minute;
            dateTimePicker1.Value = dateTimePicker1.Value.AddMinutes(-min);
            int hour = 18 - dateTimePicker1.Value.Hour;
            dateTimePicker1.Value = dateTimePicker1.Value.AddHours(+hour);

            Timer.Text = dateTimePicker1.Value.ToString();
            //myTime = dateTimePicker1.Value;
            //myTimeMon = myTime;
            timeMultiplier = Convert.ToInt16(comboBox1.Text);
            timer1.Interval = 1000 / timeMultiplier;
            timer2.Interval = 5000 / timeMultiplier;
            myBaseTime = myTime;
            thermoStart = 95;
            thermoEnd = 100;
            checkEditSimulate.Checked = false;
            groupControl1.Enabled = false;

            xValues = new List<double>();
            yValues = new List<double>();
            setDt();
            SetChart();

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            //Get form ready
            SetUp();

            timer1.Start();
        }

        private void SetUp()
        {
            Connections.GetGeyserNodes(out dtGeyserNodes);
            int count = dtGeyserNodes.Rows.Count+1;
            power = new double[count];
            serial = new string[count];
            timeCooling = new double[count];
            timeHeating = new double[count];
            timeStartCooling = new DateTime[count];
            timeStartHeating = new DateTime[count];
            gradCooling = new double[count];
            gradHeating = new double[count];
            baseCooling = new double[count];
            baseHeating = new double[count];
            rand = new Random();
            rand2 = new Random();
            HWIndex = new double[count];

            on_off = new int[count];
            sw_on_off = new int[count];
            index = new double[count];
            energy = new double[count];

            xValues = new List<double>();
            yValues = new List<double>();

            for (int i = 0; i < dtGeyserNodes.Rows.Count; i++)
            {
                //Generate initial values
                serial[i] = Convert.ToString(dtGeyserNodes.Rows[i]["SerialNo"]);
                double myRand2 = rand2.NextDouble() * 8;
                myRand2 = myRand2 / 2;
                power[i] = Math.Round(myRand2 / 0.5) * 0.5;
                if (power[i] < 1) { power[i] = 1; }

                Double myRand = 0.8 + 0.5 * rand.NextDouble();
                baseCooling[i] = (60 * 50) * myRand;
                baseHeating[i] = (60 * 10) * myRand;
                timeCooling[i] = Math.Round(baseCooling[i] * (0.5 + rand.NextDouble()), 0);
                timeHeating[i] = Math.Round(baseHeating[i] * (0.5 + rand.NextDouble()), 0);
                gradCooling[i] = (-5 / timeCooling[i]);
                gradHeating[i] = (5 / timeHeating[i]);
                HWIndex[i] = 100;
                myRand = rand.NextDouble();
                on_off[i] = Convert.ToInt16(myRand);
                sw_on_off[i] = 1;
                if (on_off[i] == 1)
                {
                    index[i] = thermoStart;
                    HWIndex[i] = thermoStart + 5 * rand.NextDouble();
                }
                else
                {
                    index[i] = thermoEnd;
                    HWIndex[i] = thermoStart + 5 * rand.NextDouble();
                }
                //index[i] = Math.Round(95 + 5 * rand.NextDouble(), 3);
                //timeStartHeating[i] = dateTimePicker1.Value;
                //timeStartCooling[i] = dateTimePicker1.Value;
            }

            for (int i = 0; i < dtGeyserNodes.Rows.Count; i++)
            {
                //Save initial values
                DateTime timeStamp = DateTime.Now;
                if (checkEditSimulate.Checked == true)
                {
                    timeStamp = dateTimePicker1.Value;
                } 
                double load = 0;
                if (on_off[i] == 0)
                {
                    timeStamp = timeStamp.AddSeconds(-timeCooling[i] * rand.NextDouble());
                    timeStartCooling[i] = timeStamp;
                }
                else
                {
                    timeStamp = timeStamp.AddSeconds(-timeHeating[i] * rand.NextDouble());
                    timeStartHeating[i] = timeStamp;
                    load = power[i];
                }
                Connections.ThermosStatSetStatus(serial[i],timeStamp,load,gradHeating[i],gradCooling[i]);
            }

           
            if (dtGeyserNodes.Rows.Count <= 20) { rowsToShow = dtGeyserNodes.Rows.Count; }
            else { rowsToShow = 20; }

            //for (int i = 0; i < rowsToShow; i++)
            //{
            //    dt.Rows.Add();
            //    string item = (i + 1).ToString();
            //    if (item.Length == 1) { item = "0" + item; }
            //    dt.Rows[i]["Item"] = item;
            //    dt.Rows[i]["Node"] = serial[i];
            //    dt.Rows[i]["Power"] = power[i];
            //    dt.Rows[i]["Cooling"] = timeCooling[i];
            //    dt.Rows[i]["Heating"] = timeHeating[i];
            //    dt.Rows[i]["GradCooling"] = Math.Round(gradCooling[i], 4);
            //    dt.Rows[i]["GradHeating"] = Math.Round(gradHeating[i], 4);
            //    dt.Rows[i]["HWIndex"] = HWIndex[i];
            //    dt.Rows[i]["ThermoStatus"] = on_off[i];
            //    dt.Rows[i]["NodeStatus"] = 1;
            //}
            gridControl1.DataSource = dt;

            myTime = dateTimePicker1.Value;
            prevTime = myTime;
            gridView1.Columns["Item"].SortOrder = DevExpress.Data.ColumnSortOrder.Descending;

            seriesC = new Series[21];
            seriesC[1] = chartControl2.GetSeriesByName("1");
            seriesC[2] = chartControl2.GetSeriesByName("2");
            seriesC[3] = chartControl2.GetSeriesByName("3");
            seriesC[4] = chartControl2.GetSeriesByName("4");
            seriesC[5] = chartControl2.GetSeriesByName("5");
            seriesC[6] = chartControl2.GetSeriesByName("6");
            seriesC[7] = chartControl2.GetSeriesByName("7");
            seriesC[8] = chartControl2.GetSeriesByName("8");
            seriesC[9] = chartControl2.GetSeriesByName("9");
            seriesC[10] = chartControl2.GetSeriesByName("10");
            seriesC[11] = chartControl2.GetSeriesByName("11");
            seriesC[12] = chartControl2.GetSeriesByName("12");
            seriesC[13] = chartControl2.GetSeriesByName("13");
            seriesC[14] = chartControl2.GetSeriesByName("14");
            seriesC[15] = chartControl2.GetSeriesByName("15");
            seriesC[16] = chartControl2.GetSeriesByName("16");
            seriesC[17] = chartControl2.GetSeriesByName("17");
            seriesC[18] = chartControl2.GetSeriesByName("18");
            seriesC[19] = chartControl2.GetSeriesByName("19");
            seriesC[20] = chartControl2.GetSeriesByName("20");
            series1 = new Series();
            series1 = chartControl1.GetSeriesByName("DEHW 1");
            series1.Points.Clear();
            series2 = new Series();
            series2 = chartControl1.GetSeriesByName("DEHW 2");
            series2.Points.Clear();
            series3 = new Series();
            series3 = chartControl1.GetSeriesByName("DEHW 3");
            series3.Points.Clear();
            series4 = new Series();
            series4 = chartControl1.GetSeriesByName("DEHW 4");
            series4.Points.Clear();
            series5 = new Series();
            series5 = chartControl1.GetSeriesByName("DEHW 5");
            series5.Points.Clear();


        }
        private void setDt()
        {
            dt = new DataTable();
            dt.Columns.Add("Item");
            dt.Columns.Add("Node");
            dt.Columns.Add("ThermoStatus");
            dt.Columns.Add("LastSwitch");
            dt.Columns.Add("NodeStatus");
            dt.Columns.Add("Power");
            dt.Columns.Add("Cooling");
            dt.Columns.Add("Heating");
            dt.Columns.Add("GradCooling");
            dt.Columns.Add("GradHeating");
            dt.Columns.Add("HWIndex");
        }

        private void SetChart()
        {
            ((XYDiagram)chartControl1.Diagram).EnableAxisXScrolling = true;
            ((XYDiagram)chartControl1.Diagram).AxisX.WholeRange.Auto = false;
            ((XYDiagram)chartControl1.Diagram).AxisX.WholeRange.SetMinMaxValues(0, 14400 / 60);
            ((XYDiagram)chartControl1.Diagram).AxisX.VisualRange.AutoSideMargins = false;
            ((XYDiagram)chartControl1.Diagram).AxisX.VisualRange.SetMinMaxValues(0, 7200 / 60);
            ((XYDiagram)chartControl1.Diagram).AxisX.VisualRange.Auto = false;


            ((XYDiagram)chartControl2.Diagram).EnableAxisXScrolling = true;
            ((XYDiagram)chartControl2.Diagram).AxisX.WholeRange.Auto = false;
            ((XYDiagram)chartControl2.Diagram).AxisX.WholeRange.SetMinMaxValues(0, 14400 / 60);
            ((XYDiagram)chartControl2.Diagram).AxisX.VisualRange.AutoSideMargins = false;
            ((XYDiagram)chartControl2.Diagram).AxisX.VisualRange.SetMinMaxValues(0, 7200 / 60);
            ((XYDiagram)chartControl2.Diagram).AxisX.VisualRange.Auto = false;
            ((XYDiagram)chartControl2.Diagram).AxisY.WholeRange.Auto = false;
            ((XYDiagram)chartControl2.Diagram).AxisY.WholeRange.SetMinMaxValues(2, 21);
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            // For simulation
            myTime = myTime.AddSeconds(1);
            Timer.Text = myTime.ToString();
            for (int j = 0; j < dtGeyserNodes.Rows.Count; j++)
            {
                if (on_off[j] == 1)
                {
                    //busy warming
                    TimeSpan duration = (myTime - timeStartHeating[j]);
                    double dur = Convert.ToDouble(duration.TotalSeconds);
                    HWIndex[j] = index[j] + gradHeating[j] * (dur);
                    if (HWIndex[j] >= thermoEnd)
                    {
                        //Start cooling
                        index[j] = HWIndex[j];
                        on_off[j] = 0;
                        gradHeating[j] = (thermoEnd - thermoStart) / dur;
                        Connections.ThermostatChangeStatus(serial[j],myTime,0);

                        //Values for the following cooling cycle
                        timeStartCooling[j] = myTime;
                        timeCooling[j] = Math.Round(baseCooling[j] * (0.8 + 0.4 * rand.NextDouble()), 0);
                        gradCooling[j] = (thermoStart - thermoEnd) / timeCooling[j];
                    }
                }
                else
                {
                    //cooling
                    TimeSpan duration = (myTime - timeStartCooling[j]);
                    double dur = Convert.ToDouble(duration.TotalSeconds);
                    HWIndex[j] = index[j] + gradCooling[j] * (
                        dur);
                    if (HWIndex[j] <= thermoStart)
                    {
                        //Start heating
                        index[j] = HWIndex[j];
                        on_off[j] = 1;
                        gradCooling[j] = (thermoStart - thermoEnd) / dur;
                        Connections.ThermostatChangeStatus(serial[j], myTime, power[j]);

                        //Values for the following heating cycle

                        timeStartHeating[j] = myTime;
                        timeHeating[j] = Math.Round(baseHeating[j] * (0.8 + 0.4 * rand.NextDouble()), 0);
                        gradHeating[j] = (thermoEnd - thermoStart) / timeHeating[j];
                    }
                }
            }


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            timeMultiplier = Convert.ToInt16(comboBox1.Text);
            timer1.Interval = 1000 / timeMultiplier;
            timer2.Interval = 5000 / timeMultiplier;
        }

        private async void timer2_Tick(object sender, EventArgs e)
        {
            //Update the data
            i = 5;
            myTime=myTime.AddSeconds(i);
            nowSec += i;
            dt.Rows.Clear();
            labelUpdated.Text = "Updated: " + myTime;
            string serialNo = "";
            double value = 0;
            double gradCool = 0;
            double gradHeat = 0;
            double hwIndex = 0;
            int dur = 0;
            for (int i = 1; i < recCount +1; i++)
            {
                //Connections.ReadThermostats(NodeId[i-1], out DataTable dtThermostat);
                Connections.ReadThermostats(out DataTable dtThermostat);
                dt.Rows.Add();
                string item = (i ).ToString();
                if (item.Length == 1) { item = "0" + item; }
                //dt.Rows[i]["Item"] = item;
                serialNo = Convert.ToString(dtThermostat.Rows[0]["SerialNo"]);
                value=Convert.ToDouble( dtThermostat.Rows[0]["Value"]);
                gradCool = Convert.ToDouble(dtThermostat.Rows[0]["CoolingGrad"]);
                gradHeat= Convert.ToDouble(dtThermostat.Rows[0]["HeatingGrad"]);
                dur = (myTime - Convert.ToDateTime(dtThermostat.Rows[0]["TimeStamp"])).Seconds;
                serial[i] = serialNo;
                power[i] = value;
                gradHeating[i] = gradHeat;
                gradCooling[i] = gradCool;
              
                // calc HWIndex
                if (value > 0)
                {
                    //Thermostat on
                    HWIndex[i] = thermoStart + gradHeat * dur;
                    timeHeating[i] = dur;
                    timeCooling[i] = 0;
                    on_off[i] = 1;
                }
                else
                {
                    HWIndex[i] = thermoEnd + gradCool * dur;
                    timeHeating[i] = 0;
                    timeCooling[i] = dur;
                    on_off[i] = 0;
                }

               
            }
           
            dt.Rows.Clear();
            for (int i = 1; i < rowsToShow+1; i++)
            {
                dt.Rows.Add();
                string item = (i ).ToString();
                if (item.Length == 1) { item = "0" + item; }
                dt.Rows[i-1]["Item"] = item;
                dt.Rows[i-1]["Node"] = serial[i];
                dt.Rows[i-1]["Power"] = power[i];
                dt.Rows[i-1]["Cooling"] = timeCooling[i];
                dt.Rows[i-1]["Heating"] = timeHeating[i];
                dt.Rows[i-1]["GradCooling"] = Math.Round(gradCooling[i], 4);
                dt.Rows[i-1]["GradHeating"] = Math.Round(gradHeating[i], 4);
                dt.Rows[i-1]["HWIndex"] = HWIndex[i];
                dt.Rows[i-1]["ThermoStatus"] = on_off[i];
                dt.Rows[i-1]["NodeStatus"] = 1;
            }
            Integrate(myTime);
        }

        private async void chkMonitor_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMonitor.Checked==true)
            {
                 
                Connections.GetGeyserNodes(out dtGeyserNodes);
                
                recCount = dtGeyserNodes.Rows.Count;

                if (recCount > 20) { recCount = 20; }
                NodeId = new int[recCount];  //This is the record ID
                serial= new string[recCount];
                for (int i = 0; i < recCount; i++)
                {
                    serial[i] = Convert.ToString(dtGeyserNodes.Rows[i]["SerialNo"]);
                    NodeId[i] = Convert.ToInt32(dtGeyserNodes.Rows[i]["Oid"]);
                }
                if (checkEditSimulate.Checked == true)
                {
                    myTime = dateTimePicker1.Value;
                    timer2.Interval = 5000 / timeMultiplier;
                }
                else
                {
                    timer2.Interval = 5000; 
                    myTime = DateTime.Now;
                }
                SetUp();
                timer2.Start();
            }
            else
            {
                timer2.Stop();
            }
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer2.Stop();
        }

        private void buttonResume_Click(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEditSimulate.Checked == true)
            {
                groupControl1.Enabled = true;
                myTime = dateTimePicker1.Value;
                timer2.Interval = 5000 / timeMultiplier;
                SetUp();
            }
            else
            {
                groupControl1.Enabled = false;
                timer2.Interval = 5000;
                myTime = DateTime.Now;
            }
        }
        private async void Integrate(DateTime myTime)
        {
            double diffSec = (myTime - prevTime).Hours * 3600 + (myTime - prevTime).Minutes * 60 + (myTime - prevTime).Seconds;
            double hMin = Convert.ToDouble(HIMin.Text);
            ((XYDiagram)chartControl1.Diagram).AxisY.WholeRange.SetMinMaxValues(hMin, 100);

            if (nowSec > 7200)
            {

                ((XYDiagram)chartControl2.Diagram).AxisX.WholeRange.SetMinMaxValues(0, nowSec / 60);
                ((XYDiagram)chartControl2.Diagram).AxisX.VisualRange.SetMinMaxValues((nowSec - 7200) / 60, (nowSec + 7200) / 60);

                ((XYDiagram)chartControl1.Diagram).AxisX.WholeRange.SetMinMaxValues(0, nowSec / 60);
                ((XYDiagram)chartControl1.Diagram).AxisX.VisualRange.SetMinMaxValues((nowSec - 7200) / 60, (nowSec + 14400) / 60);


            }
            xValues.Add(nowSec);

            //values60.Add(currentDemand);
            //values120.Add(currentDemand);
            int i = Convert.ToInt16(comboBox1.Text);
            int gnode = 0;
            int status = 0;




            //if (myTime.Second == 0 || myTime.Second == 30)
            if (myTime.Second % 60 == 0) //every 5 seconds
            {
                points += 1;
                if (points > 230)
                {
                    series1.Points.RemoveAt(0);
                    series2.Points.RemoveAt(0);
                    series3.Points.RemoveAt(0);
                    series4.Points.RemoveAt(0);
                    series5.Points.RemoveAt(0);
                }
                series1.Points.Add(new SeriesPoint(nowSec / 60, HWIndex[1]));
                series2.Points.Add(new SeriesPoint(nowSec / 60, HWIndex[2]));
                series3.Points.Add(new SeriesPoint(nowSec / 60, HWIndex[3]));
                series4.Points.Add(new SeriesPoint(nowSec / 60, HWIndex[4]));
                series5.Points.Add(new SeriesPoint(nowSec / 60, HWIndex[5]));

                for (int j = 1; j < 21; j++)
                {

                    bool remove = false;
                    //gnode = Convert.ToInt16(serial[j]);
                    if (points > 230) { remove = true; }
                    status = on_off[j];
                    if (on_off[j] == 1 && sw_on_off[j] == 1) { status = 1; }
                    else { status = 0; }

                    if (remove == true) { seriesC[j].Points.RemoveAt(0); }

                    if (status == 1)
                    {
                        seriesC[j].Points.Add(new SeriesPoint(nowSec / 60, j));
                    }
                    else
                    {
                        seriesC[j].Points.Add(new SeriesPoint(nowSec / 60, 0));

                    }
                }
                prevSec = 0;
                prevTime = myTime;

                xValues.Clear();
                yValues.Clear();
            }
        }

        private void chkShift_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            int min = dateTimePicker1.Value.Minute;
            // dateTimePicker1.Value = dateTimePicker1.Value.AddSeconds(-min);
            int sec = dateTimePicker1.Value.Second;
            dateTimePicker1.Value = dateTimePicker1.Value.AddSeconds(-sec);
            Timer.Text = dateTimePicker1.Value.ToString();
            series1 = new Series();
            series1 = chartControl1.GetSeriesByName("DEHW 1");
            series1.Points.Clear();
            series2 = new Series();
            series2 = chartControl1.GetSeriesByName("DEHW 2");
            series2.Points.Clear();
            series3 = new Series();
            series3 = chartControl1.GetSeriesByName("DEHW 3");
            series3.Points.Clear();
            series4 = new Series();
            series4 = chartControl1.GetSeriesByName("DEHW 4");
            series4.Points.Clear();
            series5 = new Series();
            series5 = chartControl1.GetSeriesByName("DEHW 5");
            series5.Points.Clear();


            seriesC = new Series[22];

            seriesC[1] = chartControl2.GetSeriesByName("1");
            seriesC[2] = chartControl2.GetSeriesByName("2");
            seriesC[3] = chartControl2.GetSeriesByName("3");
            seriesC[4] = chartControl2.GetSeriesByName("4");
            seriesC[5] = chartControl2.GetSeriesByName("5");
            seriesC[6] = chartControl2.GetSeriesByName("6");
            seriesC[7] = chartControl2.GetSeriesByName("7");
            seriesC[8] = chartControl2.GetSeriesByName("8");
            seriesC[9] = chartControl2.GetSeriesByName("9");
            seriesC[10] = chartControl2.GetSeriesByName("10");
            seriesC[11] = chartControl2.GetSeriesByName("11");
            seriesC[12] = chartControl2.GetSeriesByName("12");
            seriesC[13] = chartControl2.GetSeriesByName("13");
            seriesC[14] = chartControl2.GetSeriesByName("14");
            seriesC[15] = chartControl2.GetSeriesByName("15");
            seriesC[16] = chartControl2.GetSeriesByName("16");
            seriesC[17] = chartControl2.GetSeriesByName("17");
            seriesC[18] = chartControl2.GetSeriesByName("18");
            seriesC[19] = chartControl2.GetSeriesByName("19");
            seriesC[20] = chartControl2.GetSeriesByName("20");
            seriesC[21] = chartControl2.GetSeriesByName("21");

            for (int j = 1; j < 21; j++)
            {
                seriesC[j].Points.Clear();
            }
        }
    }
}