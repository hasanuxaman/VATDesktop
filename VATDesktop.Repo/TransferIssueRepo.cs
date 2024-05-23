using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.TransferIssueWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class TransferIssueRepo : ITransferIssue
    {
       TransferIssueWCFClient wcf = new TransferIssueWCFClient();


       public DataSet FormLoad(UOMVM UOMvm, ProductVM Productvm, string Name, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string UOMvmwcf = JsonConvert.SerializeObject(UOMvm);
               string Productvmwcf = JsonConvert.SerializeObject(Productvm);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.FormLoad(UOMvmwcf, Productvmwcf, Name, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataTable GetExcelData(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string invoiceListwcf = JsonConvert.SerializeObject(invoiceList);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.GetExcelData(invoiceListwcf, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }

       public DataTable GetExcelTransferData(List<string> invoiceList, string transactionType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {

           try
           {
               string invoiceListwcf = JsonConvert.SerializeObject(invoiceList);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.GetExcelTransferData(invoiceListwcf, transactionType,connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public string[] ImportData(DataTable dtTrIssueM, DataTable dtTrIssueD, int branchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, Action callBack = null, bool integration = false, SysDBInfoVMTemp connVM = null, string UserId = "")
       {

           try
           {
               string dtTrIssueMwcf = JsonConvert.SerializeObject(dtTrIssueM);
               string dtTrIssueDwcf = JsonConvert.SerializeObject(dtTrIssueD);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.ImportData(dtTrIssueMwcf, dtTrIssueDwcf,branchId,integration, connVMwcf);

               string[] results = JsonConvert.DeserializeObject<string[]>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public string[] ImportTransferData(DataTable dtTrIssueM, DataTable dtTrIssueD, int branchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string dtTrIssueMwcf = JsonConvert.SerializeObject(dtTrIssueM);
               string dtTrIssueDwcf = JsonConvert.SerializeObject(dtTrIssueD);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.ImportTransferData(dtTrIssueMwcf, dtTrIssueDwcf, branchId, connVMwcf);

               string[] results = JsonConvert.DeserializeObject<string[]>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public string[] Insert(TransferIssueVM Master, List<TransferIssueDetailVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null, List<TrackingVM> Trackings = null, bool CodeGenaration = true)
       {
           try
           {
               string Masterwcf = JsonConvert.SerializeObject(Master);
               string Detailswcf = JsonConvert.SerializeObject(Details);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.Insert(Masterwcf, Detailswcf, connVMwcf);

               string[] results = JsonConvert.DeserializeObject<string[]>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null, string UserId = null)
       {
           try
           {
               string Idswcf = JsonConvert.SerializeObject(Ids);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.MultiplePost(Idswcf, connVMwcf);

               string[] results = JsonConvert.DeserializeObject<string[]>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public string[] Post(TransferIssueVM Master, List<TransferIssueDetailVM> Details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string Masterwcf = JsonConvert.SerializeObject(Master);
               string Detailswcf = JsonConvert.SerializeObject(Details);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.Post(Masterwcf, Detailswcf, connVMwcf);

               string[] results = JsonConvert.DeserializeObject<string[]>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public string[] PostTransfer(TransferIssueVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string Masterwcf = JsonConvert.SerializeObject(Master);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.PostTransfer(Masterwcf, connVMwcf);

               string[] results = JsonConvert.DeserializeObject<string[]>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public string[] SaveTempTransfer(DataTable data, string BranchCode, string transactionType, string CurrentUser, int branchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool integration = false, SysDBInfoVMTemp connVM = null)
       {

           try
           {
               string datawcf = JsonConvert.SerializeObject(data);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SaveTempTransfer(datawcf,BranchCode,transactionType,CurrentUser,branchId,integration, connVMwcf);

               string[] results = JsonConvert.DeserializeObject<string[]>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public DataTable SearchTransfer(TransferVM vm, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string vmwcf = JsonConvert.SerializeObject(vm);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SearchTransfer(vmwcf, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public DataTable SearchTransferDetail(string TransferIssueNo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SearchTransferDetail(TransferIssueNo, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public DataTable SearchTransferIssue(TransferIssueVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string vmwcf = JsonConvert.SerializeObject(vm);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SearchTransferIssue(vmwcf, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
               string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf, Dt,connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public List<TransferIssueVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
               string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);

               List<TransferIssueVM> results = JsonConvert.DeserializeObject<List<TransferIssueVM>>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public List<TransferIssueDetailVM> SelectDetail(string TransferIssueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
               string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SelectDetail(TransferIssueNo, conditionFieldswcf, conditionValueswcf, connVMwcf);

               List<TransferIssueDetailVM> results = JsonConvert.DeserializeObject<List<TransferIssueDetailVM>>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public DataSet TransferMovement(string ItemNo, string FDate, string TDate, int BranchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Summery = false, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TransferMovement(ItemNo, FDate, TDate,BranchId,Summery, connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public DataSet TransferStock(string ItemNo, string FDate, int BranchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null) 
       {

           try
           {
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TransferStock(ItemNo, FDate, BranchId,connVMwcf);

               DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public string[] Update(TransferIssueVM Master, List<TransferIssueDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "", SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
       {

           try
           {
               string Masterwcf = JsonConvert.SerializeObject(Master);
               string Detailswcf = JsonConvert.SerializeObject(Details);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.Update(Masterwcf, Detailswcf, connVMwcf);

               string[] results = JsonConvert.DeserializeObject<string[]>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public string[] UpdateTransferIssue(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string invoiceListwcf = JsonConvert.SerializeObject(invoiceList);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.UpdateTransferIssue(invoiceListwcf, connVMwcf);

               string[] results = JsonConvert.DeserializeObject<string[]>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }


    }
}
