﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VATDesktop.Repo.CustomerWCF {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CustomerWCF.ICustomerWCF")]
    public interface ICustomerWCF {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/DoWork", ReplyAction="http://tempuri.org/ICustomerWCF/DoWorkResponse")]
        void DoWork();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/DoWork", ReplyAction="http://tempuri.org/ICustomerWCF/DoWorkResponse")]
        System.Threading.Tasks.Task DoWorkAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/SelectAllList", ReplyAction="http://tempuri.org/ICustomerWCF/SelectAllListResponse")]
        string SelectAllList(string Id, string conditionFieldswcf, string conditionValueswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/SelectAllList", ReplyAction="http://tempuri.org/ICustomerWCF/SelectAllListResponse")]
        System.Threading.Tasks.Task<string> SelectAllListAsync(string Id, string conditionFieldswcf, string conditionValueswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/Delete", ReplyAction="http://tempuri.org/ICustomerWCF/DeleteResponse")]
        string Delete(string vmwcf, string idswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/Delete", ReplyAction="http://tempuri.org/ICustomerWCF/DeleteResponse")]
        System.Threading.Tasks.Task<string> DeleteAsync(string vmwcf, string idswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/DeleteCustomerAddress", ReplyAction="http://tempuri.org/ICustomerWCF/DeleteCustomerAddressResponse")]
        string DeleteCustomerAddress(string CustomerID, string Id, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/DeleteCustomerAddress", ReplyAction="http://tempuri.org/ICustomerWCF/DeleteCustomerAddressResponse")]
        System.Threading.Tasks.Task<string> DeleteCustomerAddressAsync(string CustomerID, string Id, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/DropDown", ReplyAction="http://tempuri.org/ICustomerWCF/DropDownResponse")]
        string DropDown(string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/DropDown", ReplyAction="http://tempuri.org/ICustomerWCF/DropDownResponse")]
        System.Threading.Tasks.Task<string> DropDownAsync(string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/GetExcelAddress", ReplyAction="http://tempuri.org/ICustomerWCF/GetExcelAddressResponse")]
        string GetExcelAddress(string idswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/GetExcelAddress", ReplyAction="http://tempuri.org/ICustomerWCF/GetExcelAddressResponse")]
        System.Threading.Tasks.Task<string> GetExcelAddressAsync(string idswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/GetExcelData", ReplyAction="http://tempuri.org/ICustomerWCF/GetExcelDataResponse")]
        string GetExcelData(string idswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/GetExcelData", ReplyAction="http://tempuri.org/ICustomerWCF/GetExcelDataResponse")]
        System.Threading.Tasks.Task<string> GetExcelDataAsync(string idswcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/InsertToCustomerAddress", ReplyAction="http://tempuri.org/ICustomerWCF/InsertToCustomerAddressResponse")]
        string InsertToCustomerAddress(string vmwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/InsertToCustomerAddress", ReplyAction="http://tempuri.org/ICustomerWCF/InsertToCustomerAddressResponse")]
        System.Threading.Tasks.Task<string> InsertToCustomerAddressAsync(string vmwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/InsertToCustomerNew", ReplyAction="http://tempuri.org/ICustomerWCF/InsertToCustomerNewResponse")]
        string InsertToCustomerNew(string vmwcf, bool CustomerAutoSave, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/InsertToCustomerNew", ReplyAction="http://tempuri.org/ICustomerWCF/InsertToCustomerNewResponse")]
        System.Threading.Tasks.Task<string> InsertToCustomerNewAsync(string vmwcf, bool CustomerAutoSave, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/SearchCountry", ReplyAction="http://tempuri.org/ICustomerWCF/SearchCountryResponse")]
        string SearchCountry(string customer, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/SearchCountry", ReplyAction="http://tempuri.org/ICustomerWCF/SearchCountryResponse")]
        System.Threading.Tasks.Task<string> SearchCountryAsync(string customer, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/SearchCustomerAddress", ReplyAction="http://tempuri.org/ICustomerWCF/SearchCustomerAddressResponse")]
        string SearchCustomerAddress(string CustomerID, string Id, string address, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/SearchCustomerAddress", ReplyAction="http://tempuri.org/ICustomerWCF/SearchCustomerAddressResponse")]
        System.Threading.Tasks.Task<string> SearchCustomerAddressAsync(string CustomerID, string Id, string address, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/SearchCustomerByCode", ReplyAction="http://tempuri.org/ICustomerWCF/SearchCustomerByCodeResponse")]
        string SearchCustomerByCode(string customerCode, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/SearchCustomerByCode", ReplyAction="http://tempuri.org/ICustomerWCF/SearchCustomerByCodeResponse")]
        System.Threading.Tasks.Task<string> SearchCustomerByCodeAsync(string customerCode, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/SelectAll", ReplyAction="http://tempuri.org/ICustomerWCF/SelectAllResponse")]
        string SelectAll(string Id, string conditionFieldswcf, string conditionValueswcf, bool Dt, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/SelectAll", ReplyAction="http://tempuri.org/ICustomerWCF/SelectAllResponse")]
        System.Threading.Tasks.Task<string> SelectAllAsync(string Id, string conditionFieldswcf, string conditionValueswcf, bool Dt, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/UpdateToCustomerAddress", ReplyAction="http://tempuri.org/ICustomerWCF/UpdateToCustomerAddressResponse")]
        string UpdateToCustomerAddress(string vmwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/UpdateToCustomerAddress", ReplyAction="http://tempuri.org/ICustomerWCF/UpdateToCustomerAddressResponse")]
        System.Threading.Tasks.Task<string> UpdateToCustomerAddressAsync(string vmwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/UpdateToCustomerNew", ReplyAction="http://tempuri.org/ICustomerWCF/UpdateToCustomerNewResponse")]
        string UpdateToCustomerNew(string vmwcf, string connVMwcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICustomerWCF/UpdateToCustomerNew", ReplyAction="http://tempuri.org/ICustomerWCF/UpdateToCustomerNewResponse")]
        System.Threading.Tasks.Task<string> UpdateToCustomerNewAsync(string vmwcf, string connVMwcf);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICustomerWCFChannel : VATDesktop.Repo.CustomerWCF.ICustomerWCF, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CustomerWCFClient : System.ServiceModel.ClientBase<VATDesktop.Repo.CustomerWCF.ICustomerWCF>, VATDesktop.Repo.CustomerWCF.ICustomerWCF {
        
        public CustomerWCFClient() {
        }
        
        public CustomerWCFClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CustomerWCFClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CustomerWCFClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CustomerWCFClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void DoWork() {
            base.Channel.DoWork();
        }
        
        public System.Threading.Tasks.Task DoWorkAsync() {
            return base.Channel.DoWorkAsync();
        }
        
        public string SelectAllList(string Id, string conditionFieldswcf, string conditionValueswcf, string connVMwcf) {
            return base.Channel.SelectAllList(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SelectAllListAsync(string Id, string conditionFieldswcf, string conditionValueswcf, string connVMwcf) {
            return base.Channel.SelectAllListAsync(Id, conditionFieldswcf, conditionValueswcf, connVMwcf);
        }
        
        public string Delete(string vmwcf, string idswcf, string connVMwcf) {
            return base.Channel.Delete(vmwcf, idswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> DeleteAsync(string vmwcf, string idswcf, string connVMwcf) {
            return base.Channel.DeleteAsync(vmwcf, idswcf, connVMwcf);
        }
        
        public string DeleteCustomerAddress(string CustomerID, string Id, string connVMwcf) {
            return base.Channel.DeleteCustomerAddress(CustomerID, Id, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> DeleteCustomerAddressAsync(string CustomerID, string Id, string connVMwcf) {
            return base.Channel.DeleteCustomerAddressAsync(CustomerID, Id, connVMwcf);
        }
        
        public string DropDown(string connVMwcf) {
            return base.Channel.DropDown(connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> DropDownAsync(string connVMwcf) {
            return base.Channel.DropDownAsync(connVMwcf);
        }
        
        public string GetExcelAddress(string idswcf, string connVMwcf) {
            return base.Channel.GetExcelAddress(idswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> GetExcelAddressAsync(string idswcf, string connVMwcf) {
            return base.Channel.GetExcelAddressAsync(idswcf, connVMwcf);
        }
        
        public string GetExcelData(string idswcf, string connVMwcf) {
            return base.Channel.GetExcelData(idswcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> GetExcelDataAsync(string idswcf, string connVMwcf) {
            return base.Channel.GetExcelDataAsync(idswcf, connVMwcf);
        }
        
        public string InsertToCustomerAddress(string vmwcf, string connVMwcf) {
            return base.Channel.InsertToCustomerAddress(vmwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> InsertToCustomerAddressAsync(string vmwcf, string connVMwcf) {
            return base.Channel.InsertToCustomerAddressAsync(vmwcf, connVMwcf);
        }
        
        public string InsertToCustomerNew(string vmwcf, bool CustomerAutoSave, string connVMwcf) {
            return base.Channel.InsertToCustomerNew(vmwcf, CustomerAutoSave, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> InsertToCustomerNewAsync(string vmwcf, bool CustomerAutoSave, string connVMwcf) {
            return base.Channel.InsertToCustomerNewAsync(vmwcf, CustomerAutoSave, connVMwcf);
        }
        
        public string SearchCountry(string customer, string connVMwcf) {
            return base.Channel.SearchCountry(customer, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchCountryAsync(string customer, string connVMwcf) {
            return base.Channel.SearchCountryAsync(customer, connVMwcf);
        }
        
        public string SearchCustomerAddress(string CustomerID, string Id, string address, string connVMwcf) {
            return base.Channel.SearchCustomerAddress(CustomerID, Id, address, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchCustomerAddressAsync(string CustomerID, string Id, string address, string connVMwcf) {
            return base.Channel.SearchCustomerAddressAsync(CustomerID, Id, address, connVMwcf);
        }
        
        public string SearchCustomerByCode(string customerCode, string connVMwcf) {
            return base.Channel.SearchCustomerByCode(customerCode, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SearchCustomerByCodeAsync(string customerCode, string connVMwcf) {
            return base.Channel.SearchCustomerByCodeAsync(customerCode, connVMwcf);
        }
        
        public string SelectAll(string Id, string conditionFieldswcf, string conditionValueswcf, bool Dt, string connVMwcf) {
            return base.Channel.SelectAll(Id, conditionFieldswcf, conditionValueswcf, Dt, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> SelectAllAsync(string Id, string conditionFieldswcf, string conditionValueswcf, bool Dt, string connVMwcf) {
            return base.Channel.SelectAllAsync(Id, conditionFieldswcf, conditionValueswcf, Dt, connVMwcf);
        }
        
        public string UpdateToCustomerAddress(string vmwcf, string connVMwcf) {
            return base.Channel.UpdateToCustomerAddress(vmwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> UpdateToCustomerAddressAsync(string vmwcf, string connVMwcf) {
            return base.Channel.UpdateToCustomerAddressAsync(vmwcf, connVMwcf);
        }
        
        public string UpdateToCustomerNew(string vmwcf, string connVMwcf) {
            return base.Channel.UpdateToCustomerNew(vmwcf, connVMwcf);
        }
        
        public System.Threading.Tasks.Task<string> UpdateToCustomerNewAsync(string vmwcf, string connVMwcf) {
            return base.Channel.UpdateToCustomerNewAsync(vmwcf, connVMwcf);
        }
    }
}
