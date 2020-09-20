using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
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
        private int[] thermoStatusX;

        private DateTime[] lastSwitch;
        private DateTime[] lastSwitchX;
        private DateTime startTime;
        private DateTime[] timeStartCooling;
        private DateTime[] timeStartHeating;
        private DateTime[] timeStartCoolingX;
        private DateTime[] timeStartHeatingX;
        private DateTime processTime;
        private DateTime prevTime;
        private DateTime prevTimeTable;
        private int nowSec;
        private int[] nodeStatus;
        private double[] power;
        private double[] value;
        private int[] coolTime;
        private int[] heatTime; //sec
        private int[] coolTimeX;
        private int[] heatTimeX; //sec
        private double[] coolGrad;
        private double[] heatGrad;
        private double[] coolGradS;
        private double[] heatGradS;
        private double[] hWIndex;
        private double[] hWIndexX;
        private bool bStarted = false;
        //private bool[] bRestored;
        //private bool bRestoring;
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
        private Series series1x;
        private Series series2x;
        private Series series3x;
        private Series series4x;
        private Series series5x;
        private List<double> xValues;
        private List<double> yValues;
        private int points = 0;

        private double[] index;
        private double[] indexX;
        private bool bDSM;

        private double loadshedMax;
        private double loadshedMin;

        private int restoreSteps = 0;
        private int countRestore = 0;

        private double[] energy;
        private double[] energyX;
        private double[] energyXT;  //[24hrs,i]
        private double[] energyT;
        private double[] costXT;
        private double[] costT;
        private double rate =0;
        private double[] loadShift;
        private DateTime[] lsStart;
        private DateTime[] lsEnd;
        private int[] forceOff;
        private int processUpdate = 0; //0 = no DSM, 1 = DSM, 2 = Restore;

        private int[] processStatus; //0=nil, 1=Prepare for DSM, 2= DSM, 3=Prepare to Restore, 4=Restore, 5=Restore Done
        private int retailerId;
        private int tariffId;
        private DataTable dtTariffs;
        private DataTable dtRetailers;
        private int selectedIndex;

        private DataTable dtGeysers; 
        public DEHW()
        {
            InitializeComponent();
        }
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
            cmbMultiplier.SelectedIndex = 4;
            thermoEnd = Convert.ToDouble(end.Text);
            thermoStart = Convert.ToDouble(start.Text);
            groupControl1.Enabled = false;
            
           
            PopulateCombos();

        }

        private void PopulateCombos()
        {
            Connections.Retailers(out  dtRetailers);
            for (int i = 0; i < dtRetailers.Rows.Count; i++)
            {
                cmbRetailer.Items.Add(dtRetailers.Rows[i]["RetailerName"]);
            }

            cmbRetailer.SelectedIndex = 0;
            Connections.GetGeyserNodes(out dtGeysers);

        }
        private void SetChart()
        {


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

            for (int i = 1; i < 21; i++)
            {
                seriesC[i].Points.Clear();
            }
            series1 = new Series();
            series1 = chartControl1.GetSeriesByName("DEHW1");
            series1.Points.Clear();
            series2 = new Series();
            series2 = chartControl1.GetSeriesByName("DEHW5");
            series2.Points.Clear();
            series3 = new Series();
            series3 = chartControl1.GetSeriesByName("DEHW10");
            series3.Points.Clear();
            series4 = new Series();
            series4 = chartControl1.GetSeriesByName("DEHW15");
            series4.Points.Clear();
            series5 = new Series();
            series5 = chartControl1.GetSeriesByName("DEHW20");
            series5.Points.Clear();

            series1x = new Series();
            series1x = chartControl3.GetSeriesByName("DEHW1");
            series1x.Points.Clear();
            series2x = new Series();
            series2x = chartControl3.GetSeriesByName("DEHW5");
            series2x.Points.Clear();
            series3x = new Series();
            series3x = chartControl3.GetSeriesByName("DEHW10");
            series3x.Points.Clear();
            series4x = new Series();
            series4x = chartControl3.GetSeriesByName("DEHW15");
            series4x.Points.Clear();
            series5x = new Series();
            series5x = chartControl3.GetSeriesByName("DEHW20");
            series5x.Points.Clear();

            ((XYDiagram)chartControl1.Diagram).EnableAxisXScrolling = true;
            ((XYDiagram)chartControl1.Diagram).AxisX.WholeRange.Auto = false;
            ((XYDiagram)chartControl1.Diagram).AxisX.WholeRange.SetMinMaxValues(0, 14400 / 60);
            ((XYDiagram)chartControl1.Diagram).AxisX.VisualRange.AutoSideMargins = false;
            ((XYDiagram)chartControl1.Diagram).AxisX.VisualRange.SetMinMaxValues(0, 7200 / 60);
            ((XYDiagram)chartControl1.Diagram).AxisX.VisualRange.Auto = false;

            ((XYDiagram)chartControl3.Diagram).EnableAxisXScrolling = true;
            ((XYDiagram)chartControl3.Diagram).AxisX.WholeRange.Auto = false;
            ((XYDiagram)chartControl3.Diagram).AxisX.WholeRange.SetMinMaxValues(0, 14400 / 60);
            ((XYDiagram)chartControl3.Diagram).AxisX.VisualRange.AutoSideMargins = false;
            ((XYDiagram)chartControl3.Diagram).AxisX.VisualRange.SetMinMaxValues(0, 7200 / 60);
            ((XYDiagram)chartControl3.Diagram).AxisX.VisualRange.Auto = false;


            ((XYDiagram)chartControl2.Diagram).EnableAxisXScrolling = true;
            ((XYDiagram)chartControl2.Diagram).AxisX.WholeRange.Auto = false;
            ((XYDiagram)chartControl2.Diagram).AxisX.WholeRange.SetMinMaxValues(0, 14400 / 60);
            ((XYDiagram)chartControl2.Diagram).AxisX.VisualRange.AutoSideMargins = false;
            ((XYDiagram)chartControl2.Diagram).AxisX.VisualRange.SetMinMaxValues(0, 7200 / 60);
            ((XYDiagram)chartControl2.Diagram).AxisX.VisualRange.Auto = false;
            ((XYDiagram)chartControl2.Diagram).AxisY.WholeRange.Auto = false;
            ((XYDiagram)chartControl2.Diagram).AxisY.WholeRange.SetMinMaxValues(2, 21);
        }
        private void GetNodeData()
        {
            Connections.GetGeyserNodes(out dtNodeData);
            nodeCount = dtNodeData.Rows.Count;

            thermoStatus = new int[nodeCount];
            thermoStatusX = new int[nodeCount];

            if (bStarted==false){nodeStatus = new int[nodeCount];}
            value = new double[nodeCount];
            heatTime = new int[nodeCount];
            coolTime = new int[nodeCount];
            heatTimeX = new int[nodeCount];
            coolTimeX = new int[nodeCount];
            coolGrad = new double[nodeCount];
            heatGrad = new double[nodeCount];
            coolGradS = new double[nodeCount];
            heatGradS = new double[nodeCount];
            //hWIndex = new double[nodeCount];
            energy =new double[nodeCount];
            energyX = new double[nodeCount];
            if (bStarted == false) {index = new double[nodeCount];}
            nodeOid = new int[nodeCount];
            lastSwitch = new DateTime[nodeCount];
            lastSwitchX = new DateTime[nodeCount];
            serialNo = new string[nodeCount];
            loadShift=new double[nodeCount];
            
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
            dtTable.Columns.Add("HWIndexX");
            dtTable.Columns.Add("Energy");
            dtTable.Columns.Add("EnergyX");
            dtTable.Columns.Add("LoadshedStart");
            dtTable.Columns.Add("LoadshedEnd");

        }
        private void PopulateDG()
        {
            showCount = thermoCount;
            if (showCount > 20) { showCount = 20; }

            string sRow;
            dtTable.Rows.Clear();
            for (int i = 0; i < showCount; i++)
            {

                sRow = (i + 1).ToString();
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
                dtTable.Rows[i]["GradCooling"] = coolGrad[i].ToString("N4");
                dtTable.Rows[i]["GradHeating"] = heatGrad[i].ToString("N4");
                dtTable.Rows[i]["HWIndex"] = hWIndex[i].ToString("N4");
                dtTable.Rows[i]["HWIndexX"] = hWIndexX[i].ToString("N4");
                dtTable.Rows[i]["Energy"] = energy[i].ToString("N4");
                dtTable.Rows[i]["EnergyX"] = energyX[i].ToString("N4");
                if (Convert.ToString(lsStart[i]) != "1980/01/01 00:00:00")
                {
                    dtTable.Rows[i]["LoadshedStart"] = lsStart[i];
                }
                if (Convert.ToString(lsEnd[i]) != "1980/01/01 00:00:00")
                {
                    dtTable.Rows[i]["LoadshedEnd"] = lsEnd[i];
                }
                SkipToNext: ;
            }
            SkipOver: ;
            gridControl1.DataSource = dtTable;
            gridView1.Columns["Item"].SortOrder = DevExpress.Data.ColumnSortOrder.Descending;
        }
        private void GetThermoData()
        {
           Connections.ReadThermostats(out DataTable dt);
            thermoCount = dt.Rows.Count;
            for (int i = 0; i < thermoCount; i++)
            {
                nodeOid[i] = Convert.ToInt32(dt.Rows[i]["Node"]);
                lastSwitch[i] = Convert.ToDateTime(dt.Rows[i]["TimeStamp"]).ToLocalTime();
                value[i] = Convert.ToDouble(dt.Rows[i]["Value"]);
                coolGrad[i]= Convert.ToDouble(dt.Rows[i]["CoolingGrad"]);
                heatGrad[i]= Convert.ToDouble(dt.Rows[i]["HeatingGrad"]);
                serialNo[i] = Convert.ToString(dt.Rows[i]["SerialNo"]);
                if(bStarted==false){processTime=DateTime.Now;}
                if (value[i] > 0)
                {
                    //Geyser on
                    if(processStatus[i]==0) { index[i] = thermoStart; }

                   
                    TimeSpan ts = processTime - lastSwitch[i];
                    double dur = Convert.ToDouble(ts.TotalSeconds);
                    heatTime[i] = (int)ts.TotalSeconds;
                    coolTime[i] = 0;
                    
                   
                    if (thermoStatus[i] == 0)
                    {
                        //Update required
                        Connections.UpdateCoolGrad(nodeOid[i],coolGradS[i]);
                    }
                    heatGradS[i] = (thermoEnd - thermoStart) / dur;
                    hWIndex[i] = index[i] + heatGrad[i] * heatTime[i];
                    if (hWIndex[i] > thermoEnd)
                    {
                        hWIndex[i] = thermoEnd;
                    }
                    thermoStatus[i] = 1;
                }
                else
                {
                    //0=nil, 1=Prepare for DSM, 2= DSM, 3=Prepare to Restore, 4=Restore, 5=Restore Done
                    if (processStatus[i] == 0) { index[i] = thermoEnd; }
                    if (processStatus[i] == 5)
                    {
                        processStatus[i] = 0;
                    }
                    
                    TimeSpan ts = processTime - lastSwitch[i];
                    double dur = Convert.ToDouble(ts.TotalSeconds);
                    coolTime[i] = (int)ts.TotalSeconds;
                    heatTime[i] = 0;
                    
                    
                    if (thermoStatus[i] == 1)
                    {
                        //Update required
                        Connections.UpdateHeatGrad(nodeOid[i],heatGradS[i]);
                    }
                    coolGradS[i] = -(thermoEnd - thermoStart) / dur;
                    hWIndex[i] = index[i] + coolGrad[i] * coolTime[i];
                    if (hWIndex[i] < thermoStart)
                    {
                        hWIndex[i] = thermoStart;}

                    thermoStatus[i] = 0;
                }

                if (processStatus[i] == 0)
                {
                    hWIndexX[i] = hWIndex[i];
                    lastSwitchX[i] = lastSwitch[i];
                    heatTimeX[i] = heatTime[i];
                    coolTimeX[i] = coolTime[i];
                    indexX[i] = index[i];
                    thermoStatusX[i] = thermoStatus[i];
                    timeStartHeatingX[i] = timeStartHeating[i];
                    timeStartCoolingX[i] = timeStartCooling[i];
                }
                nodeStatus[i]= Connections.GetNodeBasicData(nodeOid[i]);
            }
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bStarted == false && checkEditSimulate.Checked==true )
            {
                timer1.Stop();
                goto ExitHere;
            }
            if (processUpdate != 0) { ProcessUpdate();}
            GetThermoData();
            labelControlTime.Text = processTime.ToString();
         
            if (bStarted == true) { Simulate(); }
            if (bDSM==true){DSM();}
            if (bDSM == true) { Verify(); }
         
            if (chkMonitor.Checked == true) { PopulateDG();}
            if (chkMonitor.Checked == true) { DrawGraph(); }

            prevTimeTable = processTime;
            processTime = processTime.AddSeconds(Convert.ToInt32(textEditResolution.Text));
            nowSec += Convert.ToInt32(textEditResolution.Text);
           
            if (chkAuto.Checked == true)
            {
                if ( processTime.Second==30 && processTime.Hour == 18 && processTime.Minute == 0)
                {
                    checkDSM.Checked = true;
                }

                if (processTime.Hour == 22 && processTime.Second == 0 && processTime.Minute == 0)
                {
                    checkDSM.Checked=false;
                }
            }
            ExitHere: ;
        }

       
        private void Verify()
        {
          
            for (int i = 0; i < thermoCount; i++)
            {
                //ProcessStatus: 0=nil, 1=Prepare for DSM, 2= DSM, 3=Prepare to Restore, 4=Restore, 5=Restore Done
                if (processStatus[i] == 0 ) { goto skipVerify; }
                if (thermoStatusX[i] == 1 || forceOff[i]==1)
                {
                    //thermo on
                    TimeSpan ts = processTime - lastSwitchX[i];
                    heatTimeX[i] = (int)ts.TotalSeconds;
                    coolTimeX[i] = 0;
                    hWIndexX[i] = indexX[i] + heatGrad[i] * heatTimeX[i];
                    thermoStatusX[i] = 1;
                    rate = CalcRates.GetRate(tariffId,processTime);
                    double energyL;
                    double costL;
                    if (processStatus[i] != 5)
                    {
                        energyL= (power[i] * (processTime - prevTimeTable).Seconds) / 3600;
                        energyX[i] += energyL;
                        energyXT[i] += energyL;
                        costL = energyL * rate;
                        costXT[i] += costL;
                     
                        Log.Info("costXL["  + i + "] = " + costL);
                        Log.Info("energyXL[" + i + "] = " + energyL);
                        Log.Info("Rate = " + rate);
                    }
                    
                    if (hWIndexX[i] >= thermoEnd)
                    {
                        //turn off
                        thermoStatusX[i] = 0;
                        lastSwitchX[i] = processTime;
                        heatTimeX[i] = 0;
                        indexX[i] = thermoEnd;
                        hWIndexX[i] = thermoEnd;
                        timeStartCoolingX[i] = processTime;
                    }

                    forceOff[i] = 0; }
                else
                {
                    //thermo off
                   TimeSpan ts = processTime - lastSwitchX[i];
                    coolTimeX[i] = (int)ts.TotalSeconds;
                    heatTimeX[i] = 0;
                    hWIndexX[i] = indexX[i] + coolGrad[i] * coolTimeX[i];
                    if (hWIndexX[i] <= thermoStart)
                    {
                        //turn on
                        thermoStatusX[i] = 1;
                        lastSwitchX[i] = processTime;
                        coolTimeX[i] = 0;
                        indexX[i] = thermoStart;
                        timeStartHeatingX[i] = processTime;
                    }
                }
                skipVerify: ;
            }
        }
        
        private void DSM()
        {
            //ProcessStatus: 0=nil, 1=Prepare for DSM, 2= DSM, 3=Prepare to Restore, 4=Restore, 5=Restore Done

            Connections.GetGeyserNodes(out dtNodeData);
            int restoreComplete = 1;  //True
            Log.Info(processTime);
            double energyL;
            double costL;

            for (int i = 0; i < thermoCount; i++)
            {
                if (processStatus[i] == 5) { goto SkipDSM;}
                string filter = "Oid = " + nodeOid[i];
                DataRow[] dr = dtNodeData.Select(filter);
                nodeStatus[i] = Convert.ToInt32(dr[0]["Status"]);
                if(processStatus[i]==0){goto SkipDSM;}
                restoreComplete = 0; //False
                rate = CalcRates.GetRate(tariffId, processTime);
                if (nodeStatus[i] == 1)
                {
                    //Node on
                    energyL= (value[i] * (processTime - prevTimeTable).Seconds) / 3600;

                    energy[i] += energyL;
                    energyT[i] += energyL;
                    costL = energyL * rate;
                    costT[i] += costL;
                    Log.Info("costL["  + i + "] = " + costL);
                    Log.Info("energyL["  + i + "] = " + energyL);
                    Log.Info("Rate = " + rate);

                    //Log.Info("energy[" + i + "] = " + energy[i]);

                    if (thermoStatus[i] == 1)
                    {
                        //Busy heating

                        if (processStatus[i] == 1)  //Prepare for DSM
                        {
                            //Normal Heating
                            //Switch off for DSM
                            nodeStatus[i] = 0;
                            value[i] = 0;
                            index[i] = hWIndex[i];
                            thermoStatus[i] = 0;
                            timeStartCooling[i] = processTime;
                            lastSwitch[i] = processTime;
                            forceOff[i] = 1;
                            if (bStarted == true)
                            {
                                //In simulation mode, force thermostat off
                                Connections.ThermosStatSetStatus(serialNo[i], processTime, value[i], heatGrad[i], coolGrad[i]);
                            }
                            Connections.SwitchStatusChange(serialNo[i], processTime, nodeStatus[i]);
                            processStatus[i] = 2; //Normal DSM
                        }
                        else if (processStatus[i] == 2) //normal DSM
                        {
                            //Heating During DSM
                            //Check if not exceeds upper limit
                            if (hWIndex[i] >= loadshedMax)
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
                        }
                        else if (processStatus[i] == 3) //prepare to restore
                        {
                            processStatus[i] = 4;
                        }
                        else if (processStatus[i] == 4) //Restoring
                        {
                            
                            //Restoring
                            //Check if above verify
                            if ( hWIndex[i]>= hWIndexX[i] )
                            {
                                processStatus[i] = 5; //Restore Done
                                //Save new values
                                timeStartHeating[i] = processTime;
                                timeStartHeatingX[i] = processTime;
                                nodeStatus[i] = 1;
                                thermoStatus[i] = 1;
                                thermoStatusX[i] = 1;
                                value[i] = power[i];
                                index[i] = hWIndex[i];
                                lastSwitch[i] = processTime;
                                lastSwitchX[i] = processTime;
                                indexX[i]= hWIndex[i];
                                if (energyX[i]!=energy[i])
                                {
                                    //energyX[i] = energy[i]; //in case of overshoot
                                }
                                if (bStarted == true)
                                {
                                    //In simulation mode, force thermostat on
                                    Connections.ThermosStatSetStatus(serialNo[i], processTime, value[i], heatGrad[i], coolGrad[i]);
                                }
                                Connections.SwitchStatusChange(serialNo[i], processTime, nodeStatus[i]);
                                //set start LS
                                if (lsEnd[i] == Convert.ToDateTime("1980/01/01"))
                                {
                                    lsEnd[i] = processTime;
                                }
                            }
                        }
                    }
                    else  //Node on, thermo still off
                    {
                        //processStatus: 0=nil, 1=Prepare for DSM, 2= DSM, 3=Prepare to Restore, 4=Restore, 5=Restore Done

                        if (processStatus[i] == 3)
                        {
                            //back to normal before DSM even starts
                            //if (lsEnd[i] == Convert.ToDateTime("1980/01/01"))
                            //{
                            //    lsEnd[i] = processTime;
                            //}
                            if (hWIndex[i] <= thermoStart)
                            // (hWIndexX[i] <= hWIndex[i])
                            {
                                // switch on
                                processStatus[i] = 0;
                                index[i] = hWIndex[i];
                                TimeSpan duration = (processTime - timeStartCooling[i]);
                                double dur = Convert.ToDouble(duration.TotalSeconds);
                                thermoStatus[i] = 1;
                                nodeStatus[i] = 1;
                                coolGrad[i] = (thermoStart - hWIndex[i]) / dur;
                                value[i] = power[i];
                                if (bStarted == true)
                                {
                                    //In simulation mode, force thermostat off
                                    Connections.ThermosStatSetStatus(serialNo[i], processTime, value[i], heatGrad[i], coolGrad[i]);
                                }
                                Connections.SwitchStatusChange(serialNo[i], processTime, nodeStatus[i]);
                            }
                            else
                            {
                                processStatus[i] = 0;  //continue normal operation
                            }
                        }
                        else if (processStatus[i] == 1) //prepare DSM
                        {
                            if (hWIndex[i] <= thermoStart)
                            {
                                //continue with cooling
                                //get the updated values
                                TimeSpan duration = (processTime - timeStartCooling[i]);
                                double dur = Convert.ToDouble(duration.TotalSeconds);
                                coolGrad[i] = (hWIndex[i] - index[i]) / dur;
                                timeStartCooling[i] = processTime;
                                nodeStatus[i] = 0;
                                thermoStatus[i] = 0;
                                index[i] = hWIndex[i];
                                if (bStarted == true)
                                {
                                    //In simulation mode, force thermostat off
                                    Connections.ThermosStatSetStatus(serialNo[i], processTime, value[i], heatGrad[i], coolGrad[i]);
                                }
                                Connections.SwitchStatusChange(serialNo[i], processTime, nodeStatus[i]);
                                processStatus[i] = 2;  //DSM
                            }
                        }
                        else if (processStatus[i] == 4)

                        {
                            if (hWIndex[i] >= hWIndexX[i])
                            {
                                processStatus[i] = 5; //Restore Done
                                //Save new values
                                timeStartCooling[i] = processTime;
                                timeStartCoolingX[i] = processTime;
                                nodeStatus[i] = 1;
                                thermoStatus[i] = 0;
                                thermoStatusX[i] = 0;
                                value[i] = power[i];
                                index[i] = hWIndex[i];
                                lastSwitch[i] = processTime;
                                lastSwitchX[i] = processTime;
                                indexX[i] = hWIndex[i];
                                if (energyX[i] != energy[i])
                                {
                                    //energyX[i] = energy[i]; //in case of overshoot
                                }
                                if (bStarted == true)
                                {
                                    //In simulation mode, force thermostat on
                                    Connections.ThermosStatSetStatus(serialNo[i], processTime, value[i], heatGrad[i], coolGrad[i]);
                                }
                                Connections.SwitchStatusChange(serialNo[i], processTime, nodeStatus[i]);
                                //set start LS
                                if (lsEnd[i] == Convert.ToDateTime("1980/01/01"))
                                {
                                    lsEnd[i] = processTime;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //node off
                    //Process Status: 0=nil, 1=Prepare for DSM, 2= DSM, 3=Prepare to Restore, 4=Restore, 5=Restore Done
                    if (hWIndex[i] < thermoStart)
                    {
                        //set start LS
                        if (lsStart[i] == Convert.ToDateTime("1980/01/01"))
                        {
                            lsStart[i] = processTime;
                        }
                    }
                    //if (processStatus[i] == 1) //Preparing for DSM
                    //{
                    //    processStatus[i] = 2;
                    //}
                    if (processStatus[i] == 2) //Normal DSM
                    {
                        //Check if lower limit reached
                        if (hWIndex[i] <= loadshedMin)
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

                        //else if (hWIndex[i] < thermoStart && processStatus[i] == 3 && hWIndex[i]>loadshedMax)
                        //{
                        //    //turn on
                        //    timeStartHeating[i] = processTime;
                        //    nodeStatus[i] = 1;
                        //    thermoStatus[i] = 1;
                        //    value[i] = power[i];
                        //    index[i] = hWIndex[i];

                        //    if (bStarted == true)
                        //    {
                        //        //In simulation mode, force thermostat on
                        //        Connections.ThermosStatSetStatus(serialNo[i], processTime, value[i], heatGrad[i], coolGrad[i]);
                        //    }
                        //    Connections.SwitchStatusChange(serialNo[i], processTime, nodeStatus[i]);
                        //}
                    }
                    else if (processStatus[i] == 3) //prepare to restore
                    {
                        if (hWIndex[i] <= loadshedMax-1 && hWIndex[i] >= loadshedMin)
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

                            processStatus[i] = 4; //Restoring
                        }
                        else if (hWIndex[i] <= thermoStart  && hWIndex[i] >= loadshedMax)
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
                            processStatus[i] = 4; //Restoring
                        }
                        else if (hWIndex[i] <= loadshedMin)
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
                            processStatus[i] = 4; //Restoring
                        }
                    }
                    
                }
                SkipDSM: ;
            }

            if (restoreComplete == 1)
            {
                bDSM = false;
                Log.Info("---------------------");
                Log.Info("Restoration Complete");
                //for (int i = 0; i < thermoCount; i++)
                //{
                //    Log.Info("energy[" + i + "] = " + energy[i]);
                //    Log.Info("energyX[" + i + "] = " + energyX[i]);
                //    Log.Info("costXT["  + i + "] = " + costXT[i]);
                //    Log.Info("costT[" + i + "] = " + costT[i]);
                //}

                double[] eCombined;
                eCombined=new double[thermoCount];
                double[] cCombined;
                cCombined = new double[thermoCount];
                double eMoved = 0;
                double cSaving = 0;
                //for (int j = 18; j < 22; j++)
                //{
                //    for (int i = 0; i < thermoCount; i++)
                //    {
                //        Log.Info("energyXT[" + j + "," + i + "] = " + energyXT[j, i]);
                //        Log.Info("energyT[" + j + "," + i + "] = " + energyT[j, i]);
                //        Log.Info("costXT[" + j + "," + i + "] = " + costXT[j, i]);
                //        Log.Info("costT[" + j + "," + i + "] = " + costT[j, i]);

                //        eCombined[i] = energyT[j, i] - energyXT[j, i];
                //        cCombined[i] =costT[j, i] - costXT[j, i];
                //    }
                        
                //}

                for (int i = 0; i < thermoCount; i++)
                {
                         
                    
                    eCombined[i] += energyT[i] - energyXT[i];
                    cCombined[i] += costT[i] - costXT[i];

                    energyXT[i] = Math.Round(energyXT[i], 3);
                    energyT[i] = Math.Round(energyT[i], 3);
                    costXT[i] = Math.Round(costXT[i], 3);
                    costT[i] = Math.Round(costT[i], 3);
                    Log.Info("energyXT[" + i + "] = " + energyXT[i]);
                    Log.Info("energyT["  + i + "] = " + energyT[i]);
                    Log.Info("costXT["  + i + "] = " + costXT[i]);
                    Log.Info("costT["  + i + "] = " + costT[i]);

                        
                    
                }



                for (int i = 0; i < thermoCount; i++)
                {
                    eMoved += energyT[i];
                    cSaving += cCombined[i];
                    eCombined[i] = Math.Round(eCombined[i],3);
                    cCombined[i] = Math.Round(cCombined[i],3);
                    Log.Info("eCombined[" + i + "] = " + eCombined[i]);
                    Log.Info("cCombined[" + i + "] = " + cCombined[i]);
                }

                cSaving = -cSaving;
                textEditEnergyMoved.Text = eMoved.ToString("N");
                textEditSaving.Text = cSaving.ToString("C");
                if (chkAuto.Checked == true)
                {
                    buttonPause.PerformClick();
                }
            }
        }

        private void Simulate()
        {
            for (int i = 0; i < thermoCount; i++)
            {
                if (processStatus[i] > 0  &&  processStatus[i]< 4){goto StillRestoring;}
                if (thermoStatus[i] == 1)
                {
                    //busy warming

                    if (hWIndex[i] >= thermoEnd)
                    {
                        //End of heating
                        if (i == 5 || i == 10)
                        {
                            Log.Info("hWIndex[" + i + "] = " + hWIndex[i]);
                            Log.Info("Set hWIndex[" + i + "] = " + thermoEnd);
                            Log.Info("Set index[" + i + "] = " + thermoEnd);
                        }
                        TimeSpan duration = (processTime - timeStartHeating[i]);
                        double dur = Convert.ToDouble(duration.TotalSeconds);
                        index[i] = thermoEnd;
                        hWIndex[i]=thermoEnd;
                        thermoStatus[i] = 0;
                        heatGrad[i] = (thermoEnd - thermoStart) / dur;
                        value[i] = 0;

                        //values for the following cooling cycle
                        timeStartCooling[i] = processTime;
                        timeStartCoolingX[i] = timeStartCooling[i];
                        coolTime[i] = (int)Math.Round(baseCooling[i] * (0.8 + 0.4 * rand.NextDouble()), 0);
                       
                        coolGrad[i] = (thermoStart - thermoEnd) / coolTime[i];

                        Connections.ThermosStatSetStatus(serialNo[i],processTime,value[i],heatGrad[i],coolGrad[i]);
                    }
                }
                else
                {
                    //Cooling
                    if (hWIndex[i] <= thermoStart  )
                    {
                        //End of cooling
                        index[i] = hWIndex[i];
                        TimeSpan duration = (processTime - timeStartCooling[i]);
                        double dur = Convert.ToDouble(duration.TotalSeconds);
                        thermoStatus[i] = 1;
                        coolGrad[i] = (thermoStart - thermoEnd) / dur;
                        value[i] = power[i];
                        //values for following heating cycle
                        timeStartHeating[i] = processTime;
                        timeStartHeatingX[i] = timeStartHeating[i];
                        
                        heatTime[i] = (int)Math.Round(baseHeating[i] * (0.8 + 0.4 * rand.NextDouble()), 0);
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
            ((XYDiagram)chartControl3.Diagram).AxisY.WholeRange.SetMinMaxValues(hMin, 100);

            if (nowSec > 7200)
            {
                ((XYDiagram)chartControl2.Diagram).AxisX.WholeRange.SetMinMaxValues(0, nowSec / 60);
                ((XYDiagram)chartControl2.Diagram).AxisX.VisualRange.SetMinMaxValues((nowSec - 7200) / 60, (nowSec + 7200) / 60);

                ((XYDiagram)chartControl1.Diagram).AxisX.WholeRange.SetMinMaxValues(0, nowSec / 60);
                ((XYDiagram)chartControl1.Diagram).AxisX.VisualRange.SetMinMaxValues((nowSec - 7200) / 60, (nowSec + 14400) / 60);

                ((XYDiagram)chartControl3.Diagram).AxisX.WholeRange.SetMinMaxValues(0, nowSec / 60);
                ((XYDiagram)chartControl3.Diagram).AxisX.VisualRange.SetMinMaxValues((nowSec - 7200) / 60, (nowSec + 14400) / 60);
            }
            xValues.Add(nowSec);

            if (processTime.Second % 60 == 0 || bStarted == false) //every 60s
            {
                points += 1;
                if (points > 6930)
                {
                    series1.Points.RemoveAt(0);
                    if (showCount > 4){ series2.Points.RemoveAt(0);}
                    if (showCount > 9) {series3.Points.RemoveAt(0);}
                    if (showCount > 14) {series4.Points.RemoveAt(0);}
                    if (showCount > 19) {series5.Points.RemoveAt(0);}

                    series1x.Points.RemoveAt(0);
                    if (showCount > 4) {series2x.Points.RemoveAt(0);}
                    if (showCount > 9){ series3x.Points.RemoveAt(0);}
                    if (showCount > 14) {series4x.Points.RemoveAt(0);}
                    if (showCount > 19) {series5x.Points.RemoveAt(0);}
                }
                series1.Points.Add(new SeriesPoint(nowSec / 60, hWIndex[0]));
                if (showCount > 4){series2.Points.Add(new SeriesPoint(nowSec / 60, hWIndex[4]));}
                if (showCount > 9) {series3.Points.Add(new SeriesPoint(nowSec / 60, hWIndex[9]));}
                if (showCount > 14) {series4.Points.Add(new SeriesPoint(nowSec / 60, hWIndex[14]));}
                if (showCount > 19) {series5.Points.Add(new SeriesPoint(nowSec / 60, hWIndex[19]));}

                series1x.Points.Add(new SeriesPoint(nowSec / 60, hWIndexX[0]));
                if (showCount > 4) {series2x.Points.Add(new SeriesPoint(nowSec / 60, hWIndexX[4]));}
                if (showCount > 9) {series3x.Points.Add(new SeriesPoint(nowSec / 60, hWIndexX[9]));}
                if (showCount > 14) {series4x.Points.Add(new SeriesPoint(nowSec / 60, hWIndexX[14]));}
                if (showCount > 19) {series5x.Points.Add(new SeriesPoint(nowSec / 60, hWIndexX[19]));}


                for (int i = 1; i < showCount+1; i++)
                {
                    bool remove = false;
                   
                    if (points > 930) { remove = true; }
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
                    SetChart();

                    power = new double[nodeCount];
                    index = new double[nodeCount];
                    indexX = new double[nodeCount];
                    startTime = dateTimePicker1.Value;
                    timeStartCooling = new DateTime[nodeCount];
                    timeStartHeating = new DateTime[nodeCount];
                    timeStartCoolingX = new DateTime[nodeCount];
                    timeStartHeatingX = new DateTime[nodeCount];
                    baseCooling = new double[nodeCount];
                    baseHeating = new double[nodeCount];
                    nodeStatus = new int[nodeCount];
                    hWIndex = new double[nodeCount];
                    hWIndexX = new double[nodeCount];
                    forceOff = new int[nodeCount];
                    
                    energyXT = new double[nodeCount];
                    energyT = new double[nodeCount];
                    costXT = new double[nodeCount];
                    costT = new double[nodeCount];
                    xValues = new List<double>();
                    yValues = new List<double>();
                    textEditEnergyMoved.Text = "";
                    textEditSaving.Text = "";

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
                lsStart = new DateTime[nodeCount];
                lsEnd = new DateTime[nodeCount];
                processStatus = new int[nodeCount];

                for (int i = 0; i < nodeCount; i++)
                {
                    lsStart[i] = Convert.ToDateTime("1980/01/01 00:00:00");
                    lsEnd[i] = Convert.ToDateTime("1980/01/01 00:00:00");
                    processStatus[i] = 0; //0=nil, 1=DSM, 2=Prepare to Restore, 3=Restoring, 4=Restore Done
                    nodeStatus[i] = 1;
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

            prevTimeTable = processTime;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            bStarted = true;
            startTime = dateTimePicker1.Value;
            processTime = startTime;
            prevTimeTable = processTime;
            nowSec = 0;
            //chartControl1.Series.Clear();

            //chartControl2.Series.Clear();

            //chartControl3.Series.Clear();
            xValues = new List<double>();
            yValues = new List<double>();
            textEditEnergyMoved.Text = "";
            textEditSaving.Text = "";
            PopulateDG();
            InitialValues();
            SetDataTable();
            GetNodeData();
            SetChart();
            //resetShedding();

            timer1.Interval = (Convert.ToInt32(textEditResolution.Text) * 1000 )/ multiplier;

            timer1.Start();
           
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            bStarted = false;
            //timer1.Stop();
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
            prevTimeTable = processTime;
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
            indexX = new double[nodeCount];
            startTime = dateTimePicker1.Value;
            timeStartCooling = new DateTime[nodeCount];
            timeStartHeating = new DateTime[nodeCount];
            timeStartCoolingX = new DateTime[nodeCount];
            timeStartHeatingX = new DateTime[nodeCount];
            baseCooling = new double[nodeCount];
            baseHeating = new double[nodeCount];
            nodeStatus=new int[nodeCount];
            hWIndex = new double[nodeCount];
            hWIndexX = new double[nodeCount];
            forceOff=new int[nodeCount];
            DateTime myTime;
            energyXT = new double[nodeCount];
            energyT = new double[nodeCount];
            costXT = new double[nodeCount];
            costT = new double[nodeCount];
            processStatus=new int[nodeCount];
            //rate=new double[24];
            //rate[17] = 1.5;
            //rate[18] = 3;
            //rate[19] = 3;
            //rate[20] = 2.0;
            //rate[21] = 1.5;

            for (int i = 0; i < nodeCount; i++)
            {
                serialNo[i] = Convert.ToString(dtNodeData.Rows[i]["SerialNo"]);
                nodeOid[i] = Convert.ToInt32(dtNodeData.Rows[i]["Oid"]);
                double myRand2 = rand2.NextDouble() * 8;
                myRand2 = myRand2 / 2;
                power[i] = Math.Round(myRand2 / 0.5) * 0.5;
                if (power[i] < 1) { power[i] = 1; }

                Double myRand = 0.8 + 0.4 * rand.NextDouble();
                baseCooling[i] = (60 * 45) * myRand;
                baseHeating[i] = (60 * 15) * myRand;
                coolTime[i]= (int)Math.Round(baseCooling[i] * (0.8 + 0.4 * rand.NextDouble()), 0);
                heatTime[i]=  (int)Math.Round(baseHeating[i] * (0.8 + 0.4 * rand.NextDouble()), 0);
                coolGrad[i] = -(thermoEnd - thermoStart) / coolTime[i];
                heatGrad[i]= (thermoEnd - thermoStart) / heatTime[i];
                nodeStatus[i] = 1;
                energy[i] = 0;
                forceOff[i] = 0;
                myRand = rand.NextDouble();
                thermoStatus[i] = Convert.ToInt16(myRand);
                // timeStartCooling[i] = startTime;
                //timeStartHeating[i] = startTime;
                processStatus[i] = 0; //0=nil, 1=Prepare for DSM, 2= DSM, 3=Prepare to Restore, 4=Restore, 5=Restore Done
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
                processUpdate = 1; //DSM

            }
            else
            {
                processUpdate = 2; //Restore
            }
        }

        private void ProcessUpdate()
        {
            //ProcessStatus: 0=nil, 1=Prepare for DSM, 2= DSM, 3=Prepare to Restore, 4=Restore, 5=Restore Done
            if (processUpdate == 1)
            {
                bDSM = true;
                for (int i = 0; i < nodeCount; i++)

                {
                    processStatus[i] = 1; 
                    lsStart[i] = Convert.ToDateTime("1980/01/01");
                    lsEnd[i] = Convert.ToDateTime("1980/01/01");
                    energy[i] = 0;
                    energyX[i] = 0;
                    costT[i] = 0;
                    costXT[i] = 0;
                    
                }

                textEditEnergyMoved.Text = "";
                textEditSaving.Text = "";
                loadshedMax = Convert.ToDouble(HIMax.Text);
                loadshedMin = Convert.ToDouble(HIMin.Text);

            }
            else if (processUpdate == 2)
            {

                restoreSteps = (int)(thermoCount / 20);
                countRestore = restoreSteps;

                for (int i = 0; i < nodeCount; i++)
                {
                    processStatus[i] = 3; //0=nil, 1=Prepare for DSM, 2= DSM, 3=Prepare to Restore, 4=Restore, 5=Restore Done
                }
            }

            processUpdate = 0;
        }
        private void resetShedding()
        {
            //for load shedding
            lsStart = new DateTime[nodeCount];
            lsEnd = new DateTime[nodeCount];
            processStatus=new int[nodeCount];
           
            for (int i = 0; i < nodeCount; i++)
            {
                lsStart[i] = Convert.ToDateTime("1980/01/01 00:00:00");
                lsEnd[i] = Convert.ToDateTime("1980/01/01 00:00:00");
                processStatus[i] = 1; //0=nil, 1=DSM, 2=Prepare to Restore, 3=Restoring, 4=Restore Done

            }
        }
        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            //if (e.Column.FieldName == "Energy")
            //{
            //    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            //}
            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        }

        private void cmbRetailer_SelectedIndexChanged(object sender, EventArgs e)
        {
            retailerId = Convert.ToInt32( dtRetailers.Rows[cmbRetailer.SelectedIndex]["Oid"]);
            Connections.TariffsByRetailer(retailerId,out dtTariffs);
            for (int i = 0; i < dtTariffs.Rows.Count; i++)
            {
                cmbTariff.Items.Add(dtTariffs.Rows[i]["TariffName"]);
            }

            cmbTariff.SelectedIndex = 2;
        }

        private void cmbTariff_SelectedIndexChanged(object sender, EventArgs e)
        {
            tariffId= Convert.ToInt32(dtTariffs.Rows[cmbTariff.SelectedIndex]["TariffID"]);
        }

        
    }
}