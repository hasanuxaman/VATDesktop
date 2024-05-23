namespace VATClient
{
    partial class FormBanderolProduct
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBUsedQty = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBandProID = new System.Windows.Forms.TextBox();
            this.txtBanderol = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtBandeSize = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.dtpOpeningDate = new System.Windows.Forms.DateTimePicker();
            this.label17 = new System.Windows.Forms.Label();
            this.txtOpeningQty = new System.Windows.Forms.TextBox();
            this.txtWastQty = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label11 = new System.Windows.Forms.Label();
            this.chkActiveStatus = new System.Windows.Forms.CheckBox();
            this.rbtnProduct = new System.Windows.Forms.RadioButton();
            this.cmbProductName = new System.Windows.Forms.ComboBox();
            this.rbtnCode = new System.Windows.Forms.RadioButton();
            this.btnSearchPackage = new System.Windows.Forms.Button();
            this.cmbProductCode = new System.Windows.Forms.ComboBox();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.txtProductCode = new System.Windows.Forms.TextBox();
            this.txtPackId = new System.Windows.Forms.TextBox();
            this.txtBandeId = new System.Windows.Forms.TextBox();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.txtBandeUom = new System.Windows.Forms.TextBox();
            this.txtpacketUOM = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.btnSearchBanderol = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.txtPacketQty = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtPacket = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.backgroundWorkerAdd = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerDelete = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 155);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "B. Used Qty";
            // 
            // txtBUsedQty
            // 
            this.txtBUsedQty.Location = new System.Drawing.Point(105, 152);
            this.txtBUsedQty.MaximumSize = new System.Drawing.Size(4, 25);
            this.txtBUsedQty.MinimumSize = new System.Drawing.Size(75, 20);
            this.txtBUsedQty.Name = "txtBUsedQty";
            this.txtBUsedQty.Size = new System.Drawing.Size(75, 21);
            this.txtBUsedQty.TabIndex = 7;
            this.txtBUsedQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtBUsedQty.TextChanged += new System.EventHandler(this.txtBUsedQty_TextChanged);
            this.txtBUsedQty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBUsedQty_KeyDown);
            this.txtBUsedQty.Leave += new System.EventHandler(this.txtBUsedQty_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(391, 155);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "HS Code";
            // 
            // txtBandProID
            // 
            this.txtBandProID.Location = new System.Drawing.Point(105, 13);
            this.txtBandProID.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtBandProID.MinimumSize = new System.Drawing.Size(206, 20);
            this.txtBandProID.Name = "txtBandProID";
            this.txtBandProID.ReadOnly = true;
            this.txtBandProID.Size = new System.Drawing.Size(206, 20);
            this.txtBandProID.TabIndex = 0;
            this.txtBandProID.TabStop = false;
            // 
            // txtBanderol
            // 
            this.txtBanderol.Location = new System.Drawing.Point(105, 85);
            this.txtBanderol.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtBanderol.MinimumSize = new System.Drawing.Size(206, 20);
            this.txtBanderol.Name = "txtBanderol";
            this.txtBanderol.Size = new System.Drawing.Size(206, 20);
            this.txtBanderol.TabIndex = 2;
            this.txtBanderol.TextChanged += new System.EventHandler(this.txtBanderol_TextChanged);
            this.txtBanderol.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBanderol_KeyDown);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 111);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(71, 13);
            this.label18.TabIndex = 17;
            this.label18.Text = "Banderol Size";
            // 
            // txtBandeSize
            // 
            this.txtBandeSize.Location = new System.Drawing.Point(105, 108);
            this.txtBandeSize.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtBandeSize.MinimumSize = new System.Drawing.Size(100, 20);
            this.txtBandeSize.Name = "txtBandeSize";
            this.txtBandeSize.ReadOnly = true;
            this.txtBandeSize.Size = new System.Drawing.Size(100, 20);
            this.txtBandeSize.TabIndex = 3;
            this.txtBandeSize.TabStop = false;
            this.txtBandeSize.TextChanged += new System.EventHandler(this.txtBandeSize_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.dtpOpeningDate);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.txtOpeningQty);
            this.groupBox1.Controls.Add(this.txtWastQty);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.chkActiveStatus);
            this.groupBox1.Controls.Add(this.rbtnProduct);
            this.groupBox1.Controls.Add(this.cmbProductName);
            this.groupBox1.Controls.Add(this.rbtnCode);
            this.groupBox1.Controls.Add(this.btnSearchPackage);
            this.groupBox1.Controls.Add(this.cmbProductCode);
            this.groupBox1.Controls.Add(this.txtItemNo);
            this.groupBox1.Controls.Add(this.txtProductCode);
            this.groupBox1.Controls.Add(this.txtPackId);
            this.groupBox1.Controls.Add(this.txtBandeId);
            this.groupBox1.Controls.Add(this.txtProductName);
            this.groupBox1.Controls.Add(this.txtBandeUom);
            this.groupBox1.Controls.Add(this.txtpacketUOM);
            this.groupBox1.Controls.Add(this.label36);
            this.groupBox1.Controls.Add(this.txtBandeSize);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.txtBanderol);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label35);
            this.groupBox1.Controls.Add(this.btnSearchBanderol);
            this.groupBox1.Controls.Add(this.btnAddNew);
            this.groupBox1.Controls.Add(this.txtPacketQty);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txtPacket);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.txtBUsedQty);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtBandProID);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(447, 256);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(206, 133);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(49, 13);
            this.label16.TabIndex = 224;
            this.label16.Text = "Quantity";
            // 
            // dtpOpeningDate
            // 
            this.dtpOpeningDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpOpeningDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpOpeningDate.Location = new System.Drawing.Point(105, 130);
            this.dtpOpeningDate.MaxDate = new System.DateTime(9900, 1, 1, 0, 0, 0, 0);
            this.dtpOpeningDate.MaximumSize = new System.Drawing.Size(4, 20);
            this.dtpOpeningDate.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpOpeningDate.MinimumSize = new System.Drawing.Size(100, 21);
            this.dtpOpeningDate.Name = "dtpOpeningDate";
            this.dtpOpeningDate.Size = new System.Drawing.Size(100, 21);
            this.dtpOpeningDate.TabIndex = 5;
            this.dtpOpeningDate.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpOpeningDate.ValueChanged += new System.EventHandler(this.dtpOpeningDate_ValueChanged);
            this.dtpOpeningDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpOpeningDate_KeyDown);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 133);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(73, 13);
            this.label17.TabIndex = 222;
            this.label17.Text = "Opening Date";
            // 
            // txtOpeningQty
            // 
            this.txtOpeningQty.Location = new System.Drawing.Point(261, 130);
            this.txtOpeningQty.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtOpeningQty.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtOpeningQty.Name = "txtOpeningQty";
            this.txtOpeningQty.Size = new System.Drawing.Size(50, 20);
            this.txtOpeningQty.TabIndex = 6;
            this.txtOpeningQty.Text = "0";
            this.txtOpeningQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtOpeningQty.TextChanged += new System.EventHandler(this.txtOpeningQty_TextChanged);
            this.txtOpeningQty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtOpeningQty_KeyDown);
            this.txtOpeningQty.Leave += new System.EventHandler(this.txtOpeningQty_Leave);
            // 
            // txtWastQty
            // 
            this.txtWastQty.Location = new System.Drawing.Point(261, 152);
            this.txtWastQty.MaximumSize = new System.Drawing.Size(50, 20);
            this.txtWastQty.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtWastQty.Name = "txtWastQty";
            this.txtWastQty.Size = new System.Drawing.Size(50, 21);
            this.txtWastQty.TabIndex = 8;
            this.txtWastQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtWastQty.TextChanged += new System.EventHandler(this.txtWastQty_TextChanged);
            this.txtWastQty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtWastQty_KeyDown);
            this.txtWastQty.Leave += new System.EventHandler(this.txtWastQty_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(189, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 220;
            this.label4.Text = "Wastage %";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(76, 226);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 182;
            this.progressBar1.Visible = false;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(333, 204);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 13);
            this.label11.TabIndex = 183;
            this.label11.Text = "Active Status";
            // 
            // chkActiveStatus
            // 
            this.chkActiveStatus.AutoSize = true;
            this.chkActiveStatus.Checked = true;
            this.chkActiveStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActiveStatus.Location = new System.Drawing.Point(407, 204);
            this.chkActiveStatus.Name = "chkActiveStatus";
            this.chkActiveStatus.Size = new System.Drawing.Size(15, 14);
            this.chkActiveStatus.TabIndex = 184;
            this.chkActiveStatus.TabStop = false;
            this.chkActiveStatus.UseVisualStyleBackColor = true;
            // 
            // rbtnProduct
            // 
            this.rbtnProduct.AutoSize = true;
            this.rbtnProduct.BackColor = System.Drawing.SystemColors.Window;
            this.rbtnProduct.Location = new System.Drawing.Point(279, 66);
            this.rbtnProduct.Name = "rbtnProduct";
            this.rbtnProduct.Size = new System.Drawing.Size(14, 13);
            this.rbtnProduct.TabIndex = 213;
            this.rbtnProduct.UseVisualStyleBackColor = false;
            this.rbtnProduct.CheckedChanged += new System.EventHandler(this.rbtnProduct_CheckedChanged);
            // 
            // cmbProductName
            // 
            this.cmbProductName.Enabled = false;
            this.cmbProductName.FormattingEnabled = true;
            this.cmbProductName.Location = new System.Drawing.Point(105, 62);
            this.cmbProductName.Name = "cmbProductName";
            this.cmbProductName.Size = new System.Drawing.Size(206, 21);
            this.cmbProductName.Sorted = true;
            this.cmbProductName.TabIndex = 14;
            this.cmbProductName.SelectedIndexChanged += new System.EventHandler(this.cmbProductName_SelectedIndexChanged);
            this.cmbProductName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbProductName_KeyDown);
            this.cmbProductName.Leave += new System.EventHandler(this.cmbProductName_Leave);
            // 
            // rbtnCode
            // 
            this.rbtnCode.AutoSize = true;
            this.rbtnCode.BackColor = System.Drawing.SystemColors.Window;
            this.rbtnCode.Checked = true;
            this.rbtnCode.Location = new System.Drawing.Point(279, 43);
            this.rbtnCode.Name = "rbtnCode";
            this.rbtnCode.Size = new System.Drawing.Size(14, 13);
            this.rbtnCode.TabIndex = 212;
            this.rbtnCode.TabStop = true;
            this.rbtnCode.UseVisualStyleBackColor = false;
            this.rbtnCode.CheckedChanged += new System.EventHandler(this.rbtnCode_CheckedChanged);
            // 
            // btnSearchPackage
            // 
            this.btnSearchPackage.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchPackage.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchPackage.Location = new System.Drawing.Point(336, 176);
            this.btnSearchPackage.Name = "btnSearchPackage";
            this.btnSearchPackage.Size = new System.Drawing.Size(30, 20);
            this.btnSearchPackage.TabIndex = 218;
            this.btnSearchPackage.UseVisualStyleBackColor = false;
            this.btnSearchPackage.Click += new System.EventHandler(this.btnSearchPackage_Click);
            // 
            // cmbProductCode
            // 
            this.cmbProductCode.FormattingEnabled = true;
            this.cmbProductCode.Location = new System.Drawing.Point(105, 38);
            this.cmbProductCode.MinimumSize = new System.Drawing.Size(206, 0);
            this.cmbProductCode.Name = "cmbProductCode";
            this.cmbProductCode.Size = new System.Drawing.Size(206, 21);
            this.cmbProductCode.Sorted = true;
            this.cmbProductCode.TabIndex = 1;
            this.cmbProductCode.SelectedIndexChanged += new System.EventHandler(this.cmbProductCode_SelectedIndexChanged);
            this.cmbProductCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbProductCode_KeyDown);
            this.cmbProductCode.Leave += new System.EventHandler(this.cmbProductCode_Leave);
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(391, 39);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.ReadOnly = true;
            this.txtItemNo.Size = new System.Drawing.Size(50, 21);
            this.txtItemNo.TabIndex = 211;
            this.txtItemNo.TabStop = false;
            this.txtItemNo.Visible = false;
            this.txtItemNo.TextChanged += new System.EventHandler(this.txtItemNo_TextChanged);
            this.txtItemNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtItemNo_KeyDown);
            // 
            // txtProductCode
            // 
            this.txtProductCode.Location = new System.Drawing.Point(337, 40);
            this.txtProductCode.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtProductCode.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.ReadOnly = true;
            this.txtProductCode.Size = new System.Drawing.Size(50, 21);
            this.txtProductCode.TabIndex = 211;
            this.txtProductCode.TabStop = false;
            this.txtProductCode.Visible = false;
            this.txtProductCode.TextChanged += new System.EventHandler(this.txtProductCode_TextChanged);
            this.txtProductCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProductCode_KeyDown);
            // 
            // txtPackId
            // 
            this.txtPackId.Location = new System.Drawing.Point(391, 179);
            this.txtPackId.MaximumSize = new System.Drawing.Size(180, 20);
            this.txtPackId.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtPackId.Name = "txtPackId";
            this.txtPackId.ReadOnly = true;
            this.txtPackId.Size = new System.Drawing.Size(50, 21);
            this.txtPackId.TabIndex = 210;
            this.txtPackId.TabStop = false;
            this.txtPackId.Visible = false;
            this.txtPackId.TextChanged += new System.EventHandler(this.txtPackId_TextChanged);
            this.txtPackId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPackId_KeyDown);
            // 
            // txtBandeId
            // 
            this.txtBandeId.Location = new System.Drawing.Point(381, 86);
            this.txtBandeId.MaximumSize = new System.Drawing.Size(180, 20);
            this.txtBandeId.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtBandeId.Name = "txtBandeId";
            this.txtBandeId.ReadOnly = true;
            this.txtBandeId.Size = new System.Drawing.Size(50, 21);
            this.txtBandeId.TabIndex = 210;
            this.txtBandeId.TabStop = false;
            this.txtBandeId.Visible = false;
            this.txtBandeId.TextChanged += new System.EventHandler(this.txtBandeId_TextChanged);
            this.txtBandeId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBandeId_KeyDown);
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(382, 62);
            this.txtProductName.MaximumSize = new System.Drawing.Size(180, 20);
            this.txtProductName.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(50, 21);
            this.txtProductName.TabIndex = 210;
            this.txtProductName.TabStop = false;
            this.txtProductName.Visible = false;
            this.txtProductName.TextChanged += new System.EventHandler(this.txtProductName_TextChanged);
            this.txtProductName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProductName_KeyDown);
            // 
            // txtBandeUom
            // 
            this.txtBandeUom.Location = new System.Drawing.Point(261, 108);
            this.txtBandeUom.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtBandeUom.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtBandeUom.Name = "txtBandeUom";
            this.txtBandeUom.ReadOnly = true;
            this.txtBandeUom.Size = new System.Drawing.Size(50, 20);
            this.txtBandeUom.TabIndex = 4;
            this.txtBandeUom.TabStop = false;
            this.txtBandeUom.TextChanged += new System.EventHandler(this.txtBandeUom_TextChanged);
            // 
            // txtpacketUOM
            // 
            this.txtpacketUOM.Location = new System.Drawing.Point(261, 201);
            this.txtpacketUOM.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtpacketUOM.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtpacketUOM.Name = "txtpacketUOM";
            this.txtpacketUOM.ReadOnly = true;
            this.txtpacketUOM.Size = new System.Drawing.Size(50, 20);
            this.txtpacketUOM.TabIndex = 11;
            this.txtpacketUOM.TabStop = false;
            this.txtpacketUOM.TextChanged += new System.EventHandler(this.txtpacketUOM_TextChanged);
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(224, 204);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(30, 13);
            this.label36.TabIndex = 217;
            this.label36.Text = "UOM";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(224, 112);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(30, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "UOM";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(6, 65);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(74, 13);
            this.label26.TabIndex = 213;
            this.label26.Text = "Product Name";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 88);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(79, 13);
            this.label14.TabIndex = 213;
            this.label14.Text = "Banderol Name";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(6, 43);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(72, 13);
            this.label35.TabIndex = 214;
            this.label35.Text = "Product Code";
            // 
            // btnSearchBanderol
            // 
            this.btnSearchBanderol.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchBanderol.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchBanderol.Location = new System.Drawing.Point(337, 85);
            this.btnSearchBanderol.Name = "btnSearchBanderol";
            this.btnSearchBanderol.Size = new System.Drawing.Size(30, 20);
            this.btnSearchBanderol.TabIndex = 181;
            this.btnSearchBanderol.UseVisualStyleBackColor = false;
            this.btnSearchBanderol.Click += new System.EventHandler(this.btnSearchBanderol_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAddNew.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddNew.Location = new System.Drawing.Point(382, 16);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(30, 20);
            this.btnAddNew.TabIndex = 7;
            this.btnAddNew.TabStop = false;
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // txtPacketQty
            // 
            this.txtPacketQty.Location = new System.Drawing.Point(105, 201);
            this.txtPacketQty.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPacketQty.MinimumSize = new System.Drawing.Size(100, 20);
            this.txtPacketQty.Name = "txtPacketQty";
            this.txtPacketQty.ReadOnly = true;
            this.txtPacketQty.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtPacketQty.Size = new System.Drawing.Size(100, 20);
            this.txtPacketQty.TabIndex = 10;
            this.txtPacketQty.TabStop = false;
            this.txtPacketQty.TextChanged += new System.EventHandler(this.txtPacketQty_TextChanged);
            this.txtPacketQty.Leave += new System.EventHandler(this.txtPacketQty_Leave);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 204);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(92, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "Package Capacity";
            // 
            // txtPacket
            // 
            this.txtPacket.Location = new System.Drawing.Point(105, 176);
            this.txtPacket.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPacket.MinimumSize = new System.Drawing.Size(206, 20);
            this.txtPacket.Name = "txtPacket";
            this.txtPacket.Size = new System.Drawing.Size(206, 20);
            this.txtPacket.TabIndex = 9;
            this.txtPacket.TextChanged += new System.EventHandler(this.txtPacket_TextChanged);
            this.txtPacket.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPacket_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 179);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(96, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "Nature of Package";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(337, 15);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.TabStop = false;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // backgroundWorkerAdd
            // 
            this.backgroundWorkerAdd.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerAdd_DoWork);
            this.backgroundWorkerAdd.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerAdd_RunWorkerCompleted);
            // 
            // backgroundWorkerDelete
            // 
            this.backgroundWorkerDelete.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerDelete_DoWork);
            this.backgroundWorkerDelete.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerDelete_RunWorkerCompleted);
            // 
            // backgroundWorkerUpdate
            // 
            this.backgroundWorkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpdate_DoWork);
            this.backgroundWorkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerUpdate_RunWorkerCompleted);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(399, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Location = new System.Drawing.Point(-6, 265);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(495, 40);
            this.panel1.TabIndex = 12;
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(276, 6);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(84, 28);
            this.btnPrint.TabIndex = 3;
            this.btnPrint.Text = "&Print";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(107, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 1;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.Save;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(24, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(249, 57);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDelete.Image = global::VATClient.Properties.Resources.Delete;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(191, 61);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 28);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // FormBanderolProduct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(484, 312);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.Name = "FormBanderolProduct";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Product Mapping- Banderol";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBanderolProduct_FormClosing);
            this.Load += new System.EventHandler(this.FormBanderolProduct_Load);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBUsedQty;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBandProID;
        private System.Windows.Forms.TextBox txtBanderol;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnSearchBanderol;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtBandeSize;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Button btnPrint;
        private System.ComponentModel.BackgroundWorker backgroundWorkerAdd;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerDelete;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdate;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox txtProductCode;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.TextBox txtpacketUOM;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtPacketQty;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtPacket;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnSearchPackage;
        private System.Windows.Forms.TextBox txtBandeUom;
        private System.Windows.Forms.ComboBox cmbProductName;
        private System.Windows.Forms.ComboBox cmbProductCode;
        private System.Windows.Forms.RadioButton rbtnProduct;
        private System.Windows.Forms.RadioButton rbtnCode;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkActiveStatus;
        private System.Windows.Forms.TextBox txtBandeId;
        private System.Windows.Forms.TextBox txtPackId;
        private System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.TextBox txtWastQty;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.DateTimePicker dtpOpeningDate;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtOpeningQty;
        public System.Windows.Forms.Button btnUpdate;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnDelete;
    }
}