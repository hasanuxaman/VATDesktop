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
   public interface ITransferReceive
    {

       DataSet FormLoad(UOMVM UOMvm, ProductVM Productvm, string Name, SysDBInfoVMTemp connVM = null);

       string[] Insert(TransferReceiveVM Master, List<TransferReceiveDetailVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null, bool CodeGenaration = true);

        string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null);

        string[] Post(TransferReceiveVM Master, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null);

        DataTable SearchTransferDetail(string TransferReceiveNo, SysDBInfoVMTemp connVM = null);

        DataTable SearchTransferReceive(TransferReceiveVM vm, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null);

        List<TransferReceiveVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        List<TransferReceiveDetailVM> SelectDetail(string TransferReceiveNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] Update(TransferReceiveVM Master, List<TransferReceiveDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "", SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null);


    }
}
