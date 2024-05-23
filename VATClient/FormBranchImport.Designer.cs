namespace VATClient
{
    partial class FormBranchImport
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnExport = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.cmbImport = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.lbCount = new System.Windows.Forms.Label();
            this.chkSame = new System.Windows.Forms.CheckBox();
            this.btnImport = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.chkTrack = new System.Windows.Forms.CheckBox();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QuantityS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Heading2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Heading1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProductNameS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemNoS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProductCodeS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LineNoS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvSerialTrack = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSerialTrack)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 71);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(668, 250);
            this.dataGridView1.TabIndex = 178;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnExport.Location = new System.Drawing.Point(293, 42);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(78, 26);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.UseWaitCursor = true;
            this.btnExport.Visible = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(583, 12);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(99, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Double Check";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // cmbImport
            // 
            this.cmbImport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImport.FormattingEnabled = true;
            this.cmbImport.Location = new System.Drawing.Point(12, 12);
            this.cmbImport.Name = "cmbImport";
            this.cmbImport.Size = new System.Drawing.Size(162, 21);
            this.cmbImport.TabIndex = 0;
            this.cmbImport.SelectedIndexChanged += new System.EventHandler(this.cmbImport_SelectedIndexChanged);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSave.Image = global::VATClient.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(12, 39);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(162, 26);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lbCount
            // 
            this.lbCount.AutoSize = true;
            this.lbCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCount.Location = new System.Drawing.Point(12, 339);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(110, 13);
            this.lbCount.TabIndex = 184;
            this.lbCount.Text = "Total Record(s): 0";
            // 
            // chkSame
            // 
            this.chkSame.AutoSize = true;
            this.chkSame.Location = new System.Drawing.Point(184, 14);
            this.chkSame.Name = "chkSame";
            this.chkSame.Size = new System.Drawing.Size(72, 17);
            this.chkSame.TabIndex = 1;
            this.chkSame.TabStop = false;
            this.chkSame.Text = "Same File";
            this.chkSame.UseVisualStyleBackColor = true;
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnImport.Image = global::VATClient.Properties.Resources.Load;
            this.btnImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImport.Location = new System.Drawing.Point(264, 9);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(59, 26);
            this.btnImport.TabIndex = 2;
            this.btnImport.Text = "Load";
            this.btnImport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(191, 325);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 206;
            this.progressBar1.Visible = false;
            // 
            // chkTrack
            // 
            this.chkTrack.AutoSize = true;
            this.chkTrack.Location = new System.Drawing.Point(184, 45);
            this.chkTrack.Name = "chkTrack";
            this.chkTrack.Size = new System.Drawing.Size(103, 17);
            this.chkTrack.TabIndex = 208;
            this.chkTrack.Text = "Show Trackings";
            this.chkTrack.UseVisualStyleBackColor = true;
            this.chkTrack.Visible = false;
            this.chkTrack.CheckedChanged += new System.EventHandler(this.chkTrack_CheckedChanged);
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            this.Value.Visible = false;
            // 
            // QuantityS
            // 
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle19.Format = "N6";
            this.QuantityS.DefaultCellStyle = dataGridViewCellStyle19;
            this.QuantityS.HeaderText = "Quantity";
            this.QuantityS.Name = "QuantityS";
            this.QuantityS.ReadOnly = true;
            this.QuantityS.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.QuantityS.Visible = false;
            // 
            // Heading2
            // 
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle20.Format = "N6";
            dataGridViewCellStyle20.NullValue = null;
            this.Heading2.DefaultCellStyle = dataGridViewCellStyle20;
            this.Heading2.FillWeight = 150F;
            this.Heading2.HeaderText = "Heading2";
            this.Heading2.Name = "Heading2";
            // 
            // Heading1
            // 
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Heading1.DefaultCellStyle = dataGridViewCellStyle21;
            this.Heading1.FillWeight = 150F;
            this.Heading1.HeaderText = "Heading1";
            this.Heading1.MinimumWidth = 10;
            this.Heading1.Name = "Heading1";
            // 
            // ProductNameS
            // 
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ProductNameS.DefaultCellStyle = dataGridViewCellStyle22;
            this.ProductNameS.HeaderText = "Product Name";
            this.ProductNameS.Name = "ProductNameS";
            this.ProductNameS.ReadOnly = true;
            // 
            // ItemNoS
            // 
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ItemNoS.DefaultCellStyle = dataGridViewCellStyle23;
            this.ItemNoS.HeaderText = "Item No";
            this.ItemNoS.Name = "ItemNoS";
            this.ItemNoS.ReadOnly = true;
            this.ItemNoS.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ItemNoS.Visible = false;
            // 
            // ProductCodeS
            // 
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.ProductCodeS.DefaultCellStyle = dataGridViewCellStyle24;
            this.ProductCodeS.HeaderText = "Code";
            this.ProductCodeS.Name = "ProductCodeS";
            this.ProductCodeS.ReadOnly = true;
            // 
            // LineNoS
            // 
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LineNoS.DefaultCellStyle = dataGridViewCellStyle25;
            this.LineNoS.FillWeight = 50F;
            this.LineNoS.HeaderText = "Line No";
            this.LineNoS.Name = "LineNoS";
            this.LineNoS.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvSerialTrack
            // 
            this.dgvSerialTrack.AllowUserToAddRows = false;
            this.dgvSerialTrack.AllowUserToDeleteRows = false;
            this.dgvSerialTrack.AllowUserToOrderColumns = true;
            dataGridViewCellStyle26.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle26.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle26.ForeColor = System.Drawing.Color.Blue;
            this.dgvSerialTrack.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle26;
            this.dgvSerialTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSerialTrack.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSerialTrack.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle27.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle27.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle27.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle27.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle27.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle27.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle27.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSerialTrack.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle27;
            this.dgvSerialTrack.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSerialTrack.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LineNoS,
            this.ProductCodeS,
            this.ItemNoS,
            this.ProductNameS,
            this.Heading1,
            this.Heading2,
            this.QuantityS,
            this.Value});
            this.dgvSerialTrack.GridColor = System.Drawing.SystemColors.HotTrack;
            this.dgvSerialTrack.Location = new System.Drawing.Point(264, 104);
            this.dgvSerialTrack.Name = "dgvSerialTrack";
            this.dgvSerialTrack.RowHeadersVisible = false;
            this.dgvSerialTrack.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSerialTrack.Size = new System.Drawing.Size(298, 167);
            this.dgvSerialTrack.TabIndex = 207;
            // 
            // FormBranchImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(692, 361);
            this.Controls.Add(this.chkTrack);
            this.Controls.Add(this.dgvSerialTrack);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.chkSame);
            this.Controls.Add(this.lbCount);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbImport);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnImport);
            this.MaximizeBox = false;
            this.Name = "FormBranchImport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormImport_FormClosing);
            this.Load += new System.EventHandler(this.FormImport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSerialTrack)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ComboBox cmbImport;
        private System.Windows.Forms.Label lbCount;
        private System.Windows.Forms.CheckBox chkSame;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox chkTrack;
        public System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn QuantityS;
        private System.Windows.Forms.DataGridViewTextBoxColumn Heading2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Heading1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProductNameS;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemNoS;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProductCodeS;
        private System.Windows.Forms.DataGridViewTextBoxColumn LineNoS;
        private System.Windows.Forms.DataGridView dgvSerialTrack;
    }
}