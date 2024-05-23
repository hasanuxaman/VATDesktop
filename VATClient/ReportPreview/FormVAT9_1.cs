using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports;
using SymphonySofttech.Reports.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATClient.Integration.NBR;
using VATServer.Library;
using VATServer.License;
using VATViewModel.DTOs;

namespace VATClient.ReportPreview
{
    public partial class FormVAT9_1 : Form
    {
        private ReportDocument reportDocument = new ReportDocument();
        string post1, post2, vPostStatus = "Y";
        public FormVAT9_1()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
			 
        }

        #region Global Variables

        private DataSet dsVAT9_1 = new DataSet();
        private DataTable dtVATReturnHeader = new DataTable();

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private VATReturnVM varVATReturnVM = new VATReturnVM();
        private VATReturnHeaderVM varVATReturnHeaderVM = new VATReturnHeaderVM();
        private string PeriodStart;

        private int BranchId = 0;
        private string BranchName = "";

        private string[] sqlResults;


        #endregion

        private _9_1_VATReturnDAL _ReportDSDAL = new _9_1_VATReturnDAL();

        #region Form Load

        private void FormVAT9_1_Load(object sender, EventArgs e)
        {
            try
            {
                cmbPost.Text = "Y";
                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true,true,connVM);

                #region cmbBranch DropDownWidth Change
                string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth",null, connVM);
                cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
                #endregion

                List<BranchProfileVM> VMs = new List<BranchProfileVM>();
                VMs = new BranchProfileDAL().SelectAllList(null,null,null,null,null,connVM);

                if (VMs != null && VMs.Count > 0)
                {
                    if (VMs.Count == 1)
                    {
                        cmbBranch.Enabled = false;
                    }
                }


                #region Post Dropdown

                CommonDAL commonDal = new CommonDAL();

                string PreviewOnly = commonDal.settingsDesktop("Reports", "PreviewOnly",null ,connVM);

                if (PreviewOnly.ToLower() == "n")
                {
                    cmbPost.Text = "Y";
                    cmbPost.Visible = false;
                    label9.Visible = false;
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
                FileLogger.Log(this.Name, "FormVAT9_1_Load", exMessage);
            }
            #endregion

        }

        #endregion

        #region VATReturnHeader

        private void dtpDate_Leave(object sender, EventArgs e)
        {
            PeriodStart = dtpDate.Text;
            bgwVATReturnHeader.RunWorkerAsync();

        }

        private void bgwVATReturnHeader_DoWork(object sender, DoWorkEventArgs e)
        {
            varVATReturnVM = new VATReturnVM();
            varVATReturnVM.PeriodStart = PeriodStart;
            varVATReturnVM.post1 = post1;
            varVATReturnVM.post2 = post2;
            varVATReturnVM.PeriodName = dtpDate.Value.ToString("MMMM-yyyy");
            varVATReturnVM.BranchId = BranchId;
            varVATReturnVM.Date = dtpDate.Value.ToString("dd-MMM-yyyy");

            dtVATReturnHeader = _ReportDSDAL.SelectAll_VATReturnHeader(varVATReturnVM, connVM);//postStatus

        }

        private void bgwVATReturnHeader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            try
            {


                if (dtVATReturnHeader != null && dtVATReturnHeader.Rows.Count > 0)
                {

                    DataRow dr = dtVATReturnHeader.Rows[0];
                    cmbBranch.SelectedValue = dr["BranchId"].ToString();
                    //cmbBranch.Text = dr["BranchName"].ToString();
                    cmbPost.Text = dr["PostStatus"].ToString();
                    chbMainOReturn.Checked = Convert.ToBoolean(dr["MainOrginalReturn"].ToString() == "Y" ? true : false);
                    chbLateReturn.Checked = Convert.ToBoolean(dr["LateReturn"].ToString() == "Y" ? true : false);
                    chbAmendReturn.Checked = Convert.ToBoolean(dr["AmendReturn"].ToString() == "Y" ? true : false);
                    chbAlternativeReturn.Checked = Convert.ToBoolean(dr["FullAdditionalAlternativeReturn"].ToString() == "Y" ? true : false);
                    chbNoActivites.Checked = Convert.ToBoolean(dr["NoActivites"].ToString() == "Y" ? true : false);
                    chbIsTraderVAT.Checked = Convert.ToBoolean(dr["IsTraderVAT"].ToString() == "Y" ? true : false);
                    txtNoActivitesDetails.Text = dr["NoActivitesDetails"].ToString();
                    dtpSubmissionDate.Value = Convert.ToDateTime(dr["DateOfSubmission"].ToString());

                    #region Post Button

                    string Post = dr["Post"].ToString();

                    btnPost.Enabled = Post == "Y" ? false : true;

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
                FileLogger.Log(this.Name, "bgwVATReturnHeader_RunWorkerCompleted", exMessage);
            }
            #endregion Catch
        }

