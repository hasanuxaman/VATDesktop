using Newtonsoft.Json;
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
using VATDesktop.Repo.BankInformationWCF;

namespace VATDesktop.Repo
{
    public class BankInformationRepo : IBankInformation
    {
        //BankInformationDAL _dal = new BankInformationDAL();

        BankInformationWCFClient wcf = new BankInformationWCFClient();


        public string[] InsertToBankInformation(BankInformationVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool IsIntegrationAutoCode = false)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);
                string vmwcf = JsonConvert.SerializeObject(vm);


                string result = wcf.InsertToBankInformation(vmwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        
        }

        public string[] UpdateBankInformation(BankInformationVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);
                string vmwcf = JsonConvert.SerializeObject(vm);


                string result = wcf.UpdateBankInformation(vmwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        
        }

        public string[] Delete(BankInformationVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string idswcf = JsonConvert.SerializeObject(ids);

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.Delete(vmwcf,idswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);



                string result = wcf.SelectAll( Id ,conditionFieldswcf , conditionValueswcf ,  Dt , connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string idswcf = JsonConvert.SerializeObject(ids);



                string result = wcf.GetExcelData( idswcf, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public List<BankInformationVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDownAll( connVMwcf);

                List<BankInformationVM> results = JsonConvert.DeserializeObject<List<BankInformationVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<BankInformationVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDown(connVMwcf);

                List<BankInformationVM> results = JsonConvert.DeserializeObject<List<BankInformationVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<BankInformationVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);

                string result = wcf.SelectAllList( Id,conditionFieldswcf ,conditionValueswcf,connVMwcf);

                List<BankInformationVM> results = JsonConvert.DeserializeObject<List<BankInformationVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
