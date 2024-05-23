using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Interface
{
  public  interface ISettingRole
    {
        string[] SettingsUpdate(List<SettingsVM> settingsVM, SysDBInfoVMTemp connVM = null, string userName = "", string userID = "");

        DataSet SearchSettingsRole(string UserId, string UserName, SysDBInfoVMTemp connVM = null);



    }
}
