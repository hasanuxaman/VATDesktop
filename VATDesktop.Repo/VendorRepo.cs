using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.VendorWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
   public class VendorRepo:IVendor
    {

        VendorWCFClient wcf = new VendorWCFClient();

        public string[] Delete(VendorVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string idswcf = JsonConvert.SerializeObject(ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.Delete(vmwcf, idswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<VendorVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.DropDown(connVMwcf);

                List<VendorVM> results = JsonConvert.DeserializeObject<List<VendorVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<VendorVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.DropDownAll(connVMwcf);

                List<VendorVM> results = JsonConvert.DeserializeObject<List<VendorVM>>(result);
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

                string idswcf = JsonConvert.SerializeObject(ids);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.GetExcelData(idswcf,connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public string[] InsertToVendorNewSQL(VendorVM vm, bool autoSave = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.InsertToVendorNewSQL(vmwcf, autoSave, connVMwcf);

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
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf, Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<VendorVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<VendorVM> results = JsonConvert.DeserializeObject<List<VendorVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateVendorNewSQL(VendorVM vm, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.UpdateVendorNewSQL(vmwcf, connVMwcf);

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
