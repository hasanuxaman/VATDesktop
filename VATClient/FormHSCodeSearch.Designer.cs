namespace VATClient
{
    partial class FormHSCodeSearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtHSCode = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dgvHSCode = new System.Windows.Forms.DataGridView();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HSCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbRecordCount = new System.Windows.Forms.ComboBox();
            this.btnHSCodeExport = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbFiscalyear = new System.Windows.Forms.ComboBox();
            this.bgwSelectYear = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHSCode)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(45, 19);
            this.txtCode.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCode.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(185, 20);
            this.txtCode.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Code:";
            // 
            // txtHSCode
            // 
            this.txtHSCode.Location = new System.Drawing.Point(292, 19);
            this.txtHSCode.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtHSCode.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtHSCode.Name = "txtHSCode";
            this.txtHSCode.Size = new System.Drawing.Size(185, 20);
            this.txtHSCode.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(236, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "HSCode:";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(488, 15);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dgvHSCode
            // 
            this.dgvHSCode.AllowUserToAddRows = false;
            this.dgvHSCode.AllowUserToDeleteRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvHSCode.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvHSCode.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHSCode.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Code,
            this.Id,
            this.HSCode});
            this.dgvHSCode.Location = new System.Drawing.Point(3, 99);
            this.dgvHSCode.Name = "dgvHSCode";
            this.dgvHSCode.RowHeadersVisible = false;
            this.dgvHSCode.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHSCode.Size = new System.Drawing.Size(582, 272);
            this.dgvHSCode.TabIndex = 10;
            this.dgvHSCode.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHSCode_CellContentClick);
            this.dgvHSCode.DoubleClick += new System.EventHandler(this.dgvHSCode_DoubleClick);
            // 
            // Code
            // 
            this.Code.DataPropertyName = "Code";
            this.Code.HeaderText = "Code";
            this.Code.Name = "Code";
            this.Code.ReadOnly = true;
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Id.Visible = false;
            this.Id.Width = 75;
            // 
            // HSCode
            // 
            this.HSCode.DataPropertyName = "HSCode";
            this.HSCode.HeaderText = "HSCode";
            this.HSCode.Name = "HSCode";
            this.HSCode.Width = 85;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbFiscalyear);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbRecordCount);
            this.groupBox1.Controls.Add(this.btnHSCodeExport);
            this.groupBox1.Controls.Add(this.txtCode);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtHSCode);
            this.groupBox1.Location = new System.Drawing.Point(3, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(582, 82);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Searching Criteria";
            // 
            // cmbRecordCount
            // 
            this.cmbRecordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordCount.FormattingEnabled = true;
            this.cmbRecordCount.Location = new System.Drawing.Point(45, 55);
            this.cmbRecordCount.Name = "cmbRecordCount";
            this.cmbRecordCount.Size = new System.Drawing.Size(78, 21);
            this.cmbRecordCount.TabIndex = 236;
            // 
            // btnHSCodeExport
            // 
            this.btnHSCodeExport.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnHSCodeExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHSCodeExport.Location = new System.Drawing.Point(488, 50);
            this.btnHSCodeExport.Name = "btnHSCodeExport";
            this.btnHSCodeExport.Size = new System.Drawing.Size(75, 28);
            this.btnHSCodeExport.TabIndex = 8;
            this.btnHSCodeExport.Text = "Export";
            this.btnHSCodeExport.UseVisualStyleBackColor = false;
            this.btnHSCodeExport.Click += new System.EventHandler(this.btnHSCodeExport_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Location = new System.Drawing.Point(-51, 375);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(639, 40);
            this.panel1.TabIndex = 12;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Image = global::VATClient.Properties.Resources.Back;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(551, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 28);
            this.button2.TabIndex = 249;
            this.button2.Text = "&Close";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(120, 9);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 229;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(690, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 11;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(159, 154);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(288, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 211;
            this.progressBar1.Visible = false;
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(224, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 237;
            this.label3.Text = "Fiscal Year:";
            // 
            // cmbFiscalyear
            // 
            this.cmbFiscalyear.FormattingEnabled = true;
            this.cmbFiscalyear.Location = new System.Drawing.Point(293, 46);
            this.cmbFiscalyear.Name = "cmbFiscalyear";
            this.cmbFiscalyear.Size = new System.Drawing.Size(121, 21);
            this.cmbFiscalyear.TabIndex = 238;
            // 
            // bgwSelectYear
            // 
            this.bgwSelectYear.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSelectYear_DoWork);
            this.bgwSelectYear.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSelectYear_RunWorkerCompleted);
            // 
            // FormHSCodeSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(587, 414);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgvHSCode);
            this.Name = "FormHSCodeSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HSCodeSearch";
            this.Load += new System.EventHandler(this.FormHSCodeSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHSCode)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtHSCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgvHSCode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnOk;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnHSCodeExport;
        public System.Windows.Forms.ComboBox cmbRecordCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn HSCode;
        private System.Windows.Forms.ComboBox cmbFiscalyear;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker bgwSelectYear;
    }
}