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
using SymphonySofttech.Reports;
using System.Drawing.Printing;

namespace VATClient
{
    public partial class FormClient6_3 : Form
    {
        public FormClient6_3()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }

        #region Global Variable

        ParameterVM paramVM = new ParameterVM();
        ResultVM rVM = new ResultVM();

        private Client6_3VM masterVM = new Client6_3VM();
        private List<Client6_3DetailVM> detailVMs = new List<Client6_3DetailVM>();
        private ReportDocument reportDocument = new ReportDocument();
        private string VPrinterName = string.Empty;
        private bool PreviewOnly;
        private string WantToPrint = "N";
        DataTable dtMaster = new DataTable();

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private bool IsPost = false;
        private bool ChangeData = false;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        private Client6_3DAL _dal = new Client6_3DAL();
        private string transactionType = string.Empty;
        //public static string vVendorID = "0";
        private string CurrentVendorId = "0";
        

        #endregion

        private void FormClient6_3_Load(object sender, EventArgs e)
        {

            #region Event Attach

            txtUnitPrice.TextChanged += LineCalculation_MethodCall;
            txtSDRate.TextChanged += LineCalculation_MethodCall;
            txtVATRate.TextChanged += LineCalculation_MethodCall;

            #endregion

            #region Form Maker

            FormMaker();

            #endregion

            #region transaction Types

            transactionTypes();

            #endregion

        }


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

                string defaultVendorName = txtVendor.Text;
                string defaultAddress = "";
                string defaultVendorId = txtVendorID.Text;

                #region Client 63

                transactionType = "ClientFGReceiveWOBOM";

                DataTable dtRow = FormPurchaseSearch.SelectMultiple(transactionType, false, true,false,false,null,true);

