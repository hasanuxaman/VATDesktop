using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATClient.ModelDTO;
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormBankChannelPayment : Form
    {

        #region variable
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        BankChannelPaymentVM MasterVM = new BankChannelPaymentVM();
        private string NextID = string.Empty;
        private bool IsUpdate = false;

        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;

        private string[] sqlResults;

        private bool ChangeData = false;
        private bool IsPost = false;
        private bool IsPostAll = false;

        DataTable dt = new DataTable();

        List<BankChannelPaymentVM> BankVMList = new List<BankChannelPaymentVM>();


        #endregion variable

        public FormBankChannelPayment()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        public void SetGridViewData(DataTable BankChannelDT)
        {
            #region try
            
            try
            {
                #region Statement

                ////// Start Complete
                dgvBankChannel.Rows.Clear();
                int j = 0;
                foreach (DataRow item in BankChannelDT.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvBankChannel.Rows.Add(NewRow);

                    string INVNo = item["PurchaseInvoiceNo"].ToString();

                    #region Select

                    BankChannelPaymentDAL _dal = new BankChannelPaymentDAL();

                    string[] cValues = new string[] { INVNo };
                    string[] cFields = new string[] { "bcp.PurchaseInvoiceNo" };

                    dt = _dal.SelectAll(null, cFields, cValues,null,null,null,connVM);

                    #endregion

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow vitem in dt.Rows)
                        {

                            dgvBankChannel.Rows[j].Cells["Id"].Value = vitem["Id"].ToString();
                            dgvBankChannel.Rows[j].Cells["PurchaseInvoiceNo"].Value = vitem["PurchaseInvoiceNo"].ToString();
                            dgvBankChannel.Rows[j].Cells["PaymentAmount"].Value = vitem["PaymentAmount"].ToString();
                            dgvBankChannel.Rows[j].Cells["VATAmount"].Value = vitem["VATAmount"].ToString();
                            dgvBankChannel.Rows[j].Cells["Remarks"].Value = vitem["Remarks"].ToString();
                            dgvBankChannel.Rows[j].Cells["BankID"].Value = vitem["BankID"].ToString();
                            dgvBankChannel.Rows[j].Cells["BankName"].Value = vitem["BankName"].ToString();
                            dgvBankChannel.Rows[j].Cells["Post"].Value = vitem["Post"].ToString();
                            dgvBankChannel.Rows[j].Cells["PaymentDate"].Value = Convert.ToDateTime(vitem["PaymentDate"].ToString());
                            dgvBankChannel.Rows[j].Cells["PaymentType"].Value = vitem["PaymentType"].ToString();

                        }
                    }
                    else
                    {
                        dgvBankChannel.Rows[j].Cells["PurchaseInvoiceNo"].Value = INVNo;
                        dgvBankChannel.Rows[j].Cells["PaymentAmount"].Value = item["TotalAmount"].ToString();
                        dgvBankChannel.Rows[j].Cells["VATAmount"].Value = item["TotalVATAmount"].ToString();

                    }

                    j = j + 1;

                }

                #endregion
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
                FileLogger.Log(this.Name, "SetGridViewData", exMessage);
            }

            #endregion

        }

        private void txtBankName_DoubleClick(object sender, EventArgs e)
        {
            BankLoad();
        }

        private void BankLoad()
        {
            DataGridViewRow selectedRow = new DataGridViewRow();
            string SqlText = @" select BankID,BankName,BranchName,AccountNumber,Address1,City from BankInformations
            where 1=1 and ActiveStatus='Y' ";

            string SQLTextRecordCount = @" select count(BankID)RecordNo from BankInformations";
            string[] shortColumnName = { "BankID", "BankName", "BranchName", "AccountNumber", "Address1", "City" };
            string tableName = "";
            selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
            if (selectedRow != null && selectedRow.Selected == true)
            {
                txtBankID.Text = selectedRow.Cells["BankID"].Value.ToString();
                txtBankName.Text = selectedRow.Cells["BankName"].Value.ToString();
                ////txtBranchName.Text = selectedRow.Cells["BranchName"].Value.ToString();
                ////txtAccountNumber.Text = selectedRow.Cells["AccountNumber"].Value.ToString();
                ////txtDistrict.Text = selectedRow.Cells["City"].Value.ToString();

            }
        }

        private void txtBankName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                BankLoad();
            }
        }

        private void SaveBankChannelData()
        {

            #region variable
            FiscalYearVM varFiscalYearVM = new FiscalYearVM();
            string vDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

            #endregion

            #region try

            try
            {
                #region Null check

                ////if (string.IsNullOrWhiteSpace(txtRemarks.Text))
                ////{
                ////    txtRemarks.Text = "-";
                ////}

                ////if (string.IsNullOrWhiteSpace(txtPurchaseInvoiceNo.Text))
                ////{
                ////    MessageBox.Show(MessageVM.BankChannelmsgPleaseSelectPurchaseInvoiceNo);
                ////    return;
                ////}
                ////if (string.IsNullOrWhiteSpace(txtPaymentAmount.Text))
                ////{
                ////    MessageBox.Show(MessageVM.BankChannelmsgPleaseEnterPaymentAmount);
                ////    return;
                ////}

                ////if (string.IsNullOrWhiteSpace(txtVATAmount.Text))
                ////{
                ////    MessageBox.Show(MessageVM.BankChannelmsgPleaseEnterVATAmount);
                ////    return;
                ////}

                ////if (string.IsNullOrWhiteSpace(txtBankID.Text))
                ////{
                ////    MessageBox.Show(MessageVM.BankChannelmsgPleaseEnterBankInformation);
                ////    return;
                ////}

                #endregion null

                #region Check Point

                ////NextID = txtPurchaseInvoiceNo.Text.Trim();

                #region Exist Check

                ////BankChannelPaymentVM vm = new BankChannelPaymentVM();
                ////if (!string.IsNullOrWhiteSpace(NextID))
                ////{
                ////    vm = new BankChannelPaymentDAL().SelectAllList(0, new[] { "bcp.PurchaseInvoiceNo" }, new[] { NextID }).FirstOrDefault();
                ////}

                ////if (vm != null && !string.IsNullOrWhiteSpace(vm.Id))
                ////{
                ////    throw new Exception("This Invoice Already Exist! Cannot Add!" + Environment.NewLine + "Invoice No: " + NextID);

                ////}

                #endregion

                #endregion

                #region Master

                ////MasterVM = new BankChannelPaymentVM();
                ////MasterVM.PurchaseInvoiceNo = NextID;
                ////MasterVM.BankID = txtBankID.Text.Trim();
                ////MasterVM.PaymentDate = dtpPaymentDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                ////MasterVM.PaymentAmount = Convert.ToDecimal(txtPaymentAmount.Text.Trim());
                ////MasterVM.VATAmount = Convert.ToDecimal(txtVATAmount.Text.Trim());
                ////MasterVM.Remarks = txtRemarks.Text.Trim().Replace(" ", "");
                ////MasterVM.CreatedBy = Program.CurrentUser;
                ////MasterVM.CreatedOn = vDateTime;
                ////MasterVM.LastModifiedBy = Program.CurrentUser;
                ////MasterVM.LastModifiedOn = vDateTime;
                ////MasterVM.Post = "N";

                #endregion Master

                #region dgvBankChannel

                BankVMList = new List<BankChannelPaymentVM>();

                for (int i = 0; i < dgvBankChannel.RowCount; i++)
                {
                    BankChannelPaymentVM BankVm = new BankChannelPaymentVM();

                    BankVm.Id = dgvBankChannel.Rows[i].Cells["Id"].Value == null ? "" : dgvBankChannel.Rows[i].Cells["Id"].Value.ToString();
                    BankVm.PurchaseInvoiceNo = dgvBankChannel.Rows[i].Cells["PurchaseInvoiceNo"].Value.ToString();

                    BankVm.BankID = dgvBankChannel.Rows[i].Cells["BankID"].Value == null ? "" : dgvBankChannel.Rows[i].Cells["BankID"].Value.ToString();
                    
                    if (string.IsNullOrWhiteSpace(BankVm.BankID))
                    {
                        MessageBox.Show(MessageVM.BankChannelmsgPleaseEnterBankInformation + " on " + BankVm.PurchaseInvoiceNo,this.Text);
                        return;
                    }
                    
                    BankVm.BankName = dgvBankChannel.Rows[i].Cells["BankName"].Value.ToString();
                    BankVm.PaymentDate = Convert.ToDateTime(dgvBankChannel.Rows[i].Cells["PaymentDate"].Value).ToString("yyyy-MMM-dd HH:mm:ss");

                    #region Find Fiscal Year Lock

                    string PeriodName = Convert.ToDateTime(BankVm.PaymentDate).ToString("MMMM-yyyy");
                    string[] cValues = { PeriodName };
                    string[] cFields = { "PeriodName" };
                    varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues,null,null,connVM).FirstOrDefault();

                    if (varFiscalYearVM == null)
                    {
                        throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");
                    }

                    if (varFiscalYearVM.VATReturnPost == "Y")
                    {
                        throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!" + "\n" + "PurchaseInvoiceNo :" + BankVm.PurchaseInvoiceNo);
                    }

                    #endregion

                    BankVm.PaymentAmount = Convert.ToDecimal(dgvBankChannel.Rows[i].Cells["PaymentAmount"].Value.ToString());
                    BankVm.VATAmount = Convert.ToDecimal(dgvBankChannel.Rows[i].Cells["VATAmount"].Value.ToString());
                    BankVm.Remarks = dgvBankChannel.Rows[i].Cells["Remarks"].Value.ToString();
                    BankVm.PaymentType = dgvBankChannel.Rows[i].Cells["PaymentType"].Value.ToString();
                    BankVm.Post = "N";
                    BankVm.CreatedBy = Program.CurrentUser;
                    BankVm.CreatedOn = vDateTime;
                    BankVm.LastModifiedBy = Program.CurrentUser;
                    BankVm.LastModifiedOn = vDateTime;

                    BankVMList.Add(BankVm);

                }

                #endregion details

                #region Button Stats

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Background Worker

                bgwSave.RunWorkerAsync();

                #endregion

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
                FileLogger.Log(this.Name, "btnSave_Click", exMessage + "\n" + ex.StackTrace);
            }
            #endregion

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveBankChannelData();
        }

        private void bgwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                #region Statement

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                BankChannelPaymentDAL _dal = new BankChannelPaymentDAL();

                sqlResults = _dal.BankChannelInsertList(BankVMList,null,null,connVM);

                SAVE_DOWORK_SUCCESS = true;

                #endregion

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSave_DoWork", exMessage + "\n" + ex.StackTrace);
            }
            #endregion

        }

        private void bgwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                #region Start

                if (SAVE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //txtItemNo.Text = newId;
                            ChangeData = false;
                        }

                        if (result == "Success")
                        {
                            //////txtId.Text = sqlResults[4].ToString();

                            IsUpdate = true;

                            ButtonStats();

                            SelectInvoiceNo();
                        }
                    }
                ChangeData = false;

                #endregion

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
                FileLogger.Log(this.Name, "bgwSave_RunWorkerCompleted", exMessage + "\n" + ex.StackTrace);
            }
            #endregion

            #region finally

            finally
            {
                ChangeData = false;
                this.btnSave.Enabled = true;
                this.progressBar1.Visible = false;

            }
            #endregion

        }

        private void ButtonStats()
        {
            if (IsUpdate)
            {
                this.btnSave.Enabled = false;
            }
            else
            {
                this.btnSave.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }

        private void FormBankChannelPayment_Load(object sender, EventArgs e)
        {
            ////searchDT();
        }

        private void bthUpdate_Click(object sender, EventArgs e)
        {
            #region variable
            FiscalYearVM varFiscalYearVM = new FiscalYearVM();
            string vDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

            #endregion

            try
            {

                #region Null check

                if (string.IsNullOrWhiteSpace(txtRemarks.Text))
                {
                    txtRemarks.Text = "-";
                }

                if (string.IsNullOrWhiteSpace(txtPurchaseInvoiceNo.Text))
                {
                    MessageBox.Show(MessageVM.BankChannelmsgPleaseSelectPurchaseInvoiceNo);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtPaymentAmount.Text))
                {
                    MessageBox.Show(MessageVM.BankChannelmsgPleaseEnterPaymentAmount);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtVATAmount.Text))
                {
                    MessageBox.Show(MessageVM.BankChannelmsgPleaseEnterVATAmount);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtBankID.Text))
                {
                    MessageBox.Show(MessageVM.BankChannelmsgPleaseEnterBankInformation);
                    return;
                }

                #endregion null

                #region Master

                MasterVM = new BankChannelPaymentVM();
                MasterVM.Id = txtId.Text;
                MasterVM.PurchaseInvoiceNo = txtPurchaseInvoiceNo.Text;

                MasterVM.BankID = txtBankID.Text.Trim();
                MasterVM.PaymentDate = dtpPaymentDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                MasterVM.PaymentAmount = Convert.ToDecimal(txtPaymentAmount.Text.Trim());
                MasterVM.VATAmount = Convert.ToDecimal(txtVATAmount.Text.Trim());
                MasterVM.Remarks = txtRemarks.Text.Trim().Replace(" ", "");
                MasterVM.CreatedBy = Program.CurrentUser;
                MasterVM.CreatedOn = vDateTime;
                MasterVM.LastModifiedBy = Program.CurrentUser;
                MasterVM.LastModifiedOn = vDateTime;
                MasterVM.Post = "N";

                #endregion Master

                #region Background Worker - Update

                bgwUpdate.RunWorkerAsync();

                #endregion

                #region Button Stats

                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

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
                FileLogger.Log(this.Name, "bthUpdate_Click", exMessage);
            }
            #endregion

        }

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {

                #region Statement

                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                BankChannelPaymentDAL _dal = new BankChannelPaymentDAL();

                sqlResults = _dal.Update(MasterVM,null,null,connVM);
                UPDATE_DOWORK_SUCCESS = true;

                #endregion

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
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
            }
            #endregion

        }

        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {

                #region Start

                if (UPDATE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                        }

                    }

                #endregion

                ChangeData = false;

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
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion

            #region finally

            finally
            {
                ChangeData = false;
                this.btnUpdate.Enabled = true;
                this.progressBar1.Visible = false;

            }
            #endregion

        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            #region variable
            FiscalYearVM varFiscalYearVM = new FiscalYearVM();
            string vDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

            #endregion

            try
            {

                #region Check Point

                if (IsPostAll == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }

                string Message = MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1;
                DialogResult dlgRes = MessageBox.Show(Message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                if (dlgRes != DialogResult.Yes)
                {
                    return;
                }
                ////else if (IsUpdate == false)
                ////{
                ////    MessageBox.Show(MessageVM.msgNotPost, this.Text);
                ////    return;
                ////}

                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtPurchaseInvoiceNo.Text.Trim();
                }

                #endregion

                #region Master

                ////MasterVM = new BankChannelPaymentVM();
                ////MasterVM.Id = txtId.Text;
                ////MasterVM.PurchaseInvoiceNo = NextID;
                ////MasterVM.BankID = txtBankID.Text.Trim();
                ////MasterVM.PaymentDate = dtpPaymentDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                ////MasterVM.PaymentAmount = Convert.ToDecimal(txtPaymentAmount.Text.Trim());
                ////MasterVM.VATAmount = Convert.ToDecimal(txtVATAmount.Text.Trim());
                ////MasterVM.Remarks = txtRemarks.Text.Trim().Replace(" ", "");
                ////MasterVM.CreatedBy = Program.CurrentUser;
                ////MasterVM.CreatedOn = vDateTime;
                ////MasterVM.LastModifiedBy = Program.CurrentUser;
                ////MasterVM.LastModifiedOn = vDateTime;
                ////MasterVM.Post = "Y";

                #endregion Master

                #region dgvBankChannel

                BankVMList = new List<BankChannelPaymentVM>();

                for (int i = 0; i < dgvBankChannel.RowCount; i++)
                {
                    BankChannelPaymentVM BankVm = new BankChannelPaymentVM();

                    BankVm.Id = dgvBankChannel.Rows[i].Cells["Id"].Value == null ? "" : dgvBankChannel.Rows[i].Cells["Id"].Value.ToString();
                    BankVm.PurchaseInvoiceNo = dgvBankChannel.Rows[i].Cells["PurchaseInvoiceNo"].Value.ToString();

                    BankVm.BankID = dgvBankChannel.Rows[i].Cells["BankID"].Value.ToString();
                    if (string.IsNullOrWhiteSpace(BankVm.BankID))
                    {
                        MessageBox.Show(MessageVM.BankChannelmsgPleaseEnterBankInformation);
                        return;
                    }

                    BankVm.BankName = dgvBankChannel.Rows[i].Cells["BankName"].Value.ToString();
                    BankVm.PaymentDate = Convert.ToDateTime(dgvBankChannel.Rows[i].Cells["PaymentDate"].Value).ToString("yyyy-MMM-dd HH:mm:ss");
                    BankVm.PaymentAmount = Convert.ToDecimal(dgvBankChannel.Rows[i].Cells["PaymentAmount"].Value.ToString());
                    BankVm.VATAmount = Convert.ToDecimal(dgvBankChannel.Rows[i].Cells["VATAmount"].Value.ToString());
                    BankVm.Remarks = dgvBankChannel.Rows[i].Cells["Remarks"].Value.ToString();
                    BankVm.PaymentType = dgvBankChannel.Rows[i].Cells["PaymentType"].Value.ToString();
                    BankVm.Post = "Y";
                    BankVm.CreatedBy = Program.CurrentUser;
                    BankVm.CreatedOn = vDateTime;
                    BankVm.LastModifiedBy = Program.CurrentUser;
                    BankVm.LastModifiedOn = vDateTime;

                    BankVMList.Add(BankVm);

                }

                #endregion details

                #region Button Stats

                this.btnPost.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Background Worker - Post

                bgwPost.RunWorkerAsync();

                #endregion

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
                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
            }
            #endregion


        }

        private void bgwPost_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {

                #region Statement

                POST_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                BankChannelPaymentDAL _dal = new BankChannelPaymentDAL();

                sqlResults = _dal.BankChannelPostList(BankVMList,null,null,connVM);

                POST_DOWORK_SUCCESS = true;

                #endregion

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
                FileLogger.Log(this.Name, "bgwPost_DoWork", exMessage);
            }
            #endregion

        }

        private void bgwPost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                #region Start

                if (POST_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                        }

                        if (result == "Success")
                        {
                            IsUpdate = true;

                            IsPostAll = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;

                            SelectInvoiceNo();
                        }
                    }

                #endregion

                ChangeData = false;
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
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                ChangeData = false;
                this.btnPost.Enabled = true;
                this.progressBar1.Visible = false;

            }

        }

        private void searchDT()
        {
            try
            {
                bgwSearch.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "searchDT", exMessage);
            }
            #endregion

        }

        private void bgwSearch_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                #region Statement

                BankChannelPaymentDAL _dal = new BankChannelPaymentDAL();

                string[] cValues = new string[] { txtPurchaseInvoiceNo.Text.Trim() };
                string[] cFields = new string[] { "bcp.PurchaseInvoiceNo" };

                dt = _dal.SelectAll(null, cFields, cValues,null,null,null,connVM);

                #endregion

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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion

        }

        private void bgwSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            try
            {
                #region Statement

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {

                        txtId.Text = item["Id"].ToString();
                        txtPaymentAmount.Text = item["PaymentAmount"].ToString();
                        txtVATAmount.Text = item["VATAmount"].ToString();
                        txtRemarks.Text = item["Remarks"].ToString();
                        txtBankID.Text = item["BankID"].ToString();
                        txtBankName.Text = item["BankName"].ToString();
                        IsPost = item["Post"].ToString() == "Y";
                        dtpPaymentDate.Value = Convert.ToDateTime(item["PaymentDate"].ToString());

                    }
                }


                #endregion
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }

            #endregion

            finally
            {
                this.progressBar1.Visible = false;
            }

        }

        private void dgvBankChannel_DoubleClick(object sender, EventArgs e)
        {

            try
            {

                if (dgvBankChannel.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataGridViewSelectedRowCollection selectedRows = dgvBankChannel.SelectedRows;

                #region Statement

                if (selectedRows != null && selectedRows.Count > 0)
                {
                    DataGridViewRow dgvRow = selectedRows[0];
                    if (dgvRow != null)
                    {
                        #region Value Assign

                        txtId.Text = dgvRow.Cells["Id"].Value == null ? "" : dgvRow.Cells["Id"].Value.ToString();
                        txtPurchaseInvoiceNo.Text = dgvRow.Cells["PurchaseInvoiceNo"].Value == null ? "" : dgvRow.Cells["PurchaseInvoiceNo"].Value.ToString();
                        txtPaymentAmount.Text = dgvRow.Cells["PaymentAmount"].Value == null ? "" : dgvRow.Cells["PaymentAmount"].Value.ToString();
                        txtVATAmount.Text = dgvRow.Cells["VATAmount"].Value == null ? "" : dgvRow.Cells["VATAmount"].Value.ToString();
                        txtBankID.Text = dgvRow.Cells["BankID"].Value == null ? "" : dgvRow.Cells["BankID"].Value.ToString();
                        txtBankName.Text = dgvRow.Cells["BankName"].Value == null ? "" : dgvRow.Cells["BankName"].Value.ToString();
                        txtRemarks.Text = dgvRow.Cells["Remarks"].Value == null ? "" : dgvRow.Cells["Remarks"].Value.ToString();
                        if (dgvRow.Cells["PaymentType"].Value != null)
                        {
                            cmbPaymentType.Text = dgvRow.Cells["PaymentType"].Value.ToString();
                        }
                        IsPost = dgvRow.Cells["Post"].Value == null ? false : dgvRow.Cells["Post"].Value.ToString() == "Y";

                        if (dgvRow.Cells["PaymentDate"].Value != null)
                        {
                            dtpPaymentDate.Value = Convert.ToDateTime(dgvRow.Cells["PaymentDate"].Value.ToString());

                        }

                        #endregion

                    }
                }

                #endregion

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            #region try
            try
            {

                #region Null check

                if (string.IsNullOrWhiteSpace(txtRemarks.Text))
                {
                    txtRemarks.Text = "-";
                }

                if (string.IsNullOrWhiteSpace(txtPurchaseInvoiceNo.Text))
                {
                    MessageBox.Show(MessageVM.BankChannelmsgPleaseSelectPurchaseInvoiceNo);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtPaymentAmount.Text))
                {
                    MessageBox.Show(MessageVM.BankChannelmsgPleaseEnterPaymentAmount);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtVATAmount.Text))
                {
                    MessageBox.Show(MessageVM.BankChannelmsgPleaseEnterVATAmount);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtBankID.Text))
                {
                    MessageBox.Show(MessageVM.BankChannelmsgPleaseEnterBankInformation);
                    return;
                }

                if (string.IsNullOrWhiteSpace(cmbPaymentType.Text))
                {
                    MessageBox.Show(MessageVM.BankChannelmsgPleaseSelectPaymentType);
                    return;
                }

                #endregion null

                int Index = dgvBankChannel.CurrentRow.Index;

                DataGridViewLoad(Index, "Change");

                ClearAll();

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
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }
            #endregion

        }

        private void DataGridViewLoad(int paramIndex, string RowType)
        {
            #region try

            try
            {

                #region DataGrid Load

                dgvBankChannel["PurchaseInvoiceNo", paramIndex].Value = txtPurchaseInvoiceNo.Text.Trim();
                dgvBankChannel["BankID", paramIndex].Value = txtBankID.Text.Trim();
                dgvBankChannel["BankName", paramIndex].Value = txtBankName.Text.Trim();
                dgvBankChannel["PaymentDate", paramIndex].Value = dtpPaymentDate.Value.ToString();
                dgvBankChannel["PaymentAmount", paramIndex].Value = txtPaymentAmount.Text.Trim();
                dgvBankChannel["VATAmount", paramIndex].Value = txtVATAmount.Text.Trim();
                dgvBankChannel["Remarks", paramIndex].Value = txtRemarks.Text.Trim();
                dgvBankChannel["PaymentType", paramIndex].Value = cmbPaymentType.Text.Trim();
                if (IsPost)
                {
                    dgvBankChannel["Post", paramIndex].Value = "Y";
                }
                else if (!IsPost)
                {
                    dgvBankChannel["Post", paramIndex].Value = "N";
                }

                #endregion

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
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }
            #endregion

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            try
            {
                txtPurchaseInvoiceNo.Clear();
                txtId.Clear();
                txtPaymentAmount.Clear();
                txtBankID.Clear();
                txtBankName.Clear();
                txtVATAmount.Clear();
                txtRemarks.Clear();
                IsPost = false;
                dtpPaymentDate.Value = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SelectInvoiceNo()
        {
            try
            {
                string PurchaseInvoiceNo = "";

                for (int i = 0; i < dgvBankChannel.RowCount; i++)
                {
                    BankChannelPaymentVM BankVm = new BankChannelPaymentVM();

                    string InvoiceNo = dgvBankChannel.Rows[i].Cells["PurchaseInvoiceNo"].Value.ToString();

                    PurchaseInvoiceNo += "'" + InvoiceNo + "'" + ",";

                }

                PurchaseInvoiceNo = PurchaseInvoiceNo.Trim();
                PurchaseInvoiceNo = PurchaseInvoiceNo.Trim(',');

                #region Statement

                ////// Start Complete
               

                #region Select

                BankChannelPaymentDAL _dal = new BankChannelPaymentDAL();

                dt = _dal.SelectAll(null, null, null, null, null, PurchaseInvoiceNo,connVM);

                #endregion

                dgvBankChannel.Rows.Clear();
                int j = 0;

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow vitem in dt.Rows)
                    {
                        DataGridViewRow NewRow = new DataGridViewRow();

                        dgvBankChannel.Rows.Add(NewRow);

                        dgvBankChannel.Rows[j].Cells["Id"].Value = vitem["Id"].ToString();
                        dgvBankChannel.Rows[j].Cells["PurchaseInvoiceNo"].Value = vitem["PurchaseInvoiceNo"].ToString();
                        dgvBankChannel.Rows[j].Cells["PaymentAmount"].Value = vitem["PaymentAmount"].ToString();
                        dgvBankChannel.Rows[j].Cells["VATAmount"].Value = vitem["VATAmount"].ToString();
                        dgvBankChannel.Rows[j].Cells["Remarks"].Value = vitem["Remarks"].ToString();
                        dgvBankChannel.Rows[j].Cells["BankID"].Value = vitem["BankID"].ToString();
                        dgvBankChannel.Rows[j].Cells["BankName"].Value = vitem["BankName"].ToString();
                        dgvBankChannel.Rows[j].Cells["Post"].Value = vitem["Post"].ToString();
                        dgvBankChannel.Rows[j].Cells["PaymentDate"].Value = Convert.ToDateTime(vitem["PaymentDate"].ToString());
                        dgvBankChannel.Rows[j].Cells["PaymentType"].Value = vitem["PaymentType"].ToString();

                        j = j + 1;

                    }
                }

                #endregion


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
