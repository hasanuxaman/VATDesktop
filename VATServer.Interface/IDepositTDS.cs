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
    public interface IDepositTDS
    {


        string[] DepositInsert(DepositMasterVM Master, List<VDSMasterVM> tds, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);
        string[] DepositInsert2(DepositMasterVM Master, List<VDSMasterVM> tds, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);
        string[] DepositInsertX(DepositMasterVM Master, List<VDSMasterVM> tds, AdjustmentHistoryVM adjHistory, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);
        string[] DepositPost(DepositMasterVM Master, List<VDSMasterVM> tds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null);
        string[] DepositPostX(DepositMasterVM Master, List<VDSMasterVM> tds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null);
        string[] DepositUpdate(DepositMasterVM Master, List<VDSMasterVM> tds, SysDBInfoVMTemp connVM = null);
        string[] DepositUpdateX(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SysDBInfoVMTemp connVM = null);
        string[] ImportData(DataTable dtDeposit, DataTable dtVDS, int branchId = 0, SysDBInfoVMTemp connVM = null);
        string[] UpdateVdsComplete(string flag, string VdsId, SysDBInfoVMTemp connVM = null);
        decimal ReverseAmount(string reverseDepositId, SysDBInfoVMTemp connVM = null);
        DataTable SearchVDSDT(string DepositNumber, SysDBInfoVMTemp connVM = null);
        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null);
        List<DepositMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
    



    }
}
