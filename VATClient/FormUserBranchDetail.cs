using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATDesktop.Repo;
using VATServer.Interface;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormUserBranchDetail : Form
    {
        #region Global variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DataTable dtUserBranchDetailResult = new DataTable();
        List<UserBranchDetailVM> Details;
        private string[] sqlResults;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
       

        string loadTable;
        DataTable dt;
        DataSet ds;

        #endregion
        public FormUserBranchDetail()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnUserSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string result = FormUserSearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else
                {
                    string[] UserInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtUserID.Text = UserInfo[0];
                    txtUserName.Text = UserInfo[1];
                }
                this.progressBar1.Visible = true;
                bgwUserSearch.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bgwUserSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(txtUserID.Text);
                IUserBranchDetail userBranchDetailDal = OrdinaryVATDesktop.GetObject<UserBranchDetailDAL, UserBranchDetailRepo, IUserBranchDetail>(OrdinaryVATDesktop.IsWCF);

                dtUserBranchDetailResult = userBranchDetailDal.SelectAll(userId, null, null, null, null, true,connVM);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bgwUserSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                dgvUserGrid.Rows.Clear();
                int j = 0;
                foreach (DataRow dr in dtUserBranchDetailResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvUserGrid.Rows.Add(NewRow);

                    dgvUserGrid.Rows[j].Cells["Id"].Value = dr["Id"].ToString();
                    dgvUserGrid.Rows[j].Cells["BranchId"].Value = dr["BranchId"].ToString();
                    dgvUserGrid.Rows[j].Cells["Comments"].Value = dr["Comments"].ToString();
                    dgvUserGrid.Rows[j].Cells["UserName"].Value = dr["UserName"].ToString();
                    dgvUserGrid.Rows[j].Cells["UserId"].Value = dr["UserId"].ToString();
                    dgvUserGrid.Rows[j].Cells["BranchName"].Value = dr["BranchName"].ToString();

                    j++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private void FormUserBranchDetail_Load(object sender, EventArgs e)
        {
            try
            {
                #region ToolTip
                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();            
                ToolTip1.SetToolTip(this.btnImport, "Import from Excel file");                
                #endregion

                string[] Condition = new string[] { "ActiveStatus='Y'" };

                cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar",true,false,connVM);

                #region cmbBranch DropDownWidth Change
                string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth",null,connVM);
                cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
                #endregion


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                #region Null check
                if (txtUserID.Text == "")
                {
                    MessageBox.Show("Select a user first !", this.Text);
                    return;
                }
                if (txtComments.Text == "")
                {
                    txtComments.Text = "NA";
                }
                for (int i = 0; i < dgvUserGrid.RowCount; i++)
                {
                    if (dgvUserGrid.Rows[i].Cells["BranchId"].Value.ToString() == cmbBranch.SelectedValue.ToString())
                    {
                        MessageBox.Show("Same Branch already exist.", this.Text);
                        return;
                    }
                }

                DataGridViewRow DNewRow = new DataGridViewRow();
                dgvUserGrid.Rows.Add(DNewRow);

                dgvUserGrid["Id", dgvUserGrid.RowCount - 1].Value = "0";
                dgvUserGrid["BranchId", dgvUserGrid.RowCount - 1].Value = cmbBranch.SelectedValue.ToString();
                dgvUserGrid["Comments", dgvUserGrid.RowCount - 1].Value = txtComments.Text;
                dgvUserGrid["UserName", dgvUserGrid.RowCount - 1].Value = txtUserName.Text;
                dgvUserGrid["UserId", dgvUserGrid.RowCount - 1].Value = txtUserID.Text;
                dgvUserGrid["BranchName", dgvUserGrid.RowCount - 1].Value = cmbBranch.Text;

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvUserGrid.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nBranch : " + dgvUserGrid.CurrentRow.Cells["BranchName"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvUserGrid.Rows.RemoveAt(dgvUserGrid.CurrentRow.Index);
                    }
                }
                else
                {
                    MessageBox.Show("No Items Found in Remove.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvUserGrid.Rows.Count <= 0)
            {
                MessageBox.Show("No Branch added !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            this.progressBar1.Visible = true;
            bgwUserSave.RunWorkerAsync();
        }

        private void bgwUserSave_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                IUserBranchDetail dal = OrdinaryVATDesktop.GetObject<UserBranchDetailDAL, UserBranchDetailRepo, IUserBranchDetail>(OrdinaryVATDesktop.IsWCF);

                #region Process models
                Details = new List<UserBranchDetailVM>();
                for (int i = 0; i < dgvUserGrid.RowCount; i++)
                {
                    UserBranchDetailVM DetailVm = new UserBranchDetailVM();
                    DetailVm.Id = Convert.ToInt32(dgvUserGrid.Rows[i].Cells["Id"].Value.ToString());
                    DetailVm.UserId = Convert.ToInt32(dgvUserGrid.Rows[i].Cells["UserId"].Value.ToString());
                    DetailVm.BranchId = Convert.ToInt32(dgvUserGrid.Rows[i].Cells["BranchId"].Value.ToString());
                    DetailVm.Comments = dgvUserGrid.Rows[i].Cells["Comments"].Value.ToString();
                    DetailVm.CreatedBy = Program.CurrentUser;
                    DetailVm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    DetailVm.LastModifiedBy = Program.CurrentUser;
                    DetailVm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    Details.Add(DetailVm);
                }
	            #endregion
                sqlResults = dal.Insert(Details, null, null,connVM);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void bgwUserSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                string result = sqlResults[0];
                if (string.IsNullOrEmpty(result))
                {
                    throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                    "Unexpected error.");
                }
                if (result == "Success")
                {
                    MessageBox.Show("Branches added to user successfully ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Could not add Branches to the User ! ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvUserGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
                    MessageBox.Show(this, "Please select the right file for import");
                }
                else
                {

                    string[] extention = fileName.Split(".".ToCharArray());
                    if (extention[extention.Length - 1] == "xls" || extention[extention.Length - 1] == "xlsx")
                    {
                        ds = loadExcel();

                        dt = ds.Tables["UserInformation"];
                        //dgvUserGrid.DataSource = dt;

                        Program.ImportFileName = null;

                        //UserBranchDetailDAL dal = new UserBranchDetailDAL();
                        IUserBranchDetail dal = OrdinaryVATDesktop.GetObject<UserBranchDetailDAL, UserBranchDetailRepo, IUserBranchDetail>(OrdinaryVATDesktop.IsWCF);


                        //Details = new List<UserBranchDetailVM>();
                        //    for (int i = 0; i < dt.Rows.Count; i++)
                        //    {
                        //        UserBranchDetailVM DetailVm = new UserBranchDetailVM();
                        //        //DetailVm.Id = Convert.ToInt32(dt.Rows[i]["Id"].ToString());
                        //        //DetailVm.UserId = Convert.ToInt32(dt.Rows[i]["UserId"].ToString());
                        //        DetailVm.BranchName = dt.Rows[i]["BranchName"].ToString();
                        //        DetailVm.BranchCode = dt.Rows[i]["BranchCode"].ToString();
                        //        //DetailVm.Comments = dt.Rows[i]["Comments"].ToString();
                        //        DetailVm.CreatedBy = Program.CurrentUser;
                        //        DetailVm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                        //        DetailVm.LastModifiedBy = Program.CurrentUser;
                        //        DetailVm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                        //        Details.Add(DetailVm);
                        //    }

                        //sqlResults = dal.InsertfromExcel(dt, Details, null, null);

                        string CreatedBy = Program.CurrentUser;
                        string Createdon = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                        sqlResults = dal.InsertfromExcel(dt, null, null, CreatedBy, Createdon,connVM);

                        if (sqlResults[0] == "Success")
                        {
                            MessageBox.Show("Data Import successfully", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //dtUserBranchDetailResult=dal.SelectAll()
                        }
                    }

                    else
                    {
                        MessageBox.Show("You can select Excel files only");
                    }


                }
                
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            

        }

        private DataSet loadExcel()
        {
            DataSet ds = new DataSet();
            try
            {
                IImport dal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);

                ImportVM paramVm = new ImportVM();
                paramVm.FilePath = Program.ImportFileName;
                ds = dal.GetDataSetFromExcel(paramVm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
          

            return ds;
        }

        private void BrowsFile()
        {
             #region try
            try
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "User Information Import Open File Dialog";
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
            }
            #endregion
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
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

        private void btnImport_Click_1(object sender, EventArgs e)
        {
            FormBranchImport fmi = new FormBranchImport();
            fmi.Show(this);
        }
        

    }
}
