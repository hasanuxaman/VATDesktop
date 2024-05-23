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
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using System.Data.SqlClient;

namespace SymphonySofttech.Reports
{
    public class MISReport
    {
        #region Global Variable
        public string DateFrom = "";
        public string DateTo = "";
        public bool dtpFromDate;
        public bool dtpToDate;
        public bool dtpLCFromDate;
        public bool dtpLCToDate;
        public string VendorName = "";
        public bool Local;
        public bool dtpPurchaseFromDate;
        public bool dtpPurchaseToDate;

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();


        #endregion

        public MISReport()
        {
            try
            {
                // DefaultCellStyle.Font.Name= "SutonnyMJ";mo
                connVM.SysdataSource = SysDBInfoVM.SysdataSource;
                connVM.SysPassword = SysDBInfoVM.SysPassword;
                connVM.SysUserName = SysDBInfoVM.SysUserName;
                connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public ReportDocument BankChannelMISReport(string[] conditionFields = null, string[] conditionValues = null, BankChannelPaymentVM vm = null, SysDBInfoVMTemp connVM = null)
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

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = reportDsdal.BankChannelMISReport(conditionFields, conditionValues, null, null, connVM);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "dtBankChannelPayment";

                rptBankChannelMIS objrpt = new rptBankChannelMIS();

                objrpt.SetDataSource(ReportResult);

                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Bank Channel Payment'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";

                objrpt.DataDefinition.FormulaFields["From Date"].Text = "'" + vm.PurchaseFromDate + "'";
                objrpt.DataDefinition.FormulaFields["ToDate"].Text = "'" + vm.PurchaseToDate + "'";
                objrpt.DataDefinition.FormulaFields["BankingPay"].Text = "'" + vm.bankPayRange + "'";
                objrpt.DataDefinition.FormulaFields["BankChannel"].Text = "'" + vm.IsBankingChannelPay + "'";

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

                #endregion

                return objrpt;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument TransferIssueOutReport(string IssueNo, string IssueDateFrom, string IssueDateTo, string TType, int BranchId = 0, int TransferTo = 0, string ShiftId = "0", SysDBInfoVMTemp connVM = null)
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

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.TransferIssueOutReport(IssueNo, IssueDateFrom, IssueDateTo, TType, BranchId, TransferTo, connVM, ShiftId);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "dtTransferIssue";


                rptTranserIssue objrpt = new rptTranserIssue();

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);



                objrpt.SetDataSource(ReportResult);


                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Transfer Issue Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";

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

                #endregion


                return objrpt;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument TransferReceiveInReport(string IssueNo, string IssueDateFrom, string IssueDateTo, string TType, int BranchId = 0, int BranchFromId = 0, SysDBInfoVMTemp connVM = null)
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

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.TransferReceiveInReport(IssueNo, IssueDateFrom, IssueDateTo, TType, BranchId, BranchFromId, connVM);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "dtTransferReceive";


                RptTransferReceive objrpt = new RptTransferReceive();

                objrpt.SetDataSource(ReportResult);


                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Transfer Receive  Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
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
                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ReportDocument DepositNew(string DepositNo, string DepositDateFrom, string DepositDateTo, string BankID, string Post, string transactionType, string FormNumeric = "2", SysDBInfoVMTemp connVM = null,int BranchId=0)
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

                //formula
                BranchProfileDAL _branchDAL = new BranchProfileDAL();

                string BranchName = "All";

                if (BranchId != 0)
                {
                    DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true, connVM);
                    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
                }
                //end formula

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.DepositNew(DepositNo, DepositDateFrom, DepositDateTo, BankID, Post, transactionType, connVM);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "DsDeposit";
                RptTreasuryDepositTransaction objrpt = new RptTreasuryDepositTransaction();

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion




                objrpt.SetDataSource(ReportResult);

                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Deposit Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                // objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";
                objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FormNumeric", FormNumeric);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);

                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //public ReportDocument VAT18New(string UserName, string StartDate, string EndDate, string post1, string post2)
        //{

        //    ReportDocument result = new ReportDocument();

        //    DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        //    if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
        //        Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
        //    {
        //        return null;
        //    }
        //    DataSet ReportResult = new DataSet();

        //    ReportDSDAL reportDsdal = new ReportDSDAL();
        //    ReportResult = reportDsdal.VAT18New(UserName, StartDate, EndDate, post1, post2);


        //    #region Complete
        //    if (ReportResult.Tables.Count <= 0)
        //    {
        //        return null;
        //    }
        //    ReportResult.Tables[0].TableName = "DsVAT18";
        //    ReportClass objrptNew = new ReportClass();
        //    //ReportDocument objrptNew = new ReportDocument();

        //    objrptNew = new RptVAT18();

        //    if (dtpFromDate.Value > Convert.ToDateTime("2014/6/30"))
        //    {
        //        //objrptNew.Load(Program.ReportAppPath + @"\RptVAT18_New.rpt");

        //        objrptNew = new RptVAT18_New();

        //        if (PreviewOnly == true)
        //        {
        //            objrptNew.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
        //        }
        //        else
        //        {
        //            objrptNew.DataDefinition.FormulaFields["Preview"].Text = "''";
        //        }

        //        if (Program.IsBureau == true)
        //        {
        //            string strSort = "StartDateTime ASC, TransID ASC";
        //            DataView dtView = new DataView(ReportResult.Tables[0]);
        //            dtView.Sort = strSort;
        //            DataTable dtSorted = dtView.ToTable();
        //            dtSorted.TableName = "DsVAT18";
        //            objrptNew.SetDataSource(dtSorted);
        //        }
        //        else
        //        {
        //            objrptNew.SetDataSource(ReportResult);
        //        }

        //        objrptNew.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
        //        objrptNew.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
        //        objrptNew.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
        //        objrptNew.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
        //        objrptNew.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
        //        objrptNew.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
        //        objrptNew.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

        //        FormulaFieldDefinitions crFormulaF;
        //        crFormulaF = objrptNew.DataDefinition.FormulaFields;
        //        CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

        //        _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "FontSize", Program.FontSize);

        //    #endregion


        //        return objrpt;
        //    }



        //}

        public ReportDocument Adjustment(string HeadId, string AdjType, string StartDate, string EndDate, string Post, int BranchId = 0, SysDBInfoVMTemp connVM = null)
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

                //formula
                BranchProfileDAL _branchDAL = new BranchProfileDAL();

                string BranchName = "All";

                if (BranchId != 0)
                {
                    DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true, connVM);
                    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
                }
                //end formula

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.Adjustment(HeadId, AdjType, StartDate, EndDate, Post, BranchId, connVM);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "DtAdjustment";



                ReportClass objrpt = new ReportClass();

                objrpt = new RptAdjustment();


                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Adjustment Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";

                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

                //if (dtpFromDate.Checked == false)
                //{ objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + dtpFromDate.Value.ToString("dd/MM/yyyy") + "'  "; }

