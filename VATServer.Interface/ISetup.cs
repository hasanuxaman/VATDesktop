using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Interface
{
    public interface ISetup
    {

        DataSet ResultVATStatus(DateTime StartDate, string databaseName, SysDBInfoVMTemp connVM = null);

        string[] InsertToSetupNew(SetupMaster setupMaster, SysDBInfoVMTemp connVM = null);

        DataTable ResultIssueBOMNew(string databaseName, SysDBInfoVMTemp connVM = null);

        DataTable SearchSetupDataTable(string databaseName, SysDBInfoVMTemp connVM = null);

        string SearchSetupNew(string databaseName, SysDBInfoVMTemp connVM = null);
            


    }
}
