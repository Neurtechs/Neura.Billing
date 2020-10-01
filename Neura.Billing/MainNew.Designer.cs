namespace Neura.Billing
{
    partial class MainNew
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainNew));
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.textEditLimit = new DevExpress.XtraEditors.TextEdit();
            this.checkLimit = new DevExpress.XtraEditors.CheckEdit();
            this.textEditCyclePeriod = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.checkEditResult = new DevExpress.XtraEditors.CheckEdit();
            this.simpleButtonStartBilling = new DevExpress.XtraEditors.SimpleButton();
            this.checkEditLogTest = new DevExpress.XtraEditors.CheckEdit();
            this.simpleButtonStopBilling = new DevExpress.XtraEditors.SimpleButton();
            this.timerBilling = new System.Windows.Forms.Timer(this.components);
            this.listBoxControl1 = new DevExpress.XtraEditors.ListBoxControl();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEditLimit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkLimit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCyclePeriod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditResult.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditLogTest.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.textEditLimit);
            this.groupControl1.Controls.Add(this.checkLimit);
            this.groupControl1.Controls.Add(this.textEditCyclePeriod);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Controls.Add(this.checkEditResult);
            this.groupControl1.Controls.Add(this.simpleButtonStartBilling);
            this.groupControl1.Controls.Add(this.checkEditLogTest);
            this.groupControl1.Controls.Add(this.simpleButtonStopBilling);
            this.groupControl1.Location = new System.Drawing.Point(89, 1003);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(697, 226);
            this.groupControl1.TabIndex = 20;
            this.groupControl1.Text = "Run Billing";
            // 
            // textEditLimit
            // 
            this.textEditLimit.EditValue = "100";
            this.textEditLimit.Location = new System.Drawing.Point(460, 102);
            this.textEditLimit.Name = "textEditLimit";
            this.textEditLimit.Size = new System.Drawing.Size(84, 40);
            this.textEditLimit.TabIndex = 14;
            // 
            // checkLimit
            // 
            this.checkLimit.Location = new System.Drawing.Point(397, 48);
            this.checkLimit.Name = "checkLimit";
            this.checkLimit.Properties.Caption = "Limit readings/cycle:";
            this.checkLimit.Size = new System.Drawing.Size(241, 40);
            this.checkLimit.TabIndex = 13;
            this.checkLimit.CheckedChanged += new System.EventHandler(this.checkLimit_CheckedChanged);
            // 
            // textEditCyclePeriod
            // 
            this.textEditCyclePeriod.EditValue = "600";
            this.textEditCyclePeriod.Location = new System.Drawing.Point(257, 49);
            this.textEditCyclePeriod.Name = "textEditCyclePeriod";
            this.textEditCyclePeriod.Size = new System.Drawing.Size(84, 40);
            this.textEditCyclePeriod.TabIndex = 7;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(18, 55);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(219, 25);
            this.labelControl1.TabIndex = 8;
            this.labelControl1.Text = "Cycle period (seconds):";
            // 
            // checkEditResult
            // 
            this.checkEditResult.EditValue = true;
            this.checkEditResult.Location = new System.Drawing.Point(200, 162);
            this.checkEditResult.Name = "checkEditResult";
            this.checkEditResult.Properties.Caption = "Results Log";
            this.checkEditResult.Size = new System.Drawing.Size(150, 40);
            this.checkEditResult.TabIndex = 12;
            // 
            // simpleButtonStartBilling
            // 
            this.simpleButtonStartBilling.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButtonStartBilling.ImageOptions.Image")));
            this.simpleButtonStartBilling.Location = new System.Drawing.Point(18, 98);
            this.simpleButtonStartBilling.Name = "simpleButtonStartBilling";
            this.simpleButtonStartBilling.Size = new System.Drawing.Size(150, 46);
            this.simpleButtonStartBilling.TabIndex = 9;
            this.simpleButtonStartBilling.Text = "Start";
            this.simpleButtonStartBilling.Click += new System.EventHandler(this.simpleButtonStartBilling_Click);
            // 
            // checkEditLogTest
            // 
            this.checkEditLogTest.EditValue = true;
            this.checkEditLogTest.Location = new System.Drawing.Point(33, 162);
            this.checkEditLogTest.Name = "checkEditLogTest";
            this.checkEditLogTest.Properties.Caption = "Test Log";
            this.checkEditLogTest.Size = new System.Drawing.Size(150, 40);
            this.checkEditLogTest.TabIndex = 11;
            // 
            // simpleButtonStopBilling
            // 
            this.simpleButtonStopBilling.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButtonStopBilling.ImageOptions.Image")));
            this.simpleButtonStopBilling.Location = new System.Drawing.Point(191, 98);
            this.simpleButtonStopBilling.Name = "simpleButtonStopBilling";
            this.simpleButtonStopBilling.Size = new System.Drawing.Size(150, 46);
            this.simpleButtonStopBilling.TabIndex = 10;
            this.simpleButtonStopBilling.Text = "Stop";
            this.simpleButtonStopBilling.Click += new System.EventHandler(this.simpleButtonStopBilling_Click);
            // 
            // timerBilling
            // 
            this.timerBilling.Tick += new System.EventHandler(this.timerBilling_Tick);
            // 
            // listBoxControl1
            // 
            this.listBoxControl1.Location = new System.Drawing.Point(89, 12);
            this.listBoxControl1.Name = "listBoxControl1";
            this.listBoxControl1.Size = new System.Drawing.Size(383, 412);
            this.listBoxControl1.TabIndex = 21;
            // 
            // MainNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2285, 1268);
            this.Controls.Add(this.listBoxControl1);
            this.Controls.Add(this.groupControl1);
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("MainNew.IconOptions.Image")));
            this.Name = "MainNew";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.MainNew_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEditLimit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkLimit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCyclePeriod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditResult.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditLogTest.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.TextEdit textEditLimit;
        private DevExpress.XtraEditors.CheckEdit checkLimit;
        private DevExpress.XtraEditors.TextEdit textEditCyclePeriod;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckEdit checkEditResult;
        private DevExpress.XtraEditors.SimpleButton simpleButtonStartBilling;
        private DevExpress.XtraEditors.CheckEdit checkEditLogTest;
        private DevExpress.XtraEditors.SimpleButton simpleButtonStopBilling;
        private System.Windows.Forms.Timer timerBilling;
        private DevExpress.XtraEditors.ListBoxControl listBoxControl1;
    }
}