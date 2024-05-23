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
    public interface ITDSs
    {
        DataTable CurrentTDSAmount(string PurchaseInvoiceNo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null
            , bool Dt = true, SysDBInfoVMTemp connVM = null);

        DataSet TDSAmount(string VendorID, string ReceiveDate, string TDSCode, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);

        string[] UpdatePurchaseTDSs(string Id, decimal TDSAmount, SysDBInfoVMTemp connVM = null);

        string[] Delete(TDSsVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] InsertToTDSsNew(TDSsVM vm, bool BranchProfileAutoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);

        List<TDSsVM> SelectAllList(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] UpdateToTDSsNew(TDSsVM vm, SysDBInfoVMTemp connVM = null);


    }
}
