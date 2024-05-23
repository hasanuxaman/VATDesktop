namespace VATClient.ReportPages
{
    partial class FormRptReceiveInformation
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
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbDecimal = new System.Windows.Forms.ComboBox();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.cmbBranchName = new System.Windows.Forms.ComboBox();
            this.labelBranch = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.chkShiftAll = new System.Windows.Forms.CheckBox();
            this.cmbShift = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkCategoryLike = new System.Windows.Forms.CheckBox();
            this.cmbProductType = new System.Windows.Forms.ComboBox();
            this.txtTransactionType = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtProductType = new System.Windows.Forms.TextBox();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtPGroup = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnSingle = new System.Windows.Forms.RadioButton();
            this.rbSummery = new System.Windows.Forms.RadioButton();
            this.rbDetail = new System.Windows.Forms.RadioButton();
            this.btnProduct = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpReceiveToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpReceiveFromDate = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.txtReceiveNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPGroupId = new System.Windows.Forms.TextBox();
            this.backgroundWorkerPreviewSummary = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerPreviewDetails = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerMIS = new System.ComponentModel.BackgroundWorker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.grbBankInformation.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.label10);
            this.grbBankInformation.Controls.Add(this.label8);
            this.grbBankInformation.Controls.Add(this.cmbDecimal);
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.cmbBranchName);
            this.grbBankInformation.Controls.Add(this.labelBranch);
            this.grbBankInformation.Controls.Add(this.progressBar1);
            this.grbBankInformation.Controls.Add(this.chkShiftAll);
            this.grbBankInformation.Controls.Add(this.cmbShift);
            this.grbBankInformation.Controls.Add(this.label7);
            this.grbBankInformation.Controls.Add(this.chkCategoryLike);
            this.grbBankInformation.Controls.Add(this.cmbProductType);
            this.grbBankInformation.Controls.Add(this.txtTransactionType);
            this.grbBankInformation.Controls.Add(this.button3);
            this.grbBankInformation.Controls.Add(this.label6);
            this.grbBankInformation.Controls.Add(this.txtProductType);
            this.grbBankInformation.Controls.Add(this.cmbPost);
            this.grbBankInformation.Controls.Add(this.label9);
            this.grbBankInformation.Controls.Add(this.button1);
            this.grbBankInformation.Controls.Add(this.txtPGroup);
            this.grbBankInformation.Controls.Add(this.label3);
            this.grbBankInformation.Controls.Add(this.groupBox1);
            this.grbBankInformation.Controls.Add(this.btnProduct);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Controls.Add(this.dtpReceiveToDate);
            this.grbBankInformation.Controls.Add(this.dtpReceiveFromDate);
            this.grbBankInformation.Controls.Add(this.label5);
            this.grbBankInformation.Controls.Add(this.txtReceiveNo);
            this.grbBankInformation.Controls.Add(this.label4);
            this.grbBankInformation.Controls.Add(this.txtProductName);
            this.grbBankInformation.Controls.Add(this.label12);
            this.grbBankInformation.Location = new System.Drawing.Point(0, -3);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(481, 250);
            this.grbBankInformation.TabIndex = 77;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            this.grbBankInformation.Enter += new System.EventHandler(this.grbBankInformation_Enter);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(387, 228);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 13);
            this.label10.TabIndex = 527;
            this.label10.Text = "Decimal";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 228);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 526;
            this.label8.Text = "Font";
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
            this.cmbDecimal.Location = new System.Drawing.Point(436, 225);
            this.cmbDecimal.Name = "cmbDecimal";
            this.cmbDecimal.Size = new System.Drawing.Size(36, 21);
            this.cmbDecimal.TabIndex = 525;
            this.cmbDecimal.Text = "8";
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
            this.cmbFontSize.Location = new System.Drawing.Point(48, 225);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 524;
            this.cmbFontSize.Text = "8";
            // 
            // cmbBranchName
            // 
            this.cmbBranchName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranchName.FormattingEnabled = true;
            this.cmbBranchName.Location = new System.Drawing.Point(100, 198);
            this.cmbBranchName.Name = "cmbBranchName";
            this.cmbBranchName.Size = new System.Drawing.Size(136, 21);
            this.cmbBranchName.TabIndex = 522;
            // 
            // labelBranch
            // 
            this.labelBranch.AutoSize = true;
            this.labelBranch.Location = new System.Drawing.Point(13, 202);
            this.labelBranch.Name = "labelBranch";
            this.labelBranch.Size = new System.Drawing.Size(40, 13);
            this.labelBranch.TabIndex = 523;
            this.labelBranch.Text = "Branch";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(111, 225);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(265, 13);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 192;
            this.progressBar1.Visible = false;
            // 
            // chkShiftAll
            // 
            this.chkShiftAll.AutoSize = true;
            this.chkShiftAll.Checked = true;
            this.chkShiftAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShiftAll.Location = new System.Drawing.Point(298, 141);
            this.chkShiftAll.Name = "chkShiftAll";
            this.chkShiftAll.Size = new System.Drawing.Size(37, 17);
            this.chkShiftAll.TabIndex = 521;
            this.chkShiftAll.Text = "All";
            this.chkShiftAll.UseVisualStyleBackColor = true;
            // 
            // cmbShift
            // 
            this.cmbShift.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShift.FormattingEnabled = true;
            this.cmbShift.Location = new System.Drawing.Point(217, 138);
            this.cmbShift.MaximumSize = new System.Drawing.Size(80, 0);
            this.cmbShift.MinimumSize = new System.Drawing.Size(80, 0);
            this.cmbShift.Name = "cmbShift";
            this.cmbShift.Size = new System.Drawing.Size(80, 21);
            this.cmbShift.TabIndex = 520;
            this.cmbShift.TabStop = false;
            this.cmbShift.SelectedIndexChanged += new System.EventHandler(this.cmbShift_SelectedIndexChanged);
            this.cmbShift.Leave += new System.EventHandler(this.cmbShift_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(189, 142);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 519;
            this.label7.Text = "Shift";
            // 
            // chkCategoryLike
            // 
            this.chkCategoryLike.AutoSize = true;
            this.chkCategoryLike.Location = new System.Drawing.Point(339, 93);
            this.chkCategoryLike.Name = "chkCategoryLike";
            this.chkCategoryLike.Size = new System.Drawing.Size(15, 14);
            this.chkCategoryLike.TabIndex = 517;
            this.chkCategoryLike.UseVisualStyleBackColor = true;
            this.chkCategoryLike.Click += new System.EventHandler(this.chkCategoryLike_Click);
            // 
            // cmbProductType
            // 
            this.cmbProductType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProductType.FormattingEnabled = true;
            this.cmbProductType.Location = new System.Drawing.Point(100, 114);
            this.cmbProductType.Name = "cmbProductType";
            this.cmbProductType.Size = new System.Drawing.Size(197, 21);
            this.cmbProductType.TabIndex = 511;
            this.cmbProductType.SelectedIndexChanged += new System.EventHandler(this.cmbProductType_SelectedIndexChanged);
            // 
            // txtTransactionType
            // 
            this.txtTransactionType.Location = new System.Drawing.Point(323, 168);
            this.txtTransactionType.Name = "txtTransactionType";
            this.txtTransactionType.Size = new System.Drawing.Size(25, 21);
            this.txtTransactionType.TabIndex = 214;
            this.txtTransactionType.Visible = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button3.Image = global::VATClient.Properties.Resources.search;
            this.button3.Location = new System.Drawing.Point(305, 113);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(30, 20);
            this.button3.TabIndex = 213;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 117);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 212;
            this.label6.Text = "Product Type";
            // 
            // txtProductType
            // 
            this.txtProductType.Location = new System.Drawing.Point(62, 138);
            this.txtProductType.MinimumSize = new System.Drawing.Size(4, 20);
            this.txtProductType.Name = "txtProductType";
            this.txtProductType.ReadOnly = true;
            this.txtProductType.Size = new System.Drawing.Size(14, 21);
            this.txtProductType.TabIndex = 211;
            this.txtProductType.Visible = false;
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(100, 138);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(82, 21);
            this.cmbPost.TabIndex = 209;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 142);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 210;
            this.label9.Text = "Post";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.search;
            this.button1.Location = new System.Drawing.Point(307, 89);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 20);
            this.button1.TabIndex = 191;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtPGroup
            // 
            this.txtPGroup.Location = new System.Drawing.Point(100, 89);
            this.txtPGroup.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtPGroup.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtPGroup.Name = "txtPGroup";
            this.txtPGroup.ReadOnly = true;
            this.txtPGroup.Size = new System.Drawing.Size(197, 21);
            this.txtPGroup.TabIndex = 189;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 190;
            this.label3.Text = "Product Group";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtnSingle);
            this.groupBox1.Controls.Add(this.rbSummery);
            this.groupBox1.Controls.Add(this.rbDetail);
            this.groupBox1.Location = new System.Drawing.Point(68, 157);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(235, 33);
            this.groupBox1.TabIndex = 191;
            this.groupBox1.TabStop = false;
            // 
            // rbtnSingle
            // 
            this.rbtnSingle.AutoSize = true;
            this.rbtnSingle.Location = new System.Drawing.Point(164, 11);
            this.rbtnSingle.Name = "rbtnSingle";
            this.rbtnSingle.Size = new System.Drawing.Size(53, 17);
            this.rbtnSingle.TabIndex = 194;
            this.rbtnSingle.Text = "Single";
            this.rbtnSingle.UseVisualStyleBackColor = true;
            // 
            // rbSummery
            // 
            this.rbSummery.AutoSize = true;
            this.rbSummery.Location = new System.Drawing.Point(88, 10);
            this.rbSummery.Name = "rbSummery";
            this.rbSummery.Size = new System.Drawing.Size(69, 17);
            this.rbSummery.TabIndex = 192;
            this.rbSummery.Text = "Summery";
            this.rbSummery.UseVisualStyleBackColor = true;
            // 
            // rbDetail
            // 
            this.rbDetail.AutoSize = true;
            this.rbDetail.Checked = true;
            this.rbDetail.Location = new System.Drawing.Point(6, 10);
            this.rbDetail.Name = "rbDetail";
            this.rbDetail.Size = new System.Drawing.Size(52, 17);
            this.rbDetail.TabIndex = 191;
            this.rbDetail.TabStop = true;
            this.rbDetail.Text = "Detail";
            this.rbDetail.UseVisualStyleBackColor = true;
            // 
            // btnProduct
            // 
            this.btnProduct.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnProduct.Image = global::VATClient.Properties.Resources.search;
            this.btnProduct.Location = new System.Drawing.Point(307, 65);
            this.btnProduct.Name = "btnProduct";
            this.btnProduct.Size = new System.Drawing.Size(30, 20);
            this.btnProduct.TabIndex = 187;
            this.btnProduct.UseVisualStyleBackColor = false;
            this.btnProduct.Click += new System.EventHandler(this.btnProduct_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(307, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 186;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(280, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 185;
            this.label1.Text = "to";
            // 
            // dtpReceiveToDate
            // 
            this.dtpReceiveToDate.Checked = false;
            this.dtpReceiveToDate.CustomFormat = "dd/MMM/yyyy HH:mm:ss";
            this.dtpReceiveToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReceiveToDate.Location = new System.Drawing.Point(301, 39);
            this.dtpReceiveToDate.Name = "dtpReceiveToDate";
            this.dtpReceiveToDate.ShowCheckBox = true;
            this.dtpReceiveToDate.Size = new System.Drawing.Size(174, 21);
            this.dtpReceiveToDate.TabIndex = 184;
            this.dtpReceiveToDate.ValueChanged += new System.EventHandler(this.dtpReceiveToDate_ValueChanged);
            // 
            // dtpReceiveFromDate
            // 
            this.dtpReceiveFromDate.Checked = false;
            this.dtpReceiveFromDate.CustomFormat = "dd/MMM/yyyy HH:mm:ss";
            this.dtpReceiveFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReceiveFromDate.Location = new System.Drawing.Point(100, 40);
            this.dtpReceiveFromDate.Name = "dtpReceiveFromDate";
            this.dtpReceiveFromDate.ShowCheckBox = true;
            this.dtpReceiveFromDate.Size = new System.Drawing.Size(174, 21);
            this.dtpReceiveFromDate.TabIndex = 182;
            this.dtpReceiveFromDate.ValueChanged += new System.EventHandler(this.dtpReceiveFromDate_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 183;
            this.label5.Text = "Receive Date";
            // 
            // txtReceiveNo
            // 
            this.txtReceiveNo.Location = new System.Drawing.Point(100, 16);
            this.txtReceiveNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtReceiveNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtReceiveNo.Name = "txtReceiveNo";
            this.txtReceiveNo.ReadOnly = true;
            this.txtReceiveNo.Size = new System.Drawing.Size(197, 21);
            this.txtReceiveNo.TabIndex = 181;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 180;
            this.label4.Text = "Receive No";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(100, 65);
            this.txtProductName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtProductName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(197, 21);
            this.txtProductName.TabIndex = 177;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 69);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(74, 13);
            this.label12.TabIndex = 179;
            this.label12.Text = "Product Name";
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(494, 83);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(25, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.Size = new System.Drawing.Size(25, 20);
            this.txtItemNo.TabIndex = 176;
            this.txtItemNo.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(199, 271);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 178;
            this.label2.Text = "Product No";
            this.label2.Visible = false;
            // 
            // txtPGroupId
            // 
            this.txtPGroupId.Location = new System.Drawing.Point(509, 113);
            this.txtPGroupId.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPGroupId.MinimumSize = new System.Drawing.Size(25, 20);
            this.txtPGroupId.Name = "txtPGroupId";
            this.txtPGroupId.Size = new System.Drawing.Size(25, 20);
            this.txtPGroupId.TabIndex = 188;
            this.txtPGroupId.Visible = false;
            // 
            // backgroundWorkerPreviewSummary
            // 
            this.backgroundWorkerPreviewSummary.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreviewSummary_DoWork);
            this.backgroundWorkerPreviewSummary.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreviewSummary_RunWorkerCompleted);
            // 
            // backgroundWorkerPreviewDetails
            // 
            this.backgroundWorkerPreviewDetails.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreviewDetails_DoWork);
            this.backgroundWorkerPreviewDetails.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreviewDetails_RunWorkerCompleted);
            // 
            // backgroundWorkerMIS
            // 
            this.backgroundWorkerMIS.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerMIS_DoWork);
            this.backgroundWorkerMIS.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerMIS_RunWorkerCompleted);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnDownload);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Location = new System.Drawing.Point(1, 253);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(480, 40);
            this.panel1.TabIndex = 75;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(396, 4);
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
            this.btnCancel.Location = new System.Drawing.Point(90, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(8, 7);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 6;
            this.btnPreview.Text = "Preview";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDownload.Image = global::VATClient.Properties.Resources.Load;
            this.btnDownload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDownload.Location = new System.Drawing.Point(253, 7);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(81, 28);
            this.btnDownload.TabIndex = 529;
            this.btnDownload.Text = "Download ";
            this.btnDownload.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDownload.UseVisualStyleBackColor = false;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // FormRptReceiveInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(484, 296);
            this.Controls.Add(this.txtPGroupId);
            this.Controls.Add(this.grbBankInformation);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtItemNo);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 400);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 270);
            this.Name = "FormRptReceiveInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report (Receive FG-Production)";
            this.Load += new System.EventHandler(this.FormRptReceiveInformation_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpReceiveToDate;
        private System.Windows.Forms.DateTimePicker dtpReceiveFromDate;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox txtReceiveNo;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnProduct;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbSummery;
        private System.Windows.Forms.RadioButton rbDetail;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtPGroupId;
        private System.Windows.Forms.TextBox txtPGroup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreviewSummary;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreviewDetails;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMIS;
        private System.Windows.Forms.RadioButton rbtnSingle;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtProductType;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox txtTransactionType;
        private System.Windows.Forms.ComboBox cmbProductType;
        private System.Windows.Forms.CheckBox chkCategoryLike;
        public System.Windows.Forms.ComboBox cmbShift;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkShiftAll;
        private System.Windows.Forms.ComboBox cmbBranchName;
        private System.Windows.Forms.Label labelBranch;
        private System.Windows.Forms.ComboBox cmbFontSize;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbDecimal;
        private System.Windows.Forms.Button btnDownload;
    }
}