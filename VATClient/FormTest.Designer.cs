namespace VATClient
{
    partial class FormTest
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
            this.dgvReceiveHistory = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReceiveHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvReceiveHistory
            // 
            this.dgvReceiveHistory.AllowUserToAddRows = false;
            this.dgvReceiveHistory.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvReceiveHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvReceiveHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReceiveHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.dgvReceiveHistory.Location = new System.Drawing.Point(80, 41);
            this.dgvReceiveHistory.Name = "dgvReceiveHistory";
            this.dgvReceiveHistory.RowHeadersVisible = false;
            this.dgvReceiveHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReceiveHistory.Size = new System.Drawing.Size(385, 144);
            this.dgvReceiveHistory.TabIndex = 220;
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            this.Select.ReadOnly = true;
            // 
            // FormTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 437);
            this.Controls.Add(this.dgvReceiveHistory);
            this.Name = "FormTest";
            this.Text = "FormTest";
            ((System.ComponentModel.ISupportInitialize)(this.dgvReceiveHistory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvReceiveHistory;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
    }
}