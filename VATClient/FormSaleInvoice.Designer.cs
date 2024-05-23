namespace VATClient
{
    partial class FormSaleInvoice
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvAvgPrice = new System.Windows.Forms.DataGridView();
            this.LineNoR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InvoiceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvgPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InvoiceDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewAvgPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAvgPrice)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAvgPrice
            // 
            this.dgvAvgPrice.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Blue;
            this.dgvAvgPrice.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAvgPrice.Anchor = System.Windows.Forms.AnchorStyles.None;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAvgPrice.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvAvgPrice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAvgPrice.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LineNoR,
            this.InvoiceNo,
            this.ItemNo,
            this.AvgPrice,
            this.InvoiceDateTime,
            this.NewAvgPrice});
            this.dgvAvgPrice.GridColor = System.Drawing.SystemColors.HotTrack;
            this.dgvAvgPrice.Location = new System.Drawing.Point(23, 13);
            this.dgvAvgPrice.Name = "dgvAvgPrice";
            this.dgvAvgPrice.RowHeadersVisible = false;
            this.dgvAvgPrice.Size = new System.Drawing.Size(634, 325);
            this.dgvAvgPrice.TabIndex = 201;
            this.dgvAvgPrice.TabStop = false;
            // 
            // LineNoR
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LineNoR.DefaultCellStyle = dataGridViewCellStyle3;
            this.LineNoR.FillWeight = 50F;
            this.LineNoR.HeaderText = "Line No";
            this.LineNoR.Name = "LineNoR";
            this.LineNoR.ReadOnly = true;
            this.LineNoR.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.LineNoR.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.LineNoR.Width = 60;
            // 
            // InvoiceNo
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "N2";
            this.InvoiceNo.DefaultCellStyle = dataGridViewCellStyle4;
            this.InvoiceNo.HeaderText = "Invoice No";
            this.InvoiceNo.Name = "InvoiceNo";
            this.InvoiceNo.ReadOnly = true;
            this.InvoiceNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.InvoiceNo.Width = 80;
            // 
            // ItemNo
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ItemNo.DefaultCellStyle = dataGridViewCellStyle5;
            this.ItemNo.HeaderText = "Item No";
            this.ItemNo.Name = "ItemNo";
            this.ItemNo.ReadOnly = true;
            this.ItemNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ItemNo.Width = 80;
            // 
            // AvgPrice
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "N2";
            this.AvgPrice.DefaultCellStyle = dataGridViewCellStyle6;
            this.AvgPrice.HeaderText = "Avg Price";
            this.AvgPrice.Name = "AvgPrice";
            this.AvgPrice.ReadOnly = true;
            this.AvgPrice.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.AvgPrice.Width = 75;
            // 
            // InvoiceDateTime
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.Format = "N2";
            this.InvoiceDateTime.DefaultCellStyle = dataGridViewCellStyle7;
            this.InvoiceDateTime.HeaderText = "Invoice Date";
            this.InvoiceDateTime.Name = "InvoiceDateTime";
            this.InvoiceDateTime.ReadOnly = true;
            this.InvoiceDateTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // NewAvgPrice
            // 
            this.NewAvgPrice.HeaderText = "New Avg Price";
            this.NewAvgPrice.Name = "NewAvgPrice";
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(297, 344);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(62, 28);
            this.btnUpdate.TabIndex = 203;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLoad.Image = global::VATClient.Properties.Resources.Save;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(23, 344);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(62, 28);
            this.btnLoad.TabIndex = 202;
            this.btnLoad.Text = "&Load";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.Save;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(194, 344);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(62, 28);
            this.button1.TabIndex = 204;
            this.button1.Text = "&Load";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormSaleInvoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 393);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.dgvAvgPrice);
            this.Name = "FormSaleInvoice";
            this.Text = "FormSaleInvoice";
            this.Load += new System.EventHandler(this.FormSaleInvoice_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAvgPrice)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAvgPrice;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.DataGridViewTextBoxColumn LineNoR;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvoiceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvgPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvoiceDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn NewAvgPrice;
        private System.Windows.Forms.Button button1;

    }
}