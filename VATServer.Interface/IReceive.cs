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
    public interface IReceive
    {

        DataTable SearchByReferenceNo(string ReferenceNo, string ItemNo = "", SysDBInfoVMTemp connVM = null, string transactionType = "", string ShiftId = "0");

        decimal FormatingNumeric(decimal value, int DecPlace, SysDBInfoVMTemp connVM = null);

        DataTable GetExcelData(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] GetUSDCurrency(decimal subTotal, SysDBInfoVMTemp connVM = null);

        string[] ImportData(DataTable dtReceiveM, DataTable dtReceiveD, int branchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, Action callBack = null, SysDBInfoVMTemp connVM = null, string UserId = "");

        string[] ImportData_Sanofi(DataTable dtReceiveM, DataTable dtReceiveD, SysDBInfoVMTemp connVM = null);

        string[] ImportExcelFile(ReceiveMasterVM paramVM, SysDBInfoVMTemp connVM = null);

        string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null, string UserId = "");

        string[] ReceiveInsert(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn, int BranchId = 0, SysDBInfoVMTemp connVM = null, string UserId = "");

        string[] ReceivePost(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings, SqlTransaction transaction = null, SqlConnection currConn = null, SysDBInfoVMTemp connVM = null, string UserId = "");

        string[] ReceiveUpdate(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings, SysDBInfoVMTemp connVM = null, SqlConnection currConn = null, SqlTransaction transaction = null, string UserId = "");

        decimal ReturnReceiveQty(string receiveReturnId, string itemNo, SysDBInfoVMTemp connVM = null);

        string[] SaveTempReceive(DataTable dtTableResult, string transactionType, string CurrentUser, int branchId, Action callBack, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string BranchCode = "");

        DataTable SearchReceiveDetailNew(string ReceiveNo, string databaseName, SysDBInfoVMTemp connVM = null);

        DataTable SearchReceiveHeaderDTNew(string ReceiveNo, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ReceiveMasterVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null);

        List<ReceiveMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ReceiveMasterVM likeVM = null, SysDBInfoVMTemp connVM = null);

        List<ReceiveDetailVM> SelectReceiveDetail(string ReceiveNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

    }
}
