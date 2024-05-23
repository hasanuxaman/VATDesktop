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
    public interface ISaleMPL
    {

        string[] SalesMPLInsert(SalesInvoiceMPLHeaderVM MasterVM,SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        string[] SalesMPLUpdate(SalesInvoiceMPLHeaderVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);
        
        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SalesInvoiceMPLHeaderVM likeVM = null,
            bool Dt = false, string TransectionType = null, string MultipleSearch = "N", SysDBInfoVMTemp connVM = null, string Orderby = "Y", string[] ids = null, string Is6_3TollCompleted = null, string IsVDSCompleted = null);

        List<SalesInvoiceMPLHeaderVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SalesInvoiceMPLHeaderVM likeVM = null,
            SysDBInfoVMTemp connVM = null, string transactionType = null, string Orderby = "Y", string[] ids = null, string Is6_3TollCompleted = null);

        List<SalesInvoiceMPLDetailVM> SearchSaleMPLDetailList(string salesInvoiceMPLHeaderId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        List<SalesInvoiceMPLBankPaymentVM> SearchSaleMPLBankPaymentList(string salesInvoiceMPLHeaderId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        List<SalesInvoiceMPLCRInfoVM> SearchSaleMPLCRInfoList(string salesInvoiceMPLHeaderId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        List<SalesInvoiceMPLHeaderVM> DropDown(SysDBInfoVMTemp connVM = null);

        string[] Delete(SalesInvoiceMPLHeaderVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] UpdateSalesDetailsTankInfo(SalesInvoiceMPLDetailVM details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        string[] SaleMPLPost(SalesInvoiceMPLHeaderVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);
    }
}
