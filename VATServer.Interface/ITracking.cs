using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Interface
{
    public interface ITracking
    {
        DataTable SearchExistingTrackingItems(string isReceive, string receiveNo, string isSale, string saleInvoiceNo, SysDBInfoVMTemp connVM = null);

        string TrackingDelete(List<string> Headings, SysDBInfoVMTemp connVM = null);

        DataTable FindTrackingItems(string fItemNo, string vatName, string effectDate, SysDBInfoVMTemp connVM = null);

        DataTable SearchReceiveTrackItems(string itemNo, string isTransaction, string transactionId, string type, SysDBInfoVMTemp connVM = null);

        DataTable SearchTrackingForReturn(string transactionType, string itemNo, string transactionID, SysDBInfoVMTemp connVM = null);

        DataTable SearchTrackingItems(string itemNo, string isIssue, string isReceive, string isSale, string finishItemNo, string receiveNo, string issueNo, string saleInvoiceNo, SysDBInfoVMTemp connVM = null);

        DataTable SearchTrackings(string itemNo, SysDBInfoVMTemp connVM = null);

        string TrackingInsert(List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        string TrackingUpdate(List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

    
    }
}
