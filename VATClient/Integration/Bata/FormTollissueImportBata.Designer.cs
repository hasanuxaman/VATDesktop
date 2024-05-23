namespace VATClient.Integration.Bata
{
    partial class FormTollissueImportBata
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblRecord = new System.Windows.Forms.Label();
            this.bgwBigData = new System.ComponentModel.BackgroundWorker();
            this.dtpPoDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtpPoDateFrom = new System.Windows.Forms.DateTimePicker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvLoadedTable = new System.Windows.Forms.DataGridView();
            this.btnSaveTemp = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtGatePassNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpGpDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtpGpDateFrom = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.bgwLoad = new System.ComponentModel.BackgroundWorker();
            this.btnOk = new System.Windows.Forms.Button();
            this.GATE_PASS_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GP_DATE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ITEM_CODE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ITEM_NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UNIT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransactionQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProcessQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RestQNTY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PO_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PO_DATE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SUPL_CODE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SUPP_NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PLANTCODE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PLANT_NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PRODUCT_NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JOB_NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblRecord
            // 
            this.lblRecord.AutoSize = true;
            this.lblRecord.Location = new System.Drawing.Point(41, 396);
            this.lblRecord.Name = "lblRecord";
            this.lblRecord.Size = new System.Drawing.Size(79, 13);
            this.lblRecord.TabIndex = 15;
            this.lblRecord.Text = "Record Count: ";
            // 
            // bgwBigData
            // 
            this.bgwBigData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwBigData_DoWork);
            this.bgwBigData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwBigData_RunWorkerCompleted);
            // 
            // dtpPoDateTo
            // 
            this.dtpPoDateTo.Checked = false;
            this.dtpPoDateTo.CustomFormat = "dd/MMM/yyyy";
            this.dtpPoDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPoDateTo.Location = new System.Drawing.Point(937, 29);
            this.dtpPoDateTo.Name = "dtpPoDateTo";
            this.dtpPoDateTo.ShowCheckBox = true;
            this.dtpPoDateTo.Size = new System.Drawing.Size(103, 20);
            this.dtpPoDateTo.TabIndex = 17;
            this.dtpPoDateTo.Visible = false;
            // 
            // dtpPoDateFrom
            // 
            this.dtpPoDateFrom.Checked = false;
            this.dtpPoDateFrom.CustomFormat = "dd/MMM/yyyy";
            this.dtpPoDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPoDateFrom.Location = new System.Drawing.Point(828, 29);
            this.dtpPoDateFrom.Name = "dtpPoDateFrom";
            this.dtpPoDateFrom.ShowCheckBox = true;
            this.dtpPoDateFrom.Size = new System.Drawing.Size(103, 20);
            this.dtpPoDateFrom.TabIndex = 16;
            this.dtpPoDateFrom.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(239, 202);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(510, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 13;
            this.progressBar1.Visible = false;
            // 
            // dgvLoadedTable
            // 
            this.dgvLoadedTable.AllowUserToAddRows = false;
            this.dgvLoadedTable.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Blue;
            this.dgvLoadedTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLoadedTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoadedTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.GATE_PASS_NO,
            this.GP_DATE,
            this.ITEM_CODE,
            this.ITEM_NAME,
            this.UNIT,
            this.TotalQuantity,
            this.TransactionQuantity,
            this.ProcessQuantity,
            this.RestQNTY,
            this.PO_NO,
            this.PO_DATE,
            this.SUPL_CODE,
            this.SUPP_NAME,
            this.PLANTCODE,
            this.PLANT_NAME,
            this.PRODUCT_NAME,
            this.JOB_NAME});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Blue;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLoadedTable.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvLoadedTable.Location = new System.Drawing.Point(4, 86);
            this.dgvLoadedTable.Name = "dgvLoadedTable";
            this.dgvLoadedTable.Size = new System.Drawing.Size(973, 297);
            this.dgvLoadedTable.TabIndex = 12;
            this.dgvLoadedTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLoadedTable_CellContentClick);
            this.dgvLoadedTable.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLoadedTable_CellLeave);
            this.dgvLoadedTable.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLoadedTable_CellValueChanged);
            this.dgvLoadedTable.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLoadedTable_RowLeave);
            this.dgvLoadedTable.DoubleClick += new System.EventHandler(this.dgvLoadedTable_DoubleClick);
            // 
            // btnSaveTemp
            // 
            this.btnSaveTemp.BackColor = System.Drawing.Color.White;
            this.btnSaveTemp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveTemp.Location = new System.Drawing.Point(287, 26);
            this.btnSaveTemp.Name = "btnSaveTemp";
            this.btnSaveTemp.Size = new System.Drawing.Size(76, 29);
            this.btnSaveTemp.TabIndex = 12;
            this.btnSaveTemp.Text = "Save";
            this.btnSaveTemp.UseVisualStyleBackColor = false;
            this.btnSaveTemp.Click += new System.EventHandler(this.btnSaveTemp_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(718, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "PO NO";
            this.label3.Visible = false;
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(718, 28);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(104, 20);
            this.txtId.TabIndex = 9;
            this.txtId.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnExport);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtGatePassNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpGpDateTo);
            this.groupBox1.Controls.Add(this.dtpGpDateFrom);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpPoDateTo);
            this.groupBox1.Controls.Add(this.dtpPoDateFrom);
            this.groupBox1.Controls.Add(this.btnSaveTemp);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtId);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(973, 66);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(402, 26);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(86, 29);
            this.btnExport.TabIndex = 284;
            this.btnExport.Text = "Export All";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(128, 10);
            this.label11.MaximumSize = new System.Drawing.Size(10, 10);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(9, 10);
            this.label11.TabIndex = 283;
            this.label11.Text = "*";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 15);
            this.label4.TabIndex = 23;
            this.label4.Text = "GATE PASS NO(F9)";
            // 
            // txtGatePassNo
            // 
            this.txtGatePassNo.Location = new System.Drawing.Point(12, 31);
            this.txtGatePassNo.Name = "txtGatePassNo";
            this.txtGatePassNo.Size = new System.Drawing.Size(142, 20);
            this.txtGatePassNo.TabIndex = 22;
            this.txtGatePassNo.DoubleClick += new System.EventHandler(this.txtGatePassNo_DoubleClick);
            this.txtGatePassNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGatePassNo_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(543, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "GP DATE";
            this.label2.Visible = false;
            // 
            // dtpGpDateTo
            // 
            this.dtpGpDateTo.Checked = false;
            this.dtpGpDateTo.CustomFormat = "dd/MMM/yyyy";
            this.dtpGpDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpGpDateTo.Location = new System.Drawing.Point(652, 28);
            this.dtpGpDateTo.Name = "dtpGpDateTo";
            this.dtpGpDateTo.ShowCheckBox = true;
            this.dtpGpDateTo.Size = new System.Drawing.Size(103, 20);
            this.dtpGpDateTo.TabIndex = 20;
            this.dtpGpDateTo.Visible = false;
            // 
            // dtpGpDateFrom
            // 
            this.dtpGpDateFrom.Checked = false;
            this.dtpGpDateFrom.CustomFormat = "dd/MMM/yyyy";
            this.dtpGpDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpGpDateFrom.Location = new System.Drawing.Point(543, 28);
            this.dtpGpDateFrom.Name = "dtpGpDateFrom";
            this.dtpGpDateFrom.ShowCheckBox = true;
            this.dtpGpDateFrom.Size = new System.Drawing.Size(103, 20);
            this.dtpGpDateFrom.TabIndex = 19;
            this.dtpGpDateFrom.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(828, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "PO DATE";
            this.label1.Visible = false;
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.White;
            this.btnLoad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoad.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(184, 26);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(76, 29);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Search";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // bgwLoad
            // 
            this.bgwLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwLoad_DoWork);
            this.bgwLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwLoad_RunWorkerCompleted);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.Color.White;
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOk.Location = new System.Drawing.Point(861, 389);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(76, 36);
            this.btnOk.TabIndex = 24;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // GATE_PASS_NO
            // 
            this.GATE_PASS_NO.DataPropertyName = "GATE_PASS_NO";
            this.GATE_PASS_NO.HeaderText = "GATE PASS NO";
            this.GATE_PASS_NO.Name = "GATE_PASS_NO";
            this.GATE_PASS_NO.ReadOnly = true;
            // 
            // GP_DATE
            // 
            this.GP_DATE.DataPropertyName = "GP_DATE";
            this.GP_DATE.HeaderText = "GP DATE";
            this.GP_DATE.Name = "GP_DATE";
            this.GP_DATE.ReadOnly = true;
            // 
            // ITEM_CODE
            // 
            this.ITEM_CODE.DataPropertyName = "ITEM_CODE";
            this.ITEM_CODE.HeaderText = "ITEM CODE";
            this.ITEM_CODE.Name = "ITEM_CODE";
            this.ITEM_CODE.ReadOnly = true;
            this.ITEM_CODE.Width = 80;
            // 
            // ITEM_NAME
            // 
            this.ITEM_NAME.DataPropertyName = "ITEM_NAME";
            this.ITEM_NAME.HeaderText = "ITEM NAME";
            this.ITEM_NAME.Name = "ITEM_NAME";
            this.ITEM_NAME.ReadOnly = true;
            // 
            // UNIT
            // 
            this.UNIT.DataPropertyName = "UNIT";
            this.UNIT.HeaderText = "UNIT";
            this.UNIT.Name = "UNIT";
            this.UNIT.ReadOnly = true;
            this.UNIT.Width = 50;
            // 
            // TotalQuantity
            // 
            this.TotalQuantity.DataPropertyName = "TotalQuantity";
            this.TotalQuantity.HeaderText = "Total Quantity";
            this.TotalQuantity.Name = "TotalQuantity";
            this.TotalQuantity.ReadOnly = true;
            // 
            // TransactionQuantity
            // 
            this.TransactionQuantity.DataPropertyName = "TransactionQuantity";
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Lime;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.NullValue = null;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            this.TransactionQuantity.DefaultCellStyle = dataGridViewCellStyle2;
            this.TransactionQuantity.HeaderText = "Transaction Quantity";
            this.TransactionQuantity.Name = "TransactionQuantity";
            // 
            // ProcessQuantity
            // 
            this.ProcessQuantity.DataPropertyName = "ProcessQuantity";
            this.ProcessQuantity.HeaderText = "Process Quantity";
            this.ProcessQuantity.Name = "ProcessQuantity";
            this.ProcessQuantity.ReadOnly = true;
            // 
            // RestQNTY
            // 
            this.RestQNTY.DataPropertyName = "RestQNTY";
            this.RestQNTY.HeaderText = "Rest Quantity";
            this.RestQNTY.Name = "RestQNTY";
            this.RestQNTY.ReadOnly = true;
            // 
            // PO_NO
            // 
            this.PO_NO.DataPropertyName = "PO_NO";
            this.PO_NO.HeaderText = "PO_NO";
            this.PO_NO.Name = "PO_NO";
            this.PO_NO.ReadOnly = true;
            // 
            // PO_DATE
            // 
            this.PO_DATE.DataPropertyName = "PO_DATE";
            this.PO_DATE.HeaderText = "PO_DATE";
            this.PO_DATE.Name = "PO_DATE";
            this.PO_DATE.ReadOnly = true;
            // 
            // SUPL_CODE
            // 
            this.SUPL_CODE.DataPropertyName = "SUPL_CODE";
            this.SUPL_CODE.HeaderText = "SUPL_CODE";
            this.SUPL_CODE.Name = "SUPL_CODE";
            this.SUPL_CODE.ReadOnly = true;
            // 
            // SUPP_NAME
            // 
            this.SUPP_NAME.DataPropertyName = "SUPP_NAME";
            this.SUPP_NAME.HeaderText = "SUPP_NAME";
            this.SUPP_NAME.Name = "SUPP_NAME";
            this.SUPP_NAME.ReadOnly = true;
            // 
            // PLANTCODE
            // 
            this.PLANTCODE.DataPropertyName = "PLANTCODE";
            this.PLANTCODE.HeaderText = "PLANTCODE";
            this.PLANTCODE.Name = "PLANTCODE";
            this.PLANTCODE.ReadOnly = true;
            // 
            // PLANT_NAME
            // 
            this.PLANT_NAME.DataPropertyName = "PLANT_NAME";
            this.PLANT_NAME.HeaderText = "PLANT_NAME";
            this.PLANT_NAME.Name = "PLANT_NAME";
            this.PLANT_NAME.ReadOnly = true;
            // 
            // PRODUCT_NAME
            // 
            this.PRODUCT_NAME.DataPropertyName = "PRODUCT_NAME";
            this.PRODUCT_NAME.HeaderText = "PRODUCT_NAME";
            this.PRODUCT_NAME.Name = "PRODUCT_NAME";
            this.PRODUCT_NAME.ReadOnly = true;
            // 
            // JOB_NAME
            // 
            this.JOB_NAME.DataPropertyName = "JOB_NAME";
            this.JOB_NAME.HeaderText = "JOB_NAME";
            this.JOB_NAME.Name = "JOB_NAME";
            this.JOB_NAME.ReadOnly = true;
            // 
            // FormTollissueImportBata
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(989, 427);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblRecord);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvLoadedTable);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FormTollissueImportBata";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Toll issue 6.4 Import Bata";
            this.Load += new System.EventHandler(this.FormTollissueImportBata_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRecord;
        private System.ComponentModel.BackgroundWorker bgwBigData;
        private System.Windows.Forms.DateTimePicker dtpPoDateTo;
        private System.Windows.Forms.DateTimePicker dtpPoDateFrom;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvLoadedTable;
        private System.Windows.Forms.Button btnSaveTemp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtGatePassNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpGpDateTo;
        private System.Windows.Forms.DateTimePicker dtpGpDateFrom;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker bgwLoad;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.DataGridViewTextBoxColumn GATE_PASS_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn GP_DATE;
        private System.Windows.Forms.DataGridViewTextBoxColumn ITEM_CODE;
        private System.Windows.Forms.DataGridViewTextBoxColumn ITEM_NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn UNIT;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransactionQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProcessQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn RestQNTY;
        private System.Windows.Forms.DataGridViewTextBoxColumn PO_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn PO_DATE;
        private System.Windows.Forms.DataGridViewTextBoxColumn SUPL_CODE;
        private System.Windows.Forms.DataGridViewTextBoxColumn SUPP_NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn PLANTCODE;
        private System.Windows.Forms.DataGridViewTextBoxColumn PLANT_NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn PRODUCT_NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn JOB_NAME;
    }
}