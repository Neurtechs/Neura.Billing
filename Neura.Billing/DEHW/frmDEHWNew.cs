using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using log4net.Filter;
using Neura.Billing.DEHWData;

namespace Neura.Billing.DEHW
{
    
    public partial class DEHW : DevExpress.XtraEditors.XtraForm
    {
        private DataTable dt; //thermodata
        private DataTable dtTable; //extract of thermodata for table
        private DataTable dtNodeData;
        private int nodeCount;
        private int thermoCount;
        private int showCount;
        private int[] nodeOid;
        private string[] serialNo;
        private int[] thermoStatus;
        
        private DateTime[] lastSwitch;
        private DateTime startTime;
        private DateTime[] timeStartCooling;
        private DateTime[] timeStartHeating;
        private DateTime processTime;
        private DateTime prevTime;
        private int nowSec;
        private int[] nodeStatus;
        private double[] power;
        private double[] value;
        private int[] coolTime;
        private int[] heatTime; //sec
        private double[] coolGrad;
        private double[] heatGrad;
        private double[] hWIndex;
        private bool bStarted = false;
        private bool[] bRestored;
        private bool bRestoring;
        private int multiplier = 1;

        private double[] baseCooling;
        private double[] baseHeating;
        private Random rand;
        private Random rand2;
        private double thermoStart;
        private double thermoEnd;

        private Series[] seriesC;
        private Series series1;
        private Series series2;
        private Series series3;
        private Series series4;
        private Series series5;
        private List<double> xValues;
        private List<double> yValues;
        private int points = 0;

        private double[] index;
        private bool bDSM;

        private double loadshedMax;
        private double loadshedMin;

        private double loadshedMaxSteps;
        private double loadshedMinSteps;
        public DEHW()
        {
            InitializeComponent();
        }

        private void frmDEHWNew_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy/MM/dd HH:mm:ss";

            int sec = dateTimePicker1.Value.Second;
            dateTimePicker1.Value = dateTimePicker1.Value.AddSeconds(-sec);
            int min = dateTimePicker1.Value.Minute;
            dateTimePicker1.Value = dateTimePicker1.Value.AddMinutes(-min);
            int hour = 18 - dateTimePicker1.Value.Hour;
            dateTimePicker1.Value = dateTimePicker1.Value.AddHours(+hour);
            GetNodeData();
            SetDataTable();
            cmbMultiplier.SelectedIndex = 3;
            thermoEnd = Convert.ToDouble(end.Text);
            thermoStart = Convert.ToDouble(start.Text);
            groupControl1.Enabled = false;

            xValues = new List<double>();
            yValues = new List<double>();
            SetChart();
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
            series1 = chartControl1.GetSeriesByName("DEHW1");
            series1.Points.Clear();
            series2 = new Series();
            series2 = chartControl1.GetSeriesByName("DEHW2");
            series2.Points.Clear();
            series3 = new Series();
            series3 = chartControl1.GetSeriesByName("DEHW3");
            series3.Points.Clear();
            series4 = new Series();
            series4 = chartControl1.GetSeriesByName("DEHW4");
            series4.Points.Clear();
            series5 = new Series();
            series5 = chartControl1.GetSeriesByName("DEHW5");
            series5.Points.Clear();
        }
        private void GetNodeData()
        {
            Connections.GetGeyserNodes(out dtNodeData);
            nodeCount = dtNodeData.Rows.Count;

            thermoStatus = new int[nodeCount];
            if(bStarted==false){nodeStatus = new int[nodeCount];}
            value = new double[nodeCount];
            heatTime = new int[nodeCount];
            coolTime = new int[nodeCount];
            coolGrad = new double[nodeCount];
            heatGrad = new double[nodeCount];
            hWIndex = new double[nodeCount];
            index = new double[nodeCount];
            nodeOid = new int[nodeCount];
            lastSwitch = new DateTime[nodeCount];
            serialNo = new string[nodeCount];

        }
        private void SetDataTable()
        {
            dtTable = new DataTable();
            dtTable.Columns.Add("Item");
            dtTable.Columns.Add("SerialNo");
            dtTable.Columns.Add("ThermoStatus");
            dtTable.Columns.Add("LastSwitch");
            dtTable.Columns.Add("NodeStatus");
            dtTable.Columns.Add("Power");
            dtTable.Columns.Add("CoolTime");
            dtTable.Columns.Add("HeatTime");
            dtTable.Columns.Add("GradCooling");
            dtTable.Columns.Add("GradHeating");
            dtTable.Columns.Add("HWIndex");
        }