        #endregion

        #region Save

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.progressBar1.Visible = true;
                this.Enabled = false;

                PostStatus();
                BranchId = Convert.ToInt32(cmbBranch.SelectedValue);
                BranchName = cmbBranch.Text;

                #region Find Fiscal Year Lock
                string PeriodName = dtpDate.Value.ToString("MMMM-yyyy");
                string[] cValues = { PeriodName };
                string[] cFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues, null, null, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new ArgumentException(PeriodName + ": This Fiscal Period is not Exist!");
                }

                #endregion

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new ArgumentException(PeriodName + ": VAT Return (9.1) already submitted for this month!");
                }
                else
                {
                    ReportLoad();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.progressBar1.Visible = false;
                this.Enabled = true;
            }
            finally
            {

            }
        }

        private void ReportLoad()
        {
            try
            {
                sqlResults = new string[3];

                ckVAT9_1();

                #region VATReturnHeaders

                varVATReturnHeaderVM = new VATReturnHeaderVM();
                PeriodStart = dtpDate.Text;

                varVATReturnHeaderVM.BranchId = Convert.ToInt32(cmbBranch.SelectedValue);
                varVATReturnHeaderVM.BranchName = cmbBranch.Text;
                varVATReturnHeaderVM.PostStatus = cmbPost.Text.Trim();
                varVATReturnHeaderVM.MainOrginalReturn = chbMainOReturn.Checked;
                varVATReturnHeaderVM.LateReturn = chbLateReturn.Checked;
                varVATReturnHeaderVM.AmendReturn = chbAmendReturn.Checked;
                varVATReturnHeaderVM.AlternativeReturn = chbAlternativeReturn.Checked;
                varVATReturnHeaderVM.NoActivites = chbNoActivites.Checked;
                varVATReturnHeaderVM.NoActivitesDetails = txtNoActivitesDetails.Text.Trim();
                varVATReturnHeaderVM.DateOfSubmission = dtpSubmissionDate.Value.ToString("yyyy-MMM-dd");
                varVATReturnHeaderVM.PeriodStart = PeriodStart;
                varVATReturnHeaderVM.PeriodName = dtpDate.Value.ToString("MMMM-yyyy");
                varVATReturnHeaderVM.IsTraderVAT = chbIsTraderVAT.Checked;

                #region Find UserInfo

                DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + Program.CurrentUserID + "'");
                string UserName = userInfo[0]["FullName"].ToString();

                varVATReturnHeaderVM.UserName = UserName;
                varVATReturnHeaderVM.SignatoryName = userInfo[0]["FullName"].ToString();
                varVATReturnHeaderVM.SignatoryDesig = userInfo[0]["Designation"].ToString();
                varVATReturnHeaderVM.Email = userInfo[0]["Email"].ToString();
                varVATReturnHeaderVM.Mobile = userInfo[0]["ContactNo"].ToString();
                varVATReturnHeaderVM.NationalID = userInfo[0]["NationalID"].ToString();
                #endregion

                #endregion

                bgwLoad.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "ReportLoad", exMessage);
            }
            #endregion Catch
        }

        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                dsVAT9_1 = new DataSet();
                NBRReports _report = new NBRReports();

                #region VAT9_1_Complete

                varVATReturnVM = new VATReturnVM();
                varVATReturnVM.varVATReturnHeaderVM = varVATReturnHeaderVM;

                varVATReturnVM.PeriodStart = dtpDate.Text;
                varVATReturnVM.Post = vPostStatus;
                varVATReturnVM.post1 = post1;
                varVATReturnVM.post2 = post2;
                varVATReturnVM.PeriodName = dtpDate.Value.ToString("MMMM-yyyy");
                varVATReturnVM.BranchId = BranchId;
                varVATReturnVM.Date = dtpDate.Value.ToString("dd-MMM-yyyy");


                dsVAT9_1 = _ReportDSDAL.VAT9_1_CompleteSave(varVATReturnVM, connVM);//postStatus

                #endregion

                #region False / Apr-28-2020


                #endregion

                VATReturnSubFormVM subFormVM = new VATReturnSubFormVM();

                //subFormVM.PeriodName = varVATReturnVM.PeriodName;
                //subFormVM.PeriodId = dtpDate.Value.ToString("MMyyyy");
                //subFormVM.ExportInBDT = "Y";
                //subFormVM.post1 = post1;
                //subFormVM.post2 = post2;

                //var resultVm = _ReportDSDAL.SaveSubforms(subFormVM);
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
                FileLogger.Log(this.Name, "bgwLoad_DoWork", exMessage);
            }
            #endregion Catch

        }

        private void bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                bgwLoad1.RunWorkerAsync();
                //////Load_Complete();

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
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", exMessage);
            }
            //finally
            //{
            //    this.progressBar1.Visible = false;
            //    this.Enabled = true;
            //}
            #endregion Catch

        }

        #endregion

        #region Load

        private void btnLoad_Click(object sender, EventArgs e)
        {
            this.progressBar1.Visible = true;
            this.Enabled = false;
            PostStatus();
            BranchId = Convert.ToInt32(cmbBranch.SelectedValue);
            BranchName = cmbBranch.Text;
            bgwLoad1.RunWorkerAsync();

        }


        private void bgwLoad1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                dsVAT9_1 = new DataSet();

                varVATReturnVM = new VATReturnVM();
                varVATReturnVM.PeriodStart = dtpDate.Text;
                varVATReturnVM.Post = vPostStatus;
                varVATReturnVM.post1 = post1;
                varVATReturnVM.post2 = post2;
                varVATReturnVM.PeriodName = dtpDate.Value.ToString("MMMM-yyyy");
                varVATReturnVM.BranchId = BranchId;
                varVATReturnVM.Date = dtpDate.Value.ToString("dd-MMM-yyyy");


                dsVAT9_1 = _ReportDSDAL.VAT9_1_CompleteLoad(varVATReturnVM, connVM);//postStatus




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
                FileLogger.Log(this.Name, "bgwLoad_DoWork", exMessage);
            }
            #endregion Catch
        }

        private void bgwLoad1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Load_Complete();
                CellFormat9_1();

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
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", exMessage);
            }
            #endregion Catch
            finally
            {
                this.progressBar1.Visible = false;
                this.Enabled = true;
            }
        }
        DataTable Pivot(DataTable dt, DataColumn pivotColumn, DataColumn pivotValue)
        {
            // find primary key columns 
            //(i.e. everything but pivot column and pivot value)
            DataTable temp = dt.Copy();
            temp.Columns.Remove(pivotColumn.ColumnName);
            temp.Columns.Remove(pivotValue.ColumnName);
            string[] pkColumnNames = temp.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToArray();

            // prep results table
            DataTable result = temp.DefaultView.ToTable(true, pkColumnNames).Copy();
            result.PrimaryKey = result.Columns.Cast<DataColumn>().ToArray();
            dt.AsEnumerable()
                .Select(r => r[pivotColumn.ColumnName].ToString())
                .Distinct().ToList()
                .ForEach(c => result.Columns.Add(c, pivotColumn.DataType));

            // load it
            foreach (DataRow row in dt.Rows)
            {
                // find row to update
                DataRow aggRow = result.Rows.Find(
                    pkColumnNames
                        .Select(c => row[c])
                        .ToArray());
                // the aggregate used here is LATEST 
                // adjust the next line if you want (SUM, MAX, etc...)
                aggRow[row[pivotColumn.ColumnName].ToString()] = row[pivotValue.ColumnName];
            }

            return result;
        }

        private void Load_Complete()
        {

            DataTable dt = new DataTable();

            dt = dsVAT9_1.Tables[0];
            DataTable dt3 = new DataTable();

            if (dsVAT9_1 != null && dsVAT9_1.Tables.Count > 3)
            {

                dt3 = dsVAT9_1.Tables[3];
                if (dt3 != null && dt3.Rows.Count > 0)
                {

                    DataRow dr = dt3.Rows[0];
                    cmbBranch.SelectedValue = dr["BranchId"].ToString();
                    //cmbBranch.Text = dr["BranchName"].ToString();
                    cmbPost.Text = dr["PostStatus"].ToString();
                    chbMainOReturn.Checked = Convert.ToBoolean(dr["MainOrginalReturn"].ToString() == "Y" ? true : false);
                    chbLateReturn.Checked = Convert.ToBoolean(dr["LateReturn"].ToString() == "Y" ? true : false);
                    chbAmendReturn.Checked = Convert.ToBoolean(dr["AmendReturn"].ToString() == "Y" ? true : false);
                    chbAlternativeReturn.Checked = Convert.ToBoolean(dr["FullAdditionalAlternativeReturn"].ToString() == "Y" ? true : false);
                    chbNoActivites.Checked = Convert.ToBoolean(dr["NoActivites"].ToString() == "Y" ? true : false);
                    chbIsTraderVAT.Checked = Convert.ToBoolean(dr["IsTraderVAT"].ToString() == "Y" ? true : false);
                    txtNoActivitesDetails.Text = dr["NoActivitesDetails"].ToString();
                    vPostStatus = cmbPost.Text;
                    dtpSubmissionDate.Value = Convert.ToDateTime(dr["DateOfSubmission"].ToString());
                }
            }

            foreach (DataRow drr in dt.Rows)
            {
                drr["SubFormName"] = Program.AddSpacesToSentence(drr["SubFormName"].ToString());
               // drr["LineA"] = Program.ParseDecimalObject(Convert.ToDecimal(drr["LineA"].ToString())).ToString();
            }
            if (!dt.Columns.Contains("NoteNo"))
            {
                MessageBox.Show("There is no data to Load", this.Text, MessageBoxButtons.OK,
                                              MessageBoxIcon.Information);
                dt = new DataTable();
            }
            else
            {
                if (dt.Rows[0]["NoteNo"].ToString() != "1")
                {
                    MessageBox.Show("There is no data to Load", this.Text, MessageBoxButtons.OK,
                                                  MessageBoxIcon.Information);
                    dt = new DataTable();
                }

            }
            
            
            CultureInfo ci = CultureInfo.CreateSpecificCulture("en-US");
            dgvVAT9_1.Columns[2].ValueType = typeof(decimal);
            dgvVAT9_1.Columns[2].DefaultCellStyle.FormatProvider = ci;
            dgvVAT9_1.Columns[2].DefaultCellStyle.Format = "N2";

            dgvVAT9_1.Columns[3].ValueType = typeof(decimal);
            dgvVAT9_1.Columns[3].DefaultCellStyle.FormatProvider = ci;
            dgvVAT9_1.Columns[3].DefaultCellStyle.Format = "N2";

            dgvVAT9_1.Columns[4].ValueType = typeof(decimal);
            dgvVAT9_1.Columns[4].DefaultCellStyle.FormatProvider = ci;
            dgvVAT9_1.Columns[4].DefaultCellStyle.Format = "N2";



            dgvVAT9_1.DataSource = dt;
            int i = 0;
            foreach (DataGridViewRow row in dgvVAT9_1.Rows)
            {
                var tt = dgvVAT9_1["NoteDescription", i].Value.ToString();
                if (tt.ToLower().Contains("sum"))
                {
                    row.DefaultCellStyle.BackColor = Color.SkyBlue;
                }
                i++;
            }

        }


        private void CellFormat9_1()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Font = new Font(dgvVAT9_1.Font.FontFamily, 8, FontStyle.Regular);

            foreach (DataGridViewRow Myrow in dgvVAT9_1.Rows)
            {            //Here 2 cell is target value and 1 cell is Volume
                if (
                    Convert.ToInt32(Myrow.Cells["NoteNo"].Value) == 9
                    || Convert.ToInt32(Myrow.Cells["NoteNo"].Value) == 23
                    || Convert.ToInt32(Myrow.Cells["NoteNo"].Value) == 28
                    || Convert.ToInt32(Myrow.Cells["NoteNo"].Value) == 33
                    )// Or your condition 
                {
                    Myrow.DefaultCellStyle.BackColor = Color.Gray;

                    Myrow.DefaultCellStyle.ApplyStyle(style);

                }
                //else
                //{
                //    Myrow.DefaultCellStyle.BackColor = Color.Green;
                //}
            }
        }


        #endregion

        #region Print

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {

                this.progressBar1.Visible = true;
                this.Enabled = false;

                PostStatus();
                ckVAT9_1();


                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                BranchId = Convert.ToInt32(cmbBranch.SelectedValue);
                BranchName = cmbBranch.Text;

                //if (dsVAT9_1==null|| dsVAT9_1.Tables[0]==null ||  dsVAT9_1.Tables[0].Rows.Count<=0)
                //{
                //     MessageBox.Show("There is no data to preview\nPlease Load first", this.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                //print();

                bgwPrint.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "btnPrint_Click", exMessage);
            }
            #endregion Catch
        }

        private void bgwPrint_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                #region Find Fiscal Year Lock
                string PeriodName = dtpDate.Value.ToString("MMMM-yyyy");
                string[] cValues = { PeriodName };
                string[] cFields = { "PeriodName" };
                var YearLock = new FiscalYearDAL().SelectAll(0, cFields, cValues, null, null, connVM).FirstOrDefault();
                string PriodLock = YearLock.PeriodLock;
                #endregion

                dsVAT9_1 = new DataSet();
                NBRReports _report = new NBRReports();

                varVATReturnVM = new VATReturnVM();
                varVATReturnVM.PeriodStart = dtpDate.Text;
                varVATReturnVM.PeriodName = dtpDate.Value.ToString("MMMM-yyyy");
                varVATReturnVM.Post = vPostStatus;
                varVATReturnVM.post1 = post1;
                varVATReturnVM.post2 = post2;
                varVATReturnVM.BranchId = BranchId;
                varVATReturnVM.Date = dtpDate.Value.ToString("dd-MMM-yyyy");
                varVATReturnVM.PriodLock = PriodLock;

                reportDocument = _report.VAT9_1Print(varVATReturnVM, connVM);

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
                FileLogger.Log(this.Name, "bgwPrint_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwPrint_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                BranchId = Convert.ToInt32(cmbBranch.SelectedValue);
                BranchName = cmbBranch.Text;
                bgwLoad1.RunWorkerAsync();


                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    return;
                }

                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(reportDocument);
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
                FileLogger.Log(this.Name, "bgwPrint_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.btnSave.Enabled = true;
                this.btnLoad.Enabled = true;
                this.btnPrint.Enabled = true;
            }
        }


        #endregion

        #region Sub Forms

        private void btnSubForm_Click(object sender, EventArgs e)
        {
            FormSubForm frm = new FormSubForm();

            frm.dtpDate.Text = dtpDate.Text;

            frm.Show();
        }

        private void dgvVAT9_1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {


                var tt = dgvVAT9_1.CurrentCell.RowIndex;
                int columnIndex = e.ColumnIndex;

                if (columnIndex == 5)
                {

                    string NoteNo = dgvVAT9_1.Rows[e.RowIndex].Cells["NoteNo"].Value.ToString();
                    string SubFormName = dgvVAT9_1.Rows[e.RowIndex].Cells["SubFormName"].Value.ToString();
                     if (SubFormName.ToLower() == "-")
                    {
                        return;
                    }
                    FormSubForm frm = new FormSubForm();
                    frm.cmbNoteNo.Text = NoteNo;
                    frm.cmbPost.Text = vPostStatus;
                    frm.dtpDate.Value = dtpDate.Value;

                    frm.BranchId = BranchId;
                    frm.txtBranchName.Text = BranchName;
                    ////frm.ckbVAT9_1.Checked = ckbVAT9_1.Checked;

                    frm.Show();
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
                FileLogger.Log(this.Name, "dgvVAT9_1_CellContentClick", exMessage);
            }
            #endregion
        }

        #endregion

        #region MSC Functions

        private void PostStatus()
        {
            post1 = "y";
            post2 = "n";
            if (cmbPost.Text.Trim().ToLower() == "y")
            {
                post1 = "y";
                post2 = "y";
            }
            else if (cmbPost.Text.Trim().ToLower() == "n")
            {
                post1 = "n";
                post2 = "n";
            }
            vPostStatus = cmbPost.Text;

        }

        private void ckVAT9_1()
        {
            DateTime PeriodStart = Convert.ToDateTime(dtpDate.Text);

            DateTime HardDecember2019 = Convert.ToDateTime("December-2019");

            if (PeriodStart < HardDecember2019)
            {
                ckbVAT9_1.Checked = false;
            }
            else
            {
                ckbVAT9_1.Checked = true;
            }
        }

        #endregion


        #region Unused Methods

        #region Unused Methods

        #region Print Method

        private void print()
        {
            try
            {

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();


                ReportDocument objrpt = new ReportDocument();

                dsVAT9_1.Tables[0].TableName = "dtVATReturns";
                DataTable dtIncreasingAdjustment = new DataTable();
                DataTable dtDecreasingAdjustment = new DataTable();
                dtIncreasingAdjustment = dsVAT9_1.Tables[1];
                dtDecreasingAdjustment = dsVAT9_1.Tables[2];
                string IncreasingAdjustment = "";
                string DecreasingAdjustment = "";
                if (dsVAT9_1.Tables[1].Rows.Count > 0 && dsVAT9_1.Tables[1] != null)
                {
                    foreach (DataRow item in dsVAT9_1.Tables[1].Rows)
                    {
                        IncreasingAdjustment = IncreasingAdjustment + "~" + item["AdjName"].ToString();
                    }
                }

                if (IncreasingAdjustment.Length > 0)
                {
                    IncreasingAdjustment = IncreasingAdjustment.Substring(1, IncreasingAdjustment.Length - 1);
                }


                if (dsVAT9_1.Tables[2].Rows.Count > 0 && dsVAT9_1.Tables[2] != null)
                {
                    foreach (DataRow item in dsVAT9_1.Tables[2].Rows)
                    {
                        DecreasingAdjustment = DecreasingAdjustment + "~" + item["AdjName"].ToString();
                    }
                }

                if (DecreasingAdjustment.Length > 0)
                {
                    DecreasingAdjustment = DecreasingAdjustment.Substring(1, DecreasingAdjustment.Length - 1);
                }

                //objrpt = new RptVAT9_1();
                objrpt.Load(Program.ReportAppPath + @"\RptVAT9_1.rpt");


                objrpt.SetDataSource(dsVAT9_1);

                CompanyprofileDAL cdal = new CompanyprofileDAL();
                CompanyProfileVM cvm = new CompanyProfileVM();

                CommonDAL commonDal = new CommonDAL();
                string TotalDepositVAT = commonDal.settingsDesktop("OperationalCode", "TotalDepositVAT",null, connVM);
                string TotalDepositSD = commonDal.settingsDesktop("OperationalCode", "TotalDepositSD", null, connVM);
                string InterestOnOveredVATDeposit = commonDal.settingsDesktop("OperationalCode", "InterestOnOveredVATDeposit", null, connVM);
                string InterestOnOveredSDDeposit = commonDal.settingsDesktop("OperationalCode", "InterestOnOveredSDDeposit", null, connVM);
                string FineOrPenaltyDeposit = commonDal.settingsDesktop("OperationalCode", "FineOrPenaltyDeposit", null, connVM);
                string ExciseDutyDeposit = commonDal.settingsDesktop("OperationalCode", "ExciseDutyDeposit", null, connVM);
                string DevelopmentSurchargeDeposit = commonDal.settingsDesktop("OperationalCode", "DevelopmentSurchargeDeposit", null, connVM);
                string ICTDevelopmentSurchargeDeposit = commonDal.settingsDesktop("OperationalCode", "ICTDevelopmentSurchargeDeposit", null, connVM);
                string HelthCareSurchargeDeposit = commonDal.settingsDesktop("OperationalCode", "HelthCareSurchargeDeposit", null, connVM);
                string EnvironmentProtectionSurchargeDeposit = commonDal.settingsDesktop("OperationalCode", "EnvironmentProtectionSurchargeDeposit", null, connVM);




                objrpt.DataDefinition.FormulaFields["IncreasingAdjustment"].Text = "'" + IncreasingAdjustment + "'";
                objrpt.DataDefinition.FormulaFields["DecreasingAdjustment"].Text = "'" + DecreasingAdjustment + "'";

                objrpt.DataDefinition.FormulaFields["TotalDepositVAT"].Text = "'" + TotalDepositVAT + "'";
                objrpt.DataDefinition.FormulaFields["TotalDepositSD"].Text = "'" + TotalDepositSD + "'";
                objrpt.DataDefinition.FormulaFields["InterestOnOveredVATDeposit"].Text = "'" + InterestOnOveredVATDeposit + "'";
                objrpt.DataDefinition.FormulaFields["InterestOnOveredSDDeposit"].Text = "'" + InterestOnOveredSDDeposit + "'";
                objrpt.DataDefinition.FormulaFields["FineOrPenaltyDeposit"].Text = "'" + FineOrPenaltyDeposit + "'";
                objrpt.DataDefinition.FormulaFields["ExciseDutyDeposit"].Text = "'" + ExciseDutyDeposit + "'";
                objrpt.DataDefinition.FormulaFields["DevelopmentSurchargeDeposit"].Text = "'" + DevelopmentSurchargeDeposit + "'";
                objrpt.DataDefinition.FormulaFields["ICTDevelopmentSurchargeDeposit"].Text = "'" + ICTDevelopmentSurchargeDeposit + "'";
                objrpt.DataDefinition.FormulaFields["HelthCareSurchargeDeposit"].Text = "'" + HelthCareSurchargeDeposit + "'";
                objrpt.DataDefinition.FormulaFields["EnvironmentProtectionSurchargeDeposit"].Text = "'" + EnvironmentProtectionSurchargeDeposit + "'";

                cvm = cdal.SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["CompanyAddress"].Text = "'" + cvm.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["AccountType"].Text = "'" + cvm.AccountingNature + "'";
                objrpt.DataDefinition.FormulaFields["BIN"].Text = "'" + cvm.BIN + "'";
                objrpt.DataDefinition.FormulaFields["BusinessType"].Text = "'" + cvm.BusinessNature + "'";
                objrpt.DataDefinition.FormulaFields["PeriodName"].Text = "'" + dtpDate.Value.ToString("MMMM  /  yyyy") + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);

                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();

                //string query = "SELECT DISTINCT ShipCountry FROM Orders";
                //DataTable dt = dsVAT9_1.Tables[0];
                //chart1.DataSource = dt;
                //chart1.Series[0].ChartType = (SeriesChartType)int.Parse(rblChartType.SelectedItem.Value);
                //chart1.Legends[0].Enabled = true;
                //chart1.Series[0].XValueMember = "ShipCity";
                //chart1.Series[0].YValueMembers = "Total";
                //chart1.DataBind();
                //chart1.DataSource = dsVAT9_1.Tables[0];
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
                FileLogger.Log(this.Name, "bgwPrint_RunWorkerCompleted", exMessage);
            }
            #endregion
        }

        #endregion

        #endregion

        #region Unused Methods

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnLoad1_Click(object sender, EventArgs e)
        {

        }

        private void dgvVAT9_1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //DataGridViewCellStyle style = new DataGridViewCellStyle();
            //style.Font = new Font(dgvVAT9_1.Font.FontFamily, 10, FontStyle.Bold);

            //foreach (DataGridViewRow Myrow in dgvVAT9_1.Rows)
            //{
               // Myrow.Cells["NoteNo"].DefaultCellStyle.Format = "N2";
              //  Myrow.Columns("NoteNo").DefaultCellStyle.Format = "##,0";
                //Here 2 cell is target value and 1 cell is Volume
                //if (
                //    Convert.ToInt32(Myrow.Cells["NoteNo"].Value) == 9
                //    || Convert.ToInt32(Myrow.Cells["LineA"].Value) == 23
                //    || Convert.ToInt32(Myrow.Cells["LineB"].Value) == 28
                //    || Convert.ToInt32(Myrow.Cells["LineC"].Value) == 33
                //    )// Or your condition 
                //{
                //    Myrow.DefaultCellStyle.BackColor = Color.Gray;

                //    Myrow.DefaultCellStyle.ApplyStyle(style);

                //}
                //else
                //{
                //    Myrow.DefaultCellStyle.BackColor = Color.Green;
                //}
            //}
        }

        private void chkPost_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkPost_Click(object sender, EventArgs e)
        {


        }


        #endregion

        #region Unused Methods

        private void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {

                string[] retResults = new string[4];

                BranchId = Convert.ToInt32(cmbBranch.SelectedValue);


                if (BranchId == 0)
                {
                    cmbBranch.Focus();
                    throw new ArgumentNullException("", "Please Select Branch for VAT Process!");
                }

                 
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }

                MessageBox.Show(retResults[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);




                ////if (retResults[0] != "Fail")
                ////{
                ////    ////Call 9.1_Report
                ////}

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
                FileLogger.Log(this.Name, "btnProcess_Click", exMessage);
            }
            #endregion
        }

        private void btnLoadNew_Click(object sender, EventArgs e)
        {
            try
            {

                dsVAT9_1 = new DataSet();
 

                Load_Complete();


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
                FileLogger.Log(this.Name, "btnLoadNew_Click", exMessage);
            }
            #endregion
        }

        #endregion

        #endregion


        private void btnPost_Click(object sender, EventArgs e)
        {
            #region try
            try
            {

                if (MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text, MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                #region Find UserInfo
                varVATReturnHeaderVM = new VATReturnHeaderVM();

                DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + Program.CurrentUserID + "'");
                varVATReturnHeaderVM.SignatoryName = userInfo[0]["FullName"].ToString();
                varVATReturnHeaderVM.SignatoryDesig = userInfo[0]["Designation"].ToString();
                varVATReturnHeaderVM.Email = userInfo[0]["Email"].ToString();
                varVATReturnHeaderVM.Mobile = userInfo[0]["ContactNo"].ToString();
                varVATReturnHeaderVM.NationalID = userInfo[0]["NationalID"].ToString();
                #endregion
                ResultVM rVM = new ResultVM();

                VATReturnVM vm = new VATReturnVM();
                vm.PeriodName = dtpDate.Value.ToString("MMMM-yyyy");
                vm.varVATReturnHeaderVM = varVATReturnHeaderVM;
                rVM = _ReportDSDAL.Post(vm,null,null,connVM);

                MessageBox.Show(rVM.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
            }
            #endregion
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnAttachmentUpload_Click(object sender, EventArgs e)
        {
            try
            {
                string PeriodName = dtpDate.Value.ToString("MMMM-yyyy");
                string[] cValues = { PeriodName };
                string[] cFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues, null, null, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new ArgumentException(PeriodName + ": This Fiscal Period is not Exist!");
                }

                FormVAT9_1FileUpload frm = new FormVAT9_1FileUpload();

                frm.txtPeriodName.Text = varFiscalYearVM.PeriodName;
                frm.txtPeriodId.Text = varFiscalYearVM.PeriodID;

                frm.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text);
            }

        }

        private void txtSubmit_Click(object sender, EventArgs e)
        {
            try
            {


                //////string Message = "Do you want to submit VAT return (9.1) for month of " + dtpDate.Value.ToString("MMMM-yyyy");
                //////DialogResult dlgRes = MessageBox.Show(Message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //////if (dlgRes != DialogResult.Yes)
                //////{
                //////    return;
                //////}

                bool IsIVASUser = FormLoginIVAS.SelectOne();

                if (IsIVASUser)
                {
                    FormNBRAPI form = new FormNBRAPI();
                    form.dptMonth.Value = dtpDate.Value;
                    form.txtPeriodName.Text = dtpDate.Value.ToString("MMMM-yyyy");
                    form.ShowDialog();
                }

            }
            catch (Exception exception)
            {
                FileLogger.Log("FormVAT9_1", "txtSubmit_Click", exception.ToString());
                MessageBox.Show(exception.Message);
            }
        }

    }
}
