namespace DEHWControl
{
    partial class Main
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
            this.groupControlSimulate = new DevExpress.XtraEditors.GroupControl();
            this.label2 = new System.Windows.Forms.Label();
            this.Timer = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonResume = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.groupControlSimulate)).BeginInit();
            this.groupControlSimulate.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupControlSimulate
            // 
            this.groupControlSimulate.Controls.Add(this.label2);
            this.groupControlSimulate.Controls.Add(this.Timer);
            this.groupControlSimulate.Controls.Add(this.label9);
            this.groupControlSimulate.Controls.Add(this.buttonResume);
            this.groupControlSimulate.Controls.Add(this.buttonPause);
            this.groupControlSimulate.Controls.Add(this.buttonStart);
            this.groupControlSimulate.Controls.Add(this.label1);
            this.groupControlSimulate.Controls.Add(this.dateTimePicker1);
            this.groupControlSimulate.Controls.Add(this.label3);
            this.groupControlSimulate.Controls.Add(this.comboBox1);
            this.groupControlSimulate.Location = new System.Drawing.Point(48, 3);
            this.groupControlSimulate.Name = "groupControlSimulate";
            this.groupControlSimulate.Size = new System.Drawing.Size(675, 296);
            this.groupControlSimulate.TabIndex = 0;
            this.groupControlSimulate.Text = "Simmulate DEHW Thermostat Switching";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(85, 226);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 31);
            this.label2.TabIndex = 60;
            this.label2.Text = "Timer:";
            // 
            // Timer
            // 
            this.Timer.AutoSize = true;
            this.Timer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Timer.Location = new System.Drawing.Point(197, 224);
            this.Timer.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Timer.Name = "Timer";
            this.Timer.Size = new System.Drawing.Size(141, 37);
            this.Timer.TabIndex = 59;
            this.Timer.Text = "13:00:00";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(28, 232);
            this.label9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(0, 25);
            this.label9.TabIndex = 57;
            // 
            // buttonResume
            // 
            this.buttonResume.Location = new System.Drawing.Point(388, 151);
            this.buttonResume.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.buttonResume.Name = "buttonResume";
            this.buttonResume.Size = new System.Drawing.Size(149, 43);
            this.buttonResume.TabIndex = 54;
            this.buttonResume.Text = "Resume";
            this.buttonResume.UseVisualStyleBackColor = true;
            // 
            // buttonPause
            // 
            this.buttonPause.Location = new System.Drawing.Point(204, 156);
            this.buttonPause.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(149, 43);
            this.buttonPause.TabIndex = 53;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(42, 156);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(149, 43);
            this.buttonStart.TabIndex = 52;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 64);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 25);
            this.label1.TabIndex = 51;
            this.label1.Text = "Select start time:";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(226, 61);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(396, 33);
            this.dateTimePicker1.TabIndex = 50;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 109);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(218, 25);
            this.label3.TabIndex = 49;
            this.label3.Text = "Select time multiplier:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "1",
            "15",
            "30",
            "60",
            "120",
            "900"});
            this.comboBox1.Location = new System.Drawing.Point(265, 106);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(132, 33);
            this.comboBox1.TabIndex = 48;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2269, 1232);
            this.Controls.Add(this.groupControlSimulate);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Main";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControlSimulate)).EndInit();
            this.groupControlSimulate.ResumeLayout(false);
            this.groupControlSimulate.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControlSimulate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonResume;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label Timer;
    }
}

