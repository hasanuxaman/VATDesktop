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
    public interface IAdjustmentHistory
    {

       string[] InsertAdjustmentHistory(AdjustmentHistoryVM vm, SysDBInfoVMTemp connVM = null);

       string[] UpdateAdjustmentHistory(AdjustmentHistoryVM vm, SysDBInfoVMTemp connVM = null);

           string[] PostAdjustmentHistory(string AdjHistoryID, string AdjId, string AdjDate, decimal AdjAmount, decimal AdjVATRate, decimal AdjVATAmount, decimal AdjSD,
        decimal AdjSDAmount, decimal AdjOtherAmount, string AdjType, string AdjDescription, string CreatedBy,
        string CreatedOn, string LastModifiedBy, string LastModifiedOn
        , decimal AdjInputAmount, decimal AdjInputPercent, string AdjReferance, string Post, string AdjHistoryNo, SysDBInfoVMTemp connVM = null);

           DataTable SearchAdjustmentHistory(string AdjHistoryNo, string AdjReferance, string AdjType, string Post,
       string AdjFromDate, string AdjToDate, int BranchId = 0, SysDBInfoVMTemp connVM = null);

       DataTable SearchAdjustmentHistoryForDeposit(string AdjHistoryNo, SysDBInfoVMTemp connVM = null);

       List<AdjustmentHistoryVM> SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       List<AdjustmentHistoryVM> SelectAllCashPayable(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);



    }
}
