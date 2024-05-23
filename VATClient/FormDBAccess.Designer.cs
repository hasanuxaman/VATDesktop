namespace VATClient
{
    partial class FormDBAccess
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnExecute = new System.Windows.Forms.Button();
            this.txtSQL = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerExecute = new System.ComponentModel.BackgroundWorker();
            this.dgvUserGroup = new System.Windows.Forms.DataGridView();
            this.backgroundWorkerSelect = new System.ComponentModel.BackgroundWorker();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.txtSQLUpdate = new System.Windows.Forms.TextBox();
            this.btnExecuteUpdate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnExecuteRefresh = new System.Windows.Forms.Button();
            this.btnExecuteUpdateRefresh = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.txtTableName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.SelectManual = new System.Windows.Forms.TabPage();
            this.SelectAuto = new System.Windows.Forms.TabPage();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.Execution = new System.Windows.Forms.TabPage();
            this.tbAvgProcess = new System.Windows.Forms.TabPage();
            this.btn6_2Process = new System.Windows.Forms.Button();
            this.btn6_1Process = new System.Windows.Forms.Button();
            this.btnDbAll = new System.Windows.Forms.Button();
            this.btnReceive = new System.Windows.Forms.Button();
            this.btnSale = new System.Windows.Forms.Button();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.btnAvgPrice = new System.Windows.Forms.Button();
            this.btnStock = new System.Windows.Forms.Button();
            this.bgwAvgStockProcess = new System.ComponentModel.BackgroundWorker();
            this.bgwAdjustmentProcess = new System.ComponentModel.BackgroundWorker();
            this.btnImportStock = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserGroup)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SelectManual.SuspendLayout();
            this.SelectAuto.SuspendLayout();
            this.Execution.SuspendLayout();
            this.tbAvgProcess.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(477, 213);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(145, 26);
            this.btnExecute.TabIndex = 0;
            this.btnExecute.Text = "Execute Select";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtSQL
            // 
            this.txtSQL.Location = new System.Drawing.Point(8, 26);
            this.txtSQL.Multiline = true;
            this.txtSQL.Name = "txtSQL";
            this.txtSQL.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSQL.Size = new System.Drawing.Size(644, 181);
            this.txtSQL.TabIndex = 2;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(198, 480);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(225, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 185;
            this.progressBar1.Visible = false;
            // 
            // backgroundWorkerExecute
            // 
            this.backgroundWorkerExecute.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerExecute_DoWork);
            this.backgroundWorkerExecute.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerExecute_RunWorkerCompleted);
            // 
            // dgvUserGroup
            // 
            this.dgvUserGroup.AllowUserToAddRows = false;
            this.dgvUserGroup.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvUserGroup.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvUserGroup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUserGroup.Location = new System.Drawing.Point(12, 286);
            this.dgvUserGroup.Name = "dgvUserGroup";
            this.dgvUserGroup.RowHeadersVisible = false;
            this.dgvUserGroup.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUserGroup.Size = new System.Drawing.Size(664, 199);
            this.dgvUserGroup.TabIndex = 186;
            // 
            // backgroundWorkerSelect
            // 
            this.backgroundWorkerSelect.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSelect_DoWork);
            this.backgroundWorkerSelect.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSelect_RunWorkerCompleted);
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(9, 488);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 230;
            this.LRecordCount.Text = "Record Count :";
            // 
            // txtSQLUpdate
            // 
            this.txtSQLUpdate.Location = new System.Drawing.Point(6, 19);
            this.txtSQLUpdate.Multiline = true;
            this.txtSQLUpdate.Name = "txtSQLUpdate";
            this.txtSQLUpdate.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSQLUpdate.Size = new System.Drawing.Size(642, 190);
            this.txtSQLUpdate.TabIndex = 231;
            // 
            // btnExecuteUpdate
            // 
            this.btnExecuteUpdate.Location = new System.Drawing.Point(478, 214);
            this.btnExecuteUpdate.Name = "btnExecuteUpdate";
            this.btnExecuteUpdate.Size = new System.Drawing.Size(145, 26);
            this.btnExecuteUpdate.TabIndex = 232;
            this.btnExecuteUpdate.Text = "Execute Update";
            this.btnExecuteUpdate.UseVisualStyleBackColor = true;
            this.btnExecuteUpdate.Click += new System.EventHandler(this.btnExecuteUpdate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 16);
            this.label1.TabIndex = 233;
            this.label1.Text = "Select Query";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 16);
            this.label2.TabIndex = 234;
            this.label2.Text = "Update Query";
            // 
            // btnExecuteRefresh
            // 
            this.btnExecuteRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnExecuteRefresh.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnExecuteRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExecuteRefresh.Location = new System.Drawing.Point(629, 213);
            this.btnExecuteRefresh.Name = "btnExecuteRefresh";
            this.btnExecuteRefresh.Size = new System.Drawing.Size(24, 23);
            this.btnExecuteRefresh.TabIndex = 235;
            this.btnExecuteRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExecuteRefresh.UseVisualStyleBackColor = false;
            this.btnExecuteRefresh.Click += new System.EventHandler(this.btnExecuteRefresh_Click);
            // 
            // btnExecuteUpdateRefresh
            // 
            this.btnExecuteUpdateRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnExecuteUpdateRefresh.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnExecuteUpdateRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExecuteUpdateRefresh.Location = new System.Drawing.Point(629, 216);
            this.btnExecuteUpdateRefresh.Name = "btnExecuteUpdateRefresh";
            this.btnExecuteUpdateRefresh.Size = new System.Drawing.Size(24, 23);
            this.btnExecuteUpdateRefresh.TabIndex = 236;
            this.btnExecuteUpdateRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExecuteUpdateRefresh.UseVisualStyleBackColor = false;
            this.btnExecuteUpdateRefresh.Click += new System.EventHandler(this.btnExecuteUpdateRefresh_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(543, 485);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(95, 23);
            this.btnDownload.TabIndex = 237;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // txtTableName
            // 
            this.txtTableName.Location = new System.Drawing.Point(96, 7);
            this.txtTableName.MaximumSize = new System.Drawing.Size(250, 20);
            this.txtTableName.MinimumSize = new System.Drawing.Size(210, 20);
            this.txtTableName.Name = "txtTableName";
            this.txtTableName.ReadOnly = true;
            this.txtTableName.Size = new System.Drawing.Size(250, 20);
            this.txtTableName.TabIndex = 241;
            this.txtTableName.TextChanged += new System.EventHandler(this.txtTableName_TextChanged);
            this.txtTableName.DoubleClick += new System.EventHandler(this.txtTableName_DoubleClick);
            this.txtTableName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTableName_KeyDown);
            this.txtTableName.Leave += new System.EventHandler(this.txtTableName_Leave);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(4, 10);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(86, 13);
            this.label12.TabIndex = 242;
            this.label12.Text = "Table Name (F9)";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(96, 34);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox1.Size = new System.Drawing.Size(250, 199);
            this.listBox1.TabIndex = 243;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.SelectManual);
            this.tabControl1.Controls.Add(this.SelectAuto);
            this.tabControl1.Controls.Add(this.Execution);
            this.tabControl1.Controls.Add(this.tbAvgProcess);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(668, 268);
            this.tabControl1.TabIndex = 245;
            // 
            // SelectManual
            // 
            this.SelectManual.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.SelectManual.Controls.Add(this.txtSQL);
            this.SelectManual.Controls.Add(this.label1);
            this.SelectManual.Controls.Add(this.btnExecuteRefresh);
            this.SelectManual.Controls.Add(this.btnExecute);
            this.SelectManual.Location = new System.Drawing.Point(4, 22);
            this.SelectManual.Name = "SelectManual";
            this.SelectManual.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.SelectManual.Size = new System.Drawing.Size(660, 242);
            this.SelectManual.TabIndex = 0;
            this.SelectManual.Text = "Select Manual";
            // 
            // SelectAuto
            // 
            this.SelectAuto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.SelectAuto.Controls.Add(this.chkSelectAll);
            this.SelectAuto.Controls.Add(this.button2);
            this.SelectAuto.Controls.Add(this.button3);
            this.SelectAuto.Controls.Add(this.txtTableName);
            this.SelectAuto.Controls.Add(this.label12);
            this.SelectAuto.Controls.Add(this.listBox1);
            this.SelectAuto.Location = new System.Drawing.Point(4, 22);
            this.SelectAuto.Name = "SelectAuto";
            this.SelectAuto.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.SelectAuto.Size = new System.Drawing.Size(660, 242);
            this.SelectAuto.TabIndex = 1;
            this.SelectAuto.Text = "Auto Select";
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(10, 34);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(70, 17);
            this.chkSelectAll.TabIndex = 248;
            this.chkSelectAll.Text = "Select All";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button2.Image = global::VATClient.Properties.Resources.Referesh;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(629, 210);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(24, 23);
            this.button2.TabIndex = 246;
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(356, 206);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(145, 26);
            this.button3.TabIndex = 245;
            this.button3.Text = "Execute Select";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Execution
            // 
            this.Execution.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.Execution.Controls.Add(this.label2);
            this.Execution.Controls.Add(this.txtSQLUpdate);
            this.Execution.Controls.Add(this.btnExecuteUpdate);
            this.Execution.Controls.Add(this.btnExecuteUpdateRefresh);
            this.Execution.Location = new System.Drawing.Point(4, 22);
            this.Execution.Name = "Execution";
            this.Execution.Size = new System.Drawing.Size(660, 242);
            this.Execution.TabIndex = 2;
            this.Execution.Text = "Execution";
            // 
            // tbAvgProcess
            // 
            this.tbAvgProcess.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbAvgProcess.Controls.Add(this.btnImportStock);
            this.tbAvgProcess.Controls.Add(this.btn6_2Process);
            this.tbAvgProcess.Controls.Add(this.btn6_1Process);
            this.tbAvgProcess.Controls.Add(this.btnDbAll);
            this.tbAvgProcess.Controls.Add(this.btnReceive);
            this.tbAvgProcess.Controls.Add(this.btnSale);
            this.tbAvgProcess.Controls.Add(this.progressBar2);
            this.tbAvgProcess.Controls.Add(this.btnAvgPrice);
            this.tbAvgProcess.Controls.Add(this.btnStock);
            this.tbAvgProcess.Location = new System.Drawing.Point(4, 22);
            this.tbAvgProcess.Name = "tbAvgProcess";
            this.tbAvgProcess.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tbAvgProcess.Size = new System.Drawing.Size(660, 242);
            this.tbAvgProcess.TabIndex = 3;
            this.tbAvgProcess.Text = "Stock and Avg Process";
            // 
            // btn6_2Process
            // 
            this.btn6_2Process.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn6_2Process.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn6_2Process.Location = new System.Drawing.Point(285, 58);
            this.btn6_2Process.Name = "btn6_2Process";
            this.btn6_2Process.Size = new System.Drawing.Size(127, 37);
            this.btn6_2Process.TabIndex = 251;
            this.btn6_2Process.Text = "Process 6_2";
            this.btn6_2Process.UseVisualStyleBackColor = false;
            this.btn6_2Process.Click += new System.EventHandler(this.btn6_2Process_Click);
            // 
            // btn6_1Process
            // 
            this.btn6_1Process.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn6_1Process.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn6_1Process.Location = new System.Drawing.Point(152, 58);
            this.btn6_1Process.Name = "btn6_1Process";
            this.btn6_1Process.Size = new System.Drawing.Size(127, 37);
            this.btn6_1Process.TabIndex = 250;
            this.btn6_1Process.Text = "Process 6_1";
            this.btn6_1Process.UseVisualStyleBackColor = false;
            this.btn6_1Process.Click += new System.EventHandler(this.btn6_1Process_Click);
            // 
            // btnDbAll
            // 
            this.btnDbAll.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDbAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDbAll.Location = new System.Drawing.Point(15, 60);
            this.btnDbAll.Name = "btnDbAll";
            this.btnDbAll.Size = new System.Drawing.Size(115, 36);
            this.btnDbAll.TabIndex = 249;
            this.btnDbAll.Text = "DB Migration All ";
            this.btnDbAll.UseVisualStyleBackColor = false;
            this.btnDbAll.Click += new System.EventHandler(this.btnDbAll_Click);
            // 
            // btnReceive
            // 
            this.btnReceive.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnReceive.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReceive.Location = new System.Drawing.Point(424, 17);
            this.btnReceive.Name = "btnReceive";
            this.btnReceive.Size = new System.Drawing.Size(115, 36);
            this.btnReceive.TabIndex = 248;
            this.btnReceive.Text = "Receive Adjustment Process";
            this.btnReceive.UseVisualStyleBackColor = false;
            this.btnReceive.Visible = false;
            this.btnReceive.Click += new System.EventHandler(this.btnReceive_Click);
            // 
            // btnSale
            // 
            this.btnSale.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSale.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSale.Location = new System.Drawing.Point(293, 17);
            this.btnSale.Name = "btnSale";
            this.btnSale.Size = new System.Drawing.Size(115, 36);
            this.btnSale.TabIndex = 247;
            this.btnSale.Text = "Sale Adjustment Process";
            this.btnSale.UseVisualStyleBackColor = false;
            this.btnSale.Visible = false;
            this.btnSale.Click += new System.EventHandler(this.btnSale_Click);
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(15, 130);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(639, 35);
            this.progressBar2.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar2.TabIndex = 246;
            this.progressBar2.Visible = false;
            // 
            // btnAvgPrice
            // 
            this.btnAvgPrice.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAvgPrice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAvgPrice.Location = new System.Drawing.Point(15, 17);
            this.btnAvgPrice.Name = "btnAvgPrice";
            this.btnAvgPrice.Size = new System.Drawing.Size(127, 37);
            this.btnAvgPrice.TabIndex = 10;
            this.btnAvgPrice.Text = "AVG Rate Process";
            this.btnAvgPrice.UseVisualStyleBackColor = false;
            this.btnAvgPrice.Click += new System.EventHandler(this.btnAvgPrice_Click);
            // 
            // btnStock
            // 
            this.btnStock.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnStock.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStock.Location = new System.Drawing.Point(164, 17);
            this.btnStock.Name = "btnStock";
            this.btnStock.Size = new System.Drawing.Size(115, 36);
            this.btnStock.TabIndex = 9;
            this.btnStock.Text = "Fresh Stock Process";
            this.btnStock.UseVisualStyleBackColor = false;
            this.btnStock.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // bgwAvgStockProcess
            // 
            this.bgwAvgStockProcess.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwAvgStockProcess_DoWork);
            this.bgwAvgStockProcess.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwAvgStockProcess_RunWorkerCompleted);
            // 
            // bgwAdjustmentProcess
            // 
            this.bgwAdjustmentProcess.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwAdjustmentProcess_DoWork);
            this.bgwAdjustmentProcess.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwAdjustmentProcess_RunWorkerCompleted);
            // 
            // btnImportStock
            // 
            this.btnImportStock.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnImportStock.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImportStock.Location = new System.Drawing.Point(418, 60);
            this.btnImportStock.Name = "btnImportStock";
            this.btnImportStock.Size = new System.Drawing.Size(127, 37);
            this.btnImportStock.TabIndex = 252;
            this.btnImportStock.Text = "P Stock Import";
            this.btnImportStock.UseVisualStyleBackColor = false;
            this.btnImportStock.Click += new System.EventHandler(this.btnImportStock_Click);
            // 
            // FormDBAccess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(692, 510);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.LRecordCount);
            this.Controls.Add(this.dgvUserGroup);
            this.Controls.Add(this.progressBar1);
            this.MaximizeBox = false;
            this.Name = "FormDBAccess";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DBAccess";
            this.Load += new System.EventHandler(this.FormDBAccess_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserGroup)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.SelectManual.ResumeLayout(false);
            this.SelectManual.PerformLayout();
            this.SelectAuto.ResumeLayout(false);
            this.SelectAuto.PerformLayout();
            this.Execution.ResumeLayout(false);
            this.Execution.PerformLayout();
            this.tbAvgProcess.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TextBox txtSQL;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerExecute;
        private System.Windows.Forms.DataGridView dgvUserGroup;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSelect;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.TextBox txtSQLUpdate;
        private System.Windows.Forms.Button btnExecuteUpdate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnExecuteRefresh;
        private System.Windows.Forms.Button btnExecuteUpdateRefresh;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.TextBox txtTableName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage SelectManual;
        private System.Windows.Forms.TabPage SelectAuto;
        private System.Windows.Forms.TabPage Execution;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.TabPage tbAvgProcess;
        private System.Windows.Forms.Button btnStock;
        private System.Windows.Forms.Button btnAvgPrice;
        private System.ComponentModel.BackgroundWorker bgwAvgStockProcess;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Button btnSale;
        private System.ComponentModel.BackgroundWorker bgwAdjustmentProcess;
        private System.Windows.Forms.Button btnReceive;
        private System.Windows.Forms.Button btnDbAll;
        private System.Windows.Forms.Button btn6_1Process;
        private System.Windows.Forms.Button btn6_2Process;
        private System.Windows.Forms.Button btnImportStock;
    }
}