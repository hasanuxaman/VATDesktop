namespace VATClient.ReportPreview
{
    partial class FormRptVAT16
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
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.rbtnMonth = new System.Windows.Forms.RadioButton();
            this.dtpMonth = new System.Windows.Forms.DateTimePicker();
            this.rbtnDate = new System.Windows.Forms.RadioButton();
            this.labelBranch = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chkBranch = new System.Windows.Forms.CheckBox();
            this.chkGroupTopSheet = new System.Windows.Forms.CheckBox();
            this.chkTopSheet = new System.Windows.Forms.CheckBox();
            this.cmbBranchName = new System.Windows.Forms.ComboBox();
            this.dgvProduct = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnProductCode = new System.Windows.Forms.RadioButton();
            this.rbtnProductName = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.chkMultiple = new System.Windows.Forms.CheckBox();
            this.txtConvertion = new System.Windows.Forms.TextBox();
            this.cmbProductType = new System.Windows.Forms.ComboBox();
            this.txtPGroup = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtUOM = new System.Windows.Forms.TextBox();
            this.btnSearchProductGroup = new System.Windows.Forms.Button();
            this.rbtnProductType = new System.Windows.Forms.RadioButton();
            this.cmbUOM = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtProductCode = new System.Windows.Forms.TextBox();
            this.txtProName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.rbtnProductGroup = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.chbInEnglish = new System.Windows.Forms.CheckBox();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.btnVAT6_1 = new System.Windows.Forms.Button();
            this.txtTransType = new System.Windows.Forms.TextBox();
            this.rbtnTollRegisterRaw = new System.Windows.Forms.RadioButton();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbDecimal = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.rbtnTollRegisterFinish = new System.Windows.Forms.RadioButton();
            this.pnlHidden = new System.Windows.Forms.Panel();
            this.txtPGroupId = new System.Windows.Forms.TextBox();
            this.cmbValue = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grbBankInformation.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProduct)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlHidden.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.progressBar1);
            this.grbBankInformation.Controls.Add(this.rbtnMonth);
            this.grbBankInformation.Controls.Add(this.dtpMonth);
            this.grbBankInformation.Controls.Add(this.rbtnDate);
            this.grbBankInformation.Controls.Add(this.labelBranch);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Controls.Add(this.dtpToDate);
            this.grbBankInformation.Controls.Add(this.dtpFromDate);
            this.grbBankInformation.Controls.Add(this.panel2);
            this.grbBankInformation.Controls.Add(this.dgvProduct);
            this.grbBankInformation.Controls.Add(this.groupBox1);
            this.grbBankInformation.Location = new System.Drawing.Point(1, 0);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(677, 360);
            this.grbBankInformation.TabIndex = 79;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(205, 247);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(262, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 80;
            this.progressBar1.Visible = false;
            // 
            // rbtnMonth
            // 
            this.rbtnMonth.AutoSize = true;
            this.rbtnMonth.Location = new System.Drawing.Point(8, 44);
            this.rbtnMonth.Name = "rbtnMonth";
            this.rbtnMonth.Size = new System.Drawing.Size(55, 17);
            this.rbtnMonth.TabIndex = 537;
            this.rbtnMonth.Text = "Month";
            this.rbtnMonth.UseVisualStyleBackColor = true;
            this.rbtnMonth.CheckedChanged += new System.EventHandler(this.rbtnMonth_CheckedChanged);
            // 
            // dtpMonth
            // 
            this.dtpMonth.Checked = false;
            this.dtpMonth.CustomFormat = "MMMM-yyyy";
            this.dtpMonth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpMonth.Location = new System.Drawing.Point(95, 44);
            this.dtpMonth.Name = "dtpMonth";
            this.dtpMonth.Size = new System.Drawing.Size(117, 20);
            this.dtpMonth.TabIndex = 538;
            this.dtpMonth.Visible = false;
            this.dtpMonth.ValueChanged += new System.EventHandler(this.dtpMonth_ValueChanged);
            this.dtpMonth.Leave += new System.EventHandler(this.dtpMonth_Leave);
            // 
            // rbtnDate
            // 
            this.rbtnDate.AutoSize = true;
            this.rbtnDate.Checked = true;
            this.rbtnDate.Location = new System.Drawing.Point(8, 19);
            this.rbtnDate.Name = "rbtnDate";
            this.rbtnDate.Size = new System.Drawing.Size(48, 17);
            this.rbtnDate.TabIndex = 536;
            this.rbtnDate.TabStop = true;
            this.rbtnDate.Text = "Date";
            this.rbtnDate.UseVisualStyleBackColor = true;
            // 
            // labelBranch
            // 
            this.labelBranch.AutoSize = true;
            this.labelBranch.Location = new System.Drawing.Point(377, 21);
            this.labelBranch.Name = "labelBranch";
            this.labelBranch.Size = new System.Drawing.Size(41, 13);
            this.labelBranch.TabIndex = 83;
            this.labelBranch.Text = "Branch";
            this.labelBranch.Click += new System.EventHandler(this.labelBranch_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(216, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 46;
            this.label1.Text = "To";
            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(241, 17);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(117, 20);
            this.dtpToDate.TabIndex = 43;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(96, 18);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(117, 20);
            this.dtpFromDate.TabIndex = 42;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.chkBranch);
            this.panel2.Controls.Add(this.chkGroupTopSheet);
            this.panel2.Controls.Add(this.chkTopSheet);
            this.panel2.Controls.Add(this.cmbBranchName);
            this.panel2.Location = new System.Drawing.Point(5, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(671, 55);
            this.panel2.TabIndex = 539;
            // 
            // chkBranch
            // 
            this.chkBranch.AutoSize = true;
            this.chkBranch.Location = new System.Drawing.Point(236, 31);
            this.chkBranch.Name = "chkBranch";
            this.chkBranch.Size = new System.Drawing.Size(87, 17);
            this.chkBranch.TabIndex = 568;
            this.chkBranch.Text = "Branch Wise";
            this.chkBranch.UseVisualStyleBackColor = true;
            // 
            // chkGroupTopSheet
            // 
            this.chkGroupTopSheet.AutoSize = true;
            this.chkGroupTopSheet.Location = new System.Drawing.Point(372, 31);
            this.chkGroupTopSheet.Name = "chkGroupTopSheet";
            this.chkGroupTopSheet.Size = new System.Drawing.Size(102, 17);
            this.chkGroupTopSheet.TabIndex = 567;
            this.chkGroupTopSheet.Text = "GroupTopSheet";
            this.chkGroupTopSheet.UseVisualStyleBackColor = true;
            // 
            // chkTopSheet
            // 
            this.chkTopSheet.AutoSize = true;
            this.chkTopSheet.Location = new System.Drawing.Point(477, 32);
            this.chkTopSheet.Name = "chkTopSheet";
            this.chkTopSheet.Size = new System.Drawing.Size(73, 17);
            this.chkTopSheet.TabIndex = 566;
            this.chkTopSheet.Text = "TopSheet";
            this.chkTopSheet.UseVisualStyleBackColor = true;
            // 
            // cmbBranchName
            // 
            this.cmbBranchName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranchName.FormattingEnabled = true;
            this.cmbBranchName.Location = new System.Drawing.Point(418, 6);
            this.cmbBranchName.Name = "cmbBranchName";
            this.cmbBranchName.Size = new System.Drawing.Size(136, 21);
            this.cmbBranchName.TabIndex = 82;
            this.cmbBranchName.SelectedIndexChanged += new System.EventHandler(this.cmbBranchName_SelectedIndexChanged);
            // 
            // dgvProduct
            // 
            this.dgvProduct.AllowUserToAddRows = false;
            this.dgvProduct.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvProduct.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvProduct.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProduct.Location = new System.Drawing.Point(5, 185);
            this.dgvProduct.Name = "dgvProduct";
            this.dgvProduct.ReadOnly = true;
            this.dgvProduct.RowHeadersVisible = false;
            this.dgvProduct.RowTemplate.Height = 24;
            this.dgvProduct.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProduct.Size = new System.Drawing.Size(671, 170);
            this.dgvProduct.TabIndex = 545;
            this.dgvProduct.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProduct_CellContentClick);
            this.dgvProduct.Click += new System.EventHandler(this.dgvProduct_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtnProductCode);
            this.groupBox1.Controls.Add(this.rbtnProductName);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.chkMultiple);
            this.groupBox1.Controls.Add(this.txtConvertion);
            this.groupBox1.Controls.Add(this.cmbProductType);
            this.groupBox1.Controls.Add(this.txtPGroup);
            this.groupBox1.Controls.Add(this.chbInEnglish);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.cmbFontSize);
            this.groupBox1.Controls.Add(this.txtUOM);
            this.groupBox1.Controls.Add(this.btnSearchProductGroup);
            this.groupBox1.Controls.Add(this.rbtnProductType);
            this.groupBox1.Controls.Add(this.cmbUOM);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtProductCode);
            this.groupBox1.Controls.Add(this.txtProName);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.rbtnProductGroup);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Location = new System.Drawing.Point(5, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(670, 115);
            this.groupBox1.TabIndex = 561;
            this.groupBox1.TabStop = false;
            // 
            // rbtnProductCode
            // 
            this.rbtnProductCode.AutoSize = true;
            this.rbtnProductCode.Location = new System.Drawing.Point(306, 92);
            this.rbtnProductCode.Name = "rbtnProductCode";
            this.rbtnProductCode.Size = new System.Drawing.Size(14, 13);
            this.rbtnProductCode.TabIndex = 561;
            this.rbtnProductCode.UseVisualStyleBackColor = true;
            this.rbtnProductCode.CheckedChanged += new System.EventHandler(this.rbtnProductCode_CheckedChanged);
            // 
            // rbtnProductName
            // 
            this.rbtnProductName.AutoSize = true;
            this.rbtnProductName.Checked = true;
            this.rbtnProductName.Location = new System.Drawing.Point(306, 67);
            this.rbtnProductName.Name = "rbtnProductName";
            this.rbtnProductName.Size = new System.Drawing.Size(14, 13);
            this.rbtnProductName.TabIndex = 560;
            this.rbtnProductName.TabStop = true;
            this.rbtnProductName.UseVisualStyleBackColor = true;
            this.rbtnProductName.CheckedChanged += new System.EventHandler(this.rbtnProductName_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 13);
            this.label8.TabIndex = 554;
            this.label8.Text = "Product Type";
            // 
            // chkMultiple
            // 
            this.chkMultiple.AutoSize = true;
            this.chkMultiple.Location = new System.Drawing.Point(410, 67);
            this.chkMultiple.Name = "chkMultiple";
            this.chkMultiple.Size = new System.Drawing.Size(55, 17);
            this.chkMultiple.TabIndex = 562;
            this.chkMultiple.Text = "Single";
            this.chkMultiple.UseVisualStyleBackColor = true;
            this.chkMultiple.CheckedChanged += new System.EventHandler(this.chkMultiple_CheckedChanged);
            this.chkMultiple.Click += new System.EventHandler(this.chkMultiple_Click);
            // 
            // txtConvertion
            // 
            this.txtConvertion.Location = new System.Drawing.Point(628, 13);
            this.txtConvertion.MaximumSize = new System.Drawing.Size(250, 20);
            this.txtConvertion.Multiline = true;
            this.txtConvertion.Name = "txtConvertion";
            this.txtConvertion.ReadOnly = true;
            this.txtConvertion.Size = new System.Drawing.Size(32, 20);
            this.txtConvertion.TabIndex = 533;
            this.txtConvertion.Text = "0";
            this.txtConvertion.Visible = false;
            // 
            // cmbProductType
            // 
            this.cmbProductType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProductType.Enabled = false;
            this.cmbProductType.FormattingEnabled = true;
            this.cmbProductType.Location = new System.Drawing.Point(106, 13);
            this.cmbProductType.Name = "cmbProductType";
            this.cmbProductType.Size = new System.Drawing.Size(178, 21);
            this.cmbProductType.TabIndex = 555;
            this.cmbProductType.Leave += new System.EventHandler(this.cmbProductType_Leave);
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
            this.txtPGroup.TabIndex = 550;
            this.txtPGroup.TextChanged += new System.EventHandler(this.txtPGroup_TextChanged);
            this.txtPGroup.DoubleClick += new System.EventHandler(this.txtPGroup_DoubleClick);
            this.txtPGroup.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPGroup_KeyDown);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 13);
            this.label7.TabIndex = 552;
            this.label7.Text = "Product Group(F9)";
            // 
            // txtUOM
            // 
            this.txtUOM.Location = new System.Drawing.Point(418, 13);
            this.txtUOM.MaximumSize = new System.Drawing.Size(250, 20);
            this.txtUOM.Multiline = true;
            this.txtUOM.Name = "txtUOM";
            this.txtUOM.ReadOnly = true;
            this.txtUOM.Size = new System.Drawing.Size(68, 20);
            this.txtUOM.TabIndex = 534;
            // 
            // btnSearchProductGroup
            // 
            this.btnSearchProductGroup.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchProductGroup.Enabled = false;
            this.btnSearchProductGroup.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchProductGroup.Location = new System.Drawing.Point(329, 38);
            this.btnSearchProductGroup.Name = "btnSearchProductGroup";
            this.btnSearchProductGroup.Size = new System.Drawing.Size(30, 20);
            this.btnSearchProductGroup.TabIndex = 553;
            this.btnSearchProductGroup.UseVisualStyleBackColor = false;
            this.btnSearchProductGroup.Visible = false;
            this.btnSearchProductGroup.Click += new System.EventHandler(this.btnSearchProductGroup_Click);
            // 
            // rbtnProductType
            // 
            this.rbtnProductType.AutoSize = true;
            this.rbtnProductType.Location = new System.Drawing.Point(306, 17);
            this.rbtnProductType.Name = "rbtnProductType";
            this.rbtnProductType.Size = new System.Drawing.Size(14, 13);
            this.rbtnProductType.TabIndex = 549;
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
            this.cmbUOM.TabIndex = 535;
            this.cmbUOM.SelectedIndexChanged += new System.EventHandler(this.cmbUOM_SelectedIndexChanged);
            this.cmbUOM.Leave += new System.EventHandler(this.cmbUOM_Leave);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 92);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 13);
            this.label9.TabIndex = 556;
            this.label9.Text = "Product Code(F9)";
            // 
            // txtProductCode
            // 
            this.txtProductCode.Enabled = false;
            this.txtProductCode.Location = new System.Drawing.Point(106, 88);
            this.txtProductCode.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtProductCode.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.Size = new System.Drawing.Size(178, 20);
            this.txtProductCode.TabIndex = 557;
            this.txtProductCode.TextChanged += new System.EventHandler(this.txtProductCode_TextChanged);
            this.txtProductCode.DoubleClick += new System.EventHandler(this.txtProductCode_DoubleClick);
            this.txtProductCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProductCode_KeyDown);
            // 
            // txtProName
            // 
            this.txtProName.Location = new System.Drawing.Point(106, 63);
            this.txtProName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtProName.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtProName.Name = "txtProName";
            this.txtProName.Size = new System.Drawing.Size(178, 20);
            this.txtProName.TabIndex = 559;
            this.txtProName.TextChanged += new System.EventHandler(this.txtProName_TextChanged);
            this.txtProName.DoubleClick += new System.EventHandler(this.txtProName_DoubleClick);
            this.txtProName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProName_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(493, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 538;
            this.label5.Text = "Pkt Size";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(377, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 536;
            this.label4.Text = "UOM";
            // 
            // rbtnProductGroup
            // 
            this.rbtnProductGroup.AutoSize = true;
            this.rbtnProductGroup.Location = new System.Drawing.Point(306, 42);
            this.rbtnProductGroup.Name = "rbtnProductGroup";
            this.rbtnProductGroup.Size = new System.Drawing.Size(14, 13);
            this.rbtnProductGroup.TabIndex = 551;
            this.rbtnProductGroup.UseVisualStyleBackColor = true;
            this.rbtnProductGroup.CheckedChanged += new System.EventHandler(this.rbtnProductGroup_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 67);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(93, 13);
            this.label10.TabIndex = 558;
            this.label10.Text = "Product Name(F9)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(526, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 13);
            this.label6.TabIndex = 560;
            this.label6.Text = "Font";
            // 
            // chbInEnglish
            // 
            this.chbInEnglish.AutoSize = true;
            this.chbInEnglish.Location = new System.Drawing.Point(600, 87);
            this.chbInEnglish.Name = "chbInEnglish";
            this.chbInEnglish.Size = new System.Drawing.Size(59, 17);
            this.chbInEnglish.TabIndex = 527;
            this.chbInEnglish.Text = "Bangla";
            this.chbInEnglish.UseVisualStyleBackColor = true;
            this.chbInEnglish.CheckedChanged += new System.EventHandler(this.chbInEnglish_CheckedChanged);
            this.chbInEnglish.Click += new System.EventHandler(this.chbInEnglish_Click);
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
            this.cmbFontSize.Location = new System.Drawing.Point(558, 84);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 84;
            this.cmbFontSize.Text = "8";
            // 
            // btnVAT6_1
            // 
            this.btnVAT6_1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnVAT6_1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVAT6_1.Location = new System.Drawing.Point(313, 2);
            this.btnVAT6_1.Name = "btnVAT6_1";
            this.btnVAT6_1.Size = new System.Drawing.Size(72, 22);
            this.btnVAT6_1.TabIndex = 42;
            this.btnVAT6_1.Text = "VAT 6.1";
            this.btnVAT6_1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVAT6_1.UseVisualStyleBackColor = false;
            this.btnVAT6_1.Visible = false;
            this.btnVAT6_1.Click += new System.EventHandler(this.btnVAT6_1_Click);
            // 
            // txtTransType
            // 
            this.txtTransType.Location = new System.Drawing.Point(273, 3);
            this.txtTransType.Name = "txtTransType";
            this.txtTransType.Size = new System.Drawing.Size(34, 20);
            this.txtTransType.TabIndex = 81;
            this.txtTransType.Visible = false;
            // 
            // rbtnTollRegisterRaw
            // 
            this.rbtnTollRegisterRaw.AutoSize = true;
            this.rbtnTollRegisterRaw.Location = new System.Drawing.Point(121, 3);
            this.rbtnTollRegisterRaw.Name = "rbtnTollRegisterRaw";
            this.rbtnTollRegisterRaw.Size = new System.Drawing.Size(103, 17);
            this.rbtnTollRegisterRaw.TabIndex = 80;
            this.rbtnTollRegisterRaw.TabStop = true;
            this.rbtnTollRegisterRaw.Text = "TollRegisterRaw";
            this.rbtnTollRegisterRaw.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(425, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 40;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(387, 3);
            this.txtProductName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtProductName.MinimumSize = new System.Drawing.Size(4, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(34, 20);
            this.txtProductName.TabIndex = 35;
            this.txtProductName.TextChanged += new System.EventHandler(this.txtProductName_TextChanged);
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(232, 3);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(4, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.Size = new System.Drawing.Size(34, 20);
            this.txtItemNo.TabIndex = 44;
            this.txtItemNo.Visible = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.Print;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(451, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 28);
            this.button1.TabIndex = 41;
            this.button1.Text = "VAT 6.1";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.cmbValue);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cmbDecimal);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnPrev);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(3, 364);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(675, 40);
            this.panel1.TabIndex = 78;
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
            this.cmbDecimal.Location = new System.Drawing.Point(198, 12);
            this.cmbDecimal.Name = "cmbDecimal";
            this.cmbDecimal.Size = new System.Drawing.Size(36, 21);
            this.cmbDecimal.TabIndex = 563;
            this.cmbDecimal.Text = "2";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(147, 17);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(46, 13);
            this.label13.TabIndex = 562;
            this.label13.Text = "Quantity";
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(9, 12);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 561;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrev.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrev.Location = new System.Drawing.Point(375, 6);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(72, 28);
            this.btnPrev.TabIndex = 42;
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
            this.btnClose.Location = new System.Drawing.Point(609, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(61, 28);
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
            this.btnCancel.Location = new System.Drawing.Point(528, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // backgroundWorkerPreview
            // 
            this.backgroundWorkerPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreview_DoWork);
            this.backgroundWorkerPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreview_RunWorkerCompleted);
            // 
            // rbtnTollRegisterFinish
            // 
            this.rbtnTollRegisterFinish.AutoSize = true;
            this.rbtnTollRegisterFinish.Location = new System.Drawing.Point(4, 1);
            this.rbtnTollRegisterFinish.Name = "rbtnTollRegisterFinish";
            this.rbtnTollRegisterFinish.Size = new System.Drawing.Size(108, 17);
            this.rbtnTollRegisterFinish.TabIndex = 81;
            this.rbtnTollRegisterFinish.TabStop = true;
            this.rbtnTollRegisterFinish.Text = "TollRegisterFinish";
            this.rbtnTollRegisterFinish.UseVisualStyleBackColor = true;
            // 
            // pnlHidden
            // 
            this.pnlHidden.Controls.Add(this.txtPGroupId);
            this.pnlHidden.Controls.Add(this.rbtnTollRegisterRaw);
            this.pnlHidden.Controls.Add(this.rbtnTollRegisterFinish);
            this.pnlHidden.Controls.Add(this.txtProductName);
            this.pnlHidden.Controls.Add(this.txtItemNo);
            this.pnlHidden.Controls.Add(this.btnVAT6_1);
            this.pnlHidden.Controls.Add(this.btnSearch);
            this.pnlHidden.Controls.Add(this.txtTransType);
            this.pnlHidden.Location = new System.Drawing.Point(7, 310);
            this.pnlHidden.Name = "pnlHidden";
            this.pnlHidden.Size = new System.Drawing.Size(669, 30);
            this.pnlHidden.TabIndex = 82;
            this.pnlHidden.Visible = false;
            // 
            // txtPGroupId
            // 
            this.txtPGroupId.Location = new System.Drawing.Point(461, 4);
            this.txtPGroupId.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPGroupId.MinimumSize = new System.Drawing.Size(25, 20);
            this.txtPGroupId.Name = "txtPGroupId";
            this.txtPGroupId.Size = new System.Drawing.Size(25, 20);
            this.txtPGroupId.TabIndex = 561;
            this.txtPGroupId.Visible = false;
            // 
            // cmbValue
            // 
            this.cmbValue.FormattingEnabled = true;
            this.cmbValue.Items.AddRange(new object[] {
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
            this.cmbValue.Location = new System.Drawing.Point(317, 10);
            this.cmbValue.Name = "cmbValue";
            this.cmbValue.Size = new System.Drawing.Size(36, 21);
            this.cmbValue.TabIndex = 565;
            this.cmbValue.Text = "2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(266, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 564;
            this.label2.Text = "Value";
            // 
            // FormRptVAT16
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(680, 407);
            this.Controls.Add(this.pnlHidden);
            this.Controls.Add(this.grbBankInformation);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRptVAT16";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report (VAT 6.1) Purchase Register";
            this.Load += new System.EventHandler(this.FormRptVAT16_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProduct)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlHidden.ResumeLayout(false);
            this.pnlHidden.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Button btnSearch;
        public System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.DateTimePicker dtpFromDate;
        public System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        public System.Windows.Forms.TextBox txtTransType;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.RadioButton rbtnTollRegisterRaw;
        public System.Windows.Forms.RadioButton rbtnTollRegisterFinish;
        private System.Windows.Forms.ComboBox cmbBranchName;
        private System.Windows.Forms.Label labelBranch;
        private System.Windows.Forms.Button btnVAT6_1;
        private System.Windows.Forms.ComboBox cmbFontSize;
        private System.Windows.Forms.CheckBox chbInEnglish;
        private System.Windows.Forms.RadioButton rbtnMonth;
        private System.Windows.Forms.DateTimePicker dtpMonth;
        private System.Windows.Forms.RadioButton rbtnDate;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.TextBox txtProName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.RadioButton rbtnProductGroup;
        public System.Windows.Forms.TextBox txtProductCode;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton rbtnProductType;
        private System.Windows.Forms.Button btnSearchProductGroup;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPGroup;
        private System.Windows.Forms.ComboBox cmbProductType;
        private System.Windows.Forms.TextBox txtConvertion;
        public System.Windows.Forms.TextBox txtUOM;
        public System.Windows.Forms.ComboBox cmbUOM;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel pnlHidden;
        private System.Windows.Forms.DataGridView dgvProduct;
        private System.Windows.Forms.TextBox txtPGroupId;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.RadioButton rbtnProductCode;
        private System.Windows.Forms.RadioButton rbtnProductName;
        private System.Windows.Forms.CheckBox chkMultiple;
        private System.Windows.Forms.CheckBox chkGroupTopSheet;
        private System.Windows.Forms.CheckBox chkTopSheet;
        private System.Windows.Forms.CheckBox chkBranch;
        public System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmbDecimal;
        private System.Windows.Forms.ComboBox cmbValue;
        public System.Windows.Forms.Label label2;
    }
}