                //if (dtpToDate.Checked == false)
                //{ objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + dtpToDate.Value.ToString("dd/MM/yyyy") + "'  "; }

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
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);
                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument InputOutputCoEfficient(string RawItemNo, string StartDate, string EndDate, string Post1, string Post2, SysDBInfoVMTemp connVM = null)
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

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.InputOutputCoEfficient(RawItemNo, StartDate, EndDate, Post1, Post2, connVM);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "DtInputOutputCoEfficientT1";
                ReportResult.Tables[1].TableName = "DtInputOutputCoEfficientT2";
                RptInputOutputCoEfficient objrpt = new RptInputOutputCoEfficient();

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);



                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' VAT 16 MIS Report'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";

                objrpt.DataDefinition.FormulaFields["DateFrom"].Text = "'" + DateFrom + "'  ";
                objrpt.DataDefinition.FormulaFields["DateTo"].Text = "'" + DateTo + "'  ";
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";
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
                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //public ReportDocument StockWastage(string ProductNo, string CategoryNo, string ItemType, string StartDate, string EndDate, string Post1, string Post2, bool WithoutZero = false, int BranchId = 0, string ProductName = "", string PGroup = "", bool Total = false, bool Qty = false, bool Summery = false, bool Weastage = false)
        //{
        //    try
        //    {
        //        ReportDocument result = new ReportDocument();

        //        DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        //        if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
        //            Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
        //        {
        //            return null;
        //        }
        //        DataSet ReportResult = new DataSet();

        //        ReportDSDAL reportDsdal = new ReportDSDAL();
        //        ReportResult = reportDsdal.StockWastage(ProductNo, CategoryNo, ItemType, StartDate, EndDate, Post1, Post2, WithoutZero, BranchId);


        //        #region Complete
        //        if (ReportResult.Tables.Count <= 0)
        //        {
        //            return null;
        //        }
        //        ReportClass objrpt = new ReportClass();
        //        if (Total)
        //        {
        //            if (Qty == true)
        //            {
        //                objrpt = new RptStockQty();
        //            }
        //            else
        //            {
        //                objrpt = new RptStockAll();
        //            }
        //            objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Stock Information'";
        //            if (Summery)
        //            {
        //                objrpt.DataDefinition.FormulaFields["FSummery"].Text = "'Y'";
        //            }

        //        }
        //        else if (Weastage)
        //        {
        //            objrpt = new RptWastage();
        //            objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Wastage Information'";

        //        }
        //        #region Complete

        //        DataTable dtStock = ReportResult.Tables[0].Select("ItemType <> 'Overhead'").CopyToDataTable();
        //        //ReportResult.Tables[0].TableName = "DsStockNew";
        //        //objrpt.SetDataSource(ReportResult);
        //        objrpt.SetDataSource(dtStock);


        //        objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
        //        objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
        //        objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
        //        //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
        //        //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
        //        objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
        //        objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";


        //        if (ProductName == "")
        //        {
        //            objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
        //        }
        //        else
        //        {
        //            objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + ProductName + "'  ";
        //        }


        //        if (PGroup == "")
        //        {
        //            objrpt.DataDefinition.FormulaFields["PCategory"].Text = "'[All]'";
        //        }
        //        else
        //        {
        //            objrpt.DataDefinition.FormulaFields["PCategory"].Text = "'" + PGroup +
        //                                                                    "'  ";
        //        }

        //        if (ItemType == "")
        //        {
        //            objrpt.DataDefinition.FormulaFields["PType"].Text = "'[All]'";
        //        }
        //        else
        //        {
        //            objrpt.DataDefinition.FormulaFields["PType"].Text = "'" + ItemType + "'  ";
        //        }


        //        if (dtpFromDate == false)
        //        {
        //            objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
        //        }
        //        else
        //        {
        //            objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" +
        //                                                                    StartDate +
        //                                                                    "'  ";
        //        }

        //        if (dtpToDate == false)
        //        {
        //            objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
        //        }
        //        else
        //        {
        //            objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" +
        //                                                                  EndDate +
        //                                                                  "'  ";
        //        }
        //        objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";

        //        FormulaFieldDefinitions crFormulaF;
        //        crFormulaF = objrpt.DataDefinition.FormulaFields;
        //        CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
        //        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);

        //        #endregion
        //        #endregion

        //        return objrpt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        public ReportDocument StockNew(string ProductNo, string CategoryNo, string ItemType, string StartDate
            , string EndDate, string Post1, string Post2, bool WithoutZero = false
            , bool pCategoryLike = false, string PGroup = "", int BranchId = 0, string ProductName = "", bool Total = false, bool Qty = false
            , bool Summery = false, bool Weastage = false, SysDBInfoVMTemp connVM = null
            , string FormNumeric = "2", string CurrentUserID = "", string IsTrading = "N", string ReportType = "Details"
            , string UserName = "", bool isStartDate = false, bool isEndDate = false)
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

                #region formula

                string BranchName = "All";
                int branchId = BranchId;
                if (BranchId != 0)
                {
                    DataTable dtBranch = new BranchProfileDAL().SelectAll(BranchId.ToString(), null, null, null, null);
                    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
                }

                #endregion

                #region Value assign

                StockMovementVM vm = new StockMovementVM();
                vm.ItemNo = ProductNo;
                vm.StartDate = StartDate;
                vm.ToDate = EndDate;
                vm.BranchId = BranchId;
                vm.Post1 = Post1;
                vm.Post2 = Post2;
                vm.ProductType = ItemType;
                vm.CategoryId = CategoryNo;
                vm.ProductGroupName = PGroup;
                vm.CategoryLike = pCategoryLike;
                vm.FormNumeric = FormNumeric;
                vm.CurrentUserID = CurrentUserID;
                vm.Branchwise = BranchId == 0 ? false : true;
                vm.chkTrading = IsTrading;

                #endregion

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                //ReportResult = reportDsdal.StockNew(ProductNo, CategoryNo, ItemType, StartDate, EndDate, Post1, Post2, WithoutZero, pCategoryLike, PGroup, BranchId,connVM);

                ProductDAL productDal = new ProductDAL();

                ReportResult = productDal.StockMovement(vm, null, null, connVM);

                #region Complete

                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }

                //////ReportClass objrpt = new ReportClass();

                ReportDocument objrpt = new ReportDocument();
                FormulaFieldDefinitions crFormulaF;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                #region Report Switch

                if (ReportType == "Report_1")
                {
                    objrpt = new RptStockMovement();

                }
                else if (ReportType == "Report_2")
                {
                    objrpt = new RptStockMovement2();

                }
                else if (ReportType == "Report_3")
                {
                    objrpt = new RptStockMovement3();

                }
                else if (ReportType == "Report_4")
                {
                    objrpt = new RptStockMovement4();

                }
                else
                {
                    objrpt = new RptStockMovement();
                }

                #endregion




                #region 16Jan2022

                DataTable dtStock = ReportResult.Tables[0];
                dtStock = OrdinaryVATDesktop.DtDeleteColumns(dtStock, new string[] { "ItemNo" });
                dtStock = OrdinaryVATDesktop.DtColumnNameChange(dtStock, "ProductCode", "ItemNo");
                dtStock = dtStock.Columns.Contains("ProductType") ?
                    dtStock.Select("ProductType <> 'Overhead'").CopyToDataTable() :
                    dtStock.Select("ItemType <> 'Overhead'").CopyToDataTable();

                objrpt.SetDataSource(dtStock);

                #endregion

                crFormulaF = objrpt.DataDefinition.FormulaFields;
                #region Formula Fields

                crFormulaF = objrpt.DataDefinition.FormulaFields;

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UserName", UserName);


                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOM", UOM);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOMConversion", "1");

                crFormulaF = objrpt.DataDefinition.FormulaFields;
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Stock_1", WithoutZero ? "Y" : "N");

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Stock Information");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);

                objrpt.DataDefinition.FormulaFields["Summery"].Text = Summery ? "'Y'" : "'N'";
                ////objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";
                objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

                #region Conditionals


                if (ProductName == "")
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PProduct", "[All]");
                }
                else
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PProduct", ProductName.Trim());
                }


                if (PGroup == "")
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PCategory", "[All]");
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["PCategory"].Text = "'" + PGroup.Trim() + "'  ";

                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PCategory", PGroup.Trim());

                }

                if (ItemType == "")
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PType", "[All]");

                }
                else
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PType", ItemType.Trim());

                }

                if (isStartDate == false)
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateFrom", "[All]");
                }
                else
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateFrom", Convert.ToDateTime(StartDate).ToString("dd/MMM/yyyy"));
                }

                if (isEndDate == false)
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateTo", "[All]");
                }
                else
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateTo", Convert.ToDateTime(EndDate).ToString("dd/MMM/yyyy"));
                }

                #endregion


                crFormulaF = objrpt.DataDefinition.FormulaFields;
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FormNumeric", FormNumeric);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);

                #endregion

                #region Old Code

                //if (Total)
                //{
                //    if (Qty == true)
                //    {
                //        objrpt = new RptStockQty();
                //    }
                //    else
                //    {
                //        objrpt = new RptStockAll();
                //    }

                //    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Stock Information'";

                //    if (Summery)
                //    {
                //        objrpt.DataDefinition.FormulaFields["FSummery"].Text = "'Y'";
                //    }

                //}
                //else if (Weastage)
                //{
                //    objrpt = new RptWastage();
                //    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Wastage Information'";

                //}
                #region Complete

                ////DataTable dtStock = ReportResult.Tables[0].Select("ItemType <> 'Overhead'").CopyToDataTable();



                ////////////ReportResult.Tables[0].TableName = "DsStockNew";
                ////////////objrpt.SetDataSource(ReportResult);
                //objrpt.SetDataSource(dtStock);


                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
                ////////////objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                ////////////objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //////////////objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //////////////objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                ////////////objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                ////////////objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";

                
                //if (ProductName == "")
                //{
                //    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + ProductName + "'  ";
                //}


                //if (PGroup == "")
                //{
                //    objrpt.DataDefinition.FormulaFields["PCategory"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PCategory"].Text = "'" + PGroup +
                //                                                            "'  ";
                //}

                //if (ItemType == "")
                //{
                //    objrpt.DataDefinition.FormulaFields["PType"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PType"].Text = "'" + ItemType + "'  ";
                //}


                //if (dtpFromDate == false)
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + StartDate + "'  ";
                //}

                //if (dtpToDate == false)
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + EndDate + "'  ";
                //}
                //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";

                //FormulaFieldDefinitions crFormulaF;
                //crFormulaF = objrpt.DataDefinition.FormulaFields;
                //CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

                #endregion

                #endregion

                #endregion

                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument StockNewMPL(string ProductNo, string CategoryNo, string ItemType, string StartDate
            , string EndDate, string Post1, string Post2, bool WithoutZero = false
            , bool pCategoryLike = false, string PGroup = "", int BranchId = 0, string ProductName = "",
            bool Total = false, bool Qty = false
            , bool Summery = false, bool Weastage = false, SysDBInfoVMTemp connVM = null
            , string FormNumeric = "2", string CurrentUserID = "", string IsTrading = "N", string ReportType = "Details"
            , string UserName = "", bool isStartDate = false, bool isEndDate = false, string ZoneID = null)
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

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM)
                    .FirstOrDefault();

                #endregion

                #region formula

                string BranchName = "All";
                int branchId = BranchId;
                if (BranchId != 0)
                {
                    DataTable dtBranch = new BranchProfileDAL().SelectAll(BranchId.ToString(), null, null, null, null);
                    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
                }

                #endregion

                #region Value assign

                StockMovementVM vm = new StockMovementVM();
                vm.ItemNo = ProductNo;
                vm.StartDate = StartDate;
                vm.ToDate = EndDate;
                vm.BranchId = BranchId;
                vm.Post1 = Post1;
                vm.Post2 = Post2;
                vm.ProductType = ItemType;
                vm.CategoryId = CategoryNo;
                vm.ProductGroupName = PGroup;
                vm.CategoryLike = pCategoryLike;
                vm.FormNumeric = FormNumeric;
                vm.CurrentUserID = CurrentUserID;
                vm.Branchwise = BranchId == 0 ? false : true;
                vm.chkTrading = IsTrading;
                vm.ZoneID = ZoneID;
                vm.WithOutZero = WithoutZero;



                #endregion

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ProductDAL productDal = new ProductDAL();

                ReportResult = productDal.StockMovementMPL(vm, null, null, connVM);

                #region Complete

                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }


                ReportDocument objrpt = new ReportDocument();
                FormulaFieldDefinitions crFormulaF;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                #region Report Switch

                objrpt = new RptMPLStockMovement();

                #endregion

                #region 16Jan2022

                DataTable dtStock = ReportResult.Tables[0];

                objrpt.SetDataSource(dtStock);

                #endregion

                crFormulaF = objrpt.DataDefinition.FormulaFields;

                #region Formula Fields

                crFormulaF = objrpt.DataDefinition.FormulaFields;

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UserName", UserName);


                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOMConversion", "1");

                crFormulaF = objrpt.DataDefinition.FormulaFields;
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Stock_1", WithoutZero ? "Y" : "N");

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Stock Information");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);

                objrpt.DataDefinition.FormulaFields["Summery"].Text = Summery ? "'Y'" : "'N'";
                objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

                #region Conditionals


                if (ProductName == "")
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PProduct", "[All]");
                }
                else
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PProduct", ProductName.Trim());
                }


                if (PGroup == "")
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PCategory", "[All]");
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["PCategory"].Text = "'" + PGroup.Trim() + "'  ";

                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PCategory", PGroup.Trim());

                }

                if (string.IsNullOrEmpty(ItemType))
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PType", "[All]");

                }
                else
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PType", ItemType.Trim());

                }

                if (isStartDate == false)
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateFrom", "[All]");
                }
                else
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateFrom",
                        Convert.ToDateTime(StartDate).ToString("dd/MMM/yyyy"));
                }

                if (isEndDate == false)
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateTo", "[All]");
                }
                else
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateTo",
                        Convert.ToDateTime(EndDate).ToString("dd/MMM/yyyy"));
                }

                #endregion


                crFormulaF = objrpt.DataDefinition.FormulaFields;
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FormNumeric", FormNumeric);

                #endregion


                #endregion

                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ReportDocument PurchaseWithLCInfo(string PurchaseInvoiceNo, string LCDateFrom, string LCDateTo, string VendorId, string ItemNo, string VendorGroupId, string LCNo, string Post, SysDBInfoVMTemp connVM = null)
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

                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.PurchaseWithLCInfo(PurchaseInvoiceNo, LCDateFrom, LCDateTo, VendorId, ItemNo, VendorGroupId, LCNo, Post, connVM);


                #region Complete
                if (ReportResult.Tables.Count <= 0)
                {
                    return null;
                }
                ReportClass objrpt = new ReportClass();

                ReportResult.Tables[0].TableName = "DsPurchaseLC";

                objrpt = new RptPurchase_LCInfo();
                objrpt.SetDataSource(ReportResult);
                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);






                #region Formulla
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";

                if (LCNo == "")
                { objrpt.DataDefinition.FormulaFields["LCNo"].Text = "'[All]'"; }
                else
                { objrpt.DataDefinition.FormulaFields["LCNo"].Text = "'" + LCNo + "'  "; }

                if (VendorName == "")
                { objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'[All]'"; }
                else
                { objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'" + VendorName + "'  "; }

                if (PurchaseInvoiceNo == "")
                { objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'[All]'"; }
                else
                { objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'" + PurchaseInvoiceNo + "'  "; }

                if (dtpLCFromDate == false)
                { objrpt.DataDefinition.FormulaFields["LCDateFrom"].Text = "'[All]'"; }
                else
                { objrpt.DataDefinition.FormulaFields["LCDateFrom"].Text = "'" + LCDateFrom + "'  "; }

                if (dtpLCToDate == false)
                { objrpt.DataDefinition.FormulaFields["LCDateTo"].Text = "'[All]'"; }
                else
                { objrpt.DataDefinition.FormulaFields["LCDateTo"].Text = "'" + LCDateTo + "'  "; }

                #endregion Formulla

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
                #endregion


                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //PurchaseNew(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
        //                     string VendorId, string ItemNo, string CategoryID, string ProductType,
        //                     string TransactionType, string Post,
        //                     string PurchaseType, string VendorGroupId, string FromBOM, string UOM, string UOMn,
        //                     decimal UOMc, decimal TotalQty, decimal rCostPrice, bool pCategoryLike = false, string PGroup = "", int BranchId = 0, string VatType = ""

        //public ReportDocument PurchaseNew(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
        //                     string VendorId, string ItemNo, string CategoryID, string ProductType,
        //                     string TransactionType, string Post,
        //                     string PurchaseType, string VendorGroupId, string FromBOM, string UOM, string UOMn,
        //                     decimal UOMc, decimal TotalQty, decimal rCostPrice, bool pCategoryLike = false, string PGroup = "", int BranchId = 0, string VatType = "", string ReportType = "", string reportName = "", string ProductName = "", string VendorName = "")
        //{


        //    ReportDocument result = new ReportDocument();

        //    DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        //    if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
        //        Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
        //    {
        //        return null;
        //    }
        //    DataSet ReportResult = new DataSet();

        //    ReportDSDAL reportDsdal = new ReportDSDAL();
        //    ReportResult = reportDsdal.PurchaseNew(PurchaseInvoiceNo, InvoiceDateFrom, InvoiceDateTo, VendorId, ItemNo, CategoryID, ProductType, TransactionType, Post, PurchaseType, VendorGroupId, FromBOM, UOM, UOMn, UOMc, TotalQty, rCostPrice, pCategoryLike, PGroup, BranchId, VatType);


        //    #region Complete
        //    if (ReportResult.Tables.Count <= 0)
        //    {
        //        return null;
        //    }
        //    if (string.IsNullOrEmpty(reportName))
        //    {
        //        reportName = "All";
        //    }
        //    ReportClass objrpt = new ReportClass();

        //    ReportResult.Tables[0].TableName = "DsPurchase";
        //    if (ReportType == "summary")
        //    {
        //        objrpt = new RptPurchaseSummery();
        //        objrpt.SetDataSource(ReportResult);
        //        reportName = reportName + " (summary)";
        //        objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + reportName + "'";
        //    }
        //    else if (ReportType == "summarybyproduct")
        //    {
        //        objrpt = new RptPurchaseSummeryByProduct();
        //        objrpt.SetDataSource(ReportResult);
        //        reportName = reportName + " (summary)";
        //        objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + reportName + "'";
        //    }
        //    else
        //    {
        //        ReportResult.Tables[0].TableName = "DsPurchase";
        //        if (Local == false)
        //        {
        //            objrpt = new RptPurchaseTransaction();
        //        }
        //        else if (Local == true)
        //        {
        //            objrpt = new RptPurchaseTransaction_Duty();
        //        }
        //        objrpt.SetDataSource(ReportResult);
        //        objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + reportName + "'";
        //    }


        //    //}
        //    #region Formulla
        //    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
        //    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
        //    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
        //    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
        //    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";

        //    if (ProductName == "")
        //    { objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'"; }
        //    else
        //    { objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + ProductName + "'  "; }

        //    if (VendorName == "")
        //    { objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'[All]'"; }
        //    else
        //    { objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'" + VendorName + "'  "; }

        //    if (PurchaseInvoiceNo == "")
        //    { objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'[All]'"; }
        //    else
        //    { objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'" + PurchaseInvoiceNo + "'  "; }

        //    if (dtpPurchaseFromDate == false)
        //    { objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'"; }
        //    else
        //    { objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + InvoiceDateFrom + "'  "; }

        //    if (dtpPurchaseToDate == false)
        //    { objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'"; }
        //    else
        //    { objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + InvoiceDateTo + "'  "; }

        //    #endregion Formulla

        //    FormulaFieldDefinitions crFormulaF;
        //    crFormulaF = objrpt.DataDefinition.FormulaFields;
        //    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
        //    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);

        //    #endregion


        //    return objrpt;
        //}


        public ReportDocument PurchaseInformation(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
                            string VendorId, string ItemNo, string CategoryID, string ProductType,
                            string TransactionType, string Post,
                            string PurchaseType, string VendorGroupId, string FromBOM, string UOM, string UOMn,
                            decimal UOMc, decimal TotalQty, decimal rCostPrice, bool pCategoryLike = false, string PGroup = "", int BranchId = 0, string VatType = "", string ReportType = "", string reportName = "", string ProductName = "", string VendorName = "", SysDBInfoVMTemp connVM = null)
        {


            DataTable settingsDt = new DataTable();

            if (settingVM.SettingsDTUser == null)
            {
                settingsDt = new CommonDAL().SettingDataAll(null, null);
            }
            string FormNumeric = new CommonDAL().settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);
            #region CompanyInformation

            CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

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
            ReportResult = reportDsdal.PurchaseNew(PurchaseInvoiceNo, InvoiceDateFrom, InvoiceDateTo, VendorId, ItemNo, CategoryID, ProductType, TransactionType
                , Post, PurchaseType, VendorGroupId, FromBOM, UOM, UOMn, UOMc, TotalQty, rCostPrice, pCategoryLike, PGroup, BranchId, VatType, "",connVM);


            #region Complete
            if (ReportResult.Tables.Count <= 0)
            {
                return null;
            }

            ReportClass objrpt = new ReportClass();

            ReportResult.Tables[0].TableName = "DsPurchaseExtention";

            objrpt = new RptPurchaseInformation();

            objrpt.SetDataSource(ReportResult);

            FormulaFieldDefinitions crFormulaF;
            crFormulaF = objrpt.DataDefinition.FormulaFields;
            CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FormNumeric", FormNumeric);

            #endregion


            return objrpt;
        }

        public ReportDocument IssueNew(string IssueNo, string IssueDateFrom, string IssueDateTo, string itemNo,
                                string categoryID, string productType, string TransactionType, string Post, string waste,
            bool pCategoryLike = false, string PGroup = "", int BranchId = 0, string ProductName = "", string reportType = "", string FormNumeric = "2", SysDBInfoVMTemp connVM = null)
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

            //formula
            BranchProfileDAL _branchDAL = new BranchProfileDAL();

            string BranchName = "All";

            if (BranchId != 0)
            {
                DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true, connVM);
                BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
            }
            //end formula

            DataSet ReportResult = new DataSet();


            ReportDSDAL reportDsdal = new ReportDSDAL();
            ReportResult = reportDsdal.IssueNew(
                  IssueNo
                  , IssueDateFrom
                  , IssueDateTo
                  , itemNo
                  , categoryID
                  , productType
                  , TransactionType
                  , Post
                  , waste
                  , pCategoryLike, PGroup
                  , BranchId, connVM);


            #region Complete
            if (ReportResult.Tables.Count <= 0)
            {
                return null;
            }

            ReportClass objrpt = new ReportClass();

            if (reportType == "Summary")
            {
                ReportResult.Tables[0].TableName = "DsIssue";

                objrpt = new RptIssueSummery();
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Raw product issue summery Information'";

            }
            else if (reportType == "Detail")
            {
                ReportResult.Tables[0].TableName = "DsIssueExtention";

                objrpt = new RptIssue();
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Raw product issue information'";


            }

            #region Settings Load

            CommonDAL _cDal = new CommonDAL();

            DataTable settingsDt = new DataTable();

            if (settingVM.SettingsDTUser == null)
            {
                settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
            }

            #endregion




            objrpt.SetDataSource(ReportResult);
            objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
            //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
            //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
            //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
            //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
            //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
            //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
            objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
            //objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";
            objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";


            objrpt.DataDefinition.FormulaFields["PQty"].Text = "'" + waste + "'";

            if (ProductName == "")
            {
                objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + ProductName + "'  ";
            }
            if (IssueNo == "")
            {
                objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'" + IssueNo + "'  ";
            }
            if (string.IsNullOrWhiteSpace(IssueDateFrom))
            {
                objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" +
                                                                        IssueDateFrom +
                                                                        "'  ";
            }
            if (string.IsNullOrWhiteSpace(IssueDateTo))
            {
                objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" +
                                                                      IssueDateTo +
                                                                      "'  ";
            }

            FormulaFieldDefinitions crFormulaF;
            crFormulaF = objrpt.DataDefinition.FormulaFields;
            CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FormNumeric", FormNumeric);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);


            #endregion


            return objrpt;

        }

        public ReportDocument IssueMis(string IssueId, int BranchId, SysDBInfoVMTemp connVM = null)
        {

            ReportDocument result = new ReportDocument();

            //formula
            BranchProfileDAL _branchDAL = new BranchProfileDAL();

            string BranchName = "All";

            if (BranchId != 0)
            {
                DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true, connVM);
                BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
            }
            //end formula

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
            ReportResult = reportDsdal.IssueMis(
                  IssueId
                  , BranchId, connVM);


            #region Complete
            if (ReportResult.Tables.Count <= 0)
            {
                return null;
            }

            ReportClass objrpt = new ReportClass();

            ReportResult.Tables[0].TableName = "DsIssueHeaders";
            ReportResult.Tables[1].TableName = "DsIssueDetails";

            #region Settings Load

            CommonDAL _cDal = new CommonDAL();

            DataTable settingsDt = new DataTable();

            if (settingVM.SettingsDTUser == null)
            {
                settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
            }

            #endregion

            string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);




            objrpt = new RptMISIssue();

            objrpt.SetDataSource(ReportResult);

            objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
            objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";
            objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";


            FormulaFieldDefinitions crFormulaF;
            crFormulaF = objrpt.DataDefinition.FormulaFields;
            CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);

            #endregion


            return objrpt;


        }


        public ReportDocument ReceiveNew(string ReceiveNo, string ReceiveDateFrom, string ReceiveDateTo, string itemNo,
                                  string categoryID, string productType, string transactionType, string post, string ShiftId = "0",
        int BranchId = 0, string ProductName = "", string reportType = "", string FormNumeric = "2", SysDBInfoVMTemp connVM = null)
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


            DataSet ReportResult = new DataSet();

            //formula
            BranchProfileDAL _branchDAL = new BranchProfileDAL();

            string BranchName = "All";

            if (BranchId != 0)
            {
                DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true, connVM);
                BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
            }
            //end formula

            ReportDSDAL reportDsdal = new ReportDSDAL();
            ReportResult = reportDsdal.ReceiveNew(
                  ReceiveNo
                  , ReceiveDateFrom
                  , ReceiveDateTo
                  , itemNo
                  , categoryID
                  , productType
                  , transactionType
                  , post
                  , ShiftId
                  , BranchId, connVM);


            #region Complete
            if (ReportResult.Tables.Count <= 0)
            {
                return null;
            }

            ReportClass objrpt = new ReportClass();

            ReportResult.Tables[0].TableName = "DsReceive";
            if (reportType == "Summary")
            {
                objrpt = new RptReceivingSummery();
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Raw product issue summery Information'";

            }
            else if (reportType == "Detail")
            {
                objrpt = new RptReceivingReport();
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Raw product issue information'";


            }

            #region Settings Load

            CommonDAL _cDal = new CommonDAL();

            DataTable settingsDt = new DataTable();

            if (settingVM.SettingsDTUser == null)
            {
                settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
            }

            #endregion


            objrpt.SetDataSource(ReportResult);
            objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
            objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Receive Information'";
            //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
            //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
            //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
            //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
            //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
            //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
            objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
            objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";
            objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

            if (ProductName == "")
            {
                objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + ProductName + "'  ";
            }


            if (ReceiveNo == "")
            {
                objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'" + ReceiveNo + "'  ";
            }

            if (ReceiveDateFrom == "")
            {
                objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + ReceiveDateFrom + "'  ";
            }

            if (ReceiveDateTo == "")
            {
                objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + ReceiveDateTo + "'  ";
            }


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
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);
            #endregion


            return objrpt;



        }

        public ReportDocument ReceiveMis(string ReceiveId, string ShiftId = "0", int BranchId = 0, SysDBInfoVMTemp connVM = null)
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

            DataSet ReportResult = new DataSet();

            ReportDSDAL reportDsdal = new ReportDSDAL();
            ReportResult = reportDsdal.ReceiveMis(
                  ReceiveId
                  , ShiftId
                  , BranchId, connVM);


            #region Complete
            if (ReportResult.Tables.Count <= 0)
            {
                return null;
            }

            ReportClass objrpt = new ReportClass();

            ReportResult.Tables[0].TableName = "DsReceiveHeader";
            //ReportResult.Tables[0].TableName = "DsReceiveDetails";
            objrpt = new RptMISReceive();
            objrpt.SetDataSource(ReportResult);


            //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
            //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
            //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
            //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";


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
            #endregion


            return objrpt;




        }


        public ReportDocument Wastage(string ItemNo, string CategoryID, string ProductType, string post1, string post2, string StartDate, string EndDate, int branchId = 0, string rDateFrom = "", string rDateTo = "", string ProductName = "", string rProductType = "", SysDBInfoVMTemp connVM = null)
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
            ReportResult = reportDsdal.Wastage(ItemNo, CategoryID, ProductType, post1, post2, StartDate, EndDate, branchId, connVM);

            ReportResult.Tables[0].TableName = "dtWastage";

            ReportClass objrpt = new ReportClass();

            objrpt = new RptWastage();

            #region Settings Load

            //formula
            BranchProfileDAL _branchDAL = new BranchProfileDAL();

            string BranchName = "All";

            if (branchId != 0)
            {
                DataTable dtBranch = _branchDAL.SelectAll(branchId.ToString(), null, null, null, null, true,connVM);
                BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
            }
            //end formula


            CommonDAL _cDal = new CommonDAL();

            DataTable settingsDt = new DataTable();

            if (settingVM.SettingsDTUser == null)
            {
                settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
            }

            #endregion

            string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);




            objrpt.SetDataSource(ReportResult);


            FormulaFieldDefinitions crFormulaF;
            crFormulaF = objrpt.DataDefinition.FormulaFields;
            CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);

            objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Wastage Information'";
            objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
            objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
            objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";

            objrpt.DataDefinition.FormulaFields["FromDate"].Text = "'" + rDateFrom + "'";
            objrpt.DataDefinition.FormulaFields["ToDate"].Text = "'" + rDateTo + "'";
            objrpt.DataDefinition.FormulaFields["ProductName"].Text = "'" + ProductName + "'";
            objrpt.DataDefinition.FormulaFields["ProductType"].Text = "'" + rProductType + "'";
            objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";
            objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";
            return objrpt;

        }


        public ReportDocument VDSReport(string DepositNo, string DepositDateFrom, string DepositDateTo, string BankID, string Post,
                                 string transactionType, string VendorId, string Report = "", SysDBInfoVMTemp connVM = null)
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

            DataSet ReportResult = new DataSet();

            ReportDSDAL reportDsdal = new ReportDSDAL();
            CommonDAL _cDal = new CommonDAL();
            CommonDAL commonDal = new CommonDAL();
            DataTable settingsDt = new DataTable();

            ReportResult = reportDsdal.VDSReport(DepositNo, DepositDateFrom, DepositDateTo, BankID, Post, transactionType, VendorId, connVM);

            ReportResult.Tables[0].TableName = "DsVDS";
            ReportClass objrpt = new ReportClass();                // Start Complete
            string CompanyCode = commonDal.settingValue("CompanyCode", "Code", connVM);
            //string CompanyCode = _cDal.settingsDesktop("CompanyCode", "Code", settingsDt, connVM);
            if (Report == "VDS-TR6")
            {

                if (CompanyCode.ToLower() == "itl")
                {
                    objrpt = new RptVDS_TR6_ITL();

                }
                else
                {
                    objrpt = new RptVDS_TR6();

                }

            }
            else
            {
                objrpt = new RptVDS();
            }

            #region Settings Load


            if (settingVM.SettingsDTUser == null)
            {
                settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
            }

            #endregion

            string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);

            objrpt.SetDataSource(ReportResult);

            //objrpt.DataDefinition.FormulaFields["Company Name"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
            //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
            //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
            objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";


            FormulaFieldDefinitions crFormulaF;
            crFormulaF = objrpt.DataDefinition.FormulaFields;
            CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Company Name", CompanyVm.CompanyLegalName);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

            return objrpt;



        }


        public ReportDocument SaleReceiveMIS(string StartDate, string EndDate, string ShiftId = "0", string Post = null, string Toll = "N", string FormNumeric = "2", SysDBInfoVMTemp connVM = null)
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

            DataSet ReportResult = new DataSet();

            ReportDSDAL reportDsdal = new ReportDSDAL();
            ReportResult = reportDsdal.SaleReceiveMIS(StartDate, EndDate, ShiftId, Post, connVM, Toll);


            ReportDocument objrpt = new ReportDocument();

            ReportResult.Tables[0].TableName = "dtSaleReceive";
            objrpt = new RptSaleReceive();
            //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report" + @"\RptSaleReceive.rpt");


            objrpt.SetDataSource(ReportResult);
            //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
            objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
            //objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";


            string ShiftName = "[All]";
            if (Convert.ToInt32(ShiftId) <= 1)
            {
                ShiftName = "[All]";
            }
            objrpt.DataDefinition.FormulaFields["shift"].Text = "'" + ShiftName + "'";

            if (StartDate == "")
            {
                objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + StartDate + "'  ";
            }

            if (EndDate == "")
            {
                objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + EndDate + "'  ";
            }
            FormulaFieldDefinitions crFormulaF;
            crFormulaF = objrpt.DataDefinition.FormulaFields;
            CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FormNumeric", FormNumeric);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);



            return objrpt;


        }


        //public ReportDocument ProductNew(string ItemNo, string CategoryID, string IsRaw, SysDBInfoVMTemp connVM = null)
        //{

        //}


        public ReportDocument VAT18(string UserName, string StartDate, string EndDate, string post1, string post2, bool PreviewOnly = false, SysDBInfoVMTemp connVM = null)
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

            DataSet ReportResult = new DataSet();

            ReportDSDAL reportDsdal = new ReportDSDAL();

            if (Convert.ToDateTime(StartDate) > Convert.ToDateTime("2014/6/30"))
            {
                ReportResult = reportDsdal.VAT18New(UserName, StartDate, EndDate, post1, post2, connVM);

            }
            else
            {
                ReportResult = reportDsdal.VAT18_OldFormat(UserName, StartDate, EndDate, post1, post2, connVM);

            }


            ReportResult.Tables[0].TableName = "DsVAT18";
            ReportClass objrptNew = new ReportClass();

            objrptNew = new RptVAT18();

            if (Convert.ToDateTime(StartDate) > Convert.ToDateTime("2014/6/30"))
            {

                objrptNew = new RptVAT18_New();

                if (PreviewOnly == true)
                {
                    objrptNew.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrptNew.DataDefinition.FormulaFields["Preview"].Text = "''";
                }

                objrptNew.SetDataSource(ReportResult);


                //objrptNew.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //objrptNew.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrptNew.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                //objrptNew.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                //objrptNew.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                //objrptNew.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                //objrptNew.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrptNew.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

            }
            else
            {


                if (PreviewOnly == true)
                {
                    objrptNew.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrptNew.DataDefinition.FormulaFields["Preview"].Text = "''";
                }

                objrptNew.SetDataSource(ReportResult);


                //objrptNew.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                //objrptNew.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                //objrptNew.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                //objrptNew.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                //objrptNew.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                //objrptNew.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                //objrptNew.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrptNew.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
            }


            return objrptNew;


        }

        public ReportDocument VAT18_IsBureau(string UserName, string StartDate, string EndDate, string post1, string post2, bool PreviewOnly = false, string FormNumeric = "2", SysDBInfoVMTemp connVM = null)
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

            if (Convert.ToDateTime(StartDate) > Convert.ToDateTime("2014/6/30"))
            {
                ReportResult = reportDsdal.BureauVAT18Report(UserName, StartDate, EndDate, post1, post2, connVM);

            }
            else
            {
                ReportResult = reportDsdal.BureauVAT18_OldFormat(UserName, StartDate, EndDate, post1, post2, connVM);

            }



            ReportResult.Tables[0].TableName = "DsVAT18";
            ReportClass objrptNew = new ReportClass();

            objrptNew = new RptVAT18();

            if (Convert.ToDateTime(StartDate) > Convert.ToDateTime("2014/6/30"))
            {

                objrptNew = new RptVAT18_New();

                if (PreviewOnly == true)
                {
                    objrptNew.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrptNew.DataDefinition.FormulaFields["Preview"].Text = "''";
                }

                string strSort = "StartDateTime ASC, TransID ASC";
                DataView dtView = new DataView(ReportResult.Tables[0]);
                dtView.Sort = strSort;
                DataTable dtSorted = dtView.ToTable();
                dtSorted.TableName = "DsVAT18";
                objrptNew.SetDataSource(dtSorted);

                objrptNew.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                objrptNew.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                objrptNew.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                objrptNew.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                objrptNew.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                objrptNew.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                objrptNew.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrptNew.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "FormNumeric", FormNumeric);



            }
            else
            {


                if (PreviewOnly == true)
                {
                    objrptNew.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrptNew.DataDefinition.FormulaFields["Preview"].Text = "''";
                }

                string strSort = "StartDateTime ASC, TransID ASC";
                DataView dtView = new DataView(ReportResult.Tables[0]);
                dtView.Sort = strSort;
                DataTable dtSorted = dtView.ToTable();
                dtSorted.TableName = "DsVAT18";
                objrptNew.SetDataSource(dtSorted);


                objrptNew.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                objrptNew.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                objrptNew.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                objrptNew.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                objrptNew.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                objrptNew.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                objrptNew.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrptNew.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);

            }


            return objrptNew;


        }

        public ReportDocument VAT18_Sanofi(string UserName, string StartDate, string EndDate, string post1, string post2, bool PreviewOnly = false, SysDBInfoVMTemp connVM = null)
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

            ReportResult = reportDsdal.VAT18_Sanofi(UserName, StartDate, EndDate, post1, post2, connVM);


            ReportResult.Tables[0].TableName = "DsVAT18";
            ReportClass objrptNew = new ReportClass();

            objrptNew = new RptVAT18();

            if (Convert.ToDateTime(StartDate) > Convert.ToDateTime("2014/6/30"))
            {

                objrptNew = new RptVAT18_New();

                if (PreviewOnly == true)
                {
                    objrptNew.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrptNew.DataDefinition.FormulaFields["Preview"].Text = "''";
                }

                objrptNew.SetDataSource(ReportResult);


                objrptNew.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                objrptNew.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                objrptNew.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                objrptNew.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                objrptNew.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                objrptNew.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                objrptNew.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrptNew.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);


            }
            else
            {


                if (PreviewOnly == true)
                {
                    objrptNew.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrptNew.DataDefinition.FormulaFields["Preview"].Text = "''";
                }

                objrptNew.SetDataSource(ReportResult);


                objrptNew.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                objrptNew.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                objrptNew.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                objrptNew.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                objrptNew.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                objrptNew.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                objrptNew.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrptNew.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                _vCommonFormMethod.FormulaField(objrptNew, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);

            }


            return objrptNew;



        }

        //public ReportDocument SerialStockStatus(string ItemNo, string CategoryID, string ProductType, string StartDate, string ToDate, string post1, string post2, string Heading1, string Heading2)
        //{

        //    ReportDocument result = new ReportDocument();

        //    DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        //    if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
        //        Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
        //    {
        //        return null;
        //    }
        //    DataSet ReportResult = new DataSet();

        //    ReportDSDAL reportDsdal = new ReportDSDAL();
        //    ReportResult = reportDsdal.SerialStockStatus(ItemNo, CategoryID, ProductType, StartDate, ToDate, post1, post2);



        //    ReportResult.Tables[0].TableName = "DsSerialStock";
        //    RptSerialStockStatus objrpt = new RptSerialStockStatus();
        //    objrpt.SetDataSource(ReportResult);

        //    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
        //    objrpt.DataDefinition.FormulaFields["Heading1"].Text = "'" + Heading1 + "'";
        //    objrpt.DataDefinition.FormulaFields["Heading2"].Text = "'" + Heading2 + "'";

        //    if (StartDate == "")
        //    {
        //        objrpt.DataDefinition.FormulaFields["FromDate"].Text = "'[All]'";
        //        objrpt.DataDefinition.FormulaFields["FromDate"].Text = "'-'";
        //    }
        //    else
        //    {
        //        objrpt.DataDefinition.FormulaFields["FromDate"].Text = "'" + StartDate + "'  ";
        //    }

        //    if (ToDate == "")
        //    {
        //        objrpt.DataDefinition.FormulaFields["ToDate"].Text = "'-'";
        //    }
        //    else
        //    {
        //        objrpt.DataDefinition.FormulaFields["ToDate"].Text = "'" + ToDate +"' ";
        //    }

        //    FormulaFieldDefinitions crFormulaF;
        //    crFormulaF = objrpt.DataDefinition.FormulaFields;
        //    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();



        //    return objrpt;


        //}


        public ReportDocument PurchaseNew(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, string VendorId, string ItemNo, string CategoryID,
            string ProductType, string TransactionType, string Post, string PurchaseType, string VendorGroupId, string FromBOM, string UOM, string UOMn, decimal UOMc,
            decimal TotalQty, decimal rCostPrice, bool pCategoryLike = false, string PGroup = "", int BranchId = 0, string VatType = "", string reportType = "",
            string reportName = "", string ProductName = "", string VendorName = "", bool chkLocal = false, string IsRebate = null, string FormNumeric = "2", SysDBInfoVMTemp connVM = null)
        {

            ReportDocument result = new ReportDocument();

            DBSQLConnection _dbsqlConnection = new DBSQLConnection();

            //formula
            BranchProfileDAL _branchDAL = new BranchProfileDAL();

            string BranchName = "All";

            if (BranchId != 0)
            {
                DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true, connVM);
                //DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true);
                BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
            }
            //end formula

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

            ReportResult = reportDsdal.PurchaseNew(PurchaseInvoiceNo, InvoiceDateFrom, InvoiceDateTo, VendorId, ItemNo, CategoryID,
                ProductType, TransactionType, Post, PurchaseType, VendorGroupId, FromBOM, UOM, UOMn, UOMc, TotalQty, rCostPrice,
                pCategoryLike, PGroup, BranchId, VatType,IsRebate, connVM);

            if (string.IsNullOrEmpty(reportName))
            {
                reportName = "All";
            }
            ReportClass objrpt = new ReportClass();

            ReportResult.Tables[0].TableName = "DsPurchase";
            if (reportType.ToLower() == "summary")
            {
                objrpt = new RptPurchaseSummery();
                objrpt.SetDataSource(ReportResult);
                reportName = reportName + " (summary)";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + reportName + "'";
            }
            else if (reportType.ToLower() == "summarybyproduct")
            {
                objrpt = new RptPurchaseSummeryByProduct();
                objrpt.SetDataSource(ReportResult);
                reportName = reportName + " (summary)";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + reportName + "'";
            }
            else
            {
                ReportResult.Tables[0].TableName = "DsPurchase";
                if (chkLocal == false)
                {
                    objrpt = new RptPurchaseTransaction();
                }
                else if (chkLocal == true)
                {
                    if (reportType.ToLower() == "at")
                    {
                        objrpt = new RptPurchaseTransaction_Duty_Summary();
                    }
                    else
                    {
                        objrpt = new RptPurchaseTransaction_Duty();
                    }
                }
                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + reportName + "'";
            }

            #region Settings Load

            CommonDAL _cDal = new CommonDAL();

            DataTable settingsDt = new DataTable();

            if (settingVM.SettingsDTUser == null)
            {
                settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
            }

            #endregion


            //}
            #region Formulla
            //////objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
            //////objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
            //////objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
            //////objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
            //////objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
            //////objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";
            objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";
            if (ProductName == "")
            {
                objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + ProductName + "'  ";
            }

            if (VendorName == "")
            {
                objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'" + VendorName + "'  ";
            }

            if (PurchaseInvoiceNo == "")
            {
                objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'" + PurchaseInvoiceNo + "'  ";
            }

            if (InvoiceDateFrom == "")
            {
                objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + InvoiceDateFrom + "'  ";
            }

            if (InvoiceDateTo == "")
            {
                objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
            }
            else
            {
                objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + InvoiceDateTo + "'  ";
            }

            #endregion Formulla

            FormulaFieldDefinitions crFormulaF;
            crFormulaF = objrpt.DataDefinition.FormulaFields;
            CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FormNumeric", FormNumeric);

            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);
            return objrpt;





        }


        public ReportDocument PurchaseMis(string PurchaseId, int BranchId = 0, string VatType = "", string reportName = "", string FormNumeric = "2", SysDBInfoVMTemp connVM = null)
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


            DataSet ReportResult = new DataSet();

            ReportDSDAL reportDsdal = new ReportDSDAL();

            ReportResult = reportDsdal.PurchaseMis(PurchaseId, BranchId, VatType, connVM);

            ReportClass objrpt = new ReportClass();                // Start Complete


            ReportResult.Tables[0].TableName = "DsPurchaseHeader";
            ReportResult.Tables[1].TableName = "DsPurchaseDetails";
            //ReportMIS.Tables[2].TableName = "DsPurchaseDuty";

            objrpt = new RptMISPurchase1();

            objrpt.SetDataSource(ReportResult);

            #region Settings Load

            CommonDAL _cDal = new CommonDAL();

            DataTable settingsDt = new DataTable();

            if (settingVM.SettingsDTUser == null)
            {
                settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
            }

            #endregion
            //formula
            BranchProfileDAL _branchDAL = new BranchProfileDAL();

            string BranchName = "All";

            if (BranchId != 0)
            {
                DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true, connVM);
                BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
            }
            //end formula

            //string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt);




            //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
            //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
            //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
            //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
            objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + reportName + "'";
            objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";
            objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

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
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);
            return objrpt;

        }

        public ReportDocument IssueInformation(string IssueNo, int BranchId = 0, SysDBInfoVMTemp connVM = null)
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


            DataSet ReportResult = new DataSet();

            ReportDSDAL reportDsdal = new ReportDSDAL();
            ReportResult = reportDsdal.IssueInformation(IssueNo, BranchId, connVM);


            #region Complete
            if (ReportResult.Tables.Count <= 0)
            {
                return null;
            }

            ReportClass objrpt = new ReportClass();

            ReportResult.Tables[0].TableName = "DsIssue";
            objrpt = new RptIssueInformation();

            objrpt.SetDataSource(ReportResult);
            FormulaFieldDefinitions crFormulaF;
            crFormulaF = objrpt.DataDefinition.FormulaFields;
            CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);


            #endregion


            return objrpt;

        }


        public ReportDocument ReceiveInformation(string ReceiveNo, int BranchId = 0, SysDBInfoVMTemp connVM = null)
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

            DataSet ReportResult = new DataSet();

            ReportDSDAL reportDsdal = new ReportDSDAL();
            ReportResult = reportDsdal.ReceiveInformation(ReceiveNo, BranchId, connVM);


            #region Complete
            if (ReportResult.Tables.Count <= 0)
            {
                return null;
            }

            ReportClass objrpt = new ReportClass();

            ReportResult.Tables[0].TableName = "DsReceive";
            ReportResult.Tables[1].TableName = "DsReceive1";
            //ReportResult.Tables[2].TableName = "DsReceive1";

            objrpt = new RptReceiveInformation();

            objrpt.SetDataSource(ReportResult);
            FormulaFieldDefinitions crFormulaF;
            crFormulaF = objrpt.DataDefinition.FormulaFields;
            CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);


            #endregion


            return objrpt;

        }

        public StockMISViewModel MISStock_Download(StockMISViewModel vm, SysDBInfoVMTemp connVM = null)
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

                //////vm = new _9_1_VATReturnDAL().VAT9_1_SubForm_Download(vm);

                DataTable dtComapnyProfile = new DataTable();

                DataSet tempDS = new DataSet();
                tempDS = new ReportDSDAL().ComapnyProfile("", connVM);
                dtComapnyProfile = tempDS.Tables[0];
                vm.dtComapnyProfile = dtComapnyProfile;
                DataRow dr = vm.dtComapnyProfile.Rows[0];

                string ComapnyName = dr["CompanyLegalName"].ToString();
                string VatRegistrationNo = dr["VatRegistrationNo"].ToString();
                string Address1 = dr["Address1"].ToString();

                string ParamFromDate;
                string ParamToDate;


                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    ParamFromDate = Convert.ToDateTime(vm.DateFrom).ToString("dd-MMM-yyyy");
                }
                else
                {
                    ParamFromDate = "All";
                }

                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    ParamToDate = Convert.ToDateTime(vm.DateTo).ToString("dd-MMM-yyyy");
                }
                else
                {
                    ParamToDate = "All";
                }


                string[] ReportHeaders = new string[] { ComapnyName, VatRegistrationNo, Address1, vm.ReportHeaderName, "Form Date:" + ParamFromDate + "                To Date:" + ParamToDate };
                //////string[] ReportHeaders = new string[] { ComapnyName, VatRegistrationNo, Address1, "Stock Summery" };

                DataTable dt = vm.dt;

                if (dt == null || dt.Rows.Count == 0)
                {
                    return vm;
                }

                #region Column Name Change


                dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

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

                //vm.FileName = "_Stock_Report";

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
                    vm.SheetName = "_Stock_Report";
                }

                #endregion

                ExcelPackage package = new ExcelPackage(objFileStrm);

                ////ExcelWorksheet ws = package.Workbook.Worksheets.Add("_Stock_Report");
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Stock");

                ////////ws.Cells["A1"].LoadFromDataTable(dt, true);
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

                //////ws.Cells[ReportHeaders.Length + 3, 1, ReportHeaders.Length + 3 + dt.Rows.Count, dt.Columns.Count].Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

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

        public DepositMISViewModel MISSummaryVDS_Download(DepositMISViewModel vm, SysDBInfoVMTemp connVM = null)
        {
            ReportDocument objrpt = new ReportDocument();
            try
            {
                //////Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                DataTable Table = new DataTable();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }

                Table = new ReportDSDAL().SummaryVDSReport(vm);

                DataTable dtComapnyProfile = new DataTable();

                DataSet tempDS = new DataSet();
                tempDS = new ReportDSDAL().ComapnyProfile("", connVM);
                dtComapnyProfile = tempDS.Tables[0];
                vm.dtComapnyProfile = dtComapnyProfile;
                DataRow dr = vm.dtComapnyProfile.Rows[0];

                string ComapnyName = dr["CompanyLegalName"].ToString();
                string VatRegistrationNo = dr["VatRegistrationNo"].ToString();
                string Address1 = dr["Address1"].ToString();

                string ParamFromDate;
                string ParamToDate;


                if (!string.IsNullOrWhiteSpace(vm.DepositDateFrom))
                {
                    ParamFromDate = Convert.ToDateTime(vm.DepositDateFrom).ToString("dd-MMM-yyyy");
                }
                else
                {
                    ParamFromDate = "All";
                }

                if (!string.IsNullOrWhiteSpace(vm.DepositDateTo))
                {
                    ParamToDate = Convert.ToDateTime(vm.DepositDateTo).ToString("dd-MMM-yyyy");
                }
                else
                {
                    ParamToDate = "All";
                }


                string[] ReportHeaders = new string[] { ComapnyName, VatRegistrationNo, Address1, "VDS", "Form Date:" + ParamFromDate + "                To Date:" + ParamToDate };

                DataTable dt = Table;

                if (dt == null || dt.Rows.Count == 0)
                {
                    return vm;
                }

                #region Column Name Change


                dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

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

                vm.FileName = "VDS Summary";

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
                    vm.SheetName = "VDS Summary ";
                }

                #endregion

                ExcelPackage package = new ExcelPackage(objFileStrm);

                ////ExcelWorksheet ws = package.Workbook.Worksheets.Add("_Stock_Report");
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("VDS_Summary");

                ////////ws.Cells["A1"].LoadFromDataTable(dt, true);
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

                //////ws.Cells[ReportHeaders.Length + 3, 1, ReportHeaders.Length + 3 + dt.Rows.Count, dt.Columns.Count].Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

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


        public ProductionMISViewModel IssueNew(ProductionMISViewModel vm, SysDBInfoVMTemp connVM = null)
        {
            ReportDocument objrpt = new ReportDocument();
            try
            {
                //////Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                var post = vm.Post ? "Y" : "N";
                var waste = vm.Wastage ? "Y" : "N";
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                DataTable dtresult = new DataTable();
                DataTable dt = new DataTable();

                DataSet ds = new ReportDSDAL().IssueNew(vm.IssueNo, vm.IssueDateFrom, vm.IssueDateTo, vm.ItemNo, vm.ProductGroup, vm.ProductType, "", post, waste, false, "", vm.BranchId, connVM);
                dtresult = ds.Tables[0];

                var dataView = new DataView(dtresult);
                dtresult = OrdinaryVATDesktop.DtColumnNameChange(dtresult, "ImportIDExcel", "Reference ID");
                dt = dataView.ToTable(true, "IssueDateTime", "ProductCode", "ProductName", "IssueNo", "Reference ID",
                          "UOM", "UOMQty", "Comments");
                DataTable dtComapnyProfile = new DataTable();

                DataSet tempDS = new DataSet();
                tempDS = new ReportDSDAL().ComapnyProfile("", connVM);
                dtComapnyProfile = tempDS.Tables[0];
                vm.dtComapnyProfile = dtComapnyProfile;
                DataRow dr = vm.dtComapnyProfile.Rows[0];

                string ComapnyName = dr["CompanyLegalName"].ToString();

                string ParamFromDate;
                string ParamToDate;


                if (!string.IsNullOrWhiteSpace(vm.IssueDateFrom))
                {
                    ParamFromDate = Convert.ToDateTime(vm.IssueDateFrom).ToString("dd-MMM-yyyy");
                }
                else
                {
                    ParamFromDate = "All";
                }

                if (!string.IsNullOrWhiteSpace(vm.IssueDateTo))
                {
                    ParamToDate = Convert.ToDateTime(vm.IssueDateTo).ToString("dd-MMM-yyyy");
                }
                else
                {
                    ParamToDate = "All";
                }


                string[] ReportHeaders = new string[] { ComapnyName, "MIS Report for production issue", "Form Date:" + ParamFromDate + "                To Date:" + ParamToDate };
                //string[] ReportHeaders = new string[] { ComapnyName, "MIS Report for production issue" };

                //////string[] ReportHeaders = new string[] { ComapnyName, VatRegistrationNo, Address1, "Stock Summery" };



                if (dt == null || dt.Rows.Count == 0)
                {
                    return vm;
                }

                #region Column Name Change


                dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

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

                vm.FileName = "MIS Production Issue";
                fileDirectory += "\\" + vm.FileName + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";

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
                    vm.SheetName = "MIS Production Issue";
                }

                #endregion

                ExcelPackage package = new ExcelPackage(objFileStrm);

                ////ExcelWorksheet ws = package.Workbook.Worksheets.Add("_Stock_Report");
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Stock");

                ////////ws.Cells["A1"].LoadFromDataTable(dt, true);
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

                //////ws.Cells[ReportHeaders.Length + 3, 1, ReportHeaders.Length + 3 + dt.Rows.Count, dt.Columns.Count].Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

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

        #region Branch Report

        public ReportDocument BranchListReport(List<string> BranchIdList, SysDBInfoVMTemp connVM = null)
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

                DataSet ReportResult = new DataSet();

                ProductDAL productDal = new ProductDAL();

                var dal = new BranchProfileDAL();
                DataTable data = new DataTable("dsBranchList");
                data = dal.GetExcelData(BranchIdList);

                #region Complete

                if (data == null || data.Rows.Count <= 0)
                {
                    return null;
                }

                ReportDocument objrpt = new ReportDocument();
                FormulaFieldDefinitions crFormulaF;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                objrpt = new RptBranchList();

                data.TableName = "dsBranchList";
                objrpt.SetDataSource(data);

                crFormulaF = objrpt.DataDefinition.FormulaFields;
                #region Formula Fields

                crFormulaF = objrpt.DataDefinition.FormulaFields;

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UserName", OrdinaryVATDesktop.CurrentUser);

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

                crFormulaF = objrpt.DataDefinition.FormulaFields;

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Branch Information");

                crFormulaF = objrpt.DataDefinition.FormulaFields;
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);

                #endregion

                #endregion

                return objrpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        public ReportDocument YearlySalesData(SaleMISViewModel vm, SysDBInfoVMTemp connVM = null)
        {

            ReportDocument result = new ReportDocument();

            DBSQLConnection _dbsqlConnection = new DBSQLConnection();

            string reportName = "";
            string ProductName = "";
            string FristYear = "";
            string SecondYear = "";

            #region Branch 
            
            BranchProfileDAL _branchDAL = new BranchProfileDAL();

            string BranchName = "All";

            ////if (BranchId != 0)
            ////{
            ////    DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true, connVM);
            ////    //DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true);
            ////    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
            ////}

            #endregion

            if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
            {
                return null;
            }

            #region CompanyInformation

            CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

            #endregion

            DataSet ReportResult = new DataSet();
            DataTable dt = new DataTable();

            ReportDSDAL reportDsdal = new ReportDSDAL();

            dt = reportDsdal.GetYearlySaleData(vm, null, null, connVM);
            ReportResult.Tables.Add(dt);
            reportName = vm.reportName;
            ProductName = vm.ProductName;

            if (string.IsNullOrEmpty(vm.reportName))
            {
                reportName = "All";
            }
            if (string.IsNullOrEmpty(vm.ProductName))
            {
                ProductName = "All Product";
            }

            FristYear = "Year - " + (Convert.ToDecimal(vm.DateFrom) - 1).ToString() + "-" + vm.DateFrom;
            SecondYear = "Year - " + (Convert.ToDecimal(vm.DateTo) - 1).ToString() + "-" + vm.DateTo;

            ReportClass objrpt = new ReportClass();

            ReportResult.Tables[0].TableName = "YearlySale";

            objrpt = new RptYearlySale();
            objrpt.SetDataSource(ReportResult);
            //reportName = reportName;

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
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", reportName);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductName", ProductName);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FristYear", FristYear);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SecondYear", SecondYear);
            
            return objrpt;

        }

        public ReportDocument MonthlySalesData(SaleMISViewModel vm,string FirstPeriodName="",string SecondPeriodName="", string FirstPriodID = "", string SecondPriodID = "", SysDBInfoVMTemp connVM = null)
        {

            ReportDocument result = new ReportDocument();

            DBSQLConnection _dbsqlConnection = new DBSQLConnection();

            string reportName = "";
            string ProductName = "";
            string FristYear = FirstPeriodName;
            string SecondYear = SecondPeriodName;

            #region Branch

            BranchProfileDAL _branchDAL = new BranchProfileDAL();

            string BranchName = "All";

            ////if (BranchId != 0)
            ////{
            ////    DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true, connVM);
            ////    //DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true);
            ////    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
            ////}

            #endregion

            if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
            {
                return null;
            }

            reportName = vm.reportName;
            ProductName = vm.ProductName;

            if (string.IsNullOrEmpty(vm.ProductName))
            {
                ProductName = "All Product";
            }
            if (string.IsNullOrEmpty(vm.ProductName))
            {
                ProductName = "All Product";
            }

            #region CompanyInformation

            CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

            #endregion

            DataSet ReportResult = new DataSet();
            DataTable dt = new DataTable();

            ReportDSDAL reportDsdal = new ReportDSDAL();

            ReportResult = reportDsdal.GetMonthlySaleData(vm, FirstPriodID, SecondPriodID, null, null, connVM);

            if (string.IsNullOrEmpty(vm.reportName))
            {
                reportName = "All";
            }
            ReportClass objrpt = new ReportClass();

            ReportResult.Tables[0].TableName = "MonthlySale";
            ReportResult.Tables[1].TableName = "dsMonthlySale";

            objrpt = new RptMonthlySale();
            objrpt.SetDataSource(ReportResult);
            //reportName = reportName;

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
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", reportName);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductName", ProductName);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FristYear", FristYear);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SecondYear", SecondYear);
            return objrpt;

        }

        public ReportDocument YearlyPurchaseData(PurchaseMISViewModel vm, SysDBInfoVMTemp connVM = null)
        {

            ReportDocument result = new ReportDocument();

            DBSQLConnection _dbsqlConnection = new DBSQLConnection();

            string reportName = "";
            string ProductName = "";
            string FristYear = "";
            string SecondYear = "";

            #region Branch

            BranchProfileDAL _branchDAL = new BranchProfileDAL();

            string BranchName = "All";

            ////if (BranchId != 0)
            ////{
            ////    DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true, connVM);
            ////    //DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true);
            ////    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
            ////}

            #endregion

            if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
            {
                return null;
            }

            #region CompanyInformation

            CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

            #endregion

            DataSet ReportResult = new DataSet();
            DataTable dt = new DataTable();

            ReportDSDAL reportDsdal = new ReportDSDAL();

            dt = reportDsdal.GetYearlyPurchaseData(vm, null, null, connVM);
            ReportResult.Tables.Add(dt);

            reportName = vm.reportName;
            ProductName = vm.ProductName;

            if (string.IsNullOrEmpty(vm.reportName))
            {
                reportName = "All";
            }
            if (string.IsNullOrEmpty(vm.ProductName))
            {
                ProductName = "All Product";
            }

            FristYear = "Year - " + (Convert.ToDecimal(vm.ReceiveDateFrom) - 1).ToString() + "-" + vm.ReceiveDateFrom;
            SecondYear = "Year - " + (Convert.ToDecimal(vm.ReceiveDateTo) - 1).ToString() + "-" + vm.ReceiveDateTo;

            ReportClass objrpt = new ReportClass();

            ReportResult.Tables[0].TableName = "YearlyPurchase";

            objrpt = new RptYearlyPurchase();
            objrpt.SetDataSource(ReportResult);
            //reportName = reportName;

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
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", reportName);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductName", ProductName);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FristYear", FristYear);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SecondYear", SecondYear);

            return objrpt;

        }

        public ReportDocument MonthlyPurchaseData(PurchaseMISViewModel vm,string FirstPeriodName="",string SecondPeriodName="",  string FirstPriodID = "", string SecondPriodID = "", SysDBInfoVMTemp connVM = null)
        {

            ReportDocument result = new ReportDocument();

            DBSQLConnection _dbsqlConnection = new DBSQLConnection();

            string reportName = "";
            string ProductName = "";
            string FristYear = FirstPeriodName;
            string SecondYear = SecondPeriodName;
            //string PeriodName = "";

            #region Branch

            BranchProfileDAL _branchDAL = new BranchProfileDAL();

            string BranchName = "All";

            ////if (BranchId != 0)
            ////{
            ////    DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true, connVM);
            ////    //DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true);
            ////    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
            ////}

            #endregion

            if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
            {
                return null;
            }

            reportName = vm.reportName;
            ProductName = vm.ProductName;

            #region CompanyInformation

            CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

            #endregion

            DataSet ReportResult = new DataSet();
            DataTable dt = new DataTable();

            ReportDSDAL reportDsdal = new ReportDSDAL();

            ReportResult = reportDsdal.GetMonthlyPurchaseData(vm, FirstPriodID, SecondPriodID, null, null, connVM);

            if (string.IsNullOrEmpty(vm.reportName))
            {
                reportName = "All";
            }
            if (string.IsNullOrEmpty(vm.ProductName))
            {
                ProductName = "All Product";
            }

            ReportClass objrpt = new ReportClass();

            //ReportResult.Tables[0].TableName = "MonthlyPurchase";
            ReportResult.Tables[0].TableName = "MonthlySale";
            ReportResult.Tables[1].TableName = "dsMonthlySale";
            objrpt = new RptMonthlyPurchase();
            objrpt.SetDataSource(ReportResult);
            //reportName = reportName;

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
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", reportName);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ProductName", ProductName);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FristYear", FristYear);
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SecondYear", SecondYear);

            return objrpt;

        }

        #region Meghna 
        public ReportDocument MegnaCA16Report(string Id, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                ReportDocument result = new ReportDocument();
                ReportDocument objrpt = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                DataTable ReportResult = new DataTable();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.MegnaCA16Report(Id, connVM);


                #region Complete
                if (ReportResult.Rows.Count <= 0)
                {
                    return null;
                }
                ReportResult.TableName = "dtMegnaDepositSlip";

                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptVATMegnaBankDepositSlip_V12V2.rpt");
                objrpt = new RptVATMegnaBankDepositSlip_V12V2();

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);



                objrpt.SetDataSource(ReportResult);


               
                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

                #endregion


                return objrpt;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public ReportDocument MegnaTradeChallan(string Id, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                ReportDocument result = new ReportDocument();
                ReportDocument objrpt = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                DataTable ReportResult = new DataTable();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.MegnaTradeChallan(Id, connVM);


                #region Complete
                if (ReportResult.Rows.Count <= 0)
                {
                    return null;
                }
                ReportResult.TableName = "dtTradeChallan";

                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptMeghnaTradeChallan.rpt");
                objrpt = new RptMeghnaTradeChallan();

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);



                objrpt.SetDataSource(ReportResult);



                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

                #endregion


                return objrpt;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public ReportDocument MegnaAccountReport(MeghnaMISViewModel vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                ReportDocument result = new ReportDocument();
                ReportDocument objrpt = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                DataTable ReportResult = new DataTable();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.AccountsReport(vm, connVM);


                #region Complete
                if (ReportResult.Rows.Count <= 0)
                {
                    return null;
                }
                ReportResult.TableName = "dtMeghnaAccountsReport";

                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptMeghnaAccountsReport.rpt");
                objrpt = new RptMeghnaAccountsReport();

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);



                objrpt.SetDataSource(ReportResult);



                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                if (string.IsNullOrEmpty(vm.BranchId))
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", "[All]");
                }
                else
                {
                    if (ReportResult.Rows.Count>0)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName",  ReportResult.Rows[0]["BranchName"].ToString());
                    }
                }
                 if (string.IsNullOrEmpty(vm.SelfBankId))
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BankCode", "[All]");
                }
                else
                {
                    if (ReportResult.Rows.Count>0)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BankCode",  ReportResult.Rows[0]["BankCode"].ToString());
                    }
                }
                #endregion


                return objrpt;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument MeghnaBankDepositInfo(MeghnaMISViewModel vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                ReportDocument result = new ReportDocument();
                ReportDocument objrpt = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                DataTable ReportResult = new DataTable();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.MeghnaBankDepositInfo(vm, connVM);


                #region Complete
                if (ReportResult.Rows.Count <= 0)
                {
                    return null;
                }
                ReportResult.TableName = "dtMeghnaAccountsReport";

                objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptMeghnaAccountsReport.rpt");
                //objrpt = new RptMeghnaAccountsReport();

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);



                objrpt.SetDataSource(ReportResult);



                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                if (string.IsNullOrEmpty(vm.BranchId))
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", "[All]");
                }
                else
                {
                    if (ReportResult.Rows.Count > 0)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", ReportResult.Rows[0]["BranchName"].ToString());
                    }
                }
                if (string.IsNullOrEmpty(vm.SelfBankId))
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BankCode", "[All]");
                }
                else
                {
                    if (ReportResult.Rows.Count > 0)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BankCode", ReportResult.Rows[0]["BankCode"].ToString());
                    }
                }
                #endregion


                return objrpt;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public ReportDocument MeghnaCustomerLedger(MeghnaMISViewModel vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                ReportDocument result = new ReportDocument();
                ReportDocument objrpt = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                DataTable ReportResult = new DataTable();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.MeghnaCustemerLedgerReport(vm, connVM);


                #region Complete
                if (ReportResult.Rows.Count <= 0)
                {
                    return null;
                }
                ReportResult.TableName = "dtMeghnaCustomerLedger";

                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptMeghnaCustomerLedger.rpt");
                objrpt = new RptMeghnaCustomerLedger();

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);



                objrpt.SetDataSource(ReportResult);



                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                if (string.IsNullOrEmpty(vm.BranchId))
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", "[All]");
                }
                else
                {
                    if (ReportResult.Rows.Count > 0)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", ReportResult.Rows[0]["BranchName"].ToString());
                    }
                }
                if (string.IsNullOrEmpty(vm.SelfBankId))
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BankCode", "[All]");
                }
                else
                {
                    if (ReportResult.Rows.Count > 0)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BankCode", ReportResult.Rows[0]["BankCode"].ToString());
                    }
                }
                #endregion


                return objrpt;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public ReportDocument MeghnaInvoiceCreditInfo(MeghnaMISViewModel vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                ReportDocument result = new ReportDocument();
                ReportDocument objrpt = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                DataTable ReportResult = new DataTable();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.MeghnaInvoiceCreditInfo(vm, connVM);


                #region Complete
                if (ReportResult.Rows.Count <= 0)
                {
                    return null;
                }
                ReportResult.TableName = "dtMeghnaAccountsReport";

                objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptMeghnaAccountsReport.rpt");
                //objrpt = new RptMeghnaAccountsReport();

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);



                objrpt.SetDataSource(ReportResult);



                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                if (string.IsNullOrEmpty(vm.BranchId))
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", "[All]");
                }
                else
                {
                    if (ReportResult.Rows.Count > 0)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", ReportResult.Rows[0]["BranchName"].ToString());
                    }
                }
                if (string.IsNullOrEmpty(vm.SelfBankId))
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BankCode", "[All]");
                }
                else
                {
                    if (ReportResult.Rows.Count > 0)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BankCode", ReportResult.Rows[0]["BankCode"].ToString());
                    }
                }
                #endregion


                return objrpt;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ReportDocument MeghnaTransferIssueInfo(MeghnaMISViewModel vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                ReportDocument result = new ReportDocument();
                ReportDocument objrpt = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                DataTable ReportResult = new DataTable();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.MeghnaTransferIssueInfo(vm, connVM);


                #region Complete
                if (ReportResult.Rows.Count <= 0)
                {
                    return null;
                }
                ReportResult.TableName = "dtMeghnaAccountsReport";

                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptMeghnaAccountsReport.rpt");
                objrpt = new RptMeghnaAccountsReport();

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);



                objrpt.SetDataSource(ReportResult);



                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                if (string.IsNullOrEmpty(vm.BranchId))
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", "[All]");
                }
                else
                {
                    if (ReportResult.Rows.Count > 0)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", ReportResult.Rows[0]["BranchName"].ToString());
                    }
                }
                if (string.IsNullOrEmpty(vm.SelfBankId))
                {
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BankCode", "[All]");
                }
                else
                {
                    if (ReportResult.Rows.Count > 0)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BankCode", ReportResult.Rows[0]["BankCode"].ToString());
                    }
                }
                #endregion


                return objrpt;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public MeghnaMISViewModel MeghnaTransferIssue(MeghnaMISViewModel vm, SysDBInfoVMTemp connVM = null)
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

                DataTable dt = new ReportDSDAL().MeghnaTransferIssueInfo(vm);

                DataTable dtComapnyProfile = new DataTable();

                DataSet tempDS = new DataSet();
                tempDS = new ReportDSDAL().ComapnyProfile("", connVM);
                dtComapnyProfile = tempDS.Tables[0];
                vm.dtComapnyProfile = dtComapnyProfile;
                DataRow dr = vm.dtComapnyProfile.Rows[0];

                string ComapnyName = dr["CompanyLegalName"].ToString();
                string VatRegistrationNo = dr["VatRegistrationNo"].ToString();
                string Address1 = dr["Address1"].ToString();

                string ParamFromDate;
                string ParamToDate;


                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    ParamFromDate = Convert.ToDateTime(vm.DateFrom).ToString("dd-MMM-yyyy");
                }
                else
                {
                    ParamFromDate = "All";
                }

                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    ParamToDate = Convert.ToDateTime(vm.DateTo).ToString("dd-MMM-yyyy");
                }
                else
                {
                    ParamToDate = "All";
                }


                string[] ReportHeaders = new string[] { ComapnyName, VatRegistrationNo, Address1, vm.ReportHeaderName, "Form Date:" + ParamFromDate + "                To Date:" + ParamToDate };
                //////string[] ReportHeaders = new string[] { ComapnyName, VatRegistrationNo, Address1, "Stock Summery" };


                if (dt == null || dt.Rows.Count == 0)
                {
                    return vm;
                }

                #region Column Name Change


                dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

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

                //vm.FileName = "_Stock_Report";

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
                    vm.SheetName = "_Stock_Report";
                }

                #endregion

                ExcelPackage package = new ExcelPackage(objFileStrm);

                ////ExcelWorksheet ws = package.Workbook.Worksheets.Add("_Stock_Report");
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Stock");

                ////////ws.Cells["A1"].LoadFromDataTable(dt, true);
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

                //////ws.Cells[ReportHeaders.Length + 3, 1, ReportHeaders.Length + 3 + dt.Rows.Count, dt.Columns.Count].Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

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


        public ReportDocument MegnaCA29Report(string Id, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                ReportDocument result = new ReportDocument();
                ReportDocument objrpt = new ReportDocument();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) <=
                    Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                {
                    return null;
                }
                #region CompanyInformation

                CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

                #endregion

                DataTable ReportResult = new DataTable();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.MegnaCA29Report(Id, connVM);


                #region Complete
                if (ReportResult.Rows.Count <= 0)
                {
                    return null;
                }
                ReportResult.TableName = "dtBankPaymentReceive";

                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptMegnaCA29_V12V2.rpt");
                objrpt = new RptMegnaCA29_V12V2();

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);



                objrpt.SetDataSource(ReportResult);



                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

                #endregion


                return objrpt;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public ReportDocument MegnaDayEndReport(ReportParamVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                ReportDocument result = new ReportDocument();
                ReportDocument objrpt = new ReportDocument();

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
                ReportResult = reportDsdal.MegnaDayEndReport(vm, connVM);


                #region Complete
                if (ReportResult==null)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "dtMeghnaDayEndSales";
                ReportResult.Tables[1].TableName = "dtMeghnaDayEndBank";
                ReportResult.Tables[2].TableName = "dtMeghnaDayEndOther";


                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptMeghnaDayEndReport.rpt");
                objrpt = new RptMeghnaDayEndReport();

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);



                objrpt.SetDataSource(ReportResult);



                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

                #endregion


                return objrpt;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public ReportDocument MeghnaIN89Report(ReportParamVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                ReportDocument result = new ReportDocument();
                ReportDocument objrpt = new ReportDocument();

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
                ReportResult = reportDsdal.MeghnaIN89Report(vm, connVM);


                #region Complete
                if (ReportResult == null)
                {
                    return null;
                }
                ReportResult.Tables[0].TableName = "dtMPLIN89";
                ReportResult.Tables[1].TableName = "dtMPLIN89IssueDetails";
                ReportResult.Tables[2].TableName = "dtMPLIN89ReceiveDetails";
                



                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptMeghnaIN89Report.rpt");
                objrpt = new RptMeghnaIN89Report();

                #region Settings Load

                CommonDAL _cDal = new CommonDAL();

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                #endregion

                string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "FormNumeric", settingsDt, connVM);



                objrpt.SetDataSource(ReportResult);



                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

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

