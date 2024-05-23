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
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATDesktop.Repo;


namespace VATClient
{
    public partial class FormSalesInvoiceExpSearch : Form
    {
        public FormSalesInvoiceExpSearch()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }
        #region Global Variable 
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private DataTable SalesInvoiceExpResult;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private string SelectedValue = string.Empty;
        public static string SelectOne()
        #endregion
        {
            string SearchValue = string.Empty;
            try
            {
                FormSalesInvoiceExpSearch frmHSCodeSearch = new FormSalesInvoiceExpSearch();
                frmHSCodeSearch.ShowDialog();
                SearchValue = frmHSCodeSearch.SelectedValue;
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormBankInformationSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormBankInformationSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormBankInformationSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormBankInformationSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormBankInformationSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormBankInformationSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormBankInformationSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormBankInformationSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormBankInformationSearch", "SelectOne", exMessage);
            }
            #endregion Catch

            return SearchValue;

        }
        private void dgvHSCode_DoubleClick(object sender, EventArgs e)
        {
            GridSelected();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.btnSearch.Visible = false;
            this.progressBar1.Visible = true;
            bgwSearch.RunWorkerAsync();
        }

        private void bgwSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try
            try
            {
                //SalesInvoiceExpDAL SDAL = new SalesInvoiceExpDAL();
                ISalesInvoiceExp SDAL = OrdinaryVATDesktop.GetObject<SalesInvoiceExpDAL, SalesInvoiceExpRepo, ISalesInvoiceExp>(OrdinaryVATDesktop.IsWCF);
                string[] cFields = { "ID like", "PINo like" };
                string[] cValues = { txtId.Text.Trim(),txtPINo.Text.Trim() };
                SalesInvoiceExpResult = SDAL.SelectAll(0, cFields, cValues, null, null, true,connVM);
            #endregion
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
            }
            #endregion

        }

        private void bgwSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try
            try
            {
                int j = 0;
                dgvSalesInvoiceExp.Rows.Clear();
                foreach (DataRow item2 in SalesInvoiceExpResult.Rows)
                {

                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvSalesInvoiceExp.Rows.Add(NewRow);

                    dgvSalesInvoiceExp.Rows[j].Cells["ID"].Value = item2["ID"].ToString();
                    dgvSalesInvoiceExp.Rows[j].Cells["LCDate"].Value = item2["LCDate"].ToString();
                    dgvSalesInvoiceExp.Rows[j].Cells["LCBank"].Value = item2["LCBank"].ToString();
                    dgvSalesInvoiceExp.Rows[j].Cells["PINo"].Value = item2["PINo"].ToString();
                    dgvSalesInvoiceExp.Rows[j].Cells["PIDate"].Value = item2["PIDate"].ToString();
                    dgvSalesInvoiceExp.Rows[j].Cells["EXPNo"].Value = item2["EXPNo"].ToString();
                    dgvSalesInvoiceExp.Rows[j].Cells["EXPDate"].Value = item2["EXPDate"].ToString();
                    dgvSalesInvoiceExp.Rows[j].Cells["PortFrom"].Value = item2["PortFrom"].ToString();
                    dgvSalesInvoiceExp.Rows[j].Cells["PortTo"].Value = item2["PortTo"].ToString();
                    dgvSalesInvoiceExp.Rows[j].Cells["LCNumber"].Value = item2["LCNumber"].ToString();
                    dgvSalesInvoiceExp.Rows[j].Cells["Remarks"].Value = item2["Remarks"].ToString();
            
           
                    j = j + 1;
                }
                LRecordCount.Text = "Record Count: " + dgvSalesInvoiceExp.Rows.Count.ToString();
                this.btnSearch.Visible = true;
                this.progressBar1.Visible = false;
                
            }
            #endregion
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion
        }
        private void GridSelected()
        {
            try
            {
                if (dgvSalesInvoiceExp.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (dgvSalesInvoiceExp.Rows.Count > 0)
                {
                    string SalesInvoiceExpInfo = string.Empty;
                    int ColIndex = dgvSalesInvoiceExp.CurrentCell.ColumnIndex;
                    int RowIndex1 = dgvSalesInvoiceExp.CurrentCell.RowIndex;
                    if (RowIndex1 >= 0)
                    {
                        //if (Program.fromOpen != "Me" &&
                        //    dgvBankInformation.Rows[RowIndex1].Cells["ActiveStatus"].Value.ToString() != "Y")
                        //{
                        //    MessageBox.Show("This Selected Item is not Active");
                        //    return;
                        //}
                        string ID = dgvSalesInvoiceExp.Rows[RowIndex1].Cells["ID"].Value.ToString();
                        string LCDate = dgvSalesInvoiceExp.Rows[RowIndex1].Cells["LCDate"].Value.ToString();
                        string LCBank = dgvSalesInvoiceExp.Rows[RowIndex1].Cells["LCBank"].Value.ToString();
                        string PINo = dgvSalesInvoiceExp.Rows[RowIndex1].Cells["PINo"].Value.ToString();
                        string PIDate = dgvSalesInvoiceExp.Rows[RowIndex1].Cells["PIDate"].Value.ToString();
                        string EXPNo = dgvSalesInvoiceExp.Rows[RowIndex1].Cells["EXPNo"].Value.ToString();
                        string EXPDate = dgvSalesInvoiceExp.Rows[RowIndex1].Cells["EXPDate"].Value.ToString();
                        string PortFrom = dgvSalesInvoiceExp.Rows[RowIndex1].Cells["PortFrom"].Value.ToString();
                        string PortTo = dgvSalesInvoiceExp.Rows[RowIndex1].Cells["PortTo"].Value.ToString();
                        string LCNumber = dgvSalesInvoiceExp.Rows[RowIndex1].Cells["LCNumber"].Value.ToString();
                        string Remarks = dgvSalesInvoiceExp.Rows[RowIndex1].Cells["Remarks"].Value.ToString();

                        SalesInvoiceExpInfo =
                            ID + FieldDelimeter +
                            LCDate + FieldDelimeter +
                            LCBank + FieldDelimeter +
                            PINo + FieldDelimeter +
                            PIDate + FieldDelimeter +
                            EXPNo + FieldDelimeter +
                            EXPDate + FieldDelimeter +
                            PortFrom + FieldDelimeter +
                            PortTo + FieldDelimeter +
                            LCNumber + FieldDelimeter +
                            Remarks + FieldDelimeter;


                        SelectedValue = SalesInvoiceExpInfo;

                    }
                } this.Close();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "GridSelected", exMessage);
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
                FileLogger.Log(this.Name, "GridSelected", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "GridSelected", exMessage);
            }
            #endregion Catch


        }

        private void dgvHSCode_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FormHSCodeSearch_Load(object sender, EventArgs e)
        {

        }
    }
}
