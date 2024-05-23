namespace VATClient
{
    partial class FormSaleBombaySearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grbVehicles = new System.Windows.Forms.GroupBox();
            this.cmbIsProcessed = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtpDateFrom = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.lblIsProcessed = new System.Windows.Forms.Label();
            this.txtInvoiceNo = new System.Windows.Forms.TextBox();
            this.lblInvoiceNo = new System.Windows.Forms.Label();
            this.dgvSaleBombay = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.PopUp = new System.Windows.Forms.DataGridViewButtonColumn();
            this.grbVehicles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaleBombay)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbVehicles
            // 
            this.grbVehicles.Controls.Add(this.cmbIsProcessed);
            this.grbVehicles.Controls.Add(this.label11);
            this.grbVehicles.Controls.Add(this.dtpDateTo);
            this.grbVehicles.Controls.Add(this.dtpDateFrom);
            this.grbVehicles.Controls.Add(this.label5);
            this.grbVehicles.Controls.Add(this.chkSelectAll);
            this.grbVehicles.Controls.Add(this.btnSearch);
            this.grbVehicles.Controls.Add(this.lblIsProcessed);
            this.grbVehicles.Controls.Add(this.txtInvoiceNo);
            this.grbVehicles.Controls.Add(this.lblInvoiceNo);
            this.grbVehicles.Location = new System.Drawing.Point(12, 12);
            this.grbVehicles.Name = "grbVehicles";
            this.grbVehicles.Size = new System.Drawing.Size(695, 112);
            this.grbVehicles.TabIndex = 13;
            this.grbVehicles.TabStop = false;
            this.grbVehicles.Text = "Searching Criteria";
            // 
            // cmbIsProcessed
            // 
            this.cmbIsProcessed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIsProcessed.FormattingEnabled = true;
            this.cmbIsProcessed.Items.AddRange(new object[] {
            "Y",
            "N",
            "H"});
            this.cmbIsProcessed.Location = new System.Drawing.Point(85, 54);
            this.cmbIsProcessed.Name = "cmbIsProcessed";
            this.cmbIsProcessed.Size = new System.Drawing.Size(44, 21);
            this.cmbIsProcessed.TabIndex = 241;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(391, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(16, 13);
            this.label11.TabIndex = 240;
            this.label11.Text = "to";
            this.label11.Click += new System.EventHandler(this.label11_Click);
            // 
            // dtpDateTo
            // 
            this.dtpDateTo.Checked = false;
            this.dtpDateTo.CustomFormat = "dd/MMM/yyyy";
            this.dtpDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateTo.Location = new System.Drawing.Point(413, 20);
            this.dtpDateTo.Name = "dtpDateTo";
            this.dtpDateTo.ShowCheckBox = true;
            this.dtpDateTo.Size = new System.Drawing.Size(103, 20);
            this.dtpDateTo.TabIndex = 239;
            // 
            // dtpDateFrom
            // 
            this.dtpDateFrom.Checked = false;
            this.dtpDateFrom.CustomFormat = "dd/MMM/yyyy";
            this.dtpDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateFrom.Location = new System.Drawing.Point(271, 20);
            this.dtpDateFrom.Name = "dtpDateFrom";
            this.dtpDateFrom.ShowCheckBox = true;
            this.dtpDateFrom.Size = new System.Drawing.Size(109, 20);
            this.dtpDateFrom.TabIndex = 238;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(210, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 237;
            this.label5.Text = "Start Date";
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(12, 89);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(67, 17);
            this.chkSelectAll.TabIndex = 224;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // lblIsProcessed
            // 
            this.lblIsProcessed.AutoSize = true;
            this.lblIsProcessed.Location = new System.Drawing.Point(14, 57);
            this.lblIsProcessed.Name = "lblIsProcessed";
            this.lblIsProcessed.Size = new System.Drawing.Size(65, 13);
            this.lblIsProcessed.TabIndex = 2;
            this.lblIsProcessed.Text = "IsProcessed";
            // 
            // txtInvoiceNo
            // 
            this.txtInvoiceNo.Location = new System.Drawing.Point(79, 20);
            this.txtInvoiceNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtInvoiceNo.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtInvoiceNo.Name = "txtInvoiceNo";
            this.txtInvoiceNo.Size = new System.Drawing.Size(125, 20);
            this.txtInvoiceNo.TabIndex = 0;
            // 
            // lblInvoiceNo
            // 
            this.lblInvoiceNo.AutoSize = true;
            this.lblInvoiceNo.Location = new System.Drawing.Point(14, 23);
            this.lblInvoiceNo.Name = "lblInvoiceNo";
            this.lblInvoiceNo.Size = new System.Drawing.Size(59, 13);
            this.lblInvoiceNo.TabIndex = 0;
            this.lblInvoiceNo.Text = "Invoice No";
            // 
            // dgvSaleBombay
            // 
            this.dgvSaleBombay.AllowUserToAddRows = false;
            this.dgvSaleBombay.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvSaleBombay.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSaleBombay.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSaleBombay.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.PopUp});
            this.dgvSaleBombay.Location = new System.Drawing.Point(13, 130);
            this.dgvSaleBombay.Name = "dgvSaleBombay";
            this.dgvSaleBombay.RowHeadersVisible = false;
            this.dgvSaleBombay.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSaleBombay.Size = new System.Drawing.Size(694, 236);
            this.dgvSaleBombay.TabIndex = 16;
            this.dgvSaleBombay.TabStop = false;
            this.dgvSaleBombay.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSaleBombay_CellContentClick);
            this.dgvSaleBombay.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvSaleBombay_CellPainting);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(191, 222);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.TabIndex = 209;
            this.progressBar1.Visible = false;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(393, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(13, 362);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(694, 33);
            this.panel1.TabIndex = 17;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(111, 13);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 212;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(15, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(523, 47);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // PopUp
            // 
            this.PopUp.HeaderText = "Pop Up";
            this.PopUp.Name = "PopUp";
            // 
            // FormSaleBombaySearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(719, 393);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvSaleBombay);
            this.Controls.Add(this.grbVehicles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FormSaleBombaySearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormSaleBombaySearch";
            this.Load += new System.EventHandler(this.FormSaleBombaySearch_Load);
            this.grbVehicles.ResumeLayout(false);
            this.grbVehicles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaleBombay)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbVehicles;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblIsProcessed;
        private System.Windows.Forms.TextBox txtInvoiceNo;
        private System.Windows.Forms.Label lblInvoiceNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpDateTo;
        private System.Windows.Forms.DateTimePicker dtpDateFrom;
        private System.Windows.Forms.DataGridView dgvSaleBombay;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ComboBox cmbIsProcessed;
        private System.Windows.Forms.DataGridViewButtonColumn PopUp;
    }
}