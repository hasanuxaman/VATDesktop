using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Interface;
using VATDesktop.Repo.ShiftWCF;
using System.Data;
using VATViewModel.DTOs;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace VATDesktop.Repo
{
    public class ShiftRepo : IShift
    {


        ShiftWCFClient wcf = new ShiftWCFClient();

        public DataTable SearchForTime(string time, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SearchForTime(time, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] DeleteShiftNew(string Id, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DeleteShiftNew(Id, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<ShiftVM> DropDown(int branchId = 0, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.DropDown(branchId, connVMwcf);

                List<ShiftVM> results = JsonConvert.DeserializeObject<List<ShiftVM>>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] InsertToShiftNew(ShiftVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.InsertToShiftNew(vmwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchShift(string ShiftName, int Id = 0, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SearchShift(ShiftName, Id, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<ShiftVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.SelectAll(Id,conditionFieldswcf,conditionValueswcf, connVMwcf);

                List<ShiftVM> results = JsonConvert.DeserializeObject<List<ShiftVM>>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateToShiftNew(ShiftVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string table = wcf.UpdateToShiftNew(vmwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(table);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
