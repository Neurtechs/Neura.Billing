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
using Neura.Billing.DEHWData;

namespace Neura.Billing.DEHW
{
    
    public partial class frmDEHWNew : DevExpress.XtraEditors.XtraForm
    {
        private DataTable dt; //thermodata
        private DataTable dtTable; //extract of thermodata for table
        private DataTable dtNodeData;
        private int nodeCount;
        private int thermoCount;
        private int[] nodeOid;
        private string[] serialNo;
        private int[] thermoStatus;
        private DateTime[] lastSwitch;
        private DateTime startTime;
        private DateTime[] timeStartCooling;
        private DateTime[] timeStartHeating;
        private DateTime processTime;
        private int[] nodeStatus;
        private double[] power;
        private double[] value;
        private int[] coolTime;
        private int[] heatTime; //sec
        private double[] coolGrad;
        private double[] heatGrad;
        private double[] hWIndex;
        private bool bStarted = false;
        private int multiplier = 1;

        private double[] baseCooling;
        private double[] baseHeating;
        private Random rand;
        private Random rand2;
        private double thermoStart;
        private double thermoEnd;

        
        private double[] index;
        public frmDEHWNew()
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
        }
        private void GetNodeData()
        {
            Connections.GetGeyserNodes(out dtNodeData);
            nodeCount = dtNodeData.Rows.Count;

            thermoStatus = new int[nodeCount];
            nodeStatus = new int[nodeCount];
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
                    index[i] = thermoStart;
                    thermoStatus[i] = 1;
                    TimeSpan ts = myTime - lastSwitch[i];
                    heatTime[i] = (int)ts.TotalSeconds;
                    coolTime[i] = 0;
                    hWIndex[i] = index[i] + heatGrad[i] * heatTime[i];
                }
                else
                {
                    index[i] = thermoEnd;
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
            int myRows = thermoCount;
            if (myRows > 20) { myRows = 20;}

            string sRow;
            dtTable.Rows.Clear();
            for (int i = 0; i < myRows; i++)
            {
                sRow = (i+1).ToString();
                if (sRow.Length == 1) { sRow = "0" + sRow; }

                dtTable.Rows.Add();
                dtTable.Rows[i]["Item"] = sRow;
                dtTable.Rows[i]["SerialNo"] = serialNo[i];
                dtTable.Rows[i]["ThermoStatus"] = thermoStatus[i];
                dtTable.Rows[i]["LastSwitch"] = lastSwitch[i];
                dtTable.Rows[i]["NodeStatus"] = 1;
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
            if(chkMonitor.Checked==true){PopulateDG();}
            processTime = processTime.AddSeconds(5);
            if(bStarted==true){Simulate();}
        }

        private void Simulate()
        {
            for (int i = 0; i < thermoCount; i++)
            {
                if (thermoStatus[i] == 1)
                {
                    //busy warming
                    TimeSpan duration = (processTime - timeStartHeating[i]);
                    double dur = Convert.ToDouble(duration.TotalSeconds);
                    index[i] = thermoStart;
                    hWIndex[i] = index[i] + heatGrad[i] * dur;
                    if (hWIndex[i] > thermoEnd)
                    {
                        //End of heating
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
                    TimeSpan duration = (processTime - timeStartCooling[i]);
                    double dur = Convert.ToDouble(duration.TotalSeconds);
                    index[i] = thermoEnd;
                    hWIndex[i] = index[i] + coolGrad[i] * dur;
                   
                    if (hWIndex[i] < thermoStart)
                    {
                        //End of cooling
                        index[i] = thermoStart;
                        
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
                    
                    timer1.Interval = 5000;
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
                    
                    timer1.Interval = 5000;
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

            
            timer1.Interval = 5000/multiplier;

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
            if(checkEditSimulate.Checked==true){timer1.Interval = 5000 / multiplier;}
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
            }
        }
    }
}