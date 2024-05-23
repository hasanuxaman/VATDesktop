using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormIsPrinting : Form
    {
        public static string PrinterName = "";
        public static string InvNo = "";


        public FormIsPrinting()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public static string SelectOne(string Printer, string Inv = "")
        {
            string SearchValue = string.Empty;

            try
            {
                PrinterName = Printer;
                InvNo = Inv;
                FormIsPrinting frmSalePrint = new FormIsPrinting();
                frmSalePrint.Show();
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

        private void FormIsPrinting_Load(object sender, EventArgs e)
        {
            ////label3.Text = "VAT 6.3 #" + InvNo + " is printing on " + PrinterName + "";
        }
    }
}
