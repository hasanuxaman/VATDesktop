using System;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
////
using VATViewModel.DTOs;

namespace VATClient.ReportPreview
{
    public partial class FormRptDebitNote : Form
    {
        public FormRptDebitNote()
        {
            InitializeComponent();       
            connVM = Program.OrdinaryLoad();
			 
			 
        }
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
         //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
         //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        
        //No server Side Method

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //No DAL Side Method
            try
            {
                Program.fromOpen = "Me";
                FormSaleSearch SSearch = new FormSaleSearch();
                Program.SalesType = "Debit";
                DataGridViewRow selectedRow = FormSaleSearch.SelectOne("Other");

                if(selectedRow!=null)
                {
                 
                    txtSalesInvoiceNo.Text = selectedRow.Cells["SalesInvoiceNo"].Value.ToString();
                    txtCustomerID.Text = selectedRow.Cells["CustomerID"].Value.ToString();
                    txtVehicleID.Text = selectedRow.Cells["VehicleID"].Value.ToString();
                    txtCustomerName.Text = selectedRow.Cells["CustomerName"].Value.ToString();
                    dtpInvoiceDate.Value = Convert.ToDateTime(selectedRow.Cells["InvoiceDate"].Value.ToString());
                }
                ////string result = FormSaleSearch.SelectOne("Other");
                ////if (result == "")
                ////{
                ////    return;
                ////}
                ////else //if (result == ""){return;}else//if (result != "")
                ////{
                ////    string[] Saleinfo = result.Split(FieldDelimeter.ToCharArray());

                ////    txtSalesInvoiceNo.Text = Saleinfo[0];
                ////    txtCustomerID.Text = Saleinfo[1];
                ////    txtVehicleID.Text = Saleinfo[7];
                ////    txtCustomerName.Text = Saleinfo[2];
                ////    dtpInvoiceDate.Value = Convert.ToDateTime(Saleinfo[13]);

                ////}
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }
            #endregion Catch
        }

        //No server Side Method
        private void btnVAT11_Click(object sender, EventArgs e)
        {
            Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
            if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
            {
                MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                return;
            }
            //ReportShowCustomer();
            //ReportShowVehicle();
            //ReportDebitNote();
            
        }
       
        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtCustomerName.Text = "";
            txtSalesInvoiceNo.Text = "";
            dtpInvoiceDate.Text = "";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormRptDebitNote_Load(object sender, EventArgs e)
        {
            //RollDetailsInfo();
        }

    }
}