        private void GetThermoData(DateTime myTime)
        {
            Connections.ReadThermostats(out dt);
            thermoCount = dt.Rows.Count;
            for (int i = 0; i < thermoCount; i++)
            {
                nodeOid[i] = Convert.ToInt32(dt.Rows[i]["NodeId"]);
                lastSwitch[i] = Convert.ToDateTime(dt.Rows[i]["TimeStamp"]);
                value[i] = Convert.ToDouble(dt.Rows[i]["Value"]);
                coolGrad[i]= Convert.ToDouble(dt.Rows[i]["CoolingGrad"]);
                heatGrad[i]= Convert.ToDouble(dt.Rows[i]["HeatingGrad"]);
                serialNo[i] = Convert.ToString(dt.Rows[i]["SerialNo"]);
                
                if (value[i] > 0)
                {
                    //Geyser on
                    if(bDSM==false){index[i] = thermoStart;}
                    thermoStatus[i] = 1;
                    TimeSpan ts = myTime - lastSwitch[i];
                    heatTime[i] = (int)ts.TotalSeconds;
                    coolTime[i] = 0;
                    hWIndex[i] = index[i] + heatGrad[i] * heatTime[i];
                }
                else
                {
                    if (bDSM == false) {index[i] = thermoEnd;}
                    thermoStatus[i] = 0;
                    TimeSpan ts = myTime - lastSwitch[i];
                    coolTime[i] = (int)ts.TotalSeconds;
                    heatTime[i] = 0;
                    hWIndex[i] = index[i] + coolGrad[i] * coolTime[i];
                }

                
            }
        }

        private void PopulateDG()
        {
           showCount = thermoCount;
            if (showCount > 20) { showCount = 20;}

            string sRow;
            dtTable.Rows.Clear();
            for (int i = 0; i < showCount; i++)
            {
                sRow = (i+1).ToString();
                if (sRow.Length == 1) { sRow = "0" + sRow; }

                dtTable.Rows.Add();
                dtTable.Rows[i]["Item"] = sRow;
                dtTable.Rows[i]["SerialNo"] = serialNo[i];
                dtTable.Rows[i]["ThermoStatus"] = thermoStatus[i];
                dtTable.Rows[i]["LastSwitch"] = lastSwitch[i];
                dtTable.Rows[i]["NodeStatus"] = nodeStatus[i];
                dtTable.Rows[i]["Power"] = value[i];
                dtTable.Rows[i]["CoolTime"] = coolTime[i];
                dtTable.Rows[i]["HeatTime"] = heatTime[i];
                dtTable.Rows[i]["GradCooling"] = coolGrad[i];
                dtTable.Rows[i]["GradHeating"] = heatGrad[i];
                dtTable.Rows[i]["HWIndex"] = hWIndex[i];
              

            }
            gridControl1.DataSource = dtTable;
            gridView1.Columns["Item"].SortOrder = DevExpress.Data.ColumnSortOrder.Descending;
        }

      
        private void timer1_Tick(object sender, EventArgs e)
        {
            GetThermoData(processTime);
            labelControlTime.Text = processTime.ToString();
            if (bDSM == false && bStarted == true){Simulate();}
            if (bDSM==true){DSM();}
            
            if (chkMonitor.Checked == true) { PopulateDG();}
            if (chkMonitor.Checked == true) { DrawGraph(); }
            if(bRestoring==true){Restore();}
            processTime = processTime.AddSeconds(Convert.ToInt32(textEditResolution.Text));
            nowSec += Convert.ToInt32(textEditResolution.Text);
            
        }

