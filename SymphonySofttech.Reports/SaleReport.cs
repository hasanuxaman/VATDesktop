using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SymphonySofttech.Reports.Report;
using VATServer.Library;
using VATServer.License;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATDesktop.Repo;
using SymphonySofttech.Reports.Report.Rpt63;
using Newtonsoft.Json;
using SymphonySofttech.Reports.Report.Rpt63V12V2;
using SymphonySofttech.Reports.Report.V12V2;
using VATServer.Library.Integration;
using System.IO;
using System.Net.Mail;
using CrystalDecisions.Shared;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;

namespace SymphonySofttech.Reports
{
    public class SaleReport
    {
        IReport showReport;


        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();


        public SaleReport()
        {
            try
            {
                // DefaultCellStyle.Font.Name= "SutonnyMJ";mo
                connVM.SysdataSource = SysDBInfoVM.SysdataSource;
                connVM.SysPassword = SysDBInfoVM.SysPassword;
                connVM.SysUserName = SysDBInfoVM.SysUserName;
                connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
                showReport = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        public IReport GetObject()
        {
            if (OrdinaryVATDesktop.IsWCF.ToLower() == "y")
            {
                return new ReportDSRepo();
            }
            else
            {
                return new ReportDSDAL();

            }
        }


        public ReportDocument VendorReportNew(string VendorID, string VendorGroupID, SysDBInfoVMTemp connVM = null)
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
            ReportResult = reportDsdal.VendorReportNew(VendorID, VendorGroupID);


            #region Complete
            if (ReportResult.Tables.Count <= 0)
            {
                return null;
            }
            ReportResult.Tables[0].TableName = "DsVendor";
            RptVendorListing objrpt = new RptVendorListing();
            objrpt.SetDataSource(ReportResult);
            objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
            objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Vendor Information'";
            //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
            ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
            //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
            //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
            //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
            //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
            //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
            //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
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

            #endregion


            return objrpt;
        }

        public ReportDocument BillInvoiceReportNew(string BillNo, SysDBInfoVMTemp connVM = null)
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


            ReportResult = reportDsdal.BillInvoiceReportNew(BillNo);


            #region Complete
            if (ReportResult.Tables.Count <= 0)
            {
                return null;
            }
            ReportResult.Tables[0].TableName = "DsBillInvoice";
            RptBillInvoice objrpt = new RptBillInvoice();
            objrpt.SetDataSource(ReportResult);
            ////objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
            ////objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Vendor Information'";
            //////objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
            ////////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
            //////objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
            //////objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
            //////objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
            //////objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
            //////objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
            //////objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
            ////objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";

            ////FormulaFieldDefinitions crFormulaF;
            ////crFormulaF = objrpt.DataDefinition.FormulaFields;
            ////CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

            #endregion


            return objrpt;
        }

        public ReportDocument SaleDeliveryChallanReportNew(string DeliveryChallanNo, SysDBInfoVMTemp connVM = null)
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


            ReportResult = reportDsdal.SaleDeliveryChallanReportNew(DeliveryChallanNo);


            #region Complete
            if (ReportResult.Tables.Count <= 0)
            {
                return null;
            }
            ReportResult.Tables[0].TableName = "DsSaleDeliveryChallan";
            RptVAT_SaleDeliveryChallan objrpt = new RptVAT_SaleDeliveryChallan();
            objrpt.SetDataSource(ReportResult);
            ////objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
            ////objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Vendor Information'";
            //////objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
            ////////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
            //////objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
            //////objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
            //////objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
            //////objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
            //////objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
            //////objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
            ////objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";

            ////FormulaFieldDefinitions crFormulaF;
            ////crFormulaF = objrpt.DataDefinition.FormulaFields;
            ////CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", OrdinaryVATDesktop.FontSize);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
            ////_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

            #endregion


            return objrpt;
        }

