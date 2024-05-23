namespace VATClient
{
    partial class FormCustomerFinder
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
            this.txtCustCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCustName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dgvCustomerFinder = new System.Windows.Forms.DataGridView();
            this.CustomerID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Address1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VATRegistrationNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomerFinder)).BeginInit();
            this.SuspendLayout();
            // 
            // txtCustCode
            // 
            this.txtCustCode.Location = new System.Drawing.Point(299, 4);
            this.txtCustCode.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCustCode.MinimumSize = new System.Drawing.Size(100, 20);
            this.txtCustCode.Name = "txtCustCode";
            this.txtCustCode.Size = new System.Drawing.Size(100, 20);
            this.txtCustCode.TabIndex = 1;
            this.txtCustCode.TextChanged += new System.EventHandler(this.txtCustCode_TextChanged);
            this.txtCustCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustCode_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(260, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 220;
            this.label4.Text = "Code";
            // 
            // txtCustName
            // 
            this.txtCustName.Location = new System.Drawing.Point(57, 4);
            this.txtCustName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCustName.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtCustName.Name = "txtCustName";
            this.txtCustName.Size = new System.Drawing.Size(150, 20);
            this.txtCustName.TabIndex = 0;
            this.txtCustName.TextChanged += new System.EventHandler(this.txtCustName_TextChanged);
            this.txtCustName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustName_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 218;
            this.label5.Text = "Name";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // dgvCustomerFinder
            // 
            this.dgvCustomerFinder.AllowUserToAddRows = false;
            this.dgvCustomerFinder.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvCustomerFinder.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCustomerFinder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomerFinder.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CustomerID,
            this.CustomerCode,
            this.CustomerName,
            this.Address1,
            this.VATRegistrationNo});
            this.dgvCustomerFinder.Location = new System.Drawing.Point(12, 28);
            this.dgvCustomerFinder.Name = "dgvCustomerFinder";
            this.dgvCustomerFinder.RowHeadersVisible = false;
            this.dgvCustomerFinder.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomerFinder.Size = new System.Drawing.Size(608, 303);
            this.dgvCustomerFinder.TabIndex = 216;
            this.dgvCustomerFinder.TabStop = false;
            this.dgvCustomerFinder.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomerFinder_CellContentClick);
            this.dgvCustomerFinder.DoubleClick += new System.EventHandler(this.dgvCustomerFinder_DoubleClick);
            this.dgvCustomerFinder.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvCustomerFinder_KeyDown);
            // 
            // CustomerID
            // 
            this.CustomerID.HeaderText = "ID";
            this.CustomerID.Name = "CustomerID";
            this.CustomerID.ReadOnly = true;
            this.CustomerID.Width = 50;
            // 
            // CustomerCode
            // 
            this.CustomerCode.HeaderText = "Code";
            this.CustomerCode.Name = "CustomerCode";
            this.CustomerCode.ReadOnly = true;
            // 
            // CustomerName
            // 
            this.CustomerName.HeaderText = "Name";
            this.CustomerName.Name = "CustomerName";
            this.CustomerName.ReadOnly = true;
            this.CustomerName.Width = 150;
            // 
            // Address1
            // 
            this.Address1.HeaderText = "Address";
            this.Address1.Name = "Address1";
            this.Address1.ReadOnly = true;
            this.Address1.Width = 200;
            // 
            // VATRegistrationNo
            // 
            this.VATRegistrationNo.HeaderText = "VAT/BIN ";
            this.VATRegistrationNo.Name = "VATRegistrationNo";
            this.VATRegistrationNo.ReadOnly = true;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(547, 337);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 221;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // FormCustomerFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(634, 361);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtCustCode);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtCustName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dgvCustomerFinder);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(650, 399);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(547, 399);
            this.Name = "FormCustomerFinder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Finder (F9 for Close)";
            this.Load += new System.EventHandler(this.FormCustomerFinder_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomerFinder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCustCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCustName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dgvCustomerFinder;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address1;
        private System.Windows.Forms.DataGridViewTextBoxColumn VATRegistrationNo;
    }
}