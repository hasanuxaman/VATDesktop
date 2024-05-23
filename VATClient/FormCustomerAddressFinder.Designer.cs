namespace VATClient
{
    partial class FormCustomerAddressFinder
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
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.dgvCustomerAddress = new System.Windows.Forms.DataGridView();
            this.SL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomerAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(94, 4);
            this.txtAddress.MaximumSize = new System.Drawing.Size(400, 20);
            this.txtAddress.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(346, 20);
            this.txtAddress.TabIndex = 0;
            this.txtAddress.TextChanged += new System.EventHandler(this.txtCustName_TextChanged);
            this.txtAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustName_KeyDown);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(444, 333);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 221;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(43, 7);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(45, 13);
            this.label17.TabIndex = 223;
            this.label17.Text = "Address";
            // 
            // dgvCustomerAddress
            // 
            this.dgvCustomerAddress.AllowUserToAddRows = false;
            this.dgvCustomerAddress.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvCustomerAddress.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCustomerAddress.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomerAddress.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SL,
            this.Id,
            this.CustomerID,
            this.CustomerAddress});
            this.dgvCustomerAddress.Location = new System.Drawing.Point(12, 30);
            this.dgvCustomerAddress.Name = "dgvCustomerAddress";
            this.dgvCustomerAddress.RowHeadersVisible = false;
            this.dgvCustomerAddress.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomerAddress.Size = new System.Drawing.Size(507, 297);
            this.dgvCustomerAddress.TabIndex = 225;
            this.dgvCustomerAddress.DoubleClick += new System.EventHandler(this.dgvCustomerFinder_DoubleClick);
            this.dgvCustomerAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvCustomerFinder_KeyDown);
            // 
            // SL
            // 
            this.SL.HeaderText = "SL";
            this.SL.Name = "SL";
            this.SL.ReadOnly = true;
            this.SL.Width = 50;
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            this.Id.Width = 75;
            // 
            // CustomerID
            // 
            this.CustomerID.HeaderText = "CustomerID";
            this.CustomerID.Name = "CustomerID";
            this.CustomerID.ReadOnly = true;
            this.CustomerID.Visible = false;
            // 
            // CustomerAddress
            // 
            this.CustomerAddress.HeaderText = "Address";
            this.CustomerAddress.Name = "CustomerAddress";
            this.CustomerAddress.ReadOnly = true;
            this.CustomerAddress.Width = 350;
            // 
            // FormCustomerAddressFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(531, 361);
            this.Controls.Add(this.dgvCustomerAddress);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtAddress);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(547, 399);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(547, 399);
            this.Name = "FormCustomerAddressFinder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Address Finder (F9 for Close)";
            this.Load += new System.EventHandler(this.FormCustomerFinder_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomerAddress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.DataGridView dgvCustomerAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn SL;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerAddress;
    }
}