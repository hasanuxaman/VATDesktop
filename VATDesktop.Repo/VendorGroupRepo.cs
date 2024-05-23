using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.VendorGroupWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
  public  class VendorGroupRepo:IVendorGroup
    {
        VendorGroupWCFClient wcf = new VendorGroupWCFClient();

        public string[] Delete(VendorGroupVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

        public List<VendorGroupVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.DropDown(connVMwcf);

                List<VendorGroupVM> results = JsonConvert.DeserializeObject<List<VendorGroupVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<VendorGroupVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.DropDownAll(connVMwcf);

                List<VendorGroupVM> results = JsonConvert.DeserializeObject<List<VendorGroupVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] InsertToVendorGroup(VendorGroupVM vm, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.InsertToVendorGroup(vmwcf, connVMwcf);

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


                string result = wcf.SelectAll(Id,conditionFieldswcf,conditionValueswcf,Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<VendorGroupVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SelectAllList(Id,conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<VendorGroupVM> results = JsonConvert.DeserializeObject<List<VendorGroupVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateToVendorGroup(VendorGroupVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.UpdateToVendorGroup(vmwcf, connVMwcf);

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
