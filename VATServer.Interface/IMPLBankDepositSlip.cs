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
    public interface IMPLBankDepositSlip
    {

        string[] InsertToMPLBankDepositSlip(MPLBankDepositSlipHeaderVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        string[] UpdateMPLBankDepositSlip(MPLBankDepositSlipHeaderVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100");

        List<MPLBankDepositSlipHeaderVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100");

        List<MPLBankDepositSlipDetailVM> SearchMPLBankDepositSlipDetailList(string bankDepositSlipHeaderId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        List<MPLBankDepositSlipDetailVM> GetMPLBankDepositSlipDetailList(MPLBankDepositSlipHeaderVM MasterVM, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] Delete(MPLBankDepositSlipHeaderVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        List<MPLBankDepositSlipDetailVM> DropDown(SysDBInfoVMTemp connVM = null);

    }
}
