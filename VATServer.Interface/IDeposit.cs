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
    public interface IDeposit
    {

        string[] DepositInsert(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        string[] DepositInsert2(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        string[] DepositInsertX(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        string[] DepositPost(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null);

        string[] DepositPostX(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null);

        string[] DepositUpdate(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null);

        string[] ImportData(DataTable dtDeposit, DataTable dtVDS, int branchId = 0, SysDBInfoVMTemp connVM = null);

        string[] ImportDataSingle(DataTable dtDeposit, DataTable dtVDS, int branchId = 1, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] SaveTempVDS(DataTable data, string BranchCode, string transactionType, string CurrentUser, int branchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] UpdateVdsComplete(string flag, string VdsId, SysDBInfoVMTemp connVM = null, string TransactionType ="");

        decimal ReverseAmount(string reverseDepositId, SysDBInfoVMTemp connVM = null);

        DataTable GetExcelData(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        DataTable SearchVDSDT(string DepositNumber, SysDBInfoVMTemp connVM = null, bool chkPurchaseVDS = false);

        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null, string transactionOpening = "", string CashPayableSearch = "");

        List<DepositMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string transactionOpening = "");
    



    }
}
