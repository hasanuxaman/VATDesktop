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
    public interface IMPLBankPaymentReceive
    {

        string[] InsertToMPLBankPaymentReceive(MPLBankPaymentReceiveVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        string[] UpdateMPLBankPaymentReceive(MPLBankPaymentReceiveVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string Orderby = "Y", string SelectTop = "100");

        List<MPLBankPaymentReceiveVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string Orderby = "Y", string SelectTop = "100");

        List<MPLBankPaymentReceiveVM> DropDown(SysDBInfoVMTemp connVM = null);

        string[] Delete(MPLBankPaymentReceiveVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

    }
}
