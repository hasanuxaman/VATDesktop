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
   public interface IShift
    {
       DataTable SearchForTime(string time, SysDBInfoVMTemp connVM = null);

        string[] DeleteShiftNew(string Id, SysDBInfoVMTemp connVM = null);

        List<ShiftVM> DropDown(int branchId = 0, SysDBInfoVMTemp connVM = null);

        string[] InsertToShiftNew(ShiftVM vm, SysDBInfoVMTemp connVM = null);
       
        DataTable SearchShift(string ShiftName, int Id = 0, SysDBInfoVMTemp connVM = null);

        List<ShiftVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] UpdateToShiftNew(ShiftVM vm, SysDBInfoVMTemp connVM = null);



    }
}
