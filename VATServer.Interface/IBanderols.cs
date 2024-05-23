using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Interface
{
    public interface IBanderols
    {

        string[] InsertToBanderol(string BanderolID, string BanderolName, string BanderolSize, string Uom, string Description,
            string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, SysDBInfoVMTemp connVM = null);

        string[] UpdateBanderol(string BanderolID, string BanderolName, string BanderolSize, string Uom, string Description,
            string ActiveStatus, string LastModifiedBy, string LastModifiedOn, SysDBInfoVMTemp connVM = null);

        string[] DeleteBanderolInformation(string BanderolID, SysDBInfoVMTemp connVM = null);

        DataTable SearchBanderols(string BanderolName, string BandeSize, string OpeningDateFrom, string OpeningDateTo,
        string ActiveStatus, SysDBInfoVMTemp connVM = null);



    }
}
