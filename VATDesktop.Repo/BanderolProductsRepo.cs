using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Interface;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATDesktop.Repo.BanderolProductsWCF;
using Newtonsoft.Json;

namespace VATDesktop.Repo
{
    public class BanderolProductsRepo : IBanderolProducts
    {

        //BanderolProductsDAL _dal = new BanderolProductsDAL();

        BanderolProductsWCFClient wcf = new BanderolProductsWCFClient();

        public string[] InsertToBanderolProducts(string BandProductId, string ItemNo, string BanderolID, string PackagingId, 
            decimal BUsedQty, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn,
            string WastageQty, decimal OpeningQty, string OpeningDate, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string resultInsert = wcf.InsertToBanderolProducts( BandProductId,  ItemNo,  BanderolID,  PackagingId, 
             BUsedQty,  ActiveStatus,  CreatedBy,  CreatedOn,  LastModifiedBy,  LastModifiedOn,
             WastageQty,  OpeningQty,  OpeningDate, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(resultInsert);

                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] UpdateBanderolProduct(string BandProductId, string ItemNo, string BanderolID, string PackagingId, 
            decimal BUsedQty, string ActiveStatus, string LastModifiedBy, string LastModifiedOn, string WastageQty,
            decimal OpeningQty, string OpeningDate, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdateBanderolProduct(BandProductId, ItemNo, BanderolID, PackagingId, BUsedQty, ActiveStatus,
              LastModifiedBy, LastModifiedOn, WastageQty, OpeningQty, OpeningDate, connVMwcf);
               string[] results = JsonConvert.DeserializeObject<string[]>(result);

               return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DeleteBanderolProduct(string BandProductId, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DeleteBanderolProduct(BandProductId, connVMwcf);
                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchBanderolProducts(string ProductName, string ProductCode, string BanderolId, string BanderolName, 
            string PackagingId, string PackagingNature, string ActiveStatus, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SearchBanderolProducts( ProductName,  ProductCode,  BanderolId,  BanderolName, 
             PackagingId,  PackagingNature,  ActiveStatus, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable SearchBanderol(string BandProductId, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf. SearchBanderol( BandProductId, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }

        }


    }
}
