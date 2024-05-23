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
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormTransactionHistorySearch : Form
    {
        #region Constructors

        public FormTransactionHistorySearch()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        #endregion

        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private string SelectedValue = string.Empty;
        //public string VFIN = "316";

        #region Global Variables For BackGroundWorker

        private string TransData = string.Empty;
        private string TransResult = string.Empty;

        private DataTable TransSearchResult = null;

        private string varTransactionNo, varTransactionType, varTransactionDateFrom, varTransactionDateTo, varProductName;

        #endregion

        string TransFromDate, TransToDate;

        #endregion

        #region Methods

        private void ClearAllFields()
        {

            try
            {
                #region Statement

                txtTransactionNo.Text = "";
                cmbTransType.SelectedIndex = -1;
                dtpTransFromDate.Text = "";
                dtpTransToDate.Text = "";
                txtProduct.Text = "";
                dgvTransactionHistory.Rows.Clear();

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
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
                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAllFields", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
            }
            #endregion

        }

        private void NullCheck()
        {
            try
            {
                #region Statement

                if (dtpTransFromDate.Checked == false)
                {
                    TransFromDate = "";
                }
                else
                {
                    TransFromDate = dtpTransFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpTransToDate.Checked == false)
                {
                    TransToDate = "";
                }
                else
                {
                    TransToDate = dtpTransToDate.Value.ToString("yyyy-MMM-dd");
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

        private void Search()
        {

            try
            {
                #region Statement

                NullCheck();
                //string TransData = string.Empty;

                //  TransData =
                //      txtTransactionNo.Text.Trim() + FieldDelimeter +
                //      cmbTransType.Text.Trim() + FieldDelimeter +
                //   TransFromDate + FieldDelimeter +
                //   TransToDate + FieldDelimeter +
                //  txtProduct.Text.Trim() + FieldDelimeter
                //+ LineDelimeter;

                varTransactionNo = txtTransactionNo.Text.Trim();
                if (cmbTransType.SelectedIndex!=-1)
                {
                    varTransactionType = cmbTransType.Text.Trim();    
                }
                varTransactionDateFrom = TransFromDate;
                varTransactionDateTo = TransToDate;
                varProductName = txtProduct.Text.Trim();

                #region Commented Kodz

//// Start DoWork
                //string encriptedTransData = Converter.DESEncrypt(PassPhrase, EnKey, TransData);
                //CommonSoapClient TranaSearch = new CommonSoapClient();

                //string TransResult = TranaSearch.SearchTransanctionHistory(encriptedTransData.ToString(), Program.DatabaseName);  // Change 04
                //// End DoWork

                //// Start Complete
                //string decriptedTransData = Converter.DESDecrypt(PassPhrase, EnKey, TransResult);
                //string[] TransLines = decriptedTransData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                //dgvTransactionHistory.Rows.Clear();

                //for (int j = 0; j < Convert.ToInt32(TransLines.Length); j++)
                //{
                //    string[] TransFields = TransLines[j].Split(FieldDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                //    DataGridViewRow NewRow = new DataGridViewRow();

                //    dgvTransactionHistory.Rows.Add(NewRow);

                //    dgvTransactionHistory.Rows[j].Cells["TransactionNo"].Value = TransFields[0].ToString();
                //    dgvTransactionHistory.Rows[j].Cells["TransactionType"].Value = TransFields[1].ToString();
                //    dgvTransactionHistory.Rows[j].Cells["TransDate"].Value = Convert.ToDateTime(TransFields[2].ToString()).ToString("dd/MMM/yyyy");
                //    dgvTransactionHistory.Rows[j].Cells["Product"].Value = TransFields[3].ToString();
                //    dgvTransactionHistory.Rows[j].Cells["UOM"].Value = TransFields[4].ToString();
                //    dgvTransactionHistory.Rows[j].Cells["Quantity"].Value = Convert.ToDecimal(TransFields[5].ToString()).ToString("0.00");
                //    dgvTransactionHistory.Rows[j].Cells["Price"].Value = Convert.ToDecimal(TransFields[6].ToString()).ToString("0.00");
                //    dgvTransactionHistory.Rows[j].Cells["TradingMarkUp"].Value = Convert.ToDecimal(TransFields[7].ToString()).ToString("0.00");
                //    dgvTransactionHistory.Rows[j].Cells["SD"].Value = Convert.ToDecimal(TransFields[8].ToString()).ToString("0.00");
                //    dgvTransactionHistory.Rows[j].Cells["VATRate"].Value = Convert.ToDecimal(TransFields[9].ToString()).ToString("0.00");
                //    dgvTransactionHistory.Rows[j].Cells["TCost"].Value = Convert.ToDecimal(TransFields[10].ToString()).ToString("0.00");
                //    dgvTransactionHistory.Rows[j].Cells["CreatedBy"].Value = TransFields[11].ToString();
                //    dgvTransactionHistory.Rows[j].Cells["CreatedOn"].Value = TransFields[12].ToString();
                //    dgvTransactionHistory.Rows[j].Cells["LastModifiedBy"].Value = TransFields[13].ToString();
                //    dgvTransactionHistory.Rows[j].Cells["LastModifiedOn"].Value = TransFields[14].ToString();


                //    //dgvTransactionHistory.Rows[j].Cells["IssueNo"].Value = ReceiveFields[0].ToString();

                //}
                //// End Complete

                #endregion

                this.btnSearch.Enabled = false;
                this.progressBar1.Visible = true;

                backgroundWorkerSearch.RunWorkerAsync();

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "Search", exMessage);
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
                FileLogger.Log(this.Name, "Search", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "Search", exMessage);
            }
            #endregion

        }

        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        #region TextBox KeyDown Event

        private void txtTransactionNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtTransactionType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpAuditFromDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpAuditToDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void cmbItemNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtSalesPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtNBRPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtUOM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            btnSearch.Focus();
        }

        #endregion

        private void btnProduct_Click(object sender, EventArgs e)
        {
            //No DAL Side Method
            try
            {
                #region Statement

                
                Program.fromOpen = "Me";
                Program.R_F = "";

                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtProduct.Text = selectedRow.Cells["ProductName"].Value.ToString();
                }

               

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnProduct_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnProduct_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnProduct_Click", exMessage);
            }
            #endregion

        }

        private void FormTransactionHistorySearch_Load(object sender, EventArgs e)
        {

        }

        #region backgroundWorkerSearch Event

        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //this.btnSearch.Enabled = false;
                //this.progressBar1.Visible = true;

                // Start DoWork
                //string encriptedTransData = Converter.DESEncrypt(PassPhrase, EnKey, TransData);
                //CommonSoapClient TranaSearch = new CommonSoapClient();

                //TransResult = TranaSearch.SearchTransanctionHistory(encriptedTransData.ToString(), Program.DatabaseName);  // Change 04
                TransSearchResult = new DataTable();

                CommonDAL commonDal = new CommonDAL();

                TransSearchResult = commonDal.SearchTransanctionHistoryNew(varTransactionNo, varTransactionType, varTransactionDateFrom, varTransactionDateTo, varProductName, Program.DatabaseName,connVM);  // Change 04

                // End DoWork

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                // Start Complete
                //string decriptedTransData = Converter.DESDecrypt(PassPhrase, EnKey, TransResult);
                //string[] TransLines = decriptedTransData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                dgvTransactionHistory.Rows.Clear();
                int j = 0;
                foreach (DataRow item in TransSearchResult.Rows)
                {



                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvTransactionHistory.Rows.Add(NewRow);

                    dgvTransactionHistory.Rows[j].Cells["TransactionNo"].Value = item["TransactionNo"].ToString();// TransFields[0].ToString();
                    dgvTransactionHistory.Rows[j].Cells["TransactionType"].Value = item["TransactionType"].ToString();// TransFields[1].ToString();
                    dgvTransactionHistory.Rows[j].Cells["TransDate"].Value =  Convert.ToDateTime(item["TransactionDate"]).ToString("dd/MMM/yyyy");// Convert.ToDateTime(TransFields[2].ToString()).ToString("dd/MMM/yyyy");
                    dgvTransactionHistory.Rows[j].Cells["Product"].Value = item["ProductName"].ToString();// TransFields[3].ToString();
                    dgvTransactionHistory.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();// TransFields[4].ToString();
                    dgvTransactionHistory.Rows[j].Cells["Quantity"].Value = Convert.ToDecimal(item["Quantity"]).ToString("0.00");// Convert.ToDecimal(TransFields[5].ToString()).ToString("0.00");
                    dgvTransactionHistory.Rows[j].Cells["Price"].Value = Convert.ToDecimal(item["UPrice"]).ToString("0.00");// Convert.ToDecimal(TransFields[6].ToString()).ToString("0.00");
                    dgvTransactionHistory.Rows[j].Cells["TradingMarkUp"].Value = Convert.ToDecimal(item["TradingMarkup"]).ToString("0.00");// Convert.ToDecimal(TransFields[7].ToString()).ToString("0.00");
                    dgvTransactionHistory.Rows[j].Cells["SD"].Value = Convert.ToDecimal(item["SDRate"]).ToString("0.00");// Convert.ToDecimal(TransFields[8].ToString()).ToString("0.00");
                    dgvTransactionHistory.Rows[j].Cells["VATRate"].Value = Convert.ToDecimal(item["VATRate"]).ToString("0.00");// Convert.ToDecimal(TransFields[9].ToString()).ToString("0.00");
                    dgvTransactionHistory.Rows[j].Cells["TCost"].Value = Convert.ToDecimal(item["TCost"]).ToString("0.00");// Convert.ToDecimal(TransFields[10].ToString()).ToString("0.00");
                    dgvTransactionHistory.Rows[j].Cells["CreatedBy"].Value = item["CreatedBy"].ToString();// TransFields[11].ToString();
                    dgvTransactionHistory.Rows[j].Cells["CreatedOn"].Value = item["CreatedOn"].ToString();// TransFields[12].ToString();
                    dgvTransactionHistory.Rows[j].Cells["LastModifiedBy"].Value = item["LastModifiedBy"].ToString();// TransFields[13].ToString();
                    dgvTransactionHistory.Rows[j].Cells["LastModifiedOn"].Value = item["LastModifiedOn"].ToString();// TransFields[14].ToString();
                    j = j + 1;

                    //dgvTransactionHistory.Rows[j].Cells["IssueNo"].Value = ReceiveFields[0].ToString();

                }
                // End Complete
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        #endregion

        private void cmbTransType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

    }
}
