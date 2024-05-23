namespace VATClient
{
    partial class FormIntegrationRecon
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
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.btnDownload = new System.Windows.Forms.Button();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.txtRef = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgwDeleteProcess = new System.ComponentModel.BackgroundWorker();
            this.grbTransactionHistory.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.btnDownload);
            this.grbTransactionHistory.Controls.Add(this.cmbType);
            this.grbTransactionHistory.Controls.Add(this.txtRef);
            this.grbTransactionHistory.Location = new System.Drawing.Point(2, 7);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(619, 219);
            this.grbTransactionHistory.TabIndex = 115;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Prosess";
            // 
            // btnDownload
            // 
            this.btnDownload.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDownload.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDownload.Image = global::VATClient.Properties.Resources.administrator;
            this.btnDownload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDownload.Location = new System.Drawing.Point(411, 24);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(113, 24);
            this.btnDownload.TabIndex = 14;
            this.btnDownload.Text = "&Download";
            this.btnDownload.UseVisualStyleBackColor = false;
            this.btnDownload.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmbType
            // 
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "Invoice_No",
            "Ref"});
            this.cmbType.Location = new System.Drawing.Point(87, 23);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(121, 25);
            this.cmbType.TabIndex = 1;
            // 
            // txtRef
            // 
            this.txtRef.Location = new System.Drawing.Point(214, 23);
            this.txtRef.Name = "txtRef";
            this.txtRef.Size = new System.Drawing.Size(182, 24);
            this.txtRef.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Location = new System.Drawing.Point(2, 362);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(635, 40);
            this.panel1.TabIndex = 11;
            this.panel1.TabStop = true;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(4, 12);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(121, 21);
            this.LRecordCount.TabIndex = 221;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(544, 5);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 13;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // bgwUpdate
            // 
            this.bgwUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUpdate_DoWork);
            this.bgwUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUpdate_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(40, 232);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(506, 43);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 211;
            this.progressBar1.Visible = false;
            // 
            // bgwDeleteProcess
            // 
            this.bgwDeleteProcess.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwDeleteProcess_DoWork);
            this.bgwDeleteProcess.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwDeleteProcess_RunWorkerCompleted);
            // 
            // FormIntegrationRecon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(633, 400);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.Name = "FormIntegrationRecon";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Reconciliation";
            this.Load += new System.EventHandler(this.FormIssueMultiple_Load);
            this.grbTransactionHistory.ResumeLayout(false);
            this.grbTransactionHistory.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.Label LRecordCount;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwDeleteProcess;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.TextBox txtRef;
    }
}