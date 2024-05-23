using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    //public class CurrencyVM
    //{

    //    public string CurrencyId { get; set; }
    //    public string CurrencyName { get; set; }
    //    public string CurrencyCode { get; set; }
    //    public string Country { get; set; }
    //    public string Comments { get; set; }
    //    public string ActiveStatus { get; set; }
       

    //}
    public class CurrencyVM
    {
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public string Country { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string CurrencyMajor { get; set; }
        public string CurrencyMinor { get; set; }
        public string CurrencySymbol { get; set; }
        public string Operation { get; set; }
    }

    public class Currency
    {
        public decimal SubTotal { get; set; }
        public decimal CurrencyValue { get; set; }
    }

//    public class CurrencyConversionVM
//    {
//        public string CurrencyConversionId { get; set; }
//        public string CurrencyCodeFrom { get; set; }
//        public string CurrencyCodeF { get; set; }
//        public string CurrencyNameF { get; set; }
//        public string CurrencyCodeTo { get; set; }
//        public string CurrencyCodeT { get; set; }
//        public string CurrencyNameT { get; set; }
//        public decimal CurrencyRate { get; set; }
//        public string Comments { get; set; }
//        public string ActiveStatus { get; set; }
//        public string ConvertionDate { get; set; }
//}
    public class CurrencyConversionVM
    {
        public string CurrencyConversionId { get; set; }
        public string CurrencyCodeFrom { get; set; }
        public string CurrencyCodeF { get; set; }
        public string CurrencyNameF { get; set; }
        public string CurrencyCodeTo { get; set; }
        public string CurrencyCodeT { get; set; }
        public string CurrencyNameT { get; set; }
        public string CurrencyNameFrom { get; set; }
        public string CurrencyNameTo { get; set; }
        public decimal CurrencyRate { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string ConvertionDate { get; set; }

        public string Operation { get; set; }

    }

    public class Customer
    {
        public string CustomerName { get; set; }
        
    }

    public class ProcessVM
    {
        public bool VAT6_1{ get; set; }
        public bool VAT6_2 { get; set; }

        public VAT6_1ParamVM VAT6_1Model { get; set; }
        public VAT6_2ParamVM VAT6_2Model { get; set; }
        public VAT6_2ParamVM VAT6_2_1Model { get; set; }
    }
}
