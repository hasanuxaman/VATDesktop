namespace VATClient
{
    partial class FormTDSsSearch
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvTDS = new System.Windows.Forms.DataGridView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.Description = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTDSCode = new System.Windows.Forms.TextBox();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSection = new System.Windows.Forms.TextBox();
            this.cmbRecordCount = new System.Windows.Forms.ComboBox();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Section = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTDS)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvTDS);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Location = new System.Drawing.Point(8, 92);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(715, 281);
            this.groupBox1.TabIndex = 107;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(102, 103);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(315, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 106;
            this.progressBar1.Visible = false;
            // 
            // dgvTDS
            // 
            this.dgvTDS.AllowUserToAddRows = false;
            this.dgvTDS.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvTDS.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTDS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTDS.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Section,
            this.Code});
            this.dgvTDS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTDS.Location = new System.Drawing.Point(3, 16);
            this.dgvTDS.Name = "dgvTDS";
            this.dgvTDS.ReadOnly = true;
            this.dgvTDS.RowHeadersVisible = false;
            this.dgvTDS.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTDS.Size = new System.Drawing.Size(709, 262);
            this.dgvTDS.TabIndex = 6;
            this.dgvTDS.DoubleClick += new System.EventHandler(this.dgvTDS_DoubleClick);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(632, 75);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 108;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(12, 376);
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
            this.btnOk.Location = new System.Drawing.Point(395, 65);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 228;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(395, 37);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 227;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(395, 8);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 226;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(80, 55);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(180, 20);
            this.txtDescription.TabIndex = 233;
            // 
            // Description
            // 
            this.Description.AutoSize = true;
            this.Description.Location = new System.Drawing.Point(14, 58);
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(60, 13);
            this.Description.TabIndex = 232;
            this.Description.Text = "Description";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 231;
            this.label1.Text = "Sub Section";
            // 
            // txtTDSCode
            // 
            this.txtTDSCode.Location = new System.Drawing.Point(80, 29);
            this.txtTDSCode.MaximumSize = new System.Drawing.Size(400, 25);
            this.txtTDSCode.MinimumSize = new System.Drawing.Size(145, 20);
            this.txtTDSCode.Name = "txtTDSCode";
            this.txtTDSCode.Size = new System.Drawing.Size(180, 20);
            this.txtTDSCode.TabIndex = 230;
            this.txtTDSCode.TabStop = false;
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 235;
            this.label2.Text = "Section";
            // 
            // txtSection
            // 
            this.txtSection.Location = new System.Drawing.Point(80, 3);
            this.txtSection.MaximumSize = new System.Drawing.Size(400, 25);
            this.txtSection.MinimumSize = new System.Drawing.Size(145, 20);
            this.txtSection.Name = "txtSection";
            this.txtSection.Size = new System.Drawing.Size(180, 20);
            this.txtSection.TabIndex = 234;
            this.txtSection.TabStop = false;
            // 
            // cmbRecordCount
            // 
            this.cmbRecordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordCount.FormattingEnabled = true;
            this.cmbRecordCount.Location = new System.Drawing.Point(91, 76);
            this.cmbRecordCount.Name = "cmbRecordCount";
            this.cmbRecordCount.Size = new System.Drawing.Size(78, 21);
            this.cmbRecordCount.TabIndex = 236;
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            // 
            // Section
            // 
            this.Section.DataPropertyName = "Section";
            this.Section.HeaderText = "Section";
            this.Section.Name = "Section";
            this.Section.ReadOnly = true;
            // 
            // Code
            // 
            this.Code.DataPropertyName = "Code";
            this.Code.HeaderText = "Sub Section";
            this.Code.Name = "Code";
            this.Code.ReadOnly = true;
            // 
            // FormTDSsSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(728, 392);
            this.Controls.Add(this.cmbRecordCount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSection);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.Description);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTDSCode);
            this.Controls.Add(this.LRecordCount);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnCancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTDSsSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormTDSsSearch";
            this.Load += new System.EventHandler(this.FormTDSsSearch_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTDS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvTDS;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label Description;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTDSCode;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSection;
        public System.Windows.Forms.ComboBox cmbRecordCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Section;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
    }
}