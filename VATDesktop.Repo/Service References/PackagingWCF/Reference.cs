﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VATDesktop.Repo.PackagingWCF {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PackagingWCF.IPackagingWCF")]
    public interface IPackagingWCF {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPackagingWCF/DoWork", ReplyAction="http://tempuri.org/IPackagingWCF/DoWorkResponse")]
        void DoWork();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPackagingWCF/DoWork", ReplyAction="http://tempuri.org/IPackagingWCF/DoWorkResponse")]
        System.Threading.Tasks.Task DoWorkAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPackagingWCF/DeletePackageInformation", ReplyAction="http://tempuri.org/IPackagingWCF/DeletePackageInformationResponse")]
        string DeletePackageInformation(string PackId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPackagingWCF/DeletePackageInformation", ReplyAction="http://tempuri.org/IPackagingWCF/DeletePackageInformationResponse")]
        System.Threading.Tasks.Task<string> DeletePackageInformationAsync(string PackId, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPackagingWCF/InsertToPackage", ReplyAction="http://tempuri.org/IPackagingWCF/InsertToPackageResponse")]
        string InsertToPackage(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPackagingWCF/InsertToPackage", ReplyAction="http://tempuri.org/IPackagingWCF/InsertToPackageResponse")]
        System.Threading.Tasks.Task<string> InsertToPackageAsync(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPackagingWCF/SearchPackage", ReplyAction="http://tempuri.org/IPackagingWCF/SearchPackageResponse")]
        string SearchPackage(string PackName, string PackgeSize, string ActiveStatus, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPackagingWCF/SearchPackage", ReplyAction="http://tempuri.org/IPackagingWCF/SearchPackageResponse")]
        System.Threading.Tasks.Task<string> SearchPackageAsync(string PackName, string PackgeSize, string ActiveStatus, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPackagingWCF/UpdatePackage", ReplyAction="http://tempuri.org/IPackagingWCF/UpdatePackageResponse")]
        string UpdatePackage(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string LastModifiedBy, string LastModifiedOn, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPackagingWCF/UpdatePackage", ReplyAction="http://tempuri.org/IPackagingWCF/UpdatePackageResponse")]
        System.Threading.Tasks.Task<string> UpdatePackageAsync(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string LastModifiedBy, string LastModifiedOn, string connVMwcf);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPackagingWCFChannel : VATDesktop.Repo.PackagingWCF.IPackagingWCF, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PackagingWCFClient : System.ServiceModel.ClientBase<VATDesktop.Repo.PackagingWCF.IPackagingWCF>, VATDesktop.Repo.PackagingWCF.IPackagingWCF {
        
        public PackagingWCFClient() {
        }
        
        public PackagingWCFClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PackagingWCFClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PackagingWCFClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PackagingWCFClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void DoWork() {
            base.Channel.DoWork();
        }
        
        public System.Threading.Tasks.Task DoWorkAsync() {
            return base.Channel.DoWorkAsync();
        }
        
        public string DeletePackageInformation(string PackId, string connVMwcf) {
            return base.Channel.DeletePackageInformation(PackId, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> DeletePackageInformationAsync(string PackId, string connVMwcf) {
            return base.Channel.DeletePackageInformationAsync(PackId, connVMwcf);
        }
        
        public string InsertToPackage(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string connVMwcf) {
            return base.Channel.InsertToPackage(PackID, PackName, PackSize, Uom, Description, ActiveStatus, CreatedBy, CreatedOn, LastModifiedBy, LastModifiedOn, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> InsertToPackageAsync(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string connVMwcf) {
            return base.Channel.InsertToPackageAsync(PackID, PackName, PackSize, Uom, Description, ActiveStatus, CreatedBy, CreatedOn, LastModifiedBy, LastModifiedOn, connVMwcf);
        }
        
        public string SearchPackage(string PackName, string PackgeSize, string ActiveStatus, string connVMwcf) {
            return base.Channel.SearchPackage(PackName, PackgeSize, ActiveStatus, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchPackageAsync(string PackName, string PackgeSize, string ActiveStatus, string connVMwcf) {
            return base.Channel.SearchPackageAsync(PackName, PackgeSize, ActiveStatus, connVMwcf);
        }
        
        public string UpdatePackage(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string LastModifiedBy, string LastModifiedOn, string connVMwcf) {
            return base.Channel.UpdatePackage(PackID, PackName, PackSize, Uom, Description, ActiveStatus, LastModifiedBy, LastModifiedOn, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> UpdatePackageAsync(string PackID, string PackName, string PackSize, string Uom, string Description, string ActiveStatus, string LastModifiedBy, string LastModifiedOn, string connVMwcf) {
            return base.Channel.UpdatePackageAsync(PackID, PackName, PackSize, Uom, Description, ActiveStatus, LastModifiedBy, LastModifiedOn, connVMwcf);
        }
    }
}
