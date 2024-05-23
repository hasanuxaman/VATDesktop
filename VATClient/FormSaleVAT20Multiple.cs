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
using VATServer.Library;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;
using VATClient.ReportPages;
using VATViewModel.DTOs;
using VATServer.License;

namespace VATClient
{
    public partial class FormSaleVAT20Multiple : Form
    {

        #region Constructors

        public FormSaleVAT20Multiple()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        #endregion

        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string p = "";

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        private DataGridViewRow selectedRow = new DataGridViewRow();
        public static string SelectedValue = string.Empty;
        private static string transactionType { get; set; }
        //public string VFIN = "303";
        private string Type = string.Empty;
        private string TradingItem = string.Empty;
        private string Print = string.Empty;
        private string post = string.Empty;
        private DataTable SaleResult;
        public string SaleInvoices = string.Empty;

        private string SalesFromDate, SalesToDate;
        private DataSet ReportResultVAT20;
        private static string SaleinvoiceNo;
        //private static string _transactionType;
        #endregion


        public static string SelectOne(string type = null)
        {
            //string frmSearchSelectValue = String.Empty; 

            transactionType = type;
            #region try

            try
            {
                SelectedValue = "";
                FormSaleVAT20Multiple frmProductCategorySearch = new FormSaleVAT20Multiple();
                frmProductCategorySearch.ShowDialog();
                return SelectedValue;
            }
            #endregion
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormProductCategorySearch", "SelectOne",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormProductCategorySearch", "SelectOne",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormProductCategorySearch", "SelectOne",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormProductCategorySearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormProductCategorySearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormProductCategorySearch", "SelectOne",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormProductCategorySearch", "SelectOne",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "ProductGroupSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormProductCategorySearch", "SelectOne", exMessage);
            }

            #endregion

            return SelectedValue;
        }


        private void SingleSaleInvoice(string saleInvoiceNo)
        {
            //txtInvoiceNo.Text = saleInvoiceNo;
            searchDT();
        }

        private void NullCheck()
        {
            try
            {
                #region Statement

                if (dtpSaleFromDate.Checked == false)
                {
                    SalesFromDate = "1900-01-01";
                    //SalesFromDate = Convert.ToDateTime(dtpSaleFromDate.MinDate.ToString("yyyy-MMM-dd"));
                }
                else
                {
                    SalesFromDate = dtpSaleFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpSaleToDate.Checked == false)
                {
                    SalesToDate = "9000-12-31";
                    //SalesToDate = Convert.ToDateTime(dtpSaleToDate.MaxDate.ToString("yyyy-MMM-dd"));
                }
                else
                {
                    SalesToDate = dtpSaleToDate.Value.ToString("yyyy-MMM-dd");
                }

                if (txtGrandTotalFrom.Text == "")
                {
                    txtGrandTotalFrom.Text = "0.00";
                }

                if (txtGrandTotalTo.Text == "")
                {
                    txtGrandTotalTo.Text = "0.00";
                }



                if (txtVatAmountTo.Text == "")
                {
                    txtVatAmountTo.Text = "0.00";
                }

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "NullCheck", exMessage);
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
                FileLogger.Log(this.Name, "NullCheck", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "NullCheck", exMessage);
            }

