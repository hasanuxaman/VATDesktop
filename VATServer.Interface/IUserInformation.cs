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
   public interface IUserInformation
    {
       List<UserInformationVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

       List<UserInformationVM> DropDown(SysDBInfoVMTemp connVM = null);
       DataTable GetExcelData(List<string> ids, SqlConnection Vcon = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
       string[] InsertToUserInformationNew(UserInformationVM vm, SysDBInfoVMTemp connVM = null);
       string[] InsertToUserRoll(List<UserRollVM> userRollVMs, string databaseName, SysDBInfoVMTemp connVM = null);
       string[] InsertUserInformation(string UserID, string UserName, string UserPassword, string FullName, string Designation, string ContactNo, string Email, string Address, string ActiveStatus, string IsMainSettings, string LastLoginDateTime, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string databaseName, SysDBInfoVMTemp connVM = null, string NationalID = null, byte[] bimage=null);
       string InsertUserLogin(List<UserLogsVM> Details, string LogOutDateTime, SysDBInfoVMTemp connVM = null);
       string InsertUserLogOut(string LogID, string LogOutDateTime, SysDBInfoVMTemp connVM = null);
       string InsertUserLogOutByList(List<UserLogsVM> Details, string LogOutDateTime, SysDBInfoVMTemp connVM = null);
       DataTable SearchUserDataTable(string UserID, string UserName, string ActiveStatus, string databaseName, SysDBInfoVMTemp connVM = null);
       DataTable SearchUserHas(string UserName, SysDBInfoVMTemp connVM = null, string password = "");
       DataTable SearchUserHasNew(string UserName, string databaseName, SysDBInfoVMTemp connVM = null);
       DataTable SearchUserLog(string ComputerLoginUserName, string SoftwareUserName, string ComputerName, string StartDate, string EndDate, SysDBInfoVMTemp connVM = null);
       string SearchUserRoll(string UserID, SysDBInfoVMTemp connVM = null);
       List<UserInformationVM> SelectForLogin(LoginVM Logvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);
       string[] UpdateUserInformation(string UserID, string UserName, string FullName, string Designation, string ContactNo, string Email, string Address, string ActiveStatus, string IsMainSettings, string LastModifiedBy, string LastModifiedOn, string databaseName, SysDBInfoVMTemp connVM = null, string NationalID = null, bool IsLock = false, byte[] bimage = null);
       string[] UpdateUserPasswordNew(string UserName, string UserPassword, string LastModifiedBy, string LastModifiedOn, string databaseName, SysDBInfoVMTemp connVM = null);



    }
}
