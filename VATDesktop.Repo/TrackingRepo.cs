using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.TrackingWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class TrackingRepo : ITracking
    {
      TrackingWCFClient wcf = new TrackingWCFClient();


       public DataTable SearchExistingTrackingItems(string isReceive, string receiveNo, string isSale, string saleInvoiceNo, SysDBInfoVMTemp connVM = null)
      {
          try
          {
             
              string connVMwcf = JsonConvert.SerializeObject(connVM);


              string result = wcf.SearchExistingTrackingItems(isReceive, receiveNo, isSale, saleInvoiceNo, connVMwcf);

              DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
              return results;


          }
          catch (Exception e)
          {
              throw e;
          }
      }

       public string TrackingDelete(List<string> Headings, SysDBInfoVMTemp connVM = null)
       {
           try
           {
               string Headingswcf = JsonConvert.SerializeObject(Headings);
               string connVMwcf = JsonConvert.SerializeObject(connVM);

               string result = wcf.TrackingDelete(Headingswcf, connVMwcf);

               return result;

           }
           catch (Exception ex)
           {
               
               throw ex;
           }
       }

       public DataTable FindTrackingItems(string fItemNo, string vatName, string effectDate, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.FindTrackingItems(fItemNo, vatName, effectDate, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public DataTable SearchReceiveTrackItems(string itemNo, string isTransaction, string transactionId, string type, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SearchReceiveTrackItems(itemNo, isTransaction, transactionId,type, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public DataTable SearchTrackingForReturn(string transactionType, string itemNo, string transactionID, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SearchTrackingForReturn(transactionType, itemNo, transactionID, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public DataTable SearchTrackingItems(string itemNo, string isIssue, string isReceive, string isSale, string finishItemNo, string receiveNo, string issueNo, string saleInvoiceNo, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SearchTrackingItems(itemNo, isIssue, isReceive,isSale,finishItemNo,receiveNo,issueNo,saleInvoiceNo, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public DataTable SearchTrackings(string itemNo, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SearchTrackings(itemNo, connVMwcf);

               DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public string TrackingInsert(List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string Trackingswcf = JsonConvert.SerializeObject(Trackings);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.SearchTrackings(Trackingswcf, connVMwcf);

               string results = JsonConvert.DeserializeObject<string>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }
       public string TrackingUpdate(List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
       {
           try
           {

               string Trackingswcf = JsonConvert.SerializeObject(Trackings);
               string connVMwcf = JsonConvert.SerializeObject(connVM);


               string result = wcf.TrackingUpdate(Trackingswcf, connVMwcf);

               string results = JsonConvert.DeserializeObject<string>(result);
               return results;


           }
           catch (Exception e)
           {
               throw e;
           }
       }







    }
}
