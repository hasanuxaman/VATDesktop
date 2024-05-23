namespace VATClient
{
    partial class FormSaleImportNESTLE
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
            this.dgvLoadedTable = new System.Windows.Forms.DataGridView();
            this.Save = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Details = new System.Windows.Forms.DataGridViewButtonColumn();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundSaveSale = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbImportType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpSaleFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpSaleToDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.bgwBigData = new System.ComponentModel.BackgroundWorker();
            this.lblRecord = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvLoadedTable
            // 
            this.dgvLoadedTable.AllowUserToAddRows = false;
            this.dgvLoadedTable.AllowUserToDeleteRows = false;
            this.dgvLoadedTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoadedTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Save,
            this.Details});
            this.dgvLoadedTable.Location = new System.Drawing.Point(12, 83);
            this.dgvLoadedTable.Name = "dgvLoadedTable";
            this.dgvLoadedTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLoadedTable.Size = new System.Drawing.Size(657, 267);
            this.dgvLoadedTable.TabIndex = 6;
            this.dgvLoadedTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLoadedTable_CellContentClick);
            this.dgvLoadedTable.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvLoadedTable_CellMouseDoubleClick);
            this.dgvLoadedTable.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvLoadedTable_CellPainting);
            this.dgvLoadedTable.DoubleClick += new System.EventHandler(this.dgvLoadedTable_DoubleClick);
            // 
            // Save
            // 
            this.Save.HeaderText = "Save";
            this.Save.Name = "Save";
            this.Save.Text = "Add";
            this.Save.Width = 50;
            // 
            // Details
            // 
            this.Details.HeaderText = "Details";
            this.Details.Name = "Details";
            this.Details.Width = 50;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(232, 226);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(181, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRefresh);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cmbImportType);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpSaleFromDate);
            this.groupBox1.Controls.Add(this.dtpSaleToDate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtId);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Location = new System.Drawing.Point(12, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(657, 72);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(280, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "To Date";
            // 
            // cmbImportType
            // 
            this.cmbImportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImportType.FormattingEnabled = true;
            this.cmbImportType.Location = new System.Drawing.Point(567, 28);
            this.cmbImportType.Name = "cmbImportType";
            this.cmbImportType.Size = new System.Drawing.Size(93, 21);
            this.cmbImportType.TabIndex = 13;
            this.cmbImportType.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(163, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "From Date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(593, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Type";
            this.label2.Visible = false;
            // 
            // dtpSaleFromDate
            // 
            this.dtpSaleFromDate.Checked = false;
            this.dtpSaleFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpSaleFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSaleFromDate.Location = new System.Drawing.Point(153, 28);
            this.dtpSaleFromDate.Name = "dtpSaleFromDate";
            this.dtpSaleFromDate.ShowCheckBox = true;
            this.dtpSaleFromDate.Size = new System.Drawing.Size(103, 20);
            this.dtpSaleFromDate.TabIndex = 32;
            // 
            // dtpSaleToDate
            // 
            this.dtpSaleToDate.Checked = false;
            this.dtpSaleToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpSaleToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSaleToDate.Location = new System.Drawing.Point(268, 28);
            this.dtpSaleToDate.Name = "dtpSaleToDate";
            this.dtpSaleToDate.ShowCheckBox = true;
            this.dtpSaleToDate.Size = new System.Drawing.Size(103, 20);
            this.dtpSaleToDate.TabIndex = 31;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Chalan No";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(7, 28);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(138, 20);
            this.txtId.TabIndex = 9;
            // 
            // bgwBigData
            // 
            this.bgwBigData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwBigData_DoWork);
            this.bgwBigData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwBigData_RunWorkerCompleted);
            // 
            // lblRecord
            // 
            this.lblRecord.AutoSize = true;
            this.lblRecord.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecord.Location = new System.Drawing.Point(49, 361);
            this.lblRecord.Name = "lblRecord";
            this.lblRecord.Size = new System.Drawing.Size(96, 16);
            this.lblRecord.TabIndex = 11;
            this.lblRecord.Text = "Record Count: ";
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(456, 23);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(70, 25);
            this.btnRefresh.TabIndex = 35;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.White;
            this.btnLoad.Image = global::VATClient.Properties.Resources.Load;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(386, 25);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(60, 23);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // FormSaleImportNESTLE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(686, 386);
            this.Controls.Add(this.lblRecord);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvLoadedTable);
            this.MaximizeBox = false;
            this.Name = "FormSaleImportNESTLE";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nestle Sale Import";
            this.Load += new System.EventHandler(this.FormMasterImport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.DataGridView dgvLoadedTable;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundSaveSale;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker bgwBigData;
        private System.Windows.Forms.Label lblRecord;
        private System.Windows.Forms.ComboBox cmbImportType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpSaleFromDate;
        private System.Windows.Forms.DateTimePicker dtpSaleToDate;
        private System.Windows.Forms.DataGridViewButtonColumn Save;
        private System.Windows.Forms.DataGridViewButtonColumn Details;
        private System.Windows.Forms.Button btnRefresh;
    }
}