                if (dtRow != null && dtRow.Rows.Count > 0)
                {

                    #region Variables
                    
                    DataTable dtPurchase = new DataTable();
                    ProductDAL _ProductDAL = new ProductDAL();
                    DataTable dtProduct = new DataTable();

                    CommonDAL _CommonDAL = new CommonDAL();
                     

                    string ItemNo = "";
                    decimal Quantity = 0;
                    decimal UnitPrice = 0;
                    decimal Subtotal = 0;
                    decimal SDRate = 0;
                    decimal SDAmount = 0;
                    decimal VATRate = 0;
                    decimal VATAmount = 0;
                    decimal LineTotalAmount = 0;

                    #endregion

                    int LineNo = 0;
                    foreach (DataRow dr in dtRow.Rows)
                    {
                        #region Vendor Validation

                        if (LineNo == 0 && string.IsNullOrWhiteSpace(defaultVendorId))
                        {
                            defaultVendorName = dr["VendorName"].ToString();
                            defaultVendorId = dr["VendorID"].ToString();
                            CurrentVendorId = dr["VendorID"].ToString();



                            VendorVM vVendorVM = new VendorVM();
                            VendorDAL VendorDAL = new VendorDAL();

                            vVendorVM = VendorDAL.SelectAllList(Convert.ToInt32(defaultVendorId), null, null,null,null,connVM).FirstOrDefault();

                            defaultAddress = vVendorVM.Address1;
                        }
                        else
                        {
                            if (defaultVendorName != dr["VendorName"].ToString())
                            {
                                dgvClient6_3.Rows.Clear();

                                MessageBox.Show("Please Select Same Vendor", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                SearchData();

                                break;
                            }
                        }

                        #endregion

                        dtPurchase = new DataTable();

                        string ReceiveNo = dr["PurchaseInvoiceNo"].ToString();

                        dtPurchase = new PurchaseDAL().SelectPurchaseDetail(ReceiveNo);



                        foreach (DataRow drD in dtPurchase.Rows)
                        {
                            #region Reset Fields

                            Quantity = 0;
                            UnitPrice = 0;
                            Subtotal = 0;
                            SDRate = 0;
                            SDAmount = 0;
                            VATRate = 0;
                            VATAmount = 0;
                            LineTotalAmount = 0;

                            #endregion

                            DataGridViewRow NewRow = new DataGridViewRow();
                            dgvClient6_3.Rows.Add(NewRow);

                            int Index = dgvClient6_3.RowCount - 1;

                            dgvClient6_3["InvoiceLineNo", Index].Value = (LineNo + 1);

                            dgvClient6_3["ReceiveNo", Index].Value = drD["PurchaseInvoiceNo"].ToString();
                            dgvClient6_3["InvoiceDateTime", Index].Value = drD["InvoiceDateTime"].ToString();

                            #region Product Call
                            string code = _CommonDAL.settingValue("CompanyCode", "Code",connVM);

                            ItemNo = drD["ItemNo"].ToString();
                            if (code.ToLower()=="scbl")
                            {
                                paramVM = new ParameterVM();
                                paramVM.TableName = "Products p ";
                                paramVM.JoinClause = @" left Outer join ProductCustomerRate pc on p.ItemNo=pc.ItemNo and p.BranchId=pc.BranchId";
                                paramVM.selectFields = new[] { "ISNULL(pc.TollCharge,0) TollCharge", "p.SDRate", "p.VATRate" };
                                paramVM.conditionFields = new[] { "pc.ItemNo", "pc.CustomerId" };
                                paramVM.conditionValues = new[] { ItemNo, defaultVendorId };
                                dtProduct = _CommonDAL.Select(paramVM,null,null,connVM);
                            }
                            else
                            {
                                paramVM = new ParameterVM();
                                paramVM.TableName = "Products";
                                paramVM.selectFields = new[] { "ISNULL(TollCharge,0) TollCharge", "SDRate", "VATRate" };
                                paramVM.conditionFields = new[] { "ItemNo" };
                                paramVM.conditionValues = new[] { ItemNo };

                                dtProduct = _CommonDAL.Select(paramVM,null,null,connVM);
                            }
                           

                            if (dtProduct == null || dtProduct.Rows.Count == 0)
                            {
                                continue;
                            }

                            #endregion

                            DataRow drProduct = dtProduct.Rows[0];

                            Quantity = Convert.ToDecimal(Program.ParseDecimalObject(drD["Quantity"].ToString()));

                            UnitPrice = Convert.ToDecimal(drProduct["TollCharge"] ?? 0);
                            Subtotal = Quantity*UnitPrice;
                            SDRate = Convert.ToDecimal(drProduct["SDRate"] == DBNull.Value ? 0 : drProduct["SDRate"]);
                            SDAmount = Subtotal * SDRate/100;
                            VATRate = Convert.ToDecimal(drProduct["VATRate"] ?? 0);
                            VATAmount = (Subtotal+SDAmount)*VATRate/100;
                            LineTotalAmount = Subtotal+SDAmount+VATAmount;

                            dgvClient6_3["ItemNo", Index].Value = ItemNo;
                            dgvClient6_3["ItemName", Index].Value = drD["ProductName"].ToString();
                            dgvClient6_3["UOM", Index].Value = drD["UOM"].ToString();
                            dgvClient6_3["Quantity", Index].Value = Quantity;
                            dgvClient6_3["UnitPrice", Index].Value = Program.ParseDecimalObject(UnitPrice);
                            dgvClient6_3["Subtotal", Index].Value = Program.ParseDecimalObject(Subtotal);
                            dgvClient6_3["SDRate", Index].Value = Program.ParseDecimalObject(SDRate);
                            dgvClient6_3["SDAmount", Index].Value = Program.ParseDecimalObject(SDAmount);
                            dgvClient6_3["VATRate", Index].Value = Program.ParseDecimalObject(VATRate);
                            dgvClient6_3["VATAmount", Index].Value = Program.ParseDecimalObject(VATAmount);
                            dgvClient6_3["LineTotalAmount", Index].Value = Program.ParseDecimalObject(LineTotalAmount);

                            LineNo = LineNo + 1;

                        }


                    }


                    txtVendor.Text = defaultVendorName;
                    txtVendorID.Text = defaultVendorId;
                    txtVendorAddress.Text = defaultAddress;

                    btnSave.Enabled = true;
                    btnUpdate.Enabled = true;
                    btnPost.Enabled = true;
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

            for (int i = 0; i < dgvClient6_3.RowCount; i++)
            {
                dgvClient6_3[0, i].Value = i + 1;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvClient6_3.RowCount > 0)
            {
                if (MessageBox.Show(MessageVM.msgWantToRemoveRow, this.Text, MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dgvClient6_3.Rows.RemoveAt(dgvClient6_3.CurrentRow.Index);
                }
                Rowcalculate();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                #region Check Point

                DataTable dtMaster = new DataTable();

                if (!string.IsNullOrWhiteSpace(txtInvoiceNo.Text))
                {

                    dtMaster = new Client6_3DAL().Select(new[] { "cln.InvoiceNo" }, new[] { txtInvoiceNo.Text });

                    if (dtMaster != null && dtMaster.Rows.Count > 0)
                    {
                        MessageBox.Show("Invoice Already Exist! " + txtInvoiceNo.Text, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }


                #endregion

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

                masterVM.Details = detailVMs;

                rVM = _dal.Insert(masterVM,null,null,connVM);

                #endregion

                #region Results

                if (rVM.Status == "Success")
                {
                    txtInvoiceNo.Text = rVM.InvoiceNo;

                }

                #endregion

                #region Message

                MessageBox.Show(rVM.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

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

                if (string.IsNullOrWhiteSpace(txtInvoiceNo.Text))
                {
                    MessageBox.Show("Please Save First!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


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

                masterVM.Details = detailVMs;

                rVM = _dal.Update(masterVM,null,null,connVM);

                #endregion

                #region Message

                MessageBox.Show(rVM.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

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

                masterVM = new Client6_3VM();
                masterVM.InvoiceNo = txtInvoiceNo.Text.Trim();
                masterVM.VendorID = txtVendorID.Text.Trim();

                masterVM.Address = txtVendorAddress.Text.Trim();
                masterVM.InvoiceDateTime = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                masterVM.BranchId = Program.BranchId;
                masterVM.Comments = txtComments.Text.Trim();
                masterVM.TransactionType = transactionType;
                masterVM.Post = "N";

                #endregion

                #region Details

                detailVMs = new List<Client6_3DetailVM>();
                foreach (DataGridViewRow dr in dgvClient6_3.Rows)
                {
                    Client6_3DetailVM detailVM = new Client6_3DetailVM();

                    detailVM.InvoiceNo = txtInvoiceNo.Text.Trim(); ;
                    detailVM.InvoiceLineNo = Convert.ToInt32(dr.Cells["InvoiceLineNo"].Value.ToString());
                    detailVM.ReceiveNo = dr.Cells["ReceiveNo"].Value.ToString();
                    detailVM.InvoiceDateTime = dr.Cells["InvoiceDateTime"].Value.ToString();


                    detailVM.ItemNo = dr.Cells["ItemNo"].Value.ToString();
                    detailVM.UOM = dr.Cells["UOM"].Value.ToString();
                    detailVM.Quantity = Convert.ToDecimal(Program.ParseDecimalObject(dr.Cells["Quantity"].Value));
                    detailVM.UnitPrice = Convert.ToDecimal(Program.ParseDecimalObject(dr.Cells["UnitPrice"].Value));
                    detailVM.Subtotal = Convert.ToDecimal(Program.ParseDecimalObject(dr.Cells["Subtotal"].Value));
                    detailVM.SDRate = Convert.ToDecimal(Program.ParseDecimalObject(dr.Cells["SDRate"].Value));
                    detailVM.SDAmount = Convert.ToDecimal(Program.ParseDecimalObject(dr.Cells["SDAmount"].Value));
                    detailVM.VATRate = Convert.ToDecimal(Program.ParseDecimalObject(dr.Cells["VATRate"].Value));
                    detailVM.VATAmount = Convert.ToDecimal(Program.ParseDecimalObject(dr.Cells["VATAmount"].Value));
                    detailVM.LineTotalAmount = Convert.ToDecimal(Program.ParseDecimalObject(dr.Cells["LineTotalAmount"].Value));
                    detailVM.Comments = (dr.Cells["Comments"].Value ?? "").ToString();
                    detailVM.Post = "N";


                    detailVM.BranchId = Program.BranchId;
                    detailVM.TransactionType = transactionType;

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


        private void transactionTypes()
        {
            #region try

            try
            {

                #region Transaction Type

                transactionType = "Other";

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


                label37.Text = "Vendor";
                this.Text = "Client 6.3";

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

        private void btnInvoiceNo_Click(object sender, EventArgs e)
        {
            try
            {

                #region Transaction Type

                transactionTypes();

                #endregion

                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormClient6_3Search.SelectOne(transactionType);

                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    #region Master

                    Client6_3VM masterVM = new Client6_3VM();
                    string[] cFields = new[] { "cln.Id" };
                    string[] cValues = new[] { selectedRow.Cells["Id"].Value.ToString() };


                    masterVM = _dal.SelectVM(cFields, cValues,null,null,connVM).FirstOrDefault();

                    txtInvoiceNo.Text = masterVM.InvoiceNo;
                    txtVendorID.Text = masterVM.VendorID.ToString();
                    txtVendor.Text = masterVM.VendorName;
                    CurrentVendorId = masterVM.VendorID.ToString();

                    txtVendorAddress.Text = masterVM.Address;
                    dtpInvoiceDate.Text = masterVM.InvoiceDateTime;
                    txtPost.Text = masterVM.Post;
                    txtComments.Text = masterVM.Comments;

                    #endregion

                    #region Detail

                    List<Client6_3DetailVM> dVMs = new List<Client6_3DetailVM>();
                    cFields = new[] { "clnd.InvoiceNo" };
                    cValues = new[] { masterVM.InvoiceNo };
                    dVMs = _dal.SelectDetailVM(cFields, cValues,null,null,connVM);

                    dgvClient6_3.Rows.Clear();
                    int j = 0;

                    foreach (Client6_3DetailVM detailVM in dVMs)
                    {


                        DataGridViewRow NewRow = new DataGridViewRow();
                        dgvClient6_3.Rows.Add(NewRow);


                        dgvClient6_3.Rows[j].Cells["InvoiceLineNo"].Value = detailVM.InvoiceLineNo;
                        dgvClient6_3.Rows[j].Cells["ReceiveNo"].Value = detailVM.ReceiveNo;
                        dgvClient6_3.Rows[j].Cells["InvoiceDateTime"].Value = detailVM.InvoiceDateTime;

                        dgvClient6_3.Rows[j].Cells["ItemNo"].Value = detailVM.ItemNo;
                        dgvClient6_3.Rows[j].Cells["ItemName"].Value = detailVM.ProductName;

                        dgvClient6_3.Rows[j].Cells["UOM"].Value = detailVM.UOM;
                        dgvClient6_3.Rows[j].Cells["Quantity"].Value = Program.ParseDecimalObject(detailVM.Quantity);
                        dgvClient6_3.Rows[j].Cells["UnitPrice"].Value = Program.ParseDecimalObject(detailVM.UnitPrice);
                        dgvClient6_3.Rows[j].Cells["Subtotal"].Value = Program.ParseDecimalObject(detailVM.Subtotal);
                        dgvClient6_3.Rows[j].Cells["SDRate"].Value = Program.ParseDecimalObject(detailVM.SDRate);
                        dgvClient6_3.Rows[j].Cells["SDAmount"].Value = Program.ParseDecimalObject(detailVM.SDAmount);
                        dgvClient6_3.Rows[j].Cells["VATRate"].Value = Program.ParseDecimalObject(detailVM.VATRate);
                        dgvClient6_3.Rows[j].Cells["VATAmount"].Value = Program.ParseDecimalObject(detailVM.VATAmount);
                        dgvClient6_3.Rows[j].Cells["LineTotalAmount"].Value = Program.ParseDecimalObject(detailVM.LineTotalAmount);
                        dgvClient6_3.Rows[j].Cells["Comments"].Value = detailVM.Comments;


                        j++;

                    }



                    #endregion

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


            txtInvoiceNo.Text = "";
            txtVendorID.Text = "";
            txtVendorAddress.Text = DateTime.Now.ToString();
            dtpInvoiceDate.Text = "";
            txtPost.Text = "";
            txtComments.Text = "";
            txtVendor.Text = "";
            txtVendorAddress.Text = "";


            #region Details

            txtReceiveNo.Text = "";
            txtItemName.Text = "";

            txtQuantity.Text = "0";
            txtUnitPrice.Text = "0";
            txtSubtotal.Text = "0";
            txtSDRate.Text = "0";
            txtSDAmount.Text = "0";
            txtVATRate.Text = "0";
            txtVATAmount.Text = "0";
            txtLineTotalAmount.Text = "0";

            #endregion

            dgvClient6_3.Rows.Clear();



        }

        private void btnPost_Click(object sender, EventArgs e)
        {

            try
            {

                #region Check Point

                if (string.IsNullOrWhiteSpace(txtInvoiceNo.Text.Trim()))
                {
                    MessageBox.Show("Please Save First!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                dtMaster = new Client6_3DAL().Select(new[] { "cln.InvoiceNo" }, new[] { txtInvoiceNo.Text });

                if (dtMaster != null && dtMaster.Rows.Count > 0 && dtMaster.Rows[0]["Post"].ToString() == "Y")
                {
                    MessageBox.Show("Invoice Already Posted! " + txtInvoiceNo.Text, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                #endregion


                masterVM = new Client6_3VM();
                masterVM.InvoiceNo = txtInvoiceNo.Text.Trim();



                paramVM = new ParameterVM();
                paramVM.InvoiceNo = txtInvoiceNo.Text.Trim();

                #region Find UserInfo

                DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + Program.CurrentUserID + "'");

                #endregion

                paramVM.SignatoryName = userInfo[0]["FullName"].ToString();
                paramVM.SignatoryDesig = userInfo[0]["Designation"].ToString();

                rVM = _dal.Post(paramVM,null,null,connVM);

                //////_dal.UpdateTollCompleted("Y", masterVM.TollNo);


                if (rVM.Status != "Fail")
                {
                    txtPost.Text = rVM.Post;

                }
              

                MessageBox.Show(rVM.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);


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

        private void btnClient6_3Report_Click(object sender, EventArgs e)
        {
            try
            {


                #region Check Point

                if (txtInvoiceNo.Text == "~~~ New ~~~" || txtInvoiceNo.Text == "")
                {
                    MessageBox.Show("Please , Select Invoice No First!", this.Text);
                    return;
                }
                if (txtPost.Text == "N")
                {
                    MessageBox.Show("This transaction not posted, please post first", this.Text);
                    return;
                }
                #endregion

                #region Flag Update

                PreviewOnly = false;

                #endregion

                #region Element States


                #endregion

                #region Static Values

                Program.fromOpen = "Me";

                #endregion

                #region Print Progress

                string result = FormSalePrint.SelectOne(0);
                string[] PrintResult = result.Split(FieldDelimeter.ToCharArray());
                WantToPrint = PrintResult[1];


                if (WantToPrint == "N")
                {
                    return;


                }
                else
                {

                    WantToPrint = PrintResult[1];
                    VPrinterName = PrintResult[2];

                }

                #endregion

                #region Report Preview

                PreviewDetails(false);

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
                FileLogger.Log(this.Name, "btnToll6_3_Click", exMessage);
            }

            #endregion
        }


        private void PreviewDetails(bool PreviewOnly = false)
        {
            #region Try
            try
            {
                #region Get Data

                NBRReports _report = new NBRReports();

                reportDocument = _report.Client6_3(txtInvoiceNo.Text.Trim(), Program.BranchId, PreviewOnly,connVM);

                #endregion

                #region Show Report

                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (PreviewOnly == true)
                {
                    FormReport reports = new FormReport();
                    reports.crystalReportViewer1.Refresh();
                    reports.setReportSource(reportDocument);
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();
                }
                else
                {

                    System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
                    printerSettings.PrinterName = VPrinterName;
                    reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);

                    MessageBox.Show("You have successfully print ");

                }

                #endregion
        }

            #endregion

            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +
                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }
                MessageBox.Show(ex.Message, this.Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "PreviewDetails",
                               exMessage);
            }
            #endregion

            #region Finally
            finally
            {
                this.btnClient6_3Report.Enabled = true;
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
                    settingsDt = new CommonDAL().SettingDataAll(null, null);
                }
                CommonDAL _cDal = new CommonDAL();
                vVAT2012V2 = _cDal.settingsDesktop("Version", "VAT2012V2", settingsDt,connVM);
                string TollDate = "";
                string SignatoryName = "";
                string SignatoryDesig = "";
                DateTime invoiceDateTime = DateTime.Now;
                DataSet ds = new DataSet();
                IReport Reportdal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

                ds = Reportdal.VAT6_3Toll(txtInvoiceNo.Text.Trim(), "", "", connVM);
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

        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {

                LineCalculation();

                int Index = dgvClient6_3.CurrentRow.Index;

                dgvClient6_3["UnitPrice", Index].Value = Program.ParseDecimalObject(txtUnitPrice.Text);
                dgvClient6_3["Subtotal", Index].Value = Program.ParseDecimalObject(txtSubtotal.Text);
                dgvClient6_3["SDRate", Index].Value = Program.ParseDecimalObject(txtSDRate.Text);
                dgvClient6_3["SDAmount", Index].Value = Program.ParseDecimalObject(txtSDAmount.Text);
                dgvClient6_3["VATRate", Index].Value = Program.ParseDecimalObject(txtVATRate.Text);
                dgvClient6_3["VATAmount", Index].Value = Program.ParseDecimalObject(txtVATAmount.Text);
                dgvClient6_3["LineTotalAmount", Index].Value = Program.ParseDecimalObject(txtLineTotalAmount.Text);


                #region Reset Form Elements

                txtReceiveNo.Text = "";
                txtItemName.Text = "";
                txtUOM.Text = "";
                txtQuantity.Text = "0";


                txtUnitPrice.Text = "0";
                txtSubtotal.Text = "0";
                txtSDRate.Text = "0";
                txtSDAmount.Text = "0";
                txtVATRate.Text = "0";
                txtVATAmount.Text = "0";
                txtLineTotalAmount.Text = "0";

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
                FileLogger.Log(this.Name, "btnChange_Click", exMessage);
            }

            #endregion
        }

        private void dgvClient6_3_DoubleClick(object sender, EventArgs e)
        {
            try
            {

                DataGridViewRow dgvr = dgvClient6_3.CurrentRow;
                txtReceiveNo.Text = dgvr.Cells["ReceiveNo"].Value.ToString();
                txtItemName.Text = dgvr.Cells["ItemName"].Value.ToString();
                txtUOM.Text = dgvr.Cells["UOM"].Value.ToString();
                txtQuantity.Text = dgvr.Cells["Quantity"].Value.ToString();
                txtUnitPrice.Text = dgvr.Cells["UnitPrice"].Value.ToString();
                txtSubtotal.Text = dgvr.Cells["Subtotal"].Value.ToString();
                txtSDRate.Text = dgvr.Cells["SDRate"].Value.ToString();
                txtSDAmount.Text = dgvr.Cells["SDAmount"].Value.ToString();
                txtVATRate.Text = dgvr.Cells["VATRate"].Value.ToString();
                txtVATAmount.Text = dgvr.Cells["VATAmount"].Value.ToString();
                txtLineTotalAmount.Text = dgvr.Cells["LineTotalAmount"].Value.ToString();

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
                FileLogger.Log(this.Name, "dgvClient6_3_DoubleClick", exMessage);
            }

            #endregion
        }

        private void LineCalculation()
        {
            try
            {

                decimal Quantity = 0;
                decimal UnitPrice = 0;
                decimal Subtotal = 0;
                decimal SDRate = 0;
                decimal SDAmount = 0;
                decimal VATRate = 0;
                decimal VATAmount = 0;
                decimal LineTotalAmount = 0;

                Quantity = Convert.ToDecimal(Program.ParseDecimalObject(txtQuantity.Text));
                UnitPrice = Convert.ToDecimal(Program.ParseDecimalObject(txtUnitPrice.Text));
                Subtotal = Convert.ToDecimal(Program.ParseDecimalObject(txtSubtotal.Text));
                SDRate = Convert.ToDecimal(Program.ParseDecimalObject(txtSDRate.Text));
                SDAmount = Convert.ToDecimal(Program.ParseDecimalObject(txtSDAmount.Text));
                VATRate = Convert.ToDecimal(Program.ParseDecimalObject(txtVATRate.Text));
                VATAmount = Convert.ToDecimal(Program.ParseDecimalObject(txtVATAmount.Text));
                LineTotalAmount = Convert.ToDecimal(Program.ParseDecimalObject(txtLineTotalAmount.Text));

                Subtotal = Quantity * UnitPrice;
                SDAmount = Subtotal * SDRate / 100;
                VATAmount = (Subtotal + SDAmount) * VATRate / 100;
                LineTotalAmount = Subtotal + SDAmount + VATAmount;

                txtSubtotal.Text = Subtotal.ToString();
                txtSDAmount.Text = SDAmount.ToString();
                txtVATAmount.Text = VATAmount.ToString();
                txtLineTotalAmount.Text = LineTotalAmount.ToString();

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
                FileLogger.Log(this.Name, "LineCalculation", exMessage);
            }

            #endregion

        }


        private void LineCalculation_MethodCall(object sender, EventArgs e)
        {
            LineCalculation();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            #region Check Point

            if (txtInvoiceNo.Text == "~~~ New ~~~" || txtInvoiceNo.Text == "")
            {
                MessageBox.Show("Please , Select Dispose No First!", this.Text);
                return;
            }

            #endregion

            #region Report Preview

            PreviewDetails(true);

            #endregion
        }

        private void txtVendorAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
            if (e.KeyCode.Equals(Keys.F9))
            {
                vendorAddressLoad();
            }
        }

        private void vendorAddressLoad()
        {
            try
            {

                if (string.IsNullOrEmpty(CurrentVendorId) || CurrentVendorId == "0")
                {
                    MessageBox.Show("Please select the Vendor first");
                    return;
                }
                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"   select v.VendorID, vg.VendorGroupName,v.VendorCode,v.VendorName,
 isnull(va.VendorAddress,v.Address1)VendorAddress from Vendors v 
left outer join VendorGroups vg on v.VendorGroupID=vg.VendorGroupID 
left outer join VendorsAddress va on v.VendorID=va.VendorID 
where 1=1 and  v.ActiveStatus='Y' ";

                SqlText += @"  and v.VendorID='" + CurrentVendorId + "'  ";

                string SQLTextRecordCount = @" select count(VendorCode)RecordNo from Vendors";

                string[] shortColumnName = { "vg.VendorGroupName", "v.VendorCode", "v.VendorName", "va.VendorAddress" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, "", SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                  
                    txtVendorAddress.Text = "";
                    txtVendorAddress.Text = selectedRow.Cells["VendorAddress"].Value.ToString();//ProductInfo[0];
                    txtVendorAddress.Focus();

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "VendorsAddressLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //DataGridViewRow selectedRow = new DataGridViewRow();
            //selectedRow = FormCustomerAddressFinder.SelectOne(vCustomerID);
            //if (selectedRow != null && selectedRow.Index >= 0)
            //{
            //    txtDeliveryAddress1.Text = "";
            //    txtDeliveryAddress1.Text = selectedRow.Cells["CustomerAddress"].Value.ToString();//ProductInfo[0];
            //}
            //txtDeliveryAddress1.Focus();
        }

        private void txtVendorAddress_DoubleClick(object sender, EventArgs e)
        {
            vendorAddressLoad();
        }


    }
}
