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
using VATServer.Library;
using VATViewModel.DTOs;
using System.Threading;
using System.Windows.Controls;
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Ordinary;

namespace VATClient
{
    public partial class FormReleaseNotes : Form
    {
        #region Constructors

        public FormReleaseNotes()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }


        //

        #endregion

        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DataSet notesResult;

        private bool ChangeData = false;
        string FolderPath = "";

        private string preSettingValue = string.Empty;
        private bool IsSerialTracking = false;

        #region sql save, update, delete

        private SettingDAL settingDal = new SettingDAL();
        private string[] sqlResults;
        private string sqlResultssettings;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;

        #endregion

        #endregion


        private void FormReleaseNotes_Load(object sender, EventArgs e)
        {
            bgwDeleteInsert.RunWorkerAsync();
        }


        private void GetData()
        {
            try
            {
                bgwLoad.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "GetData", exMessage);
            }
        }


        private void PopulateGrid(DataTable table)
        {
            if (table == null) return;

            int rowsCount = table.Rows.Count;
            dgvNotes.Rows.Clear();
            for (var index = 0; index < rowsCount; index++)
            {
                DataGridViewRow grow = new DataGridViewRow();
                dgvNotes.Rows.Add(grow);

                dgvNotes["ID", index].Value = table.Rows[index]["Id"];
                dgvNotes["SL", index].Value = table.Rows[index]["SL"];
                dgvNotes["Date", index].Value = table.Rows[index]["Date"];
                dgvNotes["Version", index].Value = table.Rows[index]["Version"];
                dgvNotes["Name1", index].Value = table.Rows[index]["Name"];
                dgvNotes["Issue", index].Value = table.Rows[index]["Issue"];
                dgvNotes["Description", index].Value = table.Rows[index]["Description"];
            }
        }

        private void PopulateGroup(DataTable table)
        {
            if (table == null) return;

            cmbVersionGroup.Items.Add("All");

            foreach (DataRow tableRow in table.Rows)
            {
                cmbVersionGroup.Items.Add(tableRow["Version"].ToString());
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void cmbSettingGroup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void FormSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }



        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string search = "'%" + txtSearch.Text.Trim().ToLower() + "%'";

                if (notesResult == null || notesResult.Tables.Count <= 0) return;

                DataRow[] rows = notesResult.Tables[0].Select("Version like " + search + " or Convert(SL, 'System.String') like " + search + " or Name like" +
                                                              search + " or Issue like" + search + " or Description like" + search);

                if (rows.Length > 0)
                {
                    PopulateGrid(rows.CopyToDataTable());
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
                FileLogger.Log(this.Name, "GetData", exMessage);
            }
        }

        private void bgwDeleteInsert_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ReleaseNotesDAL notesDal = new ReleaseNotesDAL();
                
                notesDal.Insert();

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
                FileLogger.Log(this.Name, "GetData", exMessage);
            }
        }

        private void bgwDeleteInsert_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GetData();
        }

        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            ReleaseNotesDAL notesDal = new ReleaseNotesDAL();

            notesResult = notesDal.SearchNotes(connVM);
        }

        private void bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (notesResult != null && notesResult.Tables.Count > 0)
            {
                PopulateGrid(notesResult.Tables[0]);
                PopulateGroup(notesResult.Tables[1]);
            }

        }

        private void cmbVersionGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectValue = cmbVersionGroup.Text;

            if (selectValue != "All")
            {
                DataTable temp = notesResult.Tables[0].Select("Version = '" + selectValue + "'").CopyToDataTable();
                PopulateGrid(temp);

            }
            else
            {
                PopulateGrid(notesResult.Tables[0]);
            }
        }

        private void dgvNotes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void dgvNotes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvNotes.Rows[e.RowIndex];

            Details.ShowDetails(row);
        }

        private void dgvNotes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}