        private void Restore()
        {
            //Slowly increase the values
            //if (processTime.Second % 60 == 0) //every minute
            //{
            //    loadshedMax += loadshedMaxSteps;
            //    loadshedMinSteps += loadshedMinSteps;
            //    if (loadshedMax > thermoEnd) { loadshedMax = thermoEnd; }
            //    if (loadshedMin < thermoStart) { loadshedMin = thermoStart; }

            //    if (loadshedMax == thermoEnd && loadshedMin == thermoStart)
            //    {
            //        bRestoiring = false;
            //    }
            //}

            //for (int i = 0; i < thermoCount; i++)
            //{
            //    if(bRestored[i]==false){goto ExitHere;}

            //    bRestoring = false;
            //    bDSM = false;
            //}
            //ExitHere: ;
        }
        private void DSM()
        {
            Connections.GetGeyserNodes(out dtNodeData);
           
            for (int i = 0; i < thermoCount; i++)
            {
                if (bRestoring == true)
                {
                    nodeStatus[i] = 1;

                }
                else
                {
                    string filter = "Oid = " + nodeOid[i];
                    DataRow[] dr = dtNodeData.Select(filter);
                    nodeStatus[i] = Convert.ToInt32(dr[0]["Status"]);
                }




                if (nodeStatus[i]==1)
                {
                    if (thermoStatus[i] == 1)
                    {
                        //Busy heating
                        if (bRestoring == true)
                        {
                            bRestored[i] = true;
                            goto SkipTurnOff;
                        }
                        if (hWIndex[i] > loadshedMax)
                        {
                            //turn off
                          
                            nodeStatus[i] = 0;
                            value[i] = 0;
                            index[i] = hWIndex[i];
                            thermoStatus[i] = 0;
                            timeStartCooling[i] = processTime;
                            if (bStarted == true)
                            {
                                //In simulation mode, force thermostat off
                                Connections.ThermosStatSetStatus(serialNo[i], processTime, value[i], heatGrad[i], coolGrad[i]);
                            }
                            Connections.SwitchStatusChange(serialNo[i], processTime, nodeStatus[i]);
                        }
                        SkipTurnOff: ;
                    }
                    else
                    {
                        
                        if (hWIndex[i] < loadshedMin)
                        {
                            //Need to turn on
                            //turn on
                            timeStartHeating[i] = processTime;
                            nodeStatus[i] = 1;
                            thermoStatus[i] = 1;
                            value[i] = power[i];
                            index[i] = hWIndex[i];
                            if (bStarted == true)
                            {
                                //In simulation mode, force thermostat on
                                Connections.ThermosStatSetStatus(serialNo[i], processTime, value[i], heatGrad[i], coolGrad[i]);
                            }
                            Connections.SwitchStatusChange(serialNo[i], processTime, nodeStatus[i]);
                        }
                        else if (hWIndex[i] < thermoStart) //Still cooling down after loadshedding start
                        {
                            //continue with cooling
                            //get the updated values
                            TimeSpan duration = (processTime - timeStartCooling[i]);
                            double dur = Convert.ToDouble(duration.TotalSeconds);
                            coolGrad[i] = (hWIndex[i] - index[i]) / dur;
                            timeStartCooling[i] = processTime;
                            nodeStatus[i] = 0;
                            index[i] = hWIndex[i];
                            if (bStarted == true)
                            {
                                //In simulation mode, force thermostat off
                                Connections.ThermosStatSetStatus(serialNo[i], processTime, value[i], heatGrad[i], coolGrad[i]);
                            }
                            Connections.SwitchStatusChange(serialNo[i], processTime, nodeStatus[i]);
                        }
                    }
                }
                else //node off
                {
                    if (thermoStatus[i] == 0)
                    {
                        if (hWIndex[i] < loadshedMin)
                        {
                            //turn on
                            timeStartHeating[i] = processTime;
                            nodeStatus[i] = 1;
                            thermoStatus[i] = 1;
                            value[i] = power[i];
                            index[i] = hWIndex[i];
                            if (bStarted == true)
                            {
                                //In simulation mode, force thermostat on
                                Connections.ThermosStatSetStatus(serialNo[i], processTime, value[i], heatGrad[i], coolGrad[i]);
                            }
                            Connections.SwitchStatusChange(serialNo[i], processTime, nodeStatus[i]);
                        }
                    }
                    else //thermo on
                    {
                        //this situation should not be possible
                    }
                }
                SkipDSM: ;
            }

            bRestoring = false;
        }
        private void Simulate()
        {
            for (int i = 0; i < thermoCount; i++)
            {
                //if (bRestored[i]==false){goto StillRestoring;}
                if (thermoStatus[i] == 1)
                {
                    //busy warming
                    //TimeSpan duration = (processTime - timeStartHeating[i]);
                    //double dur = Convert.ToDouble(duration.TotalSeconds);
                    //index[i] = thermoStart;
                    //hWIndex[i] = index[i] + heatGrad[i] * dur;
                    if (hWIndex[i] > thermoEnd)
                    {
                        //End of heating
                        TimeSpan duration = (processTime - timeStartHeating[i]);
                        double dur = Convert.ToDouble(duration.TotalSeconds);
                        index[i] = thermoEnd;
                        thermoStatus[i] = 0;
                        heatGrad[i] = (thermoEnd - thermoStart) / dur;
                        value[i] = 0;

                        //values for the following cooling cycle
                        timeStartCooling[i] = processTime;
                        coolTime[i] = (int)Math.Round(baseCooling[i] * (0.8 + 0.4 * rand.NextDouble()), 0);
                        coolGrad[i] = (thermoStart - thermoEnd) / coolTime[i];

                        Connections.ThermosStatSetStatus(serialNo[i],processTime,value[i],heatGrad[i],coolGrad[i]);
                    }
                }
                else
                {
                    //Cooling
                    //TimeSpan duration = (processTime - timeStartCooling[i]);
                    //double dur = Convert.ToDouble(duration.TotalSeconds);
                    //index[i] = thermoEnd;
                    //hWIndex[i] = index[i] + coolGrad[i] * dur;
                   
                    if (hWIndex[i] < thermoStart)
                    {
                        //End of cooling
                        index[i] = thermoStart;
                        TimeSpan duration = (processTime - timeStartCooling[i]);
                        double dur = Convert.ToDouble(duration.TotalSeconds);
                        thermoStatus[i] = 1;

                        coolGrad[i] = (thermoStart - thermoEnd) / dur;
                        value[i] = power[i];
                        //values for following heating cycle
                        timeStartHeating[i] = processTime;
                        heatTime[i] = (int) Math.Round(baseHeating[i] * (0.8 + 0.4 * rand.NextDouble()), 0);
                        heatGrad[i] = (thermoEnd - thermoStart) / heatTime[i];
                        Connections.ThermosStatSetStatus(serialNo[i], processTime, value[i], heatGrad[i], coolGrad[i]);
                    }
                }
                StillRestoring: ;
            }
        }

