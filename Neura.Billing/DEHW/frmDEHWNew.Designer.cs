namespace Neura.Billing.DEHW
{
    partial class frmDEHWNew
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControlTime = new DevExpress.XtraEditors.LabelControl();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.chkMonitor = new System.Windows.Forms.CheckBox();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.checkEditSimulate = new DevExpress.XtraEditors.CheckEdit();
            this.buttonResume = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbMultiplier = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.end = new DevExpress.XtraEditors.TextEdit();
            this.start = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditSimulate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.end.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.start.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(81, 357);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1614, 973);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(1213, 124);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(125, 25);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "Process time:";
            // 
            // labelControlTime
            // 
            this.labelControlTime.Location = new System.Drawing.Point(1376, 124);
            this.labelControlTime.Name = "labelControlTime";
            this.labelControlTime.Size = new System.Drawing.Size(51, 25);
            this.labelControlTime.TabIndex = 3;
            this.labelControlTime.Text = "00:00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 66);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 25);
            this.label1.TabIndex = 46;
            this.label1.Text = "Select start time:";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(272, 63);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(396, 33);
            this.dateTimePicker1.TabIndex = 45;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // chkMonitor
            // 
            this.chkMonitor.AutoSize = true;
            this.chkMonitor.Location = new System.Drawing.Point(869, 7);
            this.chkMonitor.Name = "chkMonitor";
            this.chkMonitor.Size = new System.Drawing.Size(114, 29);
            this.chkMonitor.TabIndex = 47;
            this.chkMonitor.Text = "Monitor";
            this.chkMonitor.UseVisualStyleBackColor = true;
            this.chkMonitor.CheckedChanged += new System.EventHandler(this.chkMonitor_CheckedChanged);
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.label8);
            this.groupControl1.Controls.Add(this.label9);
            this.groupControl1.Controls.Add(this.end);
            this.groupControl1.Controls.Add(this.start);
            this.groupControl1.Controls.Add(this.buttonResume);
            this.groupControl1.Controls.Add(this.buttonPause);
            this.groupControl1.Controls.Add(this.buttonStart);
            this.groupControl1.Controls.Add(this.label3);
            this.groupControl1.Controls.Add(this.cmbMultiplier);
            this.groupControl1.Controls.Add(this.dateTimePicker1);
            this.groupControl1.Controls.Add(this.label1);
            this.groupControl1.Location = new System.Drawing.Point(141, 47);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(704, 282);
            this.groupControl1.TabIndex = 48;
            this.groupControl1.Text = "Simulation";
            // 
            // checkEditSimulate
            // 
            this.checkEditSimulate.AllowDrop = true;
            this.checkEditSimulate.Location = new System.Drawing.Point(141, 1);
            this.checkEditSimulate.Name = "checkEditSimulate";
            this.checkEditSimulate.Properties.Caption = "Simmulation Mode";
            this.checkEditSimulate.Size = new System.Drawing.Size(256, 40);
            this.checkEditSimulate.TabIndex = 49;
            this.checkEditSimulate.CheckedChanged += new System.EventHandler(this.checkEditSimulate_CheckedChanged);
            // 
            // buttonResume
            // 
            this.buttonResume.Location = new System.Drawing.Point(385, 209);
            this.buttonResume.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.buttonResume.Name = "buttonResume";
            this.buttonResume.Size = new System.Drawing.Size(149, 43);
            this.buttonResume.TabIndex = 54;
            this.buttonResume.Text = "Resume";
            this.buttonResume.UseVisualStyleBackColor = true;
            this.buttonResume.Click += new System.EventHandler(this.buttonResume_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.Location = new System.Drawing.Point(213, 209);
            this.buttonPause.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(149, 43);
            this.buttonPause.TabIndex = 53;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(54, 209);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(149, 43);
            this.buttonStart.TabIndex = 52;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 110);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(218, 25);
            this.label3.TabIndex = 51;
            this.label3.Text = "Select time multiplier:";
            // 
            // cmbMultiplier
            // 
            this.cmbMultiplier.FormattingEnabled = true;
            this.cmbMultiplier.Items.AddRange(new object[] {
            "1",
            "15",
            "30",
            "60",
            "120",
            "900"});
            this.cmbMultiplier.Location = new System.Drawing.Point(286, 107);
            this.cmbMultiplier.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.cmbMultiplier.Name = "cmbMultiplier";
            this.cmbMultiplier.Size = new System.Drawing.Size(132, 33);
            this.cmbMultiplier.TabIndex = 50;
            this.cmbMultiplier.SelectedIndexChanged += new System.EventHandler(this.cmbMultiplier_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(468, 157);
            this.label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 25);
            this.label8.TabIndex = 58;
            this.label8.Text = "and";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(32, 158);
            this.label9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(273, 25);
            this.label9.TabIndex = 57;
            this.label9.Text = "Thermostat  range between";
            // 
            // end
            // 
            this.end.EditValue = "100";
            this.end.Location = new System.Drawing.Point(541, 150);
            this.end.Name = "end";
            this.end.Size = new System.Drawing.Size(108, 40);
            this.end.TabIndex = 56;
            // 
            // start
            // 
            this.start.EditValue = "95";
            this.start.Location = new System.Drawing.Point(345, 150);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(115, 40);
            this.start.TabIndex = 55;
            // 
            // frmDEHWNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2132, 1364);
            this.Controls.Add(this.checkEditSimulate);
            this.Controls.Add(this.groupControl1);
            this.Controls.Add(this.chkMonitor);
            this.Controls.Add(this.labelControlTime);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.gridControl1);
            this.Name = "frmDEHWNew";
            this.Text = "frmDEHWNew";
            this.Load += new System.EventHandler(this.frmDEHWNew_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditSimulate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.end.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.start.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControlTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.CheckBox chkMonitor;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.CheckEdit checkEditSimulate;
        private System.Windows.Forms.Button buttonResume;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbMultiplier;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private DevExpress.XtraEditors.TextEdit end;
        private DevExpress.XtraEditors.TextEdit start;
    }
}