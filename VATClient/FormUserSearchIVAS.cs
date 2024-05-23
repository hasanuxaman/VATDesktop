using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormUserSearchIVAS : Form
    {
        #region Global Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private string SelectedValue = string.Empty;
        private DataTable UserResult;
        private string activeStatus = string.Empty;

        #endregion

        public FormUserSearchIVAS()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        public static string SelectOne()
        {
            string searchValue = string.Empty;
            try
            {
                FormUserSearchIVAS frmUserSearch = new FormUserSearchIVAS();
                frmUserSearch.ShowDialog();
                searchValue = frmUserSearch.SelectedValue;
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
                MessageBox.Show(ex.Message, "FormUserSearc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormUserSearc", "Search", exMessage);
            }
            #endregion Catch

            return searchValue;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            try
            {
                activeStatus = string.Empty;

                activeStatus = cmbActive.SelectedIndex != -1 ? cmbActive.Text.Trim() : string.Empty;
                this.btnSearch.Enabled = false;
                this.progressBar1.Visible = true;
                bgwUser.RunWorkerAsync();


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
                FileLogger.Log(this.Name, "Search", exMessage);
            }
            #endregion Catch

        }


        private void ClearAllFields()
        {
            try
            {
                txtUserID.Text = "";
                txtUserName.Text = "";
                cmbActive.Text = "";
                dgvUserInformation.Rows.Clear();
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
                FileLogger.Log(this.Name, "ClearAllFields", exMessage);
            }
            #endregion Catch
        }

        private void bgwUser_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                UserInformationDAL userInformationDal = new UserInformationDAL();

                UserResult = userInformationDal.SearchIVASUserDataTable(txtUserID.Text.Trim(), txtUserName.Text.Trim(), activeStatus, connVM);

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
                FileLogger.Log(this.Name, "bgwUser_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwUser_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete

                dgvUserInformation.Rows.Clear();
                int j = 0;
                foreach (DataRow item2 in UserResult.Rows)
                {

                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvUserInformation.Rows.Add(NewRow);
                    dgvUserInformation.Rows[j].Cells["Select"].Value = false;// Convert.ToDecimal(UserFields[0]);
                    dgvUserInformation.Rows[j].Cells["UserID"].Value = item2["UserID"].ToString();// Convert.ToDecimal(UserFields[0]);
                    dgvUserInformation.Rows[j].Cells["UserName"].Value = item2["UserName"].ToString();//UserFields[1].ToString();
                    dgvUserInformation.Rows[j].Cells["ActiveStatusN"].Value = item2["ActiveStatus"].ToString();//UserFields[2].ToString();
                    dgvUserInformation.Rows[j].Cells["Password"].Value = item2["UserPassword"].ToString();//UserFields[3].ToString();
                    dgvUserInformation.Rows[j].Cells["FullName"].Value = item2["FullName"].ToString();//UserFields[3].ToString();
                  
                    j = j + 1;
                }

                // End Complete

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
                FileLogger.Log(this.Name, "bgwUser_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                LRecordCount.Text = "Record Count: " + dgvUserInformation.Rows.Count.ToString();
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void GridSelected()
        {
            try
            {
                if (dgvUserInformation.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                if (dgvUserInformation.Rows.Count > 0)
                {
                    string UserInfo = string.Empty;
                    int ColIndex = dgvUserInformation.CurrentCell.ColumnIndex;
                    int RowIndex1 = dgvUserInformation.CurrentCell.RowIndex;
                    if (RowIndex1 >= 0)
                    {
                        if (Program.fromOpen != "Me" &&
                            dgvUserInformation.Rows[RowIndex1].Cells["ActiveStatusN"].Value.ToString() != "Y")
                        {
                            MessageBox.Show("This Selected Item is not Active");
                            return;
                        }
                        string UserID = dgvUserInformation.Rows[RowIndex1].Cells["UserID"].Value.ToString();
                        string UserName = dgvUserInformation.Rows[RowIndex1].Cells["UserName"].Value.ToString();
                        string ActiveStatus = dgvUserInformation.Rows[RowIndex1].Cells["ActiveStatusN"].Value.ToString();
                        string Password = dgvUserInformation.Rows[RowIndex1].Cells["Password"].Value.ToString();
                        string FullName = dgvUserInformation.Rows[RowIndex1].Cells["FullName"].Value.ToString();

                        UserInfo = UserID + FieldDelimeter + UserName + FieldDelimeter + ActiveStatus + FieldDelimeter +
                            Password + FieldDelimeter + FullName;

                        SelectedValue = UserInfo;

                    }
                }
                this.Close();
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
                FileLogger.Log(this.Name, "GridSelected", exMessage);
            }
            #endregion Catch
        }

        private void dgvUserInformation_DoubleClick(object sender, EventArgs e)
        {
            GridSelected();
        }


    }
}
