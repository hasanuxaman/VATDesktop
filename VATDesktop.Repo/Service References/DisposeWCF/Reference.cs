﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VATDesktop.Repo.DisposeWCF {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DisposeWCF.IDisposeWCF")]
    public interface IDisposeWCF {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDisposeWCF/DoWork", ReplyAction="http://tempuri.org/IDisposeWCF/DoWorkResponse")]
        void DoWork();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDisposeWCF/DoWork", ReplyAction="http://tempuri.org/IDisposeWCF/DoWorkResponse")]
        System.Threading.Tasks.Task DoWorkAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDisposeWCF/DisposeInsert", ReplyAction="http://tempuri.org/IDisposeWCF/DisposeInsertResponse")]
        string DisposeInsert(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDisposeWCF/DisposeInsert", ReplyAction="http://tempuri.org/IDisposeWCF/DisposeInsertResponse")]
        System.Threading.Tasks.Task<string> DisposeInsertAsync(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDisposeWCF/DisposePost", ReplyAction="http://tempuri.org/IDisposeWCF/DisposePostResponse")]
        string DisposePost(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDisposeWCF/DisposePost", ReplyAction="http://tempuri.org/IDisposeWCF/DisposePostResponse")]
        System.Threading.Tasks.Task<string> DisposePostAsync(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDisposeWCF/DisposeUpdate", ReplyAction="http://tempuri.org/IDisposeWCF/DisposeUpdateResponse")]
        string DisposeUpdate(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDisposeWCF/DisposeUpdate", ReplyAction="http://tempuri.org/IDisposeWCF/DisposeUpdateResponse")]
        System.Threading.Tasks.Task<string> DisposeUpdateAsync(string Masterwcf, string Detailswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDisposeWCF/SearchDisposeDetailDTNew", ReplyAction="http://tempuri.org/IDisposeWCF/SearchDisposeDetailDTNewResponse")]
        string SearchDisposeDetailDTNew(string DisposeNumber, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDisposeWCF/SearchDisposeDetailDTNew", ReplyAction="http://tempuri.org/IDisposeWCF/SearchDisposeDetailDTNewResponse")]
        System.Threading.Tasks.Task<string> SearchDisposeDetailDTNewAsync(string DisposeNumber, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDisposeWCF/SearchDisposeHeaderDTNew", ReplyAction="http://tempuri.org/IDisposeWCF/SearchDisposeHeaderDTNewResponse")]
        string SearchDisposeHeaderDTNew(string DisposeNumber, string DisposeDateFrom, string DisposeDateTo, string transactionType, string Post, string databasename, int BranchId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDisposeWCF/SearchDisposeHeaderDTNew", ReplyAction="http://tempuri.org/IDisposeWCF/SearchDisposeHeaderDTNewResponse")]
        System.Threading.Tasks.Task<string> SearchDisposeHeaderDTNewAsync(string DisposeNumber, string DisposeDateFrom, string DisposeDateTo, string transactionType, string Post, string databasename, int BranchId, string connVMwcf);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDisposeWCFChannel : VATDesktop.Repo.DisposeWCF.IDisposeWCF, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DisposeWCFClient : System.ServiceModel.ClientBase<VATDesktop.Repo.DisposeWCF.IDisposeWCF>, VATDesktop.Repo.DisposeWCF.IDisposeWCF {
        
        public DisposeWCFClient() {
        }
        
        public DisposeWCFClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DisposeWCFClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DisposeWCFClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DisposeWCFClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void DoWork() {
            base.Channel.DoWork();
        }
        
        public System.Threading.Tasks.Task DoWorkAsync() {
            return base.Channel.DoWorkAsync();
        }
        
        public string DisposeInsert(string Masterwcf, string Detailswcf, string connVMwcf) {
            return base.Channel.DisposeInsert(Masterwcf, Detailswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> DisposeInsertAsync(string Masterwcf, string Detailswcf, string connVMwcf) {
            return base.Channel.DisposeInsertAsync(Masterwcf, Detailswcf, connVMwcf);
        }
        
        public string DisposePost(string Masterwcf, string Detailswcf, string connVMwcf) {
            return base.Channel.DisposePost(Masterwcf, Detailswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> DisposePostAsync(string Masterwcf, string Detailswcf, string connVMwcf) {
            return base.Channel.DisposePostAsync(Masterwcf, Detailswcf, connVMwcf);
        }
        
        public string DisposeUpdate(string Masterwcf, string Detailswcf, string connVMwcf) {
            return base.Channel.DisposeUpdate(Masterwcf, Detailswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> DisposeUpdateAsync(string Masterwcf, string Detailswcf, string connVMwcf) {
            return base.Channel.DisposeUpdateAsync(Masterwcf, Detailswcf, connVMwcf);
        }
        
        public string SearchDisposeDetailDTNew(string DisposeNumber, string connVMwcf) {
            return base.Channel.SearchDisposeDetailDTNew(DisposeNumber, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchDisposeDetailDTNewAsync(string DisposeNumber, string connVMwcf) {
            return base.Channel.SearchDisposeDetailDTNewAsync(DisposeNumber, connVMwcf);
        }
        
        public string SearchDisposeHeaderDTNew(string DisposeNumber, string DisposeDateFrom, string DisposeDateTo, string transactionType, string Post, string databasename, int BranchId, string connVMwcf) {
            return base.Channel.SearchDisposeHeaderDTNew(DisposeNumber, DisposeDateFrom, DisposeDateTo, transactionType, Post, databasename, BranchId, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchDisposeHeaderDTNewAsync(string DisposeNumber, string DisposeDateFrom, string DisposeDateTo, string transactionType, string Post, string databasename, int BranchId, string connVMwcf) {
            return base.Channel.SearchDisposeHeaderDTNewAsync(DisposeNumber, DisposeDateFrom, DisposeDateTo, transactionType, Post, databasename, BranchId, connVMwcf);
        }
    }
}
