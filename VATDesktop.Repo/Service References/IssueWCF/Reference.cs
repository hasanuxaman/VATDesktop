﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VATDesktop.Repo.IssueWCF {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="IssueWCF.IIssueWCF")]
    public interface IIssueWCF {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/DoWork", ReplyAction="http://tempuri.org/IIssueWCF/DoWorkResponse")]
        void DoWork();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/DoWork", ReplyAction="http://tempuri.org/IIssueWCF/DoWorkResponse")]
        System.Threading.Tasks.Task DoWorkAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/GetExcelData", ReplyAction="http://tempuri.org/IIssueWCF/GetExcelDataResponse")]
        string GetExcelData(string invoiceListwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/GetExcelData", ReplyAction="http://tempuri.org/IIssueWCF/GetExcelDataResponse")]
        System.Threading.Tasks.Task<string> GetExcelDataAsync(string invoiceListwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/GetUnProcessedCount", ReplyAction="http://tempuri.org/IIssueWCF/GetUnProcessedCountResponse")]
        string GetUnProcessedCount(string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/GetUnProcessedCount", ReplyAction="http://tempuri.org/IIssueWCF/GetUnProcessedCountResponse")]
        System.Threading.Tasks.Task<string> GetUnProcessedCountAsync(string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/ImportBigData", ReplyAction="http://tempuri.org/IIssueWCF/ImportBigDataResponse")]
        string ImportBigData(string issueDatawcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/ImportBigData", ReplyAction="http://tempuri.org/IIssueWCF/ImportBigDataResponse")]
        System.Threading.Tasks.Task<string> ImportBigDataAsync(string issueDatawcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/ImportData", ReplyAction="http://tempuri.org/IIssueWCF/ImportDataResponse")]
        string ImportData(string dtIssueMwcf, string dtIssueDwcf, int branchId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/ImportData", ReplyAction="http://tempuri.org/IIssueWCF/ImportDataResponse")]
        System.Threading.Tasks.Task<string> ImportDataAsync(string dtIssueMwcf, string dtIssueDwcf, int branchId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/ImportExcelFile", ReplyAction="http://tempuri.org/IIssueWCF/ImportExcelFileResponse")]
        string ImportExcelFile(string paramVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/ImportExcelFile", ReplyAction="http://tempuri.org/IIssueWCF/ImportExcelFileResponse")]
        System.Threading.Tasks.Task<string> ImportExcelFileAsync(string paramVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/ImportReceiveBigData", ReplyAction="http://tempuri.org/IIssueWCF/ImportReceiveBigDataResponse")]
        string ImportReceiveBigData(string receiveDatawcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/ImportReceiveBigData", ReplyAction="http://tempuri.org/IIssueWCF/ImportReceiveBigDataResponse")]
        System.Threading.Tasks.Task<string> ImportReceiveBigDataAsync(string receiveDatawcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/IssueInsert", ReplyAction="http://tempuri.org/IIssueWCF/IssueInsertResponse")]
        string IssueInsert(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/IssueInsert", ReplyAction="http://tempuri.org/IIssueWCF/IssueInsertResponse")]
        System.Threading.Tasks.Task<string> IssueInsertAsync(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/IssuePost", ReplyAction="http://tempuri.org/IIssueWCF/IssuePostResponse")]
        string IssuePost(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/IssuePost", ReplyAction="http://tempuri.org/IIssueWCF/IssuePostResponse")]
        System.Threading.Tasks.Task<string> IssuePostAsync(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/IssueUpdate", ReplyAction="http://tempuri.org/IIssueWCF/IssueUpdateResponse")]
        string IssueUpdate(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/IssueUpdate", ReplyAction="http://tempuri.org/IIssueWCF/IssueUpdateResponse")]
        System.Threading.Tasks.Task<string> IssueUpdateAsync(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/MultiplePost", ReplyAction="http://tempuri.org/IIssueWCF/MultiplePostResponse")]
        string MultiplePost(string Idswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/MultiplePost", ReplyAction="http://tempuri.org/IIssueWCF/MultiplePostResponse")]
        System.Threading.Tasks.Task<string> MultiplePostAsync(string Idswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/ReturnIssueQty", ReplyAction="http://tempuri.org/IIssueWCF/ReturnIssueQtyResponse")]
        string ReturnIssueQty(string issueReturnId, string itemNo, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/ReturnIssueQty", ReplyAction="http://tempuri.org/IIssueWCF/ReturnIssueQtyResponse")]
        System.Threading.Tasks.Task<string> ReturnIssueQtyAsync(string issueReturnId, string itemNo, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/SaveTempIssue", ReplyAction="http://tempuri.org/IIssueWCF/SaveTempIssueResponse")]
        string SaveTempIssue(string dtTable, string transactionType, string CurrentUser, int branchId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/SaveTempIssue", ReplyAction="http://tempuri.org/IIssueWCF/SaveTempIssueResponse")]
        System.Threading.Tasks.Task<string> SaveTempIssueAsync(string dtTable, string transactionType, string CurrentUser, int branchId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/SearchIssueDetailDTNew", ReplyAction="http://tempuri.org/IIssueWCF/SearchIssueDetailDTNewResponse")]
        string SearchIssueDetailDTNew(string IssueNo, string databaseName, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/SearchIssueDetailDTNew", ReplyAction="http://tempuri.org/IIssueWCF/SearchIssueDetailDTNewResponse")]
        System.Threading.Tasks.Task<string> SearchIssueDetailDTNewAsync(string IssueNo, string databaseName, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/SelectAll", ReplyAction="http://tempuri.org/IIssueWCF/SelectAllResponse")]
        string SelectAll(int Id, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, bool Dt, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/SelectAll", ReplyAction="http://tempuri.org/IIssueWCF/SelectAllResponse")]
        System.Threading.Tasks.Task<string> SelectAllAsync(int Id, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, bool Dt, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/SelectAllList", ReplyAction="http://tempuri.org/IIssueWCF/SelectAllListResponse")]
        string SelectAllList(int Id, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/SelectAllList", ReplyAction="http://tempuri.org/IIssueWCF/SelectAllListResponse")]
        System.Threading.Tasks.Task<string> SelectAllListAsync(int Id, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/SelectIssueDetail", ReplyAction="http://tempuri.org/IIssueWCF/SelectIssueDetailResponse")]
        string SelectIssueDetail(string issueNo, string conditionFieldswcf, string conditionValueswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueWCF/SelectIssueDetail", ReplyAction="http://tempuri.org/IIssueWCF/SelectIssueDetailResponse")]
        System.Threading.Tasks.Task<string> SelectIssueDetailAsync(string issueNo, string conditionFieldswcf, string conditionValueswcf, string connVMwcf);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IIssueWCFChannel : VATDesktop.Repo.IssueWCF.IIssueWCF, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class IssueWCFClient : System.ServiceModel.ClientBase<VATDesktop.Repo.IssueWCF.IIssueWCF>, VATDesktop.Repo.IssueWCF.IIssueWCF {
        
        public IssueWCFClient() {
        }
        
        public IssueWCFClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public IssueWCFClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IssueWCFClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IssueWCFClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void DoWork() {
            base.Channel.DoWork();
        }
        
        public System.Threading.Tasks.Task DoWorkAsync() {
            return base.Channel.DoWorkAsync();
        }
        
        public string GetExcelData(string invoiceListwcf, string connVMwcf) {
            return base.Channel.GetExcelData(invoiceListwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> GetExcelDataAsync(string invoiceListwcf, string connVMwcf) {
            return base.Channel.GetExcelDataAsync(invoiceListwcf, connVMwcf);
        }
        
        public string GetUnProcessedCount(string connVMwcf) {
            return base.Channel.GetUnProcessedCount(connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> GetUnProcessedCountAsync(string connVMwcf) {
            return base.Channel.GetUnProcessedCountAsync(connVMwcf);
        }
        
        public string ImportBigData(string issueDatawcf, string connVMwcf) {
            return base.Channel.ImportBigData(issueDatawcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> ImportBigDataAsync(string issueDatawcf, string connVMwcf) {
            return base.Channel.ImportBigDataAsync(issueDatawcf, connVMwcf);
        }
        
        public string ImportData(string dtIssueMwcf, string dtIssueDwcf, int branchId, string connVMwcf) {
            return base.Channel.ImportData(dtIssueMwcf, dtIssueDwcf, branchId, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> ImportDataAsync(string dtIssueMwcf, string dtIssueDwcf, int branchId, string connVMwcf) {
            return base.Channel.ImportDataAsync(dtIssueMwcf, dtIssueDwcf, branchId, connVMwcf);
        }
        
        public string ImportExcelFile(string paramVMwcf) {
            return base.Channel.ImportExcelFile(paramVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> ImportExcelFileAsync(string paramVMwcf) {
            return base.Channel.ImportExcelFileAsync(paramVMwcf);
        }
        
        public string ImportReceiveBigData(string receiveDatawcf, string connVMwcf) {
            return base.Channel.ImportReceiveBigData(receiveDatawcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> ImportReceiveBigDataAsync(string receiveDatawcf, string connVMwcf) {
            return base.Channel.ImportReceiveBigDataAsync(receiveDatawcf, connVMwcf);
        }
        
        public string IssueInsert(string Masterwcf, string Detailswcf, string connVMwcf) {
            return base.Channel.IssueInsert(Masterwcf, Detailswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> IssueInsertAsync(string Masterwcf, string Detailswcf, string connVMwcf) {
            return base.Channel.IssueInsertAsync(Masterwcf, Detailswcf, connVMwcf);
        }
        
        public string IssuePost(string Masterwcf, string Detailswcf, string connVMwcf) {
            return base.Channel.IssuePost(Masterwcf, Detailswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> IssuePostAsync(string Masterwcf, string Detailswcf, string connVMwcf) {
            return base.Channel.IssuePostAsync(Masterwcf, Detailswcf, connVMwcf);
        }
        
        public string IssueUpdate(string Masterwcf, string Detailswcf, string connVMwcf) {
            return base.Channel.IssueUpdate(Masterwcf, Detailswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> IssueUpdateAsync(string Masterwcf, string Detailswcf, string connVMwcf) {
            return base.Channel.IssueUpdateAsync(Masterwcf, Detailswcf, connVMwcf);
        }
        
        public string MultiplePost(string Idswcf, string connVMwcf) {
            return base.Channel.MultiplePost(Idswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> MultiplePostAsync(string Idswcf, string connVMwcf) {
            return base.Channel.MultiplePostAsync(Idswcf, connVMwcf);
        }
        
        public string ReturnIssueQty(string issueReturnId, string itemNo, string connVMwcf) {
            return base.Channel.ReturnIssueQty(issueReturnId, itemNo, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> ReturnIssueQtyAsync(string issueReturnId, string itemNo, string connVMwcf) {
            return base.Channel.ReturnIssueQtyAsync(issueReturnId, itemNo, connVMwcf);
        }
        
        public string SaveTempIssue(string dtTable, string transactionType, string CurrentUser, int branchId, string connVMwcf) {
            return base.Channel.SaveTempIssue(dtTable, transactionType, CurrentUser, branchId, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SaveTempIssueAsync(string dtTable, string transactionType, string CurrentUser, int branchId, string connVMwcf) {
            return base.Channel.SaveTempIssueAsync(dtTable, transactionType, CurrentUser, branchId, connVMwcf);
        }
        
        public string SearchIssueDetailDTNew(string IssueNo, string databaseName, string connVMwcf) {
            return base.Channel.SearchIssueDetailDTNew(IssueNo, databaseName, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchIssueDetailDTNewAsync(string IssueNo, string databaseName, string connVMwcf) {
            return base.Channel.SearchIssueDetailDTNewAsync(IssueNo, databaseName, connVMwcf);
        }
        
        public string SelectAll(int Id, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, bool Dt, string connVMwcf) {
            return base.Channel.SelectAll(Id, conditionFieldswcf, conditionValueswcf, likeVMwcf, Dt, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SelectAllAsync(int Id, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, bool Dt, string connVMwcf) {
            return base.Channel.SelectAllAsync(Id, conditionFieldswcf, conditionValueswcf, likeVMwcf, Dt, connVMwcf);
        }
        
        public string SelectAllList(int Id, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, string connVMwcf) {
            return base.Channel.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, likeVMwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SelectAllListAsync(int Id, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, string connVMwcf) {
            return base.Channel.SelectAllListAsync(Id, conditionFieldswcf, conditionValueswcf, likeVMwcf, connVMwcf);
        }
        
        public string SelectIssueDetail(string issueNo, string conditionFieldswcf, string conditionValueswcf, string connVMwcf) {
            return base.Channel.SelectIssueDetail(issueNo, conditionFieldswcf, conditionValueswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SelectIssueDetailAsync(string issueNo, string conditionFieldswcf, string conditionValueswcf, string connVMwcf) {
            return base.Channel.SelectIssueDetailAsync(issueNo, conditionFieldswcf, conditionValueswcf, connVMwcf);
        }
    }
}
