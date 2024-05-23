namespace VATClient
{
    partial class FormMultipleVAT11
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMultipleVAT11));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.numericUDPrintCopy = new System.Windows.Forms.NumericUpDown();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbPrinterName = new System.Windows.Forms.ComboBox();
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.cmbRecordCount = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSalesInvoiceNo = new System.Windows.Forms.TextBox();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.cmbTransaction = new System.Windows.Forms.ComboBox();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.label16 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.grbPrintInformation = new System.Windows.Forms.GroupBox();
            this.chk3rd = new System.Windows.Forms.CheckBox();
            this.chk2nd = new System.Windows.Forms.CheckBox();
            this.chk1st = new System.Windows.Forms.CheckBox();
            this.lblPrintPage = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPreview = new System.Windows.Forms.Button();
            this.chkIsBlank = new System.Windows.Forms.CheckBox();
            this.btnSelectedPrint = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.dgvSales = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InvoiceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InvoiceDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsPrint = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Trading = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnPrintAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUDPrintCopy)).BeginInit();
            this.grbBankInformation.SuspendLayout();
            this.grbPrintInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSales)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUDPrintCopy
            // 
            this.numericUDPrintCopy.Location = new System.Drawing.Point(336, 66);
            this.numericUDPrintCopy.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numericUDPrintCopy.Name = "numericUDPrintCopy";
            this.numericUDPrintCopy.Size = new System.Drawing.Size(101, 22);
            this.numericUDPrintCopy.TabIndex = 14;
            this.numericUDPrintCopy.Visible = false;
            this.numericUDPrintCopy.ValueChanged += new System.EventHandler(this.numericUDPrintCopy_ValueChanged);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.Image = global::VATClient.Properties.Resources.Print;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(557, 489);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(83, 34);
            this.btnOk.TabIndex = 52;
            this.btnOk.Text = "&Print";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Visible = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(529, 7);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(83, 34);
            this.btnCancel.TabIndex = 52;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmbPrinterName
            // 
            this.cmbPrinterName.FormattingEnabled = true;
            this.cmbPrinterName.Location = new System.Drawing.Point(152, 444);
            this.cmbPrinterName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbPrinterName.Name = "cmbPrinterName";
            this.cmbPrinterName.Size = new System.Drawing.Size(276, 24);
            this.cmbPrinterName.TabIndex = 53;
            this.cmbPrinterName.Leave += new System.EventHandler(this.cmbPrinterName_Leave);
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.cmbRecordCount);
            this.grbBankInformation.Controls.Add(this.label18);
            this.grbBankInformation.Controls.Add(this.label6);
            this.grbBankInformation.Controls.Add(this.txtSalesInvoiceNo);
            this.grbBankInformation.Controls.Add(this.cmbBranch);
            this.grbBankInformation.Controls.Add(this.label2);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.label3);
            this.grbBankInformation.Controls.Add(this.dtpToDate);
            this.grbBankInformation.Controls.Add(this.cmbTransaction);
            this.grbBankInformation.Controls.Add(this.dtpFromDate);
            this.grbBankInformation.Controls.Add(this.label16);
            this.grbBankInformation.Controls.Add(this.label4);
            this.grbBankInformation.Location = new System.Drawing.Point(29, 14);
            this.grbBankInformation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grbBankInformation.Size = new System.Drawing.Size(611, 122);
            this.grbBankInformation.TabIndex = 83;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // cmbRecordCount
            // 
            this.cmbRecordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordCount.FormattingEnabled = true;
            this.cmbRecordCount.Location = new System.Drawing.Point(409, 58);
            this.cmbRecordCount.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbRecordCount.Name = "cmbRecordCount";
            this.cmbRecordCount.Size = new System.Drawing.Size(105, 24);
            this.cmbRecordCount.TabIndex = 240;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(337, 65);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(33, 17);
            this.label18.TabIndex = 239;
            this.label18.Text = "Top";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(337, 28);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 17);
            this.label6.TabIndex = 217;
            this.label6.Text = "Branch";
            // 
            // txtSalesInvoiceNo
            // 
            this.txtSalesInvoiceNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtSalesInvoiceNo.Location = new System.Drawing.Point(123, 23);
            this.txtSalesInvoiceNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtSalesInvoiceNo.MaximumSize = new System.Drawing.Size(4, 25);
            this.txtSalesInvoiceNo.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtSalesInvoiceNo.Name = "txtSalesInvoiceNo";
            this.txtSalesInvoiceNo.Size = new System.Drawing.Size(199, 22);
            this.txtSalesInvoiceNo.TabIndex = 87;
            this.txtSalesInvoiceNo.TabStop = false;
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Cash Payable",
            "Credit Payable",
            "Rebatable"});
            this.cmbBranch.Location = new System.Drawing.Point(412, 23);
            this.cmbBranch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(189, 24);
            this.cmbBranch.Sorted = true;
            this.cmbBranch.TabIndex = 216;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 28);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 17);
            this.label2.TabIndex = 88;
            this.label2.Text = "Invoice No";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(503, 84);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 34);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(264, 94);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 17);
            this.label3.TabIndex = 45;
            this.label3.Text = "To";
            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(300, 89);
            this.dtpToDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(136, 22);
            this.dtpToDate.TabIndex = 43;
            // 
            // cmbTransaction
            // 
            this.cmbTransaction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransaction.FormattingEnabled = true;
            this.cmbTransaction.Location = new System.Drawing.Point(123, 55);
            this.cmbTransaction.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbTransaction.Name = "cmbTransaction";
            this.cmbTransaction.Size = new System.Drawing.Size(199, 24);
            this.cmbTransaction.TabIndex = 53;
            this.cmbTransaction.SelectedIndexChanged += new System.EventHandler(this.cmbTransaction_SelectedIndexChanged);
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(123, 89);
            this.dtpFromDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(136, 22);
            this.dtpFromDate.TabIndex = 42;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(15, 92);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(89, 17);
            this.label16.TabIndex = 39;
            this.label16.Text = "Challan Date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 17);
            this.label4.TabIndex = 34;
            this.label4.Text = "Sale Types";
            // 
            // grbPrintInformation
            // 
            this.grbPrintInformation.Controls.Add(this.chk3rd);
            this.grbPrintInformation.Controls.Add(this.chk2nd);
            this.grbPrintInformation.Controls.Add(this.chk1st);
            this.grbPrintInformation.Controls.Add(this.lblPrintPage);
            this.grbPrintInformation.Controls.Add(this.label1);
            this.grbPrintInformation.Controls.Add(this.numericUDPrintCopy);
            this.grbPrintInformation.Location = new System.Drawing.Point(680, 385);
            this.grbPrintInformation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grbPrintInformation.Name = "grbPrintInformation";
            this.grbPrintInformation.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grbPrintInformation.Size = new System.Drawing.Size(611, 132);
            this.grbPrintInformation.TabIndex = 84;
            this.grbPrintInformation.TabStop = false;
            this.grbPrintInformation.Text = "Print Information";
            // 
            // chk3rd
            // 
            this.chk3rd.AllowDrop = true;
            this.chk3rd.AutoSize = true;
            this.chk3rd.Location = new System.Drawing.Point(383, 100);
            this.chk3rd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chk3rd.Name = "chk3rd";
            this.chk3rd.Size = new System.Drawing.Size(51, 21);
            this.chk3rd.TabIndex = 56;
            this.chk3rd.Text = "3rd";
            this.chk3rd.UseVisualStyleBackColor = true;
            this.chk3rd.Visible = false;
            this.chk3rd.CheckedChanged += new System.EventHandler(this.chk3rd_CheckedChanged);
            // 
            // chk2nd
            // 
            this.chk2nd.AllowDrop = true;
            this.chk2nd.AutoSize = true;
            this.chk2nd.Location = new System.Drawing.Point(316, 100);
            this.chk2nd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chk2nd.Name = "chk2nd";
            this.chk2nd.Size = new System.Drawing.Size(54, 21);
            this.chk2nd.TabIndex = 56;
            this.chk2nd.Text = "2nd";
            this.chk2nd.UseVisualStyleBackColor = true;
            this.chk2nd.Visible = false;
            this.chk2nd.CheckedChanged += new System.EventHandler(this.chk2nd_CheckedChanged);
            // 
            // chk1st
            // 
            this.chk1st.AllowDrop = true;
            this.chk1st.AutoSize = true;
            this.chk1st.Location = new System.Drawing.Point(251, 100);
            this.chk1st.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chk1st.Name = "chk1st";
            this.chk1st.Size = new System.Drawing.Size(49, 21);
            this.chk1st.TabIndex = 56;
            this.chk1st.Text = "1st";
            this.chk1st.UseVisualStyleBackColor = true;
            this.chk1st.Visible = false;
            this.chk1st.CheckedChanged += new System.EventHandler(this.chk1st_CheckedChanged);
            // 
            // lblPrintPage
            // 
            this.lblPrintPage.AutoSize = true;
            this.lblPrintPage.Location = new System.Drawing.Point(15, 100);
            this.lblPrintPage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPrintPage.Name = "lblPrintPage";
            this.lblPrintPage.Size = new System.Drawing.Size(223, 17);
            this.lblPrintPage.TabIndex = 54;
            this.lblPrintPage.Text = "Which page do you want  to print?";
            this.lblPrintPage.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 69);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(238, 17);
            this.label1.TabIndex = 54;
            this.label1.Text = "How many copies you want  to print?";
            this.label1.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(53, 448);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 17);
            this.label5.TabIndex = 55;
            this.label5.Text = "Printer Name";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(105, 489);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(400, 28);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 85;
            this.progressBar1.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.btnPrintAll);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Controls.Add(this.chkIsBlank);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSelectedPrint);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Location = new System.Drawing.Point(13, 524);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(627, 49);
            this.panel1.TabIndex = 86;
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(185, 6);
            this.btnPreview.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(99, 34);
            this.btnPreview.TabIndex = 54;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // chkIsBlank
            // 
            this.chkIsBlank.AutoSize = true;
            this.chkIsBlank.Location = new System.Drawing.Point(107, 16);
            this.chkIsBlank.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkIsBlank.Name = "chkIsBlank";
            this.chkIsBlank.Size = new System.Drawing.Size(65, 21);
            this.chkIsBlank.TabIndex = 53;
            this.chkIsBlank.Text = "Blank";
            this.chkIsBlank.UseVisualStyleBackColor = true;
            // 
            // btnSelectedPrint
            // 
            this.btnSelectedPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSelectedPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnSelectedPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelectedPrint.Location = new System.Drawing.Point(4, 6);
            this.btnSelectedPrint.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSelectedPrint.Name = "btnSelectedPrint";
            this.btnSelectedPrint.Size = new System.Drawing.Size(99, 34);
            this.btnSelectedPrint.TabIndex = 52;
            this.btnSelectedPrint.Text = "Print";
            this.btnSelectedPrint.UseVisualStyleBackColor = false;
            this.btnSelectedPrint.Click += new System.EventHandler(this.btnSelectedPrint_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(425, 7);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 34);
            this.btnRefresh.TabIndex = 7;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSelectAll);
            this.groupBox1.Controls.Add(this.dgvSales);
            this.groupBox1.Location = new System.Drawing.Point(29, 143);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(611, 294);
            this.groupBox1.TabIndex = 54;
            this.groupBox1.TabStop = false;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(15, 0);
            this.chkSelectAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(84, 21);
            this.chkSelectAll.TabIndex = 214;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // dgvSales
            // 
            this.dgvSales.AllowUserToAddRows = false;
            this.dgvSales.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            this.dgvSales.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSales.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.ID,
            this.InvoiceNo,
            this.InvoiceDate,
            this.CustomerName,
            this.IsPrint,
            this.Trading});
            this.dgvSales.Location = new System.Drawing.Point(8, 23);
            this.dgvSales.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvSales.Name = "dgvSales";
            this.dgvSales.RowHeadersVisible = false;
            this.dgvSales.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSales.Size = new System.Drawing.Size(595, 263);
            this.dgvSales.TabIndex = 7;
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            this.Select.Width = 50;
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Visible = false;
            // 
            // InvoiceNo
            // 
            this.InvoiceNo.HeaderText = "Invoice No";
            this.InvoiceNo.Name = "InvoiceNo";
            this.InvoiceNo.ReadOnly = true;
            // 
            // InvoiceDate
            // 
            this.InvoiceDate.DataPropertyName = "InvoiceDate";
            this.InvoiceDate.HeaderText = "Sales Date";
            this.InvoiceDate.Name = "InvoiceDate";
            this.InvoiceDate.ReadOnly = true;
            // 
            // CustomerName
            // 
            this.CustomerName.DataPropertyName = "CustomerName";
            this.CustomerName.HeaderText = "CustomerName";
            this.CustomerName.Name = "CustomerName";
            this.CustomerName.ReadOnly = true;
            this.CustomerName.Width = 200;
            // 
            // IsPrint
            // 
            this.IsPrint.HeaderText = "Print";
            this.IsPrint.Name = "IsPrint";
            this.IsPrint.ReadOnly = true;
            // 
            // Trading
            // 
            this.Trading.HeaderText = "Trading";
            this.Trading.Name = "Trading";
            this.Trading.ReadOnly = true;
            this.Trading.Visible = false;
            // 
            // btnPrintAll
            // 
            this.btnPrintAll.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrintAll.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrintAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrintAll.Location = new System.Drawing.Point(295, 7);
            this.btnPrintAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrintAll.Name = "btnPrintAll";
            this.btnPrintAll.Size = new System.Drawing.Size(99, 34);
            this.btnPrintAll.TabIndex = 55;
            this.btnPrintAll.Text = "Print All";
            this.btnPrintAll.UseVisualStyleBackColor = false;
            this.btnPrintAll.Click += new System.EventHandler(this.btnPrintAll_Click);
            // 
            // FormMultipleVAT11
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(641, 580);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbPrintInformation);
            this.Controls.Add(this.cmbPrinterName);
            this.Controls.Add(this.grbBankInformation);
            this.Location = new System.Drawing.Point(100, 100);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1594, 789);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(527, 481);
            this.Name = "FormMultipleVAT11";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report (VAT 6.3) Sales Challan";
            this.Load += new System.EventHandler(this.FormMultipleVAT11_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUDPrintCopy)).EndInit();
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.grbPrintInformation.ResumeLayout(false);
            this.grbPrintInformation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSales)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.NumericUpDown numericUDPrintCopy;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbPrinterName;
        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.DateTimePicker dtpToDate;
        public System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox grbPrintInformation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cmbTransaction;
        private System.Windows.Forms.CheckBox chk3rd;
        private System.Windows.Forms.CheckBox chk2nd;
        private System.Windows.Forms.CheckBox chk1st;
        private System.Windows.Forms.Label lblPrintPage;
        private System.Windows.Forms.CheckBox chkIsBlank;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgvSales;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Button btnSelectedPrint;
        private System.Windows.Forms.TextBox txtSalesInvoiceNo;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvoiceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvoiceDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsPrint;
        private System.Windows.Forms.DataGridViewTextBoxColumn Trading;
        private System.Windows.Forms.Label label18;
        public System.Windows.Forms.ComboBox cmbRecordCount;
        private System.Windows.Forms.Button btnPrintAll;
    }
}