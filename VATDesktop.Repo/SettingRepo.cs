using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.SettingWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class SettingRepo : ISetting
    {
        SettingWCFClient wcf = new SettingWCFClient();


        public List<SettingsVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DropDownAll(connVMwcf);

                List<SettingsVM> results = JsonConvert.DeserializeObject<List<SettingsVM>>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<SettingsVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<SettingsVM> results = JsonConvert.DeserializeObject<List<SettingsVM>>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] SettingsUpdatelist(List<SettingsVM> settingsVM, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string settingsVMwcf = JsonConvert.SerializeObject(settingsVM);

                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SettingsUpdatelist(settingsVMwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void SettingsUpdate(string companyId, int BranchId = 0, SysDBInfoVMTemp connVM = null, Version version = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                wcf.SettingsUpdate(companyId, BranchId, connVMwcf);



            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataSet SearchSettings(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SearchSettings(connVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string UpdateInternalIssueValue(SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.UpdateInternalIssueValue(connVMwcf);

                string results = JsonConvert.DeserializeObject<string>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string settingsDataInsert(string settingGroup, string settingName, string settingType, string settingValue, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.settingsDataInsert(settingGroup, settingName,settingType,settingValue, connVMwcf);

                string results = JsonConvert.DeserializeObject<string>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] UpdateCode(string CodeGroup, string CodeName, int LastId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.UpdateCode(CodeGroup, CodeName,LastId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] UpdateCodeGeneration(int BranchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.UpdateCodeGeneration(BranchId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
