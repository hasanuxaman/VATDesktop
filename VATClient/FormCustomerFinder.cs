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
using VATDesktop.Repo;
using VATServer.Interface;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormCustomerFinder : Form
    {
        public FormCustomerFinder()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        DataGridViewRow selectedRow = new DataGridViewRow();

        private DataTable CustomerResult;
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private void FormCustomerFinder_Load(object sender, EventArgs e)
        {
            customers();
        }
        private void customers()
        {
            #region Customer
            CustomerResult = new DataTable();
            //CustomerDAL customerDal = new CustomerDAL();
            ICustomer customerDal = OrdinaryVATDesktop.GetObject<CustomerDAL, CustomerRepo, ICustomer>(OrdinaryVATDesktop.IsWCF);

            //CustomerResult = customerDal.SearchCustomerSingleDTNew(txtCustCode.Text.Trim(), txtCustName.Text.Trim(), "",
            //"", "", "", "Y", ""); // Change 04

            string[] cValues = new string[] { OrdinaryVATDesktop.SanitizeInput(txtCustCode.Text.Trim()), OrdinaryVATDesktop.SanitizeInput(txtCustName.Text.Trim()), "Y" };
            string[] cFields = new string[] { "c.CustomerCode like", "c.CustomerName like", "c.ActiveStatus like", };
            CustomerResult = customerDal.SelectAll(null, cFields, cValues, null, null, false,connVM);
    
            int j = 0;
            dgvCustomerFinder.Rows.Clear();
            foreach (DataRow item2 in CustomerResult.Rows)
            {
                DataGridViewRow NewRow = new DataGridViewRow();
                dgvCustomerFinder.Rows.Add(NewRow);
                dgvCustomerFinder.Rows[j].Cells["CustomerID"].Value = item2["CustomerID"].ToString();// Convert.ToDecimal(BOMFields[0]);
                dgvCustomerFinder.Rows[j].Cells["CustomerCode"].Value = item2["CustomerCode"].ToString();// BOMFields[1].ToString();
                dgvCustomerFinder.Rows[j].Cells["CustomerName"].Value = item2["CustomerName"].ToString();// BOMFields[2].ToString();
                dgvCustomerFinder.Rows[j].Cells["Address1"].Value = item2["Address1"].ToString();//BOMFields[3].ToString();
                dgvCustomerFinder.Rows[j].Cells["VATRegistrationNo"].Value = item2["VATRegistrationNo"].ToString();//BOMFields[3].ToString();
                j = j + 1;

            }
            #endregion Customer
        }
        private void GridSelected()
        {
            try
            {
                if (dgvCustomerFinder.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                DataGridViewSelectedRowCollection selectedRows = dgvCustomerFinder.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }

 
                this.Close();
            }
            #region catch

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
            #endregion
        }

        public static DataGridViewRow SelectOne()
        {
            DataGridViewRow selectedRowTemp = null;
            string frmSearchSelectValue = String.Empty;
            try
            {
                FormCustomerFinder frm = new FormCustomerFinder();
                frm.ShowDialog();
                selectedRowTemp = frm.selectedRow;

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormDepositSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormDepositSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormDepositSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormDepositSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDepositSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDepositSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDepositSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormDepositSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDepositSearch", "SelectOne", exMessage);
            }
            #endregion
            return selectedRowTemp;
        }

        private void dgvCustomerFinder_DoubleClick(object sender, EventArgs e)
        {
            GridSelected();
        }

        private void txtCustName_TextChanged(object sender, EventArgs e)
        {
            customers();
        }

        private void txtCustCode_TextChanged(object sender, EventArgs e)
        {
            customers();
        }

        private void dgvCustomerFinder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSelected();

        }

        private void dgvCustomerFinder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                GridSelected();
            }
        }

        private void txtCustName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Down))
            {
                dgvCustomerFinder.Select();
            }
        }

        private void txtCustCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Down))
            {
                dgvCustomerFinder.Select();
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

    }
}
