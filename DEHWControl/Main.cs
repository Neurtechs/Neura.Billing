using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DEHWControl.Data;

namespace DEHWControl
{
    public partial class Main : DevExpress.XtraEditors.XtraForm
    {
        private double[] power;
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            //Get all geysers
            Connections.GetGeyserNodes(out DataTable dtGeyserNodes);
            power= new double[dtGeyserNodes.Rows.Count];
        }
    }
}
