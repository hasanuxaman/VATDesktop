using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SymphonySofttech.Reports.Report;
using VATServer.Library;
using VATServer.License;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using System.Data.SqlClient;
using SymphonySofttech.Reports.List;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using SymphonySofttech.Reports.Report.V12V2;
using VATServer.Library.Integration;
using VATServer.Interface;
using VATDesktop.Repo;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Threading;
using System.Net;
using CrystalDecisions.Shared;
using System.Reflection;

namespace SymphonySofttech.Reports
{
    public class NBRReports
    {
        #region Global Variable
        public bool PreviewOnly = false;
        //public bool ckbVAT9_1 = false;
        public string branchName = "";
        public string VAT11Name = "";
        public string VATName = "";
        CommonDAL commonDal = new CommonDAL();
        public string InEnglish = "";
        public string FormNumeric = string.Empty;
        DataSet ReportResultTracking = new DataSet();
        VATRegistersDAL _vatRegistersDAL = new VATRegistersDAL();
        #endregion


        public ReportDocument VAT6_10Report(string TotalAmount, string StartDate, string EndDate, string post1, string post2, int BranchId = 0, string InEnglish = "", SysDBInfoVMTemp connVM = null)
        {

            try
            {
                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }

                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion
                #region User Information
                UserInformationVM uvm = new UserInformationVM();
                UserInformationDAL _udal = new UserInformationDAL();
                uvm = _udal.SelectAll(Convert.ToInt32(OrdinaryVATDesktop.CurrentUserID), null, null, null, null, connVM).FirstOrDefault();
                #endregion
                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.VAT6_10Report(TotalAmount, StartDate, EndDate, post1, post2, BranchId, connVM);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "DsVA6_10";
                ReportDocument objrpt = new ReportDocument();
                if (InEnglish == "N")
                {
                    objrpt = new RptVAT6_10();

                }
                else
                {
                    objrpt = new RptVAT6_10_English();

                }

                //objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_10.rpt");


                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                objrpt.SetDataSource(ReportResult);

                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + branchName + "'";
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SignatoryName", uvm.FullName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SignatoryDesig", uvm.Designation);


                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ReportDocument TransferIssueNew(string TransferIssueNo, string IssueDateFrom, string IssueDateTo, string itemNo, string categoryID, string productType, string TransactionType, string Post, bool PreviewOnly, string DBName = null, SysDBInfoVMTemp connVM = null)
        {

            try
            {

                #region Settings
                string vVAT2012V2 = "2012-Jul-01";

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }
                InEnglish = commonDal.settingsDesktop("Reports", "VAT6_5English", settingsDt, connVM);
                VAT11Name = commonDal.settingsDesktop("Reports", "VAT6_3", settingsDt, connVM);

                string CompanyCode = commonDal.settingsDesktop("CompanyCode", "Code", settingsDt, connVM);


                string Quantity6_5 = commonDal.settingsDesktop("DecimalPlace", "Quantity6_5", settingsDt, connVM);
                string Amount6_5 = commonDal.settingsDesktop("DecimalPlace", "Amount6_5", settingsDt, connVM);
                string FontSize = commonDal.settingsDesktop("FontSize", "VAT6_5", settingsDt, connVM);
                vVAT2012V2 = commonDal.settingsDesktop("Version", "VAT2012V2", settingsDt, connVM);
                bool TrackingTrace = Convert.ToBoolean(commonDal.settingsDesktop("TrackingTrace", "Tracking", settingsDt, connVM) == "Y" ? true : false);
                string Heading1 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_1", settingsDt, connVM);
                string Heading2 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_2", settingsDt, connVM);

                #endregion

                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                DateTime TransferDateTime = DateTime.Now;
                DateTime VAT2012V2 = Convert.ToDateTime(vVAT2012V2);

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                #region Print Flag Update


                if (CompanyCode == "BCL")
                {
                    if (PreviewOnly == false)
                    {
                        IntegrationParam paramVM = new IntegrationParam();
                        paramVM.MulitipleInvoice = TransferIssueNo;
                        new BeximcoIntegrationDAL().PrintSource_TransferData(paramVM, connVM);
                    }

                }

                else if (OrdinaryVATDesktop.IsACICompany(CompanyCode))
                {
                    if (PreviewOnly == false)
                    {
                        IntegrationParam paramVM = new IntegrationParam();
                        paramVM.MulitipleInvoice = TransferIssueNo;
                        new ImportDAL().PrintSource_TransferData(paramVM, connVM);
                    }

                }
                #endregion


                ReportResult = reportDsdal.TransferIssueNew(TransferIssueNo, IssueDateFrom, IssueDateTo, itemNo, categoryID, productType, TransactionType, Post, DBName, connVM);
                TransferDateTime = Convert.ToDateTime(ReportResult.Tables[0].Rows[0]["TransferDateTime"]);

                ReportResultTracking = reportDsdal.TransferTrackingReport(TransferIssueNo, connVM);

                ReportResult.Tables[0].TableName = "DsTransferIssue";
                ReportResultTracking.Tables[0].TableName = "DsSaleTracking";
                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportDocument objrpt = new ReportDocument();
                var dal = new BranchProfileDAL();

                var toVM = dal.SelectAllList(ReportResult.Tables[0].Rows[0]["TransferTo"].ToString(), null, null, null, null, connVM).FirstOrDefault();
                var FromVM = dal.SelectAllList(ReportResult.Tables[0].Rows[0]["FromBranchId"].ToString(), null, null, null, null, connVM).FirstOrDefault();

                var legalName = toVM == null ? branchName : toVM.BranchLegalName;
                var legalName1 = FromVM == null ? branchName : FromVM.BranchLegalName;
                var fromBranchAddress = FromVM == null ? "-" : FromVM.Address;

                if (VAT11Name.ToLower() == "acig")
                {
                    UserInformationVM uvm = new UserInformationVM();
                    UserInformationDAL _udal = new UserInformationDAL();
                    uvm = _udal.SelectAll(Convert.ToInt32(OrdinaryVATDesktop.CurrentUserID), null, null, null, null, connVM).FirstOrDefault();

                    //objrpt = new RptVAT6_5_ACIG();
                    if (VAT2012V2 <= TransferDateTime)
                    {
                        ////New Report -- 
                        objrpt = new RptVAT6_5_ACIG_V12V2();
                    }
                    else
                    {
                        objrpt = new RptVAT6_5_ACIG();
                    }

                    objrpt.DataDefinition.FormulaFields["UserFullName"].Text = "'" + uvm.FullName + "'";
                    objrpt.DataDefinition.FormulaFields["UserDesignation"].Text = "'" + uvm.Designation + "'";
                }
                else if (VAT11Name.ToLower() == "igloo")
                {
                    //objrpt = new RptVAT6_5_Legal();
                    if (VAT2012V2 <= TransferDateTime)
                    {
                        ////New Report -- 
                        objrpt = new RptVAT6_5_Legal_V12V2();
                    }
                    else
                    {
                        objrpt = new RptVAT6_5_Legal();
                    }
                }
                else if (VAT11Name.ToLower() == "aci" || VAT11Name.ToLower() == "acicentral")
                {
                    objrpt = new RptVAT6_5_ACI_V12V2();
                }

                else if (VAT11Name.ToLower() == "nita")
                {
                    objrpt = new RptVAT6_5_NITA_V12V2();
                    //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_5_NITA_V12V2.rpt");

                }
                #region Berger Fosroc
                else if (VAT11Name.ToLower() == "bfl")
                {
                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_5_BFL_V12V2.rpt");
                    objrpt = new RptVAT6_5_BFL_V12V2();
                }
                #endregion
                //Jenson & Nicholson (BD) Ltd

                else if (VAT11Name.ToLower() == "jnbl")
                {
                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_5_JNNB_V12V2.rpt");

                    objrpt = new RptVAT6_5_JNNB_V12V2();

                }
                else if (VAT11Name.ToLower() == "cptri")
                {
                    objrpt = new RptVAT6_5_CPT_V12V2();
                }


                else if (VAT11Name.ToLower() == "scbl")
                {
                    UserInformationVM uvm = new UserInformationVM();
                    UserInformationDAL _udal = new UserInformationDAL();

                    uvm = _udal.SelectAll(Convert.ToInt32(OrdinaryVATDesktop.CurrentUserID), null, null, null, null, connVM).FirstOrDefault();
                    //objrpt = new RptVAT6_5_LetterSCBL();
                    //objrpt = new RptVAT6_5_LetterSCBLLandscapre();
                    if (VAT2012V2 <= TransferDateTime)
                    {
                        ////New Report -- 
                        objrpt = new RptVAT6_5_LetterSCBLLandscapre_V12V2();
                    }
                    else
                    {
                        objrpt = new RptVAT6_5_LetterSCBLLandscapre();
                    }
                    //objrpt.Load(OrdinaryVATDesktop.AppPathForRootLocation + @"\RptVAT6_5_SCBL.rpt");

                    objrpt.DataDefinition.FormulaFields["UserFullName"].Text = "'" + uvm.FullName + "'";
                    objrpt.DataDefinition.FormulaFields["UserDesignation"].Text = "'" + uvm.Designation + "'";

                }
                else
                {
                    //objrpt = new RptVAT6_5();
                    if (VAT2012V2 <= TransferDateTime)
                    {
                        ////New Report -- 
                        objrpt = new RptVAT6_5_V12V2();
                    }
                    else
                    {
                        objrpt = new RptVAT6_5();
                    }

                    //objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_5.rpt");

                }
                //objrpt = new RptVAT6_5();
                //objrpt.Load(Program.ReportAppPath + @"\RptVAT6_5.rpt");




                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["VATRegNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + legalName + "'";
                objrpt.DataDefinition.FormulaFields["FromBranchName"].Text = "'" + legalName1 + "'";
                objrpt.DataDefinition.FormulaFields["FromBranchAddress"].Text = "'" + fromBranchAddress + "'";
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                if (TrackingTrace == true)
                {
                    objrpt.Subreports[0].SetDataSource(ReportResultTracking);
                    objrpt.Subreports[0].DataDefinition.FormulaFields["HeadingName1"].Text = "'" + Heading1 + "'";
                    objrpt.Subreports[0].DataDefinition.FormulaFields["HeadingName2"].Text = "'" + Heading2 + "'";
                }
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_5", Quantity6_5);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_5", Amount6_5);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VATRegNo", CompanyVm.VatRegistrationNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TrackingTrace", TrackingTrace.ToString());



                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ReportDocument BOMNew(string BOMId, string VATName, string IsPercent, int BranchId = 0, string InEnglish = "N", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vVAT2012V2 = "2012-Jul-01";
                DataTable settingsDt = new DataTable();
                CommonDAL commonDal = new CommonDAL();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }
                vVAT2012V2 = commonDal.settingsDesktop("Version", "VAT2012V2", settingsDt, connVM);

                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }

                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                DateTime EffectDate = DateTime.Now;
                DateTime VAT2012V2 = Convert.ToDateTime(vVAT2012V2);

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.BOMNew(BOMId, VATName, IsPercent, BranchId, connVM);
                EffectDate = Convert.ToDateTime(ReportResult.Tables[0].Rows[0]["EffectDate"]);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "BOMFinish";
                ReportResult.Tables[1].TableName = "BOMRaw";
                ReportResult.Tables[2].TableName = "BOMOh";
                ReportResult.Tables[3].TableName = "BOMRawP";
                ReportResult.Tables[4].TableName = "BOMOhP";
                bool isIntermediateProduction = false;

                string vItemNature, visIntermediateProduction = string.Empty;
                vItemNature = commonDal.settingsDesktop("BOM", "ItemNature", settingsDt, connVM);
                string vReportName = commonDal.settingsDesktop("BOM", "ReportName", settingsDt, connVM);
                string VAT4_3 = commonDal.settingsDesktop("Reports", "VAT4.3", settingsDt, connVM);
                visIntermediateProduction = commonDal.settingsDesktop("BOM", "IntermediateProduction", settingsDt, connVM);

