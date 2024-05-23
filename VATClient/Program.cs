using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Threading;
using System.Xml.Linq;

//
using SymphonySofttech.Reports.Report;
using SymphonySofttech.Utilities;
using VATClient.ReportPreview;
using VATServer.Library;

using VATViewModel.DTOs;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Reflection;
using VATServer.Ordinary;

namespace VATClient
{
    internal static class Program
    {
        #region PublicDatafields

        public static FormMain MainForm { get; set; }
        public static MDIMainInterface MdiForm { get; set; }
        public static FormLogIn LoginForm { get; set; }

        public static int BranchId = 0;
        public static string BranchCode = "";

        public static string CurrentUser { get; set; }
        public static string CurrentUserID { get; set; }
        public static bool IsLoading { get; set; }
        public static string R_F { get; set; }
        public static string fromOpen { get; set; }
        public static string SalesType { get; set; }
        public static string Trading { get; set; }

        public static string DatabaseName { get; set; }

        public static string[] PublicRollLines { get; set; }
        public static DateTime SessionDate { get; set; }
        public static DateTime SessionTime { get; set; }
        public static int ChangeTime { get; set; }
        public static DateTime ServerDateTime { get; set; }
        public static DateTime vMinDate = Convert.ToDateTime("1753/01/02");
        public static DateTime vMaxDate = Convert.ToDateTime("9998/12/30");
        public static bool successLogin = false;
        public static string FontSize = "8";

        public static string Access { get; set; }
        public static string Post { get; set; }
        public static DateTime LicenceDate { get; set; }
        public static DateTime serverDate { get; set; }

        public static bool IsTrial = false;
        public static string Trial = "";
        public static string TrialComments = "Unregister Version";

        public static string ImportFileName { get; set; }
        public static string ItemType = "Other";
        public static RptStart objrpt1 { get; set; }

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        public static ReportClass _objRpt { get; set; }// = new ReportClass();


        public static bool IsAlpha = false;
        public static string Alpha = "";
        public static string AlphaComments = "Alpha Version";

        public static bool IsBeta = false;
        public static string Beta = "";
        public static string BetaComments = "Beta Version";

        public static bool IsBureau = false;

        public static string Add { get; set; }
        public static string Edit { get; set; }
        public static DataSet publicDataSet { get; set; }
        public static DataTable UserMenuAllRolls { get; set; }
        public static DataTable UserMenuRolls { get; set; }

        public static string IsWCF { get; set; }
        #region CompanyLicenseVM

        public static string LicenseKey = "NA";
        public static bool IsCentralBIN = true;
        public static bool IsManufacturing = true;
        public static bool IsTrading = true;
        public static bool IsService = true;
        public static bool IsTender = true;
        public static bool IsTDS = true;
        public static bool IsBandroll = true;
        public static bool IsTollClient = true;
        public static bool IsTollContractor = true;
        public static bool IsIntegrationExcel = true;
        public static bool IsIntegrationOthers = true;
        public static bool IsIntegrationAPI = true;
        public static int Depos = 0;

        #endregion CompanyLicenseVM

        #region Company Profile

        public static string CompanyID { get; set; }
        public static string CompanyNameLog { get; set; }
        public static string CompanyName { get; set; }
        public static string CompanyLegalName { get; set; }
        public static string Address1 { get; set; }
        public static string Address2 { get; set; }
        public static string Address3 { get; set; }
        public static string City { get; set; }
        public static string ZipCode { get; set; }
        public static string TelephoneNo { get; set; }
        public static string FaxNo { get; set; }
        public static string Email { get; set; }
        public static string ContactPerson { get; set; }
        public static string ContactPersonDesignation { get; set; }
        public static string ContactPersonTelephone { get; set; }
        public static string ContactPersonEmail { get; set; }
        public static string TINNo { get; set; }
        public static string VatRegistrationNo { get; set; }
        public static string Comments { get; set; }
        public static string Section { get; set; }
        public static string ActiveStatus { get; set; }
        public static DateTime FMonthStart { get; set; }
        public static DateTime FMonthEnd { get; set; }
        public static decimal VATAmount { get; set; }


        #endregion
        #endregion

        public static DataSet publicDsCompanyProfile = new DataSet();

        
        public static string AppPath
        {

            get
            {
                string directory = string.Empty;
                try
                {
                    directory = AppDomain.CurrentDomain.BaseDirectory;
                }
                #region Catch
                catch (IndexOutOfRangeException ex)
                {
                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (NullReferenceException ex)
                {
                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (FormatException ex)
                {

                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }

                catch (SoapHeaderException ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }

                    FileLogger.Log("Program", "AppPath", exMessage);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (SoapException ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "AppPath", exMessage);
                }
                catch (EndpointNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "AppPath", exMessage);
                }
                #endregion Catch

                return directory;
            }

        }
        public static string AppPathRootLocation
        {

            get
            {
                string directory = string.Empty;
                try
                {
                    directory = AppDomain.CurrentDomain.BaseDirectory;
                }
                #region Catch
                catch (IndexOutOfRangeException ex)
                {
                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (NullReferenceException ex)
                {
                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (FormatException ex)
                {

                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }

                catch (SoapHeaderException ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }

                    FileLogger.Log("Program", "AppPath", exMessage);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (SoapException ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "AppPath", exMessage);
                }
                catch (EndpointNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "AppPath", exMessage);
                }
                #endregion Catch

                return directory;
            }

        }

