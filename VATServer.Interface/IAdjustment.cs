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
    public interface IAdjustment
    {


        string[] DeleteAdjustmentName(string AdjId, SysDBInfoVMTemp connVM = null);

        List<AdjustmentVM> DropDown( SysDBInfoVMTemp connVM=null);
        string[] InsertAdjustmentName(AdjustmentVM vm, SysDBInfoVMTemp connVM = null);
        string[] UpdateAdjustmentName(AdjustmentVM vm, SysDBInfoVMTemp connVM = null);
        List<AdjustmentVM> SelectAllList(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null);









    }
}
