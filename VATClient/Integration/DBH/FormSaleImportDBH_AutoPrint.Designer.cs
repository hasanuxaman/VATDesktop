namespace VATClient.Integration.DBH
{
    partial class FormSaleImportDBH_AutoPrint
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnTimerStop = new System.Windows.Forms.Button();
            this.btnTimerStart = new System.Windows.Forms.Button();
            this.bgwBigData = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.bgwDataSaveAndProcess = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 30000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnTimerStop
            // 
            this.btnTimerStop.BackColor = System.Drawing.Color.White;
            this.btnTimerStop.Location = new System.Drawing.Point(264, 275);
            this.btnTimerStop.Name = "btnTimerStop";
            this.btnTimerStop.Size = new System.Drawing.Size(97, 34);
            this.btnTimerStop.TabIndex = 21;
            this.btnTimerStop.Text = "Process Stop";
            this.btnTimerStop.UseVisualStyleBackColor = false;
            this.btnTimerStop.Click += new System.EventHandler(this.btnTimerStop_Click);
            // 
            // btnTimerStart
            // 
            this.btnTimerStart.BackColor = System.Drawing.Color.White;
            this.btnTimerStart.Location = new System.Drawing.Point(106, 274);
            this.btnTimerStart.Name = "btnTimerStart";
            this.btnTimerStart.Size = new System.Drawing.Size(97, 34);
            this.btnTimerStart.TabIndex = 20;
            this.btnTimerStart.Text = "Process Start";
            this.btnTimerStart.UseVisualStyleBackColor = false;
            this.btnTimerStart.Click += new System.EventHandler(this.btnTimerStart_Click);
            // 
            // bgwBigData
            // 
            this.bgwBigData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwBigData_DoWork);
            this.bgwBigData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwBigData_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(47, 237);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(368, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 23;
            this.progressBar1.Visible = false;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 10);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(439, 212);
            this.listBox1.TabIndex = 24;
            // 
            // bgwDataSaveAndProcess
            // 
            this.bgwDataSaveAndProcess.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwDataSaveAndProcess_DoWork);
            this.bgwDataSaveAndProcess.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwDataSaveAndProcess_RunWorkerCompleted);
            // 
            // FormSaleImportDBH_AutoPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(463, 321);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnTimerStop);
            this.Controls.Add(this.btnTimerStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FormSaleImportDBH_AutoPrint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sale Import DBH ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSaleImportDHLAirport_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSaleImportDHLAirport_FormClosed);
            this.Load += new System.EventHandler(this.FormSaleImportDHLAirport_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnTimerStop;
        private System.Windows.Forms.Button btnTimerStart;
        private System.ComponentModel.BackgroundWorker bgwBigData;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ListBox listBox1;
        private System.ComponentModel.BackgroundWorker bgwDataSaveAndProcess;
    }
}