using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.ReportWCF;
using VATServer.Interface;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class ReportDSRepo : IReport
    {
       //ReportDSDAL _dal = new ReportDSDAL();
       ReportWCFClient wcf = new ReportWCFClient();

        public DataSet TransferIssueNew(string TransferIssueNo, string IssueDateFrom, string IssueDateTo, string itemNo,
                                string categoryID, string productType, string TransactionType, string Post, string DBName = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TransferIssueNew(TransferIssueNo, IssueDateFrom, IssueDateTo, itemNo, categoryID, productType,TransactionType,Post,DBName, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VAT6_10Report(string TotalAmount, string StartDate
            , string EndDate, string post1, string post2, int BranchId = 0, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.VAT6_10Report(TotalAmount, StartDate, EndDate, post1, post2, BranchId,connVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

       public DataTable MIS19(string StartDate, string EndDate, SysDBInfoVMTemp connVM = null)
       {
           try
           {
              
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.MIS19(StartDate, EndDate,connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT6_3(string SalesInvoiceNo, string Post1, string Post2, string ddmmyy = "n"
           , SysDBInfoVMTemp connVM = null, bool mulitplePreview = false, string getTopValue = "", string pdfFlag = "N", int FromRow = 0, int ToRow = 99999, string transactionType = "")
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT11ReportNew(SalesInvoiceNo, Post1,Post2,ddmmyy, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet MegnaVAT6_3(string SalesInvoiceId, string Post1, string Post2, string ddmmyy = "n"
        , SysDBInfoVMTemp connVM = null, bool mulitplePreview = false, string getTopValue = "", string pdfFlag = "N", int FromRow = 0, int ToRow = 99999, string transactionType = "")
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT11ReportNew(SalesInvoiceId, Post1, Post2, ddmmyy, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet PurchaseReturn(string PurchaseInvoiceNo, string Post1, string Post2, SysDBInfoVMTemp connVM = null, int FromRow = 0, int ToRow = 99999, string transactionType = "")
       {
           try
           {

               //string connVMwcf = JsonConvert.SerializeObject(connVM);


               //string result = wcf.VAT11ReportNew(SalesInvoiceNo, Post1, Post2, ddmmyy, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>("");
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT6_3Toll(string TollNo, string Post1, string Post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT6_3Toll(TollNo, Post1, Post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT11ReportCommercialImporterNew(string SalesInvoiceNo, string Post1, string Post2, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT11ReportCommercialImporterNew(SalesInvoiceNo, Post1, Post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet SaleTrackingReport(string SalesInvoiceNo, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SaleTrackingReport(SalesInvoiceNo,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT20ReportNew(string SalesInvoiceNo, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT20ReportNew(SalesInvoiceNo, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT6_7(string SalesInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT6_7(SalesInvoiceNo, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT6_8(string SalesInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT6_8(SalesInvoiceNo, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet CreditNoteNew(string SalesInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.CreditNoteNew(SalesInvoiceNo, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet CreditNoteAmountNew(string SalesInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.CreditNoteAmountNew(SalesInvoiceNo, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet DebitNoteNew(string SalesInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.DebitNoteNew(SalesInvoiceNo, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet BatchTracking(string BatchNo, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BatchTracking(BatchNo, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet BatchTracking1(string BatchNo, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BatchTracking1(BatchNo, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet BatchTracking2(string BatchNo, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BatchTracking2(BatchNo, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT16NewforTollRegister(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, string ReportName, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT16NewforTollRegister(ItemNo, UserName, StartDate, EndDate,post1,post2,ReportName, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT6_1_WithConn(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, string ReportName, int BranchId = 0
           , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Opening = false
           , SysDBInfoVMTemp connVM = null, bool OpeningFromProducct = true, string ProdutType = "", string ProdutCategoryId = ""
           , bool VAT6_2_1 = false, bool StockMovement = false)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT6_1_WithConn(ItemNo, UserName, StartDate, EndDate, post1, post2, ReportName, BranchId, Opening, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT6_1Toll(string ItemNo, string UserName, string StartDate, string EndDate, string post1,
           string post2, string ReportName, int BranchId = 0, SysDBInfoVMTemp connVM = null, bool IsOpening = true,
           SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, VAT6_1ParamVM vm = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT6_1Toll(ItemNo, UserName, StartDate, EndDate, post1, post2, ReportName, BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT6_1_Branching(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, string ReportName, string DBName = "", string BranchName = "", SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT6_1_Branching(ItemNo, UserName, StartDate, EndDate, post1, post2, ReportName, DBName, BranchName, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT6_1(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, string ReportName, string DBName = "", string BranchName = "", SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT6_1(ItemNo, UserName, StartDate, EndDate, post1, post2, ReportName, DBName, BranchName, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
        
       }

       public DataSet VAT6_2(string ItemNo, string StartDate, string EndDate, string post1, string post2, int BranchId = 0, bool Opening = false, bool Opening6_2 = false, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT6_2(ItemNo, StartDate, EndDate, post1, post2, BranchId, Opening, Opening6_2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT6_2Toll(string ItemNo, string StartDate, string EndDate, string post1, string post2,
           int BranchId = 0, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null,
           SqlTransaction Vtransaction = null, VAT6_2ParamVM vm = null, bool openingValue = false)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT6_2Toll(ItemNo, StartDate, EndDate, post1, post2, BranchId,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT18New(string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {

           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT18New(UserName,StartDate, EndDate,post1,post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet SD_Data(string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SD_Data(UserName, StartDate, EndDate, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT19NewNewformat(string PeriodName, string ExportInBDT, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT19NewNewformat(PeriodName,ExportInBDT, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       
        #region Comments Jul-12-2020
       
       //////#region 2012 Law - VAT 9.1


       //////public DataSet VAT9_1(string PeriodName, int BranchId = 0, string Date = "", SysDBInfoVMTemp connVM = null)
       //////{
       //////    try
       //////    {

       //////        string connVMwcf = JsonConvert.SerializeObject(connVM);


       //////        string result = wcf.VAT9_1(PeriodName, BranchId,Date, connVMwcf);

       //////        DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
       //////        return results;


       //////    }
       //////    catch (Exception e)
       //////    {
       //////        throw e;
       //////    }
       //////}

       ////////// LineNo=1,2,3,4,5,7
       //////public DataTable VAT9_1_SubFormAPart3(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       //////{
       //////    try
       //////    {

       //////        string connVMwcf = JsonConvert.SerializeObject(connVM);


       //////        string result = wcf.VAT9_1_SubFormAPart3(PeriodName, ExportInBDT, LineNo, BranchId, connVMwcf);

       //////        DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
       //////        return results;


       //////    }
       //////    catch (Exception e)
       //////    {
       //////        throw e;
       //////    }
       //////}

       ////////// LineNo=10,11,12,13,14,15,16,17,19,20,21,22
       //////public DataTable VAT9_1_SubFormAPart4(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       //////{
       //////    try
       //////    {

       //////        string connVMwcf = JsonConvert.SerializeObject(connVM);


       //////        string result = wcf.VAT9_1_SubFormAPart4(PeriodName, ExportInBDT, LineNo, BranchId, connVMwcf);

       //////        DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
       //////        return results;


       //////    }
       //////    catch (Exception e)
       //////    {
       //////        throw e;
       //////    }
       //////}

       //////// LineNo=8
       //////public DataTable VAT9_1_SubFormBPart3(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       //////{
       //////    try
       //////    {

       //////        string connVMwcf = JsonConvert.SerializeObject(connVM);


       //////        string result = wcf.VAT9_1_SubFormBPart3(PeriodName, ExportInBDT, LineNo, BranchId, connVMwcf);

       //////        DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
       //////        return results;


       //////    }
       //////    catch (Exception e)
       //////    {
       //////        throw e;
       //////    }
       //////}

       //////// LineNo=6
       //////public DataTable VAT9_1_SubFormCPart3(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       //////{
       //////    try
       //////    {

       //////        string connVMwcf = JsonConvert.SerializeObject(connVM);


       //////        string result = wcf.VAT9_1_SubFormCPart3(PeriodName, ExportInBDT, LineNo, BranchId, connVMwcf);

       //////        DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
       //////        return results;


       //////    }
       //////    catch (Exception e)
       //////    {
       //////        throw e;
       //////    }
       //////}

       //////// LineNo=18
       //////public DataTable VAT9_1_SubFormCPart4(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       //////{
       //////    try
       //////    {

       //////        string connVMwcf = JsonConvert.SerializeObject(connVM);


       //////        string result = wcf.VAT9_1_SubFormCPart4(PeriodName, ExportInBDT, LineNo, BranchId, connVMwcf);

       //////        DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
       //////        return results;


       //////    }
       //////    catch (Exception e)
       //////    {
       //////        throw e;
       //////    }
       //////}

       //////// LineNo=52,53,54,55,56,57,58,59,60,61
       //////public DataTable VAT9_1_SubFormGPart8(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       //////{

       //////    try
       //////    {

       //////        string connVMwcf = JsonConvert.SerializeObject(connVM);


       //////        string result = wcf.VAT9_1_SubFormGPart8(PeriodName, ExportInBDT, LineNo, BranchId, connVMwcf);

       //////        DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
       //////        return results;


       //////    }
       //////    catch (Exception e)
       //////    {
       //////        throw e;
       //////    }
       //////}

       //////// LineNo=24
       //////public DataTable VAT9_1_SubFormDPart5(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       //////{
       //////    try
       //////    {

       //////        string connVMwcf = JsonConvert.SerializeObject(connVM);


       //////        string result = wcf.VAT9_1_SubFormDPart5(PeriodName, ExportInBDT, LineNo, BranchId, connVMwcf);

       //////        DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
       //////        return results;


       //////    }
       //////    catch (Exception e)
       //////    {
       //////        throw e;
       //////    }
       //////}

       //////// LineNo=29
       //////public DataTable VAT9_1_SubFormEPart6(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       //////{
       //////    try
       //////    {

       //////        string connVMwcf = JsonConvert.SerializeObject(connVM);


       //////        string result = wcf.VAT9_1_SubFormEPart6(PeriodName, ExportInBDT, LineNo, BranchId, connVMwcf);

       //////        DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
       //////        return results;


       //////    }
       //////    catch (Exception e)
       //////    {
       //////        throw e;
       //////    }
       //////}

       //////// LineNo=30
       //////public DataTable VAT9_1_SubFormFPart6(string PeriodName, string ExportInBDT = "Y", int LineNo = 1, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       //////{
       //////    try
       //////    {

       //////        string connVMwcf = JsonConvert.SerializeObject(connVM);


       //////        string result = wcf.VAT9_1_SubFormFPart6(PeriodName, ExportInBDT, LineNo, BranchId, connVMwcf);

       //////        DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
       //////        return results;


       //////    }
       //////    catch (Exception e)
       //////    {
       //////        throw e;
       //////    }
       //////}

       //////public string[] VAT9_1_Process(string PeriodName, int BranchId = 0, string ExportInBDT = "Y", SysDBInfoVMTemp connVM = null)
       //////{
       //////    try
       //////    {
              
       //////        string connVMwcf = JsonConvert.SerializeObject(connVM);

       //////        string resultInsert = wcf.VAT9_1_Process(PeriodName, BranchId, ExportInBDT,connVMwcf);

       //////        string[] results = JsonConvert.DeserializeObject<string[]>(resultInsert);

       //////        return results;


       //////    }
       //////    catch (Exception e)
       //////    {
       //////        throw e;
       //////    }
       //////}

       //////public DataSet VAT9_1_Report(string PeriodName, int BranchId = 0, string ExportInBDT = "Y", SysDBInfoVMTemp connVM = null)
       //////{
       //////    try
       //////    {

       //////        string connVMwcf = JsonConvert.SerializeObject(connVM);


       //////        string result = wcf.VAT9_1_Report(PeriodName,BranchId, ExportInBDT,connVMwcf);

       //////        DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
       //////        return results;


       //////    }
       //////    catch (Exception e)
       //////    {
       //////        throw e;
       //////    }
       //////}

       ////// public DataSet VAT9_1_V2Save(string PeriodName, int BranchId = 0, string Date = "", SysDBInfoVMTemp connVM = null)
       //////{
       //////    try
       //////    {

       //////        string connVMwcf = JsonConvert.SerializeObject(connVM);


       //////        string result = wcf.VAT9_1_V2(PeriodName, BranchId, Date, connVMwcf);

       //////        DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
       //////        return results;


       //////    }
       //////    catch (Exception e)
       //////    {
       //////        throw e;
       //////    }
       //////}
       //////#endregion

       #endregion

        public DataSet VAT18Breakdown(string PeriodName, string ExportInBDT, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT18Breakdown(PeriodName, ExportInBDT, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet RepFormKaTradingNew(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.RepFormKaTradingNew(ItemNo, UserName, StartDate,EndDate,post1,post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VAT6_2_1(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT6_2_1(ItemNo, UserName, StartDate, EndDate, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VDS12KhaNew(string VendorId, string DepositNumber, string DepositDateFrom, string DepositDateTo, string IssueDateFrom, string IssueDateTo, string BillDateFrom, string BillDateTo, string PurchaseNumber, bool chkPurchaseVDS, bool chkAll, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VDS12KhaNew(VendorId, DepositNumber, DepositDateFrom, DepositDateTo, IssueDateFrom, IssueDateTo,BillDateFrom,BillDateTo,PurchaseNumber,chkPurchaseVDS,chkAll,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VAT24(string ddbackno, string ddbFinishItemNo, string SalesInvoiceNo, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT24(ddbackno, ddbFinishItemNo, SalesInvoiceNo, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VAT22(string ddbackno, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT22(ddbackno, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VATDDB(string ddbackno, string salesInvoice, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VATDDB(ddbackno,salesInvoice, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet PurchaseMis(string PurchaseId, int BranchId = 0, string VatType = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.PurchaseMis(PurchaseId, BranchId, VatType, connVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
       }

        public DataSet SaleMis(string SaleId, string ShiftId = "0", int BranchId = 0, string VatType = "", SysDBInfoVMTemp connVM = null, string OrderBy = "")
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SaleMis(SaleId,ShiftId, BranchId, VatType, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet IssueMis(string IssueId, int BranchId, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.IssueMis(IssueId,BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet ReceiveMis(string ReceiveId, string ShiftId = "0", int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {

           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.ReceiveMis(ReceiveId,ShiftId, BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet SaleReceiveMIS(string StartDate, string EndDate, string ShiftId = "0", string Post = null, SysDBInfoVMTemp connVM = null,string Toll="N")
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SaleReceiveMIS(StartDate, EndDate, ShiftId, Post, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VAT1KaNew(string FinishItemNo, string EffectDate, string VATName, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT1KaNew(FinishItemNo, EffectDate, VATName,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VAT1KhaNew(string FinishItemNo, string EffectDate, string VATName, SysDBInfoVMTemp connVM = null)
       {

           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT1KhaNew(FinishItemNo, EffectDate, VATName, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VAT1GaNew(string FinishItemNo, string EffectDate, string VATName, string IsPercent, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT1GaNew(FinishItemNo, EffectDate, VATName,IsPercent, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VAT1GhaNew(string finishitemno, string EffectDate, string VATName, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT1GhaNew(finishitemno, EffectDate, VATName, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet FormKaNew(string FinishItemNo, string EffectDate, string VATName, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.FormKaNew(FinishItemNo, EffectDate, VATName, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet BOMNew_withFNo(string FinishItemNo, string EffectDate, string VATName, string IsPercent, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BOMNew_withFNo(FinishItemNo, EffectDate, VATName,IsPercent, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet BOMNew(string BOMId, string VATName, string IsPercent, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BOMNew(BOMId, VATName, IsPercent,BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet BOMDownload(SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.BOMDownload(connVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
       }

        public DataSet BankNew(string BankID, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BankNew(BankID,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet CustomerGroupNew(string CustomerGroupID, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.CustomerGroupNew(CustomerGroupID, connVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
           
       }

        public DataSet CustomerNew(string CustomerID, string CustomerGroupID, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.CustomerNew(CustomerID,CustomerGroupID, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

         public DataSet DepositNew(string DepositNo,
                                 string DepositDateFrom, string DepositDateTo, string BankID, string Post,
                                 string transactionType, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.DepositNew(DepositNo, DepositDateFrom,DepositDateTo,BankID,Post,transactionType, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VATDisposeDsNew(string DisposeNumber, SysDBInfoVMTemp connVM = null)
         {
             try
             {

                 string connVMwcf = JsonConvert.SerializeObject(connVM);


                 string result = wcf.VATDisposeDsNew(DisposeNumber, connVMwcf);

                 DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
                 return results;


             }
             catch (Exception e)
             {
                 throw e;
             }
       }

        public DataSet MISVAT16New(string CategoryId, string StartDate, string EndDate, string UserName, string ItemNo, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.MISVAT16New(CategoryId, StartDate,EndDate,UserName,ItemNo, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet MISVAT17New(string CategoryId, string UserName, string StartDate, string EndDate, string ItemNo, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.MISVAT17New(CategoryId, UserName,StartDate, EndDate, ItemNo, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet MISVAT18New(string UserName, string StartDate, string EndDate, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.MISVAT18New(UserName, StartDate, EndDate,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet ProductCategoryNew(string cgID, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.ProductCategoryNew(cgID, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet ProductNew(string ItemNo, string CategoryID, string IsRaw, SysDBInfoVMTemp connVM = null, string ProductCode = "")
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.ProductNew(ItemNo,CategoryID,IsRaw, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet PurchaseNew(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
                             string VendorId, string ItemNo, string CategoryID, string ProductType,
                             string TransactionType, string Post,
                             string PurchaseType, string VendorGroupId, string FromBOM, string UOM, string UOMn,
                             decimal UOMc, decimal TotalQty, decimal rCostPrice, bool pCategoryLike = false, string PGroup = "", int BranchId = 0, string VatType = "", string IsRebate = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.PurchaseNew(PurchaseInvoiceNo, InvoiceDateFrom, InvoiceDateTo,VendorId,ItemNo,CategoryID,ProductType,TransactionType,Post,PurchaseType,VendorGroupId,
                    FromBOM, UOM, UOMn, UOMc, TotalQty, rCostPrice, pCategoryLike, PGroup, BranchId, VatType, IsRebate,connVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
       }

        public DataSet IssueNew(string IssueNo, string IssueDateFrom, string IssueDateTo, string itemNo,
                                string categoryID, string productType, string TransactionType, string Post, string waste, bool pCategoryLike = false, string PGroup = "", int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.IssueNew(IssueNo,IssueDateFrom,IssueDateTo, itemNo,
                                categoryID, productType, TransactionType, Post, waste, pCategoryLike, PGroup, BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet ReceiveNew(string ReceiveNo, string ReceiveDateFrom, string ReceiveDateTo, string itemNo,
                                  string categoryID, string productType, string transactionType, string post, string ShiftId = "0", int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.ReceiveNew(ReceiveNo,ReceiveDateFrom,ReceiveDateTo,itemNo,
                                  categoryID, productType, transactionType, post, ShiftId, BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet SaleNew(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, string Customerid,
                            string ItemNo, string CategoryID, string productType, string TransactionType, string Post,
                            string onlyDiscount, bool bPromotional, string CustomerGroupID, bool pCategoryLike = false, string PGroup = ""
            , string ShiftId = "0", int branchId = 0, string VatType = "", SysDBInfoVMTemp connVM = null, string OrderBy = "", string DataSource = "", string Toll = "N", string Type = "", string ReportType = "")
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SaleNew(SalesInvoiceNo, InvoiceDateFrom, InvoiceDateTo, Customerid,
                                  ItemNo, CategoryID, productType, TransactionType, Post, onlyDiscount,bPromotional,CustomerGroupID,pCategoryLike,PGroup,ShiftId,branchId,VatType, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet SaleNewWithChassisEngine(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, string Customerid,
                               string ItemNo, string CategoryID, string productType, string TransactionType, string Post,
                               string onlyDiscount, bool bPromotional, string CustomerGroupID, string chassis, string engine, string ShiftId = "0",
            int branchId = 0, SysDBInfoVMTemp connVM = null, string OrderBy = "")
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SaleNewWithChassisEngine(SalesInvoiceNo,InvoiceDateFrom,InvoiceDateTo,Customerid,
                                  ItemNo,CategoryID,productType,TransactionType,Post,onlyDiscount,bPromotional,CustomerGroupID,chassis,engine,ShiftId,branchId,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet StockNew(string ProductNo, string CategoryNo, string ItemType, string StartDate, string EndDate,
                                string Post1, string Post2, bool WithoutZero = false, bool pCategoryLike = false, string PGroup = "", int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.StockNew(ProductNo, CategoryNo, ItemType, StartDate,
                                  EndDate, Post1, Post2, WithoutZero, pCategoryLike, PGroup, BranchId,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet StockWithAdjNew(string ProductNo, string CategoryNo, string ItemType, string StartDate, string EndDate,
                               string Post1, string Post2, bool WithoutZero = false, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.StockWithAdjNew(ProductNo, CategoryNo, ItemType, StartDate,
                                  EndDate, Post1, Post2, WithoutZero,BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet StockWastage(string ProductNo, string CategoryNo, string ItemType, string StartDate, string EndDate,
                             string Post1, string Post2, bool WithoutZero = false, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.StockWastage(ProductNo, CategoryNo, ItemType, StartDate,
                                  EndDate, Post1, Post2, WithoutZero, BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VehicleNew(string VehicleNo, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VehicleNew(VehicleNo, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet Adjustment(string HeadId, string AdjType, string StartDate, string EndDate, string Post, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.Adjustment(HeadId,AdjType,StartDate,EndDate,Post,BranchId,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VendorGroupNew(string VendorGroupID, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VendorGroupNew(VendorGroupID,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet InputOutputCoEfficient(string RawItemNo, string StartDate, string EndDate, string Post1, string Post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.InputOutputCoEfficient(RawItemNo,StartDate,EndDate,Post1,Post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet VendorReportNew(string VendorID, string VendorGroupID, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.VendorReportNew(VendorID, VendorGroupID, connVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
       }

        public DataSet TrasurryDepositeNew(string DepositId, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TrasurryDepositeNew(DepositId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        #region TDS Reports
        public DataSet TDSDeposit(string DepositId, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TDSDeposit(DepositId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet TDSDepositDetail(string DepositId, SysDBInfoVMTemp connVM = null)
       {

           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TDSDepositDetail(DepositId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet TDSDepositDetail_MISReport(string DepositNo,
                               string DepositDateFrom, string DepositDateTo, string BankID, string Post,
                               string transactionType, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.TDSDepositDetail_MISReport(DepositNo, DepositDateFrom,DepositDateTo,BankID,Post,transactionType, connVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
       }

        public DataSet TDSDeposit_MISReport(string DepositNo,
                                 string DepositDateFrom, string DepositDateTo, string BankID, string Post,
                                 string transactionType, SysDBInfoVMTemp connVM = null)
       {

           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TDSDeposit_MISReport(DepositNo, DepositDateFrom, DepositDateTo, BankID, Post, transactionType, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        #endregion
        public DataSet VDSDepositNew(string DepositId, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VDSDepositNew(DepositId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet SDTrasurryDepositeNew(string DepositId, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SDTrasurryDepositeNew(DepositId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet ComapnyProfileString(string CompanyID, string UserId, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.ComapnyProfileString(CompanyID, UserId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet ComapnyProfile(string CompanyID, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.ComapnyProfile(CompanyID,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet CurrencyReportNew(SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.CurrencyReportNew(connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet CostingNew(string ID, string ItemNo, string UOM, string UOMn, decimal UOMc,
                                  decimal totalQty, decimal rCostPrice, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.CostingNew(ID,ItemNo,UOM,UOMn,UOMc,
                                  totalQty, rCostPrice, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

        public DataSet ComapnyProfileSecurity(string CompanyID, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.ComapnyProfileSecurity(CompanyID, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataTable MonthlyPurchases(string PurchaseInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
          string VendorId, string ItemNo, string CategoryID, string ProductType, string TransactionType, string Post,
          string PurchaseType, string VendorGroupId, string FromBOM, string UOM, string UOMn, decimal UOMc, decimal TotalQty, decimal rCostPrice, int BranchId = 0, string VatType = "", SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.MonthlyPurchases(PurchaseInvoiceNo,InvoiceDateFrom,InvoiceDateTo,
         VendorId,ItemNo,CategoryID,ProductType,TransactionType,Post,
          PurchaseType, VendorGroupId, FromBOM, UOM, UOMn, UOMc, TotalQty, rCostPrice, BranchId, VatType, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }


       public DataSet VDSReport(string DepositNo,
                                 string DepositDateFrom, string DepositDateTo, string BankID, string Post,
                                 string transactionType, string Vendor, SysDBInfoVMTemp connVM = null)
       {

           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VDSReport(DepositNo,
                              DepositDateFrom,DepositDateTo,BankID,Post,
                              transactionType,Vendor, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet DemandReport(string DemandNo, string DemandDateFrom, string DemandDateTo, string TransactionType, string Post, SysDBInfoVMTemp connVM = null)
       {

           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.DemandReport(DemandNo,
                              DemandDateFrom, DemandDateTo, TransactionType, Post,
                               connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet BanderolForm_4(string BanderolID, string post1, string StartDate, string EndDate, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BanderolForm_4(BanderolID,
                              post1, StartDate, EndDate, post2,
                               connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet BanderolForm_5(string PeriodName, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BanderolForm_5(PeriodName,
                               connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public string GetReturnType(string itemNo, string transactionType, SysDBInfoVMTemp connVM = null)
       {

           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.GetReturnType(itemNo,transactionType,
                               connVMwcf);

               string results = JsonConvert.DeserializeObject<string>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet SelectMultipleInvoices(int noOfChallan, string transactionType, string challanDateFrom, string challanDateTo, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SelectMultipleInvoices(noOfChallan, transactionType,challanDateFrom,challanDateTo,
                               connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       #region RptBanderolProduct
       public DataSet RptBanderolProduct(string ProductCode, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.RptBanderolProduct(ProductCode,
                               connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       #endregion

        #region Bureau
       public DataSet BureauVAT11Report(string SalesInvoiceNo, string Post1, string Post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BureauVAT11Report(SalesInvoiceNo,Post1,Post2,
                               connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet BureauVAT6_1Report(string ItemNo, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {

           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BureauVAT6_1Report(ItemNo, StartDate, EndDate,post1,post2,
                               connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet BureauVAT18Report(string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BureauVAT18Report(UserName, StartDate, EndDate, post1, post2,
                               connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       //Format before 30 June2014
       public DataSet BureauVAT18_OldFormat(string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BureauVAT18_OldFormat(UserName, StartDate, EndDate, post1, post2,
                               connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataTable BureauMonthlySales(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo,
                                     string Customerid, string ItemNo, string CategoryID, string productType,
                                     string TransactionType, string Post, string onlyDiscount, bool bPromotional,
                                     string CustomerGroupID, string ShiftId = "1", int branchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BureauMonthlySales(SalesInvoiceNo,InvoiceDateFrom,InvoiceDateTo,
                                     Customerid,ItemNo,CategoryID,productType,
                                     TransactionType,Post,onlyDiscount,bPromotional,
                                    CustomerGroupID, ShiftId, branchId, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet BureauSaleMis(string SaleId, string ShiftId = "0", int branchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BureauSaleMis(SaleId, ShiftId, branchId,
                               connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet BureauSaleNew(string SalesInvoiceNo, string InvoiceDateFrom, string InvoiceDateTo, string Customerid,
                              string ItemNo, string CategoryID, string productType, string TransactionType, string Post,
                              string onlyDiscount, bool bPromotional, string CustomerGroupID, string ShiftId = "1", int branchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BureauSaleNew(SalesInvoiceNo,InvoiceDateFrom,InvoiceDateTo,Customerid,
                             ItemNo,CategoryID, productType,TransactionType,Post,
                              onlyDiscount, bPromotional, CustomerGroupID, ShiftId, branchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet BureauCreditNote(string SalesInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BureauCreditNote(SalesInvoiceNo, post1,post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }


       public DataSet BureauVAT19Report(string PeriodName, string ExportInBDT, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.BureauVAT19Report(PeriodName, ExportInBDT, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
        #endregion

       public DataSet RptDeliveryReport(string challanNo, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.RptDeliveryReport(challanNo, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet RptVAT7Report(string vat7No, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.RptVAT7Report(vat7No,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet TollRegister(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TollRegister(ItemNo,UserName,StartDate,EndDate,post1,post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet TollRegisterRaw(string ItemNo, string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TollRegisterRaw(ItemNo, UserName, StartDate, EndDate, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT16AttachToll(string ItemNo, string UserName, string StartDate, string EndDate, string post1,
                               string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT16AttachToll(ItemNo, UserName, StartDate, EndDate, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet PurchaseReturnNew(string PurchaseInvoiceNo, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.PurchaseReturnNew(PurchaseInvoiceNo, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet Current_AC_VAT18(string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.Current_AC_VAT18(StartDate,EndDate, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet SerialStockStatus(string ItemNo, string CategoryID, string ProductType, string StartDate, string ToDate, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {

           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SerialStockStatus(ItemNo, CategoryID, ProductType, StartDate, ToDate, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }


       public DataSet PurchaseWithLCInfo(string PurchaseInvoiceNo, string LCDateFrom, string LCDateTo,
                                 string VendorId, string ItemNo, string VendorGroupId, string LCNo, string Post
                               , SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.PurchaseWithLCInfo(PurchaseInvoiceNo, LCDateFrom, LCDateTo, VendorId, ItemNo, VendorGroupId, LCNo, Post, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VAT18_Sanofi(string UserName, string StartDate, string EndDate, string post1, string post2, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VAT18_Sanofi(UserName, StartDate, EndDate, post1, post2, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet TDSReport(SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TDSReport(connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet LocalPurchaseReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.LocalPurchaseReport(StartDate, EndDate, BranchId,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet ImportDataReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.ImportDataReport(StartDate, EndDate, BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet ReceiedVsSaleReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null,string Toll="N")
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.ReceiedVsSaleReport(StartDate, EndDate, BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet SalesStatementForServiceReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null, string Toll = "N")
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SalesStatementForServiceReport(StartDate, EndDate, BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet SalesStatementDeliveryReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null,string Toll="N")
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SalesStatementDeliveryReport(StartDate, EndDate, BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet StockReportFGReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null,String FiscalYear=null,string UserId = "")
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.StockReportFGReport(StartDate, EndDate, BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
           
       }

       public DataSet StockReportRMReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null, string UserId = "")
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.StockReportRMReport(StartDate, EndDate, BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet TransferToDepotReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TransferToDepotReport(StartDate, EndDate, BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet VDSStatementReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.VDSStatementReport(StartDate, EndDate, BranchId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet Chak_kaReport(string StartDate, string EndDate, int BranchId = 0, int TransferTo = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.Chak_kaReport(StartDate, EndDate, BranchId,TransferTo, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet Chak_khaReport(string StartDate, string EndDate, int BranchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.Chak_khaReport(StartDate, EndDate, BranchId,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet TDS_Certificatet(string DepositId, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TDS_Certificatet(DepositId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet TDSAmountReport(string[] conditionFields = null, string[] conditionValues = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);

               string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
               string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
               string result = wcf.TDSAmountReport(conditionFieldswcf, conditionValueswcf,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet TransferIssueOutReport(string IssueNo, string IssueDateFrom, string IssueDateTo, string TType, int BranchId = 0, int TransferTo = 0, SysDBInfoVMTemp connVM = null, string ShiftId = "0")
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TransferIssueOutReport(IssueNo,IssueDateFrom,IssueDateTo,TType,BranchId,TransferTo, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet TransferReceiveInReport(string IssueNo, string IssueDateFrom, string IssueDateTo, string TType, int BranchId = 0, int BranchFromId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TransferReceiveInReport(IssueNo, IssueDateFrom, IssueDateTo, TType, BranchId, BranchFromId, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataSet Wastage(string ItemNo, string ProdutCategoryId, string ProductType, string post1, string post2, string StartDate, string EndDate, int branchId = 0, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);

               string result ="";
               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);

               //string result = wcf.Wastage(ItemNo, CategoryID, Type, Post, StartDate, EndDate, BranchId, connVMwcf);

               //DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       #region Web 
       //public List<VAT_16VM> VAT16List(DataSet ReportResult, SysDBInfoVMTemp connVM = null)
       //{
       //    List<VAT_16VM> VMs = new List<VAT_16VM>();
       //    try
       //    {
       //        VMs = _dal.VAT16List(ReportResult,connVM);

       //    }
       //    catch (Exception ex)
       //    {

       //    }
       //    return VMs;
       //}

       //public List<VAT_17VM> VAT17List(DataSet ReportResult, string itemNo, SysDBInfoVMTemp connVM = null)
       //{
       //    List<VAT_17VM> VMs = new List<VAT_17VM>();
       //    try
       //    {
       //        VMs = _dal.VAT17List(ReportResult,itemNo, connVM);

       //    }
       //    catch (Exception ex)
       //    {

       //    }
       //    return VMs;
       //}
       #endregion

    }
}
