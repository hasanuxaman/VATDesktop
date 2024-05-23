namespace VATClient
{
    partial class Form6_1Process
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
            this.label11 = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.chkProduct = new System.Windows.Forms.CheckBox();
            this.txtProName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStartProcess = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.txtProductCode = new System.Windows.Forms.TextBox();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgwDeleteProcess = new System.ComponentModel.BackgroundWorker();
            this.btnDay = new System.Windows.Forms.Button();
            this.btnBranch = new System.Windows.Forms.Button();
            this.bgwProcessDay = new System.ComponentModel.BackgroundWorker();
            this.btnBranchDay = new System.Windows.Forms.Button();
            this.grbTransactionHistory.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(485, 408);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 17);
            this.label11.TabIndex = 112;
            this.label11.Text = "To Date";
            this.label11.Visible = false;
            // 
            // dtpToDate
            // 
            this.dtpToDate.Checked = false;
            this.dtpToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpToDate.Enabled = false;
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(346, 75);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(103, 24);
            this.dtpToDate.TabIndex = 3;
            // 
            // dtpDate
            // 
            this.dtpDate.Checked = false;
            this.dtpDate.CustomFormat = "MMM-yyyy";
            this.dtpDate.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.Location = new System.Drawing.Point(191, 44);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(173, 29);
            this.dtpDate.TabIndex = 2;
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtpIssueFromDate_ValueChanged);
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.btnBranchDay);
            this.grbTransactionHistory.Controls.Add(this.btnBranch);
            this.grbTransactionHistory.Controls.Add(this.btnDay);
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Controls.Add(this.label2);
            this.grbTransactionHistory.Controls.Add(this.dtpFromDate);
            this.grbTransactionHistory.Controls.Add(this.btnDelete);
            this.grbTransactionHistory.Controls.Add(this.btnDownload);
            this.grbTransactionHistory.Controls.Add(this.chkProduct);
            this.grbTransactionHistory.Controls.Add(this.dtpToDate);
            this.grbTransactionHistory.Controls.Add(this.txtProName);
            this.grbTransactionHistory.Controls.Add(this.label10);
            this.grbTransactionHistory.Controls.Add(this.btnUpdate);
            this.grbTransactionHistory.Controls.Add(this.dtpDate);
            this.grbTransactionHistory.Controls.Add(this.label3);
            this.grbTransactionHistory.Location = new System.Drawing.Point(2, 7);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(587, 267);
            this.grbTransactionHistory.TabIndex = 115;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Prosess";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(62, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 21);
            this.label1.TabIndex = 574;
            this.label1.Text = "From Date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(310, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 21);
            this.label2.TabIndex = 573;
            this.label2.Text = "To";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Checked = false;
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpFromDate.Enabled = false;
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(191, 75);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(103, 24);
            this.dtpFromDate.TabIndex = 572;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnDelete.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(194, 124);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(170, 57);
            this.btnDelete.TabIndex = 571;
            this.btnDelete.Text = "&Delete Process";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnDownload.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownload.Image = global::VATClient.Properties.Resources.Print;
            this.btnDownload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDownload.Location = new System.Drawing.Point(374, 124);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(188, 57);
            this.btnDownload.TabIndex = 570;
            this.btnDownload.Text = "Download Negative Data";
            this.btnDownload.UseVisualStyleBackColor = false;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // chkProduct
            // 
            this.chkProduct.AutoSize = true;
            this.chkProduct.Location = new System.Drawing.Point(404, 19);
            this.chkProduct.Name = "chkProduct";
            this.chkProduct.Size = new System.Drawing.Size(112, 21);
            this.chkProduct.TabIndex = 562;
            this.chkProduct.Text = "With Product";
            this.chkProduct.UseVisualStyleBackColor = true;
            this.chkProduct.CheckedChanged += new System.EventHandler(this.chkProduct_CheckedChanged);
            // 
            // txtProName
            // 
            this.txtProName.Location = new System.Drawing.Point(191, 17);
            this.txtProName.Margin = new System.Windows.Forms.Padding(4);
            this.txtProName.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtProName.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtProName.Name = "txtProName";
            this.txtProName.Size = new System.Drawing.Size(199, 24);
            this.txtProName.TabIndex = 561;
            this.txtProName.DoubleClick += new System.EventHandler(this.txtProName_DoubleClick);
            this.txtProName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProName_KeyDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(62, 20);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(121, 17);
            this.label10.TabIndex = 560;
            this.label10.Text = "Product Name(F9)";
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnUpdate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(8, 124);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(170, 57);
            this.btnUpdate.TabIndex = 9;
            this.btnUpdate.Text = "Process";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(62, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 21);
            this.label3.TabIndex = 3;
            this.label3.Text = "Month";
            // 
            // btnStartProcess
            // 
            this.btnStartProcess.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnStartProcess.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartProcess.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnStartProcess.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStartProcess.Location = new System.Drawing.Point(147, 8);
            this.btnStartProcess.Name = "btnStartProcess";
            this.btnStartProcess.Size = new System.Drawing.Size(159, 32);
            this.btnStartProcess.TabIndex = 10;
            this.btnStartProcess.Text = "Re-Process";
            this.btnStartProcess.UseVisualStyleBackColor = false;
            this.btnStartProcess.Visible = false;
            this.btnStartProcess.Click += new System.EventHandler(this.btnStartProcess_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnStartProcess);
            this.panel1.Controls.Add(this.txtItemNo);
            this.panel1.Controls.Add(this.txtProductCode);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Location = new System.Drawing.Point(2, 365);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(603, 40);
            this.panel1.TabIndex = 11;
            this.panel1.TabStop = true;
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(192, 16);
            this.txtItemNo.Margin = new System.Windows.Forms.Padding(4);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.Size = new System.Drawing.Size(199, 24);
            this.txtItemNo.TabIndex = 562;
            this.txtItemNo.Visible = false;
            // 
            // txtProductCode
            // 
            this.txtProductCode.Location = new System.Drawing.Point(147, 16);
            this.txtProductCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtProductCode.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtProductCode.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.Size = new System.Drawing.Size(199, 24);
            this.txtProductCode.TabIndex = 563;
            this.txtProductCode.Visible = false;
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
            this.btnOk.Location = new System.Drawing.Point(525, 9);
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
            this.progressBar1.Location = new System.Drawing.Point(2, 280);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(587, 43);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 211;
            this.progressBar1.Visible = false;
            // 
            // bgwDeleteProcess
            // 
            this.bgwDeleteProcess.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwDeleteProcess_DoWork);
            this.bgwDeleteProcess.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwDeleteProcess_RunWorkerCompleted);
            // 
            // btnDay
            // 
            this.btnDay.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnDay.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDay.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnDay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDay.Location = new System.Drawing.Point(8, 187);
            this.btnDay.Name = "btnDay";
            this.btnDay.Size = new System.Drawing.Size(175, 57);
            this.btnDay.TabIndex = 575;
            this.btnDay.Text = "Process Day";
            this.btnDay.UseVisualStyleBackColor = false;
            this.btnDay.Click += new System.EventHandler(this.btnDay_Click);
            // 
            // btnBranch
            // 
            this.btnBranch.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnBranch.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBranch.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnBranch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBranch.Location = new System.Drawing.Point(194, 187);
            this.btnBranch.Name = "btnBranch";
            this.btnBranch.Size = new System.Drawing.Size(170, 57);
            this.btnBranch.TabIndex = 576;
            this.btnBranch.Text = "Branch Process";
            this.btnBranch.UseVisualStyleBackColor = false;
            this.btnBranch.Click += new System.EventHandler(this.btnBranch_Click);
            // 
            // bgwProcessDay
            // 
            this.bgwProcessDay.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwProcessDay_DoWork);
            this.bgwProcessDay.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwProcessDay_RunWorkerCompleted);
            // 
            // btnBranchDay
            // 
            this.btnBranchDay.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnBranchDay.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBranchDay.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnBranchDay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBranchDay.Location = new System.Drawing.Point(370, 187);
            this.btnBranchDay.Name = "btnBranchDay";
            this.btnBranchDay.Size = new System.Drawing.Size(192, 57);
            this.btnBranchDay.TabIndex = 577;
            this.btnBranchDay.Text = "Branch Day Process";
            this.btnBranchDay.UseVisualStyleBackColor = false;
            this.btnBranchDay.Click += new System.EventHandler(this.btnBranchDay_Click);
            // 
            // Form6_1Process
            // 
            this.AcceptButton = this.btnUpdate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(601, 404);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.Name = "Form6_1Process";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "6.1 Permanent Process";
            this.Load += new System.EventHandler(this.FormIssueMultiple_Load);
            this.grbTransactionHistory.ResumeLayout(false);
            this.grbTransactionHistory.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label LRecordCount;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnStartProcess;
        public System.Windows.Forms.TextBox txtProName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkProduct;
        public System.Windows.Forms.TextBox txtItemNo;
        public System.Windows.Forms.TextBox txtProductCode;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnDelete;
        private System.ComponentModel.BackgroundWorker bgwDeleteProcess;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDay;
        private System.Windows.Forms.Button btnBranch;
        private System.ComponentModel.BackgroundWorker bgwProcessDay;
        private System.Windows.Forms.Button btnBranchDay;
    }
}