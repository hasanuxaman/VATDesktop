namespace VATClient
{
    partial class FormProductCategory
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPane = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtHSCodeNo = new System.Windows.Forms.TextBox();
            this.txtCategoryID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkActiveStatus = new System.Windows.Forms.CheckBox();
            this.chkPropagationRate = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCategoryName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtVATRate = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtHSDescription = new System.Windows.Forms.TextBox();
            this.cmbHSCode = new System.Windows.Forms.ComboBox();
            this.btnSearchProductCategory = new System.Windows.Forms.Button();
            this.btnSearchHSCode = new System.Windows.Forms.Button();
            this.cmbProductType = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtSDRate = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cmbReportType = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSaleComp = new System.Windows.Forms.Button();
            this.btnNotes = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.chkNonStock = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.chkTrading = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.pnlHidden = new System.Windows.Forms.Panel();
            this.SCBL = new System.Windows.Forms.Button();
            this.backgroundWorkerAdd = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerDelete = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnlHidden.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnPane);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Location = new System.Drawing.Point(-2, 306);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(585, 40);
            this.panel1.TabIndex = 12;
            // 
            // btnPane
            // 
            this.btnPane.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPane.Image = global::VATClient.Properties.Resources.Print;
            this.btnPane.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPane.Location = new System.Drawing.Point(571, 20);
            this.btnPane.Name = "btnPane";
            this.btnPane.Size = new System.Drawing.Size(14, 20);
            this.btnPane.TabIndex = 200;
            this.btnPane.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPane.UseVisualStyleBackColor = false;
            this.btnPane.Click += new System.EventHandler(this.btnPane_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(242, 6);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(84, 28);
            this.btnPrint.TabIndex = 16;
            this.btnPrint.Text = "&Print";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(477, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 17;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(107, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 14;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.Save;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(24, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 13;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(249, 57);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDelete.Image = global::VATClient.Properties.Resources.Delete;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(191, 50);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 28);
            this.btnDelete.TabIndex = 15;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Product Group ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(125, 122);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(249, 21);
            this.txtDescription.TabIndex = 7;
            this.txtDescription.TabStop = false;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            this.txtDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDescription_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(391, 155);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "HS Code";
            // 
            // txtHSCodeNo
            // 
            this.txtHSCodeNo.Location = new System.Drawing.Point(3, 57);
            this.txtHSCodeNo.Name = "txtHSCodeNo";
            this.txtHSCodeNo.ReadOnly = true;
            this.txtHSCodeNo.Size = new System.Drawing.Size(36, 21);
            this.txtHSCodeNo.TabIndex = 4;
            this.txtHSCodeNo.Tag = "000";
            this.txtHSCodeNo.Text = "0.00";
            this.txtHSCodeNo.Visible = false;
            this.txtHSCodeNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtHSCodeNo_KeyDown);
            // 
            // txtCategoryID
            // 
            this.txtCategoryID.Location = new System.Drawing.Point(125, 20);
            this.txtCategoryID.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCategoryID.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCategoryID.Name = "txtCategoryID";
            this.txtCategoryID.ReadOnly = true;
            this.txtCategoryID.Size = new System.Drawing.Size(185, 20);
            this.txtCategoryID.TabIndex = 0;
            this.txtCategoryID.TabStop = false;
            this.txtCategoryID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCategoryID_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Active Status";
            // 
            // chkActiveStatus
            // 
            this.chkActiveStatus.AutoSize = true;
            this.chkActiveStatus.Checked = true;
            this.chkActiveStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActiveStatus.Location = new System.Drawing.Point(125, 209);
            this.chkActiveStatus.Name = "chkActiveStatus";
            this.chkActiveStatus.Size = new System.Drawing.Size(15, 14);
            this.chkActiveStatus.TabIndex = 9;
            this.chkActiveStatus.TabStop = false;
            this.chkActiveStatus.UseVisualStyleBackColor = true;
            this.chkActiveStatus.CheckedChanged += new System.EventHandler(this.chkActiveStatus_CheckedChanged);
            // 
            // chkPropagationRate
            // 
            this.chkPropagationRate.AutoSize = true;
            this.chkPropagationRate.Enabled = false;
            this.chkPropagationRate.Location = new System.Drawing.Point(475, 310);
            this.chkPropagationRate.Name = "chkPropagationRate";
            this.chkPropagationRate.Size = new System.Drawing.Size(15, 14);
            this.chkPropagationRate.TabIndex = 7;
            this.chkPropagationRate.TabStop = false;
            this.chkPropagationRate.UseVisualStyleBackColor = true;
            this.chkPropagationRate.Visible = false;
            this.chkPropagationRate.CheckedChanged += new System.EventHandler(this.chkPropagationRate_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Product Group";
            // 
            // txtCategoryName
            // 
            this.txtCategoryName.Location = new System.Drawing.Point(125, 46);
            this.txtCategoryName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCategoryName.MinimumSize = new System.Drawing.Size(225, 20);
            this.txtCategoryName.Name = "txtCategoryName";
            this.txtCategoryName.Size = new System.Drawing.Size(225, 20);
            this.txtCategoryName.TabIndex = 0;
            this.txtCategoryName.TextChanged += new System.EventHandler(this.txtCategoryName_TextChanged);
            this.txtCategoryName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCategoryName_KeyDown);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 98);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "VAT Rate (%)";
            // 
            // txtVATRate
            // 
            this.txtVATRate.Location = new System.Drawing.Point(125, 96);
            this.txtVATRate.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtVATRate.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtVATRate.Name = "txtVATRate";
            this.txtVATRate.Size = new System.Drawing.Size(85, 20);
            this.txtVATRate.TabIndex = 2;
            this.txtVATRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVATRate.TextChanged += new System.EventHandler(this.txtVATRate_TextChanged);
            this.txtVATRate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVATRate_KeyDown);
            this.txtVATRate.Leave += new System.EventHandler(this.txtVATRate_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Comments";
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(125, 148);
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(249, 21);
            this.txtComments.TabIndex = 8;
            this.txtComments.TabStop = false;
            this.txtComments.TextChanged += new System.EventHandler(this.txtComments_TextChanged);
            this.txtComments.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtComments_KeyDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(21, 74);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "Product Type";
            // 
            // txtHSDescription
            // 
            this.txtHSDescription.Location = new System.Drawing.Point(3, 30);
            this.txtHSDescription.MinimumSize = new System.Drawing.Size(4, 20);
            this.txtHSDescription.Name = "txtHSDescription";
            this.txtHSDescription.ReadOnly = true;
            this.txtHSDescription.Size = new System.Drawing.Size(36, 21);
            this.txtHSDescription.TabIndex = 5;
            this.txtHSDescription.Text = "-";
            this.txtHSDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtHSDescription_KeyDown);
            // 
            // cmbHSCode
            // 
            this.cmbHSCode.FormattingEnabled = true;
            this.cmbHSCode.Location = new System.Drawing.Point(3, 3);
            this.cmbHSCode.Name = "cmbHSCode";
            this.cmbHSCode.Size = new System.Drawing.Size(36, 21);
            this.cmbHSCode.TabIndex = 179;
            this.cmbHSCode.Text = "NA";
            this.cmbHSCode.SelectedIndexChanged += new System.EventHandler(this.cmbHSCode_SelectedIndexChanged);
            // 
            // btnSearchProductCategory
            // 
            this.btnSearchProductCategory.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchProductCategory.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchProductCategory.Location = new System.Drawing.Point(316, 20);
            this.btnSearchProductCategory.Name = "btnSearchProductCategory";
            this.btnSearchProductCategory.Size = new System.Drawing.Size(30, 20);
            this.btnSearchProductCategory.TabIndex = 1;
            this.btnSearchProductCategory.TabStop = false;
            this.btnSearchProductCategory.UseVisualStyleBackColor = false;
            this.btnSearchProductCategory.Click += new System.EventHandler(this.btnSearchProductCategory_Click);
            // 
            // btnSearchHSCode
            // 
            this.btnSearchHSCode.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchHSCode.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchHSCode.Location = new System.Drawing.Point(45, 4);
            this.btnSearchHSCode.Name = "btnSearchHSCode";
            this.btnSearchHSCode.Size = new System.Drawing.Size(30, 20);
            this.btnSearchHSCode.TabIndex = 181;
            this.btnSearchHSCode.UseVisualStyleBackColor = false;
            // 
            // cmbProductType
            // 
            this.cmbProductType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProductType.FormattingEnabled = true;
            this.cmbProductType.Location = new System.Drawing.Point(125, 70);
            this.cmbProductType.Name = "cmbProductType";
            this.cmbProductType.Size = new System.Drawing.Size(180, 21);
            this.cmbProductType.TabIndex = 1;
            this.cmbProductType.SelectedIndexChanged += new System.EventHandler(this.cmbProductType_SelectedIndexChanged);
            this.cmbProductType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbProductType_KeyDown);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(216, 100);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(68, 13);
            this.label18.TabIndex = 17;
            this.label18.Text = "SD Rate (%)";
            // 
            // txtSDRate
            // 
            this.txtSDRate.Location = new System.Drawing.Point(289, 96);
            this.txtSDRate.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtSDRate.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtSDRate.Name = "txtSDRate";
            this.txtSDRate.Size = new System.Drawing.Size(85, 20);
            this.txtSDRate.TabIndex = 3;
            this.txtSDRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSDRate.TextChanged += new System.EventHandler(this.txtSDRate_TextChanged);
            this.txtSDRate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSDRate_KeyDown);
            this.txtSDRate.Leave += new System.EventHandler(this.txtSDRate_Leave);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.cmbReportType);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnSaleComp);
            this.groupBox1.Controls.Add(this.btnNotes);
            this.groupBox1.Controls.Add(this.btnAddNew);
            this.groupBox1.Controls.Add(this.chkNonStock);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.chkTrading);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txtSDRate);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.cmbProductType);
            this.groupBox1.Controls.Add(this.btnSearchProductCategory);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtComments);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtVATRate);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtCategoryName);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.chkActiveStatus);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtCategoryID);
            this.groupBox1.Controls.Add(this.txtDescription);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(19, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(564, 293);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(389, 299);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 199;
            this.button3.Text = "Purchase Toll Charge";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(474, 290);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 198;
            this.button2.Text = "PAVGPriceDownload";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // cmbReportType
            // 
            this.cmbReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReportType.FormattingEnabled = true;
            this.cmbReportType.Location = new System.Drawing.Point(125, 176);
            this.cmbReportType.Name = "cmbReportType";
            this.cmbReportType.Size = new System.Drawing.Size(249, 21);
            this.cmbReportType.TabIndex = 197;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(22, 176);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 13);
            this.label13.TabIndex = 196;
            this.label13.Text = "Report Type:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.Red;
            this.label12.Location = new System.Drawing.Point(111, 77);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(14, 14);
            this.label12.TabIndex = 185;
            this.label12.Text = "*";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(111, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 14);
            this.label5.TabIndex = 184;
            this.label5.Text = "*";
            // 
            // btnSaleComp
            // 
            this.btnSaleComp.Location = new System.Drawing.Point(565, 176);
            this.btnSaleComp.Name = "btnSaleComp";
            this.btnSaleComp.Size = new System.Drawing.Size(84, 23);
            this.btnSaleComp.TabIndex = 32;
            this.btnSaleComp.Text = "Sale Comp Report";
            this.btnSaleComp.UseVisualStyleBackColor = true;
            this.btnSaleComp.Visible = false;
            this.btnSaleComp.Click += new System.EventHandler(this.btnSaleComp_Click);
            // 
            // btnNotes
            // 
            this.btnNotes.Location = new System.Drawing.Point(565, 15);
            this.btnNotes.Name = "btnNotes";
            this.btnNotes.Size = new System.Drawing.Size(84, 23);
            this.btnNotes.TabIndex = 30;
            this.btnNotes.Text = "PTR";
            this.btnNotes.UseVisualStyleBackColor = true;
            this.btnNotes.Click += new System.EventHandler(this.btnNotes_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAddNew.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddNew.Location = new System.Drawing.Point(352, 20);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(30, 20);
            this.btnAddNew.TabIndex = 2;
            this.btnAddNew.TabStop = false;
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // chkNonStock
            // 
            this.chkNonStock.AutoSize = true;
            this.chkNonStock.Location = new System.Drawing.Point(339, 207);
            this.chkNonStock.Name = "chkNonStock";
            this.chkNonStock.Size = new System.Drawing.Size(15, 14);
            this.chkNonStock.TabIndex = 22;
            this.chkNonStock.TabStop = false;
            this.chkNonStock.UseVisualStyleBackColor = true;
            this.chkNonStock.Visible = false;
            this.chkNonStock.Click += new System.EventHandler(this.chkNonStock_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(289, 207);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Nonstock";
            this.label10.Visible = false;
            // 
            // chkTrading
            // 
            this.chkTrading.AutoSize = true;
            this.chkTrading.Location = new System.Drawing.Point(223, 209);
            this.chkTrading.Name = "chkTrading";
            this.chkTrading.Size = new System.Drawing.Size(15, 14);
            this.chkTrading.TabIndex = 10;
            this.chkTrading.TabStop = false;
            this.chkTrading.UseVisualStyleBackColor = true;
            this.chkTrading.Visible = false;
            this.chkTrading.Click += new System.EventHandler(this.chkTrading_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(180, 208);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(43, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Trading";
            this.label11.Visible = false;
            // 
            // pnlHidden
            // 
            this.pnlHidden.Controls.Add(this.cmbHSCode);
            this.pnlHidden.Controls.Add(this.SCBL);
            this.pnlHidden.Controls.Add(this.txtHSCodeNo);
            this.pnlHidden.Controls.Add(this.txtHSDescription);
            this.pnlHidden.Controls.Add(this.btnSearchHSCode);
            this.pnlHidden.Location = new System.Drawing.Point(589, 76);
            this.pnlHidden.Name = "pnlHidden";
            this.pnlHidden.Size = new System.Drawing.Size(160, 97);
            this.pnlHidden.TabIndex = 183;
            this.pnlHidden.Visible = false;
            // 
            // SCBL
            // 
            this.SCBL.Location = new System.Drawing.Point(50, 55);
            this.SCBL.Name = "SCBL";
            this.SCBL.Size = new System.Drawing.Size(75, 23);
            this.SCBL.TabIndex = 26;
            this.SCBL.Text = "SCBL";
            this.SCBL.UseVisualStyleBackColor = true;
            this.SCBL.Click += new System.EventHandler(this.button2_Click);
            // 
            // backgroundWorkerAdd
            // 
            this.backgroundWorkerAdd.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerAdd_DoWork);
            this.backgroundWorkerAdd.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerAdd_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(103, 269);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 182;
            this.progressBar1.Visible = false;
            // 
            // backgroundWorkerDelete
            // 
            this.backgroundWorkerDelete.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerDelete_DoWork);
            this.backgroundWorkerDelete.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerDelete_RunWorkerCompleted);
            // 
            // backgroundWorkerUpdate
            // 
            this.backgroundWorkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpdate_DoWork);
            this.backgroundWorkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerUpdate_RunWorkerCompleted);
            // 
            // FormProductCategory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(585, 351);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkPropagationRate);
            this.Controls.Add(this.pnlHidden);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 600);
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "FormProductCategory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Product Group";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormProductCategory_FormClosing);
            this.Load += new System.EventHandler(this.FormProductCategory_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlHidden.ResumeLayout(false);
            this.pnlHidden.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtHSCodeNo;
        private System.Windows.Forms.TextBox txtCategoryID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkActiveStatus;
        private System.Windows.Forms.CheckBox chkPropagationRate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCategoryName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtVATRate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtHSDescription;
        private System.Windows.Forms.ComboBox cmbHSCode;
        private System.Windows.Forms.Button btnSearchProductCategory;
        private System.Windows.Forms.Button btnSearchHSCode;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtSDRate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkTrading;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkNonStock;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Button btnPrint;
        private System.ComponentModel.BackgroundWorker backgroundWorkerAdd;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerDelete;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdate;
        public System.Windows.Forms.ComboBox cmbProductType;
        private System.Windows.Forms.Button SCBL;
        public System.Windows.Forms.Button btnUpdate;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnNotes;
        private System.Windows.Forms.Button btnSaleComp;
        private System.Windows.Forms.Panel pnlHidden;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbReportType;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnPane;
    }
}