﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VATDesktop.Repo.IssueBOMWCF {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="IssueBOMWCF.IIssueBOMWCF")]
    public interface IIssueBOMWCF {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/DoWork", ReplyAction="http://tempuri.org/IIssueBOMWCF/DoWorkResponse")]
        void DoWork();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/DoWork", ReplyAction="http://tempuri.org/IIssueBOMWCF/DoWorkResponse")]
        System.Threading.Tasks.Task DoWorkAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/ImportData", ReplyAction="http://tempuri.org/IIssueBOMWCF/ImportDataResponse")]
        string ImportData(string dtIssueMwcf, string dtIssueDwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/ImportData", ReplyAction="http://tempuri.org/IIssueBOMWCF/ImportDataResponse")]
        System.Threading.Tasks.Task<string> ImportDataAsync(string dtIssueMwcf, string dtIssueDwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/ImportExcelFile", ReplyAction="http://tempuri.org/IIssueBOMWCF/ImportExcelFileResponse")]
        string ImportExcelFile(string paramVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/ImportExcelFile", ReplyAction="http://tempuri.org/IIssueBOMWCF/ImportExcelFileResponse")]
        System.Threading.Tasks.Task<string> ImportExcelFileAsync(string paramVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/IssueInsert", ReplyAction="http://tempuri.org/IIssueBOMWCF/IssueInsertResponse")]
        string IssueInsert(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/IssueInsert", ReplyAction="http://tempuri.org/IIssueBOMWCF/IssueInsertResponse")]
        System.Threading.Tasks.Task<string> IssueInsertAsync(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/IssuePost", ReplyAction="http://tempuri.org/IIssueBOMWCF/IssuePostResponse")]
        string IssuePost(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/IssuePost", ReplyAction="http://tempuri.org/IIssueBOMWCF/IssuePostResponse")]
        System.Threading.Tasks.Task<string> IssuePostAsync(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/IssueUpdate", ReplyAction="http://tempuri.org/IIssueBOMWCF/IssueUpdateResponse")]
        string IssueUpdate(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/IssueUpdate", ReplyAction="http://tempuri.org/IIssueBOMWCF/IssueUpdateResponse")]
        System.Threading.Tasks.Task<string> IssueUpdateAsync(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/ReturnIssueQty", ReplyAction="http://tempuri.org/IIssueBOMWCF/ReturnIssueQtyResponse")]
        string ReturnIssueQty(string issueReturnId, string itemNo, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/ReturnIssueQty", ReplyAction="http://tempuri.org/IIssueBOMWCF/ReturnIssueQtyResponse")]
        System.Threading.Tasks.Task<string> ReturnIssueQtyAsync(string issueReturnId, string itemNo, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/SearchIssueDetailDTNew", ReplyAction="http://tempuri.org/IIssueBOMWCF/SearchIssueDetailDTNewResponse")]
        string SearchIssueDetailDTNew(string IssueNo, string databaseName, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/SearchIssueDetailDTNew", ReplyAction="http://tempuri.org/IIssueBOMWCF/SearchIssueDetailDTNewResponse")]
        System.Threading.Tasks.Task<string> SearchIssueDetailDTNewAsync(string IssueNo, string databaseName, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/SelectAll", ReplyAction="http://tempuri.org/IIssueBOMWCF/SelectAllResponse")]
        string SelectAll(int Id, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, bool Dt, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/SelectAll", ReplyAction="http://tempuri.org/IIssueBOMWCF/SelectAllResponse")]
        System.Threading.Tasks.Task<string> SelectAllAsync(int Id, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, bool Dt, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/SelectAllList", ReplyAction="http://tempuri.org/IIssueBOMWCF/SelectAllListResponse")]
        string SelectAllList(int Id, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/SelectAllList", ReplyAction="http://tempuri.org/IIssueBOMWCF/SelectAllListResponse")]
        System.Threading.Tasks.Task<string> SelectAllListAsync(int Id, string conditionFieldswcf, string conditionValueswcf, string likeVMwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/SelectIssueDetail", ReplyAction="http://tempuri.org/IIssueBOMWCF/SelectIssueDetailResponse")]
        string SelectIssueDetail(string issueNo, string conditionFieldswcf, string conditionValueswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIssueBOMWCF/SelectIssueDetail", ReplyAction="http://tempuri.org/IIssueBOMWCF/SelectIssueDetailResponse")]
        System.Threading.Tasks.Task<string> SelectIssueDetailAsync(string issueNo, string conditionFieldswcf, string conditionValueswcf, string connVMwcf);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IIssueBOMWCFChannel : VATDesktop.Repo.IssueBOMWCF.IIssueBOMWCF, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class IssueBOMWCFClient : System.ServiceModel.ClientBase<VATDesktop.Repo.IssueBOMWCF.IIssueBOMWCF>, VATDesktop.Repo.IssueBOMWCF.IIssueBOMWCF {
        
        public IssueBOMWCFClient() {
        }
        
        public IssueBOMWCFClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public IssueBOMWCFClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IssueBOMWCFClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IssueBOMWCFClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void DoWork() {
            base.Channel.DoWork();
        }
        
        public System.Threading.Tasks.Task DoWorkAsync() {
            return base.Channel.DoWorkAsync();
        }
        
        public string ImportData(string dtIssueMwcf, string dtIssueDwcf, string connVMwcf) {
            return base.Channel.ImportData(dtIssueMwcf, dtIssueDwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> ImportDataAsync(string dtIssueMwcf, string dtIssueDwcf, string connVMwcf) {
            return base.Channel.ImportDataAsync(dtIssueMwcf, dtIssueDwcf, connVMwcf);
        }
        
        public string ImportExcelFile(string paramVMwcf) {
            return base.Channel.ImportExcelFile(paramVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> ImportExcelFileAsync(string paramVMwcf) {
            return base.Channel.ImportExcelFileAsync(paramVMwcf);
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
        
        public string ReturnIssueQty(string issueReturnId, string itemNo, string connVMwcf) {
            return base.Channel.ReturnIssueQty(issueReturnId, itemNo, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> ReturnIssueQtyAsync(string issueReturnId, string itemNo, string connVMwcf) {
            return base.Channel.ReturnIssueQtyAsync(issueReturnId, itemNo, connVMwcf);
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
