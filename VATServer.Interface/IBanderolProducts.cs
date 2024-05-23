using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Interface
{
    public interface IBanderolProducts
    {

        string[] InsertToBanderolProducts(string BandProductId, string ItemNo, string BanderolID, string PackagingId,
            decimal BUsedQty, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn,
            string WastageQty, decimal OpeningQty, string OpeningDate, SysDBInfoVMTemp connVM = null);

        string[] UpdateBanderolProduct(string BandProductId, string ItemNo, string BanderolID, string PackagingId,
            decimal BUsedQty, string ActiveStatus, string LastModifiedBy, string LastModifiedOn, string WastageQty,
            decimal OpeningQty, string OpeningDate, SysDBInfoVMTemp connVM = null);

        string[] DeleteBanderolProduct(string BandProductId, SysDBInfoVMTemp connVM = null);

            DataTable SearchBanderolProducts(string ProductName, string ProductCode, string BanderolId, string BanderolName, 
            string PackagingId, string PackagingNature, string ActiveStatus, SysDBInfoVMTemp connVM = null);

        DataTable SearchBanderol(string BandProductId, SysDBInfoVMTemp connVM = null);






    }
}
