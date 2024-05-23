namespace VATClient
{
    partial class FormImportProductPriceHistory
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
            this.bgwSave = new System.ComponentModel.BackgroundWorker();
            this.QuantityS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Heading2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Heading1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProductNameS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemNoS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProductCodeS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LineNoS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.chkSame = new System.Windows.Forms.CheckBox();
            this.lbCount = new System.Windows.Forms.Label();
            this.bgwProduct = new System.ComponentModel.BackgroundWorker();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // bgwSave
            // 
            this.bgwSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSave_DoWork);
            this.bgwSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSave_RunWorkerCompleted);
            // 
            // QuantityS
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "N6";
            this.QuantityS.DefaultCellStyle = dataGridViewCellStyle1;
            this.QuantityS.HeaderText = "Quantity";
            this.QuantityS.Name = "QuantityS";
            this.QuantityS.ReadOnly = true;
            this.QuantityS.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.QuantityS.Visible = false;
            // 
            // Heading2
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Format = "N6";
            dataGridViewCellStyle2.NullValue = null;
            this.Heading2.DefaultCellStyle = dataGridViewCellStyle2;
            this.Heading2.FillWeight = 150F;
            this.Heading2.HeaderText = "Heading2";
            this.Heading2.Name = "Heading2";
            // 
            // Heading1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Heading1.DefaultCellStyle = dataGridViewCellStyle3;
            this.Heading1.FillWeight = 150F;
            this.Heading1.HeaderText = "Heading1";
            this.Heading1.MinimumWidth = 10;
            this.Heading1.Name = "Heading1";
            // 
            // ProductNameS
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ProductNameS.DefaultCellStyle = dataGridViewCellStyle4;
            this.ProductNameS.HeaderText = "Product Name";
            this.ProductNameS.Name = "ProductNameS";
            this.ProductNameS.ReadOnly = true;
            // 
            // ItemNoS
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ItemNoS.DefaultCellStyle = dataGridViewCellStyle5;
            this.ItemNoS.HeaderText = "Item No";
            this.ItemNoS.Name = "ItemNoS";
            this.ItemNoS.ReadOnly = true;
            this.ItemNoS.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ItemNoS.Visible = false;
            // 
            // ProductCodeS
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.ProductCodeS.DefaultCellStyle = dataGridViewCellStyle6;
            this.ProductCodeS.HeaderText = "Code";
            this.ProductCodeS.Name = "ProductCodeS";
            this.ProductCodeS.ReadOnly = true;
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            this.Value.Visible = false;
            // 
            // LineNoS
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LineNoS.DefaultCellStyle = dataGridViewCellStyle7;
            this.LineNoS.FillWeight = 50F;
            this.LineNoS.HeaderText = "Line No";
            this.LineNoS.Name = "LineNoS";
            this.LineNoS.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(279, 396);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(387, 33);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 213;
            this.progressBar1.Visible = false;
            // 
            // chkSame
            // 
            this.chkSame.AutoSize = true;
            this.chkSame.Location = new System.Drawing.Point(239, 24);
            this.chkSame.Margin = new System.Windows.Forms.Padding(4);
            this.chkSame.Name = "chkSame";
            this.chkSame.Size = new System.Drawing.Size(92, 21);
            this.chkSame.TabIndex = 208;
            this.chkSame.TabStop = false;
            this.chkSame.Text = "Same File";
            this.chkSame.UseVisualStyleBackColor = true;
            // 
            // lbCount
            // 
            this.lbCount.AutoSize = true;
            this.lbCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCount.Location = new System.Drawing.Point(11, 395);
            this.lbCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(141, 17);
            this.lbCount.TabIndex = 212;
            this.lbCount.Text = "Total Record(s): 0";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(10, 80);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(891, 308);
            this.dataGridView1.TabIndex = 211;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Image = global::VATClient.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(478, 18);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(104, 32);
            this.btnSave.TabIndex = 210;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnImport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImport.Image = global::VATClient.Properties.Resources.Load;
            this.btnImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImport.Location = new System.Drawing.Point(346, 18);
            this.btnImport.Margin = new System.Windows.Forms.Padding(4);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(104, 32);
            this.btnImport.TabIndex = 209;
            this.btnImport.Text = "Import";
            this.btnImport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // FormImportProductPriceHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(911, 439);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.chkSame);
            this.Controls.Add(this.lbCount);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormImportProductPriceHistory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import Product Price History";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker bgwSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn QuantityS;
        private System.Windows.Forms.DataGridViewTextBoxColumn Heading2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Heading1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProductNameS;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemNoS;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProductCodeS;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn LineNoS;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox chkSame;
        private System.Windows.Forms.Label lbCount;
        public System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnImport;
        private System.ComponentModel.BackgroundWorker bgwProduct;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}