using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Interface;
using VATViewModel.DTOs;
using VATDesktop.Repo.FiscalYearWCF;
using Newtonsoft.Json;


namespace VATDesktop.Repo
{
    public class FiscalYearRepo : IFiscalYear
    {

        FiscalYearWCFClient wcf = new FiscalYearWCFClient();

        public string[] FiscalYearInsert(List<FiscalYearVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.FiscalYearInsert(Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] FiscalYearUpDate(List<FiscalYearVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.FiscalYearUpDate(Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] FiscalYearUpdate(List<FiscalYearVM> Details, string modifiedBy, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.FiscalYearUpdate1(Detailswcf,modifiedBy, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int LockChek(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.LockChek( connVMwcf);

                int results = Convert.ToInt32(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string MaxDate(SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.MaxDate(connVMwcf);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchYear(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchYear(connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable LoadYear(string CurrentYear, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.LoadYear(CurrentYear,connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<FiscalYearVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);

                List<FiscalYearVM> results = JsonConvert.DeserializeObject<List<FiscalYearVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public FiscalYearPeriodVM StartEndPeriod(string year, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.StartEndPeriod(year,connVMwcf);

                FiscalYearPeriodVM results = JsonConvert.DeserializeObject<FiscalYearPeriodVM>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<FiscalYearVM> DropDownAll(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDownAll( connVMwcf);

                List<FiscalYearVM> results = JsonConvert.DeserializeObject<List<FiscalYearVM>>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    

    }
}
