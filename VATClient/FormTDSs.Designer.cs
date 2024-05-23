namespace VATClient
{
    partial class FormTDSs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTDSs));
            this.label1 = new System.Windows.Forms.Label();
            this.txtTDSCode = new System.Windows.Forms.TextBox();
            this.btnSearchTDS = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.Description = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.MinValue = new System.Windows.Forms.Label();
            this.txtMinValue = new System.Windows.Forms.TextBox();
            this.MaxValue = new System.Windows.Forms.Label();
            this.txtMaxValue = new System.Windows.Forms.TextBox();
            this.Rate = new System.Windows.Forms.Label();
            this.txtRate = new System.Windows.Forms.TextBox();
            this.Comments = new System.Windows.Forms.Label();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnPrintGrid = new System.Windows.Forms.Button();
            this.btnPrintList = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtId = new System.Windows.Forms.TextBox();
            this.backgroundWorkerAdd = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerDelete = new System.ComponentModel.BackgroundWorker();
            this.chkIsArchive = new System.Windows.Forms.CheckBox();
            this.LSection = new System.Windows.Forms.Label();
            this.txtSection = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Sub Section";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtTDSCode
            // 
            this.txtTDSCode.Location = new System.Drawing.Point(77, 29);
            this.txtTDSCode.MaximumSize = new System.Drawing.Size(400, 25);
            this.txtTDSCode.MinimumSize = new System.Drawing.Size(145, 20);
            this.txtTDSCode.Name = "txtTDSCode";
            this.txtTDSCode.Size = new System.Drawing.Size(180, 21);
            this.txtTDSCode.TabIndex = 18;
            this.txtTDSCode.TabStop = false;
            this.txtTDSCode.TextChanged += new System.EventHandler(this.txtTDSCode_TextChanged);
            // 
            // btnSearchTDS
            // 
            this.btnSearchTDS.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchTDS.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchTDS.Location = new System.Drawing.Point(263, 3);
            this.btnSearchTDS.Name = "btnSearchTDS";
            this.btnSearchTDS.Size = new System.Drawing.Size(30, 20);
            this.btnSearchTDS.TabIndex = 19;
            this.btnSearchTDS.TabStop = false;
            this.btnSearchTDS.UseVisualStyleBackColor = false;
            this.btnSearchTDS.Click += new System.EventHandler(this.btnSearchTDS_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAddNew.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddNew.Location = new System.Drawing.Point(299, 3);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(30, 20);
            this.btnAddNew.TabIndex = 20;
            this.btnAddNew.TabStop = false;
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // Description
            // 
            this.Description.AutoSize = true;
            this.Description.Location = new System.Drawing.Point(11, 63);
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(60, 13);
            this.Description.TabIndex = 22;
            this.Description.Text = "Description";
            this.Description.Click += new System.EventHandler(this.Description_Click);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(77, 56);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(180, 21);
            this.txtDescription.TabIndex = 23;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // MinValue
            // 
            this.MinValue.AutoSize = true;
            this.MinValue.Location = new System.Drawing.Point(10, 86);
            this.MinValue.Name = "MinValue";
            this.MinValue.Size = new System.Drawing.Size(49, 13);
            this.MinValue.TabIndex = 24;
            this.MinValue.Text = "MinValue";
            this.MinValue.Click += new System.EventHandler(this.MinValue_Click);
            // 
            // txtMinValue
            // 
            this.txtMinValue.Location = new System.Drawing.Point(77, 82);
            this.txtMinValue.Name = "txtMinValue";
            this.txtMinValue.Size = new System.Drawing.Size(78, 21);
            this.txtMinValue.TabIndex = 25;
            this.txtMinValue.Text = "0";
            this.txtMinValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMinValue.TextChanged += new System.EventHandler(this.txtMinValue_TextChanged);
            this.txtMinValue.Leave += new System.EventHandler(this.txtMinValue_Leave);
            // 
            // MaxValue
            // 
            this.MaxValue.AutoSize = true;
            this.MaxValue.Location = new System.Drawing.Point(192, 86);
            this.MaxValue.Name = "MaxValue";
            this.MaxValue.Size = new System.Drawing.Size(53, 13);
            this.MaxValue.TabIndex = 26;
            this.MaxValue.Text = "MaxValue";
            this.MaxValue.Click += new System.EventHandler(this.MaxValue_Click);
            // 
            // txtMaxValue
            // 
            this.txtMaxValue.Location = new System.Drawing.Point(257, 82);
            this.txtMaxValue.Name = "txtMaxValue";
            this.txtMaxValue.Size = new System.Drawing.Size(78, 21);
            this.txtMaxValue.TabIndex = 27;
            this.txtMaxValue.Text = "0";
            this.txtMaxValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMaxValue.TextChanged += new System.EventHandler(this.txtMaxValue_TextChanged);
            this.txtMaxValue.Leave += new System.EventHandler(this.txtMaxValue_Leave);
            // 
            // Rate
            // 
            this.Rate.AutoSize = true;
            this.Rate.Location = new System.Drawing.Point(10, 111);
            this.Rate.Name = "Rate";
            this.Rate.Size = new System.Drawing.Size(49, 13);
            this.Rate.TabIndex = 28;
            this.Rate.Text = "Rate(%)";
            this.Rate.Click += new System.EventHandler(this.Rate_Click);
            // 
            // txtRate
            // 
            this.txtRate.Location = new System.Drawing.Point(77, 108);
            this.txtRate.Name = "txtRate";
            this.txtRate.Size = new System.Drawing.Size(78, 21);
            this.txtRate.TabIndex = 29;
            this.txtRate.Text = "0";
            this.txtRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRate.Leave += new System.EventHandler(this.txtRate_Leave);
            // 
            // Comments
            // 
            this.Comments.AutoSize = true;
            this.Comments.Location = new System.Drawing.Point(11, 167);
            this.Comments.Name = "Comments";
            this.Comments.Size = new System.Drawing.Size(57, 13);
            this.Comments.TabIndex = 173;
            this.Comments.Text = "Comments";
            this.Comments.Click += new System.EventHandler(this.Comments_Click);
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(76, 135);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(180, 45);
            this.txtComments.TabIndex = 174;
            this.txtComments.TextChanged += new System.EventHandler(this.txtComments_TextChanged);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.btnPrev);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnPrintGrid);
            this.panel1.Controls.Add(this.btnPrintList);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(-4, 191);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(384, 40);
            this.panel1.TabIndex = 175;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDelete.Image = global::VATClient.Properties.Resources.Delete;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(145, 9);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 28);
            this.btnDelete.TabIndex = 28;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(226, 9);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(35, 28);
            this.btnRefresh.TabIndex = 17;
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrev.Image = ((System.Drawing.Image)(resources.GetObject("btnPrev.Image")));
            this.btnPrev.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrev.Location = new System.Drawing.Point(334, 9);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(47, 28);
            this.btnPrev.TabIndex = 188;
            this.btnPrev.Text = "(P)";
            this.btnPrev.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(74, 9);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(65, 28);
            this.btnUpdate.TabIndex = 14;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnPrintGrid
            // 
            this.btnPrintGrid.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrintGrid.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrintGrid.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrintGrid.Location = new System.Drawing.Point(471, 55);
            this.btnPrintGrid.Name = "btnPrintGrid";
            this.btnPrintGrid.Size = new System.Drawing.Size(75, 28);
            this.btnPrintGrid.TabIndex = 24;
            this.btnPrintGrid.Text = "&Grid";
            this.btnPrintGrid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrintGrid.UseVisualStyleBackColor = false;
            // 
            // btnPrintList
            // 
            this.btnPrintList.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrintList.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrintList.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrintList.Location = new System.Drawing.Point(378, 53);
            this.btnPrintList.Name = "btnPrintList";
            this.btnPrintList.Size = new System.Drawing.Size(75, 28);
            this.btnPrintList.TabIndex = 4;
            this.btnPrintList.Text = "&Print";
            this.btnPrintList.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrintList.UseVisualStyleBackColor = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(267, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(65, 28);
            this.btnClose.TabIndex = 19;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.Save;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(3, 9);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(65, 28);
            this.btnAdd.TabIndex = 13;
            this.btnAdd.Text = "&Add";
            this.btnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(285, 54);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(280, 113);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(100, 21);
            this.txtId.TabIndex = 176;
            this.txtId.Visible = false;
            this.txtId.TextChanged += new System.EventHandler(this.txtId_TextChanged);
            // 
            // backgroundWorkerAdd
            // 
            this.backgroundWorkerAdd.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerAdd_DoWork);
            this.backgroundWorkerAdd.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerAdd_RunWorkerCompleted);
            // 
            // backgroundWorkerUpdate
            // 
            this.backgroundWorkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpdate_DoWork);
            this.backgroundWorkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerUpdate_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(61, 167);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(295, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 187;
            this.progressBar1.Visible = false;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // backgroundWorkerDelete
            // 
            this.backgroundWorkerDelete.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerDelete_DoWork);
            this.backgroundWorkerDelete.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerDelete_RunWorkerCompleted);
            // 
            // chkIsArchive
            // 
            this.chkIsArchive.AutoSize = true;
            this.chkIsArchive.Location = new System.Drawing.Point(337, 166);
            this.chkIsArchive.Name = "chkIsArchive";
            this.chkIsArchive.Size = new System.Drawing.Size(45, 17);
            this.chkIsArchive.TabIndex = 192;
            this.chkIsArchive.Text = "TDS";
            this.chkIsArchive.UseVisualStyleBackColor = true;
            this.chkIsArchive.Visible = false;
            this.chkIsArchive.CheckedChanged += new System.EventHandler(this.chkIsArchive_CheckedChanged);
            // 
            // LSection
            // 
            this.LSection.AutoSize = true;
            this.LSection.Location = new System.Drawing.Point(10, 11);
            this.LSection.Name = "LSection";
            this.LSection.Size = new System.Drawing.Size(42, 13);
            this.LSection.TabIndex = 194;
            this.LSection.Text = "Section";
            this.LSection.Click += new System.EventHandler(this.LSection_Click);
            // 
            // txtSection
            // 
            this.txtSection.Location = new System.Drawing.Point(77, 3);
            this.txtSection.Name = "txtSection";
            this.txtSection.Size = new System.Drawing.Size(180, 21);
            this.txtSection.TabIndex = 195;
            this.txtSection.TextChanged += new System.EventHandler(this.txtSection_TextChanged);
            // 
            // FormTDSs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(383, 231);
            this.Controls.Add(this.txtSection);
            this.Controls.Add(this.LSection);
            this.Controls.Add(this.chkIsArchive);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtComments);
            this.Controls.Add(this.Comments);
            this.Controls.Add(this.txtRate);
            this.Controls.Add(this.Rate);
            this.Controls.Add(this.txtMaxValue);
            this.Controls.Add(this.MaxValue);
            this.Controls.Add(this.txtMinValue);
            this.Controls.Add(this.MinValue);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.Description);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSearchTDS);
            this.Controls.Add(this.btnAddNew);
            this.Controls.Add(this.txtTDSCode);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTDSs";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TDS";
            this.Load += new System.EventHandler(this.FormTDSs_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSearchTDS;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.TextBox txtTDSCode;
        private System.Windows.Forms.Label Description;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label MinValue;
        private System.Windows.Forms.TextBox txtMinValue;
        private System.Windows.Forms.Label MaxValue;
        private System.Windows.Forms.TextBox txtMaxValue;
        private System.Windows.Forms.Label Rate;
        private System.Windows.Forms.TextBox txtRate;
        private System.Windows.Forms.Label Comments;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnPrintGrid;
        private System.Windows.Forms.Button btnPrintList;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtId;
        private System.ComponentModel.BackgroundWorker backgroundWorkerAdd;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdate;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnPrev;
        private System.ComponentModel.BackgroundWorker backgroundWorkerDelete;
        private System.Windows.Forms.CheckBox chkIsArchive;
        private System.Windows.Forms.Label LSection;
        private System.Windows.Forms.TextBox txtSection;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Button btnDelete;
        public System.Windows.Forms.Button btnUpdate;
    }
}