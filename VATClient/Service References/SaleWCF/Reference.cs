﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VATClient.SaleWCF {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="SaleWCF.ISaleWCF")]
    public interface ISaleWCF {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/DoWork", ReplyAction="http://tempuri.org/ISaleWCF/DoWorkResponse")]
        void DoWork();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/DoWork", ReplyAction="http://tempuri.org/ISaleWCF/DoWorkResponse")]
        System.Threading.Tasks.Task DoWorkAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/GetCategoryName", ReplyAction="http://tempuri.org/ISaleWCF/GetCategoryNameResponse")]
        string GetCategoryName(string itemNo, VATViewModel.DTOs.SysDBInfoVMTemp connVM);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/GetCategoryName", ReplyAction="http://tempuri.org/ISaleWCF/GetCategoryNameResponse")]
        System.Threading.Tasks.Task<string> GetCategoryNameAsync(string itemNo, VATViewModel.DTOs.SysDBInfoVMTemp connVM);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/SalesInsert", ReplyAction="http://tempuri.org/ISaleWCF/SalesInsertResponse")]
        System.Collections.Generic.List<string> SalesInsert(VATViewModel.DTOs.SaleMasterVM MasterVM, System.Collections.Generic.List<VATViewModel.DTOs.SaleDetailVm> DetailVMs, System.Collections.Generic.List<VATViewModel.DTOs.SaleExportVM> ExportDetails, System.Collections.Generic.List<VATViewModel.DTOs.TrackingVM> Trackings, int branchId, VATViewModel.DTOs.SysDBInfoVMTemp connVM);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/SalesInsert", ReplyAction="http://tempuri.org/ISaleWCF/SalesInsertResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<string>> SalesInsertAsync(VATViewModel.DTOs.SaleMasterVM MasterVM, System.Collections.Generic.List<VATViewModel.DTOs.SaleDetailVm> DetailVMs, System.Collections.Generic.List<VATViewModel.DTOs.SaleExportVM> ExportDetails, System.Collections.Generic.List<VATViewModel.DTOs.TrackingVM> Trackings, int branchId, VATViewModel.DTOs.SysDBInfoVMTemp connVM);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/Test", ReplyAction="http://tempuri.org/ISaleWCF/TestResponse")]
        System.Data.DataSet Test(System.Data.DataTable dt, VATViewModel.DTOs.SysDBInfoVMTemp connVM);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/Test", ReplyAction="http://tempuri.org/ISaleWCF/TestResponse")]
        System.Threading.Tasks.Task<System.Data.DataSet> TestAsync(System.Data.DataTable dt, VATViewModel.DTOs.SysDBInfoVMTemp connVM);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/Test1", ReplyAction="http://tempuri.org/ISaleWCF/Test1Response")]
        System.Data.DataSet Test1(System.Data.DataSet set);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/Test1", ReplyAction="http://tempuri.org/ISaleWCF/Test1Response")]
        System.Threading.Tasks.Task<System.Data.DataSet> Test1Async(System.Data.DataSet set);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/ListTest", ReplyAction="http://tempuri.org/ISaleWCF/ListTestResponse")]
        System.Collections.Generic.List<VATViewModel.DTOs.ProductVM> ListTest(System.Collections.Generic.List<VATViewModel.DTOs.ProductVM> list);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/ListTest", ReplyAction="http://tempuri.org/ISaleWCF/ListTestResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<VATViewModel.DTOs.ProductVM>> ListTestAsync(System.Collections.Generic.List<VATViewModel.DTOs.ProductVM> list);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/ArrayTest", ReplyAction="http://tempuri.org/ISaleWCF/ArrayTestResponse")]
        System.Collections.Generic.List<string> ArrayTest(System.Collections.Generic.List<string> list);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/ArrayTest", ReplyAction="http://tempuri.org/ISaleWCF/ArrayTestResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<string>> ArrayTestAsync(System.Collections.Generic.List<string> list);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/ArrayTest2", ReplyAction="http://tempuri.org/ISaleWCF/ArrayTest2Response")]
        System.Data.DataTable ArrayTest2(System.Collections.Generic.List<string> list);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISaleWCF/ArrayTest2", ReplyAction="http://tempuri.org/ISaleWCF/ArrayTest2Response")]
        System.Threading.Tasks.Task<System.Data.DataTable> ArrayTest2Async(System.Collections.Generic.List<string> list);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISaleWCFChannel : VATClient.SaleWCF.ISaleWCF, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SaleWCFClient : System.ServiceModel.ClientBase<VATClient.SaleWCF.ISaleWCF>, VATClient.SaleWCF.ISaleWCF {
        
        public SaleWCFClient() {
        }
        
        public SaleWCFClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SaleWCFClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SaleWCFClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SaleWCFClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void DoWork() {
            base.Channel.DoWork();
        }
        
        public System.Threading.Tasks.Task DoWorkAsync() {
            return base.Channel.DoWorkAsync();
        }
        
        public string GetCategoryName(string itemNo, VATViewModel.DTOs.SysDBInfoVMTemp connVM) {
            return base.Channel.GetCategoryName(itemNo, connVM);
        }
        
        public System.Threading.Tasks.Task<string> GetCategoryNameAsync(string itemNo, VATViewModel.DTOs.SysDBInfoVMTemp connVM) {
            return base.Channel.GetCategoryNameAsync(itemNo, connVM);
        }
        
        public System.Collections.Generic.List<string> SalesInsert(VATViewModel.DTOs.SaleMasterVM MasterVM, System.Collections.Generic.List<VATViewModel.DTOs.SaleDetailVm> DetailVMs, System.Collections.Generic.List<VATViewModel.DTOs.SaleExportVM> ExportDetails, System.Collections.Generic.List<VATViewModel.DTOs.TrackingVM> Trackings, int branchId, VATViewModel.DTOs.SysDBInfoVMTemp connVM) {
            return base.Channel.SalesInsert(MasterVM, DetailVMs, ExportDetails, Trackings, branchId, connVM);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.List<string>> SalesInsertAsync(VATViewModel.DTOs.SaleMasterVM MasterVM, System.Collections.Generic.List<VATViewModel.DTOs.SaleDetailVm> DetailVMs, System.Collections.Generic.List<VATViewModel.DTOs.SaleExportVM> ExportDetails, System.Collections.Generic.List<VATViewModel.DTOs.TrackingVM> Trackings, int branchId, VATViewModel.DTOs.SysDBInfoVMTemp connVM) {
            return base.Channel.SalesInsertAsync(MasterVM, DetailVMs, ExportDetails, Trackings, branchId, connVM);
        }
        
        public System.Data.DataSet Test(System.Data.DataTable dt, VATViewModel.DTOs.SysDBInfoVMTemp connVM) {
            return base.Channel.Test(dt, connVM);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> TestAsync(System.Data.DataTable dt, VATViewModel.DTOs.SysDBInfoVMTemp connVM) {
            return base.Channel.TestAsync(dt, connVM);
        }
        
        public System.Data.DataSet Test1(System.Data.DataSet set) {
            return base.Channel.Test1(set);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> Test1Async(System.Data.DataSet set) {
            return base.Channel.Test1Async(set);
        }
        
        public System.Collections.Generic.List<VATViewModel.DTOs.ProductVM> ListTest(System.Collections.Generic.List<VATViewModel.DTOs.ProductVM> list) {
            return base.Channel.ListTest(list);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.List<VATViewModel.DTOs.ProductVM>> ListTestAsync(System.Collections.Generic.List<VATViewModel.DTOs.ProductVM> list) {
            return base.Channel.ListTestAsync(list);
        }
        
        public System.Collections.Generic.List<string> ArrayTest(System.Collections.Generic.List<string> list) {
            return base.Channel.ArrayTest(list);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.List<string>> ArrayTestAsync(System.Collections.Generic.List<string> list) {
            return base.Channel.ArrayTestAsync(list);
        }
        
        public System.Data.DataTable ArrayTest2(System.Collections.Generic.List<string> list) {
            return base.Channel.ArrayTest2(list);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataTable> ArrayTest2Async(System.Collections.Generic.List<string> list) {
            return base.Channel.ArrayTest2Async(list);
        }
    }
}
