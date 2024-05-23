namespace VATClient
{
    partial class FormReverseAdjustment
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.rbtnAdjustment = new System.Windows.Forms.RadioButton();
            this.rbtnSD = new System.Windows.Forms.RadioButton();
            this.rbtnTreasury = new System.Windows.Forms.RadioButton();
            this.rbtnVDS = new System.Windows.Forms.RadioButton();
            this.label16 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.grbBankInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(23, 96);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(338, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 88;
            this.progressBar1.Visible = false;
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.rbtnAdjustment);
            this.grbBankInformation.Controls.Add(this.rbtnSD);
            this.grbBankInformation.Controls.Add(this.rbtnTreasury);
            this.grbBankInformation.Controls.Add(this.rbtnVDS);
            this.grbBankInformation.Controls.Add(this.label16);
            this.grbBankInformation.Location = new System.Drawing.Point(23, 13);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(338, 77);
            this.grbBankInformation.TabIndex = 87;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reverse Criteria";
            // 
            // rbtnAdjustment
            // 
            this.rbtnAdjustment.AutoSize = true;
            this.rbtnAdjustment.Location = new System.Drawing.Point(255, 34);
            this.rbtnAdjustment.Name = "rbtnAdjustment";
            this.rbtnAdjustment.Size = new System.Drawing.Size(77, 17);
            this.rbtnAdjustment.TabIndex = 41;
            this.rbtnAdjustment.Text = "Adjustment";
            this.rbtnAdjustment.UseVisualStyleBackColor = true;
            // 
            // rbtnSD
            // 
            this.rbtnSD.AutoSize = true;
            this.rbtnSD.Location = new System.Drawing.Point(204, 34);
            this.rbtnSD.Name = "rbtnSD";
            this.rbtnSD.Size = new System.Drawing.Size(40, 17);
            this.rbtnSD.TabIndex = 40;
            this.rbtnSD.Text = "SD";
            this.rbtnSD.UseVisualStyleBackColor = true;
            // 
            // rbtnTreasury
            // 
            this.rbtnTreasury.AutoSize = true;
            this.rbtnTreasury.Location = new System.Drawing.Point(130, 34);
            this.rbtnTreasury.Name = "rbtnTreasury";
            this.rbtnTreasury.Size = new System.Drawing.Size(66, 17);
            this.rbtnTreasury.TabIndex = 41;
            this.rbtnTreasury.Text = "Treasury";
            this.rbtnTreasury.UseVisualStyleBackColor = true;
            // 
            // rbtnVDS
            // 
            this.rbtnVDS.AutoSize = true;
            this.rbtnVDS.Checked = true;
            this.rbtnVDS.Location = new System.Drawing.Point(77, 34);
            this.rbtnVDS.Name = "rbtnVDS";
            this.rbtnVDS.Size = new System.Drawing.Size(47, 17);
            this.rbtnVDS.TabIndex = 40;
            this.rbtnVDS.TabStop = true;
            this.rbtnVDS.Text = "VDS";
            this.rbtnVDS.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(18, 34);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(34, 13);
            this.label16.TabIndex = 39;
            this.label16.Text = "Type:";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSearch);
            this.panel1.Controls.Add(this.btnNew);
            this.panel1.Location = new System.Drawing.Point(0, 126);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(384, 40);
            this.panel1.TabIndex = 89;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(288, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(195, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(104, 7);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(87, 28);
            this.btnSearch.TabIndex = 41;
            this.btnSearch.Text = "Search";
            this.btnSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnNew.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNew.Location = new System.Drawing.Point(11, 7);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(87, 28);
            this.btnNew.TabIndex = 41;
            this.btnNew.Text = "New";
            this.btnNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // FormReverseAdjustment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(384, 171);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.grbBankInformation);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 210);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 210);
            this.Name = "FormReverseAdjustment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reverse Adjustment";
            this.Load += new System.EventHandler(this.FormReverseAdjustment_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnNew;
        public System.Windows.Forms.RadioButton rbtnAdjustment;
        public System.Windows.Forms.RadioButton rbtnSD;
        public System.Windows.Forms.RadioButton rbtnTreasury;
        public System.Windows.Forms.RadioButton rbtnVDS;
        private System.Windows.Forms.Button btnSearch;

    }
}