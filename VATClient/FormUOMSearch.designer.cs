namespace VATClient
{
    partial class FormUOMSearch
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
            this.UOMInsert = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbActive = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtUOMTo = new System.Windows.Forms.TextBox();
            this.txtUOMFrom = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUOMId = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgUOM = new System.Windows.Forms.DataGridView();
            this.UOMId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UOMFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UOMTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Convertion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CTypes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActiveStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.bntRefresh = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bgwUOMSearch = new System.ComponentModel.BackgroundWorker();
            this.UOMInsert.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgUOM)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UOMInsert
            // 
            this.UOMInsert.Controls.Add(this.label11);
            this.UOMInsert.Controls.Add(this.cmbActive);
            this.UOMInsert.Controls.Add(this.btnAdd);
            this.UOMInsert.Controls.Add(this.btnSearch);
            this.UOMInsert.Controls.Add(this.txtUOMTo);
            this.UOMInsert.Controls.Add(this.txtUOMFrom);
            this.UOMInsert.Controls.Add(this.label3);
            this.UOMInsert.Controls.Add(this.label1);
            this.UOMInsert.Location = new System.Drawing.Point(1, -2);
            this.UOMInsert.Name = "UOMInsert";
            this.UOMInsert.Size = new System.Drawing.Size(485, 66);
            this.UOMInsert.TabIndex = 1;
            this.UOMInsert.TabStop = false;
            this.UOMInsert.Text = "Basic";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(183, 18);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 212;
            this.label11.Text = "Active";
            // 
            // cmbActive
            // 
            this.cmbActive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActive.FormattingEnabled = true;
            this.cmbActive.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbActive.Location = new System.Drawing.Point(269, 15);
            this.cmbActive.Name = "cmbActive";
            this.cmbActive.Size = new System.Drawing.Size(44, 21);
            this.cmbActive.TabIndex = 211;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(404, 60);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 192;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Visible = false;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(404, 11);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtUOMTo
            // 
            this.txtUOMTo.Location = new System.Drawing.Point(47, 38);
            this.txtUOMTo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtUOMTo.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtUOMTo.Name = "txtUOMTo";
            this.txtUOMTo.Size = new System.Drawing.Size(125, 20);
            this.txtUOMTo.TabIndex = 1;
            this.txtUOMTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUOMTo_KeyDown);
            // 
            // txtUOMFrom
            // 
            this.txtUOMFrom.Location = new System.Drawing.Point(47, 15);
            this.txtUOMFrom.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtUOMFrom.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtUOMFrom.Name = "txtUOMFrom";
            this.txtUOMFrom.Size = new System.Drawing.Size(125, 20);
            this.txtUOMFrom.TabIndex = 0;
            this.txtUOMFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUOMFrom_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "To";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "From";
            // 
            // txtUOMId
            // 
            this.txtUOMId.Location = new System.Drawing.Point(405, 66);
            this.txtUOMId.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtUOMId.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtUOMId.Name = "txtUOMId";
            this.txtUOMId.Size = new System.Drawing.Size(125, 20);
            this.txtUOMId.TabIndex = 184;
            this.txtUOMId.TabStop = false;
            this.txtUOMId.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgUOM);
            this.groupBox1.Location = new System.Drawing.Point(1, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(491, 165);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Result";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(97, 77);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.TabIndex = 207;
            this.progressBar1.Visible = false;
            // 
            // dgUOM
            // 
            this.dgUOM.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.dgUOM.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgUOM.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgUOM.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.UOMId,
            this.UOMFrom,
            this.UOMTo,
            this.Convertion,
            this.CTypes,
            this.ActiveStatus});
            this.dgUOM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgUOM.Location = new System.Drawing.Point(3, 16);
            this.dgUOM.Name = "dgUOM";
            this.dgUOM.Size = new System.Drawing.Size(485, 146);
            this.dgUOM.TabIndex = 0;
            this.dgUOM.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgUOM_CellDoubleClick);
            // 
            // UOMId
            // 
            this.UOMId.DataPropertyName = "UOMId";
            this.UOMId.HeaderText = "Id";
            this.UOMId.Name = "UOMId";
            this.UOMId.ReadOnly = true;
            this.UOMId.Visible = false;
            // 
            // UOMFrom
            // 
            this.UOMFrom.DataPropertyName = "UOMFrom";
            this.UOMFrom.HeaderText = "From";
            this.UOMFrom.Name = "UOMFrom";
            this.UOMFrom.ReadOnly = true;
            // 
            // UOMTo
            // 
            this.UOMTo.DataPropertyName = "UOMTo";
            this.UOMTo.HeaderText = "To";
            this.UOMTo.Name = "UOMTo";
            this.UOMTo.ReadOnly = true;
            // 
            // Convertion
            // 
            this.Convertion.DataPropertyName = "Convertion";
            this.Convertion.HeaderText = "Convertion";
            this.Convertion.Name = "Convertion";
            this.Convertion.ReadOnly = true;
            // 
            // CTypes
            // 
            this.CTypes.DataPropertyName = "CTypes";
            this.CTypes.HeaderText = "CTypes";
            this.CTypes.Name = "CTypes";
            this.CTypes.ReadOnly = true;
            this.CTypes.Visible = false;
            // 
            // ActiveStatus
            // 
            this.ActiveStatus.DataPropertyName = "ActiveStatus";
            this.ActiveStatus.HeaderText = "ActiveStatus";
            this.ActiveStatus.Name = "ActiveStatus";
            this.ActiveStatus.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.bntRefresh);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(2, 218);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(487, 36);
            this.panel1.TabIndex = 4;
            this.panel1.TabStop = true;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(101, 10);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 212;
            this.LRecordCount.Text = "Record Count :";
            // 
            // bntRefresh
            // 
            this.bntRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.bntRefresh.Image = global::VATClient.Properties.Resources.Referesh;
            this.bntRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bntRefresh.Location = new System.Drawing.Point(20, 4);
            this.bntRefresh.Name = "bntRefresh";
            this.bntRefresh.Size = new System.Drawing.Size(75, 28);
            this.bntRefresh.TabIndex = 5;
            this.bntRefresh.Text = "&Refresh";
            this.bntRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bntRefresh.UseVisualStyleBackColor = false;
            this.bntRefresh.Click += new System.EventHandler(this.bntRefresh_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button2.Image = global::VATClient.Properties.Resources.Print;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(431, 71);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 28);
            this.button2.TabIndex = 22;
            this.button2.Text = "Report(Grid)";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.Print;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(329, 71);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 28);
            this.button1.TabIndex = 21;
            this.button1.Text = "Report List";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(403, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(96, 59);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // bgwUOMSearch
            // 
            this.bgwUOMSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUOMSearch_DoWork);
            this.bgwUOMSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUOMSearch_RunWorkerCompleted);
            // 
            // FormUOMSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(504, 254);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.UOMInsert);
            this.Controls.Add(this.txtUOMId);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormUOMSearch";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Conversion Search";
            this.Load += new System.EventHandler(this.FormUOMSearch_Load);
            this.UOMInsert.ResumeLayout(false);
            this.UOMInsert.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgUOM)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox UOMInsert;
        private System.Windows.Forms.TextBox txtUOMId;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUOMTo;
        private System.Windows.Forms.TextBox txtUOMFrom;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgUOM;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSearch;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button bntRefresh;
        private System.Windows.Forms.DataGridViewTextBoxColumn UOMId;
        private System.Windows.Forms.DataGridViewTextBoxColumn UOMFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn UOMTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Convertion;
        private System.Windows.Forms.DataGridViewTextBoxColumn CTypes;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActiveStatus;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwUOMSearch;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbActive;
    }
}