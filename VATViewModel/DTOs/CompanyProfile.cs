using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    //public class CompanyProfileVM
    //{
    //    //
    //    public string CompanyID { get; set; }
    //    public string CompanyName { get; set; }
    //    public string CompanyLegalName { get; set; }
    //    public string Address1 { get; set; }
    //    public string Address2 { get; set; }
    //    public string Address3 { get; set; }
    //    public string City { get; set; }
    //    public string ZipCode { get; set; }
    //    public string TelephoneNo { get; set; }
    //    public string FaxNo { get; set; }
    //    public string Email { get; set; }
    //    public string ContactPerson { get; set; }
    //    public string ContactPersonDesignation { get; set; }
    //    public string ContactPersonTelephone { get; set; }
    //    public string ContactPersonEmail { get; set; }
    //    public string TINNo { get; set; }
    //    public string VatRegistrationNo { get; set; }
    //    public string Comments { get; set; }
    //    public string ActiveStatus { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }
    //    public string LastModifiedBy { get; set; }
    //    public string LastModifiedOn { get; set; }
    //    public string StartDateTime { get; set; }
    //    public string FYearStart { get; set; }
    //    public string FYearEnd { get; set; }
    //    public string Tom { get; set; }       //encrypted companyName
    //    public string Jary { get; set; }      //encrypted CompanyLegalName
    //    public string Miki { get; set; }      //encrypted VatRegistrationNo
    //    public string Mouse { get; set; }     //encrypted ProcessorId
     
    //}
    public class CompanyProfileVM
    {
        //
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLegalName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactPersonTelephone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string TINNo { get; set; }
        public string VatRegistrationNo { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string StartDateTime { get; set; }
        public string FYearStart { get; set; }
        public string FYearEnd { get; set; }
        public string Tom { get; set; }       //encrypted companyName
        public string Jary { get; set; }      //encrypted CompanyLegalName
        public string Miki { get; set; }      //encrypted VatRegistrationNo
        public string Mouse { get; set; }     //encrypted ProcessorId
        public string Operation { get; set; }
        public string IsVDSWithHolder { get; set; }

        public string BusinessNature { get; set; }
        public string AccountingNature { get; set; }
        public string BIN { get; set; }
        public string License { get; set; } //encrypted License
        public bool IsSymphonyUser { get; set; }
        public string Section { get; set; }
        public string CompanyType { get; set; }



    }


    public class NBR_APIVM
    {
        public NBR_APIVM()
        {
            CheckedDocuments = new List<DocumentVM>();
        }

        public string CurrentUserId { get; set; }

        public DateTime Period { get; set; }
        public string TransactionType { get; set; }

        public List<DocumentVM> CheckedDocuments { get; set; }
        public List<AttachedFileVM> AttachedFiles { get; set; }
    }

    public class DocumentVM
    {
        public string ATT_DOCTYPE { get; set; }
        public string TEXT { get; set; }

        public string NOTES { get; set; }

        public string CHKBOX { get; set; }

    }

    public class AttachedFileVM
    {
        public AttachedFileVM()
        {
            DOCTYPE = "ATTA";
        }

        public string FILENAME { get; set; }
        public string CONTENT { get; set; }

        public string FILETYPE { get; set; }

        public string DOCTYPE { get; set; }

    }
}