        public static string ReportAppPath
        {

            get
            {
                string directory = string.Empty;
                try
                {
                    //string dirName = AppDomain.CurrentDomain.BaseDirectory; // Starting Dir
                    //FileInfo fileInfo = new FileInfo(dirName);
                    //DirectoryInfo parentDir = fileInfo.Directory..Parent;
                    directory = @"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report"; // Parent of Starting Dir

                }
                #region Catch
                catch (IndexOutOfRangeException ex)
                {
                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (NullReferenceException ex)
                {
                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (FormatException ex)
                {

                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }

                catch (SoapHeaderException ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }

                    FileLogger.Log("Program", "AppPath", exMessage);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (SoapException ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "AppPath", exMessage);
                }
                catch (EndpointNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "AppPath", ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "AppPath", exMessage);
                }
                #endregion Catch

                return directory;
            }

        }
        public static string TempFilePath
        {
            get
            {
                string tempFilePath = string.Empty;
                try
                {
                    //return System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Temp";
                    tempFilePath = "C:\\Product.xls";
                }
                #region Catch
                catch (IndexOutOfRangeException ex)
                {
                    FileLogger.Log("Program", "TempFilePath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (NullReferenceException ex)
                {
                    FileLogger.Log("Program", "TempFilePath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (FormatException ex)
                {

                    FileLogger.Log("Program", "TempFilePath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }

                catch (SoapHeaderException ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }

                    FileLogger.Log("Program", "TempFilePath", exMessage);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (SoapException ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "TempFilePath", exMessage);
                }
                catch (EndpointNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "TempFilePath", ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "TempFilePath", ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "TempFilePath", exMessage);
                }
                #endregion Catch

                return tempFilePath;
            }

        }
        public static string ReportFilePath
        {
            get
            {
                string reportFilePath = string.Empty;
                try
                {
                    //return "C:\\Report";
                    //MessageBox.Show(AppPath + "Reports");
                    reportFilePath = AppDomain.CurrentDomain.BaseDirectory + "Reports";
                    //return AppPath + "Reports";
                }
                #region Catch
                catch (IndexOutOfRangeException ex)
                {
                    FileLogger.Log("Program", "ReportFilePath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (NullReferenceException ex)
                {
                    FileLogger.Log("Program", "ReportFilePath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (FormatException ex)
                {

                    FileLogger.Log("Program", "ReportFilePath", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }

                catch (SoapHeaderException ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }

                    FileLogger.Log("Program", "ReportFilePath", exMessage);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (SoapException ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "ReportFilePath", exMessage);
                }
                catch (EndpointNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "ReportFilePath", ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "ReportFilePath", ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                    ex.StackTrace;

                    }
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "ReportFilePath", exMessage);
                }
                #endregion Catch

                return reportFilePath;
            }

        }
        public static void MDILoad1()
        {
            MDIMainInterface frm = new MDIMainInterface();
            string a = "";
            frm.LCompany.Text = a + Program.CompanyName + "-" + Program.CompanyNameLog;
            int MW = frm.Size.Width;
            int MH = frm.Size.Height;

            int LW = frm.LCompany.Size.Width;
            int LH = frm.LCompany.Size.Height;

            frm.LCompany.Left = MW - LW - 15;
            frm.LCompany.Top = MH - LH - 60;
        }
        public static void SaveToErroFile(string ErrorID, string ErrorDesc, string UserName)
        {
            try
            {
                XElement xml = null;
                XDocument Xdoc = new XDocument();
                if (System.IO.File.Exists(AppPath + "/ErrorLog/ErrorInformation.xml"))
                {
                    Xdoc = XDocument.Load(AppPath + "/ErrorLog/ErrorInformation.xml");
                    xml = new XElement("ErrorLog",

                                       new XAttribute("ErrorID", ErrorID),
                                       new XElement("ErrorDesc", ErrorDesc),
                                       new XElement("UserName", UserName),
                                       new XElement("DateTime", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"))
                        );
                }
                else
                {
                    xml = new XElement("Companies",
                                       new XElement("ErrorLog",
                                                    new XAttribute("ErrorID", ErrorID),
                                                    new XElement("ErrorDesc", ErrorDesc),
                                                    new XElement("UserName", UserName),
                                                    new XElement("DateTime",
                                                                 DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"))
                                           ));

                }
                if (Xdoc.Descendants().Count() > 0)
                    Xdoc.Descendants().First().Add(xml);
                else
                {
                    Xdoc.Add(xml);
                }
                Xdoc.Save(AppPath + "/ErrorLog/ErrorInformation.xml");
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "SaveToErroFile", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "SaveToErroFile", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "SaveToErroFile", exMessage);
            }
            #endregion Catch
        }
        public static bool SaveToSuperFile(string tom, string jery, string mini, string chkIsWindowsAuthentication = "N")
        {
            bool result = false;

            try
            {


                //if (DeleteFile() == false)
                //{
                //    MessageBox.Show("File not Deleted");
                //    result = false;
                //}

                DeleteFile();
                XElement xml = null;
                XDocument Xdoc = new XDocument();
                if (System.IO.File.Exists(AppPath + "/SuperInformation.xml"))
                {
                    Xdoc = XDocument.Load(AppPath + "/SuperInformation.xml");
                    xml = new XElement("SuperInfo",

                                       new XAttribute("tom", tom),
                                       new XElement("jery", jery),
                                       new XElement("mini", mini),
                                       new XElement("doremon", chkIsWindowsAuthentication),
                                       new XElement("DateTime", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"))
                        );
                }
                else
                {
                    xml = new XElement("Super",
                                       new XElement("SuperInfo",
                                                    new XAttribute("tom", tom),
                                                    new XElement("jery", jery),
                                                    new XElement("mini", mini),
                                                    new XElement("doremon", chkIsWindowsAuthentication),
                                                    new XElement("DateTime", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"))
                                           ));


                }
                if (Xdoc.Descendants().Count() > 0)
                    Xdoc.Descendants().First().Add(xml);
                else
                {
                    Xdoc.Add(xml);
                }
                Xdoc.Save(AppPath + "/SuperInformation.xml");

                #region Save data into System Database

                //CommonDAL commonDal=new CommonDAL();
                //commonDal.UpdateSystemData(tom, jery, mini);
                //UpdateSystemData
                #endregion Save data into System Database

                result = true;

            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "SaveToErroFile", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "SaveToErroFile", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "SaveToErroFile", exMessage);
            }
            #endregion Catch
            return result;

        }
        public static bool DeleteFile()
        {
            bool result = false;
            if (System.IO.File.Exists(AppPath + "/SuperInformation.xml"))
            {


                // Use a try block to catch IOExceptions, to 
                // handle the case of the file already being 
                // opened by another process. 
                try
                {
                    System.IO.File.Delete(AppPath + "/SuperInformation.xml");
                    result = true;
                }
                #region Catch

                catch (IndexOutOfRangeException ex)
                {
                    FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);


                }
                catch (NullReferenceException ex)
                {
                    FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);


                }
                catch (FormatException ex)
                {

                    FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }

                catch (SoapHeaderException ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message +
                                    Environment.NewLine +
                                    ex.StackTrace;

                    }

                    FileLogger.Log("Program", "SaveToErroFile", exMessage);
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (SoapException ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message +
                                    Environment.NewLine +
                                    ex.StackTrace;

                    }
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "SaveToErroFile", exMessage);
                }
                catch (EndpointNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "SaveToErroFile", ex.Message + Environment.NewLine + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    string exMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message +
                                    Environment.NewLine +
                                    ex.StackTrace;

                    }
                    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FileLogger.Log("Program", "SaveToErroFile", exMessage);
                }

                #endregion Catch

            }
            else
            {
                result = true;

            }

            return result;
        }
        public static string SpellDecimal(decimal number)
        {
            string[] digit =
      {
            "", "one", "two", "three", "four", "five", "six", 
            "seven", "eight", "nine", "ten", "eleven", "twelve", 
            "thirteen", "fourteen", "fifteen", "sixteen", 
            "seventeen", "eighteen", "nineteen" 
      };

            string[] baseten = 
      {
            "", "", "twenty", "thirty", "fourty", "fifty", 
            "sixty", "seventy", "eighty", "ninety" 
      };

            string[] expo = 
      { 
            "", "thousand", "million", "billion", "trillion",
            "quadrillion", "quintillion"
      };

            StringBuilder sb = new StringBuilder();
            int thousands = 0;
            decimal power = 1;

            if (number < 0)
            {
                sb.Append("minus ");
                number = -number;
            }

            decimal n = Decimal.Truncate(number);
            decimal cents = Decimal.Truncate((number - n) * 100);

            if (n == Decimal.Zero)
                sb.Append("zero");

            for (decimal i = n; i >= 1000; i /= 1000)
            {
                power *= 1000;
                thousands++;
            }

            bool sep = false;
            for (decimal i = n; thousands >= 0; i %= power, thousands--, power /= 1000)
            {
                int j = (int)(i / power);
                int k = j % 100;
                int hundreds = j / 100;
                int tens = j % 100 / 10;
                int ones = j % 10;

                if (j == 0)
                    continue;

                if (hundreds > 0)
                {
                    if (sep)
                        sb.Append(", ");

                    sb.Append(digit[hundreds]);
                    sb.Append(" hundred");
                    sep = true;
                }

                if (k != 0)
                {
                    if (sep)
                    {
                        sb.Append(" and ");
                        sep = false;
                    }

                    if (k < 20)
                        sb.Append(digit[k]);
                    else
                    {
                        sb.Append(baseten[tens]);
                        if (ones > 0)
                        {
                            sb.Append("-");
                            sb.Append(digit[ones]);
                        }
                    }
                }

                if (thousands > 0)
                {
                    sb.Append(" ");
                    sb.Append(expo[thousands]);
                    sep = true;
                }
            }

            sb.Append(" and ");
            if (cents < 10) sb.Append("0");
            sb.Append(cents);
            sb.Append("/100");

            return sb.ToString();
        }
        public static string NumberToWords(int number)
        {
            string words = string.Empty;
            try
            {
                words = OrdinaryVATDesktop.NumberToWords(number);
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "NumberToWords", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "NumberToWords", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "NumberToWords", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "NumberToWords", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "NumberToWords", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "NumberToWords", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "NumberToWords", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "NumberToWords", exMessage);
            }
            #endregion Catch
            return words;
        }

        [STAThread]
        private static void Main()
        {
            try
            {
                #region Licence

                LicenceDate = Convert.ToDateTime("2030/2/28");

                #endregion

               
                Application.EnableVisualStyles();
 

                #region Start

                MdiForm = new MDIMainInterface();
                Application.Run(MdiForm);
                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new FormLogIn());

                

                #endregion Start


            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "Main", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "Main", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "Main", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "Main", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "Main", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "Main", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "Main", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "Main", exMessage);
            }
            #endregion Catch


        }



