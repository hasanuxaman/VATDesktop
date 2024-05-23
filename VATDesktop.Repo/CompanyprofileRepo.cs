using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;
using VATDesktop.Repo.CompanyprofileWCF;
using VATServer.Interface;
using System.Data;

namespace VATDesktop.Repo
{
    public class CompanyprofileRepo : ICompanyprofile
    {
        CompanyprofileWCFClient wcf = new CompanyprofileWCFClient();

        public List<CompanyProfileVM> SelectAllList(string Id = null, string[] conditionFields = null
            , string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null
            , SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);
                List<CompanyProfileVM> results = JsonConvert.DeserializeObject<List<CompanyProfileVM>>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable SearchCompanyProfile(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchCompanyProfile(connVMwcf);
                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAll(Id, conditionFieldswcf, conditionValueswcf,Dt, connVMwcf);
                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] UpdateCompanyProfileNew(CompanyProfileVM companyProfiles, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string companyProfileswcf = JsonConvert.SerializeObject(companyProfiles);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UpdateCompanyProfileNew(companyProfileswcf, connVMwcf);
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
