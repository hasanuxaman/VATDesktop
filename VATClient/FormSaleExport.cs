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

namespace VATClient
{
    public partial class FormSaleExport : Form
    {
        public FormSaleExport()
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
        #endregion 
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {


                SaleExportVM Master = new SaleExportVM();
                Master.SaleExportDate = dtpSaleExportDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                Master.Description = txtDescription.Text.Trim();
                Master.Comments = txtComments.Text.Trim();
                Master.Quantity = txtQuantity.Text.Trim();
                Master.GrossWeight = txtGrossWeight.Text.Trim();
                Master.NetWeight = txtNetWeight.Text.Trim();
                Master.NumberFrom = txtNumberFrom.Text.Trim();
                Master.NumberTo = txtNumberTo.Text.Trim();
                Master.PortFrom = txtPortFrom.Text.Trim();
                Master.PortTo = txtPortTo.Text.Trim();
                Master.BranchId = Program.BranchId;
                Master.CreatedBy = Program.CurrentUser;
                Master.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                Master.LastModifiedBy = Program.CurrentUser;
                Master.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                List<SaleExportInvoiceVM> Details = new List<SaleExportInvoiceVM>();


                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    SaleExportInvoiceVM detail = new SaleExportInvoiceVM();

                    detail.SL = dgvSale.Rows[i].Cells["SL"].Value.ToString();
                    detail.SalesInvoiceNo = dgvSale.Rows[i].Cells["SalesInvoiceNo"].Value.ToString();
                    Details.Add(detail);
                }
                if (Details.Count() <= 0)
                {
                    MessageBox.Show("Please insert Sale Invoice information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                SaleExportDAL dal = new SaleExportDAL();

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                sqlResults = dal.SaleExportInsert(Master, Details, null, null,connVM);
                SAVE_DOWORK_SUCCESS = true;
                if (SAVE_DOWORK_SUCCESS)
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

                        }
                        if (result == "Success")
                        {
                            txtSaleExportNo.Text = sqlResults[2].ToString();
                            //IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                        }
                    }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormSaleVAT20Multiple frm = new FormSaleVAT20Multiple();
            string invoices = "";

            invoices = FormSaleVAT20Multiple.SelectOne();
            dgvSale.Rows.Clear();
            for (int i = 0; i < invoices.Split('^').Length; i++)
            {
                var tt = invoices.Split('^')[i];
                if (!string.IsNullOrEmpty(tt.Trim()))
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvSale.Rows.Add(NewRow);
                    dgvSale["SalesInvoiceNo", dgvSale.RowCount - 1].Value = tt.ToString().Trim().Split('~')[0];
                    dgvSale["InvoiceDate", dgvSale.RowCount - 1].Value = tt.ToString().Trim().Split('~')[1];
                }
            }
            Rowcalculate();
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
        private void Rowcalculate()
        {

            for (int i = 0; i < dgvSale.RowCount; i++)
            {
                dgvSale[0, i].Value = i + 1;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FormSaleVAT20Multiple frm = new FormSaleVAT20Multiple();
            //FormSaleVAT20Multiple.SelectOne("Export", null);


        }

        private void bthUpdate_Click(object sender, EventArgs e)
        {
            try
            {

                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                SaleExportVM Master = new SaleExportVM();
                Master.SaleExportNo = txtSaleExportNo.Text.Trim();
                Master.SaleExportDate = dtpSaleExportDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                Master.Description = txtDescription.Text.Trim();
                Master.Comments = txtComments.Text.Trim();
                Master.Quantity = txtQuantity.Text.Trim();
                Master.GrossWeight = txtGrossWeight.Text.Trim();
                Master.NetWeight = txtNetWeight.Text.Trim();
                Master.NumberFrom = txtNumberFrom.Text.Trim();
                Master.NumberTo = txtNumberTo.Text.Trim();
                Master.PortFrom = txtPortFrom.Text.Trim();
                Master.PortTo = txtPortTo.Text.Trim();
                Master.BranchId = Program.BranchId;
                Master.LastModifiedBy = Program.CurrentUser;
                Master.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                List<SaleExportInvoiceVM> Details = new List<SaleExportInvoiceVM>();
                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    SaleExportInvoiceVM detail = new SaleExportInvoiceVM();

                    detail.SL = dgvSale.Rows[i].Cells["SL"].Value.ToString();
                    detail.SalesInvoiceNo = dgvSale.Rows[i].Cells["SalesInvoiceNo"].Value.ToString();
                    Details.Add(detail);
                }
                if (Details.Count() <= 0)
                {
                    MessageBox.Show("Please insert Sale Invoice information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                SaleExportDAL dal = new SaleExportDAL();

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                sqlResults = dal.SaleExportUpdate(Master, Details,connVM);
                SAVE_DOWORK_SUCCESS = true;
                if (SAVE_DOWORK_SUCCESS)
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

                        }
                        if (result == "Success")
                        {
                            txtSaleExportNo.Text = sqlResults[2].ToString();
                            //IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                        }
                    }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void btnSearchInvoiceNo_Click(object sender, EventArgs e)
        {
            #region try
            try
            {

                DataGridViewRow selectedRow = null;
                selectedRow = FormSaleExportSearch.SelectOne();
                //SaleExportNo
                //SaleExportDate
                //Description
                //Comments
                //Quantity
                //GrossWeight
                //NetWeight
                //NumberFrom
                //NumberTo
                //PortFrom
                //PortTo
                //Post
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    //activestatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");
                    txtSaleExportNo.Text = selectedRow.Cells["SaleExportNo"].Value.ToString();
                    dtpSaleExportDate.Value = Convert.ToDateTime(selectedRow.Cells["SaleExportDate"].Value.ToString());
                    txtDescription.Text = selectedRow.Cells["Description"].Value.ToString();
                    txtComments.Text = selectedRow.Cells["Comments"].Value.ToString();
                    txtQuantity.Text = selectedRow.Cells["Quantity"].Value.ToString();
                    txtGrossWeight.Text = selectedRow.Cells["GrossWeight"].Value.ToString();
                    txtNetWeight.Text = selectedRow.Cells["NetWeight"].Value.ToString();
                    txtNumberFrom.Text = selectedRow.Cells["NumberFrom"].Value.ToString();
                    txtNumberTo.Text = selectedRow.Cells["NumberTo"].Value.ToString();
                    txtPortFrom.Text = selectedRow.Cells["PortFrom"].Value.ToString();
                    txtPortTo.Text = selectedRow.Cells["PortTo"].Value.ToString();
                    IsPost = Convert.ToString(selectedRow.Cells["Post"].Value.ToString()) == "Y" ? true : false;
                }

                DataTable dt = new DataTable();
                SaleExportDAL dal = new SaleExportDAL();
                dt = dal.SearchSaleExportDetailDTNew(txtSaleExportNo.Text.Trim(),connVM);
                dgvSale.Rows.Clear();
                int j = 0;
                foreach (DataRow item in dt.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvSale.Rows.Add(NewRow);

                    dgvSale.Rows[j].Cells["SL"].Value = item["SL"].ToString();
                    dgvSale.Rows[j].Cells["SalesInvoiceNo"].Value = item["SalesInvoiceNo"].ToString();
                    j = j + 1;

                }


                ChangeData = false;
            }
            #endregion
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
                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnVendorGroup_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnVendorGroup_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnVendorGroup_Click", exMessage);
            }
            #endregion
            finally
            {

                ChangeData = false;

            }
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                else if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }

                SaleExportVM Master = new SaleExportVM();
                Master.SaleExportNo = txtSaleExportNo.Text.Trim();

                Master.SaleExportDate = dtpSaleExportDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                Master.Description = txtDescription.Text.Trim();
                Master.Comments = txtComments.Text.Trim();
                Master.Quantity = txtQuantity.Text.Trim();
                Master.GrossWeight = txtGrossWeight.Text.Trim();
                Master.NetWeight = txtNetWeight.Text.Trim();
                Master.NumberFrom = txtNumberFrom.Text.Trim();
                Master.NumberTo = txtNumberTo.Text.Trim();
                Master.PortFrom = txtPortFrom.Text.Trim();
                Master.PortTo = txtPortTo.Text.Trim();
                Master.BranchId = Program.BranchId;
                Master.LastModifiedBy = Program.CurrentUser;
                Master.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                List<SaleExportInvoiceVM> Details = new List<SaleExportInvoiceVM>();
                for (int i = 0; i < dgvSale.RowCount; i++)
                {
                    SaleExportInvoiceVM detail = new SaleExportInvoiceVM();

                    detail.SL = dgvSale.Rows[i].Cells["SL"].Value.ToString();
                    detail.SalesInvoiceNo = dgvSale.Rows[i].Cells["SalesInvoiceNo"].Value.ToString();
                    Details.Add(detail);
                }
                SaleExportDAL dal = new SaleExportDAL();

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                sqlResults = dal.SaleExportPost(Master,connVM);
                SAVE_DOWORK_SUCCESS = true;
                if (SAVE_DOWORK_SUCCESS)
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

                        }
                        if (result == "Success")
                        {
                            txtSaleExportNo.Text = sqlResults[2].ToString();
                            IsPost = true;
                        }
                    }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void btn20_Click(object sender, EventArgs e)
        {

            string invoiceNov = "";
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();

            string invoiceNo;
            DataTable dt0 = new DataTable();
            DataTable dt1 = new DataTable();
            for (int i = 0; i < dgvSale.Rows.Count; i++)
            {

                invoiceNo = dgvSale["SalesInvoiceNo", i].Value.ToString();
                if (i == 0)
                {
                    invoiceNov = invoiceNo;
                }
                else
                {
                    invoiceNov += " , " + invoiceNo;
                }
            }
            ReportDSDAL ShowReport = new ReportDSDAL();
            DataSet ds = new DataSet();
            ds = ShowReport.VAT20ReportNew(txtSaleExportNo.Text.Trim());
            dt0.Merge(ds.Tables[0]);
            dt1.Merge(ds.Tables[1]);

            dt0.TableName = "DsVAT20Item";
            dt1.TableName = "DsVAT20Pack";

            DataSet ReportResultMultipleVAT20 = new DataSet();
            ReportResultMultipleVAT20.Tables.Add(dt0);
            ReportResultMultipleVAT20.Tables.Add(dt1);

            ReportClass objrpt = new ReportClass();
            //ReportDocument objrpt = new ReportDocument();

            objrpt = new RptVAT20();
            //objrpt.Load(Program.ReportAppPath + @"\RptVAT20.rpt");

            objrpt.SetDataSource(ReportResultMultipleVAT20);
            objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
            objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
            objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
            objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
            objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
            objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
            objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
            objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + Program.ZipCode + "'";
            objrpt.DataDefinition.FormulaFields["SalesInvoiceNo"].Text = "'" + invoiceNov + "'";
            objrpt.DataDefinition.FormulaFields["SaleExportDate"].Text = "'" + dtpSaleExportDate.Value.ToString("dd-MMM-yyyy") + "'";
            //objrpt.DataDefinition.FormulaFields["PortFrom"].Text = "'" + txtPortFrom.Text + "'";
            //objrpt.DataDefinition.FormulaFields["PortTo"].Text = "'" + txtPortTo.Text + "'";

            FormReport reports = new FormReport();
            objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
            reports.crystalReportViewer1.Refresh();
            reports.setReportSource(objrpt);
            if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
            reports.Show();
        }

