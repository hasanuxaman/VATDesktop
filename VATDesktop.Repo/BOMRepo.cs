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
using Newtonsoft.Json;
using VATDesktop.Repo.BOMWCF;
using VATServer.Interface;


namespace VATDesktop.Repo
{
    public class BOMRepo : IBOM
    {

        //BOMDAL _dal = new BOMDAL();
        BOMWCFClient wcf = new BOMWCFClient();

        public DataTable SearchBOMMasterNew(string BOMId, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SearchBOMMasterNew(BOMId, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        
        
        }

        public DataTable SearchBOMRawNew(string BOMId, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SearchBOMRawNew(BOMId, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        
        }

        public DataTable SearchOHNew(string BOMId, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);


                string result = wcf.SearchOHNew(BOMId, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public string[] BOMInsert(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string bomItemswcf= JsonConvert.SerializeObject(bomItems);
                string bomOHswcf= JsonConvert.SerializeObject(bomOHs);
                string bomMasterwcf= JsonConvert.SerializeObject(bomMaster);
                string connVMwcf = JsonConvert.SerializeObject(connVM);
                
                string result = wcf.BOMInsert( bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        
        }

        public string[] BOMUpdate(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string bomItemswcf = JsonConvert.SerializeObject(bomItems);
                string bomOHswcf = JsonConvert.SerializeObject(bomOHs);
                string bomMasterwcf = JsonConvert.SerializeObject(bomMaster);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.BOMUpdate(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        
        }

        public string[] DeleteBOM(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {

            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DeleteBOM( itemNo,  VatName,  effectDate, CustomerID, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public string[] BOMPost(BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null, SqlTransaction vtransaction = null, SqlConnection vcurrConn = null)
        {
            try
            {
                string bomMasterwcf = JsonConvert.SerializeObject(bomMaster);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.BOMPost(bomMasterwcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        
        }

        public string[] FindCostingFrom(string itemNo, string effectDate, SqlConnection curConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.FindCostingFrom( itemNo,  effectDate, CustomerID, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable SelectAll(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, 
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, BOMNBRVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string likeVMwcf = JsonConvert.SerializeObject(likeVM);


                string result = wcf.SelectAll( BOMId , conditionFieldswcf , conditionValueswcf ,  likeVMwcf , Dt, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<BOMNBRVM> SelectAllList(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, BOMNBRVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string likeVMwcf = JsonConvert.SerializeObject(likeVM);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAllList(BOMId, conditionFieldswcf, conditionValueswcf, likeVMwcf, connVMwcf);
                List<BOMNBRVM> results = JsonConvert.DeserializeObject<List<BOMNBRVM>>(result);
                return results;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public DataTable SearchBOMMaster(string finItem, string vatName, string effectDate, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchBOMMaster( finItem,  vatName,  effectDate,  CustomerID, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchBOMRaw(string finItem, string vatName, string effectDate, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchBOMRaw(finItem, vatName, effectDate, CustomerID, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchOH(string finItem, string vatName, string effectDate, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchOH(finItem, vatName, effectDate, CustomerID, connVMwcf);

                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] ServiceInsert(List<BOMNBRVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ServiceInsert(Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] ServiceUpdate(List<BOMNBRVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ServiceUpdate(Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] ServicePost(List<BOMNBRVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ServicePost(Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] ServiceDelete(List<BOMNBRVM> Details, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string Detailswcf = JsonConvert.SerializeObject(Details);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.ServiceDelete(Detailswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable SearchInputValues(string FinishItemName, string EffectDate, string VATName, string post,
                                           string FinishItemNo, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchInputValues( FinishItemName,  EffectDate,  VATName,  post,
                                            FinishItemNo, connVMwcf);


                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable SearchServicePrice(string BOMId, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchServicePrice(BOMId, connVMwcf);


                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable CustomerByBomId(string BOMId, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.CustomerByBomId(BOMId, connVMwcf);


                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable UseQuantityDT(string FinishItemNo, decimal Quantity, string EffectDate, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.UseQuantityDT( FinishItemNo,  Quantity,  EffectDate, CustomerID, connVMwcf);


                DataTable results = JsonConvert.DeserializeObject<DataTable>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<BOMItemVM> SelectAllItems(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAllItems( BOMId, conditionFieldswcf , conditionValueswcf, connVMwcf);


                List<BOMItemVM> results = JsonConvert.DeserializeObject<List<BOMItemVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<BOMOHVM> SelectAllOverheads(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAllOverheads(BOMId, conditionFieldswcf, conditionValueswcf, connVMwcf);


                List<BOMOHVM> results = JsonConvert.DeserializeObject<List<BOMOHVM>>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] BOMPreInsert(BOMNBRVM vm, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string vmwcf = JsonConvert.SerializeObject(vm);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.BOMPreInsert(vmwcf, connVMwcf);


                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindBOMIDOverHead(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.FindBOMIDOverHead( itemNo,  VatName,  effectDate, CustomerID, connVMwcf);

                return result;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] BOMImport(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string bomItemswcf = JsonConvert.SerializeObject(bomItems);
                string bomOHswcf = JsonConvert.SerializeObject(bomOHs);
                string bomMasterwcf = JsonConvert.SerializeObject(bomMaster);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.BOMImport(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);


                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] BOMInsertX(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string bomItemswcf = JsonConvert.SerializeObject(bomItems);
                string bomOHswcf = JsonConvert.SerializeObject(bomOHs);
                string bomMasterwcf = JsonConvert.SerializeObject(bomMaster);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.BOMInsertX(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);


                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] BOMInsert2(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SqlTransaction transaction = null, SqlConnection currConn = null, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string bomItemswcf = JsonConvert.SerializeObject(bomItems);
                string bomOHswcf = JsonConvert.SerializeObject(bomOHs);
                string bomMasterwcf = JsonConvert.SerializeObject(bomMaster);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.BOMInsert2(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);


                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] BOMUpdateX(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string bomItemswcf = JsonConvert.SerializeObject(bomItems);
                string bomOHswcf = JsonConvert.SerializeObject(bomOHs);
                string bomMasterwcf = JsonConvert.SerializeObject(bomMaster);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.BOMUpdateX(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);


                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] BOMPostX(BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string bomMasterwcf = JsonConvert.SerializeObject(bomMaster);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.BOMPostX(bomMasterwcf, connVMwcf);


                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string FindBOMID(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.FindBOMID( itemNo,  VatName,  effectDate, CustomerID, connVMwcf);

                return result;


            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
