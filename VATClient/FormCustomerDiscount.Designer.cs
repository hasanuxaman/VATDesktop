namespace VATClient
{
    partial class FormCustomerDiscount
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCustomerDiscount));
            this.btnSearchTDS = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.MaxValue = new System.Windows.Forms.Label();
            this.Comments = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnPrintGrid = new System.Windows.Forms.Button();
            this.btnPrintList = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtId = new System.Windows.Forms.TextBox();
            this.backgroundWorkerAdd = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerDelete = new System.ComponentModel.BackgroundWorker();
            this.txtCustomerId = new System.Windows.Forms.TextBox();
            this.txtCustomerCode = new System.Windows.Forms.TextBox();
            this.btnCustomerName = new System.Windows.Forms.Button();
            this.chkIsArchive = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.txtMaxValue = new System.Windows.Forms.TextBox();
            this.txtRate = new System.Windows.Forms.TextBox();
            this.Rate = new System.Windows.Forms.Label();
            this.txtMinValue = new System.Windows.Forms.TextBox();
            this.MinValue = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.Description = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.LID = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSearchTDS
            // 
            this.btnSearchTDS.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchTDS.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchTDS.Location = new System.Drawing.Point(277, 5);
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
            this.btnAddNew.Location = new System.Drawing.Point(313, 5);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(30, 20);
            this.btnAddNew.TabIndex = 20;
            this.btnAddNew.TabStop = false;
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // MaxValue
            // 
            this.MaxValue.AutoSize = true;
            this.MaxValue.Location = new System.Drawing.Point(180, 86);
            this.MaxValue.Name = "MaxValue";
            this.MaxValue.Size = new System.Drawing.Size(53, 13);
            this.MaxValue.TabIndex = 26;
            this.MaxValue.Text = "MaxValue";
            this.MaxValue.Click += new System.EventHandler(this.MaxValue_Click);
            // 
            // Comments
            // 
            this.Comments.AutoSize = true;
            this.Comments.Location = new System.Drawing.Point(11, 142);
            this.Comments.Name = "Comments";
            this.Comments.Size = new System.Drawing.Size(57, 13);
            this.Comments.TabIndex = 173;
            this.Comments.Text = "Comments";
            this.Comments.Click += new System.EventHandler(this.Comments_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnPrintGrid);
            this.panel1.Controls.Add(this.btnPrintList);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(-4, 186);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(384, 40);
            this.panel1.TabIndex = 175;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDelete.Image = global::VATClient.Properties.Resources.Delete;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(148, 53);
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
            this.btnRefresh.Location = new System.Drawing.Point(229, 6);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(35, 28);
            this.btnRefresh.TabIndex = 17;
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(77, 6);
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
            this.btnClose.Location = new System.Drawing.Point(270, 6);
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
            this.btnAdd.Location = new System.Drawing.Point(6, 6);
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
            this.txtId.Location = new System.Drawing.Point(89, 6);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(182, 21);
            this.txtId.TabIndex = 176;
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
            // backgroundWorkerDelete
            // 
            this.backgroundWorkerDelete.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerDelete_DoWork);
            this.backgroundWorkerDelete.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerDelete_RunWorkerCompleted);
            // 
            // txtCustomerId
            // 
            this.txtCustomerId.Location = new System.Drawing.Point(271, 117);
            this.txtCustomerId.Name = "txtCustomerId";
            this.txtCustomerId.Size = new System.Drawing.Size(100, 21);
            this.txtCustomerId.TabIndex = 198;
            this.txtCustomerId.Visible = false;
            // 
            // txtCustomerCode
            // 
            this.txtCustomerCode.Location = new System.Drawing.Point(279, 63);
            this.txtCustomerCode.Name = "txtCustomerCode";
            this.txtCustomerCode.Size = new System.Drawing.Size(100, 21);
            this.txtCustomerCode.TabIndex = 199;
            this.txtCustomerCode.Visible = false;
            // 
            // btnCustomerName
            // 
            this.btnCustomerName.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCustomerName.Image = ((System.Drawing.Image)(resources.GetObject("btnCustomerName.Image")));
            this.btnCustomerName.Location = new System.Drawing.Point(277, 32);
            this.btnCustomerName.Name = "btnCustomerName";
            this.btnCustomerName.Size = new System.Drawing.Size(30, 20);
            this.btnCustomerName.TabIndex = 200;
            this.btnCustomerName.TabStop = false;
            this.btnCustomerName.UseVisualStyleBackColor = false;
            this.btnCustomerName.Click += new System.EventHandler(this.btnCustomerName_Click);
            // 
            // chkIsArchive
            // 
            this.chkIsArchive.AutoSize = true;
            this.chkIsArchive.Location = new System.Drawing.Point(337, 160);
            this.chkIsArchive.Name = "chkIsArchive";
            this.chkIsArchive.Size = new System.Drawing.Size(45, 17);
            this.chkIsArchive.TabIndex = 192;
            this.chkIsArchive.Text = "TDS";
            this.chkIsArchive.UseVisualStyleBackColor = true;
            this.chkIsArchive.Visible = false;
            this.chkIsArchive.CheckedChanged += new System.EventHandler(this.chkIsArchive_CheckedChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(61, 161);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(295, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 187;
            this.progressBar1.Visible = false;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(88, 132);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(180, 35);
            this.txtComments.TabIndex = 174;
            this.txtComments.TextChanged += new System.EventHandler(this.txtComments_TextChanged);
            // 
            // txtMaxValue
            // 
            this.txtMaxValue.Location = new System.Drawing.Point(245, 82);
            this.txtMaxValue.Name = "txtMaxValue";
            this.txtMaxValue.Size = new System.Drawing.Size(78, 21);
            this.txtMaxValue.TabIndex = 27;
            this.txtMaxValue.Text = "0";
            this.txtMaxValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMaxValue.TextChanged += new System.EventHandler(this.txtMaxValue_TextChanged);
            this.txtMaxValue.Leave += new System.EventHandler(this.txtMaxValue_Leave);
            // 
            // txtRate
            // 
            this.txtRate.Location = new System.Drawing.Point(89, 108);
            this.txtRate.Name = "txtRate";
            this.txtRate.Size = new System.Drawing.Size(78, 21);
            this.txtRate.TabIndex = 29;
            this.txtRate.Text = "0";
            this.txtRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRate.Leave += new System.EventHandler(this.txtRate_Leave);
            // 
            // Rate
            // 
            this.Rate.AutoSize = true;
            this.Rate.Location = new System.Drawing.Point(11, 111);
            this.Rate.Name = "Rate";
            this.Rate.Size = new System.Drawing.Size(49, 13);
            this.Rate.TabIndex = 28;
            this.Rate.Text = "Rate(%)";
            this.Rate.Click += new System.EventHandler(this.Rate_Click);
            // 
            // txtMinValue
            // 
            this.txtMinValue.Location = new System.Drawing.Point(89, 82);
            this.txtMinValue.Name = "txtMinValue";
            this.txtMinValue.Size = new System.Drawing.Size(78, 21);
            this.txtMinValue.TabIndex = 25;
            this.txtMinValue.Text = "0";
            this.txtMinValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMinValue.TextChanged += new System.EventHandler(this.txtMinValue_TextChanged);
            this.txtMinValue.Leave += new System.EventHandler(this.txtMinValue_Leave);
            // 
            // MinValue
            // 
            this.MinValue.AutoSize = true;
            this.MinValue.Location = new System.Drawing.Point(11, 86);
            this.MinValue.Name = "MinValue";
            this.MinValue.Size = new System.Drawing.Size(49, 13);
            this.MinValue.TabIndex = 24;
            this.MinValue.Text = "MinValue";
            this.MinValue.Click += new System.EventHandler(this.MinValue_Click);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(89, 56);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(180, 21);
            this.txtDescription.TabIndex = 23;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // Description
            // 
            this.Description.AutoSize = true;
            this.Description.Location = new System.Drawing.Point(11, 61);
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(60, 13);
            this.Description.TabIndex = 22;
            this.Description.Text = "Description";
            this.Description.Click += new System.EventHandler(this.Description_Click);
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(89, 31);
            this.txtCustomerName.MaximumSize = new System.Drawing.Size(400, 25);
            this.txtCustomerName.MinimumSize = new System.Drawing.Size(145, 20);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Size = new System.Drawing.Size(180, 21);
            this.txtCustomerName.TabIndex = 196;
            this.txtCustomerName.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 197;
            this.label2.Text = "Customer Name";
            // 
            // LID
            // 
            this.LID.AutoSize = true;
            this.LID.Location = new System.Drawing.Point(18, 12);
            this.LID.Name = "LID";
            this.LID.Size = new System.Drawing.Size(18, 13);
            this.LID.TabIndex = 202;
            this.LID.Text = "ID";
            // 
            // FormCustomerDiscount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(383, 228);
            this.Controls.Add(this.LID);
            this.Controls.Add(this.btnCustomerName);
            this.Controls.Add(this.txtCustomerCode);
            this.Controls.Add(this.txtCustomerId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtCustomerName);
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
            this.Controls.Add(this.btnSearchTDS);
            this.Controls.Add(this.btnAddNew);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCustomerDiscount";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Discount";
            this.Load += new System.EventHandler(this.FormTDSs_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearchTDS;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Label MaxValue;
        private System.Windows.Forms.Label Comments;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnPrintGrid;
        private System.Windows.Forms.Button btnPrintList;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtId;
        private System.ComponentModel.BackgroundWorker backgroundWorkerAdd;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdate;
        private System.ComponentModel.BackgroundWorker backgroundWorkerDelete;
        private System.Windows.Forms.TextBox txtCustomerId;
        private System.Windows.Forms.TextBox txtCustomerCode;
        public System.Windows.Forms.Button btnCustomerName;
        private System.Windows.Forms.CheckBox chkIsArchive;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.TextBox txtMaxValue;
        private System.Windows.Forms.TextBox txtRate;
        private System.Windows.Forms.Label Rate;
        private System.Windows.Forms.TextBox txtMinValue;
        private System.Windows.Forms.Label MinValue;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label Description;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LID;
    }
}