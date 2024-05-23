namespace VATClient.ReportPreview
{
    partial class FormSaleSummary
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
            this.label16 = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Download = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblTransferTo = new System.Windows.Forms.Label();
            this.cmbReportName = new System.Windows.Forms.ComboBox();
            this.bgwPreview = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(15, 18);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(42, 17);
            this.label16.TabIndex = 530;
            this.label16.Text = "Date:";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(126, 15);
            this.dtpFromDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.ShowCheckBox = true;
            this.dtpFromDate.Size = new System.Drawing.Size(161, 22);
            this.dtpFromDate.TabIndex = 532;
            this.dtpFromDate.Value = new System.DateTime(2024, 1, 31, 0, 0, 0, 0);
            // 
            // dtpToDate
            // 
            this.dtpToDate.Checked = false;
            this.dtpToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(340, 15);
            this.dtpToDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.ShowCheckBox = true;
            this.dtpToDate.Size = new System.Drawing.Size(165, 22);
            this.dtpToDate.TabIndex = 534;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(307, 18);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 17);
            this.label3.TabIndex = 533;
            this.label3.Text = "to";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.Download);
            this.panel1.Location = new System.Drawing.Point(-3, 135);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(533, 49);
            this.panel1.TabIndex = 531;
            // 
            // Download
            // 
            this.Download.Image = global::VATClient.Properties.Resources.Load;
            this.Download.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Download.Location = new System.Drawing.Point(209, 8);
            this.Download.Margin = new System.Windows.Forms.Padding(4);
            this.Download.Name = "Download";
            this.Download.Size = new System.Drawing.Size(115, 33);
            this.Download.TabIndex = 110;
            this.Download.Text = "Download";
            this.Download.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Download.UseVisualStyleBackColor = true;
            this.Download.Click += new System.EventHandler(this.Download_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(77, 107);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(333, 20);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 535;
            this.progressBar1.Visible = false;
            // 
            // lblTransferTo
            // 
            this.lblTransferTo.AutoSize = true;
            this.lblTransferTo.Location = new System.Drawing.Point(15, 55);
            this.lblTransferTo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTransferTo.Name = "lblTransferTo";
            this.lblTransferTo.Size = new System.Drawing.Size(92, 17);
            this.lblTransferTo.TabIndex = 537;
            this.lblTransferTo.Text = "Report Name";
            // 
            // cmbReportName
            // 
            this.cmbReportName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReportName.FormattingEnabled = true;
            this.cmbReportName.Items.AddRange(new object[] {
            "DayWise",
            "BranchWise",
            "ItemWise"});
            this.cmbReportName.Location = new System.Drawing.Point(121, 51);
            this.cmbReportName.Margin = new System.Windows.Forms.Padding(4);
            this.cmbReportName.Name = "cmbReportName";
            this.cmbReportName.Size = new System.Drawing.Size(166, 24);
            this.cmbReportName.TabIndex = 536;
            // 
            // bgwPreview
            // 
            this.bgwPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPreview_DoWork);
            this.bgwPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPreview_RunWorkerCompleted);
            // 
            // FormSaleSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(527, 183);
            this.Controls.Add(this.lblTransferTo);
            this.Controls.Add(this.cmbReportName);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.dtpFromDate);
            this.Controls.Add(this.dtpToDate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "FormSaleSummary";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sale Summary";
            this.Load += new System.EventHandler(this.FormSaleSummary_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Download;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblTransferTo;
        private System.Windows.Forms.ComboBox cmbReportName;
        private System.ComponentModel.BackgroundWorker bgwPreview;

    }
}