using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Interface;
using VATDesktop.Repo.DisposeWCF;
using VATViewModel.DTOs;
using System.Data;
using Newtonsoft.Json;

namespace VATDesktop.Repo
{
    public class DisposeRepo : IDispose
    {
        DisposeWCFClient wcf = new DisposeWCFClient();

        public string[] DisposeInsert(DisposeMasterVM Master, List<DisposeDetailVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DisposeInsert(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] DisposePost(DisposeMasterVM Master, List<DisposeDetailVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DisposePost(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string[] DisposeUpdate(DisposeMasterVM Master, List<DisposeDetailVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Masterwcf = JsonConvert.SerializeObject(Master);
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DisposeUpdate(Masterwcf, Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchDisposeDetailDTNew(string DisposeNumber, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchDisposeDetailDTNew(DisposeNumber, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public DataTable SearchDisposeHeaderDTNew(string DisposeNumber, string DisposeDateFrom, string DisposeDateTo, string transactionType, string Post, string databasename, int BranchId, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchDisposeHeaderDTNew(DisposeNumber,DisposeDateFrom,DisposeDateTo,transactionType,  Post,  databasename, BranchId ,connVMwcf);

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
