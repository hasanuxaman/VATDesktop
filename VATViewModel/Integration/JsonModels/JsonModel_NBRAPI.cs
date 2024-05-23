using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.Integration.JsonModels
{
    public class JsonModel_NBRAPI
    {

    }
    public class NBRApiDetails
    {
        public List<NBRApiResult> results { get; set; }
    }

    public class NBRApiMetadata
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string type { get; set; }
    }

    public class NBRApiResult
    {
        
        public NBRApiMetadata __metadata { get; set; }
        public string BIN { get; set; }
        public string PeriodKey { get; set; }
        public string Txt50 { get; set; }
        public string DueDate { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string GoodsServiceCode { get; set; }
        public string GoodsServiceName { get; set; }
        public string SDRate { get; set; }
        public string VATRate { get; set; }
        public string SpecRate06 { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public string ItemID { get; set; }
        public string Note { get; set; }
        public string ManualInput { get; set; }
        public string UnitOfMeasure { get; set; }
        public string TarriffValue { get; set; }
        public string StandardVatRate { get; set; }
        public string TotTate { get; set; }
        public string CategoryID { get; set; }
        public string Category { get; set; }
        public string BanCD { get; set; }
        public string RoutingNumber { get; set; }
        public string Bankn { get; set; }
        public string Brannm { get; set; }
        public string BanCDDatefrom { get; set; }
        public string BanCDDateto { get; set; }
        public string RoutingNumberDatefrom { get; set; }
        public string RoutingNumberDateto { get; set; }
        public string CpcCode { get; set; }
        public string Description { get; set; }
        public string Serial { get; set; }

        public string BoENumber { get; set; }
        public string BoEDate { get; set; }
        public string BoEOffCode { get; set; }
        public string BoEItmNo { get; set; }
        public string CPCCode { get; set; }
        public string GoodService { get; set; }
        public string AssessValue { get; set; }
        public string VAT { get; set; }
        public string SD { get; set; }
        public string AT { get; set; }
        public string PeriodId { get; set; }
        public string NoteNo { get; set; }

        //For submit API
        public string FBTyp { get; set; }
        public string SubmitDate { get; set; }
        public string SubmitTime { get; set; }
        public string Depositor { get; set; }
        public string SubmissionID { get; set; }

    }

    public class Root_NBRAPI
    {
        public NBRApiDetails d { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Application
    {
        public string component_id { get; set; }
        public string service_namespace { get; set; }
        public string service_id { get; set; }
        public string service_version { get; set; }
    }

    public class Error
    {
        public string code { get; set; }
        public Message message { get; set; }
        public Innererror innererror { get; set; }
    }

    public class Errordetail
    {
        public string code { get; set; }
        public string message { get; set; }
        public string propertyref { get; set; }
        public string severity { get; set; }
        public string target { get; set; }
    }

    public class ErrorResolution
    {
        public string SAP_Transaction { get; set; }
        public string SAP_Note { get; set; }
    }

    public class Innererror
    {
        public Application application { get; set; }
        public string transactionid { get; set; }
        public string timestamp { get; set; }
        public ErrorResolution Error_Resolution { get; set; }
        public List<Errordetail> errordetails { get; set; }
    }

    public class Message
    {
        public string lang { get; set; }
        public string value { get; set; }
    }

    public class Root_NBRAPI_Error
    {
        public Error error { get; set; }
    }



}
