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
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;
using VATServer.Library;
using VATServer.License;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using VATServer.Interface;
using VATDesktop.Repo;
using SymphonySofttech.Reports.Report.V12V2;

namespace VATClient
{
    public partial class FormToll6_3Invoice : Form
    {
        public FormToll6_3Invoice()
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
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;
        private bool IsPost = false;
        private bool ChangeData = false;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        //private Toll6_3InvoiceDAL _dal = new Toll6_3InvoiceDAL();
        IToll6_3Invoice _dal = OrdinaryVATDesktop.GetObject<Toll6_3InvoiceDAL, Toll6_3InvoiceRepo, IToll6_3Invoice>(OrdinaryVATDesktop.IsWCF);
        private string transactionType = string.Empty;

        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {

                SearchData();
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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }

            #endregion
        }

        private void SearchData()
        {

            try
            {
                var transactionType = "";

                string defaultCustomerName = "";
                string defaultAddress = "";
                string defaultCustomerId = "";

                #region Contractor 63

                if (rbtnContractor63.Checked)
                {
                    transactionType = "TollFinishIssue";

                    FormSaleVAT20Multiple frm = new FormSaleVAT20Multiple();
                    string invoices = "";

                    invoices = FormSaleVAT20Multiple.SelectOne(transactionType);

                    invoices = invoices.Trim();
                    invoices = invoices.Trim('^');

                    dgvSale.Rows.Clear();

                    if (!string.IsNullOrWhiteSpace(invoices))
                    {

                        int invoiceLength = 0;

                        invoiceLength = invoices.Split('^').Length;
                        string[] invoiceArray = invoices.Split('^');

                        for (int i = 0; i < invoiceLength; i++)
                        {
                            string Items = invoiceArray[i];

                            if (!string.IsNullOrWhiteSpace(Items.Trim()))
                            {
                                string[] ItemArray = Items.ToString().Trim().Split('~');

                                if (i == 0)
                                {
                                    defaultCustomerName = ItemArray[2];
                                    defaultCustomerId = ItemArray[3];

                                    CustomerVM vCustomerVM = new CustomerVM();
                                    ICustomer Customerdal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);

                                    vCustomerVM = Customerdal.SelectAllList(defaultCustomerId, null, null, null, null, connVM).FirstOrDefault();

                                    defaultAddress = vCustomerVM.Address1;//// ItemArray[4];
                                }
                                else
                                {
                                    if (defaultCustomerName != ItemArray[2])
                                    {
                                        dgvSale.Rows.Clear();

                                        MessageBox.Show("Please Select Same Customer", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                        SearchData();

                                        break;
                                    }
                                }

                                DataGridViewRow NewRow = new DataGridViewRow();
                                dgvSale.Rows.Add(NewRow);

                                dgvSale["TollLineNo", dgvSale.RowCount - 1].Value = (i + 1);

                                dgvSale["SalesInvoiceNo", dgvSale.RowCount - 1].Value = ItemArray[0];
                                dgvSale["InvoiceDateTime", dgvSale.RowCount - 1].Value = ItemArray[1];
                            }
                        }


                        txtTollNo.Text = "";
                        txtCustomer.Text = defaultCustomerName;
                        txtCustomerID.Text = defaultCustomerId;
                        txtCustomerAddress.Text = defaultAddress;
                        dtpTollDate.Text = DateTime.Now.ToString();
                        txtPost.Text = "N";
                    }

                    btnSave.Enabled = true;
                    btnUpdate.Enabled = true;
                    btnPost.Enabled = true;


                }

                #endregion

                #region Client 63

                else if (rbtnClient63.Checked)
                {
                    transactionType = "ClientFGReceiveWOBOM";
                    ////transactionType = "Other";

                    DataTable dtRow = FormPurchaseSearch.SelectMultiple(transactionType, false, true);

                    if (dtRow != null && dtRow.Rows.Count > 0)
                    {

                        int LineNo = 0;
                        foreach (DataRow dr in dtRow.Rows)
                        {

                            if (LineNo == 0)
                            {
                                defaultCustomerName = dr["VendorName"].ToString();
                                defaultCustomerId = dr["VendorID"].ToString();

                                VendorVM vVendorVM = new VendorVM();
                                VendorDAL VendorDAL = new VendorDAL();

                                vVendorVM = VendorDAL.SelectAllList(Convert.ToInt32(defaultCustomerId), null, null,null,null,connVM).FirstOrDefault();

                                defaultAddress = vVendorVM.Address1;//// ItemArray[4];
                            }
                            else
                            {
                                if (defaultCustomerName != dr["VendorName"].ToString())
                                {
                                    dgvSale.Rows.Clear();

                                    MessageBox.Show("Please Select Same Vendor", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                    SearchData();

                                    break;
                                }
                            }

                            DataGridViewRow NewRow = new DataGridViewRow();
                            dgvSale.Rows.Add(NewRow);

                            dgvSale["TollLineNo", dgvSale.RowCount - 1].Value = (LineNo + 1);

                            dgvSale["SalesInvoiceNo", dgvSale.RowCount - 1].Value = dr["PurchaseInvoiceNo"].ToString();
                            dgvSale["InvoiceDateTime", dgvSale.RowCount - 1].Value = dr["InvoiceDateTime"].ToString();


                            LineNo = LineNo + 1;
                        }

                        txtTollNo.Text = "";
                        txtCustomer.Text = defaultCustomerName;
                        txtCustomerID.Text = defaultCustomerId;
                        txtCustomerAddress.Text = defaultAddress;
                        dtpTollDate.Text = DateTime.Now.ToString();
                        txtPost.Text = "N";


                        btnSave.Enabled = true;
                        btnUpdate.Enabled = true;
                        btnPost.Enabled = true;
                    }

                }

                #endregion

                Rowcalculate();

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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchData", exMessage);
            }

            #endregion


        }

        private void Rowcalculate()
        {

            for (int i = 0; i < dgvSale.RowCount; i++)
            {
                dgvSale[0, i].Value = i + 1;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvSale.RowCount > 0)
            {
                if (MessageBox.Show(MessageVM.msgWantToRemoveRow, this.Text, MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dgvSale.Rows.RemoveAt(dgvSale.CurrentRow.Index);
                }
                Rowcalculate();
            }
        }

        private Toll6_3InvoiceVM masterVM = new Toll6_3InvoiceVM();
        private List<Toll6_3InvoiceDetailVM> detailVMs = new List<Toll6_3InvoiceDetailVM>();

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                #region Transaction Type

                transactionTypes();

                #endregion

                #region Data Load

                DataLoad();

                masterVM.CreatedBy = Program.CurrentUser;
                masterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                #endregion

                #region Find UserInfo

                DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + Program.CurrentUserID + "'");
                masterVM.SignatoryName = userInfo[0]["FullName"].ToString();
                masterVM.SignatoryDesig = userInfo[0]["Designation"].ToString();

                #endregion

                #region Insert Data

                sqlResults = _dal.Insert(masterVM, detailVMs, null, null, connVM);

                #endregion

                #region Results

                if (sqlResults[0] == "Success")
                {
                    txtTollNo.Text = sqlResults[2].ToString();

                    btnSave.Enabled = false;
                }

                #endregion

                #region Message

                MessageBox.Show(sqlResults[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
            }

            #endregion

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {

                #region Transaction Type

                transactionTypes();

                #endregion

                #region Data Load

                DataLoad();

                masterVM.LastModifiedBy = Program.CurrentUser;
                masterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                #endregion

                #region Find UserInfo

                DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + Program.CurrentUserID + "'");
                masterVM.SignatoryName = userInfo[0]["FullName"].ToString();
                masterVM.SignatoryDesig = userInfo[0]["Designation"].ToString();

                #endregion

                #region Update

                sqlResults = _dal.Update(masterVM, detailVMs, connVM);

                #endregion

                #region Message

                MessageBox.Show(sqlResults[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }

            #endregion

        }

        private void DataLoad()
        {
            try
            {

                #region Master

                masterVM = new Toll6_3InvoiceVM();
                masterVM.TollNo = txtTollNo.Text.Trim();

                if (transactionType == "Client63")
                {
                    masterVM.VendorID = Convert.ToInt32(txtCustomerID.Text.Trim());
                }
                else
                {
                    masterVM.CustomerID = txtCustomerID.Text.Trim();
                }

                if (string.IsNullOrWhiteSpace(masterVM.CustomerID))
                {
                    masterVM.CustomerID = "0";
                }
                masterVM.Address = txtCustomerAddress.Text.Trim();
                masterVM.TollDateTime = dtpTollDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                masterVM.BranchId = Program.BranchId;
                masterVM.Comments = txtComments.Text.Trim();
                masterVM.TransactionType = transactionType;

                #endregion

                #region Details

                ////int dgvRowCount =  0;
                ////dgvRowCount = dgvSale.RowCount;

                detailVMs = new List<Toll6_3InvoiceDetailVM>();
                foreach (DataGridViewRow dr in dgvSale.Rows)
                {
                    Toll6_3InvoiceDetailVM detailVM = new Toll6_3InvoiceDetailVM();

                    detailVM.TollNo = txtTollNo.Text.Trim(); ;
                    detailVM.TollLineNo = dr.Cells["TollLineNo"].Value.ToString();
                    detailVM.SalesInvoiceNo = dr.Cells["SalesInvoiceNo"].Value.ToString();
                    detailVM.InvoiceDateTime = dr.Cells["InvoiceDateTime"].Value.ToString();
                    detailVM.BranchId = Program.BranchId;
                    detailVM.TransactionType = transactionType;

                    //////detailVM.Comments = dr.Cells["Comments"].Value.ToString();

                    detailVMs.Add(detailVM);

                }

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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "DataLoad", exMessage);
            }

            #endregion


        }

        private void FormToll6_3Invoice_Load(object sender, EventArgs e)
        {

            #region Form Maker

            FormMaker();

            #endregion

            #region transaction Types

            transactionTypes();

            #endregion

        }
        private void transactionTypes()
        {
            #region try

            try
            {

                #region Transaction Type

                if (rbtnContractor63.Checked)
                {
                    transactionType = "Contractor63";
                }
                else if (rbtnClient63.Checked)
                {
                    transactionType = "Client63";
                }

                #endregion Transaction Type

            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormMaker", exMessage);
            }

            #endregion

        }

        private void FormMaker()
        {
            try
            {

                #region TransactionType

                #region Other


                if (rbtnClient63.Checked) //start
                {
                    label37.Text = "Vendor";
                    this.Text = "Toll 6.3 Receive";

                }
                #endregion Other

                #endregion Transaction Type

            }
            #region catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormMaker", exMessage);
            }
            #endregion

        }


