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
    public partial class FormCPCDetailsSearch : Form
    {
        public FormCPCDetailsSearch()
        {
            InitializeComponent();
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }

        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private DataTable HSCodeResult;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private string SelectedValue = string.Empty;
        private DataTable YearResult;
        private int YearLines;
        private string fiscalyear;

        #region RecordCount
        private string RecordCount = "0";
        
        #endregion
        #endregion

        public static string SelectOne()
        {
            string SearchValue = string.Empty;
            try
            {
                FormCPCDetailsSearch frmHSCodeSearch = new FormCPCDetailsSearch();
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

            RecordCount = cmbRecordCount.Text.Trim();

            bgwSearch.RunWorkerAsync();
        }

        private void bgwSearch_DoWork(object sender, DoWorkEventArgs e)
        {

            #region try
            try
            {
                //HSCodeDAL HDAL = new HSCodeDAL();
                ICPCDetails Hdal = OrdinaryVATDesktop.GetObject<CPCDetailsDAL, CPCDetailsRepo, ICPCDetails>(OrdinaryVATDesktop.IsWCF);

                string[] cFields = { "Code like", "Name like", "SelectTop" };
                string[] cValues = { OrdinaryVATDesktop.SanitizeInput(txtCode.Text.Trim()), OrdinaryVATDesktop.SanitizeInput(txtName.Text.Trim()) , RecordCount };
                HSCodeResult = Hdal.SelectAll(0, cFields, cValues, null, null, true, connVM);

                string[] columnNames = { "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn" };

                HSCodeResult = OrdinaryVATDesktop.DtDeleteColumns(HSCodeResult, columnNames);

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
            #region TotalRecordCount
            string TotalRecordCount = "0";
            #endregion

            #region try
            try
            {

                dgvHSCode.DataSource = null;
                if (HSCodeResult != null && HSCodeResult.Rows.Count > 0)
                {

                    TotalRecordCount = HSCodeResult.Rows[HSCodeResult.Rows.Count - 1][0].ToString();

                    HSCodeResult.Rows.RemoveAt(HSCodeResult.Rows.Count - 1);

                    dgvHSCode.DataSource = HSCodeResult;
                    dgvHSCode.Columns["Id"].Visible = false;

                }

                #region Old
                //int j = 0;
                //dgvHSCode.Rows.Clear();
                //foreach (DataRow item2 in HSCodeResult.Rows)
                //{

                //    DataGridViewRow NewRow = new DataGridViewRow();

                //    dgvHSCode.Rows.Add(NewRow);

                //    dgvHSCode.Rows[j].Cells["Id"].Value = item2["Id"].ToString();
                //    dgvHSCode.Rows[j].Cells["Code"].Value = item2["Code"].ToString();
                //    dgvHSCode.Rows[j].Cells["HSCode"].Value = item2["HSCode"].ToString();
                //    dgvHSCode.Rows[j].Cells["Description"].Value = item2["Description"].ToString();
                //    dgvHSCode.Rows[j].Cells["CD"].Value = item2["CD"].ToString();
                //    dgvHSCode.Rows[j].Cells["SD"].Value = item2["SD"].ToString();
                //    dgvHSCode.Rows[j].Cells["VAT"].Value = item2["VAT"].ToString();
                //    dgvHSCode.Rows[j].Cells["AIT"].Value = item2["AIT"].ToString();
                //    dgvHSCode.Rows[j].Cells["RD"].Value = item2["RD"].ToString();
                //    dgvHSCode.Rows[j].Cells["AT"].Value = item2["AT"].ToString();
                //    dgvHSCode.Rows[j].Cells["OtherSD"].Value = item2["OtherSD"].ToString();
                //    dgvHSCode.Rows[j].Cells["OtherVAT"].Value = item2["OtherVAT"].ToString();

                //    dgvHSCode.Rows[j].Cells["IsFixedOtherVAT"].Value = item2["IsFixedOtherVAT"].ToString();
                //    dgvHSCode.Rows[j].Cells["IsFixedOtherSD"].Value = item2["IsFixedOtherSD"].ToString();

                //    dgvHSCode.Rows[j].Cells["IsFixedCD"].Value = item2["IsFixedCD"].ToString();
                //    dgvHSCode.Rows[j].Cells["IsFixedRD"].Value = item2["IsFixedRD"].ToString();
                //    dgvHSCode.Rows[j].Cells["IsFixedSD"].Value = item2["IsFixedSD"].ToString();
                //    dgvHSCode.Rows[j].Cells["IsFixedVAT"].Value = item2["IsFixedVAT"].ToString();
                //    dgvHSCode.Rows[j].Cells["IsFixedAT"].Value = item2["IsFixedAT"].ToString();
                //    dgvHSCode.Rows[j].Cells["IsFixedAIT"].Value = item2["IsFixedAIT"].ToString();

                //    //dgvHSCode.Rows[j].Cells["OtherVAT"].Value = item2["OtherVAT"].ToString();
                //    //dgvHSCode.Rows[j].Cells["OtherCD"].Value = item2["OtherCD"].ToString();
                //    dgvHSCode.Rows[j].Cells["Comments"].Value = item2["Comments"].ToString();
            
           
                //    j = j + 1;
                //}
                #endregion

                //LRecordCount.Text = "Record Count: " + dgvHSCode.Rows.Count.ToString();
                this.btnSearch.Visible = true;
                this.progressBar1.Visible = false;
                LRecordCount.Text = "Total Record Count " + (HSCodeResult.Rows.Count) + " of " + TotalRecordCount.ToString();

                
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
                if (dgvHSCode.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (dgvHSCode.Rows.Count > 0)
                {
                    string CPCDetailsInfo = string.Empty;
                    int ColIndex = dgvHSCode.CurrentCell.ColumnIndex;
                    int RowIndex1 = dgvHSCode.CurrentCell.RowIndex;
                    if (RowIndex1 >= 0)
                    {
                        //if (Program.fromOpen != "Me" &&
                        //    dgvBankInformation.Rows[RowIndex1].Cells["ActiveStatus"].Value.ToString() != "Y")
                        //{
                        //    MessageBox.Show("This Selected Item is not Active");
                        //    return;
                        //}
                        string Id = dgvHSCode.Rows[RowIndex1].Cells["Id"].Value.ToString();
                        string Code = dgvHSCode.Rows[RowIndex1].Cells["Code"].Value.ToString();

                        CPCDetailsInfo =
                            Id + FieldDelimeter +
                            Code + FieldDelimeter;


                        SelectedValue = CPCDetailsInfo;

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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnHSCodeExport_Click(object sender, EventArgs e)
        {

            try
            {

               var HSCode = new List<string>();

                var len = dgvHSCode.RowCount;
                for (int i = 0; i < len; i++)
                {
                    HSCode.Add(dgvHSCode["HSCode", i].Value.ToString());
                }


                if (HSCode.Count == 0)
                {
                    MessageBox.Show("No Data Found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //var HSCodeDal = new HSCodeDAL();
                IHSCode HSCodeDal = OrdinaryVATDesktop.GetObject<HSCodeDAL, HSCodeRepo, IHSCode>(OrdinaryVATDesktop.IsWCF);

                DataTable dt = HSCodeDal.GetExcelData(HSCode,null,null,connVM);

                OrdinaryVATDesktop.SaveExcel(dt, "HSCode", "HSCode");
              

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }



        }

        private void FormHSCodeSearch_Load(object sender, EventArgs e)
        {
            #region RecordCount
            cmbRecordCount.DataSource = new RecordSelect().RecordSelectList;
            #endregion
        }

       
    }
}
