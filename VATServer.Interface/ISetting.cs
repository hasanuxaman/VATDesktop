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
   public interface ISetting
    {
         List<SettingsVM> DropDownAll(SysDBInfoVMTemp connVM = null);

         List<SettingsVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

         DataSet SearchSettings(SysDBInfoVMTemp connVM = null);

         string UpdateInternalIssueValue(SysDBInfoVMTemp connVM = null, string UserId = "");

         string settingsDataInsert(string settingGroup, string settingName, string settingType, string settingValue, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);

         string[] UpdateCode(string CodeGroup, string CodeName, int LastId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

         string[] UpdateCodeGeneration(int BranchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

         string[] SettingsUpdatelist(List<SettingsVM> settingsVM, SysDBInfoVMTemp connVM = null);

         void SettingsUpdate(string companyId, int BranchId = 0, SysDBInfoVMTemp connVM = null, Version version = null);

    }
}
