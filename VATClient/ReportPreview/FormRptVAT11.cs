using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
////
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
using VATClient.ReportPreview;
using CrystalDecisions.Shared;
using System.Data.Odbc;
using System.Threading;
using VATViewModel.DTOs;

namespace VATClient.ReportPreview
{
    public partial class FormRptVAT11 : Form
    {
        public FormRptVAT11()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
			 
        }
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        //public string VFIN = "";
         //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
         //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }
        private void ClearAllFields()
        {
            txtCustomerName.Text = "";
            txtSalesInvoiceNo.Text = "";
            dtpInvoiceDate.Text = "";

        }

        private void btnVAT11_Click(object sender, EventArgs e)
        {
            //No server side method
            if (Program.CheckLicence(dtpInvoiceDate.Value) == true)
            {
                MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                return;
            }
            //MessageBox.Show("OK");
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void FormRptVAT11_Load(object sender, EventArgs e)
        {

        }
   
        //===========================================

        private void btnSearch_Click(object sender, EventArgs e)
        {

            try
            {
                Program.fromOpen = "Me";
                FormSaleSearch SSearch = new FormSaleSearch();
                Program.SalesType = "New";


                DataGridViewRow selectedRow = FormSaleSearch.SelectOne("Other");

                if (selectedRow != null && selectedRow.Selected==true)
                {
                    txtSalesInvoiceNo.Text = selectedRow.Cells["SalesInvoiceNo"].Value.ToString();
                    txtCustomerName.Text = selectedRow.Cells["CustomerName"].Value.ToString();
                    dtpInvoiceDate.Value = Convert.ToDateTime(selectedRow.Cells["InvoiceDate"].Value.ToString());
                }

                ////string result = FormSaleSearch.SelectOne("Other");
                ////if (result == "")
                ////{
                ////    return;
                ////}
                ////else //if (result != "")
                ////{
                ////    string[] Saleinfo = result.Split(FieldDelimeter.ToCharArray());

                ////    txtSalesInvoiceNo.Text = Saleinfo[0];

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

        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {

        }
        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void grbBankInformation_Enter(object sender, EventArgs e)
        {

        }


        //===========================================


    }
}
