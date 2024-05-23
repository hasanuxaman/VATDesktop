namespace VATClient
{
    partial class FormMasterImport
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
            this.cmbTable = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbImportType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.dgvLoadedTable = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundSaveSale = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerPurchaseSave = new System.ComponentModel.BackgroundWorker();
            this.bgwIssueSave = new System.ComponentModel.BackgroundWorker();
            this.bgwReceiveSave = new System.ComponentModel.BackgroundWorker();
            this.bgwVDSSave = new System.ComponentModel.BackgroundWorker();
            this.bgwBOMSave = new System.ComponentModel.BackgroundWorker();
            this.chkSame = new System.Windows.Forms.CheckBox();
            this.bgwTransferIssue = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbTransferTo = new System.Windows.Forms.ComboBox();
            this.btnSaveTemp = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBigData = new System.Windows.Forms.Button();
            this.txtId = new System.Windows.Forms.TextBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.bgwBigData = new System.ComponentModel.BackgroundWorker();
            this.btnUnprocessed = new System.Windows.Forms.Button();
            this.bgwSaveUnprocessed = new System.ComponentModel.BackgroundWorker();
            this.bgwPurchaseBigData = new System.ComponentModel.BackgroundWorker();
            this.bgwIssueBigData = new System.ComponentModel.BackgroundWorker();
            this.bgwReceiveBigData = new System.ComponentModel.BackgroundWorker();
            this.bgwTransferReceive = new System.ComponentModel.BackgroundWorker();
            this.lblRecord = new System.Windows.Forms.Label();
            this.bgwExcellValidation = new System.ComponentModel.BackgroundWorker();
            this.bgwIssueExcellValidation = new System.ComponentModel.BackgroundWorker();
            this.bgwTransferIssueExcellValidation = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbTable
            // 
            this.cmbTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTable.FormattingEnabled = true;
            this.cmbTable.Location = new System.Drawing.Point(5, 27);
            this.cmbTable.Name = "cmbTable";
            this.cmbTable.Size = new System.Drawing.Size(121, 21);
            this.cmbTable.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Table";
            // 
            // cmbImportType
            // 
            this.cmbImportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImportType.FormattingEnabled = true;
            this.cmbImportType.Location = new System.Drawing.Point(135, 27);
            this.cmbImportType.Name = "cmbImportType";
            this.cmbImportType.Size = new System.Drawing.Size(121, 21);
            this.cmbImportType.TabIndex = 2;
            this.cmbImportType.SelectedValueChanged += new System.EventHandler(this.cmbImportType_SelectedValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(137, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Type";
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.White;
            this.btnLoad.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(455, 25);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(60, 23);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.Color.White;
            this.btnImport.Image = global::VATClient.Properties.Resources.Load;
            this.btnImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImport.Location = new System.Drawing.Point(521, 25);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 5;
            this.btnImport.Text = "Save";
            this.btnImport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // dgvLoadedTable
            // 
            this.dgvLoadedTable.AllowUserToAddRows = false;
            this.dgvLoadedTable.AllowUserToDeleteRows = false;
            this.dgvLoadedTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoadedTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.dgvLoadedTable.Location = new System.Drawing.Point(17, 88);
            this.dgvLoadedTable.Name = "dgvLoadedTable";
            this.dgvLoadedTable.Size = new System.Drawing.Size(657, 276);
            this.dgvLoadedTable.TabIndex = 6;
            this.dgvLoadedTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLoadedTable_CellContentClick);
            this.dgvLoadedTable.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvLoadedTable_CellMouseDoubleClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
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
            // backgroundSaveSale
            // 
            this.backgroundSaveSale.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundSaveSale_DoWork);
            this.backgroundSaveSale.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundSaveSale_RunWorkerCompleted);
            // 
            // backgroundWorkerPurchaseSave
            // 
            this.backgroundWorkerPurchaseSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPurchaseSave_DoWork);
            this.backgroundWorkerPurchaseSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPurchaseSave_RunWorkerCompleted);
            // 
            // bgwIssueSave
            // 
            this.bgwIssueSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwIssueSave_DoWork);
            this.bgwIssueSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwIssueSave_RunWorkerCompleted);
            // 
            // bgwReceiveSave
            // 
            this.bgwReceiveSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwReceiveSave_DoWork);
            this.bgwReceiveSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwReceiveSave_RunWorkerCompleted);
            // 
            // bgwVDSSave
            // 
            this.bgwVDSSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwVDSSave_DoWork);
            this.bgwVDSSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwVDSSave_RunWorkerCompleted);
            // 
            // bgwBOMSave
            // 
            this.bgwBOMSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwBOMSave_DoWork);
            this.bgwBOMSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwBOMSave_RunWorkerCompleted);
            // 
            // chkSame
            // 
            this.chkSame.AutoSize = true;
            this.chkSame.Location = new System.Drawing.Point(599, 27);
            this.chkSame.Name = "chkSame";
            this.chkSame.Size = new System.Drawing.Size(53, 17);
            this.chkSame.TabIndex = 8;
            this.chkSame.Text = "Same";
            this.chkSame.UseVisualStyleBackColor = true;
            // 
            // bgwTransferIssue
            // 
            this.bgwTransferIssue.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwTransferIssue_DoWork);
            this.bgwTransferIssue.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwTransferIssue_RunWorkerCompleted);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbTransferTo);
            this.groupBox1.Controls.Add(this.btnSaveTemp);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnBigData);
            this.groupBox1.Controls.Add(this.txtId);
            this.groupBox1.Controls.Add(this.cmbTable);
            this.groupBox1.Controls.Add(this.chkSame);
            this.groupBox1.Controls.Add(this.cmbImportType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnImport);
            this.groupBox1.Location = new System.Drawing.Point(12, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(657, 77);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // cmbTransferTo
            // 
            this.cmbTransferTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransferTo.FormattingEnabled = true;
            this.cmbTransferTo.Location = new System.Drawing.Point(135, 51);
            this.cmbTransferTo.Name = "cmbTransferTo";
            this.cmbTransferTo.Size = new System.Drawing.Size(121, 21);
            this.cmbTransferTo.TabIndex = 13;
            this.cmbTransferTo.Visible = false;
            // 
            // btnSaveTemp
            // 
            this.btnSaveTemp.BackColor = System.Drawing.Color.White;
            this.btnSaveTemp.Location = new System.Drawing.Point(521, 49);
            this.btnSaveTemp.Name = "btnSaveTemp";
            this.btnSaveTemp.Size = new System.Drawing.Size(75, 23);
            this.btnSaveTemp.TabIndex = 12;
            this.btnSaveTemp.Text = "Save Temp";
            this.btnSaveTemp.UseVisualStyleBackColor = false;
            this.btnSaveTemp.Visible = false;
            this.btnSaveTemp.Click += new System.EventHandler(this.btnSaveTemp_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(263, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "ID";
            // 
            // btnBigData
            // 
            this.btnBigData.BackColor = System.Drawing.Color.White;
            this.btnBigData.Location = new System.Drawing.Point(343, 49);
            this.btnBigData.Name = "btnBigData";
            this.btnBigData.Size = new System.Drawing.Size(85, 23);
            this.btnBigData.TabIndex = 11;
            this.btnBigData.Text = "Test";
            this.btnBigData.UseVisualStyleBackColor = false;
            this.btnBigData.Visible = false;
            this.btnBigData.Click += new System.EventHandler(this.btnBigData_Click);
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(263, 27);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(183, 20);
            this.txtId.TabIndex = 9;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(59, 54);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(70, 17);
            this.chkSelectAll.TabIndex = 10;
            this.chkSelectAll.Text = "Select All";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.Click += new System.EventHandler(this.chkSelectAll_Click);
            // 
            // bgwBigData
            // 
            this.bgwBigData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwBigData_DoWork);
            this.bgwBigData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwBigData_RunWorkerCompleted);
            // 
            // btnUnprocessed
            // 
            this.btnUnprocessed.BackColor = System.Drawing.Color.White;
            this.btnUnprocessed.Location = new System.Drawing.Point(467, 54);
            this.btnUnprocessed.Name = "btnUnprocessed";
            this.btnUnprocessed.Size = new System.Drawing.Size(60, 23);
            this.btnUnprocessed.TabIndex = 12;
            this.btnUnprocessed.Text = "Process";
            this.btnUnprocessed.UseVisualStyleBackColor = false;
            this.btnUnprocessed.Visible = false;
            this.btnUnprocessed.Click += new System.EventHandler(this.btnUnprocessed_Click);
            // 
            // bgwSaveUnprocessed
            // 
            this.bgwSaveUnprocessed.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSaveUnprocessed_DoWork);
            this.bgwSaveUnprocessed.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSaveUnprocessed_RunWorkerCompleted);
            // 
            // bgwPurchaseBigData
            // 
            this.bgwPurchaseBigData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPurchaseBigData_DoWork);
            this.bgwPurchaseBigData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPurchaseBigData_RunWorkerCompleted);
            // 
            // bgwIssueBigData
            // 
            this.bgwIssueBigData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwIssueBigData_DoWork);
            this.bgwIssueBigData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwIssueBigData_RunWorkerCompleted);
            // 
            // bgwReceiveBigData
            // 
            this.bgwReceiveBigData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwReceiveBigData_DoWork);
            this.bgwReceiveBigData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwReceiveBigData_RunWorkerCompleted);
            // 
            // bgwTransferReceive
            // 
            this.bgwTransferReceive.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwTransferReceive_DoWork);
            this.bgwTransferReceive.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwTransferReceive_RunWorkerCompleted);
            // 
            // lblRecord
            // 
            this.lblRecord.AutoSize = true;
            this.lblRecord.Location = new System.Drawing.Point(68, 367);
            this.lblRecord.Name = "lblRecord";
            this.lblRecord.Size = new System.Drawing.Size(79, 13);
            this.lblRecord.TabIndex = 13;
            this.lblRecord.Text = "Record Count: ";
            // 
            // bgwExcellValidation
            // 
            this.bgwExcellValidation.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwExcellValidation_DoWork);
            this.bgwExcellValidation.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwExcellValidation_RunWorkerCompleted);
            // 
            // bgwIssueExcellValidation
            // 
            this.bgwIssueExcellValidation.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwIssueExcellValidation_DoWork);
            this.bgwIssueExcellValidation.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwIssueExcellValidation_RunWorkerCompleted);
            // 
            // bgwTransferIssueExcellValidation
            // 
            this.bgwTransferIssueExcellValidation.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwTransferIssueExcellValidation_DoWork);
            this.bgwTransferIssueExcellValidation.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwTransferIssueExcellValidation_RunWorkerCompleted);
            // 
            // FormMasterImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(699, 386);
            this.Controls.Add(this.lblRecord);
            this.Controls.Add(this.btnUnprocessed);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvLoadedTable);
            this.Name = "FormMasterImport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Master Import";
            this.Load += new System.EventHandler(this.FormMasterImport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbImportType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.DataGridView dgvLoadedTable;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundSaveSale;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPurchaseSave;
        private System.ComponentModel.BackgroundWorker bgwIssueSave;
        private System.ComponentModel.BackgroundWorker bgwReceiveSave;
        private System.ComponentModel.BackgroundWorker bgwVDSSave;
        private System.ComponentModel.BackgroundWorker bgwBOMSave;
        private System.Windows.Forms.CheckBox chkSame;
        private System.Windows.Forms.Button btnImport;
        private System.ComponentModel.BackgroundWorker bgwTransferIssue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBigData;
        private System.ComponentModel.BackgroundWorker bgwBigData;
        private System.Windows.Forms.Button btnUnprocessed;
        private System.ComponentModel.BackgroundWorker bgwSaveUnprocessed;
        private System.Windows.Forms.Button btnSaveTemp;
        private System.ComponentModel.BackgroundWorker bgwPurchaseBigData;
        private System.ComponentModel.BackgroundWorker bgwIssueBigData;
        private System.ComponentModel.BackgroundWorker bgwReceiveBigData;
        private System.ComponentModel.BackgroundWorker bgwTransferReceive;
        private System.Windows.Forms.Label lblRecord;
        private System.Windows.Forms.ComboBox cmbTransferTo;
        private System.ComponentModel.BackgroundWorker bgwExcellValidation;
        private System.ComponentModel.BackgroundWorker bgwIssueExcellValidation;
        private System.ComponentModel.BackgroundWorker bgwTransferIssueExcellValidation;
    }
}