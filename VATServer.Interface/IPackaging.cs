using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Interface
{
    public interface IPackaging
    {

        string[] DeletePackageInformation(string PackId, SysDBInfoVMTemp connVM = null);

        string[] InsertToPackage(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, SysDBInfoVMTemp connVM = null);

        DataTable SearchPackage(string PackName, string PackgeSize, string ActiveStatus, SysDBInfoVMTemp connVM = null);

        string[] UpdatePackage(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string LastModifiedBy, string LastModifiedOn, SysDBInfoVMTemp connVM = null);
        

    }
}
