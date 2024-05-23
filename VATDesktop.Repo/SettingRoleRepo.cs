using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.SettingRoleWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class SettingRoleRepo : ISettingRole
    {

        SettingRoleWCFClient wcf = new SettingRoleWCFClient();

        public string[] SettingsUpdate(List<SettingsVM> settingsVM, SysDBInfoVMTemp connVM = null, string userName = "", string userId = "")
        {
            try
            {

                string settingsVMwcf = JsonConvert.SerializeObject(settingsVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SettingsUpdate(settingsVMwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataSet SearchSettingsRole(string UserId, string UserName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SearchSettingsRole(UserId,UserName, connVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
