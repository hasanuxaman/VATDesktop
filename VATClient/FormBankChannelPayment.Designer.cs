namespace VATClient
{
    partial class FormBankChannelPayment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBankChannelPayment));
            this.txtPurchaseInvoiceNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpPaymentDate = new System.Windows.Forms.DateTimePicker();
            this.label31 = new System.Windows.Forms.Label();
            this.txtPaymentAmount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtVATAmount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.txtBankName = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtBankID = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgwSave = new System.ComponentModel.BackgroundWorker();
            this.bgwPost = new System.ComponentModel.BackgroundWorker();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.cmbPaymentType = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvBankChannel = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnChange = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bthUpdate = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PurchaseInvoiceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BankID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BankName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PaymentType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PaymentDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PaymentAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VATAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Remarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankChannel)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtPurchaseInvoiceNo
            // 
            this.txtPurchaseInvoiceNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtPurchaseInvoiceNo.Location = new System.Drawing.Point(106, 13);
            this.txtPurchaseInvoiceNo.MaximumSize = new System.Drawing.Size(200, 25);
            this.txtPurchaseInvoiceNo.MinimumSize = new System.Drawing.Size(100, 20);
            this.txtPurchaseInvoiceNo.Name = "txtPurchaseInvoiceNo";
            this.txtPurchaseInvoiceNo.Size = new System.Drawing.Size(193, 20);
            this.txtPurchaseInvoiceNo.TabIndex = 52;
            this.txtPurchaseInvoiceNo.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 53;
            this.label2.Text = "Pur No";
            // 
            // dtpPaymentDate
            // 
            this.dtpPaymentDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpPaymentDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPaymentDate.Location = new System.Drawing.Point(391, 13);
            this.dtpPaymentDate.Name = "dtpPaymentDate";
            this.dtpPaymentDate.Size = new System.Drawing.Size(138, 20);
            this.dtpPaymentDate.TabIndex = 204;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(307, 17);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(74, 13);
            this.label31.TabIndex = 205;
            this.label31.Text = "Payment Date";
            // 
            // txtPaymentAmount
            // 
            this.txtPaymentAmount.BackColor = System.Drawing.Color.White;
            this.txtPaymentAmount.Location = new System.Drawing.Point(106, 43);
            this.txtPaymentAmount.MaximumSize = new System.Drawing.Size(200, 25);
            this.txtPaymentAmount.MinimumSize = new System.Drawing.Size(100, 20);
            this.txtPaymentAmount.Name = "txtPaymentAmount";
            this.txtPaymentAmount.Size = new System.Drawing.Size(193, 20);
            this.txtPaymentAmount.TabIndex = 206;
            this.txtPaymentAmount.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 207;
            this.label1.Text = "Payment Amount";
            // 
            // txtVATAmount
            // 
            this.txtVATAmount.BackColor = System.Drawing.Color.White;
            this.txtVATAmount.Location = new System.Drawing.Point(391, 43);
            this.txtVATAmount.MaximumSize = new System.Drawing.Size(200, 25);
            this.txtVATAmount.MinimumSize = new System.Drawing.Size(100, 20);
            this.txtVATAmount.Name = "txtVATAmount";
            this.txtVATAmount.Size = new System.Drawing.Size(138, 20);
            this.txtVATAmount.TabIndex = 208;
            this.txtVATAmount.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(307, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 209;
            this.label3.Text = "VATAmount";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 121);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(49, 13);
            this.label14.TabIndex = 211;
            this.label14.Text = "Remarks";
            // 
            // txtRemarks
            // 
            this.txtRemarks.BackColor = System.Drawing.SystemColors.Window;
            this.txtRemarks.Location = new System.Drawing.Point(106, 105);
            this.txtRemarks.MaximumSize = new System.Drawing.Size(500, 60);
            this.txtRemarks.MinimumSize = new System.Drawing.Size(170, 21);
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.Size = new System.Drawing.Size(423, 44);
            this.txtRemarks.TabIndex = 210;
            this.txtRemarks.TabStop = false;
            // 
            // txtBankName
            // 
            this.txtBankName.Location = new System.Drawing.Point(106, 74);
            this.txtBankName.MaximumSize = new System.Drawing.Size(500, 20);
            this.txtBankName.MinimumSize = new System.Drawing.Size(190, 20);
            this.txtBankName.Name = "txtBankName";
            this.txtBankName.ReadOnly = true;
            this.txtBankName.Size = new System.Drawing.Size(193, 20);
            this.txtBankName.TabIndex = 212;
            this.txtBankName.DoubleClick += new System.EventHandler(this.txtBankName_DoubleClick);
            this.txtBankName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBankName_KeyDown);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(7, 77);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(81, 13);
            this.label15.TabIndex = 213;
            this.label15.Text = "Bank Name(F9)";
            // 
            // txtBankID
            // 
            this.txtBankID.Location = new System.Drawing.Point(524, 79);
            this.txtBankID.MaximumSize = new System.Drawing.Size(20, 20);
            this.txtBankID.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtBankID.Name = "txtBankID";
            this.txtBankID.ReadOnly = true;
            this.txtBankID.Size = new System.Drawing.Size(20, 20);
            this.txtBankID.TabIndex = 214;
            this.txtBankID.TabStop = false;
            this.txtBankID.Visible = false;
            // 
            // label40
            // 
            this.label40.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.ForeColor = System.Drawing.Color.Red;
            this.label40.Location = new System.Drawing.Point(96, 17);
            this.label40.MaximumSize = new System.Drawing.Size(10, 10);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(9, 10);
            this.label40.TabIndex = 278;
            this.label40.Text = "*";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(96, 48);
            this.label4.MaximumSize = new System.Drawing.Size(10, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(9, 10);
            this.label4.TabIndex = 279;
            this.label4.Text = "*";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(380, 18);
            this.label5.MaximumSize = new System.Drawing.Size(10, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 10);
            this.label5.TabIndex = 280;
            this.label5.Text = "*";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(380, 48);
            this.label6.MaximumSize = new System.Drawing.Size(10, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 10);
            this.label6.TabIndex = 281;
            this.label6.Text = "*";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(96, 78);
            this.label7.MaximumSize = new System.Drawing.Size(10, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(9, 10);
            this.label7.TabIndex = 282;
            this.label7.Text = "*";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(524, 105);
            this.txtId.MaximumSize = new System.Drawing.Size(20, 20);
            this.txtId.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(20, 20);
            this.txtId.TabIndex = 283;
            this.txtId.TabStop = false;
            this.txtId.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(122, 82);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 284;
            this.progressBar1.Visible = false;
            // 
            // bgwSave
            // 
            this.bgwSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSave_DoWork);
            this.bgwSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSave_RunWorkerCompleted);
            // 
            // bgwPost
            // 
            this.bgwPost.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPost_DoWork);
            this.bgwPost.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPost_RunWorkerCompleted);
            // 
            // bgwUpdate
            // 
            this.bgwUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUpdate_DoWork);
            this.bgwUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUpdate_RunWorkerCompleted);
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.cmbPaymentType);
            this.groupBox1.Controls.Add(this.btnRefresh);
            this.groupBox1.Controls.Add(this.btnChange);
            this.groupBox1.Controls.Add(this.txtRemarks);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtId);
            this.groupBox1.Controls.Add(this.txtBankID);
            this.groupBox1.Controls.Add(this.txtPurchaseInvoiceNo);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label31);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.dtpPaymentDate);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtPaymentAmount);
            this.groupBox1.Controls.Add(this.label40);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtVATAmount);
            this.groupBox1.Controls.Add(this.txtBankName);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Location = new System.Drawing.Point(4, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(535, 184);
            this.groupBox1.TabIndex = 285;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Bank Channel";
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(380, 79);
            this.label8.MaximumSize = new System.Drawing.Size(10, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(10, 10);
            this.label8.TabIndex = 289;
            this.label8.Text = "*";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(307, 78);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(75, 13);
            this.label24.TabIndex = 288;
            this.label24.Text = "Payment Type";
            // 
            // cmbPaymentType
            // 
            this.cmbPaymentType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbPaymentType.FormattingEnabled = true;
            this.cmbPaymentType.ItemHeight = 13;
            this.cmbPaymentType.Items.AddRange(new object[] {
            "Cheque",
            "Online"});
            this.cmbPaymentType.Location = new System.Drawing.Point(391, 74);
            this.cmbPaymentType.Name = "cmbPaymentType";
            this.cmbPaymentType.Size = new System.Drawing.Size(138, 21);
            this.cmbPaymentType.Sorted = true;
            this.cmbPaymentType.TabIndex = 287;
            this.cmbPaymentType.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.dgvBankChannel);
            this.groupBox2.Location = new System.Drawing.Point(5, 184);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(534, 174);
            this.groupBox2.TabIndex = 286;
            this.groupBox2.TabStop = false;
            // 
            // dgvBankChannel
            // 
            this.dgvBankChannel.AllowUserToAddRows = false;
            this.dgvBankChannel.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvBankChannel.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBankChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBankChannel.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvBankChannel.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.Id,
            this.PurchaseInvoiceNo,
            this.BankID,
            this.BankName,
            this.PaymentType,
            this.PaymentDate,
            this.PaymentAmount,
            this.VATAmount,
            this.Remarks,
            this.Post});
            this.dgvBankChannel.Location = new System.Drawing.Point(6, 11);
            this.dgvBankChannel.Name = "dgvBankChannel";
            this.dgvBankChannel.RowHeadersVisible = false;
            this.dgvBankChannel.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBankChannel.Size = new System.Drawing.Size(524, 158);
            this.dgvBankChannel.TabIndex = 7;
            this.dgvBankChannel.DoubleClick += new System.EventHandler(this.dgvBankChannel_DoubleClick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(409, 152);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 28);
            this.btnRefresh.TabIndex = 286;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnChange
            // 
            this.btnChange.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnChange.Image = global::VATClient.Properties.Resources.Update;
            this.btnChange.Location = new System.Drawing.Point(316, 152);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(75, 28);
            this.btnChange.TabIndex = 285;
            this.btnChange.TabStop = false;
            this.btnChange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnChange.UseVisualStyleBackColor = false;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.bthUpdate);
            this.panel1.Controls.Add(this.btnPost);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(-1, 363);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(543, 40);
            this.panel1.TabIndex = 51;
            // 
            // bthUpdate
            // 
            this.bthUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.bthUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bthUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.bthUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bthUpdate.Location = new System.Drawing.Point(329, 7);
            this.bthUpdate.Name = "bthUpdate";
            this.bthUpdate.Size = new System.Drawing.Size(62, 28);
            this.bthUpdate.TabIndex = 52;
            this.bthUpdate.Text = "&Update";
            this.bthUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bthUpdate.UseVisualStyleBackColor = false;
            this.bthUpdate.Visible = false;
            this.bthUpdate.Click += new System.EventHandler(this.bthUpdate_Click);
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPost.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPost.Image = global::VATClient.Properties.Resources.Post;
            this.btnPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPost.Location = new System.Drawing.Point(95, 7);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(62, 28);
            this.btnPost.TabIndex = 53;
            this.btnPost.Text = "&Post";
            this.btnPost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(100, 59);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 20;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(290, 59);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 28);
            this.btnPrint.TabIndex = 3;
            this.btnPrint.Text = "&Print";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Image = global::VATClient.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(13, 7);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(62, 28);
            this.btnSave.TabIndex = 51;
            this.btnSave.Text = "&Add";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(181, 58);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            this.Select.Visible = false;
            this.Select.Width = 43;
            // 
            // Id
            // 
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.Visible = false;
            this.Id.Width = 41;
            // 
            // PurchaseInvoiceNo
            // 
            this.PurchaseInvoiceNo.HeaderText = "Purchase InvoiceNo";
            this.PurchaseInvoiceNo.Name = "PurchaseInvoiceNo";
            this.PurchaseInvoiceNo.Width = 129;
            // 
            // BankID
            // 
            this.BankID.HeaderText = "BankID";
            this.BankID.Name = "BankID";
            this.BankID.Visible = false;
            this.BankID.Width = 68;
            // 
            // BankName
            // 
            this.BankName.HeaderText = "Bank Name";
            this.BankName.Name = "BankName";
            this.BankName.Width = 88;
            // 
            // PaymentType
            // 
            this.PaymentType.HeaderText = "Payment Type";
            this.PaymentType.Name = "PaymentType";
            // 
            // PaymentDate
            // 
            this.PaymentDate.HeaderText = "Payment Date";
            this.PaymentDate.Name = "PaymentDate";
            this.PaymentDate.Width = 99;
            // 
            // PaymentAmount
            // 
            this.PaymentAmount.HeaderText = "Payment Amount";
            this.PaymentAmount.Name = "PaymentAmount";
            this.PaymentAmount.Width = 112;
            // 
            // VATAmount
            // 
            this.VATAmount.HeaderText = "VAT Amount";
            this.VATAmount.Name = "VATAmount";
            this.VATAmount.Width = 92;
            // 
            // Remarks
            // 
            this.Remarks.HeaderText = "Remarks";
            this.Remarks.Name = "Remarks";
            this.Remarks.Width = 74;
            // 
            // Post
            // 
            this.Post.HeaderText = "Post";
            this.Post.Name = "Post";
            this.Post.Width = 53;
            // 
            // FormBankChannelPayment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(540, 403);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FormBankChannelPayment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bank Channel Payment";
            this.Load += new System.EventHandler(this.FormBankChannelPayment_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankChannel)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Button bthUpdate;
        public System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnPrint;
        public System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpPaymentDate;
        public System.Windows.Forms.Label label31;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.TextBox txtBankName;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtBankID;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwSave;
        private System.ComponentModel.BackgroundWorker bgwPost;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        public System.Windows.Forms.TextBox txtPurchaseInvoiceNo;
        public System.Windows.Forms.TextBox txtPaymentAmount;
        public System.Windows.Forms.TextBox txtVATAmount;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvBankChannel;
        private System.Windows.Forms.Button btnChange;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.ComboBox cmbPaymentType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn PurchaseInvoiceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn BankID;
        private System.Windows.Forms.DataGridViewTextBoxColumn BankName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PaymentType;
        private System.Windows.Forms.DataGridViewTextBoxColumn PaymentDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn PaymentAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn VATAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Remarks;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;

    }
}