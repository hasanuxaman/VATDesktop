namespace VATClient
{
    partial class FormAdjustmentNameSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdjustmentNameSearch));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dgvAdjHistory = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbActiveStatus = new System.Windows.Forms.ComboBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.AdjId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdjName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAdjHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(415, 227);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(21, 227);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 28);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            this.btnRefresh.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnRefresh_KeyDown);
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(102, 207);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(288, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 219;
            this.progressBar1.Visible = false;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(415, 6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnVendorGroup_Click);
            this.btnSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnSearch_KeyDown);
            // 
            // dgvAdjHistory
            // 
            this.dgvAdjHistory.AllowUserToAddRows = false;
            this.dgvAdjHistory.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvAdjHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAdjHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAdjHistory.Location = new System.Drawing.Point(21, 39);
            this.dgvAdjHistory.Name = "dgvAdjHistory";
            this.dgvAdjHistory.RowHeadersVisible = false;
            this.dgvAdjHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAdjHistory.Size = new System.Drawing.Size(469, 182);
            this.dgvAdjHistory.TabIndex = 3;
            this.dgvAdjHistory.TabStop = false;
            this.dgvAdjHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAdjHistory_CellContentClick);
            this.dgvAdjHistory.DoubleClick += new System.EventHandler(this.dgvAdjHistory_DoubleClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(281, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 224;
            this.label4.Text = "Active";
            // 
            // cmbActiveStatus
            // 
            this.cmbActiveStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActiveStatus.FormattingEnabled = true;
            this.cmbActiveStatus.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbActiveStatus.Location = new System.Drawing.Point(323, 10);
            this.cmbActiveStatus.Name = "cmbActiveStatus";
            this.cmbActiveStatus.Size = new System.Drawing.Size(55, 21);
            this.cmbActiveStatus.TabIndex = 1;
            this.cmbActiveStatus.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbActiveStatus_KeyDown);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(57, 10);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(207, 20);
            this.txtName.TabIndex = 0;
            this.txtName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtName_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 226;
            this.label1.Text = "Name";
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(102, 237);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 231;
            this.LRecordCount.Text = "Record Count :";
            // 
            // AdjId
            // 
            this.AdjId.HeaderText = "Adj Id";
            this.AdjId.Name = "AdjId";
            this.AdjId.ReadOnly = true;
            this.AdjId.Visible = false;
            // 
            // AdjName
            // 
            this.AdjName.HeaderText = "Adj Name";
            this.AdjName.Name = "AdjName";
            this.AdjName.ReadOnly = true;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            // 
            // FormAdjustmentNameSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(504, 262);
            this.Controls.Add(this.LRecordCount);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvAdjHistory);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbActiveStatus);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnSearch);
            this.MaximizeBox = false;
            this.Name = "FormAdjustmentNameSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Adjustment Name Search";
            this.Load += new System.EventHandler(this.FormAdjustmentNameSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAdjHistory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnRefresh;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgvAdjHistory;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.ComboBox cmbActiveStatus;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdjId;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdjName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
    }
}