namespace VATClient
{
    partial class FormIssueMultiple
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormIssueMultiple));
            this.label11 = new System.Windows.Forms.Label();
            this.dtpIssueToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpIssueFromDate = new System.Windows.Forms.DateTimePicker();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.chkProduct = new System.Windows.Forms.CheckBox();
            this.txtProName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnStartProcess = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.txtProductCode = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgwRefresh = new System.ComponentModel.BackgroundWorker();
            this.btn6_2Process = new System.Windows.Forms.Button();
            this.btn6_1Process = new System.Windows.Forms.Button();
            this.btnPermanent = new System.Windows.Forms.Button();
            this.btnRegular = new System.Windows.Forms.Button();
            this.grbTransactionHistory.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(369, 36);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 17);
            this.label11.TabIndex = 112;
            this.label11.Text = "To Date";
            this.label11.Visible = false;
            // 
            // dtpIssueToDate
            // 
            this.dtpIssueToDate.Checked = false;
            this.dtpIssueToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpIssueToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueToDate.Location = new System.Drawing.Point(150, 16);
            this.dtpIssueToDate.Name = "dtpIssueToDate";
            this.dtpIssueToDate.Size = new System.Drawing.Size(103, 24);
            this.dtpIssueToDate.TabIndex = 3;
            this.dtpIssueToDate.Visible = false;
            // 
            // dtpIssueFromDate
            // 
            this.dtpIssueFromDate.Checked = false;
            this.dtpIssueFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpIssueFromDate.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpIssueFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueFromDate.Location = new System.Drawing.Point(6, 80);
            this.dtpIssueFromDate.Name = "dtpIssueFromDate";
            this.dtpIssueFromDate.Size = new System.Drawing.Size(173, 29);
            this.dtpIssueFromDate.TabIndex = 2;
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.chkProduct);
            this.grbTransactionHistory.Controls.Add(this.txtProName);
            this.grbTransactionHistory.Controls.Add(this.label10);
            this.grbTransactionHistory.Controls.Add(this.btnStartProcess);
            this.grbTransactionHistory.Controls.Add(this.btnUpdate);
            this.grbTransactionHistory.Controls.Add(this.dtpIssueFromDate);
            this.grbTransactionHistory.Controls.Add(this.label3);
            this.grbTransactionHistory.Location = new System.Drawing.Point(2, 7);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(567, 129);
            this.grbTransactionHistory.TabIndex = 115;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Prosess";
            // 
            // chkProduct
            // 
            this.chkProduct.AutoSize = true;
            this.chkProduct.Location = new System.Drawing.Point(382, 17);
            this.chkProduct.Name = "chkProduct";
            this.chkProduct.Size = new System.Drawing.Size(112, 21);
            this.chkProduct.TabIndex = 565;
            this.chkProduct.Text = "With Product";
            this.chkProduct.UseVisualStyleBackColor = true;
            this.chkProduct.CheckedChanged += new System.EventHandler(this.chkProduct_CheckedChanged);
            // 
            // txtProName
            // 
            this.txtProName.Location = new System.Drawing.Point(169, 15);
            this.txtProName.Margin = new System.Windows.Forms.Padding(4);
            this.txtProName.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtProName.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtProName.Name = "txtProName";
            this.txtProName.Size = new System.Drawing.Size(199, 24);
            this.txtProName.TabIndex = 564;
            this.txtProName.DoubleClick += new System.EventHandler(this.txtProName_DoubleClick);
            this.txtProName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProName_KeyDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(40, 18);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(121, 17);
            this.label10.TabIndex = 563;
            this.label10.Text = "Product Name(F9)";
            // 
            // btnStartProcess
            // 
            this.btnStartProcess.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnStartProcess.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartProcess.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnStartProcess.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStartProcess.Location = new System.Drawing.Point(357, 77);
            this.btnStartProcess.Name = "btnStartProcess";
            this.btnStartProcess.Size = new System.Drawing.Size(159, 32);
            this.btnStartProcess.TabIndex = 10;
            this.btnStartProcess.Text = "Re-Process";
            this.btnStartProcess.UseVisualStyleBackColor = false;
            this.btnStartProcess.Click += new System.EventHandler(this.btnStartProcess_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnUpdate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(197, 77);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(145, 34);
            this.btnUpdate.TabIndex = 9;
            this.btnUpdate.Text = "Process";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 21);
            this.label3.TabIndex = 3;
            this.label3.Text = "From Date";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.txtItemNo);
            this.panel1.Controls.Add(this.txtProductCode);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.dtpIssueToDate);
            this.panel1.Location = new System.Drawing.Point(2, 375);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(570, 40);
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
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(112, 21);
            this.txtItemNo.Margin = new System.Windows.Forms.Padding(4);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.Size = new System.Drawing.Size(199, 24);
            this.txtItemNo.TabIndex = 566;
            this.txtItemNo.Visible = false;
            // 
            // txtProductCode
            // 
            this.txtProductCode.Location = new System.Drawing.Point(112, 25);
            this.txtProductCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtProductCode.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtProductCode.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.Size = new System.Drawing.Size(199, 24);
            this.txtProductCode.TabIndex = 567;
            this.txtProductCode.Visible = false;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(446, 6);
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
            this.progressBar1.Location = new System.Drawing.Point(10, 297);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(559, 39);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 211;
            this.progressBar1.Visible = false;
            // 
            // bgwRefresh
            // 
            this.bgwRefresh.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwRefresh_DoWork);
            this.bgwRefresh.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwRefresh_RunWorkerCompleted);
            // 
            // btn6_2Process
            // 
            this.btn6_2Process.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn6_2Process.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn6_2Process.Location = new System.Drawing.Point(366, 143);
            this.btn6_2Process.Margin = new System.Windows.Forms.Padding(4);
            this.btn6_2Process.Name = "btn6_2Process";
            this.btn6_2Process.Size = new System.Drawing.Size(157, 46);
            this.btn6_2Process.TabIndex = 253;
            this.btn6_2Process.Text = "Process 6_2";
            this.btn6_2Process.UseVisualStyleBackColor = false;
            this.btn6_2Process.Click += new System.EventHandler(this.btn6_2Process_Click);
            // 
            // btn6_1Process
            // 
            this.btn6_1Process.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn6_1Process.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn6_1Process.Location = new System.Drawing.Point(213, 143);
            this.btn6_1Process.Margin = new System.Windows.Forms.Padding(4);
            this.btn6_1Process.Name = "btn6_1Process";
            this.btn6_1Process.Size = new System.Drawing.Size(145, 46);
            this.btn6_1Process.TabIndex = 252;
            this.btn6_1Process.Text = "Process 6_1";
            this.btn6_1Process.UseVisualStyleBackColor = false;
            this.btn6_1Process.Click += new System.EventHandler(this.btn6_1Process_Click);
            // 
            // btnPermanent
            // 
            this.btnPermanent.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPermanent.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPermanent.Location = new System.Drawing.Point(25, 143);
            this.btnPermanent.Margin = new System.Windows.Forms.Padding(4);
            this.btnPermanent.Name = "btnPermanent";
            this.btnPermanent.Size = new System.Drawing.Size(171, 46);
            this.btnPermanent.TabIndex = 568;
            this.btnPermanent.Text = "6.1 && 6.2";
            this.btnPermanent.UseVisualStyleBackColor = false;
            this.btnPermanent.Click += new System.EventHandler(this.btnPermanent_Click);
            // 
            // btnRegular
            // 
            this.btnRegular.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRegular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRegular.Location = new System.Drawing.Point(366, 197);
            this.btnRegular.Margin = new System.Windows.Forms.Padding(4);
            this.btnRegular.Name = "btnRegular";
            this.btnRegular.Size = new System.Drawing.Size(157, 46);
            this.btnRegular.TabIndex = 569;
            this.btnRegular.Text = "Regular Process";
            this.btnRegular.UseVisualStyleBackColor = false;
            this.btnRegular.Click += new System.EventHandler(this.btnRegular_Click);
            // 
            // FormIssueMultiple
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(581, 413);
            this.Controls.Add(this.btnRegular);
            this.Controls.Add(this.btnPermanent);
            this.Controls.Add(this.btn6_2Process);
            this.Controls.Add(this.btn6_1Process);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.Name = "FormIssueMultiple";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = resources.GetString("$this.Text");
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
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpIssueToDate;
        private System.Windows.Forms.DateTimePicker dtpIssueFromDate;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label LRecordCount;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnStartProcess;
        private System.ComponentModel.BackgroundWorker bgwRefresh;
        private System.Windows.Forms.Button btn6_2Process;
        private System.Windows.Forms.Button btn6_1Process;
        private System.Windows.Forms.CheckBox chkProduct;
        public System.Windows.Forms.TextBox txtProName;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.TextBox txtProductCode;
        public System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.Button btnPermanent;
        private System.Windows.Forms.Button btnRegular;
    }
}