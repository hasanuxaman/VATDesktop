using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using SymphonySofttech.Utilities;
using VATClient.ReportPages;
using VATClient.ReportPreview;

using VATServer.Library;
using VATViewModel.DTOs;
using VATClient.ModelDTO;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Management;
using System.Globalization;
using System.Net.NetworkInformation;

//bari



namespace VATClient
{
    public partial class MDIMainInterface : Form, IMessageFilter
    {
        private int childFormNumber = 0;
        private bool IsSymphonyUser = false;
        private Timer mTimer = null;
        private IPStatus status = IPStatus.TimedOut;
        MdiClient mdi;

        #region Methods 01

        public MDIMainInterface()
        {
            InitializeComponent();
            foreach (Control c in this.Controls)
            {
                if (c is MdiClient)
                {
                    mdi = (MdiClient)c;
                    break;
                }
            }

            StartSessionTimer();
        }

        public void StartSessionTimer(int interval = 0)
        {
            if (interval > 0)
            {
                mTimer = new Timer { Interval = interval };

                mTimer.Tick += LogoutUser;
                mTimer.Enabled = true;
                Application.AddMessageFilter(this);
            }
        }

        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #region Log
        private string DataBaseName = string.Empty;
        private string LogID = string.Empty;
        private string ComputerName = string.Empty;
        private string ComputerLoginUserName = string.Empty;
        private string ComputerIPAddress = string.Empty;
        private string SoftwareUserId = string.Empty;
        private string SoftwareUserName = string.Empty;
        private DateTime SessionDate = DateTime.Now;
        private DateTime LogInDateTime = DateTime.Now;
        private DateTime LogOutDateTime = DateTime.Now;
        List<UserLogsVM> userLogs = new List<UserLogsVM>();

        #endregion Log

        private void MDIMainInterface_Load(object sender, EventArgs e)
        {

            try
            {
                ribbon1.BackColor = Color.Green;
                //ribbon1.Tabs.Remove(rTabSetup);
                //rTabSetup.Panels.Remove(ribbonPanel1);
                //ribbonPanel1.Items.Remove(ribbonButton10);

                findLogInformations();
                MenuMaker();

                if (string.IsNullOrEmpty(Program.CurrentUserID))
                {
                    FormLogIn frmlLogIn = new FormLogIn();
                    frmlLogIn.ShowDialog();
                }
                if (!string.IsNullOrEmpty(Program.CurrentUserID) && Program.BranchId == 0)
                {
                    FormLoginBranch frmlLogIn = new FormLoginBranch();
                    frmlLogIn.ShowDialog();
                }

                MDIName();

                //// is the encp field== decrypt
                // bool isSecured = CheckSecurity();
                //     if (!isSecured)
                //     {
                //         Program.IsTrial = true;
                //     }

                Version v = Assembly.GetExecutingAssembly().GetName().Version;
                string About = string.Format(CultureInfo.InvariantCulture, @"YourApp Version {0}.{1}.{2} (r{3})", v.Major, v.Minor, v.Build, v.Revision);
                tSSL7.Text = About;

                #region Hard Code Panels & Pannel Items Remove
                rTabSale.Panels.Remove(rpExp);
                rTabNBRReport.Panels.Remove(rp20);
                //rTabAdjustment.Panels.Remove(rpDispose);
                //rTabAdjustment.Panels.Remove(rpDrawBack);
                rpClient.Items.Remove(ribbonButton145);
                rTabUserAccount.Panels.Remove(rpUserRole);

                string code = new CommonDAL().settingValue("CompanyCode", "Code");
                if (code.ToLower()!="GDIC".ToLower())
                {
                    ribbon1.Tabs.Remove(ribbonTabGDIC);
                }
                if (code.ToUpper() != "QEL")
                {
                    rpStock.Items.Remove(rbtnQuaziStock);
                }
                if (code.ToUpper() != "NESTLE")
                {
                    rTabSale.Panels.Remove(rpNestle);
                    rTabPurchase.Panels.Remove(rpPurchaseNestle);
                }
               
                else
                {
                    rTabSale.Panels.Remove(rpTReceive);
                    rTabSale.Panels.Remove(rpExp);
                    rTabSale.Panels.Remove(rpPackage);
                    rpSale.Items.Remove(ribbonButton138);
                    rpSale.Items.Remove(ribbonButton139);
                    rpSale.Items.Remove(rbnTenderTrading);
                    rpSale.Items.Remove(ribbonButton136);
                 
                    rpSale.Items.Remove(rbtnRawSale);
                    rpSale.Items.Remove(rbtnWastageSale);
                    rpSale.Items.Remove(rbtnTollSale);
                    rpSale.Items.Remove(ribbonSeparator5);
                    rpSale.Items.Remove(ribbonSeparator6);
                    rpSale.Items.Remove(ribbonSeparator7);

                    ribbon1.Tabs.Remove(rTabProduction);
                    //ribbon1.Tabs.Remove(rTabDeposit);
                    ribbon1.Tabs.Remove(rTabToll)
                        ;
                    rTabDeposit.Panels.Remove(rpVDS);
                    rTabDeposit.Panels.Remove(rpSD);
                    rTabDeposit.Panels.Remove(rpCashPayble);
                    rTabDeposit.Panels.Remove(rpReverse);
                    rTabDeposit.Panels.Remove(rpTDS);
                    rTabDeposit.Panels.Remove(rbpBankingchannel);

                    //ribbon1.Tabs.Remove(rTabAdjustment);
                    rTabAdjustment.Panels.Remove(rpnPurchase);
                    rTabAdjustment.Panels.Remove(rpDispose);
                    rTabAdjustment.Panels.Remove(rpDrawBack);
                    rTabAdjustment.Panels.Remove(rpVATAdjustment);
                    rTabAdjustment.Panels.Remove(rpVATAdjustment);
                    rplSale.Items.Remove(rbtnRawSaleCN);

                    //rTabPurchase.Panels.Remove(rpPurchase);

                    rTabNBRReport.Panels.Remove(rp61);
                    rTabNBRReport.Panels.Remove(rp62);
                    rTabNBRReport.Panels.Remove(rp65);
                    rTabNBRReport.Panels.Remove(rp6_6);

                    rpPurchase.Items.Remove(rbtnImport);
                    rpPurchase.Items.Remove(ribbonButton68);
                    rpPurchase.Items.Remove(rbtnTollCharge);
                    rpPurchase.Items.Remove(ribbonButton53);
                    rpPurchase.Items.Remove(ribbonButton64);
                }

                #endregion
            }

            catch (Exception)
            {
                Program.IsTrial = true;
            }
        }

        private void Unregistered()
        {


#if (UnregisterVersion)
                                Program.IsTrial = true;
#else
            Program.IsTrial = false;
#endif
#if (Alpha)
                       
                         Program.IsAlpha = true;
#else
            Program.IsAlpha = false;
#endif
#if (Beta)
                       
                         Program.IsBeta = true;
#else
            Program.IsBeta = false;
#endif
#if (BureauDebug)

                        //Program.IsBureau=true;
#else
            //Program.IsBureau = false;
#endif
        }