        private void DrawGraph()
        {
            int status = 0;
            double diffSec = (processTime - prevTime).Hours * 3600 + (processTime - prevTime).Minutes * 60 + (processTime - prevTime).Seconds;
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

            if (processTime.Second % 60 == 0) //every minute
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
                series1.Points.Add(new SeriesPoint(nowSec / 60, hWIndex[1]));
                series2.Points.Add(new SeriesPoint(nowSec / 60, hWIndex[2]));
                series3.Points.Add(new SeriesPoint(nowSec / 60, hWIndex[3]));
                series4.Points.Add(new SeriesPoint(nowSec / 60, hWIndex[4]));
                series5.Points.Add(new SeriesPoint(nowSec / 60, hWIndex[5]));
                for (int i = 1; i < showCount+1; i++)
                {
                    bool remove = false;
                   
                    if (points > 230) { remove = true; }
                    status = thermoStatus[i-1];
                    if (thermoStatus[i-1] == 1 ) { status = 1; }
                    else { status = 0; }

                    if (remove == true) { seriesC[i].Points.RemoveAt(0); }

                    if (status == 1)
                    {
                        seriesC[i].Points.Add(new SeriesPoint(nowSec / 60, i));
                    }
                    else
                    {
                        seriesC[i].Points.Add(new SeriesPoint(nowSec / 60, 0));

                    }
                }
               
                prevTime = processTime;

                xValues.Clear();
                yValues.Clear();
            }
        }
        private void chkMonitor_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMonitor.Checked == true)
            {
                if (checkEditSimulate.Checked == false)
                {
                    SetDataTable();
                    GetNodeData();
                    
                    timer1.Interval = Convert.ToInt32(textEditResolution.Text) * 1000;
                    processTime=DateTime.Now;
                    timer1.Start();
                }
                else
                {
                    if (bStarted == true)
                    {
                        //GetNodeData();
                        //SetDataTable();
                        //timer1.Interval = 5000;

                        //timer1.Start();
                    }
                }
            }
            else
            {
                if (bStarted == false)
                {
                    timer1.Stop();
                    dtTable.Rows.Clear();
                    labelControlTime.Text = "00:00";
                }
                else
                {
                    dtTable.Rows.Clear();
                }
            }
        }

        private void checkEditSimulate_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Stop();
            dtTable.Rows.Clear();
            labelControlTime.Text = "00:00";
            if (checkEditSimulate.Checked == true)
            {
                groupControl1.Enabled = true;
                startTime = dateTimePicker1.Value;
                processTime = startTime;
            }
            else
            {
                groupControl1.Enabled = false;
                if (chkMonitor.Checked == true)
                {
                    SetDataTable();
                    GetNodeData();
                    
                    timer1.Interval = Convert.ToInt32(textEditResolution.Text) * 1000;
                    processTime = DateTime.Now;
                    timer1.Start();
                }
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            bStarted = true;
            startTime = dateTimePicker1.Value;
            processTime = startTime;
            
            InitialValues();
            SetDataTable();
            GetNodeData();

            
            timer1.Interval = (Convert.ToInt32(textEditResolution.Text) * 1000 )/ multiplier;

            timer1.Start();
           
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            bStarted = false;
            timer1.Stop();
        }

        private void buttonResume_Click(object sender, EventArgs e)
        {
            bStarted = true;
            timer1.Start();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            startTime = dateTimePicker1.Value;
            processTime = startTime;
        }

        private void cmbMultiplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            multiplier = Convert.ToInt32(cmbMultiplier.Text);
            if (checkEditSimulate.Checked == true)
            {
                timer1.Interval = (Convert.ToInt32(textEditResolution.Text) * 1000) / multiplier;
            }
        }

        private void InitialValues()
        {

            rand = new Random();
            rand2 = new Random();
            power=new double[nodeCount];
            index = new double[nodeCount];
            startTime = dateTimePicker1.Value;
            timeStartCooling = new DateTime[nodeCount];
            timeStartHeating = new DateTime[nodeCount];
            baseCooling = new double[nodeCount];
            baseHeating = new double[nodeCount];
            nodeStatus=new int[nodeCount];
            DateTime myTime;
          
            for (int i = 0; i < nodeCount; i++)
            {
                serialNo[i] = Convert.ToString(dtNodeData.Rows[i]["SerialNo"]);
                nodeOid[i] = Convert.ToInt32(dtNodeData.Rows[i]["Oid"]);
                double myRand2 = rand2.NextDouble() * 8;
                myRand2 = myRand2 / 2;
                power[i] = Math.Round(myRand2 / 0.5) * 0.5;
                if (power[i] < 1) { power[i] = 1; }
                Double myRand = 0.8 + 0.5 * rand.NextDouble();
                baseCooling[i] = (60 * 50) * myRand;
                baseHeating[i] = (60 * 10) * myRand;
                coolTime[i]= (int)Math.Round(baseCooling[i] * (0.5 + rand.NextDouble()), 0);
                heatTime[i]=  (int)Math.Round(baseHeating[i] * (0.5 + rand.NextDouble()), 0);
                coolGrad[i] = -(thermoEnd - thermoStart) / coolTime[i];
                heatGrad[i]= (thermoEnd - thermoStart) / heatTime[i];
                nodeStatus[i] = 1;

                myRand = rand.NextDouble();
                thermoStatus[i] = Convert.ToInt16(myRand);
               // timeStartCooling[i] = startTime;
                //timeStartHeating[i] = startTime;
                if (thermoStatus[i] == 0)
                {
                    index[i] = thermoStart;
                    hWIndex[i] = thermoStart + 5 * rand.NextDouble();
                    timeStartCooling[i] = startTime.AddSeconds(-coolTime[i] * rand.NextDouble());
                    //timeStartCooling[i] = startTime;
                    myTime = timeStartCooling[i];
                    value[i] = 0;
                }
                else
                {
                    index[i] = thermoEnd;
                    hWIndex[i] = thermoStart + 5 * rand.NextDouble();
                    timeStartHeating[i] = startTime.AddSeconds(-heatTime[i] * rand.NextDouble());
                    //timeStartHeating[i] = startTime;
                    value[i] = power[i];
                    myTime = timeStartHeating[i];
                }
                Connections.ThermosStatSetStatus(serialNo[i],myTime,value[i],heatGrad[i],coolGrad[i]);
                Connections.SwitchStatusChange(serialNo[i],processTime,1);
            }
        }

        private async void checkDSM_CheckedChanged(object sender, EventArgs e)
        {
            if (checkDSM.Checked == true)
            {
                bDSM = true; 
                bRestored=new bool[nodeCount];
                bRestoring = false;
               for (int i = 0; i < nodeCount; i++)
               {
                   bRestored[i] = false;
               }
                //if (bStarted == true) // for simulation only
                //{
                   
                //    for (int i = 0; i <thermoCount; i++)
                //    {
                //        Connections.SwitchStatusChange(serialNo[i], processTime, 1);
                //        nodeStatus[i] = Convert.ToInt32(dtNodeData.Rows[i]["Status"]);
                //    }
                   
                //}

                bRestoring = false;
                loadshedMax = Convert.ToDouble(HIMax.Text);
                loadshedMin = Convert.ToDouble(HIMin.Text);
                //loadshedMaxSteps = (thermoEnd - loadshedMax) / 10;
                //loadshedMinSteps = (thermoStart - loadshedMin) / 10;
            }
            else
            {
                bRestoring = true;
                loadshedMax = thermoEnd;
                loadshedMin = thermoStart;
               

            }
        }
    }
}