                if (string.IsNullOrEmpty(vItemNature)
                    || string.IsNullOrEmpty(visIntermediateProduction))
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave);
                    return null;
                }

                string ItemNature = vItemNature;

                isIntermediateProduction = Convert.ToBoolean(visIntermediateProduction == "Y" ? true : false);

                BOMDAL _bomdal = new BOMDAL();
                DataTable dt = new DataTable();

                dt = _bomdal.CustomerByBomId(ReportResult.Tables[0].Rows[0]["BOMId"].ToString(), connVM);
                ReportDocument objrpt = new ReportDocument();

                if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                {
                    objrpt = new ReportDocument();
                    //if (vReportName.ToLower() == "pccl")
                    //{
                    //    objrpt = new RptVAT4_3_Padma();
                    //}
                    //else
                    //{
                    //objrpt.Load(Program.ReportAppPath + @"\RptVAT4_3.rpt");

                    if (VATName.ToLower() == "vat porisisto ka")
                    {
                        objrpt = new RptVATPoristoKa();
                        //objrpt.Load(Program.ReportAppPath + @"\RptVATPoristoKa.rpt");

                    }
                    else
                    {
                        ////objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report\" + @"\RptVAT4_3.rpt");

                        //objrpt = new RptVAT4_3();
                        if (VAT2012V2 <= EffectDate)
                        {
                            if (VAT4_3.ToLower() == "rtl")
                            {
                                objrpt = new RptVAT4_3_RupshaTyre();

                            }

                            else if (VAT4_3.ToLower() == "arbab")
                            {
                                objrpt = new RptVAT4_3_V12V2_Arbab();
                                //objrpt = new RptVAT4_3_ArbabPolypack();

                            }


                            else
                            {
                                objrpt = new RptVAT4_3_V12V2();

                            }
                        }
                        else
                        {
                            objrpt = new RptVAT4_3();

                        }


                    }

                    //objrpt = new RptVAT4_3();

                    //}

                }
                CommonDAL _cDal = new CommonDAL();
                string Quantity4_3 = _cDal.settingsDesktop("DecimalPlace", "Quantity4_3", settingsDt, connVM);
                string Amount4_3 = _cDal.settingsDesktop("DecimalPlace", "Amount4_3", settingsDt, connVM);

                objrpt.DataDefinition.FormulaFields["Quantity4_3"].Text = "'" + Quantity4_3 + "'";
                objrpt.DataDefinition.FormulaFields["Amount4_3"].Text = "'" + Amount4_3 + "'";

                //objrpt.SetParameterValue("@Amount4_3", Amount4_3, "OH");
                //RptVAT1New objrpt = new RptVAT1New();
                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["CustomerName"].Text = "'" + dt.Rows[0]["CustomerName"] + "'";
                objrpt.DataDefinition.FormulaFields["BOMCode"].Text = "'" + dt.Rows[0]["BOMCode"] + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + OrdinaryVATDesktop.ZipCode + "'";
                //objrpt.DataDefinition.FormulaFields["OHTotal"].Text = "'" + TotalOH + "'";
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";

                objrpt.DataDefinition.FormulaFields["ItemNature"].Text = "'" + OrdinaryVATDesktop.ToTitle(ItemNature) + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument VAT7_1(string ddbackno, string salesInvoice, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                //DataSet ReportResult = new DataSet();

                DutyDrawBackDAL _dal = new DutyDrawBackDAL();

                DataTable ReportResultDt = new DataTable();
                ReportResultDt = _dal.VAT7_1(ddbackno, salesInvoice, connVM);


                #region Complete
                if (ReportResultDt.Rows.Count <= 0)
                {
                    return null;
                }
                ReportResultDt.TableName = "dtVAT7_1";
                ReportDocument objrpt = new ReportDocument();
                objrpt = new Rpt7_1();
                //objrpt.Load(Program.ReportAppPath + @"\Rpt7_1.rpt");
                objrpt.SetDataSource(ReportResultDt);

                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                // objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + Program.ZipCode + "'";
                //objrpt.DataDefinition.FormulaFields["UsedQuantity"].Text = "" + 2 + "";


                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument SD_Data(string UserName, string StartDate, string EndDate, string post1, string post2, string InEnglish = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {


                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.SD_Data(UserName, StartDate, EndDate, post1, post2, connVM);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "DsVAT18";
                //ReportResult.Tables[0].TableName = "DsReportSD";
                RptSD objrpt = new RptSD();
                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                objrpt.SetDataSource(ReportResult);
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);



                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument Chak_kaReport(string StartDate, string EndDate, int BranchId = 0, int TransferTo = 0, string InEnglish = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                //InEnglish = commonDal.settingsDesktop("Reports", "VAT6_3English");

                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }


                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.Chak_kaReport(StartDate, EndDate, BranchId, TransferTo, connVM);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "dtChakKa";



                ReportDocument objrpt = new ReportDocument();
                objrpt = new RptChakKa();

                ////RptChakKa 

                ////objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptChakKa.rpt");


                objrpt.SetDataSource(ReportResult);


                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);


                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument Chak_khaReport(string StartDate, string EndDate, int BranchId = 0, string InEnglish = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {

                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.Chak_khaReport(StartDate, EndDate, BranchId, connVM);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "dtChakKha";



                RptChakKha objrpt = new RptChakKha();

                objrpt.SetDataSource(ReportResult);


                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);



                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument VAT6_1_WithConnBackup(string ItemNo, string IssueFromDate, string IssueToDate, string post1, string post2
            , int BranchId, bool PreviewOnly, string InEnglish = "N", decimal UomConversion = 1, string UOM = "", SysDBInfoVMTemp connVM = null)
        {
            return null;
        }

        public ReportDocument VAT6_1_WithConn(VAT6_1ParamVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Comments


            #endregion

            #region Variables and Objects

            string IsRaw = "";
            ReportDocument objrpt = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            string FromDate = vm.StartDate;
            string ToDate = vm.EndDate;
            #endregion
            VATRegistersDAL _vATRegistersDAL = new VATRegistersDAL();
            List<VAT_16> vat16s = new List<VAT_16>();
            VAT_16 vat16 = new VAT_16();
            DataSet ReportResult = new DataSet();
            DataTable ReportResultDt = new DataTable();
            try
            {
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                #region UOM Conversion

                string UOMConversion = "";

                UOMConversion = Convert.ToString(vm.UOMConversion > 0 ? vm.UOMConversion : 1);

                #endregion

                #region Settings Load

                CommonDAL commonDal = new CommonDAL();

                bool Permanent6_1 = commonDal.settings("VAT6_1", "6_1Permanent", null, null, connVM) == "Y";
                bool DayEndProcess = commonDal.settingsMaster("DayEnd", "DayEndProcess", null, null, connVM) == "Y";
                DataTable settingsDt = new DataTable();
                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion


                #region Data Call

                decimal Quanity = 0;
                decimal UnitCost = 0;
                decimal RunningTotal = 0;
                decimal RunningValue = 0;
                decimal vTempVATAmount = 0;
                decimal vTempSDAmount = 0;

                ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();


                if (vm.FromSP)
                {
                    ReportResult = _vATRegistersDAL.Save6_1_ReportFromPermanentByQuery(vm, connVM);
                    List<string> oldColumnNames = new List<string> { "RowNumber", "StartDateTime", "StartingQuantity", "StartingAmount", "XXXXXXX", "StartDateTime", "VendorName", "VendorAddress", "VendorBIN", "ProductNameColumn10", "purchaseQuantity", "purchaseAmount", "SD", "VATRate", "IssueQuantity", "IssueAmount", "RunningTotal", "RunningValue", "Remarks", "XXXXXXX", "XXXXXXX" };
                    List<string> newColumnNames = new List<string> { "Column1", "Column2", "Column3", "Column4", "Column5", "Column6", "Column7", "Column8", "Column9", "Column10", "Column11", "Column12", "Column13", "Column14", "Column15", "Column16", "Column17", "Column18", "Column19" };

                    ReportResultDt = OrdinaryVATDesktop.DtColumnNameChangeList(ReportResult.Tables[0], oldColumnNames, newColumnNames);

                    //OrdinaryVATDesktop.SaveExcel(ReportResultDt, "New", "NewData");
                }
                else
                {
                    reportDsdal = new ReportDSDAL();
                    #region Else
                    if (vm.IsMonthly == true)
                    {
                        ReportResult = _vatRegistersDAL.VAT6_1_Permanent_DayWise(vm, null, null, connVM);
                    }
                    else
                    {
                        ReportResult = _vatRegistersDAL.VAT6_1_WithConn(vm, null, null, connVM);
                    }

                    if (ReportResult != null && ReportResult.Tables.Count > 0)
                    {
                        if (ReportResult.Tables[0] != null && ReportResult.Tables[0].Rows.Count > 0)
                        {
                            if (ReportResult.Tables[0].Columns.Contains("CreateDateTime"))
                            {
                                ReportResult.Tables[0].Columns["CreateDateTime"].ColumnName = "CreatedDateTime";
                            }
                        }
                    }
                    else if (vm.IsTopSheet == true || vm.IsGroupTopSheet == true)
                    {
                        if (ReportResult != null && ReportResult.Tables.Count > 0)
                        {
                            if (ReportResult.Tables[0] != null && ReportResult.Tables[0].Rows.Count > 0)
                            {
                                ReportResult = reportDsdal.VAT6_1_Monthly(ReportResult, vm.StartDate, vm.IsTopSheet, vm.IsGroupTopSheet, connVM);
                            }
                        }
                    }
                    #endregion Else

                }
                #endregion  Data Call
                if (vm.FromSP)
                {
                    vat16s = JsonConvert.DeserializeObject<List<VAT_16>>(JsonConvert.SerializeObject(ReportResultDt));

                    //vat16s = ConvertDataTableToList<VAT_16>(ReportResultDt);
                }
                else
                {
                    #region Column Add

                    if (!ReportResult.Tables[0].Columns.Contains("Day"))
                    {
                        ReportResult.Tables[0].Columns.Add("Day");
                    }

                    #endregion
                    if (ReportResult.Tables.Count <= 0)
                    {
                        return null;
                    }
                    vat16s = new List<VAT_16>();
                    vat16 = new VAT_16();
                    DataView view = new DataView(ReportResult.Tables[0]);
                    DataTable distinctValues = view.ToTable(true, "ItemNo");
                    PurchaseDAL purchaseDal = new PurchaseDAL();
                    string stockNewMethod = commonDal.settings("Product", "AVGStockNewMethod", null, null, connVM);
                    foreach (DataRow DistinctItem in distinctValues.Rows)
                    {
                        #region ReportLoop
                        ProductVM vProductVM = new ProductVM();
                        string vitemno = DistinctItem["itemno"].ToString();
                        vProductVM = new ProductDAL().SelectAll(vitemno, null, null, null, null, null, connVM).FirstOrDefault();
                        IsRaw = vProductVM.ProductType.ToLower();
                        bool nonstock = false;
                        if (IsRaw.ToLower() == "service(nonstock)"
                            || IsRaw.ToLower() == "overhead"
                            || IsRaw.ToLower() == "noninventory"
                            )
                        {
                            nonstock = true;
                        }

                        #region


                        decimal vColumn1 = 0;
                        string vColumnDay = "";
                        DateTime vColumn2 = DateTime.MinValue;
                        decimal vColumn3 = 0;
                        decimal vColumn4 = 0;
                        string vColumn5 = "-";
                        DateTime vColumn6 = DateTime.MinValue;
                        string vColumn7 = string.Empty;
                        string vColumn8 = string.Empty;
                        string vColumn9 = string.Empty;
                        string vColumn10 = string.Empty;
                        decimal vColumn11 = 0;
                        decimal vColumn12 = 0;
                        decimal vColumn13 = 0;
                        decimal vColumn14 = 0;
                        decimal vColumn15 = 0;
                        decimal vColumn16 = 0;
                        decimal vColumn17 = 0;
                        decimal vColumn18 = 0;
                        string vColumn19 = string.Empty;

                        decimal vTempSerial = 0;
                        DateTime vTempStartDateTime = DateTime.MinValue;
                        string vTempVendorName = string.Empty;
                        string vTempVATRegistrationNo = string.Empty;
                        string vTempAddress1 = string.Empty;
                        string vTempTransID = string.Empty;
                        DateTime vTempInvoiceDateTime = DateTime.MinValue;
                        string vTempProductName = string.Empty;
                        string vTempProductCode = string.Empty;
                        string vTempBENumber = string.Empty;

                        string vTempremarks = string.Empty;
                        string vTemptransType = string.Empty;
                        string vUOM = string.Empty;

                        decimal OpeningQty = 0;
                        decimal OpeningAmnt = 0;
                        decimal PurchaseQty = 0;
                        decimal PurchaseAmnt = 0;
                        decimal SDAmnt = 0;
                        decimal VATAmnt = 0;

                        decimal IssueQty = 0;
                        decimal IssueAmnt = 0;
                        decimal CloseQty = 0;
                        decimal CloseAmnt = 0;

                        string HSCode = string.Empty;
                        string ProductName = string.Empty;
                        string ProductCode = string.Empty;
                        string UOM = string.Empty;



                        #endregion

                        int i = 1;
                        DataRow[] DetailRawsOthers = null;

                        #region Opening

                        DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("transType='Opening' and Itemno='" + vitemno + "'");

                        vat16 = new VAT_16();

                        foreach (DataRow row in DetailRawsOpening)
                        {
                            Quanity = Convert.ToDecimal(row["Quantity"]);
                            UnitCost = Convert.ToDecimal(row["UnitCost"]);
                            RunningTotal = Convert.ToDecimal(row["RunningTotal"]);
                            RunningValue = Convert.ToDecimal(row["RunningValue"]);

                            vColumnDay = Convert.ToString(row["Day"]);
                            vTempremarks = row["Remarks"].ToString().Trim();
                            vTemptransType = row["TransType"].ToString().Trim();
                            ProductName = row["ProductName"].ToString().Trim();
                            ProductCode = row["ProductCodeA"].ToString().Trim();
                            HSCode = row["HSCodeNo"].ToString().Trim();
                            UOM = row["UOM"].ToString().Trim();
                            vTempStartDateTime = Convert.ToDateTime(row["StartDateTime"].ToString().Trim());


                            #region if row 1 Opening
                            PurchaseQty = 0;
                            PurchaseAmnt = 0;
                            IssueQty = 0;
                            IssueAmnt = 0;
                            SDAmnt = 0;
                            VATAmnt = 0;
                            if (vm.IsTopSheet || vm.IsGroupTopSheet)
                            {
                                OpeningQty = Convert.ToDecimal(row["StartingQuantity"].ToString().Trim());
                                OpeningAmnt = Convert.ToDecimal(row["StartingAmount"].ToString().Trim());
                                IssueQty = Convert.ToDecimal(row["IssueQuantity"].ToString().Trim());
                                IssueAmnt = Convert.ToDecimal(row["IssueAmount"].ToString().Trim());
                                PurchaseQty = Convert.ToDecimal(row["PurchaseQuantity"].ToString().Trim());
                                PurchaseAmnt = Convert.ToDecimal(row["PurchaseAmount"].ToString().Trim());
                                SDAmnt = Convert.ToDecimal(row["PurchaseSD"].ToString().Trim());
                                VATAmnt = Convert.ToDecimal(row["VATRateAmount"].ToString().Trim());
                            }
                            else
                            {

                            }


                            OpeningQty = Quanity;
                            OpeningAmnt = UnitCost;
                            CloseQty = RunningTotal;
                            CloseAmnt = RunningValue;
                            vColumn2 = vTempStartDateTime;
                            vColumn3 = OpeningQty;
                            vColumn4 = OpeningAmnt;
                            vColumn5 = "-";
                            vColumn6 = vTempStartDateTime;
                            vColumn7 = "-";
                            vColumn8 = "-";
                            vColumn9 = "-";
                            vColumn10 = "-";
                            vColumn11 = PurchaseQty;
                            vColumn12 = PurchaseAmnt;
                            vColumn13 = SDAmnt;
                            vColumn14 = VATAmnt;
                            vColumn15 = IssueQty;
                            vColumn16 = IssueAmnt;
                            vColumn17 = CloseQty;
                            vColumn18 = CloseAmnt;
                            vColumn19 = vTempremarks;


                            #endregion  if row 1 Opening

                            #region AssignValue to List

                            vat16.Day = vColumnDay;
                            vat16.Column1 = vColumn1; //    i.ToString();      // Serial No   
                            vat16.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                            vat16.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                            vat16.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                            vat16.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                            vat16.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                            vat16.Column6String = ""; //    item["UnitCost"].ToString();      // Production Price
                            vat16.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                            vat16.Column8 = vColumn8;   //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                            vat16.Column9 = vColumn9;   //    item["Address1"].ToString();      // Customer Address
                            vat16.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                            vat16.Column11 = vColumn11; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                            vat16.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                            vat16.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                            vat16.Column14 = vColumn14; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                            vat16.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                            vat16.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount
                            vat16.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                            vat16.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                            vat16.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                            vat16.ProductName = ProductName;
                            vat16.ProductCode = ProductCode;
                            vat16.FormUom = UOM;

                            if (Permanent6_1)
                            {
                                vat16.Column4 = vColumn4;
                                vat16.Column3 = vColumn3;
                                vat16.Column17 = vColumn17;
                                vat16.Column18 = vColumn18;
                            }

                            if (nonstock == true || vTempremarks.Trim() == "ServiceNS" || vTempremarks.Trim() == "ServiceNSImport")
                            {
                                vat16.Column4 = 0;
                                vat16.Column3 = 0;
                                vat16.Column17 = 0;
                                vat16.Column18 = 0;
                                vat16.Column15 = 0;
                                vat16.Column16 = 0;
                            }
                            vat16s.Add(vat16);
                            i = i + 1;

                            #endregion AssignValue to List

                        }
                        #endregion Opening

                        DetailRawsOthers = ReportResult.Tables[0].Select("transType<>'Opening' and Itemno='" + vitemno + "' ", "ItemNo, StartDateTime, SerialNo");

                        if (DetailRawsOthers != null && DetailRawsOthers.Any())
                        {
                            #region Not  Opening
                            string strSort = "ItemNo, StartDateTime, SerialNo";
                            DataView dtview = new DataView(DetailRawsOthers.CopyToDataTable());
                            dtview.Sort = strSort;
                            DataTable dtsorted = dtview.ToTable();

                            foreach (DataRow item in dtsorted.Rows)
                            {
                                Quanity = Convert.ToDecimal(item["Quantity"]);
                                UnitCost = Convert.ToDecimal(item["UnitCost"]);
                                RunningTotal = Convert.ToDecimal(item["RunningTotal"]);
                                RunningValue = Convert.ToDecimal(item["RunningValue"]);
                                vat16 = new VAT_16();

                                #region Get from Datatable

                                PurchaseQty = 0;
                                PurchaseAmnt = 0;
                                IssueQty = 0;
                                IssueAmnt = 0;
                                vColumn1 = i;
                                vColumnDay = Convert.ToString(item["Day"]);
                                vTempStartDateTime = Convert.ToDateTime(item["StartDateTime"].ToString()); // Date

                                vTempVendorName = item["VendorName"].ToString(); // Customer Name

                                vTempVATRegistrationNo = item["VATRegistrationNo"].ToString(); // Customer VAT Reg No
                                vTempAddress1 = item["Address1"].ToString(); // Customer Address
                                vTempTransID = item["TransID"].ToString(); // Sale Invoice No
                                vTempInvoiceDateTime = Convert.ToDateTime(item["InvoiceDateTime"].ToString()); // Sale Invoice Date and Time
                                vTempBENumber = item["BENumber"].ToString(); // Sale Invoice Date and Time
                                vTempProductName = item["ProductName"].ToString(); // Sale Product Name
                                vTempProductCode = item["ProductCodeA"].ToString(); // Sale Product Name
                                vTempSDAmount = Convert.ToDecimal(item["SD"].ToString()); // SD Amount
                                vTempVATAmount = Convert.ToDecimal(item["VATRate"].ToString()); // VAT Amount
                                vTempremarks = item["remarks"].ToString(); // Remarks
                                vTemptransType = item["TransType"].ToString().Trim();
                                vUOM = item["UOM"].ToString();
                                #endregion Get from Datatable

                                if (vTemptransType == "Issue")
                                {
                                    #region if row 1 Opening

                                    OpeningQty = CloseQty;
                                    OpeningAmnt = CloseAmnt;
                                    PurchaseQty = 0;
                                    PurchaseAmnt = 0;
                                    IssueQty = Quanity;
                                    IssueAmnt = UnitCost;
                                    vColumn2 = vTempStartDateTime;
                                    vColumn3 = CloseQty;
                                    vColumn4 = CloseAmnt;
                                    vColumn5 = "";
                                    vColumn6 = vTempStartDateTime;
                                    vColumn7 = vTempVendorName;
                                    vColumn8 = vTempAddress1;
                                    vColumn9 = vTempVATRegistrationNo;
                                    vColumn10 = vTempProductName;
                                    vColumn11 = PurchaseQty;
                                    vColumn12 = PurchaseAmnt;
                                    vColumn13 = vTempSDAmount;
                                    vColumn14 = vTempVATAmount;
                                    vColumn15 = IssueQty;
                                    vColumn16 = IssueAmnt;
                                    vColumn17 = RunningTotal;
                                    vColumn18 = RunningValue;
                                    vColumn19 = vTempremarks;
                                    if (vColumn17 == 0)
                                    {
                                        vColumn18 = 0;
                                    }
                                    else
                                    {
                                        if (stockNewMethod == "Y")
                                        {
                                            vColumn18 = RunningValue;// vClosingQuantity* AvgPrice;
                                        }
                                        else
                                        {
                                            vColumn18 = RunningValue;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -Convert.ToDecimal(IssueAmnt));
                                        }
                                    }


                                    #endregion
                                    #region AssignValue to List

                                    vat16.Day = vColumnDay;
                                    vat16.Column1 = vColumn1; //   
                                    vat16.Column2 = vColumn2; //   
                                    vat16.Column3 = vColumn3; //   
                                    vat16.Column4 = vColumn4; //   
                                    vat16.Column5 = vColumn5; //   
                                    vat16.Column6 = vColumn6; //   
                                    vat16.Column6String = ""; //   
                                    vat16.Column7 = vColumn7; //   
                                    vat16.Column8 = vColumn8;//    
                                    vat16.Column9 = vColumn9; //   
                                    vat16.Column10 = vColumn10; // 
                                    vat16.Column11 = vColumn11;//  
                                    vat16.Column12 = vColumn12; // 
                                    vat16.Column13 = vColumn13; // 
                                    vat16.Column14 = vColumn14;//  
                                    vat16.Column15 = vColumn15; // 
                                    vat16.Column16 = vColumn16; // 
                                    vat16.Column17 = vColumn17; // 
                                    vat16.Column18 = vColumn18; // 
                                    vat16.Column19 = vColumn19; // 
                                    vat16.ProductName = vTempProductName;
                                    vat16.ProductCode = vTempProductCode;
                                    vat16.FormUom = vUOM;


                                    if (nonstock == true || vTempremarks.Trim() == "ServiceNS" || vTempremarks.Trim() == "ServiceNSImport")
                                    {
                                        vat16.Column4 = 0;
                                        vat16.Column3 = 0;
                                        vat16.Column17 = 0;
                                        vat16.Column18 = 0;
                                        vat16.Column15 = 0;
                                        vat16.Column16 = 0;
                                    }
                                    vat16s.Add(vat16);
                                    i = i + 1;
                                    CloseQty = vColumn17;
                                    CloseAmnt = vColumn18;
                                    #endregion AssignValue to List



                                }
                                else if (vTemptransType == "Purchase" || vTemptransType == "Toll Receive")
                                {

                                    #region if row 1 Opening


                                    OpeningQty = CloseQty;
                                    OpeningAmnt = CloseAmnt;

                                    PurchaseQty = Quanity;
                                    PurchaseAmnt = UnitCost;

                                    IssueQty = 0;
                                    IssueAmnt = 0;
                                    vColumn2 = vTempStartDateTime;
                                    vColumn3 = OpeningQty;
                                    vColumn4 = OpeningAmnt;
                                    vColumn5 = vTempBENumber;
                                    vColumn6 = vTempInvoiceDateTime;
                                    vColumn7 = vTempVendorName;
                                    vColumn8 = vTempAddress1;
                                    vColumn9 = vTempVATRegistrationNo;
                                    vColumn10 = vTempProductName;
                                    vColumn11 = PurchaseQty;
                                    vColumn12 = PurchaseAmnt;
                                    vColumn13 = vTempSDAmount;
                                    vColumn14 = vTempVATAmount;
                                    vColumn15 = IssueQty;
                                    vColumn16 = IssueAmnt;
                                    vColumn17 = RunningTotal;
                                    vColumn18 = RunningValue;
                                    vColumn19 = vTempremarks;

                                    if (vColumn17 == 0)
                                    {
                                        vColumn18 = 0;
                                    }

                                    #endregion

                                    #region AssignValue to List


                                    vat16.Day = vColumnDay;
                                    vat16.Column1 = vColumn1; //   
                                    vat16.Column2 = vColumn2; //   
                                    vat16.Column3 = vColumn3; //   
                                    vat16.Column4 = vColumn4; //   
                                    vat16.Column5 = vColumn5; //   
                                    vat16.Column6 = vColumn6; //   
                                    vat16.Column6String = Convert.ToDateTime(vColumn6).ToString("dd-MMM-yyyy"); //    item["UnitCost"].ToString();      // Production Price
                                    vat16.Column7 = vColumn7; //   
                                    vat16.Column8 = vColumn8;   // 
                                    vat16.Column9 = vColumn9;   // 
                                    vat16.Column10 = vColumn10; // 
                                    vat16.Column11 = vColumn11; // 
                                    vat16.Column12 = vColumn12; // 
                                    vat16.Column13 = vColumn13; // 
                                    vat16.Column14 = vColumn14; // 
                                    vat16.Column15 = vColumn15; // 
                                    vat16.Column16 = vColumn16; // 
                                    vat16.Column17 = vColumn17; // 
                                    vat16.Column18 = vColumn18; // 
                                    vat16.Column19 = vColumn19; // 
                                    vat16.ProductName = vTempProductName;
                                    vat16.ProductCode = vTempProductCode;
                                    vat16.FormUom = vUOM;


                                    if (nonstock == true || vTempremarks.Trim() == "ServiceNS" || vTempremarks.Trim() == "ServiceNSImport")
                                    {
                                        vat16.Column4 = 0;
                                        vat16.Column3 = 0;
                                        vat16.Column17 = 0;
                                        vat16.Column18 = 0;
                                        vat16.Column15 = 0;
                                        vat16.Column16 = 0;
                                    }

                                    vat16s.Add(vat16);
                                    i = i + 1;
                                    CloseQty = vColumn17;
                                    CloseAmnt = vColumn18;
                                    #endregion AssignValue to List


                                }
                            }
                            #endregion Not  Opening
                        }

                        #endregion ReportLoop
                    }
                }
                #region Report Preview
                //if (true)
                //{
                //    DataTable dtx = new DataTable();
                //    dtx = ToDataTable(vat16s);
                //    OrdinaryVATDesktop.SaveExcel(dtx, "vat16s", "vat16");
                //}

                if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                {
                    //objrpt.Load(Program.ReportAppPath + @"\RptVAT6_1.rpt");
                    if (vm.IsTopSheet == true || vm.IsGroupTopSheet == true)
                    {
                        objrpt = new RptVAT6_1_TopSheet();
                    }
                    else
                    {
                        if (vm.InEnglish == "Y")
                        {
                            objrpt = new RptVAT6_1_English();

                        }
                        else
                        {
                            if (Permanent6_1)
                            {
                                objrpt = new RptVAT6_1_Permanet();

                            }
                            else
                            {
                                objrpt = new RptVAT6_1();

                            }

                            //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_1.rpt");


                        }
                    }

                }

                if (vat16s.Count <= 0)
                {
                    return null;
                }
                objrpt.SetDataSource(vat16s);

                if (vm.PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                ProductDAL _pDal = new ProductDAL();
                //string ProductDescription = _pDal.ProductDTByItemNo(vm.ItemNo).Rows[0]["ProductDescription"].ToString();

                CommonDAL _cDal = new CommonDAL();


                string Quantity6_1 = _cDal.settingsDesktop("DecimalPlace", "Quantity6_1", settingsDt, connVM);
                string Amount6_1 = _cDal.settingsDesktop("DecimalPlace", "Amount6_1", settingsDt, connVM);


                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;

                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Month", Convert.ToDateTime(vm.StartDate).ToString("MMMM-yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsMonthly", vm.IsMonthly ? "Y" : "N");

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", string.IsNullOrEmpty(vm.FontSize.ToString()) ? OrdinaryVATDesktop.FontSize : vm.FontSize.ToString());
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FormNumeric", vm.FormNumeric==null?"2");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOMConversion", UOMConversion);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOM", vm.UOM);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FromDate", Convert.ToDateTime(FromDate).ToString("dd/MM/yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ToDate", Convert.ToDateTime(ToDate).ToString("dd/MM/yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_1", vm.DecimalPlaceValue == null ? Amount6_1 : vm.DecimalPlaceValue);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_1", vm.DecimalPlace == null ? Quantity6_1 : vm.DecimalPlace);


                #endregion preview

                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static List<T> ConvertDataTableToList<T>(DataTable dataTable) where T : new()
        {
            return dataTable.AsEnumerable()
                            .Select(row => CreateItemFromRow<T>(row))
                            .ToList();
        }
        private static T CreateItemFromRow<T>(DataRow row) where T : new()
        {
            T obj = new T();

            foreach (DataColumn column in row.Table.Columns)
            {
                PropertyInfo property = typeof(T).GetProperty(column.ColumnName);
                if (property != null && row[column] != DBNull.Value)
                {
                    property.SetValue(obj, row[column]);
                }
            }

            return obj;
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            var properties = typeof(T).GetProperties();

            // Create the columns in the DataTable
            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, prop.PropertyType);
            }

            // Use LINQ to populate the DataTable
            items.ToList().ForEach(item =>
            {
                var values = properties.Select(prop => prop.GetValue(item));
                dataTable.Rows.Add(values.ToArray());
            });

            return dataTable;
        }
        public ReportDocument VAT6_1_WithConn09032021(VAT6_1ParamVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Comments


            #endregion

            #region Variables and Objects

            string IsRaw = "";
            ReportDocument objrpt = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();

            #endregion

            try
            {
                #region UOM Conversion

                string UOMConversion = "";

                UOMConversion = Convert.ToString(vm.UOMConversion > 0 ? vm.UOMConversion : 1);

                #endregion

                #region Settings Load


                DataTable settingsDt = new DataTable();
                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                #region Data Call


                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = _vatRegistersDAL.VAT6_1_WithConn(vm, null, null, connVM);

                #region Debug


                ////if (false)
                ////{
                ////string names = "";
                ////if (ReportResult != null && ReportResult.Tables.Count > 0)
                ////{
                ////    foreach (DataColumn column in ReportResult.Tables[0].Columns)
                ////    {
                ////        names += column.ColumnName + ",";
                ////    }

                ////}

                ////}

                #endregion

                if (ReportResult != null && ReportResult.Tables.Count > 0)
                {
                    if (ReportResult.Tables[0] != null && ReportResult.Tables[0].Rows.Count > 0)
                    {
                        if (ReportResult.Tables[0].Columns.Contains("CreateDateTime"))
                        {

                            ReportResult.Tables[0].Columns["CreateDateTime"].ColumnName = "CreatedDateTime";
                        }
                    }
                }

                if (vm.IsMonthly == true)
                {
                    if (ReportResult != null && ReportResult.Tables.Count > 0)
                    {
                        if (ReportResult.Tables[0] != null && ReportResult.Tables[0].Rows.Count > 0)
                        {
                            ReportResult = reportDsdal.VAT6_1_Monthly(ReportResult, vm.StartDate, false, false, connVM);
                        }

                    }
                }
                else if (vm.IsTopSheet == true || vm.IsGroupTopSheet == true)
                {
                    if (ReportResult != null && ReportResult.Tables.Count > 0)
                    {
                        if (ReportResult.Tables[0] != null && ReportResult.Tables[0].Rows.Count > 0)
                        {
                            ReportResult = reportDsdal.VAT6_1_Monthly(ReportResult, vm.StartDate, vm.IsTopSheet, vm.IsGroupTopSheet, connVM);
                        }

                    }
                }
                //else if (vm.IsGroupTopSheet == true)
                //{
                //    if (ReportResult != null && ReportResult.Tables.Count > 0)
                //    {
                //        if (ReportResult.Tables[0] != null && ReportResult.Tables[0].Rows.Count > 0)
                //        {
                //            ReportResult = reportDsdal.VAT6_1_Monthly(ReportResult, vm.StartDate, vm.IsTopSheet, vm.IsGroupTopSheet);
                //        }

                //    }
                //}
                #endregion

                #region Column Add

                if (!ReportResult.Tables[0].Columns.Contains("Day"))
                {
                    ReportResult.Tables[0].Columns.Add("Day");
                }

                #endregion

                #region Debug Code



                #endregion

                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                List<VAT_16> vat16s = new List<VAT_16>();
                VAT_16 vat16 = new VAT_16();
                DataView view = new DataView(ReportResult.Tables[0]);
                DataTable distinctValues = view.ToTable(true, "ItemNo");
                decimal AvgPrice = 0;
                CommonDAL commonDal = new CommonDAL();
                PurchaseDAL purchaseDal = new PurchaseDAL();
                string stockNewMethod = commonDal.settings("Product", "AVGStockNewMethod", null, null, connVM);

                foreach (DataRow DistinctItem in distinctValues.Rows)
                {

                    ProductVM vProductVM = new ProductVM();
                    string vitemno = DistinctItem["itemno"].ToString();
                    vProductVM = new ProductDAL().SelectAll(vitemno, null, null, null, null, null, connVM).FirstOrDefault();
                    //if (vProductVM.ProductType.ToLower() == "service(nonstock)")
                    //{
                    IsRaw = vProductVM.ProductType.ToLower();
                    //}
                    bool nonstock = false;
                    if (IsRaw.ToLower() == "service(nonstock)"
                        || IsRaw.ToLower() == "overhead"
                        || IsRaw.ToLower() == "noninventory"
                        )
                    {
                        nonstock = true;
                    }
                    #region Statement

                    #region


                    decimal vColumn1 = 0;
                    string vColumnDay = "";
                    DateTime vColumn2 = DateTime.MinValue;
                    decimal vColumn3 = 0;
                    decimal vColumn4 = 0;
                    string vColumn5 = "-";
                    DateTime vColumn6 = DateTime.MinValue;
                    string vColumn7 = string.Empty;
                    string vColumn8 = string.Empty;
                    string vColumn9 = string.Empty;
                    string vColumn10 = string.Empty;
                    decimal vColumn11 = 0;
                    decimal vColumn12 = 0;
                    decimal vColumn13 = 0;
                    decimal vColumn14 = 0;
                    decimal vColumn15 = 0;
                    decimal vColumn16 = 0;
                    decimal vColumn17 = 0;
                    decimal vColumn18 = 0;
                    string vColumn19 = string.Empty;

                    decimal vTempSerial = 0;
                    DateTime vTempStartDateTime = DateTime.MinValue;
                    decimal vTempStartingQuantity = 0;
                    decimal vTempStartingAmount = 0;
                    decimal vTempQuantity = 0;
                    decimal vTempSubtotal = 0;
                    string vTempVendorName = string.Empty;
                    string vTempVATRegistrationNo = string.Empty;
                    string vTempAddress1 = string.Empty;
                    string vTempTransID = string.Empty;
                    DateTime vTempInvoiceDateTime = DateTime.MinValue;
                    string vTempProductName = string.Empty;
                    string vTempProductCode = string.Empty;
                    string vTempBENumber = string.Empty;

                    decimal vTempSDAmount = 0;
                    decimal vTempVATAmount = 0;
                    string vTempremarks = string.Empty;
                    string vTemptransType = string.Empty;
                    string vUOM = string.Empty;

                    decimal vClosingQuantity = 0;
                    decimal vClosingAmount = 0;
                    //decimal vClosingAvgRate = 0;

                    decimal OpeningQty = 0;
                    decimal OpeningAmnt = 0;
                    decimal PurchaseQty = 0;
                    decimal PurchaseAmnt = 0;
                    decimal SDAmnt = 0;
                    decimal VATAmnt = 0;

                    decimal IssueQty = 0;
                    decimal IssueAmnt = 0;
                    decimal CloseQty = 0;
                    decimal CloseAmnt = 0;

                    decimal OpQty = 0;
                    decimal OpAmnt = 0;
                    //decimal avgRate = 0;
                    string HSCode = string.Empty;
                    string ProductName = string.Empty;
                    string ProductCode = string.Empty;
                    string UOM = string.Empty;



                    #endregion

                    int i = 1;
                    DataRow[] DetailRawsOthers = null;

                    #region Opening
                    if (vm.IsTopSheet == false && vm.IsGroupTopSheet == false)
                    {
                        DetailRawsOthers = ReportResult.Tables[0].Select("transType<>'Opening' and Itemno='" + vitemno + "'");
                        if (DetailRawsOthers == null || !DetailRawsOthers.Any())
                        {
                            continue;
                        }
                    }
                    DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("transType='Opening' and Itemno='" + vitemno + "'");
                    vat16 = new VAT_16();

                    foreach (DataRow row in DetailRawsOpening)
                    {


                        vColumnDay = Convert.ToString(row["Day"]);
                        vTempremarks = row["Remarks"].ToString().Trim();
                        vTemptransType = row["TransType"].ToString().Trim();
                        ProductName = row["ProductName"].ToString().Trim();
                        ProductCode = row["ProductCodeA"].ToString().Trim();
                        HSCode = row["HSCodeNo"].ToString().Trim();
                        UOM = row["UOM"].ToString().Trim();
                        vTempStartDateTime = Convert.ToDateTime(row["StartDateTime"].ToString().Trim());

                        if (stockNewMethod == "Y")
                        {
                            DataSet dtavgPrice = purchaseDal.GetAvgPrice(vitemno, vTempStartDateTime.ToString(), null, null, connVM);

                            if (dtavgPrice != null && dtavgPrice.Tables[0].Rows.Count > 0)
                            {
                                AvgPrice = Convert.ToDecimal(
                                    string.IsNullOrEmpty(dtavgPrice.Tables[0].Rows[0]["AvgPrice"].ToString())
                                        ? 0
                                        : dtavgPrice.Tables[0].Rows[0]["AvgPrice"]);
                            }
                        }


                        #region if row 1 Opening
                        PurchaseQty = 0;
                        PurchaseAmnt = 0;
                        IssueQty = 0;
                        IssueAmnt = 0;
                        SDAmnt = 0;
                        VATAmnt = 0;
                        if (vm.IsTopSheet || vm.IsGroupTopSheet)
                        {
                            OpQty = Convert.ToDecimal(row["StartingQuantity"].ToString().Trim());
                            if (stockNewMethod == "Y")
                            {
                                OpAmnt = OpQty * AvgPrice;
                            }
                            else
                            {
                                OpAmnt = Convert.ToDecimal(row["StartingAmount"].ToString().Trim());
                            }
                            IssueQty = Convert.ToDecimal(row["IssueQuantity"].ToString().Trim());
                            IssueAmnt = Convert.ToDecimal(row["IssueAmount"].ToString().Trim());

                            PurchaseQty = Convert.ToDecimal(row["PurchaseQuantity"].ToString().Trim());
                            PurchaseAmnt = Convert.ToDecimal(row["PurchaseAmount"].ToString().Trim());
                            SDAmnt = Convert.ToDecimal(row["PurchaseSD"].ToString().Trim());
                            VATAmnt = Convert.ToDecimal(row["VATRateAmount"].ToString().Trim());
                        }
                        else
                        {
                            OpQty = Convert.ToDecimal(row["Quantity"].ToString().Trim());
                            if (stockNewMethod == "Y")
                            {
                                OpAmnt = OpQty * AvgPrice;

                            }
                            else
                            {
                                OpAmnt = Convert.ToDecimal(row["UnitCost"].ToString().Trim());
                            }
                        }
                        OpeningQty = OpQty;
                        OpeningAmnt = OpAmnt;
                        OpAmnt = 0;
                        OpQty = 0;



                        CloseQty =
                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                             Convert.ToDecimal(IssueQty));
                        if (stockNewMethod == "Y")
                        {
                            CloseAmnt = CloseQty * AvgPrice;
                        }
                        else
                        {
                            CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                         Convert.ToDecimal(IssueAmnt));
                        }


                        vColumn2 = vTempStartDateTime;
                        vColumn3 = OpeningQty;
                        vColumn4 = OpeningAmnt;
                        vColumn5 = "-";
                        vColumn6 = vTempStartDateTime;
                        vColumn7 = "-";
                        vColumn8 = "-";
                        vColumn9 = "-";
                        vColumn10 = "-";
                        vColumn11 = PurchaseQty;
                        vColumn12 = PurchaseAmnt;
                        vColumn13 = SDAmnt;
                        vColumn14 = VATAmnt;
                        vColumn15 = IssueQty;
                        vColumn16 = IssueAmnt;
                        vColumn17 = CloseQty;
                        vColumn18 = CloseAmnt;
                        vColumn19 = vTempremarks;

                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                                            Convert.ToDecimal(IssueQty));
                        if (vClosingQuantity == 0)
                        {
                            vClosingAmount = 0;


                        }
                        else
                        {
                            if (stockNewMethod == "Y")
                            {
                                vClosingAmount = vClosingQuantity * AvgPrice;
                            }
                            else
                            {
                                vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                                  Convert.ToDecimal(IssueAmnt));
                            }
                            //vClosingAvgRate = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));


                        }

                        #endregion

                        #region AssignValue to List
                        if (nonstock == true)
                        {

                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        vat16.Day = vColumnDay;
                        vat16.Column1 = vColumn1; //    i.ToString();      // Serial No   
                        vat16.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                        vat16.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                        vat16.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                        vat16.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                        vat16.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                        vat16.Column6String = ""; //    item["UnitCost"].ToString();      // Production Price
                        vat16.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                        vat16.Column8 = vColumn8;   //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                        vat16.Column9 = vColumn9;   //    item["Address1"].ToString();      // Customer Address
                        vat16.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                        vat16.Column11 = vColumn11; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                        vat16.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                        vat16.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                        vat16.Column14 = vColumn14; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                        vat16.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                        vat16.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount
                        vat16.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                        vat16.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                        vat16.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                        vat16.ProductName = ProductName;
                        vat16.ProductCode = ProductCode;
                        vat16.FormUom = UOM;

                        vat16s.Add(vat16);
                        i = i + 1;

                        #endregion AssignValue to List


                    }
                    #endregion Opening
                    if (vm.IsTopSheet == false && vm.IsGroupTopSheet == false)
                    {
                        #region Not  Opening



                        string strSort = "CreatedDateTime ASC, SerialNo ASC";

                        DataView dtview = new DataView(DetailRawsOthers.CopyToDataTable());
                        dtview.Sort = strSort;
                        DataTable dtsorted = dtview.ToTable();

                        foreach (DataRow item in dtsorted.Rows)
                        {
                            vat16 = new VAT_16();

                            #region Get from Datatable

                            OpeningQty = 0;
                            OpeningAmnt = 0;
                            PurchaseQty = 0;
                            PurchaseAmnt = 0;
                            IssueQty = 0;
                            IssueAmnt = 0;
                            CloseQty = 0;
                            CloseAmnt = 0;

                            vColumn1 = i;
                            vColumnDay = Convert.ToString(item["Day"]);
                            vTempStartDateTime = Convert.ToDateTime(item["StartDateTime"].ToString()); // Date
                            vTempQuantity = Convert.ToDecimal(item["Quantity"].ToString()); // Production Quantity
                            //vTempSubtotal = vTempQuantity * AvgPrice;
                            vTempSubtotal = Convert.ToDecimal(item["UnitCost"].ToString()); // Production Price
                            vTempVendorName = item["VendorName"].ToString(); // Customer Name
                            vTempVATRegistrationNo = item["VATRegistrationNo"].ToString(); // Customer VAT Reg No
                            vTempAddress1 = item["Address1"].ToString(); // Customer Address
                            vTempTransID = item["TransID"].ToString(); // Sale Invoice No
                            vTempInvoiceDateTime = Convert.ToDateTime(item["InvoiceDateTime"].ToString()); // Sale Invoice Date and Time
                            vTempBENumber = item["BENumber"].ToString(); // Sale Invoice Date and Time
                            vTempProductName = item["ProductName"].ToString(); // Sale Product Name
                            vTempProductCode = item["ProductCodeA"].ToString(); // Sale Product Name
                            vTempSDAmount = Convert.ToDecimal(item["SD"].ToString()); // SD Amount
                            vTempVATAmount = Convert.ToDecimal(item["VATRate"].ToString()); // VAT Amount
                            vTempremarks = item["remarks"].ToString(); // Remarks
                            vTemptransType = item["TransType"].ToString().Trim();
                            vUOM = item["UOM"].ToString();
                            #endregion Get from Datatable

                            if (vTemptransType == "Issue")
                            {
                                #region if row 1 Opening
                                if (vTempremarks.Trim() == "ServiceNS"
                                   || vTempremarks.Trim() == "ServiceNSImport"
                                   )
                                {
                                    OpeningQty = 0;
                                    OpeningAmnt = 0;
                                }
                                else
                                {
                                    OpeningQty = OpQty + vClosingQuantity;
                                    if (stockNewMethod == "Y")
                                    {
                                        OpeningAmnt = OpeningQty * AvgPrice;
                                    }
                                    else
                                    {
                                        OpeningAmnt = OpAmnt + vClosingAmount;
                                    }

                                }



                                OpAmnt = 0;
                                OpQty = 0;

                                PurchaseQty = 0;
                                PurchaseAmnt = 0;
                                if (vTempremarks.Trim() == "ServiceNS"
                                  || vTempremarks.Trim() == "ServiceNSImport"
                                  )
                                {
                                    IssueQty = 0;
                                    IssueAmnt = 0;
                                }
                                else
                                {
                                    IssueQty = vTempQuantity;
                                    if (stockNewMethod == "Y")
                                    {
                                        IssueAmnt = IssueQty * AvgPrice;
                                    }
                                    else
                                    {
                                        IssueAmnt = vTempSubtotal;// vTempQuantity* vClosingAvgRate;
                                    }
                                }

                                //if (IssueQty == 0)
                                //{
                                //    AvgPrice = 0;
                                //}
                                //else
                                //{
                                //    AvgPrice = IssueAmnt / IssueQty;
                                //}
                                CloseQty =
                                    (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                                     Convert.ToDecimal(IssueQty));
                                if (stockNewMethod == "Y")
                                {
                                    CloseAmnt = CloseQty * AvgPrice;
                                }
                                else
                                {
                                    CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                                 Convert.ToDecimal(IssueAmnt));
                                }

                                vColumn2 = vTempStartDateTime;

                                vColumn3 = OpeningQty;
                                vColumn4 = OpeningAmnt;

                                vColumn5 = "";
                                vColumn6 = vTempStartDateTime;

                                vColumn7 = vTempVendorName;
                                vColumn8 = vTempAddress1;
                                vColumn9 = vTempVATRegistrationNo;
                                vColumn10 = vTempProductName;
                                vColumn11 = PurchaseQty;
                                vColumn12 = PurchaseAmnt;
                                vColumn13 = vTempSDAmount;
                                vColumn14 = vTempVATAmount;
                                vColumn15 = IssueQty;
                                vColumn16 = IssueAmnt;

                                vColumn17 = CloseQty;
                                vColumn18 = CloseAmnt;

                                vColumn19 = vTempremarks;
                                vClosingQuantity = CloseQty;// (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -Convert.ToDecimal(IssueQty));
                                if (vTempremarks.Trim() == "ServiceNS"
                                  || vTempremarks.Trim() == "ServiceNSImport"
                                  )
                                {
                                    vClosingQuantity = 0;
                                    vClosingAmount = 0;
                                }
                                else if (vClosingQuantity == 0)
                                {
                                    vClosingAmount = 0;
                                }
                                else
                                {
                                    if (stockNewMethod == "Y")
                                    {
                                        vClosingAmount = CloseAmnt;// vClosingQuantity* AvgPrice;
                                    }
                                    else
                                    {

                                        vClosingAmount = CloseAmnt;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -Convert.ToDecimal(IssueAmnt));
                                    }

                                }
                                if (vTempremarks.Trim() == "ServiceNS"
                                  || vTempremarks.Trim() == "ServiceNSImport"
                                  )
                                {
                                    vClosingQuantity = 0;
                                    vClosingAmount = 0;

                                }

                                #endregion
                                #region AssignValue to List
                                if (nonstock == true)
                                {
                                    vColumn3 = 0;
                                    vColumn4 = 0;
                                    vColumn17 = 0;
                                    vColumn18 = 0;
                                }

                                vat16.Day = vColumnDay;
                                vat16.Column1 = vColumn1; //   
                                vat16.Column2 = vColumn2; //   
                                vat16.Column3 = vColumn3; //   
                                vat16.Column4 = vColumn4; //   
                                vat16.Column5 = vColumn5; //   
                                vat16.Column6 = vColumn6; //   
                                vat16.Column6String = ""; //   
                                vat16.Column7 = vColumn7; //   
                                vat16.Column8 = vColumn8;//    
                                vat16.Column9 = vColumn9; //   
                                vat16.Column10 = vColumn10; // 
                                vat16.Column11 = vColumn11;//  
                                vat16.Column12 = vColumn12; // 
                                vat16.Column13 = vColumn13; // 
                                vat16.Column14 = vColumn14;//  
                                vat16.Column15 = vColumn15; // 
                                vat16.Column16 = vColumn16; // 
                                vat16.Column17 = vColumn17; // 
                                vat16.Column18 = vColumn18; // 
                                vat16.Column19 = vColumn19; // 
                                vat16.ProductName = vTempProductName;
                                vat16.ProductCode = vTempProductCode;
                                vat16.FormUom = vUOM;

                                vat16s.Add(vat16);
                                i = i + 1;

                                #endregion AssignValue to List



                            }
                            else if (vTemptransType == "Purchase" || vTemptransType == "Toll Receive")
                            {
                                if (stockNewMethod == "Y")
                                {
                                    DataSet dtavgPrice = purchaseDal.GetAvgPrice(vitemno, vTempStartDateTime.ToString(), null, null, connVM);

                                    if (dtavgPrice != null && dtavgPrice.Tables[0].Rows.Count > 0)
                                    {
                                        AvgPrice = Convert.ToDecimal(
                                            string.IsNullOrEmpty(dtavgPrice.Tables[0].Rows[0]["AvgPrice"].ToString())
                                                ? 0
                                                : dtavgPrice.Tables[0].Rows[0]["AvgPrice"]);
                                    }
                                }
                                #region if row 1 Opening

                                if (vTempremarks.Trim() == "ServiceNS"
                                  || vTempremarks.Trim() == "ServiceNSImport"
                                  )
                                {
                                    OpeningQty = 0;
                                    OpeningAmnt = 0;
                                }
                                else
                                {
                                    OpeningQty = OpQty + vClosingQuantity;
                                    OpeningAmnt = OpAmnt + vClosingAmount;

                                    //if (stockNewMethod == "Y")
                                    //{
                                    //    OpeningAmnt = OpeningQty * AvgPrice;
                                    //}
                                    //else
                                    //{
                                    //    OpeningAmnt = OpAmnt + vClosingAmount;
                                    //}
                                }
                                //OpeningQty = OpQty + vClosingQuantity;
                                //if (stockNewMethod == "Y")
                                //{
                                //    OpeningAmnt = OpeningQty * AvgPrice;
                                //}
                                //else
                                //{
                                //    OpeningAmnt = OpAmnt + vClosingAmount;
                                //}
                                OpAmnt = 0;
                                OpQty = 0;


                                PurchaseQty = vTempQuantity;
                                PurchaseAmnt = vTempSubtotal;

                                IssueQty = 0;
                                IssueAmnt = 0;
                                //if (PurchaseQty == 0)
                                //{
                                //    AvgPrice = 0;
                                //}
                                //else
                                //{
                                //    AvgPrice = PurchaseAmnt / PurchaseQty;
                                //}


                                if (vTempremarks.Trim() == "ServiceNS"
                                  || vTempremarks.Trim() == "ServiceNSImport"
                                  )
                                {
                                    CloseQty = 0;
                                    CloseAmnt = 0;
                                }
                                else
                                {
                                    CloseQty =
                                   (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                                    Convert.ToDecimal(IssueQty));
                                    if (stockNewMethod == "Y")
                                    {

                                        CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                                    Convert.ToDecimal(IssueAmnt));

                                        //CloseAmnt = CloseQty * AvgPrice;
                                        AvgPrice = CloseAmnt / CloseQty;
                                    }
                                    else
                                    {
                                        CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                                     Convert.ToDecimal(IssueAmnt));
                                    }
                                }


                                vColumn2 = vTempStartDateTime;
                                vColumn3 = OpeningQty;
                                vColumn4 = OpeningAmnt;
                                vColumn5 = vTempBENumber;
                                vColumn6 = vTempInvoiceDateTime;
                                vColumn7 = vTempVendorName;
                                vColumn8 = vTempAddress1;
                                vColumn9 = vTempVATRegistrationNo;
                                vColumn10 = vTempProductName;
                                vColumn11 = PurchaseQty;
                                vColumn12 = PurchaseAmnt;
                                vColumn13 = vTempSDAmount;
                                vColumn14 = vTempVATAmount;
                                vColumn15 = IssueQty;
                                vColumn16 = IssueAmnt;
                                vColumn17 = CloseQty;


                                vColumn18 = CloseAmnt;
                                vColumn19 = vTempremarks;

                                vClosingQuantity = CloseQty;// (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -Convert.ToDecimal(IssueQty));
                                if (vClosingQuantity == 0)
                                {
                                    vClosingAmount = 0;


                                }
                                else
                                {
                                    if (stockNewMethod == "Y")
                                    {
                                        vClosingAmount = CloseAmnt;// vClosingQuantity* AvgPrice;
                                    }
                                    else
                                    {
                                        vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                                          Convert.ToDecimal(IssueAmnt));
                                    }

                                    //vClosingAvgRate = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                                }

                                if (vTempremarks.Trim() == "ServiceNS" || vTempremarks.Trim() == "ServiceNSImport")
                                {
                                    vClosingQuantity = 0;
                                    vClosingAmount = 0;
                                }


                                #endregion

                                #region AssignValue to List
                                if (nonstock == true)
                                {
                                    vColumn3 = 0;
                                    vColumn4 = 0;
                                    vColumn17 = 0;
                                    vColumn18 = 0;
                                }

                                vat16.Day = vColumnDay;
                                vat16.Column1 = vColumn1; //   
                                vat16.Column2 = vColumn2; //   
                                vat16.Column3 = vColumn3; //   
                                vat16.Column4 = vColumn4; //   
                                vat16.Column5 = vColumn5; //   
                                vat16.Column6 = vColumn6; //   
                                vat16.Column6String = Convert.ToDateTime(vColumn6).ToString("dd-MMM-yyyy"); //    item["UnitCost"].ToString();      // Production Price
                                vat16.Column7 = vColumn7; //   
                                vat16.Column8 = vColumn8;   // 
                                vat16.Column9 = vColumn9;   // 
                                vat16.Column10 = vColumn10; // 
                                vat16.Column11 = vColumn11; // 
                                vat16.Column12 = vColumn12; // 
                                vat16.Column13 = vColumn13; // 
                                vat16.Column14 = vColumn14; // 
                                vat16.Column15 = vColumn15; // 
                                vat16.Column16 = vColumn16; // 
                                vat16.Column17 = vColumn17; // 
                                vat16.Column18 = vColumn18; // 
                                vat16.Column19 = vColumn19; // 
                                vat16.ProductName = vTempProductName;
                                vat16.ProductCode = vTempProductCode;
                                vat16.FormUom = vUOM;

                                vat16s.Add(vat16);
                                i = i + 1;

                                #endregion AssignValue to List
                            }
                        }
                        #endregion Not  Opening
                    }
                }
                #region Report Preview


                if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                {
                    //objrpt.Load(Program.ReportAppPath + @"\RptVAT6_1.rpt");
                    if (vm.IsTopSheet == true || vm.IsGroupTopSheet == true)
                    {
                        objrpt = new RptVAT6_1_TopSheet();
                    }
                    else
                    {
                        if (vm.InEnglish == "Y")
                        {
                            objrpt = new RptVAT6_1_English();

                        }
                        else
                        {

                            objrpt = new RptVAT6_1();

                            //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_1.rpt");


                        }
                    }

                }

                if (vat16s.Count <= 0)
                {
                    return null;
                }
                objrpt.SetDataSource(vat16s);

                if (vm.PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                ProductDAL _pDal = new ProductDAL();
                //string ProductDescription = _pDal.ProductDTByItemNo(vm.ItemNo).Rows[0]["ProductDescription"].ToString();

                CommonDAL _cDal = new CommonDAL();


                string Quantity6_1 = _cDal.settingsDesktop("DecimalPlace", "Quantity6_1", settingsDt, connVM);
                string Amount6_1 = _cDal.settingsDesktop("DecimalPlace", "Amount6_1", settingsDt, connVM);


                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;

                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Month", Convert.ToDateTime(vm.StartDate).ToString("MMMM-yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsMonthly", vm.IsMonthly ? "Y" : "N");

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOMConversion", UOMConversion);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOM", vm.UOM);


                objrpt.DataDefinition.FormulaFields["Quantity6_1"].Text = "'" + Quantity6_1 + "'";
                objrpt.DataDefinition.FormulaFields["Amount6_1"].Text = "'" + Amount6_1 + "'";


                //objrpt.DataDefinition.FormulaFields["HSCode"].Text = "'" + HSCode + "'";
                //objrpt.DataDefinition.FormulaFields["ProductName"].Text = "'" + ProductName + "'";
                //objrpt.DataDefinition.FormulaFields["ProductDescription"].Text = "'" + ProductDescription + "'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";

                #endregion preview
                    #endregion
                #endregion

                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument VAT6_2_Backup(string ItemNo, string IssueFromDate, string IssueToDate, string post1, string post2, int BranchId
            , bool rbtnService, bool rbtnWIP, bool IsBureau, bool AutoAdjustment, bool PreviewOnly, string InEnglish = "N"
            , decimal UomConversion = 1, string UOM = "", bool IsMonthly = false, SysDBInfoVMTemp connVM = null)
        {
            return null;
        }


        public ReportDocument VAT6_2_Multiple(VAT6_2ParamVM vm, SysDBInfoVMTemp connVM = null)
        {


            #region Variables
            string FromDate = vm.StartDate;
            string ToDate = vm.EndDate;
            string IsRaw = "";
            bool TollProduct = false;

            ReportDocument objrpt = new ReportDocument();
            DataSet ReportResult = new DataSet();
            ReportDSDAL reportDsdal = new ReportDSDAL();
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable settingsDt = new DataTable();

            #endregion

            try
            {
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                #region UOM Conversion

                ////string UOMConversion = Convert.ToString(vm.UOMConversion);

                string UOMConversion = "";
                UOMConversion = Convert.ToString(vm.UOMConversion > 0 ? vm.UOMConversion : 1);

                #endregion

                #region Data Call

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                ReportResult = new DataSet();

                if (vm.IsBureau == true)
                {
                    ReportResult = reportDsdal.BureauVAT6_1Report(vm.ItemNo, vm.StartDate, vm.EndDate, vm.Post1, vm.Post2, connVM);

                }
                else
                {

                    if (vm.IsMonthly)
                    {

                        ReportResult = reportDsdal.VAT6_2_Permanent_DayWise(vm, null, null, connVM);
                    }
                    else
                    {
                        ReportResult = _vatRegistersDAL.VAT6_2(vm, null, null, connVM);

                    }

                }

                //if (vm.IsMonthly == true)
                //{
                //    if (ReportResult != null && ReportResult.Tables.Count > 0)
                //    {
                //        if (ReportResult.Tables[0] != null && ReportResult.Tables[0].Rows.Count > 0)
                //        {
                //            ReportResult = reportDsdal.VAT6_2_Monthly(ReportResult, vm.StartDate, vm.IsTopSheet, connVM);
                //        }

                //    }
                //}

                #endregion

                #region Column Add

                if (!ReportResult.Tables[0].Columns.Contains("Day"))
                {
                    ReportResult.Tables[0].Columns.Add("Day");
                }

                #endregion

                #region Report Generate

                #region CheckPoint

                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }

                #endregion

                List<VAT_17> vat17s = new List<VAT_17>();
                VAT_17 vat17 = new VAT_17();

                DataView view = new DataView(ReportResult.Tables[0]);
                DataTable distinctValues = view.ToTable(true, "ItemNo");

                foreach (DataRow DistinctItem in distinctValues.Rows)
                {
                    #region Process Begin

                    #region Product Call

                    ProductVM vProductVM = new ProductVM();
                    string vitemno = DistinctItem["itemno"].ToString();

                    vProductVM = new ProductDAL().SelectAll(vitemno, null, null, null, null, null, connVM).FirstOrDefault();

                    if (vProductVM == null)
                    {
                        IsRaw = vProductVM.ProductType.ToLower();
                    }

                    IsRaw = vProductVM.ProductType.ToLower();

                    bool nonstock = false;

                    if (IsRaw.ToLower() == "service(nonstock)" || IsRaw.ToLower() == "overhead" || IsRaw.ToLower() == "noninventory")
                    {
                        nonstock = true;
                    }

                    #endregion

                    #region Variables

                    decimal vColumn1 = 0;
                    string vColumnDay = "";

                    DateTime vColumn2 = DateTime.MinValue;
                    decimal vColumn3 = 0;
                    decimal vColumn4 = 0;
                    decimal vColumn5 = 0;
                    decimal vColumn6 = 0;
                    string vColumn7 = string.Empty;
                    string vColumn8 = string.Empty;
                    string vColumn9 = string.Empty;
                    string vColumn10 = string.Empty;
                    DateTime vColumn11 = DateTime.MinValue;
                    string vColumn12 = string.Empty;
                    decimal vColumn13 = 0;
                    decimal vColumn14 = 0;
                    decimal vColumn15 = 0;
                    decimal vColumn16 = 0;
                    decimal vColumn17 = 0;
                    decimal vColumn18 = 0;
                    string vColumn19 = string.Empty;

                    DateTime vTempStartDateTime = DateTime.MinValue;
                    decimal vTempStartingQuantity = 0;
                    decimal vTempStartingAmount = 0;
                    decimal vTempQuantity = 0;
                    decimal vTempSubtotal = 0;
                    string vTempCustomerName = string.Empty;
                    string vTempVATRegistrationNo = string.Empty;
                    string vTempAddress1 = string.Empty;
                    string vTempTransID = string.Empty;
                    DateTime vTemptransDate = DateTime.MinValue;
                    string vTempProductName = string.Empty;
                    string vTempProductCode = string.Empty;
                    string vFormUOM = string.Empty;
                    decimal vTempSDAmount = 0;
                    decimal vTempVATAmount = 0;
                    string vTempremarks = string.Empty;
                    string vTemptransType = string.Empty;

                    decimal vClosingQuantity = 0;
                    decimal vClosingAmount = 0;
                    decimal vClosingAvgRatet = 0;

                    decimal OpeningQty = 0;
                    decimal OpeningAmnt = 0;
                    decimal ProductionQty = 0;
                    decimal ProductionAmnt = 0;
                    decimal SaleQty = 0;
                    decimal SaleAmnt = 0;
                    decimal CloseQty = 0;
                    decimal CloseAmnt = 0;

                    decimal OpQty = 0;
                    decimal OpAmnt = 0;
                    decimal avgRate = 0;
                    string HSCode = string.Empty;
                    string ProductName = string.Empty;
                    string ProductCode = string.Empty;
                    string FormUOM = string.Empty;
                    decimal vClosingQuantityNew = 0;
                    decimal vClosingAmountNew = 0;
                    decimal vUnitRate = 0;
                    decimal vAdjustmentValue = 0;


                    #endregion

                    int i = 1;
                    if (vm.rbtnWIP == false)
                    {

                        //DataRow[] DetailRawsOpening = ReportResult.Tables[1].Select("transType='Opening'");
                        //DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("transType='Opening'");
                        DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("transType='Opening' and Itemno='" + vitemno + "'");
                        vat17 = new VAT_17();
                        #region Opening

                        foreach (DataRow row in DetailRawsOpening)
                        {
                            ProductDAL productDal = new ProductDAL();
                            vTempremarks = row["remarks"].ToString().Trim();
                            vTemptransType = row["TransType"].ToString().Trim();
                            //vTemptransType = row["TransType"].ToString().Trim();
                            ProductName = row["ProductName"].ToString().Trim();
                            ProductCode = row["ProductCode"].ToString().Trim();
                            FormUOM = row["UOM"].ToString().Trim();
                            HSCode = row["HSCodeNo"].ToString().Trim();
                            var tt = row["StartDateTime"].ToString().Trim();
                            vTempStartDateTime = Convert.ToDateTime(row["StartDateTime"].ToString().Trim());


                            decimal LastNBRPrice = productDal.GetLastNBRPriceFromBOM(vProductVM.ItemNo, Convert.ToDateTime(vTempStartDateTime).ToString("yyyy-MMM-dd"), null, null, "0", connVM);

                            //Convert.ToDecimal(Program.FormatingNumeric(dollerRate.ToString(), SalePlaceDollar));

                            decimal q11 = Convert.ToDecimal(row["Quantity"].ToString().Trim());
                            decimal q12 = Convert.ToDecimal(row["UnitCost"].ToString().Trim());
                            //OpQty = TCloseQty;//
                            //OpAmnt = TCloseAmnt;
                            OpQty = q11;//
                            OpAmnt = q12;//
                            //vat17 = new VAT_17();

                            #region if row 1 Opening

                            OpeningQty = OpQty;
                            OpeningAmnt = OpAmnt;// OpQty* LastNBRPrice;
                            OpAmnt = 0;
                            OpQty = 0;

                            ProductionQty = 0;
                            ProductionAmnt = 0;
                            SaleQty = 0;
                            SaleAmnt = 0;

                            CloseQty =
                                (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                 Convert.ToDecimal(SaleQty));
                            CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                         Convert.ToDecimal(SaleAmnt));

                            vColumnDay = Convert.ToString(row["Day"]);
                            vColumn2 = vTempStartDateTime;
                            vColumn3 = OpeningQty;
                            vColumn4 = OpeningAmnt;
                            vColumn5 = 0; // Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                            vColumn6 = 0; // Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                            vColumn7 = "-";
                            vColumn8 = "-";
                            vColumn9 = "-";
                            vColumn10 = "-";
                            vColumn11 = DateTime.MinValue;
                            vColumn12 = "-";
                            vColumn13 = 0; // Convert.ToDecimal(Program.FormatingNumeric(SaleQty.ToString(), SalePlaceQty));
                            vColumn14 = 0; // Convert.ToDecimal(Program.FormatingNumeric(SaleAmnt.ToString(), SalePlaceTaka));
                            vColumn15 = 0;
                            vColumn16 = 0;
                            vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                            vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                            vColumn19 = vTempremarks;
                            vUnitRate = Convert.ToDecimal(row["UnitRate"].ToString().Trim());
                            vClosingQuantityNew = vColumn17;
                            vClosingAmountNew = vColumn18;

                            vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                                Convert.ToDecimal(SaleQty));

                            if (vClosingQuantity == 0)
                            {
                                vClosingAmount = 0;
                                vClosingAvgRatet = 0;

                            }
                            else
                            {
                                vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                                  Convert.ToDecimal(SaleAmnt));
                                vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                            }

                            #endregion


                            #region AssignValue to List
                            if (nonstock == true)
                            {
                                vColumn3 = 0;
                                vColumn4 = 0;
                                vColumn5 = 0;
                                vColumn6 = 0;
                                vColumn17 = 0;
                                vColumn18 = 0;
                            }

                            vat17.Column1 = i; //    i.ToString();      // Serial No   
                            vat17.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                            vat17.Day = vColumnDay; //    item["StartDateTime"].ToString();      // Date
                            vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                            vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                            vat17.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                            vat17.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                            vat17.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                            vat17.Column8 = vColumn8;   //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                            vat17.Column9 = vColumn9;   //    item["Address1"].ToString();      // Customer Address
                            vat17.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                            vat17.Column11 = vColumn11; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                            vat17.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                            vat17.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                            vat17.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                            vat17.Column14 = vColumn14; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                            vat17.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                            vat17.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                            vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                            vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                            vat17.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                            vat17.ProductName = ProductName;
                            vat17.ProductCode = ProductCode;
                            vat17.FormUom = FormUOM;
                            vat17s.Add(vat17);
                            i = i + 1;

                            #endregion AssignValue to List

                        }
                        #endregion Opening
                    }


                    //DataRow[] DetailRawsOthers = ReportResult.Tables[1].Select("transType<>'Opening'");
                    DataRow[] DetailRawsOthers = ReportResult.Tables[0].Select("transType<>'Opening' and Itemno='" + vitemno + "'");

                    if (DetailRawsOthers == null || !DetailRawsOthers.Any())
                    {
                        continue;
                    }
                    //string strSort = "StartDateTime ASC, SerialNo ASC";
                    string strSort = "StartDateTime ASC, SerialNo ASC";

                    DataView dtview = new DataView(DetailRawsOthers.CopyToDataTable());
                    dtview.Sort = strSort;
                    DataTable dtsorted = dtview.ToTable();

                    #endregion

                    #region Process Continue

                    foreach (DataRow item in dtsorted.Rows)
                    {
                        vat17 = new VAT_17();
                        VAT_17 vat17Adj = new VAT_17();

                        #region Get from Datatable

                        OpeningQty = 0;
                        OpeningAmnt = 0;
                        ProductionQty = 0;
                        ProductionAmnt = 0;
                        SaleQty = 0;
                        SaleAmnt = 0;
                        CloseQty = 0;
                        CloseAmnt = 0;

                        vColumn1 = i;
                        vColumnDay = Convert.ToString(item["Day"]);
                        vTempStartDateTime = Convert.ToDateTime(item["StartDateTime"].ToString()); // Date
                        vTempStartingQuantity = Convert.ToDecimal(item["StartingQuantity"].ToString()); // Opening Quantity
                        vTempStartingAmount = Convert.ToDecimal(item["StartingAmount"].ToString()); // Opening Price
                        vTempQuantity = Convert.ToDecimal(item["Quantity"].ToString()); // Production Quantity
                        vTempSubtotal = Convert.ToDecimal(item["UnitCost"].ToString()); // Production Price
                        vTempCustomerName = item["CustomerName"].ToString(); // Customer Name
                        vTempVATRegistrationNo = item["VATRegistrationNo"].ToString(); // Customer VAT Reg No
                        vTempAddress1 = item["Address1"].ToString(); // Customer Address
                        vTempTransID = item["TransID"].ToString(); // Sale Invoice No
                        vTemptransDate = Convert.ToDateTime(item["StartDateTime"].ToString()); // Sale Invoice Date and Time
                        vTempProductName = item["ProductName"].ToString(); // Sale Product Name
                        vTempSDAmount = Convert.ToDecimal(item["SD"].ToString()); // SD Amount
                        vTempVATAmount = Convert.ToDecimal(item["VATRate"].ToString()); // VAT Amount
                        vTempremarks = item["remarks"].ToString(); // Remarks
                        vTemptransType = item["TransType"].ToString().Trim();
                        vTempProductCode = item["ProductCode"].ToString().Trim();
                        vFormUOM = item["UOM"].ToString().Trim();
                        #region Bureau Condition

                        if (vm.IsBureau == true)
                        {
                            ProductName = item["ProductName"].ToString().Trim();
                            HSCode = item["HSCodeNo"].ToString().Trim();
                        }

                        #endregion

                        #endregion Get from Datatable

                        if (vTemptransType.Trim() == "Sale")
                        {
                            #region Sale Data

                            //vat17 = new VAT_17();
                            #region if row 1 Opening

                            if (vTempremarks.Trim() == "ServiceNS" || vTempremarks.Trim() == "ServiceNSImport")
                            {
                                OpeningQty = 0;
                                OpeningAmnt = 0;
                            }
                            else
                            {
                                OpeningQty = OpQty + vClosingQuantity;
                                OpeningAmnt = OpAmnt + vClosingAmount;
                            }


                            OpAmnt = 0;
                            OpQty = 0;

                            ProductionQty = 0;
                            ProductionAmnt = 0;
                            SaleQty = vTempQuantity;
                            if (
                                vTempremarks.Trim() == "TradingTender" || vTempremarks.Trim() == "ExportTradingTender"
                                || vTempremarks.Trim() == "ExportTender" || vTempremarks.Trim() == "Tender"
                                || vTempremarks.Trim() == "Export"
                                )
                            {
                                SaleAmnt = vTempSubtotal;
                            }
                            else
                            {
                                SaleAmnt = vTempSubtotal;
                            }

                            if (vTempremarks.Trim() == "ExportTradingTender" || vTempremarks.Trim() == "ExportTender" || vTempremarks.Trim() == "Export")
                            {
                            }
                            else
                            {
                                SaleAmnt = Convert.ToDecimal(SaleAmnt.ToString());
                            }


                            if (SaleQty == 0)
                            {
                                avgRate = 0;
                            }
                            else
                            {
                                avgRate = SaleAmnt / SaleQty;
                            }

                            if (vTempremarks.Trim() == "ExportTradingTender" || vTempremarks.Trim() == "ExportTender" || vTempremarks.Trim() == "Export")
                            {
                            }
                            else
                            {
                                avgRate = Convert.ToDecimal(avgRate.ToString());
                            }

                            CloseQty = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) - Convert.ToDecimal(SaleQty));
                            CloseAmnt = CloseQty * avgRate;
                            vColumn2 = vTempStartDateTime;

                            string returnTransType = reportDsdal.GetReturnType(vProductVM.ItemNo, vTempremarks.Trim(), connVM);

                            if (vTempremarks.Trim() == "ServiceNS" || vTempremarks.Trim() == "ExportServiceNS" || returnTransType == "ServiceNS")
                            {
                                vColumn3 = 0;
                                vColumn4 = 0;
                                vColumn5 = 0;
                                vColumn6 = 0;
                            }
                            else
                            {
                                vColumn3 = OpeningQty;// vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                                vColumn4 = OpeningAmnt;// vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));

                                vColumn5 = 0;// Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                                vColumn6 = 0;// Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                            }
                            vColumn7 = vTempCustomerName;
                            vColumn8 = vTempVATRegistrationNo;
                            vColumn9 = vTempAddress1;
                            vColumn10 = vTempTransID;
                            vColumn11 = vTemptransDate;
                            vColumn12 = vTempProductName;
                            if (vTempremarks.Trim() == "ExportTradingTender"
                                || vTempremarks.Trim() == "ExportTender"
                                || vTempremarks.Trim() == "Export")
                            {
                                vColumn13 = SaleQty;
                                vColumn14 = SaleAmnt;
                            }
                            else
                            {
                                vColumn13 = Convert.ToDecimal(SaleQty.ToString());
                                vColumn14 = Convert.ToDecimal(SaleAmnt.ToString());
                            }
                            vColumn15 = vTempSDAmount;
                            vColumn16 = vTempVATAmount;
                            if (vTempremarks.Trim() == "ServiceNS"
                                || vTempremarks.Trim() == "ExportServiceNS"
                                || returnTransType == "ServiceNS"
                                )
                            {
                                vColumn17 = 0;
                                vColumn18 = 0;
                            }
                            else
                            {
                                vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));

                                //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                            }
                            vColumn19 = vTempremarks;

                            if (vTempremarks.ToLower() == "Other".ToLower())
                            {
                                vColumn19 = "Sale (Local)";
                            }



                            vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                                Convert.ToDecimal(SaleQty));

                            vClosingQuantityNew = vColumn17;
                            vClosingAmountNew = vColumn18;
                            vUnitRate = Convert.ToDecimal(item["UnitRate"].ToString().Trim());

                            if (vClosingQuantity == 0)
                            {
                                //Change at 29-04-14
                                //vClosingAmount = 0;
                                //vClosingAvgRatet = 0;
                                vClosingAmount = CloseAmnt;

                            }
                            else
                            {
                                //vClosingAmount = vClosingQuantity * avgRate;
                                vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                if (vTempremarks.Trim() == "TradingTender"
                                || vTempremarks.Trim() == "ExportTradingTender"
                                || vTempremarks.Trim() == "ExportTender"
                                || vTempremarks.Trim() == "Tender"
                                || vTempremarks.Trim() == "Export"
                                )
                                {

                                }
                                else
                                {
                                    vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));
                                }

                            }
                            if (vTempremarks.Trim() == "ExportTradingTender"
                                || vTempremarks.Trim() == "ExportTender"
                                || vTempremarks.Trim() == "Export")
                            {

                            }
                            else
                            {
                                vClosingAvgRatet = Convert.ToDecimal(vClosingAvgRatet.ToString());
                            }
                            #endregion
                            #region AssignValue to List
                            if (nonstock == true)
                            {
                                vColumn3 = 0;
                                vColumn4 = 0;
                                vColumn5 = 0;
                                vColumn6 = 0;
                                vColumn17 = 0;
                                vColumn18 = 0;
                            }
                            vat17.Column1 = i; //    i.ToString();      // Serial No   
                            vat17.Day = vColumnDay;
                            vat17.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                            vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                            vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                            vat17.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                            vat17.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                            vat17.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                            vat17.Column8 = vColumn8;//    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                            vat17.Column9 = vColumn9; //    item["Address1"].ToString();      // Customer Address
                            vat17.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                            vat17.Column11 = vColumn11;//    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                            vat17.Column11string = Convert.ToDateTime(vColumn11).ToString("dd/MMM/yy HH:mm"); //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                            vat17.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                            vat17.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                            vat17.Column14 = vColumn14;//    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                            vat17.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                            vat17.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                            vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                            vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                            vat17.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                            if (vColumn18 != vColumn17 * vUnitRate)
                            {
                                //AutoAdjustment = true;
                                //vAdjustmentValue = vColumn18 - (vColumn17 * vUnitRate);
                                vAdjustmentValue = (vColumn17 * vUnitRate) - vColumn18;
                            }
                            vat17.ProductName = vTempProductName;
                            vat17.ProductCode = vTempProductCode;
                            vat17.FormUom = vFormUOM;
                            vat17s.Add(vat17);
                            i = i + 1;

                            #endregion AssignValue to List

                            //// Service related company has no need auto adjustment
                            if (vm.IsBureau == false)
                            {
                                if (avgRate == vClosingAvgRatet)
                                {
                                    //vat17s.Add(vat17);
                                }
                                if (vm.AutoAdjustment == true)
                                {
                                    #region SaleAdjustment
                                    //if (avgRate != vClosingAvgRatet)
                                    if (vColumn18 != vColumn17 * vUnitRate)
                                    {

                                        decimal a = 0;
                                        decimal b = 0;
                                        if (vClosingQuantity == 0)
                                        {
                                            a = avgRate;          //1950
                                            b = vClosingAvgRatet; //1350
                                        }
                                        else
                                        {
                                            a = avgRate * vClosingQuantity;           //1950
                                            b = vClosingAvgRatet * vClosingQuantity; //1350

                                        }

                                        decimal c = b - a;// Convert.ToDecimal(Program.FormatingNumeric(b.ToString(),1)) - Convert.ToDecimal(Program.FormatingNumeric(a.ToString(),1));
                                        c = Convert.ToDecimal(c.ToString());
                                        //hide 0 value row
                                        if (c != 0)
                                        {
                                            //VAT_17 vat17Adj = new VAT_17();
                                            #region if row 1 Opening

                                            OpeningQty = OpQty + vClosingQuantity;
                                            OpeningAmnt = OpAmnt + vClosingAmount;
                                            OpAmnt = 0;
                                            OpQty = 0;

                                            ProductionQty = 0;
                                            ProductionAmnt = 0;
                                            SaleQty = 0;
                                            if (vTempremarks.Trim() == "TradingTender"
                                       || vTempremarks.Trim() == "ExportTradingTender"
                                       || vTempremarks.Trim() == "ExportTender"
                                       || vTempremarks.Trim() == "Tender"
                                                || vTempremarks.Trim() == "Export"
                                       )
                                            {
                                                if (vClosingQuantity == 0)
                                                {

                                                    //SaleAmnt = (avgRate - vClosingAvgRatet) * vTempQuantity;
                                                    SaleAmnt = (vClosingAvgRatet - avgRate) * vTempQuantity;
                                                }
                                                else
                                                {
                                                    SaleAmnt = (vTempQuantity * vClosingAvgRatet) - vTempSubtotal;

                                                }
                                            }
                                            else
                                            {
                                                //SaleAmnt=(avgRate* vClosingQuantity)-(vClosingAvgRatet * vClosingQuantity);  
                                                if (vClosingQuantity == 0)
                                                {
                                                    SaleAmnt = c * SaleQty;
                                                }
                                                else
                                                {
                                                    SaleAmnt = c;
                                                }
                                            }
                                            if (vTempremarks.Trim() == "ExportTradingTender"
                                                    || vTempremarks.Trim() == "ExportTender"
                                                    || vTempremarks.Trim() == "Export"
                                            )
                                            {

                                            }
                                            else
                                            {
                                                SaleAmnt = Convert.ToDecimal(SaleAmnt.ToString());
                                            }


                                            //SaleAmnt = c;

                                            if (SaleQty == 0)
                                            {
                                                avgRate = 0;
                                            }
                                            else
                                            {
                                                avgRate = SaleAmnt / SaleQty;
                                            }
                                            if (vTempremarks.Trim() == "ExportTradingTender"
                                                 || vTempremarks.Trim() == "ExportTender"
                                                 || vTempremarks.Trim() == "Export")
                                            {

                                            }
                                            else
                                            {
                                                avgRate = Convert.ToDecimal(avgRate.ToString());
                                            }
                                            CloseQty =
                                                (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                                 Convert.ToDecimal(SaleQty));

                                            CloseAmnt = CloseQty * avgRate;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                            vColumn2 = vTempStartDateTime;

                                            vColumn3 = vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                                            vColumn4 = vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                                            vColumn5 = 0;// Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                                            vColumn6 = 0;//Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                                            vColumn7 = vTempCustomerName;
                                            vColumn8 = vTempVATRegistrationNo;
                                            vColumn9 = vTempAddress1;
                                            vColumn10 = vTempTransID;
                                            vColumn11 = vTemptransDate;
                                            vColumn12 = vTempProductName;
                                            if (vTempremarks.Trim() == "ExportTradingTender"
                                                || vTempremarks.Trim() == "ExportTender"
                                                || vTempremarks.Trim() == "Export")
                                            {
                                                vColumn13 = 0;
                                                vColumn14 = vAdjustmentValue;//;
                                            }
                                            else
                                            {
                                                vColumn13 = 0;// Convert.ToDecimal(Program.FormatingNumeric(SaleQty.ToString(), SalePlaceQty));
                                                vColumn14 = Convert.ToDecimal(vAdjustmentValue.ToString());
                                            }
                                            vColumn15 = 0;
                                            vColumn16 = 0;
                                            vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                            vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));

                                            vClosingQuantityNew = vColumn17;
                                            vClosingAmountNew = vColumn18;

                                            //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                            //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                            vColumn19 = "SaleAdjustment";
                                            //vClosingAmount = vClosingQuantity * avgRate;

                                            vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) - Convert.ToDecimal(SaleQty));
                                            if (vClosingQuantity == 0)
                                            {
                                                vClosingAmount = 0;
                                                vClosingAvgRatet = 0;
                                            }
                                            else
                                            {
                                                vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                                                  Convert.ToDecimal(SaleAmnt));
                                                vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));
                                            }

                                            #endregion
                                            #region AssignValue to List
                                            if (nonstock == true)
                                            {
                                                vColumn3 = 0;
                                                vColumn4 = 0;
                                                vColumn5 = 0;
                                                vColumn6 = 0;
                                                vColumn17 = 0;
                                                vColumn18 = 0;
                                            }
                                            vat17Adj.Column1 = i; //    i.ToString();      // Serial No   
                                            vat17Adj.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                                            vat17Adj.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                            vat17Adj.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                                            vat17Adj.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                                            vat17Adj.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                                            vat17Adj.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                                            vat17Adj.Column8 = vColumn8;//    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                                            vat17Adj.Column9 = vColumn9; //    item["Address1"].ToString();      // Customer Address
                                            vat17Adj.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                                            vat17Adj.Column11 = vColumn11;//    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                            vat17Adj.Column11string = Convert.ToDateTime(vColumn11).ToString("dd/MMM/yy HH:mm"); //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                            vat17Adj.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                                            vat17Adj.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                                            vat17Adj.Column14 = vColumn14;//    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                                            vat17Adj.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                                            vat17Adj.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount


                                            vat17Adj.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                                            vat17Adj.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                                            vat17Adj.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                                            vat17Adj.ProductName = vTempProductName;
                                            vat17Adj.ProductCode = vTempProductCode;
                                            vat17Adj.FormUom = vFormUOM;

                                            //vat17.Column18 = vat17.Column18 + vat17Adj.Column6;
                                            //vat17s.Add(vat17);

                                            vat17Adj.Column4 = vat17.Column18;
                                            vat17s.Add(vat17Adj);
                                            //AutoAdjustment = false;

                                            i = i + 1;


                                            #endregion AssignValue to List
                                        }
                                    }
                                    #endregion SaleAdjustment
                                }

                            }

                            #endregion
                        }
                        else if (vTemptransType == "Receive")
                        {
                            #region Receive Data

                            if (IsRaw.ToLower() == "service(nonstock)" || IsRaw.ToLower() == "overhead" || IsRaw.ToLower() == "noninventory")
                            {
                                continue;
                            }

                            //vat17 = new VAT_17();

                            #region if row 1 Opening

                            OpeningQty = OpQty + vClosingQuantity;
                            OpeningAmnt = OpAmnt + vClosingAmount;
                            OpAmnt = 0;
                            OpQty = 0;

                            ProductionQty = vTempQuantity;
                            ProductionAmnt = vTempSubtotal;
                            SaleQty = 0;
                            SaleAmnt = 0;
                            if (ProductionQty == 0)
                            {
                                avgRate = 0;
                            }
                            else
                            {
                                avgRate = ProductionAmnt / ProductionQty;

                            }
                            if (vTempremarks.Trim() == "ExportTradingTender"
                                       || vTempremarks.Trim() == "ExportTender"
                                       || vTempremarks.Trim() == "Export")
                            {

                            }
                            else
                            {
                                avgRate = Convert.ToDecimal(avgRate.ToString());
                            }
                            CloseQty =
                                (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                 Convert.ToDecimal(SaleQty));
                            CloseAmnt = CloseQty * avgRate;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                            vColumn2 = vTempStartDateTime;
                            vColumn3 = vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                            vColumn4 = vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                            vColumn5 = Convert.ToDecimal(ProductionQty.ToString());
                            vColumn6 = Convert.ToDecimal(ProductionAmnt.ToString());
                            vColumn7 = "-";
                            vColumn8 = "-";
                            vColumn9 = "-";
                            vColumn10 = "-";
                            vColumn11 = Convert.ToDateTime("1900/01/01");
                            vColumn12 = "-";
                            vColumn13 = 0;
                            vColumn14 = 0;
                            vColumn15 = 0;
                            vColumn16 = 0;
                            vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                            vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                            vClosingQuantityNew = vColumn17;
                            vClosingAmountNew = vColumn18;
                            vUnitRate = Convert.ToDecimal(item["UnitRate"].ToString().Trim());
                            //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                            //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                            vColumn19 = vTempremarks;

                            if (vTempremarks.ToLower() == "Other".ToLower())
                            {
                                vColumn19 = "Receive";
                            }

                            vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                                Convert.ToDecimal(SaleQty));
                            if (vClosingQuantity == 0)
                            {
                                vClosingAmount = 0;
                                vClosingAvgRatet = 0;

                            }
                            else
                            {
                                //vClosingAmount = vClosingQuantity * avgRate;

                                vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                if (vTempremarks.Trim() == "TradingTender"
                               || vTempremarks.Trim() == "ExportTradingTender"
                               || vTempremarks.Trim() == "ExportTender"
                               || vTempremarks.Trim() == "Tender"
                               || vTempremarks.Trim() == "Export"
                               )
                                {
                                    // change at 20150324 for Nita requierment
                                    vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) /
                                                         Convert.ToDecimal(vClosingQuantity));
                                }
                                else
                                {
                                    vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) /
                                                        Convert.ToDecimal(vClosingQuantity));
                                }

                            }
                            if (vTempremarks.Trim() == "ExportTradingTender"
                                        || vTempremarks.Trim() == "ExportTender"
                                        || vTempremarks.Trim() == "Export")
                            {

                            }
                            else
                            {
                                vClosingAvgRatet = Convert.ToDecimal(vClosingAvgRatet.ToString());
                            }
                            #endregion

                            #region AssignValue to List
                            if (nonstock == true)
                            {
                                vColumn3 = 0;
                                vColumn4 = 0;
                                vColumn5 = 0;
                                vColumn6 = 0;
                                vColumn17 = 0;
                                vColumn18 = 0;
                            }
                            vat17.Column1 = i; //    i.ToString();      // Serial No   
                            vat17.Day = vColumnDay;
                            vat17.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                            vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                            vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                            vat17.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                            vat17.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                            vat17.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                            vat17.Column8 = vColumn8;   //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                            vat17.Column9 = vColumn9;   //    item["Address1"].ToString();      // Customer Address
                            vat17.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                            vat17.Column11 = vColumn11; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                            vat17.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                            vat17.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                            vat17.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                            vat17.Column14 = vColumn14; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                            vat17.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                            vat17.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                            vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                            vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                            vat17.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                            vat17.ProductName = vTempProductName;
                            vat17.ProductCode = vTempProductCode;
                            vat17.FormUom = vFormUOM;

                            vat17s.Add(vat17);
                            i = i + 1;
                            if (vColumn18 != vColumn17 * vUnitRate)
                            {
                                //AutoAdjustment = true;
                                vAdjustmentValue = (vColumn17 * vUnitRate) - vColumn18;
                            }

                            #endregion AssignValue to List

                            //if (avgRate == vClosingAvgRatet)
                            //{
                            //    vat17s.Add(vat17);
                            //}
                            if (vm.AutoAdjustment == true)
                            {
                                #region ProductionAdjustment


                                //if (avgRate != vClosingAvgRatet)
                                if (vColumn18 != vColumn17 * vUnitRate)
                                {
                                    //vColumn4
                                    //vClosingAvgRatet = vColumn4 / vColumn3;
                                    //decimal x = vColumn4 / vColumn3;
                                    decimal a = avgRate * vClosingQuantity;         //7200
                                    decimal b = vClosingAvgRatet * vClosingQuantity;//7300
                                    decimal c = a - b;
                                    //  b = x * vClosingQuantity;//7300
                                    //c = a - b;
                                    if (vTempremarks.Trim() == "TradingTender"
                                  || vTempremarks.Trim() == "ExportTradingTender"
                                  || vTempremarks.Trim() == "ExportTender"
                                  || vTempremarks.Trim() == "Tender"
                                  || vTempremarks.Trim() == "Export"
                                  )
                                    {
                                        c = (vClosingAvgRatet - avgRate) * ProductionQty;
                                    }
                                    if (vTempremarks.Trim() == "ExportTradingTender"
                                           || vTempremarks.Trim() == "ExportTender"
                                           || vTempremarks.Trim() == "Export")
                                    {

                                    }
                                    else
                                    {
                                        c = Convert.ToDecimal(c.ToString());
                                    }
                                    //hide 0 value row
                                    if (c != 0)
                                    {
                                        //VAT_17 vat17Adj = new VAT_17();

                                        #region if row 1 Opening

                                        OpeningQty = OpQty + vClosingQuantity;
                                        OpeningAmnt = OpAmnt + vClosingAmount;
                                        OpAmnt = 0;
                                        OpQty = 0;

                                        ProductionQty = 0;
                                        ProductionAmnt = c;
                                        SaleQty = 0;
                                        SaleAmnt = 0;
                                        CloseQty =
                                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                             Convert.ToDecimal(SaleQty));
                                        CloseAmnt = avgRate * vClosingQuantity;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                        vColumn2 = vTempStartDateTime;
                                        vColumn3 = vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                                        vColumn4 = vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                                        vColumn5 = Convert.ToDecimal(ProductionQty.ToString());
                                        vColumn6 = vAdjustmentValue;// vColumn18 - vColumn4;// Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                                        vColumn7 = "-";
                                        vColumn8 = "-";
                                        vColumn9 = "-";
                                        vColumn10 = "-";
                                        vColumn11 = Convert.ToDateTime("1900/01/01");
                                        vColumn12 = "-";
                                        vColumn13 = 0;
                                        vColumn14 = 0;
                                        vColumn15 = 0;
                                        vColumn16 = 0;
                                        vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                        vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                        vClosingQuantityNew = vColumn17;
                                        vClosingAmountNew = vColumn18;
                                        //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                        //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                        vColumn19 = "ProductionAdjustment";

                                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                                            Convert.ToDecimal(SaleQty));
                                        if (vClosingQuantity == 0)
                                        {
                                            vClosingAmount = 0;
                                            vClosingAvgRatet = 0;

                                        }
                                        else
                                        {
                                            //vClosingAmount = vClosingQuantity * avgRate;
                                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                            vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                                        }

                                        #endregion

                                        #region AssignValue to List
                                        if (nonstock == true)
                                        {
                                            vColumn3 = 0;
                                            vColumn4 = 0;
                                            vColumn5 = 0;
                                            vColumn6 = 0;
                                            vColumn17 = 0;
                                            vColumn18 = 0;
                                        }
                                        vat17Adj.Column1 = i; //    i.ToString();      // Serial No   
                                        vat17Adj.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                                        vat17Adj.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                        vat17Adj.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                                        vat17Adj.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                                        vat17Adj.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                                        vat17Adj.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                                        vat17Adj.Column8 = vColumn8;
                                        //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                                        vat17Adj.Column9 = vColumn9; //    item["Address1"].ToString();      // Customer Address
                                        vat17Adj.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                                        vat17Adj.Column11 = vColumn11;//    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                        vat17Adj.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                        vat17Adj.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                                        vat17Adj.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                                        vat17Adj.Column14 = vColumn14;
                                        //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                                        vat17Adj.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                                        vat17Adj.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                                        vat17Adj.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                                        vat17Adj.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                                        vat17Adj.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                                        vat17Adj.ProductName = vTempProductName;
                                        vat17Adj.ProductCode = vTempProductCode;
                                        vat17Adj.FormUom = vFormUOM;


                                        //vat17.Column18 = vat17.Column18 + vat17Adj.Column6;
                                        //vat17s.Add(vat17);
                                        //vat17Adj.Column4 = vat17.Column18;
                                        vat17s.Add(vat17Adj);
                                        i = i + 1;

                                        #endregion AssignValue to List
                                    }
                                }
                                #endregion ProductionAdjustment
                            }

                            #endregion
                        }

                    }

                    #endregion
                }

                #region Report preview
                CommonDAL commonDal = new CommonDAL();
                string v3Digits = commonDal.settingsDesktop("VAT6_2", "Report3Digits", settingsDt, connVM);
                bool Permanent6_2 = true;

                if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                {
                    if (vm.InEnglish == "Y")
                    {
                        objrpt = new ReportDocument();
                        objrpt = new RptVAT6_2_English();
                        ////objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_2_English.rpt");
                    }
                    else
                    {
                        objrpt = new ReportDocument();
                        if (Permanent6_2)
                        {
                            objrpt = new RptVAT6_2_permanet();
                            ////objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_2_permanet.rpt");

                        }
                        else
                        {
                            objrpt = new RptVAT6_2();

                        }

                    }

                }

                //}


                if (vm.PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    //objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                if (vm.rbtnService)
                {
                    //objrpt.DataDefinition.FormulaFields["TransactionType"].Text = "'Service'";

                }
                else
                {
                    //  objrpt.DataDefinition.FormulaFields["TransactionType"].Text = "'Others'";

                }
                if (TollProduct)
                {
                    objrpt.DataDefinition.FormulaFields["IsToll"].Text = "'Y'";

                }


                #region Set DataSource


                objrpt.SetDataSource(vat17s);

                #endregion

                #region Formula Fields

                CommonDAL _cDal = new CommonDAL();
                string Quantity6_2 = _cDal.settingsDesktop("DecimalPlace", "Quantity6_2", settingsDt, connVM);
                string Amount6_2 = _cDal.settingsDesktop("DecimalPlace", "Amount6_2", settingsDt, connVM);
                string AutoAdj = _cDal.settingsDesktop("VAT6_2", "AutoAdjustment", settingsDt, connVM);

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;

                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Month", Convert.ToDateTime(vm.StartDate).ToString("MMMM-yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsMonthly", vm.IsMonthly ? "Y" : "N");

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", string.IsNullOrEmpty(vm.FontSize) ? OrdinaryVATDesktop.FontSize : vm.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_2", vm.DecimalPlace == null ? Amount6_2 : vm.DecimalPlace);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOMConversion", UOMConversion);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOM", vm.UOM);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FromDate", Convert.ToDateTime(FromDate).ToString("dd/MM/yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ToDate", Convert.ToDateTime(ToDate).ToString("dd/MM/yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "AutoAdjustment", AutoAdj);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_2", vm.DecimalPlace == null ? Quantity6_2 : vm.DecimalPlace);

                //objrpt.DataDefinition.FormulaFields["Quantity6_2"].Text = "'" + Quantity6_2 + "'";


                //objrpt.DataDefinition.FormulaFields["HSCode"].Text = "'" + HSCode + "'";
                //objrpt.DataDefinition.FormulaFields["ProductName"].Text = "'" + ProductName + "'";
                //string ProductDescription = vProductVM.ProductDescription;// _pDal.ProductDTByItemNo(txtItemNo.Text.Trim()).Rows[0]["ProductDescription"].ToString();
                //objrpt.DataDefinition.FormulaFields["ProductDescription"].Text = "'" + ProductDescription + "'";


                #endregion

                #endregion Report preview

                #endregion

                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument VAT6_2Backup(VAT6_2ParamVM vm, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            string IsRaw = "";
            bool TollProduct = false;

            ReportDocument objrpt = new ReportDocument();
            DataSet ReportResult = new DataSet();
            ReportDSDAL reportDsdal = new ReportDSDAL();
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable settingsDt = new DataTable();

            #endregion

            try
            {
                #region UOM Conversion

                ////string UOMConversion = Convert.ToString(vm.UOMConversion);

                string UOMConversion = "";
                UOMConversion = Convert.ToString(vm.UOMConversion > 0 ? vm.UOMConversion : 1);

                #endregion

                #region Data Call

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                ReportResult = new DataSet();

                if (vm.IsBureau == true)
                {
                    ReportResult = reportDsdal.BureauVAT6_1Report(vm.ItemNo, vm.StartDate, vm.EndDate, vm.Post1, vm.Post2, connVM);

                }
                else
                {
                    //ProductDAL pdal = new ProductDAL();

                    //var pro = pdal.SearchByItemNo(vm.ItemNo);

                    //if (pro.Rows[0]["TollProduct"].ToString() == "Y")
                    //{
                    //    TollProduct = true;
                    //}


                    ReportResult = _vatRegistersDAL.VAT6_2(vm, null, null, connVM);

                }

                if (vm.IsMonthly == true)
                {
                    if (ReportResult != null && ReportResult.Tables.Count > 0)
                    {
                        if (ReportResult.Tables[0] != null && ReportResult.Tables[0].Rows.Count > 0)
                        {
                            ReportResult = reportDsdal.VAT6_2_Monthly(vm, connVM);
                        }

                    }
                }

                #endregion

                #region Column Add

                if (!ReportResult.Tables[0].Columns.Contains("Day"))
                {
                    ReportResult.Tables[0].Columns.Add("Day");
                }

                #endregion


                #region Report Generate

                #region CheckPoint

                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }

                #region Product Call

                ProductVM vProductVM = new ProductVM();

                vProductVM = new ProductDAL().SelectAll(vm.ItemNo, null, null, null, null, null, connVM).FirstOrDefault();

                IsRaw = vProductVM.ProductType.ToLower();

                bool nonstock = false;

                if (IsRaw.ToLower() == "service(nonstock)" || IsRaw.ToLower() == "overhead" || IsRaw.ToLower() == "noninventory")
                {
                    nonstock = true;
                }

                #endregion


                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }

                #endregion

                #region Variables

                decimal vColumn1 = 0;
                string vColumnDay = "";

                DateTime vColumn2 = DateTime.MinValue;
                decimal vColumn3 = 0;
                decimal vColumn4 = 0;
                decimal vColumn5 = 0;
                decimal vColumn6 = 0;
                string vColumn7 = string.Empty;
                string vColumn8 = string.Empty;
                string vColumn9 = string.Empty;
                string vColumn10 = string.Empty;
                DateTime vColumn11 = DateTime.MinValue;
                string vColumn12 = string.Empty;
                decimal vColumn13 = 0;
                decimal vColumn14 = 0;
                decimal vColumn15 = 0;
                decimal vColumn16 = 0;
                decimal vColumn17 = 0;
                decimal vColumn18 = 0;
                string vColumn19 = string.Empty;

                DateTime vTempStartDateTime = DateTime.MinValue;
                decimal vTempStartingQuantity = 0;
                decimal vTempStartingAmount = 0;
                decimal vTempQuantity = 0;
                decimal vTempSubtotal = 0;
                string vTempCustomerName = string.Empty;
                string vTempVATRegistrationNo = string.Empty;
                string vTempAddress1 = string.Empty;
                string vTempTransID = string.Empty;
                DateTime vTemptransDate = DateTime.MinValue;
                string vTempProductName = string.Empty;
                string vTempProductCode = string.Empty;
                string vFormUOM = string.Empty;
                decimal vTempSDAmount = 0;
                decimal vTempVATAmount = 0;
                string vTempremarks = string.Empty;
                string vTemptransType = string.Empty;

                decimal vClosingQuantity = 0;
                decimal vClosingAmount = 0;
                decimal vClosingAvgRatet = 0;

                decimal OpeningQty = 0;
                decimal OpeningAmnt = 0;
                decimal ProductionQty = 0;
                decimal ProductionAmnt = 0;
                decimal SaleQty = 0;
                decimal SaleAmnt = 0;
                decimal CloseQty = 0;
                decimal CloseAmnt = 0;

                decimal OpQty = 0;
                decimal OpAmnt = 0;
                decimal avgRate = 0;
                string HSCode = string.Empty;
                string ProductName = string.Empty;
                string ProductCode = string.Empty;
                string FormUOM = string.Empty;
                decimal vClosingQuantityNew = 0;
                decimal vClosingAmountNew = 0;
                decimal vUnitRate = 0;
                decimal vAdjustmentValue = 0;


                #endregion

                #region Process Begin

                List<VAT_17> vat17s = new List<VAT_17>();
                VAT_17 vat17 = new VAT_17();

                DataView view = new DataView(ReportResult.Tables[0]);
                DataTable distinctValues = view.ToTable(true, "ItemNo");

                int i = 1;
                if (vm.rbtnWIP == false)
                {

                    //DataRow[] DetailRawsOpening = ReportResult.Tables[1].Select("transType='Opening'");
                    DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("transType='Opening'");

                    #region Opening

                    foreach (DataRow row in DetailRawsOpening)
                    {
                        ProductDAL productDal = new ProductDAL();
                        vTempremarks = row["remarks"].ToString().Trim();
                        vTemptransType = row["TransType"].ToString().Trim();
                        //vTemptransType = row["TransType"].ToString().Trim();
                        ProductName = row["ProductName"].ToString().Trim();
                        ProductCode = row["ProductCode"].ToString().Trim();
                        FormUOM = row["UOM"].ToString().Trim();
                        HSCode = row["HSCodeNo"].ToString().Trim();
                        var tt = row["StartDateTime"].ToString().Trim();
                        vTempStartDateTime = Convert.ToDateTime(row["StartDateTime"].ToString().Trim());


                        decimal LastNBRPrice = productDal.GetLastNBRPriceFromBOM(vm.ItemNo, Convert.ToDateTime(vTempStartDateTime).ToString("yyyy-MMM-dd"), null, null, "0", connVM);

                        //Convert.ToDecimal(Program.FormatingNumeric(dollerRate.ToString(), SalePlaceDollar));

                        decimal q11 = Convert.ToDecimal(row["Quantity"].ToString().Trim());
                        decimal q12 = Convert.ToDecimal(row["UnitCost"].ToString().Trim());
                        //OpQty = TCloseQty;//
                        //OpAmnt = TCloseAmnt;
                        OpQty = q11;//
                        OpAmnt = q12;//
                        vat17 = new VAT_17();

                        #region if row 1 Opening

                        OpeningQty = OpQty;
                        OpeningAmnt = OpAmnt;// OpQty* LastNBRPrice;
                        OpAmnt = 0;
                        OpQty = 0;

                        ProductionQty = 0;
                        ProductionAmnt = 0;
                        SaleQty = 0;
                        SaleAmnt = 0;

                        CloseQty =
                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                             Convert.ToDecimal(SaleQty));
                        CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                     Convert.ToDecimal(SaleAmnt));

                        vColumnDay = Convert.ToString(row["Day"]);
                        vColumn2 = vTempStartDateTime;
                        vColumn3 = OpeningQty;
                        vColumn4 = OpeningAmnt;
                        vColumn5 = 0; // Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                        vColumn6 = 0; // Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                        vColumn7 = "-";
                        vColumn8 = "-";
                        vColumn9 = "-";
                        vColumn10 = "-";
                        vColumn11 = DateTime.MinValue;
                        vColumn12 = "-";
                        vColumn13 = 0; // Convert.ToDecimal(Program.FormatingNumeric(SaleQty.ToString(), SalePlaceQty));
                        vColumn14 = 0; // Convert.ToDecimal(Program.FormatingNumeric(SaleAmnt.ToString(), SalePlaceTaka));
                        vColumn15 = 0;
                        vColumn16 = 0;
                        vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                        vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                        vColumn19 = vTempremarks;
                        vUnitRate = Convert.ToDecimal(row["UnitRate"].ToString().Trim());
                        vClosingQuantityNew = vColumn17;
                        vClosingAmountNew = vColumn18;

                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                            Convert.ToDecimal(SaleQty));

                        if (vClosingQuantity == 0)
                        {
                            vClosingAmount = 0;
                            vClosingAvgRatet = 0;

                        }
                        else
                        {
                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                              Convert.ToDecimal(SaleAmnt));
                            vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                        }

                        #endregion


                        #region AssignValue to List
                        if (nonstock == true)
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn5 = 0;
                            vColumn6 = 0;
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }

                        vat17.Column1 = i; //    i.ToString();      // Serial No   
                        vat17.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                        vat17.Day = vColumnDay; //    item["StartDateTime"].ToString();      // Date
                        vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                        vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                        vat17.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                        vat17.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                        vat17.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                        vat17.Column8 = vColumn8;   //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                        vat17.Column9 = vColumn9;   //    item["Address1"].ToString();      // Customer Address
                        vat17.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                        vat17.Column11 = vColumn11; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                        vat17.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                        vat17.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                        vat17.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                        vat17.Column14 = vColumn14; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                        vat17.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                        vat17.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                        vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                        vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                        vat17.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                        vat17.ProductName = ProductName;
                        vat17.ProductCode = ProductCode;
                        vat17.FormUom = FormUOM;
                        vat17s.Add(vat17);
                        i = i + 1;

                        #endregion AssignValue to List

                    }
                    #endregion Opening
                }


                //DataRow[] DetailRawsOthers = ReportResult.Tables[1].Select("transType<>'Opening'");
                DataRow[] DetailRawsOthers = ReportResult.Tables[0].Select("transType<>'Opening'");
                if (DetailRawsOthers == null || !DetailRawsOthers.Any())
                {
                    return null;
                }
                //string strSort = "StartDateTime ASC, SerialNo ASC";
                string strSort = "StartDateTime ASC, SerialNo ASC";

                DataView dtview = new DataView(DetailRawsOthers.CopyToDataTable());
                dtview.Sort = strSort;
                DataTable dtsorted = dtview.ToTable();

                #endregion

                #region Process Continue

                foreach (DataRow item in dtsorted.Rows)
                {
                    #region Get from Datatable

                    OpeningQty = 0;
                    OpeningAmnt = 0;
                    ProductionQty = 0;
                    ProductionAmnt = 0;
                    SaleQty = 0;
                    SaleAmnt = 0;
                    CloseQty = 0;
                    CloseAmnt = 0;

                    vColumn1 = i;
                    vColumnDay = Convert.ToString(item["Day"]);
                    vTempStartDateTime = Convert.ToDateTime(item["StartDateTime"].ToString()); // Date
                    vTempStartingQuantity = Convert.ToDecimal(item["StartingQuantity"].ToString()); // Opening Quantity
                    vTempStartingAmount = Convert.ToDecimal(item["StartingAmount"].ToString()); // Opening Price
                    vTempQuantity = Convert.ToDecimal(item["Quantity"].ToString()); // Production Quantity
                    vTempSubtotal = Convert.ToDecimal(item["UnitCost"].ToString()); // Production Price
                    vTempCustomerName = item["CustomerName"].ToString(); // Customer Name
                    vTempVATRegistrationNo = item["VATRegistrationNo"].ToString(); // Customer VAT Reg No
                    vTempAddress1 = item["Address1"].ToString(); // Customer Address
                    vTempTransID = item["TransID"].ToString(); // Sale Invoice No
                    vTemptransDate = Convert.ToDateTime(item["StartDateTime"].ToString()); // Sale Invoice Date and Time
                    vTempProductName = item["ProductName"].ToString(); // Sale Product Name
                    vTempSDAmount = Convert.ToDecimal(item["SD"].ToString()); // SD Amount
                    vTempVATAmount = Convert.ToDecimal(item["VATRate"].ToString()); // VAT Amount
                    vTempremarks = item["remarks"].ToString(); // Remarks
                    vTemptransType = item["TransType"].ToString().Trim();
                    vTempProductCode = item["ProductCode"].ToString().Trim();
                    vFormUOM = item["UOM"].ToString().Trim();
                    #region Bureau Condition

                    if (vm.IsBureau == true)
                    {
                        ProductName = item["ProductName"].ToString().Trim();
                        HSCode = item["HSCodeNo"].ToString().Trim();
                    }

                    #endregion

                    #endregion Get from Datatable

                    if (vTemptransType.Trim() == "Sale")
                    {
                        #region Sale Data

                        vat17 = new VAT_17();
                        #region if row 1 Opening

                        if (vTempremarks.Trim() == "ServiceNS" || vTempremarks.Trim() == "ServiceNSImport")
                        {
                            OpeningQty = 0;
                            OpeningAmnt = 0;
                        }
                        else
                        {
                            OpeningQty = OpQty + vClosingQuantity;
                            OpeningAmnt = OpAmnt + vClosingAmount;
                        }


                        OpAmnt = 0;
                        OpQty = 0;

                        ProductionQty = 0;
                        ProductionAmnt = 0;
                        SaleQty = vTempQuantity;
                        if (
                            vTempremarks.Trim() == "TradingTender" || vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender" || vTempremarks.Trim() == "Tender"
                            || vTempremarks.Trim() == "Export"
                            )
                        {
                            SaleAmnt = vTempSubtotal;
                        }
                        else
                        {
                            SaleAmnt = vTempSubtotal;
                        }

                        if (vTempremarks.Trim() == "ExportTradingTender" || vTempremarks.Trim() == "ExportTender" || vTempremarks.Trim() == "Export")
                        {
                        }
                        else
                        {
                            SaleAmnt = Convert.ToDecimal(SaleAmnt.ToString());
                        }


                        if (SaleQty == 0)
                        {
                            avgRate = 0;
                        }
                        else
                        {
                            avgRate = SaleAmnt / SaleQty;
                        }

                        if (vTempremarks.Trim() == "ExportTradingTender" || vTempremarks.Trim() == "ExportTender" || vTempremarks.Trim() == "Export")
                        {
                        }
                        else
                        {
                            avgRate = Convert.ToDecimal(avgRate.ToString());
                        }

                        CloseQty = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) - Convert.ToDecimal(SaleQty));
                        CloseAmnt = CloseQty * avgRate;
                        vColumn2 = vTempStartDateTime;

                        string returnTransType = reportDsdal.GetReturnType(vm.ItemNo, vTempremarks.Trim(), connVM);

                        if (vTempremarks.Trim() == "ServiceNS" || vTempremarks.Trim() == "ExportServiceNS" || returnTransType == "ServiceNS")
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn5 = 0;
                            vColumn6 = 0;
                        }
                        else
                        {
                            vColumn3 = OpeningQty;// vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                            vColumn4 = OpeningAmnt;// vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));

                            vColumn5 = 0;// Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                            vColumn6 = 0;// Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                        }
                        vColumn7 = vTempCustomerName;
                        vColumn8 = vTempVATRegistrationNo;
                        vColumn9 = vTempAddress1;
                        vColumn10 = vTempTransID;
                        vColumn11 = vTemptransDate;
                        vColumn12 = vTempProductName;
                        if (vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Export")
                        {
                            vColumn13 = SaleQty;
                            vColumn14 = SaleAmnt;
                        }
                        else
                        {
                            vColumn13 = Convert.ToDecimal(SaleQty.ToString());
                            vColumn14 = Convert.ToDecimal(SaleAmnt.ToString());
                        }
                        vColumn15 = vTempSDAmount;
                        vColumn16 = vTempVATAmount;
                        if (vTempremarks.Trim() == "ServiceNS"
                            || vTempremarks.Trim() == "ExportServiceNS"
                            || returnTransType == "ServiceNS"
                            )
                        {
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        else
                        {
                            vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                            vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));

                            //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                            //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                        }
                        vColumn19 = vTempremarks;

                        if (vTempremarks.ToLower() == "Other".ToLower())
                        {
                            vColumn19 = "Sale (Local)";
                        }



                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                            Convert.ToDecimal(SaleQty));

                        vClosingQuantityNew = vColumn17;
                        vClosingAmountNew = vColumn18;
                        vUnitRate = Convert.ToDecimal(item["UnitRate"].ToString().Trim());

                        if (vClosingQuantity == 0)
                        {
                            //Change at 29-04-14
                            //vClosingAmount = 0;
                            //vClosingAvgRatet = 0;
                            vClosingAmount = CloseAmnt;

                        }
                        else
                        {
                            //vClosingAmount = vClosingQuantity * avgRate;
                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                            if (vTempremarks.Trim() == "TradingTender"
                            || vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Tender"
                            || vTempremarks.Trim() == "Export"
                            )
                            {

                            }
                            else
                            {
                                vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));
                            }

                        }
                        if (vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Export")
                        {

                        }
                        else
                        {
                            vClosingAvgRatet = Convert.ToDecimal(vClosingAvgRatet.ToString());
                        }
                        #endregion
                        #region AssignValue to List
                        if (nonstock == true)
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn5 = 0;
                            vColumn6 = 0;
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        vat17.Column1 = i; //    i.ToString();      // Serial No   
                        vat17.Day = vColumnDay;
                        vat17.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                        vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                        vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                        vat17.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                        vat17.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                        vat17.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                        vat17.Column8 = vColumn8;//    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                        vat17.Column9 = vColumn9; //    item["Address1"].ToString();      // Customer Address
                        vat17.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                        vat17.Column11 = vColumn11;//    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                        vat17.Column11string = Convert.ToDateTime(vColumn11).ToString("dd/MMM/yy HH:mm"); //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                        vat17.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                        vat17.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                        vat17.Column14 = vColumn14;//    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                        vat17.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                        vat17.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                        vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                        vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                        vat17.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                        if (vColumn18 != vColumn17 * vUnitRate)
                        {
                            //AutoAdjustment = true;
                            //vAdjustmentValue = vColumn18 - (vColumn17 * vUnitRate);
                            vAdjustmentValue = (vColumn17 * vUnitRate) - vColumn18;
                        }
                        vat17.ProductName = vTempProductName;
                        vat17.ProductCode = vTempProductCode;
                        vat17.FormUom = vFormUOM;
                        vat17s.Add(vat17);
                        i = i + 1;

                        #endregion AssignValue to List

                        //// Service related company has no need auto adjustment
                        if (vm.IsBureau == false)
                        {
                            if (avgRate == vClosingAvgRatet)
                            {
                                //vat17s.Add(vat17);
                            }
                            if (vm.AutoAdjustment == true)
                            {
                                #region SaleAdjustment
                                //if (avgRate != vClosingAvgRatet)
                                if (vColumn18 != vColumn17 * vUnitRate)
                                {

                                    decimal a = 0;
                                    decimal b = 0;
                                    if (vClosingQuantity == 0)
                                    {
                                        a = avgRate;          //1950
                                        b = vClosingAvgRatet; //1350
                                    }
                                    else
                                    {
                                        a = avgRate * vClosingQuantity;           //1950
                                        b = vClosingAvgRatet * vClosingQuantity; //1350

                                    }

                                    decimal c = b - a;// Convert.ToDecimal(Program.FormatingNumeric(b.ToString(),1)) - Convert.ToDecimal(Program.FormatingNumeric(a.ToString(),1));
                                    c = Convert.ToDecimal(c.ToString());
                                    //hide 0 value row
                                    if (c != 0)
                                    {
                                        VAT_17 vat17Adj = new VAT_17();
                                        #region if row 1 Opening

                                        OpeningQty = OpQty + vClosingQuantity;
                                        OpeningAmnt = OpAmnt + vClosingAmount;
                                        OpAmnt = 0;
                                        OpQty = 0;

                                        ProductionQty = 0;
                                        ProductionAmnt = 0;
                                        SaleQty = 0;
                                        if (vTempremarks.Trim() == "TradingTender"
                                   || vTempremarks.Trim() == "ExportTradingTender"
                                   || vTempremarks.Trim() == "ExportTender"
                                   || vTempremarks.Trim() == "Tender"
                                            || vTempremarks.Trim() == "Export"
                                   )
                                        {
                                            if (vClosingQuantity == 0)
                                            {

                                                //SaleAmnt = (avgRate - vClosingAvgRatet) * vTempQuantity;
                                                SaleAmnt = (vClosingAvgRatet - avgRate) * vTempQuantity;
                                            }
                                            else
                                            {
                                                SaleAmnt = (vTempQuantity * vClosingAvgRatet) - vTempSubtotal;

                                            }
                                        }
                                        else
                                        {
                                            //SaleAmnt=(avgRate* vClosingQuantity)-(vClosingAvgRatet * vClosingQuantity);  
                                            if (vClosingQuantity == 0)
                                            {
                                                SaleAmnt = c * SaleQty;
                                            }
                                            else
                                            {
                                                SaleAmnt = c;
                                            }
                                        }
                                        if (vTempremarks.Trim() == "ExportTradingTender"
                                                || vTempremarks.Trim() == "ExportTender"
                                                || vTempremarks.Trim() == "Export"
                                        )
                                        {

                                        }
                                        else
                                        {
                                            SaleAmnt = Convert.ToDecimal(SaleAmnt.ToString());
                                        }


                                        //SaleAmnt = c;

                                        if (SaleQty == 0)
                                        {
                                            avgRate = 0;
                                        }
                                        else
                                        {
                                            avgRate = SaleAmnt / SaleQty;
                                        }
                                        if (vTempremarks.Trim() == "ExportTradingTender"
                                             || vTempremarks.Trim() == "ExportTender"
                                             || vTempremarks.Trim() == "Export")
                                        {

                                        }
                                        else
                                        {
                                            avgRate = Convert.ToDecimal(avgRate.ToString());
                                        }
                                        CloseQty =
                                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                             Convert.ToDecimal(SaleQty));

                                        CloseAmnt = CloseQty * avgRate;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                        vColumn2 = vTempStartDateTime;

                                        vColumn3 = vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                                        vColumn4 = vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                                        vColumn5 = 0;// Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                                        vColumn6 = 0;//Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                                        vColumn7 = vTempCustomerName;
                                        vColumn8 = vTempVATRegistrationNo;
                                        vColumn9 = vTempAddress1;
                                        vColumn10 = vTempTransID;
                                        vColumn11 = vTemptransDate;
                                        vColumn12 = vTempProductName;
                                        if (vTempremarks.Trim() == "ExportTradingTender"
                                            || vTempremarks.Trim() == "ExportTender"
                                            || vTempremarks.Trim() == "Export")
                                        {
                                            vColumn13 = 0;
                                            vColumn14 = vAdjustmentValue;//;
                                        }
                                        else
                                        {
                                            vColumn13 = 0;// Convert.ToDecimal(Program.FormatingNumeric(SaleQty.ToString(), SalePlaceQty));
                                            vColumn14 = Convert.ToDecimal(vAdjustmentValue.ToString());
                                        }
                                        vColumn15 = 0;
                                        vColumn16 = 0;
                                        vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                        vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));

                                        vClosingQuantityNew = vColumn17;
                                        vClosingAmountNew = vColumn18;

                                        //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                        //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                        vColumn19 = "SaleAdjustment";
                                        //vClosingAmount = vClosingQuantity * avgRate;

                                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) - Convert.ToDecimal(SaleQty));
                                        if (vClosingQuantity == 0)
                                        {
                                            vClosingAmount = 0;
                                            vClosingAvgRatet = 0;
                                        }
                                        else
                                        {
                                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                                              Convert.ToDecimal(SaleAmnt));
                                            vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));
                                        }

                                        #endregion
                                        #region AssignValue to List
                                        if (nonstock == true)
                                        {
                                            vColumn3 = 0;
                                            vColumn4 = 0;
                                            vColumn5 = 0;
                                            vColumn6 = 0;
                                            vColumn17 = 0;
                                            vColumn18 = 0;
                                        }
                                        vat17Adj.Column1 = i; //    i.ToString();      // Serial No   
                                        vat17Adj.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                                        vat17Adj.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                        vat17Adj.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                                        vat17Adj.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                                        vat17Adj.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                                        vat17Adj.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                                        vat17Adj.Column8 = vColumn8;//    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                                        vat17Adj.Column9 = vColumn9; //    item["Address1"].ToString();      // Customer Address
                                        vat17Adj.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                                        vat17Adj.Column11 = vColumn11;//    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                        vat17Adj.Column11string = Convert.ToDateTime(vColumn11).ToString("dd/MMM/yy HH:mm"); //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                        vat17Adj.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                                        vat17Adj.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                                        vat17Adj.Column14 = vColumn14;//    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                                        vat17Adj.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                                        vat17Adj.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount


                                        vat17Adj.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                                        vat17Adj.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                                        vat17Adj.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                                        vat17.ProductName = vTempProductName;
                                        vat17.ProductCode = vTempProductCode;
                                        vat17.FormUom = vFormUOM;

                                        //vat17.Column18 = vat17.Column18 + vat17Adj.Column6;
                                        //vat17s.Add(vat17);

                                        vat17Adj.Column4 = vat17.Column18;
                                        vat17s.Add(vat17Adj);
                                        //AutoAdjustment = false;

                                        i = i + 1;


                                        #endregion AssignValue to List
                                    }
                                }
                                #endregion SaleAdjustment
                            }

                        }

                        #endregion
                    }
                    else if (vTemptransType == "Receive")
                    {
                        #region Receive Data

                        if (IsRaw.ToLower() == "service(nonstock)" || IsRaw.ToLower() == "overhead" || IsRaw.ToLower() == "noninventory")
                        {
                            continue;
                        }

                        vat17 = new VAT_17();

                        #region if row 1 Opening

                        OpeningQty = OpQty + vClosingQuantity;
                        OpeningAmnt = OpAmnt + vClosingAmount;
                        OpAmnt = 0;
                        OpQty = 0;

                        ProductionQty = vTempQuantity;
                        ProductionAmnt = vTempSubtotal;
                        SaleQty = 0;
                        SaleAmnt = 0;
                        if (ProductionQty == 0)
                        {
                            avgRate = 0;
                        }
                        else
                        {
                            avgRate = ProductionAmnt / ProductionQty;

                        }
                        if (vTempremarks.Trim() == "ExportTradingTender"
                                   || vTempremarks.Trim() == "ExportTender"
                                   || vTempremarks.Trim() == "Export")
                        {

                        }
                        else
                        {
                            avgRate = Convert.ToDecimal(avgRate.ToString());
                        }
                        CloseQty =
                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                             Convert.ToDecimal(SaleQty));
                        CloseAmnt = CloseQty * avgRate;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                        vColumn2 = vTempStartDateTime;
                        vColumn3 = vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                        vColumn4 = vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                        vColumn5 = Convert.ToDecimal(ProductionQty.ToString());
                        vColumn6 = Convert.ToDecimal(ProductionAmnt.ToString());
                        vColumn7 = "-";
                        vColumn8 = "-";
                        vColumn9 = "-";
                        vColumn10 = "-";
                        vColumn11 = Convert.ToDateTime("1900/01/01");
                        vColumn12 = "-";
                        vColumn13 = 0;
                        vColumn14 = 0;
                        vColumn15 = 0;
                        vColumn16 = 0;
                        vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                        vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                        vClosingQuantityNew = vColumn17;
                        vClosingAmountNew = vColumn18;
                        vUnitRate = Convert.ToDecimal(item["UnitRate"].ToString().Trim());
                        //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                        //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                        vColumn19 = vTempremarks;

                        if (vTempremarks.ToLower() == "Other".ToLower())
                        {
                            vColumn19 = "Receive";
                        }

                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                            Convert.ToDecimal(SaleQty));
                        if (vClosingQuantity == 0)
                        {
                            vClosingAmount = 0;
                            vClosingAvgRatet = 0;

                        }
                        else
                        {
                            //vClosingAmount = vClosingQuantity * avgRate;

                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                            if (vTempremarks.Trim() == "TradingTender"
                           || vTempremarks.Trim() == "ExportTradingTender"
                           || vTempremarks.Trim() == "ExportTender"
                           || vTempremarks.Trim() == "Tender"
                           || vTempremarks.Trim() == "Export"
                           )
                            {
                                // change at 20150324 for Nita requierment
                                vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) /
                                                     Convert.ToDecimal(vClosingQuantity));
                            }
                            else
                            {
                                vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) /
                                                    Convert.ToDecimal(vClosingQuantity));
                            }

                        }
                        if (vTempremarks.Trim() == "ExportTradingTender"
                                    || vTempremarks.Trim() == "ExportTender"
                                    || vTempremarks.Trim() == "Export")
                        {

                        }
                        else
                        {
                            vClosingAvgRatet = Convert.ToDecimal(vClosingAvgRatet.ToString());
                        }
                        #endregion

                        #region AssignValue to List
                        if (nonstock == true)
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn5 = 0;
                            vColumn6 = 0;
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        vat17.Column1 = i; //    i.ToString();      // Serial No   
                        vat17.Day = vColumnDay;
                        vat17.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                        vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                        vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                        vat17.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                        vat17.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                        vat17.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                        vat17.Column8 = vColumn8;   //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                        vat17.Column9 = vColumn9;   //    item["Address1"].ToString();      // Customer Address
                        vat17.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                        vat17.Column11 = vColumn11; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                        vat17.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                        vat17.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                        vat17.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                        vat17.Column14 = vColumn14; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                        vat17.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                        vat17.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                        vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                        vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                        vat17.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                        vat17.ProductName = vTempProductName;
                        vat17.ProductCode = vTempProductCode;
                        vat17.FormUom = vFormUOM;

                        vat17s.Add(vat17);
                        i = i + 1;
                        if (vColumn18 != vColumn17 * vUnitRate)
                        {
                            //AutoAdjustment = true;
                            vAdjustmentValue = (vColumn17 * vUnitRate) - vColumn18;
                        }

                        #endregion AssignValue to List

                        //if (avgRate == vClosingAvgRatet)
                        //{
                        //    vat17s.Add(vat17);
                        //}
                        if (vm.AutoAdjustment == true)
                        {
                            #region ProductionAdjustment


                            //if (avgRate != vClosingAvgRatet)
                            if (vColumn18 != vColumn17 * vUnitRate)
                            {
                                //vColumn4
                                //vClosingAvgRatet = vColumn4 / vColumn3;
                                //decimal x = vColumn4 / vColumn3;
                                decimal a = avgRate * vClosingQuantity;         //7200
                                decimal b = vClosingAvgRatet * vClosingQuantity;//7300
                                decimal c = a - b;
                                //  b = x * vClosingQuantity;//7300
                                //c = a - b;
                                if (vTempremarks.Trim() == "TradingTender"
                              || vTempremarks.Trim() == "ExportTradingTender"
                              || vTempremarks.Trim() == "ExportTender"
                              || vTempremarks.Trim() == "Tender"
                              || vTempremarks.Trim() == "Export"
                              )
                                {
                                    c = (vClosingAvgRatet - avgRate) * ProductionQty;
                                }
                                if (vTempremarks.Trim() == "ExportTradingTender"
                                       || vTempremarks.Trim() == "ExportTender"
                                       || vTempremarks.Trim() == "Export")
                                {

                                }
                                else
                                {
                                    c = Convert.ToDecimal(c.ToString());
                                }
                                //hide 0 value row
                                if (c != 0)
                                {
                                    VAT_17 vat17Adj = new VAT_17();

                                    #region if row 1 Opening

                                    OpeningQty = OpQty + vClosingQuantity;
                                    OpeningAmnt = OpAmnt + vClosingAmount;
                                    OpAmnt = 0;
                                    OpQty = 0;

                                    ProductionQty = 0;
                                    ProductionAmnt = c;
                                    SaleQty = 0;
                                    SaleAmnt = 0;
                                    CloseQty =
                                        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                         Convert.ToDecimal(SaleQty));
                                    CloseAmnt = avgRate * vClosingQuantity;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                    vColumn2 = vTempStartDateTime;
                                    vColumn3 = vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                                    vColumn4 = vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                                    vColumn5 = Convert.ToDecimal(ProductionQty.ToString());
                                    vColumn6 = vAdjustmentValue;// vColumn18 - vColumn4;// Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                                    vColumn7 = "-";
                                    vColumn8 = "-";
                                    vColumn9 = "-";
                                    vColumn10 = "-";
                                    vColumn11 = Convert.ToDateTime("1900/01/01");
                                    vColumn12 = "-";
                                    vColumn13 = 0;
                                    vColumn14 = 0;
                                    vColumn15 = 0;
                                    vColumn16 = 0;
                                    vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                    vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                    vClosingQuantityNew = vColumn17;
                                    vClosingAmountNew = vColumn18;
                                    //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                    //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                    vColumn19 = "ProductionAdjustment";

                                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                                        Convert.ToDecimal(SaleQty));
                                    if (vClosingQuantity == 0)
                                    {
                                        vClosingAmount = 0;
                                        vClosingAvgRatet = 0;

                                    }
                                    else
                                    {
                                        //vClosingAmount = vClosingQuantity * avgRate;
                                        vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                        vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                                    }

                                    #endregion

                                    #region AssignValue to List
                                    if (nonstock == true)
                                    {
                                        vColumn3 = 0;
                                        vColumn4 = 0;
                                        vColumn5 = 0;
                                        vColumn6 = 0;
                                        vColumn17 = 0;
                                        vColumn18 = 0;
                                    }
                                    vat17Adj.Column1 = i; //    i.ToString();      // Serial No   
                                    vat17Adj.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                                    vat17Adj.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                    vat17Adj.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                                    vat17Adj.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                                    vat17Adj.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                                    vat17Adj.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                                    vat17Adj.Column8 = vColumn8;
                                    //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                                    vat17Adj.Column9 = vColumn9; //    item["Address1"].ToString();      // Customer Address
                                    vat17Adj.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                                    vat17Adj.Column11 = vColumn11;//    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                    vat17Adj.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                    vat17Adj.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                                    vat17Adj.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                                    vat17Adj.Column14 = vColumn14;
                                    //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                                    vat17Adj.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                                    vat17Adj.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                                    vat17Adj.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                                    vat17Adj.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                                    vat17Adj.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                                    vat17.ProductName = vTempProductName;
                                    vat17.ProductCode = vTempProductCode;
                                    vat17.FormUom = vFormUOM;


                                    //vat17.Column18 = vat17.Column18 + vat17Adj.Column6;
                                    //vat17s.Add(vat17);
                                    //vat17Adj.Column4 = vat17.Column18;
                                    vat17s.Add(vat17Adj);
                                    i = i + 1;

                                    #endregion AssignValue to List
                                }
                            }
                            #endregion ProductionAdjustment
                        }

                        #endregion
                    }

                }

                #endregion

                #region Report preview
                CommonDAL commonDal = new CommonDAL();
                string v3Digits = commonDal.settingsDesktop("VAT6_2", "Report3Digits", settingsDt, connVM);
                bool Permanent6_2 = commonDal.settings("VAT6_2", "6_2Permanent", null, null, connVM) == "Y";


                //bool InEnglish = (commonDal.settingsDesktop("Reports", "VAT6_2English") == "Y" ? true : false);

                if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                {
                    if (vm.InEnglish == "Y")
                    {
                        objrpt = new ReportDocument();
                        objrpt = new RptVAT6_2_English();
                        ////objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_2_English.rpt");
                    }
                    else
                    {
                        objrpt = new ReportDocument();
                        if (Permanent6_2)
                        {
                            //objrpt = new RptVAT6_2_permanet();
                            objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_2_permanet.rpt");

                        }
                        else
                        {
                            objrpt = new RptVAT6_2();

                        }


                    }


                }

                //}


                if (vm.PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    //objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                if (vm.rbtnService)
                {
                    //objrpt.DataDefinition.FormulaFields["TransactionType"].Text = "'Service'";

                }
                else
                {
                    //  objrpt.DataDefinition.FormulaFields["TransactionType"].Text = "'Others'";

                }
                if (TollProduct)
                {
                    objrpt.DataDefinition.FormulaFields["IsToll"].Text = "'Y'";

                }


                #region Set DataSource


                objrpt.SetDataSource(vat17s);

                #endregion

                #region Formula Fields

                CommonDAL _cDal = new CommonDAL();
                string Quantity6_2 = _cDal.settingsDesktop("DecimalPlace", "Quantity6_2", settingsDt, connVM);
                string Amount6_2 = _cDal.settingsDesktop("DecimalPlace", "Amount6_2", settingsDt, connVM);

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;

                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Month", Convert.ToDateTime(vm.StartDate).ToString("MMMM-yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsMonthly", vm.IsMonthly ? "Y" : "N");

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOMConversion", UOMConversion);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOM", vm.UOM);


                objrpt.DataDefinition.FormulaFields["Quantity6_2"].Text = "'" + Quantity6_2 + "'";
                objrpt.DataDefinition.FormulaFields["Amount6_2"].Text = "'" + Amount6_2 + "'";




                objrpt.DataDefinition.FormulaFields["HSCode"].Text = "'" + HSCode + "'";
                objrpt.DataDefinition.FormulaFields["ProductName"].Text = "'" + ProductName + "'";
                string ProductDescription = vProductVM.ProductDescription;// _pDal.ProductDTByItemNo(txtItemNo.Text.Trim()).Rows[0]["ProductDescription"].ToString();
                objrpt.DataDefinition.FormulaFields["ProductDescription"].Text = "'" + ProductDescription + "'";
                // //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                // //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                // //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                // objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                // objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                // //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";

                #endregion

                #endregion Report preview


                #endregion

                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument VAT6_2(VAT6_2ParamVM vm, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            string FromDate = vm.StartDate;
            string ToDate = vm.EndDate;
            string IsRaw = "";
            bool TollProduct = false;
            string HSCode = string.Empty;
            DataTable ReportData = new DataTable();
            ReportDocument objrpt = new ReportDocument();
            DataSet ReportResult = new DataSet();
            ReportDSDAL reportDsdal = new ReportDSDAL();
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable settingsDt = new DataTable();
            List<VAT_17> vat17s = new List<VAT_17>();
            VAT_17 vat17 = new VAT_17();
            ProductVM vProductVM = new ProductVM();
            string ProductName = string.Empty;
            string ProductCode = string.Empty;
            #endregion

            try
            {
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();
                CommonDAL commonDal = new CommonDAL();
                string v3Digits = commonDal.settingsDesktop("VAT6_2", "Report3Digits", settingsDt, connVM);
                string FromDataTable = commonDal.settingsDesktop("VAT6_2", "FromDataTable", settingsDt, connVM);
                #endregion

                #region UOM Conversion

                ////string UOMConversion = Convert.ToString(vm.UOMConversion);

                string UOMConversion = "";
                UOMConversion = Convert.ToString(vm.UOMConversion > 0 ? vm.UOMConversion : 1);
                bool Permanent6_2 = true; //new CommonDAL().settings("VAT6_2", "6_2Permanent", null, null, connVM) == "Y";

                #endregion

                #region Data Call

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                ReportResult = new DataSet();


                if (vm.IsBureau == true)
                {
                    ReportResult = reportDsdal.BureauVAT6_1Report(vm.ItemNo, vm.StartDate, vm.EndDate, vm.Post1, vm.Post2, connVM);

                }
                else
                {
                    if (vm.IsMonthly)
                    {

                        ReportResult = reportDsdal.VAT6_2_Permanent_DayWise(vm, null, null, connVM);
                    }
                    else
                    {
                        FileLogger.Log("VAT6_2", "VAT6_2 Call Start", DateTime.Now.ToString());

                        ReportResult = _vatRegistersDAL.VAT6_2(vm, null, null, connVM);
                        FileLogger.Log("VAT6_2", "VAT6_2 Call End", DateTime.Now.ToString());

                    }

                }


                //if (vm.IsMonthly)
                //{
                //    if (ReportResult != null && ReportResult.Tables.Count > 0)
                //    {
                //        if (ReportResult.Tables[0] != null && ReportResult.Tables[0].Rows.Count > 0)
                //        {
                //            ReportResult = reportDsdal.VAT6_2_Monthly(vm, connVM);
                //        }

                //    }
                //}
                if (FromDataTable.ToLower() == "y")
                {
                    ReportData = ReportResult.Tables[0].Copy();
                    ReportData.TableName = "dsVAT6_2Permanent";
                }
                else
                {
                    DataRow[] vReportResult = ReportResult.Tables[0].Select("");
                    if (vReportResult != null && vReportResult.Any())
                    {

                        //string strSort = "StartDateTime ASC, SerialNo ASC";
                        string strSort = " ItemNo, StartDateTime, SerialNo,TransID";

                        DataView Vdtview = new DataView(vReportResult.CopyToDataTable());
                        Vdtview.Sort = strSort;
                        ReportResult = new DataSet();
                        DataTable Vtsorted = Vdtview.ToTable();
                        Vtsorted.TableName = "Table1";
                        ReportResult.Tables.Add(Vtsorted);

                    }

                #endregion

                    #region Column Add

                    if (!ReportResult.Tables[0].Columns.Contains("Day"))
                    {
                        ReportResult.Tables[0].Columns.Add("Day");
                    }

                    #endregion


                    #region Report Generate

                    #region CheckPoint

                    if (ReportResult.Tables.Count <= 0)
                    {
                        return null;
                    }

                    #region Product Call


                    vProductVM = new ProductDAL().SelectAll(vm.ItemNo, null, null, null, null, null, connVM, null).FirstOrDefault();

                    if (vProductVM == null)
                    {
                        IsRaw = vProductVM.ProductType.ToLower();

                    }

                    IsRaw = vProductVM.ProductType.ToLower();

                    bool nonstock = false;

                    if (IsRaw.ToLower() == "service(nonstock)" || IsRaw.ToLower() == "overhead" || IsRaw.ToLower() == "noninventory")
                    {
                        nonstock = true;
                    }

                    #endregion


                    if (ReportResult.Tables.Count <= 0)
                    {
                        return null;
                    }

                    #endregion

                    #region Variables

                    decimal vColumn1 = 0;
                    string vColumnDay = "";

                    DateTime vColumn2 = DateTime.MinValue;
                    decimal vColumn3 = 0;
                    decimal vColumn4 = 0;
                    decimal vColumn5 = 0;
                    decimal vColumn6 = 0;
                    string vColumn7 = string.Empty;
                    string vColumn8 = string.Empty;
                    string vColumn9 = string.Empty;
                    string vColumn10 = string.Empty;
                    DateTime vColumn11 = DateTime.MinValue;
                    string vColumn12 = string.Empty;
                    decimal vColumn13 = 0;
                    decimal vColumn14 = 0;
                    decimal vColumn15 = 0;
                    decimal vColumn16 = 0;
                    decimal vColumn17 = 0;
                    decimal vColumn18 = 0;
                    decimal vQuantity = 0;
                    decimal vUnitCost = 0;
                    string vColumn19 = string.Empty;

                    DateTime vTempStartDateTime = DateTime.MinValue;
                    decimal vTempStartingQuantity = 0;
                    decimal vTempStartingAmount = 0;
                    decimal vTempQuantity = 0;
                    decimal vTempSubtotal = 0;
                    string vTempCustomerName = string.Empty;
                    string vTempVATRegistrationNo = string.Empty;
                    string vTempAddress1 = string.Empty;
                    string vTempTransID = string.Empty;
                    DateTime vTemptransDate = DateTime.MinValue;
                    string vTempProductName = string.Empty;
                    string vTempProductCode = string.Empty;
                    string vFormUOM = string.Empty;
                    string ReturnTransactionType = string.Empty;
                    decimal vTempSDAmount = 0;
                    decimal vTempVATAmount = 0;
                    string vTempremarks = string.Empty;
                    string vTemptransType = string.Empty;

                    decimal vClosingQuantity = 0;
                    decimal vClosingAmount = 0;
                    decimal vClosingAvgRatet = 0;

                    decimal OpeningQty = 0;
                    decimal OpeningAmnt = 0;
                    decimal ProductionQty = 0;
                    decimal ProductionAmnt = 0;
                    decimal SaleQty = 0;
                    decimal SaleAmnt = 0;
                    decimal CloseQty = 0;
                    decimal CloseAmnt = 0;

                    decimal OpQty = 0;
                    decimal OpAmnt = 0;
                    decimal avgRate = 0;

                    string FormUOM = string.Empty;
                    decimal vClosingQuantityNew = 0;
                    decimal vClosingAmountNew = 0;
                    decimal vUnitRate = 0;
                    decimal vAdjustmentValue = 0;

                    decimal vRunningOpeningValueFinal = 0;
                    decimal vRunningOpeningQuantityFinal = 0;
                    decimal vRunningTotal = 0;
                    decimal vRunningTotalValueFinal = 0;

                    #endregion

                    #region Process Begin

                    vat17s = new List<VAT_17>();
                    vat17 = new VAT_17();

                    DataView view = new DataView(ReportResult.Tables[0]);
                    DataTable distinctValues = view.ToTable(true, "ItemNo");

                    int i = 1;
                    if (vm.rbtnWIP == false)
                    {

                        //DataRow[] DetailRawsOpening = ReportResult.Tables[1].Select("transType='Opening'");
                        //DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("SettingGroup='Opening'");
                        DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("transType='Opening'");

                        #region Opening

                        foreach (DataRow row in DetailRawsOpening)
                        {
                            ProductDAL productDal = new ProductDAL();
                            vTempremarks = row["remarks"].ToString().Trim();
                            vTemptransType = row["TransType"].ToString().Trim();
                            //vTemptransType = row["TransType"].ToString().Trim();
                            ProductName = row["ProductName"].ToString().Trim();
                            ProductCode = row["ProductCode"].ToString().Trim();
                            FormUOM = row["UOM"].ToString().Trim();
                            HSCode = row["HSCodeNo"].ToString().Trim();
                            var tt = row["StartDateTime"].ToString().Trim();
                            vTempStartDateTime = Convert.ToDateTime(row["StartDateTime"].ToString().Trim());
                            vAdjustmentValue = 0;//Convert.ToDecimal(row["AdjustmentValue"].ToString().Trim());


                            //decimal LastNBRPrice = productDal.GetLastNBRPriceFromBOM(vm.ItemNo, Convert.ToDateTime(vTempStartDateTime).ToString("yyyy-MMM-dd"), null, null);

                            //Convert.ToDecimal(Program.FormatingNumeric(dollerRate.ToString(), SalePlaceDollar));

                            vQuantity = Convert.ToDecimal(row["Quantity"].ToString().Trim());
                            vUnitCost = Convert.ToDecimal(row["UnitCost"].ToString().Trim());

                            if (Permanent6_2)
                            {
                                if (nonstock == false)
                                {
                                    vRunningTotal = Convert.ToDecimal(row["RunningTotal"].ToString().Trim());
                                    vRunningTotalValueFinal = Convert.ToDecimal(row["RunningTotalValueFinal"].ToString().Trim());
                                }
                                //vRunningOpeningValueFinal = vRunningTotalValueFinal;// Convert.ToDecimal(row["RunningOpeningValueFinal"].ToString().Trim());
                                //vRunningOpeningQuantityFinal = vRunningTotal;// Convert.ToDecimal(row["RunningOpeningQuantityFinal"].ToString().Trim());
                                //vQuantity = vRunningOpeningQuantityFinal;//
                                //vUnitCost = vRunningOpeningValueFinal;//


                            }


                            OpQty = vQuantity;//
                            OpAmnt = vUnitCost;//
                            vat17 = new VAT_17();

                            #region if row 1 Opening


                            vColumnDay = Convert.ToString(row["Day"]);
                            vColumn2 = vTempStartDateTime;
                            //vColumn3 = OpeningQty;
                            //vColumn4 = OpeningAmnt;
                            vColumn5 = 0; // Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                            vColumn6 = 0; // Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                            vColumn7 = "-";
                            vColumn8 = "-";
                            vColumn9 = "-";
                            vColumn10 = "-";
                            vColumn11 = DateTime.MinValue;
                            vColumn12 = "-";
                            vColumn13 = 0; // Convert.ToDecimal(Program.FormatingNumeric(SaleQty.ToString(), SalePlaceQty));
                            vColumn14 = 0; // Convert.ToDecimal(Program.FormatingNumeric(SaleAmnt.ToString(), SalePlaceTaka));
                            vColumn15 = 0;
                            vColumn16 = 0;
                            //vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                            //vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                            vColumn19 = vTempremarks;
                            vUnitRate = Convert.ToDecimal(row["UnitRate"].ToString().Trim());


                            #endregion


                            #region AssignValue to List

                            if (Permanent6_2)
                            {
                                vColumn3 = vRunningTotal;
                                vColumn4 = vRunningTotalValueFinal;
                                vColumn17 = vRunningTotal;
                                vColumn18 = vRunningTotalValueFinal;

                                vRunningOpeningQuantityFinal = vRunningTotal;
                                vRunningOpeningValueFinal = vRunningTotalValueFinal;
                            }
                            if (nonstock == true)
                            {
                                vColumn3 = 0;
                                vColumn4 = 0;
                                vColumn5 = 0;
                                vColumn6 = 0;
                                vColumn17 = 0;
                                vColumn18 = 0;
                            }

                            vat17.Column1 = i; //    i.ToString();      // Serial No   
                            vat17.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                            vat17.Day = vColumnDay; //    item["StartDateTime"].ToString();      // Date
                            vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                            vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                            vat17.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                            vat17.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                            vat17.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                            vat17.Column8 = vColumn8;   //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                            vat17.Column9 = vColumn9;   //    item["Address1"].ToString();      // Customer Address
                            vat17.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                            vat17.Column11 = vColumn11; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                            vat17.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                            vat17.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                            vat17.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                            vat17.Column14 = vColumn14; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                            vat17.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                            vat17.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                            vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                            vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                            vat17.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                            vat17.ProductName = ProductName;
                            vat17.ProductCode = ProductCode;
                            vat17.FormUom = FormUOM;

                            vat17.AdjustmentValue = vAdjustmentValue;

                            vat17s.Add(vat17);
                            i = i + 1;

                            #endregion AssignValue to List

                        }
                        #endregion Opening
                    }


                    //DataRow[] DetailRawsOthers = ReportResult.Tables[1].Select("transType<>'Opening'");
                    DataRow[] DetailRawsOthers = ReportResult.Tables[0].Select("transType<>'Opening'");
                    if (DetailRawsOthers != null && DetailRawsOthers.Any())
                    {

                        //string strSort = "StartDateTime ASC, SerialNo ASC";
                        string strSort = " ItemNo, StartDateTime, SerialNo";

                        DataView dtview = new DataView(DetailRawsOthers.CopyToDataTable());
                        dtview.Sort = strSort;
                        DataTable dtsorted = dtview.ToTable();

                    #endregion

                        #region Process Continue

                        foreach (DataRow item in dtsorted.Rows)
                        {
                            #region Get from Datatable

                            OpeningQty = 0;
                            OpeningAmnt = 0;
                            ProductionQty = 0;
                            ProductionAmnt = 0;
                            SaleQty = 0;
                            SaleAmnt = 0;
                            CloseQty = 0;
                            CloseAmnt = 0;

                            vColumn1 = i;
                            vColumnDay = Convert.ToString(item["Day"]);
                            vTempStartDateTime = Convert.ToDateTime(item["StartDateTime"].ToString()); // Date
                            if (nonstock == false)
                            {
                                vTempStartingQuantity = Convert.ToDecimal(item["StartingQuantity"].ToString()); // Opening Quantity
                                vTempStartingAmount = Convert.ToDecimal(item["StartingAmount"].ToString()); // Opening Price
                            }

                            vTempQuantity = Convert.ToDecimal(item["Quantity"].ToString()); // Production Quantity
                            vTempSubtotal = Convert.ToDecimal(item["UnitCost"].ToString()); // Production Price
                            vTempCustomerName = item["CustomerName"].ToString(); // Customer Name
                            vTempVATRegistrationNo = item["VATRegistrationNo"].ToString(); // Customer VAT Reg No
                            vTempAddress1 = item["Address1"].ToString(); // Customer Address
                            vTempTransID = item["TransID"].ToString(); // Sale Invoice No
                            vTemptransDate = Convert.ToDateTime(item["StartDateTime"].ToString()); // Sale Invoice Date and Time
                            vTempProductName = item["ProductName"].ToString(); // Sale Product Name
                            vTempSDAmount = Convert.ToDecimal(item["SD"].ToString()); // SD Amount
                            vTempVATAmount = Convert.ToDecimal(item["VATRate"].ToString()); // VAT Amount
                            vTempremarks = item["remarks"].ToString(); // Remarks
                            vTemptransType = item["TransType"].ToString().Trim();
                            vTempProductCode = item["ProductCode"].ToString().Trim();
                            vFormUOM = item["UOM"].ToString().Trim();
                            ReturnTransactionType = item["ReturnTransactionType"].ToString().Trim();
                            if (Permanent6_2)
                            {
                                if (nonstock == false)
                                {
                                    vRunningTotal = Convert.ToDecimal(item["RunningTotal"].ToString()); // VAT Amount
                                    vRunningTotalValueFinal = Convert.ToDecimal(item["RunningTotalValueFinal"].ToString()); // VAT Amount
                                }
                                //vRunningOpeningValueFinal = Convert.ToDecimal(item["RunningOpeningValueFinal"].ToString()); // VAT Amount
                                //vRunningOpeningQuantityFinal = Convert.ToDecimal(item["RunningOpeningQuantityFinal"].ToString()); // VAT Amount

                            }
                            if (nonstock == false)
                            {
                                vAdjustmentValue = Convert.ToDecimal(item["AdjustmentValue"].ToString().Trim());
                            }

                            #region Bureau Condition

                            if (vm.IsBureau == true)
                            {
                                ProductName = item["ProductName"].ToString().Trim();
                                HSCode = item["HSCodeNo"].ToString().Trim();
                            }

                            #endregion

                            #endregion Get from Datatable

                            if (vTemptransType.Trim() == "Sale")
                            {
                                #region Sale Data

                                vat17 = new VAT_17();
                                #region if row 1 Opening

                                if (vTempremarks.Trim() == "ServiceNS" || vTempremarks.Trim() == "ServiceNSImport")
                                {
                                    OpeningQty = 0;
                                    OpeningAmnt = 0;
                                }
                                //else
                                //{
                                //    OpeningQty = OpQty + vClosingQuantity;
                                //    OpeningAmnt = OpAmnt + vClosingAmount;
                                //}


                                //OpAmnt = 0;
                                //OpQty = 0;

                                ProductionQty = 0;
                                ProductionAmnt = 0;
                                SaleQty = vTempQuantity;
                                if (
                                    vTempremarks.Trim() == "TradingTender" || vTempremarks.Trim() == "ExportTradingTender"
                                    || vTempremarks.Trim() == "ExportTender" || vTempremarks.Trim() == "Tender"
                                    || vTempremarks.Trim() == "Export"
                                    )
                                {
                                    SaleAmnt = vTempSubtotal;
                                }
                                else
                                {
                                    SaleAmnt = vTempSubtotal;
                                }

                                if (vTempremarks.Trim() == "ExportTradingTender" || vTempremarks.Trim() == "ExportTender" || vTempremarks.Trim() == "Export")
                                {
                                }
                                else
                                {
                                    SaleAmnt = Convert.ToDecimal(SaleAmnt.ToString());
                                }


                                //if (SaleQty == 0)
                                //{
                                //    avgRate = 0;
                                //}
                                //else
                                //{
                                //    avgRate = SaleAmnt / SaleQty;
                                //}

                                if (vTempremarks.Trim() == "ExportTradingTender" || vTempremarks.Trim() == "ExportTender" || vTempremarks.Trim() == "Export")
                                {
                                }
                                else
                                {
                                    //avgRate = Convert.ToDecimal(avgRate.ToString());
                                }

                                //CloseQty = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) - Convert.ToDecimal(SaleQty));
                                //CloseAmnt = CloseQty * avgRate;
                                vColumn2 = vTempStartDateTime;

                                string returnTransType = ReturnTransactionType;//reportDsdal.GetReturnType(vm.ItemNo, vTempremarks.Trim(), connVM);

                                if (vTempremarks.Trim() == "ServiceNS" || vTempremarks.Trim() == "ExportServiceNS" || returnTransType == "ServiceNS")
                                {
                                    vColumn3 = 0;
                                    vColumn4 = 0;
                                    vColumn5 = 0;
                                    vColumn6 = 0;
                                }
                                else
                                {
                                    vColumn3 = OpeningQty;// vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                                    vColumn4 = OpeningAmnt;// vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));

                                    vColumn5 = 0;// Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                                    vColumn6 = 0;// Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                                }
                                vColumn7 = vTempCustomerName;
                                vColumn8 = vTempVATRegistrationNo;
                                vColumn9 = vTempAddress1;
                                vColumn10 = vTempTransID;

                                vColumn11 = vTemptransDate;
                                vColumn12 = vTempProductName;
                                if (vTempremarks.Trim() == "ExportTradingTender"
                                    || vTempremarks.Trim() == "ExportTender"
                                    || vTempremarks.Trim() == "Export")
                                {
                                    vColumn13 = SaleQty;
                                    vColumn14 = SaleAmnt;
                                }
                                else
                                {
                                    vColumn13 = Convert.ToDecimal(SaleQty.ToString());
                                    vColumn14 = Convert.ToDecimal(SaleAmnt.ToString());
                                }
                                vColumn15 = vTempSDAmount;
                                vColumn16 = vTempVATAmount;
                                if (vTempremarks.Trim() == "ServiceNS"
                                    || vTempremarks.Trim() == "ExportServiceNS"
                                    || returnTransType == "ServiceNS"
                                    )
                                {
                                    vColumn17 = 0;
                                    vColumn18 = 0;
                                }
                                else
                                {
                                    vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                    vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                }
                                vColumn19 = vTempremarks;

                                if (vTempremarks.ToLower() == "Other".ToLower())
                                {
                                    vColumn19 = "Sale (Local)";
                                }



                                vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                                    Convert.ToDecimal(SaleQty));

                                vClosingQuantityNew = vColumn17;
                                vClosingAmountNew = vColumn18;
                                vUnitRate = Convert.ToDecimal(item["UnitRate"].ToString().Trim());

                                if (vClosingQuantity == 0)
                                {
                                    vClosingAmount = CloseAmnt;

                                }
                                else
                                {
                                    vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                    if (vTempremarks.Trim() == "TradingTender"
                                    || vTempremarks.Trim() == "ExportTradingTender"
                                    || vTempremarks.Trim() == "ExportTender"
                                    || vTempremarks.Trim() == "Tender"
                                    || vTempremarks.Trim() == "Export"
                                    )
                                    {

                                    }
                                    else
                                    {
                                        vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));
                                    }

                                }
                                if (vTempremarks.Trim() == "ExportTradingTender"
                                    || vTempremarks.Trim() == "ExportTender"
                                    || vTempremarks.Trim() == "Export")
                                {

                                }
                                else
                                {
                                    vClosingAvgRatet = Convert.ToDecimal(vClosingAvgRatet.ToString());
                                }
                                #endregion
                                #region AssignValue to List

                                if (Permanent6_2)
                                {
                                    vColumn3 = vRunningOpeningQuantityFinal;
                                    vColumn4 = vRunningOpeningValueFinal;
                                    vColumn17 = vRunningTotal;
                                    vColumn18 = vRunningTotalValueFinal;


                                }

                                if (nonstock == true)
                                {
                                    vColumn3 = 0;
                                    vColumn4 = 0;
                                    vColumn5 = 0;
                                    vColumn6 = 0;
                                    vColumn17 = 0;
                                    vColumn18 = 0;
                                }
                                vat17.Column1 = i; //    i.ToString();      // Serial No   
                                vat17.Day = vColumnDay;
                                vat17.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date

                                vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price

                                vat17.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                                vat17.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                                vat17.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                                vat17.Column8 = vColumn8;//    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                                vat17.Column9 = vColumn9; //    item["Address1"].ToString();      // Customer Address
                                vat17.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                                vat17.Column11 = vColumn11;//    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                vat17.Column11string = Convert.ToDateTime(vColumn11).ToString("dd/MMM/yy HH:mm"); //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                vat17.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                                vat17.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                                vat17.Column14 = vColumn14;//    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                                vat17.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                                vat17.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                                vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                                vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount

                                if (Permanent6_2)
                                {
                                    vRunningOpeningQuantityFinal = vColumn17; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                    vRunningOpeningValueFinal = vColumn18; //    item["StartingAmount"].ToString();      // Opening Price
                                }
                                //else
                                //{
                                //    vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                //    vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                                //    vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                                //    vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                                //}


                                vat17.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks

                                vat17.ProductName = vTempProductName;
                                vat17.ProductCode = vTempProductCode;
                                vat17.FormUom = vFormUOM;
                                vat17.AdjustmentValue = vAdjustmentValue;

                                vat17s.Add(vat17);
                                i = i + 1;

                                #endregion AssignValue to List

                                //// Service related company has no need auto adjustment
                                if (vm.IsBureau == false)
                                {
                                    if (avgRate == vClosingAvgRatet)
                                    {
                                        //vat17s.Add(vat17);
                                    }
                                    if (vm.AutoAdjustment == true)
                                    {
                                        #region SaleAdjustment
                                        //if (avgRate != vClosingAvgRatet)
                                        if (vColumn18 != vColumn17 * vUnitRate)
                                        {

                                            decimal a = 0;
                                            decimal b = 0;
                                            if (vClosingQuantity == 0)
                                            {
                                                a = avgRate;          //1950
                                                b = vClosingAvgRatet; //1350
                                            }
                                            else
                                            {
                                                a = avgRate * vClosingQuantity;           //1950
                                                b = vClosingAvgRatet * vClosingQuantity; //1350

                                            }

                                            decimal c = b - a;// Convert.ToDecimal(Program.FormatingNumeric(b.ToString(),1)) - Convert.ToDecimal(Program.FormatingNumeric(a.ToString(),1));
                                            c = Convert.ToDecimal(c.ToString());
                                            //hide 0 value row
                                            if (c != 0)
                                            {
                                                VAT_17 vat17Adj = new VAT_17();
                                                #region if row 1 Opening

                                                OpeningQty = OpQty + vClosingQuantity;
                                                OpeningAmnt = OpAmnt + vClosingAmount;
                                                OpAmnt = 0;
                                                OpQty = 0;

                                                ProductionQty = 0;
                                                ProductionAmnt = 0;
                                                SaleQty = 0;
                                                if (vTempremarks.Trim() == "TradingTender"
                                           || vTempremarks.Trim() == "ExportTradingTender"
                                           || vTempremarks.Trim() == "ExportTender"
                                           || vTempremarks.Trim() == "Tender"
                                                    || vTempremarks.Trim() == "Export"
                                           )
                                                {
                                                    if (vClosingQuantity == 0)
                                                    {

                                                        //SaleAmnt = (avgRate - vClosingAvgRatet) * vTempQuantity;
                                                        SaleAmnt = (vClosingAvgRatet - avgRate) * vTempQuantity;
                                                    }
                                                    else
                                                    {
                                                        SaleAmnt = (vTempQuantity * vClosingAvgRatet) - vTempSubtotal;

                                                    }
                                                }
                                                else
                                                {
                                                    //SaleAmnt=(avgRate* vClosingQuantity)-(vClosingAvgRatet * vClosingQuantity);  
                                                    if (vClosingQuantity == 0)
                                                    {
                                                        SaleAmnt = c * SaleQty;
                                                    }
                                                    else
                                                    {
                                                        SaleAmnt = c;
                                                    }
                                                }
                                                if (vTempremarks.Trim() == "ExportTradingTender"
                                                        || vTempremarks.Trim() == "ExportTender"
                                                        || vTempremarks.Trim() == "Export"
                                                )
                                                {

                                                }
                                                else
                                                {
                                                    SaleAmnt = Convert.ToDecimal(SaleAmnt.ToString());
                                                }


                                                //SaleAmnt = c;

                                                if (SaleQty == 0)
                                                {
                                                    avgRate = 0;
                                                }
                                                else
                                                {
                                                    avgRate = SaleAmnt / SaleQty;
                                                }
                                                if (vTempremarks.Trim() == "ExportTradingTender"
                                                     || vTempremarks.Trim() == "ExportTender"
                                                     || vTempremarks.Trim() == "Export")
                                                {

                                                }
                                                else
                                                {
                                                    avgRate = Convert.ToDecimal(avgRate.ToString());
                                                }
                                                CloseQty =
                                                    (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                                     Convert.ToDecimal(SaleQty));

                                                CloseAmnt = CloseQty * avgRate;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                                vColumn2 = vTempStartDateTime;

                                                vColumn3 = vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                                                vColumn4 = vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                                                vColumn5 = 0;// Convert.ToDecimal(Program.FormatingNumeric(ProductionQty.ToString(), SalePlaceQty));
                                                vColumn6 = 0;//Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                                                vColumn7 = vTempCustomerName;
                                                vColumn8 = vTempVATRegistrationNo;
                                                vColumn9 = vTempAddress1;
                                                vColumn10 = vTempTransID;
                                                vColumn11 = vTemptransDate;
                                                vColumn12 = vTempProductName;
                                                if (vTempremarks.Trim() == "ExportTradingTender"
                                                    || vTempremarks.Trim() == "ExportTender"
                                                    || vTempremarks.Trim() == "Export")
                                                {
                                                    vColumn13 = 0;
                                                    vColumn14 = vAdjustmentValue;//;
                                                }
                                                else
                                                {
                                                    vColumn13 = 0;// Convert.ToDecimal(Program.FormatingNumeric(SaleQty.ToString(), SalePlaceQty));
                                                    vColumn14 = Convert.ToDecimal(vAdjustmentValue.ToString());
                                                }
                                                vColumn15 = 0;
                                                vColumn16 = 0;
                                                vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                                vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));

                                                vClosingQuantityNew = vColumn17;
                                                vClosingAmountNew = vColumn18;

                                                //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                                //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                                vColumn19 = "SaleAdjustment";
                                                //vClosingAmount = vClosingQuantity * avgRate;

                                                vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) - Convert.ToDecimal(SaleQty));
                                                if (vClosingQuantity == 0)
                                                {
                                                    vClosingAmount = 0;
                                                    vClosingAvgRatet = 0;
                                                }
                                                else
                                                {
                                                    vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                                                      Convert.ToDecimal(SaleAmnt));
                                                    vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));
                                                }

                                                #endregion
                                                #region AssignValue to List
                                                if (nonstock == true)
                                                {
                                                    vColumn3 = 0;
                                                    vColumn4 = 0;
                                                    vColumn5 = 0;
                                                    vColumn6 = 0;
                                                    vColumn17 = 0;
                                                    vColumn18 = 0;
                                                }
                                                vat17Adj.Column1 = i; //    i.ToString();      // Serial No   
                                                vat17Adj.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                                                vat17Adj.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                                vat17Adj.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                                                vat17Adj.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                                                vat17Adj.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                                                vat17Adj.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                                                vat17Adj.Column8 = vColumn8;//    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                                                vat17Adj.Column9 = vColumn9; //    item["Address1"].ToString();      // Customer Address
                                                vat17Adj.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                                                vat17Adj.Column11 = vColumn11;//    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                                vat17Adj.Column11string = Convert.ToDateTime(vColumn11).ToString("dd/MMM/yy HH:mm"); //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                                vat17Adj.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                                                vat17Adj.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                                                vat17Adj.Column14 = vColumn14;//    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                                                vat17Adj.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                                                vat17Adj.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount


                                                vat17Adj.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                                                vat17Adj.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                                                vat17Adj.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                                                vat17Adj.ProductName = vTempProductName;
                                                vat17Adj.ProductCode = vTempProductCode;
                                                vat17Adj.FormUom = vFormUOM;

                                                //vat17.Column18 = vat17.Column18 + vat17Adj.Column6;
                                                //vat17s.Add(vat17);

                                                vat17Adj.Column4 = vat17.Column18;
                                                vat17s.Add(vat17Adj);
                                                //AutoAdjustment = false;

                                                i = i + 1;


                                                #endregion AssignValue to List
                                            }
                                        }
                                        #endregion SaleAdjustment
                                    }

                                }

                                #endregion
                            }
                            else if (vTemptransType == "Receive")
                            {
                                #region Receive Data

                                if (IsRaw.ToLower() == "service(nonstock)" || IsRaw.ToLower() == "overhead" || IsRaw.ToLower() == "noninventory")
                                {
                                    continue;
                                }

                                vat17 = new VAT_17();

                                #region if row 1 Opening

                                OpeningQty = OpQty + vClosingQuantity;
                                OpeningAmnt = OpAmnt + vClosingAmount;
                                OpAmnt = 0;
                                OpQty = 0;

                                ProductionQty = vTempQuantity;
                                ProductionAmnt = vTempSubtotal;
                                SaleQty = 0;
                                SaleAmnt = 0;
                                if (ProductionQty == 0)
                                {
                                    avgRate = 0;
                                }
                                else
                                {
                                    avgRate = ProductionAmnt / ProductionQty;

                                }
                                if (vTempremarks.Trim() == "ExportTradingTender"
                                           || vTempremarks.Trim() == "ExportTender"
                                           || vTempremarks.Trim() == "Export")
                                {

                                }
                                else
                                {
                                    avgRate = Convert.ToDecimal(avgRate.ToString());
                                }
                                CloseQty =
                                    (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                     Convert.ToDecimal(SaleQty));
                                CloseAmnt = CloseQty * avgRate;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                vColumn2 = vTempStartDateTime;
                                vColumn3 = vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                                vColumn4 = vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                                vColumn5 = Convert.ToDecimal(ProductionQty.ToString());
                                vColumn6 = Convert.ToDecimal(ProductionAmnt.ToString());
                                vColumn7 = "-";
                                vColumn8 = "-";
                                vColumn9 = "-";
                                vColumn10 = "-";
                                vColumn11 = Convert.ToDateTime("1900/01/01");
                                vColumn12 = "-";
                                vColumn13 = 0;
                                vColumn14 = 0;
                                vColumn15 = 0;
                                vColumn16 = 0;
                                vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                vClosingQuantityNew = vColumn17;
                                vClosingAmountNew = vColumn18;
                                vUnitRate = Convert.ToDecimal(item["UnitRate"].ToString().Trim());
                                //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                vColumn19 = vTempremarks;

                                if (vTempremarks.ToLower() == "Other".ToLower())
                                {
                                    vColumn19 = "Receive";
                                }

                                vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                                    Convert.ToDecimal(SaleQty));
                                if (vClosingQuantity == 0)
                                {
                                    vClosingAmount = 0;
                                    vClosingAvgRatet = 0;

                                }
                                else
                                {
                                    //vClosingAmount = vClosingQuantity * avgRate;

                                    vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                    if (vTempremarks.Trim() == "TradingTender"
                                   || vTempremarks.Trim() == "ExportTradingTender"
                                   || vTempremarks.Trim() == "ExportTender"
                                   || vTempremarks.Trim() == "Tender"
                                   || vTempremarks.Trim() == "Export"
                                   )
                                    {
                                        // change at 20150324 for Nita requierment
                                        vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) /
                                                             Convert.ToDecimal(vClosingQuantity));
                                    }
                                    else
                                    {
                                        vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) /
                                                            Convert.ToDecimal(vClosingQuantity));
                                    }

                                }
                                if (vTempremarks.Trim() == "ExportTradingTender"
                                            || vTempremarks.Trim() == "ExportTender"
                                            || vTempremarks.Trim() == "Export")
                                {

                                }
                                else
                                {
                                    vClosingAvgRatet = Convert.ToDecimal(vClosingAvgRatet.ToString());
                                }
                                #endregion

                                #region AssignValue to List

                                if (Permanent6_2)
                                {
                                    vColumn3 = vRunningOpeningQuantityFinal;
                                    vColumn4 = vRunningOpeningValueFinal;
                                    vColumn17 = vRunningTotal;
                                    vColumn18 = vRunningTotalValueFinal;


                                }
                                if (nonstock == true)
                                {
                                    vColumn3 = 0;
                                    vColumn4 = 0;
                                    vColumn5 = 0;
                                    vColumn6 = 0;
                                    vColumn17 = 0;
                                    vColumn18 = 0;
                                }
                                vat17.Column1 = i; //    i.ToString();      // Serial No   
                                vat17.Day = vColumnDay;
                                vat17.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                                vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                                vat17.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                                vat17.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                                vat17.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                                vat17.Column8 = vColumn8;   //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                                vat17.Column9 = vColumn9;   //    item["Address1"].ToString();      // Customer Address
                                vat17.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                                vat17.Column11 = vColumn11; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                vat17.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                vat17.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                                vat17.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                                vat17.Column14 = vColumn14; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                                vat17.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                                vat17.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                                vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                                vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                                vat17.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                                vat17.ProductName = vTempProductName;
                                vat17.ProductCode = vTempProductCode;
                                vat17.FormUom = vFormUOM;
                                vat17.AdjustmentValue = vAdjustmentValue;
                                if (Permanent6_2)
                                {
                                    vRunningOpeningQuantityFinal = vColumn17; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                    vRunningOpeningValueFinal = vColumn18; //    item["StartingAmount"].ToString();      // Opening Price
                                }

                                vat17s.Add(vat17);
                                i = i + 1;
                                //if (vColumn18 != vColumn17 * vUnitRate)
                                //{
                                //    //AutoAdjustment = true;
                                //    vAdjustmentValue = (vColumn17 * vUnitRate) - vColumn18;
                                //}

                                #endregion AssignValue to List

                                //if (avgRate == vClosingAvgRatet)
                                //{
                                //    vat17s.Add(vat17);
                                //}
                                if (vm.AutoAdjustment == true)
                                {
                                    #region ProductionAdjustment


                                    //if (avgRate != vClosingAvgRatet)
                                    if (vColumn18 != vColumn17 * vUnitRate)
                                    {
                                        //vColumn4
                                        //vClosingAvgRatet = vColumn4 / vColumn3;
                                        //decimal x = vColumn4 / vColumn3;
                                        decimal a = avgRate * vClosingQuantity;         //7200
                                        decimal b = vClosingAvgRatet * vClosingQuantity;//7300
                                        decimal c = a - b;
                                        //  b = x * vClosingQuantity;//7300
                                        //c = a - b;
                                        if (vTempremarks.Trim() == "TradingTender"
                                      || vTempremarks.Trim() == "ExportTradingTender"
                                      || vTempremarks.Trim() == "ExportTender"
                                      || vTempremarks.Trim() == "Tender"
                                      || vTempremarks.Trim() == "Export"
                                      )
                                        {
                                            c = (vClosingAvgRatet - avgRate) * ProductionQty;
                                        }
                                        if (vTempremarks.Trim() == "ExportTradingTender"
                                               || vTempremarks.Trim() == "ExportTender"
                                               || vTempremarks.Trim() == "Export")
                                        {

                                        }
                                        else
                                        {
                                            c = Convert.ToDecimal(c.ToString());
                                        }
                                        //hide 0 value row
                                        if (c != 0)
                                        {
                                            VAT_17 vat17Adj = new VAT_17();

                                            #region if row 1 Opening

                                            OpeningQty = OpQty + vClosingQuantity;
                                            OpeningAmnt = OpAmnt + vClosingAmount;
                                            OpAmnt = 0;
                                            OpQty = 0;

                                            ProductionQty = 0;
                                            ProductionAmnt = c;
                                            SaleQty = 0;
                                            SaleAmnt = 0;
                                            CloseQty =
                                                (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                                 Convert.ToDecimal(SaleQty));
                                            CloseAmnt = avgRate * vClosingQuantity;// (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                            vColumn2 = vTempStartDateTime;
                                            vColumn3 = vClosingQuantityNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningQty.ToString(), SalePlaceQty));
                                            vColumn4 = vClosingAmountNew;// Convert.ToDecimal(Program.FormatingNumeric(OpeningAmnt.ToString(), SalePlaceTaka));
                                            vColumn5 = Convert.ToDecimal(ProductionQty.ToString());
                                            vColumn6 = vAdjustmentValue;// vColumn18 - vColumn4;// Convert.ToDecimal(Program.FormatingNumeric(ProductionAmnt.ToString(), SalePlaceTaka));
                                            vColumn7 = "-";
                                            vColumn8 = "-";
                                            vColumn9 = "-";
                                            vColumn10 = "-";
                                            vColumn11 = Convert.ToDateTime("1900/01/01");
                                            vColumn12 = "-";
                                            vColumn13 = 0;
                                            vColumn14 = 0;
                                            vColumn15 = 0;
                                            vColumn16 = 0;
                                            vColumn17 = Convert.ToDecimal(vColumn3 + vColumn5 - vColumn13);// Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                            vColumn18 = Convert.ToDecimal(vColumn4 + vColumn6 - vColumn14);// Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                            vClosingQuantityNew = vColumn17;
                                            vClosingAmountNew = vColumn18;
                                            //vColumn17 = Convert.ToDecimal(Program.FormatingNumeric(CloseQty.ToString(), SalePlaceQty));
                                            //vColumn18 = Convert.ToDecimal(Program.FormatingNumeric(CloseAmnt.ToString(), SalePlaceTaka));
                                            vColumn19 = "ProductionAdjustment";

                                            vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                                                Convert.ToDecimal(SaleQty));
                                            if (vClosingQuantity == 0)
                                            {
                                                vClosingAmount = 0;
                                                vClosingAvgRatet = 0;

                                            }
                                            else
                                            {
                                                //vClosingAmount = vClosingQuantity * avgRate;
                                                vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                                vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                                            }

                                            #endregion

                                            #region AssignValue to List
                                            if (nonstock == true)
                                            {
                                                vColumn3 = 0;
                                                vColumn4 = 0;
                                                vColumn5 = 0;
                                                vColumn6 = 0;
                                                vColumn17 = 0;
                                                vColumn18 = 0;
                                            }
                                            vat17Adj.Column1 = i; //    i.ToString();      // Serial No   
                                            vat17Adj.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                                            vat17Adj.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                            vat17Adj.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                                            vat17Adj.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                                            vat17Adj.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                                            vat17Adj.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                                            vat17Adj.Column8 = vColumn8;
                                            //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                                            vat17Adj.Column9 = vColumn9; //    item["Address1"].ToString();      // Customer Address
                                            vat17Adj.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                                            vat17Adj.Column11 = vColumn11;//    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                            vat17Adj.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                            vat17Adj.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                                            vat17Adj.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                                            vat17Adj.Column14 = vColumn14;
                                            //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                                            vat17Adj.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                                            vat17Adj.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                                            vat17Adj.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                                            vat17Adj.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                                            vat17Adj.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks
                                            vat17.ProductName = vTempProductName;
                                            vat17.ProductCode = vTempProductCode;
                                            vat17.FormUom = vFormUOM;
                                            if (Permanent6_2)
                                            {
                                                vat17.Column3 = vRunningOpeningQuantityFinal; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                                vat17.Column4 = vRunningOpeningValueFinal; //    item["StartingAmount"].ToString();      // Opening Price
                                                vat17.Column17 = vRunningTotal; //    item["StartDateTime"].ToString();      // Closing Quantity
                                                vat17.Column18 = vRunningTotalValueFinal; //    item["StartDateTime"].ToString();      // Closing Amount
                                            }
                                            else
                                            {
                                                vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                                vat17.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                                                vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                                                vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                                            }

                                            //vat17.Column18 = vat17.Column18 + vat17Adj.Column6;
                                            //vat17s.Add(vat17);
                                            //vat17Adj.Column4 = vat17.Column18;
                                            vat17s.Add(vat17Adj);
                                            i = i + 1;

                                            #endregion AssignValue to List
                                        }
                                    }
                                    #endregion ProductionAdjustment
                                }

                                #endregion
                            }

                        }
                    }
                        #endregion
                }
                #region Report preview


                if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                {

                    if (vm.InEnglish == "Y")
                    {
                        objrpt = new ReportDocument();
                        objrpt = new RptVAT6_2_English();
                        ////objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_2_English.rpt");
                    }
                    else
                    {
                        objrpt = new ReportDocument();
                        if (Permanent6_2)
                        {
                            if (FromDataTable.ToLower() == "y")
                            {
                                //objrpt = new RptVAT6_2_permanetDT();
                                objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\" + @"RptVAT6_2_permanetDT.rpt");

                            }
                            else
                            {
                                objrpt = new RptVAT6_2_permanet();
                                ////objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_2_permanet.rpt");
                            }
                        }
                        else
                        {
                            objrpt = new RptVAT6_2();

                        }


                    }


                }

                //}


                if (vm.PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    //objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                if (vm.rbtnService)
                {
                    //objrpt.DataDefinition.FormulaFields["TransactionType"].Text = "'Service'";

                }
                else
                {
                    //  objrpt.DataDefinition.FormulaFields["TransactionType"].Text = "'Others'";

                }
                if (TollProduct)
                {
                    objrpt.DataDefinition.FormulaFields["IsToll"].Text = "'Y'";

                }


                #region Set DataSource

                if (FromDataTable.ToLower() == "y")
                {
                    objrpt.SetDataSource(ReportData);

                }
                else
                {
                    objrpt.SetDataSource(vat17s);
                }

                #endregion

                #region Formula Fields

                CommonDAL _cDal = new CommonDAL();
                string Quantity6_2 = _cDal.settingsDesktop("DecimalPlace", "Quantity6_2", settingsDt, connVM);
                string Amount6_2 = _cDal.settingsDesktop("DecimalPlace", "Amount6_2", settingsDt, connVM);
                string AutoAdj = _cDal.settingsDesktop("VAT6_2", "AutoAdjustment", settingsDt, connVM);

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;

                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Month", Convert.ToDateTime(vm.StartDate).ToString("MMMM-yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsMonthly", vm.IsMonthly ? "Y" : "N");

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", string.IsNullOrEmpty(vm.FontSize) ? OrdinaryVATDesktop.FontSize : vm.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_2", vm.ValuePlace == null ? Amount6_2 : vm.ValuePlace);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOMConversion", UOMConversion);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOM", vm.UOM);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FromDate", Convert.ToDateTime(FromDate).ToString("dd/MM/yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ToDate", Convert.ToDateTime(ToDate).ToString("dd/MM/yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_2", vm.DecimalPlace == null ? Quantity6_2 : vm.DecimalPlace);

                //objrpt.DataDefinition.FormulaFields["Amount6_2"].Text = "'" + Amount6_2 + "'";
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "AutoAdjustment", AutoAdj);

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "HSCode", HSCode);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductName", ProductName);

                //objrpt.DataDefinition.FormulaFields["ProductName"].Text = "'" + ProductName + "'";
                string ProductDescription = vProductVM.ProductDescription;// _pDal.ProductDTByItemNo(txtItemNo.Text.Trim()).Rows[0]["ProductDescription"].ToString();
                ProductDescription = "1";
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductDescription", ProductDescription);

                // //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                // //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //// //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //// objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //// objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                // //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";

                FileLogger.Log("VAT6_2", "VAT6_2 Report Open", DateTime.Now.ToString());

                #endregion

                #endregion Report preview


                    #endregion

                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public ReportDocument VDS12KhaNew(string VendorId, string DepositNumber, string DepositDateFrom, string DepositDateTo,
                                    string IssueDateFrom, string IssueDateTo, string BillDateFrom, string BillDateTo,
                                    string PurchaseNumber, bool chkPurchaseVDS, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                #region Settings
                DataTable settingsDt = new DataTable();
                DateTime invoiceDateTime = DateTime.Now;
                string vVAT2012V2 = "2020-Jul-01";
                string VAT11Name = "";


                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }
                vVAT2012V2 = commonDal.settingsDesktop("Version", "VAT2012V2", settingsDt, connVM);

                InEnglish = commonDal.settingsDesktop("Reports", "VAT6_3English", settingsDt, connVM);
                VAT11Name = commonDal.settingsDesktop("Reports", "VAT6_3", settingsDt, connVM);
                DateTime VAT2012V2 = Convert.ToDateTime(vVAT2012V2);

                #endregion

                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                //DataSet ReportResult = new DataSet();
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                ReportDSDAL reportDsdal = new ReportDSDAL();


                DataSet ReportResultDs = new DataSet();
                ReportResultDs = reportDsdal.VDS12KhaNew(VendorId, DepositNumber, DepositDateFrom, DepositDateTo, IssueDateFrom
                    , IssueDateTo, BillDateFrom, BillDateTo, PurchaseNumber, chkPurchaseVDS, connVM);
                invoiceDateTime = Convert.ToDateTime(ReportResultDs.Tables[0].Rows[0]["DepositDate"]);


                #region Complete
                if (ReportResultDs.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResultDs.Tables[0].TableName = "DsVAT12Kha";

                ReportDocument objrpt = new ReportDocument();
                if (VAT2012V2 <= invoiceDateTime)
                {
                    if (VAT11Name.ToLower() == "nnpl")
                    {
                        objrpt = new RptVAT6_6_NovoNordisk_V12V2();
                    }
                    else if (VAT11Name.ToLower() == "link3")
                    {
                        objrpt = new RptVAT6_6_Link3_V12V2();
                    }
                    else if (VAT11Name.ToUpper() == "BRACDAIRY")
                    {
                        objrpt = new RptVAT6_6_Brac_V12V2();
                    }
                    else if (VAT11Name.ToUpper() == "MCL")
                    {
                        objrpt = new RptVAT6_6_Square_V12V2();
                    }
                    else
                    {
                        objrpt = new RptVAT6_6_V12V2();
                    }
                    ////New Report -- 
                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\V12V2" + @"\RptVAT6_6_V12V2.rpt");



                }
                else
                {
                    UserInformationVM uvm = new UserInformationVM();
                    UserInformationDAL _udal = new UserInformationDAL();
                    uvm = _udal.SelectAll(Convert.ToInt32(OrdinaryVATDesktop.CurrentUserID), null, null, null, null, connVM).FirstOrDefault();

                    objrpt = new RptVAT6_6();

                    objrpt.DataDefinition.FormulaFields["UserFullName"].Text = "'" + uvm.FullName + "'";
                    objrpt.DataDefinition.FormulaFields["UserDesignation"].Text = "'" + uvm.Designation + "'";

                }

                //objrpt = new RptVAT6_6();
                objrpt.SetDataSource(ReportResultDs);
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                ////objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 26'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address11"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address21"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address31"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["ZIPCode"].Text = "'" + OrdinaryVATDesktop.ZipCode + "'";
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address11", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address21", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address31", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument VDS12KhaNew_Multiple(string DepositNumber, bool chkPurchaseVDS, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                #region Settings
                DataTable settingsDt = new DataTable();
                DateTime invoiceDateTime = DateTime.Now;
                string vVAT2012V2 = "2020-Jul-01";
                string VAT11Name = "";


                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }
                vVAT2012V2 = commonDal.settingsDesktop("Version", "VAT2012V2", settingsDt, connVM);

                InEnglish = commonDal.settingsDesktop("Reports", "VAT6_3English", settingsDt, connVM);
                VAT11Name = commonDal.settingsDesktop("Reports", "VAT6_3", settingsDt, connVM);

                DateTime VAT2012V2 = Convert.ToDateTime(vVAT2012V2);

                #endregion

                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                //DataSet ReportResult = new DataSet();
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                ReportDSDAL reportDsdal = new ReportDSDAL();


                DataSet ReportResultDs = new DataSet();
                ReportResultDs = reportDsdal.VDS12KhaNew_Multiple(DepositNumber, chkPurchaseVDS, connVM);
                invoiceDateTime = Convert.ToDateTime(ReportResultDs.Tables[0].Rows[0]["DepositDate"]);


                #region Complete
                if (ReportResultDs.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResultDs.Tables[0].TableName = "DsVAT12Kha";

                ReportDocument objrpt = new ReportDocument();

                if (VAT2012V2 <= invoiceDateTime)
                {
                    ////New Report -- 
                    if (VAT11Name.ToLower() == "nnpl")
                    {
                        objrpt = new RptVAT6_6_NovoNordisk_V12V2();
                    }
                    else
                    {
                        objrpt = new RptVAT6_6_V12V2();
                    }

                }
                else
                {
                    objrpt = new RptVAT6_6();



                }

                //objrpt = new RptVAT6_6();
                objrpt.SetDataSource(ReportResultDs);
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                ////objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 26'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address11"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address21"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address31"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                objrpt.DataDefinition.FormulaFields["ZIPCode"].Text = "'" + OrdinaryVATDesktop.ZipCode + "'";
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address11", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address21", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address31", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);


                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #region VAT Return - 9.1

        public ReportDocument VAT9_1Print(VATReturnVM vm, SysDBInfoVMTemp connVM = null)
        {
            ReportDocument objrpt = new ReportDocument();

            try
            {


                DateTime datePeriodStart = Convert.ToDateTime(vm.PeriodName);
                DateTime HardDecember2019 = Convert.ToDateTime("December-2019");
                if (datePeriodStart < HardDecember2019)
                {
                    objrpt = VAT9_1(vm);
                }
                else
                {
                    vm.IsVersion2 = true;
                    objrpt = VAT9_1_V2(vm);
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objrpt;


        }


        public ReportDocument VAT9_1_V2(VATReturnVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                #region Settings
                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                DataSet ReportResult = new DataSet();

                _9_1_VATReturnDAL reportDsdal = new _9_1_VATReturnDAL();
                ReportResult = reportDsdal.VAT9_1_V2Load(vm, connVM); //postStatus


                #region Complete

                DataTable dt = new DataTable();

                dt = ReportResult.Tables[0];
                if (!dt.Columns.Contains("NoteNo"))
                {

                    return null;
                }
                else
                {
                    if (dt.Rows[0]["NoteNo"].ToString() != "1")
                    {

                        return null;
                    }

                }
                ReportResult.Tables[0].TableName = "dtVATReturns";
                ReportDocument objrpt = new ReportDocument();

                DataTable dtIncreasingAdjustment = new DataTable();
                DataTable dtDecreasingAdjustment = new DataTable();
                dtIncreasingAdjustment = ReportResult.Tables[1];
                dtDecreasingAdjustment = ReportResult.Tables[2];
                string IncreasingAdjustment = "";
                string DecreasingAdjustment = "";

                CommonDAL commonDal = new CommonDAL();
                string Note27Description = commonDal.settingsDesktop("VAT9_1", "Note27Description", settingsDt, connVM);
                string Note32Description = commonDal.settingsDesktop("VAT9_1", "Note32Description", settingsDt, connVM);
                string VAT18_6Adjustment = commonDal.settingsDesktop("VAT9_1", "VAT18_6Adjustment", settingsDt, connVM);


                #region IncreasingAdjustment

                if (dtIncreasingAdjustment != null && dtIncreasingAdjustment.Rows.Count > 0)
                {
                    if (Note27Description == "Y")
                    {
                        foreach (DataRow item in dtIncreasingAdjustment.Rows)
                        {
                            IncreasingAdjustment = IncreasingAdjustment + "~ " + item["AdjName"].ToString() + " - " + item["AdjDescription"].ToString();
                        }
                    }
                    else
                    {
                        foreach (DataRow item in dtIncreasingAdjustment.Rows)
                        {
                            IncreasingAdjustment = IncreasingAdjustment + "~ " + item["AdjName"].ToString();
                        }
                    }

                }

                if (!string.IsNullOrWhiteSpace(IncreasingAdjustment))
                {
                    IncreasingAdjustment = IncreasingAdjustment.Trim('~');
                    IncreasingAdjustment = IncreasingAdjustment.Trim();
                    IncreasingAdjustment = IncreasingAdjustment.Replace("~", ",");
                }

                #endregion

                #region Comments / Apr-12-2020

                ////if (ReportResult.Tables[2].Rows.Count > 0 && ReportResult.Tables[2] != null)
                ////{
                ////    foreach (DataRow item in ReportResult.Tables[2].Rows)
                ////    {
                ////        DecreasingAdjustment = DecreasingAdjustment + "~" + item["AdjName"].ToString();
                ////    }
                ////}

                ////if (DecreasingAdjustment.Length > 0)
                ////{
                ////    DecreasingAdjustment = DecreasingAdjustment.Substring(1, DecreasingAdjustment.Length - 1);
                ////}

                #endregion

                #region DecreasingAdjustment

                if (dtDecreasingAdjustment != null && dtDecreasingAdjustment.Rows.Count > 0)
                {
                    if (Note32Description == "Y")
                    {
                        foreach (DataRow item in dtDecreasingAdjustment.Rows)
                        {
                            DecreasingAdjustment = DecreasingAdjustment + "~ " + item["AdjName"].ToString() + " - " + item["AdjDescription"].ToString();
                        }
                    }
                    else
                    {
                        foreach (DataRow item in dtDecreasingAdjustment.Rows)
                        {
                            DecreasingAdjustment = DecreasingAdjustment + "~ " + item["AdjName"].ToString();
                        }
                    }

                }

                if (!string.IsNullOrWhiteSpace(DecreasingAdjustment))
                {
                    DecreasingAdjustment = DecreasingAdjustment.Trim('~');
                    DecreasingAdjustment = DecreasingAdjustment.Trim();
                    DecreasingAdjustment = DecreasingAdjustment.Replace("~", ",");
                }

                #endregion




                if (vm.IsVersion2)
                {
                    //////Version 2
                    objrpt = new RptVAT9_1_English();
                    //////objrpt.Load(Program.ReportAppPath + @"\RptVAT9_1_English.rpt");
                }
                else
                {
                    objrpt = new RptVAT9_1();
                }
                DataTable dt3 = new DataTable();
                dt3 = ReportResult.Tables[3];

                string submitDate = "";
                string OrginalReturn = "";
                string LateReturn = "";
                string AmendReturn = "";
                string AlternativeReturn = "";
                string NoActivites = "";
                string Details = "";
                string PostStatus = "";
                string Post = "";
                string FullName = "";
                string Designation = "";
                string ContactNo = "";
                string Email = "";
                string NationalID = "";
                string IsTraderVAT = "";



                if (dt3 != null && dt3.Rows.Count > 0)
                {
                    DataRow dr = dt3.Rows[0];
                    submitDate = Convert.ToDateTime(dr["DateOfSubmission"].ToString()).ToString("dd/MM/yyyy");
                    OrginalReturn = dr["MainOrginalReturn"].ToString();
                    LateReturn = dr["LateReturn"].ToString();
                    AmendReturn = dr["AmendReturn"].ToString();
                    AlternativeReturn = dr["FullAdditionalAlternativeReturn"].ToString();
                    NoActivites = dr["NoActivites"].ToString();
                    Details = dr["NoActivitesDetails"].ToString();
                    PostStatus = dr["PostStatus"].ToString();
                    Post = dr["Post"].ToString();
                    FullName = dr["SignatoryName"].ToString();
                    Designation = dr["SignatoryDesig"].ToString();
                    ContactNo = dr["Mobile"].ToString();
                    Email = dr["Email"].ToString();
                    NationalID = dr["NationalID"].ToString();
                    IsTraderVAT = dr["IsTraderVAT"].ToString();
                }


                objrpt.SetDataSource(ReportResult);
                CompanyprofileDAL cdal = new CompanyprofileDAL();
                CompanyProfileVM cvm = new CompanyProfileVM();

                #region Formula Fields

                string TotalDepositVAT = commonDal.settingsDesktop("OperationalCode", "TotalDepositVAT", settingsDt, connVM);
                string TotalDepositSD = commonDal.settingsDesktop("OperationalCode", "TotalDepositSD", settingsDt, connVM);
                string InterestOnOveredVATDeposit = commonDal.settingsDesktop("OperationalCode", "InterestOnOveredVATDeposit", settingsDt, connVM);
                string InterestOnOveredSDDeposit = commonDal.settingsDesktop("OperationalCode", "InterestOnOveredSDDeposit", settingsDt, connVM);
                string FineOrPenaltyDeposit = commonDal.settingsDesktop("OperationalCode", "FineOrPenaltyDeposit", settingsDt, connVM);
                string ExciseDutyDeposit = commonDal.settingsDesktop("OperationalCode", "ExciseDutyDeposit", settingsDt, connVM);
                string DevelopmentSurchargeDeposit = commonDal.settingsDesktop("OperationalCode", "DevelopmentSurchargeDeposit", settingsDt, connVM);
                string ICTDevelopmentSurchargeDeposit = commonDal.settingsDesktop("OperationalCode", "ICTDevelopmentSurchargeDeposit", settingsDt, connVM);
                string HelthCareSurchargeDeposit = commonDal.settingsDesktop("OperationalCode", "HelthCareSurchargeDeposit", settingsDt, connVM);
                string EnvironmentProtectionSurchargeDeposit = commonDal.settingsDesktop("OperationalCode", "EnvironmentProtectionSurchargeDeposit", settingsDt, connVM);

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
                string Period = Convert.ToDateTime(vm.Date).ToString("MMMM - yyyy");
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["CompanyAddress"].Text = "'" + cvm.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["AccountType"].Text = "'" + cvm.AccountingNature + "'";
                objrpt.DataDefinition.FormulaFields["BIN"].Text = "'" + cvm.BIN + "'";
                objrpt.DataDefinition.FormulaFields["BusinessType"].Text = "'" + cvm.BusinessNature + "'";
                objrpt.DataDefinition.FormulaFields["PeriodName"].Text = "'" + vm.PeriodName + "'";

                if (false)
                {

                    ////Period = "July - 2020";
                    ////objrpt.DataDefinition.FormulaFields["PeriodName"].Text = "'" + "July - 2020" + "'";
                }


                #endregion



                #region Preveiw
                if (Post.ToLower() != "y")
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    if (vm.PriodLock == "N")
                    {
                        objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                    }
                }
                #endregion

                #region More Formula Fields

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FullName", FullName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Designation", Designation);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ContactNo", ContactNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Email", Email);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "NationalID", NationalID);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "OrginalReturn", OrginalReturn);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "LateReturn", LateReturn);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "AmendReturn", AmendReturn);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "AlternativeReturn", AlternativeReturn);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "NoActivites", NoActivites);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Details", Details);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SubmitDate", submitDate);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Note56", VAT18_6Adjustment);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsTraderVAT", IsTraderVAT);

                #endregion



                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument VAT9_1(VATReturnVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                #region Settings
                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                DataSet ReportResult = new DataSet();

                _9_1_VATReturnDAL reportDsdal = new _9_1_VATReturnDAL();
                ReportResult = reportDsdal.VAT9_1(vm, connVM);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "dtVATReturns";
                ReportDocument objrpt = new ReportDocument();

                DataTable dtIncreasingAdjustment = new DataTable();
                DataTable dtDecreasingAdjustment = new DataTable();
                dtIncreasingAdjustment = ReportResult.Tables[1];
                dtDecreasingAdjustment = ReportResult.Tables[2];
                string IncreasingAdjustment = "";
                string DecreasingAdjustment = "";
                if (ReportResult.Tables[1].Rows.Count > 0 && ReportResult.Tables[1] != null)
                {
                    foreach (DataRow item in ReportResult.Tables[1].Rows)
                    {
                        IncreasingAdjustment = IncreasingAdjustment + "~ " + item["AdjName"].ToString();
                    }
                }

                if (IncreasingAdjustment.Length > 0)
                {
                    IncreasingAdjustment = IncreasingAdjustment.Substring(1, IncreasingAdjustment.Length - 1);
                }


                if (ReportResult.Tables[2].Rows.Count > 0 && ReportResult.Tables[2] != null)
                {
                    foreach (DataRow item in ReportResult.Tables[2].Rows)
                    {
                        DecreasingAdjustment = DecreasingAdjustment + "~" + item["AdjName"].ToString();
                    }
                }

                if (DecreasingAdjustment.Length > 0)
                {
                    DecreasingAdjustment = DecreasingAdjustment.Substring(1, DecreasingAdjustment.Length - 1);
                }

                if (vm.IsVersion2)
                {
                    //////Version 2
                    //objrpt = new RptVAT9_1();
                    objrpt = new RptVAT9_1_English();
                    //////objrpt.Load(Program.ReportAppPath + @"\RptVAT9_1_English.rpt");
                }
                else
                {
                    objrpt = new RptVAT9_1();
                }


                objrpt.SetDataSource(ReportResult);
                CompanyprofileDAL cdal = new CompanyprofileDAL();
                CompanyProfileVM cvm = new CompanyProfileVM();

                #region Formula Fields

                CommonDAL commonDal = new CommonDAL();
                string TotalDepositVAT = commonDal.settingsDesktop("OperationalCode", "TotalDepositVAT", settingsDt, connVM);
                string TotalDepositSD = commonDal.settingsDesktop("OperationalCode", "TotalDepositSD", settingsDt, connVM);
                string InterestOnOveredVATDeposit = commonDal.settingsDesktop("OperationalCode", "InterestOnOveredVATDeposit", settingsDt, connVM);
                string InterestOnOveredSDDeposit = commonDal.settingsDesktop("OperationalCode", "InterestOnOveredSDDeposit", settingsDt, connVM);
                string FineOrPenaltyDeposit = commonDal.settingsDesktop("OperationalCode", "FineOrPenaltyDeposit", settingsDt, connVM);
                string ExciseDutyDeposit = commonDal.settingsDesktop("OperationalCode", "ExciseDutyDeposit", settingsDt, connVM);
                string DevelopmentSurchargeDeposit = commonDal.settingsDesktop("OperationalCode", "DevelopmentSurchargeDeposit", settingsDt, connVM);
                string ICTDevelopmentSurchargeDeposit = commonDal.settingsDesktop("OperationalCode", "ICTDevelopmentSurchargeDeposit", settingsDt, connVM);
                string HelthCareSurchargeDeposit = commonDal.settingsDesktop("OperationalCode", "HelthCareSurchargeDeposit", settingsDt, connVM);
                string EnvironmentProtectionSurchargeDeposit = commonDal.settingsDesktop("OperationalCode", "EnvironmentProtectionSurchargeDeposit", settingsDt, connVM);

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
                objrpt.DataDefinition.FormulaFields["PeriodName"].Text = "'" + vm.PeriodName + "'";

                #endregion

                #region UserInfo

                UserInformationVM varUserInformationVM = new UserInformationVM();
                UserInformationDAL _UserInformationDAL = new UserInformationDAL();
                varUserInformationVM = _UserInformationDAL.SelectAll(0, new[] { "ui.UserID" }, new[] { OrdinaryVATDesktop.CurrentUserID }, null, null, connVM).FirstOrDefault();

                //////DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + OrdinaryVATDesktop.CurrentUserID + "'");
                string FullName = varUserInformationVM.FullName;
                string Designation = varUserInformationVM.Designation;
                string ContactNo = varUserInformationVM.Mobile;

                #endregion

                #region More Formula Fields

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FullName", FullName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Designation", Designation);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ContactNo", ContactNo);

                #endregion



                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument VAT9_1_SubForm(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            ReportDocument objrpt = new ReportDocument();
            try
            {


                #region Variables and Objects

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                #endregion

                #region Settings
                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                //////Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();

                #region Check Point

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }

                #endregion

                #region Data Call

                dt = new _9_1_VATReturnDAL().VAT9_1_SubForm(vm);

                ds = new DataSet();
                ds.Tables.Add(dt);

                #endregion

                #region Version Control


                string InEnglish = new CommonDAL().settingsDesktop("Reports", "VAT9_1SubFormEnglish", settingsDt, connVM);

                DateTime datePeriodStart = Convert.ToDateTime(vm.PeriodName);
                DateTime HardDecember2019 = Convert.ToDateTime("December-2019");
                if (datePeriodStart >= HardDecember2019)
                {
                    InEnglish = "Y";
                }

                #endregion

                #region Company Profile

                DataTable dtComapnyProfile = new DataTable();

                DataSet tempDS = new DataSet();
                tempDS = new ReportDSDAL().ComapnyProfile("");
                dtComapnyProfile = tempDS.Tables[0].Copy();

                dtComapnyProfile.TableName = "DsCompanyProfile";

                ds.Tables.Add(dtComapnyProfile);

                #endregion


                if (dt.Rows.Count <= 0)
                {
                    return null;
                }

                #region Table Name

                string tableName = "";

                tableName = dt.TableName;

                switch (tableName)
                {
                    case "dtVATReturnSubFormA_2021":
                        objrpt = new rptVATReturnSubFormA_2021_New();
                        ////objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\rptVATReturnSubFormA.rpt");
                        break;
                    case "dtVATReturnSubFormA":
                        objrpt = new rptVATReturnSubFormA();
                        ////objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\rptVATReturnSubFormA.rpt");
                        break;
                    case "dtVATReturnSubFormB":
                        objrpt = new rptVATReturnSubFormB();
                        break;
                    case "dtVATReturnSubFormC":
                        objrpt = new rptVATReturnSubFormC();
                        break;
                    case "dtVATReturnSubFormD":
                        objrpt = new rptVATReturnSubFormD();
                        break;
                    case "dtVATReturnSubFormE":
                        objrpt = new rptVATReturnSubFormE();
                        break;
                    case "dtVATReturnSubFormF":
                        objrpt = new rptVATReturnSubFormF();
                        break;
                    case "dtVATReturnSubFormG":
                        objrpt = new rptVATReturnSubFormG();
                        break;
                    case "dtVATReturnSubFormH":
                        objrpt = new rptVATReturnSubFormH();
                        break;
                    case "dtVATReturnSubFormI":
                        objrpt = new rptVATReturnSubFormI();
                        break;
                    case "dtVATReturnSubFormJ":
                        objrpt = new rptVATReturnSubFormJ();
                        break;
                    default:
                        break;
                }

                #endregion


                #region Set Data Source

                objrpt.SetDataSource(ds);

                #endregion

                #region Formula Fields

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", vm.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "DecimalPlace", vm.DecimalPlace == null ? "2" : vm.DecimalPlace);

                #endregion



            }
            catch (Exception)
            {

                throw;
            }
            finally { }
            return objrpt;
        }

        public VATReturnSubFormVM VAT9_1_SubForm_Download(VATReturnSubFormVM vm)
        {
            ReportDocument objrpt = new ReportDocument();
            try
            {
                //////Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }

                vm = new _9_1_VATReturnDAL().VAT9_1_SubForm_Download(vm);

                DataRow dr = vm.dtComapnyProfile.Rows[0];

                string ComapnyName = dr["CompanyLegalName"].ToString();
                string VatRegistrationNo = dr["VatRegistrationNo"].ToString();
                string Address1 = dr["Address1"].ToString();

                string[] ReportHeaders = new string[] { ComapnyName, VatRegistrationNo, Address1, "Sub-form (For Note - " + vm.NoteNo + ")", "Type: " + vm.Type };

                DataTable dt = vm.dtVATReturnSubForm;

                if (dt == null || dt.Rows.Count == 0)
                {
                    return vm;
                }

                if (vm.dtVATReturnSubForm.Rows.Count > 0 && vm.NoteNo == 29)
                {
                    vm.dtVATReturnSubForm.Columns["CustomerBIN"].ColumnName = "Buyer's BIN";
                    vm.dtVATReturnSubForm.Columns["CustomerName"].ColumnName = "Buyer's Name";
                    vm.dtVATReturnSubForm.Columns["CustomerAddress"].ColumnName = "Buyer's Address";
                    vm.dtVATReturnSubForm.Columns["TotalPrice"].ColumnName = "Value";
                    vm.dtVATReturnSubForm.Columns["VDSAmount"].ColumnName = "Deducted VAT";
                    vm.dtVATReturnSubForm.Columns["InvoiceNo"].ColumnName = "Invoice No./Challan/Bill No.etc.";
                    vm.dtVATReturnSubForm.Columns["InvoiceDate"].ColumnName = "Invoice/Challan/ Bill Date";
                    vm.dtVATReturnSubForm.Columns["VDSCertificateNo"].ColumnName = "VAT Deduction At Source Certificate No.";
                    vm.dtVATReturnSubForm.Columns["VDSCertificateDate"].ColumnName = "VAT Deduction at Source Certificate Date";
                    vm.dtVATReturnSubForm.Columns["AccountCode"].ColumnName = "Tax Deposit Account Code";
                    vm.dtVATReturnSubForm.Columns["SerialNo"].ColumnName = "Tax Deposit Serial No Of Bank Transfer";
                    vm.dtVATReturnSubForm.Columns["TaxDepositDate"].ColumnName = "Tax Deposit Date";
                    vm.dtVATReturnSubForm.Columns["DetailRemarks"].ColumnName = "Notes";

                }

                #region Column Name Change


                dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

                if (vm.NoteNo == 21)
                {
                    dt.Columns["ProductCode"].ColumnName = "Goods/Service Code";
                    dt.Columns["ProductName"].ColumnName = "Goods/Service Name";
                    dt.Columns["ProductDescription"].ColumnName = "Goods/Service Commercial Description";
                }

                else if (vm.NoteNo == 22)
                {
                    dt.Columns["ProductDescription"].ColumnName = "Goods/Service Commercial Description";
                }


                string[] DtcolumnName = new string[dt.Columns.Count];
                int j = 0;
                foreach (DataColumn column in dt.Columns)
                {
                    DtcolumnName[j] = column.ColumnName;
                    j++;
                }

                for (int k = 0; k < DtcolumnName.Length; k++)
                {
                    dt = OrdinaryVATDesktop.DtColumnNameChange(dt, DtcolumnName[k], OrdinaryVATDesktop.AddSpacesToSentence(DtcolumnName[k]));
                }



                string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                string fileDirectory = pathRoot + "//Excel Files";
                Directory.CreateDirectory(fileDirectory);

                vm.FileName = "Sub-form (For Note - " + vm.NoteNo + ")" + "-" + vm.PeriodName;

                fileDirectory += "\\" + vm.FileName + ".xlsx";
                FileStream objFileStrm = File.Create(fileDirectory);

                int TableHeadRow = 0;
                TableHeadRow = ReportHeaders.Length + 2;

                int RowCount = 0;
                RowCount = dt.Rows.Count;

                int ColumnCount = 0;
                ColumnCount = dt.Columns.Count;

                int GrandTotalRow = 0;
                GrandTotalRow = TableHeadRow + RowCount + 1;
                if (string.IsNullOrEmpty(vm.SheetName))
                {
                    vm.SheetName = "DemoSubFormSheet";
                }

                #endregion

                ExcelPackage package = new ExcelPackage(objFileStrm);

                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Sub-form (For Note - " + vm.NoteNo + ")");

                ////ws.Cells["A1"].LoadFromDataTable(dt, true);
                ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                #region Format

                var format = new OfficeOpenXml.ExcelTextFormat();
                format.Delimiter = '~';
                format.TextQualifier = '"';
                format.DataTypes = new[] { eDataTypes.String };



                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                    ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

                }
                int colNumber = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        ws.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {

                        ws.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                        #region Grand Total
                        ws.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + ws.Cells[TableHeadRow + 1, colNumber].Address + ":" + ws.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                        #endregion
                    }

                }

                ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
                ws.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

                ws.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                ws.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");
                #endregion


                vm.varFileDirectory = fileDirectory;
                vm.varExcelPackage = package;
                vm.varFileStream = objFileStrm;
                ////package.Save();
                ////objFileStrm.Close();

            }
            catch (Exception)
            {

                throw;
            }
            finally { }
            return vm;
        }


        #endregion

        public ReportDocument DisposeRaw(string DisposeNo, int BranchId, bool PreviewOnly, SysDBInfoVMTemp connVM = null)
        {

            try
            {

                #region Settings
                string vVAT2012V2 = "2012-Jul-01";

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null);
                }



                string Quantity6_5 = commonDal.settingsDesktop("DecimalPlace", "Quantity6_5", settingsDt, connVM);
                string Amount6_5 = commonDal.settingsDesktop("DecimalPlace", "Amount6_5", settingsDt, connVM);
                vVAT2012V2 = commonDal.settingsDesktop("Version", "VAT2012V2", settingsDt, connVM);

                #endregion

                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                DateTime TransferDateTime = DateTime.Now;
                DateTime VAT2012V2 = Convert.ToDateTime(vVAT2012V2);

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();



                ReportResult = reportDsdal.DisposeRaw(DisposeNo, BranchId);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "DsDisposeRaw";
                ReportDocument objrpt = new ReportDocument();
                //var dal = new BranchProfileDAL();

                //var toVM = dal.SelectAllList(ReportResult.Tables[0].Rows[0]["BranchId"].ToString(), null, null, null, null, null).FirstOrDefault();
                //var FromVM = dal.SelectAllList(ReportResult.Tables[0].Rows[0]["FromBranchId"].ToString(), null, null, null, null, null).FirstOrDefault();

                //var legalName = toVM == null ? branchName : toVM.BranchLegalName;







                objrpt = new RptVAT4_4();
                //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\V12V2" + @"\Rpt4_4.rpt");




                objrpt.SetDataSource(ReportResult);
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_5", Quantity6_5);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_5", Amount6_5);

                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReportDocument DisposeFinish(string DisposeNo, int BranchId, bool PreviewOnly, SysDBInfoVMTemp connVM = null)
        {

            try
            {

                #region Settings

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null);
                }

                string Quantity6_5 = commonDal.settingsDesktop("DecimalPlace", "Quantity6_5", settingsDt, connVM);
                string Amount6_5 = commonDal.settingsDesktop("DecimalPlace", "Amount6_5", settingsDt, connVM);

                #endregion

                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = reportDsdal.DisposeFinish(DisposeNo, BranchId, connVM);

                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "dtDisposeFinish";
                ReportDocument objrpt = new ReportDocument();

                objrpt = new RptVAT4_5();
                //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\V12V2" + @"\RptVAT4_5.rpt");

                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_5", Quantity6_5);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_5", Amount6_5);

                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ReportDocument StockMovement6_2_1(string itemNo, string tranDate, string tranDateTo, int BranchId, SqlConnection VcurrConn
  , SqlTransaction Vtransaction, string Post1, string Post2, string ProdutType, string ProdutCategoryId, SysDBInfoVMTemp connVM = null, bool PreviewOnly = false, string InEnglish = null, string UserId = "")
        {
            #region Variables and Objects

            ReportDocument objrpt = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();

            #endregion

            try
            {
                #region UOM Conversion
                string UOMConversion = "1";
                #endregion

                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion


                #region Settings Load
                DataTable settingsDt = new DataTable();
                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion
                #region Data Call
                DataSet ReportResult = new DataSet();
                ReportDSDAL reportDsdal = new ReportDSDAL();
                BranchProfileDAL branchProfile = new BranchProfileDAL();
                DataTable branchList = branchProfile.SelectAll();


                ProductDAL pdal = new ProductDAL();
                ProductVM vProductVM = new ProductVM();
                #region Trading product Check
                vProductVM = new ProductDAL().SelectAll(itemNo).FirstOrDefault();

                //if (vProductVM.ProductType != "Trading")
                //{
                //    MessageBox.Show(vProductVM.ProductName + " is not a Trading Product!", "", MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return null;
                //}
                #endregion Trading product Check

                VAT6_2ParamVM paramVm = new VAT6_2ParamVM();
                paramVm.ItemNo = itemNo;
                paramVm.StartDate = tranDate;
                paramVm.EndDate = tranDateTo;
                paramVm.Post1 = Post1;
                paramVm.Post2 = Post2;
                paramVm.UserId = UserId;
                paramVm.BranchWise = BranchId != 0;
                paramVm.BranchId = BranchId;

                // ReportResult = pdal.StockMovement6_2_1(itemNo, tranDate, tranDateTo, 0, null, null, Post1, Post2, "", "", connVM, UserId); ;
                ReportResult = reportDsdal.VAT6_2_1(paramVm);

                #endregion Data Call

                #region Statement

                #region Declarations

                #endregion

                #region Check Point

                DateTime invoiceDateTime = DateTime.Now;
                string vVAT2012V2 = "2020-Jul-01";
                vVAT2012V2 = new CommonDAL().settingsDesktop("Version", "VAT2012V2", settingsDt, connVM);

                DateTime VAT2012V2 = Convert.ToDateTime(vVAT2012V2);
                // Start Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;

                }

                #endregion

                if (ReportResult.Tables[0].Rows.Count > 0)
                {
                    invoiceDateTime = Convert.ToDateTime(ReportResult.Tables[0].Rows[0]["InvoiceDateTime"]);
                }



                ProductDAL _pdal = new ProductDAL();

                decimal cost = _pdal.AvgPriceTender(itemNo, tranDate, null, null, connVM);

                ReportResult.Tables[0].TableName = "DsVAT16";

                ReportResult.Tables[0].Columns["RunningTotal"].ColumnName = "ClosingQuantity";
                ReportResult.Tables[0].Columns["RunningTotalValue"].ColumnName = "ClosingAmount";

                //ReportResult.Tables[0].Columns.Add("ClosingQuantity", typeof(decimal));
                //ReportResult.Tables[0].Columns.Add("ClosingAmount", typeof(decimal));
                decimal StartingQuantity = 0;
                decimal StartingAmount = 0;

                #region Opening Loop

                foreach (DataRow row in ReportResult.Tables[0].Rows)
                {
                    decimal Quantity = Convert.ToDecimal(row["Quantity"]);
                    decimal UnitCost = Convert.ToDecimal(row["UnitCost"]);
                    if (row["TransType"].ToString() == "Opening")
                    {
                        StartingQuantity = Convert.ToDecimal(row["StartingQuantity"]);
                        StartingAmount = StartingQuantity * cost;

                        row["ClosingQuantity"] = StartingQuantity;
                        row["ClosingAmount"] = StartingAmount;

                    }
                    else
                    {
                        row["StartingQuantity"] = StartingQuantity;
                        row["StartingAmount"] = StartingAmount;

                        if (row["TransType"].ToString() == "Purchase" || row["TransType"].ToString() == "Receive")
                        {
                            StartingQuantity = StartingQuantity + Quantity;
                            StartingAmount = StartingAmount + UnitCost;

                            row["ClosingQuantity"] = StartingQuantity;
                            row["ClosingAmount"] = StartingAmount;

                        }

                        else if (row["TransType"].ToString() == "Sale")
                        {
                            StartingQuantity = StartingQuantity - Quantity;
                            StartingAmount = Convert.ToDecimal(StartingAmount - Convert.ToDecimal(row["UnitCost"]));

                            row["ClosingQuantity"] = StartingQuantity;
                            row["ClosingAmount"] = StartingAmount;
                        }

                        else if (row["TransType"].ToString() == "Credit")
                        {
                            StartingQuantity = StartingQuantity - Quantity;

                            StartingAmount = Convert.ToDecimal(StartingAmount - Convert.ToDecimal(row["UnitCost"]));

                            row["ClosingQuantity"] = StartingQuantity;
                            row["ClosingAmount"] = StartingAmount;
                        }
                    }
                }

                #endregion

                #region Report Set

                objrpt = new ReportDocument();

                //objrpt.Load(Program.ReportAppPath + @"\RptVAT6_2_1.rpt");
                //objrpt = new RptVAT6_2_1();
                if (VAT2012V2 <= invoiceDateTime)
                {
                    ////New Report -- 
                    ////objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\V12V2" + @"\RptVAT6_2_1V12V2.rpt");

                    objrpt = new RptVAT6_2_1V12V2();
                }
                else
                {
                    objrpt = new RptVAT6_2_1();
                }

                ////////RptVATKaTrading objrpt = new RptVATKaTrading();
                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }

                #endregion

                #region Set Data Source

                objrpt.SetDataSource(ReportResult);

                #endregion

                #region Formula Fields

                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + OrdinaryVATDesktop.ZipCode + "'";
                objrpt.DataDefinition.FormulaFields["EndDate"].Text = "'" + Convert.ToDateTime(tranDateTo).ToString("dd/MMM/yyyy") + "'";

                CommonDAL _cDal = new CommonDAL();

                string Quantity6_2_1 = _cDal.settingsDesktop("DecimalPlace", "Quantity6_2_1", settingsDt, connVM);
                string Amount6_2_1 = _cDal.settingsDesktop("DecimalPlace", "Amount6_2_1", settingsDt, connVM);
                string UomConversion = Convert.ToString(UOMConversion);
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOMConversion", UomConversion);

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_2_1", Quantity6_2_1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_2_1", Amount6_2_1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FromDate", Convert.ToDateTime(tranDate).ToString("dd/MM/yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ToDate", Convert.ToDateTime(tranDateTo).ToString("dd/MM/yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductName", vProductVM.ProductName.Replace("'", String.Empty));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductCode", vProductVM.ProductCode);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";

                #endregion

                #endregion
                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReportDocument Client6_3(string InvoiceNo, int BranchId, bool PreviewOnly, SysDBInfoVMTemp connVM = null)
        {

            try
            {

                #region Settings

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                string Quantity6_3 = commonDal.settingsDesktop("DecimalPlace", "Quantity6_3", settingsDt, connVM);
                string Amount6_3 = commonDal.settingsDesktop("DecimalPlace", "Amount6_3", settingsDt, connVM);
                string FontSize = commonDal.settingsDesktop("FontSize", "VAT6_3", settingsDt, connVM);

                #endregion

                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }

                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = reportDsdal.Client6_3(InvoiceNo, connVM);

                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "DsVAT11";
                ReportDocument objrpt = new ReportDocument();

                objrpt = new RptVAT_Client6_3();
                ////objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\V12V2" + @"\RptVAT_Client6_3.rpt");

                objrpt.SetDataSource(ReportResult);
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_3", Quantity6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_3", Amount6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReportDocument VAT6_3Toll(string TollNo, string Post1, string Post2, SysDBInfoVMTemp connVM = null)
        {

            try
            {

                DataTable settingsDt = new DataTable();
                string vVAT2012V2 = "2020-Jul-01";

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }
                CommonDAL _cDal = new CommonDAL();


                vVAT2012V2 = _cDal.settingsDesktop("Version", "VAT2012V2", settingsDt, connVM);
                string TollDate = "";
                string SignatoryName = "";
                string SignatoryDesig = "";
                DateTime invoiceDateTime = DateTime.Now;
                DataSet ds = new DataSet();
                IReport Reportdal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);

                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = reportDsdal.VAT6_3Toll(TollNo, Post1, Post2, connVM);


                //ds = Reportdal.VAT6_3Toll(txtTollNo.Text.Trim(), "", "", connVM);
                invoiceDateTime = Convert.ToDateTime(ReportResult.Tables[2].Rows[0]["TollDate"]);


                ReportResult.Tables[0].TableName = "DsVAT11";
                TollDate = Convert.ToString(ReportResult.Tables[2].Rows[0]["TollDate"]);
                SignatoryName = Convert.ToString(ReportResult.Tables[2].Rows[0]["SignatoryName"]);
                SignatoryDesig = Convert.ToString(ReportResult.Tables[2].Rows[0]["SignatoryDesig"]);



                DateTime VAT2012V2 = Convert.ToDateTime(vVAT2012V2);
                ReportDocument objrpt = new ReportDocument();

                //objrpt = new RptVAT_Toll6_3();
                if (VAT2012V2 <= invoiceDateTime)
                {
                    ////New Report -- 
                    objrpt = new RptVAT_Toll6_3_V12V2();

                }
                else
                {
                    objrpt = new RptVAT_Toll6_3();

                }
                objrpt.SetDataSource(ReportResult);


                #region Formula Fields

                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT Toll 6.3'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + CompanyVm.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + CompanyVm.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + CompanyVm.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + CompanyVm.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + CompanyVm.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + CompanyVm.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + CompanyVm.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + CompanyVm.VatRegistrationNo + "'";

                #endregion

                #region CheckPoint - Formula Fields Existence


                string Quantity6_3 = _cDal.settingsDesktop("DecimalPlace", "Quantity6_3", settingsDt, connVM);
                string Amount6_3 = _cDal.settingsDesktop("DecimalPlace", "Amount6_3", settingsDt, connVM);
                string VATRate6_3 = _cDal.settingsDesktop("DecimalPlace", "VATRate6_3", settingsDt, connVM);
                string FontSize = _cDal.settingsDesktop("FontSize", "VAT6_3", settingsDt, connVM);

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;

                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_3", Quantity6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_3", Amount6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TollDate", TollDate);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SignatoryName", SignatoryName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SignatoryDesig", SignatoryDesig);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VATRate6_3", VATRate6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", FontSize);

                #endregion

                //FormReport reports = new FormReport();
                //reports.crystalReportViewer1.Refresh();

                //reports.setReportSource(objrpt);
                ////reports.ShowDialog();
                //reports.WindowState = FormWindowState.Maximized;
                //reports.Show();



                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region Toll Register

        public ReportDocument ReportToll6_1(VAT6_1ParamVM vm, SysDBInfoVMTemp connVM = null)
        {

            #region Variables and Objects

            string IsRaw = "";
            ReportDocument objrpt = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            string FromDate = vm.StartDate;
            string ToDate = vm.EndDate;
            DataSet ReportResult = new DataSet();

            CommonDAL _cDal = new CommonDAL();
            ProductDAL _pDal = new ProductDAL();
            ProductVM vProductVM = new ProductVM();

            List<VAT_16> vat16s = new List<VAT_16>();
            VAT_16 vat16 = new VAT_16();

            #endregion

            try
            {
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                #region Data Call

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = reportDsdal.VAT6_1Toll(vm.ItemNo, vm.UserName, vm.StartDate, vm.EndDate, vm.Post1, vm.Post2, "", vm.BranchId, connVM, true, null, null, vm);

                #endregion


                DataTable settingsDt = new DataTable();
                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #region Product Call


                vProductVM = _pDal.SelectAll(vm.ItemNo, null, null, null, null, null, connVM).FirstOrDefault();

                if (vProductVM.ProductType.ToLower() == "service(nonstock)")
                {
                    IsRaw = vProductVM.ProductType.ToLower();
                }

                bool nonstock = false;
                if (IsRaw.ToLower() == "service(nonstock)" || IsRaw.ToLower() == "overhead")
                {
                    nonstock = true;
                }

                #endregion

                #region Statement

                #region Check Point

                if (ReportResult.Tables.Count <= 0)
                {
                    objrpt = null;
                    return objrpt;
                }

                #endregion

                #region Variables

                decimal vColumn1 = 0;
                DateTime vColumn2 = DateTime.MinValue;
                decimal vColumn3 = 0;
                decimal vColumn4 = 0;
                string vColumn5 = "-";
                DateTime vColumn6 = DateTime.MinValue;
                string vColumn7 = string.Empty;
                string vColumn8 = string.Empty;
                string vColumn9 = string.Empty;
                string vColumn10 = string.Empty;
                decimal vColumn11 = 0;
                decimal vColumn12 = 0;
                decimal vColumn13 = 0;
                decimal vColumn14 = 0;
                decimal vColumn15 = 0;
                decimal vColumn16 = 0;
                decimal vColumn17 = 0;
                decimal vColumn18 = 0;
                string vColumn19 = string.Empty;
                DateTime vTempStartDateTime = DateTime.MinValue;
                decimal vTempQuantity = 0;
                decimal vTempSubtotal = 0;
                string vTempVendorName = string.Empty;
                string vTempVATRegistrationNo = string.Empty;
                string vTempAddress1 = string.Empty;
                string vTempTransID = string.Empty;
                DateTime vTempInvoiceDateTime = DateTime.MinValue;
                string vTempProductName = string.Empty;
                string vTempBENumber = string.Empty;
                decimal vTempSDAmount = 0;
                decimal vTempVATAmount = 0;
                string vTempremarks = string.Empty;
                string vTemptransType = string.Empty;
                decimal vClosingQuantity = 0;
                decimal vClosingAmount = 0;
                decimal vClosingAvgRate = 0;
                decimal OpeningQty = 0;
                decimal OpeningAmnt = 0;
                decimal PurchaseQty = 0;
                decimal PurchaseAmnt = 0;
                decimal IssueQty = 0;
                decimal IssueAmnt = 0;
                decimal CloseQty = 0;
                decimal CloseAmnt = 0;
                decimal OpQty = 0;
                decimal OpAmnt = 0;
                decimal avgRate = 0;
                string HSCode = string.Empty;
                string ProductName = string.Empty;

                #endregion

                int i = 1;

                #region Opening Data

                DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("transType='Opening'");
                foreach (DataRow row in DetailRawsOpening)
                {
                    vTempremarks = row["Remarks"].ToString().Trim();
                    vTemptransType = row["TransType"].ToString().Trim();
                    ProductName = row["ProductName"].ToString().Trim();
                    HSCode = row["HSCodeNo"].ToString().Trim();

                    vTempStartDateTime = Convert.ToDateTime(row["StartDateTime"].ToString().Trim());

                    OpQty = Convert.ToDecimal(row["Quantity"].ToString().Trim());
                    OpAmnt = Convert.ToDecimal(row["UnitCost"].ToString().Trim());
                    vat16 = new VAT_16();

                    #region if row 1 Opening

                    OpeningQty = OpQty;
                    OpeningAmnt = OpAmnt;
                    OpAmnt = 0;
                    OpQty = 0;

                    PurchaseQty = 0;
                    PurchaseAmnt = 0;
                    IssueQty = 0;
                    IssueAmnt = 0;

                    CloseQty =
                        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                         Convert.ToDecimal(IssueQty));
                    CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                 Convert.ToDecimal(IssueAmnt));
                    vColumn2 = vTempStartDateTime;
                    vColumn3 = OpeningQty;
                    vColumn4 = OpeningAmnt;
                    vColumn5 = "-";
                    vColumn6 = vTempStartDateTime;
                    vColumn7 = "-";
                    vColumn8 = "-";
                    vColumn9 = "-";
                    vColumn10 = "-";
                    vColumn11 = PurchaseQty;
                    vColumn12 = PurchaseAmnt;
                    vColumn13 = 0;
                    vColumn14 = 0;
                    vColumn15 = IssueQty;
                    vColumn16 = IssueAmnt;
                    vColumn17 = CloseQty;
                    vColumn18 = CloseAmnt;
                    vColumn19 = vTempremarks;

                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                                        Convert.ToDecimal(IssueQty));
                    if (vClosingQuantity == 0)
                    {
                        vClosingAmount = 0;


                    }
                    else
                    {
                        vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                          Convert.ToDecimal(IssueAmnt));
                        vClosingAvgRate = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));


                    }

                    #endregion

                    #region AssignValue to List
                    if (nonstock == true)
                    {

                        vColumn3 = 0;
                        vColumn4 = 0;
                        vColumn17 = 0;
                        vColumn18 = 0;
                    }
                    vat16.Column1 = vColumn1;
                    vat16.Column2 = vColumn2;
                    vat16.Column3 = vColumn3;
                    vat16.Column4 = vColumn4;
                    vat16.Column5 = vColumn5;
                    vat16.Column6 = vColumn6;
                    vat16.Column6String = "";
                    vat16.Column7 = vColumn7;
                    vat16.Column8 = vColumn8;
                    vat16.Column9 = vColumn9;
                    vat16.Column10 = vColumn10;
                    vat16.Column11 = vColumn11;
                    vat16.Column12 = vColumn12;
                    vat16.Column13 = vColumn13;
                    vat16.Column14 = vColumn14;
                    vat16.Column15 = vColumn15;
                    vat16.Column16 = vColumn16;
                    vat16.Column17 = vColumn17;
                    vat16.Column18 = vColumn18;
                    vat16.Column19 = vColumn19;



                    vat16s.Add(vat16);
                    i = i + 1;

                    #endregion AssignValue to List


                }

                #endregion

                #region Details Data

                DataRow[] DetailRawsOthers = ReportResult.Tables[0].Select("transType<>'Opening'");
                if (DetailRawsOthers == null || !DetailRawsOthers.Any())
                {
                    objrpt = null;
                    return objrpt;
                }

                string strSort = "CreateDateTime ASC, SerialNo ASC";

                DataView dtview = new DataView(DetailRawsOthers.CopyToDataTable());
                dtview.Sort = strSort;
                DataTable dtsorted = dtview.ToTable();


                #region For Loop

                foreach (DataRow item in dtsorted.Rows)
                {
                    #region Get from Datatable

                    OpeningQty = 0;
                    OpeningAmnt = 0;
                    PurchaseQty = 0;
                    PurchaseAmnt = 0;
                    IssueQty = 0;
                    IssueAmnt = 0;
                    CloseQty = 0;
                    CloseAmnt = 0;

                    vColumn1 = i;
                    vTempStartDateTime = Convert.ToDateTime(item["StartDateTime"].ToString()); // Date
                    vTempQuantity = Convert.ToDecimal(item["Quantity"].ToString()); // Production Quantity
                    vTempSubtotal = Convert.ToDecimal(item["UnitCost"].ToString()); // Production Price
                    vTempVendorName = item["VendorName"].ToString(); // Customer Name
                    vTempVATRegistrationNo = item["VATRegistrationNo"].ToString(); // Customer VAT Reg No
                    vTempAddress1 = item["Address1"].ToString(); // Customer Address
                    vTempTransID = item["TransID"].ToString(); // Sale Invoice No
                    vTempInvoiceDateTime = Convert.ToDateTime(item["InvoiceDateTime"].ToString()); // Sale Invoice Date and Time
                    vTempBENumber = item["BENumber"].ToString(); // Sale Invoice Date and Time
                    vTempProductName = item["ProductName"].ToString(); // Sale Product Name
                    vTempSDAmount = Convert.ToDecimal(item["SD"].ToString()); // SD Amount
                    vTempVATAmount = Convert.ToDecimal(item["VATRate"].ToString()); // VAT Amount
                    vTempremarks = item["remarks"].ToString(); // Remarks
                    vTemptransType = item["TransType"].ToString().Trim();

                    #endregion Get from Datatable

                    if (vTemptransType == "Issue")
                    {
                        vat16 = new VAT_16();
                        #region if row 1 Opening
                        if (vTempremarks.Trim() == "ServiceNS"
                           || vTempremarks.Trim() == "ServiceNSImport"
                           )
                        {
                            OpeningQty = 0;
                            OpeningAmnt = 0;
                        }
                        else
                        {


                            OpeningQty = OpQty + vClosingQuantity;
                            OpeningAmnt = OpAmnt + vClosingAmount;

                        }



                        OpAmnt = 0;
                        OpQty = 0;

                        PurchaseQty = 0;
                        PurchaseAmnt = 0;
                        if (vTempremarks.Trim() == "ServiceNS"
                          || vTempremarks.Trim() == "ServiceNSImport"
                          )
                        {
                            IssueQty = 0;
                            IssueAmnt = 0;
                        }
                        else
                        {
                            IssueQty = vTempQuantity;
                            IssueAmnt = vTempSubtotal;// vTempQuantity* vClosingAvgRate;
                        }

                        if (IssueQty == 0)
                        {
                            avgRate = 0;
                        }
                        else
                        {
                            avgRate = IssueAmnt / IssueQty;
                        }
                        CloseQty =
                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                             Convert.ToDecimal(IssueQty));
                        CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                     Convert.ToDecimal(IssueAmnt));

                        vColumn2 = vTempStartDateTime;

                        vColumn3 = OpeningQty;
                        vColumn4 = OpeningAmnt;

                        vColumn5 = "";
                        vColumn6 = vTempStartDateTime;

                        vColumn7 = vTempVendorName;
                        vColumn8 = vTempAddress1;
                        vColumn9 = vTempVATRegistrationNo;
                        vColumn10 = vTempProductName;
                        vColumn11 = PurchaseQty;
                        vColumn12 = PurchaseAmnt;
                        vColumn13 = vTempSDAmount;
                        vColumn14 = vTempVATAmount;
                        vColumn15 = IssueQty;
                        vColumn16 = IssueAmnt;

                        vColumn17 = CloseQty;
                        vColumn18 = CloseAmnt;

                        vColumn19 = vTempremarks;
                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                                            Convert.ToDecimal(IssueQty));
                        if (vTempremarks.Trim() == "ServiceNS"
                          || vTempremarks.Trim() == "ServiceNSImport"
                          )
                        {
                            vClosingQuantity = 0;
                            vClosingAmount = 0;
                        }
                        else if (vClosingQuantity == 0)
                        {
                            vClosingAmount = 0;
                        }
                        else
                        {
                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                              Convert.ToDecimal(IssueAmnt));

                        }
                        if (vTempremarks.Trim() == "ServiceNS"
                          || vTempremarks.Trim() == "ServiceNSImport"
                          )
                        {
                            vClosingQuantity = 0;
                            vClosingAmount = 0;

                        }

                        #endregion

                        #region AssignValue to List
                        if (nonstock == true)
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        vat16.Column1 = vColumn1; //   
                        vat16.Column2 = vColumn2; //   
                        vat16.Column3 = vColumn3; //   
                        vat16.Column4 = vColumn4; //   
                        vat16.Column5 = vColumn5; //   
                        vat16.Column6 = vColumn6; //   
                        vat16.Column6String = ""; //   
                        vat16.Column7 = vColumn7; //   
                        vat16.Column8 = vColumn8;//    
                        vat16.Column9 = vColumn9; //   
                        vat16.Column10 = vColumn10; // 
                        vat16.Column11 = vColumn11;//  
                        vat16.Column12 = vColumn12; // 
                        vat16.Column13 = vColumn13; // 
                        vat16.Column14 = vColumn14;//  
                        vat16.Column15 = vColumn15; // 
                        vat16.Column16 = vColumn16; // 
                        vat16.Column17 = vColumn17; // 
                        vat16.Column18 = vColumn18; // 
                        vat16.Column19 = vColumn19; // 


                        vat16s.Add(vat16);
                        i = i + 1;

                        #endregion AssignValue to List

                    }
                    else if (vTemptransType == "Purchase")
                    {
                        vat16 = new VAT_16();

                        #region if row 1 Opening

                        if (vTempremarks.Trim() == "ServiceNS"
                          || vTempremarks.Trim() == "ServiceNSImport"
                          )
                        {
                            OpeningQty = 0;
                            OpeningAmnt = 0;
                        }
                        else
                        {
                            OpeningQty = OpQty + vClosingQuantity;
                            OpeningAmnt = OpAmnt + vClosingAmount;
                        }
                        OpeningQty = OpQty + vClosingQuantity;
                        OpeningAmnt = OpAmnt + vClosingAmount;
                        OpAmnt = 0;
                        OpQty = 0;

                        PurchaseQty = vTempQuantity;
                        PurchaseAmnt = vTempSubtotal;

                        IssueQty = 0;
                        IssueAmnt = 0;
                        if (PurchaseQty == 0)
                        {
                            avgRate = 0;
                        }
                        else
                        {
                            avgRate = PurchaseAmnt / PurchaseQty;
                        }


                        if (vTempremarks.Trim() == "ServiceNS"
                          || vTempremarks.Trim() == "ServiceNSImport"
                          )
                        {
                            CloseQty = 0;
                            CloseAmnt = 0;
                        }
                        else
                        {
                            CloseQty =
                           (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                            Convert.ToDecimal(IssueQty));
                            CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                         Convert.ToDecimal(IssueAmnt));
                        }



                        vColumn2 = vTempStartDateTime;
                        vColumn3 = OpeningQty;
                        vColumn4 = OpeningAmnt;
                        vColumn5 = vTempBENumber;
                        vColumn6 = vTempInvoiceDateTime;
                        vColumn7 = vTempVendorName;
                        vColumn8 = vTempAddress1;
                        vColumn9 = vTempVATRegistrationNo;
                        vColumn10 = vTempProductName;
                        vColumn11 = PurchaseQty;
                        vColumn12 = PurchaseAmnt;
                        vColumn13 = vTempSDAmount;
                        vColumn14 = vTempVATAmount;
                        vColumn15 = IssueQty;
                        vColumn16 = IssueAmnt;
                        vColumn17 = CloseQty;
                        vColumn18 = CloseAmnt;
                        vColumn19 = vTempremarks;

                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(PurchaseQty) -
                                            Convert.ToDecimal(IssueQty));
                        if (vClosingQuantity == 0)
                        {
                            vClosingAmount = 0;


                        }
                        else
                        {
                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(PurchaseAmnt) -
                                              Convert.ToDecimal(IssueAmnt));

                            vClosingAvgRate = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                        }

                        if (vTempremarks.Trim() == "ServiceNS" || vTempremarks.Trim() == "ServiceNSImport")
                        {
                            vClosingQuantity = 0;
                            vClosingAmount = 0;
                        }


                        #endregion

                        #region AssignValue to List
                        if (nonstock == true)
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        vat16.Column1 = vColumn1; //   
                        vat16.Column2 = vColumn2; //   
                        vat16.Column3 = vColumn3; //   
                        vat16.Column4 = vColumn4; //   
                        vat16.Column5 = vColumn5; //   
                        vat16.Column6 = vColumn6; //   
                        vat16.Column6String = Convert.ToDateTime(vColumn6).ToString("dd/MMM/yyyy"); //    item["UnitCost"].ToString();      // Production Price
                        vat16.Column7 = vColumn7; //   
                        vat16.Column8 = vColumn8;   // 
                        vat16.Column9 = vColumn9;   // 
                        vat16.Column10 = vColumn10; // 
                        vat16.Column11 = vColumn11; // 
                        vat16.Column12 = vColumn12; // 
                        vat16.Column13 = vColumn13; // 
                        vat16.Column14 = vColumn14; // 
                        vat16.Column15 = vColumn15; // 
                        vat16.Column16 = vColumn16; // 
                        vat16.Column17 = vColumn17; // 
                        vat16.Column18 = vColumn18; // 
                        vat16.Column19 = vColumn19; // 


                        vat16s.Add(vat16);
                        i = i + 1;

                        #endregion AssignValue to List
                    }
                }

                #endregion

                #endregion

                #region Report Preview

                #region Report Control

                string NewRegister = _cDal.settingsDesktop("Toll", "NewRegister", settingsDt, connVM);
                if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                {
                    if (NewRegister.ToLower() == "y")
                    {
                        objrpt = new RptVAT6_1_NewRegister();
                    }
                    else
                    {
                        objrpt = new RptVAT6_1();
                    }

                    ////objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_1.rpt");

                }

                #endregion

                #region Set DataSource

                objrpt.SetDataSource(vat16s);

                #endregion

                #region Preview Condition


                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }

                #endregion

                #region Settings Values

                string TollQuantity6_1 = _cDal.settingsDesktop("DecimalPlace", "TollQuantity6_1", settingsDt, connVM);
                string TollAmount6_1 = _cDal.settingsDesktop("DecimalPlace", "TollAmount6_1", settingsDt, connVM);

                #endregion

                #region Formula Fields

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", vm.FontSize.ToString());
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOMConversion", "1");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Month", Convert.ToDateTime(vm.StartDate).ToString("MMMM-yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsMonthly", "N");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FromDate", Convert.ToDateTime(vm.StartDate).ToString("dd/MM/yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ToDate", Convert.ToDateTime(vm.EndDate).ToString("dd/MM/yyyy"));

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_1", TollQuantity6_1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_1", TollAmount6_1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "HSCode", HSCode);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Toll");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductName", ProductName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductDescription", vProductVM.ProductDescription);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

                #endregion

                #endregion preview

                #endregion

                return objrpt;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument ReportToll6_2(VAT6_1ParamVM vm, bool IsBureau = false, SysDBInfoVMTemp connVM = null)
        {
            #region Variables and Objects

            string IsRaw = "";
            ReportDocument objrpt = new ReportDocument();

            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            string FromDate = vm.StartDate;
            string ToDate = vm.EndDate;
            DataSet ReportResult = new DataSet();

            CommonDAL commonDal = new CommonDAL();
            ProductDAL _pDal = new ProductDAL();
            ProductVM vProductVM = new ProductVM();

            List<VAT_17> vat17s = new List<VAT_17>();
            VAT_17 vat17 = new VAT_17();

            bool TollProduct = false;

            #endregion

            try
            {
                var model = JsonConvert.DeserializeObject<VAT6_2ParamVM>(JsonConvert.SerializeObject(vm));

                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                #region Data Call

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = reportDsdal.VAT6_2Toll(vm.ItemNo, vm.StartDate, vm.EndDate, vm.Post1, vm.Post2, vm.BranchId, connVM, null, null, model);

                #endregion

                #region Setting Data

                DataTable settingsDt = new DataTable();
                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                string vSalePlaceQty = commonDal.settingsDesktop("Sale", "QuantityDecimalPlace", settingsDt, connVM);
                int SalePlaceQty = Convert.ToInt32(vSalePlaceQty);

                string vSalePlaceTaka = commonDal.settingsDesktop("Sale", "TakaDecimalPlace", settingsDt, connVM);
                int SalePlaceTaka = Convert.ToInt32(vSalePlaceTaka);
                string FormNumeric = commonDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);

                string vSalePlaceRate = commonDal.settingsDesktop("Sale", "RateDecimalPlace", settingsDt, connVM);
                int SalePlaceRate = Convert.ToInt32(vSalePlaceRate);

                string vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", settingsDt, connVM);
                bool AutoAdjustment = Convert.ToBoolean(vAutoAdjustment == "Y" ? true : false);

                #endregion

                #region Data Call

                vProductVM = new ProductDAL().SelectAll(vm.ItemNo, null, null, null, null, null, connVM).FirstOrDefault();

                if (vProductVM.ProductType.ToLower() == "service(nonstock)")
                {
                    IsRaw = vProductVM.ProductType.ToLower();
                }

                bool nonstock = false;

                if (IsRaw.ToLower() == "service(nonstock)" || IsRaw.ToLower() == "overhead")
                {
                    nonstock = true;
                }

                #endregion

                #region Statement

                #region Check Point

                if (ReportResult.Tables.Count <= 0)
                {
                    objrpt = null;
                    return objrpt;
                }

                #endregion

                #region Variables

                decimal vColumn1 = 0;
                DateTime vColumn2 = DateTime.MinValue;
                decimal vColumn3 = 0;
                decimal vColumn4 = 0;
                decimal vColumn5 = 0;
                decimal vColumn6 = 0;
                string vColumn7 = string.Empty;
                string vColumn8 = string.Empty;
                string vColumn9 = string.Empty;
                string vColumn10 = string.Empty;
                DateTime vColumn11 = DateTime.MinValue;
                string vColumn12 = string.Empty;
                decimal vColumn13 = 0;
                decimal vColumn14 = 0;
                decimal vColumn15 = 0;
                decimal vColumn16 = 0;
                decimal vColumn17 = 0;
                decimal vColumn18 = 0;
                string vColumn19 = string.Empty;
                DateTime vTempStartDateTime = DateTime.MinValue;
                decimal vTempStartingQuantity = 0;
                decimal vTempStartingAmount = 0;
                decimal vTempQuantity = 0;
                decimal vTempSubtotal = 0;
                string vTempCustomerName = string.Empty;
                string vTempVATRegistrationNo = string.Empty;
                string vTempAddress1 = string.Empty;
                string vTempTransID = string.Empty;
                DateTime vTemptransDate = DateTime.MinValue;
                string vTempProductName = string.Empty;
                decimal vTempSDAmount = 0;
                decimal vTempVATAmount = 0;
                string vTempremarks = string.Empty;
                string vTemptransType = string.Empty;
                decimal vClosingQuantity = 0;
                decimal vClosingAmount = 0;
                decimal vClosingAvgRatet = 0;
                decimal OpeningQty = 0;
                decimal OpeningAmnt = 0;
                decimal ProductionQty = 0;
                decimal ProductionAmnt = 0;
                decimal SaleQty = 0;
                decimal SaleAmnt = 0;
                decimal CloseQty = 0;
                decimal CloseAmnt = 0;
                decimal OpQty = 0;
                decimal OpAmnt = 0;
                decimal avgRate = 0;
                string HSCode = string.Empty;
                string ProductName = string.Empty;
                decimal vClosingQuantityNew = 0;
                decimal vClosingAmountNew = 0;
                decimal vUnitRate = 0;
                decimal vAdjustmentValue = 0;

                #endregion

                int i = 1;

                #region Opening Data

                DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("transType='Opening'");

                foreach (DataRow row in DetailRawsOpening)
                {
                    ProductDAL productDal = new ProductDAL();
                    vTempremarks = row["remarks"].ToString().Trim();
                    vTemptransType = row["TransType"].ToString().Trim();
                    ProductName = row["ProductName"].ToString().Trim();
                    HSCode = row["HSCodeNo"].ToString().Trim();
                    string ItemNo = productDal.GetProductIdByName(ProductName, connVM);
                    vTempStartDateTime = Convert.ToDateTime(row["StartDateTime"].ToString().Trim());
                    decimal LastNBRPrice = productDal.GetLastNBRPriceFromBOM(ItemNo, Convert.ToDateTime(vTempStartDateTime).ToString("yyyy-MMM-dd"), null, null, "0", connVM);

                    decimal q11 = Convert.ToDecimal(row["Quantity"].ToString().Trim());
                    decimal q12 = Convert.ToDecimal(row["UnitCost"].ToString().Trim());
                    OpQty = q11;//
                    OpAmnt = q12;//
                    vat17 = new VAT_17();

                    #region if row 1 Opening

                    OpeningQty = OpQty;
                    OpeningAmnt = OpAmnt;
                    OpAmnt = 0;
                    OpQty = 0;

                    ProductionQty = 0;
                    ProductionAmnt = 0;
                    SaleQty = 0;
                    SaleAmnt = 0;

                    CloseQty = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) - Convert.ToDecimal(SaleQty));
                    CloseAmnt = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                    vColumn2 = vTempStartDateTime;
                    vColumn3 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(OpeningQty.ToString(), FormNumeric, SalePlaceQty));
                    vColumn4 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(OpeningAmnt.ToString(), FormNumeric, SalePlaceTaka));
                    vColumn5 = 0;
                    vColumn6 = 0;
                    vColumn7 = "-";
                    vColumn8 = "-";
                    vColumn9 = "-";
                    vColumn10 = "-";
                    vColumn11 = DateTime.MinValue;
                    vColumn12 = "-";
                    vColumn13 = 0;
                    vColumn14 = 0;
                    vColumn15 = 0;
                    vColumn16 = 0;
                    vColumn17 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric((vColumn3 + vColumn5 - vColumn13).ToString(), FormNumeric, SalePlaceQty));
                    vColumn18 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric((vColumn4 + vColumn6 - vColumn14).ToString(), FormNumeric, SalePlaceQty));
                    vColumn19 = vTempremarks;
                    vUnitRate = Convert.ToDecimal(row["UnitRate"].ToString().Trim());
                    vClosingQuantityNew = vColumn17;
                    vClosingAmountNew = vColumn18;

                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                        Convert.ToDecimal(SaleQty));

                    if (vClosingQuantity == 0)
                    {
                        vClosingAmount = 0;
                        vClosingAvgRatet = 0;

                    }
                    else
                    {
                        vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                          Convert.ToDecimal(SaleAmnt));
                        vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                    }

                    #endregion

                    #region AssignValue to List
                    if (nonstock == true)
                    {
                        vColumn3 = 0;
                        vColumn4 = 0;
                        vColumn5 = 0;
                        vColumn6 = 0;
                        vColumn17 = 0;
                        vColumn18 = 0;
                    }

                    vat17.Column1 = i;
                    vat17.Column2 = vColumn2;

                    vat17.Column3 = vColumn3;
                    vat17.Column4 = vColumn4;
                    vat17.Column5 = vColumn5;
                    vat17.Column6 = vColumn6;
                    vat17.Column7 = vColumn7;
                    vat17.Column8 = vColumn8;
                    vat17.Column9 = vColumn9;
                    vat17.Column10 = vColumn10;
                    vat17.Column11 = vColumn11;
                    vat17.Column11string = "";
                    vat17.Column12 = vColumn12;
                    vat17.Column13 = vColumn13;
                    vat17.Column14 = vColumn14;
                    vat17.Column15 = vColumn15;
                    vat17.Column16 = vColumn16;

                    vat17.Column17 = vColumn17;
                    vat17.Column18 = vColumn18;
                    vat17.Column19 = vColumn19;


                    vat17s.Add(vat17);
                    i = i + 1;

                    #endregion AssignValue to List

                }
                #endregion Opening

                #region Details Data

                DataRow[] DetailRawsOthers = ReportResult.Tables[0].Select("transType<>'Opening'");
                if (DetailRawsOthers == null || !DetailRawsOthers.Any())
                {
                    objrpt = null;
                    return objrpt;
                }
                string strSort = "CreatedDateTime ASC, SerialNo ASC";

                DataView dtview = new DataView(DetailRawsOthers.CopyToDataTable());
                dtview.Sort = strSort;
                DataTable dtsorted = dtview.ToTable();

                #region Process / For Loop

                foreach (DataRow item in dtsorted.Rows)
                {
                    #region Get from Datatable

                    OpeningQty = 0;
                    OpeningAmnt = 0;
                    ProductionQty = 0;
                    ProductionAmnt = 0;
                    SaleQty = 0;
                    SaleAmnt = 0;
                    CloseQty = 0;
                    CloseAmnt = 0;

                    vColumn1 = i;
                    vTempStartDateTime = Convert.ToDateTime(item["StartDateTime"].ToString()); // Date
                    vTempStartingQuantity = Convert.ToDecimal(item["StartingQuantity"].ToString()); // Opening Quantity
                    vTempStartingAmount = Convert.ToDecimal(item["StartingAmount"].ToString()); // Opening Price
                    vTempQuantity = Convert.ToDecimal(item["Quantity"].ToString()); // Production Quantity
                    vTempSubtotal = Convert.ToDecimal(item["UnitCost"].ToString()); // Production Price
                    vTempCustomerName = item["CustomerName"].ToString(); // Customer Name
                    vTempVATRegistrationNo = item["VATRegistrationNo"].ToString(); // Customer VAT Reg No
                    vTempAddress1 = item["Address1"].ToString(); // Customer Address
                    vTempTransID = item["TransID"].ToString(); // Sale Invoice No
                    vTemptransDate = Convert.ToDateTime(item["StartDateTime"].ToString()); // Sale Invoice Date and Time
                    vTempProductName = item["ProductName"].ToString(); // Sale Product Name
                    vTempSDAmount = Convert.ToDecimal(item["SD"].ToString()); // SD Amount
                    vTempVATAmount = Convert.ToDecimal(item["VATRate"].ToString()); // VAT Amount
                    vTempremarks = item["remarks"].ToString(); // Remarks
                    vTemptransType = item["TransType"].ToString().Trim();

                    #region Bureau
                    if (IsBureau == true)
                    {
                        ProductName = item["ProductName"].ToString().Trim();
                        HSCode = item["HSCodeNo"].ToString().Trim();
                    }

                    #endregion

                    #endregion Get from Datatable

                    if (vTemptransType.Trim() == "Sale")
                    {
                        vat17 = new VAT_17();
                        #region if row 1 Opening

                        if (vTempremarks.Trim() == "ServiceNS"
                           || vTempremarks.Trim() == "ServiceNSImport"
                           )
                        {
                            OpeningQty = 0;
                            OpeningAmnt = 0;
                        }
                        else
                        {
                            OpeningQty = OpQty + vClosingQuantity;
                            OpeningAmnt = OpAmnt + vClosingAmount;
                        }


                        OpAmnt = 0;
                        OpQty = 0;

                        ProductionQty = 0;
                        ProductionAmnt = 0;
                        SaleQty = vTempQuantity;
                        if (vTempremarks.Trim() == "TradingTender"
                            || vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Tender"
                            || vTempremarks.Trim() == "Export"
                            )
                        {
                            SaleAmnt = vTempSubtotal;
                        }
                        else
                        {
                            SaleAmnt = vTempSubtotal;
                        }
                        if (vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Export")
                        { }
                        else
                        {
                            SaleAmnt = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(SaleAmnt.ToString(), FormNumeric, SalePlaceTaka));
                        }


                        if (SaleQty == 0)
                        {
                            avgRate = 0;
                        }
                        else
                        {
                            avgRate = SaleAmnt / SaleQty;

                        }
                        if (vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Export")
                        { }
                        else
                        {
                            avgRate = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(avgRate.ToString(), FormNumeric, SalePlaceRate));
                        }
                        CloseQty =
                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                             Convert.ToDecimal(SaleQty));
                        CloseAmnt = CloseQty * avgRate;
                        vColumn2 = vTempStartDateTime;

                        string returnTransType = reportDsdal.GetReturnType(vm.ItemNo, vTempremarks.Trim(), connVM);

                        if (vTempremarks.Trim() == "ServiceNS"
                            || vTempremarks.Trim() == "ExportServiceNS"
                            || returnTransType == "ServiceNS"
                            )
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn5 = 0;
                            vColumn6 = 0;
                        }
                        else
                        {
                            vColumn3 = OpeningQty;
                            vColumn4 = OpeningAmnt;

                            vColumn5 = 0;
                            vColumn6 = 0;
                        }
                        vColumn7 = vTempCustomerName;
                        vColumn8 = vTempVATRegistrationNo;
                        vColumn9 = vTempAddress1;
                        vColumn10 = vTempTransID;
                        vColumn11 = vTemptransDate;
                        vColumn12 = vTempProductName;
                        if (vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Export")
                        {
                            vColumn13 = SaleQty;
                            vColumn14 = SaleAmnt;
                        }
                        else
                        {
                            vColumn13 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(SaleQty.ToString(), FormNumeric, SalePlaceQty));
                            vColumn14 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(SaleAmnt.ToString(), FormNumeric, SalePlaceTaka));
                        }
                        vColumn15 = vTempSDAmount;
                        vColumn16 = vTempVATAmount;
                        if (vTempremarks.Trim() == "ServiceNS"
                            || vTempremarks.Trim() == "ExportServiceNS"
                            || returnTransType == "ServiceNS"
                            )
                        {
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        else
                        {
                            vColumn17 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric((vColumn3 + vColumn5 - vColumn13).ToString(), FormNumeric, SalePlaceQty));

                            vColumn18 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric((vColumn4 + vColumn6 - vColumn14).ToString(), FormNumeric, SalePlaceQty));

                        }
                        vColumn19 = vTempremarks;
                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                            Convert.ToDecimal(SaleQty));

                        vClosingQuantityNew = vColumn17;
                        vClosingAmountNew = vColumn18;
                        vUnitRate = Convert.ToDecimal(item["UnitRate"].ToString().Trim());

                        if (vClosingQuantity == 0)
                        {

                            vClosingAmount = CloseAmnt;

                        }
                        else
                        {
                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                            if (vTempremarks.Trim() == "TradingTender"
                            || vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Tender"
                            || vTempremarks.Trim() == "Export"
                            )
                            {

                            }
                            else
                            {
                                vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));
                            }

                        }
                        if (vTempremarks.Trim() == "ExportTradingTender"
                            || vTempremarks.Trim() == "ExportTender"
                            || vTempremarks.Trim() == "Export")
                        {

                        }
                        else
                        {
                            vClosingAvgRatet = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(vClosingAvgRatet.ToString(), FormNumeric, SalePlaceRate));
                        }
                        #endregion
                        #region AssignValue to List
                        if (nonstock == true)
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn5 = 0;
                            vColumn6 = 0;
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        vat17.Column1 = i;
                        vat17.Column2 = vColumn2;
                        vat17.Column3 = vColumn3;
                        vat17.Column4 = vColumn4;
                        vat17.Column5 = vColumn5;
                        vat17.Column6 = vColumn6;
                        vat17.Column7 = vColumn7;
                        vat17.Column8 = vColumn8;
                        vat17.Column9 = vColumn9;
                        vat17.Column10 = vColumn10;
                        vat17.Column11 = vColumn11;
                        vat17.Column11string = Convert.ToDateTime(vColumn11).ToString("dd/MMM/yy HH:mm");

                        vat17.Column12 = vColumn12;
                        vat17.Column13 = vColumn13;
                        vat17.Column14 = vColumn14;
                        vat17.Column15 = vColumn15;
                        vat17.Column16 = vColumn16;

                        vat17.Column17 = vColumn17;
                        vat17.Column18 = vColumn18;
                        vat17.Column19 = vColumn19;
                        if (vColumn18 != vColumn17 * vUnitRate)
                        {
                            vAdjustmentValue = (vColumn17 * vUnitRate) - vColumn18;
                        }

                        vat17s.Add(vat17);
                        i = i + 1;

                        #endregion AssignValue to List

                        //// Service related company has no need auto adjustment
                        if (IsBureau == false)
                        {
                            if (avgRate == vClosingAvgRatet)
                            {
                                //vat17s.Add(vat17);
                            }
                            if (AutoAdjustment == true)
                            {
                                #region SaleAdjustment

                                if (vColumn18 != vColumn17 * vUnitRate)
                                {

                                    decimal a = 0;
                                    decimal b = 0;
                                    if (vClosingQuantity == 0)
                                    {
                                        a = avgRate;
                                        b = vClosingAvgRatet;
                                    }
                                    else
                                    {
                                        a = avgRate * vClosingQuantity;
                                        b = vClosingAvgRatet * vClosingQuantity;

                                    }

                                    decimal c = b - a;
                                    c = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(c.ToString(), FormNumeric, SalePlaceTaka));
                                    //hide 0 value row
                                    if (c != 0)
                                    {
                                        VAT_17 vat17Adj = new VAT_17();
                                        #region if row 1 Opening

                                        OpeningQty = OpQty + vClosingQuantity;
                                        OpeningAmnt = OpAmnt + vClosingAmount;
                                        OpAmnt = 0;
                                        OpQty = 0;

                                        ProductionQty = 0;
                                        ProductionAmnt = 0;
                                        SaleQty = 0;
                                        if (vTempremarks.Trim() == "TradingTender"
                                   || vTempremarks.Trim() == "ExportTradingTender"
                                   || vTempremarks.Trim() == "ExportTender"
                                   || vTempremarks.Trim() == "Tender"
                                            || vTempremarks.Trim() == "Export"
                                   )
                                        {
                                            if (vClosingQuantity == 0)
                                            {

                                                SaleAmnt = (vClosingAvgRatet - avgRate) * vTempQuantity;
                                            }
                                            else
                                            {
                                                SaleAmnt = (vTempQuantity * vClosingAvgRatet) - vTempSubtotal;

                                            }
                                        }
                                        else
                                        {
                                            if (vClosingQuantity == 0)
                                            {
                                                SaleAmnt = c * SaleQty;
                                            }
                                            else
                                            {
                                                SaleAmnt = c;
                                            }
                                        }
                                        if (vTempremarks.Trim() == "ExportTradingTender"
                                                || vTempremarks.Trim() == "ExportTender"
                                                || vTempremarks.Trim() == "Export"
                                        )
                                        {

                                        }
                                        else
                                        {
                                            SaleAmnt = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(SaleAmnt.ToString(), FormNumeric, SalePlaceTaka));
                                        }


                                        //SaleAmnt = c;

                                        if (SaleQty == 0)
                                        {
                                            avgRate = 0;
                                        }
                                        else
                                        {
                                            avgRate = SaleAmnt / SaleQty;
                                        }
                                        if (vTempremarks.Trim() == "ExportTradingTender"
                                             || vTempremarks.Trim() == "ExportTender"
                                             || vTempremarks.Trim() == "Export")
                                        {

                                        }
                                        else
                                        {
                                            avgRate = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(avgRate.ToString(), FormNumeric, SalePlaceRate));
                                        }
                                        CloseQty =
                                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                             Convert.ToDecimal(SaleQty));

                                        CloseAmnt = CloseQty * avgRate;
                                        vColumn2 = vTempStartDateTime;
                                        vColumn3 = vClosingQuantityNew;
                                        vColumn4 = vClosingAmountNew;
                                        vColumn5 = 0;
                                        vColumn6 = 0;
                                        vColumn7 = vTempCustomerName;
                                        vColumn8 = vTempVATRegistrationNo;
                                        vColumn9 = vTempAddress1;
                                        vColumn10 = vTempTransID;
                                        vColumn11 = vTemptransDate;
                                        vColumn12 = vTempProductName;
                                        if (vTempremarks.Trim() == "ExportTradingTender"
                                            || vTempremarks.Trim() == "ExportTender"
                                            || vTempremarks.Trim() == "Export")
                                        {
                                            vColumn13 = 0;
                                            vColumn14 = vAdjustmentValue;//;
                                        }
                                        else
                                        {
                                            vColumn13 = 0;
                                            vColumn14 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(vAdjustmentValue.ToString(), FormNumeric, SalePlaceTaka));
                                        }
                                        vColumn15 = 0;
                                        vColumn16 = 0;
                                        vColumn17 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric((vColumn3 + vColumn5 - vColumn13).ToString(), FormNumeric, SalePlaceQty));

                                        vColumn18 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric((vColumn4 + vColumn6 - vColumn14).ToString(), FormNumeric, SalePlaceQty));

                                        vClosingQuantityNew = vColumn17;
                                        vClosingAmountNew = vColumn18;

                                        vColumn19 = "SaleAdjustment";

                                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) - Convert.ToDecimal(SaleQty));
                                        if (vClosingQuantity == 0)
                                        {
                                            vClosingAmount = 0;
                                            vClosingAvgRatet = 0;
                                        }
                                        else
                                        {
                                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) -
                                                              Convert.ToDecimal(SaleAmnt));
                                            vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));
                                        }

                                        #endregion
                                        #region AssignValue to List
                                        if (nonstock == true)
                                        {
                                            vColumn3 = 0;
                                            vColumn4 = 0;
                                            vColumn5 = 0;
                                            vColumn6 = 0;
                                            vColumn17 = 0;
                                            vColumn18 = 0;
                                        }
                                        vat17Adj.Column1 = i;
                                        vat17Adj.Column2 = vColumn2;
                                        vat17Adj.Column3 = vColumn3;
                                        vat17Adj.Column4 = vColumn4;
                                        vat17Adj.Column5 = vColumn5;
                                        vat17Adj.Column6 = vColumn6;
                                        vat17Adj.Column7 = vColumn7;
                                        vat17Adj.Column8 = vColumn8;
                                        vat17Adj.Column9 = vColumn9;
                                        vat17Adj.Column10 = vColumn10;
                                        vat17Adj.Column11 = vColumn11;
                                        vat17Adj.Column11string = Convert.ToDateTime(vColumn11).ToString("dd/MMM/yy HH:mm");
                                        vat17Adj.Column12 = vColumn12;
                                        vat17Adj.Column13 = vColumn13;
                                        vat17Adj.Column14 = vColumn14;
                                        vat17Adj.Column15 = vColumn15;
                                        vat17Adj.Column16 = vColumn16;


                                        vat17Adj.Column17 = vColumn17;
                                        vat17Adj.Column18 = vColumn18;
                                        vat17Adj.Column19 = vColumn19;

                                        vat17Adj.Column4 = vat17.Column18;
                                        vat17s.Add(vat17Adj);

                                        i = i + 1;


                                        #endregion AssignValue to List
                                    }
                                }
                                #endregion SaleAdjustment
                            }

                        }
                    }
                    else if (vTemptransType == "Receive")
                    {
                        vat17 = new VAT_17();

                        #region if row 1 Opening

                        OpeningQty = OpQty + vClosingQuantity;
                        OpeningAmnt = OpAmnt + vClosingAmount;
                        OpAmnt = 0;
                        OpQty = 0;

                        ProductionQty = vTempQuantity;
                        ProductionAmnt = vTempSubtotal;
                        SaleQty = 0;
                        SaleAmnt = 0;
                        if (ProductionQty == 0)
                        {
                            avgRate = 0;
                        }
                        else
                        {
                            avgRate = ProductionAmnt / ProductionQty;

                        }
                        if (vTempremarks.Trim() == "ExportTradingTender"
                                   || vTempremarks.Trim() == "ExportTender"
                                   || vTempremarks.Trim() == "Export")
                        {

                        }
                        else
                        {
                            avgRate = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(avgRate.ToString(), FormNumeric, SalePlaceRate));
                        }
                        CloseQty =
                            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                             Convert.ToDecimal(SaleQty));
                        CloseAmnt = CloseQty * avgRate;
                        vColumn2 = vTempStartDateTime;
                        vColumn3 = vClosingQuantityNew;
                        vColumn4 = vClosingAmountNew;
                        vColumn5 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(ProductionQty.ToString(), FormNumeric, SalePlaceQty));
                        vColumn6 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(ProductionAmnt.ToString(), FormNumeric, SalePlaceTaka));
                        vColumn7 = "-";
                        vColumn8 = "-";
                        vColumn9 = "-";
                        vColumn10 = "-";
                        vColumn11 = Convert.ToDateTime("1900/01/01");
                        vColumn12 = "-";
                        vColumn13 = 0;
                        vColumn14 = 0;
                        vColumn15 = 0;
                        vColumn16 = 0;
                        vColumn17 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric((vColumn3 + vColumn5 - vColumn13).ToString(), FormNumeric, SalePlaceQty));
                        vColumn18 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric((vColumn4 + vColumn6 - vColumn14).ToString(), FormNumeric, SalePlaceQty));
                        vClosingQuantityNew = vColumn17;
                        vClosingAmountNew = vColumn18;
                        vUnitRate = Convert.ToDecimal(item["UnitRate"].ToString().Trim());
                        vColumn19 = vTempremarks;

                        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                            Convert.ToDecimal(SaleQty));
                        if (vClosingQuantity == 0)
                        {
                            vClosingAmount = 0;
                            vClosingAvgRatet = 0;

                        }
                        else
                        {
                            vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                            if (vTempremarks.Trim() == "TradingTender"
                           || vTempremarks.Trim() == "ExportTradingTender"
                           || vTempremarks.Trim() == "ExportTender"
                           || vTempremarks.Trim() == "Tender"
                           || vTempremarks.Trim() == "Export"
                           )
                            {
                                // change at 20150324 for Nita requierment
                                vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) /
                                                     Convert.ToDecimal(vClosingQuantity));
                            }
                            else
                            {
                                vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) /
                                                    Convert.ToDecimal(vClosingQuantity));
                            }

                        }
                        if (vTempremarks.Trim() == "ExportTradingTender"
                                    || vTempremarks.Trim() == "ExportTender"
                                    || vTempremarks.Trim() == "Export")
                        {

                        }
                        else
                        {
                            vClosingAvgRatet = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(vClosingAvgRatet.ToString(), FormNumeric, SalePlaceRate));
                        }
                        #endregion

                        #region AssignValue to List
                        if (nonstock == true)
                        {
                            vColumn3 = 0;
                            vColumn4 = 0;
                            vColumn5 = 0;
                            vColumn6 = 0;
                            vColumn17 = 0;
                            vColumn18 = 0;
                        }
                        vat17.Column1 = i; //    i.ToString();      // Serial No   
                        vat17.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                        vat17.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                        vat17.Column4 = vColumn4; //    item["StartingAmoun"].ToString();      // Opening Price
                        vat17.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                        vat17.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                        vat17.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                        vat17.Column8 = vColumn8;   //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                        vat17.Column9 = vColumn9;   //    item["Address1"].ToString();      // Customer Address
                        vat17.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                        vat17.Column11 = vColumn11; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                        vat17.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                        vat17.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                        vat17.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                        vat17.Column14 = vColumn14; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                        vat17.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                        vat17.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                        vat17.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                        vat17.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                        vat17.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks


                        vat17s.Add(vat17);
                        i = i + 1;
                        if (vColumn18 != vColumn17 * vUnitRate)
                        {
                            //AutoAdjustment = true;
                            vAdjustmentValue = (vColumn17 * vUnitRate) - vColumn18;
                        }

                        #endregion AssignValue to List

                        if (AutoAdjustment == true)
                        {
                            #region ProductionAdjustment


                            //if (avgRate != vClosingAvgRatet)
                            if (vColumn18 != vColumn17 * vUnitRate)
                            {

                                decimal a = avgRate * vClosingQuantity;         //7200
                                decimal b = vClosingAvgRatet * vClosingQuantity;//7300
                                decimal c = a - b;

                                if (vTempremarks.Trim() == "TradingTender"
                              || vTempremarks.Trim() == "ExportTradingTender"
                              || vTempremarks.Trim() == "ExportTender"
                              || vTempremarks.Trim() == "Tender"
                              || vTempremarks.Trim() == "Export"
                              )
                                {
                                    c = (vClosingAvgRatet - avgRate) * ProductionQty;
                                }
                                if (vTempremarks.Trim() == "ExportTradingTender"
                                       || vTempremarks.Trim() == "ExportTender"
                                       || vTempremarks.Trim() == "Export")
                                {

                                }
                                else
                                {
                                    c = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(c.ToString(), FormNumeric, SalePlaceTaka));
                                }
                                //hide 0 value row
                                if (c != 0)
                                {
                                    VAT_17 vat17Adj = new VAT_17();

                                    #region if row 1 Opening

                                    OpeningQty = OpQty + vClosingQuantity;
                                    OpeningAmnt = OpAmnt + vClosingAmount;
                                    OpAmnt = 0;
                                    OpQty = 0;

                                    ProductionQty = 0;
                                    ProductionAmnt = c;
                                    SaleQty = 0;
                                    SaleAmnt = 0;
                                    CloseQty =
                                        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                         Convert.ToDecimal(SaleQty));
                                    CloseAmnt = avgRate * vClosingQuantity;
                                    vColumn2 = vTempStartDateTime;
                                    vColumn3 = vClosingQuantityNew;
                                    vColumn4 = vClosingAmountNew;
                                    vColumn5 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(ProductionQty.ToString(), FormNumeric, SalePlaceQty));
                                    vColumn6 = vAdjustmentValue;
                                    vColumn7 = "-";
                                    vColumn8 = "-";
                                    vColumn9 = "-";
                                    vColumn10 = "-";
                                    vColumn11 = Convert.ToDateTime("1900/01/01");
                                    vColumn12 = "-";
                                    vColumn13 = 0;
                                    vColumn14 = 0;
                                    vColumn15 = 0;
                                    vColumn16 = 0;
                                    vColumn17 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric((vColumn3 + vColumn5 - vColumn13).ToString(), FormNumeric, SalePlaceQty));
                                    vColumn18 = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric((vColumn4 + vColumn6 - vColumn14).ToString(), FormNumeric, SalePlaceQty));
                                    vClosingQuantityNew = vColumn17;
                                    vClosingAmountNew = vColumn18;

                                    vColumn19 = "ProductionAdjustment";

                                    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ProductionQty) -
                                                        Convert.ToDecimal(SaleQty));
                                    if (vClosingQuantity == 0)
                                    {
                                        vClosingAmount = 0;
                                        vClosingAvgRatet = 0;

                                    }
                                    else
                                    {

                                        vClosingAmount = (Convert.ToDecimal(OpeningAmnt) + Convert.ToDecimal(ProductionAmnt) - Convert.ToDecimal(SaleAmnt));
                                        vClosingAvgRatet = (Convert.ToDecimal(vClosingAmount) / Convert.ToDecimal(vClosingQuantity));

                                    }

                                    #endregion

                                    #region AssignValue to List
                                    if (nonstock == true)
                                    {
                                        vColumn3 = 0;
                                        vColumn4 = 0;
                                        vColumn5 = 0;
                                        vColumn6 = 0;
                                        vColumn17 = 0;
                                        vColumn18 = 0;
                                    }
                                    vat17Adj.Column1 = i; //    i.ToString();      // Serial No   
                                    vat17Adj.Column2 = vColumn2; //    item["StartDateTime"].ToString();      // Date
                                    vat17Adj.Column3 = vColumn3; //    item["StartingQuantity"].ToString();      // Opening Quantity
                                    vat17Adj.Column4 = vColumn4; //    item["StartingAmount"].ToString();      // Opening Price
                                    vat17Adj.Column5 = vColumn5; //    item["Quantity"].ToString();      // Production Quantity
                                    vat17Adj.Column6 = vColumn6; //    item["UnitCost"].ToString();      // Production Price
                                    vat17Adj.Column7 = vColumn7; //    item["CustomerName"].ToString();      // Customer Name
                                    vat17Adj.Column8 = vColumn8;
                                    //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                                    vat17Adj.Column9 = vColumn9; //    item["Address1"].ToString();      // Customer Address
                                    vat17Adj.Column10 = vColumn10; //    item["TransID"].ToString();      // Sale Invoice No
                                    vat17Adj.Column11 = vColumn11;//    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                    vat17Adj.Column11string = ""; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                                    vat17Adj.Column12 = vColumn12; //    item["ProductName"].ToString();      // Sale Product Name
                                    vat17Adj.Column13 = vColumn13; //    item["Quantity"].ToString();      // Sale Product Quantity
                                    vat17Adj.Column14 = vColumn14;
                                    //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                                    vat17Adj.Column15 = vColumn15; //    item["SD"].ToString();      // SD Amount
                                    vat17Adj.Column16 = vColumn16; //    item["VATRate"].ToString();      // VAT Amount

                                    vat17Adj.Column17 = vColumn17; //    item["StartDateTime"].ToString();      // Closing Quantity
                                    vat17Adj.Column18 = vColumn18; //    item["StartDateTime"].ToString();      // Closing Amount
                                    vat17Adj.Column19 = vColumn19; //    item["remarks"].ToString();      // Remarks

                                    vat17s.Add(vat17Adj);
                                    i = i + 1;

                                    #endregion AssignValue to List
                                }
                            }
                            #endregion ProductionAdjustment
                        }

                    }

                }

                #endregion Process

                #endregion

                #region Report preview

                string v3Digits = commonDal.settingsDesktop("VAT6_2", "Report3Digits", settingsDt, connVM);
                string NewRegister = commonDal.settingsDesktop("Toll", "NewRegister", settingsDt, connVM);

                if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                {

                    objrpt = new ReportDocument();

                    if (NewRegister.ToLower() == "y")
                    {
                        objrpt = new RptVAT6_2_NewRegister();
                    }
                    else
                    {
                        objrpt = new RptVAT6_2();

                    }

                }

                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }

                if (TollProduct)
                {
                    objrpt.DataDefinition.FormulaFields["IsToll"].Text = "'Y'";

                }

                objrpt.SetDataSource(vat17s);

                string TollQuantity6_2 = commonDal.settingsDesktop("DecimalPlace", "TollQuantity6_2", settingsDt, connVM);
                string TollAmount6_2 = commonDal.settingsDesktop("DecimalPlace", "TollAmount6_2", settingsDt, connVM);

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", vm.FontSize.ToString());
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Month", Convert.ToDateTime(vm.StartDate).ToString("MMMM-yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsMonthly", "N");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOMConversion", "1");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FromDate", Convert.ToDateTime(vm.StartDate).ToString("dd/MM/yyyy"));
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ToDate", Convert.ToDateTime(vm.EndDate).ToString("dd/MM/yyyy"));

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

                string ProductDescription = vProductVM.ProductDescription;

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_2", TollQuantity6_2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_2", TollAmount6_2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "HSCode", HSCode);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductName", ProductName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductDescription", ProductDescription);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Toll");

                #endregion Report preview

                #endregion

                return objrpt;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #endregion

        #region Report_VAT6_6_WithMail

        public string[] Report_VAT6_6_WithMail(string VendorId, string DepositNumber, string DepositDateFrom, string DepositDateTo,
                                    string IssueDateFrom, string IssueDateTo, string BillDateFrom, string BillDateTo,
                                    string PurchaseNumber, bool chkPurchaseVDS, SysDBInfoVMTemp connVM = null, string logginUser = null, string fileDirectory = null, string fileName = null, string emailAddress = null)
        {

            string[] retResults = new string[2];

            try
            {
                #region Settings
                DataTable settingsDt = new DataTable();
                DateTime invoiceDateTime = DateTime.Now;
                string vVAT2012V2 = "2020-Jul-01";
                string VAT11Name = "";

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }
                vVAT2012V2 = commonDal.settingsDesktop("Version", "VAT2012V2", settingsDt, connVM);

                InEnglish = commonDal.settingsDesktop("Reports", "VAT6_3English", settingsDt, connVM);
                VAT11Name = commonDal.settingsDesktop("Reports", "VAT6_3", settingsDt, connVM);
                DateTime VAT2012V2 = Convert.ToDateTime(vVAT2012V2);

                #endregion

                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }

                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                ReportDSDAL reportDsdal = new ReportDSDAL();


                DataSet ReportResultDs = new DataSet();
                ReportResultDs = reportDsdal.VDS12KhaNew(VendorId, DepositNumber, DepositDateFrom, DepositDateTo, IssueDateFrom
                    , IssueDateTo, BillDateFrom, BillDateTo, PurchaseNumber, chkPurchaseVDS, connVM);
                invoiceDateTime = Convert.ToDateTime(ReportResultDs.Tables[0].Rows[0]["DepositDate"]);


                #region Complete
                if (ReportResultDs.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResultDs.Tables[0].TableName = "DsVAT12Kha";

                ReportDocument objrpt = new ReportDocument();
                if (VAT2012V2 <= invoiceDateTime)
                {
                    if (VAT11Name.ToLower() == "nnpl")
                    {
                        objrpt = new RptVAT6_6_NovoNordisk_V12V2();
                    }
                    else if (VAT11Name.ToLower() == "link3")
                    {
                        objrpt = new RptVAT6_6_Link3_V12V2();
                    }
                    else if (VAT11Name.ToUpper() == "BRACDAIRY")
                    {
                        objrpt = new RptVAT6_6_Brac_V12V2();
                    }
                    else
                    {
                        objrpt = new RptVAT6_6_V12V2();
                    }
                }
                else
                {
                    UserInformationVM uvm = new UserInformationVM();
                    UserInformationDAL _udal = new UserInformationDAL();
                    uvm = _udal.SelectAll(Convert.ToInt32(OrdinaryVATDesktop.CurrentUserID), null, null, null, null, connVM).FirstOrDefault();

                    objrpt = new RptVAT6_6();

                    objrpt.DataDefinition.FormulaFields["UserFullName"].Text = "'" + uvm.FullName + "'";
                    objrpt.DataDefinition.FormulaFields["UserDesignation"].Text = "'" + uvm.Designation + "'";
                }

                objrpt.SetDataSource(ReportResultDs);
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address11", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address21", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address31", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
                #endregion

                #region Email Process With Attachment Ready
                Stream stream = objrpt.ExportToStream(ExportFormatType.PortableDocFormat);
                string filePath = Path.Combine(fileDirectory, fileName);

                DeleteFiles(fileDirectory);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                    fileStream.Close();
                }

                EmailProcess(emailAddress, DepositNumber, fileDirectory, fileName);

                //EmailProcessWithoutSaveFile(emailAddress, DepositNumber, fileDirectory, fileName, stream);

                #endregion

                retResults[0] = "Success";
                retResults[1] = MessageVM.mailSend;

                #region Dispose & Delete
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(500);
                DeleteFiles(fileDirectory);
                #endregion

                #region VDS Mail Send Update
                VDSDAL vds = new VDSDAL();
                VDSMasterVM vm = new VDSMasterVM();
                vm.VDSId = DepositNumber;
                vm.DepositNumber = DepositNumber;
                vm.LastModifiedBy = logginUser;

                var res = vds.VDSMailSendUpdate(vm, null, null, null);
                #endregion

                return retResults;
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message.ToString();
            }

            return retResults;
        }

        private void DeleteFiles(string fileDirectory)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(fileDirectory);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void EmailProcess(string emailAddress, string DepositNumber, string fileDirectory, string fileName)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                SaleDAL _sDal = new SaleDAL();
                MailSettings ems = new MailSettings();
                CommonDAL _setDAL = new CommonDAL();
                ems.MailHeader = _setDAL.MailsettingValue("Mail", "MailSubject");
                string mailbody = _setDAL.MailsettingValue("Mail", "MailBody");

                try
                {
                    string filePath = fileDirectory + "\\" + fileName;

                    ems.MailToAddress = emailAddress;
                    ems.Port = 25;
                    ems.ServerName = "smtp.gmail.com";//smtp.gmail.com

                    ems.UserName = "sadat.abdulla@symphonysoftt.com";
                    ems.Password = "S123456_";
                    ems.MailFromAddress = ems.UserName;

                    if (!string.IsNullOrWhiteSpace(ems.MailToAddress))
                    {
                        ems.MailBody = mailbody;
                        using (var smpt = new SmtpClient())
                        {
                            smpt.EnableSsl = ems.USsel;
                            smpt.Host = ems.ServerName;
                            smpt.Port = ems.Port;

                            smpt.UseDefaultCredentials = false;
                            smpt.EnableSsl = true;
                            smpt.Credentials = new NetworkCredential(ems.UserName, ems.Password);
                            MailMessage mailmessage = new MailMessage(
                                ems.MailFromAddress,
                                ems.MailToAddress,
                                ems.MailHeader,
                                ems.MailBody);
                            mailmessage.Attachments.Add(new Attachment(filePath));

                            smpt.Send(mailmessage);
                            mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                        }

                        Thread.Sleep(500);
                    }
                }
                catch (SmtpFailedRecipientException ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ErrorLogVM evm = new ErrorLogVM();

                    evm.ImportId = "0";
                    evm.FileName = "MailSend";
                    evm.ErrorMassage = ex.Message;
                    evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    evm.SourceName = "EmailProcess";
                    evm.ActionName = "Report_VAT6_6_WithMail";
                    evm.TransactionName = "Deposit";

                    CommonDAL _cDal = new CommonDAL();

                    string[] Logresult = _cDal.InsertErrorLogs(evm);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public void EmailProcessWithoutSaveFile(string emailAddress, string DepositNumber, string fileDirectory, string fileName, Stream fileStream)
        {
            try
            {
                SaleDAL _sDal = new SaleDAL();
                MailSettings ems = new MailSettings();
                CommonDAL _setDAL = new CommonDAL();
                ems.MailHeader = _setDAL.MailsettingValue("Mail", "MailSubject");
                string mailbody = _setDAL.MailsettingValue("Mail", "MailBody");

                try
                {
                    string filePath = fileDirectory + "\\" + fileName;

                    if (!fileStream.CanRead)
                    {
                        fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    }

                    ems.MailToAddress = emailAddress;
                    ems.Port = 25;
                    ems.ServerName = "smtp.gmail.com";//smtp.gmail.com

                    ems.UserName = "sadat.abdulla@symphonysoftt.com";
                    ems.Password = "S123456_";
                    ems.MailFromAddress = ems.UserName;

                    if (!string.IsNullOrWhiteSpace(ems.MailToAddress))
                    {
                        ems.MailBody = mailbody;
                        using (var smpt = new SmtpClient())
                        {
                            smpt.EnableSsl = ems.USsel;
                            smpt.Host = ems.ServerName;
                            smpt.Port = ems.Port;

                            smpt.UseDefaultCredentials = false;
                            smpt.EnableSsl = true;
                            smpt.Credentials = new NetworkCredential(ems.UserName, ems.Password);
                            MailMessage mailmessage = new MailMessage(
                                ems.MailFromAddress,
                                ems.MailToAddress,
                                ems.MailHeader,
                                ems.MailBody);

                            mailmessage.Attachments.Add(new Attachment(fileStream, fileName));


                            smpt.Send(mailmessage);
                            mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                        }

                        Thread.Sleep(500);
                    }
                }
                catch (SmtpFailedRecipientException ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ErrorLogVM evm = new ErrorLogVM();

                    evm.ImportId = "0";
                    evm.FileName = "MailSend";
                    evm.ErrorMassage = ex.Message;
                    evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    evm.SourceName = "EmailProcess";
                    evm.ActionName = "Report_VAT6_6_WithMail";
                    evm.TransactionName = "Deposit";

                    CommonDAL _cDal = new CommonDAL();

                    string[] Logresult = _cDal.InsertErrorLogs(evm);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        #endregion

        #region meghna

        public ReportDocument MegnaTransferIssueNew(string TransferIssueNo, string IssueDateFrom, string IssueDateTo, string itemNo, string categoryID, string productType, string TransactionType, string Post, bool PreviewOnly, string DBName = null, SysDBInfoVMTemp connVM = null)
        {

            try
            {

                #region Settings
                string vVAT2012V2 = "2012-Jul-01";

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }
                InEnglish = commonDal.settingsDesktop("Reports", "VAT6_5English", settingsDt, connVM);
                VAT11Name = commonDal.settingsDesktop("Reports", "VAT6_3", settingsDt, connVM);

                string CompanyCode = commonDal.settingsDesktop("CompanyCode", "Code", settingsDt, connVM);


                string Quantity6_5 = commonDal.settingsDesktop("DecimalPlace", "Quantity6_5", settingsDt, connVM);
                string Amount6_5 = commonDal.settingsDesktop("DecimalPlace", "Amount6_5", settingsDt, connVM);
                string FontSize = commonDal.settingsDesktop("FontSize", "VAT6_5", settingsDt, connVM);
                vVAT2012V2 = commonDal.settingsDesktop("Version", "VAT2012V2", settingsDt, connVM);
                bool TrackingTrace = Convert.ToBoolean(commonDal.settingsDesktop("TrackingTrace", "Tracking", settingsDt, connVM) == "Y" ? true : false);
                string Heading1 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_1", settingsDt, connVM);
                string Heading2 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_2", settingsDt, connVM);

                #endregion

                ReportDocument result = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                DateTime TransferDateTime = DateTime.Now;
                DateTime VAT2012V2 = Convert.ToDateTime(vVAT2012V2);

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();



                ReportResult = reportDsdal.MegnaTransferIssueNew(TransferIssueNo, IssueDateFrom, IssueDateTo, itemNo, categoryID, productType, TransactionType, Post, DBName, connVM);
                TransferDateTime = Convert.ToDateTime(ReportResult.Tables[0].Rows[0]["TransferDateTime"]);


                ReportResult.Tables[0].TableName = "DsMegnaTransferIssue";

                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportDocument objrpt = new ReportDocument();
                var dal = new BranchProfileDAL();

                var toVM = dal.SelectAllList(ReportResult.Tables[0].Rows[0]["TransferTo"].ToString(), null, null, null, null, connVM).FirstOrDefault();
                var FromVM = dal.SelectAllList(ReportResult.Tables[0].Rows[0]["FromBranchId"].ToString(), null, null, null, null, connVM).FirstOrDefault();

                var legalName = toVM == null ? branchName : toVM.BranchLegalName;
                var legalName1 = FromVM == null ? branchName : FromVM.BranchLegalName;
                var fromBranchAddress = FromVM == null ? "-" : FromVM.Address;


                //objrpt = new RptVATMegna_6_5_V12V2();


                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptVATMegna_6_5_V12V2.rpt");

                objrpt = new RptVATMegna_6_5_V12V2();

                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";

                objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + legalName + "'";
                objrpt.DataDefinition.FormulaFields["FromBranchName"].Text = "'" + legalName1 + "'";
                objrpt.DataDefinition.FormulaFields["FromBranchAddress"].Text = "'" + fromBranchAddress + "'";
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                if (PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_5", Quantity6_5);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_5", Amount6_5);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VATRegNo", CompanyVm.VatRegistrationNo);



                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


    }


}
