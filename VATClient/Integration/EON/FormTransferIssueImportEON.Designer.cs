namespace VATClient.Integration.EON
{
    partial class FormTransferIssueImportEON
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
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dptTime = new System.Windows.Forms.DateTimePicker();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.dtpSaleFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpSaleToDate = new System.Windows.Forms.DateTimePicker();
            this.chkSame = new System.Windows.Forms.CheckBox();
            this.bgwBigData = new System.ComponentModel.BackgroundWorker();
            this.btnUnprocessed = new System.Windows.Forms.Button();
            this.bgwSaveUnprocessed = new System.ComponentModel.BackgroundWorker();
            this.lblRecord = new System.Windows.Forms.Label();
            this.bgwPost = new System.ComponentModel.BackgroundWorker();
            this.DbNames = new System.Windows.Forms.Label();
            this.cmbDepoNames = new System.Windows.Forms.ComboBox();
            this.chkOldDb = new System.Windows.Forms.CheckBox();
            this.lblDB = new System.Windows.Forms.Label();
            this.cmbDBList = new System.Windows.Forms.ComboBox();
            this.btnSaveTemp = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.cmbImportType = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbIsProcessed = new System.Windows.Forms.ComboBox();
            this.backgroundSaveSale = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvLoadedTable = new System.Windows.Forms.DataGridView();
            this.bgwDataLoad = new System.ComponentModel.BackgroundWorker();
            this.btnLoad = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(433, 10);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 17);
            this.label4.TabIndex = 30;
            this.label4.Text = "To Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(319, 121);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 17);
            this.label1.TabIndex = 29;
            this.label1.Text = "Entry Date Time";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(267, 11);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 17);
            this.label5.TabIndex = 28;
            this.label5.Text = "From Date";
            // 
            // dptTime
            // 
            this.dptTime.CustomFormat = "dd/MMM/yyyy HH:mm:ss";
            this.dptTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dptTime.Location = new System.Drawing.Point(323, 121);
            this.dptTime.Margin = new System.Windows.Forms.Padding(4);
            this.dptTime.Name = "dptTime";
            this.dptTime.ShowCheckBox = true;
            this.dptTime.ShowUpDown = true;
            this.dptTime.Size = new System.Drawing.Size(212, 22);
            this.dptTime.TabIndex = 27;
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(709, 117);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(100, 28);
            this.btnPrint.TabIndex = 26;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(611, 126);
            this.btnPreview.Margin = new System.Windows.Forms.Padding(4);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(100, 28);
            this.btnPreview.TabIndex = 25;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = false;
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.Color.White;
            this.btnPost.Location = new System.Drawing.Point(787, 119);
            this.btnPost.Margin = new System.Windows.Forms.Padding(4);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(100, 28);
            this.btnPost.TabIndex = 24;
            this.btnPost.Text = "Post";
            this.btnPost.UseVisualStyleBackColor = false;
            // 
            // dtpSaleFromDate
            // 
            this.dtpSaleFromDate.Checked = false;
            this.dtpSaleFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpSaleFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSaleFromDate.Location = new System.Drawing.Point(259, 33);
            this.dtpSaleFromDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSaleFromDate.Name = "dtpSaleFromDate";
            this.dtpSaleFromDate.ShowCheckBox = true;
            this.dtpSaleFromDate.Size = new System.Drawing.Size(148, 22);
            this.dtpSaleFromDate.TabIndex = 23;
            // 
            // dtpSaleToDate
            // 
            this.dtpSaleToDate.Checked = false;
            this.dtpSaleToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpSaleToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSaleToDate.Location = new System.Drawing.Point(421, 32);
            this.dtpSaleToDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSaleToDate.Name = "dtpSaleToDate";
            this.dtpSaleToDate.ShowCheckBox = true;
            this.dtpSaleToDate.Size = new System.Drawing.Size(148, 22);
            this.dtpSaleToDate.TabIndex = 22;
            // 
            // chkSame
            // 
            this.chkSame.AutoSize = true;
            this.chkSame.Location = new System.Drawing.Point(833, 32);
            this.chkSame.Margin = new System.Windows.Forms.Padding(4);
            this.chkSame.Name = "chkSame";
            this.chkSame.Size = new System.Drawing.Size(66, 21);
            this.chkSame.TabIndex = 8;
            this.chkSame.Text = "Same";
            this.chkSame.UseVisualStyleBackColor = true;
            this.chkSame.Visible = false;
            // 
            // bgwBigData
            // 
            this.bgwBigData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwBigData_DoWork);
            this.bgwBigData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwBigData_RunWorkerCompleted);
            // 
            // btnUnprocessed
            // 
            this.btnUnprocessed.BackColor = System.Drawing.Color.White;
            this.btnUnprocessed.Location = new System.Drawing.Point(572, 122);
            this.btnUnprocessed.Margin = new System.Windows.Forms.Padding(4);
            this.btnUnprocessed.Name = "btnUnprocessed";
            this.btnUnprocessed.Size = new System.Drawing.Size(100, 28);
            this.btnUnprocessed.TabIndex = 12;
            this.btnUnprocessed.Text = "Process";
            this.btnUnprocessed.UseVisualStyleBackColor = false;
            this.btnUnprocessed.Visible = false;
            // 
            // lblRecord
            // 
            this.lblRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRecord.AutoSize = true;
            this.lblRecord.Location = new System.Drawing.Point(65, 453);
            this.lblRecord.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRecord.Name = "lblRecord";
            this.lblRecord.Size = new System.Drawing.Size(103, 17);
            this.lblRecord.TabIndex = 15;
            this.lblRecord.Text = "Record Count: ";
            // 
            // DbNames
            // 
            this.DbNames.AutoSize = true;
            this.DbNames.Location = new System.Drawing.Point(9, 116);
            this.DbNames.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DbNames.Name = "DbNames";
            this.DbNames.Size = new System.Drawing.Size(98, 17);
            this.DbNames.TabIndex = 17;
            this.DbNames.Text = "Depot  Names";
            this.DbNames.Visible = false;
            // 
            // cmbDepoNames
            // 
            this.cmbDepoNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDepoNames.FormattingEnabled = true;
            this.cmbDepoNames.Location = new System.Drawing.Point(8, 116);
            this.cmbDepoNames.Margin = new System.Windows.Forms.Padding(4);
            this.cmbDepoNames.Name = "cmbDepoNames";
            this.cmbDepoNames.Size = new System.Drawing.Size(145, 24);
            this.cmbDepoNames.TabIndex = 16;
            this.cmbDepoNames.Visible = false;
            // 
            // chkOldDb
            // 
            this.chkOldDb.AutoSize = true;
            this.chkOldDb.Location = new System.Drawing.Point(759, 119);
            this.chkOldDb.Margin = new System.Windows.Forms.Padding(4);
            this.chkOldDb.Name = "chkOldDb";
            this.chkOldDb.Size = new System.Drawing.Size(75, 21);
            this.chkOldDb.TabIndex = 15;
            this.chkOldDb.Text = "Old DB";
            this.chkOldDb.UseVisualStyleBackColor = true;
            this.chkOldDb.Visible = false;
            // 
            // lblDB
            // 
            this.lblDB.AutoSize = true;
            this.lblDB.Location = new System.Drawing.Point(804, 114);
            this.lblDB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDB.Name = "lblDB";
            this.lblDB.Size = new System.Drawing.Size(53, 17);
            this.lblDB.TabIndex = 14;
            this.lblDB.Text = "DB List";
            this.lblDB.Visible = false;
            // 
            // cmbDBList
            // 
            this.cmbDBList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDBList.FormattingEnabled = true;
            this.cmbDBList.Location = new System.Drawing.Point(707, 118);
            this.cmbDBList.Margin = new System.Windows.Forms.Padding(4);
            this.cmbDBList.Name = "cmbDBList";
            this.cmbDBList.Size = new System.Drawing.Size(160, 24);
            this.cmbDBList.TabIndex = 13;
            this.cmbDBList.Visible = false;
            // 
            // btnSaveTemp
            // 
            this.btnSaveTemp.BackColor = System.Drawing.Color.White;
            this.btnSaveTemp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveTemp.Location = new System.Drawing.Point(694, 27);
            this.btnSaveTemp.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveTemp.Name = "btnSaveTemp";
            this.btnSaveTemp.Size = new System.Drawing.Size(85, 28);
            this.btnSaveTemp.TabIndex = 12;
            this.btnSaveTemp.Text = "Save";
            this.btnSaveTemp.UseVisualStyleBackColor = false;
            this.btnSaveTemp.Click += new System.EventHandler(this.btnSaveTemp_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(134, 11);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "ID";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(103, 32);
            this.txtId.Margin = new System.Windows.Forms.Padding(4);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(147, 22);
            this.txtId.TabIndex = 9;
            // 
            // cmbImportType
            // 
            this.cmbImportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImportType.FormattingEnabled = true;
            this.cmbImportType.Location = new System.Drawing.Point(884, 35);
            this.cmbImportType.Margin = new System.Windows.Forms.Padding(4);
            this.cmbImportType.Name = "cmbImportType";
            this.cmbImportType.Size = new System.Drawing.Size(112, 24);
            this.cmbImportType.TabIndex = 2;
            this.cmbImportType.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cmbImportType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cmbIsProcessed);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.dptTime);
            this.groupBox1.Controls.Add(this.btnPrint);
            this.groupBox1.Controls.Add(this.btnPreview);
            this.groupBox1.Controls.Add(this.btnPost);
            this.groupBox1.Controls.Add(this.dtpSaleFromDate);
            this.groupBox1.Controls.Add(this.dtpSaleToDate);
            this.groupBox1.Controls.Add(this.DbNames);
            this.groupBox1.Controls.Add(this.cmbDepoNames);
            this.groupBox1.Controls.Add(this.chkOldDb);
            this.groupBox1.Controls.Add(this.lblDB);
            this.groupBox1.Controls.Add(this.cmbDBList);
            this.groupBox1.Controls.Add(this.btnUnprocessed);
            this.groupBox1.Controls.Add(this.btnSaveTemp);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtId);
            this.groupBox1.Controls.Add(this.chkSame);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(908, 76);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(885, 15);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Type";
            this.label2.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 10);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 17);
            this.label6.TabIndex = 33;
            this.label6.Text = "Processed";
            // 
            // cmbIsProcessed
            // 
            this.cmbIsProcessed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIsProcessed.FormattingEnabled = true;
            this.cmbIsProcessed.Items.AddRange(new object[] {
            "Y",
            "N",
            "All"});
            this.cmbIsProcessed.Location = new System.Drawing.Point(4, 31);
            this.cmbIsProcessed.Margin = new System.Windows.Forms.Padding(4);
            this.cmbIsProcessed.Name = "cmbIsProcessed";
            this.cmbIsProcessed.Size = new System.Drawing.Size(87, 24);
            this.cmbIsProcessed.TabIndex = 32;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(215, 225);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(501, 28);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 13;
            this.progressBar1.Visible = false;
            // 
            // dgvLoadedTable
            // 
            this.dgvLoadedTable.AllowUserToAddRows = false;
            this.dgvLoadedTable.AllowUserToDeleteRows = false;
            this.dgvLoadedTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLoadedTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoadedTable.Location = new System.Drawing.Point(12, 91);
            this.dgvLoadedTable.Margin = new System.Windows.Forms.Padding(4);
            this.dgvLoadedTable.Name = "dgvLoadedTable";
            this.dgvLoadedTable.Size = new System.Drawing.Size(908, 354);
            this.dgvLoadedTable.TabIndex = 12;
            // 
            // bgwDataLoad
            // 
            this.bgwDataLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwDataLoad_DoWork);
            this.bgwDataLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwDataLoad_RunWorkerCompleted);
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.White;
            this.btnLoad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoad.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(603, 28);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(80, 28);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // FormTransferIssueImportEON
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(932, 475);
            this.Controls.Add(this.lblRecord);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvLoadedTable);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FormTransferIssueImportEON";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transfer Import EON";
            this.Load += new System.EventHandler(this.FormTransferIssueImportEON_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dptTime;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.DateTimePicker dtpSaleFromDate;
        private System.Windows.Forms.DateTimePicker dtpSaleToDate;
        private System.Windows.Forms.CheckBox chkSame;
        private System.ComponentModel.BackgroundWorker bgwBigData;
        private System.Windows.Forms.Button btnUnprocessed;
        private System.ComponentModel.BackgroundWorker bgwSaveUnprocessed;
        private System.Windows.Forms.Label lblRecord;
        private System.ComponentModel.BackgroundWorker bgwPost;
        private System.Windows.Forms.Label DbNames;
        private System.Windows.Forms.ComboBox cmbDepoNames;
        private System.Windows.Forms.CheckBox chkOldDb;
        private System.Windows.Forms.Label lblDB;
        private System.Windows.Forms.ComboBox cmbDBList;
        private System.Windows.Forms.Button btnSaveTemp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.ComboBox cmbImportType;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLoad;
        private System.ComponentModel.BackgroundWorker backgroundSaveSale;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvLoadedTable;
        private System.ComponentModel.BackgroundWorker bgwDataLoad;
        private System.Windows.Forms.ComboBox cmbIsProcessed;
        private System.Windows.Forms.Label label6;

    }
}