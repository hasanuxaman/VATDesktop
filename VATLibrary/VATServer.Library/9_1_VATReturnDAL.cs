using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using VATViewModel.DTOs;
using System.Reflection;
using VATServer.Ordinary;
using System.Collections.Generic;
using System.Xml;
using VATServer.Library.NBRWebService;
using System.ComponentModel;
using System.Xml.Schema;
using System.IO;
using System.Xml.Linq;


namespace VATServer.Library
{
    public class _9_1_VATReturnDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private NbrApi _nbrApi = new NbrApi();

        #endregion

        public DataSet VAT9_1_CompleteSave(VATReturnVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataSet dsVAT9_1 = new DataSet();
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                PrepareData(vm);

                #region VATReturnHeaders

                InsertToVATReturnHeaders(vm.varVATReturnHeaderVM, connVM, null, null);

                #endregion

                DateTime datePeriodStart = Convert.ToDateTime(vm.PeriodName);
                DateTime HardDecember2019 = Convert.ToDateTime("December-2019");
                if (datePeriodStart < HardDecember2019)
                {
                    #region Comments - Nov-01-2020

                    dsVAT9_1 = VAT9_1(vm);
                    //                      0-63920-03313-29060-07749
                    //                Rashed
                    //                    User: symphony
                    //Pass: symphony2020
                    //HOST: 103.123.8.169
                    //Port : 1521
                    //SID: xe

                    #endregion
                }
                else
                {

                    #region Comments - Nov-01-2020

                    //DataTable BranchDt = new DataTable();
                    //DataTable Dt = new DataTable();
                    //string[] result = new string[4];

                    //BranchDt= new BranchProfileDAL().SelectAll(null,null,null);
                    //int BranchId = vm.BranchId;
                    //foreach (DataRow dr in BranchDt.Rows)
                    //{
                    //    vm.BranchId = Convert.ToInt32(dr["BranchID"]);

                    //    result = DeleteVATReturnV2Details(vm);
                    //    dsVAT9_1 = VAT9_1_V2Save(vm);
                    //    Dt = Select_VATReturnV2s(vm);

                    //    result = new CommonDAL().BulkInsert("VATReturnV2Details", Dt);
                    //}

                    //////// branch dt
                    ////////int BranchId = vm.BranchId;
                    ////////for (int i = 0; i < 5; i++)
                    ////////{
                    ////////    // Delete 
                    ////////    //delete VATReturnV2Details where  PeriodID = @PeriodId and BranchId = @SelectBranchId  
                    ////////    vm.BranchId = 1;
                    ////////    dsVAT9_1 = VAT9_1_V2Save(vm); //postStatus
                    ////////    // Select VATReturnV2s  where  PeriodID = @PeriodId and BranchId = @SelectBranchId 
                    ////////    // Bulk Insert VATReturnV2Details
                    ////////}
                    ////////vm.BranchId = BranchId;

                    //vm.BranchId = BranchId;

                    #endregion

                    dsVAT9_1 = VAT9_1_V2Save(vm);
                }

                #region Period Id save for 9.1

                VATReturnSubFormVM sVM = new VATReturnSubFormVM();
                sVM.PeriodName = vm.PeriodName;
                sVM.IsSummary = false;

                string[] retResult = checkVAT9_1SubFormMethod(sVM, connVM, currConn, transaction);

                #endregion

                #region Commit

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();

                }

                #endregion Commit

                #region Save sub form

                for (int i = 1; i <= 64; i++)
                {
                    sVM.NoteNo = i;

                    if (sVM.NoteNo == 31)
                    {

                    }

                    if (sVM.NoteNo != 23)
                    {
                        DataTable dt = VAT9_1_SubForm(sVM, connVM, true);
                    }

                }

                #endregion

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_CompleteSave", ex.ToString());

            }
            #endregion

            #region finally

            finally
            {

            }
            #endregion

            return dsVAT9_1;

        }

        private void PrepareData(VATReturnVM vm, SysDBInfoVMTemp connVM = null)
        {

            #region try

            try
            {

                vm.PeriodName = Convert.ToDateTime(vm.PeriodName).ToString("MMMM-yyyy");

                if (string.IsNullOrWhiteSpace(vm.PeriodStart))
                {
                    vm.PeriodStart = OrdinaryVATDesktop.DateToDate(vm.PeriodName);
                }
                if (string.IsNullOrWhiteSpace(vm.Date))
                {
                    vm.Date = Convert.ToDateTime(vm.PeriodName).ToString("dd-MMM-yyyy"); ;
                }

                if (string.IsNullOrWhiteSpace(Convert.ToString(vm.varVATReturnHeaderVM.PeriodName)))
                {
                    vm.varVATReturnHeaderVM.PeriodName = vm.PeriodName;
                }
                if (string.IsNullOrWhiteSpace(Convert.ToString(vm.varVATReturnHeaderVM.PeriodStart)))
                {
                    vm.varVATReturnHeaderVM.PeriodStart = vm.PeriodStart;
                }
                if (vm.varVATReturnHeaderVM.BranchId == 0)
                {
                    vm.varVATReturnHeaderVM.BranchId = vm.BranchId;
                }
                if (string.IsNullOrWhiteSpace(Convert.ToString(vm.varVATReturnHeaderVM.BranchName)))
                {
                    vm.varVATReturnHeaderVM.BranchName = vm.BranchName;
                }
                if (string.IsNullOrWhiteSpace(Convert.ToString(vm.varVATReturnHeaderVM.BranchName)))
                {
                    vm.varVATReturnHeaderVM.PostStatus = vm.Post;
                }
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "PrepareData", ex.ToString());

                throw ex;
            }
            #endregion

        }

        public DataSet VAT9_1_CompleteLoad(VATReturnVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataSet dsVAT9_1 = new DataSet();
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                vm.PeriodName = Convert.ToDateTime(vm.PeriodName).ToString("MMMM-yyyy");

                if (string.IsNullOrWhiteSpace(vm.PeriodStart))
                {
                    vm.PeriodStart = OrdinaryVATDesktop.DateToDate(vm.PeriodName);
                }
                if (string.IsNullOrWhiteSpace(vm.Date))
                {
                    vm.Date = Convert.ToDateTime(vm.PeriodName).ToString("dd-MMM-yyyy"); ;
                }

                DateTime datePeriodStart = Convert.ToDateTime(vm.PeriodName);
                DateTime HardDecember2019 = Convert.ToDateTime("December-2019");
                if (datePeriodStart < HardDecember2019)
                {
                    dsVAT9_1 = VAT9_1(vm);
                }
                else
                {
                    dsVAT9_1 = VAT9_1_V2Load(vm);
                }

                #region Commit

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();

                }

                #endregion Commit

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_CompleteLoad", ex.ToString());

            }
            #endregion

            #region finally

            finally
            {

            }
            #endregion

            return dsVAT9_1;

        }

        public DataSet VAT9_1(VATReturnVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            string PreviousPeriodID = "";
            DataSet dataSet = new DataSet("VAT19Report");

            #endregion

            #region Try

            try
            {
                if (!string.IsNullOrWhiteSpace((vm.Date)))
                {
                    PreviousPeriodID = Convert.ToDateTime(vm.Date).AddMonths(-1).ToString("MMyyyy");
                }
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement
                #region MyRegion
                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);
declare @ATVRebate as varchar(100);
declare @AutoPartialRebateProcess as varchar(1);

----declare @periodName VARCHAR (200);
----declare @ExportInBDT VARCHAR (200);
declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

----SET @periodName='December-2018';
----SET @ExportInBDT='Y'


set @UserName ='admin'
set @Branch ='HO_DB'

----declare @BranchId as int
----set @BranchId = 0
----declare @SelectBranchId as int = 0

declare @PeriodId as varchar(100);
----declare @PreviousPeriodID as int = 072019;

declare @LastLine62 as decimal(18, 5);
declare @LastLine63 as decimal(18, 5);


----------------------------------Initialization------------------------
------------------------------------------------------------------------
select  @PeriodId=PeriodId, @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;



select @ATVRebate=settingValue  FROM Settings where SettingGroup='ImportPurchase' and SettingName='ATVRebate';
select @AutoPartialRebateProcess=settingValue  FROM Settings where SettingGroup='Sale' and SettingName='AutoPartialRebateProcess';

-----------------------------Initialization/Rebate Cancel----------------------------
-------------------------------------------------------------------------------------
DECLARE @Line9Subtotal AS DECIMAL = 0
DECLARE @Line23VAT AS DECIMAL = 0

SELECT @Line9Subtotal=ISNULL(SUM(LineA),0)
FROM VATReturns
WHERE  1=1 AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 9

SELECT @Line23VAT=ISNULL(SUM(LineB),0)
FROM VATReturns
WHERE  1=1 AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 23

";
                #region Clear Data

                sqlText = sqlText + @"

--------------------------------------------------------------------
----------------------------------Clear Data------------------------
delete VATReturns where  PeriodID = @PeriodId and BranchId = @SelectBranchId  


";
                #endregion

                #region Insert Data

                #endregion

                #region Note: 1-26

                sqlText = sqlText + @"

----------------------------------Insert Data-----------------------
--------------------------------------------------------------------

insert into VATReturns (BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
--insert into VATReturns (BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,  @UserName,@Branch,'1' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'SubForm-Ka'SubFormName ,'Export'Remarks 
union all
select  @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'1' NoteNo,'1' SubNoteNo,ROUND( sum(CurrencyValue),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ka'SubFormName
,'Export'Remarks from SalesInvoiceDetails where 1=1 and post='Y' and  Type in('Export') 
and TransactionType in('Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

union all
select  @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'1' NoteNo,'2' SubNoteNo,ROUND( sum(CurrencyValue),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ka'SubFormName
,'Export'Remarks from BureauSalesInvoiceDetails where 1=1 and post='Y' and  Type in('Export') 
and TransactionType in('Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'2' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'SubForm-Ka'SubFormName ,'DeemExport'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'2' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ka'SubFormName
,'DeemExport'Remarks from SalesInvoiceDetails where  1=1 and post='Y' and Type in('DeemExport')
and TransactionType in('Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'2' NoteNo,'2' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ka'SubFormName
,'DeemExport'Remarks from BureauSalesInvoiceDetails where  1=1 and post='Y' and Type in('DeemExport')
and TransactionType in('Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

insert into VATReturns (BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'3' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'SubForm-Ka'SubFormName ,'NonVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'3' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ka'SubFormName
,'NonVAT'Remarks from SalesInvoiceDetails where  1=1 and post='Y' and Type in('NonVAT')
and TransactionType in('Other','RawSale','PackageSale','Wastage','CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'3' NoteNo,'2' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ka'SubFormName
,'NonVAT'Remarks from BureauSalesInvoiceDetails where  1=1 and post='Y' and Type in('NonVAT')
and TransactionType in('Other','RawSale','PackageSale','Wastage','CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

insert into VATReturns  (BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'4' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'SubForm-Ka'SubFormName ,'StandardVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'4' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ka'SubFormName 
,'StandardVAT'Remarks  from SalesInvoiceDetails where  1=1 and post='Y' and Type in('VAT')
and TransactionType in('TollFinishIssue','Other','RawSale','PackageSale','Wastage','CommercialImporter','ServiceNS'
,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'4' NoteNo,'2' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ka'SubFormName 
,'StandardVAT'Remarks  from BureauSalesInvoiceDetails where  1=1 and post='Y' and Type in('VAT')
and TransactionType in('Other','RawSale','PackageSale','Wastage','CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId
 

insert into VATReturns  (BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'5' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'SubForm-Ka'SubFormName ,'MRPRate'Remarks 

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'5' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ka'SubFormName 
,'MRPRate'Remarks  from SalesInvoiceDetails where  1=1 and post='Y' and Type in('MRPRate','MRPRate(SC)')
and TransactionType in('Other','RawSale','PackageSale','Wastage','CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'5' NoteNo,'2' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ka'SubFormName 
,'MRPRate'Remarks  from BureauSalesInvoiceDetails where  1=1 and post='Y' and Type in('MRPRate','MRPRate(SC)')
and TransactionType in('Other','RawSale','PackageSale','Wastage','CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'6' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'SubForm-Ga'SubFormName ,'FixedVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'6' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ga'SubFormName 
,'FixedVAT'Remarks  from SalesInvoiceDetails where  1=1 and post='Y' and Type in('FixedVAT')
and TransactionType in('Other','RawSale','PackageSale','Wastage','CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'6' NoteNo,'2' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ga'SubFormName 
,'FixedVAT'Remarks  from BureauSalesInvoiceDetails where  1=1 and post='Y' and Type in('FixedVAT')
and TransactionType in('Other','RawSale','PackageSale','Wastage','CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'7' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'SubForm-Ka'SubFormName ,'OtherRate'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'7' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ka'SubFormName 
,'OtherRate'Remarks  from SalesInvoiceDetails where  1=1 and post='Y' and Type in('OtherRate')
and TransactionType in('Other','RawSale','PackageSale','Wastage','CommercialImporter','Tender','Trading','ExportTrading','TradingTender','InternalIssue')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'7' NoteNo,'2' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ka'SubFormName 
,'OtherRate'Remarks  from BureauSalesInvoiceDetails where  1=1 and post='Y' and Type in('OtherRate')
and TransactionType in('Other','RawSale','PackageSale','Wastage','CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'7' NoteNo,'3' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Ka'SubFormName 
,'OtherRate'Remarks  from SalesInvoiceDetails where  1=1 and post='Y' and Type in('OtherRate')
and TransactionType in('ServiceNS','Service')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'8' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'SubForm-Kha'SubFormName ,'Retail'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'8' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Kha'SubFormName 
,'Retail'Remarks  from SalesInvoiceDetails where  1=1 and post='Y' and Type in('Retail')
and TransactionType in('Other','RawSale','PackageSale','Wastage','CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'8' NoteNo,'2' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'SubForm-Kha'SubFormName 
,'Retail'Remarks  from BureauSalesInvoiceDetails where  1=1 and post='Y' and Type in('Retail')
and TransactionType in('Other','RawSale','PackageSale','Wastage','CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'9' NoteNo,'0' SubNoteNo,ROUND( sum( LineA),2)LineA,ROUND( sum( LineB),2)LineB,ROUND( sum( LineC),2)LineC,'-'SubFormName ,'SumSub3'Remarks 
from VATReturns where NoteNo in(1,2,3,4,5,6,7,8)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'10' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Ka'SubFormName ,'LocalPurchaseNonVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'10' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(VATAmount),2),ROUND( sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseNonVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('NonVAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'
------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'10' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( 1*sum(VATAmount),2),ROUND( 1*sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseNonVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('NonVAT')
and TransactionType in('PurchaseDN','PurchaseReturn')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId
*/

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'11' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Ka'SubFormName ,'ImportPurchaseNonVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'11' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+CDAmount+RDAmount+SDAmount+TVBAmount),2),ROUND( sum(VATAmount),2),sum(0),'SubForm-Ka'SubFormName,'ImportPurchaseNonVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('NonVAT')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

 
insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'12' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Ka'SubFormName ,'LocalPurchaseExempted'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'12' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(VATAmount),2),ROUND( sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseExempted'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('Exempted')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId
/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'12' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum(VATAmount),2),ROUND( -1*sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseExempted'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('Exempted')
and TransactionType in('PurchaseDN','PurchaseReturn')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId
*/

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'13' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Ka'SubFormName ,'ImportPurchaseExempted'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'13' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+CDAmount+RDAmount+SDAmount+TVBAmount),2),ROUND( sum(VATAmount),2),sum(0),'SubForm-Ka'SubFormName,'ImportPurchaseExempted'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('Exempted')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId



insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'14' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Ka'SubFormName ,'LocalPurchaseStandardVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'14' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(VATAmount),2),ROUND( sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseStandardVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('VAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService','TollReceive')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'14' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum(VATAmount),2),ROUND( -1*sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseStandardVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('VAT')
and TransactionType in('PurchaseDN','PurchaseReturn')
and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId
and RebatePeriodID=@PeriodId
and IsRebate='Y'
*/

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'15' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Ka'SubFormName ,'ImportPurchaseStandardVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'15' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+CDAmount+RDAmount+SDAmount+TVBAmount),2),ROUND( sum(VATAmount),2),sum(0),'SubForm-Ka'SubFormName,'ImportPurchaseStandardVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('VAT')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId



insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'16' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Ka'SubFormName ,'LocalPurchaseOtherRate'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'16' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(VATAmount),2),ROUND( sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseOtherRate'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('OtherRate')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'16' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum(VATAmount),2),ROUND( -1*sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseOtherRate'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('OtherRate')
and TransactionType in('PurchaseDN','PurchaseReturn')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'
------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId
*/

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'17' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Ka'SubFormName ,'ImportPurchaseOtherRate'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'17' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+CDAmount+RDAmount+SDAmount+TVBAmount),2),ROUND( sum(VATAmount),2),sum(0),'SubForm-Ka'SubFormName,'ImportPurchaseOtherRate'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('OtherRate')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Ga'SubFormName ,'LocalPurchaseFixedVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(VATAmount),2),ROUND( sum(SDAmount),2),'SubForm-Ga'SubFormName,'LocalPurchaseFixedVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('FixedVAT')
and TransactionType in('Other','PurchaseCNXXXX','Trading','Service','ServiceNS','InputService')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum(VATAmount),2),ROUND( -1*sum(SDAmount),2),'SubForm-Ga'SubFormName,'LocalPurchaseFixedVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('FixedVAT')
and TransactionType in('PurchaseDN','PurchaseReturn')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId
*/

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'19' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Ka'SubFormName ,'LocalPurchaseTurnover'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'19' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(VATAmount),2),ROUND( sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseTurnover'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('Turnover')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'19' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum(VATAmount),2),ROUND( -1*sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseTurnover'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('Turnover')
and TransactionType in('PurchaseDN','PurchaseReturn')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId
*/

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'20' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Ka'SubFormName ,'LocalPurchaseUnRegisterVendor'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'20' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(VATAmount),2),ROUND( sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseUnRegisterVendor'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('UnRegister')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'20' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum(VATAmount),2),ROUND( -1*sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseUnRegisterVendor'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('UnRegister')
and TransactionType in('PurchaseDN','PurchaseReturn')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId
*/


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Ka'SubFormName ,'LocalPurchaseNonRebate'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(VATAmount),2),ROUND( sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseNonRebate'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum(VATAmount),2),ROUND( -1*sum(SDAmount),2),'SubForm-Ka'SubFormName,'LocalPurchaseNonRebate'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('PurchaseDN','PurchaseReturn')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId
*/

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'22' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Ka'SubFormName ,'ImportPurchaseNonRebate'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'22' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+CDAmount+RDAmount+SDAmount+TVBAmount),2),ROUND( sum(VATAmount),2),sum(0),'SubForm-Ka'SubFormName,'ImportPurchaseNonRebate'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y' and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'23' NoteNo,'0' SubNoteNo,ROUND( sum( LineA),2)LineA,ROUND( sum( LineB),2)LineB,ROUND( sum( LineC),2)LineC,'-'SubFormName ,'SumSub4'Remarks
 from (
select @BranchId BranchId,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'23' NoteNo,'0' SubNoteNo,ROUND( sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'SumSub4'Remarks 
from VATReturns where NoteNo in(10,11,12,13)

 and PeriodID = @PeriodId and BranchId = @SelectBranchId 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'23' NoteNo,'0' SubNoteNo,ROUND( sum(LineA),2)LineA,ROUND( sum(LineB),2) LineB,0 LineC,'-'SubFormName ,'SumSub4'Remarks 
from VATReturns where NoteNo in(14,15,16,17,18)

 and PeriodID = @PeriodId and BranchId = @SelectBranchId 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'23' NoteNo,'0' SubNoteNo,ROUND( sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'SumSub4'Remarks 
from VATReturns where NoteNo in(19,20,21,22)

 and PeriodID = @PeriodId and BranchId = @SelectBranchId 
 ) as a

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'24' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Gha'SubFormName ,'PurcshaseVDS'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'24' NoteNo,'1' SubNoteNo,ROUND( sum(BillDeductAmount),2),0 VATAmount,0 SDAmount,'SubForm-Gha'SubFormName,'PurcshaseVDS'Remarks  
from VDS where  1=1 and post='Y'   
and IsPurchase in('Y'  )
and PeriodId=@PeriodId
------and DepositDate >= @Datefrom and DepositDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'25' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'AdjustmentWithoutBankPay'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'25' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'AdjustmentWithoutBankPay'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('WithoutBankPay')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'26' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'SaleDebitNote'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'26' NoteNo,'1' SubNoteNo,ROUND(sum(VATAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SaleDebitNote'Remarks 
from SalesInvoiceDetails where  1=1 and post='Y'   
and TransactionType in('Debit')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'26' NoteNo,'2' SubNoteNo,ROUND(sum(VATAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SaleDebitNote'Remarks 
from BureauSalesInvoiceDetails where  1=1 and post='Y'   
and TransactionType in('Debit')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

";

                #endregion

                #region Note: 27

                sqlText = sqlText + @"
--------insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
--------select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'27' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'IncreasingAdjustment'Remarks 
--------union all
--------select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'27' NoteNo,'1' SubNoteNo,ROUND(sum(AdjAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'IncreasingAdjustment'Remarks  
--------from AdjustmentHistorys where  1=1 and post='Y'   
--------and AdjType in('IncreasingAdjustment')
--------and AdjDate >= @Datefrom and AdjDate <dateadd(d,1,@Dateto)
--------AND BranchId=@BranchId


-----------------------------#TempAdjustmentHistorys/IncreasingAdjustment----------------------------Create/INSERT
-----------------------------------------------------------------------------------------------------
SELECT  *
INTO #TempAdjustmentHistorys 

FROM 
(
SELECT '27' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'IncreasingAdjustment'Remarks 
, '' AdjType, '' AdjName
UNION ALL
SELECT '27' NoteNo,'1' SubNoteNo,ROUND(sum(AdjAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'IncreasingAdjustment'Remarks  
, ah.AdjType, an.AdjName
FROM AdjustmentHistorys ah
LEFT OUTER JOIN AdjustmentName an ON ah.AdjId=an.AdjId
WHERE  1=1 and post='Y'   
AND ah.AdjType in('IncreasingAdjustment')
and ah.PeriodId=@PeriodId
------AND ah.AdjDate >= @Datefrom and ah.AdjDate <dateadd(d,1,@Dateto)
AND ah.BranchId=@BranchId
GROUP BY ah.AdjType, an.AdjName
UNION ALL

SELECT  '27' NoteNo,'2' SubNoteNo
,ddbd.ClaimVAT Subtotal
, 0 VATAmount,0 SDAmount,'-'SubFormName,'IncreasingAdjustment' Remarks
,ddbd.TransactionType AdjType 
,ddbd.TransactionType AdjName  
FROM DutyDrawBackDetails ddbd
LEFT OUTER JOIN PurchaseInvoiceDetails pid ON pid.PurchaseInvoiceNo=ddbd.PurchaseInvoiceNo
WHERE 1=1 AND ddbd.BranchId=@BranchId
AND ddbd.Post='Y'
--and ddbd.PeriodId=@PeriodId
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'

------AND ddbd.DDBackDate >= @Datefrom and ddbd.DDBackDate <dateadd(d,1,@Dateto)
AND ISNULL(ddbd.TransactionType,'DDB') = 'VDB'
AND pid.Type IN('VAT','FixedVAT')


UNION ALL
SELECT '27' NoteNo,'3' SubNoteNo
, CASE
WHEN @AutoPartialRebateProcess='N' THEN 0 
WHEN @Line9Subtotal > 0 THEN ISNULL(ROUND((SUM(LineA)*@Line23VAT/@Line9Subtotal),2),0) 
ELSE 0 END AS Subtotal
, 0 VATAmount,0 SDAmount,'-'SubFormName,'IncreasingAdjustment'Remarks  
,'IncreasingAdjustment' AdjType, 'Exempted Goods/Service (Rebate Cancel)' AdjName
FROM VATReturns
WHERE  1=1
AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 3

UNION ALL
SELECT '27' NoteNo,'4' SubNoteNo
, CASE 
WHEN @AutoPartialRebateProcess='N' THEN 0 
WHEN @Line9Subtotal > 0 THEN ISNULL(ROUND((SUM(LineA)*@Line23VAT/@Line9Subtotal),2),0) 
ELSE 0 END AS Subtotal
, 0 VATAmount,0 SDAmount,'-'SubFormName,'IncreasingAdjustment'Remarks  
,'IncreasingAdjustment' AdjType, 'Other Rated VAT (Rebate Cancel)' AdjName
FROM VATReturns
WHERE  1=1
AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 7 AND SubNoteNo IN(1,2)

) AS adj

INSERT INTO VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
SELECT @BranchId,@PeriodId, @DateFrom,@UserName,@Branch, '27' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'IncreasingAdjustment'Remarks 
UNION ALL

SELECT @BranchId,@PeriodId, @DateFrom,@UserName,@Branch, NoteNo, SubNoteNo, Subtotal, VATAmount, SDAmount, SubFormName, Remarks
FROM #TempAdjustmentHistorys
where SubTotal>0



-----------------------------END/Rebate Cancel----------------------------
--------------------------------------------------------------------------

";
                #endregion

                #region Note: 28-30

                sqlText = sqlText + @"

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'28' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-'SubFormName ,'SumSub5'Remarks 
from VATReturns where NoteNo in(24,25,26,27)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'29' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Umo'SubFormName ,'SaleVDS'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'29' NoteNo,'1' SubNoteNo,ROUND(sum(BillDeductAmount),2),0 VATAmount,0 SDAmount,'SubForm-Umo'SubFormName,'SaleVDS'Remarks  
from VDS where  1=1 and post='Y'   
and IsPurchase in('N' )
and PeriodId=@PeriodId
------and DepositDate >= @Datefrom and DepositDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'30' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Cha'SubFormName ,'PurchaseAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'30' NoteNo,'1' SubNoteNo
,case when @ATVRebate='Y' then ROUND(sum(ATVAmount),2) else 0 end ,sum(0),sum(0),'SubForm-Cha'SubFormName,'PurchaseAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and post='Y'  
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'

------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

";
                #endregion

                #region Note: 31

                sqlText = sqlText + @"

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'31' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'DDB'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'31' NoteNo,'1' SubNoteNo,ROUND(sum(SubTotalDDB),2),0 VATAmount,0 SDAmount,'-'SubFormName,'DDB'Remarks 
from DutyDrawBackDetails where  1=1 and post='Y'   
and PeriodId=@PeriodId
------and DDBackDate >= @Datefrom and DDBackDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId
AND ISNULL(TransactionType,'DDB') = 'DDB'
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'31' NoteNo,'2' SubNoteNo, SubTotal ,0 VATAmount,0 SDAmount,'-'SubFormName,'DDB'Remarks 
from #TempAdjustmentHistorys
WHERE 1=1 AND SubNoteNo IN (2)

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'31' NoteNo,'2' SubNoteNo,ROUND(sum(AdjAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'DDB'Remarks  
from AdjustmentHistorys where  1=1 and post='Y'   
and AdjType in('DDB')
and PeriodId=@PeriodId
------and AdjDate >= @Datefrom and AdjDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

";
                #endregion

                #region Note: 32

                sqlText = sqlText + @"

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'32' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'SaleCreditNote'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'32' NoteNo,'1' SubNoteNo,ROUND(sum(VATAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SaleCreditNote'Remarks 
from SalesInvoiceDetails where  1=1 and post='Y'   
and TransactionType in('Credit')
and PeriodId=@PeriodId
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'32' NoteNo,'2' SubNoteNo,ROUND(sum(VATAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SaleCreditNote'Remarks 
from BureauSalesInvoiceDetails where  1=1 and post='Y'   
and TransactionType in('Credit')
and PeriodId=@PeriodId
AND BranchId=@BranchId


";
                #endregion

                #region Note: 33

                sqlText = sqlText + @"

--------insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
--------select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'33' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'DecreasingAdjustment'Remarks 
--------union all
--------select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'33' NoteNo,'1' SubNoteNo,ROUND(sum(AdjAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'DecreasingAdjustment'Remarks  
--------from AdjustmentHistorys where  1=1 and post='Y'   
--------and AdjType in('DecreasingAdjustment')
--------and AdjDate >= @Datefrom and AdjDate <dateadd(d,1,@Dateto)
--------AND BranchId=@BranchId


SELECT  *
INTO #TempDecrementAdjustmentHistorys 

FROM
(
SELECT '33' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'DecreasingAdjustment'Remarks
, '' AdjType, '' AdjName 
UNION ALL
SELECT '33' NoteNo,'1' SubNoteNo,ROUND(sum(AdjAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'DecreasingAdjustment'Remarks  
, ah.AdjType, an.AdjName
FROM AdjustmentHistorys ah
LEFT OUTER JOIN AdjustmentName an ON ah.AdjId=an.AdjId
WHERE  1=1 and post='Y'   
AND ah.AdjType in('DecreasingAdjustment')
and ah.PeriodId=@PeriodId
AND ah.BranchId=@BranchId
GROUP BY ah.AdjType, an.AdjName

UNION ALL

SELECT  '33' NoteNo,'2' SubNoteNo
,ddbd.ClaimVAT Subtotal
, 0 VATAmount,0 SDAmount,'-'SubFormName,'DecreasingAdjustment' Remarks
,'DecreasingAdjustment' AdjType 
,ddbd.TransactionType AdjName  
FROM DutyDrawBackDetails ddbd
LEFT OUTER JOIN PurchaseInvoiceDetails pid ON pid.PurchaseInvoiceNo=ddbd.PurchaseInvoiceNo
WHERE 1=1 AND ddbd.BranchId=@BranchId
AND ddbd.Post='Y'
--and ddbd.PeriodId=@PeriodId
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'

AND ISNULL(ddbd.TransactionType,'DDB') = 'VDB'
AND pid.Type='NonRebate'
) decAdj

INSERT INTO VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
SELECT @BranchId,@PeriodId, @DateFrom,@UserName,@Branch, NoteNo, SubNoteNo
, Subtotal
, VATAmount, SDAmount, SubFormName,  Remarks  
FROM #TempDecrementAdjustmentHistorys


";
                #endregion

                #region Note: 34-49 [Excluding: 35, 36, 36, 37, 38]

                sqlText = sqlText + @"


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'34' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-'SubFormName ,'SumSub6'Remarks 
from VATReturns where NoteNo in(29,30,31,32,33)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'39' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'SDSaleDebitNote'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'39' NoteNo,'1' SubNoteNo,ROUND(sum(SDAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SDSaleDebitNote'Remarks 
from SalesInvoiceDetails where  1=1 and post='Y'   
and TransactionType in('Debit')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'39' NoteNo,'2' SubNoteNo,ROUND(sum(SDAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SDSaleDebitNote'Remarks 
from BureauSalesInvoiceDetails where  1=1 and post='Y'   
and TransactionType in('Debit')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'40' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'SDSaleCreditNote'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'40' NoteNo,'1' SubNoteNo,ROUND(sum(SDAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SDSaleCreditNote'Remarks 
from SalesInvoiceDetails where  1=1 and post='Y'   
and TransactionType in('Credit')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'40' NoteNo,'2' SubNoteNo,ROUND(sum(SDAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SDSaleCreditNote'Remarks 
from BureauSalesInvoiceDetails where  1=1 and post='Y'   
and TransactionType in('Credit')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'41' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'SDExportSale'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'41' NoteNo,'1' SubNoteNo,ROUND(sum(ApprovedSD),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SDExportSale'Remarks 
from DutyDrawBackHeader where  1=1 and post='Y'   
and ISNULL(DutyDrawBackHeader.TransactionType,'DDB') = 'SDB'
and PeriodId=@PeriodId
------and DDBackDate >= @Datefrom and DDBackDate <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'42' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'InterestOnOveredVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'42' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'InterestOnOveredVAT'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('InterestOnOveredVAT')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'43' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'InterestOnOveredSD'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'43' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'InterestOnOveredSD'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('InterestOnOveredSD')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'44' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'FineOrPenalty'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'44' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'FineOrPenalty'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('FineOrPenalty')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'45' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'ExciseDuty'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'45' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'ExciseDuty'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('ExciseDuty')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'46' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'DevelopmentSurcharge'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'46' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'DevelopmentSurcharge'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('DevelopmentSurcharge')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId



insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'47' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'ICTDevelopmentSurcharge'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'47' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'ICTDevelopmentSurcharge'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('ICTDevelopmentSurcharge')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'48' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'HelthCareSurcharge'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'48' NoteNo,'1' SubNoteNo,ROUND(sum(HPSAmount),2),0,0,'-'SubFormName,'HelthCareSurcharge'Remarks 
from SalesInvoiceDetails where  1=1 and post='Y' and Type in('MRPRate(SC)')
and TransactionType in('Other','RawSale','PackageSale','Wastage','CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId



insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'49' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'EnvironmentProtectionSurcharge'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'49' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'EnvironmentProtectionSurcharge'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('EnvironmentProtectionSurcharge')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

";
                #endregion

                #region Note: 50

                sqlText = sqlText + @"

---------------------------------Line50-----------------------------------------------
--------------------------------------------------------------------------------------


--select @PreviousPeriodID=format( DATEADD(M, -1,@DateFrom),'MMyyyy')
select @LastLine62=ISNULL(sum(LineA),0) FROM VATReturns WHERE NoteNo='62' and PeriodID=@PreviousPeriodID AND BranchId=@SelectBranchId;;
select @LastLine63=ISNULL(sum(LineA),0) FROM VATReturns WHERE NoteNo='63' and PeriodID=@PreviousPeriodID AND BranchId=@SelectBranchId;;


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)

select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'50' NoteNo,'1' SubNoteNo, sum(SubTotal)SubTotal,sum( VATAmount) VATAmount,sum( SDAmount)SDAmount ,'-'SubFormName ,'LastClosingVAT'Remarks
from(

select ISNULL(@LastLine62,0) SubTotal,0 VATAmount,0 SDAmount

UNION ALL

select ISNULL(DepositAmount,0) SubTotal,0 VATAmount,0 SDAmount

from Deposits
where 1=1
AND Post='Y'  
and PeriodId=@PeriodId
------AND DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND DepositType = 'Opening'
AND BranchId=@BranchId
) as a

";
                #endregion

                #region Note: 51, 35-38

                sqlText = sqlText + @"
 
insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'51' NoteNo,'0' SubNoteNo,isnull( @LastLine63,0) SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'LastClosingSD'Remarks 

----------------------------------------

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'35' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-'SubFormName ,'Line35'Remarks
 from (
select @BranchId BranchId,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'35' NoteNo,'0' SubNoteNo,ROUND(sum( LineC),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line35'Remarks 
from VATReturns where NoteNo in(9)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'35' NoteNo,'0' SubNoteNo,ROUND(-1*sum(LineB),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line35'Remarks 
from VATReturns where NoteNo in(23)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'35' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line35'Remarks 
from VATReturns where NoteNo in(28)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'35' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line35'Remarks 
from VATReturns where NoteNo in(34)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 
 ) as a


";

                sqlText = sqlText + @"

 insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'36' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-' SubFormName ,'Line36' Remarks
 from (
select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'36' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line36'Remarks 
from VATReturns where NoteNo in(35)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 

union all
select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'36' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line36'Remarks 
from VATReturns where NoteNo in(50)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 
 ) as a


";

                sqlText = sqlText + @" 
  insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'37' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-' SubFormName ,'Line37' Remarks
 from (
select  @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'37' NoteNo,'0' SubNoteNo,ROUND(sum( LineB),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line37'Remarks 
from VATReturns where NoteNo in(9)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 
union all
select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'37' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line37'Remarks 
from VATReturns where NoteNo in(39)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 

union all
select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'37' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line37'Remarks 
from VATReturns where NoteNo in(40)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'37' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line37'Remarks 
from VATReturns where NoteNo in(41)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 
 ) as a
";

                sqlText = sqlText + @"

 
 insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'38' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-' SubFormName ,'Line38' Remarks
 from (
select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'38' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line38'Remarks 
from VATReturns where NoteNo in(37)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 

union all
select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'38' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line38'Remarks 
from VATReturns where NoteNo in(51)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 
 ) as a
 ";
                #endregion

                #region Note: 52-61

                sqlText = sqlText + @" 

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'52' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName ,'TotalDepositVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'52' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName,'TotalDepositVAT'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('Treasury')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'52' NoteNo,'2' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName,'TotalDepositVAT'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('VDS') and DepositType not in('NotDeposited')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

";

                sqlText = sqlText + @"
insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'53' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName ,'TotalDepositSD'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'53' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName,'TotalDepositSD'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('SD')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId



insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'54' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName ,'InterestOnOveredVATDeposit'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'54' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName,'InterestOnOveredVATDeposit'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('InterestOnOveredVAT') and DepositType not in('NotDeposited')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'55' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName ,'InterestOnOveredSDDeposit'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'55' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName,'InterestOnOveredSDDeposit'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('InterestOnOveredSD') and DepositType not in('NotDeposited')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'56' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName ,'FineOrPenaltyDeposit'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'56' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName,'FineOrPenaltyDeposit'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('FineOrPenalty') and DepositType not in('NotDeposited')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId



insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'57' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName ,'ExciseDutyDeposit'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'57' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName,'ExciseDutyDeposit'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('ExciseDuty') and DepositType not in('NotDeposited')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId



insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'58' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName ,'DevelopmentSurchargeDeposit'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'58' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName,'DevelopmentSurchargeDeposit'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('DevelopmentSurcharge') and DepositType not in('NotDeposited')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId



insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'59' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName ,'ICTDevelopmentSurchargeDeposit'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'59' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName,'ICTDevelopmentSurchargeDeposit'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('ICTDevelopmentSurcharge') and DepositType not in('NotDeposited')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'60' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName ,'HelthCareSurchargeDeposit'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'60' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName,'HelthCareSurchargeDeposit'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('HelthCareSurcharge') and DepositType not in('NotDeposited')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId



insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'61' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName ,'EnvironmentProtectionSurchargeDeposit'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'61' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName,'EnvironmentProtectionSurchargeDeposit'Remarks  
from Deposits where  1=1 and post='Y'   
and TransactionType in('EnvironmentProtectionSurcharge') and DepositType not in('NotDeposited')
and PeriodId=@PeriodId
------and DepositDateTime >= @Datefrom and DepositDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

";
                #endregion

                #region Note: 62

                sqlText = sqlText + @"

insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'62' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-' SubFormName ,'Line62' Remarks
 from (
select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'62' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line62'Remarks 
from VATReturns where NoteNo in(52)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 

union all
select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'62' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line38'Remarks 
from VATReturns where NoteNo in(36)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 
 ) as a
  
insert into VATReturns(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'63' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-' SubFormName ,'Line63' Remarks
 from (
select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'63' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line63'Remarks 
from VATReturns where NoteNo in(53)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 

union all
select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'63' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line63'Remarks 
from VATReturns where NoteNo in(38)
 and PeriodID = @PeriodId and BranchId = @SelectBranchId 
 ) as a

";
                #endregion

                #region 9.1 Report

                sqlText = sqlText + @"

select distinct Remarks NoteDescription,  NoteNo,ROUND( Sum(LineA),2)LineA,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC,SubFormName
from VATReturns
 where  BranchId=@SelectBranchId 
and PeriodID=@PeriodID
group by NoteNo,SubFormName,Remarks
order by NoteNo

";
                #endregion

                #region DataTable 2

                sqlText = sqlText + @"
---------------------------------#TempAdjustmentHistorys/IncreasingAdjustment----------------------------SELECT
---------------------------------------------------------------------------------------------------------

SELECT DISTINCT  AdjType,ah.AdjName,ROUND(SUM(ah.SubTotal),2)AdjAmount FROM #TempAdjustmentHistorys ah
WHERE AdjType='IncreasingAdjustment' and ah.SubTotal>0
GROUP BY AdjType,ah.AdjName


DROP TABLE #TempAdjustmentHistorys;

---------------------------------END/TempAdjustmentHistorys-------------------------
------------------------------------------------------------------------------------



--------select distinct  AdjType,an.AdjName,ROUND(sum(ah.AdjAmount),2)AdjAmount from AdjustmentHistorys ah
--------left outer join AdjustmentName an on ah.AdjId=an.AdjId
--------where AdjType='IncreasingAdjustment'
--------AND ah.BranchId=@BranchId
--------and Post='Y'
--------and AdjDate >= @Datefrom and AdjDate <dateadd(d,1,@Dateto)
--------group by AdjType,an.AdjName
";
                #endregion

                #region DataTable 3
                sqlText = sqlText + @"
--------select distinct  AdjType,an.AdjName,ROUND(sum(ah.AdjAmount),2)AdjAmount from AdjustmentHistorys ah
--------left outer join AdjustmentName an on ah.AdjId=an.AdjId
--------where AdjType='DecreasingAdjustment'
--------AND ah.BranchId=@BranchId
--------and Post='Y'
--------and AdjDate >= @Datefrom and AdjDate <dateadd(d,1,@Dateto)
--------group by AdjType,an.AdjName


---------------------------------#TempDecrementAdjustmentHistorys/IncreasingAdjustment----------------------------SELECT
------------------------------------------------------------------------------------------------------------------

SELECT DISTINCT  AdjType,ah.AdjName,ROUND(SUM(ah.SubTotal),2)AdjAmount FROM #TempDecrementAdjustmentHistorys ah
WHERE AdjType='DecreasingAdjustment' and ah.SubTotal>0
GROUP BY AdjType,ah.AdjName


DROP TABLE #TempDecrementAdjustmentHistorys;

---------------------------------END/#TempDecrementAdjustmentHistorys-------------------------
----------------------------------------------------------------------------------------------

";
                #endregion




                #endregion
                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                    //sqlText = sqlText.Replace("=@SelectBranchId", ">@SelectBranchId");
                }

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter
                objCommVAT19.Parameters.AddWithValue("@SelectBranchId", vm.BranchId);
                objCommVAT19.Parameters.AddWithValue("@BranchId", vm.BranchId);
                objCommVAT19.Parameters.AddWithValue("@PreviousPeriodID", PreviousPeriodID);



                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }




                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dataSet);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1", sqlex.ToString() + "\n" + sqlText);

                throw new Exception(sqlText);
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1", ex.ToString() + "\n" + sqlText);

                throw new Exception(sqlText);
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dataSet;
        }

        public DataSet VAT9_1_V2Save(VATReturnVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string PreviousPeriodID = "";
            DataSet dataSet = new DataSet("VAT9_1_V2");

            #endregion

            #region Try

            try
            {

                PostStatus(vm);

                if (!string.IsNullOrWhiteSpace((vm.Date)))
                {
                    PreviousPeriodID = Convert.ToDateTime(vm.Date).AddMonths(-1).ToString("MMyyyy");
                }

                DateTime periodDate = Convert.ToDateTime(Convert.ToDateTime(vm.PeriodName).ToString("yyyy-MM-dd HH:mm:ss"));


                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }


                //if (transaction == null)
                //{
                //    transaction = currConn.BeginTransaction("VAT9.1");
                //}

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code", connVM);
                string OldDBName = commonDal.settingValue("DatabaseName", "DatabaseName", connVM);
                string ClientFGReceiveIn9_1 = commonDal.settingValue("Toll", "ClientFGReceiveIn9_1", connVM);


                DateTime oldDBDate = Convert.ToDateTime(Convert.ToDateTime("2021-07-01").ToString("yyyy-MM-dd HH:mm:ss"));
                DateTime oldDBDateTo = Convert.ToDateTime(Convert.ToDateTime("2021-08-31").ToString("yyyy-MM-dd HH:mm:ss"));

                // OldDBName = "ACI2012_Demo_Old_DB";


                #region Statement

                #region SQLText

                sqlText = @" ";

                #region Beginning

                sqlText = @" 
    declare @UserName as varchar(100);
    declare @Branch as varchar(100);
    declare @ATVRebate as varchar(100);
    declare @AutoPartialRebateProcess as varchar(1);

    ----declare @periodName VARCHAR (200);
    ----declare @ExportInBDT VARCHAR (200);
    declare @DateFrom [datetime];
    declare @DateTo [datetime];
    declare @MLock varchar(1);

    ----SET @periodName='December-2018';
    ----SET @ExportInBDT='Y'

    --declare @Post1 as varchar(10) ='Y'
	--declare @Post2 as varchar(10) ='N'

    set @UserName ='admin'
    set @Branch ='HO_DB'

    ----declare @BranchId as int
    ----set @BranchId = 0
    ----declare @SelectBranchId as int = 0

    declare @PeriodId as varchar(100);
    ----declare @PreviousPeriodID as int = 072019;

    declare @LastLine65 as decimal(18, 5) = 0;
    declare @LastLine66 as decimal(18, 5) = 0;

    declare @LastLine54 as decimal(18, 5) = 0;
    declare @LastLine55 as decimal(18, 5) = 0;
    declare @LastLine56 as decimal(18, 5) = 0;
    declare @LastLine57 as decimal(18, 5) = 0;


    declare @Line54 as decimal(18, 5) = 0;
    declare @Line55 as decimal(18, 5) = 0;
    declare @VAT18_6Adjustment as varchar(100);



    ----------------------------------Initialization------------------------
    ------------------------------------------------------------------------
    select  @PeriodId=PeriodId, @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;



    select @ATVRebate=settingValue  FROM Settings where SettingGroup='ImportPurchase' and SettingName='ATVRebate';
    select @AutoPartialRebateProcess=settingValue  FROM Settings where SettingGroup='Sale' and SettingName='AutoPartialRebateProcess';

    -----------------------------Initialization/Rebate Cancel----------------------------
    -------------------------------------------------------------------------------------
    DECLARE @Line9Subtotal AS DECIMAL = 0
    DECLARE @Line23VAT AS DECIMAL = 0

    SELECT @Line9Subtotal=ISNULL(SUM(LineA),0)
    FROM VATReturnV2s
    WHERE  1=1 AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 9

    SELECT @Line23VAT=ISNULL(SUM(LineB),0)
    FROM VATReturnV2s
    WHERE  1=1 AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 23

    ";
                #endregion

                #region Clear Data

                sqlText = sqlText + @"

    --------------------------------------------------------------------
    ----------------------------------Clear Data------------------------
    delete VATReturnV2s where  PeriodID = @PeriodId and BranchId = @SelectBranchId  


    ";
                #endregion

                #region Insert Data

                #endregion

                #region Note: 1-26

                sqlText = sqlText + @"

--------------------------------------------------Insert Data--------------------------------------------------
---------------------------------------------------------------------------------------------------------------
";

                #region Supply Data (Note-01 - Note-09)

                #region Note-01

                sqlText = sqlText + @"
-------------------------------------------------- Note-01 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------
insert into VATReturnV2s (BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,  @UserName,@Branch,'1' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'Sub-Form'SubFormName ,'Export'Remarks 
union all
select  @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'1' NoteNo,'1' SubNoteNo,ROUND( sum(CurrencyValue),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName
,'Export'Remarks from SalesInvoiceDetails where 1=1 and (post=@post1 or post=@post2) and  Type in('Export') 
and TransactionType in('ServiceNS','Service','Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and PeriodId=@PeriodId
AND BranchId=@BranchId

union all
select  @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'1' NoteNo,'2' SubNoteNo,ROUND( sum(CurrencyValue),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName
,'Export'Remarks from BureauSalesInvoiceDetails where 1=1 and  (post=@post1 or post=@post2)  and  Type in('Export') 
and TransactionType in('ServiceNS','Service','Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and PeriodId=@PeriodId
--and right('000000'+PeriodId,6)=@PeriodId
AND BranchId=@BranchId
";
                #endregion

                #region Note-02

                sqlText = sqlText + @"
-------------------------------------------------- Note-02 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'2' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'Sub-Form'SubFormName ,'DeemExport'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'2' NoteNo,'1' SubNoteNo,ROUND( sum(CurrencyValue),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName
,'DeemExport'Remarks from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('DeemExport')
and TransactionType in('ServiceNS','Service','Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and PeriodId=@PeriodId
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'2' NoteNo,'2' SubNoteNo,ROUND( sum(CurrencyValue),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName
,'DeemExport'Remarks from BureauSalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('DeemExport')
and TransactionType in('ServiceNS','Service','Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and PeriodId=@PeriodId
AND BranchId=@BranchId
";
                #endregion

                #region Note-03

                if (code.ToLower() == "gdic".ToLower())
                {
                    #region Note-03 for GDIC

                    sqlText = sqlText + @"
-------------------------------------------------- Note-03 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s (BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'3' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'Sub-Form'SubFormName ,'NonVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'3' NoteNo,'1' SubNoteNo,ROUND( sum(ISNULL(LeaderAmount,0)),2),ROUND( sum(SDAmount),2),0 --ROUND( sum(VATAmount-ISNULL(NonLeaderVATAmount,0)),2)
,'Sub-Form'SubFormName
,'NonVAT'Remarks from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonVAT')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
and PeriodId=@PeriodId
AND BranchId=@BranchId
and IsLeader='Y'

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'3' NoteNo,'1' SubNoteNo,ROUND( sum(ISNULL(NonLeaderAmount,0)),2),ROUND( sum(SDAmount),2),0 --ROUND( sum(VATAmount-ISNULL(NonLeaderVATAmount,0)),2)
,'Sub-Form'SubFormName
,'NonVAT'Remarks from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonVAT')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
and PeriodId=@PeriodId
AND BranchId=@BranchId
and IsLeader='N'

";
                    #endregion

                }
                else
                {
                    #region Note-03 for Other

                    sqlText = sqlText + @"
-------------------------------------------------- Note-03 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s (BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'3' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'Sub-Form'SubFormName ,'NonVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'3' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+VATAmount-ISNULL(NonLeaderAmount,0)),2),ROUND( sum(SDAmount),2),0 --ROUND( sum(VATAmount-ISNULL(NonLeaderVATAmount,0)),2)
,'Sub-Form'SubFormName
,'NonVAT'Remarks from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonVAT')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale','TollFinishIssue')
and PeriodId=@PeriodId
AND BranchId=@BranchId
and ISNULL(IsLeader,'NA') IN ('NA','Y')
AND isnull(SourcePaidQuantity,0)=0
 HAVING  (ROUND( sum(SubTotal),2)+ROUND( sum(SDAmount),2)+ROUND( sum(VATAmount),2))>0

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'3' NoteNo,'1' SubNoteNo,ROUND( sum(option2),2),0,0 --ROUND( sum(VATAmount-ISNULL(NonLeaderVATAmount,0)),2)
,'Sub-Form'SubFormName
,'NonVAT'Remarks from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonVAT','Retail')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale','TollFinishIssue')
and PeriodId=@PeriodId
AND BranchId=@BranchId
and ISNULL(IsLeader,'NA') IN ('NA','Y')
AND isnull(SourcePaidQuantity,0)=0
AND EXISTS ( select * from Settings where SettingGroup = 'CompanyCode' and SettingName = 'Code' and SettingValue = 'bata') 
 HAVING  (ROUND( sum(SubTotal),2)+ROUND( sum(SDAmount),2)+ROUND( sum(VATAmount),2))>0

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'3' NoteNo,'2' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),0 --ROUND( sum(VATAmount),2)
,'Sub-Form'SubFormName
,'NonVAT'Remarks from BureauSalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonVAT')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale','TollFinishIssue')
and PeriodId=@PeriodId
AND BranchId=@BranchId
 HAVING  (ROUND( sum(SubTotal),2)+ROUND( sum(SDAmount),2)+ROUND( sum(VATAmount),2))>0

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'3' NoteNo,'3' SubNoteNo,ROUND( sum(SubTotal-LeaderAmount),2),ROUND( sum(SDAmount),2),0 --ROUND( sum(VATAmount-LeaderVATAmount),2)
,'Sub-Form'SubFormName
,'NonVAT'Remarks from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonVAT')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale','TollFinishIssue')
and PeriodId=@PeriodId
AND BranchId=@BranchId
and ISNULL(IsLeader,'NA') ='N'
AND isnull(SourcePaidQuantity,0)=0
 HAVING  (ROUND( sum(SubTotal),2)+ROUND( sum(SDAmount),2)+ROUND( sum(VATAmount),2))>0

";
                    #endregion

                }

                #endregion

                #region Note-04

                if (code.ToLower() == "gdic".ToLower())
                {
                    #region Note-04 GDIC

                    sqlText = sqlText + @"
-------------------------------------------------- Note-04 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s  (BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'4' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'Sub-Form'SubFormName ,'StandardVAT'Remarks 

union all

select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'4' NoteNo,'1' SubNoteNo,ROUND( sum(ISNULL(SubTotal,0)),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName 
,'StandardVAT'Remarks  from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('VAT')
 and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')--,'TollFinishIssue'
and PeriodId=@PeriodId
AND BranchId=@BranchId

";
                    #endregion

                }
                else
                {
                    #region Note-04 Others

                    sqlText = sqlText + @"
-------------------------------------------------- Note-04 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s  (BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'4' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'Sub-Form'SubFormName ,'StandardVAT'Remarks 

union all

select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'4' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal-ISNULL(NonLeaderAmount,0)),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount-ISNULL(NonLeaderVATAmount,0)),2),'Sub-Form'SubFormName 
,'StandardVAT'Remarks  from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('VAT')
 and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')--,'TollFinishIssue'
and PeriodId=@PeriodId
AND BranchId=@BranchId
AND isnull(SourcePaidQuantity,0) = 0
and ISNULL(IsLeader,'NA') IN ('NA','Y')
and ProductType  is not null
and ProductType in('R','P')

union all

select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'4' NoteNo,'2' SubNoteNo,ROUND( sum(SubTotal-isnull(NonLeaderAmount,0)),2),  ROUND( sum(SDAmount),2),ROUND( sum(VATAmount-isnull(NonLeaderVATAmount,0)),2),'Sub-Form'SubFormName 
,'StandardVAT'Remarks  from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('VAT')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')--,'TollFinishIssue'
and PeriodId=@PeriodId
AND BranchId=@BranchId
AND isnull(SourcePaidQuantity,0) = 0
and ISNULL(IsLeader,'NA') IN ('NA','Y')
and ProductType is null

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'4' NoteNo,'3' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName 
,'StandardVAT'Remarks  from BureauSalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('VAT')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender'
,'Trading' ,'TradingTender','InternalIssue','Service','TollSale')
and PeriodId=@PeriodId
AND BranchId=@BranchId


union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'4' NoteNo,'4' SubNoteNo,ROUND( sum(SubTotal-LeaderAmount),2),  ROUND( sum(SDAmount),2),ROUND( sum(VATAmount-LeaderVATAmount),2),'Sub-Form'SubFormName 
,'StandardVAT'Remarks  from SalesInvoiceDetails where  1=1  and (post=@post1 or post=@post2) and Type in('VAT')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')--,'TollFinishIssue'
and PeriodId=@PeriodId
AND BranchId=@BranchId
AND isnull(SourcePaidQuantity,0) = 0
and ISNULL(IsLeader,'NA') ='N'
and ProductType is null

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'4' NoteNo,'5' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName 
,'StandardVAT'Remarks  from SalesInvoiceDetails where  1=1 and Type in('VAT')
and SalesInvoiceNo in(
select SalesInvoiceNo from Toll6_3InvoiceDetails where TollNo in(
select TollNo from Toll6_3Invoices where 1=1 
and  (post=@post1 or post=@post2) 
and TransactionType in('Contractor63')
and PeriodId=@PeriodId
AND BranchId=@BranchId
)
)

";
                    #endregion

                }

                #endregion

                #region Note-05

                sqlText = sqlText + @"
-------------------------------------------------- Note-05 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s  (BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'5' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'Sub-Form'SubFormName ,'MRPRate'Remarks 

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'5' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName 
,'MRPRate'Remarks  from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('MRPRate','MRPRate(SC)')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
and PeriodId=@PeriodId
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'5' NoteNo,'2' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName 
,'MRPRate'Remarks  from BureauSalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('MRPRate','MRPRate(SC)')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
and PeriodId=@PeriodId
AND BranchId=@BranchId
";
                #endregion

                #region Note-06

                sqlText = sqlText + @"
-------------------------------------------------- Note-06 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'6' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'Sub-Form'SubFormName ,'FixedVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'6' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName 
,'FixedVAT'Remarks  from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('FixedVAT')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
and PeriodId=@PeriodId
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'6' NoteNo,'2' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName 
,'FixedVAT'Remarks  from BureauSalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('FixedVAT')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
and PeriodId=@PeriodId
AND BranchId=@BranchId
";
                #endregion

                #region Note-07

                sqlText = sqlText + @"
-------------------------------------------------- Note-07 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'7' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'Sub-Form'SubFormName ,'OtherRate'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'7' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName 
,'OtherRate'Remarks  from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('OtherRate')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','Tender','Trading'
,'ExportTrading','TradingTender','InternalIssue','TollSale','TollFinishIssue')
and PeriodId=@PeriodId
AND BranchId=@BranchId
 HAVING  (ROUND( sum(SubTotal),2)+ROUND( sum(SDAmount),2)+ROUND( sum(VATAmount),2))>0

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'7' NoteNo,'2' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName 
,'OtherRate'Remarks  from BureauSalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('OtherRate')
and TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading','TradingTender','InternalIssue','Service','TollSale','TollFinishIssue')
and PeriodId=@PeriodId
AND BranchId=@BranchId
 HAVING  (ROUND( sum(SubTotal),2)+ROUND( sum(SDAmount),2)+ROUND( sum(VATAmount),2))>0

--union all
--select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'7' NoteNo,'3' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName 
--,'OtherRate'Remarks  from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('OtherRate')
--and TransactionType in('ServiceNS','Service')
--AND BranchId=@BranchId
";
                #endregion

                #region Note-08

                sqlText = sqlText + @"
-------------------------------------------------- Note-08 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'8' NoteNo,'0' SubNoteNo,0 SubTotal,0 SDAmount,0 VATAmount,'Sub-Form'SubFormName ,'Retail'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'8' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName 
,'Retail'Remarks  from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Retail')
and TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
and PeriodId=@PeriodId
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'8' NoteNo,'2' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum(SDAmount),2),ROUND( sum(VATAmount),2),'Sub-Form'SubFormName 
,'Retail'Remarks  from BureauSalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Retail')
and TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
and PeriodId=@PeriodId
AND BranchId=@BranchId
";
                #endregion

                #region Note-09

                sqlText = sqlText + @"
-------------------------------------------------- Note-09 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'9' NoteNo,'0' SubNoteNo,ROUND( sum( LineA),2)LineA,ROUND( sum( LineB),2)LineB,ROUND( sum( LineC),2)LineC,'-'SubFormName ,'SumSub3'Remarks 
from 
( 
	select LineA, LineB, LineC,NoteNo,PeriodID,BranchId from VATReturnV2s 
	union all 
	select 0 LineA, 0 LineB, isnull(VATAmount,0) LineC, '8' NoteNo, RebatePeriodID PeriodID,0 BranchId from  PurchaseInvoiceDetails where Section21 = 'Y' and BranchId=@BranchId

)VATReturnV2s where NoteNo in(1,2,3,4,5,6,7,8)
and PeriodID = @PeriodId and BranchId = @SelectBranchId 

";
                #endregion

                #endregion

                #region Purchase Data (Note-10 - Note-23)

                #region Note-10

                sqlText = sqlText + @"
-------------------------------------------------- Note-10 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------

insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'10' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'LocalPurchaseNonVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'10' NoteNo,'1' SubNoteNo
----------,ROUND( sum(SubTotal),2)
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)

,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseNonVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonVAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
AND RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all

select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'10' NoteNo,'1' SubNoteNo
,ROUND( sum(SubTotal),2)
----------,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseNonVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonVAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
AND RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')

/*
union all

select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'10' NoteNo,'2' SubNoteNo
,ROUND( -1*sum(SubTotal),2)
,ROUND( 1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),
ROUND( 1*sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseNonVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonVAT')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and PeriodId=@PeriodId
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
*/
";

                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {

                        sqlText = sqlText + @"
-------------------------------------------------- Note-10 Old Data -------------------------------------------
---------------------------------------------------------------------------------------------------------------

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'10' NoteNo,'1' SubNoteNo
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseNonVAT'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonVAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService','PurchaseTollcharge')

and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
AND RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all

select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'10' NoteNo,'2' SubNoteNo
,ROUND( -1*sum(SubTotal),2)
,ROUND( 1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),
ROUND( 1*sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseNonVAT'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonVAT')
and TransactionType in('PurchaseDN')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId

";
                    }
                }

                #endregion

                #endregion

                #region Note-11

                sqlText = sqlText + @"
-------------------------------------------------- Note-11 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'11' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'ImportPurchaseNonVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'11' NoteNo,'1' SubNoteNo
,ROUND( sum(SubTotal+CDAmount+RDAmount+TVBAmount),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,sum(SDAmount)
,'Sub-Form'SubFormName,'ImportPurchaseNonVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonVAT')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'

AND BranchId=@BranchId
";

                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {
                        sqlText = sqlText + @"
-------------------------------------------------- Note-11 Old data----------------------------------------------------
---------------------------------------------------------------------------------------------------------------

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'11' NoteNo,'1' SubNoteNo
,ROUND( sum(SubTotal+CDAmount+RDAmount+TVBAmount),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,sum(SDAmount)
,'Sub-Form'SubFormName,'ImportPurchaseNonVAT'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonVAT')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
";
                    }
                }

                #endregion

                #endregion

                #region Note-12

                sqlText = sqlText + @"
-------------------------------------------------- Note-12 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'12' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'LocalPurchaseExempted'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'12' NoteNo,'1' SubNoteNo
----------,ROUND( sum(SubTotal),2)
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,0 --ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseExempted'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Exempted')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'12' NoteNo,'1' SubNoteNo

,ROUND( sum(SubTotal),2)
----------,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,0 --ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)


,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseExempted'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Exempted')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'12' NoteNo,'2' SubNoteNo
,ROUND( -1*sum(SubTotal),2)
,0 --ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( -1*sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseExempted'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Exempted')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
*/

";
                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {
                        sqlText = sqlText + @"
-------------------------------------------------- Note-12 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'12' NoteNo,'1' SubNoteNo
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,0 --ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseExempted'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Exempted')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'12' NoteNo,'2' SubNoteNo
,ROUND( -1*sum(SubTotal),2)
,0 --ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( -1*sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseExempted'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Exempted')
and TransactionType in('PurchaseDN')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId

";
                    }
                }

                #endregion

                #endregion

                #region Note-13

                sqlText = sqlText + @"
-------------------------------------------------- Note-13 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'13' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'ImportPurchaseExempted'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'13' NoteNo,'1' SubNoteNo
,ROUND( sum(SubTotal+CDAmount+RDAmount+TVBAmount),2)
,0 --ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,sum(SDAmount)
,'Sub-Form'SubFormName,'ImportPurchaseExempted'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Exempted')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'

AND BranchId=@BranchId
";

                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {

                        sqlText = sqlText + @"
-------------------------------------------------- Note-13 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'13' NoteNo,'1' SubNoteNo
,ROUND( sum(SubTotal+CDAmount+RDAmount+TVBAmount),2)
,0 --ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,sum(SDAmount)
,'Sub-Form'SubFormName,'ImportPurchaseExempted'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Exempted')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
and RebatePeriodID=@PeriodId
and IsRebate='Y'

AND BranchId=@BranchId

";

                    }
                }

                #endregion

                #endregion

                #region Note-14

                sqlText = sqlText + @"
-------------------------------------------------- Note-14 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'14' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'LocalPurchaseStandardVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'14' NoteNo,'1' SubNoteNo
----------,ROUND( sum(SubTotal),2)
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)@Note14SDAmount
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)

,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseStandardVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('VAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService','PurchaseTollcharge' @ClientFGReceiveIn9_1)--,'TollReceive'
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all

select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'14' NoteNo,'1' SubNoteNo
,ROUND( sum(SubTotal),2)
----------,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)@Note14SDAmount
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)

,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseStandardVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('VAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService','PurchaseTollcharge' @ClientFGReceiveIn9_1)--,'TollReceive'
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'14' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseStandardVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('VAT')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
*/
";

                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {

                        sqlText = sqlText + @"
-------------------------------------------------- Note-14 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'14' NoteNo,'1' SubNoteNo
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)@Note14SDAmount
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseStandardVAT'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('VAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService','PurchaseTollcharge','TollReceive')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

";

                    }
                }

                #endregion


                if (false)
                {
                    string cValue = @"+ROUND( sum(SDAmount),2)";

                    sqlText = sqlText.Replace("@Note14SDAmount", cValue);
                }
                else
                {
                    sqlText = sqlText.Replace("@Note14SDAmount", "");
                }

                if (ClientFGReceiveIn9_1 == "Y")
                {
                    sqlText = sqlText.Replace("@ClientFGReceiveIn9_1", ",'TollReceive'");
                }
                else
                {
                    sqlText = sqlText.Replace("@ClientFGReceiveIn9_1", "");
                }

                #region Client 6.3

                sqlText = sqlText + @"
union all

select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'14' NoteNo,'3' SubNoteNo
,ROUND(SUM(ISNULL(Subtotal,0)),2)
,ROUND(SUM(ISNULL(VATAmount,0)),2)
,ROUND(SUM(ISNULL(SDAmount,0)),2)
,'Sub-Form'SubFormName,'LocalPurchaseStandardVAT'Remarks  
from Client6_3Details where  1=1 and  (post=@post1 or post=@post2)
and TransactionType in('Other')
and PeriodId=@PeriodId
AND BranchId=@BranchId
";

                #endregion

                #endregion

                #region Note-15

                sqlText = sqlText + @"
-------------------------------------------------- Note-15 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'15' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'ImportPurchaseStandardVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'15' NoteNo,'1' SubNoteNo
,ROUND( sum(SubTotal+CDAmount+RDAmount+TVBAmount),2)@Note15SDAmount
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,sum(SDAmount),'Sub-Form'SubFormName,'ImportPurchaseStandardVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('VAT')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'

AND BranchId=@BranchId
";


                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {
                        sqlText = sqlText + @"
-------------------------------------------------- Note-15 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'15' NoteNo,'1' SubNoteNo
,ROUND( sum(SubTotal+CDAmount+RDAmount+TVBAmount),2)@Note15SDAmount
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,sum(SDAmount),'Sub-Form'SubFormName,'ImportPurchaseStandardVAT'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('VAT')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId

";

                    }
                }

                #endregion


                if (periodDate >= Convert.ToDateTime("2021-07-01 00:00:00"))
                {
                    string cValue = @"+sum(SDAmount)";

                    sqlText = sqlText.Replace("@Note15SDAmount", cValue);
                }
                else
                {
                    sqlText = sqlText.Replace("@Note15SDAmount", "");
                }

                #region Backup Note-15 (Dec-15-2020); Changes also applied in Note-11, Note-13, Note-17, Note-22

                ////                sqlText = sqlText + @"
                ////-------------------------------------------------- Note-15 ----------------------------------------------------
                ////---------------------------------------------------------------------------------------------------------------
                ////
                ////
                ////insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
                ////select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'15' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'ImportPurchaseStandardVAT'Remarks 
                ////union all
                ////select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'15' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+CDAmount+RDAmount+SDAmount+TVBAmount),2),ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),sum(0),'Sub-Form'SubFormName,'ImportPurchaseStandardVAT'Remarks  
                ////from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('VAT')
                ////and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
                ////and PeriodId=@PeriodId
                ////AND BranchId=@BranchId
                ////";

                #endregion

                #endregion

                #region Note-16

                sqlText = sqlText + @"
-------------------------------------------------- Note-16 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'16' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'LocalPurchaseOtherRate'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'16' NoteNo,'1' SubNoteNo
----------,ROUND( sum(SubTotal),2)
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseOtherRate'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('OtherRate')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')


union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'16' NoteNo,'1' SubNoteNo
,ROUND( sum(SubTotal),2)
----------,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseOtherRate'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('OtherRate')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'16' NoteNo,'2' SubNoteNo
----------,ROUND( -1*sum(SubTotal),2)
,ROUND( -1*sum(case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end),2)
,ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( -1*sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseOtherRate'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('OtherRate')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
*/
";

                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {

                        sqlText = sqlText + @"
-------------------------------------------------- Note-16 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'16' NoteNo,'1' SubNoteNo
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseOtherRate'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('OtherRate')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'16' NoteNo,'2' SubNoteNo
,ROUND( -1*sum(case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end),2)
,ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( -1*sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseOtherRate'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('OtherRate')
and TransactionType in('PurchaseDN')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'

AND BranchId=@BranchId

";

                    }
                }

                #endregion

                #region Backup Note-16 (Dec-15-2020); Changes Also Applied in Note-10 - Note-20

                ////                sqlText = sqlText + @"
                ////-------------------------------------------------- Note-16 ----------------------------------------------------
                ////---------------------------------------------------------------------------------------------------------------
                ////
                ////
                ////insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
                ////select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'16' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'LocalPurchaseOtherRate'Remarks 
                ////union all
                ////select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'16' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),ROUND( sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseOtherRate'Remarks  
                ////from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('OtherRate')
                ////and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
                ////and PeriodId=@PeriodId
                ////AND BranchId=@BranchId
                ////
                ////union all
                ////select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'16' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseOtherRate'Remarks  
                ////from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('OtherRate')
                ////and TransactionType in('PurchaseDN','PurchaseReturn')
                ////and PeriodId=@PeriodId
                ////AND BranchId=@BranchId
                ////
                ////";
                #endregion

                #endregion

                #region Note-17

                sqlText = sqlText + @"
-------------------------------------------------- Note-17 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'17' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'ImportPurchaseOtherRate'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'17' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+CDAmount+RDAmount+TVBAmount),2),ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),sum(SDAmount),'Sub-Form'SubFormName,'ImportPurchaseOtherRate'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('OtherRate')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (
select PeriodID from FiscalYear
where PeriodStart < '2021-07-01'
)

union all

select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'17' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+CDAmount+RDAmount+TVBAmount+SDAmount),2),ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),sum(SDAmount),'Sub-Form'SubFormName,'ImportPurchaseOtherRate'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('OtherRate')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (
select PeriodID from FiscalYear
where PeriodStart >= '2021-07-01'
)

";

                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {

                        sqlText = sqlText + @"

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'17' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+CDAmount+RDAmount+TVBAmount),2),ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),sum(SDAmount),'Sub-Form'SubFormName,'ImportPurchaseOtherRate'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('OtherRate')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (
select PeriodID from FiscalYear
where PeriodStart < '2021-07-01'
)

";

                    }
                }

                #endregion

                #endregion

                #region Note-18

                sqlText = sqlText + @"
-------------------------------------------------- Note-18 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'LocalPurchaseFixedVAT'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'1' SubNoteNo
----------,ROUND( sum(SubTotal),2)
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)

,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseFixedVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('FixedVAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all

select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'2' SubNoteNo
,ROUND( sum(SubTotal),2)
----------,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)

,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseFixedVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('FixedVAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'3' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseFixedVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('FixedVAT')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId

union all

select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'4' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseFixedVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('FixedVAT(Rebate)')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
*/
union all

select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'5' SubNoteNo
----------,ROUND( sum(SubTotal),2)
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)

,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseFixedVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('FixedVAT(Rebate)')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')


union all

select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'6' SubNoteNo
,ROUND( sum(SubTotal),2)
----------,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)

,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseFixedVAT'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('FixedVAT(Rebate)')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')
";

                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {

                        sqlText = sqlText + @"

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'1' SubNoteNo
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseFixedVAT'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('FixedVAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'3' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseFixedVAT'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('FixedVAT')
and TransactionType in('PurchaseDN')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'4' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseFixedVAT'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('FixedVAT(Rebate)')
and TransactionType in('PurchaseDN')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'18' NoteNo,'5' SubNoteNo
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseFixedVAT'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('FixedVAT(Rebate)')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

";

                    }
                }

                #endregion


                #endregion

                #region Note-19

                sqlText = sqlText + @"
-------------------------------------------------- Note-19 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'19' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'LocalPurchaseTurnover'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'19' NoteNo,'1' SubNoteNo
----------,ROUND( sum(SubTotal),2)
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)

,'Sub-Form'SubFormName,'LocalPurchaseTurnover'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Turnover')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'19' NoteNo,'1' SubNoteNo
,ROUND( sum(SubTotal),2)
----------,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)

,'Sub-Form'SubFormName,'LocalPurchaseTurnover'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Turnover')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'19' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseTurnover'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Turnover')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
*/
";

                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {

                        sqlText = sqlText + @"

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'19' NoteNo,'1' SubNoteNo
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseTurnover'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Turnover')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'19' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseTurnover'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('Turnover')
and TransactionType in('PurchaseDN')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId

";

                    }
                }

                #endregion

                #endregion

                #region Note-20

                sqlText = sqlText + @"
-------------------------------------------------- Note-20 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'20' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'LocalPurchaseUnRegisterVendor'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'20' NoteNo,'1' SubNoteNo

----------,ROUND( sum(SubTotal),2)
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)

,'Sub-Form'SubFormName,'LocalPurchaseUnRegisterVendor'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('UnRegister')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'20' NoteNo,'1' SubNoteNo
,ROUND( sum(SubTotal),2)
----------,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)

,'Sub-Form'SubFormName,'LocalPurchaseUnRegisterVendor'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('UnRegister')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'20' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseUnRegisterVendor'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('UnRegister')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
*/
";

                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {

                        sqlText = sqlText + @"

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'20' NoteNo,'1' SubNoteNo
,ROUND( sum( case when RebateAmount>0 then (SubTotal*RebateRate/100) else  SubTotal end ),2)
,ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2)
,ROUND( sum(SDAmount),2)
,'Sub-Form'SubFormName,'LocalPurchaseUnRegisterVendor'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('UnRegister')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'20' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseUnRegisterVendor'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('UnRegister')
and TransactionType in('PurchaseDN')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId

";

                    }
                }

                #endregion


                #endregion

                #region Note-21

                sqlText = sqlText + @"
-------------------------------------------------- Note-21 ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------

insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'LocalPurchaseNonRebate'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2)+ROUND( sum(SDAmount),2), 0 VATAmount,ROUND( sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseNonRebate'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'

AND BranchId=@BranchId

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2), 0 VATAmount,ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseNonRebate'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'3' SubNoteNo
,ROUND( sum(Subtotal- (SubTotal*RebateRate/100)),2) RebateSubtotal
,ROUND( sum( VATAmount- RebateAmount),2) RebateVATAmount
,0 SDAmount
,'Sub-Form'SubFormName,'LocalPurchaseNonRebate'Remarks  
from PurchaseInvoiceDetails 
where  1=1 and  (post=@post1 or post=@post2)  
and Type not in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType not in('PurchaseDN','PurchaseReturn')
and PeriodId=@PeriodId
AND BranchId=@BranchId
and RebateAmount>0
and PeriodId in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')
*/

";

                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {

                        sqlText = sqlText + @"

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2), 0 VATAmount,ROUND( sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseNonRebate'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2), 0 VATAmount,ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseNonRebate'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('PurchaseDN')
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId

/*
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'3' SubNoteNo
,ROUND( sum(Subtotal- (SubTotal*RebateRate/100)),2) RebateSubtotal
,ROUND( sum( VATAmount- RebateAmount),2) RebateVATAmount
,0 SDAmount
,'Sub-Form'SubFormName,'LocalPurchaseNonRebate'Remarks  
from @db.dbo.PurchaseInvoiceDetails 
where  1=1 and  (post=@post1 or post=@post2)  
and Type not in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType not in('PurchaseDN','PurchaseReturn')
and PeriodId=@PeriodId
AND BranchId=@BranchId
and RebateAmount>0
and PeriodId in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')
*/
";

                    }
                }

                #endregion

                #region Backup Note-21 (Dec-15-2020)

                ////                sqlText = sqlText + @"
                ////-------------------------------------------------- Note-21 ----------------------------------------------------
                ////---------------------------------------------------------------------------------------------------------------
                ////
                ////insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
                ////select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'LocalPurchaseNonRebate'Remarks 
                ////union all
                ////select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2), 0 VATAmount,ROUND( sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseNonRebate'Remarks  
                ////from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
                ////and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
                ////and PeriodId=@PeriodId
                ////AND BranchId=@BranchId
                ////
                ////union all
                ////select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2), 0 VATAmount,ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseNonRebate'Remarks  
                ////from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
                ////and TransactionType in('PurchaseDN','PurchaseReturn')
                ////and PeriodId=@PeriodId
                ////AND BranchId=@BranchId
                ////
                ////";

                #endregion

                #region Backup Note-21 (Nov-21-2020)

                //////                sqlText = sqlText + @"
                //////--------------------------------------------------Note 21--------------------------------------------------
                //////-----------------------------------------------------------------------------------------------------------
                //////
                //////insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
                //////select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'LocalPurchaseNonRebate'Remarks 
                //////union all
                //////select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal),2),ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),ROUND( sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseNonRebate'Remarks  
                //////from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
                //////and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
                //////and PeriodId=@PeriodId
                //////------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
                //////AND BranchId=@BranchId
                //////
                //////union all
                //////select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'21' NoteNo,'2' SubNoteNo,ROUND( -1*sum(SubTotal),2),ROUND( -1*sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),ROUND( -1*sum(SDAmount),2),'Sub-Form'SubFormName,'LocalPurchaseNonRebate'Remarks  
                //////from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
                //////and TransactionType in('PurchaseDN','PurchaseReturn')
                //////and PeriodId=@PeriodId
                //////------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
                //////AND BranchId=@BranchId
                //////
                //////";

                #endregion

                #endregion

                #region Note-22

                sqlText = sqlText + @"
-------------------------------------------------- Note-22 --------------------------------------------------
-------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'22' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'ImportPurchaseNonRebate'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'22' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+CDAmount+RDAmount+TVBAmount),2),  0 VATAmount,sum(SDAmount),'Sub-Form'SubFormName,'ImportPurchaseNonRebate'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (
select PeriodID from FiscalYear
where PeriodStart < '2021-07-01'
)

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'22' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+CDAmount+RDAmount+TVBAmount+SDAmount),2),  0 VATAmount,sum(SDAmount),'Sub-Form'SubFormName,'ImportPurchaseNonRebate'Remarks  
from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (
select PeriodID from FiscalYear
where PeriodStart >= '2021-07-01'
)

";

                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {

                        sqlText = sqlText + @"

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'22' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+CDAmount+RDAmount+TVBAmount),2),  0 VATAmount,sum(SDAmount),'Sub-Form'SubFormName,'ImportPurchaseNonRebate'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter')
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
and RebatePeriodID in (
select PeriodID from FiscalYear
where PeriodStart < '2021-07-01'
)

";

                    }
                }

                #endregion

                #region Backup Note-22 (Nov-21-2020)

                ////                sqlText = sqlText + @"
                ////
                ////--------------------------------------------------Note 22--------------------------------------------------
                ////-----------------------------------------------------------------------------------------------------------
                ////
                ////
                ////insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
                ////select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'22' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'ImportPurchaseNonRebate'Remarks 
                ////union all
                ////select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'22' NoteNo,'1' SubNoteNo,ROUND( sum(SubTotal+CDAmount+RDAmount+SDAmount+TVBAmount),2),ROUND( sum( case when RebateAmount>0 then RebateAmount else  VATAmount end ),2),sum(0),'Sub-Form'SubFormName,'ImportPurchaseNonRebate'Remarks  
                ////from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
                ////and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
                ////and PeriodId=@PeriodId
                ////------and ReceiveDate >= @Datefrom and ReceiveDate <dateadd(d,1,@Dateto)
                ////AND BranchId=@BranchId
                ////
                ////";

                #endregion

                #endregion

                #region Note-23

                sqlText = sqlText + @"
-------------------------------------------------- Note-23 --------------------------------------------------
-------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'23' NoteNo,'0' SubNoteNo,ROUND( sum( LineA),2)LineA,ROUND( sum( LineB),2)LineB,ROUND( sum( LineC),2)LineC,'-'SubFormName ,'SumSub4'Remarks
from (
select @BranchId BranchId,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'23' NoteNo,'0' SubNoteNo,ROUND( sum( LineA),2)LineA,ROUND( sum(LineB),2) LineB, 0 LineC,'-'SubFormName ,'SumSub4'Remarks 
from VATReturnV2s where NoteNo in(10,11,12,13)

and PeriodID = @PeriodId and BranchId = @SelectBranchId 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'23' NoteNo,'1' SubNoteNo,ROUND( sum(LineA),2)LineA,ROUND( sum(LineB),2) LineB,0 LineC,'-'SubFormName ,'SumSub4'Remarks 
from VATReturnV2s where NoteNo in(14,15,16,17)
and PeriodID = @PeriodId and BranchId = @SelectBranchId 

union all

select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'23' NoteNo,'2' SubNoteNo,ROUND( sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'SumSub4'Remarks 
from VATReturnV2s where NoteNo in(19,20,21,22)
and PeriodID = @PeriodId and BranchId = @SelectBranchId 

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'23' NoteNo,'3' SubNoteNo,ROUND( sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'SumSub4'Remarks 
from VATReturnV2s where NoteNo in(18) and SubNoteNo not in ('4','5','6')
and PeriodID = @PeriodId and BranchId = @SelectBranchId 

union all

select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'23' NoteNo,'4' SubNoteNo,ROUND( sum(LineA),2)LineA,ROUND( sum(LineB),2) LineB,0 LineC,'-'SubFormName ,'SumSub4'Remarks 
from VATReturnV2s where NoteNo in(18) and SubNoteNo in   ('4','5','6')
and PeriodID = @PeriodId and BranchId = @SelectBranchId 

) as a
";

                #endregion

                #endregion

                #region Note-24

                sqlText = sqlText + @"

-------------------------------------------------- Note-24 --------------------------------------------------
-------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'24' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'PurcshaseVDS'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'24' NoteNo,'1' SubNoteNo,ROUND( sum(BillDeductAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'PurcshaseVDS'Remarks  
from VDS where  1=1 and  (post=@post1 or post=@post2)    
and IsPurchase in('Y'  )
and PeriodId=@PeriodId
AND BranchId=@BranchId

";

                #endregion

                #region Note-25

                sqlText = sqlText + @"

-------------------------------------------------- Note-25 --------------------------------------------------
-------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'25' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'AdjustmentWithoutBankPay'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'25' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'AdjustmentWithoutBankPay'Remarks  
from Deposits where  1=1 and  (post=@post1 or post=@post2)    
and TransactionType in('WithoutBankPay')
and PeriodId=@PeriodId
AND BranchId=@BranchId

";

                #endregion

                #region Note-26

                sqlText = sqlText + @"

-------------------------------------------------- Note-26 --------------------------------------------------
-------------------------------------------------------------------------------------------------------------


insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'26' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'SaleDebitNote'Remarks 
union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'26' NoteNo,'1' SubNoteNo,ROUND(sum(VATAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'SaleDebitNote'Remarks 
from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)    
and TransactionType in('Debit')
and PeriodId=@PeriodId
AND BranchId=@BranchId
AND isnull(SourcePaidQuantity,0)=0
and isnull(ProductType,'R') in ('P','R')

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'26' NoteNo,'2' SubNoteNo,ROUND(sum(VATAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'SaleDebitNote'Remarks 
from BureauSalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)    
and TransactionType in('Debit')
and PeriodId=@PeriodId
AND BranchId=@BranchId

union all

select  @BranchId,@PeriodId, @DateFrom, @UserName UserName,@Branch Branch,'26' NoteNo,'3' SubNoteNo,ROUND(sum(sid.VATAmount),2), 0 VATAmount,0 SDAmount
,'Sub-Form'SubFormName,'PurchaseDebitNote' Remarks

from PurchaseInvoiceDetails sid
--left outer join PurchaseInvoiceDetails sidp on sid.PurchaseReturnId=sidp.PurchaseInvoiceNo
left outer join PurchaseInvoiceDetails sidp on sid.PurchaseReturnId=sidp.PurchaseInvoiceNo and sid.ItemNo=sidp.ItemNo

where 1=1 
 and  (sid.post=@post1 or sid.post=@post2)  
 and sid.PeriodId=@PeriodId
AND sid.BranchId=@BranchId
and sid.TransactionType in('PurchaseReturn')


    ";

                #endregion

                #endregion

                #region Note: 27

                sqlText = sqlText + @"
--------insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
--------select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'27' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'IncreasingAdjustment'Remarks 
--------union all
--------select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'27' NoteNo,'1' SubNoteNo,ROUND(sum(AdjAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'IncreasingAdjustment'Remarks  
--------from AdjustmentHistorys where  1=1 and  (post=@post1 or post=@post2)    
--------and AdjType in('IncreasingAdjustment')
--------and AdjDate >= @Datefrom and AdjDate <dateadd(d,1,@Dateto)
--------AND BranchId=@BranchId
";
                #region Increasing Adjustment

                sqlText = sqlText + @"
-------------------------------------------------- Increasing Adjustment --------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------
SELECT  *
INTO #TempAdjustmentHistorys 

FROM 
(
SELECT '27' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'IncreasingAdjustment'Remarks 
, '' AdjType, '' AdjName
UNION ALL
SELECT '27' NoteNo,'1' SubNoteNo,ROUND(sum(AdjAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'IncreasingAdjustment'Remarks  
, ah.AdjType, an.AdjName
FROM AdjustmentHistorys ah
LEFT OUTER JOIN AdjustmentName an ON ah.AdjId=an.AdjId
WHERE  1=1 and  (post=@post1 or post=@post2)    
AND ah.AdjType in('IncreasingAdjustment')
and ah.PeriodId=@PeriodId
AND ah.BranchId=@BranchId
AND ah.IsAdjSD='N'
GROUP BY ah.AdjType, an.AdjName
UNION ALL

SELECT  '27' NoteNo,'2' SubNoteNo
,ddbd.ClaimVAT Subtotal
, 0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'IncreasingAdjustment' Remarks
,ddbd.TransactionType AdjType 
,ddbd.TransactionType AdjName  
FROM DutyDrawBackDetails ddbd
LEFT OUTER JOIN PurchaseInvoiceDetails pid ON pid.PurchaseInvoiceNo=ddbd.PurchaseInvoiceNo
WHERE 1=1 AND ddbd.BranchId=@BranchId
AND (ddbd.post=@post1 or ddbd.post=@post2) 
--and ddbd.PeriodId=@PeriodId
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'

AND ISNULL(ddbd.TransactionType,'DDB') = 'VDB'
AND pid.Type IN('VAT','FixedVAT')


UNION ALL
SELECT '27' NoteNo,'3' SubNoteNo
, CASE
WHEN @AutoPartialRebateProcess='N' THEN 0 
WHEN @Line9Subtotal > 0 THEN ISNULL(ROUND((SUM(LineA)*@Line23VAT/@Line9Subtotal),2),0) 
ELSE 0 END AS Subtotal
, 0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'IncreasingAdjustment'Remarks  
,'IncreasingAdjustment' AdjType, 'Exempted Goods/Service (Rebate Cancel)' AdjName
FROM VATReturnV2s
WHERE  1=1
AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 3

UNION ALL
SELECT '27' NoteNo,'4' SubNoteNo
, CASE 
WHEN @AutoPartialRebateProcess='N' THEN 0 
WHEN @Line9Subtotal > 0 THEN ISNULL(ROUND((SUM(LineA)*@Line23VAT/@Line9Subtotal),2),0) 
ELSE 0 END AS Subtotal
, 0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'IncreasingAdjustment'Remarks  
,'IncreasingAdjustment' AdjType, 'Other Rated VAT (Rebate Cancel)' AdjName
FROM VATReturnV2s
WHERE  1=1
AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 7 AND SubNoteNo IN(1,2)



union all

select '27' NoteNo,'5' SubNoteNo,ROUND( sum(NonLeaderVATAmount),2), 0,0,'Sub-Form'SubFormName 
,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Non-Leader VAT Amount' AdjName 
from SalesInvoiceDetails where  1=1   and Type in('VAT')
and TransactionType in('ServiceNS','Service','TollFinishIssue','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
AND BranchId=@BranchId
and PeriodId=@PeriodId
and ISNULL(IsLeader,'NA') = 'Y'


--union all
--
--select '27' NoteNo,'6' SubNoteNo,ROUND(sum(SourcePaidVATAmount),2), 0,0,'Sub-Form'SubFormName 
--,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Source Paid VAT Amount' AdjName 
--from SalesInvoiceDetails where  1=1   and Type in('VAT')
--and TransactionType in('Credit')
--AND BranchId=@BranchId
--AND PeriodId=@PeriodId
--and ISNULL(SourcePaidVATAmount,0) > 0


union all

select '27' NoteNo,'7' SubNoteNo,ROUND(sum(ISNULL(VATAmount,0)),2), 0,0,'Sub-Form'SubFormName 
,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Raw Material Dispose' AdjName 
from DisposeRawDetails where  1=1   
and TransactionType in('Other')
AND BranchId=@BranchId
AND PeriodId=@PeriodId


union all

select '27' NoteNo,'8' SubNoteNo,ROUND(sum(ISNULL(RebateVATAmount,0)),2), 0,0,'Sub-Form'SubFormName 
,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Raw Material for Finish Dispose' AdjName 
from DisposeFinishDetails where  1=1  
and TransactionType in('Other','DisposeTrading')
AND BranchId=@BranchId
AND PeriodId=@PeriodId

/*
union all
select '27' NoteNo,'9' SubNoteNo,ROUND(sum(ISNULL(VATAmount,0)+ISNULL(ATVAmount,0)),2),0,0,'Sub-Form'SubFormName 
,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Purchase Credit' AdjName 
from PurchaseInvoiceDetails where  1=1  
and TransactionType in('PurchaseReturn')
AND BranchId=@BranchId
AND PeriodId=@PeriodId
*/
--union all
--
--select '27' NoteNo,'10' SubNoteNo,ROUND(sum(ISNULL(VATAmount,0)),2),0,0,'Sub-Form'SubFormName 
--,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Exempted VAT' AdjName 
--from SalesInvoiceDetails where  1=1  
--and TransactionType in ('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
--and Type in ('NonVAT')
--AND BranchId=@BranchId
--AND PeriodId=@PeriodId
--AND isnull(SourcePaidQuantity,0)=0
----AND isnull(Channel,'-') != 'Retail'
--AND Option2 is null

union all

select '27' NoteNo,'10' SubNoteNo,ROUND(sum(ISNULL(Option2,0)),2),0,0,'Sub-Form'SubFormName 
,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Exempted VAT' AdjName 
from SalesInvoiceDetails where  1=1  
and TransactionType in (
'ServiceNS','Service'
,'Other','RawSale','PackageSale','Wastage'
, 'SaleWastage', 'CommercialImporter'
,'ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
and Type in ('OtherRate','NonVAT','Retail')
AND BranchId=@BranchId
AND PeriodId=@PeriodId
AND isnull(SourcePaidQuantity,0)=0
--AND isnull(Channel,'-') = 'Retail'
AND exists (
select SettingValue from settings
where SettingGroup = 'CompanyCode'
and SettingName = 'Code'
and SettingValue = 'Bata'
)

) AS adj

INSERT INTO VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
SELECT @BranchId,@PeriodId, @DateFrom,@UserName,@Branch, '27' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName 
,'IncreasingAdjustment'Remarks 
UNION ALL

SELECT @BranchId,@PeriodId, @DateFrom,@UserName,@Branch, NoteNo, SubNoteNo, Subtotal, VATAmount, SDAmount, SubFormName, Remarks
FROM #TempAdjustmentHistorys
where SubTotal>0



    ";
                #endregion

                #endregion

                #region Note: 28-30

                sqlText = sqlText + @"

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'28' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-'SubFormName ,'SumSub5'Remarks 
    from VATReturnV2s where NoteNo in(24,25,26,27)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'29' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'SaleVDS'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'29' NoteNo,'1' SubNoteNo,ROUND(sum(BillDeductAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'SaleVDS'Remarks  
    from VDS where  1=1 and  (post=@post1 or post=@post2)    
    and IsPurchase in('N' )
    and PeriodId=@PeriodId
    AND BranchId=@BranchId


    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'30' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'PurchaseAT'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'30' NoteNo,'1' SubNoteNo
    ,case when @ATVRebate='Y' then ROUND(sum(ATVAmount),2) else 0 end ,sum(0),sum(0),'Sub-Form'SubFormName,'PurchaseAT'Remarks  
    from PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)   
    and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
    --and PeriodId=@PeriodId
    --and RebatePeriodID=@PeriodId
    and isnull(RebatePeriodID,PeriodId)=@PeriodId
    and IsRebate='Y'

    AND BranchId=@BranchId

    ";

                #region Old Data

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {

                        sqlText = sqlText + @"

union all
select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'30' NoteNo,'1' SubNoteNo
,case when @ATVRebate='Y' then ROUND(sum(ATVAmount),2) else 0 end ,sum(0),sum(0),'Sub-Form'SubFormName,'PurchaseAT'Remarks  
from @db.dbo.PurchaseInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)   
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId

";

                    }
                }

                #endregion

                #endregion

                #region Note: 31

                sqlText = sqlText + @"

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'31' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'SaleCreditNote'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'31' NoteNo,'1' SubNoteNo,ROUND(sum(VATAmount+isnull(option2,0)),2) SubTotal, 0 VATAmount
    ,0 SDAmount,'Sub-Form'SubFormName,'SaleCreditNote'Remarks 
    from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('Credit','RawCredit')
   -- and Type in('VAT','OtherRate')
    and PeriodId=@PeriodId 
    AND BranchId=@BranchId
    AND isnull(SourcePaidQuantity,0)=0
    and isnull(ProductType,'R') in ('P','R')
    
    union all
    --select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'31' NoteNo,'2' SubNoteNo,ROUND(sum(VATAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'SaleCreditNote'Remarks 
    select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'31' NoteNo,'2' SubNoteNo,ROUND(sum(VATAmount),2)SubTotal, ROUND(sum(VATAmount),2) VATAmount
    ,0 SDAmount,'Sub-Form'SubFormName,'SaleCreditNote'Remarks 
    from BureauSalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('Credit','ExportServiceNSCredit')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId


    ";
                #endregion

                #region Note: 32 / Decreasing Adjustment

                sqlText = sqlText + @"


    SELECT  *
    INTO #TempDecrementAdjustmentHistorys 

    FROM
    (
    SELECT '32' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'DecreasingAdjustment'Remarks
    , '' AdjType, '' AdjName 
    UNION ALL
    SELECT '32' NoteNo,'1' SubNoteNo,ROUND(sum(AdjAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'DecreasingAdjustment'Remarks  
    , ah.AdjType, an.AdjName
    FROM AdjustmentHistorys ah
    LEFT OUTER JOIN AdjustmentName an ON ah.AdjId=an.AdjId
    WHERE  1=1 and  (post=@post1 or post=@post2)    
    AND ah.AdjType in('DecreasingAdjustment')
    and ah.PeriodId=@PeriodId
    AND ah.BranchId=@BranchId
    AND ah.IsAdjSD='N'
    GROUP BY ah.AdjType, an.AdjName

    UNION ALL

    SELECT  '32' NoteNo,'2' SubNoteNo
    ,ddbd.ClaimVAT Subtotal
    , 0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'DecreasingAdjustment' Remarks
    ,'DecreasingAdjustment' AdjType 
    ,ddbd.TransactionType AdjName  
    FROM DutyDrawBackDetails ddbd
    LEFT OUTER JOIN PurchaseInvoiceDetails pid ON pid.PurchaseInvoiceNo=ddbd.PurchaseInvoiceNo
    WHERE 1=1 AND ddbd.BranchId=@BranchId
    AND  (ddbd.post=@post1 or ddbd.post=@post2) 
   -- and ddbd.PeriodId=@PeriodId
    and pid.RebatePeriodID=@PeriodId
    and pid.IsRebate='Y'

    AND ISNULL(ddbd.TransactionType,'DDB') = 'VDB'
    AND pid.Type='NonRebate'


    union all

    select '32' NoteNo,'3' SubNoteNo,ROUND(sum(NonLeaderVATAmount),2), 0,0,'Sub-Form'SubFormName 
    ,'DecreasingAdjustment'Remarks, 'DecreasingAdjustment' AdjType, 'Non-Leader VAT Amount' AdjName 
    from SalesInvoiceDetails where  1=1   and Type in('VAT')
    and TransactionType in('ServiceNS','Service','TollFinishIssue','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
    ,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
    AND BranchId=@BranchId
    and PeriodId=@PeriodId
    and ISNULL(IsLeader,'NA') = 'N'

--    union all
--
--    select '32' NoteNo,'4' SubNoteNo,ROUND(sum(VATAmount),2), 0,0,'Sub-Form'SubFormName 
--    ,'DecreasingAdjustment'Remarks, 'DecreasingAdjustment' AdjType, 'Rebate Cancel(Credit)' AdjName 
--    from SalesInvoiceDetails where  1=1   and Type in('NonVAT')
--    and TransactionType in('credit')
--    AND BranchId=@BranchId
--    and PeriodId=@PeriodId
--    and ISNULL(IsLeader,'NA') in ('Y','NA')
--    and EXISTS (select * from settings where settinggroup = 'CompanyCode' and SettingName = 'code'and SettingValue = 'Bata')

    --union all
    --
    --select '32' NoteNo,'4' SubNoteNo,ROUND(sum(SourcePaidVATAmount),2), 0,0,'Sub-Form'SubFormName 
    --,'DecreasingAdjustment'Remarks, 'DecreasingAdjustment' AdjType, 'Source Paid VAT Amount' AdjName 
    --from SalesInvoiceDetails where  1=1   and Type in('VAT')
    --and TransactionType in('ServiceNS','Service','TollFinishIssue','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
    --,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
    --AND BranchId=@BranchId
    --AND PeriodId=@PeriodId
    --and ISNULL(SourcePaidVATAmount,0) > 0
/*
union all

select '32' NoteNo,'5' SubNoteNo,ROUND(sum(VATAmount),2),0,0,'Sub-Form'SubFormName
,'DecreasingAdjustment'Remarks , 'DecreasingAdjustment' AdjType, 'DecreasingAdjustmentVATAmount' AdjName  
from PurchaseInvoiceDetails where  1=1  and Type in('VAT')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and PeriodId=@PeriodId
--and RebatePeriodID=@PeriodId
and isnull(RebatePeriodID,PeriodId)=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
*/

    ) decAdj

    INSERT INTO VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    SELECT @BranchId,@PeriodId, @DateFrom,@UserName,@Branch, NoteNo, SubNoteNo
    , Subtotal
    , VATAmount, SDAmount, SubFormName,  Remarks  
    FROM #TempDecrementAdjustmentHistorys


    ";
                #endregion

                #region Note: 33

                sqlText = sqlText + @"


    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'33' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-'SubFormName ,'SumSub6'Remarks 
    from VATReturnV2s where NoteNo in(29,30,31,32)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 


    ";
                #endregion

                #region Note: 38-49 [Excluding: 34, 35, 36, 36, 37]

                sqlText = sqlText + @"

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'38' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'SDSaleDebitNote'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'38' NoteNo,'1' SubNoteNo,ROUND(sum(SDAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SDSaleDebitNote'Remarks 
    from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('Debit')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId

    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'38' NoteNo,'2' SubNoteNo,ROUND(sum(SDAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SDSaleDebitNote'Remarks 
    from BureauSalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('Debit')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId

    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'38' NoteNo,'3' SubNoteNo,ROUND(sum(AdjAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'IncreasingAdjustment'Remarks 
    from AdjustmentHistorys where  1=1 and  (post=@post1 or post=@post2)    
    and AdjType in('IncreasingAdjustment')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId
    AND IsAdjSD='Y'


    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'39' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'SDSaleCreditNote'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'39' NoteNo,'1' SubNoteNo,ROUND(sum(SDAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SDSaleCreditNote'Remarks 
    from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('Credit')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId

    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'39' NoteNo,'2' SubNoteNo,ROUND(sum(SDAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SDSaleCreditNote'Remarks 
    from BureauSalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('Credit')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId

    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'39' NoteNo,'3' SubNoteNo,ROUND(sum(AdjAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'DecreasingAdjustment'Remarks 
    from AdjustmentHistorys where  1=1 and  (post=@post1 or post=@post2)    
    and AdjType in('DecreasingAdjustment')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId
    AND IsAdjSD='Y'

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'40' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'SDExportSale'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'40' NoteNo,'1' SubNoteNo,ROUND(sum(ApprovedSD),2),0 VATAmount,0 SDAmount,'-'SubFormName,'SDExportSale'Remarks 
    from DutyDrawBackHeader where  1=1 and  (post=@post1 or post=@post2)    
    and ISNULL(DutyDrawBackHeader.TransactionType,'DDB') = 'SDB'
    and PeriodId=@PeriodId
    AND BranchId=@BranchId


    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'41' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'InterestOnOveredVAT'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'41' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'InterestOnOveredVAT'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('InterestOnOveredVAT')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId


    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'42' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'InterestOnOveredSD'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'42' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'InterestOnOveredSD'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('InterestOnOveredSD')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId



    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'43' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'FinePenaltyForNonSubmissionOfReturn'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'43' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'FinePenaltyForNonSubmissionOfReturn'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('FinePenaltyForNonSubmissionOfReturn')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId


    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'44' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'FineOrPenalty'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'44' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'FineOrPenalty'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('FineOrPenalty')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId


    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'45' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'ExciseDuty'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'45' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'ExciseDuty'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('ExciseDuty')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId


    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'46' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'DevelopmentSurcharge'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'46' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'DevelopmentSurcharge'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('DevelopmentSurcharge')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId



    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'47' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'ICTDevelopmentSurcharge'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'47' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'ICTDevelopmentSurcharge'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('ICTDevelopmentSurcharge')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId


    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'48' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'HelthCareSurcharge'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom, @UserName,@Branch,'48' NoteNo,'1' SubNoteNo,ROUND(sum(HPSAmount),2),0,0,'-'SubFormName,'HelthCareSurcharge'Remarks 
    from SalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)  and Type in('MRPRate(SC)')
    and TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId



    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'49' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'EnvironmentProtectionSurcharge'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'49' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'EnvironmentProtectionSurcharge'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('EnvironmentProtectionSurcharge')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId

    ";
                #endregion

                #region Note: 52


                DateTime PeriodStart = Convert.ToDateTime(vm.PeriodName);

                DateTime HardDecember2019 = Convert.ToDateTime("December-2019");

                if (PeriodStart <= HardDecember2019)
                {
                    sqlText = sqlText + @"

    ---------------------------------Line52-----------------------------------------------
    --------------------------------------------------------------------------------------

    select @LastLine65=ISNULL(sum(LineA),0) FROM VATReturns WHERE NoteNo='62' and PeriodID=@PreviousPeriodID AND BranchId=@SelectBranchId;
    select @LastLine66=ISNULL(sum(LineA),0) FROM VATReturns WHERE NoteNo='63' and PeriodID=@PreviousPeriodID AND BranchId=@SelectBranchId;


    ";
                }
                else
                {

                    sqlText = sqlText + @"

    ---------------------------------Line52-----------------------------------------------
    --------------------------------------------------------------------------------------


    ----------select @PreviousPeriodID=format( DATEADD(M, -1,@DateFrom),'MMyyyy')
    select @LastLine65=ISNULL(sum(LineA),0) FROM VATReturnV2s WHERE NoteNo='65' and PeriodID=@PreviousPeriodID AND BranchId=@SelectBranchId;;
    select @LastLine66=ISNULL(sum(LineA),0) FROM VATReturnV2s WHERE NoteNo='66' and PeriodID=@PreviousPeriodID AND BranchId=@SelectBranchId;;



    ";
                }

                sqlText = sqlText + @"
    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)

    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'52' NoteNo,'1' SubNoteNo, sum(SubTotal)SubTotal,sum( VATAmount) VATAmount,sum( SDAmount)SDAmount ,'-'SubFormName ,'LastClosingVAT'Remarks
    from(

    select ISNULL(@LastLine65,0) SubTotal,0 VATAmount,0 SDAmount

    UNION ALL

    select ISNULL(DepositAmount,0) SubTotal,0 VATAmount,0 SDAmount

    from Deposits
    where 1=1
    AND  (post=@post1 or post=@post2)   
    and PeriodId=@PeriodId
    AND DepositType = 'Opening'
    AND BranchId=@BranchId
    --05Feb2023
	and TransactionType!='SD-Opening'
	------
    ) as a

    ";
                #endregion

                #region Note: 53

                sqlText = sqlText + @"

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)

    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'53' NoteNo,'1' SubNoteNo, sum(SubTotal)SubTotal,sum( VATAmount) VATAmount,sum( SDAmount)SDAmount ,'-'SubFormName ,'LastClosingSD'Remarks
    from(

    select ISNULL(@LastLine66,0) SubTotal,0 VATAmount,0 SDAmount
    
    --05Feb2023
    UNION ALL

    select ISNULL(DepositAmount,0) SubTotal,0 VATAmount,0 SDAmount

    from Deposits
    where 1=1
    AND  (post=@post1 or post=@post2)   
    and PeriodId=@PeriodId
    AND DepositType = 'Opening'
    AND BranchId>@BranchId
	and TransactionType='SD-Opening'
	-------

    ) as a
    ";
                #endregion

                #region Note: 54 / New (Under Development)

                sqlText = sqlText + @" 
    select @LastLine54=ISNULL(sum(LineA),0) FROM VATReturnV2s WHERE NoteNo='54' and PeriodID=@PreviousPeriodID AND BranchId=@SelectBranchId;
    select @LastLine56=ISNULL(sum(LineA),0) FROM VATReturnV2s WHERE NoteNo='56' and PeriodID=@PreviousPeriodID AND BranchId=@SelectBranchId;



    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)

    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'54' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'ClosingBalanceVAT(18.6)'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'54' NoteNo,'1' SubNoteNo
    ,ISNULL((ROUND(sum(DepositAmount),2)),0)+ISNULL(@LastLine54,0)-ISNULL(@LastLine56,0)
    ,0 VATAmount,0 SDAmount,'-'SubFormName,'ClosingBalanceVAT(18.6)'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('Treasury') and DepositType in('ClosingBalanceVAT(18.6)')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId


    ";
                #endregion

                #region Note: 55 / New (Under Development)


                sqlText = sqlText + @" 

    select @LastLine55=ISNULL(sum(LineA),0) FROM VATReturnV2s WHERE NoteNo='55' and PeriodID=@PreviousPeriodID AND BranchId=@SelectBranchId;
    select @LastLine57=ISNULL(sum(LineA),0) FROM VATReturnV2s WHERE NoteNo='57' and PeriodID=@PreviousPeriodID AND BranchId=@SelectBranchId;



    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'55' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'ClosingBalanceSD(18.6)'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'55' NoteNo,'1' SubNoteNo
    ,ISNULL((ROUND(sum(DepositAmount),2)),0)+ISNULL(@LastLine55,0)-ISNULL(@LastLine57,0)
    ,0 VATAmount,0 SDAmount,'-'SubFormName,'ClosingBalanceSD(18.6)'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('SD') and DepositType in('ClosingBalanceSD(18.6)')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId

    ";
                #endregion

                #region Note: 58-64

                sqlText = sqlText + @" 

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'58' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'TotalDepositVAT'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'58' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'TotalDepositVAT'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('Treasury') and DepositType NOT IN('ClosingBalanceVAT(18.6)') and DepositType NOT IN('RequestedAmountForRefundVAT')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId


    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'58' NoteNo,'2' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'TotalDepositVAT'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('VDS') and DepositType not in('NotDeposited')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId


    --union all
    --select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'58' NoteNo,'3' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'TotalDepositVAT'Remarks  
    --from Deposits where  1=1 and  (post=@post1 or post=@post2) 
    --and TransactionType
    --in(
    --'WithoutBankPay'
    --,'DevelopmentSurcharge'
    --,'EnvironmentProtectionSurcharge'
    --,'ExciseDuty'
    --,'FineOrPenalty'
    --,'HelthCareSurcharge'
    --,'ICTDevelopmentSurcharge'
    --------,'InterestOnOveredSD'
    --,'InterestOnOveredVAT'
    --,'FinePenaltyForNonSubmissionOfReturn'
    --)
    --and DepositType not in('NotDeposited')
    --and PeriodId=@PeriodId
    --AND BranchId=@BranchId


    ";

                sqlText = sqlText + @"
    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'59' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'TotalDepositSD'Remarks 

    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'59' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'TotalDepositSD'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('SD') and DepositType NOT IN('ClosingBalanceSD(18.6)') and DepositType NOT IN('RequestedAmountForRefundSD')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId

    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'59' NoteNo,'2' SubNoteNo,DepositAmount,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'TotalDepositSD'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2) 
    and TransactionType
    in(
    'InterestOnOveredSD'
    )
    and DepositType not in('NotDeposited')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId




    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'60' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'ExciseDutyDeposit'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'60' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'ExciseDutyDeposit'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('ExciseDuty') and DepositType not in('NotDeposited')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId



    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'61' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'DevelopmentSurchargeDeposit'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'61' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'DevelopmentSurchargeDeposit'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('DevelopmentSurcharge') and DepositType not in('NotDeposited')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId



    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'62' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'ICTDevelopmentSurchargeDeposit'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'62' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'ICTDevelopmentSurchargeDeposit'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('ICTDevelopmentSurcharge') and DepositType not in('NotDeposited')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId


    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'63' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'HelthCareSurchargeDeposit'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'63' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'HelthCareSurchargeDeposit'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('HelthCareSurcharge') and DepositType not in('NotDeposited')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId



    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'64' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'EnvironmentProtectionSurchargeDeposit'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'64' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'EnvironmentProtectionSurchargeDeposit'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('EnvironmentProtectionSurcharge') and DepositType not in('NotDeposited')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId

    ";
                #endregion

                #region Note: 34 (9C-23B+28-33)

                sqlText = sqlText + @"

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'34' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-'SubFormName ,'Line34'Remarks
    from (
    select @BranchId BranchId,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'34' NoteNo,'0' SubNoteNo,ROUND(sum( LineC),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line34'Remarks 
    from VATReturnV2s where NoteNo in(9)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'34' NoteNo,'0' SubNoteNo,ROUND(-1*sum(LineB),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line34'Remarks 
    from VATReturnV2s where NoteNo in(23)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'34' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line34'Remarks 
    from VATReturnV2s where NoteNo in(28)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'34' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line34'Remarks 
    from VATReturnV2s where NoteNo in(33)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    ) as a


    ";
                #endregion

                #region Note: 56

                sqlText = sqlText + @"  
    select @Line54=ISNULL(sum(LineA),0) FROM VATReturnV2s WHERE NoteNo='54' and PeriodID=@PeriodID AND BranchId=@SelectBranchId;
    select @VAT18_6Adjustment= SettingValue FROM Settings WHERE SettingGroup='VAT9_1' And SettingName='VAT18_6Adjustment';




    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'56' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-'SubFormName ,'DecreasingAdjustmentForNote54'Remarks
    from (
    select @BranchId BranchId,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'56' NoteNo,'0' SubNoteNo
    ,CASE 
    WHEN SUM(ISNULL(LineA,0)) <= 0 then 0
    WHEN ROUND(sum(LineA)*@VAT18_6Adjustment/100,2)>ISNULL(@Line54,0) then ISNULL(@Line54,0) 
    WHEN ROUND(sum(LineA)*@VAT18_6Adjustment/100,2)<=ISNULL(@Line54,0) then ROUND(sum(LineA)*@VAT18_6Adjustment/100,2) 
    else 0 end LineA
    ,0 LineB,0 LineC,'-'SubFormName ,'Line56'Remarks 
    from VATReturnV2s where NoteNo in(34)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    ) as a

    ";
                #endregion

                #region Note: 35 (34-(52+56))

                sqlText = sqlText + @"

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'35' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-' SubFormName ,'Line35' Remarks
    from (
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'35' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line35'Remarks 
    from VATReturnV2s where NoteNo in(34)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 

    union all
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'35' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line35'Remarks 
    from VATReturnV2s where NoteNo in(52)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 


    union all
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'35' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line35'Remarks 
    from VATReturnV2s where NoteNo in(56)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 

    ) as a


    ";
                #endregion

                #region Note: 50 (35+41+43+44)

                sqlText = sqlText + @"

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'50' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-'SubFormName ,'Line50'Remarks
    from (
    select @BranchId BranchId,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'50' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line50'Remarks 
    from VATReturnV2s where NoteNo in(35)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'50' NoteNo,'0' SubNoteNo,ROUND(sum(LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line50'Remarks 
    from VATReturnV2s where NoteNo in(41)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'50' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line50'Remarks 
    from VATReturnV2s where NoteNo in(43)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'50' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line50'Remarks 
    from VATReturnV2s where NoteNo in(44)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    ) as a

    ";
                #endregion

                #region Note: 36 (9B+38-(39+40))

                sqlText = sqlText + @"

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'36' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-' SubFormName ,'Line36' Remarks
    from (
    select  @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'36' NoteNo,'0' SubNoteNo,ROUND(sum( LineB),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line36'Remarks 
    from VATReturnV2s where NoteNo in(9)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    union all
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'36' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line36'Remarks 
    from VATReturnV2s where NoteNo in(38)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 

    union all
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'36' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line36'Remarks 
    from VATReturnV2s where NoteNo in(39)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 

    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'36' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line36'Remarks 
    from VATReturnV2s where NoteNo in(40)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    ) as a
    ";
                #endregion

                #region Note: 57


                sqlText = sqlText + @"  
    select @Line55=ISNULL(sum(LineA),0) FROM VATReturnV2s WHERE NoteNo='55' and PeriodID=@PeriodID AND BranchId=@SelectBranchId;
    select @VAT18_6Adjustment= SettingValue FROM Settings WHERE SettingGroup='VAT9_1' And SettingName='VAT18_6Adjustment';



    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'57' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2) LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-'SubFormName ,'DecreasingAdjustmentForNote55'Remarks
    from (
    select @BranchId BranchId,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'57' NoteNo,'0' SubNoteNo
    ,CASE 
    WHEN SUM(ISNULL(LineA,0)) <= 0 then 0
    WHEN ROUND(sum(LineA)*@VAT18_6Adjustment/100,2)>ISNULL(@Line55,0) then ISNULL(@Line55,0) 
    WHEN ROUND(sum(LineA)*@VAT18_6Adjustment/100,2)<=ISNULL(@Line55,0) then ROUND(sum(LineA)*@VAT18_6Adjustment/100,2) 
    else 0 end LineA
    ,0 LineB,0 LineC,'-'SubFormName ,'Line57'Remarks 
    from VATReturnV2s where NoteNo in(36)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    ) as a

    ";
                #endregion

                #region Note: 37 (36-(53+57))

                sqlText = sqlText + @"


    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'37' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-' SubFormName ,'Line37' Remarks
    from (
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'37' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line37'Remarks 
    from VATReturnV2s where NoteNo in(36)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 

    union all
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'37' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line37'Remarks 
    from VATReturnV2s where NoteNo in(53)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 

    union all
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'37' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line37'Remarks 
    from VATReturnV2s where NoteNo in(57)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    ) as a
    ";
                #endregion

                #region Note: 51 (37+42)

                sqlText = sqlText + @"

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'51' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-' SubFormName ,'Line51' Remarks
    from (
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'51' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line51'Remarks 
    from VATReturnV2s where NoteNo in(37)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 

    union all
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'51' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line51'Remarks 
    from VATReturnV2s where NoteNo in(42)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    ) as a

    ";


                #endregion

                #region Note: 67

                sqlText = sqlText + @" 

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'67' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'RequestedAmountForRefundVAT'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'67' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'RequestedAmountForRefundVAT'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('Treasury') and DepositType in('RequestedAmountForRefundVAT')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId

    ";
                #endregion

                #region Note: 68

                sqlText = sqlText + @" 

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'68' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'-'SubFormName ,'RequestedAmountForRefundSD'Remarks 
    union all
    select @BranchId,@PeriodId, @DateFrom,@UserName,@Branch,'68' NoteNo,'1' SubNoteNo,ROUND(sum(DepositAmount),2),0 VATAmount,0 SDAmount,'-'SubFormName,'RequestedAmountForRefundSD'Remarks  
    from Deposits where  1=1 and  (post=@post1 or post=@post2)    
    and TransactionType in('SD') and DepositType in('RequestedAmountForRefundSD')
    and PeriodId=@PeriodId
    AND BranchId=@BranchId

    ";
                #endregion

                #region Note: 65 (58-(50+67))

                sqlText = sqlText + @"

    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'65' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-' SubFormName ,'Line65' Remarks
    from (
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'65' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line65'Remarks 
    from VATReturnV2s where NoteNo in(58)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 

    union all
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'65' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line65'Remarks 
    from VATReturnV2s where NoteNo in(50)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 

    union all
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'65' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line65'Remarks 
    from VATReturnV2s where NoteNo in(67)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    ) as a
    ";
                #endregion

                #region Note: 66 (59-(51+68))

                sqlText = sqlText + @"
    insert into VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
    select @BranchId,@PeriodId, @DateFrom,@UserName UserName,@Branch Branch,'66' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,ROUND(sum( LineB),2)LineB,ROUND(sum( LineC),2)LineC,'-' SubFormName ,'Line66' Remarks
    from (
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName UserName,@Branch Branch,'66' NoteNo,'0' SubNoteNo,ROUND(sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line66'Remarks 
    from VATReturnV2s where NoteNo in(59)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 

    union all
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'66' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line66'Remarks 
    from VATReturnV2s where NoteNo in(51)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 

    union all
    select @BranchId BranchId ,@PeriodId PeriodId, @DateFrom DateFrom,@UserName,@Branch,'66' NoteNo,'0' SubNoteNo,ROUND(-1*sum( LineA),2)LineA,0 LineB,0 LineC,'-'SubFormName ,'Line66'Remarks 
    from VATReturnV2s where NoteNo in(68)
    and PeriodID = @PeriodId and BranchId = @SelectBranchId 
    ) as a

    ";
                #endregion

                #region 9.1 Report

                sqlText = sqlText + @"

    select distinct Remarks NoteDescription,  NoteNo,ROUND( Sum(LineA),2)LineA,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC,SubFormName
    from VATReturnV2s
    where  BranchId=@SelectBranchId 
    and PeriodID=@PeriodID
    group by NoteNo,SubFormName,Remarks
    order by NoteNo

    ";
                #endregion

                #region DataTable 2 / IncreasingAdjustment

                sqlText = sqlText + @"
    ---------------------------------#TempAdjustmentHistorys/IncreasingAdjustment----------------------------SELECT
    ---------------------------------------------------------------------------------------------------------

    SELECT DISTINCT  AdjType,ah.AdjName,ROUND(SUM(ah.SubTotal),2)AdjAmount FROM #TempAdjustmentHistorys ah
    WHERE AdjType='IncreasingAdjustment' and ah.SubTotal>0
    GROUP BY AdjType,ah.AdjName


    DROP TABLE #TempAdjustmentHistorys;

    ";
                #endregion

                #region DataTable 3 / DecreasingAdjustment
                sqlText = sqlText + @"

    ---------------------------------#TempDecrementAdjustmentHistorys/DecreasingAdjustment----------------------------SELECT
    ------------------------------------------------------------------------------------------------------------------

    SELECT DISTINCT  AdjType,ah.AdjName,ROUND(SUM(ah.SubTotal),2)AdjAmount FROM #TempDecrementAdjustmentHistorys ah
    WHERE AdjType='DecreasingAdjustment' and ah.SubTotal>0
    GROUP BY AdjType,ah.AdjName


    DROP TABLE #TempDecrementAdjustmentHistorys;

    ";
                #endregion

                #endregion

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                    //sqlText = sqlText.Replace("= @SelectBranchId", " IN (SELECT BranchID FROM BranchProfiles) ");
                    sqlText = sqlText.Replace("=@SelectBranchId", " IN (SELECT BranchID FROM BranchProfiles) ");
                }

                #region Old Database Name

                if (code.ToLower() == "ACI-1".ToLower())
                {
                    if (periodDate >= oldDBDate && periodDate <= oldDBDateTo)
                    {
                        sqlText = sqlText.Replace("@db", OldDBName);

                    }
                }

                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn);

                objCommVAT19.CommandTimeout = 700;

                #endregion

                #region Parameter
                objCommVAT19.Parameters.AddWithValue("@SelectBranchId", vm.BranchId);
                objCommVAT19.Parameters.AddWithValue("@BranchId", vm.BranchId);
                objCommVAT19.Parameters.AddWithValue("@PreviousPeriodID", PreviousPeriodID);
                objCommVAT19.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT19.Parameters.AddWithValue("@post2", vm.post2);



                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                FileLogger.Log("VAT9.1 DAL", "9.1Query",
                    "BranchId = " + vm.BranchId + "\nPreviousPeriodID = " + PreviousPeriodID + "\npost = " + vm.post1 +
                    "," + vm.post2 + "\nPeriodName = " + vm.PeriodName);


                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dataSet);


                //if (transaction != null)
                //{
                //    transaction.Commit();
                //}

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                //if (transaction != null)
                //{
                //    transaction.Rollback();
                //}

                ////FileLogger.Log("9_1_VATReturnDAL", "9_1Save", ex.Message + "\n " + ex.StackTrace);

                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_V2Save", ex.ToString() + "\n" + sqlText);

                throw new Exception(sqlText);

            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dataSet;
        }

        public DataSet VAT9_1_V2Load(VATReturnVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            string PreviousPeriodID = "";
            DataSet dataSet = new DataSet("VAT9_1_V2");

            #endregion

            #region Try

            try
            {
                if (!string.IsNullOrWhiteSpace((vm.Date)))
                {
                    PreviousPeriodID = Convert.ToDateTime(vm.Date).AddMonths(-1).ToString("MMyyyy");
                }

                #region Post Status


                DataTable dtHeaders = new DataTable();
                dtHeaders = SelectAll_VATReturnHeader(vm, connVM);


                if (dtHeaders != null && dtHeaders.Rows.Count > 0)
                {
                    vm.Post = dtHeaders.Rows[0]["PostStatus"].ToString();
                    PostStatus(vm);

                }


                #endregion

                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                #region SQLText

                sqlText = @" ";

                #region Beginning

                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);
declare @ATVRebate as varchar(100);
declare @AutoPartialRebateProcess as varchar(1);

declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);


/*
declare @SelectBranchId as varchar(100)=0;
declare @BranchId as varchar(100)=0;
declare @post1 as varchar(100)='Y';
declare @post2 as varchar(100)='n';
declare @PeriodName as varchar(100)='June-2020';
declare @PeriodID as varchar(100);
*/



----------------------------------Initialization------------------------
------------------------------------------------------------------------
select  @PeriodId=PeriodId, @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;



select @ATVRebate=settingValue  FROM Settings where SettingGroup='ImportPurchase' and SettingName='ATVRebate';
select @AutoPartialRebateProcess=settingValue  FROM Settings where SettingGroup='Sale' and SettingName='AutoPartialRebateProcess';


----------------------------------Clear Data (Note 27, 32)------------------------
------------------------------------------------------------------------
delete VATReturnV2s where  PeriodID = @PeriodId and BranchId = @SelectBranchId  and NoteNo in (27,32)



-----------------------------Initialization/Rebate Cancel----------------------------
-------------------------------------------------------------------------------------
DECLARE @Line9Subtotal AS DECIMAL = 0
DECLARE @Line23VAT AS DECIMAL = 0

SELECT @Line9Subtotal=ISNULL(SUM(LineA),0)
FROM VATReturnV2s
WHERE  1=1 AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 9

SELECT @Line23VAT=ISNULL(SUM(LineB),0)
FROM VATReturnV2s
WHERE  1=1 AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 23

";
                #endregion

                #region Increasing Adjustment


                sqlText = sqlText + @" 

-----------------------------IncreasingAdjustment----------------------------Create/INSERT
-----------------------------------------------------------------------------------------------------
SELECT  *
INTO #TempAdjustmentHistorys 

FROM 
(
------SELECT '27' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'IncreasingAdjustment'Remarks 
------, '' AdjType, '' AdjName
------UNION ALL
SELECT '27' NoteNo,'1' SubNoteNo,ROUND(sum(AdjAmount),2)SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'IncreasingAdjustment'Remarks  
, ah.AdjType, an.AdjName, ah.AdjDescription
FROM AdjustmentHistorys ah
LEFT OUTER JOIN AdjustmentName an ON ah.AdjId=an.AdjId
WHERE  1=1 and  (post=@post1 or post=@post2)    
AND ah.AdjType in('IncreasingAdjustment')
and ah.PeriodId=@PeriodId
AND ah.BranchId=@BranchId
AND ah.IsAdjSD='N'
GROUP BY ah.AdjType, an.AdjName, ah.AdjDescription
UNION ALL

SELECT  '27' NoteNo,'2' SubNoteNo
,ddbd.ClaimVAT Subtotal
, 0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'IncreasingAdjustment' Remarks
,ddbd.TransactionType AdjType 
,ddbd.TransactionType AdjName 
,'' AdjDescription 
FROM DutyDrawBackDetails ddbd
LEFT OUTER JOIN PurchaseInvoiceDetails pid ON pid.PurchaseInvoiceNo=ddbd.PurchaseInvoiceNo
WHERE 1=1 AND ddbd.BranchId=@BranchId
AND  (ddbd.post=@post1 or ddbd.post=@post2) 
--and ddbd.PeriodId=@PeriodId
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND ISNULL(ddbd.TransactionType,'DDB') = 'VDB'
AND pid.Type IN('VAT','FixedVAT')


UNION ALL
SELECT '27' NoteNo,'3' SubNoteNo
, CASE
WHEN @AutoPartialRebateProcess='N' THEN 0 
WHEN @Line9Subtotal > 0 THEN ISNULL(ROUND((SUM(LineA)*@Line23VAT/@Line9Subtotal),2),0) 
ELSE 0 END AS Subtotal
, 0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'IncreasingAdjustment'Remarks  
,'IncreasingAdjustment' AdjType, 'Exempted Goods/Service (Rebate Cancel)' AdjName
,'' AdjDescription
FROM VATReturnV2s
WHERE  1=1
AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 3

UNION ALL
SELECT '27' NoteNo,'4' SubNoteNo
, CASE 
WHEN @AutoPartialRebateProcess='N' THEN 0 
WHEN @Line9Subtotal > 0 THEN ISNULL(ROUND((SUM(LineA)*@Line23VAT/@Line9Subtotal),2),0) 
ELSE 0 END AS Subtotal
, 0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'IncreasingAdjustment'Remarks  
,'IncreasingAdjustment' AdjType, 'Other Rated VAT (Rebate Cancel)' AdjName
,'' AdjDescription
FROM VATReturnV2s
WHERE  1=1
AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 7 AND SubNoteNo IN(1,2)


union all

select '27' NoteNo,'5' SubNoteNo,ROUND( sum(NonLeaderVATAmount),2), 0,0,'Sub-Form'SubFormName 
,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Non-Leader VAT Amount' AdjName 
,'' AdjDescription
from SalesInvoiceDetails where  1=1   and Type in('VAT')
and TransactionType in('ServiceNS','Service','TollFinishIssue','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
AND BranchId=@BranchId
and PeriodId=@PeriodId
and ISNULL(IsLeader,'NA') = 'Y'


--union all
--
--select '27' NoteNo,'6' SubNoteNo,ROUND(sum(SourcePaidVATAmount),2), 0,0,'Sub-Form'SubFormName 
--,'IncreasingAdjustment'Remarks
--,'IncreasingAdjustment' AdjType
--,'Source Paid VAT Amount' AdjName 
--,'' AdjDescription
--from SalesInvoiceDetails where  1=1   and Type in('VAT')
--and TransactionType in('Credit')
--AND BranchId=@BranchId
--AND PeriodId=@PeriodId
--and ISNULL(SourcePaidVATAmount,0) > 0



union all

select '27' NoteNo,'7' SubNoteNo,ROUND(sum(ISNULL(VATAmount,0)),2), 0,0,'Sub-Form'SubFormName 
,'IncreasingAdjustment'Remarks
,'IncreasingAdjustment' AdjType
,'Raw Material Dispose' AdjName 
,'' AdjDescription
from DisposeRawDetails where  1=1   
and TransactionType in('Other')
AND BranchId=@BranchId
AND PeriodId=@PeriodId

union all

select '27' NoteNo,'8' SubNoteNo,ROUND(sum(RebateVATAmount),2), 0,0,'Sub-Form'SubFormName 
,'IncreasingAdjustment'Remarks
,'IncreasingAdjustment' AdjType
,'Raw Material for Finish Dispose' AdjName 
,'' AdjDescription
from DisposeFinishDetails where  1=1   
and TransactionType in('Other','DisposeTrading')
AND BranchId=@BranchId
AND PeriodId=@PeriodId
/*
union all

select '27' NoteNo,'9' SubNoteNo,ROUND(sum(ISNULL(VATAmount,0)+ISNULL(ATVAmount,0)),2),0,0,'Sub-Form'SubFormName 
,'IncreasingAdjustment'Remarks
,'IncreasingAdjustment' AdjType
,'Purchase Credit' AdjName 
,'' AdjDescription
from PurchaseInvoiceDetails where  1=1   
and TransactionType in('PurchaseReturn')
AND BranchId=@BranchId
AND PeriodId=@PeriodId
*/
--union all
--
--select '27' NoteNo,'10' SubNoteNo,ROUND(sum(ISNULL(VATAmount,0)),2),0,0,'Sub-Form'SubFormName 
--,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Exempted VAT' AdjName ,'' AdjDescription
--from SalesInvoiceDetails where  1=1  
--and TransactionType in ('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
--and Type in ('NonVAT')
--AND BranchId=@BranchId
--AND PeriodId=@PeriodId
--AND isnull(SourcePaidQuantity,0)=0
----AND isnull(Channel,'-') != 'Retail'
--AND Option2 is null

union all

select '27' NoteNo,'11' SubNoteNo,ROUND(sum(ISNULL(Option2,0)),2),0,0,'Sub-Form'SubFormName 
,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Exempted VAT' AdjName ,'' AdjDescription
from SalesInvoiceDetails where  1=1  
and TransactionType in ('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
and Type in ('OtherRate','NonVAT','Retail')
AND BranchId=@BranchId
AND PeriodId=@PeriodId
AND isnull(SourcePaidQuantity,0)=0
--AND isnull(Channel,'-') = 'Retail'

) AS adj

INSERT INTO VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
SELECT @BranchId,@PeriodId, @DateFrom,@UserName,@Branch, '27' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'IncreasingAdjustment'Remarks 
UNION ALL

SELECT @BranchId,@PeriodId, @DateFrom,@UserName,@Branch, NoteNo, SubNoteNo, Subtotal, VATAmount, SDAmount, SubFormName, Remarks
FROM #TempAdjustmentHistorys
where SubTotal>0

 
";
                #endregion

                #region Decreasing Adjustment

                sqlText = sqlText + @"


SELECT  *
INTO #TempDecrementAdjustmentHistorys 

FROM
(
SELECT '32' NoteNo,'0' SubNoteNo,0 SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName ,'DecreasingAdjustment'Remarks
, '' AdjType, '' AdjName , ''AdjDescription
UNION ALL
SELECT '32' NoteNo,'1' SubNoteNo,ROUND(sum(AdjAmount),2)SubTotal,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'DecreasingAdjustment'Remarks  
, ah.AdjType, an.AdjName, ah.AdjDescription
FROM AdjustmentHistorys ah
LEFT OUTER JOIN AdjustmentName an ON ah.AdjId=an.AdjId
WHERE  1=1 and  (post=@post1 or post=@post2)    
AND ah.AdjType in('DecreasingAdjustment')
and ah.PeriodId=@PeriodId
AND ah.BranchId=@BranchId
AND ah.IsAdjSD='N'
GROUP BY ah.AdjType, an.AdjName, ah.AdjDescription

UNION ALL

SELECT  '32' NoteNo,'2' SubNoteNo
,ddbd.ClaimVAT Subtotal
, 0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'DecreasingAdjustment' Remarks
,'DecreasingAdjustment' AdjType 
,ddbd.TransactionType AdjName  
,'' AdjDescription
FROM DutyDrawBackDetails ddbd
LEFT OUTER JOIN PurchaseInvoiceDetails pid ON pid.PurchaseInvoiceNo=ddbd.PurchaseInvoiceNo
WHERE 1=1 AND ddbd.BranchId=@BranchId
AND  (ddbd.post=@post1 or ddbd.post=@post2) 
--and ddbd.PeriodId=@PeriodId
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND ISNULL(ddbd.TransactionType,'DDB') = 'VDB'
AND pid.Type='NonRebate'

union all

select '32' NoteNo,'3' SubNoteNo,ROUND(sum(NonLeaderVATAmount),2), 0,0,'Sub-Form'SubFormName 
,'DecreasingAdjustment'Remarks, 'DecreasingAdjustment' AdjType, 'Non-Leader VAT Amount' AdjName 
,'' AdjDescription
from SalesInvoiceDetails where  1=1   and Type in('VAT')
and TransactionType in('ServiceNS','Service','TollFinishIssue','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
AND BranchId=@BranchId
and PeriodId=@PeriodId
and ISNULL(IsLeader,'NA') = 'N'

--union all
--
--select '32' NoteNo,'4' SubNoteNo,ROUND(sum(VATAmount),2), 0,0,'Sub-Form'SubFormName 
--,'DecreasingAdjustment'Remarks, 'DecreasingAdjustment' AdjType, 'Rebate Cancel(Credit)' AdjName 
--,'' AdjDescription
--from SalesInvoiceDetails where  1=1   
--    and Type in('NonVAT','Retail')
--    and TransactionType in('credit')
--AND BranchId=@BranchId
--and PeriodId=@PeriodId
--and ISNULL(IsLeader,'NA') in ('Y','NA')
--and EXISTS (select * from settings where settinggroup = 'CompanyCode' and SettingName = 'code'and SettingValue = 'Bata')

--union all
--
--select '32' NoteNo,'4' SubNoteNo,ROUND(sum(SourcePaidVATAmount),2), 0,0,'Sub-Form'SubFormName 
--,'DecreasingAdjustment'Remarks, 'DecreasingAdjustment' AdjType, 'Source Paid VAT Amount' AdjName 
--,'' AdjDescription
--from SalesInvoiceDetails where  1=1   and Type in('VAT')
--and TransactionType in('ServiceNS','Service','TollFinishIssue','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
--,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
--AND BranchId=@BranchId
--AND PeriodId=@PeriodId
--and ISNULL(SourcePaidVATAmount,0) > 0
/*
union all

select '32' NoteNo,'5' SubNoteNo,ROUND(sum(VATAmount),2),0,0,'Sub-Form'SubFormName
,'DecreasingAdjustment'Remarks , 'DecreasingAdjustment' AdjType, 'DecreasingAdjustmentVATAmount' AdjName  ,'' AdjDescription
from PurchaseInvoiceDetails where  1=1  and Type in('VAT')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
and RebatePeriodID=@PeriodId
and IsRebate='Y'
AND BranchId=@BranchId
*/
) decAdj

INSERT INTO VATReturnV2s(BranchId,PeriodID,PeriodStart,UserName,Branch,NoteNo,SubNoteNo,LineA,LineB,LineC,SubFormName,Remarks)
SELECT @BranchId,@PeriodId, @DateFrom,@UserName,@Branch, NoteNo, SubNoteNo
, Subtotal
, VATAmount, SDAmount, SubFormName,  Remarks  
FROM #TempDecrementAdjustmentHistorys


";
                #endregion

                #region 9.1 Report

                sqlText = sqlText + @"

select distinct n.Description NoteDescription,  v.NoteNo,ROUND( Sum(LineA),2)LineA,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC,SubFormName
from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

 where  BranchId=@SelectBranchId 
and PeriodID=@PeriodID
group by v.NoteNo,v.SubFormName,n.Description
order by v.NoteNo

";
                #endregion

                #region DataTable 2 / IncreasingAdjustment

                sqlText = sqlText + @"
---------------------------------#TempAdjustmentHistorys/IncreasingAdjustment----------------------------SELECT
---------------------------------------------------------------------------------------------------------

SELECT DISTINCT  AdjType,ah.AdjName,ah.AdjDescription,ROUND(SUM(ah.SubTotal),2)AdjAmount FROM #TempAdjustmentHistorys ah
WHERE AdjType='IncreasingAdjustment' and ah.SubTotal>0
GROUP BY AdjType,ah.AdjName,ah.AdjDescription


DROP TABLE #TempAdjustmentHistorys;

";
                #endregion

                #region DataTable 3 / DecreasingAdjustment
                sqlText = sqlText + @"

---------------------------------#TempDecrementAdjustmentHistorys/DecreasingAdjustment----------------------------SELECT
------------------------------------------------------------------------------------------------------------------

SELECT DISTINCT  AdjType,ah.AdjName, ah.AdjDescription, ROUND(SUM(ah.SubTotal),2)AdjAmount FROM #TempDecrementAdjustmentHistorys ah
WHERE AdjType='DecreasingAdjustment' and ah.SubTotal>0
GROUP BY AdjType,ah.AdjName, ah.AdjDescription


DROP TABLE #TempDecrementAdjustmentHistorys;

";
                #endregion

                #region DataTable 4 / VATReturnHeaders
                sqlText = sqlText + @"

---------------------------------VATReturnHeaders----------------------------
-----------------------------------------------------------------------------

SELECT * FROM VATReturnHeaders ah
WHERE 1=1
and BranchId=@SelectBranchId 
and PeriodID=@PeriodID

";
                #endregion

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter

                objCommVAT19.Parameters.AddWithValue("@SelectBranchId", vm.BranchId);
                objCommVAT19.Parameters.AddWithValue("@BranchId", vm.BranchId);
                objCommVAT19.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT19.Parameters.AddWithValue("@post2", vm.post2);



                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }




                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dataSet);

                #endregion


            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_V2Load", sqlex.ToString() + "\n" + sqlText);

                throw new Exception(sqlText);
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_V2Load", ex.ToString() + "\n" + sqlText);

                throw new Exception(sqlText);
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dataSet;
        }

        private void PostStatus(VATReturnVM vm, SysDBInfoVMTemp connVM = null)
        {

            #region Post Status

            vm.post1 = "y";
            vm.post2 = "n";
            if (vm.Post != null && vm.Post.ToLower() == "y")
            {
                vm.post1 = "y";
                vm.post2 = "y";
            }
            else if (vm.Post != null && vm.Post.ToLower() == "n")
            {
                vm.post1 = "n";
                vm.post2 = "n";
            }

            #endregion
        }

        private void PostStatus(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {

            #region Post Status

            vm.post1 = "y";
            vm.post2 = "n";
            if (vm.Post != null && vm.Post.ToLower() == "y")
            {
                vm.post1 = "y";
                vm.post2 = "y";
            }
            else if (vm.Post != null && vm.Post.ToLower() == "n")
            {
                vm.post1 = "n";
                vm.post2 = "n";
            }

            #endregion
        }

        public VATReturnHeaderVM SelectAll_VATReturnHeader_Model(VATReturnVM paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string sqlText = "";
            VATReturnHeaderVM vm = new VATReturnHeaderVM();

            #endregion

            #region Try

            try
            {
                DataTable dt = new DataTable();
                dt = SelectAll_VATReturnHeader(paramVM, connVM);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.UserName = Convert.ToString(dr["UserName"]);
                    vm.Branch = Convert.ToString(dr["Branch"]);
                    vm.Remarks = Convert.ToString(dr["Remarks"]);
                    vm.PeriodID = Convert.ToString(dr["PeriodID"]);
                    vm.PeriodStart = Convert.ToString(dr["PeriodStart"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.MainOrginalReturn = Convert.ToString(dr["MainOrginalReturn"]) == "Y" ? true : false;
                    vm.LateReturn = Convert.ToString(dr["LateReturn"]) == "Y" ? true : false;
                    vm.AmendReturn = Convert.ToString(dr["AmendReturn"]) == "Y" ? true : false;
                    vm.AlternativeReturn = Convert.ToString(dr["FullAdditionalAlternativeReturn"]) == "Y" ? true : false;
                    vm.NoActivites = Convert.ToString(dr["NoActivites"]) == "Y" ? true : false;
                    vm.NoActivitesDetails = Convert.ToString(dr["NoActivitesDetails"]);
                    vm.DateOfSubmission = Convert.ToString(dr["DateOfSubmission"]);
                    vm.PostStatus = Convert.ToString(dr["PostStatus"]);
                    vm.IsTraderVAT = Convert.ToString(dr["IsTraderVAT"]) == "Y" ? true : false;


                }

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "SelectAll_VATReturnHeader_Model", sqlex.ToString() + "\n" + sqlText);

                throw new Exception(sqlText);
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "SelectAll_VATReturnHeader_Model", ex.ToString() + "\n" + sqlText);

                throw new Exception(sqlText);
            }
            finally
            {


            }

            #endregion

            return vm;
        }

        public DataTable SelectAll_VATReturnHeader(VATReturnVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable("VATReturnHeader");


            #endregion

            #region Try

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                vm.PeriodName = Convert.ToDateTime(vm.PeriodName).ToString("MMMM-yyyy");

                #region SQLText

                sqlText = @" ";


                sqlText = @" 
declare @PeriodId as varchar(100);

----------------------------------Initialization------------------------
select  @PeriodId=PeriodId FROM FiscalYear where periodName=@periodName;


";


                sqlText = sqlText + @"

---------------------------------VATReturnHeaders----------------------------

SELECT 

Id
,UserName
,Branch
,Remarks
,PeriodID
,PeriodStart
,BranchId
,MainOrginalReturn
,LateReturn
,AmendReturn
,FullAdditionalAlternativeReturn
,NoActivites
,NoActivitesDetails
,DateOfSubmission
,PostStatus
,Post
,IsTraderVAT

FROM VATReturnHeaders ah
WHERE 1=1
and BranchId=@SelectBranchId 
and PeriodID=@PeriodID

";

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter
                objCommVAT19.Parameters.AddWithValue("@SelectBranchId", vm.BranchId);



                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }




                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "SelectAll_VATReturnHeader", sqlex.ToString() + "\n" + sqlText);

                throw new Exception(sqlText);
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "SelectAll_VATReturnHeader", ex.ToString() + "\n" + sqlText);

                throw new Exception(sqlText);
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public string[] InsertToVATReturnHeaders(VATReturnHeaderVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables


            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            DataTable Dt = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            int nextId = 0;
            int ID = 0;

            #endregion

            #region try

            try
            {

                #region open connection and transaction

                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region PeriodId find checking

                sqlText = "select PeriodId FROM FiscalYear where PeriodName=@PeriodName";
                SqlCommand PeriodId = new SqlCommand(sqlText, currConn);
                PeriodId.Transaction = transaction;
                PeriodId.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(PeriodId);
                dataAdapter.Fill(Dt);
                vm.PeriodID = Dt.Rows[0]["PeriodId"].ToString();

                #endregion PeriodId find checking

                #region Delete

                sqlText = "delete VATReturnHeaders where  PeriodID = @PeriodId and BranchId = @BranchId";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Transaction = transaction;
                cmdDelete.Parameters.AddWithValue("@PeriodID", vm.PeriodID);
                cmdDelete.Parameters.AddWithValue("@BranchId", vm.BranchId);
                transResult = (int)cmdDelete.ExecuteNonQuery();

                #endregion Delete

                #region VATReturnHeaders new id generation

                sqlText = "select isnull(max(cast(Id as int)),0)+1 FROM  VATReturnHeaders";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = Convert.ToInt32(cmdNextId.ExecuteScalar());
                if (nextId <= 0)
                {

                    throw new ArgumentNullException("InsertToVATReturnHeaders", "Unable to Create");
                }

                #endregion VATReturnHeaders new id generation

                ID = nextId;

                #region Insert new VATReturnHeaders

                sqlText = "";
                sqlText += @" 
INSERT INTO VATReturnHeaders(
Id
,UserName
,Branch
,Remarks
,PeriodID
,PeriodStart
,BranchId
,MainOrginalReturn
,LateReturn
,AmendReturn
,FullAdditionalAlternativeReturn
,NoActivites
,NoActivitesDetails
,DateOfSubmission
,PostStatus
,SignatoryName
,SignatoryDesig
,Email
,Mobile
,NationalID
,IsTraderVAT

) VALUES (
 @Id
,@UserName
,@Branch
,@Remarks
,@PeriodID
,@PeriodStart
,@BranchId
,@MainOrginalReturn
,@LateReturn
,@AmendReturn
,@FullAdditionalAlternativeReturn
,@NoActivites
,@NoActivitesDetails
,@DateOfSubmission
,@PostStatus
,@SignatoryName
,@SignatoryDesig
,@Email
,@Mobile
,@NationalID
,@IsTraderVAT
) 
";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Id", ID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UserName", vm.UserName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PostStatus", vm.PostStatus);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Branch", vm.BranchName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Remarks", vm.Remarks ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PeriodID", vm.PeriodID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PeriodStart", OrdinaryVATDesktop.DateToDate(vm.PeriodStart));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", vm.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MainOrginalReturn", vm.MainOrginalReturn ? "Y" : "N");
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LateReturn", vm.LateReturn ? "Y" : "N");
                cmdInsert.Parameters.AddWithValueAndNullHandle("@AmendReturn", vm.AmendReturn ? "Y" : "N");
                cmdInsert.Parameters.AddWithValueAndNullHandle("@FullAdditionalAlternativeReturn", vm.AlternativeReturn ? "Y" : "N");
                cmdInsert.Parameters.AddWithValueAndNullHandle("@NoActivites", vm.NoActivites ? "Y" : "N");
                cmdInsert.Parameters.AddWithValueAndNullHandle("@NoActivitesDetails", vm.NoActivitesDetails ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DateOfSubmission", OrdinaryVATDesktop.DateToDate(vm.DateOfSubmission) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SignatoryName", vm.SignatoryName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", vm.SignatoryDesig);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Email", vm.Email);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Mobile", vm.Mobile);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@NationalID", vm.NationalID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsTraderVAT", vm.IsTraderVAT ? "Y" : "N");

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #region Commit

                if (transaction != null && Vtransaction == null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested VATReturnHeaders  Information successfully Added.";
                        retResults[2] = "" + nextId;

                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to add VATReturnHeaders ";
                    retResults[2] = "";
                }

                #endregion Commit

                #endregion Insert new CustomsHouse

            }
            #endregion

            #region catch & Finally

            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1_VATReturnDAL", "InsertToVATReturnHeaders", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
            }
            #endregion

            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return retResults;
        }

        string subFormName = "";
        public DataTable VAT9_1_SubForm(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null, bool IsSaveSubForm = false)
        {
            DataTable dt = new DataTable();
            string TableName = "";

            #region try

            try
            {
                _9_1_VATReturnDAL _ReportDSDAL = new _9_1_VATReturnDAL();

                DateTime datePeriodStart = Convert.ToDateTime(vm.PeriodName);
                DateTime HardDecember2019 = Convert.ToDateTime("December-2019");
                if (datePeriodStart < HardDecember2019)
                {
                    vm.IsVersion2 = false;
                }
                else
                {
                    vm.IsVersion2 = true;
                }

                #region Post Status

                VATReturnVM varVATReturnVM = new VATReturnVM();
                varVATReturnVM.PeriodName = vm.PeriodName;
                varVATReturnVM.BranchId = vm.BranchId;

                DataTable dtHeaders = new DataTable();
                dtHeaders = SelectAll_VATReturnHeader(varVATReturnVM);

                if (dtHeaders != null && dtHeaders.Rows.Count > 0)
                {
                    vm.Post = dtHeaders.Rows[0]["PostStatus"].ToString();

                    PostStatus(vm);
                }

                #endregion

                #region SubFormName
                TableName = "";
                subFormName = "";

                if ((vm.NoteNo >= 1 && vm.NoteNo <= 2))
                {
                    subFormName = "SubFormPart3_Note1_2";

                    TableName = "VAT9_1SubFormB";
                }
                else if ((vm.NoteNo >= 3 && vm.NoteNo <= 5) || vm.NoteNo == 7)
                {
                    subFormName = "SubFormAPart3";

                    TableName = "VAT9_1SubFormA";

                }
                else if (vm.NoteNo == 6)
                {
                    subFormName = "SubFormCPart3";

                    TableName = "VAT9_1SubFormC";

                }
                else if (vm.NoteNo == 8)
                {
                    subFormName = "SubFormBPart3";

                    TableName = "VAT9_1SubFormD";

                }
                else if (vm.NoteNo == 11 || vm.NoteNo == 13 || vm.NoteNo == 15 || vm.NoteNo == 17 || vm.NoteNo == 22)
                {
                    subFormName = "SubFormAPart4_2021";

                    TableName = "VAT9_1SubFormB";
                }
                else if ((vm.NoteNo == 10 || vm.NoteNo == 12 || vm.NoteNo == 14 || vm.NoteNo == 16) || (vm.NoteNo >= 19 && vm.NoteNo <= 23 && vm.NoteNo != 22))
                {
                    subFormName = "SubFormAPart4";

                    TableName = "VAT9_1SubFormA";

                }
                else if (vm.NoteNo == 18)
                {
                    subFormName = "SubFormCPart4";

                    TableName = "VAT9_1SubFormC";

                }
                else if (vm.NoteNo == 24)
                {
                    subFormName = "SubFormDPart5";

                    TableName = "VAT9_1SubFormE";

                }
                else if (vm.NoteNo == 26)//debitNote
                {
                    subFormName = "SubFormEPart5";

                    TableName = "VAT9_1SubFormF";

                }
                else if (vm.NoteNo == 27 || vm.NoteNo == 32)//Increasing Adjustment And Decreasing Adjustment
                {
                    subFormName = "SubFormEPart7";

                    TableName = "VAT9_1SubFormG";

                }
                else if (vm.NoteNo == 29)
                {
                    subFormName = "SubFormEPart6";

                    TableName = "VAT9_1SubFormH";

                }

                else if (vm.NoteNo == 30)
                {
                    subFormName = "SubFormFPart6";

                    TableName = "VAT9_1SubFormI";

                }
                else if (vm.NoteNo == 31) //CreditNo
                {
                    subFormName = "SubFormFPart7";

                    TableName = "VAT9_1SubFormJ";

                }
                else if (vm.NoteNo >= 52 && vm.NoteNo <= 61)
                {
                    subFormName = "SubFormGPart8";

                    TableName = "VAT9_1SubFormK";

                }

                if (vm.IsVersion2)
                {
                    if (vm.NoteNo >= 58 && vm.NoteNo <= 64)
                    {
                        subFormName = "SubFormGPart8_V2";

                        TableName = "VAT9_1SubFormK";

                    }
                }

                #endregion

                #region Report Data Switching

                //////int BranchId = 0;

                vm.ExportInBDT = "Y";

                switch (subFormName)
                {
                    case "SubFormPart3_Note1_2":
                        dt = _ReportDSDAL.VAT9_1_SubFormAPart3_Note_1_2(vm);//done
                        dt.TableName = "dtVATReturnSubFormA_2021";
                        break;
                    case "SubFormAPart3":
                        dt = _ReportDSDAL.VAT9_1_SubFormAPart3(vm);//done
                        dt.TableName = "dtVATReturnSubFormA";
                        break;
                    case "SubFormAPart4_2021":
                        dt = _ReportDSDAL.VAT9_1_SubFormAPart4_2021(vm);//done
                        dt.TableName = "dtVATReturnSubFormA_2021";
                        break;
                    case "SubFormAPart4":
                        dt = _ReportDSDAL.VAT9_1_SubFormAPart4(vm);//done
                        dt.TableName = "dtVATReturnSubFormA";
                        break;
                    case "SubFormBPart3":
                        dt = _ReportDSDAL.VAT9_1_SubFormBPart3(vm);//done
                        dt.TableName = "dtVATReturnSubFormB";

                        break;
                    case "SubFormCPart3":
                        dt = _ReportDSDAL.VAT9_1_SubFormCPart3(vm);//done
                        dt.TableName = "dtVATReturnSubFormC";
                        break;
                    case "SubFormCPart4":
                        dt = _ReportDSDAL.VAT9_1_SubFormCPart4(vm);//done
                        dt.TableName = "dtVATReturnSubFormC";
                        break;
                    case "SubFormDPart5":
                        dt = _ReportDSDAL.VAT9_1_SubFormDPart5(vm);//done
                        dt.TableName = "dtVATReturnSubFormD";
                        break;
                    case "SubFormEPart6":
                        dt = _ReportDSDAL.VAT9_1_SubFormEPart6(vm);//done
                        dt.TableName = "dtVATReturnSubFormE";
                        break;
                    case "SubFormEPart7":
                        dt = _ReportDSDAL.VAT9_1_SubFormEPart7(vm);//done Increasing Adj And Decreasing Adj
                        dt.TableName = "dtVATReturnSubFormJ";
                        break;
                    case "SubFormFPart6":
                        dt = _ReportDSDAL.VAT9_1_SubFormFPart6(vm);//done
                        dt.TableName = "dtVATReturnSubFormF";
                        break;
                    case "SubFormGPart8":
                        dt = _ReportDSDAL.VAT9_1_SubFormGPart8(vm);//done
                        dt.TableName = "dtVATReturnSubFormG";
                        break;
                    case "SubFormGPart8_V2":
                        dt = _ReportDSDAL.VAT9_1_SubFormGPart8_V2(vm);//done
                        dt.TableName = "dtVATReturnSubFormG";
                        break;
                    case "SubFormEPart5":
                        dt = _ReportDSDAL.VAT9_1_SubFormEPart5(vm);//done debitNote
                        dt.TableName = "dtVATReturnSubFormH";
                        break;
                    case "SubFormFPart7":
                        dt = _ReportDSDAL.VAT9_1_SubFormFPart7(vm);//done CreditNote
                        dt.TableName = "dtVATReturnSubFormI";
                        break;

                    default:
                        break;
                }

                #endregion

                #region Bulk insert for sub form

                if (IsSaveSubForm)
                {

                    if (!string.IsNullOrWhiteSpace(TableName))
                    {
                        vm.SubformTableName = TableName;

                        string[] sfResult = SaveSubFormData(vm, dt, connVM);
                    }

                }

                #endregion

            }
            #endregion

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubForm", ex.ToString());

                throw;
            }
            finally { }
            return dt;
        }

        public VATReturnSubFormVM VAT9_1_SubForm_Download(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region try

            try
            {
                DataTable dt = new DataTable();
                dt = VAT9_1_SubForm(vm);


                DataTable dtComapnyProfile = new DataTable();

                DataSet tempDS = new DataSet();
                tempDS = new ReportDSDAL().ComapnyProfile("");
                dtComapnyProfile = tempDS.Tables[0].Copy();



                if (dt != null && dt.Rows.Count > 0)
                {
                    vm.SheetName = dt.Rows[0]["SubFormName"].ToString();
                    vm.NoteNo = Convert.ToInt32(dt.Rows[0]["NoteNo"]);
                    vm.Type = dt.Rows[0]["Remarks"].ToString();
                }

                ////string[] ReportHeaders = new string[] { ComapnyName, VatRegistrationNo, Address1, vm.SheetName, " Note No: " + vm.NoteNo + "        Type: " + vm.Type };

                #region SubForm
                switch (subFormName)
                {
                    case "SubFormAPart3":
                    case "SubFormAPart4":
                        if (!dt.Columns.Contains("ImportIDExcel"))
                        {
                            dt.Columns.Add("ImportIDExcel");
                        }

                        DataView view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormA", false, "ProductDescription", "ProductCode", "ProductName", "TotalPrice", "SDAmount", "VATAmount", "DetailRemarks", "ImportIDExcel");

                        break;
                    case "SubFormBPart3":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormB", false, "ProductCategory", "ProductDescription", "ProductCode", "ProductName", "TotalPrice", "SDAmount", "VATAmount", "DetailRemarks");
                        break;
                    case "SubFormCPart3":
                    case "SubFormCPart4":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormC", false, "ProductDescription", "ProductCode", "ProductName", "UOM", "Quantity", "TotalPrice", "SDAmount", "VATAmount", "DetailRemarks");
                        break;

                    case "SubFormDPart5":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormD", false, "VendorBIN", "VendorName", "VendorAddress", "TotalPrice", "BillDeductAmount", "VDSAmount", "InvoiceNo", "InvoiceDate", "VDSCertificateNo", "VDSCertificateDate", "AccountCode", "TaxDepositSerialNo", "TaxDepositDate", "DetailRemarks");
                        break;
                    case "SubFormEPart6":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormE", false, "CustomerBIN", "CustomerName", "CustomerAddress", "TotalPrice", "VDSAmount", "InvoiceNo", "InvoiceDate", "VDSCertificateNo", "VDSCertificateDate", "AccountCode", "SerialNo", "TaxDepositDate", "DetailRemarks");

                        break;
                    case "SubFormFPart6":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormF", false, "BENumber", "Date", "CustomHouse", "ATAmount", "DetailRemarks");
                        break;
                    case "SubFormGPart8":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormG", false, "ChallanNumber", "BankName", "BankBranch", "Date", "AccountCode", "Amount", "DetailRemarks");
                        break;
                    case "SubFormEPart5":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormH", false, "DebitNoteNo", "IssuedDate", "TaxChallanNo", "TaxChallanDate", "ReasonforIssuance", "ValueinChallan", "VATinChallan", "SDinChallan", "ValueofIncreasingAdjustment", "QuantityofIncreasingAdjustment", "VATofIncreasingAdjustment", "SDofIncreasingAdjustment", "Remarks");
                        break;
                    case "SubFormFPart7":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormI", false, "CreditNoteNo", "IssuedDate", "TaxChallanNo", "TaxChallanDate", "ReasonforIssuance", "ValueinChallan", "VATinChallan", "SDinChallan", "ValueofDecreasingAdjustment", "QuantityofDecreasingAdjustment", "VATofDecreasingAdjustment", "SDofDecreasingAdjustment", "Remarks");
                        break;
                    case "SubFormEPart7":
                        view = new DataView(dt);
                        dt = new DataTable();
                        dt = view.ToTable("dtVATReturnSubFormJ", false, "ChallanNumber", "Date", "Amount", "VAT", "Notes");
                        break;

                    default:
                        break;
                }
                #endregion

                vm.dtVATReturnSubForm = dt.Copy();
                vm.dtComapnyProfile = dtComapnyProfile.Copy();

            }
            #endregion

            #region catch & finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubForm_Download", ex.ToString());

                throw;
            }
            finally { }
            #endregion

            return vm;
        }

        public ResultVM Post(VATReturnVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            ResultVM rVM = new ResultVM();
            int transResult = 0;
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region try

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Post Data

                sqlText = "";
                sqlText = @" 
--------declare @PeriodId as int = '102020'
declare @PeriodStart as date = '2020-Oct-01'

select  @PeriodStart=PeriodStart   FROM FiscalYear where PeriodID=@PeriodId;


update VATReturnHeaders
set Post = 'Y',SignatoryName=@SignatoryName,SignatoryDesig=@SignatoryDesig,Email=@Email,Mobile=@Mobile,NationalID=@NationalID
where 1=1 
and PeriodID = @PeriodId


update VATReturnV2s
set Post = 'Y'
where 1=1 and PeriodID = @PeriodId


update FiscalYear
set VATReturnPost = 'Y', PeriodLock='Y'
where 1=1 and PeriodID = @PeriodId


--------------------------------------------------Previous--------------------------------------------------
------------------------------------------------------------------------------------------------------------
update VATReturnHeaders
set Post = 'Y'
From VATReturnHeaders ret
left outer join FiscalYear fyr on ret.PeriodID=fyr.PeriodID
where 1=1 and ret.PeriodStart< @PeriodStart
and ISNULL(ret.Post, 'N') = 'N'


update VATReturnV2s
set Post = 'Y'
From VATReturnV2s ret
left outer join FiscalYear fyr on ret.PeriodID=fyr.PeriodID
where 1=1 and ret.PeriodStart< @PeriodStart
and ISNULL(ret.Post, 'N') = 'N'


update FiscalYear
set VATReturnPost = 'Y', PeriodLock='Y'
where 1=1 
and PeriodStart< @PeriodStart
and ISNULL(VATReturnPost, 'N') = 'N'
";

                paramVM.PeriodId = Convert.ToInt32(Convert.ToDateTime(paramVM.PeriodName).ToString("MMyyyy"));

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@PeriodId", paramVM.PeriodId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryName", paramVM.varVATReturnHeaderVM.SignatoryName);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", paramVM.varVATReturnHeaderVM.SignatoryDesig);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Mobile", paramVM.varVATReturnHeaderVM.Mobile);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Email", paramVM.varVATReturnHeaderVM.Email);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@NationalID", paramVM.varVATReturnHeaderVM.NationalID);


                transResult = cmdUpdate.ExecuteNonQuery();



                #endregion

                #region Commit


                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit

                #region SuccessResult

                rVM.Status = "Success";
                rVM.Message = "VAT Return Posted Successfully!";

                #endregion SuccessResult


            }
            #endregion

            #region catch & finally

            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                FileLogger.Log("_9_1_VATReturnDAL", "Post", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally { }
            #endregion

            return rVM;
        }

        #region VAT9.1 SubForms

        //// LineNo=1,2,3,4,5,7
        public DataTable VAT9_1_SubFormAPart3(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();
                string code = commonDal.settingValue("CompanyCode", "Code", connVM);

                #region Statement

                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
--------declare @BranchId int = 1


declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;

";

                #region Summary SQL

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 

SELECT DISTINCT
UserName
, Branch
, NoteNo
, SubNoteNo
, SUM(TotalPrice)  TotalPrice
, SUM(VATAmount)  VATAmount
, SUM(SDAmount)  SDAmount
, SubFormName
, Remarks
, ItemNo
, ProductDescription
, ProductName
, ProductCode
,'' DetailRemarks
,VATRate
,SDRate
FROM (

";
                }

                #endregion

                if (vm.NoteNo == 1)
                {
                    #region LineNo-1

                    sqlText = sqlText + @" 

select sal.*, p.ProductDescription, p.ProductName, sal.SalesInvoiceNo DetailRemarks  from (
SELECT 
@UserName UserName,@Branch Branch,
'1' NoteNo,'1' SubNoteNo,sid.CurrencyValue TotalPrice,sid.VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
,'Export'Remarks 
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode
from SalesInvoiceDetails sid

where 1=1 
 and  (sid.post=@post1 or sid.post=@post2)  
and sid.Type in('Export') 
and sid.TransactionType in('Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and sid.PeriodId=@PeriodId
------and sid.InvoiceDateTime >= @Datefrom and sid.InvoiceDateTime <dateadd(d,1,@Dateto)
AND sid.BranchId=@BranchId

union all
SELECT 
@UserName,@Branch,
'1' NoteNo,'2' SubNoteNo,CurrencyValue,SDAmount,VATAmount,'SubForm-Ka'SubFormName
,'Export'Remarks 
,ItemNo
,SalesInvoiceNo
,'' 
from BureauSalesInvoiceDetails where 1=1  and  (post=@post1 or post=@post2)   and  Type in('Export') 
and TransactionType in('Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

) AS sal

LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
------ORDER BY sal.SalesInvoiceNo DESC

";
                    #endregion

                }
                else if (vm.NoteNo == 2)
                {
                    #region LineNo-2

                    sqlText = sqlText + @" 

select sal.*, p.ProductDescription, p.ProductName, sal.SalesInvoiceNo DetailRemarks  from (
select 
@UserName UserName,@Branch Branch,
'2' NoteNo,'1' SubNoteNo,sid.CurrencyValue TotalPrice,sid.VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
,'DeemExport'Remarks 
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode

from SalesInvoiceDetails sid
where 1=1 
  and  (sid.post=@post1 or sid.post=@post2)  
and sid.Type in('DeemExport')
and sid.TransactionType in('Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and sid.PeriodId=@PeriodId
------and sid.InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND sid.BranchId=@BranchId


union all
select 
@UserName,@Branch,
'2' NoteNo,'2' SubNoteNo, CurrencyValue,SDAmount,VATAmount,'SubForm-Ka'SubFormName
,'DeemExport'Remarks
,ItemNo
,SalesInvoiceNo 
,''
from BureauSalesInvoiceDetails where  1=1  and  (post=@post1 or post=@post2)  and Type in('DeemExport')
and TransactionType in('Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId

) AS sal
LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
------ORDER BY sal.SalesInvoiceNo DESC
";

                    #endregion

                }
                else if (vm.NoteNo == 3)
                {
                    #region LineNo-3

                    if (code.ToLower() == "gdic".ToLower())
                    {
                        #region Note-03 for GDIC

                        sqlText = sqlText + @" 

select sal.*, p.ProductDescription, p.ProductName, sal.SalesInvoiceNo DetailRemarks  
, sih.ImportIDExcel

from (
SELECT 
@UserName UserName,@Branch Branch,
--'3' NoteNo,'1' SubNoteNo,(ISNULL(sid.LeaderAmount,0)) TotalPrice, (sid.VATAmount) VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
--'3' NoteNo,'1' SubNoteNo,(sid.SubTotal-ISNULL(sid.NonLeaderAmount,0)) TotalPrice, 0 VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
'3' NoteNo,'1' SubNoteNo,(ISNULL(sid.LeaderAmount,0)) TotalPrice, 0 VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
,'Non VAT'Remarks 
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode
,sid.VATRate
,sid.SD SDRate
from SalesInvoiceDetails sid
where 1=1 
  and  (sid.post=@post1 or sid.post=@post2)   
and sid.Type in('NonVAT')
and sid.TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender'
,'Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')
and sid.PeriodId=@PeriodId
AND sid.BranchId=@BranchId
and sid.IsLeader='Y'

union all
SELECT 
@UserName UserName,@Branch Branch,
'3' NoteNo,'1' SubNoteNo,(ISNULL(sid.NonLeaderAmount,0)) TotalPrice, 0 VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
,'Non VAT'Remarks 
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode
,sid.VATRate
,sid.SD SDRate
from SalesInvoiceDetails sid
where 1=1 
  and  (sid.post=@post1 or sid.post=@post2)   
and sid.Type in('NonVAT')
and sid.TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')
and sid.PeriodId=@PeriodId
AND sid.BranchId=@BranchId
and sid.IsLeader='N'

) AS sal

LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
LEFT OUTER JOIN SalesInvoiceHeaders sih on sih.SalesInvoiceNo=sal.SalesInvoiceNo

";

                        #endregion

                    }
                    else
                    {
                        #region Note-03 for Others

                        sqlText = sqlText + @" 

select sal.*, p.ProductDescription, p.ProductName, sal.SalesInvoiceNo DetailRemarks  
, sih.ImportIDExcel

from (
SELECT 
@UserName UserName,@Branch Branch,
--'3' NoteNo,'1' SubNoteNo,((case when type = 'Retail' then 0 else sid.SubTotal end)+isnull(option2,0) -ISNULL(sid.NonLeaderAmount,0)) TotalPrice, (sid.VATAmount-ISNULL(NonLeaderVATAmount,0)) VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
'3' NoteNo,'1' SubNoteNo,((case when type = 'Retail' then 0 else sid.SubTotal end)+isnull(option2,0) -ISNULL(sid.NonLeaderAmount,0)) TotalPrice, 0 VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
,'Non VAT'Remarks 
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode
,sid.VATRate
,sid.SD SDRate
from SalesInvoiceDetails sid
where 1=1 
  and  (sid.post=@post1 or sid.post=@post2)   
and sid.Type in('NonVAT','Retail')
and sid.TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender'
,'Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale','TollFinishIssue')
and sid.PeriodId=@PeriodId
AND sid.BranchId=@BranchId
and ISNULL(sid.IsLeader,'NA') IN ('NA','Y')
AND isnull(SourcePaidQuantity,0) = 0



UNION ALL
SELECT 
@UserName,@Branch,
--'3' NoteNo,'2' SubNoteNo, SubTotal, SDAmount,VATAmount,'SubForm-Ka'SubFormName
'3' NoteNo,'2' SubNoteNo, SubTotal, SDAmount,0,'SubForm-Ka'SubFormName
,'NonVAT'Remarks 
,ItemNo
,SalesInvoiceNo
,''
,VATRate
,SD SDRate
from BureauSalesInvoiceDetails where  1=1  and  (post=@post1 or post=@post2)   and Type in('NonVAT')
and TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender'
,'Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale','TollFinishIssue')
and PeriodId=@PeriodId
AND BranchId=@BranchId


UNION ALL
SELECT 
@UserName UserName,@Branch Branch,
--'3' NoteNo,'3' SubNoteNo,(sid.SubTotal-sid.LeaderAmount) TotalPrice, (sid.VATAmount-LeaderVATAmount) VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
'3' NoteNo,'3' SubNoteNo,(sid.SubTotal-sid.LeaderAmount) TotalPrice, 0 VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName

,'Non VAT'Remarks 
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode
,sid.VATRate
,sid.SD SDRate
from SalesInvoiceDetails sid
where 1=1 
  and  (sid.post=@post1 or sid.post=@post2)   
and sid.Type in('NonVAT')
and sid.TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender'
,'Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale','TollFinishIssue')
and sid.PeriodId=@PeriodId
AND sid.BranchId=@BranchId
and ISNULL(sid.IsLeader,'NA') = 'N'
AND isnull(SourcePaidQuantity,0) = 0

) AS sal

LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
LEFT OUTER JOIN SalesInvoiceHeaders sih on sih.SalesInvoiceNo=sal.SalesInvoiceNo
------ORDER BY sal.SalesInvoiceNo DESC
";

                        #endregion
                    }

                    #endregion

                }
                else if (vm.NoteNo == 4)
                {
                    #region LineNo-4

                    if (code.ToLower() == "gdic".ToLower())
                    {
                        #region Note-04 for GDIC

                        sqlText = sqlText + @" 
select sal.*, p.ProductDescription, p.ProductName, sal.SalesInvoiceNo DetailRemarks  
, sih.ImportIDExcel
from (

SELECT 
@UserName UserName,@Branch Branch,
'4' NoteNo,'1' SubNoteNo,(ISNULL(sid.SubTotal,0)) TotalPrice,(sid.VATAmount) VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
,'Standard VAT' Remarks
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode
,sid.VATRate
,sid.SD SDRate
from SalesInvoiceDetails sid
where 1=1 
 and  (post=@post1 or post=@post2)  
and Type in('VAT')
and TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')--,'TollFinishIssue'
and sid.PeriodId=@PeriodId
AND sid.BranchId=@BranchId

) AS sal

LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
LEFT OUTER JOIN SalesInvoiceHeaders sih on sih.SalesInvoiceNo=sal.SalesInvoiceNo

";

                        #endregion

                    }
                    else
                    {
                        #region Note-04 for Others

                        sqlText = sqlText + @" 
select sal.*, p.ProductDescription, p.ProductName, sal.SalesInvoiceNo DetailRemarks  
, sih.ImportIDExcel
from (

SELECT 
@UserName UserName,@Branch Branch,
'4' NoteNo,'1' SubNoteNo,(sid.SubTotal-ISNULL(sid.NonLeaderAmount,0)) TotalPrice,(sid.VATAmount-ISNULL(sid.NonLeaderVATAmount,0)) VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
,'Standard VAT' Remarks
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode
,sid.VATRate
,sid.SD SDRate
from SalesInvoiceDetails sid
where 1=1 
 and  (post=@post1 or post=@post2)  
and Type in('VAT')
and TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')--,'TollFinishIssue'
and sid.PeriodId=@PeriodId
AND sid.BranchId=@BranchId
AND isnull(SourcePaidQuantity,0) = 0
and ISNULL(sid.IsLeader,'NA') IN ('NA','Y')
and ProductType is null

union all

SELECT 
@UserName UserName,@Branch Branch,
'4' NoteNo,'1' SubNoteNo,(sid.SubTotal-ISNULL(sid.NonLeaderAmount,0)) TotalPrice,(sid.VATAmount-ISNULL(sid.NonLeaderVATAmount,0)) VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
,'Standard VAT' Remarks
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode
,sid.VATRate
,sid.SD SDRate
from SalesInvoiceDetails sid
where 1=1 
 and  (post=@post1 or post=@post2)  
and Type in('VAT')
and TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')--,'TollFinishIssue'
and sid.PeriodId=@PeriodId
AND sid.BranchId=@BranchId
AND isnull(SourcePaidQuantity,0) = 0
and ISNULL(sid.IsLeader,'NA') IN ('NA','Y')
and ProductType is not  null
and ProductType in ('R','P')

union all
select 
@UserName,@Branch,
'4' NoteNo,'2' SubNoteNo,SubTotal,VATAmount, SDAmount, 'SubForm-Ka'SubFormName 
,'Standard VAT'Remarks  
,ItemNo
,SalesInvoiceNo
,''
,VATRate
,SD SDRate
from BureauSalesInvoiceDetails where  1=1  and  (post=@post1 or post=@post2)   and Type in('VAT')
and TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')
and PeriodId=@PeriodId
AND BranchId=@BranchId


union all
SELECT 
@UserName UserName,@Branch Branch,
'4' NoteNo,'3' SubNoteNo,(sid.SubTotal-sid.LeaderAmount) TotalPrice,(sid.VATAmount-sid.LeaderVATAmount) VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
,'Standard VAT' Remarks
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode
,sid.VATRate
,sid.SD SDRate
from SalesInvoiceDetails sid
where 1=1 
 and  (post=@post1 or post=@post2)  
and Type in('VAT')
and TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')--,'TollFinishIssue'
and sid.PeriodId=@PeriodId
AND sid.BranchId=@BranchId
AND isnull(SourcePaidQuantity,0) = 0
and ISNULL(sid.IsLeader,'NA') ='N'
and ProductType is null

----------------------------------
--------For Seven Circle Start----------
----------------------------------

union all
select 
@UserName,@Branch,
'4' NoteNo,'2' SubNoteNo,SubTotal,VATAmount, SDAmount, 'SubForm-Ka'SubFormName 
,'Standard VAT'Remarks  
,ItemNo
,td.TollNo
,sd.HSCode ProductCode
,sd.VATRate
,sd.SD SDRate
from SalesInvoiceDetails sd 
left outer join Toll6_3InvoiceDetails td on td.SalesInvoiceNo=sd.SalesInvoiceNo
where  1=1 and sd.Type in('VAT')
and sd.SalesInvoiceNo in(
select SalesInvoiceNo from Toll6_3InvoiceDetails where TollNo in(
select TollNo from Toll6_3Invoices 
where 1=1 
and  (post=@post1 or post=@post2)  
and TransactionType in('Contractor63')
and PeriodId=@PeriodId
AND BranchId=@BranchId
)
)

----------------------------------
--------For Seven Circle End----------
----------------------------------

) AS sal

LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
LEFT OUTER JOIN SalesInvoiceHeaders sih on sih.SalesInvoiceNo=sal.SalesInvoiceNo
----ORDER BY sal.SalesInvoiceNo DESC
";

                        #endregion

                    }

                    #endregion
                }
                else if (vm.NoteNo == 5)
                {
                    #region LineNo-5

                    sqlText = sqlText + @" 
select sal.*, p.ProductDescription, p.ProductName, sal.SalesInvoiceNo DetailRemarks  from (

select 
@UserName UserName,@Branch Branch,
'5' NoteNo,'1' SubNoteNo,sid.SubTotal TotalPrice,sid.VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
,'MRP Rate'Remarks
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode
,sid.VATRate
,sid.SD SDRate
from SalesInvoiceDetails sid
where 1=1 
  and  (sid.post=@post1 or sid.post=@post2)  
and sid.Type in('MRPRate','MRPRate(SC)')
and sid.TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')
and sid.PeriodId=@PeriodId
------and sid.InvoiceDateTime >= @Datefrom and sid.InvoiceDateTime <dateadd(d,1,@Dateto)
AND sid.BranchId=@BranchId


union all
select 
@UserName,@Branch,
'5' NoteNo,'2' SubNoteNo, SubTotal, SDAmount, VATAmount,'SubForm-Ka'SubFormName 
,'MRP Rate'Remarks  
,ItemNo
,SalesInvoiceNo
,''
,VATRate
,SD SDRate
from BureauSalesInvoiceDetails where  1=1  and  (post=@post1 or post=@post2)   and Type in('MRPRate','MRPRate(SC)')
and TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')
and PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND BranchId=@BranchId


) AS sal

LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
------ORDER BY sal.SalesInvoiceNo DESC
";
                    #endregion
                }
                ////else if (vm.NoteNo == 6)   {}////Not Used
                else if (vm.NoteNo == 7)
                {
                    #region LineNo-7

                    sqlText = sqlText + @" 


select sal.*, p.ProductDescription, p.ProductName, sal.SalesInvoiceNo DetailRemarks  
, sih.ImportIDExcel
from (

select 
@UserName UserName,@Branch Branch,
'7' NoteNo,'1' SubNoteNo,sid.SubTotal TotalPrice,sid.VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
,'Other Rate'Remarks
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode
,sid.VATRate
,sid.SD SDRate
from SalesInvoiceDetails sid
where 1=1 
  and  (sid.post=@post1 or sid.post=@post2)  
and sid.Type in('OtherRate')
and sid.TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender'
,'Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale','TollFinishIssue')
and sid.PeriodId=@PeriodId
AND sid.BranchId=@BranchId

union all
select 
@UserName,@Branch,
'7' NoteNo,'2' SubNoteNo, SubTotal, SDAmount, VATAmount,'SubForm-Ka'SubFormName 
,'Other Rate'Remarks  
,ItemNo
,SalesInvoiceNo
,''
,VATRate
,SD SDRate
from BureauSalesInvoiceDetails where  1=1  and  (post=@post1 or post=@post2)   and Type in('OtherRate')
and TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading'
,'ExportTrading','TradingTender','InternalIssue','Service','TollSale','TollFinishIssue')
and PeriodId=@PeriodId
AND BranchId=@BranchId

) AS sal

LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
LEFT OUTER JOIN SalesInvoiceHeaders sih on sih.SalesInvoiceNo=sal.SalesInvoiceNo
------ORDER BY sal.SalesInvoiceNo DESC
";
                    #endregion

                }


                #region Summary SQL Group By

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 
) as Summary
Group BY

UserName
,Branch
,NoteNo
,SubNoteNo
,SubFormName
,Remarks
,ItemNo
,ProductDescription
,ProductName
,ProductCode
,VATRate
,SDRate
";
                }

                #endregion

                sqlText = sqlText + @" ORDER BY ProductName";

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }


                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn);
                objCommVAT19.CommandTimeout = 600;
                #endregion

                #region Parameter

                objCommVAT19.Parameters.AddWithValue("@BranchId", vm.BranchId);


                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT19.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT19.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT19.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT19.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT19.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormAPart3", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormAPart3", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        //// LineNo=1,2
        public DataTable VAT9_1_SubFormAPart3_Note_1_2(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string sqlText = "";
            DataTable dt = new DataTable();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region open connection and transaction

                ////currConn = _dbsqlConnection.GetConnection(connVM);
                ////if (currConn.State != ConnectionState.Open)
                ////{
                ////    currConn.Open();
                ////}

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
--------declare @BranchId int = 1


declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;

";

                #region Summary SQL

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 

SELECT DISTINCT
UserName
, Branch
, NoteNo
, SubNoteNo
 ,[Invoice/B/E No]
 ,[Date]
 ,OfficeCode
 ,BE_ItemNo
 ,CPC
, SUM(Assessablevalue)  Assessablevalue
, SUM(BasevalueofVAT)  BasevalueofVAT
, SUM(VAT)  VAT
, SUM(SD)  SD
, SUM(AT)  [AT]
, SubFormName
, Remarks
, ItemNo
, ProductDescription
, ProductName
, ProductCode
,'' DetailRemarks
,VATRate
,SDRate
FROM (

";
                }

                #endregion

                if (vm.NoteNo == 1)
                {
                    #region LineNo-1
                    if (vm.IsSaveSubForm)
                    {

                        sqlText = sqlText + @" insert into VAT9_1SubFormB UserName ,Branch ,NoteNo ,SubNoteNo ,[Invoice/B/E No] ,Date
 ,OfficeCode ,CPC   ,Assessablevalue ,BasevalueofVAT ,VAT ,SD ,AT ,SubFormName ,Remarks ,ItemNo ,ProductCode ,BE_ItemNo
 ,SalesInvoiceNo ,VATRate ,SDRate ,ProductDescription ,ProductName ,DetailRemarks ,PeriodID ,BranchId   ";

                    }
                    sqlText = sqlText + @" 

select sal.*, p.ProductName ProductDescription, p.ProductName,sih.ImportIDExcel DetailRemarks @SaveSubFormParam 
from (
SELECT 
@UserName UserName,@Branch Branch,
'1' NoteNo,'1' SubNoteNo,sih.LCNumber [Invoice/B/E No],sih.LCDate [Date]
,sih.CustomCode OfficeCode
,sid.CPCName CPC
,sid.CurrencyValue Assessablevalue
,sid.CurrencyValue+sid.SDAmount BasevalueofVAT
,sid.SDAmount SD
,sid.VATAmount VAT
, 0 [AT]
,'SubForm-Ka'SubFormName
,'Export'Remarks 
,sid.ItemNo
,sid.HSCode ProductCode
,sid.BEItemNo BE_ItemNo
,sid.SalesInvoiceNo
,sid.VATRate
,sid.SD SDRate

from SalesInvoiceDetails sid left outer join SalesInvoiceHeaders sih on sid.SalesInvoiceNo = sih.SalesInvoiceNo

where 1=1 
 and  (sid.post=@post1 or sid.post=@post2)  
and sid.Type in('Export') 
and sid.TransactionType in('Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and sid.PeriodId=@PeriodId
------and sid.InvoiceDateTime >= @Datefrom and sid.InvoiceDateTime <dateadd(d,1,@Dateto)
AND sid.BranchId>@BranchId

union all

SELECT 
@UserName,@Branch,
'1' NoteNo,'2' SubNoteNo
,sih.LCNumber
,sih.LCDate 
,sih.CustomCode OfficeCode
,'' CPC
,sid.CurrencyValue 
,sid.CurrencyValue+sid.SDAmount
,sid.SDAmount 
,sid.VATAmount 
, 0 
,'SubForm-Ka'SubFormName
,'Export'Remarks 
,ItemNo
,'' ProductCode
,'' BE_ItemNo
,sid.SalesInvoiceNo
,sid.VATRate
,sid.SD SDRate
from BureauSalesInvoiceDetails sid left outer join SalesInvoiceHeaders sih on sid.SalesInvoiceNo = sih.SalesInvoiceNo
where 1=1  
and  (sid.post=@post1 or sid.post=@post2)   and  Type in('Export') 
and sid.TransactionType in('Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and sid.PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND sid.BranchId>@BranchId

) AS sal

LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
left outer join SalesInvoiceHeaders sih on sal.SalesInvoiceNo = sih.SalesInvoiceNo

------ORDER BY sal.SalesInvoiceNo DESC

 
";
                    #endregion

                }
                else if (vm.NoteNo == 2)
                {
                    #region LineNo-2

                    sqlText = sqlText + @" 

select sal.*, p.ProductName ProductDescription, p.ProductName,sal.SalesInvoiceNo DetailRemarks  
from (
SELECT 
@UserName UserName,@Branch Branch,
'2' NoteNo,'1' SubNoteNo,sih.LCNumber [Invoice/B/E No],sih.LCDate [Date]
,sih.CustomCode OfficeCode
,sid.CPCName CPC
,sid.CurrencyValue Assessablevalue
,sid.CurrencyValue+sid.SDAmount BasevalueofVAT
,sid.SDAmount SD
,sid.VATAmount VAT
, 0 [AT]
,'SubForm-Ka'SubFormName
,'DeemExport'Remarks 
,sid.ItemNo
,sid.HSCode ProductCode
,sid.BEItemNo BE_ItemNo
,sid.SalesInvoiceNo
,sid.VATRate
,sid.SD SDRate
from SalesInvoiceDetails sid left outer join SalesInvoiceHeaders sih on sid.SalesInvoiceNo = sih.SalesInvoiceNo

where 1=1 
 and  (sid.post=@post1 or sid.post=@post2)  
and sid.Type in('DeemExport')
and sid.TransactionType in('Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and sid.PeriodId=@PeriodId
------and sid.InvoiceDateTime >= @Datefrom and sid.InvoiceDateTime <dateadd(d,1,@Dateto)
AND sid.BranchId>@BranchId

union all

SELECT 
@UserName,@Branch,
'1' NoteNo,'2' SubNoteNo
,sih.LCNumber
,sih.LCDate 
,sih.CustomCode OfficeCode
,'' CPC
,sid.CurrencyValue 
,sid.CurrencyValue+sid.SDAmount
,sid.SDAmount 
,sid.VATAmount 
, 0 
,'SubForm-Ka'SubFormName
,'Export'Remarks 
,ItemNo
,'' ProductCode
,'' BE_ItemNo
,sid.SalesInvoiceNo
,sid.VATRate
,sid.SD SDRate
from BureauSalesInvoiceDetails sid left outer join SalesInvoiceHeaders sih on sid.SalesInvoiceNo = sih.SalesInvoiceNo
where 1=1  
and  (sid.post=@post1 or sid.post=@post2)   and  Type in('DeemExport')
and sid.TransactionType in('Export','ExportServiceNS','ExportTender','ExportPackage','ExportTrading','ExportTradingTender','ExportService')
and sid.PeriodId=@PeriodId
------and InvoiceDateTime >= @Datefrom and InvoiceDateTime <dateadd(d,1,@Dateto)
AND sid.BranchId>@BranchId

) AS sal

LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
------ORDER BY sal.SalesInvoiceNo DESC

 
";

                    #endregion

                }



                #region Summary SQL Group By

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 
) as Summary
Group BY

UserName
,Branch
,NoteNo
,SubNoteNo
,SubFormName
,Remarks
,ItemNo
,ProductDescription
,ProductName
,ProductCode
,[Invoice/B/E No]
,[Date]
,OfficeCode
,BE_ItemNo
,CPC
,VATRate
,SDRate

";
                }

                #endregion

                sqlText = sqlText + @" ORDER BY ProductName";

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                if (vm.IsSaveSubForm)
                {
                    sqlText = sqlText.Replace("@SaveSubFormParam ", " ,@PeriodId PeriodID ,'0' BranchId");

                }
                else
                {
                    sqlText = sqlText.Replace("@SaveSubFormParam ", " ");
                }


                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.CommandTimeout = 300;

                #endregion

                #region Parameter

                objCommVAT19.Parameters.AddWithValue("@BranchId", vm.BranchId);


                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT19.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT19.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT19.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT19.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT19.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormAPart3", sqlex.ToString() + "\n" + sqlText);

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormAPart3", ex.ToString() + "\n" + sqlText);

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }

            }

            #endregion

            return dt;
        }

        //// LineNo=10,11,12,13,14,15,16,17,19,20,21,22
        public DataTable VAT9_1_SubFormAPart4(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();
                int CurrentPeriodId = Convert.ToInt32(Convert.ToDateTime(vm.PeriodName).ToString("MMyyyy"));

                string code = commonDal.settingValue("CompanyCode", "Code");
                string OldDBName = commonDal.settingValue("DatabaseName", "DatabaseName");
                string ClientFGReceiveIn9_1 = commonDal.settingValue("Toll", "ClientFGReceiveIn9_1", connVM);

                string OldsqlText = "";

                #region Statement

                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @BranchId as int
--------set @BranchId = 1

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;


";

                #region Summary SQL

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 

SELECT DISTINCT
UserName
, Branch
, NoteNo
, SubNoteNo
, SUM(TotalPrice)  TotalPrice
, SUM(VATAmount)  VATAmount
, SUM(SDAmount)  SDAmount
, SubFormName
, Remarks
, ItemNo
, ProductDescription
, ProductName
, ProductCode
, SerialNo DetailRemarks
,VATRate
,SDRate
FROM (

";
                }

                #endregion

                if (vm.NoteNo == 10)
                {
                    #region LineNo-10

                    sqlText = sqlText + @" 
select pur.*, p.ProductDescription, p.ProductName, pur.PurchaseInvoiceNo DetailRemarks,'' SerialNo from
(
select @UserName UserName,@Branch Branch,'10' NoteNo,'1' SubNoteNo
----------, pid.SubTotal TotalPrice
, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount 
,pid.SDAmount
,'SubForm-Ka'SubFormName
,'Local Purchase (Non-VAT)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('NonVAT')
and pid.TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService','PurchaseTollcharge')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all

select @UserName UserName,@Branch Branch,'10' NoteNo,'1' SubNoteNo
, pid.SubTotal TotalPrice
----------, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount 
,pid.SDAmount
,'SubForm-Ka'SubFormName
,'Local Purchase (Non-VAT)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('NonVAT')
and pid.TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService','PurchaseTollcharge')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')

/*
union all
select @UserName,@Branch,'10' NoteNo,'2' SubNoteNo,-1*(pid.SubTotal),1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end ),1*(pid.SDAmount),'SubForm-Ka'SubFormName
,'Local Purchase (Non-VAT)'Remarks  
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('NonVAT')
and pid.TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
*/
@OldDBsqlText

) as pur
LEFT OUTER JOIN Products p ON p.ItemNo = pur.ItemNo 
------order by pur.PurchaseInvoiceNo
";


                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'10' NoteNo,'1' SubNoteNo
, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount 
,pid.SDAmount
,'SubForm-Ka'SubFormName
,'Local Purchase (Non-VAT)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('NonVAT')
and pid.TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService','PurchaseTollcharge')
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all

select @UserName,@Branch,'10' NoteNo,'2' SubNoteNo,-1*(pid.SubTotal),1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end ),1*(pid.SDAmount),'SubForm-Ka'SubFormName
,'Local Purchase (Non-VAT)'Remarks  
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('NonVAT')
and pid.TransactionType in('PurchaseDN')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId

";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion

                }
                else if (vm.NoteNo == 11)
                {
                    #region LineNo-11

                    sqlText = sqlText + @" 
select pur.* from
(

select @UserName UserName,@Branch Branch,'11' NoteNo,'1' SubNoteNo
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
----------+pid.SDAmount
+pid.TVBAmount
) TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Import Purchase (Non-VAT)'Remarks  
, p.ProductDescription, p.ProductName, pid.HSCode ProductCode
, pid.PurchaseInvoiceNo DetailRemarks
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid 
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('NonVAT')
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId

@OldDBsqlText

------order by pid.PurchaseInvoiceNo
) as pur
";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'11' NoteNo,'1' SubNoteNo
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.TVBAmount
) TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Import Purchase (Non-VAT)'Remarks  
, p.ProductDescription, p.ProductName, pid.HSCode ProductCode
, pid.PurchaseInvoiceNo DetailRemarks
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid 
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('NonVAT')
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
--and pid.PeriodId=@PeriodId
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId

";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion

                }
                else if (vm.NoteNo == 12)
                {
                    #region LineNo-12

                    sqlText = sqlText + @" 
select pur.*, p.ProductDescription, p.ProductName, pur.PurchaseInvoiceNo DetailRemarks,'' SerialNo,pih.ImportIDExcel from
(
select @UserName UserName,@Branch Branch,'12' NoteNo,'1' SubNoteNo
----------, pid.SubTotal TotalPrice
, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount 

,pid.SDAmount

,'SubForm-Ka'SubFormName
,'Local Purchase (Exempted)'Remarks, 
pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('Exempted')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all

select @UserName UserName,@Branch Branch,'12' NoteNo,'1' SubNoteNo
, pid.SubTotal TotalPrice
----------, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount 

,pid.SDAmount

,'SubForm-Ka'SubFormName
,'Local Purchase (Exempted)'Remarks, 
pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('Exempted')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')


/*
union all

select @UserName,@Branch,'12' NoteNo,'2' SubNoteNo,-1*(pid.SubTotal),1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end ),1*(pid.SDAmount),'SubForm-Ka'SubFormName
,'Local Purchase (Exempted)'Remarks
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and pid.Type in('Exempted')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
*/
@OldDBsqlText

) as pur
LEFT OUTER JOIN Products p ON p.ItemNo = pur.ItemNo 
LEFT OUTER JOIN PurchaseInvoiceHeaders pih ON pih.PurchaseInvoiceNo = pur.PurchaseInvoiceNo 
------order by pur.PurchaseInvoiceNo
";


                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'12' NoteNo,'1' SubNoteNo
, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount 
,pid.SDAmount
,'SubForm-Ka'SubFormName
,'Local Purchase (Exempted)'Remarks, 
pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('Exempted')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

/*
union all
select @UserName,@Branch,'12' NoteNo,'2' SubNoteNo,-1*(pid.SubTotal),1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end ),1*(pid.SDAmount),'SubForm-Ka'SubFormName
,'Local Purchase (Exempted)'Remarks
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from @db.dbo.PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and pid.Type in('Exempted')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
*/
";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion

                }
                else if (vm.NoteNo == 13)
                {
                    #region LineNo-13

                    sqlText = sqlText + @" 
select pur.* from
(

select @UserName UserName,@Branch Branch,'13' NoteNo,'1' SubNoteNo
,
(pid.SubTotal
+pid.CDAmount
+pid.RDAmount
----------+pid.SDAmount
+pid.TVBAmount
) TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Import Purchase (Exempted)'Remarks  
, p.ProductDescription, p.ProductName, pid.HSCode ProductCode
, pid.PurchaseInvoiceNo DetailRemarks 
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid 
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
   and  (pid.post=@post1 or pid.post=@post2)  
and Type in('Exempted')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId

@OldDBsqlText

------order by pid.PurchaseInvoiceNo
) as pur
";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"
union all
select @UserName UserName,@Branch Branch,'13' NoteNo,'1' SubNoteNo
,
(pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.TVBAmount
) TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Import Purchase (Exempted)'Remarks  
, p.ProductDescription, p.ProductName, pid.HSCode ProductCode
, pid.PurchaseInvoiceNo DetailRemarks 
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid 
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
   and  (pid.post=@post1 or pid.post=@post2)  
and Type in('Exempted')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId

";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion
                }
                else if (vm.NoteNo == 14)
                {
                    #region LineNo-14

                    sqlText = sqlText + @" 
select pur.*, p.ProductDescription, p.ProductName, pur.PurchaseInvoiceNo DetailRemarks,pih.ImportIDExcel,p.SerialNo SerialNo  from 
(
select @UserName UserName,@Branch Branch,'14' NoteNo,'1' SubNoteNo
----------, pid.SubTotal TotalPrice
, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Local Purchase (Standard VAT)' Remarks, 
pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('VAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService','PurchaseTollcharge' @ClientFGReceiveIn9_1)--,'TollReceive'
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all

select @UserName UserName,@Branch Branch,'14' NoteNo,'1' SubNoteNo
,pid.SubTotal TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Local Purchase (Standard VAT)' Remarks, 
pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('VAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService','PurchaseTollcharge' @ClientFGReceiveIn9_1)--,'TollReceive'
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')
/*
union all

select @UserName,@Branch,'14' NoteNo,'2' SubNoteNo,-1*pid.SubTotal
,1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end )
,1*(pid.SDAmount),'SubForm-Ka'SubFormName
,'Local Purchase (Standard VAT)'Remarks
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('VAT')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
*/
@OldDBsqlText

";
                    #region Client 6.3

                    sqlText = sqlText + @" 
union all

select @UserName UserName,@Branch Branch,'14' NoteNo,'3' SubNoteNo
,clnd.SubTotal TotalPrice
,clnd.VATAmount 
,clnd.SDAmount
,'SubForm-Ka'SubFormName
,'Local Purchase (Standard VAT)' Remarks, 
clnd.InvoiceNo
,clnd.ItemNo
,clnd.HSCode ProductCode
,clnd.VATRate
,clnd.SDRate
from Client6_3Details clnd
where  1=1  and  (post=@post1 or post=@post2)  
and TransactionType in('Other')
and clnd.PeriodId=@PeriodId
AND clnd.BranchId=@BranchId

";

                    #endregion

                    sqlText = sqlText + @" 

) as pur
LEFT OUTER JOIN Products p ON p.ItemNo = pur.ItemNo 
LEFT OUTER JOIN PurchaseInvoiceHeaders pih ON pih.PurchaseInvoiceNo = pur.PurchaseInvoiceNo 

------order by pur.PurchaseInvoiceNo
";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'14' NoteNo,'1' SubNoteNo
, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Local Purchase (Standard VAT)' Remarks, 
pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('VAT')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS' ,'InputService','PurchaseTollcharge' @ClientFGReceiveIn9_1)--,'TollReceive'
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')
/*
union all
select @UserName,@Branch,'14' NoteNo,'2' SubNoteNo,-1*pid.SubTotal
,1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end )
,1*(pid.SDAmount),'SubForm-Ka'SubFormName
,'Local Purchase (Standard VAT)'Remarks
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from @db.dbo.PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('VAT')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
*/
";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    if (ClientFGReceiveIn9_1 == "Y")
                    {
                        sqlText = sqlText.Replace("@ClientFGReceiveIn9_1", ",'TollReceive'");
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@ClientFGReceiveIn9_1", "");
                    }

                    #endregion
                }
                else if (vm.NoteNo == 15)
                {
                    #region LineNo-15

                    sqlText = sqlText + @" 

select pur.* from (
select @UserName UserName,@Branch Branch,'15' NoteNo,'1' SubNoteNo
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
----------+pid.SDAmount
+pid.TVBAmount @Note15SDAmount
) TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Import Purchase (Standard VAT)'Remarks  
, p.ProductDescription, p.ProductName,pid.HSCode ProductCode
, pid.PurchaseInvoiceNo DetailRemarks 
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid 
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
   and  (pid.post=@post1 or pid.post=@post2)  
and Type in('VAT')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId

@OldDBsqlText

) as pur
----order by pid.PurchaseInvoiceNo

";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'15' NoteNo,'1' SubNoteNo
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.TVBAmount @Note15SDAmount
) TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Import Purchase (Standard VAT)'Remarks  
, p.ProductDescription, p.ProductName,pid.HSCode ProductCode
, pid.PurchaseInvoiceNo DetailRemarks 
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid 
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and Type in('VAT')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter')
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    if (CurrentPeriodId >= 072021)
                    {
                        string cValue = @"+SDAmount";

                        sqlText = sqlText.Replace("@Note15SDAmount", cValue);
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@Note15SDAmount", "");
                    }
                    #endregion

                }
                else if (vm.NoteNo == 16)
                {
                    #region LineNo-16

                    sqlText = sqlText + @" 
select pur.*, p.ProductDescription, p.ProductName, pur.PurchaseInvoiceNo DetailRemarks,pih.ImportIDExcel,p.SerialNo SerialNo  from
(
select @UserName UserName,@Branch Branch,'16' NoteNo,'1' SubNoteNo
----------, pid.SubTotal TotalPrice
, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount 
,pid.SDAmount

,'SubForm-Ka'SubFormName
,'Local Purchase (Other Rate)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('OtherRate')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all

select @UserName UserName,@Branch Branch,'16' NoteNo,'1' SubNoteNo
, pid.SubTotal TotalPrice
----------, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount 
,pid.SDAmount

,'SubForm-Ka'SubFormName
,'Local Purchase (Other Rate)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('OtherRate')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')
/*
union all

select @UserName,@Branch,'16' NoteNo,'2' SubNoteNo
----------,-1*(pid.SubTotal)
,-1*(case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end) TotalPrice
,-1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end )
,-1*(pid.SDAmount)
,'SubForm-Ka'SubFormName
,'Local Purchase (Other Rate)'Remarks  
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('OtherRate')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
*/
@OldDBsqlText

) as pur
LEFT OUTER JOIN Products p ON p.ItemNo = pur.ItemNo 
LEFT OUTER JOIN PurchaseInvoiceHeaders pih ON pih.PurchaseInvoiceNo = pur.PurchaseInvoiceNo 

------order by pur.PurchaseInvoiceNo
";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'16' NoteNo,'1' SubNoteNo
, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount 
,pid.SDAmount
,'SubForm-Ka'SubFormName
,'Local Purchase (Other Rate)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('OtherRate')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')
/*
union all
select @UserName,@Branch,'16' NoteNo,'2' SubNoteNo
,-1*(case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end) TotalPrice
,-1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end )
,-1*(pid.SDAmount)
,'SubForm-Ka'SubFormName
,'Local Purchase (Other Rate)'Remarks  
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from @db.dbo.PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('OtherRate')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
*/
";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion

                }
                else if (vm.NoteNo == 17)
                {
                    #region LineNo-17

                    sqlText = sqlText + @" 

select pur.* from (
select @UserName UserName,@Branch Branch,'17' NoteNo,'1' SubNoteNo
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
----------+pid.SDAmount
+pid.TVBAmount
) TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Import Purchase (Other Rate)'Remarks  
, p.ProductDescription, p.ProductName,pid.HSCode ProductCode
, pid.PurchaseInvoiceNo DetailRemarks 
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid 
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
   and  (pid.post=@post1 or pid.post=@post2)  
and Type in('OtherRate')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'

AND pid.BranchId=@BranchId

@OldDBsqlText

------order by pid.PurchaseInvoiceNo
) as pur

";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'17' NoteNo,'1' SubNoteNo
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.TVBAmount
) TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Import Purchase (Other Rate)'Remarks  
, p.ProductDescription, p.ProductName,pid.HSCode ProductCode
, pid.PurchaseInvoiceNo DetailRemarks 
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid 
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
   and  (pid.post=@post1 or pid.post=@post2)  
and Type in('OtherRate')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'

AND pid.BranchId=@BranchId

";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion

                }
                else if (vm.NoteNo == 19)
                {
                    #region LineNo-19

                    sqlText = sqlText + @" 
select pur.*, p.ProductDescription, p.ProductName, pur.PurchaseInvoiceNo DetailRemarks,'' SerialNo  from
(
select @UserName UserName,@Branch Branch,'19' NoteNo,'1' SubNoteNo

----------, pid.SubTotal TotalPrice
, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount

,pid.SDAmount

,'SubForm-Ka'SubFormName
,'Local Purchase (Turnover)'Remarks 
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('Turnover')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all

select @UserName UserName,@Branch Branch,'19' NoteNo,'1' SubNoteNo
, pid.SubTotal TotalPrice
----------, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount

,pid.SDAmount
,'SubForm-Ka'SubFormName
,'Local Purchase (Turnover)'Remarks 
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('Turnover')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')

/*
union all
select @UserName,@Branch,'19' NoteNo,'2' SubNoteNo,-1*(pid.SubTotal),1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end ),1*(pid.SDAmount),'SubForm-Ka'SubFormName
,'Local Purchase (Turnover)'Remarks 
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('Turnover')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
*/
@OldDBsqlText

) as pur
LEFT OUTER JOIN Products p ON p.ItemNo = pur.ItemNo 
------order by pur.PurchaseInvoiceNo
";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'19' NoteNo,'1' SubNoteNo
, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount
,pid.SDAmount
,'SubForm-Ka'SubFormName
,'Local Purchase (Turnover)'Remarks 
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('Turnover')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')
/*
union all
select @UserName,@Branch,'19' NoteNo,'2' SubNoteNo,-1*(pid.SubTotal),1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end ),1*(pid.SDAmount),'SubForm-Ka'SubFormName
,'Local Purchase (Turnover)'Remarks 
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from @db.dbo.PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('Turnover')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'

AND pid.BranchId=@BranchId
*/
";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion

                }
                else if (vm.NoteNo == 20)
                {
                    #region LineNo-20

                    sqlText = sqlText + @" 
select pur.*, p.ProductDescription, p.ProductName, pur.PurchaseInvoiceNo DetailRemarks,'' SerialNo  from
(
select @UserName UserName,@Branch Branch,'20' NoteNo,'1' SubNoteNo

----------, pid.SubTotal TotalPrice
, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount
 
,pid.SDAmount

,'SubForm-Ka'SubFormName
,'Local Purchase (Un-Register Vendor)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('UnRegister')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

union all

select @UserName UserName,@Branch Branch,'20' NoteNo,'1' SubNoteNo
, pid.SubTotal TotalPrice
----------, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount
,pid.SDAmount
,'SubForm-Ka'SubFormName
,'Local Purchase (Un-Register Vendor)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('UnRegister')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart >= '2021-12-01')

/*
union all

select @UserName,@Branch,'20' NoteNo,'2' SubNoteNo,-1*(pid.SubTotal),1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end ),1*(pid.SDAmount),'SubForm-Ka'SubFormName
,'Local Purchase (Un-Register Vendor)'Remarks 
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('UnRegister')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'

AND pid.BranchId=@BranchId
*/
@OldDBsqlText

) as pur
LEFT OUTER JOIN Products p ON p.ItemNo = pur.ItemNo 
------order by pur.PurchaseInvoiceNo
";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'20' NoteNo,'1' SubNoteNo
, case when pid.RebateAmount>0 then (pid.SubTotal*pid.RebateRate/100) else  pid.SubTotal end TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount
,pid.SDAmount
,'SubForm-Ka'SubFormName
,'Local Purchase (Un-Register Vendor)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('UnRegister')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')

/*
union all
select @UserName,@Branch,'20' NoteNo,'2' SubNoteNo,-1*(pid.SubTotal),1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end ),1*(pid.SDAmount),'SubForm-Ka'SubFormName
,'Local Purchase (Un-Register Vendor)'Remarks 
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from @db.dbo.PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('UnRegister')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
*/

";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion

                }
                else if (vm.NoteNo == 21)
                {
                    #region LineNo-21

                    sqlText = sqlText + @" 
select pur.*, p.ProductDescription, p.ProductName, pur.PurchaseInvoiceNo DetailRemarks,pih.ImportIDExcel,'' SerialNo  from
(
select @UserName UserName,@Branch Branch,'21' NoteNo,'1' SubNoteNo,pid.SubTotal TotalPrice, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Local Purchase (Non-Rebate)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'

AND pid.BranchId=@BranchId
/*
union all
select @UserName,@Branch,'21' NoteNo,'2' SubNoteNo,-1*(pid.SubTotal),1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end),1*(pid.SDAmount),'SubForm-Ka'SubFormName
,'Local Purchase (Non-Rebate)'Remarks 
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('NonRebate','Local-NonRebate','Import-NonRebate')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId

union all

select @UserName UserName,@Branch Branch,'21' NoteNo,'3' SubNoteNo
, pid.SubTotal-(pid.SubTotal*pid.RebateRate/100) TotalPrice
, (pid.VATAmount - pid.RebateAmount ) VATAmount 
, 0 SDAmount
,'SubForm-Ka'SubFormName
,'Local Purchase (Non-Rebate)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type not in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType not in('PurchaseDN','PurchaseReturn')
and pid.PeriodId=@PeriodId
AND pid.BranchId=@BranchId
and RebateAmount>0
AND (pid.SubTotal-(pid.SubTotal*pid.RebateRate/100)) > 0
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')
*/
@OldDBsqlText

) as pur
LEFT OUTER JOIN Products p ON p.ItemNo = pur.ItemNo 
LEFT OUTER JOIN PurchaseInvoiceHeaders pih ON pih.PurchaseInvoiceNo = pur.PurchaseInvoiceNo 

------order by pur.PurchaseInvoiceNo
";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'21' NoteNo,'1' SubNoteNo,pid.SubTotal TotalPrice, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Local Purchase (Non-Rebate)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
/*
union all
select @UserName,@Branch,'21' NoteNo,'2' SubNoteNo,-1*(pid.SubTotal),1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end),1*(pid.SDAmount),'SubForm-Ka'SubFormName
,'Local Purchase (Non-Rebate)'Remarks 
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from @db.dbo.PurchaseInvoiceDetails pid 
where  1=1  and  (post=@post1 or post=@post2)  
and Type in('NonRebate','Local-NonRebate','Import-NonRebate')
and TransactionType in('PurchaseDN')--,'PurchaseReturn'
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId

union all

select @UserName UserName,@Branch Branch,'21' NoteNo,'3' SubNoteNo
, pid.SubTotal-(pid.SubTotal*pid.RebateRate/100) TotalPrice
, (pid.VATAmount - pid.RebateAmount ) VATAmount 
, 0 SDAmount
,'SubForm-Ka'SubFormName
,'Local Purchase (Non-Rebate)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.HSCode ProductCode
from @db.dbo.PurchaseInvoiceDetails pid
where  1=1  and  (post=@post1 or post=@post2)  
and Type not in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType not in('PurchaseDN','PurchaseReturn')
and pid.PeriodId=@PeriodId
AND pid.BranchId=@BranchId
and RebateAmount>0
AND (pid.SubTotal-(pid.SubTotal*pid.RebateRate/100)) > 0
 AND pid.RebatePeriodID in (select PeriodID from FiscalYear where PeriodStart < '2021-12-01')
*/
";
                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion


                    #endregion

                }
                else if (vm.NoteNo == 22)
                {
                    #region LineNo-22

                    sqlText = sqlText + @" 

select pur.* from(
select @UserName UserName,@Branch Branch,'22' NoteNo,'1' SubNoteNo
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
----------+pid.SDAmount
+pid.TVBAmount
) TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Import Purchase (Non-Rebate)'Remarks  
, p.ProductDescription, p.ProductName,pid.HSCode ProductCode
, pid.PurchaseInvoiceNo DetailRemarks 
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid 
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'

AND pid.BranchId=@BranchId

@OldDBsqlText

------order by pid.PurchaseInvoiceNo
) as pur
";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'22' NoteNo,'1' SubNoteNo
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.TVBAmount
) TotalPrice
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount,'SubForm-Ka'SubFormName
,'Import Purchase (Non-Rebate)'Remarks  
, p.ProductDescription, p.ProductName,pid.HSCode ProductCode
, pid.PurchaseInvoiceNo DetailRemarks 
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid 
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter')
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId=@BranchId
";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion

                }

                #region Summary SQL Group By

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 
) as Summary
Group BY

UserName
,Branch
,NoteNo
,SubNoteNo
,SubFormName
,Remarks
,ItemNo
,ProductDescription
,ProductName
,ProductCode
,SerialNo
,VATRate
,SDRate

";
                }

                #endregion

                sqlText = sqlText + @" ORDER BY ProductName";

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }
                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter
                objCommVAT19.Parameters.AddWithValue("@BranchId", vm.BranchId);

                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT19.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT19.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT19.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT19.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT19.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormAPart4", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormAPart4", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        //// LineNo=11,13,15,17,22
        public DataTable VAT9_1_SubFormAPart4_2021(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code");
                string OldDBName = commonDal.settingValue("DatabaseName", "DatabaseName");
                int CurrentPeriodId = Convert.ToInt32(Convert.ToDateTime(vm.PeriodName).ToString("MMyyyy"));
                string OldsqlText = "";

                #region Statement

                sqlText = @" ";

                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @BranchId as int
--------set @BranchId = 1

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;

";

                #region Summary SQL

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 

SELECT DISTINCT
UserName
,Branch
,NoteNo
,SubNoteNo
--,SubNoteNo
,[Invoice/B/E No]
,[Date]
,OfficeCode
,BE_ItemNo
,CPC
,SUM(Assessablevalue)  Assessablevalue
,SUM(BasevalueofVAT)  BasevalueofVAT
,SUM(VAT)  VAT
,SUM(SD)  SD
,SUM(AT)  [AT]
,SubFormName
,Remarks
,ItemNo
,ProductDescription
,ProductName
,ProductCode
,'' DetailRemarks
,VATRate
,SDRate
FROM (

";
                }

                #endregion

                if (vm.NoteNo == 11)
                {
                    #region LineNo-11

                    sqlText = sqlText + @" 
select pur.* from
(

select @UserName UserName,@Branch Branch,'11' NoteNo,'1' SubNoteNo
, pih.BENumber [Invoice/B/E No]
, pih.InvoiceDateTime [Date]
, pih.CustomCode OfficeCode
, pid.CPCName CPC
, pid.AssessableValue
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.SDAmount
+pid.TVBAmount
) BasevalueofVAT

, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VAT
,pid.SDAmount SD
,pid.ATVAmount [AT]
,'SubForm-Ka'SubFormName
,'Import Purchase (Non-VAT)'Remarks  
, p.ProductName, p.ProductName ProductDescription, pid.HSCode ProductCode, pid.BEItemNo BE_ItemNo
, pid.PurchaseInvoiceNo DetailRemarks
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid left outer join PurchaseInvoiceHeaders pih on pid.PurchaseInvoiceNo = pih.PurchaseInvoiceNo
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('NonVAT')
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId>@BranchId

@OldDBsqlText

------order by pid.PurchaseInvoiceNo
) as pur 

";


                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'11' NoteNo,'1' SubNoteNo
, pih.BENumber [Invoice/B/E No]
, pih.InvoiceDateTime [Date]
, pih.CustomCode OfficeCode
, pid.CPCName CPC
, pid.AssessableValue
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.SDAmount
+pid.TVBAmount
) BasevalueofVAT

, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VAT
,pid.SDAmount SD
,pid.ATVAmount [AT]
,'SubForm-Ka'SubFormName
,'Import Purchase (Non-VAT)'Remarks  
, p.ProductName, p.ProductName ProductDescription, pid.HSCode ProductCode, pid.BEItemNo BE_ItemNo
, pid.PurchaseInvoiceNo DetailRemarks
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid left outer join PurchaseInvoiceHeaders pih on pid.PurchaseInvoiceNo = pih.PurchaseInvoiceNo
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('NonVAT')
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
and pid.PeriodId=@PeriodId
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId>@BranchId

";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion

                }

                else if (vm.NoteNo == 13)
                {
                    #region LineNo-13

                    sqlText = sqlText + @" 
select pur.* from
(

select @UserName UserName,@Branch Branch,'13' NoteNo,'1' SubNoteNo
, pih.BENumber [Invoice/B/E No]
, pih.InvoiceDateTime [Date]
, pih.CustomCode OfficeCode
, pid.CPCName CPC
, pid.AssessableValue
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.SDAmount
+pid.TVBAmount
) BasevalueofVAT

, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VAT
,pid.SDAmount SD
,pid.ATVAmount [AT]
,'SubForm-Ka'SubFormName
,'Import Purchase (Exempted)'Remarks  
, p.ProductName, p.ProductName ProductDescription, pid.HSCode ProductCode, pid.BEItemNo BE_ItemNo
, pid.PurchaseInvoiceNo DetailRemarks
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid left outer join PurchaseInvoiceHeaders pih on pid.PurchaseInvoiceNo = pih.PurchaseInvoiceNo
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('Exempted')
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId>@BranchId

@OldDBsqlText

------order by pid.PurchaseInvoiceNo
) as pur 

";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'13' NoteNo,'1' SubNoteNo
, pih.BENumber [Invoice/B/E No]
, pih.InvoiceDateTime [Date]
, pih.CustomCode OfficeCode
, pid.CPCName CPC
, pid.AssessableValue
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.SDAmount
+pid.TVBAmount
) BasevalueofVAT

, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VAT
,pid.SDAmount SD
,pid.ATVAmount [AT]
,'SubForm-Ka'SubFormName
,'Import Purchase (Exempted)'Remarks  
, p.ProductName, p.ProductName ProductDescription, pid.HSCode ProductCode, pid.BEItemNo BE_ItemNo
, pid.PurchaseInvoiceNo DetailRemarks
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid left outer join PurchaseInvoiceHeaders pih on pid.PurchaseInvoiceNo = pih.PurchaseInvoiceNo
LEFT OUTER JOIN @db.dbo.Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('Exempted')
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId>@BranchId

";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion
                }

                else if (vm.NoteNo == 15)
                {
                    #region LineNo-15

                    sqlText = sqlText + @" 


select pur.* from
(

select @UserName UserName,@Branch Branch,'15' NoteNo,'1' SubNoteNo
, pih.BENumber [Invoice/B/E No]
, pih.InvoiceDateTime [Date]
, pih.CustomCode OfficeCode
, pid.CPCName CPC
, pid.AssessableValue
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.SDAmount
+pid.TVBAmount
) BasevalueofVAT

, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VAT
,pid.SDAmount SD
,pid.ATVAmount [AT]
,'SubForm-Ka'SubFormName
,'Import Purchase (Standard VAT)'Remarks  
, p.ProductName, p.ProductName ProductDescription, pid.HSCode ProductCode, pid.BEItemNo BE_ItemNo
, pid.PurchaseInvoiceNo DetailRemarks
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid left outer join PurchaseInvoiceHeaders pih on pid.PurchaseInvoiceNo = pih.PurchaseInvoiceNo
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('VAT')
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
--AND pid.BranchId>@BranchId
AND pid.BranchId=@BranchId

@OldDBsqlText

------order by pid.PurchaseInvoiceNo
) as pur 
 
----order by pid.PurchaseInvoiceNo

";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'15' NoteNo,'1' SubNoteNo
, pih.BENumber [Invoice/B/E No]
, pih.InvoiceDateTime [Date]
, pih.CustomCode OfficeCode
, pid.CPCName CPC
, pid.AssessableValue
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.SDAmount
+pid.TVBAmount
) BasevalueofVAT
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VAT
,pid.SDAmount SD
,pid.ATVAmount [AT]
,'SubForm-Ka'SubFormName
,'Import Purchase (Standard VAT)'Remarks  
, p.ProductName, p.ProductName ProductDescription, pid.HSCode ProductCode, pid.BEItemNo BE_ItemNo
, pid.PurchaseInvoiceNo DetailRemarks
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid left outer join PurchaseInvoiceHeaders pih on pid.PurchaseInvoiceNo = pih.PurchaseInvoiceNo
LEFT OUTER JOIN @db.dbo.Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('VAT')
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
--AND pid.BranchId>@BranchId
AND pid.BranchId=@BranchId

";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion

                }
                else if (vm.NoteNo == 17)
                {
                    #region LineNo-17

                    sqlText = sqlText + @" 

select pur.* from
(

select @UserName UserName,@Branch Branch,'17' NoteNo,'1' SubNoteNo
, pih.BENumber [Invoice/B/E No]
, pih.InvoiceDateTime [Date]
, pih.CustomCode OfficeCode
, pid.CPCName CPC
, pid.AssessableValue
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.SDAmount
+pid.TVBAmount
) BasevalueofVAT

, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VAT
,pid.SDAmount SD
,pid.ATVAmount [AT]
,'SubForm-Ka'SubFormName
,'Import Purchase (Other Rate)'Remarks  
, p.ProductName, p.ProductName ProductDescription, pid.HSCode ProductCode, pid.BEItemNo BE_ItemNo
, pid.PurchaseInvoiceNo DetailRemarks
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid left outer join PurchaseInvoiceHeaders pih on pid.PurchaseInvoiceNo = pih.PurchaseInvoiceNo
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('OtherRate')
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
and pid.PeriodId=@PeriodId
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId>@BranchId

@OldDBsqlText

------order by pid.PurchaseInvoiceNo
) as pur
 
";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'17' NoteNo,'1' SubNoteNo
, pih.BENumber [Invoice/B/E No]
, pih.InvoiceDateTime [Date]
, pih.CustomCode OfficeCode
, pid.CPCName CPC
, pid.AssessableValue
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.SDAmount
+pid.TVBAmount
) BasevalueofVAT
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VAT
,pid.SDAmount SD
,pid.ATVAmount [AT]
,'SubForm-Ka'SubFormName
,'Import Purchase (Other Rate)'Remarks  
, p.ProductName, p.ProductName ProductDescription, pid.HSCode ProductCode, pid.BEItemNo BE_ItemNo
, pid.PurchaseInvoiceNo DetailRemarks
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid left outer join PurchaseInvoiceHeaders pih on pid.PurchaseInvoiceNo = pih.PurchaseInvoiceNo
LEFT OUTER JOIN @db.dbo.Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('OtherRate')
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
AND pid.BranchId>@BranchId

";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion

                }

                else if (vm.NoteNo == 22)
                {
                    #region LineNo-22

                    sqlText = sqlText + @" 

select pur.* from
(

select @UserName UserName,@Branch Branch,'22' NoteNo,'1' SubNoteNo
, pih.BENumber [Invoice/B/E No]
, pih.InvoiceDateTime [Date]
, pih.CustomCode OfficeCode
, pid.CPCName CPC
, pid.AssessableValue
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.SDAmount
+pid.TVBAmount
) BasevalueofVAT

, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VAT
,pid.SDAmount SD
,pid.ATVAmount [AT]
,'SubForm-Ka'SubFormName
,'Import Purchase (Non-Rebate)'Remarks  
, p.ProductName, p.ProductName ProductDescription, pid.HSCode ProductCode, pid.BEItemNo BE_ItemNo
, pid.PurchaseInvoiceNo DetailRemarks
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid left outer join PurchaseInvoiceHeaders pih on pid.PurchaseInvoiceNo = pih.PurchaseInvoiceNo
LEFT OUTER JOIN Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
--AND pid.BranchId>@BranchId
AND pid.BranchId=@BranchId

@OldDBsqlText

------order by pid.PurchaseInvoiceNo
) as pur 
 
";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all
select @UserName UserName,@Branch Branch,'22' NoteNo,'1' SubNoteNo
, pih.BENumber [Invoice/B/E No]
, pih.InvoiceDateTime [Date]
, pih.CustomCode OfficeCode
, pid.CPCName CPC
, pid.AssessableValue
,(
pid.SubTotal
+pid.CDAmount
+pid.RDAmount
+pid.SDAmount
+pid.TVBAmount
) BasevalueofVAT
, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VAT
,pid.SDAmount SD
,pid.ATVAmount [AT]
,'SubForm-Ka'SubFormName
,'Import Purchase (Non-Rebate)'Remarks  
, p.ProductName, p.ProductName ProductDescription, pid.HSCode ProductCode, pid.BEItemNo BE_ItemNo
, pid.PurchaseInvoiceNo DetailRemarks
, pid.ItemNo
,pid.VATRate
,pid.SD SDRate
from @db.dbo.PurchaseInvoiceDetails pid left outer join PurchaseInvoiceHeaders pih on pid.PurchaseInvoiceNo = pih.PurchaseInvoiceNo
LEFT OUTER JOIN @db.dbo.Products p ON p.ItemNo = pid.ItemNo 

where  1=1 
and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('NonRebate','Local-NonRebate', 'Import-NonRebate')
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' ,'CommercialImporter' )
and pid.PeriodId=@PeriodId
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'
--AND pid.BranchId>@BranchId
AND pid.BranchId=@BranchId

";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion

                    #endregion

                }

                #region Summary SQL Group By

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 
) as Summary
Group BY

UserName
,Branch
,NoteNo
,SubNoteNo
,SubFormName
,Remarks
,ItemNo
,ProductDescription
,ProductName
,ProductCode
 ,[Invoice/B/E No]
 ,[Date]
 ,OfficeCode
 ,BE_ItemNo
 ,CPC
,VATRate
,SDRate
";
                }

                #endregion

                sqlText = sqlText + @" ORDER BY ProductName";

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }
                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter
                objCommVAT19.Parameters.AddWithValue("@BranchId", vm.BranchId);


                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT19.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT19.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT19.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT19.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT19.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormAPart4", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormAPart4", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        //Note 8
        public DataTable VAT9_1_SubFormBPart3(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Statement

                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
--------declare @BranchId int = 1

declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;

";

                #region Summary SQL

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 

SELECT DISTINCT
UserName
, Branch
, NoteNo
, SubNoteNo
, SUM(TotalPrice)  TotalPrice
, SUM(VATAmount)  VATAmount
, SUM(SDAmount)  SDAmount
, SubFormName
, Remarks
, ItemNo
, ProductDescription
, ProductName
, ProductCode
,'' DetailRemarks
,ProductCategory
,VATRate
,SDRate
FROM (

";
                }

                #endregion

                if (vm.NoteNo == 8)
                {
                    #region Note-08

                    sqlText = sqlText + @" 
select sal.*, p.ProductDescription, p.ProductName
,pc.CategoryName ProductCategory 
, sal.SalesInvoiceNo DetailRemarks
from (

select 
@UserName UserName,@Branch Branch,
'8' NoteNo,'1' SubNoteNo,sid.SubTotal TotalPrice,sid.VATAmount,sid.SDAmount,'SubForm-Ka'SubFormName
,'Retail'Remarks 
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode
,sid.VATRate
,sid.SD SDRate

from SalesInvoiceDetails sid
where 1=1 
  and  (sid.post=@post1 or sid.post=@post2)  
and sid.Type in('Retail')
and sid.TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')
and sid.PeriodId=@PeriodId
AND sid.BranchId=@BranchId


union all

select 
@UserName,@Branch,
'8' NoteNo,'2' SubNoteNo, SubTotal, SDAmount, VATAmount,'SubForm-Kha'SubFormName 
,'Retail'Remarks  
,ItemNo
,SalesInvoiceNo
,''
,VATRate
,SD SDRate
from BureauSalesInvoiceDetails where  1=1 and  (post=@post1 or post=@post2)   and Type in('Retail')
and TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')
and PeriodId=@PeriodId
AND BranchId=@BranchId

) AS sal

LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
LEFT OUTER JOIN ProductCategories pc ON pc.CategoryID = p.CategoryID 
------ORDER BY sal.SalesInvoiceNo DESC
";
                    #endregion

                }

                #region Summary SQL Group By

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 
) as Summary
Group BY

UserName
,Branch
,NoteNo
,SubNoteNo
,SubFormName
,Remarks
,ItemNo
,ProductDescription
,ProductName
,ProductCode
,ProductCategory
,VATRate
,SDRate
";
                }

                #endregion

                sqlText = sqlText + @" ORDER BY ProductName";


                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter

                objCommVAT19.Parameters.AddWithValue("@BranchId", vm.BranchId);


                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT19.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT19.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT19.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT19.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT19.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormBPart3", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormBPart3", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public DataTable VAT9_1_SubFormCPart3(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Statement

                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
--------declare @BranchId int = 1
declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;

";

                #region Summary SQL

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 

SELECT DISTINCT
UserName
, Branch
, NoteNo
, SubNoteNo
, SUM(TotalPrice)  TotalPrice
, SUM(VATAmount)  VATAmount
, SUM(SDAmount)  SDAmount
, SUM(Quantity)  Quantity
, SubFormName
, Remarks
, ItemNo
, ProductDescription
, ProductName
, ProductCode
,'' DetailRemarks
,VATRate
,SDRate

FROM (

";
                }

                #endregion

                if (vm.NoteNo == 6)
                {
                    #region Note-06

                    sqlText = sqlText + @" 

select sal.*, p.ProductDescription, p.ProductName, sal.SalesInvoiceNo DetailRemarks  from (
select 
@UserName UserName,@Branch Branch,
'6' NoteNo,'1' SubNoteNo,sid.SubTotal TotalPrice,sid.VATAmount,sid.SDAmount
,sid.UOM
,sid.Quantity
,'SubForm-Ka'SubFormName
,'Tarrif'Remarks
,sid.ItemNo
,sid.SalesInvoiceNo
,sid.HSCode ProductCode
,sid.VATRate
,sid.SD SDRate
from SalesInvoiceDetails sid
where 1=1 
and  (sid.post=@post1 or sid.post=@post2) 
and sid.Type in('FixedVAT')
and sid.TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')
and sid.PeriodId=@PeriodId
AND sid.BranchId=@BranchId


union all
select 
@UserName,@Branch,
'6' NoteNo,'2' SubNoteNo, SubTotal, VATAmount, SDAmount
,UOM
,Quantity

,'SubForm-Ga'SubFormName 
,'Tarrif'Remarks  
,ItemNo
,SalesInvoiceNo
,''
,VATRate
,SD SDRate
from BureauSalesInvoiceDetails where  1=1  
and  (post=@post1 or post=@post2)   
and Type in('FixedVAT')
and TransactionType in('Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')
and PeriodId=@PeriodId
AND BranchId=@BranchId

) AS sal

LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
------ORDER BY sal.SalesInvoiceNo DESC
";
                    #endregion

                }

                #region Summary SQL Group By

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 
) as Summary
Group BY

UserName
,Branch
,NoteNo
,SubNoteNo
,SubFormName
,Remarks
,ItemNo
,ProductDescription
,ProductName
,ProductCode
,VATRate
,SDRate

";
                }

                #endregion

                sqlText = sqlText + @" ORDER BY ProductName";

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter
                objCommVAT19.Parameters.AddWithValue("@BranchId", vm.BranchId);

                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT19.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT19.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT19.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT19.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT19.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormCPart3", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormCPart3", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        //Note 18
        public DataTable VAT9_1_SubFormCPart4(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Statement

                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
--------declare @BranchId int = 1
declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;

";

                #region Summary SQL

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 

SELECT DISTINCT
UserName
, Branch
, NoteNo
, SubNoteNo
, SUM(TotalPrice)  TotalPrice
, SUM(VATAmount)  VATAmount
, SUM(SDAmount)  SDAmount
, SubFormName
, Remarks
, ItemNo
, ProductDescription
, ProductName
, ProductCode
,'' DetailRemarks
,VATRate
,SDRate
FROM (

";
                }

                #endregion

                if (vm.NoteNo == 18)
                {
                    #region LineNo-18

                    sqlText = sqlText + @" 
select pur.*, p.ProductDescription, p.ProductName, pur.PurchaseInvoiceNo DetailRemarks from
(
select @UserName UserName,@Branch Branch,'18' NoteNo,'1' SubNoteNo,pid.SubTotal TotalPrice, case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end VATAmount ,pid.SDAmount
,pid.UOM,pid.Quantity
,'SubForm-Ka'SubFormName
,'Local Purchase (Fixed VAT)'Remarks  
,pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.hscode ProductCode
,pid.VATRate
,pid.SD SDRate
from PurchaseInvoiceDetails pid
where  1=1  and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('FixedVAT','fixedvat(rebate)')
and pid.TransactionType in('Other','PurchaseCNXX','Trading','Service','ServiceNS','InputService','PurchaseTollcharge')
and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
--and pid.IsRebate='Y'

AND pid.BranchId=@BranchId
/*
union all
select @UserName,@Branch,'18' NoteNo,'2' SubNoteNo,-1*(pid.SubTotal),1*( case when pid.RebateAmount>0 then pid.RebateAmount else  pid.VATAmount end ),1*(pid.SDAmount)
,pid.UOM,pid.Quantity
,'SubForm-Ka'SubFormName
,'Local Purchase (Fixed VAT)'Remarks  
, pid.PurchaseInvoiceNo
,pid.ItemNo
,pid.hscode ProductCode
from PurchaseInvoiceDetails pid 
where  1=1   and  (pid.post=@post1 or pid.post=@post2)  
and pid.Type in('FixedVAT','fixedvat(rebate)')
and pid.TransactionType in('PurchaseDN','PurchaseReturn')
and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
--and pid.IsRebate='Y'

AND pid.BranchId=@BranchId
*/
) as pur
LEFT OUTER JOIN Products p ON p.ItemNo = pur.ItemNo 
------order by pur.PurchaseInvoiceNo
";
                    #endregion
                }

                #region Summary SQL Group By

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 
) as Summary
Group BY

UserName
,Branch
,NoteNo
,SubNoteNo
,SubFormName
,Remarks
,ItemNo
,ProductDescription
,ProductName
,ProductCode
,VATRate
,SDRate
";
                }

                #endregion

                sqlText = sqlText + @" ORDER BY ProductName";

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter
                objCommVAT19.Parameters.AddWithValue("@BranchId", vm.BranchId);


                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT19.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT19.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT19.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT19.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT19.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormCPart4", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        // Note 24
        public DataTable VAT9_1_SubFormDPart5(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Statement

                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
--------declare @BranchId int = 1
declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;

declare @SettingValue VARCHAR (200);
select @SettingValue=SettingValue from Settings WHERE 1=1 AND SettingGroup = 'OperationalCode' AND SettingName='TotalDepositVAT'

";
                if (vm.NoteNo == 24)
                {
                    #region LineNo-24

                    sqlText = sqlText + @" 
select  @UserName UserName,@Branch Branch,'24' NoteNo,'1' SubNoteNo, BillDeductAmount ,0 VATAmount,0 SDAmount,'SubForm-Gha'SubFormName
,'Purcshase VDS'Remarks  
,v.VendorName
,v.VATRegistrationNo VendorBIN
,v.Address1 VendorAddress
,vds.BillAmount TotalPrice
,ISNULL(vds.BillDeductAmount,0) VDSAmount
,vds.BillNo InvoiceNo
,vds.BillDate InvoiceDate
,dep.DepositId VDSCertificateNo
,dep.DepositDateTime  VDSCertificateDate
,bk.AccountNumber AccountCode
,dep.TreasuryNo TaxDepositSerialNo
,dep.BankDepositDate TaxDepositDate
,vds.VDSId DetailRemarks

from VDS
LEFT OUTER JOIN Vendors v ON vds.VendorId = v.VendorID 
LEFT OUTER JOIN PurchaseInvoiceHeaders pih ON vds.PurchaseNumber=pih.PurchaseInvoiceNo
LEFT OUTER JOIN Deposits dep on vds.VDSId=dep.DepositId
left outer join BankInformations bk on dep.BankID =bk.BankID

where  1=1  
and  (vds.post=@post1 or vds.post=@post2)  
and vds.IsPurchase in('Y')
and vds.PeriodId=@PeriodId
AND VDS.BranchId=@BranchId
";
                    #endregion
                }

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion

                #region SQL Command

                SqlCommand objCommVAT9_1 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter
                objCommVAT9_1.Parameters.AddWithValue("@BranchId", vm.BranchId);

                if (!objCommVAT9_1.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT9_1.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT9_1.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT9_1.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT9_1.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT9_1.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT9_1.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT9_1.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT9_1);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormDPart5", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormDPart5", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        // Note 26
        public DataTable VAT9_1_SubFormEPart5(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Statement

                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
--------declare @BranchId int = 1

declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;

";

                #region Summary SQL

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 
SELECT DISTINCT
UserName
,Branch
,NoteNo
,SubNoteNo
,DebitNoteNo
,IssuedDate
,TaxChallanNo
,TaxChallanDate
,ReasonforIssuance
,SUM(ValueinChallan)ValueinChallan
,SUM(QuantityinChallan)QuantityinChallan
,SUM(VATinChallan)VATinChallan
,SUM(SDinChallan)SDinChallan
,SUM(ValueofIncreasingAdjustment)ValueofIncreasingAdjustment
,SUM(QuantityofIncreasingAdjustment)QuantityofIncreasingAdjustment
,SUM(VATofIncreasingAdjustment)VATofIncreasingAdjustment
,SUM(SDofIncreasingAdjustment)SDofIncreasingAdjustment
,Remarks
,ItemNo
,ProductDescription
,ProductName
,SubFormName


FROM (

";
                }

                #endregion

                if (vm.NoteNo == 26)
                {
                    #region Note-26

                    sqlText = sqlText + @" 
select sal.*, p.ProductDescription, p.ProductName,sal.DebitNoteNo DetailRemarks  
from (

SELECT 
@UserName UserName,@Branch Branch,
'26' NoteNo,'1' SubNoteNo,sid.SalesInvoiceNo DebitNoteNo  ,sid.InvoiceDateTime IssuedDate ,sid.PreviousSalesInvoiceNo TaxChallanNo 
,sid.PreviousInvoiceDateTime TaxChallanDate ,sid.ReasonOfReturn  ReasonforIssuance
,(sid.PreviousSubTotal) ValueinChallan ,sid.PreviousQuantity QuantityinChallan ,sid.PreviousVATAmount VATinChallan,sid.PreviousSDAmount  SDinChallan
,(sid.SubTotal)ValueofIncreasingAdjustment,sid.Quantity QuantityofIncreasingAdjustment,sid.VATAmount VATofIncreasingAdjustment,sid.SDAmount SDofIncreasingAdjustment
,'Sale Debit' Remarks
,sid.ItemNo
,'Note 26' SubFormName
from SalesInvoiceDetails sid
where 1=1 
 and  (post=@post1 or post=@post2)  
and TransactionType in('Debit')
and PeriodId=@PeriodId
AND BranchId=@BranchId
AND isnull(SourcePaidQuantity,0) = 0
and isnull(ProductType,'R') in ('P','R')

union all

select @UserName UserName,@Branch Branch,'26' NoteNo,'2' SubNoteNo,sid.PurchaseInvoiceNo DebitNoteNo  
,sid.InvoiceDateTime IssuedDate ,sid.PurchaseReturnId TaxChallanNo
,sid.PreviousInvoiceDateTime TaxChallanDate ,''  ReasonforIssuance
,(sid.PreviousSubTotal) ValueinChallan ,sid.PreviousQuantity QuantityinChallan ,sid.PreviousVATAmount VATinChallan,sid.PreviousSDAmount  SDinChallan
,(sid.SubTotal)ValueofIncreasingAdjustment,sid.Quantity QuantityofIncreasingAdjustment,sid.VATAmount VATofIncreasingAdjustment
,sid.SDAmount SDofIncreasingAdjustment
,'PurchaseDebitNote' Remarks
,sid.ItemNo
,'Note 26' SubFormName

from PurchaseInvoiceDetails sid
--left outer join PurchaseInvoiceDetails sidp on sid.PurchaseReturnId=sidp.PurchaseInvoiceNo and sid.ItemNo=sidp.ItemNo
--left outer join PurchaseInvoiceDetails sidp on sid.PurchaseReturnId=sidp.PurchaseInvoiceNo
where 1=1 
 and  (sid.post=@post1 or sid.post=@post2)  
 and sid.PeriodId=@PeriodId
AND sid.BranchId=@BranchId
and sid.TransactionType in('PurchaseReturn')

union all
select 
@UserName,@Branch,
'26' NoteNo,'3' SubNoteNo,SalesInvoiceNo DebitNoteNo,InvoiceDateTime IssuedDate,'N/A' TaxChallanNo,''TaxChallanDate,'N/A'ReasonforIssuance
,(SubTotal)ValueinChallan,Quantity  QuantityinChallan,VATAmount VATinChallan,SDAmount SDinChallan,(SubTotal)ValueofIncreasingAdjustment,Quantity QuantityofIncreasingAdjustment,VATAmount VATofIncreasingAdjustment,SDAmount SDofIncreasingAdjustment
,'Debit'Remarks  
,ItemNo
,'Note 26' SubFormName
from BureauSalesInvoiceDetails where  1=1  and  (post=@post1 or post=@post2)
and TransactionType in('Debit')
and PeriodId=@PeriodId
AND BranchId=@BranchId

) AS sal

LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
";
                    #endregion

                }

                #region Summary SQL Group By

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 
) as Summary
Group BY

UserName
,Branch
,NoteNo
,SubNoteNo
,DebitNoteNo
,IssuedDate
,TaxChallanNo
,TaxChallanDate
,ReasonforIssuance
,Remarks
,ItemNo
,ProductDescription
,ProductName
,SubFormName

";
                }

                #endregion

                sqlText = sqlText + @" ORDER BY ProductName";


                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter

                objCommVAT19.Parameters.AddWithValue("@BranchId", vm.BranchId);


                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT19.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT19.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT19.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT19.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT19.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormEPart5", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormEPart5", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public DataTable VAT9_1_SubFormFPart7(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Statement

                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
--------declare @BranchId int = 1

declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;

";

                #region Summary SQL

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 
SELECT DISTINCT
UserName
,Branch
,NoteNo
,SubNoteNo
,CreditNoteNo
, IssuedDate
, TaxChallanNo
,TaxChallanDate
, ReasonforIssuance
, SUM(ValueinChallan)ValueinChallan
, SUM(QuantityinChallan)QuantityinChallan
, SUM(VATinChallan)VATinChallan
, SUM(SDinChallan)SDinChallan
, SUM(ValueofDecreasingAdjustment)ValueofDecreasingAdjustment
, SUM(QuantityofDecreasingAdjustment)QuantityofDecreasingAdjustment
, SUM(VATofDecreasingAdjustment)VATofDecreasingAdjustment
, SUM(SDofDecreasingAdjustment)SDofDecreasingAdjustment
, Remarks
, ItemNo
, ProductDescription
, ProductName
, SubFormName



FROM (

";
                }

                #endregion

                if (vm.NoteNo == 31)
                {
                    #region Note-31

                    sqlText = sqlText + @" 
select sal.*, p.ProductDescription, p.ProductName,sal.CreditNoteNo DetailRemarks  
from (

SELECT 
@UserName UserName,@Branch Branch,
'31' NoteNo,'1' SubNoteNo,sid.SalesInvoiceNo CreditNoteNo  ,sid.InvoiceDateTime IssuedDate,sid.PreviousSalesInvoiceNo TaxChallanNo,sid.PreviousInvoiceDateTime TaxChallanDate ,sid.ReasonOfReturn ReasonforIssuance
,(sid.PreviousSubTotal) ValueinChallan,sid.PreviousQuantity QuantityinChallan,sid.PreviousVATAmount VATinChallan ,sid.PreviousSDAmount SDinChallan
,(sid.SubTotal)ValueofDecreasingAdjustment,sid.Quantity QuantityofDecreasingAdjustment,sid.VATAmount+isnull(sid.option2,0) VATofDecreasingAdjustment ,sid.SDAmount SDofDecreasingAdjustment
,'Credit' Remarks
,sid.ItemNo
,'Note 31' SubFormName
from SalesInvoiceDetails sid
where 1=1 
 and  (post=@post1 or post=@post2)  
and TransactionType in('Credit','RawCredit')
and PeriodId=@PeriodId
AND BranchId=@BranchId
AND isnull(SourcePaidQuantity,0) = 0
and isnull(ProductType,'R') in ('P','R')


union all
select 
@UserName,@Branch,
'31' NoteNo,'2' SubNoteNo,SalesInvoiceNo CreditNoteNo,InvoiceDateTime IssuedDate,'N/A'TaxChallanNo,''TaxChallanDate,'N/A'ReasonforIssuance
,(SubTotal)ValueinChallan,Quantity QuantityinChallan,VATAmount VATinChallan,SDAmount SDinChallan,(SubTotal)ValueofDecreasingAdjustment
,Quantity QuantityofDecreasingAdjustment,VATAmount VATofDecreasingAdjustment,SDAmount SDofDecreasingAdjustment
,'Credit'Remarks  
,ItemNo
,'Note 31' SubFormName
from BureauSalesInvoiceDetails where  1=1  and  (post=@post1 or post=@post2)
and TransactionType in('Credit','ExportServiceNSCredit')
and PeriodId=@PeriodId
AND BranchId=@BranchId

) AS sal

LEFT OUTER JOIN Products p ON p.ItemNo = sal.ItemNo 
";
                    #endregion

                }

                #region Summary SQL Group By

                if (vm.IsSummary)
                {
                    sqlText = sqlText + @" 
) as Summary
Group BY


UserName
,Branch
,NoteNo
,SubNoteNo
,CreditNoteNo
,IssuedDate
,TaxChallanNo
,TaxChallanDate
,ReasonforIssuance
,Remarks
,ItemNo
,ProductDescription
,ProductName
,SubFormName

";
                }

                #endregion

                sqlText = sqlText + @" ORDER BY ProductName";


                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter

                objCommVAT19.Parameters.AddWithValue("@BranchId", vm.BranchId);


                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT19.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT19.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT19.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT19.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT19.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormEPart7", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormEPart7", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        // Note 27 32
        public DataTable VAT9_1_SubFormEPart7(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Statement

                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);
declare @ATVRebate as varchar(100);
declare @AutoPartialRebateProcess as varchar(1);
declare @SelectBranchId as int = 0
--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
--------declare @BranchId int = 1

declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;
select @ATVRebate=settingValue  FROM Settings where SettingGroup='ImportPurchase' and SettingName='ATVRebate';
select @AutoPartialRebateProcess=settingValue  FROM Settings where SettingGroup='Sale' and SettingName='AutoPartialRebateProcess';

    -----------------------------Initialization/Rebate Cancel----------------------------
    -------------------------------------------------------------------------------------
    DECLARE @Line9Subtotal AS DECIMAL = 0
    DECLARE @Line23VAT AS DECIMAL = 0

    SELECT @Line9Subtotal=ISNULL((LineA),0)
    FROM VATReturnV2s
    WHERE  1=1 AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 9

    SELECT @Line23VAT=ISNULL((LineB),0)
    FROM VATReturnV2s
    WHERE  1=1 AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 23
";


                if (vm.NoteNo == 27)
                {
                    #region Note-27

                    sqlText = sqlText + @" 
SELECT  *
INTO #TempAdjustmentHistorys 

FROM 
(
SELECT '27' NoteNo,'0' SubNoteNo,''AdjHistoryNo,''AdjDate,0 AdjInputAmount,0 SubTotal,0 VATAmount,0 SDAmount,'Note 27'SubFormName  ,'IncreasingAdjustment'Remarks 
, '' AdjType, '' AdjName,'' Notes
UNION ALL


SELECT '27' NoteNo,'1' SubNoteNo,AdjHistoryNo,AdjDate,AdjInputAmount, AdjAmount,0 VATAmount,0 SDAmount,'Note 27'SubFormName,'IncreasingAdjustment'Remarks  
, ah.AdjType, an.AdjName,ah.AdjDescription Notes
FROM AdjustmentHistorys ah
LEFT OUTER JOIN AdjustmentName an ON ah.AdjId=an.AdjId
WHERE  1=1 and  (post=@post1 or post=@post2)    
AND ah.AdjType in('IncreasingAdjustment')
and ah.PeriodId=@PeriodId
AND ah.BranchId=@BranchId
UNION ALL

SELECT  '27' NoteNo,'2' SubNoteNo,''AdjHistoryNo,''AdjDate,0 AdjInputAmount
,ddbd.ClaimVAT Subtotal
, 0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'IncreasingAdjustment' Remarks
,ddbd.TransactionType AdjType 
,ddbd.TransactionType AdjName  ,'' Notes
FROM DutyDrawBackDetails ddbd
LEFT OUTER JOIN PurchaseInvoiceDetails pid ON pid.PurchaseInvoiceNo=ddbd.PurchaseInvoiceNo
WHERE 1=1 AND ddbd.BranchId=@BranchId
AND (ddbd.post=@post1 or ddbd.post=@post2) 
--and ddbd.PeriodId=@PeriodId
and pid.RebatePeriodID=@PeriodId
and pid.IsRebate='Y'

AND ISNULL(ddbd.TransactionType,'DDB') = 'VDB'
AND pid.Type IN('VAT','FixedVAT')


UNION ALL
SELECT '27' NoteNo,'3' SubNoteNo,''AdjHistoryNo,''AdjDate, 0 AdjInputAmount
, CASE
WHEN @AutoPartialRebateProcess='N' THEN 0 
WHEN @Line9Subtotal > 0 THEN ISNULL((LineA)*@Line23VAT/@Line9Subtotal,0) 
ELSE 0 END AS Subtotal
, 0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'IncreasingAdjustment'Remarks  
,'IncreasingAdjustment' AdjType, 'Exempted Goods/Service (Rebate Cancel)' AdjName ,'' Notes
FROM VATReturnV2s
WHERE  1=1
AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 3  and SubNoteNo not IN(0) 

UNION ALL
SELECT '27' NoteNo,'4' SubNoteNo,''AdjHistoryNo,''AdjDate,0 AdjInputAmount
, CASE 
WHEN @AutoPartialRebateProcess='N' THEN 0 
WHEN @Line9Subtotal > 0 THEN ISNULL((LineA)*@Line23VAT/@Line9Subtotal,0) 
ELSE 0 END AS Subtotal
, 0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'IncreasingAdjustment'Remarks  
,'IncreasingAdjustment' AdjType, 'Other Rated VAT (Rebate Cancel)' AdjName,'' Notes
FROM VATReturnV2s
WHERE  1=1
AND PeriodID = @PeriodId AND BranchId=@SelectBranchId AND NoteNo = 7 AND SubNoteNo IN(1,2)



union all

select '27' NoteNo,'5' SubNoteNo,''AdjHistoryNo,''AdjDate,0 AdjInputAmount,NonLeaderVATAmount, 0,0,'Note 27'SubFormName
,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Non-Leader VAT Amount' AdjName ,'' Notes
from SalesInvoiceDetails where  1=1   and Type in('VAT')
and TransactionType in('ServiceNS','Service','TollFinishIssue','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
AND BranchId=@BranchId
and PeriodId=@PeriodId
and ISNULL(IsLeader,'NA') = 'Y'


--union all

--select '27' NoteNo,'6' SubNoteNo,''AdjHistoryNo,''AdjDate,0 AdjInputAmount,SourcePaidVATAmount, 0,0,'Note 27'SubFormName 
--,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Source Paid VAT Amount' AdjName ,'' Notes
--from SalesInvoiceDetails where  1=1   and Type in('VAT')
--and TransactionType in('Credit')
--AND BranchId=@BranchId
--AND PeriodId=@PeriodId
--and ISNULL(SourcePaidVATAmount,0) > 0


union all

select '27' NoteNo,'7' SubNoteNo,DisposeNo AdjHistoryNo,TransactionDateTime AdjDate,0 AdjInputAmount,ISNULL(VATAmount,0), 0,0,'Note 27'SubFormName
,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Raw Material Dispose' AdjName ,'' Notes
from DisposeRawDetails where  1=1   
and TransactionType in('Other')
AND BranchId=@BranchId
AND PeriodId=@PeriodId




union all

select '27' NoteNo,'8' SubNoteNo,DisposeNo AdjHistoryNo,TransactionDateTime AdjDate,0 AdjInputAmount,ISNULL(RebateVATAmount,0), 0,0,'Note 27'SubFormName 
,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Raw Material for Finish Dispose' AdjName ,'' Notes
from DisposeFinishDetails where  1=1  
and TransactionType in('Other','DisposeTrading')
AND BranchId=@BranchId
AND PeriodId=@PeriodId

/*
union all
select '27' NoteNo,'9' SubNoteNo,pid.PurchaseInvoiceNo AdjHistoryNo,pid.ReceiveDate AdjDate,ROUND( sum(SubTotal+CDAmount+RDAmount+TVBAmount),2) AdjInputAmount,ROUND(sum(ISNULL(VATAmount,0)+ISNULL(ATVAmount,0)),2), 0,0,'Note 27'SubFormName 
,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Purchase Credit' AdjName 
from PurchaseInvoiceDetails pid where  1=1  
and TransactionType in('PurchaseReturn')
AND BranchId=@BranchId
AND PeriodId=@PeriodId
group by pid.PurchaseInvoiceNo,pid.ReceiveDate
*/
--union all
--
--select '27' NoteNo,'10' SubNoteNo,''AdjHistoryNo,''AdjDate,0 AdjInputAmount,ISNULL(VATAmount,0), 0,0,'Note 27'SubFormName 
--,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Exempted VAT' AdjName 
--from SalesInvoiceDetails where  1=1  
--and TransactionType in ('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
--and Type in ('NonVAT')
--AND BranchId=@BranchId
--AND PeriodId=@PeriodId
--AND isnull(SourcePaidQuantity,0)=0
--AND Option2 is null

union all

select '27' NoteNo,'11' SubNoteNo,SalesInvoiceNo AdjHistoryNo,InvoiceDateTime AdjDate, case when type = 'Retail' then 0 else subtotal end AdjInputAmount,ISNULL(Option2,0), 0,0,'Note 27'SubFormName 
,'IncreasingAdjustment'Remarks, 'IncreasingAdjustment' AdjType, 'Exempted VAT' AdjName ,'' Notes
from SalesInvoiceDetails where  1=1  
and TransactionType in ('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service','TollSale')
and Type in ('OtherRate','NonVAT','Retail')
AND BranchId=@BranchId
AND PeriodId=@PeriodId
AND isnull(SourcePaidQuantity,0)=0
AND exists (
select SettingValue from settings
where SettingGroup = 'CompanyCode'
and SettingName = 'Code'
and SettingValue = 'Bata'
)

) AS adj


SELECT   NoteNo,AdjType,ah.AdjName,ah.AdjHistoryNo ChallanNumber,ah.AdjDate Date,ah.AdjInputAmount Amount,ah.SubTotal VAT,SubFormName, Notes,Remarks FROM #TempAdjustmentHistorys ah
WHERE AdjType='IncreasingAdjustment' --and ah.SubTotal>0

DROP TABLE #TempAdjustmentHistorys;
";
                    #endregion

                }

                if (vm.NoteNo == 32)
                {
                    #region Note-32

                    sqlText = sqlText + @" 
SELECT  *
    INTO #TempDecrementAdjustmentHistorys 

    FROM
    (
    SELECT '32' NoteNo,'0' SubNoteNo,''AdjHistoryNo,''AdjDate,0 AdjInputAmount,0 SubTotal,0 VATAmount,0 SDAmount,'Note32'SubFormName ,'DecreasingAdjustment'Remarks
    , '' AdjType, '' AdjName ,'' Notes
    UNION ALL
    SELECT '32' NoteNo,'1' SubNoteNo,AdjHistoryNo,AdjDate,AdjInputAmount,AdjAmount,0 VATAmount,0 SDAmount,'Note32'SubFormName,'DecreasingAdjustment'Remarks  
    , ah.AdjType, an.AdjName ,ah.AdjDescription Notes
    FROM AdjustmentHistorys ah
    LEFT OUTER JOIN AdjustmentName an ON ah.AdjId=an.AdjId
    WHERE  1=1 and  (post=@post1 or post=@post2)    
    AND ah.AdjType in('DecreasingAdjustment')
    and ah.PeriodId=@PeriodId
    AND ah.BranchId=@BranchId

    UNION ALL

    SELECT  '32' NoteNo,'2' SubNoteNo,''AdjHistoryNo,''AdjDate,0 AdjInputAmount
    ,ddbd.ClaimVAT Subtotal
    , 0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'DecreasingAdjustment' Remarks
    ,'DecreasingAdjustment' AdjType 
    ,ddbd.TransactionType AdjName  ,'' Notes
    FROM DutyDrawBackDetails ddbd
    LEFT OUTER JOIN PurchaseInvoiceDetails pid ON pid.PurchaseInvoiceNo=ddbd.PurchaseInvoiceNo
    WHERE 1=1 AND ddbd.BranchId=@BranchId
    AND  (ddbd.post=@post1 or ddbd.post=@post2) 
   -- and ddbd.PeriodId=@PeriodId
    and pid.RebatePeriodID=@PeriodId
    and pid.IsRebate='Y'
    AND ISNULL(ddbd.TransactionType,'DDB') = 'VDB'
    AND pid.Type='NonRebate'


    union all

    select '32' NoteNo,'3' SubNoteNo,''AdjHistoryNo,''AdjDate,0 AdjInputAmount,NonLeaderVATAmount, 0,0,'Note32'SubFormName 
    ,'DecreasingAdjustment'Remarks, 'DecreasingAdjustment' AdjType, 'Non-Leader VAT Amount' AdjName ,'' Notes
    from SalesInvoiceDetails where  1=1   and Type in('VAT')
    and TransactionType in('ServiceNS','Service','TollFinishIssue','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
    ,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
    AND BranchId=@BranchId
    and PeriodId=@PeriodId
    and ISNULL(IsLeader,'NA') = 'N'

    union all

    select '32' NoteNo,'4' SubNoteNo,''AdjHistoryNo,''AdjDate,0 AdjInputAmount,SourcePaidVATAmount, 0,0,'Note32'SubFormName 
    ,'DecreasingAdjustment'Remarks, 'DecreasingAdjustment' AdjType, 'Source Paid VAT Amount' AdjName ,'' Notes
    from SalesInvoiceDetails where  1=1   and Type in('VAT')
    and TransactionType in('ServiceNS','Service','TollFinishIssue','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter','ServiceNS'
    ,'Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
    AND BranchId=@BranchId
    AND PeriodId=@PeriodId
    and ISNULL(SourcePaidVATAmount,0) > 0

	union all

    select '32' NoteNo,'5' SubNoteNo,BENumber AdjHistoryNo,InvoiceDateTime AdjDate,SubTotal AdjInputAmount, VATAmount,SubTotal,SDAmount,'Note32'SubFormName 
    ,'DecreasingAdjustment'Remarks, 'DecreasingAdjustment' AdjType, 'Supplier DN' AdjName ,'' Notes
    from PurchaseInvoiceDetails where  1=1   and Type in('VAT')
    and TransactionType in('PurchaseDN')
    AND BranchId>@BranchId
    and IsRebate='Y'
    AND RebatePeriodID=@PeriodId

    ) decAdj

    



SELECT DISTINCT  NoteNo,AdjType,ah.AdjName,ah.AdjHistoryNo ChallanNumber,ah.AdjDate Date,ah.AdjInputAmount Amount,ah.SubTotal VAT,SubFormName, Notes,Remarks FROM #TempDecrementAdjustmentHistorys ah
WHERE AdjType='DecreasingAdjustment' and ah.SubTotal>0

DROP TABLE #TempDecrementAdjustmentHistorys;

";
                    #endregion

                }

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter

                objCommVAT19.Parameters.AddWithValue("@BranchId", vm.BranchId);


                if (!objCommVAT19.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT19.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT19.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT19.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT19.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT19.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT19.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT19.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormEPart7", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormEPart7", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        // Note 29
        public DataTable VAT9_1_SubFormEPart6(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Statement

                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;

";
                if (vm.NoteNo == 29)
                {
                    #region LineNo-29

                    sqlText = sqlText + @" 
select @UserName UserName,@Branch Branch,'29' NoteNo,'1' SubNoteNo, BillDeductAmount,0 VATAmount,0 SDAmount,'SubForm-Umo'SubFormName

,'Sale VDS'Remarks  
,c.CustomerName
,c.VATRegistrationNo CustomerBIN
,c.Address1 CustomerAddress
,vds.BillAmount TotalPrice
,vds.BillDeductAmount VDSAmount
,vds.PurchaseNumber 
--,vds.VDSId InvoiceNo
--,vds.PurchaseNumber InvoiceNo
,vds.BillNo InvoiceNo
,vds.BillDate InvoiceDate
,dep.TreasuryNo VDSCertificateNo 
,dep.BankDepositDate VDSCertificateDate 
,dep.ChequeNo AccountCode
,dep.ChequeBank SerialNo
,dep.ChequeDate TaxDepositDate
------,(select SettingValue from Settings WHERE 1=1 AND SettingGroup = 'OperationalCode' AND SettingName='TotalDepositVAT') AccountCode
,vds.VDSId DetailRemarks

FROM VDS 
LEFT OUTER JOIN Customers c ON vds.VendorId = c.CustomerID 
LEFT OUTER JOIN SalesInvoiceHeaders sih ON vds.PurchaseNumber=sih.SalesInvoiceNo
LEFT OUTER JOIN Deposits dep on vds.VDSId=dep.DepositId
where  1=1 
  and  (vds.post=@post1 or vds.post=@post2)  
and vds.IsPurchase in('N' )
and vds.PeriodId=@PeriodId
AND VDS.BranchId=@BranchId

";
                    #endregion
                }
                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }


                #endregion

                #region SQL Command

                SqlCommand objCommVAT9_1 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter
                objCommVAT9_1.Parameters.AddWithValue("@BranchId", vm.BranchId);

                if (!objCommVAT9_1.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT9_1.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT9_1.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT9_1.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT9_1.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT9_1.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT9_1.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT9_1.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT9_1);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormEPart6", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormEPart6", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public DataTable VAT9_1_SubFormFPart6(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction


                CommonDAL commonDal = new CommonDAL();
                int CurrentPeriodId = Convert.ToInt32(Convert.ToDateTime(vm.PeriodName).ToString("MMyyyy"));

                string code = commonDal.settingValue("CompanyCode", "Code");
                string OldDBName = commonDal.settingValue("DatabaseName", "DatabaseName");

                string OldsqlText = "";

                #region Statement

                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
--------declare @BranchId int = 1
declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'

set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;

";
                if (vm.NoteNo == 30)
                {
                    #region LineNo-30

                    sqlText = sqlText + @" 
declare @ATVRebate as varchar(100)
select @ATVRebate=settingValue  FROM Settings where SettingGroup='ImportPurchase' and SettingName='ATVRebate';



select 
@UserName UserName,@Branch Branch,
'30' NoteNo,'1' SubNoteNo
,case when @ATVRebate='Y' then pid.ATVAmount  else 0 end ATAmount,0 Column1,0 Column2,'SubForm-Cha'SubFormName
,'Purchase AT'Remarks 
,ph.InvoiceDateTime Date, ph.CustomHouse, ph.BENumber
,pid.PurchaseInvoiceNo DetailRemarks
from PurchaseInvoiceDetails pid
LEFT OUTER JOIN PurchaseInvoiceHeaders ph ON pid.PurchaseInvoiceNo = ph.PurchaseInvoiceNo
where  1=1     
and  (pid.post=@post1 or pid.post=@post2)  
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'

AND ph.BranchId=@BranchId

@OldDBsqlText

";

                    #region Old Data

                    if (code.ToLower() == "ACI-1".ToLower())
                    {
                        if (CurrentPeriodId == 072021 || CurrentPeriodId == 082021)
                        {

                            OldsqlText = @"

union all

select 
@UserName UserName,@Branch Branch,
'30' NoteNo,'1' SubNoteNo
,case when @ATVRebate='Y' then pid.ATVAmount  else 0 end ATAmount,0,0,'SubForm-Cha'SubFormName
,'Purchase AT'Remarks 
,ph.InvoiceDateTime Date, ph.CustomHouse, ph.BENumber
,pid.PurchaseInvoiceNo DetailRemarks
from @db.dbo.PurchaseInvoiceDetails pid
LEFT OUTER JOIN @db.dbo.PurchaseInvoiceHeaders ph ON pid.PurchaseInvoiceNo = ph.PurchaseInvoiceNo
where  1=1     
and  (pid.post=@post1 or pid.post=@post2)  
and pid.TransactionType in('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport','CommercialImporter'  )
--and pid.PeriodId=@PeriodId
--and pid.RebatePeriodID=@PeriodId
and isnull(pid.RebatePeriodID, pid.PeriodId)=@PeriodId
and pid.IsRebate='Y'
AND ph.BranchId=@BranchId

";

                            OldsqlText = OldsqlText.Replace("@db", OldDBName);

                            sqlText = sqlText.Replace("@OldDBsqlText", OldsqlText);

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@OldDBsqlText", "");
                        }
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@OldDBsqlText", "");
                    }

                    #endregion


                    #endregion
                }
                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion

                #region SQL Command

                SqlCommand objCommVAT9_1 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter
                objCommVAT9_1.Parameters.AddWithValue("@BranchId", vm.BranchId);

                if (!objCommVAT9_1.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT9_1.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT9_1.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT9_1.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT9_1.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT9_1.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT9_1.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT9_1.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT9_1);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormFPart6", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormFPart6", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        //// LineNo=52,53,54,55,56,57,58,59,60,61
        public DataTable VAT9_1_SubFormGPart8(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            // LineNo=52,53,54,55,56,57,58,59,60,61
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Statement

                sqlText = @" ";

                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'
--------declare @BranchId int = 1
set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,   @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;

";


                if (vm.NoteNo == 52)
                {
                    #region LineNo-52

                    sqlText = sqlText + @" 
select dep.*, dep.DepositId DetailRemarks  from (

select 
@UserName UserName,@Branch Branch,
'52' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Total Deposit VAT'Remarks  
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, d.ChequeNo AccountCode
, d.DepositId

from Deposits d where  1=1  and  (post=@post1 or post=@post2)   
and d.TransactionType in('Treasury')
and d.PeriodId=@PeriodId
AND d.BranchId=@BranchId


union all
select 
@UserName,@Branch,
'52' NoteNo,'2' SubNoteNo, DepositAmount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Total Deposit VAT'Remarks  
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, d.ChequeNo AccountCode
, d.DepositId
from Deposits d where  1=1  and  (post=@post1 or post=@post2)   
and TransactionType in('VDS') AND DepositType not in('NotDeposited')
and d.PeriodId=@PeriodId
AND BranchId=@BranchId


) AS dep

";
                    #endregion
                }
                else if (vm.NoteNo == 53)
                {
                    #region LineNo-53

                    sqlText = sqlText + @" 
select dep.* from(
select @UserName UserName,@Branch Branch,'53' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Total Deposit SD'Remarks  
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, d.ChequeNo AccountCode
, d.DepositId DetailRemarks
from Deposits d where  1=1  and  (post=@post1 or post=@post2)   
and TransactionType in('SD')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
) as dep
";

                    #endregion
                }
                else if (vm.NoteNo == 54)
                {
                    #region LineNo-54

                    sqlText = sqlText + @" 
select dep.* from (

select @UserName UserName,@Branch Branch,'54' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Interest On Overed VAT Deposit'Remarks 
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, d.ChequeNo AccountCode
, d.DepositId DetailRemarks
from Deposits d where  1=1  and  (post=@post1 or post=@post2)   
and TransactionType in('InterestOnOveredVAT') and d.DepositType not in('NotDeposited')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
) as dep
";

                    #endregion
                }
                else if (vm.NoteNo == 55)
                {
                    #region LineNo-55

                    sqlText = sqlText + @" 
select dep.* from(
select @UserName UserName,@Branch Branch,'55' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Interest On Overed SD Deposit'Remarks  
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, d.ChequeNo AccountCode
, d.DepositId DetailRemarks
from Deposits d where  1=1  and  (post=@post1 or post=@post2)   
and TransactionType in('InterestOnOveredSD') and d.DepositType not in('NotDeposited')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
) as dep

";
                    #endregion
                }
                else if (vm.NoteNo == 56)
                {
                    #region LineNo-56

                    sqlText = sqlText + @" 
select dep.* from(
select @UserName UserName,@Branch Branch,'56' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Fine Or Penalty Deposit'Remarks 
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, d.ChequeNo AccountCode
, d.DepositId DetailRemarks
from Deposits d where  1=1  and  (post=@post1 or post=@post2)   
and TransactionType in('FineOrPenalty') and d.DepositType not in('NotDeposited')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
) as dep
";

                    #endregion
                }
                else if (vm.NoteNo == 57)
                {
                    #region LineNo-57

                    sqlText = sqlText + @" 
select dep.* from(
select @UserName UserName,@Branch Branch,'57' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Excise Duty Deposit'Remarks 
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, d.ChequeNo AccountCode
, d.DepositId DetailRemarks
from Deposits d where  1=1  and  (post=@post1 or post=@post2)   
and TransactionType in('ExciseDuty') and d.DepositType not in('NotDeposited')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
) as dep
";

                    #endregion
                }
                else if (vm.NoteNo == 58)
                {
                    #region LineNo-58

                    sqlText = sqlText + @" 
select dep.* from(
select @UserName UserName,@Branch Branch,'58' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Development Surcharge Deposit'Remarks 
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, d.ChequeNo AccountCode
, d.DepositId DetailRemarks
from Deposits d where  1=1  and  (post=@post1 or post=@post2)   
and TransactionType in('DevelopmentSurcharge') and d.DepositType not in('NotDeposited')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
) as dep
";

                    #endregion
                }
                else if (vm.NoteNo == 59)
                {
                    #region LineNo-59

                    sqlText = sqlText + @" 
select dep.* from(
select @UserName UserName,@Branch Branch,'59' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'ICT Development Surcharge Deposit'Remarks 
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, d.ChequeNo AccountCode
, d.DepositId DetailRemarks
from Deposits d where  1=1  and  (post=@post1 or post=@post2)   
and TransactionType in('ICTDevelopmentSurcharge') and d.DepositType not in('NotDeposited')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
) as dep
";

                    #endregion
                }
                else if (vm.NoteNo == 60)
                {
                    #region LineNo-60

                    sqlText = sqlText + @"
select dep.* from ( 
select @UserName UserName,@Branch Branch,'60' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Health Care Surcharge Deposit'Remarks  
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, d.ChequeNo AccountCode
, d.DepositId DetailRemarks
from Deposits d where  1=1  and  (post=@post1 or post=@post2)   
and TransactionType in('HelthCareSurcharge') and d.DepositType not in('NotDeposited')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
) as dep
";

                    #endregion
                }
                else if (vm.NoteNo == 61)
                {
                    #region LineNo-61

                    sqlText = sqlText + @" 
select dep.* from(
select @UserName UserName,@Branch Branch,'61' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Environment Protection Surcharge Deposit'Remarks   
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, d.ChequeNo AccountCode
, d.DepositId DetailRemarks
from Deposits d where  1=1  and  (post=@post1 or post=@post2)   
and TransactionType in('EnvironmentProtectionSurcharge') and d.DepositType not in('NotDeposited')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
) as dep
";

                    #endregion

                }

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion

                #region SQL Command

                SqlCommand objCommVAT9_1 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter
                objCommVAT9_1.Parameters.AddWithValue("@BranchId", vm.BranchId);

                if (!objCommVAT9_1.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT9_1.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT9_1.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT9_1.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT9_1.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT9_1.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT9_1.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT9_1.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT9_1);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormGPart8", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormGPart8", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        //// LineNo=58,59,60,61,62,63,64
        public DataTable VAT9_1_SubFormGPart8_V2(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";


                sqlText = @" 
declare @UserName as varchar(100);
declare @Branch as varchar(100);

--------declare @periodName VARCHAR (200);
--------declare @ExportInBDT VARCHAR (200);
declare @DateFrom [datetime];
declare @DateTo [datetime];
declare @MLock varchar(1);

--------SET @periodName='July-2019';
--------SET @ExportInBDT='Y'
--------declare @BranchId int = 1
set @UserName ='admin'
set @Branch ='HO_DB'

declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId,  @DateFrom=PeriodStart,@DateTo=periodEnd,@MLock=PeriodLock FROM FiscalYear where periodName=@periodName;




";
                if (vm.NoteNo == 58)
                {
                    #region LineNo-58

                    sqlText = sqlText + @" 
select dep.*, dep.DepositId DetailRemarks  from (

select 
@UserName UserName,@Branch Branch,
'58' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Total Deposit VAT'Remarks  
, d.TreasuryNo ChallanNumber, b.BankName BankName, b.BranchName BankBranch, d.ChequeDate Date, b.AccountNumber AccountCode
, d.DepositId

from Deposits d 
left outer join BankInformations b on d.BankID=b.BankID
where  1=1  and  (d.post=@post1 or d.post=@post2)  
and d.TransactionType in('Treasury') and d.DepositType NOT IN('ClosingBalanceVAT(18.6)') and d.DepositType NOT IN('RequestedAmountForRefundVAT')
and d.PeriodId=@PeriodId
------and d.DepositDateTime >= @Datefrom and d.DepositDateTime <dateadd(d,1,@Dateto)
AND d.BranchId=@BranchId


union all
select 
@UserName,@Branch,
'58' NoteNo,'2' SubNoteNo, DepositAmount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Total Deposit VAT'Remarks  
, d.TreasuryNo ChallanNumber, b.BankName BankName, b.BranchName BankBranch, d.ChequeDate Date, b.AccountNumber AccountCode
, d.DepositId

from Deposits d 
left outer join BankInformations b on d.BankID=b.BankID
where  1=1  and  (post=@post1 or post=@post2)  
and d.TransactionType in('VDS') AND d.DepositType not in('NotDeposited')
and d.PeriodId=@PeriodId
------and d.DepositDateTime >= @Datefrom and d.DepositDateTime <dateadd(d,1,@Dateto)
AND d.BranchId=@BranchId


--union all
--
--select 
--@UserName,@Branch,
--'58' NoteNo,'3' SubNoteNo, DepositAmount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
--,'Total Deposit VAT'Remarks  
--, d.TreasuryNo ChallanNumber, b.BankName BankName, b.BranchName BankBranch, d.ChequeDate Date, b.AccountNumber AccountCode
--, d.DepositId
--
--from Deposits d 
--left outer join BankInformations b on d.BankID=b.BankID
--where  1=1  and  (post=@post1 or post=@post2)  
--and d.TransactionType
--in(
--'WithoutBankPay'
--,'DevelopmentSurcharge'
--,'EnvironmentProtectionSurcharge'
--,'ExciseDuty'
--,'FineOrPenalty'
--,'HelthCareSurcharge'
--,'ICTDevelopmentSurcharge'
----,'InterestOnOveredSD'
--,'InterestOnOveredVAT'
--,'FinePenaltyForNonSubmissionOfReturn'
--)
--and d.DepositType not in('NotDeposited')
--and d.PeriodId=@PeriodId
--------and d.DepositDateTime >= @Datefrom and d.DepositDateTime <dateadd(d,1,@Dateto)
--AND d.BranchId=@BranchId



) AS dep

";
                    #endregion
                }
                else if (vm.NoteNo == 59)
                {
                    #region LineNo-59

                    sqlText = sqlText + @" 
select @UserName UserName,@Branch Branch,'59' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Total Deposit SD'Remarks  
, d.TreasuryNo ChallanNumber, b.BankName BankName, b.BranchName BankBranch, d.ChequeDate Date, b.AccountNumber AccountCode
, d.DepositId DetailRemarks
from Deposits d 
left outer join BankInformations b on d.BankID=b.BankID
where  1=1  and  (d.post=@post1 or d.post=@post2)  
and d.TransactionType in('SD') and DepositType NOT IN('ClosingBalanceSD(18.6)') and d.DepositType NOT IN('RequestedAmountForRefundSD')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
------and d.DepositDateTime >= @Datefrom and d.DepositDateTime <dateadd(d,1,@Dateto)

    union all

 select @UserName UserName,@Branch Branch,'59' NoteNo,'2' SubNoteNo,DepositAmount,0 VATAmount,0 SDAmount,'Sub-Form'SubFormName,'TotalDepositSD'Remarks  
 , d.TreasuryNo ChallanNumber, b.BankName BankName, b.BranchName BankBranch, d.ChequeDate Date, b.AccountNumber AccountCode
, d.DepositId DetailRemarks
from Deposits d 
left outer join BankInformations b on d.BankID=b.BankID
where  1=1  and  (d.post=@post1 or d.post=@post2)  
 and d. TransactionType
 in(
 'InterestOnOveredSD'
 )
    and d.DepositType not in('NotDeposited')
   AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
";

                    #endregion
                }
                else if (vm.NoteNo == 60)
                {
                    #region LineNo-60

                    sqlText = sqlText + @" 
select @UserName UserName,@Branch Branch,'60' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Excise Duty Deposit'Remarks 
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, b.AccountNumber AccountCode
, d.DepositId DetailRemarks
from Deposits d 
left outer join BankInformations b on d.BankID=b.BankID
where  1=1  and  (d.post=@post1 or d.post=@post2)  
and d.TransactionType in('ExciseDuty') and d.DepositType not in('NotDeposited')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
------and d.DepositDateTime >= @Datefrom and d.DepositDateTime <dateadd(d,1,@Dateto)

";

                    #endregion
                }
                else if (vm.NoteNo == 61)
                {
                    #region LineNo-61

                    sqlText = sqlText + @" 
select @UserName UserName,@Branch Branch,'61' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Development Surcharge Deposit'Remarks 
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, b.AccountNumber AccountCode
, d.DepositId DetailRemarks
from Deposits d 
left outer join BankInformations b on d.BankID=b.BankID
where  1=1  and  (d.post=@post1 or d.post=@post2)  
and d.TransactionType in('DevelopmentSurcharge') and d.DepositType not in('NotDeposited')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
------and d.DepositDateTime >= @Datefrom and d.DepositDateTime <dateadd(d,1,@Dateto)

";

                    #endregion
                }
                else if (vm.NoteNo == 62)
                {
                    #region LineNo-62

                    sqlText = sqlText + @" 
select @UserName UserName,@Branch Branch,'62' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'ICT Development Surcharge Deposit'Remarks 
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, b.AccountNumber AccountCode
, d.DepositId DetailRemarks
from Deposits d 
left outer join BankInformations b on d.BankID=b.BankID
where  1=1  and  (d.post=@post1 or d.post=@post2)  
and d.TransactionType in('ICTDevelopmentSurcharge') and d.DepositType not in('NotDeposited')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
------and d.DepositDateTime >= @Datefrom and d.DepositDateTime <dateadd(d,1,@Dateto)

";

                    #endregion
                }
                else if (vm.NoteNo == 63)
                {
                    #region LineNo-63

                    sqlText = sqlText + @" 
select @UserName UserName,@Branch Branch,'63' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Health Care Surcharge Deposit'Remarks  
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, b.AccountNumber AccountCode
, d.DepositId DetailRemarks
from Deposits d 
left outer join BankInformations b on d.BankID=b.BankID
where  1=1  and  (d.post=@post1 or d.post=@post2)  
and d.TransactionType in('HelthCareSurcharge') and d.DepositType not in('NotDeposited')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
------and d.DepositDateTime >= @Datefrom and d.DepositDateTime <dateadd(d,1,@Dateto)
";

                    #endregion
                }
                else if (vm.NoteNo == 64)
                {
                    #region LineNo-64

                    sqlText = sqlText + @" 
select @UserName UserName,@Branch Branch,'64' NoteNo,'1' SubNoteNo,d.DepositAmount Amount,0 VATAmount,0 SDAmount,'SubForm-Chha'SubFormName
,'Environment Protection Surcharge Deposit'Remarks   
, d.TreasuryNo ChallanNumber, d.ChequeBank BankName, d.ChequeBankBranch BankBranch, d.ChequeDate Date, b.AccountNumber AccountCode
, d.DepositId DetailRemarks
from Deposits d 
left outer join BankInformations b on d.BankID=b.BankID
where  1=1  and  (d.post=@post1 or d.post=@post2)  
and d.TransactionType in('EnvironmentProtectionSurcharge') and d.DepositType not in('NotDeposited')
AND d.BranchId=@BranchId
and d.PeriodId=@PeriodId
------and d.DepositDateTime >= @Datefrom and d.DepositDateTime <dateadd(d,1,@Dateto)
";

                    #endregion

                }

                if (vm.BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");
                }

                #endregion

                #region SQL Command

                SqlCommand objCommVAT9_1 = new SqlCommand(sqlText, currConn);

                #endregion

                #region Parameter
                objCommVAT9_1.Parameters.AddWithValue("@BranchId", vm.BranchId);

                if (!objCommVAT9_1.Parameters.Contains("@PeriodName"))
                {
                    objCommVAT9_1.Parameters.AddWithValue("@PeriodName", vm.PeriodName);
                }
                else
                {
                    objCommVAT9_1.Parameters["@PeriodName"].Value = vm.PeriodName;
                }


                if (!objCommVAT9_1.Parameters.Contains("@ExportInBDT"))
                {
                    objCommVAT9_1.Parameters.AddWithValue("@ExportInBDT", vm.ExportInBDT);
                }
                else
                {
                    objCommVAT9_1.Parameters["@ExportInBDT"].Value = vm.ExportInBDT;
                }
                objCommVAT9_1.Parameters.AddWithValue("@post1", vm.post1);
                objCommVAT9_1.Parameters.AddWithValue("@post2", vm.post2);
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT9_1);
                dataAdapter.Fill(dt);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormGPart8_V2", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "VAT9_1_SubFormGPart8_V2", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        #endregion

        #region ChartBar

        public DataSet SelectAllvat9_1forChartBar(VATReturnVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string FieldDelimeter = DBConstant.FieldDelimeter;

            SqlConnection currConn = null;
            string sqlText = "";
            DataSet ds = new DataSet();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction

                #region unsued quary

                //                declare @SelectBranchId as varchar(100)=0;
                //declare @PeriodName as varchar(100)='September-2019';

                //declare @PeriodID as varchar(100);
                //select  @PeriodId=PeriodId FROM FiscalYear where periodName=@periodName;

                //select Line Gender from (
                //select 'LineA'Line
                //union all
                //select 'LineB'
                //union all
                //select 'LineC'
                //) as a
                //select Description Section from VATReturnV2Notes

                //select * from(
                //select distinct  n.Description Section , 'LineA' Gender,ROUND( Sum(LineA),2)Person
                //--,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC
                //from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

                //where  BranchId=@SelectBranchId 
                //and PeriodID=@PeriodID
                //group by v.NoteNo,v.SubFormName,n.Description

                //union all

                //select distinct  n.Description Section , 'LineB' Gender,ROUND( Sum(LineB),2)Person
                //--,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC
                //from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

                //where  BranchId=@SelectBranchId 
                //and PeriodID=@PeriodID
                //group by v.NoteNo,v.SubFormName,n.Description
                //union all 

                //select distinct  n.Description Section , 'LineC' Gender,ROUND( Sum(LineC),2)Person
                //--,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC
                //from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

                //where  BranchId=@SelectBranchId 
                //and PeriodID=@PeriodID
                //group by v.NoteNo,v.SubFormName,n.Description)
                //as a 
                //order by Section



                //                declare @SelectBranchId as varchar(100)=0;
                //declare @PeriodName as varchar(100)='September-2019';

                //declare @PeriodID as varchar(100);
                //select  @PeriodId=PeriodId FROM FiscalYear where periodName=@periodName;

                //select Line Gender from (
                //select 'LineA'Line
                //union all
                //select 'LineB'
                //union all
                //select 'LineC'
                //) as a
                //select Description Section from VATReturnV2Notes

                //select Section, Gender, ROUND(Person/100000000,0) Person from(
                //select distinct  n.Description Section , 'LineA' Gender,ROUND( Sum(LineA),2)Person
                //--,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC
                //from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

                //where  BranchId=@SelectBranchId 
                //and PeriodID=@PeriodID
                //group by v.NoteNo,v.SubFormName,n.Description

                //union all

                //select distinct  n.Description Section , 'LineB' Gender,ROUND( Sum(LineB),2)Person
                //--,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC
                //from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

                //where  BranchId=@SelectBranchId 
                //and PeriodID=@PeriodID
                //group by v.NoteNo,v.SubFormName,n.Description
                //union all 

                //select distinct  n.Description Section , 'LineC' Gender,ROUND( Sum(LineC),2)Person
                //--,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC
                //from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

                //where  BranchId=@SelectBranchId 
                //and PeriodID=@PeriodID
                //group by v.NoteNo,v.SubFormName,n.Description)
                //as a where Person>=0
                //order by Section







                //declare @SelectBranchId as varchar(100)=0;
                //declare @PeriodName as varchar(100)='September-2019';

                //declare @PeriodID as varchar(100);
                //select  @PeriodId=PeriodId FROM FiscalYear where periodName=@periodName;

                //select Line Gender from (
                //select 'LineA'Line
                //union all
                //select 'LineB'
                //union all
                //select 'LineC'
                //) as a
                //select NoteNo Section from VATReturnV2Notes

                //select Section, Gender, ROUND(Person/100000000,0) Person from(
                //select distinct  n.NoteNo Section , 'LineA' Gender,ROUND( Sum(LineA),2)Person
                //--,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC
                //from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

                //where  BranchId=@SelectBranchId 
                //and PeriodID=@PeriodID
                //group by v.NoteNo,v.SubFormName,n.NoteNo

                //union all

                //select distinct  n.NoteNo Section , 'LineB' Gender,ROUND( Sum(LineB),2)Person
                //--,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC
                //from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

                //where  BranchId=@SelectBranchId 
                //and PeriodID=@PeriodID
                //group by v.NoteNo,v.SubFormName,n.NoteNo
                //union all 

                //select distinct  n.NoteNo Section , 'LineC' Gender,ROUND( Sum(LineC),2)Person
                //--,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC
                //from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

                //where  BranchId=@SelectBranchId 
                //and PeriodID=@PeriodID
                //group by v.NoteNo,v.SubFormName,n.NoteNo)
                //as a where Person>=0
                //order by Section





                #endregion


                #region sql statement
                sqlText = @"

               -- declare @SelectBranchId as varchar(100)=0;
--declare @PeriodName as varchar(100)='September-2019';

declare @PeriodID as varchar(100);
select  @PeriodId=PeriodId FROM FiscalYear where periodName=@periodName;


select  'Note '+CONVERT(varchar(10), Description)Description, Section ,CONVERT(decimal(18,2), [Value])[Value] from(
select distinct  n.NoteNo Description , 'LineA' Section,ROUND( Sum(LineA),2) [Value]
--,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC
from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

where  BranchId=@SelectBranchId 
and PeriodID=@PeriodID
group by v.NoteNo,v.SubFormName,n.NoteNo

union all

select distinct  n.NoteNo Section , 'LineB' Gender,ROUND( Sum(LineB),2)Person
--,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC
from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

where  BranchId=@SelectBranchId 
and PeriodID=@PeriodID
group by v.NoteNo,v.SubFormName,n.NoteNo
union all 

select distinct  n.NoteNo Section , 'LineC' Gender,ROUND( Sum(LineC),2)Person
--,ROUND(Sum(LineB),2)LineB,ROUND(Sum(LineC),2)LineC
from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

where  BranchId=@SelectBranchId 
and PeriodID=@PeriodID
group by v.NoteNo,v.SubFormName,n.NoteNo)
as a 
order by  substring( convert(varchar(10), Section),5,2)

";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                //objComm.Connection = currConn;
                //objComm.CommandText = sqlText;
                //objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@SelectBranchId", vm.BranchId);
                objComm.Parameters.AddWithValue("@periodName", vm.PeriodName);
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.Fill(ds);
                #endregion

            }
            #endregion

            #region catch


            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "SelectAllvat9_1forChartBar", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "SelectAllvat9_1forChartBar", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return ds;

        }

        public List<VATReturnVM> SelectAllvat9_1forChartPie(VATReturnVM vm, SysDBInfoVMTemp connVM = null)
        {
            string FieldDelimeter = DBConstant.FieldDelimeter;

            {
                #region Variables
                SqlConnection currConn = null;
                string sqlText = "";
                List<VATReturnVM> VMs = new List<VATReturnVM>();
                VATReturnVM VM;
                #endregion

                #region try

                try
                {
                    #region open connection and transaction
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                    #endregion open connection and transaction
                    #region sql statement

                    //string Line = "LineA";

                    sqlText = @" 

 --declare @SelectBranchId as varchar(100)=0;
--declare @PeriodName as varchar(100)='September-2019';

declare @PeriodID as varchar(100);
select  @PeriodId=PeriodId FROM FiscalYear where periodName=@periodName;


select  'Note '+CONVERT(varchar(10), Description)Description, Section ,CONVERT(decimal(18,2), [Value])[Value] from(
select distinct  n.NoteNo Description , 

";

                    sqlText += @"'" + vm.Section + "'";
                    sqlText += @" Section,";

                    sqlText += @"ROUND( Sum(";
                    sqlText += vm.Section;
                    sqlText += @"),2) [Value]

from VATReturnV2s v left outer join VATReturnV2Notes n on v.NoteNo=n.NoteNo

where  BranchId=@SelectBranchId 
and PeriodID=@PeriodID   ";

                    if (vm.PartNo == "part3")
                    {
                        sqlText += @" and v.NoteNo>=1 and v.NoteNo<=8";
                    }
                    if (vm.PartNo == "part4")
                    {
                        sqlText += @" and v.NoteNo>=10 and v.NoteNo<=22";
                    }
                    if (vm.PartNo == "part5")
                    {
                        sqlText += @" and v.NoteNo>=24 and v.NoteNo<=27";
                    }
                    if (vm.PartNo == "part6")
                    {
                        sqlText += @" and v.NoteNo>=29 and v.NoteNo<=32";
                    }

                    sqlText += @" group by v.NoteNo,v.SubFormName,n.NoteNo)as a";


                    SqlCommand objComm = new SqlCommand(sqlText, currConn);
                    //objComm.Connection = currConn;
                    //objComm.CommandText = sqlText;
                    //objComm.CommandType = CommandType.Text;

                    objComm.Parameters.AddWithValue("@SelectBranchId", vm.BranchId);
                    objComm.Parameters.AddWithValue("@periodName", vm.PeriodName);

                    SqlDataReader dr;
                    dr = objComm.ExecuteReader();
                    while (dr.Read())
                    {
                        VM = new VATReturnVM();
                        VM.Description = dr["Description"].ToString();
                        VM.Value = Convert.ToDecimal(dr["Value"].ToString());
                        VM.Section = dr["Section"].ToString();
                        VMs.Add(VM);
                    }
                    dr.Close();
                    #endregion
                }
                #endregion

                #region catch
                catch (SqlException sqlex)
                {
                    FileLogger.Log("_9_1_VATReturnDAL", "SelectAllvat9_1forChartPie", sqlex.ToString() + "\n" + sqlText);

                    throw new ArgumentNullException("", sqlex.Message.ToString());

                    ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                }
                catch (Exception ex)
                {
                    FileLogger.Log("_9_1_VATReturnDAL", "SelectAllvat9_1forChartPie", ex.ToString() + "\n" + sqlText);

                    throw new ArgumentNullException("", ex.Message.ToString());

                    ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                }
                #endregion

                #region finally
                finally
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
                #endregion

                return VMs;
            }
        }

        #endregion

        #region MISC Methods

        public string[] DeleteVATReturnV2Details(VATReturnVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Insert

                sqlText = "";
                sqlText += @"
declare @PeriodId as varchar(100);
select  @PeriodId=PeriodId FROM FiscalYear where periodName=@periodName;

delete VATReturnV2Details where  PeriodID = @PeriodId and BranchId = @SelectBranchId 

";


                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn, transaction);
                //cmdInsDetail.Transaction = transaction;

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SelectBranchId", vm.BranchId);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@periodName", vm.PeriodName);




                transResult = (int)cmdInsDetail.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    //throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }
                #endregion Insert only DetailTable

                #region Commit
                if (Vtransaction == null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                #region SuccessResult


                retResults[0] = "Success";
                retResults[1] = "Requested Information successfully Delete ";

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                FileLogger.Log("_9_1_VATReturnDAL", "DeleteVATReturnV2Details", ex.ToString() + "\n" + sqlText);

                //throw ex;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }

            }
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public DataTable Select_VATReturnV2s(VATReturnVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction


                #region sql statement
                #region SqlText

                #region SqlText

                sqlText += @"

declare @PeriodId as varchar(100);

----------------------------------Initialization------------------------
select  @PeriodId=PeriodId FROM FiscalYear where periodName=@periodName;

SELECT Id
      ,UserName
      ,Branch
      ,NoteNo
      ,SubNoteNo
      ,LineA
      ,LineB
      ,LineC
      ,SubFormName
      ,Remarks
      ,PeriodID
      ,PeriodStart
      ,BranchId
  FROM VATReturnV2s
where  PeriodID = @PeriodId and BranchId = @BranchId

";
                #endregion SqlText

                #endregion SqlText

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);

                da.SelectCommand.Parameters.AddWithValue("@periodName", vm.PeriodName);

                da.Fill(dt);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }


                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "Select_VATReturnV2s", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + "~" + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "Select_VATReturnV2s", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + "~" + ex.Message.ToString());
            }
            #endregion

            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        #endregion

        #region NBR API XML Methods

        public ResultVM Get9_1NBR_XML(NBR_APIVM nbrVm)
        {
            ResultVM vm = new ResultVM();
            List<ErrorMessage> errorMessages = new List<ErrorMessage>();

            try
            {
                CompanyprofileDAL companyprofile = new CompanyprofileDAL();
                UserInformationDAL userInformationDal = new UserInformationDAL();

                #region XML For 9.1

                #region Initial Document
                XmlDocument soapEnvelopeDocument = GetSoapEnvelope();
                DateTime currentDate = nbrVm.Period; // need to be dynamic
                #endregion

                try
                {
                    #region Set Message Header

                    SetMessageHeader(nbrVm, soapEnvelopeDocument);

                    #endregion

                    XmlNode I_MAIN_FORM = soapEnvelopeDocument.SelectSingleNode("//I_MAIN_FORM");

                    SetReturnHeaders(I_MAIN_FORM, nbrVm);

                    SetUserInfo(nbrVm, I_MAIN_FORM);

                    #region Get 9_1 Data

                    VATReturnVM vatReturnVm = new VATReturnVM();
                    vatReturnVm.Date = currentDate.ToString("dd-MMM-yyyy");
                    vatReturnVm.PeriodName = currentDate.ToString("MMMM-yyyy");
                    vatReturnVm.PeriodStart = currentDate.ToString("MMMM-yyyy");

                    DataSet dsVAT9_1 = VAT9_1_V2Load(vatReturnVm);

                    #endregion

                    SetMainTag25_68(dsVAT9_1, I_MAIN_FORM);

                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Note 1_22", Message = e.Message });

                }

                #region Setting Params for subforms

                VATReturnSubFormVM subformVm = new VATReturnSubFormVM();

                subformVm.PeriodName = currentDate.ToString("MMMM-yyyy");
                subformVm.ExportInBDT = "Y";
                subformVm.post1 = "Y";
                subformVm.post2 = "Y";
                subformVm.IsSummary = true;
                subformVm.PeriodId = currentDate.ToString("MMyyyy");
                subformVm.PeriodKey = currentDate.ToString("yyMM");

                #endregion

                #region Set Note 1-22

                try
                {

                    subformVm.TableName = "APINote1_22";
                    SetNote1_22Subform(currentDate, soapEnvelopeDocument, subformVm);
                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Note 1_22", Message = e.Message });
                }

                #endregion

                #region Prepare Note 24,29 XMl

                try
                {
                    subformVm.NoteNos = new[] { 24, 29 };
                    subformVm.TableName = "APINote24_29";

                    SetVDSSubform(subformVm, soapEnvelopeDocument);
                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Note24_29", Message = e.Message });
                }

                #endregion

                #region Note 30

                //try
                //{
                //    subformVm.NoteNos = new[] { 30 };
                //    subformVm.TableName = "APINote30";

                //    SetBOESubform(subformVm, soapEnvelopeDocument);
                //}
                //catch (Exception e)
                //{
                //    errorMessages.Add(new ErrorMessage() { ColumnName = "Note30", Message = e.Message });

                //}

                #endregion

                #region Notes 58-64

                try
                {
                    subformVm.NoteNos = new[] { 58, 59, 60, 61, 62, 63, 64 };
                    subformVm.TableName = "APINote58_64";
                    SetNote53_64(subformVm, soapEnvelopeDocument);
                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Note 58 64", Message = e.Message });
                }

                #endregion

                #region Add Attachment to the XML

                //try
                //{
                //    XmlNode IT_SUBF_ATTACH = soapEnvelopeDocument.SelectSingleNode("//IT_SUBF_ATTACH");

                //    foreach (DocumentVM documentVm in nbrVm.CheckedDocuments)
                //    {
                //        XmlNode ITEM = soapEnvelopeDocument.CreateElement("ITEM");

                //        XmlNode ATT_DOCTYPE = soapEnvelopeDocument.CreateElement("ATT_DOCTYPE");
                //        ATT_DOCTYPE.InnerText = documentVm.ATT_DOCTYPE;
                //        ITEM.AppendChild(ATT_DOCTYPE);

                //        XmlNode TEXT = soapEnvelopeDocument.CreateElement("TEXT");
                //        TEXT.InnerText = documentVm.TEXT;
                //        ITEM.AppendChild(TEXT);

                //        XmlNode NOTES = soapEnvelopeDocument.CreateElement("NOTES");
                //        NOTES.InnerText = documentVm.NOTES;
                //        ITEM.AppendChild(NOTES);


                //        XmlNode CHKBOX = soapEnvelopeDocument.CreateElement("CHKBOX");
                //        if (!string.IsNullOrEmpty(documentVm.CHKBOX))
                //        {
                //            CHKBOX.InnerText = documentVm.CHKBOX;
                //        }
                //        ITEM.AppendChild(CHKBOX);

                //        IT_SUBF_ATTACH.AppendChild(ITEM);
                //    }
                //}
                //catch (Exception e)
                //{
                //    errorMessages.Add(new ErrorMessage() { ColumnName = "Attachment Check Box", Message = e.Message });
                //}


                //try
                //{
                //    XmlNode ATTACH_DOCUMENT = soapEnvelopeDocument.SelectSingleNode("//ATTACH_DOCUMENT");

                //    foreach (AttachedFileVM attachedFileVm in nbrVm.AttachedFiles)
                //    {
                //        XmlNode ITEM = soapEnvelopeDocument.CreateElement("ITEM");

                //        XmlNode FILENAME = soapEnvelopeDocument.CreateElement("FILENAME");
                //        FILENAME.InnerText = attachedFileVm.FILENAME;
                //        ITEM.AppendChild(FILENAME);

                //        XmlNode CONTENT = soapEnvelopeDocument.CreateElement("CONTENT");
                //        CONTENT.InnerText = attachedFileVm.CONTENT;
                //        ITEM.AppendChild(CONTENT);

                //        XmlNode FILETYPE = soapEnvelopeDocument.CreateElement("FILETYPE");
                //        FILETYPE.InnerText = attachedFileVm.FILETYPE;
                //        ITEM.AppendChild(FILETYPE);

                //        XmlNode DOCTYPE = soapEnvelopeDocument.CreateElement("DOCTYPE");
                //        DOCTYPE.InnerText = attachedFileVm.DOCTYPE;
                //        ITEM.AppendChild(DOCTYPE);

                //        ATTACH_DOCUMENT.AppendChild(ITEM);
                //    }
                //}
                //catch (Exception e)
                //{
                //    errorMessages.Add(new ErrorMessage() { ColumnName = "Attached Document", Message = e.Message });
                //}

                #endregion

                #region Notes 26,31

                try
                {
                    subformVm.NoteNos = new[] { 26, 31 };
                    subformVm.TableName = "APINote26_31";
                    SetNote26_31(subformVm, soapEnvelopeDocument);
                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Note 26 31", Message = e.Message });
                }

                #endregion

                #region 27, 32

                try
                {
                    subformVm.NoteNos = new[] { 27, 32 };
                    subformVm.TableName = "APINote27_32";
                    SetNote27_32(subformVm, soapEnvelopeDocument);
                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Note 27 32", Message = e.Message });
                }

                #endregion

                #endregion

                vm.XML = soapEnvelopeDocument.InnerXml;
                vm.ErrorList = errorMessages;

                // XML validation with xsd

                vm.XML = vm.XML.Replace("<MT_RET_MSG_REQ",
                    "<MT_RET_MSG_REQ xmlns=\"http://nbr.gov.bd/regist91\" xsi:schemaLocation=\"http://nbr.gov.bd/regist91 schema.xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
                vm.XML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + vm.XML;

                string xsdString = GetXSDString();

                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add("http://nbr.gov.bd/regist91", XmlReader.Create(new StringReader(xsdString)));

                XDocument validator = XDocument.Load(new StringReader(vm.XML));
                validator.Validate(schemaSet, (sender, args) =>
                {
                    vm.ErrorList.Add(new ErrorMessage() { ColumnName = "XSD Validation", Message = args.Message });

                });

                if (vm.ErrorList.Count == 0)
                {
                    vm.Status = "success";
                }
            }
            catch (Exception e)
            {
                vm.Message = e.Message;
                vm.Status = "fail";

                FileLogger.Log("FormNBRAPI", "GETXML", e.ToString());

                errorMessages.Add(new ErrorMessage() { ColumnName = "Main Method", Message = e.Message });

            }

            return vm;
        }

        private string GetXSDString()
        {
            return @"<xsd:schema targetNamespace=""http://nbr.gov.bd/regist91"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                    xmlns=""http://nbr.gov.bd/regist91"" elementFormDefault=""qualified"">
            <xsd:element name=""MT_RET_MSG_REQ"" type=""DT_RET_MSG_REQ_91""/>
            <xsd:complexType name=""DT_RET_SUBF_CHALLAN"">
                <xsd:annotation>
                    <xsd:appinfo source=""http://sap.com/xi/VersionID"">15fe1299201a11ebb5df00001e5c581e</xsd:appinfo>
                </xsd:annotation>
                <xsd:sequence>
                    <xsd:element name=""FIELD_ID"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">63503348201a11eb9cb300090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Form serial ID</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:pattern value=""SL(5[3489]|6[012])""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""CHALL_NO"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">63503349201a11eb9f6a00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Challan number</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""60""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""CHALL_DATE"" type=""date_fomat"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">6350334a201a11eba9cc00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Character Field Length = 10</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""ACCOUNT_CODE"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">6350334b201a11eb8fc100090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Account code</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""20""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""CHAN_AMT"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">6350334c201a11eb9dc700090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""NOTES"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">6350334d201a11ebb47200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Notes</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""255""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""BANCD"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">6350334e201a11eb905300090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Bank Code</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""25""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""BANKL"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">6350334f201a11eb940200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Bank number</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""60""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""A_NAMEOFBANK"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">63503350201a11ebbff100090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Name of the Bank</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""60""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""A_BRANCH"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">63503351201a11ebb55200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Name of Branch</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""60""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                </xsd:sequence>
            </xsd:complexType>
            <xsd:complexType name=""DT_RET_SUBF_VDS"">
                <xsd:annotation>
                    <xsd:appinfo source=""http://sap.com/xi/VersionID"">5ee15284201a11ebcbdd00001e5c581e</xsd:appinfo>
                </xsd:annotation>
                <xsd:sequence>
                    <xsd:element name=""FIELD_ID"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bcc9d201a11ebc47600090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Form serial ID</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:enumeration value=""SL25""/>
                                <xsd:enumeration value=""SL30""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""BIN"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bcc9e201a11eb934d00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Bin 14 digits</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:pattern value=""([0-9]\d{8}-[0-9]\d{3})|([0-9]\d{8})""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""NAME"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bcc9f201a11ebb8d000090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Suppliers name</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""255""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""ADDRESS"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bcca0201a11eb918e00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Return address</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""255""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""VALUE"" type=""empty_or_decimal_15.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bcca1201a11ebc53600090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""DEDUCT_VAT"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bcca2201a11eb92ea00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""INVOICE_NO"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bcca3201a11eba6c800090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Invoice number</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""60""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""INVOICE_DATE"" type=""date_fomat"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bcca4201a11eb9aa700090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Character Field Length = 10</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""CERT_NO"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bcca5201a11ebb4c200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Received VAT Deduction at Source, Certificate No.</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""60""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""CERT_DATE"" type=""date_fomat"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bcca6201a11eb923200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Character Field Length = 10</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""ACCOUNT_CODE"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bcca7201a11ebbfa200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>VDS account number</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""60""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""DEPOSIT_NO"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bcca8201a11ebb83400090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Tax Deposited Serial no. of Book Transfer</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""60""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""DEPOSIT_DATE"" type=""date_fomat"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bcca9201a11eb969900090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Character Field Length = 10</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""NOTE"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ac7bccaa201a11eb935500090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Notes</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""255""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                </xsd:sequence>
            </xsd:complexType>
            <xsd:complexType name=""DT_RET_SUBF_BOE"">
                <xsd:annotation>
                    <xsd:appinfo source=""http://sap.com/xi/VersionID"">58737460201911ebc58a00001e5c581e</xsd:appinfo>
                </xsd:annotation>
                <xsd:sequence>
                    <xsd:element name=""FIELD_ID"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">a4d3449a201911eb8a0300090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Form serial ID</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""30""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""BOE"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">a4d3449b201911eba22100090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>BOE number</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""20""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""BOE_DATE"" type=""date_fomat"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">a4d369e6201911eb82cb00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Character Field Length = 10</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""CUS_STATION"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">a4d369e7201911eba22700090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Customs Station</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""3""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""ATV_AMOUNT"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">a4d395b6201911eb911000090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""NOTES"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">a4d395b7201911ebaf1800090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Notes</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""255""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                </xsd:sequence>
            </xsd:complexType>
            <xsd:complexType name=""DT_RET_SUBF_ADJUST"">
                <xsd:annotation>
                    <xsd:appinfo source=""http://sap.com/xi/VersionID"">3f21578d7c9f11eba06100001e636c3a</xsd:appinfo>
                </xsd:annotation>
                <xsd:sequence>
                    <xsd:element name=""FIELD_ID"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f5d66d7c9d11ebb56300059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>Form serial ID</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:enumeration value=""SL126""/>
                                <xsd:enumeration value=""SL131""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""NOTE_NO"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f5d66e7c9d11ebb0bb00059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>Debit/Credit No</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""30""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""ISSUE_DAT"" type=""date_fomat"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f5d66f7c9d11ebc23800059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>Character Field Length = 10</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""CHALL_NO"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f5d6707c9d11ebbb0400059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>Challan number</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""60""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""CHALL_DAT"" type=""date_fomat"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f5d6717c9d11ebbdf400059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>Character Field Length = 10</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""REASON"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f5d6727c9d11eb913f00059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""255""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""VALUE"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f5d6737c9d11ebcf8800059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""QUANTITY"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f60c707c9d11eb916a00059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""VAT"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f60c717c9d11eb897200059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""SD"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f60c727c9d11ebc8db00059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""VALUE_ADJ"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f60c737c9d11eba74100059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""QUANTITY_ADJ"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f60c747c9d11eba02400059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""VAT_ADJ"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f60c757c9d11ebabf400059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""SD_ADJ"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f60c767c9d11ebb7f100059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""NOTES"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">59f60c777c9d11eba49900059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>Notes</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""255""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                </xsd:sequence>
            </xsd:complexType>
            <xsd:complexType name=""DT_RET_SUBF_ATTACH"">
                <xsd:annotation>
                    <xsd:appinfo source=""http://sap.com/xi/VersionID"">8577edde201811ebaee700001e5c581e</xsd:appinfo>
                </xsd:annotation>
                <xsd:sequence>
                    <xsd:element name=""CHKBOX"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">d104b515201811ebc64700090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Indicator: Line selected</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""1""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""ATT_DOCTYPE"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">d1049928201811eba79c00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Attachment document type</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""2""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""TEXT"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">d1049929201811eb95b500090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Error text after posting</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""250""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""NOTES"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">d104992a201811eb809400090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Notes</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""255""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                </xsd:sequence>
            </xsd:complexType>
            <xsd:complexType name=""DT_RET_SUBF_OTHER"">
                <xsd:annotation>
                    <xsd:appinfo source=""http://sap.com/xi/VersionID"">2090c04c7ca011eb8f5c00001e636c3a</xsd:appinfo>
                </xsd:annotation>
                <xsd:sequence>
                    <xsd:element name=""FIELD_ID"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">390d69617c9e11eb943600059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>Form serial ID</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:enumeration value=""SL127""/>
                                <xsd:enumeration value=""SL132""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""CHALL_NO"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">390d69627c9e11ebcc3000059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>Challan number</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""60""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""CHALL_DATE"" type=""date_fomat"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">390d69637c9e11ebccaa00059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>Character Field Length = 10</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""AMOUNT"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">390d69647c9e11eb85b600059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""VAT"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">390d69657c9e11eb89a300059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""NOTES"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">390d69667c9e11eb87df00059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>Notes</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""255""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                </xsd:sequence>
            </xsd:complexType>
            <xsd:complexType name=""DT_ATTACH_DOCUMENT"">
                <xsd:annotation>
                    <xsd:appinfo source=""http://sap.com/xi/VersionID"">37e07269201511eb9d7f00001e5c581e</xsd:appinfo>
                </xsd:annotation>
                <xsd:sequence>
                    <xsd:element name=""FILENAME"" type=""xsd:string"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ed1e9cd0201711ebb13300090faa0001
                            </xsd:appinfo>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""CONTENT"" type=""xsd:string"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ed1e9cd1201711ebc0c500090faa0001
                            </xsd:appinfo>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""FILETYPE"" type=""xsd:string"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ed1e9cd2201711ebab8700090faa0001
                            </xsd:appinfo>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""DOCTYPE"" type=""xsd:string"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">ed1e9cd3201711eb938800090faa0001
                            </xsd:appinfo>
                        </xsd:annotation>
                    </xsd:element>
                </xsd:sequence>
            </xsd:complexType>
            <xsd:complexType name=""DT_RET_TBF_GOSERV"">
                <xsd:annotation>
                    <xsd:appinfo source=""http://sap.com/xi/VersionID"">116532e8201b11eba6d700001e5c581e</xsd:appinfo>
                </xsd:annotation>
                <xsd:sequence>
                    <xsd:element name=""FIELD_ID"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Form serial ID</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:pattern value=""SL(0[1-8]|1[0-8]|2[0-3])""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""ACT_SALE_VALUE"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f82201b11eb92f800090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""ASSESS_VALUE"" type=""empty_or_decimal_15.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f82201b11eb92f800090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""AT"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7e201b11ebc9ad00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""BOE_DATE"" type=""date_fomat"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7b201b11ebc68900090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Goods/Services Code</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""BOE_ITM_NO"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7b201b11ebc68900090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Goods/Services Code</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""20""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""BOE_NO"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Form serial ID</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""50""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""BOE_OFF_CODE"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7b201b11ebc68900090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Goods/Services Code</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""10""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""BOE_TYPE"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Form serial ID</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:enumeration value=""2""/>
                                <xsd:enumeration value=""3""/>
                                <xsd:enumeration value=""""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""CATEGORY"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f86201b11ebb83500090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Category</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""160""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""CPC_CODE"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7b201b11ebc68900090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Goods/Services Code</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""50""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""DESCRIPTION"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7f201b11ebb56c00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Description</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""255""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""GOOD_SERVICE"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7b201b11ebc68900090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Goods/Services Code</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""15""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""INVOICE_DATE"" type=""date_fomat"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f84201b11ebc95d00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>item id</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""INVOICE_NO"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f84201b11ebc95d00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>item id</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""20""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""ITEM_ID"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f84201b11ebc95d00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>item id</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""10""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""NAME"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f85201b11ebbcc500090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Good, service name</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""255""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""NOTES"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f80201b11ebab7400090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Notes</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""255""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""QUANTITY"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f81201b11ebb94a00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""10""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""SD"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7d201b11ebabf600090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""14""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""UNIT"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f83201b11eb9fb500090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Units of Measurement</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:pattern value=""(0[1-9]|1[0-7])?""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""VALUE"" type=""empty_or_decimal_18.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7c201b11eb895c00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""VAT"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7e201b11ebc9ad00090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>30 Characters</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                </xsd:sequence>
            </xsd:complexType>
            <xsd:complexType name=""DT_RET_PI_XML_DATA_MF"">
                <xsd:annotation>
                    <xsd:appinfo source=""http://sap.com/xi/VersionID"">485ca2b180ab11eb924800001e636c3a</xsd:appinfo>
                </xsd:annotation>
                <xsd:sequence>
                    <!--s1-->
                    <xsd:element name=""A_F_S1_BIN"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>BIN</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:pattern value=""[0-9]{9}-[0-9]{4}""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""PERIOD_KEY"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">6d958b447c9711ebb90b00059a3c7a00
                            </xsd:appinfo>
                            <xsd:documentation>Period Key</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:pattern value=""2[0-9](0[1-9]|1[012])""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""A_F_S2_TYPE_RETURN"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Dropdownlist for 9.1 9.2</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:pattern value=""[0-9]""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""A_F_S2_ANY_ACTIVITIES"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Any Activities</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:enumeration value=""1""/>
                                <xsd:enumeration value=""2""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""A_F_S2_SUBMISSION_DAT"" type=""date_fomat"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Date of Submission</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S2_REASON_AMENDED"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Date of Submission</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""255""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""A_F_S5_PAYMENT_NOT_BANKING_AMT"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Payment Not Made Through Banking Channel</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S7_SD_AGAINST_EXPORT"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Supplementary Duty Paid on Inputs Against Exports</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S7_FINE_PENALTY_OTHER"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Other Fine/Penalty/Interest</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S7_EXCISE_DUTY"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Payable Excise Duty</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S7_DEVELOP_SURCHARGE"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Payable Development Surcharge</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S7_ICT_DEVELOP_SURCHARGE"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Payable ICT Development Surcharge</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S7_HEALTH_CARE_SURCHARGE"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Payable Health Care Surcharge</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S7_ENV_PROTECT_SURCHARGE"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Payable Environmental Protection Surcharge</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S7_CB_LAST_TP_VAT"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Closing Balance of Last Tax Period (VAT)</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S7_CB_LAST_TP_SD"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Closing Balance of Last Tax Period (SD)</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S11_REFUND_VAT"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Refund VAT</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S11_REFUND_SD"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Refund SD</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S10_REFUND"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Refund</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:pattern value=""[0-9]""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""A_F_S10_NAME"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Name</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""160""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""A_F_S10_DESIGNATION"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Designation</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""160""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""A_F_S10_MOBILE"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Mobile Number</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:pattern value=""01\d{9}""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""A_F_S10_NID_PP"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>National ID/Passport Number</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:maxLength value=""17""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""A_F_S10_EMAIL"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Email</xsd:documentation>
                        </xsd:annotation>
                        <xsd:simpleType>
                            <xsd:restriction base=""xsd:string"">
                                <xsd:pattern
                                        value=""([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})""/>
                            </xsd:restriction>
                        </xsd:simpleType>
                    </xsd:element>
                    <xsd:element name=""A_F_S7_INTEREST_OVERDUE_VAT"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Interest on Overdue VAT (Based on note 35)</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S7_INTEREST_OVERDUE_SD"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Interest on Overdue SD (Based on note 37)</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""A_F_S7_FINE_PENALTY"" type=""empty_or_decimal_10.2"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">5af97f7a201b11ebb9b200090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Fine/Penalty for Non-submission of Return</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                </xsd:sequence>
            </xsd:complexType>
            <xsd:complexType name=""DT_RET_MSG_HEADER"">
                <xsd:annotation>
                    <xsd:appinfo source=""http://sap.com/xi/VersionID"">19b1b010201811eba1e100001e5c581e</xsd:appinfo>
                </xsd:annotation>
                <xsd:sequence>
                    <xsd:element name=""MSGID"" type=""xsd:string"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">65bc4468201811eba11000090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Message ID</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <!--                    <xsd:element name=""TRANS_CODE"" type=""xsd:string"">-->
                    <!--                        <xsd:annotation>-->
                    <!--                            <xsd:appinfo source=""http://sap.com/xi/TextID"">65bc6c49201811eb991100090faa0001-->
                    <!--                            </xsd:appinfo>-->
                    <!--                            <xsd:documentation>R/2 table</xsd:documentation>-->
                    <!--                        </xsd:annotation>-->
                    <!--                    </xsd:element>-->
                    <xsd:element name=""SEND_DATE"" type=""xsd:string"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">65bc6c4a201811ebbb8400090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Char Date format</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""SEND_TIME"" type=""xsd:string"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">65bc6c4b201811eb9e1400090faa0001
                            </xsd:appinfo>
                            <xsd:documentation>Character field, 8 characters long</xsd:documentation>
                        </xsd:annotation>
                    </xsd:element>
                </xsd:sequence>
            </xsd:complexType>
            <xsd:complexType name=""DT_RET_MSG_REQ_91"">
                <xsd:annotation>
                    <xsd:appinfo source=""http://sap.com/xi/VersionID"">5d53bbad80ac11eb9ae800001e636c3a</xsd:appinfo>
                </xsd:annotation>
                <xsd:sequence>
                    <xsd:element name=""I_MSG_HDR"" type=""DT_RET_MSG_HEADER"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">6dd9f32a201d11eba6a900090faa0001
                            </xsd:appinfo>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""I_MAIN_FORM"" type=""DT_RET_PI_XML_DATA_MF"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">a0a1e8ec7c9711ebc77a00059a3c7a00
                            </xsd:appinfo>
                        </xsd:annotation>
                    </xsd:element>
                    <xsd:element name=""IT_SUBF_GOSERV""  minOccurs=""0"" maxOccurs=""unbounded"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">cf71d13c201e11eb8a9600090faa0001
                            </xsd:appinfo>
                        </xsd:annotation>
                        <xsd:complexType>
                            <xsd:sequence>
                                <xsd:element name=""ITEM"" type=""DT_RET_TBF_GOSERV"" minOccurs=""0"" maxOccurs=""unbounded"">
                                    <xsd:annotation>
                                        <xsd:appinfo source=""http://sap.com/xi/TextID"">
                                            cf71d13b201e11eba44c00090faa0001
                                        </xsd:appinfo>
                                    </xsd:annotation>
                                </xsd:element>
                            </xsd:sequence>
                        </xsd:complexType>
                    </xsd:element>
                    <xsd:element name=""IT_SUBF_VDS"" minOccurs=""0"" maxOccurs=""unbounded"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">cf71d13e201e11ebb65600090faa0001
                            </xsd:appinfo>
                        </xsd:annotation>
                        <xsd:complexType>
                            <xsd:sequence>
                                <xsd:element name=""ITEM"" type=""DT_RET_SUBF_VDS"" minOccurs=""0"" maxOccurs=""unbounded"">
                                    <xsd:annotation>
                                        <xsd:appinfo source=""http://sap.com/xi/TextID"">
                                            cf71d13d201e11eb9fa600090faa0001
                                        </xsd:appinfo>
                                    </xsd:annotation>
                                </xsd:element>
                            </xsd:sequence>
                        </xsd:complexType>
                    </xsd:element>
                    <xsd:element name=""IT_SUBF_CHALLAN"" minOccurs=""0"" maxOccurs=""unbounded"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">cf71d142201e11eba18500090faa0001
                            </xsd:appinfo>
                        </xsd:annotation>
                        <xsd:complexType>
                            <xsd:sequence>
                                <xsd:element name=""ITEM"" type=""DT_RET_SUBF_CHALLAN"" minOccurs=""1"" maxOccurs=""unbounded"">
                                    <xsd:annotation>
                                        <xsd:appinfo source=""http://sap.com/xi/TextID"">
                                            cf71d141201e11eb898300090faa0001
                                        </xsd:appinfo>
                                    </xsd:annotation>
                                </xsd:element>
                            </xsd:sequence>
                        </xsd:complexType>
                    </xsd:element>
                    <xsd:element name=""IT_SUBF_ADJUST"" minOccurs=""0"" maxOccurs=""unbounded"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">781048d880aa11eb9c2700059a3c7a00
                            </xsd:appinfo>
                        </xsd:annotation>
                        <xsd:complexType>
                            <xsd:sequence>
                                <xsd:element name=""ITEM"" type=""DT_RET_SUBF_ADJUST"" minOccurs=""0"" maxOccurs=""unbounded"">
                                    <xsd:annotation>
                                        <xsd:appinfo source=""http://sap.com/xi/TextID"">
                                            781048d780aa11ebb06500059a3c7a00
                                        </xsd:appinfo>
                                    </xsd:annotation>
                                </xsd:element>
                            </xsd:sequence>
                        </xsd:complexType>
                    </xsd:element>
                    <xsd:element name=""IT_SUBF_OTHER"" minOccurs=""0"" maxOccurs=""unbounded"">
                        <xsd:annotation>
                            <xsd:appinfo source=""http://sap.com/xi/TextID"">78108dc080aa11eb882a00059a3c7a00
                            </xsd:appinfo>
                        </xsd:annotation>
                        <xsd:complexType>
                            <xsd:sequence>
                                <xsd:element name=""ITEM"" type=""DT_RET_SUBF_OTHER"" minOccurs=""0"" maxOccurs=""unbounded"">
                                    <xsd:annotation>
                                        <xsd:appinfo source=""http://sap.com/xi/TextID"">
                                            781048d980aa11eb863a00059a3c7a00
                                        </xsd:appinfo>
                                    </xsd:annotation>
                                </xsd:element>
                            </xsd:sequence>
                        </xsd:complexType>
                    </xsd:element>
                </xsd:sequence>
            </xsd:complexType>
            <!--type empty_or_decimal_10.2-->
            <xsd:simpleType name=""empty_or_decimal_10.2"">
                <xsd:union memberTypes=""empty decimal_10.2""/>
            </xsd:simpleType>
            <!--type empty_or_decimal_15.2-->
            <xsd:simpleType name=""empty_or_decimal_15.2"">
                <xsd:union memberTypes=""empty decimal_15.2""/>
            </xsd:simpleType>
            <!--type empty_or_decimal_18.2-->
            <xsd:simpleType name=""empty_or_decimal_18.2"">
                <xsd:union memberTypes=""empty decimal_18.2""/>
            </xsd:simpleType>
            <xsd:simpleType name=""empty"">
                <xsd:restriction base=""xsd:string"">
                    <xsd:enumeration value=""""/>
                </xsd:restriction>
            </xsd:simpleType>
            <!---->
            <xsd:simpleType name=""decimal_10.2"">
                <xsd:restriction base=""xsd:decimal"">
                    <xsd:maxInclusive value=""9999999999""/>
                    <xsd:minInclusive value=""0""/>
                    <xsd:fractionDigits value=""2""/>
                </xsd:restriction>
            </xsd:simpleType>
            <!---->
            <xsd:simpleType name=""decimal_15.2"">
                <xsd:restriction base=""xsd:decimal"">
                    <xsd:maxInclusive value=""999999999999999""/>
                    <xsd:minInclusive value=""0""/>
                    <xsd:fractionDigits value=""2""/>
                </xsd:restriction>
            </xsd:simpleType>
            <!---->
            <xsd:simpleType name=""decimal_18.2"">
                <xsd:restriction base=""xsd:decimal"">
                    <xsd:maxInclusive value=""999999999999999999""/>
                    <xsd:minInclusive value=""0""/>
                    <xsd:fractionDigits value=""2""/>
                </xsd:restriction>
            </xsd:simpleType>
            <!---->
            <xsd:simpleType name=""date_fomat"">
                <xsd:restriction base=""xsd:string"">
                    <xsd:pattern value=""((0[1-9]|[12][0-9]|3[01])(\/|.)(0[1-9]|1[012])(\/|.)(19|20)\d\d)?""/>
                </xsd:restriction>
            </xsd:simpleType>
            <!---->
        </xsd:schema>";
        }

        private void SetNote53_64(VATReturnSubFormVM subformVm, XmlDocument soapEnvelopeDocument)
        {
            DataTable sunform53_64 = GetAPIData(subformVm);

            XmlNode IT_SUBF_CHALLAN = soapEnvelopeDocument.SelectSingleNode("//IT_SUBF_CHALLAN");

            NbrApi nbrApi = new NbrApi();

            List<Item> banks = nbrApi.Get_Banks();

            foreach (DataRow dataRow in sunform53_64.Rows)
            {
                XmlNode ITEM = GetNote53_64ItemXML(soapEnvelopeDocument, dataRow, banks);

                IT_SUBF_CHALLAN.AppendChild(ITEM);
            }
        }

        private void SetNote26_31(VATReturnSubFormVM subformVm, XmlDocument soapEnvelopeDocument)
        {
            DataTable sunform26_31 = GetAPIData(subformVm);

            XmlNode IT_SUBF_ADJUST = soapEnvelopeDocument.SelectSingleNode("//IT_SUBF_ADJUST");

            //if (sunform26_31.Rows.Count == 0)
            //{
            //    sunform26_31.Rows.Add(sunform26_31.NewRow());
            //    sunform26_31.Rows[0]["NoteNo"] = "26";
            //}

            foreach (DataRow dataRow in sunform26_31.Rows)
            {
                XmlNode ITEM = GetNote26_31ItemXML(soapEnvelopeDocument, dataRow);
                IT_SUBF_ADJUST.AppendChild(ITEM);
            }


        }

        private void SetNote27_32(VATReturnSubFormVM subformVm, XmlDocument soapEnvelopeDocument)
        {
            DataTable sunform27_32 = GetAPIData(subformVm);

            XmlNode IT_SUBF_OTH = soapEnvelopeDocument.SelectSingleNode("//IT_SUBF_OTHER");

            //if (sunform27_32.Rows.Count == 0)
            //{
            //    sunform27_32.Rows.Add(sunform27_32.NewRow());
            //    sunform27_32.Rows[0]["NoteNo"] = "27";
            //}

            foreach (DataRow dataRow in sunform27_32.Rows)
            {
                XmlNode ITEM = GetNote27_32ItemXML(soapEnvelopeDocument, dataRow);
                IT_SUBF_OTH.AppendChild(ITEM);
            }

            //if (sunform27_32.Rows.Count == 0)
            //{
            //    XmlNode ITEM = soapEnvelopeDocument.CreateElement("ITEM");
            //    IT_SUBF_OTH.AppendChild(ITEM);
            //}
        }

        private void SetBOESubform(VATReturnSubFormVM subformVm, XmlDocument soapEnvelopeDocument)
        {
            DataTable subFormVDSData = GetAPIData(subformVm);

            NbrApi nbrApi = new NbrApi();

            List<Item> houses = nbrApi.Get_Cushouse();

            XmlNode IT_SUBF_BOE = soapEnvelopeDocument.SelectSingleNode("//IT_SUBF_BOE");

            decimal totalAT = 0;

            foreach (DataRow dataRow in subFormVDSData.Rows)
            {
                if (dataRow["NoteNo"].ToString() == "30")
                {
                    totalAT += Convert.ToDecimal(dataRow["ATAMOUNT"]);
                }


                XmlNode ITEM = GetNote30ItemXML(soapEnvelopeDocument, dataRow, houses);

                IT_SUBF_BOE.AppendChild(ITEM);
            }

            //totalAT = totalAT;
        }

        private void SetVDSSubform(VATReturnSubFormVM subformVm, XmlDocument soapEnvelopeDocument)
        {
            DataTable subFormVDSData = GetAPIData(subformVm);

            XmlNode IT_SUBF_VDS = soapEnvelopeDocument.SelectSingleNode("//IT_SUBF_VDS");
            decimal totalVDSAmount = 0;
            foreach (DataRow dataRow in subFormVDSData.Rows)
            {
                if (dataRow["NoteNo"].ToString() == "24")
                {
                    totalVDSAmount += Convert.ToDecimal(dataRow["VDSAmount"]);
                }

                XmlNode ITEM = GetNote24_29ItemXML(soapEnvelopeDocument, dataRow);

                IT_SUBF_VDS.AppendChild(ITEM);
            }

            //totalVDSAmount = totalVDSAmount;
        }

        private void SetNote1_22Subform(DateTime currentDate, XmlDocument soapEnvelopeDocument, VATReturnSubFormVM subformVm)
        {
            #region Data Note 1-22

            DataTable subFormData1_22 = GetAPI1_22Data(subformVm);

            // calling API
            // update database
            // changing XML
            DataTable dtHSCodes = subFormData1_22.DefaultView.ToTable(true, "ProductCode", "NoteNo", "VATRate", "SDRate");
            NbrApi nbrApi = new NbrApi();
            List<Item> items = new List<Item>();

            string errorMessages = "";
            decimal totalSaleVAT = 0;
            decimal totalPurchaseVAT = 0;
            string[] manualNotes = GetManualNotes();

            CompanyprofileDAL companyprofileDal = new CompanyprofileDAL();
            List<CompanyCategoryVM> list = companyprofileDal.GetCompanyTypes();
            CompanyprofileDAL companyprofile = new CompanyprofileDAL();
            CompanyProfileVM profileVm = companyprofile.SelectAllList().FirstOrDefault();

            foreach (DataRow dataRow in dtHSCodes.Rows)
            {
                try
                {
                    string hsCode = dataRow["ProductCode"].ToString();
                    string noteNo = dataRow["NoteNo"].ToString();
                    decimal vatRate = Convert.ToDecimal(dataRow["VATRate"].ToString());
                    decimal sdRate = Convert.ToDecimal(dataRow["SDRate"].ToString());
                    string slNo = nbrApi.GetSLNo(noteNo);

                    if (profileVm.CompanyType == "05" && noteNo == "8")
                    {
                        Item item = new Item()
                        {
                            GOODS_SERVICE_CODE = hsCode,
                            NoteNo = noteNo,
                            VAT_RATE = vatRate,
                            SD_RATE = sdRate
                        };


                        if (vatRate == 15 && sdRate == 0)
                        {
                            item.ITEM_ID = list.FirstOrDefault(x =>
                                x.GOODS_SERVICE_NAME.Equals("Full VAT-able (VAT 15%, SD 0%)", StringComparison.OrdinalIgnoreCase)).ITEM_ID;

                            item.GOODS_SERVICE_CODE = list.FirstOrDefault(x =>
                                x.GOODS_SERVICE_NAME.Equals("Full VAT-able (VAT 15%, SD 0%)", StringComparison.OrdinalIgnoreCase)).GOODS_SERVICE_CODE;
                        }

                        if (vatRate == 0 && sdRate == 0)
                        {
                            item.ITEM_ID = list.FirstOrDefault(x =>
                                x.GOODS_SERVICE_NAME.Equals("100% Exemption (VAT 0%, SD 0%)", StringComparison.OrdinalIgnoreCase)).ITEM_ID;

                            item.GOODS_SERVICE_CODE = list.FirstOrDefault(x =>
                                x.GOODS_SERVICE_NAME.Equals("Full VAT-able (VAT 15%, SD 0%)", StringComparison.OrdinalIgnoreCase)).GOODS_SERVICE_CODE;
                        }

                        if (string.IsNullOrEmpty(item.ITEM_ID))
                        {
                            throw new Exception("VAT or SD rate did not match with the provided note 8 HS codes");
                        }


                        items.Add(item);
                    }
                    else
                    {
                        List<Item> tempListItems = nbrApi.GetGoods_Service(hsCode, noteNo,
                            subformVm.PeriodKey);

                        //Item item = tempListItems.FirstOrDefault();

                        //if (hsCode == "3004.90.99" && noteNo == "4")
                        //{
                        //    hsCode = hsCode;
                        //}

                        Item item = tempListItems.FirstOrDefault(x =>
                            x.ValidTo >= DateTime.Now && x.VAT_RATE == vatRate && x.SD_RATE == sdRate && x.NOTE.Contains(slNo));

                        if (item == null && !manualNotes.Contains(slNo))
                            throw new Exception("VAT or SD Rate not matched for " + hsCode + " in Note " + noteNo);

                        if (manualNotes.Contains(slNo) && item == null)
                            item = tempListItems.FirstOrDefault(x =>
                                x.ValidTo >= DateTime.Now && x.NOTE.Contains(slNo));


                        item.NoteNo = noteNo;
                        items.Add(item);
                    }

                }
                catch (Exception e)
                {
                    // need to throw here
                    FileLogger.Log("9_1 DAL", "SetNote1_22Subform", e.ToString());

                    errorMessages += e.Message + "\n";
                }
            }

            if (!string.IsNullOrEmpty(errorMessages))
            {
                throw new Exception(errorMessages);
            }

            if (items != null && items.Count > 0)
            {
                UpdateItemId(items.ToDataTable());
            }


            #endregion

            #region Prepare Note 1-22 XMl

            XmlNode IT_SUBF_GOSERV = soapEnvelopeDocument.SelectSingleNode("//IT_SUBF_GOSERV");


            PrepareXmlVm xmlVm = new PrepareXmlVm()
            {
                Items = items,
                ProfileVm = profileVm,
                XmlDocument = soapEnvelopeDocument,
            };

            foreach (DataRow dataRow in subFormData1_22.Rows)
            {
                xmlVm.DataRow = dataRow;

                //if (dataRow["NoteNo"].ToString() == "4")
                //{
                //    totalSaleVAT += Convert.ToDecimal(dataRow["VATAmount"]);
                //}
                ////|| dataRow["NoteNo"].ToString() == "15" || dataRow["NoteNo"].ToString() == "16"
                //if (dataRow["NoteNo"].ToString() == "14" )
                //{
                //    totalPurchaseVAT += Convert.ToDecimal(dataRow["VATAmount"]);
                //}

                XmlNode ITEM = GetNote1_22ItemXML(xmlVm);

                IT_SUBF_GOSERV.AppendChild(ITEM);
            }

            //totalSaleVAT = totalSaleVAT;

            #endregion
        }

        private void SetMessageHeader(NBR_APIVM nbrVm, XmlDocument soapEnvelopeDocument)
        {
            XmlNode I_MSG_HDR = soapEnvelopeDocument.SelectSingleNode("//I_MSG_HDR");

            XmlNode SEND_DATE = I_MSG_HDR.SelectSingleNode("SEND_DATE");
            XmlNode SEND_TIME = I_MSG_HDR.SelectSingleNode("SEND_TIME");
            XmlNode MSGID = I_MSG_HDR.SelectSingleNode("MSGID");

            SEND_DATE.InnerText = DateTime.Now.ToString("dd.MM.yyyy");
            SEND_TIME.InnerText = DateTime.Now.ToString("HH:mm:ss");

            MSGID.InnerText = GetMessageId(nbrVm);
        }

        private string GetMessageId(NBR_APIVM nbrApivm)
        {
            try
            {
                // Get message Id from database
                DataTable dtMsgId = GetLastMsgId();

                string transactionCode = nbrApivm.TransactionType == "9.1" ? "R910" : "R920";
                string currentDate = DateTime.Now.ToString("ddMMyyyy");
                string currentId = dtMsgId.Rows[0]["MessageId"].ToString();

                string messageId = transactionCode + currentDate + currentId.PadLeft(8, '0');

                MessageIdVM messageIdVm = new MessageIdVM()
                {
                    MessageId = currentId,
                    PeriodId = nbrApivm.Period.ToString("MMyyyy"),
                    FullId = messageId
                };

                UpdateMsgId(messageIdVm);

                return messageId;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void OnComplete(object sender, AsyncCompletedEventArgs eventArgs)
        {

        }

        private void SetMainTag25_68(DataSet dsVAT9_1, XmlNode I_MAIN_FORM)
        {
            try
            {
                string value25 = GetNoteValue(dsVAT9_1, "25");
                XmlNode A_F_S5_PAYMENT_NOT_BANKING_AMT = I_MAIN_FORM.SelectSingleNode("A_F_S5_PAYMENT_NOT_BANKING_AMT");
                A_F_S5_PAYMENT_NOT_BANKING_AMT.InnerText = value25;


                //string value26 = GetNoteValue(dsVAT9_1, "26");
                //XmlNode A_F_S5_ISSUANCE_DEBIT_AMT = I_MAIN_FORM.SelectSingleNode("A_F_S5_ISSUANCE_DEBIT_AMT");
                //A_F_S5_ISSUANCE_DEBIT_AMT.InnerText = value26;

                //string value27 = GetNoteValue(dsVAT9_1, "27");
                //XmlNode A_F_S5_OTHER_ADJUST_AMT = I_MAIN_FORM.SelectSingleNode("A_F_S5_OTHER_ADJUST_AMT");
                //A_F_S5_OTHER_ADJUST_AMT.InnerText = value27;


                //string value31 = GetNoteValue(dsVAT9_1, "31");
                //XmlNode A_F_S6_ISSUANCE_CREDIT_AMT = I_MAIN_FORM.SelectSingleNode("A_F_S6_ISSUANCE_CREDIT_AMT");
                //A_F_S6_ISSUANCE_CREDIT_AMT.InnerText = value31;

                //string value32 = GetNoteValue(dsVAT9_1, "32");
                //XmlNode A_F_S6_OTHER_ADJUST_AMT = I_MAIN_FORM.SelectSingleNode("A_F_S6_OTHER_ADJUST_AMT");
                //A_F_S6_OTHER_ADJUST_AMT.InnerText = value32;


                //string value38 = GetNoteValue(dsVAT9_1, "38");
                //XmlNode A_F_S7_SD_AGAINST_DEBIT_NOTE = I_MAIN_FORM.SelectSingleNode("A_F_S7_SD_AGAINST_DEBIT_NOTE");
                //A_F_S7_SD_AGAINST_DEBIT_NOTE.InnerText = value38;


                //string value39 = GetNoteValue(dsVAT9_1, "39");
                //XmlNode A_F_S7_SD_AGAINST_CREDIT_NOTE = I_MAIN_FORM.SelectSingleNode("A_F_S7_SD_AGAINST_CREDIT_NOTE");
                //A_F_S7_SD_AGAINST_CREDIT_NOTE.InnerText = value39;


                string value40 = GetNoteValue(dsVAT9_1, "40");
                XmlNode A_F_S7_SD_AGAINST_EXPORT = I_MAIN_FORM.SelectSingleNode("A_F_S7_SD_AGAINST_EXPORT");
                A_F_S7_SD_AGAINST_EXPORT.InnerText = value40;


                string value41 = GetNoteValue(dsVAT9_1, "41");
                XmlNode A_F_S7_INTEREST_OVERDUE_VAT = I_MAIN_FORM.SelectSingleNode("A_F_S7_INTEREST_OVERDUE_VAT"); // need concern
                //A_F_S7_INTEREST_OVERDUE_VAT.InnerText = value41;


                string value42 = GetNoteValue(dsVAT9_1, "42");
                XmlNode A_F_S7_INTEREST_OVERDUE_SD = I_MAIN_FORM.SelectSingleNode("A_F_S7_INTEREST_OVERDUE_SD"); // need concern
                //A_F_S7_INTEREST_OVERDUE_VAT.InnerText = value42;


                string value43 = GetNoteValue(dsVAT9_1, "43");
                XmlNode A_F_S7_FINE_PENALTY = I_MAIN_FORM.SelectSingleNode("A_F_S7_FINE_PENALTY"); // need concern
                //A_F_S7_FINE_PENALTY.InnerText = value43;


                string value44 = GetNoteValue(dsVAT9_1, "44");
                XmlNode A_F_S7_FINE_PENALTY_OTHER = I_MAIN_FORM.SelectSingleNode("A_F_S7_FINE_PENALTY_OTHER");
                A_F_S7_FINE_PENALTY_OTHER.InnerText = value44;


                string value45 = GetNoteValue(dsVAT9_1, "45");
                XmlNode A_F_S7_EXCISE_DUTY = I_MAIN_FORM.SelectSingleNode("A_F_S7_EXCISE_DUTY");
                A_F_S7_EXCISE_DUTY.InnerText = value45;


                string value46 = GetNoteValue(dsVAT9_1, "46");
                XmlNode A_F_S7_DEVELOP_SURCHARGE = I_MAIN_FORM.SelectSingleNode("A_F_S7_DEVELOP_SURCHARGE");
                A_F_S7_DEVELOP_SURCHARGE.InnerText = value46;


                string value47 = GetNoteValue(dsVAT9_1, "47");
                XmlNode A_F_S7_ICT_DEVELOP_SURCHARGE = I_MAIN_FORM.SelectSingleNode("A_F_S7_ICT_DEVELOP_SURCHARGE");
                A_F_S7_ICT_DEVELOP_SURCHARGE.InnerText = value47;


                string value48 = GetNoteValue(dsVAT9_1, "48");
                XmlNode A_F_S7_HEALTH_CARE_SURCHARGE = I_MAIN_FORM.SelectSingleNode("A_F_S7_HEALTH_CARE_SURCHARGE");
                A_F_S7_HEALTH_CARE_SURCHARGE.InnerText = value48;


                string value49 = GetNoteValue(dsVAT9_1, "49");
                XmlNode A_F_S7_ENV_PROTECT_SURCHARGE = I_MAIN_FORM.SelectSingleNode("A_F_S7_ENV_PROTECT_SURCHARGE");
                A_F_S7_ENV_PROTECT_SURCHARGE.InnerText = value49;


                string value52 = GetNoteValue(dsVAT9_1, "52");
                XmlNode A_F_S7_CB_LAST_TP_VAT = I_MAIN_FORM.SelectSingleNode("A_F_S7_CB_LAST_TP_VAT");
                //A_F_S7_CB_LAST_TP_VAT.InnerText = value52;


                string value53 = GetNoteValue(dsVAT9_1, "53");
                XmlNode A_F_S7_CB_LAST_TP_SD = I_MAIN_FORM.SelectSingleNode("A_F_S7_CB_LAST_TP_SD");
                //A_F_S7_CB_LAST_TP_SD.InnerText = value53;


                string value67 = GetNoteValue(dsVAT9_1, "67");
                XmlNode A_F_S11_REFUND_VAT = I_MAIN_FORM.SelectSingleNode("A_F_S11_REFUND_VAT");
                A_F_S11_REFUND_VAT.InnerText = value67;


                string value68 = GetNoteValue(dsVAT9_1, "68");
                XmlNode A_F_S11_REFUND_SD = I_MAIN_FORM.SelectSingleNode("A_F_S11_REFUND_SD");
                A_F_S11_REFUND_SD.InnerText = value68;


                // unkown Note
                string value0 = (Convert.ToDecimal(value67) + Convert.ToDecimal(value68)) > 0 ? "1" : "2";  // "NO";//GetNoteValue(dsVAT9_1, ""); // to be dynamic
                XmlNode A_F_S10_REFUND = I_MAIN_FORM.SelectSingleNode("A_F_S10_REFUND"); // need concern
                A_F_S10_REFUND.InnerText = value0;
            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetInitalDoc", e.ToString());
                throw;
            }
        }

        private void SetReturnHeaders(XmlNode I_MAIN_FORM, NBR_APIVM nbrApivm)
        {
            try
            {
                CompanyprofileDAL companyprofile = new CompanyprofileDAL();
                CompanyProfileVM companyProfileVm = companyprofile.SelectAllList().FirstOrDefault();
                if (companyProfileVm == null) throw new ArgumentNullException("companyProfileVm");

                DataTable vatReternHeader = SelectAll_VATReturnHeader(new VATReturnVM() { BranchId = 0, PeriodName = nbrApivm.Period.ToString("MMMM-yyyy") });


                XmlNode A_F_S1_BIN = I_MAIN_FORM.SelectSingleNode("A_F_S1_BIN");
                A_F_S1_BIN.InnerText = companyProfileVm.BIN;//.Substring(0, 9);

                XmlNode periodKey = I_MAIN_FORM.SelectSingleNode("PERIOD_KEY");
                periodKey.InnerText = nbrApivm.Period.ToString("yyMM"); // will need to be dynamic

                XmlNode A_F_S2_TYPE_RETURN = I_MAIN_FORM.SelectSingleNode("A_F_S2_TYPE_RETURN");

                if (vatReternHeader.Rows[0]["MainOrginalReturn"].ToString() == "Y")
                    A_F_S2_TYPE_RETURN.InnerText = "1";
                else if (vatReternHeader.Rows[0]["LateReturn"].ToString() == "Y")
                    A_F_S2_TYPE_RETURN.InnerText = "4";
                else
                    A_F_S2_TYPE_RETURN.InnerText = "-";


                XmlNode A_F_S2_ANY_ACTIVITIES = I_MAIN_FORM.SelectSingleNode("A_F_S2_ANY_ACTIVITIES"); // will need to be dynamic
                A_F_S2_ANY_ACTIVITIES.InnerText = vatReternHeader.Rows[0]["NoActivites"] == "Y" ? "2" : "1";

                XmlNode A_F_S2_REASON_AMENDED = I_MAIN_FORM.SelectSingleNode("A_F_S2_REASON_AMENDED"); // will need to be dynamic
                A_F_S2_REASON_AMENDED.InnerText = "";


                //XmlNode A_F_S2_ECO_ACTIVITIES = I_MAIN_FORM.SelectSingleNode("A_F_S2_ECO_ACTIVITIES");
                //A_F_S2_ECO_ACTIVITIES.InnerText = vatReternHeader.Rows[0]["IsTraderVAT"] == "Y" ? "1" : "2";

                XmlNode A_F_S2_SUBMISSION_DAT = I_MAIN_FORM.SelectSingleNode("A_F_S2_SUBMISSION_DAT");
                A_F_S2_SUBMISSION_DAT.InnerText = DateTime.Now.ToString("dd/MM/yyyy"); // will need to be dynamic
            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetInitalDoc", e.ToString());
                throw;
            }
        }

        private void SetUserInfo(NBR_APIVM nbrVm, XmlNode I_MAIN_FORM)
        {
            try
            {
                UserInformationDAL userInformationDal = new UserInformationDAL();
                UserInformationVM user = userInformationDal.SelectAll(Convert.ToInt32(nbrVm.CurrentUserId)).FirstOrDefault();
                if (user == null) throw new ArgumentNullException("user");


                XmlNode A_F_S10_NAME = I_MAIN_FORM.SelectSingleNode("A_F_S10_NAME");
                A_F_S10_NAME.InnerText = user.FullName;


                XmlNode A_F_S10_DESIGNATION = I_MAIN_FORM.SelectSingleNode("A_F_S10_DESIGNATION");
                A_F_S10_DESIGNATION.InnerText = user.Designation;


                XmlNode A_F_S10_MOBILE = I_MAIN_FORM.SelectSingleNode("A_F_S10_MOBILE");
                A_F_S10_MOBILE.InnerText = user.Mobile;

                XmlNode A_F_S10_NID_PP = I_MAIN_FORM.SelectSingleNode("A_F_S10_NID_PP");
                A_F_S10_NID_PP.InnerText = user.NationalId;

                XmlNode A_F_S10_EMAIL = I_MAIN_FORM.SelectSingleNode("A_F_S10_EMAIL");
                A_F_S10_EMAIL.InnerText = user.Email;


                //XmlNode A_F_S10_CONFIRM = I_MAIN_FORM.SelectSingleNode("A_F_S10_CONFIRM");
                //A_F_S10_CONFIRM.InnerText = "X"; // need to be dynamic

                // Unknown tags

                //<A_F_S10_CONFIRM>X</A_F_S10_CONFIRM>
                //<EXTENSION_FIELD />
            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetSubFormData1_22", e.ToString());
                throw;
            }
        }

        private string GetNoteValue(DataSet dsVAT9_1, string NoteNo)
        {
            string value = "0";
            if (string.IsNullOrEmpty(NoteNo))
                return value;

            DataRow[] row = dsVAT9_1.Tables[0].Select("NoteNo = '" + NoteNo + "'");
            value = row[0]["LineA"].ToString();

            if (OrdinaryVATDesktop.IsNumeric(value))
            {
                value = Convert.ToDecimal(value).ToString("0.##");
            }


            return value;
        }

        private XmlNode GetNote1_22ItemXML(PrepareXmlVm xmlVm)
        {

            try
            {
                XmlNode ITEM = xmlVm.XmlDocument.CreateElement("ITEM");
                string HSCode = xmlVm.DataRow["ProductCode"].ToString();
                string noteNo = xmlVm.DataRow["NoteNo"].ToString();
                string[] BOE_Notes = new[] { "SL13", "SL15", "SL17", "SL23" };
                string[] VATInputNotes = GetManualNotes();
                NbrApi nbrApi = new NbrApi();


                XmlNode FIELD_ID = xmlVm.XmlDocument.CreateElement("FIELD_ID");
                string slNo = nbrApi.GetSLNo(noteNo);

                FIELD_ID.InnerText = slNo;
                ITEM.AppendChild(FIELD_ID);



                XmlNode ACT_SALE_VALUE = xmlVm.XmlDocument.CreateElement("ACT_SALE_VALUE");
                if (FIELD_ID.InnerText == "SL06" || FIELD_ID.InnerText == "SL18")
                {
                    ACT_SALE_VALUE.InnerText = FormatDecimalPlace(xmlVm.DataRow["TotalPrice"]);
                }

                ITEM.AppendChild(ACT_SALE_VALUE);



                XmlNode ASSESS_VALUE = xmlVm.XmlDocument.CreateElement("ASSESS_VALUE");
                ASSESS_VALUE.InnerText = FormatDecimalPlace(xmlVm.DataRow["AssessableValue"]);
                ITEM.AppendChild(ASSESS_VALUE);


                XmlNode AT = xmlVm.XmlDocument.CreateElement("AT");
                AT.InnerText = FormatDecimalPlace(xmlVm.DataRow["AT"]);
                ITEM.AppendChild(AT);

                XmlNode BOE_DATE = xmlVm.XmlDocument.CreateElement("BOE_DATE");
                BOE_DATE.InnerText = xmlVm.DataRow["Date"].ToDateString();
                ITEM.AppendChild(BOE_DATE);

                XmlNode BOE_ITM_NO = xmlVm.XmlDocument.CreateElement("BOE_ITM_NO");
                BOE_ITM_NO.InnerText = xmlVm.DataRow["BE_ItemNo"].ToString();
                ITEM.AppendChild(BOE_ITM_NO);

                XmlNode BOE_NO = xmlVm.XmlDocument.CreateElement("BOE_NO");
                BOE_NO.InnerText = xmlVm.DataRow["Invoice/B/E No"].ToString();
                ITEM.AppendChild(BOE_NO);

                XmlNode BOE_OFF_CODE = xmlVm.XmlDocument.CreateElement("BOE_OFF_CODE");
                BOE_OFF_CODE.InnerText = xmlVm.DataRow["OfficeCode"].ToString();
                ITEM.AppendChild(BOE_OFF_CODE);

                // 
                XmlNode BOE_TYPE = xmlVm.XmlDocument.CreateElement("BOE_TYPE");

                if (BOE_Notes.Contains(slNo, StringComparer.OrdinalIgnoreCase))
                {
                    BOE_TYPE.InnerText = xmlVm.DataRow["ProductType"].ToString() == "service" ? "2" : "3";
                }
                ITEM.AppendChild(BOE_TYPE);

                XmlNode CATEGORY = xmlVm.XmlDocument.CreateElement("CATEGORY");

                if (FIELD_ID.InnerText == "SL08")
                {
                    if (xmlVm.ProfileVm != null) CATEGORY.InnerText = xmlVm.ProfileVm.CompanyType;
                }

                ITEM.AppendChild(CATEGORY);

                XmlNode CPC_CODE = xmlVm.XmlDocument.CreateElement("CPC_CODE");
                CPC_CODE.InnerText = xmlVm.DataRow["CPC"].ToString();
                ITEM.AppendChild(CPC_CODE);


                XmlNode DESCRIPTION = xmlVm.XmlDocument.CreateElement("DESCRIPTION");
                DESCRIPTION.InnerText = xmlVm.DataRow["ProductDescription"].ToString();
                ITEM.AppendChild(DESCRIPTION);

                XmlNode GOOD_SERVICE = xmlVm.XmlDocument.CreateElement("GOOD_SERVICE");
                GOOD_SERVICE.InnerText = HSCode;
                ITEM.AppendChild(GOOD_SERVICE);

                XmlNode INVOICE_DATE = xmlVm.XmlDocument.CreateElement("INVOICE_DATE");
                XmlNode INVOICE_NO = xmlVm.XmlDocument.CreateElement("INVOICE_NO");

                if (FIELD_ID.InnerText == "SL06" || FIELD_ID.InnerText == "SL18")
                {
                    INVOICE_DATE.InnerText = xmlVm.DataRow["Date"].ToString();
                    INVOICE_NO.InnerText = xmlVm.DataRow["Invoice/B/E No"].ToString();
                }

                ITEM.AppendChild(INVOICE_DATE);
                ITEM.AppendChild(INVOICE_NO);

                Item apiItem = xmlVm.Items.SingleOrDefault(x => x.GOODS_SERVICE_CODE == HSCode && x.NoteNo == noteNo);
                if (apiItem != null)
                {
                    string itemId = apiItem.ITEM_ID;

                    XmlNode ITEM_ID = xmlVm.XmlDocument.CreateElement("ITEM_ID");
                    ITEM_ID.InnerText = itemId;
                    ITEM.AppendChild(ITEM_ID);
                }


                XmlNode NAME = xmlVm.XmlDocument.CreateElement("NAME");
                //NAME.InnerText = xmlVm.DataRow["ProductName"].ToString();
                ITEM.AppendChild(NAME);


                XmlNode NOTES = xmlVm.XmlDocument.CreateElement("NOTES");
                NOTES.InnerText = xmlVm.DataRow["Remarks"].ToString();
                ITEM.AppendChild(NOTES);

                XmlNode QUANTITY = xmlVm.XmlDocument.CreateElement("QUANTITY");

                if (FIELD_ID.InnerText == "SL06" || FIELD_ID.InnerText == "SL18")
                {
                    QUANTITY.InnerText = FormatDecimalPlace(xmlVm.DataRow["Quantity"]);
                }
                ITEM.AppendChild(QUANTITY);


                XmlNode SD = xmlVm.XmlDocument.CreateElement("SD");
                if (VATInputNotes.Contains(slNo))
                {
                    SD.InnerText = FormatDecimalPlace(xmlVm.DataRow["SDAmount"]);
                }
                ITEM.AppendChild(SD);


                XmlNode UNIT = xmlVm.XmlDocument.CreateElement("UNIT");
                ITEM.AppendChild(UNIT);

                XmlNode VALUE = xmlVm.XmlDocument.CreateElement("VALUE");
                VALUE.InnerText = FormatDecimalPlace(xmlVm.DataRow["TotalPrice"]);
                ITEM.AppendChild(VALUE);


                XmlNode VAT = xmlVm.XmlDocument.CreateElement("VAT");

                if (VATInputNotes.Contains(slNo))
                {
                    VAT.InnerText = FormatDecimalPlace(xmlVm.DataRow["VATAmount"]);
                }

                ITEM.AppendChild(VAT);

                return ITEM;
            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetSubFormData1_22", e.ToString());

                throw;
            }
        }

        private string[] GetManualNotes()
        {
            return new[] { "SL01", "SL02", "SL11", "SL13", "SL15", "SL17", "SL23" };
        }

        private string FormatDecimalPlace(object value)
        {
            try
            {
                return Convert.ToDecimal(value).ToString("0.##");
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private XmlNode GetNote24_29ItemXML(XmlDocument soapEnvelopeDocument, DataRow dataRow)
        {
            try
            {
                XmlNode ITEM = soapEnvelopeDocument.CreateElement("ITEM");

                XmlNode FIELD_ID = soapEnvelopeDocument.CreateElement("FIELD_ID");
                FIELD_ID.InnerText = _nbrApi.GetSLNo(dataRow["NoteNo"].ToString());
                ITEM.AppendChild(FIELD_ID);
                XmlNode BIN = soapEnvelopeDocument.CreateElement("BIN");

                string note24 = _nbrApi.GetSLNo("24");
                string note29 = _nbrApi.GetSLNo("29");

                if (FIELD_ID.InnerText == note24)
                {
                    BIN.InnerText = dataRow["VendorBIN"].ToString();

                }
                else if (FIELD_ID.InnerText == note29)
                {
                    BIN.InnerText = dataRow["CustomerBIN"].ToString();
                }

                ITEM.AppendChild(BIN);


                XmlNode NAME = soapEnvelopeDocument.CreateElement("NAME");
                if (FIELD_ID.InnerText == note24)
                {
                    NAME.InnerText = dataRow["VendorName"].ToString();

                }
                else if (FIELD_ID.InnerText == note29)
                {
                    NAME.InnerText = dataRow["CustomerName"].ToString();
                }
                ITEM.AppendChild(NAME);


                XmlNode ADDRESS = soapEnvelopeDocument.CreateElement("ADDRESS");

                if (FIELD_ID.InnerText == note24)
                {
                    ADDRESS.InnerText = dataRow["VendorAddress"].ToString();
                }
                else if (FIELD_ID.InnerText == note29)
                {
                    ADDRESS.InnerText = dataRow["CustomerAddress"].ToString();
                }

                ITEM.AppendChild(ADDRESS);


                XmlNode VALUE = soapEnvelopeDocument.CreateElement("VALUE");
                VALUE.InnerText = FormatDecimalPlace(dataRow["TotalPrice"]);
                ITEM.AppendChild(VALUE);


                XmlNode DEDUCT_VAT = soapEnvelopeDocument.CreateElement("DEDUCT_VAT");
                DEDUCT_VAT.InnerText = FormatDecimalPlace(dataRow["VDSAmount"]);
                ITEM.AppendChild(DEDUCT_VAT);


                XmlNode INVOICE_NO = soapEnvelopeDocument.CreateElement("INVOICE_NO");
                INVOICE_NO.InnerText = dataRow["InvoiceNo"].ToString();
                ITEM.AppendChild(INVOICE_NO);


                XmlNode INVOICE_DATE = soapEnvelopeDocument.CreateElement("INVOICE_DATE");
                INVOICE_DATE.InnerText = dataRow["InvoiceDate"].ToDateString();
                ITEM.AppendChild(INVOICE_DATE);


                XmlNode CERT_NO = soapEnvelopeDocument.CreateElement("CERT_NO");
                CERT_NO.InnerText = dataRow["VDSCertificateNo"].ToString();
                ITEM.AppendChild(CERT_NO);


                XmlNode CERT_DATE = soapEnvelopeDocument.CreateElement("CERT_DATE");
                CERT_DATE.InnerText = dataRow["VDSCertificateDate"].ToDateString();
                ITEM.AppendChild(CERT_DATE);


                XmlNode ACCOUNT_CODE = soapEnvelopeDocument.CreateElement("ACCOUNT_CODE");
                ACCOUNT_CODE.InnerText = dataRow["AccountCode"].ToString();
                ITEM.AppendChild(ACCOUNT_CODE);

                XmlNode DEPOSIT_NO = soapEnvelopeDocument.CreateElement("DEPOSIT_NO");

                if (FIELD_ID.InnerText == note24)
                {
                    DEPOSIT_NO.InnerText = dataRow["TaxDepositSerialNo"].ToString();

                }
                else if (FIELD_ID.InnerText == note29)
                {
                    DEPOSIT_NO.InnerText = dataRow["SerialNo"].ToString();

                }
                ITEM.AppendChild(DEPOSIT_NO);


                XmlNode DEPOSIT_DATE = soapEnvelopeDocument.CreateElement("DEPOSIT_DATE");
                DEPOSIT_DATE.InnerText = dataRow["TaxDepositDate"].ToDateString();
                ITEM.AppendChild(DEPOSIT_DATE);


                XmlNode NOTE = soapEnvelopeDocument.CreateElement("NOTE");
                NOTE.InnerText = dataRow["Remarks"].ToString();
                ITEM.AppendChild(NOTE);


                return ITEM;
            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetSubFormData1_22", e.ToString());

                throw;
            }
        }

        private XmlNode GetNote30ItemXML(XmlDocument soapEnvelopeDocument, DataRow dataRow, List<Item> houses)
        {
            try
            {
                XmlNode ITEM = soapEnvelopeDocument.CreateElement("ITEM");


                XmlNode FIELD_ID = soapEnvelopeDocument.CreateElement("FIELD_ID");
                FIELD_ID.InnerText = _nbrApi.GetSLNo(dataRow["NoteNo"].ToString());
                ITEM.AppendChild(FIELD_ID);

                XmlNode BOE = soapEnvelopeDocument.CreateElement("BOE");
                BOE.InnerText = dataRow["BENumber"].ToString();
                ITEM.AppendChild(BOE);

                XmlNode BOE_DATE = soapEnvelopeDocument.CreateElement("BOE_DATE");
                BOE_DATE.InnerText = dataRow["Date"].ToDateString();
                ITEM.AppendChild(BOE_DATE);


                string cushouse = dataRow["CustomHouse"].ToString();
                Item apiCushouse = houses.SingleOrDefault(x => x.CUSTOM_NAME == cushouse);

                if (apiCushouse == null)
                    throw new Exception("Custom House Not Found");

                XmlNode CUS_STATION = soapEnvelopeDocument.CreateElement("CUS_STATION");
                CUS_STATION.InnerText = apiCushouse.ITEM_ID;
                ITEM.AppendChild(CUS_STATION);



                XmlNode ATV_AMOUNT = soapEnvelopeDocument.CreateElement("ATV_AMOUNT");
                ATV_AMOUNT.InnerText = FormatDecimalPlace(dataRow["ATAMOUNT"]);
                ITEM.AppendChild(ATV_AMOUNT);

                XmlNode NOTES = soapEnvelopeDocument.CreateElement("NOTES");
                NOTES.InnerText = dataRow["Remarks"].ToString();
                ITEM.AppendChild(NOTES);


                return ITEM;
            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetSubFormData1_22", e.ToString());

                throw;
            }
        }

        private XmlNode GetNote53_64ItemXML(XmlDocument soapEnvelopeDocument, DataRow dataRow, List<Item> banks)
        {
            try
            {
                string bankName = dataRow["BankName"].ToString();
                string bankBranchName = dataRow["BankBranch"].ToString();


                XmlNode ITEM = soapEnvelopeDocument.CreateElement("ITEM");

                XmlNode FIELD_ID = soapEnvelopeDocument.CreateElement("FIELD_ID");
                FIELD_ID.InnerText = _nbrApi.GetSLNo(dataRow["NoteNo"].ToString());
                ITEM.AppendChild(FIELD_ID);

                XmlNode CHALL_NO = soapEnvelopeDocument.CreateElement("CHALL_NO");
                CHALL_NO.InnerText = dataRow["ChallanNumber"].ToString();
                ITEM.AppendChild(CHALL_NO);

                XmlNode CHALL_DATE = soapEnvelopeDocument.CreateElement("CHALL_DATE"); // dateformat
                CHALL_DATE.InnerText = Convert.ToDateTime(dataRow["Date"]).ToString("dd/MM/yyyy");
                ITEM.AppendChild(CHALL_DATE);

                XmlNode ACCOUNT_CODE = soapEnvelopeDocument.CreateElement("ACCOUNT_CODE"); // dateformat
                //ACCOUNT_CODE.InnerText = dataRow["AccountCode"].ToString();
                ITEM.AppendChild(ACCOUNT_CODE);

                XmlNode CHAN_AMT = soapEnvelopeDocument.CreateElement("CHAN_AMT");
                CHAN_AMT.InnerText = FormatDecimalPlace(dataRow["Amount"]);
                ITEM.AppendChild(CHAN_AMT);

                XmlNode NOTES = soapEnvelopeDocument.CreateElement("NOTES");
                NOTES.InnerText = dataRow["Remarks"].ToString();
                ITEM.AppendChild(NOTES);

                Item bank = banks.SingleOrDefault(x => x.BANKN == bankName && x.BRANNM == bankBranchName);

                if (bank == null)
                    throw new Exception("Bank Name Does Not Match with NBR");


                XmlNode BANCD = soapEnvelopeDocument.CreateElement("BANCD"); // API data
                BANCD.InnerText = bank.BANCD;// dataRow["Remarks"].ToString();
                ITEM.AppendChild(BANCD);

                XmlNode BANKL = soapEnvelopeDocument.CreateElement("BANKL"); // API data
                BANKL.InnerText = bank.BANKL;//dataRow["Remarks"].ToString();
                ITEM.AppendChild(BANKL);

                XmlNode A_NAMEOFBANK = soapEnvelopeDocument.CreateElement("A_NAMEOFBANK");
                A_NAMEOFBANK.InnerText = bank.BANKN;
                ITEM.AppendChild(A_NAMEOFBANK);

                XmlNode A_BRANCH = soapEnvelopeDocument.CreateElement("A_BRANCH");
                A_BRANCH.InnerText = bankBranchName;
                ITEM.AppendChild(A_BRANCH);

                return ITEM;
            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetSubFormData1_22", e.ToString());

                throw;
            }
        }

        private XmlNode GetNote26_31ItemXML(XmlDocument soapEnvelopeDocument, DataRow dataRow)
        {
            try
            {

                XmlNode ITEM = soapEnvelopeDocument.CreateElement("ITEM");

                XmlNode FIELD_ID = soapEnvelopeDocument.CreateElement("FIELD_ID");
                FIELD_ID.InnerText = _nbrApi.GetSLNo(dataRow["NoteNo"].ToString());
                ITEM.AppendChild(FIELD_ID);


                XmlNode NOTE_NO = soapEnvelopeDocument.CreateElement("NOTE_NO");
                if (!string.IsNullOrEmpty(dataRow["IssuedDate"].ToString()))
                {
                    NOTE_NO.InnerText = "Note " + dataRow["NoteNo"].ToString();

                }
                ITEM.AppendChild(NOTE_NO);

                XmlNode ISSUE_DAT = soapEnvelopeDocument.CreateElement("ISSUE_DAT");
                ISSUE_DAT.InnerText = dataRow["IssuedDate"].ToDateString("dd/MM/yyyy");
                ITEM.AppendChild(ISSUE_DAT);

                XmlNode CHALL_NO = soapEnvelopeDocument.CreateElement("CHALL_NO");
                CHALL_NO.InnerText = dataRow["TaxChallanNo"].ToString();
                ITEM.AppendChild(CHALL_NO);

                XmlNode CHALL_DAT = soapEnvelopeDocument.CreateElement("CHALL_DAT");
                CHALL_DAT.InnerText = dataRow["TaxChallanDate"].ToDateString("dd/MM/yyyy");
                ITEM.AppendChild(CHALL_DAT);

                XmlNode REASON = soapEnvelopeDocument.CreateElement("REASON");
                REASON.InnerText = dataRow["ReasonforIssuance"].ToString();
                ITEM.AppendChild(REASON);

                XmlNode VALUE = soapEnvelopeDocument.CreateElement("VALUE");
                VALUE.InnerText = dataRow["ValueinChallan"].ToDecimalString();
                ITEM.AppendChild(VALUE);

                XmlNode QUANTITY = soapEnvelopeDocument.CreateElement("QUANTITY");
                QUANTITY.InnerText = dataRow["QuantityinChallan"].ToDecimalString();
                ITEM.AppendChild(QUANTITY);

                XmlNode VAT = soapEnvelopeDocument.CreateElement("VAT");
                VAT.InnerText = dataRow["VATinChallan"].ToDecimalString();
                ITEM.AppendChild(VAT);

                XmlNode SD = soapEnvelopeDocument.CreateElement("SD");
                SD.InnerText = dataRow["SDinChallan"].ToDecimalString();
                ITEM.AppendChild(SD);


                XmlNode VALUE_ADJ = soapEnvelopeDocument.CreateElement("VALUE_ADJ");
                XmlNode QUANTITY_ADJ = soapEnvelopeDocument.CreateElement("QUANTITY_ADJ");
                XmlNode VAT_ADJ = soapEnvelopeDocument.CreateElement("VAT_ADJ");
                XmlNode SD_ADJ = soapEnvelopeDocument.CreateElement("SD_ADJ");


                if (FIELD_ID.InnerText == "SL131")
                {
                    VALUE_ADJ.InnerText = dataRow["ValueofDecreasingAdjustment"].ToDecimalString();
                    QUANTITY_ADJ.InnerText = dataRow["QuantityofDecreasingAdjustment"].ToDecimalString();
                    VAT_ADJ.InnerText = dataRow["VATofDecreasingAdjustment"].ToDecimalString();
                    SD_ADJ.InnerText = dataRow["SDofDecreasingAdjustment"].ToDecimalString();
                }
                else
                {
                    VALUE_ADJ.InnerText = dataRow["ValueofIncreasingAdjustment"].ToDecimalString();
                    QUANTITY_ADJ.InnerText = dataRow["QuantityofIncreasingAdjustment"].ToDecimalString();
                    VAT_ADJ.InnerText = dataRow["VATofIncreasingAdjustment"].ToDecimalString();
                    SD_ADJ.InnerText = dataRow["SDofIncreasingAdjustment"].ToDecimalString();
                }

                ITEM.AppendChild(VALUE_ADJ);
                ITEM.AppendChild(QUANTITY_ADJ);
                ITEM.AppendChild(VAT_ADJ);
                ITEM.AppendChild(SD_ADJ);



                XmlNode NOTE = soapEnvelopeDocument.CreateElement("NOTES");
                NOTE.InnerText = dataRow["Remarks"].ToString();
                ITEM.AppendChild(NOTE);

                return ITEM;
            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetSubFormData26_31", e.ToString());

                throw;
            }
        }

        private XmlNode GetNote27_32ItemXML(XmlDocument soapEnvelopeDocument, DataRow dataRow)
        {
            try
            {

                XmlNode ITEM = soapEnvelopeDocument.CreateElement("ITEM");

                XmlNode FIELD_ID = soapEnvelopeDocument.CreateElement("FIELD_ID");
                FIELD_ID.InnerText = _nbrApi.GetSLNo(dataRow["NoteNo"].ToString());
                ITEM.AppendChild(FIELD_ID);

                XmlNode CHALL_NO = soapEnvelopeDocument.CreateElement("CHALL_NO");
                CHALL_NO.InnerText = dataRow["ChallanNumber"].ToString();
                ITEM.AppendChild(CHALL_NO);


                XmlNode CHALL_DATE = soapEnvelopeDocument.CreateElement("CHALL_DATE");
                CHALL_DATE.InnerText = dataRow["Date"].ToDateString();
                ITEM.AppendChild(CHALL_DATE);

                XmlNode AMOUNT = soapEnvelopeDocument.CreateElement("AMOUNT");
                AMOUNT.InnerText = dataRow["Amount"].ToDecimalString();
                ITEM.AppendChild(AMOUNT);

                XmlNode VAT = soapEnvelopeDocument.CreateElement("VAT");
                VAT.InnerText = dataRow["VAT"].ToDecimalString();
                ITEM.AppendChild(VAT);

                XmlNode NOTES = soapEnvelopeDocument.CreateElement("NOTES");
                NOTES.InnerText = dataRow["Notes"].ToString();
                ITEM.AppendChild(NOTES);

                return ITEM;
            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetSubFormData27_32", e.ToString());

                throw;
            }
        }

        private XmlNode GetNoteVDSItemXML(XmlDocument soapEnvelopeDocument, DataRow dataRow)
        {
            try
            {
                XmlNode ITEM = soapEnvelopeDocument.CreateElement("ITEM");


                XmlNode FIELD_ID = soapEnvelopeDocument.CreateElement("FIELD_ID");
                //FIELD_ID.InnerText = "SL01";
                FIELD_ID.InnerText = _nbrApi.GetSLNo(dataRow["NoteNo"].ToString());
                ITEM.AppendChild(FIELD_ID);



                XmlNode GOOD_SERVICE = soapEnvelopeDocument.CreateElement("GOOD_SERVICE");
                GOOD_SERVICE.InnerText = dataRow["ProductCode"].ToString();
                ITEM.AppendChild(GOOD_SERVICE);

                XmlNode VALUE = soapEnvelopeDocument.CreateElement("VALUE");
                VALUE.InnerText = FormatDecimalPlace(dataRow["TotalPrice"]);
                ITEM.AppendChild(VALUE);


                XmlNode SD = soapEnvelopeDocument.CreateElement("SD");
                SD.InnerText = FormatDecimalPlace(dataRow["SDAmount"]);
                ITEM.AppendChild(SD);


                XmlNode VAT = soapEnvelopeDocument.CreateElement("VAT");
                VAT.InnerText = FormatDecimalPlace(dataRow["VATAmount"]);
                ITEM.AppendChild(VAT);


                XmlNode DESCRIPTION = soapEnvelopeDocument.CreateElement("DESCRIPTION");
                DESCRIPTION.InnerText = dataRow["ProductDescription"].ToString();
                ITEM.AppendChild(DESCRIPTION);


                XmlNode NOTES = soapEnvelopeDocument.CreateElement("NOTES");
                NOTES.InnerText = dataRow["Remarks"].ToString();
                ITEM.AppendChild(NOTES);


                XmlNode QUANTITY = soapEnvelopeDocument.CreateElement("QUANTITY"); // unkonwn
                ITEM.AppendChild(QUANTITY);

                XmlNode ACT_SALE_VALUE = soapEnvelopeDocument.CreateElement("ACT_SALE_VALUE");
                ITEM.AppendChild(ACT_SALE_VALUE);

                XmlNode UNIT = soapEnvelopeDocument.CreateElement("UNIT"); // Unknown
                ITEM.AppendChild(UNIT);

                XmlNode ITEM_ID = soapEnvelopeDocument.CreateElement("ITEM_ID");
                ITEM_ID.InnerText = dataRow["ProductCode"].ToString();
                ITEM.AppendChild(ITEM_ID);


                XmlNode NAME = soapEnvelopeDocument.CreateElement("NAME");
                NAME.InnerText = dataRow["ProductName"].ToString();
                ITEM.AppendChild(ITEM_ID);


                XmlNode CATEGORY = soapEnvelopeDocument.CreateElement("CATEGORY");
                ITEM.AppendChild(CATEGORY);
                return ITEM;
            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetSubFormData1_22", e.ToString());

                throw;
            }
        }

        private DataTable GetSubFormData1_22(VATReturnSubFormVM subformVm)
        {
            try
            {
                int[] skip_array = new[] { 1, 2, 11, 13, 15, 17, 22 };
                int[] noteNos = Enumerable.Range(1, 22).ToArray().Where(x => !skip_array.Contains(x)).ToArray();

                DataTable subForms = new DataTable();
                DataTable dt = new DataTable();

                for (var index = 0; index < noteNos.Length; index++)
                {
                    int noteNo = noteNos[index];
                    subformVm.NoteNo = noteNo;
                    dt = VAT9_1_SubForm(subformVm);

                    subForms.Merge(dt);
                }

                return subForms;
            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetSubFormData1_22", e.ToString());

                throw;
            }
        }

        private DataTable GetSubFormData1_22_Import(VATReturnSubFormVM subformVm)
        {
            try
            {
                int[] skip_array = new[] { 1, 2, 11, 13, 15, 17, 22 };
                int[] noteNos = Enumerable.Range(1, 22).ToArray().Where(x => skip_array.Contains(x)).ToArray();

                DataTable subForms = new DataTable();
                string[] columns = new[]
                {
                    "UserName", "Branch", "NoteNo", "SubNoteNo", "Invoice/B/E No", "Date",
                    "OfficeCode", "CPC", "AssessableValue", "BasevalueofVAT", "VAT", "SD", "AT", "SubFormName",
                    "Remarks", "ProductName", "ProductDescription", "ProductCode", "BE_ItemNo", "DetailRemarks",
                    "ItemNo"
                };

                foreach (string column in columns)
                {
                    subForms.Columns.Add(column);
                }

                DataTable dt = new DataTable();

                for (var index = 0; index < noteNos.Length; index++)
                {
                    int noteNo = noteNos[index];
                    subformVm.NoteNo = noteNo;
                    dt = VAT9_1_SubForm(subformVm);

                    dt = dt.DefaultView.ToTable(false, "UserName", "Branch", "NoteNo", "SubNoteNo", "Invoice/B/E No", "Date",
                        "OfficeCode", "CPC", "AssessableValue", "BasevalueofVAT", "VAT", "SD", "AT", "SubFormName",
                        "Remarks", "ProductName", "ProductDescription", "ProductCode", "BE_ItemNo", "DetailRemarks",
                        "ItemNo");

                    dt = OrdinaryVATDesktop.DtColumnNameChange(dt, "Assessablevalue", "AssessableValue");

                    //string columnNames1 = dt.GetColumns();
                    //string columnNames2 = subForms.GetColumns();

                    subForms.Merge(dt, true, MissingSchemaAction.Ignore);
                }

                return subForms;
            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetSubFormData1_22", e.ToString());

                throw;
            }
        }

        private DataTable GetSubFormData(VATReturnSubFormVM subformVm)
        {
            try
            {
                int[] noteNos = subformVm.NoteNos.Select(x => x).ToArray();

                DataTable subForms = new DataTable();
                DataTable dt = new DataTable();

                foreach (int noteNo in noteNos)
                {
                    subformVm.NoteNo = noteNo;
                    dt = VAT9_1_SubForm(subformVm);

                    subForms.Merge(dt);
                }

                return subForms;
            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetSubFormData1_22", e.ToString());

                throw;
            }
        }

        private XmlDocument GetSoapEnvelope()
        {
            try
            {
                //  <A_F_S5_ISSUANCE_DEBIT_AMT></A_F_S5_ISSUANCE_DEBIT_AMT>
                //  <A_F_S5_OTHER_ADJUST_AMT></A_F_S5_OTHER_ADJUST_AMT>
                //    <A_F_S6_ISSUANCE_CREDIT_AMT></A_F_S6_ISSUANCE_CREDIT_AMT>
                //    <A_F_S6_OTHER_ADJUST_AMT></A_F_S6_OTHER_ADJUST_AMT>
                //             <A_F_S2_ECO_ACTIVITIES></A_F_S2_ECO_ACTIVITIES>
                string _9_1XML =
                    @"
<MT_RET_MSG_REQ>
         <I_MSG_HDR>
            <MSGID></MSGID>
            <SEND_DATE></SEND_DATE>
            <SEND_TIME></SEND_TIME>
         </I_MSG_HDR>
         <I_MAIN_FORM>
            <A_F_S1_BIN></A_F_S1_BIN>
            <PERIOD_KEY></PERIOD_KEY>
            <A_F_S2_TYPE_RETURN></A_F_S2_TYPE_RETURN>
            <A_F_S2_ANY_ACTIVITIES></A_F_S2_ANY_ACTIVITIES>
            <A_F_S2_SUBMISSION_DAT></A_F_S2_SUBMISSION_DAT>
            <A_F_S2_REASON_AMENDED></A_F_S2_REASON_AMENDED>
            <A_F_S5_PAYMENT_NOT_BANKING_AMT></A_F_S5_PAYMENT_NOT_BANKING_AMT>
 
            <A_F_S7_SD_AGAINST_EXPORT></A_F_S7_SD_AGAINST_EXPORT>
            <A_F_S7_FINE_PENALTY_OTHER></A_F_S7_FINE_PENALTY_OTHER>
            <A_F_S7_EXCISE_DUTY></A_F_S7_EXCISE_DUTY>
            <A_F_S7_DEVELOP_SURCHARGE></A_F_S7_DEVELOP_SURCHARGE>
            <A_F_S7_ICT_DEVELOP_SURCHARGE></A_F_S7_ICT_DEVELOP_SURCHARGE>
            <A_F_S7_HEALTH_CARE_SURCHARGE></A_F_S7_HEALTH_CARE_SURCHARGE>
            <A_F_S7_ENV_PROTECT_SURCHARGE></A_F_S7_ENV_PROTECT_SURCHARGE>
            <A_F_S7_CB_LAST_TP_VAT></A_F_S7_CB_LAST_TP_VAT>
            <A_F_S7_CB_LAST_TP_SD></A_F_S7_CB_LAST_TP_SD>
            <A_F_S11_REFUND_VAT></A_F_S11_REFUND_VAT>
            <A_F_S11_REFUND_SD></A_F_S11_REFUND_SD>
            <A_F_S10_REFUND></A_F_S10_REFUND>
            <A_F_S10_NAME></A_F_S10_NAME>
            <A_F_S10_DESIGNATION></A_F_S10_DESIGNATION>
            <A_F_S10_MOBILE></A_F_S10_MOBILE>
            <A_F_S10_NID_PP></A_F_S10_NID_PP>
            <A_F_S10_EMAIL></A_F_S10_EMAIL>
            <A_F_S7_INTEREST_OVERDUE_VAT></A_F_S7_INTEREST_OVERDUE_VAT>
            <A_F_S7_INTEREST_OVERDUE_SD></A_F_S7_INTEREST_OVERDUE_SD>
            <A_F_S7_FINE_PENALTY></A_F_S7_FINE_PENALTY>
         </I_MAIN_FORM>

       <IT_SUBF_GOSERV>
       </IT_SUBF_GOSERV>
        
        <IT_SUBF_VDS>
        </IT_SUBF_VDS>
    

        <IT_SUBF_CHALLAN></IT_SUBF_CHALLAN>

         <IT_SUBF_ADJUST>
         </IT_SUBF_ADJUST>
         <IT_SUBF_OTHER>
         </IT_SUBF_OTHER>

      </MT_RET_MSG_REQ>";


                //<IT_SUBF_ATTACH></IT_SUBF_ATTACH>
                //<ATTACH_DOCUMENT></ATTACH_DOCUMENT>
                //<IT_SUBF_BOE>
                //</IT_SUBF_BOE>
                //<A_F_S7_SD_AGAINST_DEBIT_NOTE></A_F_S7_SD_AGAINST_DEBIT_NOTE>
                //<A_F_S7_SD_AGAINST_CREDIT_NOTE>0</A_F_S7_SD_AGAINST_CREDIT_NOTE>
                //<A_F_S5_OTHER_ADJUST_CMT>-</A_F_S5_OTHER_ADJUST_CMT>
                //<A_F_S6_OTHER_ADJUST_CMT>-</A_F_S6_OTHER_ADJUST_CMT>
                //            <A_F_S10_CONFIRM>X</A_F_S10_CONFIRM>

                XmlDocument soapEnvelopeDocument = new XmlDocument();

                //using (XmlTextReader reader = new XmlTextReader(new StringReader(_9_1XML)))
                //{
                //    reader.Namespaces = false;
                //    soapEnvelopeDocument.Load(reader);
                //}
                soapEnvelopeDocument.LoadXml(_9_1XML);
                return soapEnvelopeDocument;

            }
            catch (Exception e)
            {
                FileLogger.Log("VATReturnDAL", "GetInitalDoc", e.ToString());

                throw;
            }
        }

        public DataTable GetLastMsgId(SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                SqlText = @"select (max(APIMsgId)+1)MessageId from FiscalYear";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                sqlDataAdapter.Fill(table);

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return table;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("9.1 DAL", "GetLastMsgId", e.ToString());
                throw;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public ResultVM UpdateMsgId(MessageIdVM msgVM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            ResultVM resultVm = new ResultVM();
            string SqlText = "";
            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion


                SqlText = @"update FiscalYear set APIMsgId = @msgId, APIMsgFullId = @APIMsgFullId
where PeriodID = @periodId";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);

                cmd.Parameters.AddWithValue("@msgId", msgVM.MessageId);
                cmd.Parameters.AddWithValue("@periodId", msgVM.PeriodId);
                cmd.Parameters.AddWithValue("@APIMsgFullId", msgVM.FullId);

                cmd.ExecuteNonQuery();

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return resultVm;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("9.1 DAL", "GetLastMsgId", e.ToString());
                throw;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public ResultVM SaveSubforms(VATReturnSubFormVM subformVm)
        {
            SqlConnection currConn = null;
            try
            {
                ResultVM resultVm = new ResultVM();
                CommonDAL commonDAl = new CommonDAL();

                subformVm.IsSummary = false;

                // vm.NoteNo == 11 || vm.NoteNo == 13 || vm.NoteNo == 15 || vm.NoteNo == 17 || vm.NoteNo == 22


                DataTable dtsubforms = GetSubFormData1_22(subformVm);
                dtsubforms.Columns.Add(new DataColumn() { ColumnName = "PeriodId", DefaultValue = subformVm.PeriodId });

                DataTable dtsubformsImport = GetSubFormData1_22_Import(subformVm);
                dtsubformsImport.Columns.Add(new DataColumn() { ColumnName = "PeriodId", DefaultValue = subformVm.PeriodId });


                string columnNames = dtsubformsImport.GetColumns();


                subformVm.IsSummary = true;

                subformVm.NoteNos = new[] { 24, 29 };
                DataTable dtVDSData = GetSubFormData(subformVm);
                dtVDSData.Columns.Add(new DataColumn() { ColumnName = "PeriodId", DefaultValue = subformVm.PeriodId });

                subformVm.NoteNos = new[] { 30 };
                DataTable dtNote30 = GetSubFormData(subformVm);
                dtNote30.Columns.Add(new DataColumn() { ColumnName = "PeriodId", DefaultValue = subformVm.PeriodId });

                subformVm.NoteNos = new[] { 58, 59, 60, 61, 62, 63, 64 };
                DataTable dt58_64 = GetSubFormData(subformVm);
                dt58_64.Columns.Add(new DataColumn() { ColumnName = "PeriodId", DefaultValue = subformVm.PeriodId });


                subformVm.NoteNos = new[] { 26, 31 };
                DataTable dt26_31 = GetSubFormData(subformVm);
                dt26_31.Columns.Add(new DataColumn() { ColumnName = "PeriodId", DefaultValue = subformVm.PeriodId });


                subformVm.NoteNos = new[] { 27, 32 };
                DataTable dt27_32 = GetSubFormData(subformVm);
                dt27_32.Columns.Add(new DataColumn() { ColumnName = "PeriodId", DefaultValue = subformVm.PeriodId });


                string columns = dtsubforms.GetColumns();

                //foreach (DataColumn dataColumn in dt27_32.Columns)
                //{
                //    columns += dataColumn.ColumnName + "\n";
                //}


                RemoveExistingData(subformVm);

                var result = SaveSubforms(dtsubforms, dtsubformsImport, dtVDSData, dtNote30, dt58_64, dt26_31, dt27_32);

                resultVm.Status = result[0];
                resultVm.Message = result[1];

                return resultVm;
            }
            catch (Exception e)
            {
                FileLogger.Log("9.1 DAL", "GetLastMsgId", e.ToString());
                throw;
            }
            finally
            {


            }
        }

        private string[] SaveSubforms(DataTable dtsubforms, DataTable dtSubformsImport, DataTable dtVDSData, DataTable dtNote30, DataTable dt58_64, DataTable dt26_31, DataTable dt27_32)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();
                #endregion open connection and transaction

                CommonDAL commonDAl = new CommonDAL();


                string[] result = commonDAl.BulkInsert("APINote1_22", dtsubforms, currConn, transaction);
                result = commonDAl.BulkInsert("APINote1_22", dtSubformsImport, currConn, transaction);

                result = commonDAl.BulkInsert("APINote24_29", dtVDSData, currConn, transaction);
                result = commonDAl.BulkInsert("APINote30", dtNote30, currConn, transaction);
                result = commonDAl.BulkInsert("APINote58_64", dt58_64, currConn, transaction);

                //string columnNames = dtsubforms.GetColumns();


                result = commonDAl.BulkInsert("APINote26_31", dt26_31, currConn, transaction);
                result = commonDAl.BulkInsert("APINote27_32", dt27_32, currConn, transaction);


                string sqlText = @"
--update APINote1_22 set VATRate = Products.VATRate,SDRate=Products.SD from Products
--where Products.ItemNo = APINote1_22.ItemNo

Update APINote1_22 set VATRate = SalesInvoiceDetails.VATRate, SDRate = SalesInvoiceDetails.SD
from SalesInvoiceDetails
where SalesInvoiceDetails.SalesInvoiceNo = APINote1_22.DetailRemarks 
and SalesInvoiceDetails.ItemNo = APINote1_22.ItemNo
and APINote1_22.PeriodId = SalesInvoiceDetails.PeriodID

Update APINote1_22 set VATRate = 0, SDRate = 0
from SalesInvoiceDetails
where SalesInvoiceDetails.SalesInvoiceNo = APINote1_22.DetailRemarks 
and SalesInvoiceDetails.ItemNo = APINote1_22.ItemNo
and APINote1_22.PeriodId = SalesInvoiceDetails.PeriodID
and SalesInvoiceDetails.Type = 'fixedvat'


Update APINote1_22 set VATRate = PurchaseInvoiceDetails.VATRate, APINote1_22.SDRate = PurchaseInvoiceDetails.SD
from PurchaseInvoiceDetails
where PurchaseInvoiceDetails.PurchaseInvoiceNo = APINote1_22.DetailRemarks 
and PurchaseInvoiceDetails.ItemNo = APINote1_22.ItemNo
and APINote1_22.PeriodId = PurchaseInvoiceDetails.PeriodID

Update APINote1_22 set APINote1_22.VATRate = 0, APINote1_22.SDRate = 0
from PurchaseInvoiceDetails
where PurchaseInvoiceDetails.PurchaseInvoiceNo = APINote1_22.DetailRemarks 
and PurchaseInvoiceDetails.ItemNo = APINote1_22.ItemNo
and APINote1_22.PeriodId = PurchaseInvoiceDetails.PeriodID
and PurchaseInvoiceDetails.Type = 'fixedvat'

Update APINote1_22 set 
VATRate = Client6_3Details.VATRate, 
APINote1_22.SDRate = Client6_3Details.SDRate
from Client6_3Details
where Client6_3Details.InvoiceNo = APINote1_22.DetailRemarks 
and Client6_3Details.ItemNo = APINote1_22.ItemNo
and APINote1_22.PeriodId = Client6_3Details.PeriodID


update APINote1_22 set ProductType = ProductCategories.IsRaw from 
APINote1_22 left outer join Products on APINote1_22.ItemNo = Products.ItemNo
left outer join ProductCategories on products.CategoryID = ProductCategories.CategoryID

";
                commonDAl.ExecuteUpdateQuery(sqlText, currConn, transaction, null, 200);

                transaction.Commit();

                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                throw;
            }
            finally
            {
                #region Close Connection

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #endregion
            }
        }

        private void RemoveExistingData(VATReturnSubFormVM subformVm)
        {
            SqlConnection currConn = null;

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Remove Existing Data

                string sqlText = " delete from APINote1_22 where PeriodId = @PeriodId";
                sqlText += " delete from APINote24_29 where PeriodId = @PeriodId";
                sqlText += " delete from APINote30 where PeriodId = @PeriodId";
                sqlText += " delete from APINote58_64 where PeriodId = @PeriodId";
                sqlText += " delete from APINote26_31 where PeriodId = @PeriodId";
                sqlText += " delete from APINote27_32 where PeriodId = @PeriodId";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@PeriodId", subformVm.PeriodId);
                cmd.ExecuteNonQuery();

                #endregion

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                #region Close Connection

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #endregion
            }
        }

        public DataTable GetAPIData(VATReturnSubFormVM subformVm)
        {
            SqlConnection currConn = null;
            DataTable table = new DataTable();
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Get Subform Data

                string sqlText = @"select * from @table where PeriodId = @PeriodId";

                sqlText = sqlText.Replace("@table", subformVm.TableName);

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@PeriodId", subformVm.PeriodId);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                return table;

                #endregion



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                #region Close Connection

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #endregion
            }
        }

        public DataTable GetAPI1_22Data(VATReturnSubFormVM subformVm)
        {
            SqlConnection currConn = null;
            DataTable table = new DataTable();
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Get Subform Data

                string sqlText = @"
select 
UserName,Branch,NoteNo,SubNoteNo,SubFormName,
Remarks,ItemNo,ProductDescription,ProductCode,ProductName,ProductCategory,ItemId,isnull(VATRate,0)VATRate,isnull(SDRate,0)SDRate
,sum(isnull(TotalPrice,BasevalueofVAT))TotalPrice
,sum(isnull(VATAMount,VAT))VATAMount
,sum(isnull(SDAmount,SD))SDAmount
,sum(isnull(Quantity,0))Quantity 
,[Invoice/B/E No]
,Date,OfficeCode
,CPC
,sum(AssessableValue)AssessableValue
,sum(BasevalueofVAT)BasevalueofVAT
,sum(VAT)VAT
,sum(SD)SD
,sum(AT)AT
,BE_ItemNo
,ProductType
from APINote1_22

where PeriodId = @PeriodId

group by 
UserName,Branch,NoteNo,SubNoteNo,SubFormName,
Remarks,ItemNo,ProductDescription
,ProductCode,ProductName,ProductCategory,ItemId,VATRate,SDRate
,[Invoice/B/E No]
,Date,OfficeCode
,CPC

,BE_ItemNo
,ProductType ";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@PeriodId", subformVm.PeriodId);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                return table;

                #endregion

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                #region Close Connection

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #endregion
            }
        }

        public ResultVM UpdateItemId(DataTable data)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataTable table = new DataTable();
            CommonDAL commonDal = new CommonDAL();
            ResultVM resultVm = new ResultVM();
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                #region Insert to Temp table

                DataTable finalTable = data.DefaultView.ToTable(false, "GOODS_SERVICE_CODE", "NoteNo", "ITEM_ID");


                string sqlText = @"create table #ItemIds( 
GOODS_SERVICE_CODE varchar(255),
NoteNo varchar(255),
ITEM_ID varchar(255)
) ";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.ExecuteNonQuery();

                string[] result = commonDal.BulkInsert("#ItemIds", finalTable, currConn, transaction);

                #endregion

                #region Update Item Id

                string updateItemId = @"update APINote1_22 set ItemId = #ItemIds.ITEM_ID
from #ItemIds
where #ItemIds.NoteNo = APINote1_22.NoteNo and APINote1_22.ProductCode = #ItemIds.GOODS_SERVICE_CODE

drop table #ItemIds";

                cmd.CommandText = updateItemId;
                cmd.ExecuteNonQuery();
                #endregion


                transaction.Commit();


                resultVm.Message = "success";
                resultVm.Status = "success";

                return resultVm;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
            finally
            {
                #region Close Connection

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                #endregion
            }
        }

        #endregion

        #region Sub form save

        public string[] SaveSubFormData(VATReturnSubFormVM vm, DataTable dt, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            int nextId = 0;

            CommonDAL commonDAl = new CommonDAL();

            #endregion

            #region try

            try
            {

                #region open connection and transaction

                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Get Period Id

                string[] vValues = { vm.PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues, currConn, transaction, connVM).FirstOrDefault();

                #endregion

                #region Delete

                sqlText = "delete " + vm.SubformTableName + " where  PeriodID = @PeriodId and NoteNo = @NoteNo";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Transaction = transaction;
                cmdDelete.Parameters.AddWithValue("@PeriodID", varFiscalYearVM.PeriodID);
                cmdDelete.Parameters.AddWithValue("@NoteNo", vm.NoteNo);
                transResult = (int)cmdDelete.ExecuteNonQuery();

                #endregion Delete

                #region Data Save

                if (dt != null && dt.Rows.Count != 0)
                {
                    var PeriodID = new DataColumn("PeriodID") { DefaultValue = varFiscalYearVM.PeriodID };
                    var BranchId = new DataColumn("BranchId") { DefaultValue = "0" };

                    if (!dt.Columns.Contains("PeriodID"))
                    {
                        dt.Columns.Add(PeriodID);
                    }
                    if (!dt.Columns.Contains("BranchId"))
                    {
                        dt.Columns.Add(BranchId);
                    }

                    if (dt.Columns.Contains("AssessableValue"))
                    {
                        dt.Columns["AssessableValue"].ColumnName = "Assessablevalue";
                    }

                    #region bulk insert

                    retResults = commonDAl.BulkInsert(vm.SubformTableName, dt, currConn, transaction);

                    #endregion

                }

                #endregion

                #region Commit

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region catch & Finally

            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1_VATReturnDAL", "InsertToSubForm", ex.ToString() + "\n" + sqlText);

                return retResults;
            }
            #endregion

            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return retResults;
        }

        public string[] checkVAT9_1SubFormMethod(VATReturnSubFormVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            DataTable Dt = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            int nextId = 0;
            int ID = 0;

            #endregion

            #region try

            try
            {

                #region open connection and transaction

                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Get Period Id

                string[] vValues = { vm.PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues, currConn, transaction, connVM).FirstOrDefault();

                #endregion

                #region Exits check

                sqlText = "select count(Id) from VAT9_1SubFormCheck where PeriodID=@PeriodID";
                SqlCommand cmdExits = new SqlCommand(sqlText, currConn);
                cmdExits.Transaction = transaction;
                cmdExits.Parameters.AddWithValue("@PeriodID", varFiscalYearVM.PeriodID);
                int isExits = (int)cmdExits.ExecuteScalar();

                #endregion

                #region insert

                if (isExits == 0)
                {

                    sqlText = "insert into VAT9_1SubFormCheck (PeriodID)values(@PeriodID)";
                    SqlCommand cmd = new SqlCommand(sqlText, currConn);
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValue("@PeriodID", varFiscalYearVM.PeriodID);
                    transResult = (int)cmd.ExecuteNonQuery();

                }

                retResults[0] = "Success";

                #endregion insert

                #region Commit

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion

            #region catch & Finally

            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1_VATReturnDAL", "InsertToSubForm", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
            }
            #endregion

            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return retResults;
        }


        #endregion

        public DataTable GetDataVAT9_1_SubFormTable(VATReturnSubFormVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, bool IsSaveSubForm = false)
        {
            DataTable dt = new DataTable();

            #region try

            try
            {

                #region SubFormName

                if ((vm.NoteNo >= 1 && vm.NoteNo <= 2) || vm.NoteNo == 11 || vm.NoteNo == 13 || vm.NoteNo == 15 || vm.NoteNo == 17 || vm.NoteNo == 22)
                {
                    dt = GetDataVAT9_1_SubFormB(vm, VcurrConn, Vtransaction, connVM);
                }
                else if ((vm.NoteNo >= 3 && vm.NoteNo <= 5) || vm.NoteNo == 7)
                {
                    dt = GetDataVAT9_1_SubFormA(vm, VcurrConn, Vtransaction, connVM);
                }
                else if ((vm.NoteNo == 10 || vm.NoteNo == 12 || vm.NoteNo == 14 || vm.NoteNo == 16) || (vm.NoteNo >= 19 && vm.NoteNo <= 23 && vm.NoteNo != 22))
                {
                    dt = GetDataVAT9_1_SubFormA(vm, VcurrConn, Vtransaction, connVM);
                }
                else if (vm.NoteNo == 6 || vm.NoteNo == 18)
                {
                    dt = GetDataVAT9_1_SubFormC(vm, VcurrConn, Vtransaction, connVM);
                }
                else if (vm.NoteNo == 8)
                {
                    dt = GetDataVAT9_1_SubFormD(vm, VcurrConn, Vtransaction, connVM);
                }
                else if (vm.NoteNo == 24)
                {
                    dt = GetDataVAT9_1_SubFormE(vm, VcurrConn, Vtransaction, connVM);
                }
                else if (vm.NoteNo == 26)//debitNote
                {
                    dt = GetDataVAT9_1_SubFormF(vm, VcurrConn, Vtransaction, connVM);
                }
                else if (vm.NoteNo == 27 || vm.NoteNo == 32)//Increasing Adjustment And Decreasing Adjustment
                {
                    dt = GetDataVAT9_1_SubFormG(vm, VcurrConn, Vtransaction, connVM);
                }
                else if (vm.NoteNo == 29)
                {
                    dt = GetDataVAT9_1_SubFormH(vm, VcurrConn, Vtransaction, connVM);
                }
                else if (vm.NoteNo == 30)
                {
                    dt = GetDataVAT9_1_SubFormI(vm, VcurrConn, Vtransaction, connVM);
                }
                else if (vm.NoteNo == 31) //CreditNo
                {
                    dt = GetDataVAT9_1_SubFormJ(vm, VcurrConn, Vtransaction, connVM);
                }
                else if (vm.NoteNo >= 58 && vm.NoteNo <= 64)
                {
                    dt = GetDataVAT9_1_SubFormK(vm, VcurrConn, Vtransaction, connVM);
                }

                #endregion

            }
            #endregion

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "GetDataVAT9_1_SubFormTable", ex.ToString());

                throw;
            }
            finally { }

            return dt;
        }

        public DataTable GetDataVAT9_1_SubFormA(VATReturnSubFormVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";

                #region sqlText

                sqlText = @" 
SELECT 
 UserName
,Branch
,NoteNo
,SubNoteNo
,TotalPrice
,VATAmount
,SDAmount
,SubFormName
,Remarks
,ItemNo
,ProductDescription
,ProductName
,ProductCode
,DetailRemarks
,VATRate
,SDRate
FROM   VAT9_1SubFormA
where 1=1 and PeriodID=@PeriodID and NoteNo=@NoteNo
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", vm.PeriodId);
                objCommVAT19.Parameters.AddWithValue("@NoteNo", vm.NoteNo);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "GetDataVAT9_1_SubFormA", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public DataTable GetDataVAT9_1_SubFormB(VATReturnSubFormVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";

                #region sqlText

                sqlText = @" 
SELECT
 UserName
,Branch
,NoteNo
,SubNoteNo
,[Invoice/B/E No]
,Date
,OfficeCode
,BE_ItemNo
,CPC
,Assessablevalue
,BasevalueofVAT
,VAT
,SD
,AT
,SubFormName
,Remarks
,ItemNo
,ProductDescription
,ProductName
,ProductCode
,DetailRemarks
,VATRate
,SDRate
FROM   VAT9_1SubFormB
where 1=1 and PeriodID=@PeriodID and NoteNo=@NoteNo
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", vm.PeriodId);
                objCommVAT19.Parameters.AddWithValue("@NoteNo", vm.NoteNo);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "GetDataVAT9_1_SubFormB", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public DataTable GetDataVAT9_1_SubFormC(VATReturnSubFormVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";

                #region sqlText

                sqlText = @" 
SELECT 
 UserName
,Branch
,NoteNo
,SubNoteNo
,TotalPrice
,VATAmount
,SDAmount
,SubFormName
,Remarks
,ItemNo
,ProductDescription
,ProductName
,ProductCode
,DetailRemarks
,VATRate
,SDRate
FROM VAT9_1SubFormC
where 1=1 and PeriodID=@PeriodID and NoteNo=@NoteNo
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", vm.PeriodId);
                objCommVAT19.Parameters.AddWithValue("@NoteNo", vm.NoteNo);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "GetDataVAT9_1_SubFormC", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public DataTable GetDataVAT9_1_SubFormD(VATReturnSubFormVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";

                #region sqlText

                sqlText = @" 
SELECT 
 UserName
,Branch
,NoteNo
,SubNoteNo
,TotalPrice
,VATAmount
,SDAmount
,SubFormName
,Remarks
,ItemNo
,ProductDescription
,ProductName
,ProductCode
,DetailRemarks
,ProductCategory
,VATRate
,SDRate
FROM VAT9_1SubFormD
where 1=1 and PeriodID=@PeriodID and NoteNo=@NoteNo
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", vm.PeriodId);
                objCommVAT19.Parameters.AddWithValue("@NoteNo", vm.NoteNo);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "GetDataVAT9_1_SubFormD", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public DataTable GetDataVAT9_1_SubFormE(VATReturnSubFormVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";

                #region sqlText

                sqlText = @" 
SELECT UserName
      ,Branch
      ,NoteNo
      ,SubNoteNo
      ,BillDeductAmount
      ,VATAmount
      ,SDAmount
      ,SubFormName
      ,Remarks
      ,VendorName
      ,VendorBIN
      ,VendorAddress
      ,TotalPrice
      ,VDSAmount
      ,InvoiceNo
      ,InvoiceDate
      ,VDSCertificateNo
      ,VDSCertificateDate
      ,AccountCode
      ,TaxDepositSerialNo
      ,TaxDepositDate
      ,DetailRemarks

  FROM VAT9_1SubFormE
where 1=1 and PeriodID=@PeriodID and NoteNo=@NoteNo
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", vm.PeriodId);
                objCommVAT19.Parameters.AddWithValue("@NoteNo", vm.NoteNo);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "GetDataVAT9_1_SubFormE", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public DataTable GetDataVAT9_1_SubFormF(VATReturnSubFormVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";

                #region sqlText

                sqlText = @" 
SELECT UserName
      ,Branch
      ,NoteNo
      ,SubNoteNo
      ,DebitNoteNo
      ,IssuedDate
      ,TaxChallanNo
      ,TaxChallanDate
      ,ReasonforIssuance
      ,ValueinChallan
      ,QuantityinChallan
      ,VATinChallan
      ,SDinChallan
      ,ValueofIncreasingAdjustment
      ,QuantityofIncreasingAdjustment
      ,VATofIncreasingAdjustment
      ,SDofIncreasingAdjustment
      ,Remarks
      ,ItemNo
      ,ProductDescription
      ,ProductName
      ,SubFormName
      
  FROM VAT9_1SubFormF
where 1=1 and PeriodID=@PeriodID and NoteNo=@NoteNo
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", vm.PeriodId);
                objCommVAT19.Parameters.AddWithValue("@NoteNo", vm.NoteNo);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "GetDataVAT9_1_SubFormF", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public DataTable GetDataVAT9_1_SubFormG(VATReturnSubFormVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";

                #region sqlText

                sqlText = @" 
SELECT UserName
      ,Branch
      ,NoteNo
      ,SubNoteNo
      ,AdjType
      ,AdjName
      ,ChallanNumber
      ,Date
      ,Amount
      ,VAT
      ,SubFormName
      ,Remarks
      ,Notes
  FROM VAT9_1SubFormG
where 1=1 
and Amount!=0 
and PeriodID=@PeriodID and NoteNo=@NoteNo
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", vm.PeriodId);
                objCommVAT19.Parameters.AddWithValue("@NoteNo", vm.NoteNo);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "GetDataVAT9_1_SubFormG", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public DataTable GetDataVAT9_1_SubFormH(VATReturnSubFormVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";

                #region sqlText

                sqlText = @" 
SELECT UserName
      ,Branch
      ,NoteNo
      ,SubNoteNo
      ,BillDeductAmount
      ,VATAmount
      ,SDAmount
      ,SubFormName
      ,Remarks
      ,CustomerName
      ,CustomerBIN
      ,CustomerAddress
      ,TotalPrice
      ,VDSAmount
      ,PurchaseNumber
      ,InvoiceNo
      ,InvoiceDate
      ,VDSCertificateNo
      ,VDSCertificateDate
      ,AccountCode
      ,SerialNo
      ,TaxDepositDate
      ,DetailRemarks

  FROM VAT9_1SubFormH
where 1=1 and PeriodID=@PeriodID and NoteNo=@NoteNo
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", vm.PeriodId);
                objCommVAT19.Parameters.AddWithValue("@NoteNo", vm.NoteNo);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "GetDataVAT9_1_SubFormH", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public DataTable GetDataVAT9_1_SubFormI(VATReturnSubFormVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";

                #region sqlText

                sqlText = @" 
SELECT UserName
      ,Branch
      ,NoteNo
      ,SubNoteNo
      ,ATVAmount
      ,Column1
      ,Column2
      ,SubFormName
      ,Remarks
      ,Date
      ,CustomHouse
      ,BENumber
      ,DetailRemarks

  FROM VAT9_1SubFormI
where 1=1 and PeriodID=@PeriodID and NoteNo=@NoteNo
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", vm.PeriodId);
                objCommVAT19.Parameters.AddWithValue("@NoteNo", vm.NoteNo);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "GetDataVAT9_1_SubFormI", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public DataTable GetDataVAT9_1_SubFormJ(VATReturnSubFormVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";

                #region sqlText

                sqlText = @" 
SELECT UserName
      ,Branch
      ,NoteNo
      ,SubNoteNo
      ,CreditNoteNo
      ,IssuedDate
      ,TaxChallanNo
      ,TaxChallanDate
      ,ReasonforIssuance
      ,ValueinChallan
      ,QuantityinChallan
      ,VATinChallan
      ,SDinChallan
      ,ValueofDecreasingAdjustment
      ,QuantityofDecreasingAdjustment
      ,VATofDecreasingAdjustment
      ,SDofDecreasingAdjustment
      ,Remarks
      ,ItemNo
      ,ProductDescription
      ,ProductName
      ,SubFormName

  FROM VAT9_1SubFormJ
where 1=1 and PeriodID=@PeriodID and NoteNo=@NoteNo
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", vm.PeriodId);
                objCommVAT19.Parameters.AddWithValue("@NoteNo", vm.NoteNo);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "GetDataVAT9_1_SubFormJ", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }

        public DataTable GetDataVAT9_1_SubFormK(VATReturnSubFormVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataTable dt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";

                #region sqlText

                sqlText = @" 
SELECT 
UserName
,Branch
,NoteNo
,SubNoteNo
,Amount
,VATAmount
,SDAmount
,Remarks
,SubFormName
,ChallanNumber
,BankName
,BankBranch
,Date
,AccountCode
,DepositId
,DetailRemarks

FROM VAT9_1SubFormK
where 1=1 and PeriodID=@PeriodID and NoteNo=@NoteNo
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", vm.PeriodId);
                objCommVAT19.Parameters.AddWithValue("@NoteNo", vm.NoteNo);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1_VATReturnDAL", "GetDataVAT9_1_SubFormK", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }

            #endregion

            return dt;
        }


    }
}
