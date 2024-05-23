namespace VATClient.ReportPages
{
    partial class FormRptIssueInformation
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
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbDecimal = new System.Windows.Forms.ComboBox();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.cmbBranchName = new System.Windows.Forms.ComboBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelBranch = new System.Windows.Forms.Label();
            this.chkCategoryLike = new System.Windows.Forms.CheckBox();
            this.cmbProductType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbWaste = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtProductType = new System.Windows.Forms.TextBox();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtTransactionType = new System.Windows.Forms.TextBox();
            this.btnPGSearch = new System.Windows.Forms.Button();
            this.txtPGroup = new System.Windows.Forms.TextBox();
            this.txtPGroupId = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnSingle = new System.Windows.Forms.RadioButton();
            this.rbSummery = new System.Windows.Forms.RadioButton();
            this.rbDetail = new System.Windows.Forms.RadioButton();
            this.btnProduct = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpIssueToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpIssueFromDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIssueNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPreview = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.backgroundWorkerPreviewSummary = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerPreviewDetails = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerMIS = new System.ComponentModel.BackgroundWorker();
            this.btnDownload = new System.Windows.Forms.Button();
            this.grbBankInformation.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.label7);
            this.grbBankInformation.Controls.Add(this.label5);
            this.grbBankInformation.Controls.Add(this.cmbDecimal);
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.cmbBranchName);
            this.grbBankInformation.Controls.Add(this.progressBar1);
            this.grbBankInformation.Controls.Add(this.labelBranch);
            this.grbBankInformation.Controls.Add(this.chkCategoryLike);
            this.grbBankInformation.Controls.Add(this.cmbProductType);
            this.grbBankInformation.Controls.Add(this.label4);
            this.grbBankInformation.Controls.Add(this.cmbWaste);
            this.grbBankInformation.Controls.Add(this.button3);
            this.grbBankInformation.Controls.Add(this.label6);
            this.grbBankInformation.Controls.Add(this.txtProductType);
            this.grbBankInformation.Controls.Add(this.cmbPost);
            this.grbBankInformation.Controls.Add(this.label9);
            this.grbBankInformation.Controls.Add(this.txtTransactionType);
            this.grbBankInformation.Controls.Add(this.btnPGSearch);
            this.grbBankInformation.Controls.Add(this.txtPGroup);
            this.grbBankInformation.Controls.Add(this.txtPGroupId);
            this.grbBankInformation.Controls.Add(this.groupBox1);
            this.grbBankInformation.Controls.Add(this.btnProduct);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.txtProductName);
            this.grbBankInformation.Controls.Add(this.label12);
            this.grbBankInformation.Controls.Add(this.txtItemNo);
            this.grbBankInformation.Controls.Add(this.label2);
            this.grbBankInformation.Controls.Add(this.label11);
            this.grbBankInformation.Controls.Add(this.dtpIssueToDate);
            this.grbBankInformation.Controls.Add(this.dtpIssueFromDate);
            this.grbBankInformation.Controls.Add(this.label3);
            this.grbBankInformation.Controls.Add(this.txtIssueNo);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Location = new System.Drawing.Point(12, 2);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(365, 270);
            this.grbBankInformation.TabIndex = 79;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            this.grbBankInformation.Enter += new System.EventHandler(this.grbBankInformation_Enter);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(239, 252);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 529;
            this.label7.Text = "Decimal";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 252);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 528;
            this.label5.Text = "Font";
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
            "9",
            "10"});
            this.cmbDecimal.Location = new System.Drawing.Point(287, 249);
            this.cmbDecimal.Name = "cmbDecimal";
            this.cmbDecimal.Size = new System.Drawing.Size(36, 21);
            this.cmbDecimal.TabIndex = 527;
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
            this.cmbFontSize.Location = new System.Drawing.Point(44, 249);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 526;
            this.cmbFontSize.Text = "8";
            // 
            // cmbBranchName
            // 
            this.cmbBranchName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranchName.FormattingEnabled = true;
            this.cmbBranchName.Location = new System.Drawing.Point(108, 204);
            this.cmbBranchName.Name = "cmbBranchName";
            this.cmbBranchName.Size = new System.Drawing.Size(136, 21);
            this.cmbBranchName.TabIndex = 524;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(59, 231);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 15);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 208;
            this.progressBar1.Visible = false;
            // 
            // labelBranch
            // 
            this.labelBranch.AutoSize = true;
            this.labelBranch.Location = new System.Drawing.Point(31, 207);
            this.labelBranch.Name = "labelBranch";
            this.labelBranch.Size = new System.Drawing.Size(40, 13);
            this.labelBranch.TabIndex = 525;
            this.labelBranch.Text = "Branch";
            // 
            // chkCategoryLike
            // 
            this.chkCategoryLike.AutoSize = true;
            this.chkCategoryLike.Location = new System.Drawing.Point(348, 97);
            this.chkCategoryLike.Name = "chkCategoryLike";
            this.chkCategoryLike.Size = new System.Drawing.Size(15, 14);
            this.chkCategoryLike.TabIndex = 517;
            this.chkCategoryLike.UseVisualStyleBackColor = true;
            // 
            // cmbProductType
            // 
            this.cmbProductType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProductType.FormattingEnabled = true;
            this.cmbProductType.Location = new System.Drawing.Point(109, 117);
            this.cmbProductType.Name = "cmbProductType";
            this.cmbProductType.Size = new System.Drawing.Size(185, 21);
            this.cmbProductType.TabIndex = 511;
            this.cmbProductType.SelectedIndexChanged += new System.EventHandler(this.cmbProductType_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(197, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 216;
            this.label4.Text = "Waste";
            // 
            // cmbWaste
            // 
            this.cmbWaste.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWaste.FormattingEnabled = true;
            this.cmbWaste.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbWaste.Location = new System.Drawing.Point(244, 140);
            this.cmbWaste.Name = "cmbWaste";
            this.cmbWaste.Size = new System.Drawing.Size(48, 21);
            this.cmbWaste.TabIndex = 215;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button3.Image = global::VATClient.Properties.Resources.search;
            this.button3.Location = new System.Drawing.Point(310, 116);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(30, 20);
            this.button3.TabIndex = 214;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 213;
            this.label6.Text = "Product Type";
            // 
            // txtProductType
            // 
            this.txtProductType.Location = new System.Drawing.Point(81, 143);
            this.txtProductType.MinimumSize = new System.Drawing.Size(10, 20);
            this.txtProductType.Name = "txtProductType";
            this.txtProductType.ReadOnly = true;
            this.txtProductType.Size = new System.Drawing.Size(10, 21);
            this.txtProductType.TabIndex = 212;
            this.txtProductType.Visible = false;
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(108, 141);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(50, 21);
            this.cmbPost.TabIndex = 210;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 146);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 211;
            this.label9.Text = "Post";
            // 
            // txtTransactionType
            // 
            this.txtTransactionType.Location = new System.Drawing.Point(337, 157);
            this.txtTransactionType.Name = "txtTransactionType";
            this.txtTransactionType.Size = new System.Drawing.Size(25, 21);
            this.txtTransactionType.TabIndex = 209;
            this.txtTransactionType.Visible = false;
            // 
            // btnPGSearch
            // 
            this.btnPGSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPGSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnPGSearch.Location = new System.Drawing.Point(312, 93);
            this.btnPGSearch.Name = "btnPGSearch";
            this.btnPGSearch.Size = new System.Drawing.Size(30, 20);
            this.btnPGSearch.TabIndex = 197;
            this.btnPGSearch.UseVisualStyleBackColor = false;
            this.btnPGSearch.Click += new System.EventHandler(this.btnPGSearch_Click);
            // 
            // txtPGroup
            // 
            this.txtPGroup.Location = new System.Drawing.Point(109, 93);
            this.txtPGroup.MaximumSize = new System.Drawing.Size(185, 20);
            this.txtPGroup.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtPGroup.Name = "txtPGroup";
            this.txtPGroup.ReadOnly = true;
            this.txtPGroup.Size = new System.Drawing.Size(185, 21);
            this.txtPGroup.TabIndex = 196;
            // 
            // txtPGroupId
            // 
            this.txtPGroupId.Location = new System.Drawing.Point(337, 184);
            this.txtPGroupId.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPGroupId.MinimumSize = new System.Drawing.Size(25, 20);
            this.txtPGroupId.Name = "txtPGroupId";
            this.txtPGroupId.ReadOnly = true;
            this.txtPGroupId.Size = new System.Drawing.Size(25, 20);
            this.txtPGroupId.TabIndex = 195;
            this.txtPGroupId.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtnSingle);
            this.groupBox1.Controls.Add(this.rbSummery);
            this.groupBox1.Controls.Add(this.rbDetail);
            this.groupBox1.Location = new System.Drawing.Point(86, 167);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(215, 33);
            this.groupBox1.TabIndex = 189;
            this.groupBox1.TabStop = false;
            // 
            // rbtnSingle
            // 
            this.rbtnSingle.AutoSize = true;
            this.rbtnSingle.Location = new System.Drawing.Point(140, 11);
            this.rbtnSingle.Name = "rbtnSingle";
            this.rbtnSingle.Size = new System.Drawing.Size(53, 17);
            this.rbtnSingle.TabIndex = 195;
            this.rbtnSingle.Text = "Single";
            this.rbtnSingle.UseVisualStyleBackColor = true;
            // 
            // rbSummery
            // 
            this.rbSummery.AutoSize = true;
            this.rbSummery.Location = new System.Drawing.Point(69, 10);
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
            this.btnProduct.Location = new System.Drawing.Point(313, 68);
            this.btnProduct.Name = "btnProduct";
            this.btnProduct.Size = new System.Drawing.Size(30, 20);
            this.btnProduct.TabIndex = 188;
            this.btnProduct.UseVisualStyleBackColor = false;
            this.btnProduct.Click += new System.EventHandler(this.btnProduct_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(313, 19);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 80;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(109, 68);
            this.txtProductName.MaximumSize = new System.Drawing.Size(185, 20);
            this.txtProductName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(185, 21);
            this.txtProductName.TabIndex = 173;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(21, 72);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(74, 13);
            this.label12.TabIndex = 175;
            this.label12.Text = "Product Name";
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(365, 68);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(25, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.ReadOnly = true;
            this.txtItemNo.Size = new System.Drawing.Size(25, 20);
            this.txtItemNo.TabIndex = 172;
            this.txtItemNo.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 174;
            this.label2.Text = "Product Group";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(216, 47);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 120;
            this.label11.Text = "to";
            // 
            // dtpIssueToDate
            // 
            this.dtpIssueToDate.Checked = false;
            this.dtpIssueToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpIssueToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueToDate.Location = new System.Drawing.Point(242, 43);
            this.dtpIssueToDate.Name = "dtpIssueToDate";
            this.dtpIssueToDate.ShowCheckBox = true;
            this.dtpIssueToDate.Size = new System.Drawing.Size(101, 21);
            this.dtpIssueToDate.TabIndex = 118;
            // 
            // dtpIssueFromDate
            // 
            this.dtpIssueFromDate.Checked = false;
            this.dtpIssueFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpIssueFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueFromDate.Location = new System.Drawing.Point(109, 43);
            this.dtpIssueFromDate.Name = "dtpIssueFromDate";
            this.dtpIssueFromDate.ShowCheckBox = true;
            this.dtpIssueFromDate.Size = new System.Drawing.Size(101, 21);
            this.dtpIssueFromDate.TabIndex = 117;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 119;
            this.label3.Text = "Issue Date";
            // 
            // txtIssueNo
            // 
            this.txtIssueNo.Location = new System.Drawing.Point(109, 19);
            this.txtIssueNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtIssueNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtIssueNo.Name = "txtIssueNo";
            this.txtIssueNo.ReadOnly = true;
            this.txtIssueNo.Size = new System.Drawing.Size(192, 21);
            this.txtIssueNo.TabIndex = 113;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 114;
            this.label1.Text = "Issue No";
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(13, 7);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 6;
            this.btnPreview.Text = "Preview";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnDownload);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Location = new System.Drawing.Point(0, 278);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(390, 40);
            this.panel1.TabIndex = 78;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(299, 7);
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
            this.btnCancel.Location = new System.Drawing.Point(95, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            // btnDownload
            // 
            this.btnDownload.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDownload.Image = global::VATClient.Properties.Resources.Load;
            this.btnDownload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDownload.Location = new System.Drawing.Point(187, 7);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(81, 28);
            this.btnDownload.TabIndex = 528;
            this.btnDownload.Text = "Download ";
            this.btnDownload.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDownload.UseVisualStyleBackColor = false;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // FormRptIssueInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(384, 321);
            this.Controls.Add(this.grbBankInformation);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 360);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 330);
            this.Name = "FormRptIssueInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report (Issue)";
            this.Load += new System.EventHandler(this.FormRptIssueInformation_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtIssueNo;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnProduct;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbSummery;
        private System.Windows.Forms.RadioButton rbDetail;
        public System.Windows.Forms.DateTimePicker dtpIssueToDate;
        public System.Windows.Forms.DateTimePicker dtpIssueFromDate;
        private System.Windows.Forms.Button btnPGSearch;
        private System.Windows.Forms.TextBox txtPGroup;
        private System.Windows.Forms.TextBox txtPGroupId;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreviewSummary;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreviewDetails;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMIS;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtProductType;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox txtTransactionType;
        private System.Windows.Forms.RadioButton rbtnSingle;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbWaste;
        private System.Windows.Forms.ComboBox cmbProductType;
        private System.Windows.Forms.CheckBox chkCategoryLike;
        private System.Windows.Forms.ComboBox cmbBranchName;
        private System.Windows.Forms.Label labelBranch;
        private System.Windows.Forms.ComboBox cmbFontSize;
        private System.Windows.Forms.ComboBox cmbDecimal;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnDownload;
    }
}