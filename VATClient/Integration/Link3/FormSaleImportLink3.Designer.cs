namespace VATClient
{
    partial class FormSaleImportLink3
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
            this.components = new System.ComponentModel.Container();
            this.cmbImportType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.dgvLoadedTable = new System.Windows.Forms.DataGridView();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundSaveSale = new System.ComponentModel.BackgroundWorker();
            this.chkSame = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stopBulk = new System.Windows.Forms.Button();
            this.btnBulk = new System.Windows.Forms.Button();
            this.chkOldDb = new System.Windows.Forms.CheckBox();
            this.lblDB = new System.Windows.Forms.Label();
            this.cmbDBList = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.chkIBS = new System.Windows.Forms.CheckBox();
            this.btnDependency = new System.Windows.Forms.Button();
            this.btnSavePdf = new System.Windows.Forms.Button();
            this.btnSaveTemp = new System.Windows.Forms.Button();
            this.txtId = new System.Windows.Forms.TextBox();
            this.chkTest = new System.Windows.Forms.CheckBox();
            this.bgwBigData = new System.ComponentModel.BackgroundWorker();
            this.bgwSaveUnprocessed = new System.ComponentModel.BackgroundWorker();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.lblRecord = new System.Windows.Forms.Label();
            this.bgwDHL = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.bgwIAS = new System.ComponentModel.BackgroundWorker();
            this.bgwReportGeneration = new System.ComponentModel.BackgroundWorker();
            this.bgwAirport = new System.ComponentModel.BackgroundWorker();
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
            this.cmbImportType.Size = new System.Drawing.Size(160, 24);
            this.cmbImportType.TabIndex = 2;
            this.cmbImportType.Visible = false;
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
            this.label2.Visible = false;
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.White;
            this.btnLoad.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(519, 69);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(80, 28);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Visible = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // dgvLoadedTable
            // 
            this.dgvLoadedTable.AllowUserToAddRows = false;
            this.dgvLoadedTable.AllowUserToDeleteRows = false;
            this.dgvLoadedTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoadedTable.Location = new System.Drawing.Point(16, 133);
            this.dgvLoadedTable.Margin = new System.Windows.Forms.Padding(4);
            this.dgvLoadedTable.Name = "dgvLoadedTable";
            this.dgvLoadedTable.ReadOnly = true;
            this.dgvLoadedTable.Size = new System.Drawing.Size(876, 314);
            this.dgvLoadedTable.TabIndex = 6;
            this.dgvLoadedTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLoadedTable_CellContentClick);
            this.dgvLoadedTable.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvLoadedTable_CellMouseDoubleClick);
            this.dgvLoadedTable.DoubleClick += new System.EventHandler(this.dgvLoadedTable_DoubleClick);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(40, 256);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(778, 50);
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
            this.chkSame.Location = new System.Drawing.Point(680, 32);
            this.chkSame.Margin = new System.Windows.Forms.Padding(4);
            this.chkSame.Name = "chkSame";
            this.chkSame.Size = new System.Drawing.Size(66, 21);
            this.chkSame.TabIndex = 8;
            this.chkSame.Text = "Same";
            this.chkSame.UseVisualStyleBackColor = true;
            this.chkSame.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.stopBulk);
            this.groupBox1.Controls.Add(this.btnBulk);
            this.groupBox1.Controls.Add(this.chkOldDb);
            this.groupBox1.Controls.Add(this.lblDB);
            this.groupBox1.Controls.Add(this.cmbDBList);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.chkSame);
            this.groupBox1.Controls.Add(this.cmbImportType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(16, 6);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(876, 91);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // stopBulk
            // 
            this.stopBulk.BackColor = System.Drawing.Color.White;
            this.stopBulk.Location = new System.Drawing.Point(407, 23);
            this.stopBulk.Margin = new System.Windows.Forms.Padding(4);
            this.stopBulk.Name = "stopBulk";
            this.stopBulk.Size = new System.Drawing.Size(118, 42);
            this.stopBulk.TabIndex = 19;
            this.stopBulk.Text = "Process Stop";
            this.stopBulk.UseVisualStyleBackColor = false;
            this.stopBulk.Click += new System.EventHandler(this.stopBulk_Click);
            // 
            // btnBulk
            // 
            this.btnBulk.BackColor = System.Drawing.Color.White;
            this.btnBulk.Location = new System.Drawing.Point(270, 23);
            this.btnBulk.Margin = new System.Windows.Forms.Padding(4);
            this.btnBulk.Name = "btnBulk";
            this.btnBulk.Size = new System.Drawing.Size(114, 42);
            this.btnBulk.TabIndex = 17;
            this.btnBulk.Text = "Process Start";
            this.btnBulk.UseVisualStyleBackColor = false;
            this.btnBulk.Click += new System.EventHandler(this.btnBulk_Click);
            // 
            // chkOldDb
            // 
            this.chkOldDb.AutoSize = true;
            this.chkOldDb.Location = new System.Drawing.Point(759, 33);
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
            this.lblDB.Location = new System.Drawing.Point(724, 90);
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
            this.cmbDBList.Location = new System.Drawing.Point(789, 62);
            this.cmbDBList.Margin = new System.Windows.Forms.Padding(4);
            this.cmbDBList.Name = "cmbDBList";
            this.cmbDBList.Size = new System.Drawing.Size(160, 24);
            this.cmbDBList.TabIndex = 13;
            this.cmbDBList.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(204, -23);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "ID";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(939, 98);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 28);
            this.btnSearch.TabIndex = 20;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Visible = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // chkIBS
            // 
            this.chkIBS.AutoSize = true;
            this.chkIBS.Checked = true;
            this.chkIBS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIBS.Location = new System.Drawing.Point(205, 111);
            this.chkIBS.Margin = new System.Windows.Forms.Padding(4);
            this.chkIBS.Name = "chkIBS";
            this.chkIBS.Size = new System.Drawing.Size(65, 21);
            this.chkIBS.TabIndex = 20;
            this.chkIBS.Text = "Is IBS";
            this.chkIBS.UseVisualStyleBackColor = true;
            this.chkIBS.Visible = false;
            // 
            // btnDependency
            // 
            this.btnDependency.BackColor = System.Drawing.Color.White;
            this.btnDependency.Location = new System.Drawing.Point(946, 90);
            this.btnDependency.Margin = new System.Windows.Forms.Padding(4);
            this.btnDependency.Name = "btnDependency";
            this.btnDependency.Size = new System.Drawing.Size(148, 28);
            this.btnDependency.TabIndex = 18;
            this.btnDependency.Text = "Start Dependency";
            this.btnDependency.UseVisualStyleBackColor = false;
            this.btnDependency.Visible = false;
            this.btnDependency.Click += new System.EventHandler(this.btnDependency_Click);
            // 
            // btnSavePdf
            // 
            this.btnSavePdf.BackColor = System.Drawing.Color.White;
            this.btnSavePdf.ForeColor = System.Drawing.Color.Black;
            this.btnSavePdf.Location = new System.Drawing.Point(942, 185);
            this.btnSavePdf.Margin = new System.Windows.Forms.Padding(4);
            this.btnSavePdf.Name = "btnSavePdf";
            this.btnSavePdf.Size = new System.Drawing.Size(120, 28);
            this.btnSavePdf.TabIndex = 16;
            this.btnSavePdf.Text = "Save Pdf";
            this.btnSavePdf.UseVisualStyleBackColor = false;
            this.btnSavePdf.Visible = false;
            this.btnSavePdf.Click += new System.EventHandler(this.btnSavePdf_Click);
            // 
            // btnSaveTemp
            // 
            this.btnSaveTemp.BackColor = System.Drawing.Color.White;
            this.btnSaveTemp.Location = new System.Drawing.Point(607, 67);
            this.btnSaveTemp.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveTemp.Name = "btnSaveTemp";
            this.btnSaveTemp.Size = new System.Drawing.Size(115, 28);
            this.btnSaveTemp.TabIndex = 12;
            this.btnSaveTemp.Text = "Save";
            this.btnSaveTemp.UseVisualStyleBackColor = false;
            this.btnSaveTemp.Visible = false;
            this.btnSaveTemp.Click += new System.EventHandler(this.btnSaveTemp_Click);
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(267, 68);
            this.txtId.Margin = new System.Windows.Forms.Padding(4);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(243, 22);
            this.txtId.TabIndex = 9;
            this.txtId.Visible = false;
            // 
            // chkTest
            // 
            this.chkTest.AutoSize = true;
            this.chkTest.Location = new System.Drawing.Point(963, 100);
            this.chkTest.Margin = new System.Windows.Forms.Padding(4);
            this.chkTest.Name = "chkTest";
            this.chkTest.Size = new System.Drawing.Size(58, 21);
            this.chkTest.TabIndex = 21;
            this.chkTest.Text = "Test";
            this.chkTest.UseVisualStyleBackColor = true;
            this.chkTest.Visible = false;
            this.chkTest.CheckedChanged += new System.EventHandler(this.chkTest_CheckedChanged);
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
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(16, 105);
            this.chkSelectAll.Margin = new System.Windows.Forms.Padding(4);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(84, 21);
            this.chkSelectAll.TabIndex = 215;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.Visible = false;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // lblRecord
            // 
            this.lblRecord.AutoSize = true;
            this.lblRecord.Location = new System.Drawing.Point(100, 450);
            this.lblRecord.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRecord.Name = "lblRecord";
            this.lblRecord.Size = new System.Drawing.Size(103, 17);
            this.lblRecord.TabIndex = 216;
            this.lblRecord.Text = "Record Count: ";
            // 
            // bgwDHL
            // 
            this.bgwDHL.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwDHL_DoWork);
            this.bgwDHL.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwDHL_RunWorkerCompleted);
            // 
            // timer1
            // 
            this.timer1.Interval = 30000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // bgwIAS
            // 
            this.bgwIAS.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwIAS_DoWork);
            this.bgwIAS.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwIAS_RunWorkerCompleted);
            // 
            // bgwReportGeneration
            // 
            this.bgwReportGeneration.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwReportGeneration_DoWork);
            this.bgwReportGeneration.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwReportGeneration_RunWorkerCompleted);
            // 
            // bgwAirport
            // 
            this.bgwAirport.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwAirport_DoWork);
            this.bgwAirport.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwAirport_RunWorkerCompleted);
            // 
            // FormSaleImportLink3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(932, 475);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnDependency);
            this.Controls.Add(this.btnSavePdf);
            this.Controls.Add(this.chkTest);
            this.Controls.Add(this.lblRecord);
            this.Controls.Add(this.chkIBS);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvLoadedTable);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.btnSaveTemp);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormSaleImportLink3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sale Import";
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
        private System.ComponentModel.BackgroundWorker bgwSaveUnprocessed;
        private System.Windows.Forms.Button btnSaveTemp;
        private System.Windows.Forms.Label lblDB;
        private System.Windows.Forms.ComboBox cmbDBList;
        private System.Windows.Forms.CheckBox chkOldDb;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Label lblRecord;
        private System.Windows.Forms.Button btnSavePdf;
        private System.Windows.Forms.Button btnBulk;
        private System.Windows.Forms.Button btnDependency;
        private System.Windows.Forms.Button stopBulk;
        private System.ComponentModel.BackgroundWorker bgwDHL;
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.BackgroundWorker bgwIAS;
        private System.Windows.Forms.CheckBox chkIBS;
        private System.Windows.Forms.CheckBox chkTest;
        private System.ComponentModel.BackgroundWorker bgwReportGeneration;
        private System.Windows.Forms.Button btnSearch;
        private System.ComponentModel.BackgroundWorker bgwAirport;
    }
}