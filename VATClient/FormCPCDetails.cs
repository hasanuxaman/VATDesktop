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
////
using VATClient.ReportPreview;
using CrystalDecisions.Shared;
using VATClient.ReportPages;
using System.Drawing.Printing;
using System.Threading;
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Ordinary;

namespace VATClient
{
    public partial class FormCPCDetails : Form
    {
        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string[] sqlResults;
        string NextID;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DataTable YearResult;
        private int YearLines;

        DataSet ds;
        DataTable dt;

        #endregion
        public FormCPCDetails()
        {
            InitializeComponent();
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {

                Program.fromOpen = "Me";
                string result = FormCPCDetailsSearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] CPCDetailesInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtId.Text = CPCDetailesInfo[0];
                    //HSCodeDAL HDAL = new HSCodeDAL();
                    ICPCDetails HDAL = OrdinaryVATDesktop.GetObject<CPCDetailsDAL, CPCDetailsRepo, ICPCDetails>(OrdinaryVATDesktop.IsWCF);

                    DataTable CPCDetailsResult = new DataTable();

                    string[] cFields = { "Id" };
                    string[] cValues = { CPCDetailesInfo[0] };
                    CPCDetailsResult = HDAL.SelectAll(0, cFields, cValues, null, null, true, connVM);
                    txtId.Text = CPCDetailsResult.Rows[0]["Id"].ToString();
                    txtCode.Text = CPCDetailsResult.Rows[0]["Code"].ToString();
                    txtName.Text = CPCDetailsResult.Rows[0]["Name"].ToString();// HSCodeInfo[2];
                    cmbType.Text = CPCDetailsResult.Rows[0]["Type"].ToString();// HSCodeInfo[2];

                   
             
                }
            }

            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }
            #endregion Catch

            finally
            {
               
            }

        }
        private void ClearAll()
        {
            #region Try
            try
            {
                txtCode.Text = "";
                txtName.Text = "";
                cmbType.Text = "Select";
                txtId.Text = string.Empty; ;
               
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
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {
                ClearAll();
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

       
        private void FormHsCode_Load(object sender, EventArgs e)
        {
            #region Try
            try
            {
               
                ClearAll();

                CommonDAL commonDal = new CommonDAL();

                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "CPC") == "Y" ? true : false);
                if(Auto)
                {
                    label3.Visible=false;
                }
                else
                {
                    label3.Visible = true;

                }
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {

                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }
                
                NextID = txtId.Text.Trim();
                this.btnAdd.Visible = true;
                this.progressBar1.Visible = true;

                CPCDetailsVM vm = new CPCDetailsVM();
                vm.Code = txtCode.Text.Trim();
                vm.Name = txtName.Text.Trim();
                vm.Type = cmbType.Text.Trim();
                vm.CreatedBy = Program.CurrentUser;
                vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                bgwSave.RunWorkerAsync(vm);
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

        private void bgwSave_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try
            try
            {
                //HSCodeDAL Hdal = new HSCodeDAL();
                ICPCDetails Hdal = OrdinaryVATDesktop.GetObject<CPCDetailsDAL, CPCDetailsRepo, ICPCDetails>(OrdinaryVATDesktop.IsWCF);
                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];

                CPCDetailsVM vm = (CPCDetailsVM)e.Argument;

                sqlResults = Hdal.InsertToCPCDetails(vm,false, connVM);
                SAVE_DOWORK_SUCCESS = true;
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

        private void bgwSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try
            try
            {
                #region Statement

                if (SAVE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        string code = sqlResults[3];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwSave_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        txtId.Text = newId;
                        txtCode.Text = code;

                    }

                this.btnAdd.Visible = true;
                this.progressBar1.Visible = false;
              
                #endregion
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {


            int ErR = ErrorReturn();
            if (ErR != 0)
            {
                return;
            }
            NextID = txtId.Text.Trim();
            this.btnUpdate.Visible = true;
            this.progressBar1.Visible = true;

            CPCDetailsVM vm = new CPCDetailsVM();
            vm.Id = Convert.ToInt32(NextID);
            vm.Code = txtCode.Text.Trim();
            vm.Name = txtName.Text.Trim();
            vm.Type = cmbType.Text.Trim();
            vm.LastModifiedBy = Program.CurrentUser;
            vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");


            bgwUpdate.RunWorkerAsync(vm);
        }

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
           
            #region Try

            try
            {

                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                //HSCodeDAL Hdal = new HSCodeDAL();
                ICPCDetails Hdal = OrdinaryVATDesktop.GetObject<CPCDetailsDAL, CPCDetailsRepo, ICPCDetails>(OrdinaryVATDesktop.IsWCF);


                CPCDetailsVM vm = (CPCDetailsVM)e.Argument;
                sqlResults = Hdal.UpdateCPCDetails(vm, connVM);
                UPDATE_DOWORK_SUCCESS = true;
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

        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try
            try
            {
                #region Statement

                if (UPDATE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        string code = sqlResults[3];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwUpdate_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        txtId.Text = newId;
                        txtCode.Text = code;

                    }
                         this.btnUpdate.Visible = true;
                         this.progressBar1.Visible = false;
              

                #endregion
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

        private void bgwDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try
            try
            {
                #region Statement

               

                // Start DoWork
                DELETE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];
                //HSCodeDAL Hdal = new HSCodeDAL();
                IHSCode Hdal = OrdinaryVATDesktop.GetObject<HSCodeDAL, HSCodeRepo, IHSCode>(OrdinaryVATDesktop.IsWCF);
                HSCodeVM vm = new HSCodeVM();
                vm.LastModifiedBy = Program.CurrentUser;
                vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                string[] ids = new string[] { txtId.Text.Trim(), "" };

                sqlResults = Hdal.Delete(vm, ids,null,null,connVM); 
                
                // End DoWork
                DELETE_DOWORK_SUCCESS = true;

                #endregion
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

        private void bgwDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try
            try
            {
                #region Statement
                if (DELETE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwDelete_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearAll();
                        }
                        this.btnDelete.Visible = true;
                        this.progressBar1.Visible = false;
                        this.btnDelete.Enabled = true;
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
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            #region try 
            try
            {
                if (txtId.Text.Trim() == "")
                {
                    MessageBox.Show("No data deleted." + "\n" + "Please select existing information first", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (txtId.Text.Trim() == "0")
                {
                    MessageBox.Show("This is Default data ." + "\n" + "This can't be deleted", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (
                    MessageBox.Show("Do you want to delete data?", this.Text, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                this.btnDelete.Enabled = false;
                this.progressBar1.Visible = true;
                bgwDelete.RunWorkerAsync();
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

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                Program.ImportFileName = null;

                if (string.IsNullOrWhiteSpace(Program.ImportFileName))
                {
                    BrowsFile();
                }
                string fileName = Program.ImportFileName;

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    //MessageBox.Show(this, "Please select the right file for import");
                    return;
                }
                else
                {
                    string[] extention = fileName.Split(".".ToCharArray());

                    if (extention[extention.Length - 1] == "xls" || extention[extention.Length - 1] == "xlsx")
                    {
                        ds = loadExcel();
                        dt = ds.Tables["HSCode"];

                        Program.ImportFileName = null;

                        //HSCodeDAL HSCodeDal = new HSCodeDAL();
                        IHSCode HSCodeDal = OrdinaryVATDesktop.GetObject<HSCodeDAL, HSCodeRepo, IHSCode>(OrdinaryVATDesktop.IsWCF);

                        string Createdby = Program.CurrentUser;
                        string Createdon = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            HSCodeVM vm = new HSCodeVM();

                            vm.HSCode = dt.Rows[i]["HSCode"].ToString();
                            vm.CD =Convert.ToDecimal(dt.Rows[i]["CD"].ToString());
                            vm.VAT = Convert.ToDecimal(dt.Rows[i]["VAT"].ToString());
                            vm.SD = Convert.ToDecimal(dt.Rows[i]["SD"].ToString());
                            vm.AIT = Convert.ToDecimal(dt.Rows[i]["AIT"].ToString());
                            vm.RD = Convert.ToDecimal(dt.Rows[i]["RD"].ToString());
                            vm.AT = Convert.ToDecimal(dt.Rows[i]["AT"].ToString());
                            vm.OtherSD = Convert.ToDecimal(dt.Rows[i]["OtherSD"].ToString());
                            vm.OtherVAT = Convert.ToDecimal(dt.Rows[i]["OtherVAT"].ToString());
                            vm.IsFixedVAT = dt.Rows[i]["IsFixedVAT"].ToString();
                            vm.IsFixedSD = dt.Rows[i]["IsFixedSD"].ToString();
                            vm.IsFixedCD = dt.Rows[i]["IsFixedCD"].ToString();
                            vm.IsFixedRD = dt.Rows[i]["IsFixedRD"].ToString();
                            vm.IsFixedAIT = dt.Rows[i]["IsFixedAIT"].ToString();
                            vm.IsFixedAT = dt.Rows[i]["IsFixedAT"].ToString();
                            vm.IsFixedOtherVAT = dt.Rows[i]["IsFixedOtherVAT"].ToString();
                            vm.IsFixedOtherSD = dt.Rows[i]["IsFixedOtherSD"].ToString();
                            vm.Description = dt.Rows[i]["Description"].ToString();
                            vm.Comments = dt.Rows[i]["Comments"].ToString();

                            vm.CreatedBy = Createdby;
                            vm.CreatedOn = Createdon;

                            vm.LastModifiedBy = Createdby;
                            vm.LastModifiedOn = Createdon;


                            sqlResults = HSCodeDal.InsertfromExcel(vm,connVM);
                            
                        }

                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (extention[extention.Length - 1] != "xls" || extention[extention.Length - 1] != "xlsx")
                    {
                        MessageBox.Show("You can select Excel files only");
                    }

                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private DataSet loadExcel()
        {
            DataSet ds = new DataSet();
            try
            {
                ImportVM paramVm = new ImportVM();
                paramVm.FilePath = Program.ImportFileName;
                //IHSCode HSCodeDal = OrdinaryVATDesktop.GetObject<HSCodeDAL, HSCodeRepo, IHSCode>(OrdinaryVATDesktop.IsWCF);
                ds = new ImportDAL().GetDataSetFromExcel(paramVm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            return ds;
        }

        private void BrowsFile()
        {
            try
            {

                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "HSCode Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";
                fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*";
                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
                fdlg.Multiselect = true;
                int count = 0;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in fdlg.FileNames)
                    {
                        //Program.ImportFileName = fdlg.FileName;
                        if (count == 0)
                        {
                            Program.ImportFileName = file;
                        }
                        else
                        {
                            Program.ImportFileName = Program.ImportFileName + " ; " + file;
                        }
                        count++;
                    }
                }

                else
                {
                    return;
                }



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
                FileLogger.Log(this.Name, "BrowsFile", exMessage);
            }

        }

        

        private void bgwSelectYear_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                // Start DoWork
                YearResult = new DataTable();
                //FiscalYearDAL fiscalYearDal = new FiscalYearDAL();
                IFiscalYear fiscalYearDal = OrdinaryVATDesktop.GetObject<FiscalYearDAL, FiscalYearRepo, IFiscalYear>(OrdinaryVATDesktop.IsWCF);
                YearResult = fiscalYearDal.SearchYear(connVM);
                // End DoWork
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
                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSelectYear_DoWork", exMessage);
            }
            #endregion

        }

        private void bgwSelectYear_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                YearLines = 0;
                YearLines = YearResult.Rows.Count;
                cmbType.Items.Clear();
                for (int j = 0; j < Convert.ToInt32(YearLines); j++)
                {
                    cmbType.Items.Add(YearResult.Rows[j][0].ToString());
                }
                cmbType.Text = (DateTime.Now.ToString("yyyy"));
                // End Complete
                #endregion
                //////ChangeData = false;
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
                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSelectYear_RunWorkerCompleted", exMessage);
            }
            #endregion
        }

       
    

        private int ErrorReturn()
        {
            #region try

            try
            {
                CommonDAL commonDal = new CommonDAL();

               bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "CPC") == "Y" ? true : false);

               if (!Auto)
               {
                   if (string.IsNullOrWhiteSpace(txtCode.Text.Trim()))
                   {
                       MessageBox.Show("Please enter Code");
                       txtCode.Focus();
                       return 1;
                   }
               }
               
                if (string.IsNullOrWhiteSpace(txtName.Text.Trim()))
                {
                    MessageBox.Show("Please enter Name");
                    txtName.Focus();
                    return 1;
                }

                if (cmbType.Text.Trim().ToLower() == "select")
                {
                    MessageBox.Show("Please enter Type");
                    cmbType.Focus();
                    return 1;
                }
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
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
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
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }

            #endregion

            return 0;
        }



    }
}
