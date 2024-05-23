using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATDesktop.Repo.BanderolsWCF;
using Newtonsoft.Json;
using VATServer.Interface;

namespace VATDesktop.Repo
{
    public class BanderolsRepo : IBanderols
    {

        //BanderolsDAL _dal = new BanderolsDAL();

        BanderolsWCFClient wcf = new BanderolsWCFClient();

        public string[] InsertToBanderol(string BanderolID, string BanderolName, string BanderolSize, string Uom, string Description, 
            string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.InsertToBanderol( BanderolID,  BanderolName,  BanderolSize,  Uom,  Description, 
             ActiveStatus,  CreatedBy,  CreatedOn,  LastModifiedBy,  LastModifiedOn, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);


                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] UpdateBanderol(string BanderolID, string BanderolName, string BanderolSize, string Uom, string Description, 
            string ActiveStatus, string LastModifiedBy, string LastModifiedOn, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdateBanderol( BanderolID,  BanderolName,  BanderolSize,  Uom,  Description, 
             ActiveStatus,  LastModifiedBy,  LastModifiedOn, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);


                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DeleteBanderolInformation(string BanderolID, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DeleteBanderolInformation( BanderolID, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);


                return results;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable SearchBanderols(string BanderolName, string BandeSize, string OpeningDateFrom, string OpeningDateTo, 
            string ActiveStatus, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SearchBanderols( BanderolName, BandeSize,OpeningDateFrom, OpeningDateTo, ActiveStatus, connVMwcf);

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