        public void MenuMaker()
        {
            bool IsSecure = Program.IsTrial;

            Unregistered();

            if (IsSecure)
            {
                Program.IsTrial = true;
            }



            findLogInformations();
            var softName = "VAT Software";

            //timer1.Start();

            if (string.IsNullOrEmpty(SoftwareUserId))
            {
                this.Text = softName + " (No Company Selected)";
                tSSL1.Text = " User: anonymous,";
                tSSL2.Text = " Session Date: " + SessionDate.ToString("dd-MMM-yyyy") + ",";
                tSSL3.Text = " Login Time: " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + ",";
                //tSSL4.Text = " PC User: anonymous,";
                //tSSL5.Text = " PC Name: anonymous,";
                //tSSL6.Text = ", IP Add: anonymous";
                tSSL4.Text = "";
                tSSL5.Text = "";
                tSSL6.Text = "";
                //tSSL7.Text = ", Current Time: ";
                if (Program.IsTrial == true)
                {
                    LCompany.Text = " " + "No Company Selected (" + Program.TrialComments + ")";

                }
                else if (Program.IsAlpha == true)
                {
                    LCompany.Text = " " + "No Company Selected (" + Program.AlphaComments + ")";

                }
                else if (Program.IsBeta == true)
                {
                    LCompany.Text = " " + "No Company Selected (" + Program.BetaComments + ")";

                }
                else
                {
                    LCompany.Text = "No Company Selected";


                }




            }
            else
            {

                this.Text = softName + @"  (" + Program.CompanyNameLog + "-" + Program.CompanyLegalName + @")";
                tSSL1.Text = @" User: " + SoftwareUserName + ",";
                tSSL2.Text = " Session Date: " + SessionDate.ToString("dd-MMM-yyyy") + ",";
                tSSL3.Text = " Login Time: " + LogInDateTime.ToString("dd-MMM-yyyy HH:mm:ss") + ",";
                MDIName();
                rpSaleCE.Enabled = false;
                //rpMIS19.Enabled = false;
                if (DatabaseInfoVM.DatabaseName.ToString().ToLower().Contains("nita"))//Rupsha
                {
                    rpSaleCE.Enabled = true;
                }

            }

            int MW = this.Size.Width;
            int MH = this.Size.Height;
            int top = this.Top;
            int LW = LCompany.Size.Width;
            int LH = LCompany.Size.Height;
            LCompany.Left = MW - LW - 100;
            LCompany.Top = top + 5;
            // LCompany.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(15)))), ((int)(((byte)(22))))); ;
            ribbon1.Refresh();
            ribbon1.ActiveTab = rTabSetup;
            string code = "";
            if (DatabaseInfoVM.DatabaseName != "master")
            {
                code = new CommonDAL().settingValue("CompanyCode", "Code");
            }
            if (Program.IsTrial == true)
            {
                if (code.ToLower() == "berger" || code.ToLower() == "dhl"||code.ToLower() == "roche")
                {
                    //MessageBox.Show("You are use an Unregister version.\r\n Please contact Symphony Softtech Ltd. for register.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Program.IsTrial = false;
                }
                else
                {
                    MessageBox.Show("You are use an Unregister version.\r\n Please contact Symphony Softtech Ltd. for register.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else if (Program.IsAlpha == true)
            {
                MessageBox.Show(MessageVM.msgAlpha, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (Program.IsBeta == true)
            {
                MessageBox.Show(MessageVM.msgBeta, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }




        }

        private void MDIName()
        {
            if (Program.IsTrial == true)
            {
                LCompany.Text = " " + Program.CompanyNameLog + " (" + Program.BranchCode + ")-(" + Program.TrialComments + ")";

            }
            else if (Program.IsAlpha == true)
            {
                LCompany.Text = " " + Program.CompanyNameLog + " (" + Program.BranchCode + ")-(" + Program.AlphaComments + ")";

            }
            else if (Program.IsBeta == true)
            {
                LCompany.Text = " " + Program.CompanyNameLog + " (" + Program.BranchCode + ")-(" + Program.BetaComments + ")";

            }
            else
            {
                LCompany.Text = " " + Program.CompanyNameLog + " (" + Program.BranchCode + ")";

            }
        }

        public void LogIn1()
        {
            //findLogInformations();
            var ULInfo = from UL in userLogs.ToList()
                         where UL.DataBaseName.ToLower() == DataBaseName.ToLower()
                         select UL;
            if (ULInfo == null || ULInfo.Count() == 0)
            {
                UserLogsVM UserLog = new UserLogsVM();
                UserLog.LogID = LogID;
                UserLog.DataBaseName = DataBaseName;
                UserLog.ComputerName = ComputerName;
                UserLog.ComputerLoginUserName = ComputerLoginUserName;
                UserLog.ComputerIPAddress = ComputerIPAddress;
                UserLog.SoftwareUserId = SoftwareUserId;
                UserLog.SoftwareUserName = SoftwareUserName;
                UserLog.SessionDate = SessionDate.ToString("yyyy-MMM-dd HH:mm:ss");
                UserLog.LogInDateTime = LogInDateTime.ToString("yyyy-MMM-dd HH:mm:ss");
                UserLog.LogOutDateTime = LogOutDateTime.ToString("yyyy-MMM-dd HH:mm:ss");
                userLogs.Add(UserLog);
            }

            UserInformationDAL userInformationDal = new UserInformationDAL();

            var logOutDate = LogOutDateTime.ToString("yyyy-MMM-dd HH:mm:ss");
            //LogID = userInformationDal.InsertUserLogin(userLogs, LogOutDateTime);
            LogID = userInformationDal.InsertUserLogin(userLogs, logOutDate);

        }

        public void LogOut1()
        {
            findLogInformations();
            var ULInfo = from UL in userLogs.ToList()
                         where UL.DataBaseName.ToLower() == DataBaseName.ToLower()
                         select UL;
            if (ULInfo == null || ULInfo.Count() == 0)
            {
                UserLogsVM UserLog = new UserLogsVM();
                UserLog.LogID = LogID;
                UserLog.DataBaseName = DataBaseName;
                UserLog.ComputerName = ComputerName;
                UserLog.ComputerLoginUserName = ComputerLoginUserName;
                UserLog.ComputerIPAddress = ComputerIPAddress;
                UserLog.SoftwareUserId = SoftwareUserId;
                UserLog.SoftwareUserName = SoftwareUserName;
                UserLog.SessionDate = SessionDate.ToString("yyyy-MMM-dd HH:mm:ss");
                UserLog.LogInDateTime = LogInDateTime.ToString("yyyy-MMM-dd HH:mm:ss");
                UserLog.LogOutDateTime = LogOutDateTime.ToString("yyyy-MMM-dd HH:mm:ss");
                userLogs.Add(UserLog);
            }

            UserInformationDAL userInformationDal = new UserInformationDAL();
            var logOutDate = LogOutDateTime.ToString("yyyy-MMM-dd HH:mm:ss");
            //LogID = userInformationDal.InsertUserLogOut(userLogs, LogOutDateTime);
            LogID = userInformationDal.InsertUserLogOutByList(userLogs, logOutDate);
        }

        private void findLogInformations()
        {

            #region Log
            DataBaseName = DatabaseInfoVM.DatabaseName;

            //ComputerName = Dns.GetHostEntry("localhost").HostName;
            //ComputerLoginUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //ComputerIPAddress = Dns.GetHostEntry(ComputerName).AddressList[1].ToString();
            SoftwareUserId = Program.CurrentUserID;
            SoftwareUserName = Program.CurrentUser;
            SessionDate = Program.SessionDate;
            LogInDateTime = DateTime.Now;
            LogOutDateTime = DateTime.Now;

            //    string strDriveLetter
            //if (strDriveLetter == "" || strDriveLetter == null)
            //    strDriveLetter = "C";
            //ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + strDriveLetter + ":\"");
            //disk.Get();
            // disk["VolumeSerialNumber"].ToString();

            #endregion Log


        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Window " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        public void RollDetailsInfo(string ID)
        {
            //Ruba
            Program.Access = "N";
            Program.Post = "N";
            Program.Add = "N";
            Program.Edit = "N";
            if (Program.PublicRollLines == null || Program.PublicRollLines.Length == 0)
            {
                //MessageBox.Show("Test");Program.Access = "N";
                //Program.Access = "N";
                //Program.Post = "N";
                return;
            }
            for (int j = 0; j < Convert.ToInt32(Program.PublicRollLines.Length); j++)
            {
                string[] RollFields = Program.PublicRollLines[j].Split(FieldDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    if (RollFields[2].ToString() == ID)
                    {
                        Program.Access = RollFields[3].ToString();
                        Program.Post = RollFields[4].ToString();
                        Program.Add = RollFields[5].ToString();
                        Program.Edit = RollFields[6].ToString();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699) { MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning); return; } MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }

        public void MenuRoll()
        {
            //Program.UserMenuAllRolls 
            //ribbon1.Tabs.Remove(rTabSetup);
            try
            {

                //return;
                //ribbonPanel1.Items.Remove(ribbonButton10);
                #region Button
                MenuButtonPermission("110110110", rpItemInformation, ribbonButton10);
                MenuButtonPermission("110110120", rpItemInformation, ribbonButton81);
                MenuButtonPermission("110110130", rpItemInformation, ribbonButton93);
                MenuButtonPermission("110110140", rpItemInformation, rbtnTDS1);
                MenuButtonPermission("110110150", rpItemInformation, rbtnHSCode);
                MenuButtonPermission("110120110", rpVendor, ribbonButton11);
                MenuButtonPermission("110120120", rpVendor, ribbonButton12);
                MenuButtonPermission("110130110", rpCustomer, ribbonButton13);
                MenuButtonPermission("110130120", rpCustomer, ribbonButton80);
                MenuButtonPermission("110140110", rpBankVehicle, ribbonButton14);
                MenuButtonPermission("110140120", rpBankVehicle, rbtnVehicle);
                MenuButtonPermission("110150110", rpPriceDeclaration, ribbonButton17);
                MenuButtonPermission("110150120", rpPriceDeclaration, ribbonButton96);
                MenuButtonPermission("110150130", rpPriceDeclaration, ribbonButton89);
                MenuButtonPermission("110160110", rpCompany, ribbonButton18);
                MenuButtonPermission("110160120", rpCompany, rbtnBranchTransfer);
                MenuButtonPermission("110170110", rpFiscalYear, ribbonButton75);
                MenuButtonPermission("110180110", rpConfiguration, rbtnSettings);
                MenuButtonPermission("110180120", rpConfiguration, ribbonButton47);
                MenuButtonPermission("110180130", rpConfiguration, rbtnShift);
                MenuButtonPermission("110190110", rpImportSync, ribbonButton46);
                MenuButtonPermission("110190120", rpImportSync, ribbonButton35);
                MenuButtonPermission("110190130", rpImportSync, rbtnExecute);
                MenuButtonPermission("110200110", rpMeasurement, ribbonButton16);
                MenuButtonPermission("110200120", rpMeasurement, ribbonButton65);
                MenuButtonPermission("110210110", rpCurrency, ribbonButton98);
                MenuButtonPermission("110210120", rpCurrency, ribbonButton120);
                MenuButtonPermission("110220110", rpBanderol, ribBtnBanderol);
                MenuButtonPermission("110220120", rpBanderol, ribBtnPackage);
                MenuButtonPermission("110220130", rpBanderol, rbnBtnProduct);
                MenuButtonPermission("120110110", rpPurchase, ribbonButton68);
                MenuButtonPermission("120110120", rpPurchase, rbtnImport);
                MenuButtonPermission("120110130", rpPurchase, rbtnInputService);
                //MenuButtonPermission("120110140", rpPurchase, ribbonButton50);
                MenuButtonPermission("120110150", rpPurchase, ribbonButton53);
                MenuButtonPermission("120110160", rpPurchase, ribbonButton64);
                MenuButtonPermission("130110110", rpIssue, ribbonButton130);
                MenuButtonPermission("130110120", rpIssue, ribbonButton131);
                MenuButtonPermission("130110130", rpIssue, rbtnIssueWOBom);
                MenuButtonPermission("130110140", rpIssue, rbtnWastageIssue);
                MenuButtonPermission("130110150", rpIssue, ribBtnTransfer);
                MenuButtonPermission("130120110", rpReceive, ribbonButton133);
                MenuButtonPermission("130120120", rpReceive, ribbonButton135);
                MenuButtonPermission("130120130", rpReceive, ribbonButton132);
                MenuButtonPermission("130120140", rpReceive, ribbonBtnpackage);
                MenuButtonPermission("140110110", rpSale, ribbonButton134);
                MenuButtonPermission("140110120", rpSale, ribbonButton138);
                MenuButtonPermission("140110130", rpSale, ribbonButton139);
                MenuButtonPermission("140110140", rpSale, rbnTenderTrading);
                MenuButtonPermission("140110150", rpSale, ribbonButton136);
                MenuButtonPermission("140110160", rpSale, ribbonButton19);
                MenuButtonPermission("140110170", rpSale, rbtnRawSale);
                MenuButtonPermission("140110180", rpSale, rbtnWastageSale);
                MenuButtonPermission("140120110", rpPackage, ribbonButton23);
                MenuButtonPermission("140130110", rpTReceive, rbtn61In);
                MenuButtonPermission("140130120", rpTReceive, rbtn62In);
                MenuButtonPermission("140130130", rpTReceive, rbtn61Out1);
                MenuButtonPermission("140130140", rpTReceive, rbtn62Out1);
                MenuButtonPermission("140140110", rpExp, ribbonButton15);
                MenuButtonPermission("150110110", rpDeposit, ribbonButton140);
                MenuButtonPermission("150120110", rpVDS, ribbonButton141);
                MenuButtonPermission("150120120", rpVDS, rbtnSaleVDS);
                MenuButtonPermission("150130110", rpSD, ribbonButton143);
                MenuButtonPermission("150140110", rpCashPayble, rbtnCashPayble);
                MenuButtonPermission("150150110", rpReverse, ribBtnReverse);
                MenuButtonPermission("150160110", rpTDS, rbtnTDS);
                MenuButtonPermission("160110110", rpClient, ribbonButton94);
                MenuButtonPermission("160110120", rpClient, ribbonButton95);
                //MenuButtonPermission("160110130", rpClient, ribbonButton145);
                MenuButtonPermission("160120110", rpContractor, ribbonButton116);
                MenuButtonPermission("160120120", rpContractor, ribbonButton117);
                MenuButtonPermission("160120130", rpContractor, ribbonButton118);
                MenuButtonPermission("160120140", rpContractor, rbtnToll6_3);
                MenuButtonPermission("160130110", rpTollRegister, rbtnToll6_1);
                MenuButtonPermission("160130120", rpTollRegister, rbtnToll6_2);
                MenuButtonPermission("170110110", rpOtherAdjustment, ribbonButton78);
                MenuButtonPermission("170110120", rpOtherAdjustment, ribbonButton129);
                MenuButtonPermission("170120110", rpnPurchase, ribbonButton122);
                //MenuButtonPermission("170120120", rpnPurchase, ribbonButton123);
                MenuButtonPermission("170130110", rplSale, ribbonButton124);
                MenuButtonPermission("170130120", rplSale, ribbonButton125);
                MenuButtonPermission("170140110", rpDispose, ribbonButton51);
                MenuButtonPermission("170140120", rpDispose, ribbonButton52);
                MenuButtonPermission("170150110", rpDrawBack, ribbonButton40);
                MenuButtonPermission("170160110", rpVATAdjustment, rbtnVATAdjustment);
                MenuButtonPermission("170160120", rpVATAdjustment, rbtnSDAdjustment);
                MenuButtonPermission("180110110", rp43, ribbonButton49);
                MenuButtonPermission("180120110", rp61, ribbonButton48);
                MenuButtonPermission("180130110", rp62, ribbonButton45);
                MenuButtonPermission("180140110", rp91, ribbonButton43);
                MenuButtonPermission("180150110", rp610, rbtn6_10);
                MenuButtonPermission("180160110", rpSDReport, ribbonButton9);
                MenuButtonPermission("180170110", rp63, ribBtnVAT11);
                MenuButtonPermission("180180110", rp65, rbtn65);
                MenuButtonPermission("180190110", rp7, ribBtnVAT7);
                MenuButtonPermission("180200110", rp20, ribbonButton26);
                MenuButtonPermission("180210110", rpBandRoll, ribBtnForm4);
                MenuButtonPermission("180210120", rpBandRoll, rbtnRecForm5);
                MenuButtonPermission("180220110", rpSumCurAcc, ribbonButtonCVAT18);
                MenuButtonPermission("180220120", rpSumCurAcc, rbtnBrakDown);
                MenuButtonPermission("180230110", rpKaKha, ribbonButtonChakKa);
                MenuButtonPermission("180230120", rpKaKha, ribbonButtonChakKha);
                MenuButtonPermission("190110110", rbpPurchase, ribbonButton57);
                MenuButtonPermission("190120110", rbpProduction, ribbonButton56);
                MenuButtonPermission("190120120", rbpProduction, ribbonButton92);
                MenuButtonPermission("190130110", rbpSale, ribbonButton60);
                MenuButtonPermission("190140110", rpStock, ribbonButton87);
                MenuButtonPermission("190140120", rpStock, rbtnReceiveSale);
                MenuButtonPermission("190140130", rpStock, rbtnReconsciliation);
                MenuButtonPermission("190140140", rpStock, rbtnBranchStockMovement);
                MenuButtonPermission("190150110", rbpDeposit, ribbonButton88);
                MenuButtonPermission("190150120", rbpDeposit, rbtnCurrentAccount);
                MenuButtonPermission("190160110", rpOthers, ribbonButton20);
                MenuButtonPermission("190160120", rpOthers, ribbonButtonCoEfficient);
                MenuButtonPermission("190160130", rpOthers, ribbonButtonWastage);
                MenuButtonPermission("190160140", rpOthers, ribbonButtonValueChange);
                MenuButtonPermission("190160150", rpOthers, ribbonButtonSerStock);
                MenuButtonPermission("190160160", rpOthers, ribbonButtonPurchaseLC);
                MenuButtonPermission("190170110", rpSaleCE, ribbonButton27);
                MenuButtonPermission("190180110", rpMIS19, rbtnMIS19);
                MenuButtonPermission("200110110", rpNewAccount, ribbonButton63);
                MenuButtonPermission("200120110", rpPasswordChange, ribbonButton62);
                MenuButtonPermission("200130110", rpUserRole, ribbonButton61);
                MenuButtonPermission("200140110", rpSettingsRole, ribBtnSettingRole);
                MenuButtonPermission("200150110", rpLogOut, ribbonButton7);
                MenuButtonPermission("200160110", rpLogin, ribbonButton66);
                MenuButtonPermission("200170110", rpLogs, ribbonButton21);
                MenuButtonPermission("200180110", rpCloseAll, ribbonButton144);
                MenuButtonPermission("200190110", rpUserBranch, ribbonButton36);
                MenuButtonPermission("200200110", rpnlUserMenu, rbtnMenuAll);
                MenuButtonPermission("200200120", rpnlUserMenu, rbtnUserMenu);
                MenuButtonPermission("210110110", rpDemand, ribBtnDemand);
                MenuButtonPermission("210120110", rbpReceive, rbnBtnBandReceive);
                MenuButtonPermission("220110110", rpScbl, ribbonButtonLocalPurcchase);
                MenuButtonPermission("220110120", rpScbl, ribbonButtonLocalSales);
                MenuButtonPermission("220110130", rpScbl, rbtnReceiveSales);
                //MenuButtonPermission("220110140", rpScbl, rbtnSaleSummary);
                //MenuButtonPermission("220110150", rpScbl, rbtnSaleSummarybyProduct);
                //MenuButtonPermission("220110160", rpScbl, rbtnImportData);
                #endregion Button

                #region Panel
                MenuPanelPermission("110110", rTabSetup, rpItemInformation);
                MenuPanelPermission("110120", rTabSetup, rpVendor);
                MenuPanelPermission("110130", rTabSetup, rpCustomer);
                MenuPanelPermission("110140", rTabSetup, rpBankVehicle);
                MenuPanelPermission("110150", rTabSetup, rpPriceDeclaration);
                MenuPanelPermission("110160", rTabSetup, rpCompany);
                MenuPanelPermission("110170", rTabSetup, rpFiscalYear);
                MenuPanelPermission("110180", rTabSetup, rpConfiguration);
                MenuPanelPermission("110190", rTabSetup, rpImportSync);
                MenuPanelPermission("110200", rTabSetup, rpMeasurement);
                MenuPanelPermission("110210", rTabSetup, rpCurrency);
                MenuPanelPermission("110220", rTabSetup, rpBanderol);
                MenuPanelPermission("120110", rTabPurchase, rpPurchase);
                MenuPanelPermission("130110", rTabProduction, rpIssue);
                MenuPanelPermission("130120", rTabProduction, rpReceive);
                MenuPanelPermission("140110", rTabSale, rpSale);
                MenuPanelPermission("140120", rTabSale, rpPackage);
                MenuPanelPermission("140130", rTabSale, rpTReceive);
                MenuPanelPermission("140140", rTabSale, rpExp);
                MenuPanelPermission("150110", rTabDeposit, rpDeposit);
                MenuPanelPermission("150120", rTabDeposit, rpVDS);
                MenuPanelPermission("150130", rTabDeposit, rpSD);
                MenuPanelPermission("150140", rTabDeposit, rpCashPayble);
                MenuPanelPermission("150150", rTabDeposit, rpReverse);
                MenuPanelPermission("150160", rTabDeposit, rpTDS);
                MenuPanelPermission("160110", rTabToll, rpClient);
                MenuPanelPermission("160120", rTabToll, rpContractor);
                MenuPanelPermission("160130", rTabToll, rpTollRegister);
                MenuPanelPermission("170110", rTabAdjustment, rpOtherAdjustment);
                MenuPanelPermission("170120", rTabAdjustment, rpnPurchase);
                MenuPanelPermission("170130", rTabAdjustment, rplSale);
                MenuPanelPermission("170140", rTabAdjustment, rpDispose);
                MenuPanelPermission("170150", rTabAdjustment, rpDrawBack);
                MenuPanelPermission("170160", rTabAdjustment, rpVATAdjustment);
                MenuPanelPermission("180110", rTabNBRReport, rp43);
                MenuPanelPermission("180120", rTabNBRReport, rp61);
                MenuPanelPermission("180130", rTabNBRReport, rp62);
                MenuPanelPermission("180140", rTabNBRReport, rp91);
                MenuPanelPermission("180150", rTabNBRReport, rp610);
                MenuPanelPermission("180160", rTabNBRReport, rpSDReport);
                MenuPanelPermission("180170", rTabNBRReport, rp63);
                MenuPanelPermission("180180", rTabNBRReport, rp65);
                MenuPanelPermission("180190", rTabNBRReport, rp7);
                MenuPanelPermission("180200", rTabNBRReport, rp20);
                MenuPanelPermission("180210", rTabNBRReport, rpBandRoll);
                MenuPanelPermission("180220", rTabNBRReport, rpSumCurAcc);
                MenuPanelPermission("180230", rTabNBRReport, rpKaKha);
                MenuPanelPermission("190110", rTabMISReport, rbpPurchase);
                MenuPanelPermission("190120", rTabMISReport, rbpProduction);
                MenuPanelPermission("190130", rTabMISReport, rbpSale);
                MenuPanelPermission("190140", rTabMISReport, rpStock);
                MenuPanelPermission("190150", rTabMISReport, rbpDeposit);
                MenuPanelPermission("190160", rTabMISReport, rpOthers);
                MenuPanelPermission("190170", rTabMISReport, rpSaleCE);
                MenuPanelPermission("190180", rTabMISReport, rpMIS19);
                MenuPanelPermission("200110", rTabUserAccount, rpNewAccount);
                MenuPanelPermission("200120", rTabUserAccount, rpPasswordChange);
                MenuPanelPermission("200130", rTabUserAccount, rpUserRole);
                MenuPanelPermission("200140", rTabUserAccount, rpSettingsRole);
                MenuPanelPermission("200150", rTabUserAccount, rpLogOut);
                MenuPanelPermission("200160", rTabUserAccount, rpLogin);
                MenuPanelPermission("200170", rTabUserAccount, rpLogs);
                MenuPanelPermission("200180", rTabUserAccount, rpCloseAll);
                MenuPanelPermission("200190", rTabUserAccount, rpUserBranch);
                MenuPanelPermission("200200", rTabUserAccount, rpnlUserMenu);
                MenuPanelPermission("210110", rTabBanderol, rpDemand);
                MenuPanelPermission("210120", rTabBanderol, rbpReceive);
                MenuPanelPermission("220110", ribbonTabSCBL, rpScbl);
                #endregion Panel

                #region Tab
                MenuTabPermission("110", rTabSetup);
                MenuTabPermission("120", rTabPurchase);
                MenuTabPermission("130", rTabProduction);
                MenuTabPermission("140", rTabSale);
                MenuTabPermission("150", rTabDeposit);
                MenuTabPermission("160", rTabToll);
                MenuTabPermission("170", rTabAdjustment);
                MenuTabPermission("180", rTabNBRReport);
                MenuTabPermission("190", rTabMISReport);
                MenuTabPermission("200", rTabUserAccount);
                MenuTabPermission("210", rTabBanderol);
                MenuTabPermission("220", ribbonTabSCBL);


                #endregion Tab

                #region Menu License Issue
                #region Manufacturing
                if (Program.IsManufacturing == false)
                {
                    #region Production Tab
                    MenuTabPermission("130", rTabProduction, false);

                    #endregion Production Tab
                }
                #endregion Manufacturing

                #region Toll

                if (Program.IsTollClient == false && Program.IsTollContractor == false)
                {
                    MenuTabPermission("160", rTabToll, false);

                }
                #region IsTollClient

                if (Program.IsTollClient == false)
                {
                    MenuPanelPermission("160110", rTabToll, rpClient, false);
                }
                #endregion IsTollClient

                #region IsTollContractor

                if (Program.IsTollContractor == false)
                {
                    MenuPanelPermission("160120", rTabToll, rpContractor, false);
                }
                #endregion IsTollContractor

                #endregion Toll

                #region IsCentralBIN
                if (Program.IsCentralBIN == false)
                {
                    #region Branch button
                    MenuButtonPermission("110160120", rpCompany, rbtnBranchTransfer, false);
                    #endregion Branch button

                    #region Branch StockMovement
                    MenuButtonPermission("190140140", rpStock, rbtnBranchStockMovement, false);
                    #endregion Branch StockMovement

                    #region TransferIssue/Receive Panel
                    MenuPanelPermission("140130", rTabSale, rpTReceive, false);
                    #endregion TransferIssue/Receive Panel

                    #region 6.5 Report Panel
                    MenuPanelPermission("180180", rTabNBRReport, rp65, false);
                    #endregion 6.5 Report Panel

                    #region KaKha Panel
                    MenuPanelPermission("180230", rTabNBRReport, rpKaKha, false);
                    #endregion KaKha Panel


                }
                #endregion IsCentralBIN

                #region IsBandroll
                if (Program.IsBandroll == false)
                {
                    #region Banderol Tab
                    MenuTabPermission("210", rTabBanderol, false);
                    #endregion Banderol Tab

                    #region Banderol Panel
                    MenuPanelPermission("180210", rTabNBRReport, rpBandRoll, false);
                    #endregion Banderol Panel

                }
                #endregion IsBandroll

                #region IsTDS
                if (Program.IsTDS == false)
                {
                    #region TDS Panel
                    MenuPanelPermission("150160", rTabDeposit, rpTDS, false);
                    #endregion TDS Panel
                }
                #endregion IsTDS

                #region IsTender
                if (Program.IsTender == false)
                {
                    #region Tender Button
                    MenuButtonPermission("140110130", rpSale, ribbonButton139, false);
                    if (Program.IsTender == false && Program.IsTrading == false)
                    {
                        MenuButtonPermission("140110140", rpSale, rbnTenderTrading, false);
                        rpSale.Items.Remove(ribbonSeparator5);
                    }
                    #endregion Tender Button
                }
                #endregion IsTender

                #region IsTrading
                if (Program.IsTrading == false)
                {
                    #region NBR Report6.2.1 Panel
                    rTabNBRReport.Panels.Remove(rp6_2_1);
                    #endregion NBR Report6.2.1 Panel
                }
                #endregion IsTrading

                #region IsService
                if (Program.IsService == false)
                {
                    #region Service Button
                    MenuButtonPermission("110150120", rpPriceDeclaration, ribbonButton96, false);
                    #endregion Service Button

                    #region PurchaseService(S) Button
                    MenuButtonPermission("120110150", rpPurchase, ribbonButton53, false);
                    #endregion PurchaseService(S) Button

                    #region PurchaseService(NS) Button
                    MenuButtonPermission("120110160", rpPurchase, ribbonButton64, false);
                    #endregion PurchaseService(NS) Button

                    #region SaleService(S) Button
                    MenuButtonPermission("140110150", rpSale, ribbonButton136, false);
                    rpSale.Items.Remove(ribbonSeparator6);

                    #endregion SaleService(S) Button

                    #region SaleService(NS) Button
                    MenuButtonPermission("140110160", rpSale, ribbonButton19, false);
                    #endregion SaleService(NS) Button
                }
                #endregion IsService



                #endregion

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public bool MenuTabPermission(string FormID, RibbonTab tab, bool LicensePermission = true)
        {
            //Ruba
            bool Access = false;
            try
            {
                if (LicensePermission == true)
                {
                    DataTable dataRows = Program.UserMenuAllRolls.Select("FormID=" + "'" + FormID + "' and AccessType='tab'").CopyToDataTable();
                    if (dataRows != null && dataRows.Rows.Count > 0)
                    {
                        string AccessRoll = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, dataRows.Rows[0]["AccessRoll"].ToString()).Split('~')[1];
                        Access = AccessRoll == "1";
                    }
                    if (Access == false)
                    {
                        ribbon1.Tabs.Remove(tab);
                    }
                }
                else
                {
                    ribbon1.Tabs.Remove(tab);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Access;

        }

        #endregion

        #region Methods 02

        public bool MenuPanelPermission(string FormID, RibbonTab tab, RibbonPanel pnl, bool LicensePermission = true)
        {
            //Ruba
            bool Access = false;
            try
            {

                if (LicensePermission == true)
                {
                    DataTable dataRows = Program.UserMenuAllRolls.Select("FormID=" + "'" + FormID + "' and AccessType='Panel'").CopyToDataTable();

                    if (dataRows != null && dataRows.Rows.Count > 0)
                    {
                        string AccessRoll = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, dataRows.Rows[0]["AccessRoll"].ToString()).Split('~')[1];
                        Access = AccessRoll == "1";
                    }
                    if (Access == false)
                    {
                        tab.Panels.Remove(pnl);
                    }
                }
                else
                {
                    tab.Panels.Remove(pnl);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Access;

        }

        public bool MenuButtonPermission(string FormID, RibbonPanel pnl, RibbonButton btn, bool LicensePermission = true)
        {
            //Ruba
            bool Access = false;
            try
            {

                if (LicensePermission == true)
                {
                    DataTable dataRows = Program.UserMenuAllRolls.Select("FormID=" + "'" + FormID + "' and AccessType='Button'").CopyToDataTable();

                    if (dataRows != null && dataRows.Rows.Count > 0)
                    {
                        string EncAccessRoll = Converter.DESEncrypt(DBConstant.PassPhrase, DBConstant.EnKey, FormID + "~" + 1);
                        string AccessRoll = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, dataRows.Rows[0]["AccessRoll"].ToString()).Split('~')[1];

                        Access = AccessRoll == "1";
                    }
                    if (Access == false)
                    {
                        pnl.Items.Remove(btn);
                    }
                }

                else
                {
                    pnl.Items.Remove(btn);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Access;

        }

        private void MDIMainInterface_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (MessageBox.Show("Do you Want to Exit?", this.Text, MessageBoxButtons.YesNo) != DialogResult.Yes)
            //{
            //    return;
            //}
            //else
            //{

            //    Application.Exit();
            //}
            //LogOut();
            //LogIn();
            Application.Exit();
        }

        private void ribbonButton11_Click(object sender, EventArgs e)
        {
            FormVendorGroup frm = new FormVendorGroup();
            #region Roll
            DataTable dt = UserMenuRolls("110120110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll


            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVendorGroup)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;


            frm.Show();
        }

        private void ribbonButton13_Click(object sender, EventArgs e)
        {
            FormCustomerGroup frm = new FormCustomerGroup();
            #region Roll
            DataTable dt = UserMenuRolls("110130110");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.button1.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormCustomerGroup)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void ribbonButton14_Click(object sender, EventArgs e)
        {
            FormBankInformation frm = new FormBankInformation();
            #region Roll
            DataTable dt = UserMenuRolls("110140110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll


            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormBankInformation)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void ribbonButton15_Click(object sender, EventArgs e)
        {

        }

        private void ribbonButton17_Click(object sender, EventArgs e)
        {
            FormBOM frm = new FormBOM();

            #region Roll
            DataTable dt = UserMenuRolls("110150110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.cmdPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormBOM)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;


            frm.Show();
        }

        private void ribbonButton18_Click(object sender, EventArgs e)
        {
            FormCompanyProfile frm = new FormCompanyProfile();
            #region Roll
            DataTable dt = UserMenuRolls("110160110");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormCompanyProfile)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.btnUpdate.Visible = true;
            frm.Show();
        }

        private void ribbonButton49_Click(object sender, EventArgs e)
        {
            FormRptVAT1 frmRptVAT1 = new FormRptVAT1();
            #region Roll
            DataTable dt = UserMenuRolls("170160110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptVAT1.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptVAT1.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT1)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVAT1.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptVAT1.MdiParent = this;
            frmRptVAT1.Show();
        }

        private void ribbonButton48_Click(object sender, EventArgs e)
        {
            FormRptVAT16 frmRptVAT16 = new FormRptVAT16();
            #region Roll
            DataTable dt = UserMenuRolls("180120110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptVAT16.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptVAT16.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT16)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frmRptVAT16.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frmRptVAT16.MdiParent = this;
            frmRptVAT16.Show();
        }

        private void ribbonButton45_Click(object sender, EventArgs e)
        {
            FormRptVAT17 frmRptVAT17 = new FormRptVAT17();
            #region Roll
            DataTable dt = UserMenuRolls("180130110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptVAT17.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptVAT17.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT17)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVAT17.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frmRptVAT17.MdiParent = this;
            frmRptVAT17.Show();
        }

        private void ribbonButton44_Click(object sender, EventArgs e)
        {

        }

        private void ribbonButton43_Click(object sender, EventArgs e)
        {
            FormVAT9_1 frmRptVAT9_1 = new FormVAT9_1();
            #region Roll
            DataTable dt = UserMenuRolls("180140110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptVAT9_1.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptVAT9_1.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVAT9_1)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frmRptVAT9_1.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptVAT9_1.MdiParent = this;
            frmRptVAT9_1.Show();
        }

        #endregion

        #region Methods 03

        #region Backup


        private void ribbonButton43_ClickBackup(object sender, EventArgs e)
        {
            FormVAT19 frmRptVAT19 = new FormVAT19();

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVAT19)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVAT19.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frmRptVAT19.MdiParent = this;

            frmRptVAT19.Show();
        }
        #endregion

        private void ribbonButton57_Click(object sender, EventArgs e)
        {
            FormRptPurchaseInformation frmRptPurchaseInformation = new FormRptPurchaseInformation();
            #region Roll
            DataTable dt = UserMenuRolls("190110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptPurchaseInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptPurchaseInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frmRptPurchaseInformation.Text = "Report  ";
            frmRptPurchaseInformation.MdiParent = this;
            frmRptPurchaseInformation.Show();
        }

        private void ribbonButton56_Click(object sender, EventArgs e)
        {
            FormRptIssueInformation frmRptIssueInformation = new FormRptIssueInformation();
            #region Roll
            DataTable dt = UserMenuRolls("190120110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptIssueInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptIssueInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptIssueInformation)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptIssueInformation.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frmRptIssueInformation.MdiParent = this;
            frmRptIssueInformation.Show();
        }

        private void ribbonButton63_Click(object sender, EventArgs e)
        {
            FormUserCreate frm = new FormUserCreate();
            #region Roll
            DataTable dt = UserMenuRolls("200110110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormUserCreate)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frm.MdiParent = this;

            frm.Show();
        }

        private void ribbonButton62_Click(object sender, EventArgs e)
        {
            FormUserPasswordChange frm = new FormUserPasswordChange();
            #region Roll
            DataTable dt = UserMenuRolls("200120110");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormUserPasswordChange)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton61_Click(object sender, EventArgs e)
        {
            FormUserRoll frm = new FormUserRoll();
            #region Roll
            DataTable dt = UserMenuRolls("200130110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormUserRoll)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton68_Click(object sender, EventArgs e)
        {

                  FormPurchase frm = new FormPurchase();
                  #region Roll
                  DataTable dt = UserMenuRolls("120110110");

                  if (dt != null && dt.Rows.Count > 0)
                  {

                      if (dt.Rows[0]["Access"].ToString() != "1")
                      {
                          MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                          return;
                      }
                      frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                      frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                      frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

                  }
                  else
                  {
                      MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                      return;
                  }

                  #endregion Roll
                  frm.MdiParent = this;
                  frm.rbtnOther.Checked = true;
                  frm.Show();
              
            
        }

        private void ribbonButton76_Click(object sender, EventArgs e)
        {
            FormPurchase frmPurchaseInformationTradingLocal = new FormPurchase();

            frmPurchaseInformationTradingLocal.MdiParent = this;


            frmPurchaseInformationTradingLocal.rbtnTrading.Checked = true;


            frmPurchaseInformationTradingLocal.Show();
        }

        private void ribbonButton75_Click_1(object sender, EventArgs e)
        {
            FormFiscalYear frm = new FormFiscalYear();

            #region Roll
            DataTable dt = UserMenuRolls("110170110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormFiscalYear)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();

        }

        private void ribbonButton80_Click(object sender, EventArgs e)
        {
            FormCustomer frm = new FormCustomer();
            #region Roll
            DataTable dt = UserMenuRolls("110130120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormCustomer)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void ribbonButton12_Click(object sender, EventArgs e)
        {
            FormVendor frm = new FormVendor();
            #region Roll
            DataTable dt = UserMenuRolls("110120120");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVendor)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;


            frm.Show();
        }

        private void ribbonButton10_Click(object sender, EventArgs e)
        {
            FormProductCategory frm = new FormProductCategory();
            #region Roll
            DataTable dt = UserMenuRolls("110110110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormProductCategory)
                {
                    f.WindowState = FormWindowState.Normal;

                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frm.MdiParent = this;

            frm.Show();
        }


        public void CloaseAllForm()
        {

            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }

        }

        public DataTable UserMenuRolls(string FormID)
        {
            //Ruba
            DataTable dt = new DataTable("Roll");

            dt = Program.UserMenuRolls.Select("FormID=" + "'" + FormID + "'").CopyToDataTable();
            // dt.Columns.Add( new DataColumn (){DefaultValue="1",ColumnName="Access" });
            // dt.Columns.Add( new DataColumn (){DefaultValue="1",ColumnName="AddAccess" });
            // dt.Columns.Add( new DataColumn (){DefaultValue="1",ColumnName="EditAccess" });
            // dt.Columns.Add( new DataColumn (){DefaultValue="1",ColumnName="PostAccess" });
            //dt.Rows.Add(dt.NewRow());

            return dt;

        }

        private void ribbonButton81_Click(object sender, EventArgs e)
        {
            FormProduct frm = new FormProduct();
            #region Roll
            DataTable dt = UserMenuRolls("110110120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.cmdUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            //foreach (Form f in mdi.MdiChildren)
            //{
            //    if (f is FormProduct)
            //    {
            //        MessageBox.Show(frmProductInformation.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}
            frm.MdiParent = this;
            frm.rbtnOther.Checked = true;
            frm.Show();
        }

        #endregion

        #region Methods 04

        private void ribbonButton46_Click(object sender, EventArgs e)
        {
            FormImport frm = new FormImport();
            #region Roll
            DataTable dt = UserMenuRolls("110190110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormImport)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void ribbonPanel41_Click(object sender, EventArgs e)
        {

        }

        private void ribbonButton77_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void ribbonButton60_Click(object sender, EventArgs e)
        {
            FormRptSalesInformation frmRptSalesInformation = new FormRptSalesInformation();
            #region Roll
            DataTable dt = UserMenuRolls("190130110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptSalesInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptSalesInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frmRptSalesInformation.Text = "Report(Local Sale)";
            frmRptSalesInformation.MdiParent = this;
            frmRptSalesInformation.Show();
        }

        private void ribbonButton87_Click(object sender, EventArgs e)
        {
            FormRptStock frmRptStock = new FormRptStock();
            #region Roll
            DataTable dt = UserMenuRolls("190140110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmRptStock.MdiParent = this;
            frmRptStock.rbtnTotal.Checked = true;
            frmRptStock.Show();
        }

        private void ribbonButton88_Click(object sender, EventArgs e)
        {
            FormRptDepositTransaction frmRptDepositInformation = new FormRptDepositTransaction();
            #region Roll
            DataTable dt = UserMenuRolls("190150110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptDepositInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptDepositInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            //foreach (Form f in mdi.MdiChildren)
            //{
            //    if (f is FormRptDepositTransaction)  // Here
            //    {
            //        MessageBox.Show(frmRptDepositInformation.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}


            frmRptDepositInformation.MdiParent = this;
            frmRptDepositInformation.Show();
        }

        private void ribbonButton20_Click(object sender, EventArgs e)
        {
            FormRptAdjustment frmRptAdjustment = new FormRptAdjustment();
            #region Roll
            DataTable dt = UserMenuRolls("190160110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptAdjustment.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptAdjustment.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptAdjustment)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptAdjustment.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frmRptAdjustment.MdiParent = this;
            frmRptAdjustment.Show();

        }

        private void ribbon1_Click(object sender, EventArgs e)
        {

        }

        private void ribbonButton96_Click_1(object sender, EventArgs e)//frmServicePrice Decleration
        {
            FormService frm = new FormService();

            #region Roll
            DataTable dt = UserMenuRolls("110150120");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";
                frm.cmdPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormService)  // Here
                {

                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;


            frm.Show();
        }

        private void ribbonButton92_Click(object sender, EventArgs e)
        {
            FormRptReceiveInformation frmRptReceiveInformation = new FormRptReceiveInformation();
            #region Roll
            DataTable dt = UserMenuRolls("190120120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptReceiveInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptReceiveInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptReceiveInformation)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptReceiveInformation.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptReceiveInformation.MdiParent = this;
            frmRptReceiveInformation.Show();
        }

        private void ribbonButton89_Click_1(object sender, EventArgs e)
        {
            FormTenderPrice frm = new FormTenderPrice();

            #region Roll
            DataTable dt = UserMenuRolls("110150130");




            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";


            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormTenderPrice)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void ribbonButton8_Click_1(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void ribbonButton7_Click_2(object sender, EventArgs e)
        {
            #region Roll
            DataTable dt = UserMenuRolls("200130110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            if (MessageBox.Show("Do you Want to Exit?", this.Text, MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            else
            {
                //CommonSoapClient DBClose = new CommonSoapClient();
                //DBClose.DBClose123();
                Application.Exit();
            }
        }

        private void rbtnSettings_Click(object sender, EventArgs e)
        {
            FormSetting frm = new FormSetting();
            #region Roll
            DataTable dt = UserMenuRolls("110180110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSetting)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.rbtnSetting.Checked = true;
            frm.Show();
        }

        private void rbtnImport_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();

            #region Roll
            DataTable dt = UserMenuRolls("120110120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnImport.Checked = true;

            frm.Show();
        }

        #endregion

        #region Methods 05

        private void rbtnInputService_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("120110130");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnInputService.Checked = true;

            frm.Show();
        }

        private void ribbonButton94_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("160110110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnTollIssue.Checked = true;
            frm.Show();
        }

        private void ribbonButton95_Click_1(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("160110120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnSave.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnTollReceive.Checked = true;
            frm.Show();

            //FormReceive frmTollReceive = new FormReceive();
            //frmTollReceive.MdiParent = this;

            //frmTollReceive.rbtnTollFinishReceive.Checked = true;


            //frmTollReceive.Show();
        }

        private void ribbonButton50_Click_5(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("120110140");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnPurchaseReturn.Checked = true;
            frm.Show();
        }

        private void ribbonButton47_Click_3(object sender, EventArgs e)
        {
            FormCode frm = new FormCode();

            #region Roll
            DataTable dt = UserMenuRolls("110180120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormCode)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void ribbonButton116_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("160120110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnTollReceiveRaw.Checked = true;
            frm.Show();
        }

        private void ribbonButton118_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("160120130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnTollFinishIssue.Checked = true;
            frm.Show();
        }

        private void ribbonButton117_Click(object sender, EventArgs e)
        {
            FormReceive frm = new FormReceive();
            #region Roll
            DataTable dt = UserMenuRolls("160120120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            //foreach (Form f in mdi.MdiChildren)
            //{
            //    if (f is FormReceive)  // Here
            //    {
            //        MessageBox.Show(frmReceive.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}
            frm.rbtnTollFinishReceive.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton78_Click_1(object sender, EventArgs e)
        {
            FormAdjustmentName frm = new FormAdjustmentName();
            #region Roll
            DataTable dt = UserMenuRolls("170110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormProductCategory)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton93_Click(object sender, EventArgs e)
        {
            FormProduct frmProductInformation = new FormProduct();
            #region Roll
            DataTable dt = UserMenuRolls("110110130");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmProductInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frmProductInformation.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frmProductInformation.cmdUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frmProductInformation.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmProductInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll



            //foreach (Form f in mdi.MdiChildren)
            //{
            //    if (f is FormProduct)
            //    {
            //        MessageBox.Show(frmProductInformation.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}
            frmProductInformation.MdiParent = this;
            frmProductInformation.rbtnOverHead.Checked = true;
            frmProductInformation.Show();
        }

        private void SDReport_Click(object sender, EventArgs e)
        {
            // SD Report
            FormRptSD frmRptSD = new FormRptSD();
            #region Roll
            DataTable dt = UserMenuRolls("180160110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptSD.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptSD.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptSD)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptSD.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptSD.MdiParent = this;
            frmRptSD.Show();
        }

        private void ribbonButton98_Click(object sender, EventArgs e)
        {
            FormCurrency frm = new FormCurrency();
            #region Roll
            DataTable dt = UserMenuRolls("110210110");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.buttonUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll


            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton120_Click_1(object sender, EventArgs e)
        {
            FormCurrencyConversion frm = new FormCurrencyConversion();
            #region Roll
            DataTable dt = UserMenuRolls("110210120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.buttonUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton129_Click(object sender, EventArgs e)
        {
            FormAdjustment frm = new FormAdjustment();
            #region Roll
            DataTable dt = UserMenuRolls("170110120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormProductCategory)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton130_Click(object sender, EventArgs e)
        {
            FormIssue frm = new FormIssue();

            #region Roll
            DataTable dt = UserMenuRolls("130110110");
            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            CommonDAL commonDal = new CommonDAL();
            string vIssueFromBOM, vIssueAutoPost = string.Empty;
            vIssueFromBOM = commonDal.settings("IssueFromBOM", "IssueFromBOM");
            vIssueAutoPost = commonDal.settings("IssueFromBOM", "IssueAutoPost");
            if (string.IsNullOrEmpty(vIssueFromBOM) || string.IsNullOrEmpty(vIssueAutoPost))
            {
                MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                return;
            }

            bool IssueFromBOM = Convert.ToBoolean(vIssueFromBOM == "Y" ? true : false);

            if (IssueFromBOM == true)
            {
                bool IssueAutoPost = Convert.ToBoolean(vIssueAutoPost == "Y" ? true : false);
                if (IssueAutoPost)
                {
                    MessageBox.Show("Raw product is issue with Finish product receiving", "Issue Items",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

            }

            frm.rbtnOther.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        #endregion

        #region Methods 06

        private void ribbonButton131_Click(object sender, EventArgs e)
        {

            CommonDAL commonDal = new CommonDAL();

            string vIssueFromBOM, vIssueAutoPost = string.Empty;
            vIssueFromBOM = commonDal.settings("IssueFromBOM", "IssueFromBOM");
            vIssueAutoPost = commonDal.settings("IssueFromBOM", "IssueAutoPost");
            if (string.IsNullOrEmpty(vIssueFromBOM)

                    || string.IsNullOrEmpty(vIssueAutoPost))
            {
                MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                return;
            }

            bool IssueFromBOM = Convert.ToBoolean(vIssueFromBOM == "Y" ? true : false);

            if (IssueFromBOM == true)
            {
                bool IssueAutoPost = Convert.ToBoolean(vIssueAutoPost == "Y" ? true : false);
                if (IssueAutoPost)
                {


                    MessageBox.Show("Raw product is issue with Finish product receiving", "Issue Items",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

            }

            FormIssue frm = new FormIssue();
            #region Roll
            DataTable dt = UserMenuRolls("130110120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnIssueReturn.Checked = true;
            frm.MdiParent = this;
            frm.Show();


        }

        private void ribbonButton133_Click(object sender, EventArgs e)
        {
            FormReceive frm = new FormReceive();
            #region Roll
            DataTable dt = UserMenuRolls("130120110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnWIP.Checked = true;
            frm.MdiParent = this;

            frm.Show();
        }

        private void ribbonButton135_Click(object sender, EventArgs e)
        {

            FormReceive frm = new FormReceive();
            #region Roll
            DataTable dt = UserMenuRolls("130120120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.rbtnOther.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton132_Click(object sender, EventArgs e)
        {

            FormReceive frm = new FormReceive();
            #region Roll
            DataTable dt = UserMenuRolls("130120130");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnReceiveReturn.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton134_Click(object sender, EventArgs e)
        {
            string value = new CommonDAL().settingValue("CompanyCode", "Code");

            if (value.ToUpper() == "NESTLE")
            {
                FormSaleNestle frm = new FormSaleNestle("");
                #region Roll
                DataTable dt = UserMenuRolls("140110110");
                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Access"].ToString() != "1")
                    {
                        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                    frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                    frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

                }
                else
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion Roll
                frm.MdiParent = this;
                frm.rbtnOther.Checked = true;

                frm.Show();
            }
            else
            {
                FormSale frm = new FormSale();
                #region Roll
                DataTable dt = UserMenuRolls("140110110");
                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Access"].ToString() != "1")
                    {
                        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                    frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                    frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

                }
                else
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion Roll
                frm.MdiParent = this;
                frm.rbtnOther.Checked = true;

                frm.Show();
            }

        }

        private void ribbonButton136_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140110150");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnService.Checked = true;
            frm.Show();
        }

        private void ribbonButton137_Click(object sender, EventArgs e)
        {
            FormSale frmSalesInformationTrading = new FormSale();
            frmSalesInformationTrading.MdiParent = this;
            frmSalesInformationTrading.rbtnTrading.Checked = true;
            frmSalesInformationTrading.Show();
        }

        private void ribbonButton138_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140110120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnExport.Checked = true;
            frm.Show();
        }

        private void ribbonButton139_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140110130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnTender.Checked = true;
            frm.Show();
        }

        private void ribbonButton125_Click(object sender, EventArgs e)
        {

            string value = new CommonDAL().settingValue("CompanyCode", "Code");

            if (value.ToUpper() == "NESTLE")
            {
                FormSaleNestle frm = new FormSaleNestle("");
                #region Roll
                DataTable dt = UserMenuRolls("170130120");

                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Access"].ToString() != "1")
                    {
                        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                    frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                    frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

                }
                else
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion Roll
                frm.MdiParent = this;
                frm.rbtnDN.Checked = true;
                frm.Show();
            }
            else
            {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("170130120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnDN.Checked = true;
            frm.Show();
        }

        }

        private void ribbonButton124_Click(object sender, EventArgs e)
        {
            if (Program.IsBureau == true)
            {
                FormBureauSale frm = new FormBureauSale();
                #region Roll
                DataTable dt = UserMenuRolls("170130110");


                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Access"].ToString() != "1")
                    {
                        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                    frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                    frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

                }
                else
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion Roll
                frm.MdiParent = this;
                frm.rbtnCN.Checked = true;
                frm.Show();
            }
            else
            {
                string value = new CommonDAL().settingValue("CompanyCode", "Code");

                if (value.ToUpper() == "NESTLE")
                {
                    FormSaleNestle frm = new FormSaleNestle("");
                    #region Roll
                    DataTable dt = UserMenuRolls("170130110");

                    if (dt != null && dt.Rows.Count > 0)
                    {

                        if (dt.Rows[0]["Access"].ToString() != "1")
                        {
                            MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                        frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                        frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

                    }
            else
            {
                        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    #endregion Roll
                    frm.MdiParent = this;
                    frm.rbtnCN.Checked = true;
                    frm.Show();
                }
                else
                {
                FormSale frm = new FormSale();
                #region Roll
                DataTable dt = UserMenuRolls("170130110");

                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Access"].ToString() != "1")
                    {
                        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                    frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                    frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

                }
                else
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion Roll
                frm.MdiParent = this;
                frm.rbtnCN.Checked = true;
                frm.Show();
            }
               
            }
        }

        private void ribbonButton140_Click(object sender, EventArgs e)
        {
            FormDeposit frm = new FormDeposit();
            #region Roll
            DataTable dt = UserMenuRolls("150110110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnTreasury.Checked = true;
            frm.chkSingleTR6.Checked = false;
            frm.chkSingleTR6.Visible = false;
            frm.Show();
        }

        private void ribbonButton141_Click(object sender, EventArgs e)
        {
            FormDeposit frm = new FormDeposit();
            #region Roll
            DataTable dt = UserMenuRolls("150120110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnVDS.Checked = true;
            frm.chkPurchaseVDS.Checked = true;
            frm.Show();
        }

        private void ribbonButton143_Click(object sender, EventArgs e)
        {
            FormDeposit frm = new FormDeposit();
            #region Roll
            DataTable dt = UserMenuRolls("150130110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnSD.Checked = true;
            frm.chkSingleTR6.Checked = false;
            frm.chkSingleTR6.Visible = false;
            frm.Show();
        }

        private void ribbonButton51_Click(object sender, EventArgs e)
        {
            //FormDisposeRaw frmDisposeRaws = new FormDisposeRaw();
            //frmDisposeRaws.Show();
            FormDisposeRaw frm = new FormDisposeRaw();
            #region Roll
            DataTable dt = UserMenuRolls("170140110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                //if (f is FormProductType)
                //{
                //    MessageBox.Show(frmDisposeRaw.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
            }
            frm.MdiParent = this;
            //frm.rbtnRaw.Checked = true;

            frm.Show();

            ////FormDisposeRaw frmDisposeRaw = new FormDisposeRaw();

            ////foreach (Form f in mdi.MdiChildren)
            ////{
            ////    if (f is FormProductType)
            ////    {
            ////        MessageBox.Show(frmDisposeRaw.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ////        return;
            ////    }
            ////}
            ////frmDisposeRaw.MdiParent = this;

            ////frmDisposeRaw.Show();
        }

        private void ribbonButton52_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();

            //FormDisposeFinish frm = new FormDisposeFinish();
            #region Roll
            DataTable dt = UserMenuRolls("170140120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                //if (f is FormProductType)
                //{
                //    MessageBox.Show(frmDisposeFinish.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
            }
            frm.MdiParent = this;
            frm.rbtnDisposeRaw.Checked = true;

            frm.Show();
        }

        #endregion

        #region Methods 07

        private void ribbonButton40_Click(object sender, EventArgs e)
        {

            FormDutyDrawBack frm = new FormDutyDrawBack();
            #region Roll
            DataTable dt = UserMenuRolls("170150110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnVDB.Checked = false;
            frm.rbtnOther.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton122_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("120110140");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnPurchaseReturn.Checked = true;
            frm.Show();

            //FormPurchase frm = new FormPurchase();
            //#region Roll
            ////////DataTable dt = UserMenuRolls("170120110");
            //DataTable dt = UserMenuRolls("120110140");

            //if (dt != null && dt.Rows.Count > 0)
            //{

            //    if (dt.Rows[0]["Access"].ToString() != "1")
            //    {
            //        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //    frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
            //    frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
            //    frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            //}
            //else
            //{
            //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            //#endregion Roll

            //frm.MdiParent = this;
            //////////frm.rbtnPurchaseDN.Checked = true;
            //frm.rbtnPurchaseReturn.Checked = true;
            //frm.Show();
        }

        private void ribbonButton123_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("170120120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            ////frm.rbtnPurchaseCN.Checked = true;
            frm.rbtnPurchaseDN.Checked = true;
            frm.MdiParent = this;
            frm.Show();

        }

        private void ribbonButton16_Click(object sender, EventArgs e)
        {
            FormUOMName frm = new FormUOMName();

            #region Roll
            DataTable dt = UserMenuRolls("110200110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.buttonUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();


        }

        private void ribbonButton41_Click(object sender, EventArgs e)
        {
            FormSale frmTollIssue = new FormSale();


            frmTollIssue.rbtnInternalIssue.Checked = true;
            frmTollIssue.MdiParent = this;

            frmTollIssue.Show();
        }

        private void ribbonButton46_CanvasChanged(object sender, EventArgs e)
        {

        }

        private void ribbonButton53_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("120110150");
            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnService.Checked = true;
            frm.MdiParent = this;

            frm.Show();
        }

        private void ribbonButton64_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("120110160");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnServiceNS.Checked = true;
            frm.MdiParent = this;

            frm.Show();
        }

        private void ribbonButton19_Click_1(object sender, EventArgs e)
        {
            if (Program.IsBureau == true)
            {
                FormBureauSale frm = new FormBureauSale();
                #region Roll
                DataTable dt = UserMenuRolls("140110160");

                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Access"].ToString() != "1")
                    {
                        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                    frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                    frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

                }
                else
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion Roll
                frm.rbtnServiceNS.Checked = true;
                frm.MdiParent = this;
                frm.Show();
            }
            else
            {

                FormSale frmSalesServiceNS = new FormSale();
                #region Roll
                DataTable dt = UserMenuRolls("140110160");

                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Access"].ToString() != "1")
                    {
                        MessageBox.Show("You do not have to access this form", frmSalesServiceNS.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    frmSalesServiceNS.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                    frmSalesServiceNS.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                    frmSalesServiceNS.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

                }
                else
                {
                    MessageBox.Show("You do not have to access this form", frmSalesServiceNS.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion Roll
                frmSalesServiceNS.rbtnServiceNS.Checked = true;
                frmSalesServiceNS.MdiParent = this;
                frmSalesServiceNS.Show();
            }
        }

        private void ribbonButton65_Click(object sender, EventArgs e)
        {
            FormUOMNew frm = new FormUOMNew();
            #region Roll
            DataTable dt = UserMenuRolls("110200120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonOrbMenuItem9_Click(object sender, EventArgs e)
        {
            FormLogIn frmlLogIn = new FormLogIn();
            //frmlLogIn.MdiParent = this;
            frmlLogIn.Show();
        }

        private void ribbonButton66_Click(object sender, EventArgs e)
        {
            #region Roll
            DataTable dt = UserMenuRolls("200160110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            if (MessageBox.Show("Do you Want to Login?", this.Text, MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            else
            {
                FormLogIn frmlLogIn = new FormLogIn();
                //frmlLogIn.MdiParent = this;
                frmlLogIn.Show();
            }


        }

        private void ribbonOrbMenuItem10_Click(object sender, EventArgs e)
        {
            FormLogIn frmlLogIn = new FormLogIn();
            //frmlLogIn.MdiParent = this;
            frmlLogIn.Show();
        }

        private void ribbonButton127_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you Want to login?", this.Text, MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            else
            {
                FormLogIn frmlLogIn = new FormLogIn();
                //frmlLogIn.MdiParent = this;
                frmlLogIn.ShowDialog();
                MDIName();

            }
        }

        private void ribbonButton142_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you Want to Exit?", this.Text, MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            else
            {
                //CommonSoapClient DBClose = new CommonSoapClient();
                //DBClose.DBClose123();
                Application.Exit();
            }
        }

        #endregion

        #region Methods 08

        private void ribbonButton144_Click(object sender, EventArgs e)
        {
            #region Roll
            DataTable dt = UserMenuRolls("200180110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //tSSL7.Text = @", Current Time: " + DateTime.Now.ToString("HH:mm:ss");
            //tSSL7.Text += @", Version : " + Assembly.GetExecutingAssembly().GetName().Version;
            //tSSL7.Text = @", Server Time: " + DateTime.Now.ToString("HH:mm:ss");

            //Program.SessionTime
            // //tSSL7.Text = ", Current Time: ";
            //ff
            // //tSSL7.Text = " ";
            // tSSL7.Text =DateTime.Now.ToString("HH:mm:ss");
        }

        private void ribbonButton145_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("160110130");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnVAT11GaGa.Checked = true;
            frm.Show();
        }

        private void rbtnCashPayble_Click(object sender, EventArgs e)
        {
            FormDeposit frm = new FormDeposit();
            #region Roll
            DataTable dt = UserMenuRolls("150140110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnAdjCashPayble.Checked = true;
            frm.Show();
        }

        private void ribbonButton21_Click_1(object sender, EventArgs e)
        {
            FormSearchUserLog frmUserLogs = new FormSearchUserLog();
            #region Roll
            DataTable dt = UserMenuRolls("200170110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmUserLogs.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmUserLogs.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmUserLogs.MdiParent = this;
            frmUserLogs.Show();
        }

        private void rbnTenderTrading_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140110140");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnTradingTender.Checked = true;
            frm.Show();
        }

        private void ribbonButton23_Click(object sender, EventArgs e)
        {
            //Package Sale
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140120110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnPackageSale.Checked = true;
            frm.Show();

        }

        private void ribbonButton22_Click(object sender, EventArgs e)
        {
            ////package Production
            //FormReceive frmPackageReceive = new FormReceive();


            //frmPackageReceive.rbtnPackageProduction.Checked = true;
            //frmPackageReceive.MdiParent = this;

            //frmPackageReceive.Show();
        }

        private void ribbonButton24_Click(object sender, EventArgs e)
        {

        }

        private void ribbonButton25_Click(object sender, EventArgs e)
        {

        }

        private void ribbonButton26_Click(object sender, EventArgs e)
        {
            //multiple vat20
            FormSaleExport frm = new FormSaleExport();
            #region Roll
            DataTable dt = UserMenuRolls("180200110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Text = "VAT 20";
            frm.Show();
        }

        private void ValueChange_Click(object sender, EventArgs e)
        {

        }

        private void ribBtnSettingRole_Click(object sender, EventArgs e)
        {
            FormSetting frmseSetting = new FormSetting();
            #region Roll
            DataTable dt = UserMenuRolls("200140110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSetting)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmseSetting.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frmseSetting.MdiParent = this;
            frmseSetting.rbtnRole.Checked = true;
            frmseSetting.Text = "User Settings Role ";
            frmseSetting.Show();
        }

        private void ribBtnBanderol_Click(object sender, EventArgs e)
        {
            FormBanderolName frm = new FormBanderolName();

            #region Roll
            DataTable dt = UserMenuRolls("110220110");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormBanderolName)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribBtnPackage_Click(object sender, EventArgs e)
        {
            FormPackaging frm = new FormPackaging();

            #region Roll
            DataTable dt = UserMenuRolls("110220120");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormPackaging)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        #endregion

        #region Methods 09

        private void rbnBtnProduct_Click(object sender, EventArgs e)
        {
            FormBanderolProduct frm = new FormBanderolProduct();

            #region Roll
            DataTable dt = UserMenuRolls("110220130");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormBanderolProduct)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribBtnDemand_Click(object sender, EventArgs e)
        {
            FormDemand frm = new FormDemand();
            #region Roll
            DataTable dt = UserMenuRolls("210110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormDemand)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbnBtnBandReceive_Click(object sender, EventArgs e)
        {
            FormBanderolReceive frm = new FormBanderolReceive();
            #region Roll
            DataTable dt = UserMenuRolls("210120110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormBanderolReceive)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonBtnpackage_Click(object sender, EventArgs e)
        {
            //package Production
            FormReceive frm = new FormReceive();
            #region Roll
            DataTable dt = UserMenuRolls("130120140");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnPackageProduction.Checked = true;
            frm.MdiParent = this;

            frm.Show();
        }

        private void ribBtnForm4_Click(object sender, EventArgs e)
        {
            FormRptForm4 frmRptForm4 = new FormRptForm4();
            #region Roll
            DataTable dt = UserMenuRolls("180210110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptForm4.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptForm4.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmRptForm4.MdiParent = this;
            //frmRptForm4.Text = "Multiple VAT 20";
            frmRptForm4.Show();
        }

        private void rbtnForm5_Click(object sender, EventArgs e)
        {
            FormRptForm5 frmRptForm5 = new FormRptForm5();
            frmRptForm5.MdiParent = this;
            frmRptForm5.Show();
        }

        private void rbtnRecForm5_Click(object sender, EventArgs e)
        {
            FormRptForm5 frmRptForm5 = new FormRptForm5();
            #region Roll
            DataTable dt = UserMenuRolls("180210120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptForm5.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptForm5.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmRptForm5.MdiParent = this;
            frmRptForm5.Show();
        }

        private void ribBtnVAT11_Click(object sender, EventArgs e)
        {
            FormMultipleVAT11 frmRptVAT11 = new FormMultipleVAT11();
            #region Roll
            DataTable dt = UserMenuRolls("180170110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptVAT11.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptVAT11.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT17)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVAT11.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptVAT11.FormType = "Sale";

            frmRptVAT11.MdiParent = this;
            frmRptVAT11.Show();
        }

        private void ribBtnVAT7_Click(object sender, EventArgs e)
        {
            FormVAT7 frmVAT7 = new FormVAT7();
            #region Roll
            DataTable dt = UserMenuRolls("180190110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmVAT7.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frmVAT7.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frmVAT7.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frmVAT7.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmVAT7.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVAT7)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmVAT7.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frmVAT7.MdiParent = this;
            frmVAT7.Show();
        }

        private void ribBtnTransfer_Click(object sender, EventArgs e)
        {
            FormTransferRaw frm = new FormTransferRaw();
            #region Roll
            DataTable dt = UserMenuRolls("130110150");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormTransferRaw)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frm.MdiParent = this;
            frm.Show();
        }

        private void ribBtnToll_Click(object sender, EventArgs e)
        {

        }

        private void ribBtnReverse_Click(object sender, EventArgs e)
        {
            FormReverseAdjustment frm = new FormReverseAdjustment();
            #region Roll
            DataTable dt = UserMenuRolls("150150110");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButtonCVAT18_Click(object sender, EventArgs e)
        {
            // VAT 18 Summery 
            FormRptCurrentVAT18 frmRptCurrentVAT18 = new FormRptCurrentVAT18();
            #region Roll
            DataTable dt = UserMenuRolls("180220110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptCurrentVAT18.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptCurrentVAT18.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptCurrentVAT18)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptCurrentVAT18.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptCurrentVAT18.MdiParent = this;
            frmRptCurrentVAT18.Show();
        }

        private void ribbonButtonSerialStock_Click(object sender, EventArgs e)
        {

        }

        private void rbtnPurchaseLC_Click(object sender, EventArgs e)
        {

        }

        private void ribbonButton22_Click_1(object sender, EventArgs e)
        {

        }

        #endregion

        #region Methods 10

        private void rbSaleCE_Click(object sender, EventArgs e)
        {
            //FormRptSalesInformation frmRptSalesInformation = new FormRptSalesInformation();
            //frmRptSalesInformation.rbtnCE.Checked = true;
            //frmRptSalesInformation.Text = "Report(Local Sale With Chassis & Engine)";


            //frmRptSalesInformation.MdiParent = this;

            //frmRptSalesInformation.Show();
        }

        private void ribbonButton27_Click(object sender, EventArgs e)
        {
            FormRptSalesInformation frmRptSalesInformation = new FormRptSalesInformation();
            #region Roll
            DataTable dt = UserMenuRolls("190170110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptSalesInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptSalesInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmRptSalesInformation.rbtnCE.Checked = true;
            frmRptSalesInformation.Text = "Report(Local Sale With Chassis & Engine)";
            frmRptSalesInformation.MdiParent = this;
            frmRptSalesInformation.Show();
        }

        private void rbtnMIS19_Click(object sender, EventArgs e)
        {
            FormMIS19 frm = new FormMIS19();
            #region Roll
            DataTable dt = UserMenuRolls("190180110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            //frm.Text = "Report(Local Sale With Chassis & Engine)";
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribBtnCommercialImporter_Click(object sender, EventArgs e)
        {
            FormSale frmSalesInformationStock = new FormSale();

            frmSalesInformationStock.MdiParent = this;
            frmSalesInformationStock.rbtnCommercialImporter.Checked = true;
            frmSalesInformationStock.Show();
        }

        private void btnCheckStock_Click(object sender, EventArgs e)
        {
            FormCheckStock frm = new FormCheckStock();

            frm.MdiParent = this;

            frm.Show();
        }

        private void ribbonButton30_Click(object sender, EventArgs e)
        {
            FormUpdatePurchaseDuty frm = new FormUpdatePurchaseDuty();

            frm.MdiParent = this;

            frm.Show();
        }

        private void MDIMainInterface_Activated(object sender, EventArgs e)
        {
            //CommonDAL commonDal = new CommonDAL();
            //string vCommercialImporter = commonDal.settings("Sale", "CommercialImporter");
            //if (vCommercialImporter.ToLower() != "y")
            //{
            //    rTabPurchase.Panels.Remove(ribbonPanelImportDuty);
            //}
        }

        private void rTabPurchase_MouseEnter(object sender, MouseEventArgs e)
        {
            CommonDAL commonDal = new CommonDAL();
            SettingDAL settingDal = new SettingDAL();
            settingDal.settingsDataInsert("CommercialImporter", "CommercialImporter", "bool", "N", null, null);

            string vCommercialImporter = commonDal.settings("CommercialImporter", "CommercialImporter");
            rTabPurchase.Panels.Remove(rpImportDuty);
            if (vCommercialImporter.ToLower() == "n")
            {
                rTabPurchase.Panels.Remove(rpImportDuty);
            }
            else
            {
                rTabPurchase.Panels.Add(rpImportDuty);
            }
        }

        private void rbtnReceiveSale_Click(object sender, EventArgs e)
        {
            FormRptSaleReceive frmRptStock = new FormRptSaleReceive();
            #region Roll
            DataTable dt = UserMenuRolls("190140110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmRptStock.MdiParent = this;
            frmRptStock.Show();
        }

        private void rbtnShift_Click(object sender, EventArgs e)
        {
            FormShifts frm = new FormShifts();
            #region Roll
            DataTable dt = UserMenuRolls("110180130");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnRawSale_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140110170");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnRawSale.Checked = true;
            frm.Show();
        }

        private void rbnCommercialImporter_Click(object sender, EventArgs e)
        {
            FormPurchase frmPurchaseImport = new FormPurchase();
            frmPurchaseImport.MdiParent = this;


            frmPurchaseImport.rbtnCommercialImporter.Checked = true;
            frmPurchaseImport.Show();
        }

        private void rbnCommercialImporterSale_Click(object sender, EventArgs e)
        {
            FormSale frmSalesInformationStock = new FormSale();

            frmSalesInformationStock.MdiParent = this;
            frmSalesInformationStock.rbtnCommercialImporter.Checked = true;
            frmSalesInformationStock.Show();
        }

        private void aAToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Methods 11

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void vendorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ribbonButton35_Click(object sender, EventArgs e)
        {
            FormSync frmSync = new FormSync();
            #region Roll
            DataTable dt = UserMenuRolls("110190120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSync.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSync.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSync)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmSync.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frmSync.MdiParent = this;

            frmSync.Show();
        }

        private void rbtnCurrentAccount_DoubleClick(object sender, EventArgs e)
        {

        }

        private void rbtnCurrentAccount_Click(object sender, EventArgs e)
        {
            //VAT18
            FormRptVAT18 frmRptVAT18 = new FormRptVAT18();
            #region Roll
            DataTable dt = UserMenuRolls("190150120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptVAT18.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptVAT18.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT18)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVAT18.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frmRptVAT18.MdiParent = this;

            frmRptVAT18.Show();
        }

        private void rbtnBrakDown_Click(object sender, EventArgs e)
        {
            //VAT18Breakdow
            FormRptVAT18Breakdown frm = new FormRptVAT18Breakdown();
            #region Roll
            DataTable dt = UserMenuRolls("180220120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT18Breakdown)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtn61IN_Click(object sender, EventArgs e)
        {
            FormTransferReceive frm = new FormTransferReceive();
            #region Roll
            DataTable dt = UserMenuRolls("140130110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtn61In.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtn62IN_Click(object sender, EventArgs e)
        {
            FormTransferReceive frm = new FormTransferReceive();
            #region Roll
            DataTable dt = UserMenuRolls("140130120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtn62In.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtn61Out_Click(object sender, EventArgs e)
        {

        }

        private void rbtn62Out_Click(object sender, EventArgs e)
        {

        }

        private void rbtnBranchTransfer_Click(object sender, EventArgs e)
        {

            FormBranchProfiles frm = new FormBranchProfiles();
            #region Roll
            DataTable dt = UserMenuRolls("110160120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormBranchProfiles)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frm.MdiParent = this;
            frm.btnUpdate.Visible = true;

            frm.Show();


        }

        private void rbtnBranchReport_Click(object sender, EventArgs e)
        {

            FormBranchReport frm = new FormBranchReport();
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormBranchReport)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frm.MdiParent = this;


            frm.Show();

        }

        private void rbtn6_10_Click(object sender, EventArgs e)
        {
            FormVAT6_10 frm = new FormVAT6_10();
            #region Roll
            DataTable dt = UserMenuRolls("180150110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVAT6_10)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtn65_Click(object sender, EventArgs e)
        {
            //FormRptVAT6_5 frm = new FormRptVAT6_5();
            FormMultipleVAT6_5 frm = new FormMultipleVAT6_5();
            #region Roll
            DataTable dt = UserMenuRolls("180180110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT6_5)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frm.MdiParent = this;
            frm.Show();

        }

        private void ribbonButton32_Click(object sender, EventArgs e)
        {
            FormDBBackupRestore frm = new FormDBBackupRestore();
            frm.MdiParent = this;

            frm.Show();

        }

        #endregion

        #region Methods 12

        private void rbtnExecute_Click(object sender, EventArgs e)
        {

           
            CommonDAL commonDal = new CommonDAL();
            string OldProcess = commonDal.settingValue("DayEnd", "OldProcess");
            // FormDBAccess


            if (OldProcess == "Y")
            {
                FormDBAccess frm = new FormDBAccess();

                #region Roll
                DataTable dt = UserMenuRolls("110190130");


                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Access"].ToString() != "1")
                    {
                        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                }
                else
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion Roll

                foreach (Form f in mdi.MdiChildren)
                {
                    if (f is FormSync)  // Here
                    {
                        f.WindowState = FormWindowState.Normal;
                        f.Activate();

                        ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                frm.MdiParent = this;

                frm.Show();
            }
            else
            {

                var IsSymphonyUser = FormSupperAdministrator.SelectOne();
                if (!IsSymphonyUser)
                {
                    MessageBox.Show("Wrong username or password");
                    return;
                }


                FormPermanentProcess frm = new FormPermanentProcess();

                #region Roll
                DataTable dt = UserMenuRolls("110190130");


                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Access"].ToString() != "1")
                    {
                        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                }
                else
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion Roll

                foreach (Form f in mdi.MdiChildren)
                {
                    if (f is FormSync)  // Here
                    {
                        f.WindowState = FormWindowState.Normal;
                        f.Activate();

                        ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                frm.MdiParent = this;

                frm.Show();
            }


            
        }

        private void ribbonButton36_Click(object sender, EventArgs e)
        {
            FormUserBranchDetail frm = new FormUserBranchDetail();
            #region Roll
            DataTable dt = UserMenuRolls("200190110");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnSave.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnRemove.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSetting)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Text = "User Branch ";
            frm.Show();
        }

        private void MDIMainInterface_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F6))
            {
                FormLoginBranch frmlLogIn = new FormLoginBranch();
                frmlLogIn.ShowDialog();
            }
        }

        private void rbtnLoginBranch_Click(object sender, EventArgs e)
        {
            FormLoginBranch frmlLogIn = new FormLoginBranch();
            frmlLogIn.ShowDialog();
            MDIName();
        }

        private void rbtnToll6_3_Click(object sender, EventArgs e)
        {
            FormToll6_3Invoice frm = new FormToll6_3Invoice();
            #region Roll
            DataTable dt = UserMenuRolls("160120140");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnContractor63.Checked = true;
            frm.Show();
        }

        private void rbtnToll6_1_Click(object sender, EventArgs e)
        {
            FormRptVAT16Toll frm = new FormRptVAT16Toll();
            #region Roll
            DataTable dt = UserMenuRolls("160130110");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT16Toll)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnToll6_2_Click(object sender, EventArgs e)
        {
            FormRptVAT17Toll frm = new FormRptVAT17Toll();
            #region Roll
            DataTable dt = UserMenuRolls("160130120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT17)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnVehicle_Click(object sender, EventArgs e)
        {
            FormVehicle frm = new FormVehicle();

            #region Roll
            DataTable dt = UserMenuRolls("110140120");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVehicle)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void rbtnTDS_Click(object sender, EventArgs e)
        {
            FormDepositTDS frm = new FormDepositTDS();
            #region Roll
            DataTable dt = UserMenuRolls("150160110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.rbtnTDS.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButtonChakKa_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("180230110");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "ChakKa";
            frmSCBL.Show();
            ////frmSCBL.ShowDialog();
        }

        private void ribbonButtonChakKha_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("180230120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "ChakKha";
            frmSCBL.Show();
            ////frmSCBL.ShowDialog();
        }

        private void ribbonButtonCoEfficient_Click(object sender, EventArgs e)
        {
            FormRptInputOutputCoEfficientReport frm = new FormRptInputOutputCoEfficientReport();
            #region Roll
            DataTable dt = UserMenuRolls("190160120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton25_CanvasChanged(object sender, EventArgs e)
        {

        }

        private void ribbonButtonWastage_Click(object sender, EventArgs e)
        {
            FormRptStock frmRptStock = new FormRptStock();
            #region Roll
            DataTable dt = UserMenuRolls("190160130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmRptStock.MdiParent = this;
            frmRptStock.rbtnWeastage.Checked = true;
            frmRptStock.Text = "Wastage Report";
            frmRptStock.Show();
        }

        #endregion

        #region Methods 13

        private void ribbonButtonPurchaseLC_Click(object sender, EventArgs e)
        {
            FormRptPurchaseWithLC frm = new FormRptPurchaseWithLC();
            #region Roll
            DataTable dt = UserMenuRolls("190160160");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButtonSerStock_Click(object sender, EventArgs e)
        {
            FormRptSerialStockInformation frm = new FormRptSerialStockInformation();
            #region Roll
            DataTable dt = UserMenuRolls("190160150");
            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButtonValueChange_Click(object sender, EventArgs e)
        {
            //Input value change
            FormRptFinishProductInformation frmFinishProductSearch = new FormRptFinishProductInformation();
            #region Roll
            DataTable dt = UserMenuRolls("190160140");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmFinishProductSearch.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmFinishProductSearch.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmFinishProductSearch.MdiParent = this;
            frmFinishProductSearch.Text = "Report(Input Value Change)";
            frmFinishProductSearch.Show();
        }

        private void rbtnTDS1_Click(object sender, EventArgs e)
        {
            FormTDSs frm = new FormTDSs();
            #region Roll
            DataTable dt = UserMenuRolls("110110140");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormTDSs)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void rbtnHSCode_Click(object sender, EventArgs e)
        {
            FormHsCode frm = new FormHsCode();
            #region Roll
            DataTable dt = UserMenuRolls("110110150");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormHsCode)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void rbtn61Out1_Click(object sender, EventArgs e)
        {
            FormTransferIssue frm = new FormTransferIssue();
            #region Roll
            DataTable dt = UserMenuRolls("140130130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtn61Out.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtn62Out1_Click(object sender, EventArgs e)
        {

            FormTransferIssue frm = new FormTransferIssue();
            #region Roll
            //DataTable dt = UserMenuRolls("140130140");
            DataTable dt = UserMenuRolls("140130140");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtn62Out.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton15_Click_1(object sender, EventArgs e)
        {
            FormSalesInvoiceExp frm = new FormSalesInvoiceExp();
            #region Roll
            DataTable dt = UserMenuRolls("140140110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.Show();
        }

        private void rbtnSaleVDS_Click(object sender, EventArgs e)
        {
            FormDeposit frm = new FormDeposit();
            #region Roll
            DataTable dt = UserMenuRolls("150120120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnVDS.Checked = true;
            frm.chkPurchaseVDS.Checked = false;
            frm.Show();
        }

        private void rbtnVDB_Click(object sender, EventArgs e)
        {

        }

        private void rbtnVATAdjustment_Click(object sender, EventArgs e)
        {
            FormDutyDrawBack frm = new FormDutyDrawBack();
            #region Roll
            DataTable dt = UserMenuRolls("170160110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.rbtnOther.Checked = false;
            frm.rbtnVDB.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnSDAdjustment_Click(object sender, EventArgs e)
        {
            FormDutyDrawBack frm = new FormDutyDrawBack();
            #region Roll
            DataTable dt = UserMenuRolls("170160120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnSDB.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnReconsciliation_Click(object sender, EventArgs e)
        {
            FormRptReconsciliation frmHsCode = new FormRptReconsciliation();
            #region Roll
            DataTable dt = UserMenuRolls("190140130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmHsCode.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmHsCode.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmHsCode.MdiParent = this;
            frmHsCode.Show();
        }

        private void rbtnBranchStockMovement_Click(object sender, EventArgs e)
        {
            FormBranchStockMovement frm = new FormBranchStockMovement();
            #region Roll
            DataTable dt = UserMenuRolls("190140140");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        #endregion

        #region Methods 14

        private void itemGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormProductCategory frm = new FormProductCategory();
            #region Roll
            DataTable dt = UserMenuRolls("110110110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormProductCategory)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frm.MdiParent = this;

            frm.Show();
        }

        private void productItemToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormProduct frm = new FormProduct();
            #region Roll
            DataTable dt = UserMenuRolls("110110120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.cmdUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            //foreach (Form f in mdi.MdiChildren)
            //{
            //    if (f is FormProduct)
            //    {
            //        MessageBox.Show(frmProductInformation.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}
            frm.MdiParent = this;
            frm.rbtnOther.Checked = true;
            frm.Show();
        }

        private void itemOverheadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormProduct frmProductInformation = new FormProduct();
            #region Roll
            DataTable dt = UserMenuRolls("110110130");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmProductInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frmProductInformation.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frmProductInformation.cmdUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frmProductInformation.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmProductInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll



            //foreach (Form f in mdi.MdiChildren)
            //{
            //    if (f is FormProduct)
            //    {
            //        MessageBox.Show(frmProductInformation.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}
            frmProductInformation.MdiParent = this;
            frmProductInformation.rbtnOverHead.Checked = true;
            frmProductInformation.Show();
        }

        private void tDSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTDSs frm = new FormTDSs();
            #region Roll
            DataTable dt = UserMenuRolls("110110140");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormTDSs)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void hSCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormHsCode frm = new FormHsCode();
            #region Roll
            DataTable dt = UserMenuRolls("110110150");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormHsCode)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void vendorGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormVendorGroup frm = new FormVendorGroup();
            #region Roll
            DataTable dt = UserMenuRolls("110120110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll


            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVendorGroup)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;


            frm.Show();
        }

        private void vendorToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FormVendor frm = new FormVendor();
            #region Roll
            DataTable dt = UserMenuRolls("110120120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVendor)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;


            frm.Show();
        }

        private void customerGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCustomerGroup frm = new FormCustomerGroup();
            #region Roll
            DataTable dt = UserMenuRolls("110130110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.button1.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormCustomerGroup)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void customerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCustomer frm = new FormCustomer();
            #region Roll
            DataTable dt = UserMenuRolls("110130120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormCustomer)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //FormBankInformation frmBankInformation = new FormBankInformation();

            //foreach (Form f in mdi.MdiChildren)
            //{
            //    if (f is FormBankInformation)  // Here
            //    {
            //        MessageBox.Show(frmBankInformation.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}
            //frmBankInformation.MdiParent = this;

            //frmBankInformation.Show();
        }

        private void vehicleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormVehicle frmVehicleInformation = new FormVehicle();
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVehicle)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frmVehicleInformation.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frmVehicleInformation.MdiParent = this;

            frmVehicleInformation.Show();
        }

        private void bankToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void vAT1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBOM frm = new FormBOM();

            #region Roll
            DataTable dt = UserMenuRolls("110150110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.cmdPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormBOM)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;


            frm.Show();
        }

        private void serviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormService frm = new FormService();

            #region Roll
            DataTable dt = UserMenuRolls("110150120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";
                frm.cmdPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormService)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void tenderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTenderPrice frm = new FormTenderPrice();

            #region Roll
            DataTable dt = UserMenuRolls("110150130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";


            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormTenderPrice)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void companynProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCompanyProfile frm = new FormCompanyProfile();
            #region Roll
            DataTable dt = UserMenuRolls("110160110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormCompanyProfile)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.btnUpdate.Visible = true;
            frm.Show();
        }

        #endregion

        #region Methods 15

        private void branchProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBranchProfiles frm = new FormBranchProfiles();
            #region Roll
            DataTable dt = UserMenuRolls("110160120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormBranchProfiles)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frm.MdiParent = this;
            frm.Show();

        }

        private void fiscalYearToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormFiscalYear frm = new FormFiscalYear();

            #region Roll
            DataTable dt = UserMenuRolls("110170110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormFiscalYear)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSetting frm = new FormSetting();
            #region Roll
            DataTable dt = UserMenuRolls("110180110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSetting)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.rbtnSetting.Checked = true;
            frm.Show();
        }

        private void prefixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCode frm = new FormCode();

            #region Roll
            DataTable dt = UserMenuRolls("110180120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormCode)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void shiftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormShifts frm = new FormShifts();
            #region Roll
            DataTable dt = UserMenuRolls("110180130");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void uOMNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUOMName frm = new FormUOMName();

            #region Roll
            DataTable dt = UserMenuRolls("110200110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.buttonUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();

        }

        private void uOMConversionToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormUOMNew frm = new FormUOMNew();
            #region Roll
            DataTable dt = UserMenuRolls("110200120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void currencyNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCurrency frm = new FormCurrency();
            #region Roll
            DataTable dt = UserMenuRolls("110210110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.buttonUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void currencyConversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCurrencyConversion frm = new FormCurrencyConversion();
            #region Roll
            DataTable dt = UserMenuRolls("110210120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.buttonUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void importToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            FormImport frm = new FormImport();
            #region Roll
            DataTable dt = UserMenuRolls("110190110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormImport)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    //MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void synchronizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSync frmSync = new FormSync();
            #region Roll
            DataTable dt = UserMenuRolls("110190120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSync.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSync.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSync)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmSync.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frmSync.MdiParent = this;

            frmSync.Show();
        }

        private void dBBackUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDBAccess frm = new FormDBAccess();
            #region Roll
            DataTable dt = UserMenuRolls("110190130");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSync)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void localToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPurchase frmPurchaseInformationLocal = new FormPurchase();
            frmPurchaseInformationLocal.MdiParent = this;
            frmPurchaseInformationLocal.rbtnOther.Checked = true;
            frmPurchaseInformationLocal.Show();
        }

        private void importToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();

            #region Roll
            DataTable dt = UserMenuRolls("120110120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnImport.Checked = true;

            frm.Show();
        }

        private void inputServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("120110130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnInputService.Checked = true;
            frm.Show();
        }

        #endregion

        #region Methods 16

        private void returnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("120110140");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnPurchaseReturn.Checked = true;
            frm.Show();
        }

        private void serviceStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("120110150");
            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnService.Checked = true;
            frm.MdiParent = this;

            frm.Show();
        }

        private void serviceNStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("120110160");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnServiceNS.Checked = true;
            frm.MdiParent = this;

            frm.Show();
        }

        private void importDutyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //FormUpdatePurchaseDuty frm = new FormUpdatePurchaseDuty();

            //frm.MdiParent = this;

            //frm.Show();
        }

        private void issueToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormIssue frm = new FormIssue();
            #region Roll
            DataTable dt = UserMenuRolls("130110110");
            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            CommonDAL commonDal = new CommonDAL();
            string vIssueFromBOM, vIssueAutoPost = string.Empty;
            vIssueFromBOM = commonDal.settings("IssueFromBOM", "IssueFromBOM");
            vIssueAutoPost = commonDal.settings("IssueFromBOM", "IssueAutoPost");
            if (string.IsNullOrEmpty(vIssueFromBOM)

                    || string.IsNullOrEmpty(vIssueAutoPost))
            {
                MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                return;
            }

            bool IssueFromBOM = Convert.ToBoolean(vIssueFromBOM == "Y" ? true : false);

            if (IssueFromBOM == true)
            {
                bool IssueAutoPost = Convert.ToBoolean(vIssueAutoPost == "Y" ? true : false);
                if (IssueAutoPost)
                {


                    MessageBox.Show("Raw product is issue with Finish product receiving", "Issue Items",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

            }

            frm.rbtnOther.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void returnToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CommonDAL commonDal = new CommonDAL();

            string vIssueFromBOM, vIssueAutoPost = string.Empty;
            vIssueFromBOM = commonDal.settings("IssueFromBOM", "IssueFromBOM");
            vIssueAutoPost = commonDal.settings("IssueFromBOM", "IssueAutoPost");
            if (string.IsNullOrEmpty(vIssueFromBOM)

                    || string.IsNullOrEmpty(vIssueAutoPost))
            {
                MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                return;
            }

            bool IssueFromBOM = Convert.ToBoolean(vIssueFromBOM == "Y" ? true : false);

            if (IssueFromBOM == true)
            {
                bool IssueAutoPost = Convert.ToBoolean(vIssueAutoPost == "Y" ? true : false);
                if (IssueAutoPost)
                {


                    MessageBox.Show("Raw product is issue with Finish product receiving", "Issue Items",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

            }

            FormIssue frm = new FormIssue();
            #region Roll
            DataTable dt = UserMenuRolls("130110120");
            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnIssueReturn.Checked = true;
            frm.MdiParent = this;
            frm.Show();

        }

        private void transferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTransferRaw frm = new FormTransferRaw();
            #region Roll
            DataTable dt = UserMenuRolls("130110150");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormTransferRaw)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frm.MdiParent = this;
            frm.Show();
        }

        private void wIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReceive frm = new FormReceive();
            #region Roll
            DataTable dt = UserMenuRolls("130120110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnWIP.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void fGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReceive frm = new FormReceive();
            #region Roll
            DataTable dt = UserMenuRolls("130120120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.rbtnOther.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void returnToolStripMenuItem2_Click(object sender, EventArgs e)
        {


            FormReceive frm = new FormReceive();
            #region Roll
            DataTable dt = UserMenuRolls("130120130");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnReceiveReturn.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void localToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormSale frmSalesInformationStock = new FormSale();

            frmSalesInformationStock.MdiParent = this;
            frmSalesInformationStock.rbtnOther.Checked = true;
            frmSalesInformationStock.Show();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140110120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnExport.Checked = true;
            frm.Show();
        }

        private void tenderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140110130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnTender.Checked = true;
            frm.Show();
        }

        private void tenderTradingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140110140");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnTradingTender.Checked = true;
            frm.Show();
        }

        private void serviceStockToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140110150");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnService.Checked = true;
            frm.Show();
        }

        private void serviceNStockToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Program.IsBureau == true)
            {
                FormBureauSale frm = new FormBureauSale();
                #region Roll
                DataTable dt = UserMenuRolls("140110160");

                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Access"].ToString() != "1")
                    {
                        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                    frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                    frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

                }
                else
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion Roll
                frm.rbtnServiceNS.Checked = true;
                frm.MdiParent = this;
                frm.Show();
            }
            else
            {

                FormSale frmSalesServiceNS = new FormSale();
                #region Roll
                DataTable dt = UserMenuRolls("140110160");

                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Access"].ToString() != "1")
                    {
                        MessageBox.Show("You do not have to access this form", frmSalesServiceNS.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    frmSalesServiceNS.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                    frmSalesServiceNS.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                    frmSalesServiceNS.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

                }
                else
                {
                    MessageBox.Show("You do not have to access this form", frmSalesServiceNS.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion Roll
                frmSalesServiceNS.rbtnServiceNS.Checked = true;
                frmSalesServiceNS.MdiParent = this;
                frmSalesServiceNS.Show();
            }
        }

        private void rawSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140110170");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnRawSale.Checked = true;
            frm.Show();
        }

        #endregion

        #region Methods 17

        private void packageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Package Sale
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140120110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnPackageSale.Checked = true;
            frm.Show();
        }

        private void treasuryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDeposit frm = new FormDeposit();
            #region Roll
            DataTable dt = UserMenuRolls("150110110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnTreasury.Checked = true;
            frm.chkSingleTR6.Checked = false;
            frm.chkSingleTR6.Visible = false;
            frm.Show();
        }

        private void purchageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDeposit frm = new FormDeposit();
            #region Roll
            DataTable dt = UserMenuRolls("150120110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnVDS.Checked = true;
            frm.chkPurchaseVDS.Checked = true;
            frm.Show();
        }

        private void saleToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            FormDeposit frm = new FormDeposit();
            #region Roll
            DataTable dt = UserMenuRolls("150120120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnVDS.Checked = true;
            frm.chkPurchaseVDS.Checked = false;
            frm.Show();
        }

        private void sDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDeposit frm = new FormDeposit();
            #region Roll
            DataTable dt = UserMenuRolls("150130110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnSD.Checked = true;
            frm.chkSingleTR6.Checked = false;
            frm.chkSingleTR6.Visible = false;
            frm.Show();
        }

        private void reverseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReverseAdjustment frm = new FormReverseAdjustment();
            #region Roll
            DataTable dt = UserMenuRolls("150150110");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void cashPaybleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDeposit frm = new FormDeposit();
            #region Roll
            DataTable dt = UserMenuRolls("150140110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnAdjCashPayble.Checked = true;
            frm.Show();
        }

        private void tDSToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormDepositTDS frm = new FormDepositTDS();
            #region Roll
            DataTable dt = UserMenuRolls("150160110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.rbtnTDS.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void eXPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSalesInvoiceExp frm = new FormSalesInvoiceExp();
            #region Roll
            DataTable dt = UserMenuRolls("140140110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.Show();
        }

        private void issue11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("160110110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnTollIssue.Checked = true;
            frm.Show();
        }

        private void fGReceiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("160110120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnSave.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnTollReceive.Checked = true;
            frm.Show();

            //FormReceive frmTollReceive = new FormReceive();
            //frmTollReceive.MdiParent = this;

            //frmTollReceive.rbtnTollFinishReceive.Checked = true;


            //frmTollReceive.Show();
        }

        private void itemReceiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("160120110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnTollReceiveRaw.Checked = true;
            frm.Show();
        }

        private void fGProductionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReceive frm = new FormReceive();
            #region Roll
            DataTable dt = UserMenuRolls("160120120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            //foreach (Form f in mdi.MdiChildren)
            //{
            //    if (f is FormReceive)  // Here
            //    {
            //        MessageBox.Show(frmReceive.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}
            frm.rbtnTollFinishReceive.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void fGIssueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("160120130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnTollFinishIssue.Checked = true;
            frm.Show();
        }

        private void toll63ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormToll6_3Invoice frm = new FormToll6_3Invoice();
            #region Roll
            DataTable dt = UserMenuRolls("160120140");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void toll6_1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRptVAT16Toll frm = new FormRptVAT16Toll();
            #region Roll
            DataTable dt = UserMenuRolls("160130110");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT16)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void toll6_2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRptVAT17Toll frm = new FormRptVAT17Toll();
            #region Roll
            DataTable dt = UserMenuRolls("160130120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT17)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void headToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormAdjustmentName frm = new FormAdjustmentName();
            #region Roll
            DataTable dt = UserMenuRolls("170110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormProductCategory)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        #endregion

        #region Methods 18

        private void transactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAdjustment frm = new FormAdjustment();
            #region Roll
            DataTable dt = UserMenuRolls("170110120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormProductCategory)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("170120110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnPurchaseDN.Checked = true;
            frm.Show();
        }

        private void deductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("170120120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnPurchaseCN.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void creditNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.IsBureau == true)
            {
                FormBureauSale frm = new FormBureauSale();
                #region Roll
                DataTable dt = UserMenuRolls("170130110");


                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Access"].ToString() != "1")
                    {
                        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                    frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                    frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

                }
                else
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion Roll
                frm.MdiParent = this;
                frm.rbtnCN.Checked = true;
                frm.Show();
            }
            else
            {

                FormSale frm = new FormSale();
                #region Roll
                DataTable dt = UserMenuRolls("170130110");

                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["Access"].ToString() != "1")
                    {
                        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                    frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                    frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

                }
                else
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion Roll
                frm.MdiParent = this;
                frm.rbtnCN.Checked = true;
                frm.Show();
            }
        }

        private void debitNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("170130120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnDN.Checked = true;
            frm.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            FormDisposeFinish frm = new FormDisposeFinish();
            #region Roll
            DataTable dt = UserMenuRolls("170140110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                //if (f is FormProductType)
                //{
                //    MessageBox.Show(frmDisposeRaw.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
            }
            frm.MdiParent = this;
            frm.rbtnRaw.Checked = true;

            frm.Show();

            ////FormDisposeRaw frmDisposeRaw = new FormDisposeRaw();

            ////foreach (Form f in mdi.MdiChildren)
            ////{
            ////    if (f is FormProductType)
            ////    {
            ////        MessageBox.Show(frmDisposeRaw.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ////        return;
            ////    }
            ////}
            ////frmDisposeRaw.MdiParent = this;

            ////frmDisposeRaw.Show();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            FormDisposeFinish frm = new FormDisposeFinish();
            #region Roll
            DataTable dt = UserMenuRolls("170140120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                //if (f is FormProductType)
                //{
                //    MessageBox.Show(frmDisposeFinish.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
            }
            frm.MdiParent = this;
            frm.rbtnFinish.Checked = true;
            frm.Show();
        }

        private void dDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDutyDrawBack frm = new FormDutyDrawBack();
            #region Roll
            DataTable dt = UserMenuRolls("170150110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnVDB.Checked = false;
            frm.rbtnOther.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void vATToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDutyDrawBack frm = new FormDutyDrawBack();
            #region Roll
            DataTable dt = UserMenuRolls("170160110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.rbtnOther.Checked = false;
            frm.rbtnVDB.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void sDToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormDutyDrawBack frm = new FormDutyDrawBack();
            #region Roll
            DataTable dt = UserMenuRolls("170160120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnSDB.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void vAT1ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            FormRptVAT1 frmRptVAT1 = new FormRptVAT1();
            #region Roll
            DataTable dt = UserMenuRolls("170160110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptVAT1.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptVAT1.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT1)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVAT1.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptVAT1.MdiParent = this;
            frmRptVAT1.Show();
        }

        private void vAT16ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRptVAT16 frmRptVAT16 = new FormRptVAT16();
            #region Roll
            DataTable dt = UserMenuRolls("180120110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptVAT16.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptVAT16.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT16)  // Here
                {

                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVAT16.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frmRptVAT16.MdiParent = this;
            frmRptVAT16.Show();
        }

        private void vAT17ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRptVAT17 frmRptVAT17 = new FormRptVAT17();
            #region Roll
            DataTable dt = UserMenuRolls("180130110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptVAT17.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptVAT17.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT17)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVAT17.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frmRptVAT17.MdiParent = this;
            frmRptVAT17.Show();
        }

        private void vAT18ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormVAT9_1 frmRptVAT9_1 = new FormVAT9_1();
            #region Roll
            DataTable dt = UserMenuRolls("180140110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptVAT9_1.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptVAT9_1.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVAT9_1)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVAT9_1.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptVAT9_1.MdiParent = this;
            frmRptVAT9_1.Show();
        }

        private void vAT19ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormVAT6_10 frm = new FormVAT6_10();
            #region Roll
            DataTable dt = UserMenuRolls("180150110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVAT6_10)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frm.MdiParent = this;
            frm.Show();
        }

        #endregion

        #region Methods 19

        private void sDReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // SD Report
            FormRptSD frmRptSD = new FormRptSD();
            #region Roll
            DataTable dt = UserMenuRolls("180160110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptSD.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptSD.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptSD)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptSD.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptSD.MdiParent = this;
            frmRptSD.Show();
        }

        private void vAT6_3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMultipleVAT11 frmRptVAT11 = new FormMultipleVAT11();
            #region Roll
            DataTable dt = UserMenuRolls("180170110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptVAT11.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptVAT11.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT17)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVAT11.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frmRptVAT11.MdiParent = this;
            frmRptVAT11.Show();
        }

        private void vAT6_5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRptVAT6_5 frm = new FormRptVAT6_5();
            #region Roll
            DataTable dt = UserMenuRolls("180180110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT6_5)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frm.MdiParent = this;
            frm.Show();
        }

        private void vAT7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormVAT7 frmVAT7 = new FormVAT7();
            #region Roll
            DataTable dt = UserMenuRolls("180190110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmVAT7.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frmVAT7.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frmVAT7.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frmVAT7.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmVAT7.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVAT7)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmVAT7.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frmVAT7.MdiParent = this;
            frmVAT7.Show();
        }

        private void VAT20ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //multiple vat20
            FormSaleExport frm = new FormSaleExport();
            #region Roll
            DataTable dt = UserMenuRolls("180200110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Text = "VAT 20";
            frm.Show();
        }

        private void form4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRptForm4 frmRptForm4 = new FormRptForm4();
            #region Roll
            DataTable dt = UserMenuRolls("180210110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptForm4.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptForm4.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmRptForm4.MdiParent = this;
            //frmRptForm4.Text = "Multiple VAT 20";
            frmRptForm4.Show();
        }

        private void form5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRptForm5 frmRptForm5 = new FormRptForm5();
            #region Roll
            DataTable dt = UserMenuRolls("180210120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptForm5.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptForm5.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmRptForm5.MdiParent = this;
            frmRptForm5.Show();
        }

        private void summeryCurrentAccountToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // VAT 18 Summery 
            FormRptCurrentVAT18 frmRptCurrentVAT18 = new FormRptCurrentVAT18();
            #region Roll
            DataTable dt = UserMenuRolls("180220110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptCurrentVAT18.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptCurrentVAT18.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptCurrentVAT18)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptCurrentVAT18.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptCurrentVAT18.MdiParent = this;
            frmRptCurrentVAT18.Show();
        }

        private void breakdownCurrentAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //VAT18Breakdow
            FormRptVAT18Breakdown frm = new FormRptVAT18Breakdown();
            #region Roll
            DataTable dt = UserMenuRolls("180220120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT18Breakdown)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void chakKaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("180230110");



            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "ChakKa";
            frmSCBL.ShowDialog();
        }

        private void chakKhaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("180230120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "ChakKha";
            frmSCBL.ShowDialog();
        }

        private void newAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUserCreate frm = new FormUserCreate();
            #region Roll
            DataTable dt = UserMenuRolls("200110110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormUserCreate)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frm.MdiParent = this;
            frm.Show();
        }

        private void passwordChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUserPasswordChange frm = new FormUserPasswordChange();
            #region Roll
            DataTable dt = UserMenuRolls("200120110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormUserPasswordChange)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frm.MdiParent = this;
            frm.Show();
        }

        private void userRoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUserRoll frm = new FormUserRoll();
            #region Roll
            DataTable dt = UserMenuRolls("200130110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormUserRoll)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frm.MdiParent = this;
            frm.Show();
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region Roll
            DataTable dt = UserMenuRolls("200130110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            if (MessageBox.Show("Do you Want to Exit?", this.Text, MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            else
            {
                //CommonSoapClient DBClose = new CommonSoapClient();
                //DBClose.DBClose123();
                Application.Exit();
            }
        }

        #endregion

        #region Methods 20

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region Roll
            DataTable dt = UserMenuRolls("200160110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            if (MessageBox.Show("Do you Want to Login?", this.Text, MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            else
            {
                FormLogIn frmlLogIn = new FormLogIn();
                //frmlLogIn.MdiParent = this;
                frmlLogIn.ShowDialog();
            }
        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSearchUserLog frmUserLogs = new FormSearchUserLog();
            #region Roll
            DataTable dt = UserMenuRolls("200170110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmUserLogs.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmUserLogs.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmUserLogs.MdiParent = this;
            frmUserLogs.Show();
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region Roll
            DataTable dt = UserMenuRolls("200180110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void settingsRoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSetting frmseSetting = new FormSetting();
            #region Roll
            DataTable dt = UserMenuRolls("200140110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSetting)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmseSetting.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frmseSetting.MdiParent = this;
            frmseSetting.rbtnRole.Checked = true;
            frmseSetting.Text = "User Settings Role ";
            frmseSetting.Show();
        }

        private void userBranchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUserBranchDetail frm = new FormUserBranchDetail();
            #region Roll
            DataTable dt = UserMenuRolls("200190110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnSave.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnRemove.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSetting)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Text = "User Branch ";
            frm.Show();
        }

        private void purchaseToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            FormRptPurchaseInformation frmRptPurchaseInformation = new FormRptPurchaseInformation();
            #region Roll
            DataTable dt = UserMenuRolls("190110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptPurchaseInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptPurchaseInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frmRptPurchaseInformation.Text = "Report  ";
            frmRptPurchaseInformation.MdiParent = this;
            frmRptPurchaseInformation.Show();
        }

        private void issueToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            FormRptIssueInformation frmRptIssueInformation = new FormRptIssueInformation();
            #region Roll
            DataTable dt = UserMenuRolls("190120110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptIssueInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptIssueInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptIssueInformation)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptIssueInformation.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frmRptIssueInformation.MdiParent = this;
            frmRptIssueInformation.Show();
        }

        private void receiveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormRptReceiveInformation frmRptReceiveInformation = new FormRptReceiveInformation();
            #region Roll
            DataTable dt = UserMenuRolls("190120120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptReceiveInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptReceiveInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptReceiveInformation)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptReceiveInformation.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptReceiveInformation.MdiParent = this;
            frmRptReceiveInformation.Show();
        }

        private void saleToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            FormRptSalesInformation frmRptSalesInformation = new FormRptSalesInformation();
            #region Roll
            DataTable dt = UserMenuRolls("190130110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptSalesInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptSalesInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frmRptSalesInformation.Text = "Report(Local Sale)";
            frmRptSalesInformation.MdiParent = this;
            frmRptSalesInformation.Show();
        }

        private void stockToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormRptStock frmRptStock = new FormRptStock();
            #region Roll
            DataTable dt = UserMenuRolls("190140110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmRptStock.MdiParent = this;
            frmRptStock.rbtnTotal.Checked = true;
            frmRptStock.Show();
        }

        private void receiveSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRptSaleReceive frmRptStock = new FormRptSaleReceive();
            #region Roll
            DataTable dt = UserMenuRolls("190140110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmRptStock.MdiParent = this;
            frmRptStock.Show();
        }

        private void recnsciliationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRptReconsciliation frmHsCode = new FormRptReconsciliation();
            #region Roll
            DataTable dt = UserMenuRolls("190140130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmHsCode.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmHsCode.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmHsCode.MdiParent = this;
            frmHsCode.Show();
        }

        private void branchStockMovemmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBranchStockMovement frm = new FormBranchStockMovement();
            #region Roll
            DataTable dt = UserMenuRolls("190140140");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void depositToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            FormRptDepositTransaction frmRptDepositInformation = new FormRptDepositTransaction();
            #region Roll
            DataTable dt = UserMenuRolls("190150110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptDepositInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptDepositInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            //foreach (Form f in mdi.MdiChildren)
            //{
            //    if (f is FormRptDepositTransaction)  // Here
            //    {
            //        MessageBox.Show(frmRptDepositInformation.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}


            frmRptDepositInformation.MdiParent = this;
            frmRptDepositInformation.Show();
        }

        private void currentDepositToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //VAT18
            FormRptVAT18 frmRptVAT18 = new FormRptVAT18();

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT18)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVAT18.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frmRptVAT18.MdiParent = this;

            frmRptVAT18.Show();
        }

        #endregion

        #region Methods 21

        private void adjustmentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormRptAdjustment frmRptAdjustment = new FormRptAdjustment();
            #region Roll
            DataTable dt = UserMenuRolls("190160110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptAdjustment.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptAdjustment.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptAdjustment)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptAdjustment.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            frmRptAdjustment.MdiParent = this;
            frmRptAdjustment.Show();
        }

        private void coEfficientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRptInputOutputCoEfficientReport frm = new FormRptInputOutputCoEfficientReport();
            #region Roll
            DataTable dt = UserMenuRolls("190160120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void wastageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRptStock frmRptStock = new FormRptStock();
            #region Roll
            DataTable dt = UserMenuRolls("190160130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmRptStock.MdiParent = this;
            frmRptStock.rbtnWeastage.Checked = true;
            frmRptStock.Text = "Wastage Report";
            frmRptStock.Show();
        }

        private void valueChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Input value change
            FormRptFinishProductInformation frmFinishProductSearch = new FormRptFinishProductInformation();
            #region Roll
            DataTable dt = UserMenuRolls("190160140");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmFinishProductSearch.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmFinishProductSearch.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmFinishProductSearch.MdiParent = this;
            frmFinishProductSearch.Text = "Report(Input Value Change)";
            frmFinishProductSearch.Show();
        }

        private void serialStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRptSerialStockInformation frm = new FormRptSerialStockInformation();
            #region Roll
            DataTable dt = UserMenuRolls("190160150");
            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void purchageLCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRptPurchaseWithLC frm = new FormRptPurchaseWithLC();
            #region Roll
            DataTable dt = UserMenuRolls("190160160");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void comparisonStatementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMIS19 frm = new FormMIS19();
            #region Roll
            DataTable dt = UserMenuRolls("190180110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            //frm.Text = "Report(Local Sale With Chassis & Engine)";
            frm.MdiParent = this;
            frm.Show();
        }

        private void rMOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTransferIssue frm = new FormTransferIssue();
            #region Roll
            DataTable dt = UserMenuRolls("140130130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtn61Out.Checked = true;
            frm.Show();
        }

        private void fGOutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormTransferIssue frm = new FormTransferIssue();
            #region Roll
            DataTable dt = UserMenuRolls("140130140");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtn62Out.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rMInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTransferReceive frm = new FormTransferReceive();
            #region Roll
            DataTable dt = UserMenuRolls("140130110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtn61In.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void fGInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTransferReceive frm = new FormTransferReceive();
            #region Roll
            DataTable dt = UserMenuRolls("140130120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtn62In.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnWastageIssue_Click(object sender, EventArgs e)
        {
            FormIssue frm = new FormIssue();
            #region Roll
            DataTable dt = UserMenuRolls("130110140");
            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnIssueWastage.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnWastageSale_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140110180");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnSaleWastage.Checked = true;
            frm.Show();
        }

        private void rbtnIssueWOBom_Click(object sender, EventArgs e)
        {
            FormIssue frm = new FormIssue();
            #region Roll
            DataTable dt = UserMenuRolls("130110130");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnOther.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButtonLocalPurcchase_CanvasChanged(object sender, EventArgs e)
        {

        }

        private void ribbonButtonLocalPurcchase_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "LocalPurchase";
            frmSCBL.Show();

        }

        #endregion

        #region Methods 22

        private void ribbonButtonLocalSales_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "SalesStatementDelivery";
            frmSCBL.Show();
        }

        private void rbtnReceiveSales_Click(object sender, EventArgs e)
        {
            FormRptSaleReceive frmRptStock = new FormRptSaleReceive();
            #region Roll
            DataTable dt = UserMenuRolls("220110130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmRptStock.MdiParent = this;
            frmRptStock.Show();
        }

        private void rbtnMenuAll_Click(object sender, EventArgs e)
        {

            if (DialogResult.No != MessageBox.Show("Are you Symphony user?", "Company", MessageBoxButtons.YesNo,
                                                          MessageBoxIcon.Question,
                                                          MessageBoxDefaultButton.Button2))
            {
                IsSymphonyUser = FormSupperAdministrator.SelectOne();
                if (IsSymphonyUser)
                {
                    FormUserMenuAllRolls frm = new FormUserMenuAllRolls();
                    #region Roll
                    DataTable dt = UserMenuRolls("200200110");


                    if (dt != null && dt.Rows.Count > 0)
                    {

                        if (dt.Rows[0]["Access"].ToString() != "1")
                        {
                            MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

                    }
                    else
                    {
                        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    #endregion Roll
                    frm.MdiParent = this;
                    frm.Show();
                }


            }
            else
            {
                return;
            }

        }

        private void rbtnUserMenu_Click(object sender, EventArgs e)
        {
            FormUserRolls frm = new FormUserRolls();
            #region Roll
            DataTable dt = UserMenuRolls("200200120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.button1.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void wastageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormIssue frm = new FormIssue();
            #region Roll
            DataTable dt = UserMenuRolls("130110140");
            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnIssueWastage.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void packageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //package Production
            FormReceive frm = new FormReceive();
            #region Roll
            DataTable dt = UserMenuRolls("130120140");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnPackageProduction.Checked = true;
            frm.MdiParent = this;

            frm.Show();
        }

        private void wastageToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("140110180");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnSaleWastage.Checked = true;
            frm.Show();
        }

        private void bankToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormBankInformation frm = new FormBankInformation();
            #region Roll
            DataTable dt = UserMenuRolls("110140110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll


            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormBankInformation)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void vehicleToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FormVehicle frm = new FormVehicle();

            #region Roll
            DataTable dt = UserMenuRolls("110140120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVehicle)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void tradingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            #region Roll
            DataTable dt = UserMenuRolls("120110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnOther.Checked = true;

            frm.Show();
        }

        private void issueWithoutBOMToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormIssue frm = new FormIssue();
            #region Roll
            DataTable dt = UserMenuRolls("130110130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.rbtnOther.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void vAT11GaGaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("160110130");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnVAT11GaGa.Checked = true;
            frm.Show();
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUserMenuAllRolls frm = new FormUserMenuAllRolls();
            #region Roll
            DataTable dt = UserMenuRolls("200200110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void userToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUserRolls frm = new FormUserRolls();
            #region Roll
            DataTable dt = UserMenuRolls("200200120");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.button1.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.Show();
        }

        private void demandToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormDemand frm = new FormDemand();
            #region Roll
            DataTable dt = UserMenuRolls("210110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormDemand)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void receiveToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            FormBanderolReceive frm = new FormBanderolReceive();
            #region Roll
            DataTable dt = UserMenuRolls("210120110");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormBanderolReceive)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        #endregion

        #region Methods 23

        private void localPurchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "LocalPurchase";
            frmSCBL.ShowDialog();
        }

        private void localSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110120");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "SalesStatementDelivery";
            frmSCBL.ShowDialog();
        }

        private void receiveSaleToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            FormRptSaleReceive frmRptStock = new FormRptSaleReceive();
            #region Roll
            DataTable dt = UserMenuRolls("220110130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmRptStock.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmRptStock.MdiParent = this;
            frmRptStock.Show();
        }

        private void rbtnCustomHouse_Click(object sender, EventArgs e)
        {
            FormCustomsHouse frm = new FormCustomsHouse();

            //#region Roll
            //DataTable dt = UserMenuRolls("110120130");


            //if (dt != null && dt.Rows.Count > 0)
            //{

            //    if (dt.Rows[0]["Access"].ToString() != "1")
            //    {
            //        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //    frm.btnAdd.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
            //    frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
            //    frm.btnDelete.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            //}
            //else
            //{
            //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            //#endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormVehicle)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void M2Login_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you Want to login?", this.Text, MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            else
            {
                FormLogIn frmlLogIn = new FormLogIn();
                //frmlLogIn.MdiParent = this;
                frmlLogIn.ShowDialog();
                MDIName();

            }
        }

        private void M2Logout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you Want to Exit?", this.Text, MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            else
            {
                //CommonSoapClient DBClose = new CommonSoapClient();
                //DBClose.DBClose123();
                Application.Exit();
            }
        }

        private void M2Branch_Click(object sender, EventArgs e)
        {
            FormLoginBranch frmlLogIn = new FormLoginBranch();
            frmlLogIn.ShowDialog();
            MDIName();
        }

        public bool PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            bool active = m.Msg == 0x100 || m.Msg == 0x101;  // WM_KEYDOWN/UP
            active = active || m.Msg == 0xA0 || m.Msg == 0x200;  // WM_(NC)MOUSEMOVE
            active = active || m.Msg == 0x10;  // WM_CLOSE, in case dialog closes
            if (active)
            {
                if (mTimer != null)
                {
                    mTimer.Enabled = false;
                    mTimer.Start();
                }
            }

            return false;
        }

        private void LogoutUser(object sender, EventArgs e)
        {
            // No activity, logout user MDIMainInterface

            try
            {
                //if (ribbon1.Visible)
                //{
                //    ribbon1.Visible = false;
                //    menuStripTop.Visible = true;

                //    List<Form> openForms = new List<Form>();

                //    foreach (Form f in Application.OpenForms)
                //        openForms.Add(f);

                //    foreach (Form form in openForms)
                //    {
                //        if(form.Name == "MDIMainInterface") continue;
                //        form.Close();
                //    }

                //    FormLogIn logIn = new FormLogIn();

                //    logIn.ShowDialog();
                //}

                Environment.Exit(1);
            }
            catch (Exception exception)
            {

            }
        }

        #endregion

        private void rbtnVAT6_7_Click(object sender, EventArgs e)
        {
            FormMultipleVAT11 frmRptVAT11 = new FormMultipleVAT11();

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT17)  // Here
                {

                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVAT11.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptVAT11.FormType = "Credit";

            frmRptVAT11.MdiParent = this;
            frmRptVAT11.Show();
        }

        private void rbtnVAT6_8_Click(object sender, EventArgs e)
        {
            FormMultipleVAT11 frmRptVAT11 = new FormMultipleVAT11();

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptVAT17)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVAT11.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptVAT11.FormType = "Debit";

            frmRptVAT11.MdiParent = this;
            frmRptVAT11.Show();
        }

        private void rbtServerStatus_Click(object sender, EventArgs e)
        {
            bgwCheckServer.RunWorkerAsync();
        }

        private void bgwCheckServer_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Ping myPing = new Ping();
            status = IPStatus.TimedOut;
            //if (SysDBInfoVM.SysdataSource != ".")
            //{
            //    PingReply reply = myPing.Send(SysDBInfoVM.SysdataSource, 1000);
            //    status = reply.Status;
            //}


            try
            {
                DBSQLConnection sqlConnection = new DBSQLConnection();

                SqlConnection connection = sqlConnection.GetConnectionMaster();
                connection.Open();

                status = IPStatus.Success;

            }
            catch (Exception exception)
            {
                status = IPStatus.TimedOut;
            }

        }

        private void bgwCheckServer_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (status != IPStatus.Success)
            {
                MessageBox.Show("Server is not responding", "Server Status",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Server is Ok", "Server Status",
                    MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }

        private void ribbonButtonReleaseNote_Click(object sender, EventArgs e)
        {
            //
            FormReleaseNotes frm = new FormReleaseNotes();

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormReleaseNotes)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frm.MdiParent = this;
            frm.Show();
        }

        private void serverCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bgwCheckServer.RunWorkerAsync();
        }

        private void rbtn6_2_1_Click(object sender, EventArgs e)
        {
            FormRptVATKa frmRptVATKa = new FormRptVATKa();


            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormReleaseNotes)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVATKa.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            frmRptVATKa.MdiParent = this;
            frmRptVATKa.Show();
        }

        private void rbtn6_6_Click(object sender, EventArgs e)
        {
            //FormRptVDS12 frmRptVDS12 = new FormRptVDS12();
            //FormDepositSearch frmRptVDS12 = new FormDepositSearch();
            string transactionType = "VDS";
            DataGridViewRow selectedRow = FormDepositSearch.SelectOne(transactionType);

            //foreach (Form f in mdi.MdiChildren)
            //{
            //    if (f is FormReleaseNotes)  // Here
            //    {
            //        MessageBox.Show(frmRptVDS12.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}
            ////frmRptVDS12._isVDs = true;

            ////frmRptVDS12.Text = "VDS Search";
            //frmRptVDS12.MdiParent = this;
            //frmRptVDS12.Show();
        }

        private void rbtnRawSaleCN_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            #region Roll
            DataTable dt = UserMenuRolls("170130110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.MdiParent = this;
            frm.rbtnRCN.Checked = true;
            frm.Show();
        }

        private void rbtnSaleSummary_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "SaleSummaryAllShift";
            frmSCBL.Show();
        }

        private void rbtnSaleSummarybyProduct_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "SaleSummarybyProduct";
            frmSCBL.Show();
        }

        private void rbtnImportData_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110130");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "ImportData";
            frmSCBL.Show();
        }

        private void rbtnReceiveVsSale_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "ReceiedVsSale";
            frmSCBL.Show();
        }

        private void rbtnSalesStatementForService_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "SalesStatementForService";
            frmSCBL.Show();
        }

        private void rbtnSalesStatementDelivery_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "SalesStatementDelivery";
            frmSCBL.Show();
        }

        private void rbtnStockReportFG_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "StockReportFG";
            frmSCBL.Show();
        }

        private void rbtnStockReportRM_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "StockReportRM";
            frmSCBL.Show();
        }

        private void rbtnTransferToDepo_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frmSCBL.reportType = "TransferToDepot";
            frmSCBL.Show();
        }

        private void rbtnVDSStatement_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            //foreach (Form f in mdi.MdiChildren)
            //{
            //    if (f is FormReportSCBL)  // Here
            //    {
            //        frmSCBL.Hide();
            //    }
            //}
            frmSCBL.MdiParent = this;
            frmSCBL.reportType = "VDSStatement";
            frmSCBL.Show();
        }

        private void rbtnMonthly_Production_Delivery_Click(object sender, EventArgs e)
        {
            FormReportSCBL_Production frmSCBL = new FormReportSCBL_Production();
            #region Roll
            DataTable dt = UserMenuRolls("220110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            //foreach (Form f in mdi.MdiChildren)
            //{
            //    if (f is FormReportSCBL)  // Here
            //    {
            //        frmSCBL.Hide();
            //    }
            //}
            frmSCBL.MdiParent = this;
            frmSCBL.Show();
        }

        private void ribbonButton81_DoubleClick(object sender, EventArgs e)
        {

        }

        private void RawReceiveFromContractor_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            frm.rbtnClientRawReceive.Checked = true;

            //FormRptReconsciliation frmHsCode = new FormRptReconsciliation();
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnFGReceiveWOBom_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            frm.rbtnClientFGReceiveWOBOM.Checked = true;

            //FormRptReconsciliation frmHsCode = new FormRptReconsciliation();
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnTollClient6_3_Click(object sender, EventArgs e)
        {
            FormClient6_3 frm = new FormClient6_3();
            ////frm.rbtnClient63.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnRawIssueToClient_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            frm.rbtnContractorRawIssue.Checked = true;

            //FormRptReconsciliation frmHsCode = new FormRptReconsciliation();
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton22_Click_2(object sender, EventArgs e)
        {
            FormDisposeFinishNew frm = new FormDisposeFinishNew();
            frm.rbtnOther.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton24_Click_1(object sender, EventArgs e)
        {
            try
            {
                FormSale frm = new FormSale();

                frm.rbtnDisposeFinish.Checked = true;
                frm.MdiParent = this;
                frm.Show();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void btnContractorIssueWOBom_Click(object sender, EventArgs e)
        {
            FormIssue frm = new FormIssue();
            #region Roll
            DataTable dt = UserMenuRolls("130110130");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
                frm.btnUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
                frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll
            frm.btnContractorIssue.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnTollSale_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();

            frm.rbtnTollSale.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnBankingChannel_Click(object sender, EventArgs e)
        {
            FormPurchaseSearch frm = new FormPurchaseSearch();

            frm.rbtnBankChannelPayment.Checked = true;
            frm.MdiParent = this;
            frm.Show();

        }

        private void rbtnProductTransfer_Click(object sender, EventArgs e)
        {
            FormProductTransfer frm = new FormProductTransfer();
            frm.rbtnIsRaw.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnDayEnd_Click(object sender, EventArgs e)
        {
            try
            {
                CommonDAL commonDal = new CommonDAL();
                string companyCode = commonDal.settingValue("DayEnd", "BigDataProcess");
                string OldProcess = commonDal.settingValue("DayEnd", "OldProcess");

                if (string.Equals(companyCode, "Y", StringComparison.CurrentCultureIgnoreCase) || OldProcess == "Y")
                {
                    FormIssueMultiple formIssueMultiple = new FormIssueMultiple();
                    formIssueMultiple.showReproc = false;
                    formIssueMultiple.MdiParent = this;

                    formIssueMultiple.Show();
                }
                else
                {
                    FormRegularProcess formIssueMultiple = new FormRegularProcess();
                    formIssueMultiple.MdiParent = this;

                    formIssueMultiple.Show();
                }

            }
            catch (Exception exception)
            {

            }
        }

        private void rbtnInputValue7_5_Change_Click(object sender, EventArgs e)
        {
            FormReportSCBL frmSCBL = new FormReportSCBL();
            #region Roll
            DataTable dt = UserMenuRolls("220110110");

            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frmSCBL.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            frmSCBL.MdiParent = this;
            frmSCBL.reportType = "InputValueChange";
            frmSCBL.Show();
        }

        private void rbtnSaleIntegration_Click(object sender, EventArgs e)
        {
            FormSaleImportNESTLE frmNESTLE = new FormSaleImportNESTLE();
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSaleImportNESTLE)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVATKa.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frmNESTLE.MdiParent = this;
            frmNESTLE.transactionType = "Other";
            frmNESTLE.Show();
        }

        private void rbtnSalesReturn_Click(object sender, EventArgs e)
        {
            FormSaleCNImportNESTLE frmNESTLE = new FormSaleCNImportNESTLE();
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSaleImportNESTLE)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVATKa.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frmNESTLE.MdiParent = this;
            frmNESTLE.transactionType = "Credit";
            frmNESTLE.Show();
        }

        private void rbtnPurchaseLocal_Click(object sender, EventArgs e)
        {
            FormNestlePurchase frm = new FormNestlePurchase();
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSaleImportNESTLE)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVATKa.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.rbtnOther.Checked = true;
            frm.Show();

        }

        private void rbtnPurchaseReturn_Click(object sender, EventArgs e)
        {
             FormNestlePurchase frm = new FormNestlePurchase();
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSaleImportNESTLE)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVATKa.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;
            frm.rbtnPurchaseReturn.Checked = true;
            frm.Show();
        }

        private void rbtnsalesDN_Click(object sender, EventArgs e)
        {
            FormSaleCNImportNESTLE frmNESTLE = new FormSaleCNImportNESTLE();
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSaleImportNESTLE)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVATKa.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frmNESTLE.MdiParent = this;
            frmNESTLE.transactionType = "Debit";
            frmNESTLE.Show();
        }

        private void rbtnIsWastageTransfer_Click(object sender, EventArgs e)
        {
            FormProductTransfer frm = new FormProductTransfer();
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormProductTransfer)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVATKa.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.rbtnIsWastage.Checked= true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnFinishTransfer_Click(object sender, EventArgs e)
        {
            FormProductTransfer frm = new FormProductTransfer();
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormProductTransfer)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVATKa.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.rbtnIsFinish.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtnTollCharge_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();

            #region Roll

            ////DataTable dt = UserMenuRolls("120110130");


            ////if (dt != null && dt.Rows.Count > 0)
            ////{

            ////    if (dt.Rows[0]["Access"].ToString() != "1")
            ////    {
            ////        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ////        return;
            ////    }
            ////    frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
            ////    frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
            ////    frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            ////}
            ////else
            ////{
            ////    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ////    return;
            ////}

            #endregion Roll

            frm.MdiParent = this;
            frm.rbtnPurchaseTollcharge.Checked = true;

            frm.Show();
        }

        private void rbtnCPC_Click(object sender, EventArgs e)
        {
            FormCPCDetails frm = new FormCPCDetails();
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormCPCDetails)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVATKa.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.Show();

        }

        private void rbbtnCharge_Click(object sender, EventArgs e)
        {
            
            FormIASChargeCode frm = new FormIASChargeCode();
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormIASChargeCode)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frmRptVATKa.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.Show();
        }

        private void rbtnStock_Click(object sender, EventArgs e)
        {
            FormRptTollStock frm = new FormRptTollStock();
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormRptTollStock)  
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();

        }

        private void rbtnQuaziStock_Click(object sender, EventArgs e)
        {
            FormQuaziStock frm = new FormQuaziStock();
           
            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormQuaziStock)
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    return;
                }
            }
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton29_Click(object sender, EventArgs e)
        {
            FormDisposeFinishNew frm = new FormDisposeFinishNew();
            frm.rbtnDisposeTrading.Checked = true;
            frm.MdiParent = this;
            frm.Show();
        }

        private void rbtn9_1Statement_Click(object sender, EventArgs e)
        {
            Form9_1Statement frm = new Form9_1Statement();
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton32_Click_1(object sender, EventArgs e)
        {
            var IsSymphonyUser = FormSupperAdministrator.SelectOne();
            if (!IsSymphonyUser)
            {
                MessageBox.Show("Wrong username or password");
                return;
            }

            FormSettingMaster frm = new FormSettingMaster();

            #region Roll
            DataTable dt = UserMenuRolls("110190130");


            if (dt != null && dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["Access"].ToString() != "1")
                {
                    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            else
            {
                MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion Roll

            foreach (Form f in mdi.MdiChildren)
            {
                if (f is FormSync)  // Here
                {
                    f.WindowState = FormWindowState.Normal;
                    f.Activate();

                    ////MessageBox.Show(frm.Text + " form is already opened.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            frm.MdiParent = this;

            frm.Show();
        }

        private void ribbonButton41_Click_1(object sender, EventArgs e)
        {
            FormReport9_1Comp frm = new FormReport9_1Comp();
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButtonCentral_Click(object sender, EventArgs e)
        {
            FormRptCentral frm = new FormRptCentral();
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton9_1_Click(object sender, EventArgs e)
        {
            FormReport9_1Comp_Monthly frm = new FormReport9_1Comp_Monthly();
            frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonbtnNeg_Click(object sender, EventArgs e)
        {
            //FormNegativeDownload frm = new FormNegativeDownload();
            //frm.MdiParent = this;
            //frm.Show();
        }

        private void ribbonButton44_Click_1(object sender, EventArgs e)
        {
            FormSaleSummary frm = new FormSaleSummary();
            frm.Show();
        }


    }
}
