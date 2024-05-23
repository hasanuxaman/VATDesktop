using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATDesktop.Repo.SaleWCF;
using VATDesktop.Repo.VehicleWCF;
using VATServer.Interface;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATDesktop.Repo
{
    public class VehicleRepo : IVehicle
    {


        //VehicleDAL _dal = new VehicleDAL();

        VehicleWCFClient wcf = new VehicleWCFClient();


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

        public string[] Delete(VehicleVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

        public List<VehicleVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.DropDown(connVMwcf);

                List<VehicleVM> results = JsonConvert.DeserializeObject<List<VehicleVM>>(result);
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
        public string[] InsertToVehicle(VehicleVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.InsertToVehicle(vmwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<VehicleVM> SelectAllLst(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SelectAllLst(Id, conditionFieldswcf, conditionValueswcf,connVMwcf);

                List<VehicleVM> results = JsonConvert.DeserializeObject<List<VehicleVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateToVehicle(VehicleVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.UpdateToVehicle(vmwcf, connVMwcf);

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
