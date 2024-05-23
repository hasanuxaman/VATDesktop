namespace VATClient
{
    partial class FormVAT9_1FileUpload
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVAT9_1FileUpload));
            this.label1 = new System.Windows.Forms.Label();
            this.btnload = new System.Windows.Forms.Button();
            this.cmbNoteNo = new System.Windows.Forms.ComboBox();
            this.txtPeriodName = new System.Windows.Forms.TextBox();
            this.txtPeriodId = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnFileLoad = new System.Windows.Forms.Button();
            this.dgvSubForm = new System.Windows.Forms.DataGridView();
            this.btnRemove = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubForm)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 45);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 17);
            this.label1.TabIndex = 242;
            this.label1.Text = "Note";
            // 
            // btnload
            // 
            this.btnload.BackColor = System.Drawing.Color.White;
            this.btnload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnload.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnload.Location = new System.Drawing.Point(8, 7);
            this.btnload.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnload.Name = "btnload";
            this.btnload.Size = new System.Drawing.Size(106, 27);
            this.btnload.TabIndex = 241;
            this.btnload.Text = "Load";
            this.btnload.UseVisualStyleBackColor = false;
            this.btnload.Click += new System.EventHandler(this.btnload_Click);
            // 
            // cmbNoteNo
            // 
            this.cmbNoteNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNoteNo.FormattingEnabled = true;
            this.cmbNoteNo.Items.AddRange(new object[] {
            "Mushak6_1",
            "Mushak6_2_1",
            "Any Other Documents",
            "Mushak6_10",
            "Note24/29",
            "Note-31",
            "Note-26",
            "Note-27",
            "Note-32"});
            this.cmbNoteNo.Location = new System.Drawing.Point(126, 42);
            this.cmbNoteNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbNoteNo.Name = "cmbNoteNo";
            this.cmbNoteNo.Size = new System.Drawing.Size(285, 24);
            this.cmbNoteNo.TabIndex = 239;
            // 
            // txtPeriodName
            // 
            this.txtPeriodName.Location = new System.Drawing.Point(126, 14);
            this.txtPeriodName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPeriodName.Name = "txtPeriodName";
            this.txtPeriodName.ReadOnly = true;
            this.txtPeriodName.Size = new System.Drawing.Size(160, 22);
            this.txtPeriodName.TabIndex = 243;
            // 
            // txtPeriodId
            // 
            this.txtPeriodId.Location = new System.Drawing.Point(135, 7);
            this.txtPeriodId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPeriodId.Name = "txtPeriodId";
            this.txtPeriodId.ReadOnly = true;
            this.txtPeriodId.Size = new System.Drawing.Size(99, 22);
            this.txtPeriodId.TabIndex = 244;
            this.txtPeriodId.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(225, 94);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(191, 22);
            this.textBox1.TabIndex = 245;
            this.textBox1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(123, 96);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 17);
            this.label2.TabIndex = 246;
            this.label2.Text = "Total Amount";
            this.label2.Visible = false;
            // 
            // btnFileLoad
            // 
            this.btnFileLoad.BackColor = System.Drawing.Color.White;
            this.btnFileLoad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFileLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnFileLoad.Image")));
            this.btnFileLoad.Location = new System.Drawing.Point(8, 47);
            this.btnFileLoad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnFileLoad.Name = "btnFileLoad";
            this.btnFileLoad.Size = new System.Drawing.Size(49, 28);
            this.btnFileLoad.TabIndex = 248;
            this.btnFileLoad.UseVisualStyleBackColor = false;
            this.btnFileLoad.Click += new System.EventHandler(this.btnFileLoad_Click);
            // 
            // dgvSubForm
            // 
            this.dgvSubForm.AllowUserToAddRows = false;
            this.dgvSubForm.AllowUserToDeleteRows = false;
            this.dgvSubForm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSubForm.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.dgvSubForm.Location = new System.Drawing.Point(7, 111);
            this.dgvSubForm.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvSubForm.Name = "dgvSubForm";
            this.dgvSubForm.Size = new System.Drawing.Size(559, 318);
            this.dgvSubForm.TabIndex = 249;
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.White;
            this.btnRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemove.Image = global::VATClient.Properties.Resources.Delete;
            this.btnRemove.Location = new System.Drawing.Point(65, 48);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(49, 28);
            this.btnRemove.TabIndex = 250;
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.panel1.Controls.Add(this.btnFileLoad);
            this.panel1.Controls.Add(this.btnRemove);
            this.panel1.Controls.Add(this.btnload);
            this.panel1.Controls.Add(this.txtPeriodId);
            this.panel1.Location = new System.Drawing.Point(441, 11);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(125, 81);
            this.panel1.TabIndex = 251;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtPeriodName);
            this.panel2.Controls.Add(this.cmbNoteNo);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Location = new System.Drawing.Point(9, 11);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(424, 81);
            this.panel2.TabIndex = 252;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 18);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 17);
            this.label3.TabIndex = 247;
            this.label3.Text = "Fiscal Period";
            // 
            // Select
            // 
            this.Select.DataPropertyName = "none";
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            this.Select.Width = 75;
            // 
            // FormVAT9_1FileUpload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(582, 432);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvSubForm);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormVAT9_1FileUpload";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File Upload";
            this.Load += new System.EventHandler(this.FormVAT9_1FileUpload_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubForm)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnload;
        public System.Windows.Forms.ComboBox cmbNoteNo;
        public System.Windows.Forms.TextBox txtPeriodName;
        public System.Windows.Forms.TextBox txtPeriodId;
        public System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnFileLoad;
        private System.Windows.Forms.DataGridView dgvSubForm;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.Label label3;
    }
}