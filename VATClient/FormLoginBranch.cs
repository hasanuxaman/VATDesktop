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
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormLoginBranch : Form
    {
        #region Global variables
      //  private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private DataTable dtUserBranchResult = new DataTable();
        private string SelectedValue = string.Empty;
        #endregion
        public FormLoginBranch()
        {
            InitializeComponent();
           // connVM = Program.OrdinaryLoad();
        }

        private void FormLoginBranch_Load(object sender, EventArgs e)
        {
            try
            {
                this.progressBar1.Visible = true;
                bgwBranchLoad.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bgwBranchLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Program.CurrentUserID);


                dtUserBranchResult = new UserBranchDetailDAL().SelectAll(userId, null, null, null, null,true);

                DataView dv = new DataView(dtUserBranchResult) {Sort = "BranchId asc"};
                dtUserBranchResult = dv.ToTable();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bgwBranchLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (dtUserBranchResult.Rows.Count<=1)
                {
                    Program.BranchId = Convert.ToInt32(dtUserBranchResult.Rows[0]["BranchId"]);
                    Program.BranchCode = dtUserBranchResult.Rows[0]["BranchCode"].ToString();

                    var branchDAl = new BranchProfileDAL();

                    settingVM.BranchInfoDT = branchDAl.SelectAll(Program.BranchId.ToString(), null, null, null, null, true);
                    Program.IsWCF = settingVM.BranchInfoDT.Rows[0]["IsWCF"].ToString();
                   
                    this.Close();
                }
                dgvUserBranch.Rows.Clear();
                int j = 0;
                foreach (DataRow dr in dtUserBranchResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvUserBranch.Rows.Add(NewRow);

                    dgvUserBranch.Rows[j].Cells["BranchId"].Value = dr["BranchId"].ToString();
                    dgvUserBranch.Rows[j].Cells["BranchName"].Value = dr["BranchName"].ToString();
                    dgvUserBranch.Rows[j].Cells["BranchCode"].Value = dr["BranchCode"].ToString();
                    dgvUserBranch.Rows[j].Cells["Address"].Value = dr["Address"].ToString();

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

        private void OrdinaryLoad()
        {
            OrdinaryVATDesktop.CompanyName = Program.CompanyName; 
 OrdinaryVATDesktop.CompanyID                           = Program.CompanyID               ;
 OrdinaryVATDesktop.CompanyNameLog                      = Program.CompanyNameLog          ;
 OrdinaryVATDesktop.CompanyName                         = Program.CompanyName             ;
 OrdinaryVATDesktop.CompanyLegalName                    = Program.CompanyLegalName        ;
 OrdinaryVATDesktop.Address1                            = Program.Address1                ;
 OrdinaryVATDesktop.Address2                            = Program.Address2                ;
 OrdinaryVATDesktop.Address3                            = Program.Address3                ;
 OrdinaryVATDesktop.City                                = Program.City                    ;
 OrdinaryVATDesktop.ZipCode                             = Program.ZipCode                 ;
 OrdinaryVATDesktop.TelephoneNo                         = Program.TelephoneNo             ;
 OrdinaryVATDesktop.FaxNo                               = Program.FaxNo                   ;
 OrdinaryVATDesktop.Email                               = Program.Email                   ;
 OrdinaryVATDesktop.ContactPerson                       = Program.ContactPerson           ;
 OrdinaryVATDesktop.ContactPersonDesignation            = Program.ContactPersonDesignation;
 OrdinaryVATDesktop.ContactPersonTelephone              = Program.ContactPersonTelephone  ;
 OrdinaryVATDesktop.ContactPersonEmail                  = Program.ContactPersonEmail      ;
 OrdinaryVATDesktop.TINNo                               = Program.TINNo                   ;
 OrdinaryVATDesktop.VatRegistrationNo                   = Program.VatRegistrationNo       ;
 OrdinaryVATDesktop.Comments                            = Program.Comments                ;
 OrdinaryVATDesktop.ActiveStatus                        = Program.ActiveStatus            ;
 OrdinaryVATDesktop.FMonthStart                         = Program.FMonthStart             ;
 OrdinaryVATDesktop.FMonthEnd                           = Program.FMonthEnd               ;
 OrdinaryVATDesktop.VATAmount                           = Program.VATAmount               ;
 OrdinaryVATDesktop.BranchId                            = Program.BranchId                ;
 OrdinaryVATDesktop.BranchCode                          = Program.BranchCode              ;
 OrdinaryVATDesktop.CurrentUser                         = Program.CurrentUser             ;
 OrdinaryVATDesktop.CurrentUserID                       = Program.CurrentUserID           ;
 OrdinaryVATDesktop.IsLoading                           = Program.IsLoading               ;
 OrdinaryVATDesktop.R_F                                 = Program.R_F                     ;
 OrdinaryVATDesktop.fromOpen                            = Program.fromOpen                ;
 OrdinaryVATDesktop.SalesType                           = Program.SalesType               ;
 OrdinaryVATDesktop.Trading                             = Program.Trading                 ;
 OrdinaryVATDesktop.DatabaseName                        = Program.DatabaseName            ;
 OrdinaryVATDesktop.PublicRollLines                     = Program.PublicRollLines         ;
 OrdinaryVATDesktop.SessionDate                         = Program.SessionDate             ;
 OrdinaryVATDesktop.SessionTime                         = Program.SessionTime             ;
 OrdinaryVATDesktop.ChangeTime                          = Program.ChangeTime              ;
 OrdinaryVATDesktop.ServerDateTime                      = Program.ServerDateTime          ;
 OrdinaryVATDesktop.vMinDate                            = Program.vMinDate                ;
 OrdinaryVATDesktop.vMaxDate                            = Program.vMaxDate                ;
 OrdinaryVATDesktop.successLogin                        = Program.successLogin            ;
 OrdinaryVATDesktop.FontSize                            = Program.FontSize                ;
 OrdinaryVATDesktop.Access                              = Program.Access                  ;
 OrdinaryVATDesktop.Post                                = Program.Post                    ;
 OrdinaryVATDesktop.LicenceDate                         = Program.LicenceDate             ;
 OrdinaryVATDesktop.serverDate                          = Program.serverDate              ;
 OrdinaryVATDesktop.IsTrial                             = Program.IsTrial                 ;
 OrdinaryVATDesktop.Trial                               = Program.Trial                   ;
 OrdinaryVATDesktop.TrialComments                       = Program.TrialComments           ;
 OrdinaryVATDesktop.ImportFileName                      = Program.ImportFileName          ;
 OrdinaryVATDesktop.ItemType                            = Program.ItemType                ;
 OrdinaryVATDesktop.IsAlpha                             = Program.IsAlpha                 ;
 OrdinaryVATDesktop.Alpha                               = Program.Alpha                   ;
 OrdinaryVATDesktop.AlphaComments                       = Program.AlphaComments           ;
 OrdinaryVATDesktop.IsBeta                              = Program.IsBeta                  ;
 OrdinaryVATDesktop.Beta                                = Program.Beta                    ;
 OrdinaryVATDesktop.BetaComments                        = Program.BetaComments            ;
 OrdinaryVATDesktop.IsBureau                            = Program.IsBureau                ;
 OrdinaryVATDesktop.Add                                 = Program.Add                     ;
 OrdinaryVATDesktop.Edit                                = Program.Edit                    ;
 OrdinaryVATDesktop.IsWCF = Program.IsWCF;
 OrdinaryVATDesktop.Section = Program.Section;

        }

        public static string SelectOne()
        {
            string searchValue = string.Empty;
            try
            {
                FormLoginBranch frmBranchSearch = new FormLoginBranch();
                frmBranchSearch.ShowDialog();
                searchValue = frmBranchSearch.SelectedValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return searchValue;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }

        private void dgvUserBranch_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvUserBranch_DoubleClick(object sender, EventArgs e)
        {
            Program.BranchId =Convert.ToInt32( dgvUserBranch.CurrentRow.Cells["BranchId"].Value);
            Program.BranchCode =  dgvUserBranch.CurrentRow.Cells["BranchCode"].Value.ToString();

            var branchDAl = new BranchProfileDAL();
            settingVM.BranchInfoDT = branchDAl.SelectAll(Program.BranchId.ToString(), null, null, null, null, true);

            Program.IsWCF = settingVM.BranchInfoDT.Rows[0]["IsWCF"].ToString();


            MDIMainInterface mdf = new MDIMainInterface();
 
          
            this.Close();


        }

        private void dgvUserBranch_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dgvUserBranch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter) || e.KeyCode.Equals(Keys.F9))
            {
                Program.BranchId = Convert.ToInt32(dgvUserBranch.CurrentRow.Cells["BranchId"].Value);
                Program.BranchCode = dgvUserBranch.CurrentRow.Cells["BranchCode"].Value.ToString();

                var branchDAl = new BranchProfileDAL();
                settingVM.BranchInfoDT = branchDAl.SelectAll(Program.BranchId.ToString(), null, null, null, null, true);

                Program.IsWCF = settingVM.BranchInfoDT.Rows[0]["IsWCF"].ToString();

               

                MDIMainInterface mdf = new MDIMainInterface();
                this.Close();

            }

        }

        private void FormLoginBranch_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.MdiForm.MenuRoll();

            string Menu = new CommonDAL().settingsDesktop("Menu", "ClassicalMenu",null);

            if (Menu.ToLower() == "y")
            {
                Program.MdiForm.ribbon1.Visible = false;
                Program.MdiForm.menuStrip1.Visible = true;
                Program.MdiForm.menuStripTop.Visible = false;
            }
            else
            {
                Program.MdiForm.ribbon1.Visible = true;
                Program.MdiForm.menuStrip1.Visible = false;
                Program.MdiForm.menuStripTop.Visible = false;
            }
            OrdinaryLoad();

            CommonDAL commonDal = new CommonDAL();

            string interval = commonDal.settingsDesktop("Session", "TimeOut",null);

            if (string.IsNullOrEmpty(interval))
            {
                interval = "0";
            }

            int inMilliseconds = Convert.ToInt32(interval) * 60 * 1000;

            Program.MdiForm.StartSessionTimer(inMilliseconds);

            #region All Form Close

            Program.MdiForm.CloaseAllForm();

            #endregion

        }

    }
}
