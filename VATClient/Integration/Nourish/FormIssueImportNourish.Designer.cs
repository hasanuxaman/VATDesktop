namespace VATClient
{
    partial class FormIssueImportNourish
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
            this.cmbImportType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.dgvLoadedTable = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundSaveSale = new System.ComponentModel.BackgroundWorker();
            this.chkSame = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dptTime = new System.Windows.Forms.DateTimePicker();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.dtpSaleFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpSaleToDate = new System.Windows.Forms.DateTimePicker();
            this.DbNames = new System.Windows.Forms.Label();
            this.cmbDepoNames = new System.Windows.Forms.ComboBox();
            this.chkOldDb = new System.Windows.Forms.CheckBox();
            this.lblDB = new System.Windows.Forms.Label();
            this.cmbDBList = new System.Windows.Forms.ComboBox();
            this.btnUnprocessed = new System.Windows.Forms.Button();
            this.btnSaveTemp = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.bgwBigData = new System.ComponentModel.BackgroundWorker();
            this.bgwSaveUnprocessed = new System.ComponentModel.BackgroundWorker();
            this.lblRecord = new System.Windows.Forms.Label();
            this.bgwPost = new System.ComponentModel.BackgroundWorker();
            this.bgwSaleLoad = new System.ComponentModel.BackgroundWorker();
            this.bgwDetails = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbImportType
            // 
            this.cmbImportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImportType.FormattingEnabled = true;
            this.cmbImportType.Location = new System.Drawing.Point(13, 31);
            this.cmbImportType.Margin = new System.Windows.Forms.Padding(4);
            this.cmbImportType.Name = "cmbImportType";
            this.cmbImportType.Size = new System.Drawing.Size(123, 24);
            this.cmbImportType.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Type";
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.White;
            this.btnLoad.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(441, 31);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(80, 28);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // dgvLoadedTable
            // 
            this.dgvLoadedTable.AllowUserToAddRows = false;
            this.dgvLoadedTable.AllowUserToDeleteRows = false;
            this.dgvLoadedTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoadedTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.dgvLoadedTable.Location = new System.Drawing.Point(16, 119);
            this.dgvLoadedTable.Margin = new System.Windows.Forms.Padding(4);
            this.dgvLoadedTable.Name = "dgvLoadedTable";
            this.dgvLoadedTable.Size = new System.Drawing.Size(876, 318);
            this.dgvLoadedTable.TabIndex = 6;
            this.dgvLoadedTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLoadedTable_CellContentClick);
            this.dgvLoadedTable.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvLoadedTable_CellMouseDoubleClick);
            this.dgvLoadedTable.DoubleClick += new System.EventHandler(this.dgvLoadedTable_DoubleClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(309, 278);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(241, 28);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Visible = false;
            // 
            // backgroundSaveSale
            // 
            this.backgroundSaveSale.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundSaveSale_DoWork);
            this.backgroundSaveSale.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundSaveSale_RunWorkerCompleted);
            // 
            // chkSame
            // 
            this.chkSame.AutoSize = true;
            this.chkSame.Location = new System.Drawing.Point(657, 37);
            this.chkSame.Margin = new System.Windows.Forms.Padding(4);
            this.chkSame.Name = "chkSame";
            this.chkSame.Size = new System.Drawing.Size(66, 21);
            this.chkSame.TabIndex = 8;
            this.chkSame.Text = "Same";
            this.chkSame.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSelectAll);
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
            this.groupBox1.Controls.Add(this.cmbImportType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Location = new System.Drawing.Point(16, 6);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(876, 117);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(8, 77);
            this.chkSelectAll.Margin = new System.Windows.Forms.Padding(4);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(88, 21);
            this.chkSelectAll.TabIndex = 31;
            this.chkSelectAll.Text = "Select All";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.Click += new System.EventHandler(this.chkSelectAll_Click_1);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(306, 56);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 17);
            this.label4.TabIndex = 30;
            this.label4.Text = "To Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(409, 126);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 17);
            this.label1.TabIndex = 29;
            this.label1.Text = "Entry Date Time";
            this.label1.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(163, 56);
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
            this.dptTime.Location = new System.Drawing.Point(389, 145);
            this.dptTime.Margin = new System.Windows.Forms.Padding(4);
            this.dptTime.Name = "dptTime";
            this.dptTime.ShowCheckBox = true;
            this.dptTime.ShowUpDown = true;
            this.dptTime.Size = new System.Drawing.Size(212, 22);
            this.dptTime.TabIndex = 27;
            this.dptTime.Visible = false;
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(755, 119);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(100, 28);
            this.btnPrint.TabIndex = 26;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Visible = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(657, 121);
            this.btnPreview.Margin = new System.Windows.Forms.Padding(4);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(100, 28);
            this.btnPreview.TabIndex = 25;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Visible = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.Color.White;
            this.btnPost.Location = new System.Drawing.Point(559, 121);
            this.btnPost.Margin = new System.Windows.Forms.Padding(4);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(100, 28);
            this.btnPost.TabIndex = 24;
            this.btnPost.Text = "Post";
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Visible = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // dtpSaleFromDate
            // 
            this.dtpSaleFromDate.Checked = false;
            this.dtpSaleFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpSaleFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSaleFromDate.Location = new System.Drawing.Point(136, 77);
            this.dtpSaleFromDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSaleFromDate.Name = "dtpSaleFromDate";
            this.dtpSaleFromDate.ShowCheckBox = true;
            this.dtpSaleFromDate.Size = new System.Drawing.Size(136, 22);
            this.dtpSaleFromDate.TabIndex = 23;
            // 
            // dtpSaleToDate
            // 
            this.dtpSaleToDate.Checked = false;
            this.dtpSaleToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpSaleToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSaleToDate.Location = new System.Drawing.Point(283, 77);
            this.dtpSaleToDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSaleToDate.Name = "dtpSaleToDate";
            this.dtpSaleToDate.ShowCheckBox = true;
            this.dtpSaleToDate.Size = new System.Drawing.Size(136, 22);
            this.dtpSaleToDate.TabIndex = 22;
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
            this.chkOldDb.CheckedChanged += new System.EventHandler(this.chkOldDb_CheckedChanged);
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
            // btnUnprocessed
            // 
            this.btnUnprocessed.BackColor = System.Drawing.Color.White;
            this.btnUnprocessed.Location = new System.Drawing.Point(572, 121);
            this.btnUnprocessed.Margin = new System.Windows.Forms.Padding(4);
            this.btnUnprocessed.Name = "btnUnprocessed";
            this.btnUnprocessed.Size = new System.Drawing.Size(100, 28);
            this.btnUnprocessed.TabIndex = 12;
            this.btnUnprocessed.Text = "Process";
            this.btnUnprocessed.UseVisualStyleBackColor = false;
            this.btnUnprocessed.Visible = false;
            this.btnUnprocessed.Click += new System.EventHandler(this.btnUnprocessed_Click);
            // 
            // btnSaveTemp
            // 
            this.btnSaveTemp.BackColor = System.Drawing.Color.White;
            this.btnSaveTemp.Location = new System.Drawing.Point(537, 31);
            this.btnSaveTemp.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveTemp.Name = "btnSaveTemp";
            this.btnSaveTemp.Size = new System.Drawing.Size(100, 28);
            this.btnSaveTemp.TabIndex = 12;
            this.btnSaveTemp.Text = "Save";
            this.btnSaveTemp.UseVisualStyleBackColor = false;
            this.btnSaveTemp.Click += new System.EventHandler(this.btnSaveTemp_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(164, 11);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "ID";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(152, 32);
            this.txtId.Margin = new System.Windows.Forms.Padding(4);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(243, 22);
            this.txtId.TabIndex = 9;
            this.txtId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtId_KeyDown);
            // 
            // bgwBigData
            // 
            this.bgwBigData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwBigData_DoWork);
            this.bgwBigData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwBigData_RunWorkerCompleted);
            // 
            // bgwSaveUnprocessed
            // 
            this.bgwSaveUnprocessed.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSaveUnprocessed_DoWork);
            this.bgwSaveUnprocessed.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSaveUnprocessed_RunWorkerCompleted);
            // 
            // lblRecord
            // 
            this.lblRecord.AutoSize = true;
            this.lblRecord.Location = new System.Drawing.Point(65, 448);
            this.lblRecord.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRecord.Name = "lblRecord";
            this.lblRecord.Size = new System.Drawing.Size(103, 17);
            this.lblRecord.TabIndex = 11;
            this.lblRecord.Text = "Record Count: ";
            // 
            // bgwPost
            // 
            this.bgwPost.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPost_DoWork);
            this.bgwPost.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPost_RunWorkerCompleted);
            // 
            // bgwSaleLoad
            // 
            this.bgwSaleLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSaleLoad_DoWork);
            this.bgwSaleLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSaleLoad_RunWorkerCompleted);
            // 
            // bgwDetails
            // 
            this.bgwDetails.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwDetails_DoWork);
            this.bgwDetails.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwDetails_RunWorkerCompleted);
            // 
            // FormIssueImportNourish
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(932, 475);
            this.Controls.Add(this.lblRecord);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvLoadedTable);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormIssueImportNourish";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Issue Import";
            this.Load += new System.EventHandler(this.FormMasterImport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbImportType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.DataGridView dgvLoadedTable;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundSaveSale;
        private System.Windows.Forms.CheckBox chkSame;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker bgwBigData;
        private System.Windows.Forms.Button btnUnprocessed;
        private System.ComponentModel.BackgroundWorker bgwSaveUnprocessed;
        private System.Windows.Forms.Button btnSaveTemp;
        private System.Windows.Forms.Label lblDB;
        private System.Windows.Forms.ComboBox cmbDBList;
        private System.Windows.Forms.CheckBox chkOldDb;
        private System.Windows.Forms.Label lblRecord;
        private System.Windows.Forms.Label DbNames;
        private System.Windows.Forms.ComboBox cmbDepoNames;
        private System.Windows.Forms.DateTimePicker dtpSaleFromDate;
        private System.Windows.Forms.DateTimePicker dtpSaleToDate;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnPost;
        private System.ComponentModel.BackgroundWorker bgwPost;
        private System.Windows.Forms.DateTimePicker dptTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker bgwSaleLoad;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.ComponentModel.BackgroundWorker bgwDetails;
    }
}