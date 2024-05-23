namespace VATClient
{
    partial class FormConfigChange
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
            this.btnReplace = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtReplaceWith = new System.Windows.Forms.TextBox();
            this.txtFindWhat = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(139, 83);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(75, 23);
            this.btnReplace.TabIndex = 2;
            this.btnReplace.Text = "Replace";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(47, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 17);
            this.label4.TabIndex = 129;
            this.label4.Text = "Replace With";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(47, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 17);
            this.label3.TabIndex = 128;
            this.label3.Text = "Find What";
            // 
            // txtReplaceWith
            // 
            this.txtReplaceWith.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReplaceWith.Location = new System.Drawing.Point(139, 42);
            this.txtReplaceWith.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtReplaceWith.MinimumSize = new System.Drawing.Size(175, 22);
            this.txtReplaceWith.Name = "txtReplaceWith";
            this.txtReplaceWith.Size = new System.Drawing.Size(175, 22);
            this.txtReplaceWith.TabIndex = 1;
            this.txtReplaceWith.Text = "localhost:63406";
            this.txtReplaceWith.TextChanged += new System.EventHandler(this.txtReplaceWith_TextChanged);
            // 
            // txtFindWhat
            // 
            this.txtFindWhat.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFindWhat.Location = new System.Drawing.Point(139, 12);
            this.txtFindWhat.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtFindWhat.MinimumSize = new System.Drawing.Size(175, 22);
            this.txtFindWhat.Name = "txtFindWhat";
            this.txtFindWhat.Size = new System.Drawing.Size(175, 22);
            this.txtFindWhat.TabIndex = 0;
            this.txtFindWhat.Text = "localhost:63406";
            this.txtFindWhat.TextChanged += new System.EventHandler(this.txtFindWhat_TextChanged);
            // 
            // FormConfigChange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(361, 121);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtReplaceWith);
            this.Controls.Add(this.txtFindWhat);
            this.Controls.Add(this.btnReplace);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigChange";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Config Change";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConfigChange_FormClosing);
            this.Load += new System.EventHandler(this.FormConfigChange_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtReplaceWith;
        private System.Windows.Forms.TextBox txtFindWhat;
    }
}