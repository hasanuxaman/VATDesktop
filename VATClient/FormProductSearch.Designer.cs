namespace VATClient
{
    partial class FormProductSearch
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
            this.btnSearch = new System.Windows.Forms.Button();
            this.cmbProduct = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBOMExport = new System.Windows.Forms.Button();
            this.btnAvgPrice = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.chkIsConfirmed = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.cmbRecordCount = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.cmbIsVDS = new System.Windows.Forms.ComboBox();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbActive = new System.Windows.Forms.ComboBox();
            this.btnSearchProductCategory = new System.Windows.Forms.Button();
            this.cmbProductCategoryName = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPCode = new System.Windows.Forms.TextBox();
            this.btnProductType = new System.Windows.Forms.Button();
            this.cmbProductType = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtCostPriceTo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSalesPriceTo = new System.Windows.Forms.TextBox();
            this.txtNBRPriceTo = new System.Windows.Forms.TextBox();
            this.txtCostPriceFrom = new System.Windows.Forms.TextBox();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtSalesPriceFrom = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNBRPriceFrom = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnStockUpdate = new System.Windows.Forms.Button();
            this.txtSerialNo = new System.Windows.Forms.TextBox();
            this.txtHSCodeNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCategoryID = new System.Windows.Forms.TextBox();
            this.txtCategoryName = new System.Windows.Forms.TextBox();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.chkVAT16 = new System.Windows.Forms.CheckBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.cmbImport = new System.Windows.Forms.ComboBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTradingMarkUp = new System.Windows.Forms.TextBox();
            this.txtPacketPrice = new System.Windows.Forms.TextBox();
            this.txtSDRate = new System.Windows.Forms.TextBox();
            this.txtVATRateTo = new System.Windows.Forms.TextBox();
            this.txtVATRateFrom = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtVATRate = new System.Windows.Forms.TextBox();
            this.txtCostPrice = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtSalesPrice = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtNRBPrice = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPHistoryExport = new System.Windows.Forms.Button();
            this.btnProductStockExport = new System.Windows.Forms.Button();
            this.btnTradingExport = new System.Windows.Forms.Button();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvProduct = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.pnlHidden = new System.Windows.Forms.Panel();
            this.cmbUOM = new System.Windows.Forms.ComboBox();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerPTypeSearch = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerProductCategorySearch = new System.ComponentModel.BackgroundWorker();
            this.bgwVATProduct = new System.ComponentModel.BackgroundWorker();
            this.bgwAvgPrice = new System.ComponentModel.BackgroundWorker();
            this.bgwStockUpdate = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProduct)).BeginInit();
            this.pnlHidden.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(658, 15);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(86, 28);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cmbProduct
            // 
            this.cmbProduct.FormattingEnabled = true;
            this.cmbProduct.Location = new System.Drawing.Point(882, 166);
            this.cmbProduct.Name = "cmbProduct";
            this.cmbProduct.Size = new System.Drawing.Size(85, 25);
            this.cmbProduct.TabIndex = 102;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(872, 55);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(95, 21);
            this.button1.TabIndex = 103;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnBOMExport);
            this.groupBox1.Controls.Add(this.btnAvgPrice);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.chkIsConfirmed);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.cmbRecordCount);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.cmbIsVDS);
            this.groupBox1.Controls.Add(this.cmbBranch);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.cmbActive);
            this.groupBox1.Controls.Add(this.btnSearchProductCategory);
            this.groupBox1.Controls.Add(this.cmbProductCategoryName);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtPCode);
            this.groupBox1.Controls.Add(this.btnProductType);
            this.groupBox1.Controls.Add(this.cmbProductType);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.txtCostPriceTo);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtSalesPriceTo);
            this.groupBox1.Controls.Add(this.txtNBRPriceTo);
            this.groupBox1.Controls.Add(this.txtCostPriceFrom);
            this.groupBox1.Controls.Add(this.txtProductName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.txtSalesPriceFrom);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtNBRPriceFrom);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btnStockUpdate);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtSerialNo);
            this.groupBox1.Controls.Add(this.txtHSCodeNo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(18, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(748, 170);
            this.groupBox1.TabIndex = 104;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Searching Criteria";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // btnBOMExport
            // 
            this.btnBOMExport.BackColor = System.Drawing.Color.White;
            this.btnBOMExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBOMExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnBOMExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBOMExport.Location = new System.Drawing.Point(649, 85);
            this.btnBOMExport.Name = "btnBOMExport";
            this.btnBOMExport.Size = new System.Drawing.Size(90, 44);
            this.btnBOMExport.TabIndex = 241;
            this.btnBOMExport.Text = "BOM Export";
            this.btnBOMExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBOMExport.UseVisualStyleBackColor = false;
            this.btnBOMExport.Click += new System.EventHandler(this.btnBOMExport_Click);
            // 
            // btnAvgPrice
            // 
            this.btnAvgPrice.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAvgPrice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAvgPrice.Location = new System.Drawing.Point(749, 101);
            this.btnAvgPrice.Name = "btnAvgPrice";
            this.btnAvgPrice.Size = new System.Drawing.Size(86, 28);
            this.btnAvgPrice.TabIndex = 240;
            this.btnAvgPrice.Text = "&Avg Price Cal";
            this.btnAvgPrice.UseVisualStyleBackColor = false;
            this.btnAvgPrice.Click += new System.EventHandler(this.btnAvgPrice_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(359, 120);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(85, 17);
            this.label19.TabIndex = 239;
            this.label19.Text = "Is Confirmed";
            // 
            // chkIsConfirmed
            // 
            this.chkIsConfirmed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.chkIsConfirmed.FormattingEnabled = true;
            this.chkIsConfirmed.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.chkIsConfirmed.Location = new System.Drawing.Point(437, 114);
            this.chkIsConfirmed.Name = "chkIsConfirmed";
            this.chkIsConfirmed.Size = new System.Drawing.Size(78, 25);
            this.chkIsConfirmed.TabIndex = 238;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(32, 116);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(32, 17);
            this.label18.TabIndex = 237;
            this.label18.Text = "Top";
            // 
            // cmbRecordCount
            // 
            this.cmbRecordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordCount.FormattingEnabled = true;
            this.cmbRecordCount.Location = new System.Drawing.Point(126, 112);
            this.cmbRecordCount.Name = "cmbRecordCount";
            this.cmbRecordCount.Size = new System.Drawing.Size(78, 25);
            this.cmbRecordCount.TabIndex = 236;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(658, 42);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(86, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(522, 70);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(34, 17);
            this.label16.TabIndex = 218;
            this.label16.Text = "VDS";
            // 
            // cmbIsVDS
            // 
            this.cmbIsVDS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIsVDS.FormattingEnabled = true;
            this.cmbIsVDS.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbIsVDS.Location = new System.Drawing.Point(550, 67);
            this.cmbIsVDS.Name = "cmbIsVDS";
            this.cmbIsVDS.Size = new System.Drawing.Size(72, 25);
            this.cmbIsVDS.TabIndex = 217;
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Location = new System.Drawing.Point(437, 90);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(185, 25);
            this.cmbBranch.TabIndex = 215;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(359, 94);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 17);
            this.label10.TabIndex = 214;
            this.label10.Text = "Branch";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(361, 70);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 17);
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
            this.cmbActive.Location = new System.Drawing.Point(437, 67);
            this.cmbActive.Name = "cmbActive";
            this.cmbActive.Size = new System.Drawing.Size(72, 25);
            this.cmbActive.TabIndex = 211;
            this.cmbActive.SelectedIndexChanged += new System.EventHandler(this.cmbActive_SelectedIndexChanged);
            // 
            // btnSearchProductCategory
            // 
            this.btnSearchProductCategory.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchProductCategory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchProductCategory.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchProductCategory.Location = new System.Drawing.Point(317, 67);
            this.btnSearchProductCategory.Name = "btnSearchProductCategory";
            this.btnSearchProductCategory.Size = new System.Drawing.Size(30, 20);
            this.btnSearchProductCategory.TabIndex = 3;
            this.btnSearchProductCategory.TabStop = false;
            this.btnSearchProductCategory.UseVisualStyleBackColor = false;
            this.btnSearchProductCategory.Click += new System.EventHandler(this.btnSearchProductCategory_Click);
            // 
            // cmbProductCategoryName
            // 
            this.cmbProductCategoryName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProductCategoryName.FormattingEnabled = true;
            this.cmbProductCategoryName.Location = new System.Drawing.Point(126, 66);
            this.cmbProductCategoryName.Name = "cmbProductCategoryName";
            this.cmbProductCategoryName.Size = new System.Drawing.Size(185, 25);
            this.cmbProductCategoryName.Sorted = true;
            this.cmbProductCategoryName.TabIndex = 2;
            this.cmbProductCategoryName.SelectedIndexChanged += new System.EventHandler(this.cmbProductCategoryName_SelectedIndexChanged);
            this.cmbProductCategoryName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbProductCategoryName_KeyDown);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(32, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 17);
            this.label8.TabIndex = 201;
            this.label8.Text = "Code";
            // 
            // txtPCode
            // 
            this.txtPCode.Location = new System.Drawing.Point(126, 19);
            this.txtPCode.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPCode.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtPCode.Name = "txtPCode";
            this.txtPCode.Size = new System.Drawing.Size(185, 20);
            this.txtPCode.TabIndex = 0;
            this.txtPCode.TextChanged += new System.EventHandler(this.txtPCode_TextChanged);
            this.txtPCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPCode_KeyDown);
            // 
            // btnProductType
            // 
            this.btnProductType.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnProductType.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProductType.Image = global::VATClient.Properties.Resources.search;
            this.btnProductType.Location = new System.Drawing.Point(317, 91);
            this.btnProductType.Name = "btnProductType";
            this.btnProductType.Size = new System.Drawing.Size(30, 20);
            this.btnProductType.TabIndex = 5;
            this.btnProductType.TabStop = false;
            this.btnProductType.UseVisualStyleBackColor = false;
            this.btnProductType.Visible = false;
            this.btnProductType.Click += new System.EventHandler(this.btnProductType_Click);
            // 
            // cmbProductType
            // 
            this.cmbProductType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProductType.FormattingEnabled = true;
            this.cmbProductType.Location = new System.Drawing.Point(126, 88);
            this.cmbProductType.Name = "cmbProductType";
            this.cmbProductType.Size = new System.Drawing.Size(185, 25);
            this.cmbProductType.TabIndex = 3;
            this.cmbProductType.SelectedIndexChanged += new System.EventHandler(this.cmbProductType_SelectedIndexChanged);
            this.cmbProductType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbProductType_KeyDown);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(32, 91);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(92, 17);
            this.label22.TabIndex = 193;
            this.label22.Text = "Product Type";
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(747, 135);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(86, 28);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtCostPriceTo
            // 
            this.txtCostPriceTo.Location = new System.Drawing.Point(938, 88);
            this.txtCostPriceTo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCostPriceTo.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtCostPriceTo.Name = "txtCostPriceTo";
            this.txtCostPriceTo.Size = new System.Drawing.Size(65, 20);
            this.txtCostPriceTo.TabIndex = 142;
            this.txtCostPriceTo.Text = "0.00";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(776, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 17);
            this.label6.TabIndex = 134;
            this.label6.Text = "VAT Rate";
            // 
            // txtSalesPriceTo
            // 
            this.txtSalesPriceTo.Location = new System.Drawing.Point(938, 113);
            this.txtSalesPriceTo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtSalesPriceTo.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtSalesPriceTo.Name = "txtSalesPriceTo";
            this.txtSalesPriceTo.Size = new System.Drawing.Size(65, 20);
            this.txtSalesPriceTo.TabIndex = 141;
            this.txtSalesPriceTo.Text = "0.00";
            // 
            // txtNBRPriceTo
            // 
            this.txtNBRPriceTo.Location = new System.Drawing.Point(938, 138);
            this.txtNBRPriceTo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtNBRPriceTo.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtNBRPriceTo.Name = "txtNBRPriceTo";
            this.txtNBRPriceTo.Size = new System.Drawing.Size(65, 20);
            this.txtNBRPriceTo.TabIndex = 140;
            this.txtNBRPriceTo.Text = "0.00";
            // 
            // txtCostPriceFrom
            // 
            this.txtCostPriceFrom.Location = new System.Drawing.Point(818, 88);
            this.txtCostPriceFrom.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCostPriceFrom.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtCostPriceFrom.Name = "txtCostPriceFrom";
            this.txtCostPriceFrom.Size = new System.Drawing.Size(65, 20);
            this.txtCostPriceFrom.TabIndex = 138;
            this.txtCostPriceFrom.Text = "0.00";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(126, 42);
            this.txtProductName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtProductName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new System.Drawing.Size(185, 20);
            this.txtProductName.TabIndex = 1;
            this.txtProductName.TextChanged += new System.EventHandler(this.txtProductName_TextChanged);
            this.txtProductName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProductName_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(757, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 17);
            this.label1.TabIndex = 139;
            this.label1.Text = "Cost Price";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(32, 45);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(96, 17);
            this.label17.TabIndex = 124;
            this.label17.Text = "Product Name";
            // 
            // txtSalesPriceFrom
            // 
            this.txtSalesPriceFrom.Location = new System.Drawing.Point(818, 113);
            this.txtSalesPriceFrom.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtSalesPriceFrom.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtSalesPriceFrom.Name = "txtSalesPriceFrom";
            this.txtSalesPriceFrom.Size = new System.Drawing.Size(65, 20);
            this.txtSalesPriceFrom.TabIndex = 136;
            this.txtSalesPriceFrom.Text = "0.00";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(757, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 17);
            this.label3.TabIndex = 137;
            this.label3.Text = "Sales Price";
            // 
            // txtNBRPriceFrom
            // 
            this.txtNBRPriceFrom.Location = new System.Drawing.Point(818, 138);
            this.txtNBRPriceFrom.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtNBRPriceFrom.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtNBRPriceFrom.Name = "txtNBRPriceFrom";
            this.txtNBRPriceFrom.Size = new System.Drawing.Size(65, 20);
            this.txtNBRPriceFrom.TabIndex = 135;
            this.txtNBRPriceFrom.Text = "0.00";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(361, 22);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(49, 17);
            this.label13.TabIndex = 116;
            this.label13.Text = "Ref No";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(757, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 17);
            this.label5.TabIndex = 134;
            this.label5.Text = "NRB Price";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(361, 45);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(61, 17);
            this.label12.TabIndex = 114;
            this.label12.Text = "HS Code";
            // 
            // btnStockUpdate
            // 
            this.btnStockUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnStockUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStockUpdate.Location = new System.Drawing.Point(746, 70);
            this.btnStockUpdate.Name = "btnStockUpdate";
            this.btnStockUpdate.Size = new System.Drawing.Size(86, 28);
            this.btnStockUpdate.TabIndex = 7;
            this.btnStockUpdate.Text = "Stock Update";
            this.btnStockUpdate.UseVisualStyleBackColor = false;
            this.btnStockUpdate.Click += new System.EventHandler(this.btnStockUpdate_Click);
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.Location = new System.Drawing.Point(437, 19);
            this.txtSerialNo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtSerialNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new System.Drawing.Size(185, 20);
            this.txtSerialNo.TabIndex = 4;
            this.txtSerialNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSerialNo_KeyDown);
            // 
            // txtHSCodeNo
            // 
            this.txtHSCodeNo.Location = new System.Drawing.Point(437, 42);
            this.txtHSCodeNo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtHSCodeNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtHSCodeNo.Name = "txtHSCodeNo";
            this.txtHSCodeNo.Size = new System.Drawing.Size(185, 20);
            this.txtHSCodeNo.TabIndex = 5;
            this.txtHSCodeNo.TextChanged += new System.EventHandler(this.txtHSCodeNo_TextChanged);
            this.txtHSCodeNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtHSCodeNo_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 17);
            this.label4.TabIndex = 103;
            this.label4.Text = "Product Group";
            // 
            // txtCategoryID
            // 
            this.txtCategoryID.Location = new System.Drawing.Point(3, 73);
            this.txtCategoryID.Name = "txtCategoryID";
            this.txtCategoryID.Size = new System.Drawing.Size(40, 24);
            this.txtCategoryID.TabIndex = 9;
            this.txtCategoryID.Visible = false;
            this.txtCategoryID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCategoryID_KeyDown);
            // 
            // txtCategoryName
            // 
            this.txtCategoryName.Location = new System.Drawing.Point(49, 73);
            this.txtCategoryName.Name = "txtCategoryName";
            this.txtCategoryName.Size = new System.Drawing.Size(40, 24);
            this.txtCategoryName.TabIndex = 10;
            this.txtCategoryName.Visible = false;
            this.txtCategoryName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCategoryName_KeyDown);
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(717, 97);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.Size = new System.Drawing.Size(20, 20);
            this.txtItemNo.TabIndex = 0;
            this.txtItemNo.Visible = false;
            this.txtItemNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtItemNo_KeyDown);
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLoad.Image = global::VATClient.Properties.Resources.search;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(3, 23);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 28);
            this.btnLoad.TabIndex = 220;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Visible = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // chkVAT16
            // 
            this.chkVAT16.AutoSize = true;
            this.chkVAT16.Location = new System.Drawing.Point(3, 54);
            this.chkVAT16.Name = "chkVAT16";
            this.chkVAT16.Size = new System.Drawing.Size(74, 21);
            this.chkVAT16.TabIndex = 219;
            this.chkVAT16.Text = "VAT 17";
            this.chkVAT16.UseVisualStyleBackColor = true;
            this.chkVAT16.Visible = false;
            this.chkVAT16.CheckedChanged += new System.EventHandler(this.chkVAT16_CheckedChanged);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(9, -1);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(78, 21);
            this.chkSelectAll.TabIndex = 214;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged_1);
            // 
            // cmbImport
            // 
            this.cmbImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbImport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImport.FormattingEnabled = true;
            this.cmbImport.Items.AddRange(new object[] {
            "Excel",
            "Text"});
            this.cmbImport.Location = new System.Drawing.Point(212, 11);
            this.cmbImport.Name = "cmbImport";
            this.cmbImport.Size = new System.Drawing.Size(80, 25);
            this.cmbImport.TabIndex = 216;
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(300, 9);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 28);
            this.btnExport.TabIndex = 213;
            this.btnExport.Text = "Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(245, 130);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 30);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 199;
            this.progressBar1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(69, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 17);
            this.label2.TabIndex = 102;
            this.label2.Text = "Item No";
            this.label2.Visible = false;
            // 
            // txtTradingMarkUp
            // 
            this.txtTradingMarkUp.Location = new System.Drawing.Point(892, 178);
            this.txtTradingMarkUp.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtTradingMarkUp.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtTradingMarkUp.Name = "txtTradingMarkUp";
            this.txtTradingMarkUp.Size = new System.Drawing.Size(65, 20);
            this.txtTradingMarkUp.TabIndex = 198;
            this.txtTradingMarkUp.Text = "0.00";
            this.txtTradingMarkUp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTradingMarkUp.Visible = false;
            // 
            // txtPacketPrice
            // 
            this.txtPacketPrice.Location = new System.Drawing.Point(821, 178);
            this.txtPacketPrice.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPacketPrice.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtPacketPrice.Name = "txtPacketPrice";
            this.txtPacketPrice.Size = new System.Drawing.Size(65, 20);
            this.txtPacketPrice.TabIndex = 197;
            this.txtPacketPrice.Text = "0.00";
            this.txtPacketPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPacketPrice.Visible = false;
            // 
            // txtSDRate
            // 
            this.txtSDRate.Location = new System.Drawing.Point(793, 176);
            this.txtSDRate.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtSDRate.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtSDRate.Name = "txtSDRate";
            this.txtSDRate.Size = new System.Drawing.Size(65, 20);
            this.txtSDRate.TabIndex = 196;
            this.txtSDRate.Text = "0.00";
            this.txtSDRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSDRate.Visible = false;
            // 
            // txtVATRateTo
            // 
            this.txtVATRateTo.Location = new System.Drawing.Point(795, 180);
            this.txtVATRateTo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtVATRateTo.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtVATRateTo.Name = "txtVATRateTo";
            this.txtVATRateTo.Size = new System.Drawing.Size(65, 20);
            this.txtVATRateTo.TabIndex = 7;
            this.txtVATRateTo.Text = "0.00";
            this.txtVATRateTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVATRateTo.Visible = false;
            this.txtVATRateTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVATRateTo_KeyDown);
            this.txtVATRateTo.Leave += new System.EventHandler(this.txtVATRateTo_Leave);
            // 
            // txtVATRateFrom
            // 
            this.txtVATRateFrom.Location = new System.Drawing.Point(805, 180);
            this.txtVATRateFrom.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtVATRateFrom.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtVATRateFrom.Name = "txtVATRateFrom";
            this.txtVATRateFrom.Size = new System.Drawing.Size(65, 20);
            this.txtVATRateFrom.TabIndex = 6;
            this.txtVATRateFrom.Text = "0.00";
            this.txtVATRateFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVATRateFrom.Visible = false;
            this.txtVATRateFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVATRateFrom_KeyDown);
            this.txtVATRateFrom.Leave += new System.EventHandler(this.txtVATRateFrom_Leave);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(686, 183);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 17);
            this.label9.TabIndex = 108;
            this.label9.Text = "VAT Rate";
            this.label9.Visible = false;
            // 
            // txtVATRate
            // 
            this.txtVATRate.Location = new System.Drawing.Point(824, 272);
            this.txtVATRate.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtVATRate.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVATRate.Name = "txtVATRate";
            this.txtVATRate.Size = new System.Drawing.Size(185, 20);
            this.txtVATRate.TabIndex = 111;
            this.txtVATRate.Text = "0.00";
            // 
            // txtCostPrice
            // 
            this.txtCostPrice.Location = new System.Drawing.Point(882, 82);
            this.txtCostPrice.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCostPrice.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtCostPrice.Name = "txtCostPrice";
            this.txtCostPrice.Size = new System.Drawing.Size(85, 20);
            this.txtCostPrice.TabIndex = 119;
            this.txtCostPrice.Text = "0.00";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(821, 85);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(69, 17);
            this.label15.TabIndex = 120;
            this.label15.Text = "Cost Price";
            // 
            // txtSalesPrice
            // 
            this.txtSalesPrice.Location = new System.Drawing.Point(882, 107);
            this.txtSalesPrice.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtSalesPrice.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtSalesPrice.Name = "txtSalesPrice";
            this.txtSalesPrice.Size = new System.Drawing.Size(85, 20);
            this.txtSalesPrice.TabIndex = 117;
            this.txtSalesPrice.Text = "0.00";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(821, 110);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 17);
            this.label14.TabIndex = 118;
            this.label14.Text = "Sales Price";
            // 
            // txtNRBPrice
            // 
            this.txtNRBPrice.Location = new System.Drawing.Point(882, 132);
            this.txtNRBPrice.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtNRBPrice.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtNRBPrice.Name = "txtNRBPrice";
            this.txtNRBPrice.Size = new System.Drawing.Size(85, 20);
            this.txtNRBPrice.TabIndex = 115;
            this.txtNRBPrice.Text = "0.00";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(821, 135);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 17);
            this.label7.TabIndex = 105;
            this.label7.Text = "NRB Price";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnPHistoryExport);
            this.panel1.Controls.Add(this.btnProductStockExport);
            this.panel1.Controls.Add(this.btnTradingExport);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Controls.Add(this.cmbImport);
            this.panel1.Location = new System.Drawing.Point(-1, 421);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(767, 40);
            this.panel1.TabIndex = 13;
            // 
            // btnPHistoryExport
            // 
            this.btnPHistoryExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPHistoryExport.BackColor = System.Drawing.Color.White;
            this.btnPHistoryExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPHistoryExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnPHistoryExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPHistoryExport.Location = new System.Drawing.Point(581, 9);
            this.btnPHistoryExport.Name = "btnPHistoryExport";
            this.btnPHistoryExport.Size = new System.Drawing.Size(91, 28);
            this.btnPHistoryExport.TabIndex = 224;
            this.btnPHistoryExport.Text = "PHistory";
            this.btnPHistoryExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPHistoryExport.UseVisualStyleBackColor = false;
            this.btnPHistoryExport.Click += new System.EventHandler(this.btnPHistoryExport_Click);
            // 
            // btnProductStockExport
            // 
            this.btnProductStockExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProductStockExport.BackColor = System.Drawing.Color.White;
            this.btnProductStockExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProductStockExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnProductStockExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnProductStockExport.Location = new System.Drawing.Point(486, 9);
            this.btnProductStockExport.Name = "btnProductStockExport";
            this.btnProductStockExport.Size = new System.Drawing.Size(91, 28);
            this.btnProductStockExport.TabIndex = 223;
            this.btnProductStockExport.Text = "StockExport";
            this.btnProductStockExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnProductStockExport.UseVisualStyleBackColor = false;
            this.btnProductStockExport.Click += new System.EventHandler(this.btnProductStockExport_Click);
            // 
            // btnTradingExport
            // 
            this.btnTradingExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTradingExport.BackColor = System.Drawing.Color.White;
            this.btnTradingExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTradingExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnTradingExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTradingExport.Location = new System.Drawing.Point(381, 9);
            this.btnTradingExport.Name = "btnTradingExport";
            this.btnTradingExport.Size = new System.Drawing.Size(101, 28);
            this.btnTradingExport.TabIndex = 222;
            this.btnTradingExport.Text = "Trading Export";
            this.btnTradingExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTradingExport.UseVisualStyleBackColor = false;
            this.btnTradingExport.Click += new System.EventHandler(this.btnTradingExport_Click);
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(21, 13);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(121, 21);
            this.LRecordCount.TabIndex = 221;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(683, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkSelectAll);
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.dgvProduct);
            this.groupBox2.Controls.Add(this.pnlHidden);
            this.groupBox2.Location = new System.Drawing.Point(18, 173);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(748, 242);
            this.groupBox2.TabIndex = 133;
            this.groupBox2.TabStop = false;
            // 
            // dgvProduct
            // 
            this.dgvProduct.AllowUserToAddRows = false;
            this.dgvProduct.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvProduct.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvProduct.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProduct.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.dgvProduct.Location = new System.Drawing.Point(3, 20);
            this.dgvProduct.Name = "dgvProduct";
            this.dgvProduct.RowHeadersVisible = false;
            this.dgvProduct.RowTemplate.Height = 24;
            this.dgvProduct.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProduct.Size = new System.Drawing.Size(740, 216);
            this.dgvProduct.TabIndex = 0;
            this.dgvProduct.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProduct_CellContentClick);
            this.dgvProduct.DoubleClick += new System.EventHandler(this.dgvProduct_DoubleClick);
            this.dgvProduct.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvProduct_KeyDown);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // pnlHidden
            // 
            this.pnlHidden.Controls.Add(this.txtItemNo);
            this.pnlHidden.Controls.Add(this.btnLoad);
            this.pnlHidden.Controls.Add(this.chkVAT16);
            this.pnlHidden.Controls.Add(this.label2);
            this.pnlHidden.Controls.Add(this.txtCategoryID);
            this.pnlHidden.Controls.Add(this.txtCategoryName);
            this.pnlHidden.Location = new System.Drawing.Point(6, 131);
            this.pnlHidden.Name = "pnlHidden";
            this.pnlHidden.Size = new System.Drawing.Size(200, 100);
            this.pnlHidden.TabIndex = 200;
            this.pnlHidden.Visible = false;
            // 
            // cmbUOM
            // 
            this.cmbUOM.FormattingEnabled = true;
            this.cmbUOM.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbUOM.Location = new System.Drawing.Point(795, 166);
            this.cmbUOM.Name = "cmbUOM";
            this.cmbUOM.Size = new System.Drawing.Size(65, 25);
            this.cmbUOM.TabIndex = 133;
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // backgroundWorkerPTypeSearch
            // 
            this.backgroundWorkerPTypeSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPTypeSearch_DoWork);
            this.backgroundWorkerPTypeSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPTypeSearch_RunWorkerCompleted);
            // 
            // backgroundWorkerProductCategorySearch
            // 
            this.backgroundWorkerProductCategorySearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerProductCategorySearch_DoWork);
            this.backgroundWorkerProductCategorySearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerProductCategorySearch_RunWorkerCompleted);
            // 
            // bgwVATProduct
            // 
            this.bgwVATProduct.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwVATProduct_DoWork);
            this.bgwVATProduct.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwVATProduct_RunWorkerCompleted);
            // 
            // bgwAvgPrice
            // 
            this.bgwAvgPrice.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwAvgPrice_DoWork);
            this.bgwAvgPrice.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwAvgPrice_RunWorkerCompleted);
            // 
            // bgwStockUpdate
            // 
            this.bgwStockUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwStockUpdate_DoWork);
            this.bgwStockUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwStockUpdate_RunWorkerCompleted);
            // 
            // FormProductSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(782, 453);
            this.Controls.Add(this.cmbUOM);
            this.Controls.Add(this.txtTradingMarkUp);
            this.Controls.Add(this.txtPacketPrice);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtSDRate);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtVATRateTo);
            this.Controls.Add(this.cmbProduct);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtCostPrice);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txtVATRateFrom);
            this.Controls.Add(this.txtSalesPrice);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtNRBPrice);
            this.Controls.Add(this.txtVATRate);
            this.Controls.Add(this.label9);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormProductSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Product Search";
            this.Load += new System.EventHandler(this.FormProductSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProduct)).EndInit();
            this.pnlHidden.ResumeLayout(false);
            this.pnlHidden.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cmbProduct;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtCategoryID;
        private System.Windows.Forms.TextBox txtCostPrice;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtSalesPrice;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtNRBPrice;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtVATRate;
        private System.Windows.Forms.TextBox txtSerialNo;
        private System.Windows.Forms.TextBox txtCategoryName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtHSCodeNo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbUOM;
        private System.Windows.Forms.TextBox txtCostPriceTo;
        private System.Windows.Forms.TextBox txtSalesPriceTo;
        private System.Windows.Forms.TextBox txtNBRPriceTo;
        private System.Windows.Forms.TextBox txtCostPriceFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSalesPriceFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNBRPriceFrom;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtVATRateTo;
        private System.Windows.Forms.TextBox txtVATRateFrom;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbProductType;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button btnProductType;
        private System.Windows.Forms.TextBox txtPacketPrice;
        private System.Windows.Forms.TextBox txtSDRate;
        private System.Windows.Forms.TextBox txtTradingMarkUp;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtPCode;
        private System.Windows.Forms.DataGridView dgvProduct;
        private System.Windows.Forms.ComboBox cmbProductCategoryName;
        private System.Windows.Forms.Button btnSearchProductCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn PCode;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPTypeSearch;
        private System.ComponentModel.BackgroundWorker backgroundWorkerProductCategorySearch;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbActive;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmbImport;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cmbIsVDS;
        private System.Windows.Forms.CheckBox chkVAT16;
        private System.Windows.Forms.Button btnLoad;
        private System.ComponentModel.BackgroundWorker bgwVATProduct;
        public System.Windows.Forms.ComboBox cmbRecordCount;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Panel pnlHidden;
        private System.Windows.Forms.Label label19;
        public System.Windows.Forms.ComboBox chkIsConfirmed;
        public System.Windows.Forms.Button btnAvgPrice;
        private System.ComponentModel.BackgroundWorker bgwAvgPrice;
        private System.Windows.Forms.Button btnStockUpdate;
        private System.ComponentModel.BackgroundWorker bgwStockUpdate;
        private System.Windows.Forms.Button btnTradingExport;
        private System.Windows.Forms.Button btnBOMExport;
        private System.Windows.Forms.Button btnProductStockExport;
        private System.Windows.Forms.Button btnPHistoryExport;
    }
}