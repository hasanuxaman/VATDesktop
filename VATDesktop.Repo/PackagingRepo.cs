using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Interface;
using VATViewModel.DTOs;
using VATDesktop.Repo.PackagingWCF;
using Newtonsoft.Json;

namespace VATDesktop.Repo
{
    public class PackagingRepo : IPackaging
    {

        PackagingWCFClient wcf = new PackagingWCFClient();


        public string[] DeletePackageInformation(string PackId, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DeletePackageInformation(PackId, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertToPackage(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.InsertToPackage(PackID,  PackName,  PackSize,  Uom,  Description,  ActiveStatus,  CreatedBy,  CreatedOn,  LastModifiedBy,  LastModifiedOn, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchPackage(string PackName, string PackgeSize, string ActiveStatus, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchPackage(PackName, PackgeSize,ActiveStatus, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdatePackage(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string LastModifiedBy, string LastModifiedOn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdatePackage(PackID,  PackName,  PackSize,  Uom,  Description,  ActiveStatus,  LastModifiedBy,  LastModifiedOn, connVMwcf);

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
