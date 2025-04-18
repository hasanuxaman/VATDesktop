﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VATDesktop.Repo.BOMWCF {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BOMWCF.IBOMWCF")]
    public interface IBOMWCF {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/DoWork", ReplyAction="http://tempuri.org/IBOMWCF/DoWorkResponse")]
        void DoWork();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/DoWork", ReplyAction="http://tempuri.org/IBOMWCF/DoWorkResponse")]
        System.Threading.Tasks.Task DoWorkAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchBOMMasterNew", ReplyAction="http://tempuri.org/IBOMWCF/SearchBOMMasterNewResponse")]
        string SearchBOMMasterNew(string BOMId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchBOMMasterNew", ReplyAction="http://tempuri.org/IBOMWCF/SearchBOMMasterNewResponse")]
        System.Threading.Tasks.Task<string> SearchBOMMasterNewAsync(string BOMId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchBOMRawNew", ReplyAction="http://tempuri.org/IBOMWCF/SearchBOMRawNewResponse")]
        string SearchBOMRawNew(string BOMId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchBOMRawNew", ReplyAction="http://tempuri.org/IBOMWCF/SearchBOMRawNewResponse")]
        System.Threading.Tasks.Task<string> SearchBOMRawNewAsync(string BOMId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchOHNew", ReplyAction="http://tempuri.org/IBOMWCF/SearchOHNewResponse")]
        string SearchOHNew(string BOMId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchOHNew", ReplyAction="http://tempuri.org/IBOMWCF/SearchOHNewResponse")]
        System.Threading.Tasks.Task<string> SearchOHNewAsync(string BOMId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMInsert", ReplyAction="http://tempuri.org/IBOMWCF/BOMInsertResponse")]
        string BOMInsert(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMInsert", ReplyAction="http://tempuri.org/IBOMWCF/BOMInsertResponse")]
        System.Threading.Tasks.Task<string> BOMInsertAsync(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMUpdate", ReplyAction="http://tempuri.org/IBOMWCF/BOMUpdateResponse")]
        string BOMUpdate(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMUpdate", ReplyAction="http://tempuri.org/IBOMWCF/BOMUpdateResponse")]
        System.Threading.Tasks.Task<string> BOMUpdateAsync(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/DeleteBOM", ReplyAction="http://tempuri.org/IBOMWCF/DeleteBOMResponse")]
        string DeleteBOM(string itemNo, string VatName, string effectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/DeleteBOM", ReplyAction="http://tempuri.org/IBOMWCF/DeleteBOMResponse")]
        System.Threading.Tasks.Task<string> DeleteBOMAsync(string itemNo, string VatName, string effectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMPost", ReplyAction="http://tempuri.org/IBOMWCF/BOMPostResponse")]
        string BOMPost(string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMPost", ReplyAction="http://tempuri.org/IBOMWCF/BOMPostResponse")]
        System.Threading.Tasks.Task<string> BOMPostAsync(string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/FindCostingFrom", ReplyAction="http://tempuri.org/IBOMWCF/FindCostingFromResponse")]
        string FindCostingFrom(string itemNo, string effectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/FindCostingFrom", ReplyAction="http://tempuri.org/IBOMWCF/FindCostingFromResponse")]
        System.Threading.Tasks.Task<string> FindCostingFromAsync(string itemNo, string effectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SelectAll", ReplyAction="http://tempuri.org/IBOMWCF/SelectAllResponse")]
        string SelectAll(string BOMId, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, bool Dt, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SelectAll", ReplyAction="http://tempuri.org/IBOMWCF/SelectAllResponse")]
        System.Threading.Tasks.Task<string> SelectAllAsync(string BOMId, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, bool Dt, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SelectAllList", ReplyAction="http://tempuri.org/IBOMWCF/SelectAllListResponse")]
        string SelectAllList(string BOMId, string conditionField, string conditionValue, string likeVMwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SelectAllList", ReplyAction="http://tempuri.org/IBOMWCF/SelectAllListResponse")]
        System.Threading.Tasks.Task<string> SelectAllListAsync(string BOMId, string conditionField, string conditionValue, string likeVMwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchBOMMaster", ReplyAction="http://tempuri.org/IBOMWCF/SearchBOMMasterResponse")]
        string SearchBOMMaster(string finItem, string vatName, string effectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchBOMMaster", ReplyAction="http://tempuri.org/IBOMWCF/SearchBOMMasterResponse")]
        System.Threading.Tasks.Task<string> SearchBOMMasterAsync(string finItem, string vatName, string effectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchBOMRaw", ReplyAction="http://tempuri.org/IBOMWCF/SearchBOMRawResponse")]
        string SearchBOMRaw(string finItem, string vatName, string effectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchBOMRaw", ReplyAction="http://tempuri.org/IBOMWCF/SearchBOMRawResponse")]
        System.Threading.Tasks.Task<string> SearchBOMRawAsync(string finItem, string vatName, string effectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchOH", ReplyAction="http://tempuri.org/IBOMWCF/SearchOHResponse")]
        string SearchOH(string finItem, string vatName, string effectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchOH", ReplyAction="http://tempuri.org/IBOMWCF/SearchOHResponse")]
        System.Threading.Tasks.Task<string> SearchOHAsync(string finItem, string vatName, string effectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/ServiceInsert", ReplyAction="http://tempuri.org/IBOMWCF/ServiceInsertResponse")]
        string ServiceInsert(string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/ServiceInsert", ReplyAction="http://tempuri.org/IBOMWCF/ServiceInsertResponse")]
        System.Threading.Tasks.Task<string> ServiceInsertAsync(string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/ServiceUpdate", ReplyAction="http://tempuri.org/IBOMWCF/ServiceUpdateResponse")]
        string ServiceUpdate(string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/ServiceUpdate", ReplyAction="http://tempuri.org/IBOMWCF/ServiceUpdateResponse")]
        System.Threading.Tasks.Task<string> ServiceUpdateAsync(string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/ServicePost", ReplyAction="http://tempuri.org/IBOMWCF/ServicePostResponse")]
        string ServicePost(string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/ServicePost", ReplyAction="http://tempuri.org/IBOMWCF/ServicePostResponse")]
        System.Threading.Tasks.Task<string> ServicePostAsync(string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/ServiceDelete", ReplyAction="http://tempuri.org/IBOMWCF/ServiceDeleteResponse")]
        string ServiceDelete(string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/ServiceDelete", ReplyAction="http://tempuri.org/IBOMWCF/ServiceDeleteResponse")]
        System.Threading.Tasks.Task<string> ServiceDeleteAsync(string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchInputValues", ReplyAction="http://tempuri.org/IBOMWCF/SearchInputValuesResponse")]
        string SearchInputValues(string FinishItemName, string EffectDate, string VATName, string post, string FinishItemNo, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchInputValues", ReplyAction="http://tempuri.org/IBOMWCF/SearchInputValuesResponse")]
        System.Threading.Tasks.Task<string> SearchInputValuesAsync(string FinishItemName, string EffectDate, string VATName, string post, string FinishItemNo, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchServicePrice", ReplyAction="http://tempuri.org/IBOMWCF/SearchServicePriceResponse")]
        string SearchServicePrice(string BOMId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SearchServicePrice", ReplyAction="http://tempuri.org/IBOMWCF/SearchServicePriceResponse")]
        System.Threading.Tasks.Task<string> SearchServicePriceAsync(string BOMId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/CustomerByBomId", ReplyAction="http://tempuri.org/IBOMWCF/CustomerByBomIdResponse")]
        string CustomerByBomId(string BOMId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/CustomerByBomId", ReplyAction="http://tempuri.org/IBOMWCF/CustomerByBomIdResponse")]
        System.Threading.Tasks.Task<string> CustomerByBomIdAsync(string BOMId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/UseQuantityDT", ReplyAction="http://tempuri.org/IBOMWCF/UseQuantityDTResponse")]
        string UseQuantityDT(string FinishItemNo, decimal Quantity, string EffectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/UseQuantityDT", ReplyAction="http://tempuri.org/IBOMWCF/UseQuantityDTResponse")]
        System.Threading.Tasks.Task<string> UseQuantityDTAsync(string FinishItemNo, decimal Quantity, string EffectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SelectAllItems", ReplyAction="http://tempuri.org/IBOMWCF/SelectAllItemsResponse")]
        string SelectAllItems(string BOMId, string conditionFieldswcf, string conditionValueswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SelectAllItems", ReplyAction="http://tempuri.org/IBOMWCF/SelectAllItemsResponse")]
        System.Threading.Tasks.Task<string> SelectAllItemsAsync(string BOMId, string conditionFieldswcf, string conditionValueswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SelectAllOverheads", ReplyAction="http://tempuri.org/IBOMWCF/SelectAllOverheadsResponse")]
        string SelectAllOverheads(string BOMId, string conditionFieldswcf, string conditionValueswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/SelectAllOverheads", ReplyAction="http://tempuri.org/IBOMWCF/SelectAllOverheadsResponse")]
        System.Threading.Tasks.Task<string> SelectAllOverheadsAsync(string BOMId, string conditionFieldswcf, string conditionValueswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMPreInsert", ReplyAction="http://tempuri.org/IBOMWCF/BOMPreInsertResponse")]
        string BOMPreInsert(string vmwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMPreInsert", ReplyAction="http://tempuri.org/IBOMWCF/BOMPreInsertResponse")]
        System.Threading.Tasks.Task<string> BOMPreInsertAsync(string vmwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/FindBOMIDOverHead", ReplyAction="http://tempuri.org/IBOMWCF/FindBOMIDOverHeadResponse")]
        string FindBOMIDOverHead(string itemNo, string VatName, string effectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/FindBOMIDOverHead", ReplyAction="http://tempuri.org/IBOMWCF/FindBOMIDOverHeadResponse")]
        System.Threading.Tasks.Task<string> FindBOMIDOverHeadAsync(string itemNo, string VatName, string effectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMImport", ReplyAction="http://tempuri.org/IBOMWCF/BOMImportResponse")]
        string BOMImport(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMImport", ReplyAction="http://tempuri.org/IBOMWCF/BOMImportResponse")]
        System.Threading.Tasks.Task<string> BOMImportAsync(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMInsertX", ReplyAction="http://tempuri.org/IBOMWCF/BOMInsertXResponse")]
        string BOMInsertX(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMInsertX", ReplyAction="http://tempuri.org/IBOMWCF/BOMInsertXResponse")]
        System.Threading.Tasks.Task<string> BOMInsertXAsync(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMInsert2", ReplyAction="http://tempuri.org/IBOMWCF/BOMInsert2Response")]
        string BOMInsert2(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMInsert2", ReplyAction="http://tempuri.org/IBOMWCF/BOMInsert2Response")]
        System.Threading.Tasks.Task<string> BOMInsert2Async(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMUpdateX", ReplyAction="http://tempuri.org/IBOMWCF/BOMUpdateXResponse")]
        string BOMUpdateX(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMUpdateX", ReplyAction="http://tempuri.org/IBOMWCF/BOMUpdateXResponse")]
        System.Threading.Tasks.Task<string> BOMUpdateXAsync(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMPostX", ReplyAction="http://tempuri.org/IBOMWCF/BOMPostXResponse")]
        string BOMPostX(string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/BOMPostX", ReplyAction="http://tempuri.org/IBOMWCF/BOMPostXResponse")]
        System.Threading.Tasks.Task<string> BOMPostXAsync(string bomMasterwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/FindBOMID", ReplyAction="http://tempuri.org/IBOMWCF/FindBOMIDResponse")]
        string FindBOMID(string itemNo, string VatName, string effectDate, string CustomerID, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBOMWCF/FindBOMID", ReplyAction="http://tempuri.org/IBOMWCF/FindBOMIDResponse")]
        System.Threading.Tasks.Task<string> FindBOMIDAsync(string itemNo, string VatName, string effectDate, string CustomerID, string connVMwcf);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBOMWCFChannel : VATDesktop.Repo.BOMWCF.IBOMWCF, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BOMWCFClient : System.ServiceModel.ClientBase<VATDesktop.Repo.BOMWCF.IBOMWCF>, VATDesktop.Repo.BOMWCF.IBOMWCF {
        
        public BOMWCFClient() {
        }
        
        public BOMWCFClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public BOMWCFClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BOMWCFClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BOMWCFClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void DoWork() {
            base.Channel.DoWork();
        }
        
        public System.Threading.Tasks.Task DoWorkAsync() {
            return base.Channel.DoWorkAsync();
        }
        
        public string SearchBOMMasterNew(string BOMId, string connVMwcf) {
            return base.Channel.SearchBOMMasterNew(BOMId, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchBOMMasterNewAsync(string BOMId, string connVMwcf) {
            return base.Channel.SearchBOMMasterNewAsync(BOMId, connVMwcf);
        }
        
        public string SearchBOMRawNew(string BOMId, string connVMwcf) {
            return base.Channel.SearchBOMRawNew(BOMId, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchBOMRawNewAsync(string BOMId, string connVMwcf) {
            return base.Channel.SearchBOMRawNewAsync(BOMId, connVMwcf);
        }
        
        public string SearchOHNew(string BOMId, string connVMwcf) {
            return base.Channel.SearchOHNew(BOMId, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchOHNewAsync(string BOMId, string connVMwcf) {
            return base.Channel.SearchOHNewAsync(BOMId, connVMwcf);
        }
        
        public string BOMInsert(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMInsert(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> BOMInsertAsync(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMInsertAsync(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);
        }
        
        public string BOMUpdate(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMUpdate(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> BOMUpdateAsync(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMUpdateAsync(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);
        }
        
        public string DeleteBOM(string itemNo, string VatName, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.DeleteBOM(itemNo, VatName, effectDate, CustomerID, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> DeleteBOMAsync(string itemNo, string VatName, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.DeleteBOMAsync(itemNo, VatName, effectDate, CustomerID, connVMwcf);
        }
        
        public string BOMPost(string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMPost(bomMasterwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> BOMPostAsync(string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMPostAsync(bomMasterwcf, connVMwcf);
        }
        
        public string FindCostingFrom(string itemNo, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.FindCostingFrom(itemNo, effectDate, CustomerID, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> FindCostingFromAsync(string itemNo, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.FindCostingFromAsync(itemNo, effectDate, CustomerID, connVMwcf);
        }
        
        public string SelectAll(string BOMId, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, bool Dt, string connVMwcf) {
            return base.Channel.SelectAll(BOMId, conditionFieldswcf, conditionValueswcf, likeVMwcf, Dt, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SelectAllAsync(string BOMId, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, bool Dt, string connVMwcf) {
            return base.Channel.SelectAllAsync(BOMId, conditionFieldswcf, conditionValueswcf, likeVMwcf, Dt, connVMwcf);
        }
        
        public string SelectAllList(string BOMId, string conditionField, string conditionValue, string likeVMwcf, string connVMwcf) {
            return base.Channel.SelectAllList(BOMId, conditionField, conditionValue, likeVMwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SelectAllListAsync(string BOMId, string conditionField, string conditionValue, string likeVMwcf, string connVMwcf) {
            return base.Channel.SelectAllListAsync(BOMId, conditionField, conditionValue, likeVMwcf, connVMwcf);
        }
        
        public string SearchBOMMaster(string finItem, string vatName, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.SearchBOMMaster(finItem, vatName, effectDate, CustomerID, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchBOMMasterAsync(string finItem, string vatName, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.SearchBOMMasterAsync(finItem, vatName, effectDate, CustomerID, connVMwcf);
        }
        
        public string SearchBOMRaw(string finItem, string vatName, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.SearchBOMRaw(finItem, vatName, effectDate, CustomerID, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchBOMRawAsync(string finItem, string vatName, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.SearchBOMRawAsync(finItem, vatName, effectDate, CustomerID, connVMwcf);
        }
        
        public string SearchOH(string finItem, string vatName, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.SearchOH(finItem, vatName, effectDate, CustomerID, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchOHAsync(string finItem, string vatName, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.SearchOHAsync(finItem, vatName, effectDate, CustomerID, connVMwcf);
        }
        
        public string ServiceInsert(string Detailswcf, string connVMwcf) {
            return base.Channel.ServiceInsert(Detailswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> ServiceInsertAsync(string Detailswcf, string connVMwcf) {
            return base.Channel.ServiceInsertAsync(Detailswcf, connVMwcf);
        }
        
        public string ServiceUpdate(string Detailswcf, string connVMwcf) {
            return base.Channel.ServiceUpdate(Detailswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> ServiceUpdateAsync(string Detailswcf, string connVMwcf) {
            return base.Channel.ServiceUpdateAsync(Detailswcf, connVMwcf);
        }
        
        public string ServicePost(string Detailswcf, string connVMwcf) {
            return base.Channel.ServicePost(Detailswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> ServicePostAsync(string Detailswcf, string connVMwcf) {
            return base.Channel.ServicePostAsync(Detailswcf, connVMwcf);
        }
        
        public string ServiceDelete(string Detailswcf, string connVMwcf) {
            return base.Channel.ServiceDelete(Detailswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> ServiceDeleteAsync(string Detailswcf, string connVMwcf) {
            return base.Channel.ServiceDeleteAsync(Detailswcf, connVMwcf);
        }
        
        public string SearchInputValues(string FinishItemName, string EffectDate, string VATName, string post, string FinishItemNo, string connVMwcf) {
            return base.Channel.SearchInputValues(FinishItemName, EffectDate, VATName, post, FinishItemNo, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchInputValuesAsync(string FinishItemName, string EffectDate, string VATName, string post, string FinishItemNo, string connVMwcf) {
            return base.Channel.SearchInputValuesAsync(FinishItemName, EffectDate, VATName, post, FinishItemNo, connVMwcf);
        }
        
        public string SearchServicePrice(string BOMId, string connVMwcf) {
            return base.Channel.SearchServicePrice(BOMId, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchServicePriceAsync(string BOMId, string connVMwcf) {
            return base.Channel.SearchServicePriceAsync(BOMId, connVMwcf);
        }
        
        public string CustomerByBomId(string BOMId, string connVMwcf) {
            return base.Channel.CustomerByBomId(BOMId, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> CustomerByBomIdAsync(string BOMId, string connVMwcf) {
            return base.Channel.CustomerByBomIdAsync(BOMId, connVMwcf);
        }
        
        public string UseQuantityDT(string FinishItemNo, decimal Quantity, string EffectDate, string CustomerID, string connVMwcf) {
            return base.Channel.UseQuantityDT(FinishItemNo, Quantity, EffectDate, CustomerID, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> UseQuantityDTAsync(string FinishItemNo, decimal Quantity, string EffectDate, string CustomerID, string connVMwcf) {
            return base.Channel.UseQuantityDTAsync(FinishItemNo, Quantity, EffectDate, CustomerID, connVMwcf);
        }
        
        public string SelectAllItems(string BOMId, string conditionFieldswcf, string conditionValueswcf, string connVMwcf) {
            return base.Channel.SelectAllItems(BOMId, conditionFieldswcf, conditionValueswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SelectAllItemsAsync(string BOMId, string conditionFieldswcf, string conditionValueswcf, string connVMwcf) {
            return base.Channel.SelectAllItemsAsync(BOMId, conditionFieldswcf, conditionValueswcf, connVMwcf);
        }
        
        public string SelectAllOverheads(string BOMId, string conditionFieldswcf, string conditionValueswcf, string connVMwcf) {
            return base.Channel.SelectAllOverheads(BOMId, conditionFieldswcf, conditionValueswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SelectAllOverheadsAsync(string BOMId, string conditionFieldswcf, string conditionValueswcf, string connVMwcf) {
            return base.Channel.SelectAllOverheadsAsync(BOMId, conditionFieldswcf, conditionValueswcf, connVMwcf);
        }
        
        public string BOMPreInsert(string vmwcf, string connVMwcf) {
            return base.Channel.BOMPreInsert(vmwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> BOMPreInsertAsync(string vmwcf, string connVMwcf) {
            return base.Channel.BOMPreInsertAsync(vmwcf, connVMwcf);
        }
        
        public string FindBOMIDOverHead(string itemNo, string VatName, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.FindBOMIDOverHead(itemNo, VatName, effectDate, CustomerID, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> FindBOMIDOverHeadAsync(string itemNo, string VatName, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.FindBOMIDOverHeadAsync(itemNo, VatName, effectDate, CustomerID, connVMwcf);
        }
        
        public string BOMImport(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMImport(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> BOMImportAsync(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMImportAsync(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);
        }
        
        public string BOMInsertX(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMInsertX(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> BOMInsertXAsync(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMInsertXAsync(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);
        }
        
        public string BOMInsert2(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMInsert2(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> BOMInsert2Async(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMInsert2Async(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);
        }
        
        public string BOMUpdateX(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMUpdateX(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> BOMUpdateXAsync(string bomItemswcf, string bomOHswcf, string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMUpdateXAsync(bomItemswcf, bomOHswcf, bomMasterwcf, connVMwcf);
        }
        
        public string BOMPostX(string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMPostX(bomMasterwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> BOMPostXAsync(string bomMasterwcf, string connVMwcf) {
            return base.Channel.BOMPostXAsync(bomMasterwcf, connVMwcf);
        }
        
        public string FindBOMID(string itemNo, string VatName, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.FindBOMID(itemNo, VatName, effectDate, CustomerID, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> FindBOMIDAsync(string itemNo, string VatName, string effectDate, string CustomerID, string connVMwcf) {
            return base.Channel.FindBOMIDAsync(itemNo, VatName, effectDate, CustomerID, connVMwcf);
        }
    }
}