        private void FormSaleExport_Load(object sender, EventArgs e)
        {
            //label4.Text = "Hello" + Environment.NewLine + "How are you?";
        }

        private void btnPush_Click(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            SaleDAL sDal = new SaleDAL();

            if (dgvSale != null)
            {
                DataGridViewSelectedRowCollection selectedRows = dgvSale.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    DataGridViewRow userSelRow = selectedRows[0];
                    if (userSelRow != null)
                    {
                        dataTable = sDal.SearchSalesHeaderDTNew(userSelRow.Cells["SalesInvoiceNo"].Value.ToString(),"",connVM);
                        txtPortFrom.Text = Program.Address1;
                        txtPortTo.Text = dataTable.Rows[0]["CustomerName"]
                            + Environment.NewLine
                            + dataTable.Rows[0]["DeliveryAddress1"];

                        txtComments.Text = "LC/TT No: " + dataTable.Rows[0]["LCNumber"]
                            //+ Environment.NewLine
            + ", LC/TT Date: " + Convert.ToDateTime(dataTable.Rows[0]["LCDate"]).ToString("dd/MMM/yyyy")
            + ", PI No: " + dataTable.Rows[0]["PINo"]
            + ",  PI Date: " + Convert.ToDateTime(dataTable.Rows[0]["PIDate"]).ToString("dd/MMM/yyyy");

                    }
                }
            }
            //    dataTable = sDal.SearchSalesHeaderDTNew(selectedRow.Cells["SalesInvoiceNo"].Value.ToString());