        private void btnTollNo_Click(object sender, EventArgs e)
        {
            try
            {
                //var transactionType = "TollFinishIssue";

                #region Transaction Type

                transactionTypes();

                #endregion

                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormToll6_3InvoiceSearch.SelectOne(transactionType);

                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    #region Master

                    Toll6_3InvoiceVM mVM = new Toll6_3InvoiceVM();

                    mVM = _dal.SelectAllList(Convert.ToInt32(selectedRow.Cells["Id"].Value), null, null, null, null, connVM, transactionType).FirstOrDefault();

                    txtTollNo.Text = mVM.TollNo;
                    if (transactionType == "Client63")
                    {
                        txtCustomerID.Text = mVM.VendorID.ToString();
                        txtCustomer.Text = mVM.VendorName;

                    }
                    else
                    {
                        txtCustomerID.Text = mVM.CustomerID;
                        txtCustomer.Text = mVM.CustomerName;

                    }
                    
                    txtCustomerAddress.Text = mVM.Address;
                    dtpTollDate.Text = mVM.TollDateTime;
                    txtPost.Text = mVM.Post;
                    txtComments.Text = mVM.Comments;

                    #endregion

                    #region Detail

                    List<Toll6_3InvoiceDetailVM> dVMs = new List<Toll6_3InvoiceDetailVM>();

                    dVMs = _dal.SelectDetail(mVM.TollNo,null,null,null,null,connVM);

                    dgvSale.Rows.Clear();
                    int j = 0;

                    foreach (Toll6_3InvoiceDetailVM detailVM in dVMs)
                    {
                        DataGridViewRow NewRow = new DataGridViewRow();
                        dgvSale.Rows.Add(NewRow);

                        dgvSale.Rows[j].Cells["TollLineNo"].Value = detailVM.TollLineNo;
                        dgvSale.Rows[j].Cells["SalesInvoiceNo"].Value = detailVM.SalesInvoiceNo;
                        dgvSale.Rows[j].Cells["InvoiceDateTime"].Value = detailVM.InvoiceDateTime;
                        
                        j++;
                    }
                    
                    #endregion
                }
                btnSave.Enabled = false;

                if (txtPost.Text == "Y")
                {
                    btnUpdate.Enabled = false;
                    btnPost.Enabled = false;

                }
                else
                {
                    btnUpdate.Enabled = true;
                    btnPost.Enabled = true;
                }

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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "DataLoad", exMessage);
            }

            #endregion
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            try
            {

                ClearAllFields();

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ClearAllFields()
        {


            btnSave.Enabled = true;
            btnUpdate.Enabled = true;
            btnPost.Enabled = true;

            txtTollNo.Text = "";
            txtCustomerID.Text = "";
            txtCustomerAddress.Text = DateTime.Now.ToString();
            dtpTollDate.Text = "";
            txtPost.Text = "";
            txtComments.Text = "";
            txtCustomer.Text = "";
            txtCustomerAddress.Text = "";

            dgvSale.Rows.Clear();



        }

        private void btnPost_Click(object sender, EventArgs e)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(txtTollNo.Text.Trim()))
                {
                    MessageBox.Show("Please Save First", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


                masterVM = new Toll6_3InvoiceVM();
                masterVM.TollNo = txtTollNo.Text.Trim();
                #region Find UserInfo

                DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + Program.CurrentUserID + "'");
                masterVM.SignatoryName = userInfo[0]["FullName"].ToString();
                masterVM.SignatoryDesig = userInfo[0]["Designation"].ToString();
                #endregion
                sqlResults = _dal.Post(masterVM, null, null, connVM);

                if (sqlResults[0].ToLower() == "success")
                    _dal.UpdateTollCompleted("Y", masterVM.TollNo,null,null,connVM);


                if (sqlResults[0] != "Fail")
                {
                    btnUpdate.Enabled = false;
                    btnPost.Enabled = false;
                }

                MessageBox.Show(sqlResults[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);


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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
            }

            #endregion


        }

        private void btnToll6_3_Click(object sender, EventArgs e)
        {
            try
            {

                ShowReport();
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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnToll6_3_Click", exMessage);
            }

            #endregion
        }

        private void ShowReport()
        {

            try
            {
                DataTable settingsDt = new DataTable();
                string vVAT2012V2 = "2020-Jul-01";

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null,connVM);
                }
                CommonDAL _cDal = new CommonDAL();
                vVAT2012V2 = _cDal.settingsDesktop("Version", "VAT2012V2", settingsDt,connVM);
                string TollDate = "";
                string SignatoryName = "";
                string SignatoryDesig = "";
                DateTime invoiceDateTime = DateTime.Now;
                DataSet ds = new DataSet();
                IReport Reportdal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

                ds = Reportdal.VAT6_3Toll(txtTollNo.Text.Trim(), "", "", connVM);
                invoiceDateTime = Convert.ToDateTime(ds.Tables[2].Rows[0]["TollDate"]);


                ds.Tables[0].TableName = "DsVAT11";
                TollDate = Convert.ToString(ds.Tables[2].Rows[0]["TollDate"]);
                SignatoryName = Convert.ToString(ds.Tables[2].Rows[0]["SignatoryName"]);
                SignatoryDesig = Convert.ToString(ds.Tables[2].Rows[0]["SignatoryDesig"]);



                DateTime VAT2012V2 = Convert.ToDateTime(vVAT2012V2);
                ReportDocument objrpt = new ReportDocument();

                //objrpt = new RptVAT_Toll6_3();
                if (VAT2012V2 <= invoiceDateTime)
                {
                    ////New Report -- 
                    objrpt = new RptVAT_Toll6_3_V12V2();

                }
                else
                {
                    objrpt = new RptVAT_Toll6_3();

                }
                objrpt.SetDataSource(ds);


                #region Formula Fields

                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT Toll 6.3'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                #endregion

                #region CheckPoint - Formula Fields Existence


                string Quantity6_3 = _cDal.settingsDesktop("DecimalPlace", "Quantity6_3", settingsDt,connVM);
                string Amount6_3 = _cDal.settingsDesktop("DecimalPlace", "Amount6_3", settingsDt,connVM);
                string VATRate6_3 = _cDal.settingsDesktop("DecimalPlace", "VATRate6_3", settingsDt,connVM);
                string FontSize = _cDal.settingsDesktop("FontSize", "VAT6_3", settingsDt,connVM);

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;

                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_3", Quantity6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_3", Amount6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TollDate", TollDate);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SignatoryName", SignatoryName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SignatoryDesig", SignatoryDesig);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VATRate6_3", VATRate6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", FontSize);

                #endregion

                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();

                reports.setReportSource(objrpt);
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();


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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ShowReport", exMessage);
            }

            #endregion

        }


    }
}
