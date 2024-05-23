namespace VATClient.ReportPreview
{
    partial class FormRptVAT17
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnProductCode = new System.Windows.Forms.RadioButton();
            this.rbtnProductName = new System.Windows.Forms.RadioButton();
            this.chkMultiple = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtProName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.rbtnProductGroup = new System.Windows.Forms.RadioButton();
            this.txtProductCode = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtUOM = new System.Windows.Forms.TextBox();
            this.txtConvertion = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rbtnProductType = new System.Windows.Forms.RadioButton();
            this.cmbUOM = new System.Windows.Forms.ComboBox();
            this.btnSearchProductGroup = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPGroup = new System.Windows.Forms.TextBox();
            this.cmbProductType = new System.Windows.Forms.ComboBox();
            this.pnlHidden = new System.Windows.Forms.Panel();
            this.txtPGroupId = new System.Windows.Forms.TextBox();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.rbtnService = new System.Windows.Forms.RadioButton();
            this.rbtnWIP = new System.Windows.Forms.RadioButton();
            this.btnSearchProduct = new System.Windows.Forms.Button();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chkBranch = new System.Windows.Forms.CheckBox();
            this.chkTopSheet = new System.Windows.Forms.CheckBox();
            this.rbtnMonth = new System.Windows.Forms.RadioButton();
            this.dtpMonth = new System.Windows.Forms.DateTimePicker();
            this.rbtnDate = new System.Windows.Forms.RadioButton();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvProduct = new System.Windows.Forms.DataGridView();
            this.chbInEnglish = new System.Windows.Forms.CheckBox();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbDecimalValue = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbDecimal = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.grbBankInformation.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnlHidden.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProduct)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.groupBox1);
            this.grbBankInformation.Controls.Add(this.pnlHidden);
            this.grbBankInformation.Controls.Add(this.progressBar1);
            this.grbBankInformation.Controls.Add(this.panel2);
            this.grbBankInformation.Controls.Add(this.dgvProduct);
            this.grbBankInformation.Location = new System.Drawing.Point(1, 0);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(677, 360);
            this.grbBankInformation.TabIndex = 82;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtnProductCode);
            this.groupBox1.Controls.Add(this.rbtnProductName);
            this.groupBox1.Controls.Add(this.chkMultiple);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtProName);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.rbtnProductGroup);
            this.groupBox1.Controls.Add(this.txtProductCode);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmbFontSize);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtUOM);
            this.groupBox1.Controls.Add(this.txtConvertion);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.rbtnProductType);
            this.groupBox1.Controls.Add(this.cmbUOM);
            this.groupBox1.Controls.Add(this.btnSearchProductGroup);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtPGroup);
            this.groupBox1.Controls.Add(this.cmbProductType);
            this.groupBox1.Location = new System.Drawing.Point(5, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(670, 115);
            this.groupBox1.TabIndex = 549;
            this.groupBox1.TabStop = false;
            // 
            // rbtnProductCode
            // 
            this.rbtnProductCode.AutoSize = true;
            this.rbtnProductCode.Location = new System.Drawing.Point(306, 92);
            this.rbtnProductCode.Name = "rbtnProductCode";
            this.rbtnProductCode.Size = new System.Drawing.Size(14, 13);
            this.rbtnProductCode.TabIndex = 550;
            this.rbtnProductCode.UseVisualStyleBackColor = true;
            this.rbtnProductCode.CheckedChanged += new System.EventHandler(this.rbtnProductCode_CheckedChanged);
            // 
            // rbtnProductName
            // 
            this.rbtnProductName.AutoSize = true;
            this.rbtnProductName.Checked = true;
            this.rbtnProductName.Location = new System.Drawing.Point(306, 68);
            this.rbtnProductName.Name = "rbtnProductName";
            this.rbtnProductName.Size = new System.Drawing.Size(14, 13);
            this.rbtnProductName.TabIndex = 549;
            this.rbtnProductName.TabStop = true;
            this.rbtnProductName.UseVisualStyleBackColor = true;
            this.rbtnProductName.CheckedChanged += new System.EventHandler(this.rbtnProductName_CheckedChanged);
            // 
            // chkMultiple
            // 
            this.chkMultiple.AutoSize = true;
            this.chkMultiple.Location = new System.Drawing.Point(419, 64);
            this.chkMultiple.Name = "chkMultiple";
            this.chkMultiple.Size = new System.Drawing.Size(55, 17);
            this.chkMultiple.TabIndex = 538;
            this.chkMultiple.Text = "Single";
            this.chkMultiple.UseVisualStyleBackColor = true;
            this.chkMultiple.Click += new System.EventHandler(this.chkMultiple_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 13);
            this.label8.TabIndex = 540;
            this.label8.Text = "Product Type";
            // 
            // txtProName
            // 
            this.txtProName.Location = new System.Drawing.Point(106, 64);
            this.txtProName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtProName.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtProName.Name = "txtProName";
            this.txtProName.Size = new System.Drawing.Size(178, 20);
            this.txtProName.TabIndex = 548;
            this.txtProName.TextChanged += new System.EventHandler(this.txtProName_TextChanged);
            this.txtProName.DoubleClick += new System.EventHandler(this.txtProName_DoubleClick);
            this.txtProName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProName_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(509, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 13);
            this.label6.TabIndex = 537;
            this.label6.Text = "Font";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 68);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(93, 13);
            this.label10.TabIndex = 547;
            this.label10.Text = "Product Name(F9)";
            // 
            // rbtnProductGroup
            // 
            this.rbtnProductGroup.AutoSize = true;
            this.rbtnProductGroup.Location = new System.Drawing.Point(306, 42);
            this.rbtnProductGroup.Name = "rbtnProductGroup";
            this.rbtnProductGroup.Size = new System.Drawing.Size(14, 13);
            this.rbtnProductGroup.TabIndex = 537;
            this.rbtnProductGroup.UseVisualStyleBackColor = true;
            this.rbtnProductGroup.CheckedChanged += new System.EventHandler(this.rbtnProductGroup_CheckedChanged);
            // 
            // txtProductCode
            // 
            this.txtProductCode.Enabled = false;
            this.txtProductCode.Location = new System.Drawing.Point(106, 88);
            this.txtProductCode.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtProductCode.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.Size = new System.Drawing.Size(178, 20);
            this.txtProductCode.TabIndex = 546;
            this.txtProductCode.TextChanged += new System.EventHandler(this.txtProductCode_TextChanged);
            this.txtProductCode.DoubleClick += new System.EventHandler(this.txtProductCode_DoubleClick);
            this.txtProductCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProductCode_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(493, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 532;
            this.label5.Text = "Pkt Size";
            // 
            // cmbFontSize
            // 
            this.cmbFontSize.FormattingEnabled = true;
            this.cmbFontSize.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.cmbFontSize.Location = new System.Drawing.Point(543, 68);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 85;
            this.cmbFontSize.Text = "8";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 92);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 13);
            this.label9.TabIndex = 545;
            this.label9.Text = "Product Code(F9)";
            // 
            // txtUOM
            // 
            this.txtUOM.Location = new System.Drawing.Point(418, 13);
            this.txtUOM.MaximumSize = new System.Drawing.Size(250, 20);
            this.txtUOM.Multiline = true;
            this.txtUOM.Name = "txtUOM";
            this.txtUOM.ReadOnly = true;
            this.txtUOM.Size = new System.Drawing.Size(68, 20);
            this.txtUOM.TabIndex = 531;
            // 
            // txtConvertion
            // 
            this.txtConvertion.Location = new System.Drawing.Point(623, 14);
            this.txtConvertion.MaximumSize = new System.Drawing.Size(250, 20);
            this.txtConvertion.Multiline = true;
            this.txtConvertion.Name = "txtConvertion";
            this.txtConvertion.ReadOnly = true;
            this.txtConvertion.Size = new System.Drawing.Size(28, 20);
            this.txtConvertion.TabIndex = 530;
            this.txtConvertion.Text = "0";
            this.txtConvertion.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(377, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 529;
            this.label4.Text = "UOM";
            // 
            // rbtnProductType
            // 
            this.rbtnProductType.AutoSize = true;
            this.rbtnProductType.Location = new System.Drawing.Point(306, 17);
            this.rbtnProductType.Name = "rbtnProductType";
            this.rbtnProductType.Size = new System.Drawing.Size(14, 13);
            this.rbtnProductType.TabIndex = 536;
            this.rbtnProductType.UseVisualStyleBackColor = true;
            this.rbtnProductType.CheckedChanged += new System.EventHandler(this.rbtnProductType_CheckedChanged);
            // 
            // cmbUOM
            // 
            this.cmbUOM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUOM.FormattingEnabled = true;
            this.cmbUOM.Location = new System.Drawing.Point(543, 13);
            this.cmbUOM.Name = "cmbUOM";
            this.cmbUOM.Size = new System.Drawing.Size(68, 21);
            this.cmbUOM.Sorted = true;
            this.cmbUOM.TabIndex = 528;
            this.cmbUOM.SelectedIndexChanged += new System.EventHandler(this.cmbUOM_SelectedIndexChanged);
            this.cmbUOM.Leave += new System.EventHandler(this.cmbUOM_Leave);
            // 
            // btnSearchProductGroup
            // 
            this.btnSearchProductGroup.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchProductGroup.Enabled = false;
            this.btnSearchProductGroup.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchProductGroup.Location = new System.Drawing.Point(342, 38);
            this.btnSearchProductGroup.Name = "btnSearchProductGroup";
            this.btnSearchProductGroup.Size = new System.Drawing.Size(30, 20);
            this.btnSearchProductGroup.TabIndex = 538;
            this.btnSearchProductGroup.UseVisualStyleBackColor = false;
            this.btnSearchProductGroup.Visible = false;
            this.btnSearchProductGroup.Click += new System.EventHandler(this.btnSearchProductGroup_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 13);
            this.label7.TabIndex = 537;
            this.label7.Text = "Product Group(F9)";
            // 
            // txtPGroup
            // 
            this.txtPGroup.Enabled = false;
            this.txtPGroup.Location = new System.Drawing.Point(106, 38);
            this.txtPGroup.MaximumSize = new System.Drawing.Size(280, 20);
            this.txtPGroup.MinimumSize = new System.Drawing.Size(100, 20);
            this.txtPGroup.Name = "txtPGroup";
            this.txtPGroup.ReadOnly = true;
            this.txtPGroup.Size = new System.Drawing.Size(178, 20);
            this.txtPGroup.TabIndex = 536;
            this.txtPGroup.DoubleClick += new System.EventHandler(this.txtPGroup_DoubleClick);
            this.txtPGroup.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPGroup_KeyDown);
            // 
            // cmbProductType
            // 
            this.cmbProductType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProductType.Enabled = false;
            this.cmbProductType.FormattingEnabled = true;
            this.cmbProductType.Location = new System.Drawing.Point(106, 13);
            this.cmbProductType.Name = "cmbProductType";
            this.cmbProductType.Size = new System.Drawing.Size(178, 21);
            this.cmbProductType.TabIndex = 541;
            this.cmbProductType.SelectedIndexChanged += new System.EventHandler(this.cmbProductType_SelectedIndexChanged);
            this.cmbProductType.TextChanged += new System.EventHandler(this.cmbProductType_TextChanged);
            this.cmbProductType.Leave += new System.EventHandler(this.cmbProductType_Leave);
            // 
            // pnlHidden
            // 
            this.pnlHidden.Controls.Add(this.txtPGroupId);
            this.pnlHidden.Controls.Add(this.txtItemNo);
            this.pnlHidden.Controls.Add(this.rbtnService);
            this.pnlHidden.Controls.Add(this.rbtnWIP);
            this.pnlHidden.Controls.Add(this.btnSearchProduct);
            this.pnlHidden.Controls.Add(this.txtProductName);
            this.pnlHidden.Location = new System.Drawing.Point(7, 310);
            this.pnlHidden.Name = "pnlHidden";
            this.pnlHidden.Size = new System.Drawing.Size(370, 30);
            this.pnlHidden.TabIndex = 542;
            this.pnlHidden.Visible = false;
            // 
            // txtPGroupId
            // 
            this.txtPGroupId.Location = new System.Drawing.Point(143, 3);
            this.txtPGroupId.Name = "txtPGroupId";
            this.txtPGroupId.Size = new System.Drawing.Size(40, 20);
            this.txtPGroupId.TabIndex = 543;
            this.txtPGroupId.Visible = false;
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(7, 3);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.Size = new System.Drawing.Size(40, 20);
            this.txtItemNo.TabIndex = 44;
            this.txtItemNo.Visible = false;
            // 
            // rbtnService
            // 
            this.rbtnService.AutoSize = true;
            this.rbtnService.Location = new System.Drawing.Point(31, 6);
            this.rbtnService.Name = "rbtnService";
            this.rbtnService.Size = new System.Drawing.Size(61, 17);
            this.rbtnService.TabIndex = 83;
            this.rbtnService.TabStop = true;
            this.rbtnService.Text = "Service";
            this.rbtnService.UseVisualStyleBackColor = true;
            this.rbtnService.Visible = false;
            // 
            // rbtnWIP
            // 
            this.rbtnWIP.AutoSize = true;
            this.rbtnWIP.Location = new System.Drawing.Point(98, 6);
            this.rbtnWIP.Name = "rbtnWIP";
            this.rbtnWIP.Size = new System.Drawing.Size(46, 17);
            this.rbtnWIP.TabIndex = 83;
            this.rbtnWIP.TabStop = true;
            this.rbtnWIP.Text = "WIP";
            this.rbtnWIP.UseVisualStyleBackColor = true;
            this.rbtnWIP.Visible = false;
            // 
            // btnSearchProduct
            // 
            this.btnSearchProduct.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchProduct.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchProduct.Location = new System.Drawing.Point(189, 4);
            this.btnSearchProduct.Name = "btnSearchProduct";
            this.btnSearchProduct.Size = new System.Drawing.Size(30, 20);
            this.btnSearchProduct.TabIndex = 40;
            this.btnSearchProduct.UseVisualStyleBackColor = false;
            this.btnSearchProduct.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(225, 3);
            this.txtProductName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(40, 20);
            this.txtProductName.TabIndex = 35;
            this.txtProductName.Visible = false;
            this.txtProductName.TextChanged += new System.EventHandler(this.txtProductName_TextChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(205, 247);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(284, 21);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 84;
            this.progressBar1.Visible = false;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.chkBranch);
            this.panel2.Controls.Add(this.chkTopSheet);
            this.panel2.Controls.Add(this.rbtnMonth);
            this.panel2.Controls.Add(this.dtpMonth);
            this.panel2.Controls.Add(this.rbtnDate);
            this.panel2.Controls.Add(this.dtpFromDate);
            this.panel2.Controls.Add(this.dtpToDate);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.cmbBranch);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(5, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(671, 55);
            this.panel2.TabIndex = 536;
            // 
            // chkBranch
            // 
            this.chkBranch.AutoSize = true;
            this.chkBranch.Location = new System.Drawing.Point(263, 31);
            this.chkBranch.Name = "chkBranch";
            this.chkBranch.Size = new System.Drawing.Size(87, 17);
            this.chkBranch.TabIndex = 540;
            this.chkBranch.Text = "Branch Wise";
            this.chkBranch.UseVisualStyleBackColor = true;
            // 
            // chkTopSheet
            // 
            this.chkTopSheet.AutoSize = true;
            this.chkTopSheet.Location = new System.Drawing.Point(352, 31);
            this.chkTopSheet.Name = "chkTopSheet";
            this.chkTopSheet.Size = new System.Drawing.Size(73, 17);
            this.chkTopSheet.TabIndex = 539;
            this.chkTopSheet.Text = "TopSheet";
            this.chkTopSheet.UseVisualStyleBackColor = true;
            this.chkTopSheet.Visible = false;
            // 
            // rbtnMonth
            // 
            this.rbtnMonth.AutoSize = true;
            this.rbtnMonth.Location = new System.Drawing.Point(5, 33);
            this.rbtnMonth.Name = "rbtnMonth";
            this.rbtnMonth.Size = new System.Drawing.Size(55, 17);
            this.rbtnMonth.TabIndex = 534;
            this.rbtnMonth.Text = "Month";
            this.rbtnMonth.UseVisualStyleBackColor = true;
            this.rbtnMonth.CheckedChanged += new System.EventHandler(this.rbtnMonth_CheckedChanged);
            // 
            // dtpMonth
            // 
            this.dtpMonth.Checked = false;
            this.dtpMonth.CustomFormat = "MMMM-yyyy";
            this.dtpMonth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpMonth.Location = new System.Drawing.Point(103, 33);
            this.dtpMonth.Name = "dtpMonth";
            this.dtpMonth.Size = new System.Drawing.Size(129, 20);
            this.dtpMonth.TabIndex = 535;
            this.dtpMonth.Visible = false;
            this.dtpMonth.ValueChanged += new System.EventHandler(this.dtpMonth_ValueChanged);
            this.dtpMonth.Leave += new System.EventHandler(this.dtpMonth_Leave);
            // 
            // rbtnDate
            // 
            this.rbtnDate.AutoSize = true;
            this.rbtnDate.Checked = true;
            this.rbtnDate.Location = new System.Drawing.Point(5, 8);
            this.rbtnDate.Name = "rbtnDate";
            this.rbtnDate.Size = new System.Drawing.Size(48, 17);
            this.rbtnDate.TabIndex = 533;
            this.rbtnDate.TabStop = true;
            this.rbtnDate.Text = "Date";
            this.rbtnDate.UseVisualStyleBackColor = true;
            this.rbtnDate.CheckedChanged += new System.EventHandler(this.rbtnDate_CheckedChanged);
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(103, 8);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.ShowCheckBox = true;
            this.dtpFromDate.Size = new System.Drawing.Size(129, 20);
            this.dtpFromDate.TabIndex = 42;
            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(263, 8);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.ShowCheckBox = true;
            this.dtpToDate.Size = new System.Drawing.Size(129, 20);
            this.dtpToDate.TabIndex = 43;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(236, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 45;
            this.label1.Text = "To";
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Cash Payable",
            "Credit Payable",
            "Rebatable"});
            this.cmbBranch.Location = new System.Drawing.Point(467, 8);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(136, 21);
            this.cmbBranch.Sorted = true;
            this.cmbBranch.TabIndex = 216;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(416, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 217;
            this.label3.Text = "Branch";
            // 
            // dgvProduct
            // 
            this.dgvProduct.AllowUserToAddRows = false;
            this.dgvProduct.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvProduct.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvProduct.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProduct.Location = new System.Drawing.Point(5, 185);
            this.dgvProduct.Name = "dgvProduct";
            this.dgvProduct.RowHeadersVisible = false;
            this.dgvProduct.RowTemplate.Height = 24;
            this.dgvProduct.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProduct.Size = new System.Drawing.Size(671, 170);
            this.dgvProduct.TabIndex = 544;
            this.dgvProduct.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProduct_CellContentClick);
            // 
            // chbInEnglish
            // 
            this.chbInEnglish.AutoSize = true;
            this.chbInEnglish.Location = new System.Drawing.Point(290, 11);
            this.chbInEnglish.Name = "chbInEnglish";
            this.chbInEnglish.Size = new System.Drawing.Size(59, 17);
            this.chbInEnglish.TabIndex = 527;
            this.chbInEnglish.Text = "Bangla";
            this.chbInEnglish.UseVisualStyleBackColor = true;
            this.chbInEnglish.CheckedChanged += new System.EventHandler(this.chbInEnglish_CheckedChanged);
            this.chbInEnglish.Click += new System.EventHandler(this.chbInEnglish_Click);
            // 
            // backgroundWorkerPreview
            // 
            this.backgroundWorkerPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreview_DoWork);
            this.backgroundWorkerPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreview_RunWorkerCompleted);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.cmbDecimalValue);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cmbDecimal);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnPrev);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.chbInEnglish);
            this.panel1.Location = new System.Drawing.Point(3, 364);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(675, 42);
            this.panel1.TabIndex = 81;
            // 
            // cmbDecimalValue
            // 
            this.cmbDecimalValue.FormattingEnabled = true;
            this.cmbDecimalValue.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.cmbDecimalValue.Location = new System.Drawing.Point(228, 18);
            this.cmbDecimalValue.Name = "cmbDecimalValue";
            this.cmbDecimalValue.Size = new System.Drawing.Size(36, 21);
            this.cmbDecimalValue.TabIndex = 567;
            this.cmbDecimalValue.Text = "2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(225, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 566;
            this.label2.Text = "Value";
            // 
            // cmbDecimal
            // 
            this.cmbDecimal.FormattingEnabled = true;
            this.cmbDecimal.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.cmbDecimal.Location = new System.Drawing.Point(163, 18);
            this.cmbDecimal.Name = "cmbDecimal";
            this.cmbDecimal.Size = new System.Drawing.Size(36, 21);
            this.cmbDecimal.TabIndex = 565;
            this.cmbDecimal.Text = "2";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(160, 2);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(46, 13);
            this.label13.TabIndex = 564;
            this.label13.Text = "Quantity";
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(9, 12);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 222;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrev.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrev.Location = new System.Drawing.Point(355, 6);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(72, 28);
            this.btnPrev.TabIndex = 43;
            this.btnPrev.Text = "Preview";
            this.btnPrev.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(597, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(516, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(435, 6);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 28);
            this.btnPrint.TabIndex = 41;
            this.btnPrint.Text = "VAT 6.2";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormRptVAT17
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(678, 411);
            this.Controls.Add(this.grbBankInformation);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(700, 595);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 211);
            this.Name = "FormRptVAT17";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report (VAT 6.2) Sales Register";
            this.Load += new System.EventHandler(this.FormRptVAT17_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlHidden.ResumeLayout(false);
            this.pnlHidden.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProduct)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbBankInformation;
        public System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnSearchProduct;
        public System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.DateTimePicker dtpToDate;
        public System.Windows.Forms.DateTimePicker dtpFromDate;
        public System.Windows.Forms.RadioButton rbtnService;
        private System.Windows.Forms.Button btnPrev;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        private System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.RadioButton rbtnWIP;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.ComboBox cmbFontSize;
        private System.Windows.Forms.CheckBox chbInEnglish;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.ComboBox cmbUOM;
        private System.Windows.Forms.TextBox txtConvertion;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox txtUOM;
        private System.Windows.Forms.RadioButton rbtnMonth;
        private System.Windows.Forms.RadioButton rbtnDate;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DateTimePicker dtpMonth;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbProductType;
        private System.Windows.Forms.TextBox txtPGroup;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnSearchProductGroup;
        private System.Windows.Forms.Panel pnlHidden;
        private System.Windows.Forms.TextBox txtPGroupId;
        private System.Windows.Forms.DataGridView dgvProduct;
        public System.Windows.Forms.TextBox txtProductCode;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox txtProName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.RadioButton rbtnProductGroup;
        private System.Windows.Forms.RadioButton rbtnProductType;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.RadioButton rbtnProductCode;
        private System.Windows.Forms.RadioButton rbtnProductName;
        private System.Windows.Forms.CheckBox chkMultiple;
        private System.Windows.Forms.CheckBox chkTopSheet;
        private System.Windows.Forms.CheckBox chkBranch;
        private System.Windows.Forms.ComboBox cmbDecimal;
        public System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmbDecimalValue;
        public System.Windows.Forms.Label label2;
    }
}