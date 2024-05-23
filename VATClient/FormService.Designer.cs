namespace VATClient
{
    partial class FormService
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormService));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtPCode = new System.Windows.Forms.TextBox();
            this.txtUOM = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.label26 = new System.Windows.Forms.Label();
            this.dtpEffectDate = new System.Windows.Forms.DateTimePicker();
            this.chkPCode = new System.Windows.Forms.CheckBox();
            this.label23 = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.cmbProduct = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtHSCode = new System.Windows.Forms.TextBox();
            this.txtCategoryName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtProductId = new System.Windows.Forms.TextBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnChange = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dgvBOM = new System.Windows.Forms.DataGridView();
            this.LineNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProductName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UOM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BasePrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Other = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OtherAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SDRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SDAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VATRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VATAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SalePrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HSCodeNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EffectDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BOMId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtSD = new System.Windows.Forms.TextBox();
            this.txtUnitCost = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.cmbProductType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.txtOther = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSalePrice = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtVATRate = new System.Windows.Forms.TextBox();
            this.chkOther = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerSearchDetails = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerSave = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerProductSearchDs = new System.ComponentModel.BackgroundWorker();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdPost = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.bgwPost = new System.ComponentModel.BackgroundWorker();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBOM)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtPCode);
            this.groupBox5.Controls.Add(this.txtUOM);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.label35);
            this.groupBox5.Controls.Add(this.button3);
            this.groupBox5.Controls.Add(this.label26);
            this.groupBox5.Controls.Add(this.dtpEffectDate);
            this.groupBox5.Controls.Add(this.chkPCode);
            this.groupBox5.Controls.Add(this.label23);
            this.groupBox5.Controls.Add(this.txtProductName);
            this.groupBox5.Controls.Add(this.cmbProduct);
            this.groupBox5.Controls.Add(this.label18);
            this.groupBox5.Controls.Add(this.label17);
            this.groupBox5.Controls.Add(this.txtHSCode);
            this.groupBox5.Controls.Add(this.txtCategoryName);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.ForeColor = System.Drawing.Color.Black;
            this.groupBox5.Location = new System.Drawing.Point(100, 1);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(546, 116);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Item";
            // 
            // txtPCode
            // 
            this.txtPCode.Location = new System.Drawing.Point(332, 41);
            this.txtPCode.Name = "txtPCode";
            this.txtPCode.ReadOnly = true;
            this.txtPCode.Size = new System.Drawing.Size(200, 20);
            this.txtPCode.TabIndex = 7;
            this.txtPCode.TabStop = false;
            // 
            // txtUOM
            // 
            this.txtUOM.Location = new System.Drawing.Point(332, 67);
            this.txtUOM.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtUOM.MinimumSize = new System.Drawing.Size(200, 20);
            this.txtUOM.Name = "txtUOM";
            this.txtUOM.ReadOnly = true;
            this.txtUOM.Size = new System.Drawing.Size(200, 20);
            this.txtUOM.TabIndex = 8;
            this.txtUOM.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(284, 70);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 13);
            this.label11.TabIndex = 213;
            this.label11.Text = "UOM";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(284, 42);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(32, 13);
            this.label35.TabIndex = 208;
            this.label35.Text = "Code";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.Location = new System.Drawing.Point(245, 14);
            this.button3.MaximumSize = new System.Drawing.Size(21, 21);
            this.button3.MinimumSize = new System.Drawing.Size(21, 21);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(21, 21);
            this.button3.TabIndex = 1;
            this.button3.TabStop = false;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(6, 41);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(35, 13);
            this.label26.TabIndex = 200;
            this.label26.Text = "Name";
            // 
            // dtpEffectDate
            // 
            this.dtpEffectDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpEffectDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEffectDate.Location = new System.Drawing.Point(65, 90);
            this.dtpEffectDate.MaxDate = new System.DateTime(9900, 1, 1, 0, 0, 0, 0);
            this.dtpEffectDate.MaximumSize = new System.Drawing.Size(4, 20);
            this.dtpEffectDate.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpEffectDate.MinimumSize = new System.Drawing.Size(185, 20);
            this.dtpEffectDate.Name = "dtpEffectDate";
            this.dtpEffectDate.Size = new System.Drawing.Size(185, 20);
            this.dtpEffectDate.TabIndex = 4;
            this.dtpEffectDate.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpEffectDate.ValueChanged += new System.EventHandler(this.dtpEffectDate_ValueChanged);
            this.dtpEffectDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpEffectDate_KeyDown);
            // 
            // chkPCode
            // 
            this.chkPCode.AutoSize = true;
            this.chkPCode.Checked = true;
            this.chkPCode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPCode.Location = new System.Drawing.Point(496, 19);
            this.chkPCode.Name = "chkPCode";
            this.chkPCode.Size = new System.Drawing.Size(15, 14);
            this.chkPCode.TabIndex = 6;
            this.chkPCode.TabStop = false;
            this.chkPCode.UseVisualStyleBackColor = true;
            this.chkPCode.CheckedChanged += new System.EventHandler(this.chkPCode_CheckedChanged);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(5, 92);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(61, 13);
            this.label23.TabIndex = 177;
            this.label23.Text = "Effect Date";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(65, 40);
            this.txtProductName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtProductName.MinimumSize = new System.Drawing.Size(200, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(200, 20);
            this.txtProductName.TabIndex = 2;
            this.txtProductName.TabStop = false;
            // 
            // cmbProduct
            // 
            this.cmbProduct.FormattingEnabled = true;
            this.cmbProduct.Location = new System.Drawing.Point(332, 14);
            this.cmbProduct.Name = "cmbProduct";
            this.cmbProduct.Size = new System.Drawing.Size(160, 21);
            this.cmbProduct.Sorted = true;
            this.cmbProduct.TabIndex = 5;
            this.cmbProduct.SelectedIndexChanged += new System.EventHandler(this.cmbProduct_SelectedIndexChanged);
            this.cmbProduct.Click += new System.EventHandler(this.cmbProduct_Click);
            this.cmbProduct.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbProduct_KeyDown);
            this.cmbProduct.Layout += new System.Windows.Forms.LayoutEventHandler(this.cmbProduct_Layout);
            this.cmbProduct.Leave += new System.EventHandler(this.cmbProduct_Leave);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 67);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(50, 13);
            this.label18.TabIndex = 168;
            this.label18.Text = "HS Code";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 16);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(36, 13);
            this.label17.TabIndex = 167;
            this.label17.Text = "Group";
            // 
            // txtHSCode
            // 
            this.txtHSCode.Location = new System.Drawing.Point(65, 66);
            this.txtHSCode.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtHSCode.MinimumSize = new System.Drawing.Size(200, 20);
            this.txtHSCode.Name = "txtHSCode";
            this.txtHSCode.ReadOnly = true;
            this.txtHSCode.Size = new System.Drawing.Size(200, 20);
            this.txtHSCode.TabIndex = 3;
            this.txtHSCode.TabStop = false;
            // 
            // txtCategoryName
            // 
            this.txtCategoryName.Location = new System.Drawing.Point(65, 14);
            this.txtCategoryName.MaximumSize = new System.Drawing.Size(180, 20);
            this.txtCategoryName.MinimumSize = new System.Drawing.Size(180, 20);
            this.txtCategoryName.Name = "txtCategoryName";
            this.txtCategoryName.ReadOnly = true;
            this.txtCategoryName.Size = new System.Drawing.Size(180, 20);
            this.txtCategoryName.TabIndex = 0;
            this.txtCategoryName.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(284, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 164;
            this.label5.Text = "Search";
            // 
            // txtProductId
            // 
            this.txtProductId.Location = new System.Drawing.Point(673, 97);
            this.txtProductId.Name = "txtProductId";
            this.txtProductId.ReadOnly = true;
            this.txtProductId.Size = new System.Drawing.Size(58, 20);
            this.txtProductId.TabIndex = 10;
            this.txtProductId.TabStop = false;
            this.txtProductId.Visible = false;
            this.txtProductId.TextChanged += new System.EventHandler(this.txtProductId_TextChanged);
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRemove.Image = global::VATClient.Properties.Resources.Remove;
            this.btnRemove.Location = new System.Drawing.Point(689, 154);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(45, 28);
            this.btnRemove.TabIndex = 20;
            this.btnRemove.TabStop = false;
            this.btnRemove.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Visible = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnChange
            // 
            this.btnChange.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnChange.Image = ((System.Drawing.Image)(resources.GetObject("btnChange.Image")));
            this.btnChange.Location = new System.Drawing.Point(636, 154);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(45, 28);
            this.btnChange.TabIndex = 19;
            this.btnChange.TabStop = false;
            this.btnChange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnChange.UseVisualStyleBackColor = false;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.Location = new System.Drawing.Point(583, 154);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(45, 28);
            this.btnAdd.TabIndex = 18;
            this.btnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnAdd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnAdd_KeyDown);
            // 
            // dgvBOM
            // 
            this.dgvBOM.AllowUserToAddRows = false;
            this.dgvBOM.AllowUserToDeleteRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Blue;
            this.dgvBOM.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvBOM.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBOM.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvBOM.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBOM.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LineNo,
            this.ItemNo,
            this.PCode,
            this.ProductName,
            this.UOM,
            this.BasePrice,
            this.Other,
            this.OtherAmount,
            this.SDRate,
            this.SDAmount,
            this.VATRate,
            this.VATAmount,
            this.SalePrice,
            this.Comment,
            this.HSCodeNo,
            this.EffectDate,
            this.BOMId});
            this.dgvBOM.GridColor = System.Drawing.SystemColors.HotTrack;
            this.dgvBOM.Location = new System.Drawing.Point(9, 188);
            this.dgvBOM.Name = "dgvBOM";
            this.dgvBOM.ReadOnly = true;
            this.dgvBOM.RowHeadersVisible = false;
            this.dgvBOM.Size = new System.Drawing.Size(730, 188);
            this.dgvBOM.TabIndex = 207;
            this.dgvBOM.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBOM_CellContentClick);
            this.dgvBOM.DoubleClick += new System.EventHandler(this.dgvBOM_DoubleClick);
            // 
            // LineNo
            // 
            this.LineNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.LineNo.HeaderText = "SL#";
            this.LineNo.Name = "LineNo";
            this.LineNo.ReadOnly = true;
            this.LineNo.Width = 51;
            // 
            // ItemNo
            // 
            this.ItemNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ItemNo.HeaderText = "Item No";
            this.ItemNo.Name = "ItemNo";
            this.ItemNo.ReadOnly = true;
            this.ItemNo.Visible = false;
            // 
            // PCode
            // 
            this.PCode.HeaderText = "Code";
            this.PCode.Name = "PCode";
            this.PCode.ReadOnly = true;
            // 
            // ProductName
            // 
            this.ProductName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ProductName.HeaderText = "Name";
            this.ProductName.Name = "ProductName";
            this.ProductName.ReadOnly = true;
            this.ProductName.Width = 140;
            // 
            // UOM
            // 
            this.UOM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.UOM.HeaderText = "UOM";
            this.UOM.Name = "UOM";
            this.UOM.ReadOnly = true;
            this.UOM.Width = 55;
            // 
            // BasePrice
            // 
            this.BasePrice.HeaderText = "BasePrice";
            this.BasePrice.Name = "BasePrice";
            this.BasePrice.ReadOnly = true;
            // 
            // Other
            // 
            this.Other.HeaderText = "Other";
            this.Other.Name = "Other";
            this.Other.ReadOnly = true;
            // 
            // OtherAmount
            // 
            this.OtherAmount.HeaderText = "OtherAmount";
            this.OtherAmount.Name = "OtherAmount";
            this.OtherAmount.ReadOnly = true;
            // 
            // SDRate
            // 
            this.SDRate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.SDRate.DefaultCellStyle = dataGridViewCellStyle7;
            this.SDRate.HeaderText = "SDRate";
            this.SDRate.Name = "SDRate";
            this.SDRate.ReadOnly = true;
            // 
            // SDAmount
            // 
            this.SDAmount.HeaderText = "SDAmount";
            this.SDAmount.Name = "SDAmount";
            this.SDAmount.ReadOnly = true;
            // 
            // VATRate
            // 
            this.VATRate.HeaderText = "VATRate";
            this.VATRate.Name = "VATRate";
            this.VATRate.ReadOnly = true;
            // 
            // VATAmount
            // 
            this.VATAmount.HeaderText = "VATAmount";
            this.VATAmount.Name = "VATAmount";
            this.VATAmount.ReadOnly = true;
            // 
            // SalePrice
            // 
            this.SalePrice.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.SalePrice.DefaultCellStyle = dataGridViewCellStyle8;
            this.SalePrice.HeaderText = "SalePrice";
            this.SalePrice.Name = "SalePrice";
            this.SalePrice.ReadOnly = true;
            // 
            // Comment
            // 
            this.Comment.HeaderText = "Comment";
            this.Comment.Name = "Comment";
            this.Comment.ReadOnly = true;
            // 
            // HSCodeNo
            // 
            this.HSCodeNo.HeaderText = "HSCodeNo";
            this.HSCodeNo.Name = "HSCodeNo";
            this.HSCodeNo.ReadOnly = true;
            this.HSCodeNo.Visible = false;
            // 
            // EffectDate
            // 
            this.EffectDate.HeaderText = "EffectDate";
            this.EffectDate.Name = "EffectDate";
            this.EffectDate.ReadOnly = true;
            // 
            // BOMId
            // 
            this.BOMId.HeaderText = "BOMId";
            this.BOMId.Name = "BOMId";
            this.BOMId.ReadOnly = true;
            // 
            // txtSD
            // 
            this.txtSD.Location = new System.Drawing.Point(371, 126);
            this.txtSD.MaximumSize = new System.Drawing.Size(80, 20);
            this.txtSD.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtSD.Name = "txtSD";
            this.txtSD.ReadOnly = true;
            this.txtSD.Size = new System.Drawing.Size(50, 20);
            this.txtSD.TabIndex = 14;
            this.txtSD.TabStop = false;
            this.txtSD.Text = "0.00";
            this.txtSD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSD.TextChanged += new System.EventHandler(this.txtSD_TextChanged);
            this.txtSD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSD_KeyDown);
            this.txtSD.Leave += new System.EventHandler(this.txtSD_Leave);
            // 
            // txtUnitCost
            // 
            this.txtUnitCost.Location = new System.Drawing.Point(76, 125);
            this.txtUnitCost.MaximumSize = new System.Drawing.Size(100, 20);
            this.txtUnitCost.MinimumSize = new System.Drawing.Size(100, 20);
            this.txtUnitCost.Name = "txtUnitCost";
            this.txtUnitCost.Size = new System.Drawing.Size(100, 20);
            this.txtUnitCost.TabIndex = 11;
            this.txtUnitCost.Text = "0.00";
            this.txtUnitCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtUnitCost.TextChanged += new System.EventHandler(this.txtUnitCost_TextChanged);
            this.txtUnitCost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUnitCost_KeyDown);
            this.txtUnitCost.Leave += new System.EventHandler(this.txtUnitCost_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(334, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 212;
            this.label1.Text = "SD(%)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 213;
            this.label2.Text = "Base Price";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSave.Image = global::VATClient.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(3, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 28);
            this.btnSave.TabIndex = 22;
            this.btnSave.Text = "&Add";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = ((System.Drawing.Image)(resources.GetObject("btnPreview.Image")));
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(422, 6);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 26;
            this.btnPreview.Text = "Print";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(336, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 25;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(655, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 27;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // cmbProductType
            // 
            this.cmbProductType.Enabled = false;
            this.cmbProductType.FormattingEnabled = true;
            this.cmbProductType.Location = new System.Drawing.Point(467, 391);
            this.cmbProductType.Name = "cmbProductType";
            this.cmbProductType.Size = new System.Drawing.Size(185, 21);
            this.cmbProductType.TabIndex = 220;
            this.cmbProductType.TabStop = false;
            this.cmbProductType.Text = "Finish";
            this.cmbProductType.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 222;
            this.label4.Text = "Comment";
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(75, 153);
            this.txtComments.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtComments.MinimumSize = new System.Drawing.Size(350, 20);
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(350, 20);
            this.txtComments.TabIndex = 17;
            this.txtComments.TabStop = false;
            this.txtComments.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtComments.TextChanged += new System.EventHandler(this.txtComments_TextChanged);
            this.txtComments.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtComments_KeyDown);
            // 
            // txtOther
            // 
            this.txtOther.Location = new System.Drawing.Point(247, 125);
            this.txtOther.MaximumSize = new System.Drawing.Size(80, 20);
            this.txtOther.MinimumSize = new System.Drawing.Size(80, 20);
            this.txtOther.Name = "txtOther";
            this.txtOther.Size = new System.Drawing.Size(80, 20);
            this.txtOther.TabIndex = 13;
            this.txtOther.Text = "0.00";
            this.txtOther.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtOther.TextChanged += new System.EventHandler(this.txtOther_TextChanged);
            this.txtOther.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtOther_KeyDown);
            this.txtOther.Leave += new System.EventHandler(this.txtOther_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(541, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 228;
            this.label3.Text = "Sale Price";
            // 
            // txtSalePrice
            // 
            this.txtSalePrice.Location = new System.Drawing.Point(601, 127);
            this.txtSalePrice.MaximumSize = new System.Drawing.Size(100, 20);
            this.txtSalePrice.MinimumSize = new System.Drawing.Size(100, 20);
            this.txtSalePrice.Name = "txtSalePrice";
            this.txtSalePrice.Size = new System.Drawing.Size(100, 20);
            this.txtSalePrice.TabIndex = 16;
            this.txtSalePrice.Text = "0.00";
            this.txtSalePrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSalePrice.TextChanged += new System.EventHandler(this.txtSalePrice_TextChanged);
            this.txtSalePrice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSalePrice_KeyDown);
            this.txtSalePrice.Leave += new System.EventHandler(this.txtSalePrice_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(423, 129);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 230;
            this.label6.Text = "VAT(%)";
            // 
            // txtVATRate
            // 
            this.txtVATRate.Location = new System.Drawing.Point(465, 126);
            this.txtVATRate.MaximumSize = new System.Drawing.Size(80, 20);
            this.txtVATRate.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtVATRate.Name = "txtVATRate";
            this.txtVATRate.ReadOnly = true;
            this.txtVATRate.Size = new System.Drawing.Size(50, 20);
            this.txtVATRate.TabIndex = 15;
            this.txtVATRate.TabStop = false;
            this.txtVATRate.Text = "0.00";
            this.txtVATRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVATRate.TextChanged += new System.EventHandler(this.txtVATRate_TextChanged);
            this.txtVATRate.Leave += new System.EventHandler(this.txtVATRate_Leave);
            // 
            // chkOther
            // 
            this.chkOther.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkOther.AutoSize = true;
            this.chkOther.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkOther.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkOther.Location = new System.Drawing.Point(182, 127);
            this.chkOther.Name = "chkOther";
            this.chkOther.Size = new System.Drawing.Size(66, 17);
            this.chkOther.TabIndex = 12;
            this.chkOther.TabStop = false;
            this.chkOther.Text = "Other(%)";
            this.chkOther.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkOther.UseVisualStyleBackColor = true;
            this.chkOther.CheckedChanged += new System.EventHandler(this.chkOther_CheckedChanged);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(652, 44);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(82, 25);
            this.button2.TabIndex = 9;
            this.button2.Text = "Search";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(257, 224);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 232;
            this.progressBar1.Visible = false;
            // 
            // backgroundWorkerSearchDetails
            // 
            this.backgroundWorkerSearchDetails.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearchDetails_DoWork);
            this.backgroundWorkerSearchDetails.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearchDetails_RunWorkerCompleted);
            // 
            // backgroundWorkerSave
            // 
            this.backgroundWorkerSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSave_DoWork);
            this.backgroundWorkerSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSave_RunWorkerCompleted);
            // 
            // backgroundWorkerProductSearchDs
            // 
            this.backgroundWorkerProductSearchDs.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerProductSearchDs_DoWork);
            this.backgroundWorkerProductSearchDs.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerProductSearchDs_RunWorkerCompleted);
            // 
            // bgwUpdate
            // 
            this.bgwUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUpdate_DoWork);
            this.bgwUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUpdate_RunWorkerCompleted);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(81, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 23;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.cmdPost);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Location = new System.Drawing.Point(1, 384);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(747, 40);
            this.panel1.TabIndex = 21;
            // 
            // cmdPost
            // 
            this.cmdPost.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cmdPost.Image = global::VATClient.Properties.Resources.Post;
            this.cmdPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdPost.Location = new System.Drawing.Point(243, 7);
            this.cmdPost.Name = "cmdPost";
            this.cmdPost.Size = new System.Drawing.Size(75, 28);
            this.cmdPost.TabIndex = 24;
            this.cmdPost.Text = "&Post";
            this.cmdPost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdPost.UseVisualStyleBackColor = false;
            this.cmdPost.Click += new System.EventHandler(this.cmdPost_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(260, 56);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 28);
            this.btnPrint.TabIndex = 3;
            this.btnPrint.Text = "&Print";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDelete.Image = global::VATClient.Properties.Resources.Delete;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(162, 52);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 28);
            this.btnDelete.TabIndex = 23;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // bgwPost
            // 
            this.bgwPost.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPost_DoWork);
            this.bgwPost.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPost_RunWorkerCompleted);
            // 
            // FormService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(746, 423);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtVATRate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSalePrice);
            this.Controls.Add(this.chkOther);
            this.Controls.Add(this.txtOther);
            this.Controls.Add(this.txtComments);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbProductType);
            this.Controls.Add(this.txtProductId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtUnitCost);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.txtSD);
            this.Controls.Add(this.btnChange);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dgvBOM);
            this.Controls.Add(this.groupBox5);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(762, 462);
            this.MinimumSize = new System.Drawing.Size(762, 462);
            this.Name = "FormService";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Service Price Declaration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormService_FormClosing);
            this.Load += new System.EventHandler(this.FormService_Load);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBOM)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.DateTimePicker dtpEffectDate;
        private System.Windows.Forms.CheckBox chkPCode;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.ComboBox cmbProduct;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtProductId;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtHSCode;
        private System.Windows.Forms.TextBox txtCategoryName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnChange;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridView dgvBOM;
        private System.Windows.Forms.TextBox txtUOM;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtSD;
        private System.Windows.Forms.TextBox txtUnitCost;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ComboBox cmbProductType;
        private System.Windows.Forms.TextBox txtPCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.TextBox txtOther;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSalePrice;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtVATRate;
        private System.Windows.Forms.CheckBox chkOther;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearchDetails;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSave;
        private System.ComponentModel.BackgroundWorker backgroundWorkerProductSearchDs;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.DataGridViewTextBoxColumn LineNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn PCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn UOM;
        private System.Windows.Forms.DataGridViewTextBoxColumn BasePrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn Other;
        private System.Windows.Forms.DataGridViewTextBoxColumn OtherAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn SDRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn SDAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn VATRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn VATAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn SalePrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.DataGridViewTextBoxColumn HSCodeNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn EffectDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn BOMId;
        private System.ComponentModel.BackgroundWorker bgwPost;
        public System.Windows.Forms.Button btnSave;
        public System.Windows.Forms.Button btnUpdate;
        public System.Windows.Forms.Button cmdPost;
        public System.Windows.Forms.Button btnDelete;
    }
}