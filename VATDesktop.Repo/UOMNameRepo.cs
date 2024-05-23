using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.UOMNameWCF;
using VATServer.Interface;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class UOMNameRepo : IUOMName
    {
       UOMNameWCFClient wcf = new UOMNameWCFClient();

        public string[] Delete(UOMNameVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
        public List<UOMNameVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.DropDown(connVMwcf);

                List<UOMNameVM> results = JsonConvert.DeserializeObject<List<UOMNameVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<UOMNameVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            try
            {

                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.DropDownAll(connVMwcf);

                List<UOMNameVM> results = JsonConvert.DeserializeObject<List<UOMNameVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertToUOMName(UOMNameVM vm, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.InsertToUOMName(vmwcf, connVMwcf);

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
        public List<UOMNameVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<UOMNameVM> results = JsonConvert.DeserializeObject<List<UOMNameVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] UpdateUOMName(UOMNameVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.UpdateUOMName(vmwcf, connVMwcf);

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
