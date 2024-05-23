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
   public interface IUserMenuRolls
    {
        string UserMenuRollsInsert(string FormID, string UserID, string FormName, string Access, string PostAccess, string AddAccess, string EditAccess, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

        string[] UserMenuRollsInsertByUser(string UserID, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        DataTable UserMenuRollsSelectAll(string UserID, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null);

        string[] UserMenuRollsUpdate(DataTable dt, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

    }
}
