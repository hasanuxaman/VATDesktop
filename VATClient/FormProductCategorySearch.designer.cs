namespace VATClient
{
    partial class FormProductCategorySearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSearch = new System.Windows.Forms.Button();
            this.grbProductCategories = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbActive = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtVatRateTo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtVatRateFrom = new System.Windows.Forms.TextBox();
            this.txtCategoryName = new System.Windows.Forms.TextBox();
            this.txtCategoryID = new System.Windows.Forms.TextBox();
            this.txtPType = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtHSCodeNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtVatRate = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvProductCategories = new System.Windows.Forms.DataGridView();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.IsRaw1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CategoryName1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.CategoryID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CategoryName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsRaw = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HSCodeNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReportType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Trading = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NonStock = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VATRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SDRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActiveStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HSDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PropergatingRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grbProductCategories.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductCategories)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(372, 66);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // grbProductCategories
            // 
            this.grbProductCategories.Controls.Add(this.button1);
            this.grbProductCategories.Controls.Add(this.label11);
            this.grbProductCategories.Controls.Add(this.cmbActive);
            this.grbProductCategories.Controls.Add(this.btnAdd);
            this.grbProductCategories.Controls.Add(this.label7);
            this.grbProductCategories.Controls.Add(this.txtVatRateTo);
            this.grbProductCategories.Controls.Add(this.label6);
            this.grbProductCategories.Controls.Add(this.txtVatRateFrom);
            this.grbProductCategories.Controls.Add(this.txtCategoryName);
            this.grbProductCategories.Controls.Add(this.txtCategoryID);
            this.grbProductCategories.Controls.Add(this.txtPType);
            this.grbProductCategories.Controls.Add(this.btnSearch);
            this.grbProductCategories.Controls.Add(this.label16);
            this.grbProductCategories.Controls.Add(this.label2);
            this.grbProductCategories.Controls.Add(this.txtHSCodeNo);
            this.grbProductCategories.Controls.Add(this.label1);
            this.grbProductCategories.Location = new System.Drawing.Point(12, 6);
            this.grbProductCategories.Name = "grbProductCategories";
            this.grbProductCategories.Size = new System.Drawing.Size(461, 106);
            this.grbProductCategories.TabIndex = 9;
            this.grbProductCategories.TabStop = false;
            this.grbProductCategories.Text = "Searching Criteria";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.add_over;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(193, 66);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 28);
            this.button1.TabIndex = 120;
            this.button1.Text = "&Add SL";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(261, 43);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 212;
            this.label11.Text = "Active";
            // 
            // cmbActive
            // 
            this.cmbActive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActive.FormattingEnabled = true;
            this.cmbActive.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbActive.Location = new System.Drawing.Point(347, 40);
            this.cmbActive.Name = "cmbActive";
            this.cmbActive.Size = new System.Drawing.Size(44, 21);
            this.cmbActive.TabIndex = 211;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(291, 66);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(615, 93);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 118;
            this.label7.Text = "to";
            this.label7.Visible = false;
            // 
            // txtVatRateTo
            // 
            this.txtVatRateTo.Location = new System.Drawing.Point(639, 89);
            this.txtVatRateTo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtVatRateTo.MinimumSize = new System.Drawing.Size(60, 20);
            this.txtVatRateTo.Name = "txtVatRateTo";
            this.txtVatRateTo.Size = new System.Drawing.Size(60, 20);
            this.txtVatRateTo.TabIndex = 6;
            this.txtVatRateTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVatRateTo.Visible = false;
            this.txtVatRateTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVatRateTo_KeyDown);
            this.txtVatRateTo.Leave += new System.EventHandler(this.txtVatRateTo_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(465, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 41;
            this.label6.Text = "VAT Rate:";
            this.label6.Visible = false;
            // 
            // txtVatRateFrom
            // 
            this.txtVatRateFrom.Location = new System.Drawing.Point(549, 89);
            this.txtVatRateFrom.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtVatRateFrom.MinimumSize = new System.Drawing.Size(60, 20);
            this.txtVatRateFrom.Name = "txtVatRateFrom";
            this.txtVatRateFrom.Size = new System.Drawing.Size(60, 20);
            this.txtVatRateFrom.TabIndex = 5;
            this.txtVatRateFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVatRateFrom.Visible = false;
            this.txtVatRateFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVatRateFrom_KeyDown);
            this.txtVatRateFrom.Leave += new System.EventHandler(this.txtVatRateFrom_Leave);
            // 
            // txtCategoryName
            // 
            this.txtCategoryName.Location = new System.Drawing.Point(51, 39);
            this.txtCategoryName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCategoryName.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtCategoryName.Name = "txtCategoryName";
            this.txtCategoryName.Size = new System.Drawing.Size(150, 21);
            this.txtCategoryName.TabIndex = 1;
            this.txtCategoryName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCategoryName_KeyDown);
            // 
            // txtCategoryID
            // 
            this.txtCategoryID.Location = new System.Drawing.Point(51, 16);
            this.txtCategoryID.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCategoryID.MinimumSize = new System.Drawing.Size(100, 20);
            this.txtCategoryID.Name = "txtCategoryID";
            this.txtCategoryID.Size = new System.Drawing.Size(100, 21);
            this.txtCategoryID.TabIndex = 0;
            this.txtCategoryID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCategoryID_KeyDown);
            // 
            // txtPType
            // 
            this.txtPType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.txtPType.FormattingEnabled = true;
            this.txtPType.Location = new System.Drawing.Point(347, 16);
            this.txtPType.Name = "txtPType";
            this.txtPType.Size = new System.Drawing.Size(100, 21);
            this.txtPType.TabIndex = 2;
            this.txtPType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbIsRaw_KeyDown);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(261, 20);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(71, 13);
            this.label16.TabIndex = 16;
            this.label16.Text = "Product Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Name";
            // 
            // txtHSCodeNo
            // 
            this.txtHSCodeNo.Location = new System.Drawing.Point(476, 37);
            this.txtHSCodeNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtHSCodeNo.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtHSCodeNo.Name = "txtHSCodeNo";
            this.txtHSCodeNo.Size = new System.Drawing.Size(150, 21);
            this.txtHSCodeNo.TabIndex = 4;
            this.txtHSCodeNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtHSCodeNo_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ID";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(516, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "VAT Rate:";
            // 
            // txtVatRate
            // 
            this.txtVatRate.Location = new System.Drawing.Point(583, 93);
            this.txtVatRate.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVatRate.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtVatRate.Name = "txtVatRate";
            this.txtVatRate.Size = new System.Drawing.Size(50, 21);
            this.txtVatRate.TabIndex = 17;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(-1, 322);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(487, 40);
            this.panel1.TabIndex = 6;
            this.panel1.TabStop = true;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button2.Image = global::VATClient.Properties.Resources.Referesh;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(206, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 28);
            this.button2.TabIndex = 222;
            this.button2.Text = "&Refresh";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(96, 13);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 221;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(399, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
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
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvProductCategories);
            this.groupBox1.Location = new System.Drawing.Point(13, 111);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(459, 192);
            this.groupBox1.TabIndex = 106;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(93, 40);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 121;
            this.progressBar1.Visible = false;
            // 
            // dgvProductCategories
            // 
            this.dgvProductCategories.AllowUserToAddRows = false;
            this.dgvProductCategories.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvProductCategories.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvProductCategories.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductCategories.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.CategoryID,
            this.CategoryName,
            this.IsRaw,
            this.Description,
            this.Comments,
            this.HSCodeNo,
            this.ReportType,
            this.Trading,
            this.NonStock,
            this.VATRate,
            this.SDRate,
            this.ActiveStatus,
            this.HSDescription,
            this.PropergatingRate});
            this.dgvProductCategories.Location = new System.Drawing.Point(3, 11);
            this.dgvProductCategories.Name = "dgvProductCategories";
            this.dgvProductCategories.RowHeadersVisible = false;
            this.dgvProductCategories.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProductCategories.Size = new System.Drawing.Size(454, 174);
            this.dgvProductCategories.TabIndex = 15;
            this.dgvProductCategories.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProductCategories_CellContentClick);
            this.dgvProductCategories.DoubleClick += new System.EventHandler(this.dgvProductCategories_DoubleClick);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IsRaw1,
            this.CategoryName1});
            this.dataGridView1.Location = new System.Drawing.Point(488, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(284, 284);
            this.dataGridView1.TabIndex = 107;
            // 
            // IsRaw1
            // 
            this.IsRaw1.HeaderText = "IsRaw";
            this.IsRaw1.Name = "IsRaw1";
            // 
            // CategoryName1
            // 
            this.CategoryName1.HeaderText = "CategoryName1";
            this.CategoryName1.Name = "CategoryName1";
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // CategoryID
            // 
            this.CategoryID.HeaderText = "ID";
            this.CategoryID.Name = "CategoryID";
            this.CategoryID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CategoryID.Visible = false;
            // 
            // CategoryName
            // 
            this.CategoryName.HeaderText = "Name";
            this.CategoryName.Name = "CategoryName";
            this.CategoryName.ReadOnly = true;
            // 
            // IsRaw
            // 
            this.IsRaw.HeaderText = "Type";
            this.IsRaw.Name = "IsRaw";
            this.IsRaw.ReadOnly = true;
            // 
            // Description
            // 
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Visible = false;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            this.Comments.Visible = false;
            // 
            // HSCodeNo
            // 
            this.HSCodeNo.HeaderText = "HSCodes";
            this.HSCodeNo.Name = "HSCodeNo";
            this.HSCodeNo.ReadOnly = true;
            this.HSCodeNo.Visible = false;
            // 
            // ReportType
            // 
            this.ReportType.HeaderText = "ReportType";
            this.ReportType.Name = "ReportType";
            // 
            // Trading
            // 
            this.Trading.HeaderText = "Trading";
            this.Trading.Name = "Trading";
            // 
            // NonStock
            // 
            this.NonStock.HeaderText = "NonStock";
            this.NonStock.Name = "NonStock";
            this.NonStock.Width = 85;
            // 
            // VATRate
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "N2";
            dataGridViewCellStyle2.NullValue = null;
            this.VATRate.DefaultCellStyle = dataGridViewCellStyle2;
            this.VATRate.HeaderText = "VAT Rate";
            this.VATRate.Name = "VATRate";
            this.VATRate.ReadOnly = true;
            // 
            // SDRate
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.SDRate.DefaultCellStyle = dataGridViewCellStyle3;
            this.SDRate.HeaderText = "SDRate";
            this.SDRate.Name = "SDRate";
            // 
            // ActiveStatus
            // 
            this.ActiveStatus.HeaderText = "Active Status";
            this.ActiveStatus.Name = "ActiveStatus";
            this.ActiveStatus.ReadOnly = true;
            // 
            // HSDescription
            // 
            this.HSDescription.HeaderText = "HSDescription";
            this.HSDescription.Name = "HSDescription";
            this.HSDescription.ReadOnly = true;
            this.HSDescription.Visible = false;
            this.HSDescription.Width = 5;
            // 
            // PropergatingRate
            // 
            this.PropergatingRate.HeaderText = "PropergatingRate";
            this.PropergatingRate.Name = "PropergatingRate";
            this.PropergatingRate.ReadOnly = true;
            this.PropergatingRate.Visible = false;
            this.PropergatingRate.Width = 5;
            // 
            // FormProductCategorySearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(484, 361);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtVatRate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbProductCategories);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 400);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "FormProductCategorySearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Product Group Search";
            this.Load += new System.EventHandler(this.FormProductCategorySearch_Load);
            this.grbProductCategories.ResumeLayout(false);
            this.grbProductCategories.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductCategories)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox grbProductCategories;
        private System.Windows.Forms.TextBox txtVatRate;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtHSCodeNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox txtPType;
        private System.Windows.Forms.TextBox txtCategoryName;
        private System.Windows.Forms.TextBox txtCategoryID;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvProductCategories;
        private System.Windows.Forms.TextBox txtVatRateTo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtVatRateFrom;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsRaw1;
        private System.Windows.Forms.DataGridViewTextBoxColumn CategoryName1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbActive;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn CategoryID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CategoryName;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsRaw;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn HSCodeNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReportType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Trading;
        private System.Windows.Forms.DataGridViewTextBoxColumn NonStock;
        private System.Windows.Forms.DataGridViewTextBoxColumn VATRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn SDRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActiveStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn HSDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn PropergatingRate;
    }
}