        public static bool CheckLicence(DateTime TransDate)
        {
            try
            {
                if (TransDate > LicenceDate)
                {
                    return true;
                }

                //FormSplash frmSplash = new FormSplash();
                ////frmSplash.Show();
                ////MessageBox.Show("Splash Open");
                ////if (DBConstant.ConnStatus() != 1)
                ////{
                ////    MessageBox.Show("Connection Not Exist");
                ////    return;
                ////}
                ////VATClient.FormLogIn.ConnStatus();

                ////// Check connection
                //while (IsLoading)
                //{                
                //}
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "CheckLicence", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "CheckLicence", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "CheckLicence", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "CheckLicence", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "CheckLicence", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "CheckLicence", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "CheckLicence", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "CheckLicence", exMessage);
            }
            #endregion Catch
            return false;
        }

        public static void FormatTextBoxRate(TextBox txtBox, string fieldTitle)
        {
            try
            {
                if (txtBox.Text != string.Empty)
                {
                    txtBox.Text = txtBox.Text.Trim().Replace(",", "");
                    if (IsNumeric(txtBox.Text) == false)
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "0.00";
                        txtBox.Focus();
                    }
                    else if (Convert.ToDecimal(txtBox.Text) < 0)
                    {
                        MessageBox.Show("Please enter positive value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "0.00";
                        txtBox.Focus();
                    }
                    else if (Convert.ToDecimal(txtBox.Text) > 100)
                    {
                        MessageBox.Show("Rate cannot exceed 100 % ", fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "0.00";
                        txtBox.Focus();
                    }
                    else if (!IsNumeric(txtBox.Text))
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "0.00";
                        txtBox.Focus();
                    }
                    else
                    {

                        txtBox.Text = ParseDecimal(txtBox.Text);

                    }

                }
                else
                {
                    txtBox.Text = "0.00";
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "FormatTextBoxRate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "FormatTextBoxRate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "FormatTextBoxRate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "FormatTextBoxRate", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBoxRate", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBoxRate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBoxRate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBoxRate", exMessage);
            }
            #endregion Catch

        }

        public static void FormatGridQty(string txtBox, string fieldTitle)
        {
            try
            {
                if (txtBox != string.Empty)
                {

                    txtBox  = txtBox.Trim().Replace(",", "");
                    if (IsNumeric(txtBox) == false)
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox = "0.00";

                    }
                    else if (Convert.ToDecimal(txtBox) < 0)
                    {
                        MessageBox.Show("Please enter positive numeric value in " + fieldTitle, fieldTitle,
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtBox = "0.00";

                    }
                    else if (!IsNumeric(txtBox))
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox = "0.00";

                    }
                    else if (Convert.ToDecimal(txtBox) > 999999999)
                    {
                        MessageBox.Show("Please give a meaning full value in " + fieldTitle, fieldTitle,
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtBox = "0.00";

                    }
                    else
                    {
                        txtBox = ParseDecimal(txtBox);

                        //if (Convert.ToDecimal(txtBox) < 1000)
                        //{
                        //    txtBox = Convert.ToDecimal(txtBox).ToString("0.00");
                        //}
                        //else
                        //{
                        //    txtBox = Convert.ToDecimal(txtBox).ToString("0,0.00");
                        //}

                    }

                }
                else
                {
                    txtBox = "0.00";
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "FormatGridQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "FormatGridQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "FormatGridQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "FormatGridQty", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatGridQty", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatGridQty", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatGridQty", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatGridQty", exMessage);
            }
            #endregion Catch
        }

        public static void CheckVATRate(TextBox txtBox, string fieldTitle)
        {
            try
            {
                string[] names = { "15", "10", "7.5", "5", "4.5", "3", "2.4", "2", "0" };
               


                if (txtBox.Text != string.Empty)
                {
                    txtBox.Text = txtBox.Text.Trim().Replace(",", "");
                    if (IsNumeric(txtBox.Text) == false)
                    {
                        MessageBox.Show("Please enter VAT Rate In " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "0.00";
                        txtBox.Focus();
                    }
                    
                    else if (!IsNumeric(txtBox.Text))
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "0.00";
                        txtBox.Focus();
                    }
                    else
                    {
                        var stringToFind = txtBox.Text.Trim();
                        string result = Array.Find(names, element => element == stringToFind); // returns "Bill"
                        if (result == null)
                        {
                            MessageBox.Show("Please enter VAT Rate in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                         MessageBoxIcon.Information);
                            txtBox.Text = "0.00";
                            txtBox.Focus();

                        }
                        else
                        {
                            txtBox.Text = ParseDecimal(result);
                        
                        }
                        
                    }

                }
                else
                {
                    txtBox.Text = "0.00";
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
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBoxRate", exMessage);
            }
            #endregion Catch

        }

        public static void FormatTextBox(TextBox txtBox, string fieldTitle)
        {
            try
            {
               
                if (txtBox.Text != string.Empty)
                {
                    txtBox.Text = txtBox.Text.Trim().Replace(",", "");
                    if (IsNumeric(txtBox.Text) == false)
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "0.00";
                        txtBox.Focus();
                    }
                    else if (Convert.ToDecimal(txtBox.Text) < 0)
                    {
                        MessageBox.Show("Please enter positive numeric value in " + fieldTitle, fieldTitle,
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtBox.Text = "0.00";
                        txtBox.Focus();
                    }
                    else if (!IsNumeric(txtBox.Text))
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "0.00";
                        txtBox.Focus();
                    }

                    else
                    {
                        txtBox.Text = ParseDecimal(txtBox.Text);

                        //if (Convert.ToDecimal(txtBox.Text) < 1000)
                        //{
                        //    txtBox.Text = Convert.ToDecimal(txtBox.Text).ToString("0.00");
                        //}
                        //else
                        //{
                        //    txtBox.Text = Convert.ToDecimal(txtBox.Text).ToString("0,0.00");
                        //}

                    }

                }
                else
                {
                    txtBox.Text = "0.00";
                }
                //txtBox.Text = ParseDecimal(txtBox.Text);
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "FormatTextBox", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBox", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBox", exMessage);
            }
            #endregion Catch
        }

        public static void FormatTextBoxQty(TextBox txtBox, string fieldTitle)
        {
            try
            {
                if (txtBox.Text != string.Empty)
                {
                    txtBox.Text = txtBox.Text.Trim().Replace(",", "");
                    if (IsNumeric(txtBox.Text) == false)
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "0.0000";
                        txtBox.Focus();
                    }
                    else if (Convert.ToDecimal(txtBox.Text) < 0)
                    {
                        MessageBox.Show("Please enter positive numeric value in " + fieldTitle, fieldTitle,
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtBox.Text = "0.0000";
                        txtBox.Focus();
                    }
                    else if (!IsNumeric(txtBox.Text))
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "0.0000";
                        txtBox.Focus();
                    }

                    else
                    {
                        txtBox.Text = ParseDecimal(txtBox.Text);

                        //if (Convert.ToDecimal(txtBox.Text) < 1000)
                        //{
                        //    txtBox.Text = Convert.ToDecimal(txtBox.Text).ToString("0.0000");
                        //}
                        //else
                        //{
                        //    txtBox.Text = Convert.ToDecimal(txtBox.Text).ToString("0,0.0000");
                        //}
                    }
                }
                else
                {
                    txtBox.Text = "0.00";
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "FormatTextBoxQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "FormatTextBoxQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "FormatTextBoxQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "FormatTextBoxQty", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBoxQty", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBoxQty", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBoxQty", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBoxQty", exMessage);
            }
            #endregion Catch
        }

        
        public static bool CheckingNumericString(string txtBox, string fieldTitle)
        {
            bool isNumeric = false;
            try
            {
                if (txtBox != string.Empty)
                {

                    txtBox = txtBox.Trim().Replace(",", "");
                    
                  //txtBox=txtBox.remo
                    if (IsNumeric(txtBox) == false)
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        isNumeric = false;
                    }
                    else if (Convert.ToDecimal(txtBox) < 0)
                    {
                        MessageBox.Show("Please enter positive numeric value in " + fieldTitle, fieldTitle,
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        isNumeric = false;

                    }
                    else if (!IsNumeric(txtBox))
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        isNumeric = false;

                    }

                    else
                    {
                        isNumeric = true;
                    }

                }
                else
                {
                    isNumeric = false;
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "CheckingNumericString", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "CheckingNumericString", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "CheckingNumericString", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "CheckingNumericString", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "CheckingNumericString", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "CheckingNumericString", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "CheckingNumericString", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "CheckingNumericString", exMessage);
            }
            #endregion Catch
            return isNumeric;
        }

        public static bool CheckingNumericTextBox(TextBox txtBox, string fieldTitle)
        {
            bool isNumeric = false;
            try
            {
                if (txtBox.Text != string.Empty)
                {

                    txtBox.Text = txtBox.Text.Trim().Replace(",", "");
                    if (IsNumeric(txtBox.Text) == false)
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);


                        txtBox.Text = "0";
                        txtBox.Focus();
                        isNumeric = false;
                    }
                    //else if (Convert.ToDecimal(txtBox.Text) < 0)
                    //{
                    //    //MessageBox.Show("Please enter positive numeric value in " + fieldTitle, fieldTitle,
                    //    //                MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    //txtBox.Text = "0";
                    //    //txtBox.Focus();
                    //    isNumeric = false;

                    //}
                    else if (!IsNumeric(txtBox.Text))
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "0";
                        txtBox.Focus();
                        isNumeric = false;

                    }

                    else
                    {
                        txtBox.Text = Convert.ToDecimal(txtBox.Text).ToString();
                        isNumeric = true;
                    }

                }
                else
                {
                    txtBox.Text = "0";
                    isNumeric = false;
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "FormatTextBox", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBox", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBox", exMessage);
            }
            #endregion Catch
            return isNumeric;
        }

        public static string FormatingNumeric(string txtBox, int DecPlace=2)
        {
            string inputValue = txtBox;
            string outPutValue = "0";
            string decPointLen = "";
            try
            {

                //for (int i = 0; i < DecPlace; i++)
                //{
                //    decPointLen = decPointLen + "0";
                //}
                outPutValue = ParseDecimal(inputValue);
                //if (Convert.ToDecimal(inputValue) < 1000)
                //        {
                //            var a = "0." + decPointLen + "";
                //            //outPutValue = Convert.ToDecimal(Convert.ToDecimal(inpQuantity).ToString("0.0000"));
                //            outPutValue = Convert.ToDecimal(inputValue).ToString(a);


                //        }
                //        else
                //        {
                //            var a = "0,0." + decPointLen + "";

                //            //outPutValue = Convert.ToDecimal(Convert.ToDecimal(inpQuantity).ToString("0,0.0000"));
                //            outPutValue = Convert.ToDecimal(inputValue).ToString(a);

                //        }


            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            }
            #endregion Catch

            return outPutValue;
        }
        public static string RemoveStringExpresion(string vstr)
        { 
            string str =vstr;

            str = str.Replace("\n", String.Empty);
            str = str.Replace("\r", String.Empty);
            str = str.Replace("\t", String.Empty);
            str = str.Replace("'", String.Empty);
            var demo = str.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s));
            //str = str.Replace("  ", " ");
            str = string.Join(" ", demo);
            return str;
        }

        public static bool FormatQty(string quantity)
        {
            try
            {
                if (quantity != string.Empty)
                {

                    quantity = quantity.Trim().Replace(",", "");
                    if (IsNumeric(quantity) == false)
                    {
                        return false;
                    }

                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "FormatQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "FormatQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "FormatQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "FormatQty", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatQty", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatQty", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatQty", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatQty", exMessage);
            }
            #endregion Catch
            return false;

        }

        public static void FormatPrefetchTextBox(TextBox txtBox, string fieldTitle)
        {
            try
            {
                if (txtBox.Text != string.Empty)
                {
                   
                    if (IsNumeric(txtBox.Text) == false)
                    {
                        MessageBox.Show("length must numeric value" + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "4";
                        txtBox.Focus();
                    }
                    else if (Convert.ToDecimal(txtBox.Text) < 4 || Convert.ToDecimal(txtBox.Text) > 10)
                    {
                        MessageBox.Show("length between 4 to 10 degits" + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "4";
                        txtBox.Focus();
                    }
                    else if (!IsNumeric(txtBox.Text))
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "4";
                        txtBox.Focus();
                    }

                    else
                    {
                        txtBox.Text = Convert.ToDecimal(txtBox.Text).ToString("0");
                    }

                }
                else
                {
                    txtBox.Text = "4";
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "FormatPrefetchTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "FormatPrefetchTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "FormatPrefetchTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "FormatPrefetchTextBox", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatPrefetchTextBox", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatPrefetchTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatPrefetchTextBox", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "FormatPrefetchTextBox", exMessage);
            }
            #endregion Catch
        }

        public static bool IsNumeric(string valueToCheck)
        {
            try
            {
                if (valueToCheck != string.Empty)
                {
                    decimal tt = 0;
                    valueToCheck = valueToCheck.Trim().Replace(",", "");
                    bool res = Decimal.TryParse(valueToCheck, out tt);
                    //Convert.ToDecimal(valueToCheck);
                    if (res)
                    {
                        return true;
                    }

                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "IsNumeric", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "IsNumeric", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "IsNumeric", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "IsNumeric", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsNumeric", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsNumeric", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsNumeric", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsNumeric", exMessage);
            }
            #endregion Catch
            return false;

        }

        public static bool IsString(string valueToCheck)
        {
            try
            {
                return true;
            }
            #region Catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "IsString", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "IsString", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "IsString", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "IsString", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsString", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsString", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsString", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsString", exMessage);
            }

            #endregion Catch

            return false;

        }

        public static bool IsInteger(string valueToCheck)
        {
            try
            {
                if (valueToCheck != string.Empty)
                {
                    valueToCheck = valueToCheck.Trim().Replace(",", "");
                    Convert.ToInt32(valueToCheck);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "IsInteger", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "IsInteger", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "IsInteger", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "IsInteger", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsInteger", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsInteger", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsInteger", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsInteger", exMessage);
            }
            #endregion Catch

            return false;

        }

        public static bool IsDate(string valueToCheck)
        {
            try
            {
                Convert.ToDateTime(valueToCheck);
                return true;
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "IsDate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "IsDate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "IsDate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "IsDate", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsDate", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsDate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsDate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsDate", exMessage);
            }
            #endregion Catch

            return false;

        }

        public static bool IsActive(string valueToCheck)
        {
            try
            {
                if (valueToCheck.Length == 1)
                {
                    if (valueToCheck.Trim() == "Y" || valueToCheck.Trim() == "N")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }

            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "IsActive", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "IsActive", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "IsActive", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "IsActive", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsActive", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsActive", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsActive", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "IsActive", exMessage);
            }
            #endregion Catch

            return false;

        }

        #region Settings Methods

        public static bool IsSettingString(string valueToCheck)
        {
            return true;
        }

        public static bool IsSettingInteger(string valueToCheck)
        {
            try
            {
                Convert.ToInt32(valueToCheck);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static bool IsSettingDecimal(string valueToCheck)
        {
            try
            {
                Convert.ToDecimal(valueToCheck);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static bool IsSettingDate(string valueToCheck)
        {
            try
            {
                Convert.ToDateTime(valueToCheck);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static bool IsSettingActive(string valueToCheck)
        {
            try
            {
                if (valueToCheck.Length == 1)
                {
                    if (valueToCheck.Trim() == "Y" || valueToCheck.Trim() == "N")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        #endregion

        public static void formatTextBoxPrefetch(TextBox txtBox, string fieldTitle)
        {
            try
            {
                if (txtBox.Text != string.Empty)
                {
                    if (Convert.ToInt32(txtBox.Text.Length) != 3)
                    {
                        MessageBox.Show("Length of prefetch must 3 degits in " + fieldTitle, fieldTitle,
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtBox.Text = "***";
                        txtBox.Focus();

                        return;
                    }
                    else
                    {
                        txtBox.Text = txtBox.Text.ToUpper();
                    }

                }
                else
                {
                    txtBox.Text = "***";
                    txtBox.Focus();
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "formatTextBoxPrefetch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "formatTextBoxPrefetch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "formatTextBoxPrefetch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "formatTextBoxPrefetch", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetch", exMessage);
            }
            #endregion Catch

        }

        public static void formatTextBoxPrefetchLength(TextBox txtBox, string fieldTitle)
        {
            try
            {
                if (txtBox.Text != string.Empty)
                {
                    txtBox.Text = txtBox.Text.Trim().Replace(",", "");
                    if (IsNumeric(txtBox.Text) == false)
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "4";
                        txtBox.Focus();
                        return;
                    }
                    else if (Convert.ToDecimal(txtBox.Text.Trim()) < 4 || Convert.ToDecimal(txtBox.Text.Trim()) > 10)
                    {
                        MessageBox.Show("Please input length between 4-10", fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        txtBox.Text = "4";
                    }
                    else
                    {
                        txtBox.Text = Convert.ToDecimal(txtBox.Text).ToString("0");
                    }
                }
                else
                {
                    txtBox.Text = "4";
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "formatTextBoxPrefetchLength", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "formatTextBoxPrefetchLength", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "formatTextBoxPrefetchLength", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "formatTextBoxPrefetchLength", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetchLength", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetchLength", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetchLength", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetchLength", exMessage);
            }
            #endregion Catch

        }

        public static bool formatCodePrefix(string fieldTitle)
        {
            try
            {
                if (!String.IsNullOrEmpty(fieldTitle))
                {
                    if (fieldTitle.Length != 3)
                    {
                        MessageBox.Show("Code prefix must be 3 characters " + fieldTitle, fieldTitle,
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                        return false;
                    }
                    else if (fieldTitle.Length == 3)
                    {
                        return true;
                    }

                }

            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "formatTextBoxPrefetch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "formatTextBoxPrefetch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "formatTextBoxPrefetch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "formatTextBoxPrefetch", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetch", exMessage);
            }
            #endregion Catch

            return false;

        }

        public static bool formatCodeLength(string fieldTitle)
        {
            try
            {
                if (!String.IsNullOrEmpty(fieldTitle))
                {

                    if (IsNumeric(fieldTitle) == false)
                    {
                        MessageBox.Show("Please enter numeric value in " + fieldTitle, fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                        return false;
                    }
                    else if (Convert.ToDecimal(fieldTitle) < 4 || Convert.ToDecimal(fieldTitle) > 6)
                    {
                        MessageBox.Show("Please input length between 4-6", fieldTitle, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("Program", "formatTextBoxPrefetchLength", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("Program", "formatTextBoxPrefetchLength", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("Program", "formatTextBoxPrefetchLength", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("Program", "formatTextBoxPrefetchLength", exMessage);
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetchLength", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetchLength", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetchLength", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Program", "formatTextBoxPrefetchLength", exMessage);
            }

            return false;

            #endregion Catch

        }

        public static void CurrentUserRollSearch(string CurrentUserID)
        {
            try
            {
                string DatabaseName = "";
                UserInformationDAL userInformationDal = new UserInformationDAL();
                string RollResult = userInformationDal.SearchUserRoll(CurrentUserID);
                string decriptedProductData = Converter.DESDecrypt(PassPhrase, EnKey, RollResult);
                Program.PublicRollLines = decriptedProductData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("CurrentUserRollSearch", "UserRollSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "CurrentUserRollSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("CurrentUserRollSearch", "UserRollSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "CurrentUserRollSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("CurrentUserRollSearch", "UserRollSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "CurrentUserRollSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("CurrentUserRollSearch", "UserRollSearch", exMessage);
                MessageBox.Show(ex.Message, "CurrentUserRollSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "CurrentUserRollSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("CurrentUserRollSearch", "UserRollSearch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "CurrentUserRollSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("CurrentUserRollSearch", "UserRollSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "CurrentUserRollSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("CurrentUserRollSearch", "UserRollSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "CurrentUserRollSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("CurrentUserRollSearch", "UserRollSearch", exMessage);
            }
            #endregion
        }

        public static string AddSpacesToSentence(string text)
        {
            String PreString = text;
            StringBuilder SB = new StringBuilder();
            int past = 0, pre = 0;
            for (int i = 0; i < PreString.Length; i++)
            {
                var a = PreString.Substring(i, 1);
                Char b = Convert.ToChar(a);

                if (Char.IsUpper(b))
                {
                    if (pre != i - 1)
                    {
                        SB.Append(' ');
                    }
                    SB.Append(b);
                    pre = i;
                }
                else if (Char.IsNumber(b))
                {
                    if (past != i - 1)
                    {
                        SB.Append(' ');
                    }
                    SB.Append(b);
                    past = i;
                }
                else if (Char.IsLetterOrDigit(b) == false)
                {
                    SB.Append(' ');
                    SB.Append(b);
                }
                //else if (b.ToString() == ")" || b.ToString() == "(")
                //{
                //    SB.Append(' ');
                //    SB.Append(b);
                //}
                else
                {
                    SB.Append(b);
                    pre = 0;
                    past = 0;
                }
            }
            return SB.ToString();
        }

        public static string ToTitle(string input)
        {
            string text = string.Empty;
            text = input;// "MD. KAMRUL HASAN";
            string output = string.Empty;
            int c = text.Length;
            for (int i = 0; i < c; i++)
            {
                if (i == 0)
                {
                    output = output + text[i].ToString().ToUpper();

                }
                else if (text[i].ToString() == " ")
                {
                    output = output + text[i].ToString();
                    output = output + text[i + 1].ToString().ToUpper();
                    i++;
                }
                else
                {
                    output = output + text[i].ToString().ToLower();
                }
            }
            return output;
        }

        public static string ConvertToWords(String numb)
        {

            numb = numb.Replace(",", "");
            String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
            String endStr = "Only";
            try
            {
                int decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) > 0)
                    {
                        andStr = "and";// just to separate whole numbers from points/cents  
                        endStr = "Paisa " + endStr;//Cents  
                        pointStr = ConvertDecimals(points);
                    }
                }
                val = String.Format("{0} {1}{2} {3}", ConvertWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
            }
            catch { }
            return val;
        }
        public static string ConvertWholeNumber(String Number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX
                bool isDone = false;//test if already translated
                double dblAmt = (Convert.ToDouble(Number));
                //if ((dblAmt > 0) && number.StartsWith("0"))
                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric
                    beginsZero = Number.StartsWith("0");

                    int numDigits = Number.Length;
                    int pos = 0;//store digit grouping
                    String place = "";//digit grouping name:hundres,thousand,etc...
                    switch (numDigits)
                    {
                        case 1://ones' range

                            word = ones(Number);
                            isDone = true;
                            break;
                        case 2://tens' range
                            word = tens(Number);
                            isDone = true;
                            break;
                        case 3://hundreds' range
                            pos = (numDigits % 3) + 1;
                            place = " Hundred ";
                            break;
                        case 4://thousands' range
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 7://millions' range
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Million ";
                            break;
                        case 10://Billions's range
                        case 11:
                        case 12:

                            pos = (numDigits % 10) + 1;
                            place = " Billion ";
                            break;
                        //add extra case options for anything above Billion...
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)
                        if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0")
                        {
                            try
                            {
                                word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));
                            }
                            catch { }
                        }
                        else
                        {
                            word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));
                        }

                        //check for trailing zeros
                        //if (beginsZero) word = " and " + word.Trim();
                    }
                    //ignore digit grouping names
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch { }
            return word.Trim();
        }
        public static string ParseDecimal(string numb)
        {

            String val = "0";
            try
            {
                numb = numb.Replace(",", "");

                if (string.IsNullOrWhiteSpace(numb))
                {
                    numb = "0";
                }
                CommonDAL _cDal = new CommonDAL();
                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric");
                string Pre = "";
                Pre = Pre.PadRight(Convert.ToInt32(FormNumeric), '#');

                val = decimal.Parse(numb.ToString(), System.Globalization.NumberStyles.Float).ToString("#,###0." + Pre);

            }
            catch { }
            return val;
        }
        public static string ParseDecimalObject(object numb)
        {

            String val = "0";
            string a = "str123";
            string numbField = numb.ToString();

            

            try
            {
                //for (int i = 0; i < numb.ToString().Length; i++)
                //{

                //    if (numb.ToString()[i].ToString() == ".")
                //    {
                //        numbField += numb.ToString()[i];
                //    }
                //    else
                //    {
                //        if (Char.IsDigit(numb.ToString()[i]))
                //            numbField += numb.ToString()[i];
                //    }
                //}

                if (string.IsNullOrWhiteSpace(numbField.ToString()))
                {
                    numbField = "0";
                }
                else
                {
                    numbField = numbField.ToString().Replace(",", "");
                }
                CommonDAL _cDal = new CommonDAL();
                string Quantity6_1 = _cDal.settingsDesktop("DecimalPlace", "FormNumeric");
                string Pre = "";
                Pre = Pre.PadRight(Convert.ToInt32(Quantity6_1), '#');
                val = decimal.Parse(numbField.ToString(), System.Globalization.NumberStyles.Float).ToString("#,###0." + Pre);

            }
            catch { }
            return val;
        }

        public static DateTime ParseDate(DateTimePicker numb)
        {

            DateTime val = DateTime.Now ;
            try
            {
                
                CommonDAL _cDal = new CommonDAL();

                string BackDateEntry = _cDal.settingsDesktop("EntryProcess", "BackDateEntry");

                if (BackDateEntry.ToLower() == "y")
                {
                    val = numb.Value;

                }
                else
                {
                    if (numb.Value < val)
                    {


                        val = Program.serverDate;
                        MessageBox.Show("Back Date Entry Process Not allow", "BackDateEntry", MessageBoxButtons.OK,
                                           MessageBoxIcon.Information);
                    }
                    else
                    {
                        val = numb.Value;
                    }
                }
                

            }
            catch { }
            return val;
        }
        
        public static String tens(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = null;
            switch (_Number)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Fourty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (_Number > 0)
                    {
                        name = tens(Number.Substring(0, 1) + "0") + " " + ones(Number.Substring(1));
                    }
                    break;
            }
            return name;
        }
        public static String ones(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = "";
            switch (_Number)
            {

                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }

        public static String ConvertDecimals(String number)
        {
            String cd = "", digit = "", engOne = "";
            for (int i = 0; i < number.Length; i++)
            {
                digit = number[i].ToString();
                if (digit.Equals("0"))
                {
                    engOne = "Zero";
                }
                else
                {
                    engOne = ones(digit);
                }
                cd += " " + engOne;
            }
            return cd;
        }


        public static string GetAppVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            var about = string.Format(CultureInfo.InvariantCulture, @"{0}.{1}.{2} (r{3})", version.Major, version.Minor, version.Build, version.Revision);

            return about;
        }
        public static SysDBInfoVMTemp OrdinaryLoad()
        {
            SysDBInfoVMTemp vm = new SysDBInfoVMTemp();
            vm.SysdataSource = SysDBInfoVM.SysdataSource;
            vm.SysPassword = SysDBInfoVM.SysPassword;
            vm.SysUserName = SysDBInfoVM.SysUserName;
            vm.SysDatabaseName = DatabaseInfoVM.DatabaseName;

            vm.CompanyName = Program.CompanyName;
            vm.CompanyID = Program.CompanyID;
            vm.CompanyNameLog = Program.CompanyNameLog;
            vm.CompanyName = Program.CompanyName;
            vm.CompanyLegalName = Program.CompanyLegalName;
            vm.Address1 = Program.Address1;
            vm.Address2 = Program.Address2;
            vm.Address3 = Program.Address3;
            vm.City = Program.City;
            vm.ZipCode = Program.ZipCode;
            vm.TelephoneNo = Program.TelephoneNo;
            vm.FaxNo = Program.FaxNo;
            vm.Email = Program.Email;
            vm.ContactPerson = Program.ContactPerson;
            vm.ContactPersonDesignation = Program.ContactPersonDesignation;
            vm.ContactPersonTelephone = Program.ContactPersonTelephone;
            vm.ContactPersonEmail = Program.ContactPersonEmail;
            vm.TINNo = Program.TINNo;
            vm.VatRegistrationNo = Program.VatRegistrationNo;
            vm.Comments = Program.Comments;
            vm.ActiveStatus = Program.ActiveStatus;
            vm.FMonthStart = Program.FMonthStart;
            vm.FMonthEnd = Program.FMonthEnd;
            vm.VATAmount = Program.VATAmount;
            vm.BranchId = Program.BranchId;
            vm.BranchCode = Program.BranchCode;
            vm.CurrentUser = Program.CurrentUser;
            vm.CurrentUserID = Program.CurrentUserID;
            vm.IsLoading = Program.IsLoading;
            vm.R_F = Program.R_F;
            vm.fromOpen = Program.fromOpen;
            vm.SalesType = Program.SalesType;
            vm.Trading = Program.Trading;
            vm.DatabaseName = Program.DatabaseName;
            vm.PublicRollLines = Program.PublicRollLines;
            vm.SessionDate = Program.SessionDate;
            vm.SessionTime = Program.SessionTime;
            vm.ChangeTime = Program.ChangeTime;
            vm.ServerDateTime = Program.ServerDateTime;
            vm.vMinDate = Program.vMinDate;
            vm.vMaxDate = Program.vMaxDate;
            vm.successLogin = Program.successLogin;
            vm.FontSize = Program.FontSize;
            vm.Access = Program.Access;
            vm.Post = Program.Post;
            vm.LicenceDate = Program.LicenceDate;
            vm.serverDate = Program.serverDate;
            vm.IsTrial = Program.IsTrial;
            vm.Trial = Program.Trial;
            vm.TrialComments = Program.TrialComments;
            vm.ImportFileName = Program.ImportFileName;
            vm.ItemType = Program.ItemType;
            vm.IsAlpha = Program.IsAlpha;
            vm.Alpha = Program.Alpha;
            vm.AlphaComments = Program.AlphaComments;
            vm.IsBeta = Program.IsBeta;
            vm.Beta = Program.Beta;
            vm.BetaComments = Program.BetaComments;
            vm.IsBureau = Program.IsBureau;
            vm.Add = Program.Add;
            vm.Edit = Program.Edit;
            vm.IsWCF = Program.IsWCF;
            vm.Section = Program.Section;

            return vm;
        }


    }
}