        public ReportDocument Report_VAT6_3_Completed(string varSalesInvoiceNo
            , string transactionType
            , bool rbtnCN
            , bool rbtnDN
            , bool rbtnRCN
            , bool rbtnTrading
            , bool rbtnTollIssue
            , bool rbtnVAT11GaGa
            , bool PreviewOnly
            , int PrintCopy
            , int AlReadyPrintNo
            , bool chkIsBlank
            , bool chkIs11
            , bool chkValueOnly
            , bool CommercialImporter = false
            , bool mulitplePreview = false
            , bool rbtnContractorRawIssue = false
            , SysDBInfoVMTemp connVM = null, string MultipleSalesInvoiceRows = "", bool IsGatepass = false, bool rbtnPurchaseReurn = false, bool PrintAll = false)
        {
            #region Golbal Variable
            ReportDocument objrpt = new ReportDocument();
            CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            //ReportDSDAL showReport = new ReportDSDAL();
            CommonDAL commonDal = new CommonDAL();
            DataSet ReportResultVAT11 = new DataSet();
            DataSet ReportResultCreditNote = new DataSet();
            DataSet ReportResultDebitNote = new DataSet();
            DataSet ReportResultTracking = new DataSet();

            FormulaFieldDefinitions crFormulaF;
            UserInformationVM uvm = new UserInformationVM();
            //UserInformationDAL _udal = new UserInformationDAL();
            AlReadyPrintNo = 0;
            int NumberOfItems = 0;
            string BranchId = "1";
            string BranchCode = "";
            string BranchName = "";
            string IsCentral = "";
            string BranchLegalName = "";
            string BranchBanglaLegalName = "";
            string BranchAddress = "";
            string BanglaAddress = "";
            string RegistredAddress = "";
            string BranchVatRegistrationNo = "";
            string VAT11Name = "";
            string CompanyCode = "";
            string vPGroupInReport = "";
            string ItemNature = "";
            string VAT11PageSize = "";
            string Heading1 = "";
            string Heading2 = "";
            string post1 = "";
            string post2 = "";
            string EntryUserName = "";
            string Address = "";
            string MaterialType = "";
            bool PrepaidVAT = false;
            bool VAT11Letter = false;
            bool VAT11A4 = false;
            bool VAT11Legal = false;
            bool VAT11English = false;
            bool TrackingTrace = false;

            #endregion

            #region Settings

            DataTable settingsDt = new DataTable();
            DateTime invoiceDateTime = DateTime.Now;
            DateTime VAT2012V2 = DateTime.Now;
            DateTime VAT2022V2 = DateTime.Now;
            string vVAT2012V2 = "2020-Jul-01";
            string vVAT2022V2 = "2022-Jul-01";

            try
            {

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                vVAT2012V2 = commonDal.settings("Version", "VAT2012V2", null, null, connVM);

                VAT2012V2 = Convert.ToDateTime(vVAT2012V2);


                vVAT2022V2 = commonDal.settings("Version", "VAT2022V2", null, null, connVM);

                VAT2022V2 = Convert.ToDateTime("01-Jul-2022");

            }


            catch (Exception ex)
            {
                throw new ArgumentException(vVAT2012V2 + ex.ToString());
            }
            CompanyCode = commonDal.settingsDesktop("CompanyCode", "Code", settingsDt, connVM);


            #endregion

            DBSQLConnection _dbsqlConnection = new DBSQLConnection();

            if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName, CompanyCode)) <=
                Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
            {
                return null;
            }

            #region CompanyInformation

            CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

            #endregion

            #region Data Calling

            try
            {
                if (PreviewOnly == true)
                {
                    post1 = "y";
                    post2 = "N";
                }
                else
                {
                    post1 = "Y";
                    post2 = "Y";
                }

                #region Settings Assign

                VAT11Name = commonDal.settings("Reports", "VAT6_3", null, null, connVM);
                var vPrepaidVAT = commonDal.settingsDesktop("PrepaidVAT", "PrepaidVAT", settingsDt, connVM);
                PrepaidVAT = Convert.ToBoolean(commonDal.settingsDesktop("PrepaidVAT", "PrepaidVAT", settingsDt, connVM) == "Y" ? true : false);
                VAT11Letter = Convert.ToBoolean(commonDal.settingsDesktop("Sale", "VAT6_3Letter", settingsDt, connVM) == "Y" ? true : false);
                VAT11A4 = Convert.ToBoolean(commonDal.settingsDesktop("Sale", "VAT6_3A4", settingsDt, connVM) == "Y" ? true : false);
                VAT11Legal = Convert.ToBoolean(commonDal.settingsDesktop("Sale", "VAT6_3Legal", settingsDt, connVM) == "Y" ? true : false);
                VAT11English = Convert.ToBoolean(commonDal.settingsDesktop("Sale", "VAT6_3English", settingsDt, connVM) == "Y" ? true : false);
                VAT11English = Convert.ToBoolean(commonDal.settingsDesktop("Sale", "VAT6_3English", settingsDt, connVM) == "Y" ? true : false);
                TrackingTrace = Convert.ToBoolean(commonDal.settingsDesktop("TrackingTrace", "Tracking", settingsDt, connVM) == "Y" ? true : false);
                Heading1 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_1", settingsDt, connVM);
                Heading2 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_2", settingsDt, connVM);

                #endregion

                #region Data Switching

                //if (CommercialImporter)
                //{
                //    ReportResultVAT11 = showReport.VAT11ReportCommercialImporterNew(varSalesInvoiceNo, post1, post2, null, null, connVM);

                //}
                if (!rbtnPurchaseReurn)
                {
                    string[] PrintResult1 = new SaleDAL().UpdatePrintNew(varSalesInvoiceNo, PreviewOnly == true ? 0 : PrintCopy);
                }

                if (CompanyCode == "BCL")
                {
                    if (PreviewOnly == false)
                    {
                        IntegrationParam paramVM = new IntegrationParam();
                        paramVM.MulitipleInvoice = varSalesInvoiceNo;
                        new BeximcoIntegrationDAL().PrintSource_SaleData(paramVM);
                    }

                }
                else if (OrdinaryVATDesktop.IsACICompany(CompanyCode))
                {
                    if (PreviewOnly == false)
                    {
                        IntegrationParam paramVM = new IntegrationParam();
                        paramVM.MulitipleInvoice = varSalesInvoiceNo;
                        new ImportDAL().PrintSource_SaleData(paramVM);
                    }
                }

                if (rbtnCN)
                {
                    ReportResultCreditNote =
                        showReport.VAT6_3(varSalesInvoiceNo, post1, post2, "n", connVM, mulitplePreview, "", "N", 0, 99999, transactionType);
                    invoiceDateTime = Convert.ToDateTime(ReportResultCreditNote.Tables[0].Rows[0]["InvoiceDate"]);

                    if (VAT2012V2 <= invoiceDateTime)
                    {
                        ////New Report -- 
                        ReportResultCreditNote = showReport.VAT6_3(varSalesInvoiceNo, post1, post2, "n", connVM,
                            mulitplePreview, "", "N", 0, 99999, transactionType);
                        invoiceDateTime = Convert.ToDateTime(ReportResultCreditNote.Tables[0].Rows[0]["InvoiceDate"]);

                    }
                    else
                    {
                        ReportResultCreditNote = showReport.VAT6_7(varSalesInvoiceNo, post1, post2, connVM);

                    }
                    //invoiceDateTime = Convert.ToDateTime(ReportResultCreditNote.Tables[0].Rows[0]["InvoiceDate"]);
                    //invoiceDateTime = Convert.ToDateTime(ReportResultCreditNote.Tables[0].Rows[0]["InvoiceDateTime"]);
                }
                else if (rbtnRCN)
                {
                    ReportResultCreditNote = showReport.VAT6_3(varSalesInvoiceNo, post1, post2, "n", connVM);
                    invoiceDateTime = Convert.ToDateTime(ReportResultCreditNote.Tables[0].Rows[0]["InvoiceDate"]);

                    if (VAT2012V2 <= invoiceDateTime)
                    {
                        ////New Report -- 
                        ReportResultCreditNote = showReport.VAT6_3(varSalesInvoiceNo, post1, post2, "n", connVM);
                        invoiceDateTime = Convert.ToDateTime(ReportResultCreditNote.Tables[0].Rows[0]["InvoiceDate"]);

                    }
                    else
                    {
                        ReportResultCreditNote = showReport.VAT6_7(varSalesInvoiceNo, post1, post2, connVM);

                    }
                    //invoiceDateTime = Convert.ToDateTime(ReportResultCreditNote.Tables[0].Rows[0]["InvoiceDate"]);
                    //invoiceDateTime = Convert.ToDateTime(ReportResultCreditNote.Tables[0].Rows[0]["InvoiceDateTime"]);
                }
                else if (rbtnDN)
                {
                    ReportResultDebitNote = showReport.VAT6_3(varSalesInvoiceNo, post1, post2, "n", connVM, mulitplePreview);
                    //invoiceDateTime = Convert.ToDateTime(ReportResultDebitNote.Tables[0].Rows[0]["InvoiceDateTime"]);
                    invoiceDateTime = Convert.ToDateTime(ReportResultDebitNote.Tables[0].Rows[0]["InvoiceDate"]);
                    if (VAT2012V2 <= invoiceDateTime)
                    {
                        ////New Report -- 
                        ReportResultDebitNote = showReport.VAT6_3(varSalesInvoiceNo, post1, post2, "n", connVM, mulitplePreview);
                        invoiceDateTime = Convert.ToDateTime(ReportResultDebitNote.Tables[0].Rows[0]["InvoiceDate"]);

                    }
                    else
                    {
                        ReportResultDebitNote = showReport.VAT6_8(varSalesInvoiceNo, post1, post2, connVM);

                    }

                }
                else if (rbtnPurchaseReurn)
                {
                    ReportResultDebitNote = showReport.PurchaseReturn(varSalesInvoiceNo, post1, post2, connVM);
                }
                else
                {
                    if (BureauInfoVM.IsBureau == true)
                    {
                        ReportResultVAT11 = new ReportDSDAL().BureauVAT11Report(varSalesInvoiceNo, post1, post2, connVM);

                    }
                    else
                    {
                        if (VAT11Name.ToLower() == "scbl")
                        {
                            ReportResultVAT11 = showReport.VAT6_3(varSalesInvoiceNo, post1, post2, "n", connVM);


                        }
                        else
                        {

                            #region Get Data

                            if (VAT11Name.ToUpper() == "NESTLE" || VAT11Name.ToUpper() == "UNILEVER" || VAT11Name.ToUpper() == "UNILEVERSUP")
                            {
                                NumberOfItems = Convert.ToInt32(commonDal.settingsDesktop("Reports", "NumberOfItems", settingsDt, connVM));

                                DataSet ds = new DataSet();
                                string[] skus = MultipleSalesInvoiceRows.Split('~');
                                string[] inv = varSalesInvoiceNo.Split(',');
                                //skus = skus.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                                for (int i = 0; i < skus.Length; i++)
                                {
                                    var tt = inv[i];
                                    int l = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(skus[i]) / NumberOfItems));
                                    for (int j = 0; j < l; j++)
                                    {
                                        //ds = new DataSet();
                                        ds = showReport.VAT6_3(inv[i], post1, post2, "n", connVM, mulitplePreview, "", "N", j * NumberOfItems, NumberOfItems);
                                        ds.Tables[0].Columns.Add(new DataColumn() { ColumnName = "GroupSalesInvoiceNo", DefaultValue = inv[i] + "-" + j.ToString() });

                                        ReportResultVAT11.Merge(ds);
                                    }
                                }


                            }
                            else
                            {
                                ReportResultVAT11 = showReport.VAT6_3(varSalesInvoiceNo, post1, post2, "n", connVM, mulitplePreview);
                                //var tt1 = ReportResultVAT11.Tables[0].Rows[0]["ProductCategory"].ToString();
                            }
                            #endregion
                        }
                        MaterialType = ReportResultVAT11.Tables[0].Rows[0]["ProductGroup"].ToString();
                    }
                    ReportResultTracking = showReport.SaleTrackingReport(varSalesInvoiceNo, connVM);
                    BranchId = ReportResultVAT11.Tables[0].Rows[0]["BranchID"].ToString();
                    EntryUserName = ReportResultVAT11.Tables[0].Rows[0]["EntryUserName"].ToString();

                    invoiceDateTime = Convert.ToDateTime(ReportResultVAT11.Tables[0].Rows[0]["InvoiceDate"]);

                }


                #endregion

                #region Branch Details

                string[] cValues = { BranchId };
                string[] cFields = { "BranchID" };

                DataTable branch = OrdinaryVATDesktop.GetObject<BranchProfileDAL, BranchProfileRepo, IBranchProfile>(connVM.IsWCF)
                    .SelectAll(null, cFields, cValues, null, null, true, connVM);

                BranchCode = branch.Rows[0]["BranchCode"].ToString();
                BranchName = branch.Rows[0]["BranchName"].ToString();
                BranchLegalName = branch.Rows[0]["BranchLegalName"].ToString();
                BranchAddress = branch.Rows[0]["Address"].ToString();
                BranchVatRegistrationNo = branch.Rows[0]["VatRegistrationNo"].ToString();
                IsCentral = branch.Rows[0]["IsCentral"].ToString();
                BranchBanglaLegalName = branch.Rows[0]["BranchBanglaLegalName"].ToString();
                BanglaAddress = branch.Rows[0]["BanglaAddress"].ToString();
                try
                {
                    #region its only For RNL Report

                    if (VAT11Name.ToLower() == "rnl")
                    {

                        string split = "Registered Address:-";
                        int SubstringLength = 0;
                        SubstringLength = BranchAddress.IndexOf(split) + split.Length;
                        if (BranchAddress.Length > SubstringLength)
                        {
                            RegistredAddress = BranchAddress.Substring(SubstringLength);

                        }
                        Address = BranchAddress.Split('.')[0];

                    }

                    #endregion

                    #region ACI Plastic CEPL Branch Address Change

                    DateTime effectDate = Convert.ToDateTime("2021-04-18 00:00:00");
                    DateTime effectDateCtg = Convert.ToDateTime("2021-03-02 00:00:00");

                    if (CompanyCode == "CEPL" && invoiceDateTime < effectDate && BranchCode == "UR1")
                    {
                        BranchAddress = "77,old/86, New Sirajuddowla Road, Khanpur, Narayangonj.";
                    }

                    if (CompanyCode == "CEPL" && invoiceDateTime < effectDateCtg && BranchCode == "AR-3")
                    {
                        BranchAddress = "CTG Depot: Kabir Complex, North Pahartoli, Ggal Gate, Chattogram.";
                    }

                    #endregion
                }
                catch (Exception e)
                {

                }
                #endregion

            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                //if (ex.InnerException != null)
                //{
                //    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                //                ex.StackTrace;

                //}
                //MessageBox.Show(ex.Message, "Report_VAT6_3_Completed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Report_VAT6_3_Completed", "", exMessage);
            }
            #endregion

            #endregion

            #region More Settings

            string VehicleRequired = commonDal.settingsDesktop("Sale", "VehicleRequired", settingsDt, connVM);
            string InEnglish = commonDal.settingsDesktop("Reports", "VAT6_3English", settingsDt, connVM);
            string VAT6_7English = commonDal.settingsDesktop("Reports", "VAT6_7English", settingsDt, connVM);
            string VAT6_8English = commonDal.settingsDesktop("Reports", "VAT6_8English", settingsDt, connVM);
            string companyCode = commonDal.settingsDesktop("CompanyCode", "Code", settingsDt, connVM);
            string FontSize6_7 = commonDal.settingsDesktop("FontSize", "VAT6_7", settingsDt, connVM);
            string Quantity6_3 = commonDal.settingsDesktop("DecimalPlace", "Quantity6_3", settingsDt, connVM);
            string Amount6_3 = commonDal.settingsDesktop("DecimalPlace", "Amount6_3", settingsDt, connVM);
            #endregion

            #region Report Generation

            try
            {

                //UserInformationDAL _udal = new UserInformationDAL();

                //uvm = _udal.SelectAll(Convert.ToInt32(connVM.CurrentUserID), null, null, null, null, connVM).FirstOrDefault();


                AlReadyPrintNo = 1;


                AlReadyPrintNo = AlReadyPrintNo - PrintCopy;


                #region VAT 11 Page Setup and Item Nature
                string vVAT11Letter = commonDal.settingsDesktop("Sale", "VAT6_3Letter", settingsDt, connVM);
                string vVAT11A4 = commonDal.settingsDesktop("Sale", "VAT6_3A4", settingsDt, connVM);
                string vVAT11Legal = commonDal.settingsDesktop("Sale", "VAT6_3Legal", settingsDt, connVM);
                ItemNature = commonDal.settingsDesktop("Sale", "ItemNature", settingsDt, connVM);
                vPGroupInReport = commonDal.settingsDesktop("Sale", "PGroupInReport", settingsDt, connVM);



                if (VAT11A4 == true)
                {
                    VAT11PageSize = "A4";
                }
                else if (VAT11Letter == true)
                {
                    VAT11PageSize = "Letter";
                }
                else if (VAT11Legal == true)
                {
                    VAT11PageSize = "Legal";
                }
                else
                {
                    VAT11PageSize = "A4";
                }
                #endregion VAT 11 Page Setup and Item Nature


                string PrintDate = _dbsqlConnection.ServerPrintDateTime();

                #region Variables

                string vQuantity, vSDAmount, vQty_UnitCost, vQty_UnitCost_SDAmount, vVATAmount, vSubtotal, QtyInWord;// = string.Empty;  

                vQuantity = string.Empty;
                vSDAmount = string.Empty;
                vQty_UnitCost = string.Empty;
                vQty_UnitCost_SDAmount = string.Empty;
                vVATAmount = string.Empty;
                vSubtotal = string.Empty;
                QtyInWord = string.Empty;
                decimal Quantity = 0,
                           SDAmount = 0,
                           Qty_UnitCost = 0,
                           Qty_UnitCost_SDAmount = 0,
                           VATAmount = 0,
                           TotalVDSAmount = 0,
                           Subtotal = 0;

                #endregion

                #region Datatable - Table Name

                //start Complete
                if (CommercialImporter)
                {
                    ReportResultVAT11.Tables[0].TableName = "DsVAT11";

                }
                else if (rbtnCN)
                {
                    if (VAT2012V2 <= invoiceDateTime)
                    {
                        ////New Report -- 
                        ReportResultCreditNote.Tables[0].TableName = "DsVAT11";


                    }
                    else
                    {
                        ReportResultCreditNote.Tables[0].TableName = "DsCreditNote";


                    }
                }
                else if (rbtnRCN)
                {
                    if (VAT2012V2 <= invoiceDateTime)
                    {
                        ////New Report -- 
                        ReportResultCreditNote.Tables[0].TableName = "DsVAT11";


                    }
                    else
                    {
                        ReportResultCreditNote.Tables[0].TableName = "DsCreditNote";


                    }
                }
                else if (rbtnDN || rbtnPurchaseReurn)
                {

                    if (VAT2012V2 <= invoiceDateTime)
                    {
                        ////New Report -- 
                        ReportResultDebitNote.Tables[0].TableName = "DsVAT11";


                    }
                    else
                    {
                        ReportResultDebitNote.Tables[0].TableName = "DsDebitNote";


                    }
                }
                else
                {
                    if (VAT11Name.ToLower() == "newsqr4")
                    {
                        ReportResultVAT11.Tables[0].TableName = "NewDsVAT11";

                    }
                    else
                    {
                        ReportResultVAT11.Tables[0].TableName = "DsVAT11";

                    }
                    ReportResultTracking.Tables[0].TableName = "DsSaleTracking";
                    if (!BureauInfoVM.IsBureau == true)
                    {
                        #region InWord

                        for (int i = 0; i < ReportResultVAT11.Tables[1].Rows.Count; i++)
                        {
                            var tt = ReportResultVAT11.Tables[1].Rows[i]["qty"];
                            tt = decimal.Parse(tt.ToString()).ToString("G29");
                            //var tt1 = OrdinaryVATDesktop.ConvertToWords(tt.ToString()).ToString();

                            var uom = ReportResultVAT11.Tables[1].Rows[i]["uom"].ToString();
                            string QtyWord = "";
                            if (tt.ToString().Contains("."))
                            {
                                QtyWord = "";
                                var preF = tt.ToString().Split('.')[0];
                                var postF = tt.ToString().Split('.')[1];
                                string WpreF = OrdinaryVATDesktop.NumberToWords(Convert.ToInt32(preF));
                                string WpostF = OrdinaryVATDesktop.NumberToWords(Convert.ToInt32(postF));
                                QtyWord = WpreF + " point " + WpostF;

                            }
                            else
                            {
                                var preF = tt.ToString().Split('.')[0];
                                string WpreF = OrdinaryVATDesktop.NumberToWords(Convert.ToInt32(preF));
                                QtyWord = WpreF;

                            }

                            QtyInWord = QtyInWord + uom + ": " + QtyWord + ", ";
                        }

                        if (Convert.ToInt32(QtyInWord.Length) <= 0)
                        {
                            QtyInWord = "In Words(Quantity): .";

                        }
                        else
                        {
                            QtyInWord = "In Words(Quantity): " +
                                        QtyInWord.Substring(0, Convert.ToInt32(QtyInWord.Length) - 2).ToString() + ".";

                        }
                        QtyInWord = QtyInWord.ToUpper();



                        for (int i = 0; i < ReportResultVAT11.Tables[0].Rows.Count; i++)
                        {
                            Quantity = Quantity + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]);
                            SDAmount = SDAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["SDAmount"]);
                            Qty_UnitCost = Qty_UnitCost + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]) * Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["UnitCost"]);
                            Qty_UnitCost_SDAmount = Qty_UnitCost_SDAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Quantity"]) * Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["UnitCost"]) + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["SDAmount"]);
                            VATAmount = VATAmount + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["VATAmount"]);
                            Subtotal = Subtotal + Convert.ToDecimal(ReportResultVAT11.Tables[0].Rows[i]["Subtotal"]);
                        }

                        vQuantity = Convert.ToDecimal(Quantity).ToString("0,0.00");
                        vSDAmount = Convert.ToDecimal(SDAmount).ToString("0,0.00");
                        vQty_UnitCost = Convert.ToDecimal(Qty_UnitCost).ToString("0,0.00");
                        vQty_UnitCost_SDAmount = Convert.ToDecimal(Qty_UnitCost_SDAmount).ToString("0,0.00");
                        vVATAmount = Convert.ToDecimal(VATAmount).ToString("0,0.00");
                        vSubtotal = Convert.ToDecimal(Subtotal).ToString("0,0.00");


                        #endregion InWord
                    }
                }
                #endregion

                string prefix = "";


                if (IsGatepass)
                {
                    objrpt = new ReportDocument();
                    //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptGatePassNew.rpt");

                    objrpt = new RptGatePassNew();
                    objrpt.SetDataSource(ReportResultVAT11);
                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    _vCommonFormMethod = new CommonFormMethod();
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Gate Pass for Finished Products");
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);


                }
                else
                {
                    #region Data Group For DHL

                    if (companyCode == "DHL" && (rbtnCN || rbtnDN))
                    {
                        #region Credit Note

                        #region Comments

                        //DataTable dt = ReportResultCreditNote.Tables[0].Copy();


                        //string tt = JsonConvert.SerializeObject(dt);

                        //List<DhlCreditNOteMOdel> list = JsonConvert.DeserializeObject<List<DhlCreditNOteMOdel>>(tt);

                        //List<DhlCreditNOteMOdel> newList = new List<DhlCreditNOteMOdel>();

                        //var group = list.GroupBy(x => x.ProductName);
                        //foreach (IGrouping<string, DhlCreditNOteMOdel> grouping in group)
                        //{

                        //    DhlCreditNOteMOdel model = grouping.FirstOrDefault();

                        //    model.Quantity = grouping.Sum(x => x.Quantity);
                        //    model.DeductionAmount = grouping.Sum(x => x.DeductionAmount);
                        //    model.CDNVATAmount = grouping.Sum(x => x.CDNVATAmount);
                        //    model.CDNSDAmount = grouping.Sum(x => x.CDNSDAmount);
                        //    model.CDNSubtotal = grouping.Sum(x => x.CDNSubtotal);
                        //    model.PromotionalQuantity = grouping.Sum(x => x.PromotionalQuantity);
                        //    model.NBRPrice = grouping.Average(x => x.NBRPrice);
                        //    model.SubCost = grouping.Sum(x => x.SubCost);
                        //    model.SDAmount = grouping.Sum(x => x.SDAmount);
                        //    model.VATAmount = grouping.Sum(x => x.VATAmount);
                        //    model.OldSubTotal = grouping.Sum(x => x.OldSubTotal);
                        //    model.oldSDAmount = grouping.Sum(x => x.oldSDAmount);
                        //    model.oldVATAmount = grouping.Sum(x => x.oldVATAmount);
                        //    model.Sort = 0;
                        //    newList.Add(model);
                        //}


                        //newList = newList.Select(x =>
                        //{
                        //    if (x.ProductName.ToLower() == "Express Worldwide Doc".ToLower())
                        //    {
                        //        x.Sort = 1;
                        //    }
                        //    else if (x.ProductName.ToLower() == "Express Worldwide Nondoc".ToLower())
                        //    {
                        //        x.Sort = 2;
                        //    }
                        //    else if (x.ProductName.ToLower() == "Express 9:00 Nondoc".ToLower())
                        //    {
                        //        x.Sort = 3;
                        //    }
                        //    else if (x.ProductName.ToLower() == "Express 10:30 Nondoc".ToLower())
                        //    {
                        //        x.Sort = 4;
                        //    }
                        //    else if (x.ProductName.ToLower() == "Express 12:00 Nondoc".ToLower())
                        //    {
                        //        x.Sort = 5;
                        //    }
                        //    return x;
                        //}).OrderBy(x => x.Sort)
                        //    .ToList();

                        //tt = JsonConvert.SerializeObject(newList);
                        //DataTable result = JsonConvert.DeserializeObject<DataTable>(tt);
                        //result.Columns.Remove("Sort");
                        //ReportResultCreditNote = new DataSet();


                        //ReportResultCreditNote.Tables.Add(result);
                        //ReportResultCreditNote.Tables[0].TableName = "DsVAT11";

                        #endregion

                        DataTable dt = null;
                        dt = rbtnCN ?
                             ReportResultCreditNote.Tables[0].Copy()
                            : ReportResultDebitNote.Tables[0].Copy();



                        var newSort = (from row in dt.AsEnumerable()
                                       group row by new
                                       {
                                           SalesInvoiceNo = row.Field<string>("SalesInvoiceNo"),
                                           InvoiceDate = row.Field<string>("InvoiceDate"),
                                           CustomerName = row.Field<string>("CustomerName"),
                                           ProductName = row.Field<string>("ProductName"),
                                           Address1 = row.Field<string>("Address1"),
                                           Address2 = row.Field<string>("Address2"),
                                           Address3 = row.Field<string>("Address3"),
                                           TelephoneNo = row.Field<string>("TelephoneNo"),
                                           DeliveryAddress1 = row.Field<string>("DeliveryAddress1"),
                                           DeliveryAddress2 = row.Field<string>("DeliveryAddress2"),
                                           DeliveryAddress3 = row.Field<string>("DeliveryAddress3"),
                                           VehicleType = row.Field<string>("VehicleType"),
                                           VehicleNo = row.Field<string>("VehicleNo"),
                                           ProductNameOld = row.Field<string>("ProductNameOld"),
                                           ProductDescription = row.Field<string>("ProductDescription"),
                                           ProductGroup = row.Field<string>("ProductGroup"),
                                           UOM = row.Field<string>("UOM"),
                                           ProductCommercialName = row.Field<string>("ProductCommercialName"),
                                           VATRegistrationNo = row.Field<string>("VATRegistrationNo"),
                                           SerialNo = row.Field<string>("SerialNo"),
                                           AlReadyPrint = row.Field<int>("AlReadyPrint"),
                                           ImportIDExcel = row.Field<string>("ImportIDExcel"),
                                           Comments = row.Field<string>("Comments"),
                                           VATType = row.Field<string>("VATType"),
                                           LCNumber = row.Field<string>("LCNumber"),
                                           LCBank = row.Field<string>("LCBank"),
                                           PINo = row.Field<string>("PINo"),
                                           EXPFormNo = row.Field<string>("EXPFormNo"),
                                           BranchId = row.Field<int>("BranchId"),
                                           SignatoryName = row.Field<string>("SignatoryName"),
                                           SignatoryDesig = row.Field<string>("SignatoryDesig"),
                                           SerialNo1 = row.Field<string>("SerialNo1"),
                                           SaleType = row.Field<string>("SaleType"),
                                           PreviousSalesInvoiceNo = row.Field<string>("PreviousSalesInvoiceNo"),
                                           TransactionType = row.Field<string>("TransactionType"),
                                           CurrencyID = row.Field<string>("CurrencyID"),
                                           TPurchaseInvoiceNo = row.Field<string>("TPurchaseInvoiceNo"),
                                           TBENumber = row.Field<string>("TBENumber"),
                                           TCustomHouse = row.Field<string>("TCustomHouse"),
                                           CustomerCode = row.Field<string>("CustomerCode"),
                                           VATRate = row.Field<decimal>("VATRate"),

                                           PreviousNBRPrice = row.Field<decimal>("PreviousNBRPrice"),
                                           PreviousQuantity = row.Field<decimal>("PreviousQuantity"),
                                           PreviousSubTotal = row.Field<decimal>("PreviousSubTotal"),
                                           PreviousVATRate = row.Field<decimal>("PreviousVATRate"),
                                           PreviousVATAmount = row.Field<decimal>("PreviousVATAmount"),
                                           PreviousSD = row.Field<decimal>("PreviousSD"),
                                           PreviousSDAmount = row.Field<decimal>("PreviousSDAmount"),
                                           LineTotal = row.Field<decimal>("LineTotal"),
                                           PreLineTotal = row.Field<decimal>("PreLineTotal"),

                                           ReasonOfReturn = row.Field<string>("ReasonOfReturn"),
                                           PreviousSalesInvoiceNoD = row.Field<string>("PreviousSalesInvoiceNoD"),
                                           PreviousInvoiceDateTime = row.Field<string>("PreviousInvoiceDateTime"),
                                           PreviousUOM = row.Field<string>("PreviousUOM")


                                       } into grp
                                       select new
                                       {
                                           SalesInvoiceNo = grp.Key.SalesInvoiceNo,
                                           InvoiceDate = grp.Key.InvoiceDate,
                                           CustomerName = grp.Key.CustomerName,
                                           ProductName = grp.Key.ProductName.ToUpper(),
                                           Address1 = grp.Key.Address1,
                                           Address2 = grp.Key.Address2,
                                           Address3 = grp.Key.Address3,
                                           TelephoneNo = grp.Key.TelephoneNo,
                                           DeliveryAddress1 = grp.Key.DeliveryAddress1,
                                           DeliveryAddress2 = grp.Key.DeliveryAddress2,
                                           DeliveryAddress3 = grp.Key.DeliveryAddress3,
                                           VehicleType = grp.Key.VehicleType,
                                           VehicleNo = grp.Key.VehicleNo,
                                           ProductNameOld = grp.Key.ProductNameOld,
                                           ProductDescription = grp.Key.ProductDescription,
                                           ProductGroup = grp.Key.ProductGroup,
                                           UOM = grp.Key.UOM,
                                           ProductCommercialName = grp.Key.ProductCommercialName,
                                           VATRegistrationNo = grp.Key.VATRegistrationNo,
                                           SerialNo = grp.Key.SerialNo,
                                           AlReadyPrint = grp.Key.AlReadyPrint,
                                           ImportIDExcel = grp.Key.ImportIDExcel,
                                           Comments = grp.Key.Comments,
                                           VATType = grp.Key.VATType,
                                           LCNumber = grp.Key.LCNumber,
                                           LCBank = grp.Key.LCBank,
                                           PINo = grp.Key.PINo,
                                           EXPFormNo = grp.Key.EXPFormNo,
                                           BranchId = grp.Key.BranchId,
                                           SignatoryName = grp.Key.SignatoryName,
                                           SignatoryDesig = grp.Key.SignatoryDesig,
                                           SerialNo1 = grp.Key.SerialNo1,
                                           SaleType = grp.Key.SaleType,
                                           PreviousSalesInvoiceNo = grp.Key.PreviousSalesInvoiceNo,
                                           TransactionType = grp.Key.TransactionType,
                                           CurrencyID = grp.Key.CurrencyID,
                                           TPurchaseInvoiceNo = grp.Key.TPurchaseInvoiceNo,
                                           TBENumber = grp.Key.TBENumber,
                                           TCustomHouse = grp.Key.TCustomHouse,
                                           VATRate = grp.Key.VATRate,
                                           grp.Key.CustomerCode,
                                           Quantity = grp.Sum(r => r.Field<Decimal>("Quantity")),
                                           UnitCost = grp.Average(r => r.Field<Decimal>("UnitCost")),
                                           SDAmount = grp.Sum(r => r.Field<Decimal>("SDAmount")),
                                           VATAmount = grp.Sum(r => r.Field<Decimal>("VATAmount")),
                                           LineTotal = grp.Sum(r => r.Field<Decimal>("LineTotal")),
                                           PreLineTotal = grp.Sum(r => r.Field<Decimal>("PreLineTotal")),
                                           grp.Key.ReasonOfReturn,
                                           grp.Key.PreviousSalesInvoiceNoD,
                                           grp.Key.PreviousInvoiceDateTime,
                                           grp.Key.PreviousUOM,
                                           Sort = 0,

                                           PreviousNBRPrice = grp.Average(r => r.Field<Decimal>("PreviousNBRPrice")),
                                           PreviousSDAmount = grp.Sum(r => r.Field<Decimal>("PreviousSDAmount")),
                                           PreviousVATAmount = grp.Sum(r => r.Field<Decimal>("PreviousVATAmount")),
                                           PreviousSubTotal = grp.Sum(r => r.Field<Decimal>("PreviousSubTotal")),
                                           PreviousQuantity = grp.Sum(r => r.Field<Decimal>("PreviousQuantity"))
                                       }).ToList();

                        string tt = JsonConvert.SerializeObject(newSort);

                        List<DhlCreditNoteMOdel> list = JsonConvert.DeserializeObject<List<DhlCreditNoteMOdel>>(tt);

                        list = list.Select(x =>
                        {
                            if (x.ProductName.ToLower() == "Express Worldwide Doc".ToLower())
                            {
                                x.Sort = 1;
                            }
                            else if (x.ProductName.ToLower() == "Express Worldwide Nondoc".ToLower())
                            {
                                x.Sort = 2;
                            }
                            else if (x.ProductName.ToLower() == "Express 9:00 Nondoc".ToLower())
                            {
                                x.Sort = 3;
                            }
                            else if (x.ProductName.ToLower() == "Express 10:30 Nondoc".ToLower())
                            {
                                x.Sort = 4;
                            }
                            else if (x.ProductName.ToLower() == "Express 12:00 Nondoc".ToLower())
                            {
                                x.Sort = 5;
                            }
                            return x;
                        }).OrderBy(x => x.Sort)
                            .ToList();


                        tt = JsonConvert.SerializeObject(list);
                        DataTable result = JsonConvert.DeserializeObject<DataTable>(tt);
                        result.Columns.Remove("Sort");


                        if (rbtnCN)
                        {
                            ReportResultCreditNote = new DataSet();
                            ReportResultCreditNote.Tables.Add(result);
                            ReportResultCreditNote.Tables[0].TableName = "DsVAT11";
                        }
                        else
                        {
                            ReportResultDebitNote = new DataSet();
                            ReportResultDebitNote.Tables.Add(result);
                            ReportResultDebitNote.Tables[0].TableName = "DsVAT11";
                        }

                        #endregion
                    }


                    #endregion

                    //ReportClass objrpt = new ReportClass();
                    #region CommercialImporter
                    if (CommercialImporter)
                    {

                        objrpt.DataDefinition.FormulaFields["Quantity"].Text = "'" + vQuantity + "'";
                        objrpt.DataDefinition.FormulaFields["SDAmount"].Text = "'" + vSDAmount + "'";
                        objrpt.DataDefinition.FormulaFields["Qty_UnitCost"].Text = "'" + vQty_UnitCost + "'";
                        objrpt.DataDefinition.FormulaFields["Qty_UnitCost_SDAmount"].Text = "'" + vQty_UnitCost_SDAmount + "'";
                        objrpt.DataDefinition.FormulaFields["VATAmount"].Text = "'" + vVATAmount + "'";
                        objrpt.DataDefinition.FormulaFields["Subtotal"].Text = "'" + vSubtotal + "'";
                        objrpt.DataDefinition.FormulaFields["ItemNature"].Text = "'" + ItemNature + "'";
                        objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + EntryUserName + "'";
                        objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 11'";
                        objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + connVM.CompanyName + "'";
                        //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + OrdinaryVATDesktop.CompanyLegalName + "'";
                        objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + connVM.Address1 + "'";
                        objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + connVM.Address2 + "'";
                        objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + connVM.Address3 + "'";
                        objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + connVM.TelephoneNo + "'";
                        objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + connVM.FaxNo + "'";
                        objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + connVM.VatRegistrationNo + "'";
                        objrpt.DataDefinition.FormulaFields["QtyInWord"].Text = "'" + QtyInWord + "'";

                        objrpt.SetDataSource(ReportResultVAT11);
                    }
                    #endregion CommercialImporter

                    #region CN / CreditNote

                    else if (rbtnCN)
                    {
                        //objrpt = new RptVAT6_7();

                        ////New Report -- 
                        //objrpt = new RptVAT6_7_V12V2();
                        if (VAT2012V2 <= invoiceDateTime)
                        {
                            if (VAT11Name.ToLower() == "bata")
                            {

                                if (VAT2022V2 <= invoiceDateTime)
                                {
                                    objrpt = new RptVAT6_7_Bata_New2_V12V2();
                                }
                                else
                                {
                                    //objrpt = new RptVAT6_7_Bata_V12V2();
                                    objrpt = new RptVAT6_7_Bata_New_V12V2();

                                }

                            }

                            else if (VAT11Name.ToLower() == "sqr4" || VAT11Name.ToLower() == "newsqr4")
                            {
                                if (VAT2022V2 <= invoiceDateTime)
                                {
                                    objrpt = new RptVAT6_7_V12V2_STL_New_SFBL();
                                }
                                else
                                {
                                    objrpt = new RptVAT6_7_V12V2_STL_SFBL();
                                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\V12V2" + @"\RptVAT6_7_V12V2_STL_SFBL.rpt");

                                }

                            }
                            else if (VAT11Name.ToLower() == "aci")
                            {
                                if (CompanyCode.ToLower() == "formulation")
                                {
                                    if (VAT2022V2 <= invoiceDateTime)
                                    {
                                        objrpt = new RptVAT6_7_V12V2_STL_New_SFBL();
                                    }
                                    else
                                    {
                                        objrpt = new RptVAT6_7_V12V2_STL_SFBL();
                                    }

                                }
                                else
                                {
                                    if (VAT2022V2 <= invoiceDateTime)
                                    {
                                        objrpt = new RptVAT6_7_ACI_New_V12V2();
                                    }
                                    else
                                    {
                                        objrpt = new RptVAT6_7_ACI_V12V2();
                                    }
                                }
                            }
                            //Novo Nordisk Pharma (Pvt.) Ltd
                            else if (VAT11Name.ToLower() == "nnpl")
                            {
                                if (VAT2022V2 <= invoiceDateTime)
                                {
                                    objrpt = new RptVAT6_7_ACI_New_V12V2();
                                }
                                else
                                {
                                    objrpt = new RptVAT6_7_ACI_V12V2();
                                }

                            }

                            else if (VAT11Name.ToLower() == "rnl")
                            {
                                if (VAT2022V2 <= invoiceDateTime)
                                {
                                    objrpt = new RptVAT6_7_V12V2_New_RPL();
                                }
                                else
                                {
                                    objrpt = new RptVAT6_7_V12V2_RPL();
                                }

                            }
                            else if (VAT11Name.ToLower() == "bcl")
                            {

                                if (VAT2022V2 <= invoiceDateTime)
                                {
                                    objrpt = new RptVAT6_7_BCL_New_V12V2();
                                }
                                else
                                {
                                    objrpt = new RptVAT6_7_BCL_V12V2();
                                }

                            }
                            else if (VAT11Name.ToLower() == "dhl")
                            {
                                objrpt = new RptVAT6_7_DHL_V12V2();

                            }
                            else if (VAT11Name.ToLower() == "aciy")
                            {
                                if (VAT2022V2 <= invoiceDateTime)
                                {
                                    objrpt = new RptVAT6_7_Yamaha_New_V12V2();
                                }
                                else
                                {
                                    objrpt = new RptVAT6_7_Yamaha_V12V2();
                                    ////objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\V12V2" + @"\RptVAT6_7_Yamaha_V12V2.rpt");
                                }

                            }
                            else if (VAT11Name.ToLower() == "mcl")
                            {
                                if (VAT2022V2 <= invoiceDateTime)
                                {
                                    objrpt = new RptVAT6_7_MCL_New_V12V2();
                                }
                                else
                                {
                                    objrpt = new RptVAT6_7_V12V2();
                                    ////objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\V12V2" + @"\RptVAT6_7_Yamaha_V12V2.rpt");
                                }

                            }
                            else
                            {
                                if (VAT2022V2 <= invoiceDateTime)
                                {
                                    objrpt = new RptVAT6_7_New_V12V2();
                                }
                                else
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_7_V12V2();
                                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_7_V12V2.rpt");
                                }

                            }

                        }

                        else
                        {
                            objrpt = new RptVAT6_7();

                        }

                        ////objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_7.rpt");

                        prefix = "CreditNote";

                        objrpt.SetDataSource(ReportResultCreditNote);

                        //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + connVM.CompanyName + "'";
                        //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + connVM.Address1 + "'";
                        //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + connVM.Address2 + "'";
                        //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + connVM.Address3 + "'";
                        //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + connVM.TelephoneNo + "'";
                        //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + connVM.FaxNo + "'";
                        //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + connVM.VatRegistrationNo + "'";
                        //FormulaFieldDefinitions crFormulaF;
                        crFormulaF = objrpt.DataDefinition.FormulaFields;
                        _vCommonFormMethod = new CommonFormMethod();
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PrintDate", PrintDate);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", VAT6_7English);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", FontSize6_7);
                        //  objrpt.DataDefinition.FormulaFields["PrintDate"].Text = "'" + PrintDate + "'";
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_3", Quantity6_3);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_3", Amount6_3);

                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
                    }
                    #endregion CN

                    #region RCN / RawCreditNote

                    else if (rbtnRCN)
                    {
                        //objrpt = new RptVAT6_7();

                        ////New Report -- 
                        //objrpt = new RptVAT6_7_V12V2();
                        if (VAT2012V2 <= invoiceDateTime)
                        {
                            ////New Report -- 
                            objrpt = new RptVAT6_7_V12V2();

                        }
                        else
                        {
                            objrpt = new RptVAT6_7();

                        }

                        ////objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_7.rpt");

                        prefix = "CreditNote";

                        objrpt.SetDataSource(ReportResultCreditNote);

                        //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + connVM.CompanyName + "'";
                        //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + connVM.Address1 + "'";
                        //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + connVM.Address2 + "'";
                        //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + connVM.Address3 + "'";
                        //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + connVM.TelephoneNo + "'";
                        //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + connVM.FaxNo + "'";
                        //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + connVM.VatRegistrationNo + "'";

                        //FormulaFieldDefinitions crFormulaF;
                        crFormulaF = objrpt.DataDefinition.FormulaFields;
                        _vCommonFormMethod = new CommonFormMethod();
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PrintDate", PrintDate);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", VAT6_7English);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", FontSize6_7);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
                        //  objrpt.DataDefinition.FormulaFields["PrintDate"].Text = "'" + PrintDate + "'";

                    }
                    #endregion RCN

                    #region DN / DebitNote
                    else if (rbtnDN || rbtnPurchaseReurn)
                    {
                        ////if (chkValueOnly)
                        ////{
                        ////    objrpt = new RptDebitNoteAmount();
                        ////}
                        ////else
                        ////{
                        ////    objrpt = new RptVAT6_8();
                        ////}

                        //objrpt = new RptVAT6_8();

                        ////New Report -- 
                        //objrpt = new RptVAT6_8_V12V2();
                        if (VAT2012V2 <= invoiceDateTime)
                        {

                            if (VAT11Name.ToLower() == "bata")
                            {
                                if (VAT2022V2 <= invoiceDateTime)
                                {
                                    objrpt = new RptVAT6_8_Bata_New_Report23_V12V2();
                                }
                                else
                                {
                                    objrpt = new RptVAT6_8_Bata_New_V12V2();
                                }
                                //objrpt = new RptVAT6_8_Bata_V12V2();
                                //objrpt = new RptVAT6_8_Bata_New_V12V2();

                            }
                            else if (VAT11Name.ToLower() == "aci")
                            {

                                if (VAT2022V2 <= invoiceDateTime)
                                {
                                    objrpt = new RptVAT6_8_ACI_New_V12V2();
                                }
                                else
                                {
                                    objrpt = new RptVAT6_8_ACI_V12V2();
                                }
                                //objrpt = new RptVAT6_8_ACI_V12V2();
                            }
                            //Novo Nordisk Pharma (Pvt.) Ltd
                            else if (VAT11Name.ToLower() == "nnpl")
                            {
                                objrpt = new RptVAT6_8_ACI_V12V2();
                            }
                            else if (VAT11Name.ToLower() == "rnl")
                            {
                                objrpt = new RptVAT6_8_V12V2_RPL();
                            }
                            else if (VAT11Name.ToLower() == "dhl")
                            {
                                objrpt = new RptVAT6_8_DHL_V12V2();
                            }
                            else
                            {

                                if (VAT2022V2 <= invoiceDateTime)
                                {
                                    //objrpt = new RptVAT6_7_MCL_New_V12V2();
                                    objrpt = new RptVAT6_8_New_V12V2();
                                }
                                else
                                {
                                    objrpt = new RptVAT6_8_V12V2();
                                }
                                ////New Report -- 
                                //objrpt = new RptVAT6_8_V12V2();

                            }


                        }

                        else
                        {
                            objrpt = new RptVAT6_8();



                        }

                        //objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_8.rpt");

                        prefix = "DebitNote";


                        objrpt.SetDataSource(ReportResultDebitNote);
                        //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + connVM.CompanyName + "'";
                        //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + connVM.Address1 + "'";
                        //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + connVM.Address2 + "'";
                        //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + connVM.Address3 + "'";
                        //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + connVM.TelephoneNo + "'";
                        //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + connVM.FaxNo + "'";
                        //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + connVM.VatRegistrationNo + "'";
                        //FormulaFieldDefinitions crFormulaF;
                        crFormulaF = objrpt.DataDefinition.FormulaFields;
                        _vCommonFormMethod = new CommonFormMethod();
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PrintDate", PrintDate);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", VAT6_8English);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", FontSize6_7);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_3", Quantity6_3);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_3", Amount6_3);

                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
                        //objrpt.DataDefinition.FormulaFields["PrintDate"].Text = "'" + PrintDate + "'";
                    }
                    #endregion CN

                    #region rbtnTrading
                    else if (rbtnTrading)
                    {

                        if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                        {
                            //////objrpt = new ReportDocument();
                            //////objrpt.Load(OrdinaryVATDesktop.ReportAppPath + @"\RptVAT6_3.rpt");

                            //objrpt = new RptVAT6_3();
                            if (VAT2012V2 <= invoiceDateTime)
                            {
                                ////New Report -- 
                                objrpt = new RptVAT6_3_V12V2();
                            }
                            else
                            {
                                objrpt = new RptVAT6_3();
                            }
                            //////objrpt.DataDefinition.FormulaFields["frmTotalVDSAmount"].Text = "'" + TotalVDSAmount + "'";
                            objrpt.DataDefinition.FormulaFields["PGroupInReport"].Text = "'" + vPGroupInReport + "'";


                        }
                        else
                        {
                            //objrpt = new RptVAT11Ka();
                            prefix = "VAT11Ka";
                        }
                        objrpt.SetDataSource(ReportResultVAT11);
                        //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + connVM.CompanyName + "'";
                        //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + connVM.Address1 + "'";
                        //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + connVM.Address2 + "'";
                        //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + connVM.Address3 + "'";
                        //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + connVM.VatRegistrationNo + "'";
                        crFormulaF = objrpt.DataDefinition.FormulaFields;
                        _vCommonFormMethod = new CommonFormMethod();
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);

                    }//end if
                    #endregion rbtnTrading

                    #region rbtnService
                    else if (false)
                    {
                        if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                        {
                            //////objrpt = new ReportDocument();
                            //////objrpt.Load(OrdinaryVATDesktop.ReportAppPath + @"\RptVAT6_3.rpt");

                            //objrpt = new RptVAT6_3();
                            if (VAT2012V2 <= invoiceDateTime)
                            {
                                ////New Report -- 
                                objrpt = new RptVAT6_3_V12V2();
                            }
                            else
                            {
                                objrpt = new RptVAT6_3();
                            }
                            //////objrpt.DataDefinition.FormulaFields["frmTotalVDSAmount"].Text = "'" + TotalVDSAmount + "'";
                            objrpt.DataDefinition.FormulaFields["PGroupInReport"].Text = "'" + vPGroupInReport + "'";


                        }
                        else
                        {
                            if (chkIs11)
                            {
                                prefix = "VAT11";
                            }
                            else
                            {
                                prefix = "VAT11Gha";
                            }
                        }
                        objrpt.SetDataSource(ReportResultVAT11);
                        objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + EntryUserName + "'";
                        objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 11 Gha'";
                        objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + connVM.CompanyName + "'";
                        objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + connVM.Address1 + "'";
                        objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + connVM.Address2 + "'";
                        objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + connVM.Address3 + "'";
                        objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + connVM.TelephoneNo + "'";
                        objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + connVM.FaxNo + "'";
                        objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + connVM.VatRegistrationNo +
                                                                                        "'";

                        #region NEW ADDED BY LITON

                        //vQuantity,vSDAmount,vQty_UnitCost,vQty_UnitCost_SDAmount,vVATAmount,vSubtotal
                        objrpt.DataDefinition.FormulaFields["ItemNature"].Text = "'" + ItemNature + "'";
                        objrpt.DataDefinition.FormulaFields["QtyInWord"].Text = "'" + QtyInWord + "'";

                        objrpt.DataDefinition.FormulaFields["Quantity"].Text = "'" + vQuantity + "'";
                        objrpt.DataDefinition.FormulaFields["SDAmount"].Text = "'" + vSDAmount + "'";
                        objrpt.DataDefinition.FormulaFields["Qty_UnitCost"].Text = "'" + vQty_UnitCost + "'";
                        objrpt.DataDefinition.FormulaFields["Qty_UnitCost_SDAmount"].Text = "'" + vQty_UnitCost_SDAmount + "'";
                        objrpt.DataDefinition.FormulaFields["VATAmount"].Text = "'" + vVATAmount + "'";
                        objrpt.DataDefinition.FormulaFields["Subtotal"].Text = "'" + vSubtotal + "'";

                        #endregion NEW ADDED BY LITON


                    }//end
                    #endregion rbtnService

                    #region rbtnTollIssue
                    else if ((rbtnTollIssue || transactionType == "TollFinishIssue")
                        )
                    {
                        if (VAT11Name.ToLower() == "aci")
                        {

                            objrpt = new RptVAT6_4_ACI();

                        }
                        else if (VAT11Name.ToLower() == "acipharma")
                        {
                            objrpt = new RptVAT6_4_ACI();

                        }
                        else if (VAT11Name.ToLower() == "sqr4" || VAT11Name.ToLower() == "newsqr4")
                        {
                            objrpt = new RptVAT6_4_SQR4();

                        }
                        else if (VAT11Name.ToLower() == "bata")
                        {
                            objrpt = new RptVAT6_4_Bata();
                        }
                        else if (VAT11Name.ToLower() == "pccl")
                        {
                            objrpt = new RptVAT6_4_Padma();
                        }
                        else if (VAT11Name.ToLower() == "scbl")
                        {
                            objrpt = new RptVAT6_4_SCBL();
                        }
                        else if (VAT11Name.ToUpper() == "SSPIL")
                        {
                            objrpt = new RptVAT6_4_SSPIL();
                        }
                        //Novo Nordisk Pharma (Pvt.) Ltd
                        else if (VAT11Name.ToLower() == "nnpl")
                        {
                            objrpt = new RptVAT6_4_ACI();
                        }
                        else if (VAT11Name.ToUpper() == "ACICENTRAL")
                        {
                            objrpt = new RptVAT6_4_ACICENTRAL();
                        }
                        else if (VAT11Name.ToUpper() == "CNL")
                        {
                            objrpt = new RptVAT6_4_CNL();
                        }
                        else
                        {
                            objrpt = new RptVAT6_4();

                        }

                        if (CompanyCode.ToUpper() == "ACI CB HYGINE".ToUpper())
                        {
                            objrpt = new RptVAT6_4_ACI_CBHygine();
                        }

                        prefix = "VAT 6.4";
                        objrpt.SetDataSource(ReportResultVAT11);
                        objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + EntryUserName + "'";
                        objrpt.DataDefinition.FormulaFields["MaterialType"].Text = "'" + MaterialType + "'";
                        if ((transactionType == "TollFinishIssue"))
                        {
                            objrpt.DataDefinition.FormulaFields["MaterialType"].Text = "'Finish Product'";
                        }
                        objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 6.4'";
                        objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + connVM.CompanyName + "'";
                        //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + OrdinaryVATDesktop.CompanyLegalName + "'";
                        objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + connVM.Address1 + "'";
                        objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + connVM.Address2 + "'";
                        objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + connVM.Address3 + "'";
                        objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + connVM.TelephoneNo + "'";
                        objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + connVM.FaxNo + "'";
                        objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + connVM.VatRegistrationNo +
                                                                                        "'";

                        string Quantity6_4 = commonDal.settings("DecimalPlace", "Quantity6_4");
                        string Amount6_4 = commonDal.settings("DecimalPlace", "Amount6_4");

                        objrpt.DataDefinition.FormulaFields["Quantity6_4"].Text = "'" + Quantity6_4 + "'";
                        objrpt.DataDefinition.FormulaFields["Amount6_4"].Text = "'" + Amount6_4 + "'";
                    }
                    #endregion rbtnTollIssue

                    #region rbtnContractorRawIssue
                    else if ((rbtnContractorRawIssue || transactionType == "ContractorRawIssue")
                        )
                    {
                        if (VAT11Name.ToLower() == "aci")
                        {
                            objrpt = new RptVAT6_4_ACI();

                        }
                        else if (VAT11Name.ToLower() == "acipharma")
                        {
                            objrpt = new RptVAT6_4_ACI();

                        }
                        else if (VAT11Name.ToLower() == "sqr4" || VAT11Name.ToLower() == "newsqr4")
                        {
                            objrpt = new RptVAT6_4_SQR4();

                        }
                        else if (VAT11Name.ToLower() == "pccl")
                        {
                            objrpt = new RptVAT6_4_Padma();
                        }
                        else if (VAT11Name.ToLower() == "scbl")
                        {
                            objrpt = new RptVAT6_4_SCBL();
                        }
                        else
                        {
                            objrpt = new RptVAT6_4();

                        }
                        prefix = "VAT 6.4";
                        objrpt.SetDataSource(ReportResultVAT11);
                        objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + EntryUserName + "'";
                        objrpt.DataDefinition.FormulaFields["MaterialType"].Text = "'" + MaterialType + "'";
                        if ((transactionType == "TollFinishIssue"))
                        {
                            objrpt.DataDefinition.FormulaFields["MaterialType"].Text = "'Finish Product'";
                        }
                        objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 6.4'";
                        objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + connVM.CompanyName + "'";
                        //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + OrdinaryVATDesktop.CompanyLegalName + "'";
                        objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + connVM.Address1 + "'";
                        objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + connVM.Address2 + "'";
                        objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + connVM.Address3 + "'";
                        objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + connVM.TelephoneNo + "'";
                        objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + connVM.FaxNo + "'";
                        objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + connVM.VatRegistrationNo +
                                                                                        "'";

                        string Quantity6_4 = commonDal.settings("DecimalPlace", "Quantity6_4");
                        string Amount6_4 = commonDal.settings("DecimalPlace", "Amount6_4");

                        objrpt.DataDefinition.FormulaFields["Quantity6_4"].Text = "'" + Quantity6_4 + "'";
                        objrpt.DataDefinition.FormulaFields["Amount6_4"].Text = "'" + Amount6_4 + "'";
                    }
                    #endregion rbtnContractorRawIssue

                    #region rbtnVAT11GaGa
                    else if (rbtnVAT11GaGa)
                    {
                        //objrpt = new RptVAT11GaGa();
                        prefix = "VAT11GaGa";
                        objrpt.SetDataSource(ReportResultVAT11);
                        objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + connVM.CompanyName + "'";
                        objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + connVM.Address1 + "'";
                        objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + connVM.VatRegistrationNo + "'";

                    }
                    #endregion rbtnVAT11GaGa

                    #region Other - Sales/6.3

                    else
                    {
                        prefix = "VAT11";

                        if (VATServer.Ordinary.DBConstant.IsProjectVersion2012)
                        {
                            objrpt = new ReportDocument();

                            #region Version Switch

                            if (VAT2012V2 <= invoiceDateTime)
                            {
                                ////New Report -- 
                                objrpt = new RptVAT6_3_V12V2();
                            }
                            else
                            {
                                objrpt = new RptVAT6_3();
                            }

                            #endregion

                            #region Page Size

                            switch (VAT11PageSize)
                            {

                                case "A4":
                                    //objrpt = new RptVAT6_3_A4(); break;

                                    if (VAT2012V2 <= invoiceDateTime)
                                    {
                                        ////New Report -- 
                                        objrpt = new RptVAT6_3_A4_V12V2();
                                        break;
                                    }
                                    else
                                    {
                                        objrpt = new RptVAT6_3_A4();
                                        break;
                                    }
                                case "Letter":
                                    //objrpt = new RptVAT6_3(); break;
                                    if (VAT2012V2 <= invoiceDateTime)
                                    {
                                        ////New Report -- 
                                        objrpt = new RptVAT6_3_V12V2();
                                        break;
                                    }
                                    else
                                    {
                                        objrpt = new RptVAT6_3();
                                        break;
                                    }
                                case "Legal":
                                    //objrpt = new RptVAT6_3(); break;
                                    if (VAT2012V2 <= invoiceDateTime)
                                    {
                                        ////New Report -- 
                                        objrpt = new RptVAT6_3_V12V2();
                                        break;
                                    }
                                    else
                                    {
                                        objrpt = new RptVAT6_3();
                                        break;
                                    }
                                case "A5":
                                    //objrpt = new RptVAT6_3_A5(); break;
                                    if (VAT2012V2 <= invoiceDateTime)
                                    {
                                        ////New Report -- 
                                        objrpt = new RptVAT6_3_A5_V12V2();
                                        break;
                                    }
                                    else
                                    {
                                        objrpt = new RptVAT6_3_A5();
                                        break;
                                    }

                                default:
                                    //objrpt = new RptVAT6_3(); break;
                                    if (VAT2012V2 <= invoiceDateTime)
                                    {
                                        ////New Report -- 
                                        objrpt = new RptVAT6_3_V12V2();
                                        break;
                                    }
                                    else
                                    {
                                        objrpt = new RptVAT6_3();
                                        break;
                                    }
                            }

                            #endregion

                            #region English Format

                            if (VAT11English == true)
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_English();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_English_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_English();

                                }

                            }

                            #endregion

                            #region Company Switch

                            #region Seven Circle - SCBL

                            if (VAT11Name.ToLower() == "scbl")
                            {

                                objrpt = new ReportDocument();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_LetterSCBL_Landscape_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_LetterSCBL_Landscape();

                                }

                                //objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_3_LetterSCBL.rpt");

                                //objrpt.DataDefinition.FormulaFields["UserFullName"].Text = "'" + uvm.FullName + "'";
                                //objrpt.DataDefinition.FormulaFields["UserDesignation"].Text = "'" + uvm.Designation + "'";

                            }

                            #endregion

                            #region Seven Circle - SSPIL

                            if (VAT11Name.ToUpper() == "SSPIL")
                            {

                                objrpt = new ReportDocument();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_LetterSCBL_Landscape_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_LetterSCBL_Landscape();

                                }

                                //objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_3_LetterSCBL.rpt");

                                //objrpt.DataDefinition.FormulaFields["UserFullName"].Text = "'" + uvm.FullName + "'";
                                //objrpt.DataDefinition.FormulaFields["UserDesignation"].Text = "'" + uvm.Designation + "'";

                            }

                            #endregion

                            #region Beximco Communication Ltd. - BCL - BexCom

                            else if (VAT11Name.ToLower() == "bcl")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_BCL();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_BCL_V12V2();
                                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_BCL_V12V2.rpt");

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_BCL();

                                }
                                ////objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report\Rpt63" + @"\RptVAT6_3_BCL.rpt");

                                crFormulaF = objrpt.DataDefinition.FormulaFields;
                                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", "Y");
                            }
                            #endregion

                            #region DHL - Dalsey Hillblom Lynn

                            else if (VAT11Name.ToLower() == "dhl")//DHL
                            {


                                DataTable dt = ReportResultVAT11.Tables[0].Copy();



                                var newSort = (from row in dt.AsEnumerable()
                                               group row by new
                                               {
                                                   SalesInvoiceNo = row.Field<string>("SalesInvoiceNo"),
                                                   InvoiceDate = row.Field<string>("InvoiceDate"),
                                                   CustomerName = row.Field<string>("CustomerName"),
                                                   ProductName = row.Field<string>("ProductName"),
                                                   Address1 = row.Field<string>("Address1"),
                                                   Address2 = row.Field<string>("Address2"),
                                                   Address3 = row.Field<string>("Address3"),
                                                   TelephoneNo = row.Field<string>("TelephoneNo"),
                                                   DeliveryAddress1 = row.Field<string>("DeliveryAddress1"),
                                                   DeliveryAddress2 = row.Field<string>("DeliveryAddress2"),
                                                   DeliveryAddress3 = row.Field<string>("DeliveryAddress3"),
                                                   VehicleType = row.Field<string>("VehicleType"),
                                                   VehicleNo = row.Field<string>("VehicleNo"),
                                                   ProductNameOld = row.Field<string>("ProductNameOld"),
                                                   ProductDescription = row.Field<string>("ProductDescription"),
                                                   ProductGroup = row.Field<string>("ProductGroup"),
                                                   UOM = row.Field<string>("UOM"),
                                                   ProductCommercialName = row.Field<string>("ProductCommercialName"),
                                                   VATRegistrationNo = row.Field<string>("VATRegistrationNo"),
                                                   SerialNo = row.Field<string>("SerialNo"),
                                                   AlReadyPrint = row.Field<int>("AlReadyPrint"),
                                                   ImportIDExcel = row.Field<string>("ImportIDExcel"),
                                                   Comments = row.Field<string>("Comments"),
                                                   VATType = row.Field<string>("VATType"),
                                                   LCNumber = row.Field<string>("LCNumber"),
                                                   LCBank = row.Field<string>("LCBank"),
                                                   PINo = row.Field<string>("PINo"),
                                                   EXPFormNo = row.Field<string>("EXPFormNo"),
                                                   BranchId = row.Field<int>("BranchId"),
                                                   SignatoryName = row.Field<string>("SignatoryName"),
                                                   SignatoryDesig = row.Field<string>("SignatoryDesig"),
                                                   SerialNo1 = row.Field<string>("SerialNo1"),
                                                   SaleType = row.Field<string>("SaleType"),
                                                   PreviousSalesInvoiceNo = row.Field<string>("PreviousSalesInvoiceNo"),
                                                   TransactionType = row.Field<string>("TransactionType"),
                                                   CurrencyID = row.Field<string>("CurrencyID"),
                                                   TPurchaseInvoiceNo = row.Field<string>("TPurchaseInvoiceNo"),
                                                   TBENumber = row.Field<string>("TBENumber"),
                                                   TCustomHouse = row.Field<string>("TCustomHouse"),
                                                   CustomerCode = row.Field<string>("CustomerCode"),
                                                   VATRate = row.Field<decimal>("VATRate"),
                                                   SD = row.Field<decimal>("SD"),
                                                   //Fixed_Subtotal = row.Field<decimal>("Fixed_Subtotal")



                                               } into grp
                                               select new
                                               {
                                                   SalesInvoiceNo = grp.Key.SalesInvoiceNo,
                                                   InvoiceDate = grp.Key.InvoiceDate,
                                                   CustomerName = grp.Key.CustomerName,
                                                   ProductName = grp.Key.ProductName.ToUpper(),
                                                   Address1 = grp.Key.Address1,
                                                   Address2 = grp.Key.Address2,
                                                   Address3 = grp.Key.Address3,
                                                   TelephoneNo = grp.Key.TelephoneNo,
                                                   DeliveryAddress1 = grp.Key.DeliveryAddress1,
                                                   DeliveryAddress2 = grp.Key.DeliveryAddress2,
                                                   DeliveryAddress3 = grp.Key.DeliveryAddress3,
                                                   VehicleType = grp.Key.VehicleType,
                                                   VehicleNo = grp.Key.VehicleNo,
                                                   ProductNameOld = grp.Key.ProductNameOld,
                                                   ProductDescription = grp.Key.ProductDescription,
                                                   ProductGroup = grp.Key.ProductGroup,
                                                   UOM = grp.Key.UOM,
                                                   ProductCommercialName = grp.Key.ProductCommercialName,
                                                   VATRegistrationNo = grp.Key.VATRegistrationNo,
                                                   SerialNo = grp.Key.SerialNo,
                                                   AlReadyPrint = grp.Key.AlReadyPrint,
                                                   ImportIDExcel = grp.Key.ImportIDExcel,
                                                   Comments = grp.Key.Comments,
                                                   VATType = grp.Key.VATType,
                                                   LCNumber = grp.Key.LCNumber,
                                                   LCBank = grp.Key.LCBank,
                                                   PINo = grp.Key.PINo,
                                                   EXPFormNo = grp.Key.EXPFormNo,
                                                   BranchId = grp.Key.BranchId,
                                                   SignatoryName = grp.Key.SignatoryName,
                                                   SignatoryDesig = grp.Key.SignatoryDesig,
                                                   SerialNo1 = grp.Key.SerialNo1,
                                                   SaleType = grp.Key.SaleType,
                                                   PreviousSalesInvoiceNo = grp.Key.PreviousSalesInvoiceNo,
                                                   TransactionType = grp.Key.TransactionType,
                                                   CurrencyID = grp.Key.CurrencyID,
                                                   TPurchaseInvoiceNo = grp.Key.TPurchaseInvoiceNo,
                                                   TBENumber = grp.Key.TBENumber,
                                                   TCustomHouse = grp.Key.TCustomHouse,
                                                   VATRate = grp.Key.VATRate,
                                                   SD = grp.Key.SD,
                                                   grp.Key.CustomerCode,
                                                   Quantity = grp.Sum(r => r.Field<Decimal>("Quantity")),
                                                   UnitCost = grp.Average(r => r.Field<Decimal>("UnitCost")),
                                                   SDAmount = grp.Sum(r => r.Field<Decimal>("SDAmount")),
                                                   VATAmount = grp.Sum(r => r.Field<Decimal>("VATAmount")),
                                                   Fixed_Subtotal = grp.Sum(r => r.Field<Decimal>("Fixed_Subtotal")),
                                                   LineTotal = grp.Sum(r => r.Field<Decimal>("LineTotal")),
                                                   Sort = 0

                                               }).ToList();

                                string tt = JsonConvert.SerializeObject(newSort);

                                List<SaleDHLReport> list = JsonConvert.DeserializeObject<List<SaleDHLReport>>(tt);

                                list = list.Select(x =>
                                {
                                    if (x.ProductName.ToLower() == "Express Worldwide Doc".ToLower())
                                    {
                                        x.Sort = 1;
                                    }
                                    else if (x.ProductName.ToLower() == "Express Worldwide Nondoc".ToLower())
                                    {
                                        x.Sort = 2;
                                    }
                                    else if (x.ProductName.ToLower() == "Express 9:00 Nondoc".ToLower())
                                    {
                                        x.Sort = 3;
                                    }
                                    else if (x.ProductName.ToLower() == "Express 10:30 Nondoc".ToLower())
                                    {
                                        x.Sort = 4;
                                    }
                                    else if (x.ProductName.ToLower() == "Express 12:00 Nondoc".ToLower())
                                    {
                                        x.Sort = 5;
                                    }
                                    return x;
                                }).OrderBy(x => x.Sort)
                                    .ToList();


                                tt = JsonConvert.SerializeObject(list);
                                DataTable result = JsonConvert.DeserializeObject<DataTable>(tt);
                                result.Columns.Remove("Sort");
                                ReportResultVAT11 = new DataSet();


                                ReportResultVAT11.Tables.Add(result);
                                ReportResultVAT11.Tables[0].TableName = "DsVAT11";

                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_DHL2_V12V2();
                                //objrpt.Load(@"D:\VATProject\VATDesktop\SymphonySofttech.Reports\Report\Rpt63" + @"\RptVAT6_3_DHL.rpt");

                                bool exists = ReportResultVAT11.Tables[0].Select().ToList().Exists(row => row["VATRate"].ToString().ToUpper() == "0");

                                crFormulaF = objrpt.DataDefinition.FormulaFields;
                                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);
                                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "exists", exists ? "Y" : "N");

                                string serialNo = result.Rows[0]["SerialNo"].ToString();

                                if (serialNo.Contains("~"))
                                {
                                    string[] splitData = serialNo.Split('~');

                                    if (splitData.Length == 3)
                                    {
                                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "AccountCode", splitData[1]);
                                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "RevenueType", splitData[2]);
                                    }
                                }


                            }

                            else if (VAT11Name.ToLower() == "dhlairport")
                            {
                                bool exists = ReportResultVAT11.Tables[0].Select().ToList().Exists(row => row["VATRate"].ToString().ToUpper() == "0");

                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_DHLAirport_V12V2();
                                //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\\Rpt63V12V2" + @"\RptVAT6_3_DHLAirport_V12V2.rpt");

                                crFormulaF = objrpt.DataDefinition.FormulaFields;
                                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);
                                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "RevenueType", "C");
                                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "exists", exists ? "Y" : "N");

                            }
                            #endregion

                            #region SMC - Social Marketing Ltd.

                            else if (VAT11Name.ToLower() == "smc" || CompanyCode.ToLower() == "smcholding")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_SMC();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_SMC_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_SMC();

                                }
                            }

                            #endregion

                            #region CP - CP Bangladesh

                            else if (VAT11Name.ToLower() == "cp")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_English_CP();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_English_CP_V12V2.rpt");

                                    objrpt = new RptVAT6_3_English_CP_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_English_CP();
                                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63" + @"\RptVAT6_3_English_CP.rpt");

                                }

                            }

                            #endregion

                            #region CPTRI - CP Bangladesh Trisal Unit

                            else if (VAT11Name.ToLower() == "cptri")
                            {
                                objrpt = new ReportDocument();

                                ////New Report -- 
                                //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_English_CPT_V12V2.rpt");

                                objrpt = new RptVAT6_3_English_CPT_V12V2();


                            }

                            #endregion

                            #region PCCL - Padma Group

                            else if (VAT11Name.ToLower() == "pccl")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_Padma();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    //objrpt = new RptVAT6_3_Pidilitee_V12V2();
                                    //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_Pidilitee_V12V2.rpt");

                                    objrpt = new RptVAT6_3_Padma_V12V2();
                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_Padma();
                                }
                            }

                            #endregion

                            #region Link 3

                            else if (VAT11Name.ToLower() == "link3")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3Link3();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3Link3_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3Link3();

                                }
                                //objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report\Rpt63" + @"\RptVAT6_3Link3.rpt");
                            }

                            #endregion

                            #region Beacon Pharma

                            else if (VAT11Name.ToLower() == "bpl")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_BPL();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_BPL_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_BPL();

                                }
                                ////objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report\Rpt63" + @"\RptVAT6_3_BPL.rpt");

                            }

                            #endregion

                            #region REXIM

                            else if (VAT11Name.ToLower() == "rexim")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_RXIM_A4_V12V2();

                            }

                            #endregion

                            #region Shamutsuk painters

                            else if (VAT11Name.ToLower() == "ssp")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_SSP_V12V2();

                            }

                            #endregion

                            #region bpl1

                            else if (VAT11Name.ToLower() == "bpl1")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_BPL1_V12V2();

                                ////objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report\Rpt63" + @"\RptVAT6_3_BPL.rpt");

                            }

                            #endregion

                            #region bpl2

                            else if (VAT11Name.ToLower() == "bpl2")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_BPL2_V12V2();

                                ////objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report\Rpt63" + @"\RptVAT6_3_BPL.rpt");

                            }

                            #endregion

                            #region Radiant Pharma

                            else if (VAT11Name.ToLower() == "rpl")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_RPL();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_RPL_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_RPL();

                                }
                            }

                            #endregion

                            #region Radiant Pharma

                            else if (VAT11Name.ToLower() == "rpl2")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_RPL2();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_RPL2_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_RPL2();

                                }
                            }

                            #endregion

                            #region rnl

                            else if (VAT11Name.ToLower() == "rnl")
                            {

                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_RNL_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_RNL();

                                }


                            }


                            #endregion

                            #region Png

                            else if (VAT11Name.ToLower() == "png")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_PNG();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_PNG_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_PNG();

                                }
                                //objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report\Rpt63" + @"\RptVAT6_3_PNG.rpt");
                            }


                            #endregion

                            #region Quazi Enterprises Limited

                            else if (VAT11Name.ToLower() == "qel")
                            {
                                objrpt = new ReportClass();
                                //objrpt = new RptVAT6_3QEL();
                                ////objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report" + @"\RptVAT6_3QEL.rpt");
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3QEL_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3QEL();

                                }
                            }

                            #endregion

                            #region Golden Harvest Limited

                            else if (VAT11Name.ToLower() == "ghl")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_BATA_V12V2();
                                objrpt = new RptVAT6_3GHL_V12V2();
                            }

                            #endregion

                            #region sc

                            else if (VAT11Name.ToLower() == "sc")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3OtherRate_SC();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3OtherRate_SC_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3OtherRate_SC();

                                }
                            }

                            #endregion

                            #region scl

                            else if (VAT11Name.ToLower() == "scl")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_SCL();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_SCL_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_SCL();

                                }
                                ////objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report\Rpt63" + @"\RptVAT6_3_SCL.rpt");

                            }

                            #endregion

                            #region Symphony Softtech Ltd

                            else if (VAT11Name.ToLower() == "ssl")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_LetterSSL();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_LetterSSL_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_LetterSSL();

                                }

                            }

                            #endregion

                            #region Abdul Monem Ltd

                            else if (VAT11Name.ToLower() == "aml")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_AML();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_AML_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_AML();

                                }

                            }

                            #endregion

                            #region Abdul Monem Ltd Coca-Cola
                            else if (VAT11Name.ToLower() == "amlcc")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_AMLCC_V12V2();
                            }
                            #endregion

                            #region Abdul Monem Ltd _Igloo

                            else if (VAT11Name.ToLower() == "igloo")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_Igloo();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_Igloo_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_Igloo();

                                }

                                //objrpt.DataDefinition.FormulaFields["VehicleRequired"].Text = "'" + VehicleRequired + "'";
                            }

                            #endregion

                            #region Square Group

                            else if (VAT11Name.ToLower() == "sqr1")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_SQR_1();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_SQR_1_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_SQR_1();

                                }
                            }
                            else if (VAT11Name.ToLower() == "sqr2")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_SQR_2();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_SQR_2_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_SQR_2();

                                }
                            }

                            else if (VAT11Name.ToLower() == "mcl")
                            {
                                objrpt = new RptVAT6_3_MCL_V12V2();
                            }

                            else if (VAT11Name.ToLower() == "sqr3")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_SQR3();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_SQR3_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_SQR3();

                                }

                            }
                            else if (VAT11Name.ToLower() == "sqr4")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_SQR4();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_SQR4_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_SQR4();

                                }
                                ////objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report" + @"\\RptVAT6_3_SQR4.rpt");

                            }

                            else if (VAT11Name.ToLower() == "newsqr4")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_SQR4_New_V12V2();

                            }

                            #endregion

                            #region Intertek Bangladesh

                            else if (VAT11Name.ToLower() == "its")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_ITS();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_ITS_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_ITS();

                                }
                            }

                            #endregion

                            #region mcbl

                            else if (VAT11Name.ToLower() == "mcbl")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_MCBL();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_MCBL_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_MCBL();

                                }
                            }

                            #endregion

                            #region Ahsan Group

                            else if (VAT11Name.ToLower() == "ag")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_AG();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_AG_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_AG();

                                }
                            }

                            #endregion

                            #region Bureau Veritas

                            else if (VAT11Name.ToLower() == "bvcps")
                            {
                                UserInformationDAL _udal = new UserInformationDAL();
                                uvm = _udal.SelectAll(Convert.ToInt32(OrdinaryVATDesktop.CurrentUserID), null, null, null, null, connVM).FirstOrDefault();
                                ////New Report -- 
                                objrpt = new ReportDocument();

                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_BVCPS_V12V2();
                                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\\RptVAT6_3_BVCPS_V12V2.rpt");


                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_BVCPS();

                                }

                                objrpt.DataDefinition.FormulaFields["UserFullName"].Text = "'" + uvm.FullName + "'";
                                objrpt.DataDefinition.FormulaFields["UserDesignation"].Text = "'" + uvm.Designation + "'";
                            }

                            #endregion

                            #region Bangladesh Trade Syndicate Limited

                            else if (VAT11Name.ToLower() == "btsl")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_BTSL();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_BTSL_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_BTSL();

                                }
                            }

                            #endregion

                            #region Mondelez International

                            else if (VAT11Name.ToLower() == "mdlz")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_MDLZ();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_MDLZ_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_MDLZ();

                                }
                                //objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report\Rpt63" + @"\RptVAT6_3_MDLZ.rpt");

                                crFormulaF = objrpt.DataDefinition.FormulaFields;
                                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);
                            }

                            #endregion

                            #region Green Delta Insurance Company

                            else if (VAT11Name.ToLower() == "gdic")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_GDIC();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_GDIC_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_GDIC();

                                }
                                crFormulaF = objrpt.DataDefinition.FormulaFields;
                                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "InEnglish", InEnglish);
                            }

                            #endregion

                            #region ACI Godrej Agrovet Private Limited

                            else if (VAT11Name.ToLower() == "acig")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_ACIG();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_ACIG_V12V2();

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_ACIG();

                                }
                            }

                            #endregion

                            #region Pidilite Bangladesh Limited

                            else if (VAT11Name.ToLower() == "pbl")
                            {
                                objrpt = new ReportDocument();
                                #region Customers TCSRate Check For Pidilte
                                CustomerDAL customerDal = new CustomerDAL();

                                var customer = customerDal.SelectAllList("0", new[] { "c.CustomerCode" }, new[] { ReportResultVAT11.Tables[0].Rows[0]["CustomerCode"].ToString() }, null, null, connVM).FirstOrDefault();
                                if (customer != null)
                                {

                                    if (customer.IsTCS == "Y")
                                    {
                                        objrpt = new RptVAT6_3_Pidilite_TCS_V12V2();

                                    }
                                    else
                                    {
                                        objrpt = new RptVAT6_3_Pidilite_V12V2();

                                    }

                                }
                                #endregion


                                ////objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report\Rpt63" + @"\RptVAT6_3_BPL.rpt");
                            }

                            #endregion

                            #region Bata

                            else if (VAT11Name.ToLower() == "bata")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_BATA_V12V2();
                                objrpt = new RptVAT6_3_Bata_New_V12V2();
                            }

                            #endregion

                            #region sym

                            else if (VAT11Name.ToLower() == "sym")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_SYM_V12V2();
                            }

                            #endregion

                            #region ACI

                            else if (VAT11Name.ToLower() == "aci")
                            {
                                if (InEnglish.ToLower() == "y")
                                {
                                    objrpt = new ReportDocument();
                                    objrpt = new RptVAT6_3_ACI_English_V12V2();
                                }
                                else
                                {
                                    objrpt = new ReportDocument();
                                    objrpt = new RptVAT6_3_ACI_V12V2();
                                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_ACI_V12V2.rpt");

                                }

                            }

                            else if (VAT11Name.ToLower() == "acicentral")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_ACI_Central_V12V2();
                            }

                            #endregion

                            #region ACI Pharma
                            else if (VAT11Name.ToLower() == "acipharma")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_ACI_Pharma_V12V2();
                            }

                            #endregion

                            #region ACI Hygin
                            else if (VAT11Name.ToLower() == "acih")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_ACIH_V12V2();
                            }
                            #endregion

                            #region ACI Food Sirajghonj
                            else if (VAT11Name.ToLower() == "acifls")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_ACIFLS_V12V2();
                                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_ACIFLS_V12V2.rpt");


                            }

                            #endregion

                            #region ACI Yamaha
                            else if (VAT11Name.ToLower() == "aciy")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_ACI_Yamaha_V12V2();
                                //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_ACI_Yamaha_V12V2.rpt");
                            }
                            #endregion

                            #region ACI Motors
                            else if (VAT11Name.ToLower() == "acimotors")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_ACI_Motors_V12V2();
                                objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_ACI_Motors_V12V2.rpt");
                            }
                            #endregion

                            #region ACI Bondor
                            else if (VAT11Name.ToLower() == "acibondor")
                            {
                                DateTime ApplicableDate = DateTime.Now;
                                ApplicableDate = Convert.ToDateTime("2022-jan-17");
                                if (ApplicableDate <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new ReportDocument();

                                    objrpt = new RptVAT6_3_ACIBondor_V12V2();
                                    //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_ACIBondor_V12V2.rpt");

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_ACIFLS_V12V2();


                                }

                                //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_ACI_Yamaha_V12V2.rpt");
                            }
                            #endregion

                            #region aci_laser

                            else if (VAT11Name.ToLower() == "aci_laser")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_ACI_Laser_V12V2();
                            }

                            #endregion

                            #region dekko

                            else if (VAT11Name.ToLower() == "dekko")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_DEKKO_V12V2();
                            }

                            #endregion

                            #region jnbl

                            else if (VAT11Name.ToLower() == "jnbl")
                            {
                                objrpt = new ReportDocument();
                                //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_JNNBL_V12V2.rpt");

                                objrpt = new RptVAT6_3_JNNBL_V12V2();
                            }

                            #endregion

                            #region Berger

                            #region bbbl
                            else if (VAT11Name.ToLower() == "bbbl")
                            {
                                objrpt = new ReportDocument();
                                //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_JNNBL_V12V2.rpt");

                                objrpt = new RptVAT6_3_BBBL_V12V2();
                            }
                            #endregion

                            #region Fosroc

                            else if (VAT11Name.ToLower() == "bfl")
                            {
                                objrpt = new ReportDocument();
                                ////objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_BFL_V12V2.rpt");
                                objrpt = new RptVAT6_3_BFL_V12V2();
                            }
                            #endregion

                            #endregion

                            #region nestle
                            else if (VAT11Name.ToLower() == "nestle")
                            {
                                objrpt = new ReportDocument();
                                ////objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_NESTLE_V12V2.rpt");
                                objrpt = new RptVAT6_3_NESTLE_V12V2();
                                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_UNILEVER_V12V2.rpt");
                                ////objrpt = new RptVAT6_3_LetterNESTLE_Landscape_V12V2();
                            }
                            #endregion

                            #region unilever
                            else if (VAT11Name.ToLower() == "unilever")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_UNILEVER_Regular_V12V2();
                                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_UNILEVER_V12V2.rpt");

                            }
                            #endregion

                            #region unileversup
                            //Unilever Sup-D 
                            else if (VAT11Name.ToLower() == "unileversup")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_UNILEVER_V12V2();
                                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_UNILEVER_V12V2.rpt");

                            }
                            #endregion

                            #region jbl
                            else if (VAT11Name.ToLower() == "jbl")
                            {
                                objrpt = new ReportDocument();
                                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_NESTLE_V12V2.rpt");
                                objrpt = new RptVAT6_3_Jotun_V12V2();

                                ////objrpt = new RptVAT6_3_LetterNESTLE_Landscape_V12V2();
                            }
                            #endregion

                            #region cbnk
                            else if (VAT11Name.ToLower() == "cbnk")
                            {
                                objrpt = new ReportDocument();
                                //objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_NESTLE_V12V2.rpt");
                                objrpt = new RptVAT6_3_CavinKare_A4_V12V2();

                            }
                            #endregion

                            #region nnpl
                            //Novo Nordisk Pharma (Pvt.) Ltd
                            else if (VAT11Name.ToLower() == "nnpl")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_ACI_V12V2();
                            }
                            #endregion

                            #region Systems Solutions & Development Technologies (SSD-TECH)
                            else if (VAT11Name.ToLower() == "ssdt")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_English_SSDT_V12V2();
                            }
                            #endregion

                            #region redx
                            else if (VAT11Name.ToLower() == "redx")
                            {
                                objrpt = new ReportClass();
                                objrpt = new RptVAT6_3_RedX_V12V2();

                            }
                            #endregion

                            #region eon
                            //EON Food 
                            else if (VAT11Name.ToLower() == "eon")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_EON_English_V12V2();

                            }
                            #endregion

                            #region ecppl
                            else if (VAT11Name.ToLower() == "ecppl")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_Parkesine_V12V2();

                            }
                            #endregion

                            #region rel
                            else if (VAT11Name.ToLower() == "rel")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_RockEnergy_V12V2();

                            }

                            #endregion

                            #region mbl
                            else if (VAT11Name.ToLower() == "mbl")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_Marico_V12V2();
                            }

                            #endregion

                            #region efl
                            else if (VAT11Name.ToLower() == "efl")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_EFL_V12V2();

                            }
                            #endregion

                            #region ARBAB
                            else if (VAT11Name.ToLower() == "ARBAB".ToLower())
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_ARBAB_V12V2();

                            }
                            #endregion

                            #region Bombay Sweets
                            else if (VAT11Name.ToLower() == "Bombay".ToLower())
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_BombaySweets_V12V2();

                            }
                            #endregion

                            #region BRAC DAIRY

                            else if (VAT11Name.ToLower() == "bracdairy")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_SQR4();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    ////New Report -- 
                                    objrpt = new RptVAT6_3_BRACDAIRY_V12V2();
                                    //////objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\\RptVAT6_3_BRACDAIRY_V12V2.rpt");

                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_SQR4();

                                }
                                ////objrpt.Load(@"D:\VATProjects\VATDesktop\SymphonySofttech.Reports\Report" + @"\\RptVAT6_3_BRACDAIRY_V12V2.rpt");

                            }

                            #endregion

                            #region DBL Ceramics

                            else if (VAT11Name.ToLower() == "dbl")
                            {
                                if (InEnglish.ToLower() == "y")
                                {
                                    objrpt = new ReportDocument();
                                    objrpt = new RptVAT6_3_ACI_English_V12V2();
                                }
                                else
                                {
                                    objrpt = new ReportDocument();
                                    //objrpt = new RptVAT6_3_ACI_V12V2();
                                    objrpt = new RptVAT6_3_DBL_V12V2();


                                    //objrpt.Load(@"D:\VATProjects\BitBucket\VATDesktop\vatdesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_ACI_V12V2.rpt");

                                }

                            }

                            #endregion

                            #region Crown Cement


                            else if (VAT11Name.ToLower() == "ccp")
                            {
                                objrpt = new ReportDocument();
                                //objrpt = new RptVAT6_3_Padma();
                                if (VAT2012V2 <= invoiceDateTime)
                                {
                                    //objrpt = new RptVAT6_3_Pidilitee_V12V2();




                                    objrpt = new RptVAT6_3_CrownCement_V12V2();
                                }
                                else
                                {
                                    objrpt = new RptVAT6_3_Padma();
                                }
                            }



                            #endregion

                            #region mclds

                            else if (VAT11Name.ToLower() == "mclds")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_MCDLS_V12V2();
                            }

                            #endregion

                            #region mcldsup

                            else if (VAT11Name.ToLower() == "mcldsup")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_McdlSup_V12V2();
                            }
                            #endregion

                            #region EC Organic (East Cost)

                            else if (VAT11Name.ToLower() == "eco")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_ECOrganic_V12V2();
                            }
                            #endregion

                            #region CARE NUTRITION LIMITED

                            else if (VAT11Name.ToLower() == "cnl")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_A4_CNL_V12V2();
                            }
                            #endregion

                            else if (VAT11Name.ToLower() == "tis")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_TIS_V12V2();
                            }
                            else if (VAT11Name.ToLower() == "gfoil")
                            {
                                objrpt = new ReportDocument();
                                objrpt = new RptVAT6_3_A4_V12V2_GFOIL();
                            }


                            #endregion

                            #region Formula Fields

                            crFormulaF = objrpt.DataDefinition.FormulaFields;
                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PGroupInReport", vPGroupInReport);
                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TrackingTrace", TrackingTrace.ToString());

                            #endregion
                        }

                        #region Set Data Source

                        objrpt.SetDataSource(ReportResultVAT11);

                        #endregion

                        #region Formula Fields
                        #region Old Formula
                        ////vQuantity,vSDAmount,vQty_UnitCost,vQty_UnitCost_SDAmount,vVATAmount,vSubtotal
                        //objrpt.DataDefinition.FormulaFields["Quantity"].Text = "'" + vQuantity + "'";
                        //objrpt.DataDefinition.FormulaFields["SDAmount"].Text = "'" + vSDAmount + "'";
                        //objrpt.DataDefinition.FormulaFields["Qty_UnitCost"].Text = "'" + vQty_UnitCost + "'";
                        //objrpt.DataDefinition.FormulaFields["Qty_UnitCost_SDAmount"].Text = "'" + vQty_UnitCost_SDAmount + "'";
                        //objrpt.DataDefinition.FormulaFields["VATAmount"].Text = "'" + vVATAmount + "'";
                        //objrpt.DataDefinition.FormulaFields["Subtotal"].Text = "'" + vSubtotal + "'";
                        //objrpt.DataDefinition.FormulaFields["ItemNature"].Text = "'" + ItemNature + "'";

                        if (TrackingTrace == true)
                        {
                            objrpt.Subreports[0].SetDataSource(ReportResultTracking);
                            objrpt.Subreports[0].DataDefinition.FormulaFields["HeadingName1"].Text = "'" + Heading1 + "'";
                            objrpt.Subreports[0].DataDefinition.FormulaFields["HeadingName2"].Text = "'" + Heading2 + "'";
                        }

                        //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + EntryUserName + "'";
                        //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 11'";
                        //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                        //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                        //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                        //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                        //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                        //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                        //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                        //objrpt.DataDefinition.FormulaFields["QtyInWord"].Text = "'" + QtyInWord + "'";
                        #endregion

                        string FontSize = commonDal.settingsDesktop("FontSize", "VAT6_3", settingsDt, connVM);
                        //string Quantity6_3 = commonDal.settingsDesktop("DecimalPlace", "Quantity6_3", settingsDt, connVM);
                        //string Amount6_3 = commonDal.settingsDesktop("DecimalPlace", "Amount6_3", settingsDt, connVM);
                        string VATRate6_3 = commonDal.settingsDesktop("DecimalPlace", "VATRate6_3", settingsDt, connVM);
                        string UnitPrice6_3 = commonDal.settingsDesktop("DecimalPlace", "UnitPrice6_3", settingsDt, connVM);


                        crFormulaF = objrpt.DataDefinition.FormulaFields;
                        _vCommonFormMethod = new CommonFormMethod();


                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", FontSize);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_3", Quantity6_3);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_3", Amount6_3);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchId", BranchId);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchCode", BranchCode);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchLegalName", BranchLegalName);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchBanglaLegalName", BranchBanglaLegalName);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchAddress", BranchAddress);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BanglaAddress", BanglaAddress);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "RegistredAddress", RegistredAddress);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address", Address);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchVatRegistrationNo", BranchVatRegistrationNo);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VATRate6_3", VATRate6_3);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "DP_UnitPrice_6_3", UnitPrice6_3);
                        //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Section", OrdinaryVATDesktop.Section);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsCentral", IsCentral);

                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity", vQuantity);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SDAmount", vSDAmount);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Qty_UnitCost", vQty_UnitCost);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Qty_UnitCost_SDAmount", vQty_UnitCost_SDAmount);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VATAmount", vVATAmount);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Subtotal", vSubtotal);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ItemNature", ItemNature);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UserName", EntryUserName);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "VAT 11");
                        //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", connVM.CompanyName);
                        //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", connVM.Address1);
                        //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", connVM.Address2);
                        //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", connVM.Address3);
                        //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", connVM.TelephoneNo);
                        //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", connVM.FaxNo);
                        //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", connVM.VatRegistrationNo);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "QtyInWord", QtyInWord);

                        if (VAT11Name.ToLower() == "ARBAB".ToLower())
                        {
                            string customerBin = "-";

                            string VATRegistrationNo = ReportResultVAT11.Tables[0].Rows[0]["VATRegistrationNo"].ToString();
                            string NIDNo = ReportResultVAT11.Tables[0].Rows[0]["NIDNo"].ToString();
                            string TelephoneNo = ReportResultVAT11.Tables[0].Rows[0]["TelephoneNo"].ToString();

                            if (!string.IsNullOrWhiteSpace(VATRegistrationNo) && VATRegistrationNo != "N/A" && VATRegistrationNo != "-" && VATRegistrationNo != "NA")
                            {
                                customerBin = VATRegistrationNo;
                            }
                            else if (!string.IsNullOrWhiteSpace(NIDNo) && NIDNo != "N/A" && NIDNo != "-" && NIDNo != "NA")
                            {
                                customerBin = NIDNo;
                            }
                            else if (!string.IsNullOrWhiteSpace(TelephoneNo) && TelephoneNo != "N/A" && TelephoneNo != "-" && TelephoneNo != "NA")
                            {
                                customerBin = TelephoneNo;
                            }

                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CustomerBin", customerBin);

                        }



                        #endregion

                    }

                    #endregion Other

                    #region currency
                    SaleDAL saleDal = new SaleDAL();

                    string currencyMajor = "";
                    string currencyMinor = "";
                    string currencySymbol = "";
                    string[] sqlResults = new string[4];

                    try
                    {

                        sqlResults = saleDal.CurrencyInfo(varSalesInvoiceNo);

                        if (sqlResults[0].ToString() != "Fail")
                        {
                            currencyMajor = sqlResults[1].ToString();
                            currencyMinor = sqlResults[2].ToString();
                            currencySymbol = sqlResults[3].ToString();
                        }
                    }
                    catch (Exception ex)
                    {


                    }

                    #endregion currency

                    #region Formula Fields

                    //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                    //objrpt.DataDefinition.FormulaFields["CurrencyMajor"].Text = "'" + currencyMajor + "'";
                    //objrpt.DataDefinition.FormulaFields["CurrencyMinor"].Text = "'" + currencyMinor + "'";
                    objrpt.DataDefinition.FormulaFields["CurrencySymbol"].Text = "'" + currencySymbol + "'";

                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    string IsBlank = "N";
                    IsBlank = chkIsBlank == true ? "Y" : "N";

                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsBlank", IsBlank);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Trial", connVM.Trial);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CurrencyMajor", currencyMajor);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CurrencyMinor", currencyMinor);
                    //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CurrencySymbol", currencySymbol);

                    #endregion

                    #region PrintCopy

                    string copiesNo = "";
                    int cpno = 0;

                    for (int i = 1; i <= PrintCopy; i++)
                    {
                        copiesNo = (AlReadyPrintNo + i).ToString();
                        cpno = AlReadyPrintNo + i;
                        #region CopyName

                        if ((cpno >= 4 && cpno <= 20) || (cpno >= 24 && cpno <= 30) || (cpno >= 34 && cpno <= 40) ||
                            (cpno >= 44 && cpno <= 50))
                        {
                            copiesNo = cpno + " th copy";
                        }
                        else if (cpno == 1 || cpno == 21 || cpno == 31 || cpno == 41)
                        {
                            copiesNo = cpno + " st copy";
                        }
                        else if (cpno == 2 || cpno == 22 || cpno == 32 || cpno == 42)
                        {
                            copiesNo = cpno + " nd copy";
                        }
                        else if (cpno == 3 || cpno == 23 || cpno == 33 || cpno == 43)
                        {
                            copiesNo = cpno + " rd copy";
                        }
                        else
                        {

                            copiesNo = cpno + " copy";

                        }

                        copiesNo = copiesNo + " (Duplicate)";
                        #endregion CopyName
                    }

                    #endregion

                    #region Preview Text

                    if (PreviewOnly == true)
                    {
                        //Preview Only
                        if (PrintAll)
                        {
                            objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";                            
                        }
                        else
                        {
                            objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                        }

                        if (cpno > 1)
                        {
                            objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "'" + copiesNo + "'";

                        }
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PrintDate", PrintDate);

                        if (PrintAll)
                        {
                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Preview", "");
                        }
                        else
                        {
                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Preview", "Preview Only");
                        }
                        return objrpt;


                    }
                    else
                    {
                        copiesNo = "";
                        cpno = 0;
                        for (int i = 1; i <= PrintCopy; i++)
                        {
                            objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                            copiesNo = (AlReadyPrintNo + i).ToString();
                            cpno = AlReadyPrintNo + i;

                            #region CopyName

                            if ((cpno >= 4 && cpno <= 20) || (cpno >= 24 && cpno <= 30) || (cpno >= 34 && cpno <= 40) ||
                                (cpno >= 44 && cpno <= 50))
                            {
                                copiesNo = cpno + " th copy";
                            }
                            else if (cpno == 1 || cpno == 21 || cpno == 31 || cpno == 41)
                            {
                                copiesNo = cpno + " st copy";
                            }
                            else if (cpno == 2 || cpno == 22 || cpno == 32 || cpno == 42)
                            {
                                copiesNo = cpno + " nd copy";
                            }
                            else if (cpno == 3 || cpno == 23 || cpno == 33 || cpno == 43)
                            {
                                copiesNo = cpno + " rd copy";
                            }
                            else
                            {
                                copiesNo = cpno + " copy";
                            }
                            copiesNo = copiesNo + " (Duplicate)";



                            #endregion CopyName
                            if (cpno > 1)
                            {
                                objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "'" + copiesNo + "'";

                            }
                            crFormulaF = objrpt.DataDefinition.FormulaFields;
                            _vCommonFormMethod = new CommonFormMethod();
                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PrintDate", PrintDate);
                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "copiesNo", copiesNo);
                            return objrpt;

                        }
                    }

                    #endregion
                }

            }
            #endregion

            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                //if (ex.InnerException != null)
                //{
                //    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                //                ex.StackTrace;

                //}

                FileLogger.Log("SaleReport", "6_3",
                    ex.Message + "\n" + ex.StackTrace + "Settings Table Count: " + settingsDt.Rows.Count);
                throw ex;
            }

            #endregion

            return objrpt;


        }

        public ReportDocument MegnaReport_VAT6_3_Completed(string varSalesInvoiceId
    , string transactionType
    , bool rbtnCN
    , bool rbtnDN
    , bool rbtnRCN
    , bool rbtnTrading
    , bool rbtnTollIssue
    , bool rbtnVAT11GaGa
    , bool PreviewOnly
    , int PrintCopy
    , int AlReadyPrintNo
    , bool chkIsBlank
    , bool chkIs11
    , bool chkValueOnly
    , bool CommercialImporter = false
    , bool mulitplePreview = false
    , bool rbtnContractorRawIssue = false
    , SysDBInfoVMTemp connVM = null, string MultipleSalesInvoiceRows = "", bool IsGatepass = false, bool rbtnPurchaseReurn = false)
        {
            #region Golbal Variable
            ReportDocument objrpt = new ReportDocument();
            CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            //ReportDSDAL showReport = new ReportDSDAL();
            CommonDAL commonDal = new CommonDAL();
            DataSet ReportResultVAT11 = new DataSet();
            DataSet ReportResultCreditNote = new DataSet();
            DataSet ReportResultDebitNote = new DataSet();
            DataSet ReportResultTracking = new DataSet();

            FormulaFieldDefinitions crFormulaF;
            UserInformationVM uvm = new UserInformationVM();
            //UserInformationDAL _udal = new UserInformationDAL();
            AlReadyPrintNo = 0;
            int NumberOfItems = 0;
            string BranchId = "1";
            string BranchCode = "";
            string BranchName = "";
            string IsCentral = "";
            string BranchLegalName = "";
            string BranchBanglaLegalName = "";
            string BranchAddress = "";
            string BanglaAddress = "";
            string RegistredAddress = "";
            string BranchVatRegistrationNo = "";
            string VAT11Name = "";
            string CompanyCode = "";
            string vPGroupInReport = "";
            string ItemNature = "";
            string VAT11PageSize = "";
            string Heading1 = "";
            string Heading2 = "";
            string post1 = "";
            string post2 = "";
            string EntryUserName = "";
            string Address = "";
            string MaterialType = "";
            bool PrepaidVAT = false;
            bool VAT11Letter = false;
            bool VAT11A4 = false;
            bool VAT11Legal = false;
            bool VAT11English = false;
            bool TrackingTrace = false;

            #endregion

            #region Settings

            DataTable settingsDt = new DataTable();
            DateTime invoiceDateTime = DateTime.Now;
            DateTime VAT2012V2 = DateTime.Now;
            DateTime VAT2022V2 = DateTime.Now;
            string vVAT2012V2 = "2020-Jul-01";
            string vVAT2022V2 = "2022-Jul-01";

            try
            {

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                vVAT2012V2 = commonDal.settings("Version", "VAT2012V2", null, null, connVM);

                VAT2012V2 = Convert.ToDateTime(vVAT2012V2);


                vVAT2022V2 = commonDal.settings("Version", "VAT2022V2", null, null, connVM);

                VAT2022V2 = Convert.ToDateTime("01-Jul-2022");

            }


            catch (Exception ex)
            {
                throw new ArgumentException(vVAT2012V2 + ex.ToString());
            }
            CompanyCode = commonDal.settingsDesktop("CompanyCode", "Code", settingsDt, connVM);


            #endregion

            DBSQLConnection _dbsqlConnection = new DBSQLConnection();

            if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName, CompanyCode)) <=
                Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
            {
                return null;
            }

            #region CompanyInformation

            CompanyProfileVM CompanyVm = new CompanyprofileDAL().SelectAllList(null, null, null, null, null, connVM).FirstOrDefault();

            #endregion

            #region Data Calling

            try
            {
                if (PreviewOnly == true)
                {
                    post1 = "y";
                    post2 = "N";
                }
                else
                {
                    post1 = "Y";
                    post2 = "Y";
                }

                #region Settings Assign

                VAT11Name = commonDal.settings("Reports", "VAT6_3", null, null, connVM);
                var vPrepaidVAT = commonDal.settingsDesktop("PrepaidVAT", "PrepaidVAT", settingsDt, connVM);
                PrepaidVAT = Convert.ToBoolean(commonDal.settingsDesktop("PrepaidVAT", "PrepaidVAT", settingsDt, connVM) == "Y" ? true : false);
                VAT11Letter = Convert.ToBoolean(commonDal.settingsDesktop("Sale", "VAT6_3Letter", settingsDt, connVM) == "Y" ? true : false);
                VAT11A4 = Convert.ToBoolean(commonDal.settingsDesktop("Sale", "VAT6_3A4", settingsDt, connVM) == "Y" ? true : false);
                VAT11Legal = Convert.ToBoolean(commonDal.settingsDesktop("Sale", "VAT6_3Legal", settingsDt, connVM) == "Y" ? true : false);
                VAT11English = Convert.ToBoolean(commonDal.settingsDesktop("Sale", "VAT6_3English", settingsDt, connVM) == "Y" ? true : false);
                VAT11English = Convert.ToBoolean(commonDal.settingsDesktop("Sale", "VAT6_3English", settingsDt, connVM) == "Y" ? true : false);


                #endregion

                #region Data Switching


                if (!rbtnPurchaseReurn)
                {
                    string[] PrintResult1 = new SaleDAL().MegnaUpdatePrintNew(varSalesInvoiceId, PreviewOnly == true ? 0 : PrintCopy);
                }


                ReportResultVAT11 = showReport.MegnaVAT6_3(varSalesInvoiceId, post1, post2, "n", connVM, mulitplePreview);
                MaterialType = ReportResultVAT11.Tables[0].Rows[0]["ProductGroup"].ToString();

                BranchId = ReportResultVAT11.Tables[0].Rows[0]["BranchID"].ToString();

                invoiceDateTime = Convert.ToDateTime(ReportResultVAT11.Tables[0].Rows[0]["InvoiceDate"]);



                #endregion

                #region Branch Details

                string[] cValues = { BranchId };
                string[] cFields = { "BranchID" };

                DataTable branch = OrdinaryVATDesktop.GetObject<BranchProfileDAL, BranchProfileRepo, IBranchProfile>(connVM.IsWCF)
                    .SelectAll(null, cFields, cValues, null, null, true, connVM);

                BranchCode = branch.Rows[0]["BranchCode"].ToString();
                BranchName = branch.Rows[0]["BranchName"].ToString();
                BranchLegalName = branch.Rows[0]["BranchLegalName"].ToString();
                BranchAddress = branch.Rows[0]["Address"].ToString();
                BranchVatRegistrationNo = branch.Rows[0]["VatRegistrationNo"].ToString();
                IsCentral = branch.Rows[0]["IsCentral"].ToString();
                BranchBanglaLegalName = branch.Rows[0]["BranchBanglaLegalName"].ToString();
                BanglaAddress = branch.Rows[0]["BanglaAddress"].ToString();

                #endregion

            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                //if (ex.InnerException != null)
                //{
                //    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                //                ex.StackTrace;

                //}
                //MessageBox.Show(ex.Message, "Report_VAT6_3_Completed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("Report_VAT6_3_Completed", "", exMessage);
            }
            #endregion

            #endregion

            #region More Settings

            string VehicleRequired = commonDal.settingsDesktop("Sale", "VehicleRequired", settingsDt, connVM);
            string InEnglish = commonDal.settingsDesktop("Reports", "VAT6_3English", settingsDt, connVM);
            string VAT6_7English = commonDal.settingsDesktop("Reports", "VAT6_7English", settingsDt, connVM);
            string VAT6_8English = commonDal.settingsDesktop("Reports", "VAT6_8English", settingsDt, connVM);
            string companyCode = commonDal.settingsDesktop("CompanyCode", "Code", settingsDt, connVM);
            string FontSize6_7 = commonDal.settingsDesktop("FontSize", "VAT6_7", settingsDt, connVM);
            string Quantity6_3 = commonDal.settingsDesktop("DecimalPlace", "Quantity6_3", settingsDt, connVM);
            string Amount6_3 = commonDal.settingsDesktop("DecimalPlace", "Amount6_3", settingsDt, connVM);
            #endregion

            #region Report Generation

            try
            {




                AlReadyPrintNo = 1;


                AlReadyPrintNo = AlReadyPrintNo - PrintCopy;


                #region VAT 11 Page Setup and Item Nature
                string vVAT11Letter = commonDal.settingsDesktop("Sale", "VAT6_3Letter", settingsDt, connVM);
                string vVAT11A4 = commonDal.settingsDesktop("Sale", "VAT6_3A4", settingsDt, connVM);
                string vVAT11Legal = commonDal.settingsDesktop("Sale", "VAT6_3Legal", settingsDt, connVM);
                ItemNature = commonDal.settingsDesktop("Sale", "ItemNature", settingsDt, connVM);
                vPGroupInReport = commonDal.settingsDesktop("Sale", "PGroupInReport", settingsDt, connVM);



                if (VAT11A4 == true)
                {
                    VAT11PageSize = "A4";
                }
                else if (VAT11Letter == true)
                {
                    VAT11PageSize = "Letter";
                }
                else if (VAT11Legal == true)
                {
                    VAT11PageSize = "Legal";
                }
                else
                {
                    VAT11PageSize = "A4";
                }
                #endregion VAT 11 Page Setup and Item Nature


                string PrintDate = _dbsqlConnection.ServerPrintDateTime();

                #region Variables

                string vQuantity, vSDAmount, vQty_UnitCost, vQty_UnitCost_SDAmount, vVATAmount, vSubtotal, QtyInWord;// = string.Empty;  

                vQuantity = string.Empty;
                vSDAmount = string.Empty;
                vQty_UnitCost = string.Empty;
                vQty_UnitCost_SDAmount = string.Empty;
                vVATAmount = string.Empty;
                vSubtotal = string.Empty;
                QtyInWord = string.Empty;
                decimal Quantity = 0,
                           SDAmount = 0,
                           Qty_UnitCost = 0,
                           Qty_UnitCost_SDAmount = 0,
                           VATAmount = 0,
                           TotalVDSAmount = 0,
                           Subtotal = 0;

                #endregion

                #region Datatable - Table Name
                ReportResultVAT11.Tables[0].TableName = "MegnaDsVAT11";
                ReportResultVAT11.Tables[1].TableName = "DsMegnaPayment";

                //start Complete

                #endregion

                string prefix = "";


                #region Other - Sales/6.3


                prefix = "VAT11";


                objrpt = new ReportDocument();
                objrpt.Load(@"D:\VATProject\BitbucketCloud\VATDesktop\SymphonySofttech.Reports\Report\Rpt63V12V2" + @"\RptVAT6_3_Megna_V12V2.rpt");

                //objrpt = new RptVAT6_3_Megna_V12V2();

                #region Formula Fields

                crFormulaF = objrpt.DataDefinition.FormulaFields;
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PGroupInReport", vPGroupInReport);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TrackingTrace", TrackingTrace.ToString());

                #endregion

                #region Set Data Source

                objrpt.SetDataSource(ReportResultVAT11);

                #endregion

                #region Formula Fields

                string FontSize = commonDal.settingsDesktop("FontSize", "VAT6_3", settingsDt, connVM);

                string VATRate6_3 = commonDal.settingsDesktop("DecimalPlace", "VATRate6_3", settingsDt, connVM);
                string UnitPrice6_3 = commonDal.settingsDesktop("DecimalPlace", "UnitPrice6_3", settingsDt, connVM);


                crFormulaF = objrpt.DataDefinition.FormulaFields;
                _vCommonFormMethod = new CommonFormMethod();


                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_3", Quantity6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_3", Amount6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchId", BranchId);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchCode", BranchCode);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchLegalName", BranchLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchBanglaLegalName", BranchBanglaLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchAddress", BranchAddress);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BanglaAddress", BanglaAddress);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "RegistredAddress", RegistredAddress);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address", Address);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchVatRegistrationNo", BranchVatRegistrationNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VATRate6_3", VATRate6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "DP_UnitPrice_6_3", UnitPrice6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsCentral", IsCentral);

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity", vQuantity);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SDAmount", vSDAmount);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Qty_UnitCost", vQty_UnitCost);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Qty_UnitCost_SDAmount", vQty_UnitCost_SDAmount);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VATAmount", vVATAmount);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Subtotal", vSubtotal);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ItemNature", ItemNature);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UserName", EntryUserName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "VAT 11");

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", CompanyVm.CompanyLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", CompanyVm.Address1);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", CompanyVm.Address2);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", CompanyVm.Address3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", CompanyVm.TelephoneNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", CompanyVm.FaxNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VatRegistrationNo", CompanyVm.VatRegistrationNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "QtyInWord", QtyInWord);



                #endregion


                #endregion Other

                #region currency
                SaleDAL saleDal = new SaleDAL();

                string currencyMajor = "";
                string currencyMinor = "";
                string currencySymbol = "";
                string[] sqlResults = new string[4];

                try
                {

                    sqlResults = saleDal.MegnaCurrencyInfo(varSalesInvoiceId);

                    if (sqlResults[0].ToString() != "Fail")
                    {
                        currencyMajor = sqlResults[1].ToString();
                        currencyMinor = sqlResults[2].ToString();
                        currencySymbol = sqlResults[3].ToString();
                    }
                }
                catch (Exception ex)
                {


                }

                #endregion currency

                #region Formula Fields


                objrpt.DataDefinition.FormulaFields["CurrencySymbol"].Text = "'" + currencySymbol + "'";

                crFormulaF = objrpt.DataDefinition.FormulaFields;
                string IsBlank = "N";
                IsBlank = chkIsBlank == true ? "Y" : "N";

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "IsBlank", IsBlank);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Trial", connVM.Trial);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CurrencyMajor", currencyMajor);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CurrencyMinor", currencyMinor);
                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CurrencySymbol", currencySymbol);

                #endregion

                #region PrintCopy

                string copiesNo = "";
                int cpno = 0;

                for (int i = 1; i <= PrintCopy; i++)
                {
                    copiesNo = (AlReadyPrintNo + i).ToString();
                    cpno = AlReadyPrintNo + i;
                    #region CopyName

                    if ((cpno >= 4 && cpno <= 20) || (cpno >= 24 && cpno <= 30) || (cpno >= 34 && cpno <= 40) ||
                        (cpno >= 44 && cpno <= 50))
                    {
                        copiesNo = cpno + " th copy";
                    }
                    else if (cpno == 1 || cpno == 21 || cpno == 31 || cpno == 41)
                    {
                        copiesNo = cpno + " st copy";
                    }
                    else if (cpno == 2 || cpno == 22 || cpno == 32 || cpno == 42)
                    {
                        copiesNo = cpno + " nd copy";
                    }
                    else if (cpno == 3 || cpno == 23 || cpno == 33 || cpno == 43)
                    {
                        copiesNo = cpno + " rd copy";
                    }
                    else
                    {

                        copiesNo = cpno + " copy";

                    }

                    copiesNo = copiesNo + " (Duplicate)";
                    #endregion CopyName
                }

                #endregion

                #region Preview Text

                if (PreviewOnly == true)
                {
                    //Preview Only

                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";

                    if (cpno > 1)
                    {
                        objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "'" + copiesNo + "'";

                    }
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PrintDate", PrintDate);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Preview", "Preview Only");

                    return objrpt;


                }
                else
                {
                    copiesNo = "";
                    cpno = 0;
                    for (int i = 1; i <= PrintCopy; i++)
                    {
                        objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                        copiesNo = (AlReadyPrintNo + i).ToString();
                        cpno = AlReadyPrintNo + i;

                        #region CopyName

                        if ((cpno >= 4 && cpno <= 20) || (cpno >= 24 && cpno <= 30) || (cpno >= 34 && cpno <= 40) ||
                            (cpno >= 44 && cpno <= 50))
                        {
                            copiesNo = cpno + " th copy";
                        }
                        else if (cpno == 1 || cpno == 21 || cpno == 31 || cpno == 41)
                        {
                            copiesNo = cpno + " st copy";
                        }
                        else if (cpno == 2 || cpno == 22 || cpno == 32 || cpno == 42)
                        {
                            copiesNo = cpno + " nd copy";
                        }
                        else if (cpno == 3 || cpno == 23 || cpno == 33 || cpno == 43)
                        {
                            copiesNo = cpno + " rd copy";
                        }
                        else
                        {
                            copiesNo = cpno + " copy";
                        }
                        copiesNo = copiesNo + " (Duplicate)";



                        #endregion CopyName
                        if (cpno > 1)
                        {
                            objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "'" + copiesNo + "'";

                        }
                        crFormulaF = objrpt.DataDefinition.FormulaFields;
                        _vCommonFormMethod = new CommonFormMethod();
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PrintDate", PrintDate);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "copiesNo", copiesNo);
                        return objrpt;

                    }
                }

                #endregion

            }
            #endregion

            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                //if (ex.InnerException != null)
                //{
                //    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                //                ex.StackTrace;

                //}

                FileLogger.Log("SaleReport", "6_3",
                    ex.Message + "\n" + ex.StackTrace + "Settings Table Count: " + settingsDt.Rows.Count);
                throw ex;
            }

            #endregion

            return objrpt;


        }

        public string ExportPDf(ReportDocument objrpt, string fileDirectory, string fileName)
        {
            Stream st = new MemoryStream();

            st = objrpt.ExportToStream(ExportFormatType.PortableDocFormat);
            string path = fileDirectory + "\\" + fileName + ".pdf";

            using (var output = new FileStream(path, FileMode.Create))
            {
                st.CopyTo(output);
            }

            st.Dispose();

            return path;

        }

        public string SalesPDFExport(string ImportId, string PathRoot, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            BritishCouncil_IntegrationDAL _dal = new BritishCouncil_IntegrationDAL();

            #endregion

            #region try

            try
            {
                ReportDocument objrpt = new ReportDocument();

                string fileDirectory = PathRoot + "\\6.3 Files";

                fileDirectory += "\\TempPDF";
                Directory.CreateDirectory(fileDirectory);

                DataTable dtSales = _dal.SelectSalesDataForPDFExport(ImportId, null, null, connVM);

                foreach (DataRow dtSalesRow in dtSales.Rows)
                {
                    string invocieNo = dtSalesRow["InvoiceNo"].ToString();
                    bool IsPDFGenerated = dtSalesRow["IsPDFGenerated"].ToString() == "Y";
                    bool IsSendMail = dtSalesRow["IsSendMail"].ToString() == "Y";
                    string Email = dtSalesRow["Email"].ToString();
                    string TransactionType = dtSalesRow["TransactionType"].ToString();

                    string fileName = invocieNo.Replace("\\", "-").Replace("/", "-") + "~" + DateTime.Now.ToString("yyyyMMdd") + "~" + DateTime.Now.ToString("hhmmss");

                    #region PDF Generation

                    if (!IsPDFGenerated)
                    {
                        objrpt = Report_VAT6_3_Completed(invocieNo, TransactionType, false, false, false, false, false, false, false, 1
                            , 0, false, false, false, false, false, false, connVM, "", false, false);

                        ExportPDf(objrpt, fileDirectory, fileName);

                    }

                    #endregion

                    #region Send Mail

                    if (!IsSendMail)
                    {
                        if (!string.IsNullOrWhiteSpace(Email) && Email != "-" && Email.ToLower() != "na" && Email.ToLower() != "n/a")
                        {
                            EmailProcess(invocieNo, fileDirectory, fileName, Email);
                        }
                    }
                    #endregion

                }

                MoveToFolder(fileDirectory, "D:\\invoice");

            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                #region Error Log

                try
                {
                    ErrorLogVM evm = new ErrorLogVM();

                    evm.ImportId = ImportId;
                    evm.FileName = "Exception";
                    evm.ErrorMassage = ex.Message;
                    evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    evm.SourceName = "SalesPDFExport";
                    evm.ActionName = "SaleReport";
                    evm.TransactionName = "Sales";

                    CommonDAL _cDal = new CommonDAL();

                    string[] Logresult = _cDal.InsertErrorLogs(evm);

                }
                catch (Exception exc)
                {

                }
                #endregion

            }
            #endregion

            #region finally
            finally
            {

            }
            #endregion

            return "Success";

        }

        public void EmailProcess(string InvoiceNo, string fileDirectory, string fileName, string CustomerEmail)
        {
            try
            {
                SaleDAL _sDal = new SaleDAL();
                MailSettings ems = new MailSettings();
                CommonDAL _setDAL = new CommonDAL();
                ems.MailHeader = _setDAL.MailsettingValue("Mail", "MailSubject");
                ////ems.MailHeader = ems.MailHeader.Replace("vmonth", FiscalPeriod);
                string mailbody = _setDAL.MailsettingValue("Mail", "MailBody");

                try
                {
                    //////string CustomerEmail = _sDal.SelectCustomerEmail(InvoiceNo);

                    string filePath = fileDirectory + "\\" + fileName + ".pdf";

                    ems.MailToAddress = CustomerEmail;

                    ems.MailToAddress = "alamgirhossain019899@gmail.com";
                    //////ems.Port = 25;
                    ems.MailFromAddress = "alamgir.hossain@symphonysoftt.com";//bcsbl.invoices@britishcouncil.org
                    ems.UserName = "alamgir.hossain@symphonysoftt.com";//bcsbl.invoices@britishcouncil.org
                    ems.ServerName = "smtp.gmail.com";//smtp.gmail.com //SMTP.britishcouncil.org
                    ems.Port = 587;//smtp.gmail.com

                    if (!string.IsNullOrWhiteSpace(ems.MailToAddress))
                    {
                        ems.MailBody = mailbody;

                        ////ems.MailBody = mailbody.Replace("vmonth", FiscalPeriod);
                        ////ems.MailBody = mailbody.Replace("vname", item["EmpName"].ToString());

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

                            string result = _sDal.UpdateIsSendMail(InvoiceNo);

                        }

                        Thread.Sleep(500);
                    }
                }
                catch (SmtpFailedRecipientException ex)
                {
                    #region Error Log

                    try
                    {
                        ErrorLogVM evm = new ErrorLogVM();

                        evm.ImportId = InvoiceNo;
                        evm.FileName = "SmtpFailedRecipientException";
                        evm.ErrorMassage = ex.Message;
                        evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        evm.SourceName = "EmailProcess";
                        evm.ActionName = "SaleReport";
                        evm.TransactionName = "Sales";

                        CommonDAL _cDal = new CommonDAL();

                        string[] Logresult = _cDal.InsertErrorLogs(evm);

                    }
                    catch (Exception exc)
                    {

                    }
                    #endregion
                }

                //rptDoc.Close();
                //thread.Abort();

            }
            catch (Exception ex)
            {
                #region Error Log

                try
                {
                    ErrorLogVM evm = new ErrorLogVM();

                    evm.ImportId = "0";
                    evm.FileName = "MailSend";
                    evm.ErrorMassage = ex.Message;
                    evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    evm.SourceName = "EmailProcess";
                    evm.ActionName = "FormSaleImportBC";
                    evm.TransactionName = "Sales";

                    CommonDAL _cDal = new CommonDAL();

                    string[] Logresult = _cDal.InsertErrorLogs(evm);

                }
                catch (Exception exc)
                {

                }
                #endregion

            }

        }

        private void MoveToFolder(string source, string dest)
        {

            List<String> files = Directory
                .GetFiles(source).ToList();

            foreach (string file in files)
            {
                if (IsFileLocked(file))
                    continue;

                string finalDest = dest + "\\" + Path.GetFileName(file);
                MoveFile(file, finalDest);
            }
        }

        const int ERROR_SHARING_VIOLATION = 32;
        const int ERROR_LOCK_VIOLATION = 33;
        private bool IsFileLocked(string file)
        {
            //check that problem is not in destination file
            if (File.Exists(file) == true)
            {
                FileStream stream = null;
                try
                {
                    stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (Exception ex2)
                {
                    //_log.WriteLog(ex2, "Error in checking whether file is locked " + file);
                    int errorCode = Marshal.GetHRForException(ex2) & ((1 << 16) - 1);
                    if ((ex2 is IOException) && (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION))
                    {
                        return true;
                    }
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            return false;
        }

        private async void MoveFile(string sourceFile, string destinationFile)
        {
            try
            {
                using (FileStream sourceStream = File.Open(sourceFile, FileMode.Open))
                {
                    using (FileStream destinationStream = File.Create(destinationFile))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                        sourceStream.Close();
                        File.Delete(sourceFile);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }



    }

}
