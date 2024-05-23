using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management;
using Microsoft.SqlServer.Management.Common;
using System.Collections.Specialized;
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormDBBackupRestore : Form
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string dbName = string.Empty;
        private SqlConnection sqlConn;
        private Server sqlServer;
        private List<Database> dbList;
        private string FolderPath = string.Empty;
        private string BackupName = string.Empty;
        private string DBBackupLocation = "";
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        public FormDBBackupRestore()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            openBakFile.InitialDirectory = Application.StartupPath;
            saveBakFile.InitialDirectory = Application.StartupPath;

            try
            {

                //sqlConn = new SqlConnection(Properties.Settings.Default.masterConnectionString);
                sqlConn = _dbsqlConnection.GetConnection();
                sqlServer = new Server(new ServerConnection(sqlConn));

                dbList = new List<Database>();
                foreach (Database db in sqlServer.Databases)
                {
                    dbList.Add(db);
                }

                cmbBackupDb.DataSource = dbList;
                cmbRestoreDb.DataSource = dbList;

                cmbBackupMode.SelectedIndex = 0;
            }
            catch (Exception exc)
            {
                MessageBox.Show(string.Format("Exception occured.\nMessage: {0}", exc.Message));
            }
        }

        private void btnFileToBack_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            FolderPath = fbd.SelectedPath;
            if (FolderPath.Length == 3)
            {
                MessageBox.Show("Select Folder!");
                FolderPath = string.Empty;
                return;
            }

            txtFileToBack.Text = FolderPath + "\\";
        }

        private void saveBakFile_FileOk(object sender, CancelEventArgs e)
        {
            txtFileToBack.Text = saveBakFile.FileName;
            //FolderBrowserDialog fbd = new FolderBrowserDialog();
        }

        private void openBakFile_FileOk(object sender, CancelEventArgs e)
        {
            txtFileToRestore.Text = openBakFile.FileName;
        }

        private void btnFileToRestore_Click(object sender, EventArgs e)
        {
            openBakFile.ShowDialog();
        }

        private void btnBackupDb_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrEmpty(txtFileToBack.Text.Trim()))
            {
                MessageBox.Show("Select Folder first!");
                return;
            }
            else
            {
                BackupDb();
            }
        }

        private void BackupDb()
        {
            dbName = ((Database)cmbBackupDb.SelectedItem).Name;
            BackupName = txtFileToBack.Text.Trim() + "\\" + dbName + DateTime.Now.ToString("_yyyy_MM_dd_HH-mm-ss") + ".bak";
            Backup dbBackup = new Backup();

            try
            {
                dbBackup.Action = BackupActionType.Database;
                dbBackup.Database = dbName;
                dbBackup.BackupSetName = string.Format("{0} backup set.", dbName);
                dbBackup.BackupSetDescription = string.Format("Database: {0}. Date: {1}.", dbName, DateTime.Now.ToString("dd.MMM.yyyy hh:m"));
                dbBackup.MediaDescription = "Disk";

                BackupDeviceItem device = new BackupDeviceItem(BackupName, DeviceType.File);
                dbBackup.Devices.Add(device);


                progressBar1.Visible = true;

                this.progressBar1.Value = 0;
                this.progressBar1.Maximum = 100;
                this.progressBar1.Value = 10;

                dbBackup.Complete += new ServerMessageEventHandler(dbBackup_Complete);
                dbBackup.PercentCompleteNotification = 10;

                dbBackup.SqlBackup(sqlServer);
            }
            catch (Exception exc)
            {
                dbBackup.Abort();
                MessageBox.Show(string.Format("Exception occured.\nMessage: {0}", exc.Message));
            }
            finally
            {
                sqlConn.Close();
                progressBar1.Visible = false;

            }


        }

        void dbBackup_Complete(object sender, ServerMessageEventArgs e)
        {
            MessageBox.Show("Backup complete");
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(openBakFile.FileName))
            {
                MessageBox.Show("Select file first!");
            }
            else
            {
               
                RestoreDb();
            }
        }

        private void RestoreDb()
        {
            Restore res = new Restore();

            this.Cursor = Cursors.WaitCursor;

            try
            {
                string fileName = this.txtFileToRestore.Text;
                Database restoreDb = (Database)cmbRestoreDb.SelectedItem;
                //dbName = restoreDb.Name;

                string databaseName = restoreDb.Name;

                res.Database = databaseName;
                res.Action = RestoreActionType.Database;
                res.Devices.AddDevice(fileName, DeviceType.File);

                progressBar1.Visible = true;

                this.progressBar1.Value = 0;
                this.progressBar1.Maximum = 100;
                this.progressBar1.Value = 10;

                res.PercentCompleteNotification = 10;
                res.ReplaceDatabase = true;
                res.PercentComplete += new PercentCompleteEventHandler(ProgressEventHandler);
                res.SqlRestore(sqlServer);

                MessageBox.Show("Restore of " + databaseName + " Complete!", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SmoException exSMO)
            {
                MessageBox.Show(exSMO.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                this.Cursor = Cursors.Default;
                progressBar1.Visible = false;

            }
        }

        public void ProgressEventHandler(object sender, PercentCompleteEventArgs e)
        {

        }
     

        private void FormDBBackupRestore_Load(object sender, EventArgs e)
        {
            CommonDAL commonDal = new CommonDAL();

              DBBackupLocation = commonDal.settingsDesktop("DBBackupLocation", "DBBackupLocation",null,connVM);
              txtFileToBack.Text = DBBackupLocation;
        }

       

       
    }
}