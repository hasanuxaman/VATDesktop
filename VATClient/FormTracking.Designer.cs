namespace VATClient
{
    partial class FormTracking
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTracking));
            this.btnSearch = new System.Windows.Forms.Button();
            this.rBtnCodeT = new System.Windows.Forms.RadioButton();
            this.rBtnNameT = new System.Windows.Forms.RadioButton();
            this.cmbPCode = new System.Windows.Forms.ComboBox();
            this.cmbPName = new System.Windows.Forms.ComboBox();
            this.label30 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.dgvTracking = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.LineNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProductName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Heading1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Heading2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FinishItemNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Issue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TIssueNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Receive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TReceiveNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sale = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TSaleInvoiceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReturnReceive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReturnSale = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.grpTransaction = new System.Windows.Forms.GroupBox();
            this.rbtnIssue = new System.Windows.Forms.RadioButton();
            this.rbtnSale = new System.Windows.Forms.RadioButton();
            this.rbtnReceive = new System.Windows.Forms.RadioButton();
            this.txtFinishQty = new System.Windows.Forms.TextBox();
            this.txtFinishItemNo = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.txtDate = new System.Windows.Forms.TextBox();
            this.txtVatName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTracking)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.grpTransaction.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(541, 39);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(68, 21);
            this.btnSearch.TabIndex = 236;
            this.btnSearch.Text = "Search";
            this.btnSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // rBtnCodeT
            // 
            this.rBtnCodeT.AutoSize = true;
            this.rBtnCodeT.BackColor = System.Drawing.SystemColors.Window;
            this.rBtnCodeT.Checked = true;
            this.rBtnCodeT.Location = new System.Drawing.Point(242, 15);
            this.rBtnCodeT.Name = "rBtnCodeT";
            this.rBtnCodeT.Size = new System.Drawing.Size(14, 13);
            this.rBtnCodeT.TabIndex = 229;
            this.rBtnCodeT.TabStop = true;
            this.rBtnCodeT.UseVisualStyleBackColor = false;
            this.rBtnCodeT.CheckedChanged += new System.EventHandler(this.rBtnCodeT_CheckedChanged);
            // 
            // rBtnNameT
            // 
            this.rBtnNameT.AutoSize = true;
            this.rBtnNameT.BackColor = System.Drawing.SystemColors.Window;
            this.rBtnNameT.Location = new System.Drawing.Point(575, 15);
            this.rBtnNameT.Name = "rBtnNameT";
            this.rBtnNameT.Size = new System.Drawing.Size(14, 13);
            this.rBtnNameT.TabIndex = 228;
            this.rBtnNameT.UseVisualStyleBackColor = false;
            // 
            // cmbPCode
            // 
            this.cmbPCode.FormattingEnabled = true;
            this.cmbPCode.Location = new System.Drawing.Point(95, 12);
            this.cmbPCode.Name = "cmbPCode";
            this.cmbPCode.Size = new System.Drawing.Size(179, 21);
            this.cmbPCode.Sorted = true;
            this.cmbPCode.TabIndex = 226;
            this.cmbPCode.Leave += new System.EventHandler(this.cmbPCode_Leave);
            // 
            // cmbPName
            // 
            this.cmbPName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbPName.Enabled = false;
            this.cmbPName.FormattingEnabled = true;
            this.cmbPName.ItemHeight = 13;
            this.cmbPName.Location = new System.Drawing.Point(388, 12);
            this.cmbPName.Name = "cmbPName";
            this.cmbPName.Size = new System.Drawing.Size(221, 21);
            this.cmbPName.Sorted = true;
            this.cmbPName.TabIndex = 227;
            this.cmbPName.TabStop = false;
            this.cmbPName.SelectedIndexChanged += new System.EventHandler(this.cmbPName_SelectedIndexChanged);
            this.cmbPName.Leave += new System.EventHandler(this.cmbPName_Leave);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(346, 15);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(35, 13);
            this.label30.TabIndex = 225;
            this.label30.Text = "Name";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(4, 15);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(85, 13);
            this.label23.TabIndex = 224;
            this.label23.Text = "Finish Item Code";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dgvTracking
            // 
            this.dgvTracking.AllowUserToAddRows = false;
            this.dgvTracking.AllowUserToDeleteRows = false;
            this.dgvTracking.AllowUserToOrderColumns = true;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.Blue;
            this.dgvTracking.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle11;
            this.dgvTracking.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTracking.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dgvTracking.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.LineNo,
            this.PCode,
            this.ItemNo,
            this.ProductName,
            this.Heading1,
            this.Heading2,
            this.Quantity,
            this.FinishItemNo,
            this.Issue,
            this.TIssueNo,
            this.Receive,
            this.TReceiveNo,
            this.Sale,
            this.TSaleInvoiceNo,
            this.ReturnReceive,
            this.ReturnSale});
            this.dgvTracking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTracking.GridColor = System.Drawing.SystemColors.HotTrack;
            this.dgvTracking.Location = new System.Drawing.Point(3, 16);
            this.dgvTracking.Name = "dgvTracking";
            this.dgvTracking.RowHeadersVisible = false;
            this.dgvTracking.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTracking.Size = new System.Drawing.Size(604, 255);
            this.dgvTracking.TabIndex = 59;
            this.dgvTracking.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTracking_CellClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            this.Select.Width = 93;
            // 
            // LineNo
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LineNo.DefaultCellStyle = dataGridViewCellStyle13;
            this.LineNo.FillWeight = 50F;
            this.LineNo.HeaderText = "Line No";
            this.LineNo.Name = "LineNo";
            this.LineNo.ReadOnly = true;
            this.LineNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.LineNo.Width = 46;
            // 
            // PCode
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.PCode.DefaultCellStyle = dataGridViewCellStyle14;
            this.PCode.HeaderText = "Code";
            this.PCode.Name = "PCode";
            this.PCode.ReadOnly = true;
            this.PCode.Width = 93;
            // 
            // ItemNo
            // 
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ItemNo.DefaultCellStyle = dataGridViewCellStyle15;
            this.ItemNo.HeaderText = "Item No";
            this.ItemNo.Name = "ItemNo";
            this.ItemNo.ReadOnly = true;
            this.ItemNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ItemNo.Visible = false;
            // 
            // ProductName
            // 
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ProductName.DefaultCellStyle = dataGridViewCellStyle16;
            this.ProductName.HeaderText = "Product Name";
            this.ProductName.Name = "ProductName";
            this.ProductName.ReadOnly = true;
            this.ProductName.Width = 93;
            // 
            // Heading1
            // 
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Heading1.DefaultCellStyle = dataGridViewCellStyle17;
            this.Heading1.FillWeight = 150F;
            this.Heading1.HeaderText = "Heading1";
            this.Heading1.MinimumWidth = 10;
            this.Heading1.Name = "Heading1";
            this.Heading1.ReadOnly = true;
            this.Heading1.Width = 139;
            // 
            // Heading2
            // 
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.Format = "N6";
            dataGridViewCellStyle18.NullValue = null;
            this.Heading2.DefaultCellStyle = dataGridViewCellStyle18;
            this.Heading2.FillWeight = 150F;
            this.Heading2.HeaderText = "Heading2";
            this.Heading2.Name = "Heading2";
            this.Heading2.ReadOnly = true;
            this.Heading2.Width = 139;
            // 
            // Quantity
            // 
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle19.Format = "N6";
            this.Quantity.DefaultCellStyle = dataGridViewCellStyle19;
            this.Quantity.HeaderText = "Quantity";
            this.Quantity.Name = "Quantity";
            this.Quantity.ReadOnly = true;
            this.Quantity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Quantity.Visible = false;
            // 
            // FinishItemNo
            // 
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.FinishItemNo.DefaultCellStyle = dataGridViewCellStyle20;
            this.FinishItemNo.HeaderText = "FinishItemNo";
            this.FinishItemNo.Name = "FinishItemNo";
            this.FinishItemNo.ReadOnly = true;
            this.FinishItemNo.Visible = false;
            // 
            // Issue
            // 
            this.Issue.HeaderText = "Issue";
            this.Issue.Name = "Issue";
            this.Issue.Visible = false;
            // 
            // TIssueNo
            // 
            this.TIssueNo.HeaderText = "IssueNo";
            this.TIssueNo.Name = "TIssueNo";
            this.TIssueNo.Visible = false;
            // 
            // Receive
            // 
            this.Receive.HeaderText = "Receive";
            this.Receive.Name = "Receive";
            this.Receive.Visible = false;
            // 
            // TReceiveNo
            // 
            this.TReceiveNo.HeaderText = "ReceiveNo";
            this.TReceiveNo.Name = "TReceiveNo";
            this.TReceiveNo.Visible = false;
            // 
            // Sale
            // 
            this.Sale.HeaderText = "Sale";
            this.Sale.Name = "Sale";
            this.Sale.Visible = false;
            // 
            // TSaleInvoiceNo
            // 
            this.TSaleInvoiceNo.HeaderText = "SaleInvoiceNo";
            this.TSaleInvoiceNo.Name = "TSaleInvoiceNo";
            this.TSaleInvoiceNo.Visible = false;
            // 
            // ReturnReceive
            // 
            this.ReturnReceive.HeaderText = "ReturnReceive";
            this.ReturnReceive.Name = "ReturnReceive";
            this.ReturnReceive.Visible = false;
            // 
            // ReturnSale
            // 
            this.ReturnSale.HeaderText = "ReturnSale";
            this.ReturnSale.Name = "ReturnSale";
            this.ReturnSale.Visible = false;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.progressBar1);
            this.groupBox6.Controls.Add(this.grpTransaction);
            this.groupBox6.Controls.Add(this.dgvTracking);
            this.groupBox6.Location = new System.Drawing.Point(7, 66);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(610, 274);
            this.groupBox6.TabIndex = 238;
            this.groupBox6.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(150, 139);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 244;
            // 
            // grpTransaction
            // 
            this.grpTransaction.AutoSize = true;
            this.grpTransaction.Controls.Add(this.rbtnIssue);
            this.grpTransaction.Controls.Add(this.rbtnSale);
            this.grpTransaction.Controls.Add(this.rbtnReceive);
            this.grpTransaction.Location = new System.Drawing.Point(175, 86);
            this.grpTransaction.Name = "grpTransaction";
            this.grpTransaction.Size = new System.Drawing.Size(166, 47);
            this.grpTransaction.TabIndex = 243;
            this.grpTransaction.TabStop = false;
            this.grpTransaction.Text = "Transaction";
            this.grpTransaction.Visible = false;
            // 
            // rbtnIssue
            // 
            this.rbtnIssue.AutoSize = true;
            this.rbtnIssue.Location = new System.Drawing.Point(110, 11);
            this.rbtnIssue.Name = "rbtnIssue";
            this.rbtnIssue.Size = new System.Drawing.Size(50, 17);
            this.rbtnIssue.TabIndex = 0;
            this.rbtnIssue.TabStop = true;
            this.rbtnIssue.Text = "Issue";
            this.rbtnIssue.UseVisualStyleBackColor = true;
            // 
            // rbtnSale
            // 
            this.rbtnSale.AutoSize = true;
            this.rbtnSale.Location = new System.Drawing.Point(65, 11);
            this.rbtnSale.Name = "rbtnSale";
            this.rbtnSale.Size = new System.Drawing.Size(46, 17);
            this.rbtnSale.TabIndex = 0;
            this.rbtnSale.TabStop = true;
            this.rbtnSale.Text = "Sale";
            this.rbtnSale.UseVisualStyleBackColor = true;
            // 
            // rbtnReceive
            // 
            this.rbtnReceive.AutoSize = true;
            this.rbtnReceive.Location = new System.Drawing.Point(6, 11);
            this.rbtnReceive.Name = "rbtnReceive";
            this.rbtnReceive.Size = new System.Drawing.Size(65, 17);
            this.rbtnReceive.TabIndex = 0;
            this.rbtnReceive.TabStop = true;
            this.rbtnReceive.Text = "Receive";
            this.rbtnReceive.UseVisualStyleBackColor = true;
            // 
            // txtFinishQty
            // 
            this.txtFinishQty.Location = new System.Drawing.Point(372, 39);
            this.txtFinishQty.Name = "txtFinishQty";
            this.txtFinishQty.Size = new System.Drawing.Size(69, 20);
            this.txtFinishQty.TabIndex = 240;
            this.txtFinishQty.Visible = false;
            // 
            // txtFinishItemNo
            // 
            this.txtFinishItemNo.Location = new System.Drawing.Point(305, 39);
            this.txtFinishItemNo.Name = "txtFinishItemNo";
            this.txtFinishItemNo.Size = new System.Drawing.Size(61, 20);
            this.txtFinishItemNo.TabIndex = 239;
            this.txtFinishItemNo.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Location = new System.Drawing.Point(4, 346);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(613, 40);
            this.panel1.TabIndex = 242;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Back;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(545, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 28);
            this.btnCancel.TabIndex = 52;
            this.btnCancel.Text = "&Ok";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(8, 6);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 28);
            this.btnRefresh.TabIndex = 7;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // txtDate
            // 
            this.txtDate.Location = new System.Drawing.Point(242, 39);
            this.txtDate.Name = "txtDate";
            this.txtDate.Size = new System.Drawing.Size(61, 20);
            this.txtDate.TabIndex = 239;
            // 
            // txtVatName
            // 
            this.txtVatName.Location = new System.Drawing.Point(167, 39);
            this.txtVatName.Name = "txtVatName";
            this.txtVatName.Size = new System.Drawing.Size(69, 20);
            this.txtVatName.TabIndex = 240;
            // 
            // FormTracking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(627, 390);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtVatName);
            this.Controls.Add(this.txtFinishQty);
            this.Controls.Add(this.txtDate);
            this.Controls.Add(this.txtFinishItemNo);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.rBtnCodeT);
            this.Controls.Add(this.rBtnNameT);
            this.Controls.Add(this.cmbPCode);
            this.Controls.Add(this.cmbPName);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.label23);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(643, 428);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(643, 428);
            this.Name = "FormTracking";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormTracking";
            this.Load += new System.EventHandler(this.FormTracking_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTracking)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.grpTransaction.ResumeLayout(false);
            this.grpTransaction.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.RadioButton rBtnCodeT;
        private System.Windows.Forms.RadioButton rBtnNameT;
        private System.Windows.Forms.ComboBox cmbPCode;
        private System.Windows.Forms.ComboBox cmbPName;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.DataGridView dgvTracking;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtFinishQty;
        private System.Windows.Forms.TextBox txtFinishItemNo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.TextBox txtDate;
        private System.Windows.Forms.TextBox txtVatName;
        private System.Windows.Forms.GroupBox grpTransaction;
        public System.Windows.Forms.RadioButton rbtnIssue;
        public System.Windows.Forms.RadioButton rbtnSale;
        public System.Windows.Forms.RadioButton rbtnReceive;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn LineNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn PCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Heading1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Heading2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn FinishItemNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Issue;
        private System.Windows.Forms.DataGridViewTextBoxColumn TIssueNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Receive;
        private System.Windows.Forms.DataGridViewTextBoxColumn TReceiveNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sale;
        private System.Windows.Forms.DataGridViewTextBoxColumn TSaleInvoiceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReturnReceive;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReturnSale;
    }
}