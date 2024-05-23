namespace VATClient.Integration.Ernst
{
    partial class FormSaleImportErnst
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
            this.bgwAirport = new System.ComponentModel.BackgroundWorker();
            this.bgwIAS = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.bgwErnst = new System.ComponentModel.BackgroundWorker();
            this.bgwSaveUnprocessed = new System.ComponentModel.BackgroundWorker();
            this.bgwBigData = new System.ComponentModel.BackgroundWorker();
            this.bgwReportGeneration = new System.ComponentModel.BackgroundWorker();
            this.lblDB = new System.Windows.Forms.Label();
            this.cmbDBList = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cmbImportType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.backgroundSaveSale = new System.ComponentModel.BackgroundWorker();
            this.chkOldDb = new System.Windows.Forms.CheckBox();
            this.btnDependency = new System.Windows.Forms.Button();
            this.btnSavePdf = new System.Windows.Forms.Button();
            this.chkTest = new System.Windows.Forms.CheckBox();
            this.txtId = new System.Windows.Forms.TextBox();
            this.btnSaveTemp = new System.Windows.Forms.Button();
            this.chkSame = new System.Windows.Forms.CheckBox();
            this.lblRecord = new System.Windows.Forms.Label();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stopBulk = new System.Windows.Forms.Button();
            this.btnBulk = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvLoadedTable = new System.Windows.Forms.DataGridView();
            this.chkIBS = new System.Windows.Forms.CheckBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 30000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // bgwErnst
            // 
            this.bgwErnst.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwErnst_DoWork);
            this.bgwErnst.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwErnst_RunWorkerCompleted);
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
            this.btnSearch.Location = new System.Drawing.Point(933, 96);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 28);
            this.btnSearch.TabIndex = 225;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Visible = false;
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
            // chkOldDb
            // 
            this.chkOldDb.AutoSize = true;
            this.chkOldDb.Location = new System.Drawing.Point(834, 15);
            this.chkOldDb.Margin = new System.Windows.Forms.Padding(4);
            this.chkOldDb.Name = "chkOldDb";
            this.chkOldDb.Size = new System.Drawing.Size(75, 21);
            this.chkOldDb.TabIndex = 15;
            this.chkOldDb.Text = "Old DB";
            this.chkOldDb.UseVisualStyleBackColor = true;
            this.chkOldDb.Visible = false;
            // 
            // btnDependency
            // 
            this.btnDependency.BackColor = System.Drawing.Color.White;
            this.btnDependency.Location = new System.Drawing.Point(940, 88);
            this.btnDependency.Margin = new System.Windows.Forms.Padding(4);
            this.btnDependency.Name = "btnDependency";
            this.btnDependency.Size = new System.Drawing.Size(148, 28);
            this.btnDependency.TabIndex = 224;
            this.btnDependency.Text = "Start Dependency";
            this.btnDependency.UseVisualStyleBackColor = false;
            this.btnDependency.Visible = false;
            // 
            // btnSavePdf
            // 
            this.btnSavePdf.BackColor = System.Drawing.Color.White;
            this.btnSavePdf.ForeColor = System.Drawing.Color.Black;
            this.btnSavePdf.Location = new System.Drawing.Point(936, 183);
            this.btnSavePdf.Margin = new System.Windows.Forms.Padding(4);
            this.btnSavePdf.Name = "btnSavePdf";
            this.btnSavePdf.Size = new System.Drawing.Size(120, 28);
            this.btnSavePdf.TabIndex = 223;
            this.btnSavePdf.Text = "Save Pdf";
            this.btnSavePdf.UseVisualStyleBackColor = false;
            this.btnSavePdf.Visible = false;
            // 
            // chkTest
            // 
            this.chkTest.AutoSize = true;
            this.chkTest.Location = new System.Drawing.Point(957, 98);
            this.chkTest.Margin = new System.Windows.Forms.Padding(4);
            this.chkTest.Name = "chkTest";
            this.chkTest.Size = new System.Drawing.Size(58, 21);
            this.chkTest.TabIndex = 227;
            this.chkTest.Text = "Test";
            this.chkTest.UseVisualStyleBackColor = true;
            this.chkTest.Visible = false;
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(755, 42);
            this.txtId.Margin = new System.Windows.Forms.Padding(4);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(79, 22);
            this.txtId.TabIndex = 221;
            this.txtId.Visible = false;
            // 
            // btnSaveTemp
            // 
            this.btnSaveTemp.BackColor = System.Drawing.Color.White;
            this.btnSaveTemp.Location = new System.Drawing.Point(842, 42);
            this.btnSaveTemp.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveTemp.Name = "btnSaveTemp";
            this.btnSaveTemp.Size = new System.Drawing.Size(115, 28);
            this.btnSaveTemp.TabIndex = 222;
            this.btnSaveTemp.Text = "Save";
            this.btnSaveTemp.UseVisualStyleBackColor = false;
            this.btnSaveTemp.Visible = false;
            // 
            // chkSame
            // 
            this.chkSame.AutoSize = true;
            this.chkSame.Location = new System.Drawing.Point(755, 14);
            this.chkSame.Margin = new System.Windows.Forms.Padding(4);
            this.chkSame.Name = "chkSame";
            this.chkSame.Size = new System.Drawing.Size(66, 21);
            this.chkSame.TabIndex = 8;
            this.chkSame.Text = "Same";
            this.chkSame.UseVisualStyleBackColor = true;
            this.chkSame.Visible = false;
            // 
            // lblRecord
            // 
            this.lblRecord.AutoSize = true;
            this.lblRecord.Location = new System.Drawing.Point(94, 448);
            this.lblRecord.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRecord.Name = "lblRecord";
            this.lblRecord.Size = new System.Drawing.Size(103, 17);
            this.lblRecord.TabIndex = 229;
            this.lblRecord.Text = "Record Count: ";
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(10, 103);
            this.chkSelectAll.Margin = new System.Windows.Forms.Padding(4);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(84, 21);
            this.chkSelectAll.TabIndex = 228;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.stopBulk);
            this.groupBox1.Controls.Add(this.btnBulk);
            this.groupBox1.Controls.Add(this.chkOldDb);
            this.groupBox1.Controls.Add(this.lblDB);
            this.groupBox1.Controls.Add(this.txtId);
            this.groupBox1.Controls.Add(this.cmbDBList);
            this.groupBox1.Controls.Add(this.btnSaveTemp);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.chkSame);
            this.groupBox1.Controls.Add(this.cmbImportType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(10, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(876, 91);
            this.groupBox1.TabIndex = 220;
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
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(34, 254);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(778, 50);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 219;
            this.progressBar1.Visible = false;
            // 
            // dgvLoadedTable
            // 
            this.dgvLoadedTable.AllowUserToAddRows = false;
            this.dgvLoadedTable.AllowUserToDeleteRows = false;
            this.dgvLoadedTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoadedTable.Location = new System.Drawing.Point(10, 131);
            this.dgvLoadedTable.Margin = new System.Windows.Forms.Padding(4);
            this.dgvLoadedTable.Name = "dgvLoadedTable";
            this.dgvLoadedTable.ReadOnly = true;
            this.dgvLoadedTable.Size = new System.Drawing.Size(876, 314);
            this.dgvLoadedTable.TabIndex = 218;
            // 
            // chkIBS
            // 
            this.chkIBS.AutoSize = true;
            this.chkIBS.Checked = true;
            this.chkIBS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIBS.Location = new System.Drawing.Point(199, 109);
            this.chkIBS.Margin = new System.Windows.Forms.Padding(4);
            this.chkIBS.Name = "chkIBS";
            this.chkIBS.Size = new System.Drawing.Size(65, 21);
            this.chkIBS.TabIndex = 226;
            this.chkIBS.Text = "Is IBS";
            this.chkIBS.UseVisualStyleBackColor = true;
            this.chkIBS.Visible = false;
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.White;
            this.btnLoad.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(513, 67);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(80, 28);
            this.btnLoad.TabIndex = 217;
            this.btnLoad.Text = "Load";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Visible = false;
            // 
            // FormSaleImportErnst
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(899, 481);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnDependency);
            this.Controls.Add(this.btnSavePdf);
            this.Controls.Add(this.chkTest);
            this.Controls.Add(this.lblRecord);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvLoadedTable);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.chkIBS);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FormSaleImportErnst";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sale Import Ernst";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSaleImportErnst_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSaleImportErnst_FormClosed);
            this.Load += new System.EventHandler(this.FormSaleImportErnst_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker bgwAirport;
        private System.ComponentModel.BackgroundWorker bgwIAS;
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.BackgroundWorker bgwErnst;
        private System.ComponentModel.BackgroundWorker bgwSaveUnprocessed;
        private System.ComponentModel.BackgroundWorker bgwBigData;
        private System.ComponentModel.BackgroundWorker bgwReportGeneration;
        private System.Windows.Forms.Label lblDB;
        private System.Windows.Forms.ComboBox cmbDBList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cmbImportType;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker backgroundSaveSale;
        private System.Windows.Forms.CheckBox chkOldDb;
        private System.Windows.Forms.Button btnDependency;
        private System.Windows.Forms.Button btnSavePdf;
        private System.Windows.Forms.CheckBox chkTest;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Button btnSaveTemp;
        private System.Windows.Forms.CheckBox chkSame;
        private System.Windows.Forms.Label lblRecord;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button stopBulk;
        private System.Windows.Forms.Button btnBulk;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvLoadedTable;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.CheckBox chkIBS;
    }
}