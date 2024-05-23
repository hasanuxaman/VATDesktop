using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormMessageShow : Form
    {
        private string SelectedValue = string.Empty;
        private string vPrintCopy, vDefaultPrinter = string.Empty;
        private int PrintCopy;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static int NoOfCopies;
        private string Is3Plyer = string.Empty;
        private string PrinterName = string.Empty;
        private string DefaultPrinter = string.Empty;


        public FormMessageShow()
        {
            InitializeComponent();

        }

        private void FormLoad()
        {
           
        }

        public static string SelectOne(string message = "")
        {
            string SearchValue = string.Empty;
            FormMessageShow formMessagemessage = new FormMessageShow();
            try
            {
                //if (!string.IsNullOrEmpty(message))
                //    formMessagemessage.lblInvoiceNo.Text = message;
                //else
                //    formMessagemessage.lblInvoiceNo.Text =
                //        "A Latest Version Is Available for the Application. \nPlease Take Update.";


                formMessagemessage.ShowDialog();
            }
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormSalePrint", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormSalePrint", "SelectOne", exMessage);
            }
            #endregion Catch

            return SearchValue;

        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            
        }

        private void FormSalePrint_Load(object sender, EventArgs e)
        {
            

        }

        public static string InvoiceNo { get; set; }
    }
}
