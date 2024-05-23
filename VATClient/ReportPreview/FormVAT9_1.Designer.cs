namespace VATClient.ReportPreview
{
    partial class FormVAT9_1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVAT9_1));
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.dgvVAT9_1 = new System.Windows.Forms.DataGridView();
            this.NoteDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NoteNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LineA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LineB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LineC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SubFormName = new System.Windows.Forms.DataGridViewButtonColumn();
            this.bgwLoad = new System.ComponentModel.BackgroundWorker();
            this.bgwPrint = new System.ComponentModel.BackgroundWorker();
            this.btnSubForm = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.ckbVAT9_1 = new System.Windows.Forms.CheckBox();
            this.chbMainOReturn = new System.Windows.Forms.CheckBox();
            this.chbLateReturn = new System.Windows.Forms.CheckBox();
            this.chbAmendReturn = new System.Windows.Forms.CheckBox();
            this.chbAlternativeReturn = new System.Windows.Forms.CheckBox();
            this.chbNoActivites = new System.Windows.Forms.CheckBox();
            this.txtNoActivitesDetails = new System.Windows.Forms.TextBox();
            this.dtpSubmissionDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.bgwLoad1 = new System.ComponentModel.BackgroundWorker();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgwVATReturnHeader = new System.ComponentModel.BackgroundWorker();
            this.pnlHidden = new System.Windows.Forms.Panel();
            this.btnProcess = new System.Windows.Forms.Button();
            this.btnLoadNew = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.chbIsTraderVAT = new System.Windows.Forms.CheckBox();
            this.btnAttachmentUpload = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.txtSubmit = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVAT9_1)).BeginInit();
            this.pnlHidden.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dtpDate
            // 
            this.dtpDate.Checked = false;
            this.dtpDate.CustomFormat = "MMMM-yyyy";
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.Location = new System.Drawing.Point(128, 6);
            this.dtpDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(193, 22);
            this.dtpDate.TabIndex = 95;
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtpDate_ValueChanged);
            this.dtpDate.Leave += new System.EventHandler(this.dtpDate_Leave);
            // 
            // dgvVAT9_1
            // 
            this.dgvVAT9_1.AllowUserToAddRows = false;
            this.dgvVAT9_1.AllowUserToDeleteRows = false;
            this.dgvVAT9_1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvVAT9_1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVAT9_1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NoteDescription,
            this.NoteNo,
            this.LineA,
            this.LineB,
            this.LineC,
            this.SubFormName});
            this.dgvVAT9_1.Location = new System.Drawing.Point(3, 171);
            this.dgvVAT9_1.Margin = new System.Windows.Forms.Padding(4);
            this.dgvVAT9_1.Name = "dgvVAT9_1";
            this.dgvVAT9_1.Size = new System.Drawing.Size(1004, 415);
            this.dgvVAT9_1.TabIndex = 98;
            this.dgvVAT9_1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVAT9_1_CellContentClick);
            this.dgvVAT9_1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvVAT9_1_CellFormatting);
            // 
            // NoteDescription
            // 
            this.NoteDescription.DataPropertyName = "NoteDescription";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.NoteDescription.DefaultCellStyle = dataGridViewCellStyle1;
            this.NoteDescription.FillWeight = 200F;
            this.NoteDescription.HeaderText = "Note Description";
            this.NoteDescription.Name = "NoteDescription";
            this.NoteDescription.ReadOnly = true;
            this.NoteDescription.Width = 200;
            // 
            // NoteNo
            // 
            this.NoteNo.DataPropertyName = "NoteNo";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.NoteNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.NoteNo.FillWeight = 50F;
            this.NoteNo.HeaderText = "Note No";
            this.NoteNo.Name = "NoteNo";
            this.NoteNo.ReadOnly = true;
            this.NoteNo.Width = 90;
            // 
            // LineA
            // 
            this.LineA.DataPropertyName = "LineA";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.LineA.DefaultCellStyle = dataGridViewCellStyle3;
            this.LineA.HeaderText = "Line A";
            this.LineA.Name = "LineA";
            this.LineA.ReadOnly = true;
            // 
            // LineB
            // 
            this.LineB.DataPropertyName = "LineB";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.LineB.DefaultCellStyle = dataGridViewCellStyle4;
            this.LineB.HeaderText = "Line B";
            this.LineB.Name = "LineB";
            this.LineB.ReadOnly = true;
            // 
            // LineC
            // 
            this.LineC.DataPropertyName = "LineC";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.LineC.DefaultCellStyle = dataGridViewCellStyle5;
            this.LineC.HeaderText = "Line C";
            this.LineC.Name = "LineC";
            this.LineC.ReadOnly = true;
            // 
            // SubFormName
            // 
            this.SubFormName.DataPropertyName = "SubFormName";
            this.SubFormName.HeaderText = "Sub Form Name";
            this.SubFormName.Name = "SubFormName";
            this.SubFormName.ReadOnly = true;
            this.SubFormName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SubFormName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SubFormName.Width = 120;
            // 
            // bgwLoad
            // 
            this.bgwLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwLoad_DoWork);
            this.bgwLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwLoad_RunWorkerCompleted);
            // 
            // bgwPrint
            // 
            this.bgwPrint.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPrint_DoWork);
            this.bgwPrint.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPrint_RunWorkerCompleted);
            // 
            // btnSubForm
            // 
            this.btnSubForm.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSubForm.Location = new System.Drawing.Point(828, 594);
            this.btnSubForm.Margin = new System.Windows.Forms.Padding(4);
            this.btnSubForm.Name = "btnSubForm";
            this.btnSubForm.Size = new System.Drawing.Size(100, 34);
            this.btnSubForm.TabIndex = 99;
            this.btnSubForm.Text = "Sub Form";
            this.btnSubForm.UseVisualStyleBackColor = false;
            this.btnSubForm.Visible = false;
            this.btnSubForm.Click += new System.EventHandler(this.btnSubForm_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(344, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 17);
            this.label2.TabIndex = 217;
            this.label2.Text = "Branch";
            // 
            // cmbFontSize
            // 
            this.cmbFontSize.FormattingEnabled = true;
            this.cmbFontSize.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.cmbFontSize.Location = new System.Drawing.Point(833, 5);
            this.cmbFontSize.Margin = new System.Windows.Forms.Padding(4);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(47, 24);
            this.cmbFontSize.TabIndex = 219;
            this.cmbFontSize.Text = "8";
            // 
            // ckbVAT9_1
            // 
            this.ckbVAT9_1.AutoSize = true;
            this.ckbVAT9_1.Checked = true;
            this.ckbVAT9_1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbVAT9_1.Enabled = false;
            this.ckbVAT9_1.Location = new System.Drawing.Point(16, 30);
            this.ckbVAT9_1.Margin = new System.Windows.Forms.Padding(4);
            this.ckbVAT9_1.Name = "ckbVAT9_1";
            this.ckbVAT9_1.Size = new System.Drawing.Size(57, 21);
            this.ckbVAT9_1.TabIndex = 220;
            this.ckbVAT9_1.Text = "New";
            this.ckbVAT9_1.UseVisualStyleBackColor = true;
            // 
            // chbMainOReturn
            // 
            this.chbMainOReturn.AutoSize = true;
            this.chbMainOReturn.Location = new System.Drawing.Point(336, 34);
            this.chbMainOReturn.Margin = new System.Windows.Forms.Padding(4);
            this.chbMainOReturn.Name = "chbMainOReturn";
            this.chbMainOReturn.Size = new System.Drawing.Size(123, 21);
            this.chbMainOReturn.TabIndex = 221;
            this.chbMainOReturn.Text = "Orginal Return";
            this.chbMainOReturn.UseVisualStyleBackColor = true;
            // 
            // chbLateReturn
            // 
            this.chbLateReturn.AutoSize = true;
            this.chbLateReturn.Location = new System.Drawing.Point(336, 57);
            this.chbLateReturn.Margin = new System.Windows.Forms.Padding(4);
            this.chbLateReturn.Name = "chbLateReturn";
            this.chbLateReturn.Size = new System.Drawing.Size(105, 21);
            this.chbLateReturn.TabIndex = 222;
            this.chbLateReturn.Text = "Late Return";
            this.chbLateReturn.UseVisualStyleBackColor = true;
            // 
            // chbAmendReturn
            // 
            this.chbAmendReturn.AutoSize = true;
            this.chbAmendReturn.Location = new System.Drawing.Point(460, 37);
            this.chbAmendReturn.Margin = new System.Windows.Forms.Padding(4);
            this.chbAmendReturn.Name = "chbAmendReturn";
            this.chbAmendReturn.Size = new System.Drawing.Size(121, 21);
            this.chbAmendReturn.TabIndex = 223;
            this.chbAmendReturn.Text = "Amend Return";
            this.chbAmendReturn.UseVisualStyleBackColor = true;
            // 
            // chbAlternativeReturn
            // 
            this.chbAlternativeReturn.AutoSize = true;
            this.chbAlternativeReturn.Location = new System.Drawing.Point(460, 63);
            this.chbAlternativeReturn.Margin = new System.Windows.Forms.Padding(4);
            this.chbAlternativeReturn.Name = "chbAlternativeReturn";
            this.chbAlternativeReturn.Size = new System.Drawing.Size(144, 21);
            this.chbAlternativeReturn.TabIndex = 224;
            this.chbAlternativeReturn.Text = "Alternative Return";
            this.chbAlternativeReturn.UseVisualStyleBackColor = true;
            // 
            // chbNoActivites
            // 
            this.chbNoActivites.AutoSize = true;
            this.chbNoActivites.Location = new System.Drawing.Point(5, 101);
            this.chbNoActivites.Margin = new System.Windows.Forms.Padding(4);
            this.chbNoActivites.Name = "chbNoActivites";
            this.chbNoActivites.Size = new System.Drawing.Size(104, 21);
            this.chbNoActivites.TabIndex = 225;
            this.chbNoActivites.Text = "No Activites";
            this.chbNoActivites.UseVisualStyleBackColor = true;
            // 
            // txtNoActivitesDetails
            // 
            this.txtNoActivitesDetails.Location = new System.Drawing.Point(125, 88);
            this.txtNoActivitesDetails.Margin = new System.Windows.Forms.Padding(4);
            this.txtNoActivitesDetails.Multiline = true;
            this.txtNoActivitesDetails.Name = "txtNoActivitesDetails";
            this.txtNoActivitesDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNoActivitesDetails.Size = new System.Drawing.Size(316, 43);
            this.txtNoActivitesDetails.TabIndex = 226;
            this.txtNoActivitesDetails.TabStop = false;
            // 
            // dtpSubmissionDate
            // 
            this.dtpSubmissionDate.CustomFormat = "dd/MMM/yyyy ";
            this.dtpSubmissionDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSubmissionDate.Location = new System.Drawing.Point(128, 39);
            this.dtpSubmissionDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSubmissionDate.MaximumSize = new System.Drawing.Size(199, 20);
            this.dtpSubmissionDate.MinimumSize = new System.Drawing.Size(132, 20);
            this.dtpSubmissionDate.Name = "dtpSubmissionDate";
            this.dtpSubmissionDate.Size = new System.Drawing.Size(193, 20);
            this.dtpSubmissionDate.TabIndex = 228;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 44);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 17);
            this.label4.TabIndex = 229;
            this.label4.Text = "Submission Date";
            // 
            // bgwLoad1
            // 
            this.bgwLoad1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwLoad1_DoWork);
            this.bgwLoad1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwLoad1_RunWorkerCompleted);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(761, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 17);
            this.label3.TabIndex = 232;
            this.label3.Text = "Font Size";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N",
            "All"});
            this.cmbPost.Location = new System.Drawing.Point(677, 5);
            this.cmbPost.Margin = new System.Windows.Forms.Padding(4);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(65, 24);
            this.cmbPost.TabIndex = 233;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(632, 10);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 17);
            this.label9.TabIndex = 234;
            this.label9.Text = "Post";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(307, 314);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(387, 41);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 235;
            this.progressBar1.Visible = false;
            // 
            // bgwVATReturnHeader
            // 
            this.bgwVATReturnHeader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwVATReturnHeader_DoWork);
            this.bgwVATReturnHeader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwVATReturnHeader_RunWorkerCompleted);
            // 
            // pnlHidden
            // 
            this.pnlHidden.Controls.Add(this.btnProcess);
            this.pnlHidden.Controls.Add(this.btnLoadNew);
            this.pnlHidden.Controls.Add(this.ckbVAT9_1);
            this.pnlHidden.Location = new System.Drawing.Point(1052, 284);
            this.pnlHidden.Margin = new System.Windows.Forms.Padding(4);
            this.pnlHidden.Name = "pnlHidden";
            this.pnlHidden.Size = new System.Drawing.Size(140, 176);
            this.pnlHidden.TabIndex = 237;
            this.pnlHidden.Visible = false;
            // 
            // btnProcess
            // 
            this.btnProcess.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnProcess.Image = ((System.Drawing.Image)(resources.GetObject("btnProcess.Image")));
            this.btnProcess.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnProcess.Location = new System.Drawing.Point(16, 58);
            this.btnProcess.Margin = new System.Windows.Forms.Padding(4);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(100, 34);
            this.btnProcess.TabIndex = 218;
            this.btnProcess.Text = "P&rocess";
            this.btnProcess.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnProcess.UseVisualStyleBackColor = false;
            this.btnProcess.Visible = false;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // btnLoadNew
            // 
            this.btnLoadNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLoadNew.Image = global::VATClient.Properties.Resources.Load;
            this.btnLoadNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoadNew.Location = new System.Drawing.Point(16, 100);
            this.btnLoadNew.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadNew.Name = "btnLoadNew";
            this.btnLoadNew.Size = new System.Drawing.Size(100, 34);
            this.btnLoadNew.TabIndex = 97;
            this.btnLoadNew.Text = "Load";
            this.btnLoadNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoadNew.UseVisualStyleBackColor = false;
            this.btnLoadNew.Visible = false;
            this.btnLoadNew.Click += new System.EventHandler(this.btnLoadNew_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 18);
            this.label1.TabIndex = 238;
            this.label1.Text = "Month-Year";
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Location = new System.Drawing.Point(407, 5);
            this.cmbBranch.Margin = new System.Windows.Forms.Padding(4);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(216, 24);
            this.cmbBranch.TabIndex = 532;
            // 
            // chbIsTraderVAT
            // 
            this.chbIsTraderVAT.AutoSize = true;
            this.chbIsTraderVAT.Location = new System.Drawing.Point(460, 89);
            this.chbIsTraderVAT.Margin = new System.Windows.Forms.Padding(4);
            this.chbIsTraderVAT.Name = "chbIsTraderVAT";
            this.chbIsTraderVAT.Size = new System.Drawing.Size(118, 21);
            this.chbIsTraderVAT.TabIndex = 533;
            this.chbIsTraderVAT.Text = "Is Trader VAT";
            this.chbIsTraderVAT.UseVisualStyleBackColor = true;
            // 
            // btnAttachmentUpload
            // 
            this.btnAttachmentUpload.BackColor = System.Drawing.SystemColors.Window;
            this.btnAttachmentUpload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAttachmentUpload.ForeColor = System.Drawing.SystemColors.Desktop;
            this.btnAttachmentUpload.Image = ((System.Drawing.Image)(resources.GetObject("btnAttachmentUpload.Image")));
            this.btnAttachmentUpload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAttachmentUpload.Location = new System.Drawing.Point(95, 77);
            this.btnAttachmentUpload.Margin = new System.Windows.Forms.Padding(4);
            this.btnAttachmentUpload.Name = "btnAttachmentUpload";
            this.btnAttachmentUpload.Size = new System.Drawing.Size(120, 34);
            this.btnAttachmentUpload.TabIndex = 534;
            this.btnAttachmentUpload.Text = "File upload";
            this.btnAttachmentUpload.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAttachmentUpload.UseVisualStyleBackColor = false;
            this.btnAttachmentUpload.Click += new System.EventHandler(this.btnAttachmentUpload_Click);
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPost.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPost.Image = ((System.Drawing.Image)(resources.GetObject("btnPost.Image")));
            this.btnPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPost.Location = new System.Drawing.Point(921, 1);
            this.btnPost.Margin = new System.Windows.Forms.Padding(4);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(80, 34);
            this.btnPost.TabIndex = 236;
            this.btnPost.Text = "&Post";
            this.btnPost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.SystemColors.Window;
            this.btnLoad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoad.ForeColor = System.Drawing.SystemColors.Desktop;
            this.btnLoad.Image = global::VATClient.Properties.Resources.Load;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(115, 18);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(100, 34);
            this.btnLoad.TabIndex = 230;
            this.btnLoad.Text = "Load";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.Window;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSave.Image = global::VATClient.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(11, 18);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 34);
            this.btnSave.TabIndex = 97;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.Window;
            this.btnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrint.ForeColor = System.Drawing.SystemColors.Desktop;
            this.btnPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(217, 18);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(100, 34);
            this.btnPrint.TabIndex = 94;
            this.btnPrint.Text = "Print";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // txtSubmit
            // 
            this.txtSubmit.BackColor = System.Drawing.SystemColors.Window;
            this.txtSubmit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtSubmit.ForeColor = System.Drawing.SystemColors.Desktop;
            this.txtSubmit.Image = ((System.Drawing.Image)(resources.GetObject("txtSubmit.Image")));
            this.txtSubmit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtSubmit.Location = new System.Drawing.Point(217, 77);
            this.txtSubmit.Margin = new System.Windows.Forms.Padding(4);
            this.txtSubmit.Name = "txtSubmit";
            this.txtSubmit.Size = new System.Drawing.Size(100, 34);
            this.txtSubmit.TabIndex = 535;
            this.txtSubmit.Text = "Submit";
            this.txtSubmit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.txtSubmit.UseVisualStyleBackColor = false;
            this.txtSubmit.Click += new System.EventHandler(this.txtSubmit_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.txtSubmit);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.btnAttachmentUpload);
            this.panel1.Controls.Add(this.btnLoad);
            this.panel1.Location = new System.Drawing.Point(679, 38);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(323, 125);
            this.panel1.TabIndex = 536;
            // 
            // FormVAT9_1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 590);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chbIsTraderVAT);
            this.Controls.Add(this.cmbBranch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlHidden);
            this.Controls.Add(this.btnPost);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.cmbPost);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dtpSubmissionDate);
            this.Controls.Add(this.txtNoActivitesDetails);
            this.Controls.Add(this.chbNoActivites);
            this.Controls.Add(this.chbAlternativeReturn);
            this.Controls.Add(this.chbAmendReturn);
            this.Controls.Add(this.chbLateReturn);
            this.Controls.Add(this.chbMainOReturn);
            this.Controls.Add(this.cmbFontSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSubForm);
            this.Controls.Add(this.dgvVAT9_1);
            this.Controls.Add(this.dtpDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormVAT9_1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VAT 9.1";
            this.Load += new System.EventHandler(this.FormVAT9_1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVAT9_1)).EndInit();
            this.pnlHidden.ResumeLayout(false);
            this.pnlHidden.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.DataGridView dgvVAT9_1;
        private System.ComponentModel.BackgroundWorker bgwLoad;
        private System.ComponentModel.BackgroundWorker bgwPrint;
        private System.Windows.Forms.Button btnSubForm;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Button btnLoadNew;
        private System.Windows.Forms.ComboBox cmbFontSize;
        private System.Windows.Forms.CheckBox ckbVAT9_1;
        private System.Windows.Forms.CheckBox chbMainOReturn;
        private System.Windows.Forms.CheckBox chbLateReturn;
        private System.Windows.Forms.CheckBox chbAmendReturn;
        private System.Windows.Forms.CheckBox chbAlternativeReturn;
        private System.Windows.Forms.CheckBox chbNoActivites;
        private System.Windows.Forms.TextBox txtNoActivitesDetails;
        private System.Windows.Forms.DateTimePicker dtpSubmissionDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnLoad;
        private System.ComponentModel.BackgroundWorker bgwLoad1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwVATReturnHeader;
        public System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.Panel pnlHidden;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn NoteDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn NoteNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn LineA;
        private System.Windows.Forms.DataGridViewTextBoxColumn LineB;
        private System.Windows.Forms.DataGridViewTextBoxColumn LineC;
        private System.Windows.Forms.DataGridViewButtonColumn SubFormName;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.CheckBox chbIsTraderVAT;
        private System.Windows.Forms.Button btnAttachmentUpload;
        private System.Windows.Forms.Button txtSubmit;
        private System.Windows.Forms.Panel panel1;
    }
}