            #endregion

        }


        public void ClearAllFields()
        {
            txtCustomerID.Text = "";
            txtInvoiceNo.Text = "";
            txtSerialNo.Text = "";
            txtVehicleNo.Text = "";
            txtVehicleID.Text = "";
            dtpSaleFromDate.Text = "";
            dtpSaleToDate.Text = "";
            dgvSalesHistory.Rows.Clear();
            txtCustomerName.Text = "";
            txtVehicleType.Text = "";
            cmbPrint.SelectedIndex = -1;
            cmbTrading.SelectedIndex = -1;
            cmbPost.SelectedIndex = -1;
            cmbType.SelectedIndex = -1;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        #region TextBox KeyDown Event

        private void txtInvoiceNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }



        private void txtCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtVehicleType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpSaleFromDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpSaleToDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtVatAmountFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtVatAmountTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtGrandTotalFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtGrandTotalTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtVehicleNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        #endregion

        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void GridSeleted()
        {
            try
            {
                #region Statement


                if (dgvSalesHistory.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                DataGridViewSelectedRowCollection selectedRows = dgvSalesHistory.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }

                ////if (dgvSalesHistory.Rows.Count > 0)
                ////{
                ////    string IssueInfo = string.Empty;
                ////    int ColIndex = dgvSalesHistory.CurrentCell.ColumnIndex;
                ////    int RowIndex1 = dgvSalesHistory.CurrentCell.RowIndex;
                ////    if (RowIndex1 >= 0)
                ////    {
                ////        string SalesInvoiceNo = dgvSalesHistory.Rows[RowIndex1].Cells["SalesInvoiceNo"].Value.ToString();
                ////        string CustomerID = dgvSalesHistory.Rows[RowIndex1].Cells["CustomerID"].Value.ToString();
                ////        string CustomerName = dgvSalesHistory.Rows[RowIndex1].Cells["CustomerName"].Value.ToString();
                ////        string CustomerGroupName = dgvSalesHistory.Rows[RowIndex1].Cells["CustomerGroupName"].Value.ToString();
                ////        string DeliveryAddress1 = dgvSalesHistory.Rows[RowIndex1].Cells["DeliveryAddress1"].Value.ToString();
                ////        string DeliveryAddress2 = dgvSalesHistory.Rows[RowIndex1].Cells["DeliveryAddress2"].Value.ToString();
                ////        string DeliveryAddress3 = dgvSalesHistory.Rows[RowIndex1].Cells["DeliveryAddress3"].Value.ToString();
                ////        string VehicleID = dgvSalesHistory.Rows[RowIndex1].Cells["VehicleID"].Value.ToString();
                ////        string VehicleType = dgvSalesHistory.Rows[RowIndex1].Cells["VehicleType"].Value.ToString();
                ////        string VehicleNo = dgvSalesHistory.Rows[RowIndex1].Cells["VehicleNo"].Value.ToString();
                ////        string TotalAmount = dgvSalesHistory.Rows[RowIndex1].Cells["TotalAmount"].Value.ToString();
                ////        string TotalVATAmount = dgvSalesHistory.Rows[RowIndex1].Cells["TotalVATAmount"].Value.ToString();
                ////        string SerialNo = dgvSalesHistory.Rows[RowIndex1].Cells["SerialNo"].Value.ToString();
                ////        string InvoiceDate = dgvSalesHistory.Rows[RowIndex1].Cells["InvoiceDate"].Value.ToString();
                ////        string Comments = dgvSalesHistory.Rows[RowIndex1].Cells["Comments"].Value.ToString();

                ////        string SaleType = dgvSalesHistory.Rows[RowIndex1].Cells["SaleType"].Value.ToString();
                ////        string PID = dgvSalesHistory.Rows[RowIndex1].Cells["PID"].Value.ToString();
                ////        TradingItem = dgvSalesHistory.Rows[RowIndex1].Cells["Trading"].Value.ToString();
                ////        string Isprint = dgvSalesHistory.Rows[RowIndex1].Cells["Isprint"].Value.ToString();
                ////        string TenderId = dgvSalesHistory.Rows[RowIndex1].Cells["TenderId"].Value.ToString();
                ////        string Post = dgvSalesHistory.Rows[RowIndex1].Cells["Post"].Value.ToString();



                ////        IssueInfo =
                ////            SalesInvoiceNo + FieldDelimeter +
                ////            CustomerID + FieldDelimeter +
                ////            CustomerName + FieldDelimeter +
                ////            CustomerGroupName + FieldDelimeter +
                ////            DeliveryAddress1 + FieldDelimeter +
                ////            DeliveryAddress2 + FieldDelimeter +
                ////            DeliveryAddress3 + FieldDelimeter +
                ////            VehicleID + FieldDelimeter +
                ////            VehicleType + FieldDelimeter +
                ////            VehicleNo + FieldDelimeter +
                ////            TotalAmount + FieldDelimeter +
                ////            TotalVATAmount + FieldDelimeter +
                ////            SerialNo + FieldDelimeter +
                ////            InvoiceDate + FieldDelimeter +
                ////            Comments + FieldDelimeter +
                ////            SaleType + FieldDelimeter +
                ////            PID + FieldDelimeter +
                ////            Trading + FieldDelimeter +
                ////            Isprint + FieldDelimeter +
                ////            TenderId + FieldDelimeter +
                ////            Post;

                ////        SelectedValue = IssueInfo;
                ////    }
                ////}
                this.Close();

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "GridSeleted", exMessage);
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
                FileLogger.Log(this.Name, "GridSeleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "GridSeleted", exMessage);
            }

            #endregion

        }

        private void dgvSalesHistory_DoubleClick(object sender, EventArgs e)
        {

        }

        private void txtVatAmountFrom_Leave(object sender, EventArgs e)
        {
        }

        private void txtVatAmountTo_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtVatAmountTo, "VAT Amount To");
        }

        private void txtGrandTotalFrom_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtGrandTotalFrom, "Grand Amount From");
        }

        private void txtGrandTotalTo_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtGrandTotalTo, "Grand Amount To");
        }

        // == Search == //

        #region // == Search == //

        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchDT();
        }

        private void searchDT()
        {
            try
            {
                this.btnSearch.Enabled = false;
                this.progressBar1.Visible = true;

                NullCheck();

                TradingItem = cmbTrading.SelectedIndex != -1 ? cmbTrading.Text.Trim() : string.Empty;
                Type = cmbType.SelectedIndex != -1 ? cmbType.Text.Trim() : string.Empty;
                Print = cmbPrint.SelectedIndex != -1 ? cmbPrint.Text.Trim() : string.Empty;
                //post = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : string.Empty;
                post = "Y";
                bgwSearch.RunWorkerAsync();
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "searchDT", exMessage);
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
                FileLogger.Log(this.Name, "searchDT", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "searchDT", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "searchDT", exMessage);
            }

            #endregion
        }

        private void bgwSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                SaleDAL saleDal = new SaleDAL();

                //if (!string.IsNullOrEmpty(SaleInvoices))
                //{
                //    txtInvoiceNo.Text = SaleInvoices;
                //}


                //               SaleResult = saleDal.SearchSalesHeaderDTNew(
                //txtInvoiceNo.Text.Trim(), txtCustomerName.Text.Trim(),
                //                   txtCustomerGroupName.Text.Trim(),
                //                   txtVehicleType.Text.Trim(), txtVehicleNo.Text.Trim(), txtSerialNo.Text.Trim(), SalesFromDate,
                //                   SalesToDate, Type, TradingItem, Print, "Export", post,txtEXPFormNo.Text.Trim()); // Change 04

                string[] cValues = {  txtInvoiceNo.Text.Trim(), 
                                      txtCustomerName.Text.Trim(),
                                      txtCustomerGroupName.Text.Trim(),
                                      txtVehicleType.Text.Trim(), 
                                      txtVehicleNo.Text.Trim(), 
                                      txtSerialNo.Text.Trim(), 
                                      SalesFromDate,
                                      SalesToDate, 
                                      Type, 
                                      TradingItem, 
                                      Print, 
                                      post, 
                                      txtEXPFormNo.Text.Trim() };
                string[] cFields = { "sih.SalesInvoiceNo like",
                                      "c.CustomerName like",
                                      "cg.CustomerGroupName  like",
                                      "v.VehicleType like",
                                      "v.VehicleNo like",
                                      "sih.SerialNo like",
                                      "sih.InvoiceDateTime>",
                                      "sih.InvoiceDateTime<",
                                      "sih.SaleType like",
                                      "sih.Trading like",
                                      "sih.IsPrint like",
                                      "sih.post like",
                                      "sih.EXPFormNo like",
                                     };

                SaleResult = saleDal.SelectAll(0, cFields, cValues,null, null, null, true, transactionType,"N",connVM);

                if (SaleResult.Rows.Count > 0)
                {
                    var rows = SaleResult.AsEnumerable().Where(x =>
                        x["Is6_3TollCompleted"].ToString() == "N" ||
                        string.IsNullOrEmpty(x["Is6_3TollCompleted"].ToString()));

                    SaleResult = rows.Any() ? rows.CopyToDataTable() : new DataTable();
                }

                #endregion
            }
            #region catch

            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSearch_DoWork", exMessage);
            }

            #endregion
        }

        private void bgwSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                dgvSalesHistory.Rows.Clear();
                int j = 0;
                foreach (DataRow item in SaleResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvSalesHistory.Rows.Add(NewRow);

                    dgvSalesHistory.Rows[j].Cells["SalesInvoiceNo"].Value = item["SalesInvoiceNo"].ToString();
                    // SaleFields[0].ToString();
                    dgvSalesHistory.Rows[j].Cells["CustomerID"].Value = item["CustomerID"].ToString();
                    // SaleFields[1].ToString();
                    dgvSalesHistory.Rows[j].Cells["CustomerName"].Value = item["CustomerName"].ToString();
                    // SaleFields[2].ToString();
                    dgvSalesHistory.Rows[j].Cells["CustomerGroupName"].Value = item["CustomerGroupName"].ToString();
                    // SaleFields[3].ToString();
                    dgvSalesHistory.Rows[j].Cells["DeliveryAddress1"].Value = item["DeliveryAddress1"].ToString();
                    // SaleFields[4].ToString();
                    dgvSalesHistory.Rows[j].Cells["DeliveryAddress2"].Value = item["DeliveryAddress2"].ToString();
                    // SaleFields[5].ToString();
                    dgvSalesHistory.Rows[j].Cells["DeliveryAddress3"].Value = item["DeliveryAddress3"].ToString();
                    // SaleFields[6].ToString();
                    dgvSalesHistory.Rows[j].Cells["VehicleID"].Value = item["VehicleID"].ToString();
                    // SaleFields[7].ToString();
                    dgvSalesHistory.Rows[j].Cells["VehicleType"].Value = item["VehicleType"].ToString();
                    // SaleFields[8].ToString();
                    dgvSalesHistory.Rows[j].Cells["VehicleNo"].Value = item["VehicleNo"].ToString();
                    // SaleFields[9].ToString();
                    dgvSalesHistory.Rows[j].Cells["TotalAmount"].Value = item["TotalAmount"].ToString();
                    // SaleFields[10].ToString();
                    dgvSalesHistory.Rows[j].Cells["TotalVATAmount"].Value = item["TotalVATAmount"].ToString();
                    // SaleFields[11].ToString();
                    dgvSalesHistory.Rows[j].Cells["SerialNo"].Value = item["SerialNo"].ToString();
                    // SaleFields[12].ToString();
                    //dgvSalesHistory.Rows[j].Cells["InvoiceDate"].Value = item["InvoiceDate"].ToString();
                    dgvSalesHistory.Rows[j].Cells["InvoiceDate"].Value = item["InvoiceDateTime"].ToString();
                    // Convert.ToDateTime(SaleFields[13]).ToString("dd/MMM/yyyy");
                    dgvSalesHistory.Rows[j].Cells["DeliveryDate"].Value = item["DeliveryDate"].ToString();
                    // Convert.ToDateTime(SaleFields[13]).ToString("dd/MMM/yyyy");
                    dgvSalesHistory.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                    // SaleFields[14].ToString();
                    dgvSalesHistory.Rows[j].Cells["SaleType"].Value = item["SaleType"].ToString();
                    // SaleFields[15].ToString();
                    dgvSalesHistory.Rows[j].Cells["PID"].Value = item["PID"].ToString(); // SaleFields[16].ToString();
                    dgvSalesHistory.Rows[j].Cells["Trading"].Value = item["Trading"].ToString();
                    // SaleFields[17].ToString();
                    dgvSalesHistory.Rows[j].Cells["Isprint"].Value = item["IsPrint"].ToString();
                    // SaleFields[18].ToString();
                    dgvSalesHistory.Rows[j].Cells["TenderId"].Value = item["TenderId"].ToString();
                    // SaleFields[19].ToString();
                    dgvSalesHistory.Rows[j].Cells["Post"].Value = item["Post"].ToString(); // SaleFields[20].ToString();
                    dgvSalesHistory.Rows[j].Cells["CurrencyID"].Value = item["CurrencyID"].ToString();
                    // SaleFields[20].ToString();
                    dgvSalesHistory.Rows[j].Cells["CurrencyCode"].Value = item["CurrencyCode"].ToString();
                    // SaleFields[20].ToString();
                    dgvSalesHistory.Rows[j].Cells["CurrencyRateFromBDT"].Value = item["CurrencyRateFromBDT"].ToString();
                    // SaleFields[20].ToString();
                    dgvSalesHistory.Rows[j].Cells["SaleReturnId"].Value = item["SaleReturnId"].ToString();
                    // SaleFields[20].ToString();
                    dgvSalesHistory.Rows[j].Cells["transactionType"].Value = item["transactionType"].ToString();
                    // SaleFields[20].ToString();
                    dgvSalesHistory.Rows[j].Cells["AlReadyPrint"].Value = item["AlReadyPrint"].ToString();
                    // SaleFields[20].ToString();
                    dgvSalesHistory.Rows[j].Cells["LCNumber"].Value = item["LCNumber"].ToString();
                    // SaleFields[20].ToString();
                    j = j + 1;

                }

                dgvSalesHistory.Columns["DeliveryAddress2"].Visible = false;
                dgvSalesHistory.Columns["DeliveryAddress3"].Visible = false;
                dgvSalesHistory.Columns["VehicleID"].Visible = false;
                dgvSalesHistory.Columns["TenderId"].Visible = false;
                dgvSalesHistory.Columns["Trading"].Visible = false;
                dgvSalesHistory.Columns["Comments"].Visible = false;

                // End Complete

                if (!string.IsNullOrEmpty(SaleinvoiceNo))
                {
                    if (dgvSalesHistory.Rows.Count > 0)
                    {
                        dgvSalesHistory[0, 0].Value = true;
                    }
                }

                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                //if (!string.IsNullOrEmpty(SaleinvoiceNo))
                //{
                //    this.btnSearch.Enabled = false;
                //}
                //else
                //{
                //    this.btnSearch.Enabled = true;
                //}
                this.progressBar1.Visible = false;
                LRecordCount.Text = "Total Record Count: " + dgvSalesHistory.Rows.Count.ToString();
            }
        }

        #endregion

        private void cmbType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbPrint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void FormSaleVAT20Multiple_Load(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                cmbType.Text = Program.SalesType;
                cmbTrading.Text = Program.Trading;

                if (!string.IsNullOrEmpty(SaleinvoiceNo))
                {
                    SingleSaleInvoice(SaleinvoiceNo);

                }
                txtPortFrom.Focus();
                //if (Program.fromOpen == "Me")
                //{

                //}

                //FormSale frms = new FormSale();

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormSaleVAT20Multiple_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormSaleVAT20Multiple_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormSaleVAT20Multiple_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormSaleVAT20Multiple_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormSaleVAT20Multiple_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormSaleVAT20Multiple_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormSaleVAT20Multiple_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormSaleVAT20Multiple_Load", exMessage);
            }

            #endregion
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {


            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            if (txtPortFrom.Text == "")
            {
                MessageBox.Show("Please Enter Port From", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPortFrom.Focus();
                return;
            }
            if (txtPortTo.Text == "")
            {
                MessageBox.Show("Please Enter Port To", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPortTo.Focus();
                return;
            }
            if (dgvSalesHistory.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to show", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;

            }
            progressBar1.Visible = true;
            string invoiceNo;
            DataTable dt0 = new DataTable();
            DataTable dt1 = new DataTable();
            for (int i = 0; i < dgvSalesHistory.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvSalesHistory["Select", i].Value) == true)
                {
                    invoiceNo = dgvSalesHistory["SalesInvoiceNo", i].Value.ToString();
                    if (i == 0)
                    {
                        SaleInvoices = invoiceNo;
                    }
                    else
                    {
                        SaleInvoices = SaleInvoices + " , " + invoiceNo;
                    }



                    ReportDSDAL ShowReport = new ReportDSDAL();

                    //ReportResultVAT20 = ShowReport.VAT20ReportNew(invoiceNo);
                    dt0.Merge(ReportResultVAT20.Tables[0]);
                    dt1.Merge(ReportResultVAT20.Tables[1]);

                }
            }
            dt0.TableName = "DsVAT20Item";
            dt1.TableName = "DsVAT20Pack";

            DataSet ReportResultMultipleVAT20 = new DataSet();
            ReportResultMultipleVAT20.Tables.Add(dt0);
            ReportResultMultipleVAT20.Tables.Add(dt1);

            ReportClass objrpt = new ReportClass();

            objrpt = new RptVAT20();
            objrpt.SetDataSource(ReportResultMultipleVAT20);
            objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
            objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
            objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
            objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
            objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
            objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
            objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
            objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + Program.ZipCode + "'";
            objrpt.DataDefinition.FormulaFields["SalesInvoiceNo"].Text = "'" + SaleInvoices + "'";
            objrpt.DataDefinition.FormulaFields["PortFrom"].Text = "'" + txtPortFrom.Text + "'";
            objrpt.DataDefinition.FormulaFields["PortTo"].Text = "'" + txtPortTo.Text + "'";

            FormReport reports = new FormReport();
            objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
            reports.crystalReportViewer1.Refresh();
            reports.setReportSource(objrpt);
            if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();
            reports.WindowState = FormWindowState.Maximized;
            reports.Show();
            progressBar1.Visible = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {


                SelectedValue = "";
                for (int i = 0; i < dgvSalesHistory.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dgvSalesHistory["Select", i].Value) == true)
                    {
                        string invoiceNo = dgvSalesHistory["SalesInvoiceNo", i].Value.ToString()
                            + "~" + dgvSalesHistory["InvoiceDate", i].Value.ToString()
                            + "~" + dgvSalesHistory["CustomerName", i].Value.ToString()
                            + "~" + dgvSalesHistory["CustomerID", i].Value.ToString()
                            + "~" + dgvSalesHistory["DeliveryAddress1", i].Value.ToString()
                            + "~" + dgvSalesHistory["CurrencyCode", i].Value.ToString()
                            + "~" + dgvSalesHistory["CurrencyID", i].Value.ToString()
                            + "~" + dgvSalesHistory["IsPrint", i].Value.ToString()
                            ;
                        if (i == 0)
                        {
                            SelectedValue = invoiceNo;
                        }
                        else
                        {
                            SelectedValue = SelectedValue + " ^ " + invoiceNo;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            this.Close();
        }

        private void txtPortFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtPortTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            searchDT();
        }

        private void txtVehicleNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvSalesHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}