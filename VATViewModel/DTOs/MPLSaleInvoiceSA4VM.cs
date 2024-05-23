using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;

namespace VATViewModel.DTOs
{
    public class MPLSaleInvoiceSA4VM
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string SalesInvoiceNo { get; set; }
        public string InvoiceDateTime { get; set; }
        public string PortOfDelivery { get; set; }
        public string DeliveryDate { get; set; }
        public string CustomerID { get; set; }
        public string DeliveredFor { get; set; }
        public string DeliveredBy { get; set; }
        public string ProuctDelivered { get; set; }
        public string VesselDockedAt { get; set; }
        public string Started { get; set; }
        public string Finished { get; set; }
        public string PumpingFrom { get; set; }
        public string PumpingTo { get; set; }
        public int TankId { get; set; }
        public string DIPBefore { get; set; }
        public string DIPAfter { get; set; }
        public string LocalVolume { get; set; }
        public string LocalMeasuredVolume { get; set; }
        public string LocalVolumeBefore { get; set; }
        public string LocalVolumeAfter { get; set; }
        public string GrossOilDeliveryQty { get; set; }
        public string IsBondedDutyPaid { get; set; }
        public string MTones { get; set; }
        public string ObservedTankTemp { get; set; }
        public string APIGravityAt { get; set; }
        public string FlashPointPM { get; set; }
        public string ViscosityKinematicCSTA { get; set; }
        public string ViscosityKinematicCSTB { get; set; }
        public string BSW { get; set; }
        public string RemarksObsSPGR { get; set; }
        public string SulphurContentWT { get; set; }
        public string LocalAgent { get; set; }
        public string NextPortofCall { get; set; }
        public string ExportRotationNo { get; set; }
        public string Plag { get; set; }
        public string CustomGuaranteeNo { get; set; }
        public string CustomGuaranteeDate { get; set; }
        public string CustomGuaranteeImoNo { get; set; }
        public string CustomGuaranteeSealMark { get; set; }
        public string AdditionalInfo { get; set; }
        public string BankId { get; set; }
        public string ModeOfPayment { get; set; }
        public string InstrumentNo { get; set; }
        public string InstrumentDate { get; set; }
        public string CRNo { get; set; }
        public string CRDate { get; set; }
        public string SupplierConformation { get; set; }

        public decimal Amount { get; set; }
        public decimal PricePerMTon { get; set; }
        public decimal CostOfOil { get; set; }
        public decimal BargeHireCharge { get; set; }
        public decimal StatutoryCharge { get; set; }
        public decimal CurrentRateFromUSD { get; set; }
        public decimal TotalCostBDT { get; set; }

        public string ReceivedVessel { get; set; }
        public string ReceivedOwner { get; set; }
        public string SupplierOrBargeCaptainCert { get; set; }
        public string Comments { get; set; }


        public string Post { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }

        public string BranchName { get; set; }
        public string CustomerName { get; set; }
        public string DeliveredForName { get; set; }
        public string DeliveredByName { get; set; }
        public string BankName { get; set; }
        public string TankCode { get; set; }
        public string Status { get; set; }


        public List<string> IDs { get; set; }
        public string Operation { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
        public string SelectTop { get; set; }

        [DisplayName("From Date")]
        public string FromDate { get; set; }
        [DisplayName("To Date")]
        public string ToDate { get; set; }


        public List<BranchProfileVM> BranchList { get; set; }

    }

}
