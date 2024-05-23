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
    public partial class FormSalePrint : Form
    {
        private string SelectedValue = string.Empty;
        private string vPrintCopy, vDefaultPrinter = string.Empty;
        private int PrintCopy;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static int NoOfCopies;
        private string Is3Plyer = string.Empty;
        private string PrinterName = string.Empty;
        private string DefaultPrinter = string.Empty;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();


        public FormSalePrint()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        private void FormLoad()
        {
            #region Set Printer Name

            List<string> names = new List<string>();
            foreach (string printerNames in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                names.Add(printerNames);
            }
            cmbPrinterName.DataSource = names;
            PrinterSettings printerSettings = new PrinterSettings();
            DefaultPrinter = printerSettings.PrinterName;
            cmbPrinterName.Text = DefaultPrinter;


            #endregion Set Printer Name

            #region No of Print
            if (!string.IsNullOrEmpty(Is3Plyer))
            {
                if (Is3Plyer.ToUpper() == "Y")
                {
                    nudPrintCopy.Value = 1;
                    nudPrintCopy.Enabled = false;
                }
                else
                {
                    nudPrintCopy.Value = PrintCopy;
                    nudPrintCopy.Enabled = true;

                }
            }
            else
            {
                nudPrintCopy.Value = PrintCopy;
                nudPrintCopy.Enabled = true;
            }
            #endregion No of Print
            #region Already print number
            if (NoOfCopies > 0)
            {
                lblAlreadyPrint.Text = lblAlreadyPrint.Text + " " + NoOfCopies + " copies of invoice no";
                lblInvoiceNo.Text = InvoiceNo;
            }
            else
            {
                lblAlreadyPrint.Text = "";
            }
            #endregion Already print number
        }

        public static string SelectOne(int alreadyPrint, string invoiceNo = "")
        {
            string SearchValue = string.Empty;

            try
            {
                NoOfCopies = alreadyPrint;
                FormSalePrint frmSalePrint = new FormSalePrint();
                InvoiceNo = invoiceNo;

                frmSalePrint.ShowDialog();
                SearchValue = frmSalePrint.SelectedValue;
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log("FormSalePrint", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], "FormSalePrint", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormSalePrint", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormSalePrint", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormSalePrint", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormSalePrint", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormSalePrint", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormSalePrint", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

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
            PrinterName = cmbPrinterName.SelectedItem.ToString();
            string results = String.Empty;

            results = nudPrintCopy.Value.ToString() + FieldDelimeter + "Y" + FieldDelimeter + PrinterName;
            SelectedValue = results;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            string results = String.Empty;
            //results = nudPrintCopy.Value.ToString() + FieldDelimeter + "N" + FieldDelimeter + PrinterName;
            results = "0" + FieldDelimeter + "N" + FieldDelimeter + PrinterName;
            SelectedValue = results;
            this.Close();
        }

        private void FormSalePrint_Load(object sender, EventArgs e)
        {
            CommonDAL commonDal = new CommonDAL();
            vPrintCopy = commonDal.settingsDesktop("Sale", "ReportNumberOfCopiesPrint",null,connVM);
            vDefaultPrinter = commonDal.settingsDesktop("Printer", "DefaultPrinter",null,connVM);

            //PrinterSettings settings = new PrinterSettings();
            //var name = settings.PrinterName;

            if (string.IsNullOrEmpty(vPrintCopy)
                || string.IsNullOrEmpty(vDefaultPrinter))
            {

                MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                return;
            }
            PrintCopy = Convert.ToInt32(vPrintCopy);
            DefaultPrinter = vDefaultPrinter;
            Is3Plyer = commonDal.settingsDesktop("Sale", "Page3Plyer",null,connVM);

            FormLoad();

        }

        public static string InvoiceNo { get; set; }
    }
}
