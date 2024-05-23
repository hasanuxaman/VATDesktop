namespace VATClient
{
    partial class FormSaleImportKCCL
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundSaveSale = new System.ComponentModel.BackgroundWorker();
            this.chkSame = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.DbNames = new System.Windows.Forms.Label();
            this.cmbDepoNames = new System.Windows.Forms.ComboBox();
            this.chkOldDb = new System.Windows.Forms.CheckBox();
            this.lblDB = new System.Windows.Forms.Label();
            this.cmbDBList = new System.Windows.Forms.ComboBox();
            this.btnUnprocessed = new System.Windows.Forms.Button();
            this.btnSaveTemp = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBigData = new System.Windows.Forms.Button();
            this.txtId = new System.Windows.Forms.TextBox();
            this.bgwBigData = new System.ComponentModel.BackgroundWorker();
            this.bgwSaveUnprocessed = new System.ComponentModel.BackgroundWorker();
            this.lblRecord = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbImportType
            // 
            this.cmbImportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImportType.FormattingEnabled = true;
            this.cmbImportType.Location = new System.Drawing.Point(10, 25);
            this.cmbImportType.Name = "cmbImportType";
            this.cmbImportType.Size = new System.Drawing.Size(121, 21);
            this.cmbImportType.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 9);
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
            this.btnLoad.Location = new System.Drawing.Point(341, 25);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(60, 23);
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
            this.dgvLoadedTable.Location = new System.Drawing.Point(12, 97);
            this.dgvLoadedTable.Name = "dgvLoadedTable";
            this.dgvLoadedTable.Size = new System.Drawing.Size(657, 258);
            this.dgvLoadedTable.TabIndex = 6;
            this.dgvLoadedTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLoadedTable_CellContentClick);
            this.dgvLoadedTable.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvLoadedTable_CellMouseDoubleClick);
            this.dgvLoadedTable.DoubleClick += new System.EventHandler(this.dgvLoadedTable_DoubleClick);
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
            // chkSame
            // 
            this.chkSame.AutoSize = true;
            this.chkSame.Location = new System.Drawing.Point(510, 26);
            this.chkSame.Name = "chkSame";
            this.chkSame.Size = new System.Drawing.Size(53, 17);
            this.chkSame.TabIndex = 8;
            this.chkSame.Text = "Same";
            this.chkSame.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.DbNames);
            this.groupBox1.Controls.Add(this.cmbDepoNames);
            this.groupBox1.Controls.Add(this.chkOldDb);
            this.groupBox1.Controls.Add(this.lblDB);
            this.groupBox1.Controls.Add(this.cmbDBList);
            this.groupBox1.Controls.Add(this.btnUnprocessed);
            this.groupBox1.Controls.Add(this.btnSaveTemp);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnBigData);
            this.groupBox1.Controls.Add(this.txtId);
            this.groupBox1.Controls.Add(this.chkSame);
            this.groupBox1.Controls.Add(this.cmbImportType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Location = new System.Drawing.Point(12, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(657, 95);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // DbNames
            // 
            this.DbNames.AutoSize = true;
            this.DbNames.Location = new System.Drawing.Point(7, 49);
            this.DbNames.Name = "DbNames";
            this.DbNames.Size = new System.Drawing.Size(75, 13);
            this.DbNames.TabIndex = 17;
            this.DbNames.Text = "Depot  Names";
            this.DbNames.Visible = false;
            // 
            // cmbDepoNames
            // 
            this.cmbDepoNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDepoNames.FormattingEnabled = true;
            this.cmbDepoNames.Location = new System.Drawing.Point(6, 65);
            this.cmbDepoNames.Name = "cmbDepoNames";
            this.cmbDepoNames.Size = new System.Drawing.Size(110, 21);
            this.cmbDepoNames.TabIndex = 16;
            this.cmbDepoNames.Visible = false;
            // 
            // chkOldDb
            // 
            this.chkOldDb.AutoSize = true;
            this.chkOldDb.Location = new System.Drawing.Point(569, 27);
            this.chkOldDb.Name = "chkOldDb";
            this.chkOldDb.Size = new System.Drawing.Size(60, 17);
            this.chkOldDb.TabIndex = 15;
            this.chkOldDb.Text = "Old DB";
            this.chkOldDb.UseVisualStyleBackColor = true;
            this.chkOldDb.Visible = false;
            this.chkOldDb.CheckedChanged += new System.EventHandler(this.chkOldDb_CheckedChanged);
            // 
            // lblDB
            // 
            this.lblDB.AutoSize = true;
            this.lblDB.Location = new System.Drawing.Point(537, 50);
            this.lblDB.Name = "lblDB";
            this.lblDB.Size = new System.Drawing.Size(41, 13);
            this.lblDB.TabIndex = 14;
            this.lblDB.Text = "DB List";
            this.lblDB.Visible = false;
            // 
            // cmbDBList
            // 
            this.cmbDBList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDBList.FormattingEnabled = true;
            this.cmbDBList.Location = new System.Drawing.Point(530, 65);
            this.cmbDBList.Name = "cmbDBList";
            this.cmbDBList.Size = new System.Drawing.Size(121, 21);
            this.cmbDBList.TabIndex = 13;
            this.cmbDBList.Visible = false;
            // 
            // btnUnprocessed
            // 
            this.btnUnprocessed.BackColor = System.Drawing.Color.White;
            this.btnUnprocessed.Location = new System.Drawing.Point(429, 50);
            this.btnUnprocessed.Name = "btnUnprocessed";
            this.btnUnprocessed.Size = new System.Drawing.Size(75, 23);
            this.btnUnprocessed.TabIndex = 12;
            this.btnUnprocessed.Text = "Process";
            this.btnUnprocessed.UseVisualStyleBackColor = false;
            this.btnUnprocessed.Visible = false;
            this.btnUnprocessed.Click += new System.EventHandler(this.btnUnprocessed_Click);
            // 
            // btnSaveTemp
            // 
            this.btnSaveTemp.BackColor = System.Drawing.Color.White;
            this.btnSaveTemp.Location = new System.Drawing.Point(429, 23);
            this.btnSaveTemp.Name = "btnSaveTemp";
            this.btnSaveTemp.Size = new System.Drawing.Size(75, 23);
            this.btnSaveTemp.TabIndex = 12;
            this.btnSaveTemp.Text = "Save";
            this.btnSaveTemp.UseVisualStyleBackColor = false;
            this.btnSaveTemp.Click += new System.EventHandler(this.btnSaveTemp_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(153, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "ID";
            // 
            // btnBigData
            // 
            this.btnBigData.BackColor = System.Drawing.Color.White;
            this.btnBigData.Location = new System.Drawing.Point(429, 72);
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
            this.txtId.Location = new System.Drawing.Point(137, 26);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(183, 20);
            this.txtId.TabIndex = 9;
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
            this.lblRecord.Location = new System.Drawing.Point(49, 364);
            this.lblRecord.Name = "lblRecord";
            this.lblRecord.Size = new System.Drawing.Size(79, 13);
            this.lblRecord.TabIndex = 11;
            this.lblRecord.Text = "Record Count: ";
            // 
            // FormSaleImportKCCL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(699, 386);
            this.Controls.Add(this.lblRecord);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvLoadedTable);
            this.Name = "FormSaleImportKCCL";
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
        private System.Windows.Forms.Button btnBigData;
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
    }
}