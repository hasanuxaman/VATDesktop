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
   public interface ITender
    {
       DataTable SearchTenderDetailSaleNew(string TenderId, string transactiondate, SysDBInfoVMTemp connVM = null);

        DataTable SearchTenderDetailSale(string TenderId, string transactiondate, SysDBInfoVMTemp connVM = null);

        string[] ImportData(DataTable dtTenderM, DataTable dtTenderD, SysDBInfoVMTemp connVM = null);

        DataTable SearchTenderDetail(string TenderId, string transactionDate, SysDBInfoVMTemp connVM = null);

        DataTable SearchTenderHeaderByCustomerGrp(string TenderId, string RefNo, string CustomerGrpID, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null);

        List<TenderDetailVM> SelectAllDetails(string tenderId = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        List<TenderMasterVM> SelectAllList(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] TenderInsert(TenderMasterVM Master, List<TenderDetailVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);

        string[] TenderUpdate(TenderMasterVM Master, List<TenderDetailVM> Details, SysDBInfoVMTemp connVM = null);

       
    }
}
