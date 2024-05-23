namespace VATClient
{
    partial class FormFiscalYear
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFiscalYear));
            this.dgvFYear = new System.Windows.Forms.DataGridView();
            this.LineNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MonthName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PeriodStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PeriodEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Lock = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cmbFiscalYear = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpFYearEnd = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpFYearStart = new System.Windows.Forms.DateTimePicker();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.chkYearLock = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.bgwSelectYear = new System.ComponentModel.BackgroundWorker();
            this.bgwSave = new System.ComponentModel.BackgroundWorker();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFYear)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvFYear
            // 
            this.dgvFYear.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Blue;
            this.dgvFYear.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFYear.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFYear.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFYear.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFYear.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LineNo,
            this.MonthName,
            this.PeriodStart,
            this.PeriodEnd,
            this.Lock});
            this.dgvFYear.GridColor = System.Drawing.SystemColors.HotTrack;
            this.dgvFYear.Location = new System.Drawing.Point(9, 92);
            this.dgvFYear.Name = "dgvFYear";
            this.dgvFYear.RowHeadersVisible = false;
            this.dgvFYear.Size = new System.Drawing.Size(509, 278);
            this.dgvFYear.TabIndex = 59;
            this.dgvFYear.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFYear_CellContentClick);
            // 
            // LineNo
            // 
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LineNo.DefaultCellStyle = dataGridViewCellStyle3;
            this.LineNo.FillWeight = 50F;
            this.LineNo.Frozen = true;
            this.LineNo.HeaderText = "Line No";
            this.LineNo.Name = "LineNo";
            this.LineNo.ReadOnly = true;
            this.LineNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.LineNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.LineNo.Width = 55;
            // 
            // MonthName
            // 
            dataGridViewCellStyle4.Format = "MMMM-yyyy";
            dataGridViewCellStyle4.NullValue = "January-1900";
            this.MonthName.DefaultCellStyle = dataGridViewCellStyle4;
            this.MonthName.HeaderText = "Month Name";
            this.MonthName.Name = "MonthName";
            this.MonthName.Width = 125;
            // 
            // PeriodStart
            // 
            dataGridViewCellStyle5.Format = "dd/MMM/yyyy";
            dataGridViewCellStyle5.NullValue = "01/01/1900";
            this.PeriodStart.DefaultCellStyle = dataGridViewCellStyle5;
            this.PeriodStart.HeaderText = "Period Start";
            this.PeriodStart.Name = "PeriodStart";
            this.PeriodStart.Width = 125;
            // 
            // PeriodEnd
            // 
            dataGridViewCellStyle6.Format = "dd/MMM/yyyy";
            dataGridViewCellStyle6.NullValue = "01/01/1900";
            this.PeriodEnd.DefaultCellStyle = dataGridViewCellStyle6;
            this.PeriodEnd.HeaderText = "Period End";
            this.PeriodEnd.Name = "PeriodEnd";
            this.PeriodEnd.Width = 125;
            // 
            // Lock
            // 
            this.Lock.HeaderText = "Lock";
            this.Lock.Name = "Lock";
            this.Lock.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Lock.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Lock.Width = 75;
            // 
            // cmbFiscalYear
            // 
            this.cmbFiscalYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiscalYear.FormattingEnabled = true;
            this.cmbFiscalYear.Location = new System.Drawing.Point(47, 6);
            this.cmbFiscalYear.Name = "cmbFiscalYear";
            this.cmbFiscalYear.Size = new System.Drawing.Size(141, 21);
            this.cmbFiscalYear.Sorted = true;
            this.cmbFiscalYear.TabIndex = 0;
            this.cmbFiscalYear.SelectedIndexChanged += new System.EventHandler(this.cmbFiscalYear_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.dtpFYearEnd);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.dtpFYearStart);
            this.groupBox2.Location = new System.Drawing.Point(12, 32);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(325, 40);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Fiscal Year";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(167, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 110;
            this.label5.Text = "To";
            // 
            // dtpFYearEnd
            // 
            this.dtpFYearEnd.CustomFormat = "dd/MMM/yyyy";
            this.dtpFYearEnd.Enabled = false;
            this.dtpFYearEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFYearEnd.Location = new System.Drawing.Point(193, 15);
            this.dtpFYearEnd.MaxDate = new System.DateTime(9900, 1, 1, 0, 0, 0, 0);
            this.dtpFYearEnd.MaximumSize = new System.Drawing.Size(4, 20);
            this.dtpFYearEnd.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpFYearEnd.MinimumSize = new System.Drawing.Size(120, 20);
            this.dtpFYearEnd.Name = "dtpFYearEnd";
            this.dtpFYearEnd.Size = new System.Drawing.Size(120, 20);
            this.dtpFYearEnd.TabIndex = 5;
            this.dtpFYearEnd.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpFYearEnd.ValueChanged += new System.EventHandler(this.dtpFYearEnd_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 108;
            this.label3.Text = "Start";
            // 
            // dtpFYearStart
            // 
            this.dtpFYearStart.CustomFormat = "dd/MMM/yyyy";
            this.dtpFYearStart.Enabled = false;
            this.dtpFYearStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFYearStart.Location = new System.Drawing.Point(39, 15);
            this.dtpFYearStart.MaxDate = new System.DateTime(9900, 1, 1, 0, 0, 0, 0);
            this.dtpFYearStart.MaximumSize = new System.Drawing.Size(4, 20);
            this.dtpFYearStart.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpFYearStart.MinimumSize = new System.Drawing.Size(120, 20);
            this.dtpFYearStart.Name = "dtpFYearStart";
            this.dtpFYearStart.Size = new System.Drawing.Size(120, 20);
            this.dtpFYearStart.TabIndex = 4;
            this.dtpFYearStart.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpFYearStart.ValueChanged += new System.EventHandler(this.dtpFYearStart_ValueChanged);
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLoad.Image = global::VATClient.Properties.Resources.Load;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(358, 5);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 28);
            this.btnLoad.TabIndex = 6;
            this.btnLoad.Text = "Load";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.Save;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(358, 33);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // chkYearLock
            // 
            this.chkYearLock.AutoSize = true;
            this.chkYearLock.Location = new System.Drawing.Point(194, 8);
            this.chkYearLock.Name = "chkYearLock";
            this.chkYearLock.Size = new System.Drawing.Size(75, 17);
            this.chkYearLock.TabIndex = 1;
            this.chkYearLock.TabStop = false;
            this.chkYearLock.Text = "Year Lock";
            this.chkYearLock.UseVisualStyleBackColor = true;
            this.chkYearLock.CheckedChanged += new System.EventHandler(this.chkYearLock_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(435, 31);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAddNew.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddNew.Location = new System.Drawing.Point(435, 5);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(75, 28);
            this.btnAddNew.TabIndex = 7;
            this.btnAddNew.Text = "&Add New";
            this.btnAddNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(283, 9);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(70, 17);
            this.chkAll.TabIndex = 2;
            this.chkAll.TabStop = false;
            this.chkAll.Text = "Select All";
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 186;
            this.label1.Text = "Year";
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(127, 205);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(250, 25);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 188;
            this.progressBar.Visible = false;
            // 
            // bgwSelectYear
            // 
            this.bgwSelectYear.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSelectYear_DoWork);
            this.bgwSelectYear.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSelectYear_RunWorkerCompleted);
            // 
            // bgwSave
            // 
            this.bgwSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSave_DoWork);
            this.bgwSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSave_RunWorkerCompleted);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(358, 62);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 9;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // bgwUpdate
            // 
            this.bgwUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUpdate_DoWork);
            this.bgwUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUpdate_RunWorkerCompleted);
            // 
            // FormFiscalYear
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 411);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkAll);
            this.Controls.Add(this.btnAddNew);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.chkYearLock);
            this.Controls.Add(this.dgvFYear);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cmbFiscalYear);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(543, 450);
            this.MinimumSize = new System.Drawing.Size(543, 413);
            this.Name = "FormFiscalYear";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fiscal Year";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormFiscalYear_FormClosing);
            this.Load += new System.EventHandler(this.FormFiscalYear_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFYear)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvFYear;
        public System.Windows.Forms.ComboBox cmbFiscalYear;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpFYearEnd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpFYearStart;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.CheckBox chkYearLock;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn LineNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn MonthName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PeriodStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn PeriodEnd;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Lock;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker bgwSelectYear;
        private System.ComponentModel.BackgroundWorker bgwSave;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnUpdate;
    }
}