            //    txtPortTo.Text = ""+ Environment.NewLine +"";
            //    cmbShift.SelectedValue = dataTable.Rows[0]["ShiftId"];
            //    txtSalesInvoiceNo.Text = selectedRow.Cells["SalesInvoiceNo"].Value.ToString();
            //    vCustomerID = selectedRow.Cells["CustomerID"].Value.ToString();
            //    txtCustomer.Text = selectedRow.Cells["CustomerName"].Value.ToString();
            //    txtDeliveryAddress1.Text = selectedRow.Cells["DeliveryAddress1"].Value.ToString();
            //    txtDeliveryAddress2.Text = selectedRow.Cells["DeliveryAddress2"].Value.ToString();
            //    txtDeliveryAddress3.Text = selectedRow.Cells["DeliveryAddress3"].Value.ToString();
            //    txtVehicleID.Text = selectedRow.Cells["VehicleID"].Value.ToString();
            //    txtVehicleType.Text = selectedRow.Cells["VehicleType"].Value.ToString();
            //    txtVehicleNo.Text = selectedRow.Cells["VehicleNo"].Value.ToString();
            //    txtVehicleNo.Text = selectedRow.Cells["VehicleNo"].Value.ToString();
            //    txtTotalAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalAmount"].Value.ToString()).ToString();//"0,0.00");
            //    txtTotalVATAmount.Text = Convert.ToDecimal(selectedRow.Cells["TotalVATAmount"].Value.ToString()).ToString();//"0,0.00");
            //    txtSerialNo.Text = selectedRow.Cells["SerialNo"].Value.ToString();
            //    dtpInvoiceDate.Value = Convert.ToDateTime(selectedRow.Cells["InvoiceDate"].Value.ToString());
            //    dtpDeliveryDate.Value = Convert.ToDateTime(selectedRow.Cells["DeliveryDate"].Value.ToString());
            //    txtComments.Text = selectedRow.Cells["Comments"].Value.ToString();
            //    txtLCNumber.Text = selectedRow.Cells["LCNumber"].Value.ToString();
            //    dtpLCDate.Text = selectedRow.Cells["LCDate"].Value.ToString();
            //    txtLCBank.Text = selectedRow.Cells["LCBank"].Value.ToString();

            //    txtPINo.Text = selectedRow.Cells["PINo"].Value.ToString();
            //    dtpPIDate.Text = selectedRow.Cells["PIDate"].Value.ToString();
        }


    }
}
