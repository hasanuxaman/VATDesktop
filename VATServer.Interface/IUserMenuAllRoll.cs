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
   public interface IUserMenuAllRoll
    {
       DataTable UserMenuAllRollsSelectAll(string FormID = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null);

       void UserMenuAllRollsSettingChange(SysDBInfoVMTemp connVM = null);

       string[] UserMenuAllRollsUpdate(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
       string[] UserMenuAllFinalRollsUpdate(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
    }
}
