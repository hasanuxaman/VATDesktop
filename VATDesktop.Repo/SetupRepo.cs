using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.SetupWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class SetupRepo : ISetup
    {
        SetupWCFClient wcf = new SetupWCFClient();

        public DataSet ResultVATStatus(DateTime StartDate, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.ResultVATStatus(StartDate, databaseName, connVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public string[] InsertToSetupNew(SetupMaster setupMaster, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string setupMasterwcf = JsonConvert.SerializeObject(setupMaster);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.InsertToSetupNew(setupMasterwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable ResultIssueBOMNew(string databaseName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.ResultIssueBOMNew(databaseName, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchSetupDataTable(string databaseName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SearchSetupDataTable(databaseName, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string SearchSetupNew(string databaseName, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SearchSetupNew(databaseName, connVMwcf);

                string results = JsonConvert.DeserializeObject<string>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
