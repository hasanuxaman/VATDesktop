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
using VATServer.Library;
using VATClient.ModelDTO;
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormCurrencyConversionSearch : Form
    {
        public FormCurrencyConversionSearch()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        DataGridViewRow selectedRow = new DataGridViewRow();
        private DataTable CurrencyConversionResult;
        private DataTable currenciesResult;
        List<CurrencyDTO> currencyDtos = new List<CurrencyDTO>();
        private string activeStatus = string.Empty;
        private string currencyRate = string.Empty;
        private string conversionDate = string.Empty;
        public static DataGridViewRow SelectOne()
        {
            DataGridViewRow selectedRowTemp = null;
            try
            {
                #region Statement

                FormCurrencyConversionSearch frmCurrencyConversionSearch = new FormCurrencyConversionSearch();
                //transactionType = tType;
                frmCurrencyConversionSearch.ShowDialog();
                selectedRowTemp = frmCurrencyConversionSearch.selectedRow;

                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormSaleSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormSaleSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormSaleSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormSaleSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormSaleSearch", "SelectOne", exMessage);
            }
            #endregion

            return selectedRowTemp;
        }

        private void dgvCurrencyConversion_DoubleClick(object sender, EventArgs e)
        {
            GridSeleted();
        }
        private void GridSeleted()
        {
            try
            {
                #region Statement


                if (dgvCurrencyConversion.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataGridViewSelectedRowCollection selectedRows = dgvCurrencyConversion.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }

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

        private void bgwSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement


                //CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();

                //CurrencyConversionResult = currencyConversionDal.SearchCurrencyConversion(Convert.ToString(cmbFrom.SelectedValue), "", "", Convert.ToString(cmbTo.SelectedValue), "", "", "Y");


                #endregion
            }
            #region catch

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
            //try
            //{
            //    #region Statement

            //    // Start Complete

            //    dgvCurrencyConversion.Rows.Clear();
            //    int j = 0;

            //    foreach (DataRow item in CurrencyConversionResult.Rows)
            //    {
            //        DataGridViewRow NewRow = new DataGridViewRow();

            //        dgvCurrencyConversion.Rows.Add(NewRow);

            //        dgvCurrencyConversion.Rows[j].Cells["CurrencyConversionID"].Value = item["CurrencyConversionId"].ToString();
            //        dgvCurrencyConversion.Rows[j].Cells["From"].Value = item["CurrencyCodeFrom"].ToString();
            //        dgvCurrencyConversion.Rows[j].Cells["To"].Value = item["CurrencyCodeTo"].ToString();
            //        dgvCurrencyConversion.Rows[j].Cells["Rate"].Value = item["Country"].ToString();
            //       // dgvCurrencyConversion.Rows[j].Cells["ConversionDate"].Value = item["Comments"].ToString();
            //        dgvCurrencyConversion.Rows[j].Cells["ActiveStatus"].Value = item["ActiveStatus"].ToString();
            //        j = j + 1;

            //    }


            //    #endregion
            //}
            //#region catch

            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}

            //catch (SoapHeaderException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }

            //    FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", exMessage);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (SoapException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (Exception ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", exMessage);
            //}
            //#endregion

            //finally
            //{
            //    this.btnSearch.Enabled = true;
            //    this.progressBar1.Visible = false;
            //}
        }

        private void FormCurrencyConversionSearch_Load(object sender, EventArgs e)
        {
            //FormLoad();
            searchDT();
        }
        private void FormLoad()
        {
            #region Currencies
            string CurrencyName = string.Empty;
            string CurrencyCode = string.Empty;
            string Country = string.Empty;
            string ActiveStatus = string.Empty;

            #endregion Currencies
            this.btnSearch.Enabled = false;
            this.progressBar1.Visible = true;
         //  bgwLoad.RunWorkerAsync();
        }
        private void searchDT()
        {
            try
            {
                activeStatus = cmbActive.SelectedIndex != -1 ? cmbActive.Text.Trim() : string.Empty;
                this.btnSearch.Enabled = false;
                this.progressBar1.Visible = true;
                bgwLoad.RunWorkerAsync();
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchDT();
        }

        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            #region currencies

          
            currencyRate = txtCurrencyRate.Text.Trim();
            //CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();
            ICurrencyConversion currencyConversionDal = OrdinaryVATDesktop.GetObject<CurrencyConversionDAL, CurrencyConversionRepo, ICurrencyConversion>(OrdinaryVATDesktop.IsWCF);


            //CurrencyConversionResult = currencyConversionDal.SearchCurrencyConversion("", txtFrom.Text.Trim(), "", "", txtTo.Text.Trim(), "", currencyRate,dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"),activeStatus);
            string[] cValues = { txtFrom.Text.Trim(),txtTo.Text.Trim(), currencyRate, dtpConversionDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"), activeStatus };
            string[] cFields = { "cc.CurrencyCodeFrom like", "cc.CurrencyCodeTo like", "cc.CurrencyRate", "cc.ConversionDate<=", "cc.ActiveStatus like" };
            CurrencyConversionResult = currencyConversionDal.SelectAll(0, cFields, cValues, null, null, true,connVM);
            
            #endregion currencies
        }

        private void bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region currencies
            //cmbFrom.Items.Clear();
            //cmbTo.Items.Clear();


            //currencyDtos.Clear();
            //foreach (DataRow item2 in currenciesResult.Rows)
            //{
            //    var prod = new CurrencyDTO();
            //    prod.CurrencyId = item2["CurrencyId"].ToString();
            //    prod.CurrencyName = item2["CurrencyName"].ToString();
            //    prod.CurrencyCode = item2["CurrencyCode"].ToString();
            //    prod.Country = item2["Country"].ToString();
            //    prod.Comments = item2["Comments"].ToString();
            //    prod.ActiveStatus = item2["ActiveStatus"].ToString();
            //    //cmbFrom.Items.Add(item2["CurrencyId"].ToString());
            //    //cmbTo.Items.Add(item2["CurrencyId"].ToString());

            //    currencyDtos.Add(prod);
            //}//End FOR


            //cmbFrom.Items.Insert(0, "Select");
            //cmbTo.Items.Insert(0, "Select");

            //cmbFrom.DataSource = currenciesResult;
            //cmbFrom.DisplayMember = "CurrencyCode";
            //cmbFrom.ValueMember = "CurrencyId";

            //cmbTo.DataSource = currenciesResult;
            //cmbTo.DisplayMember = "CurrencyCode";
            //cmbTo.ValueMember = "CurrencyId";

            //cmbFrom.SelectedIndex = 0;
            //cmbTo.SelectedIndex = 0;

            #endregion currencies

            try
            {
                #region Statement

                // Start Complete

                dgvCurrencyConversion.Rows.Clear();
                int j = 0;

                foreach (DataRow item in CurrencyConversionResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvCurrencyConversion.Rows.Add(NewRow);

                    dgvCurrencyConversion.Rows[j].Cells["CurrencyConversionID"].Value = item["CurrencyConversionId"].ToString();
                    dgvCurrencyConversion.Rows[j].Cells["From"].Value = item["CurrencyCodeFrom"].ToString();
                    dgvCurrencyConversion.Rows[j].Cells["To"].Value = item["CurrencyCodeTo"].ToString();
                    dgvCurrencyConversion.Rows[j].Cells["Rate"].Value = item["CurrencyRate"].ToString();
                    dgvCurrencyConversion.Rows[j].Cells["ConversionDate"].Value = item["ConversionDate"].ToString();
                    dgvCurrencyConversion.Rows[j].Cells["ActiveStatus"].Value = item["ActiveStatus"].ToString();
                    dgvCurrencyConversion.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                    dgvCurrencyConversion.Rows[j].Cells["CurrencyCodeF"].Value = item["CurrencyCodeF"].ToString();
                    dgvCurrencyConversion.Rows[j].Cells["CurrencyCodeT"].Value = item["CurrencyCodeT"].ToString();

                    j = j + 1;

                }


                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                LRecordCount.Text = "Record Count: " + dgvCurrencyConversion.Rows.Count.ToString();
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }
        private void ClearAll()
        {
            DateTime vdateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"));
            txtCurrencyRate.Text = "";
            dtpConversionDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + " " + vdateTime.ToString("HH:mm:ss"));//Program.SessionDate;
            txtFrom.Text = "";
            txtTo.Text = "";
            cmbActive.SelectedIndex = -1;


        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void txtCurrencyRate_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtCurrencyRate, "CurrencyRate");
        }
    }
}
