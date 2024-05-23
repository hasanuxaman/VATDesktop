using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.UserInformationWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class UserInformationRepo : IUserInformation
    {
       UserInformationWCFClient wcf = new UserInformationWCFClient();

        public List<UserInformationVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf,connVMwcf);
                List<UserInformationVM> results = JsonConvert.DeserializeObject<List<UserInformationVM>>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<UserInformationVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            try
            {
              
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.DropDown(connVMwcf);

                List<UserInformationVM> results = JsonConvert.DeserializeObject<List<UserInformationVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable GetExcelData(List<string> ids, SqlConnection Vcon = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string idswcf = JsonConvert.SerializeObject(ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.GetExcelData(idswcf, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertToUserInformationNew(UserInformationVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.InsertToUserInformationNew(vmwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertToUserRoll(List<UserRollVM> userRollVMs, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(userRollVMs);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.InsertToUserRoll(vmwcf,databaseName, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertUserInformation(string UserID, string UserName, string UserPassword, string FullName, string Designation, string ContactNo, string Email, string Address, string ActiveStatus, string IsMainSettings, string LastLoginDateTime, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string databaseName, SysDBInfoVMTemp connVM = null, string NationalID = null, byte[] bimage=null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.InsertUserInformation(UserID,UserName,UserPassword,FullName,Designation,ContactNo,Address,ActiveStatus,
                    LastLoginDateTime, CreatedBy, CreatedOn, LastModifiedBy, LastModifiedOn, databaseName, connVMwcf, IsMainSettings, Email, NationalID);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string InsertUserLogin(List<UserLogsVM> Details, string LogOutDateTime, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.InsertToUserRoll(vmwcf, LogOutDateTime, connVMwcf);

                string results = JsonConvert.DeserializeObject<string>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string InsertUserLogOut(string LogID, string LogOutDateTime, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.InsertUserLogOut(LogID, LogOutDateTime, connVMwcf);

                string results = JsonConvert.DeserializeObject<string>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string InsertUserLogOutByList(List<UserLogsVM> Details, string LogOutDateTime, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.InsertUserLogOutByList(vmwcf, LogOutDateTime, connVMwcf);

                string results = JsonConvert.DeserializeObject<string>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchUserDataTable(string UserID, string UserName, string ActiveStatus, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SearchUserDataTable(UserID, UserName,ActiveStatus,databaseName, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchUserHas(string UserName, SysDBInfoVMTemp connVM = null, string password = "")
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SearchUserHas(UserName, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchUserHasNew(string UserName, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SearchUserHasNew(UserName,databaseName, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchUserLog(string ComputerLoginUserName, string SoftwareUserName, string ComputerName, string StartDate, string EndDate, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SearchUserLog(ComputerLoginUserName, SoftwareUserName,ComputerName,StartDate,EndDate, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string SearchUserRoll(string UserID, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SearchUserRoll(UserID, connVMwcf);

                string results = JsonConvert.DeserializeObject<string>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<UserInformationVM> SelectForLogin(LoginVM Logvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string Logvmwvf = JsonConvert.SerializeObject(Logvm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SelectForLogin(Logvmwvf,connVMwcf);

                List<UserInformationVM> results = JsonConvert.DeserializeObject<List<UserInformationVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateUserInformation(string UserID, string UserName, string FullName, string Designation, string ContactNo, string Email, string Address, string ActiveStatus, string IsMainSettings, string LastModifiedBy, string LastModifiedOn, string databaseName, SysDBInfoVMTemp connVM = null, string NationalID = null, bool IsLock = false, byte[] bimage = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.UpdateUserInformation( UserID,UserName,FullName,Designation ,ContactNo,Address
             , ActiveStatus, LastModifiedBy, LastModifiedOn, databaseName, connVMwcf, IsMainSettings, NationalID, Email);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateUserPasswordNew(string UserName, string UserPassword, string LastModifiedBy, string LastModifiedOn, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.UpdateUserPasswordNew(UserName, UserPassword, LastModifiedBy, LastModifiedOn, databaseName, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
