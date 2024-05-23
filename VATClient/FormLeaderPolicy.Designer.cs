namespace VATClient
{
    partial class FormLeaderPolicy
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLeaderAmount = new System.Windows.Forms.TextBox();
            this.cmbIsLeader = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.bgwAdd = new System.ComponentModel.BackgroundWorker();
            this.bgrDelete = new System.ComponentModel.BackgroundWorker();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtLeaderAmount);
            this.groupBox1.Controls.Add(this.cmbIsLeader);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(288, 159);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(50, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 209;
            this.label1.Text = "Leader Amount";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtLeaderAmount
            // 
            this.txtLeaderAmount.Location = new System.Drawing.Point(136, 66);
            this.txtLeaderAmount.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtLeaderAmount.MinimumSize = new System.Drawing.Size(100, 21);
            this.txtLeaderAmount.Name = "txtLeaderAmount";
            this.txtLeaderAmount.Size = new System.Drawing.Size(100, 21);
            this.txtLeaderAmount.TabIndex = 208;
            this.txtLeaderAmount.TabStop = false;
            this.txtLeaderAmount.Text = "0";
            this.txtLeaderAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtLeaderAmount.TextChanged += new System.EventHandler(this.txtLeaderAmount_TextChanged);
            this.txtLeaderAmount.Leave += new System.EventHandler(this.txtLeaderAmount_Leave);
            // 
            // cmbIsLeader
            // 
            this.cmbIsLeader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIsLeader.FormattingEnabled = true;
            this.cmbIsLeader.Items.AddRange(new object[] {
            "NA",
            "Y",
            "N"});
            this.cmbIsLeader.Location = new System.Drawing.Point(136, 36);
            this.cmbIsLeader.Name = "cmbIsLeader";
            this.cmbIsLeader.Size = new System.Drawing.Size(60, 21);
            this.cmbIsLeader.TabIndex = 206;
            this.cmbIsLeader.SelectedIndexChanged += new System.EventHandler(this.cmbIsLeader_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(50, 40);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 13);
            this.label10.TabIndex = 207;
            this.label10.Text = "Is Leader";
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Location = new System.Drawing.Point(6, 168);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(284, 40);
            this.panel1.TabIndex = 8;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.Save;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(91, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = "&Update";
            this.btnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // FormLeaderPolicy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(296, 211);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(16, 250);
            this.Name = "FormLeaderPolicy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Leader Policy";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormDN_CNAdjustment_FormClosed);
            this.Load += new System.EventHandler(this.FormDN_CNAdjustment_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.ComponentModel.BackgroundWorker bgwAdd;
        private System.ComponentModel.BackgroundWorker bgrDelete;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ComboBox cmbIsLeader;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtLeaderAmount;
    }
}