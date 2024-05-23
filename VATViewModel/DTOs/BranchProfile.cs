using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{

    public class BranchProfileVM
    {
 

        public int BranchID { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string BranchLegalName { get; set; }
        public string BranchBanglaLegalName { get; set; }
        public string Address { get; set; }
        public string BanglaAddress { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactPersonTelephone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string VatRegistrationNo { get; set; }
        public string BIN { get; set; }
        public string TINNo { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public bool IsArchive { get; set; }
        public string Operation { get; set; }


        public string IP { get; set; }

        public string DbName { get; set; }

        public string Id { get; set; }

        public string Pass { get; set; }

        public string IsWCF { get; set; }

        public bool IsCentral { get; set; }


        public string IntegrationCode { get; set; }
        public string DetailsAddress { get; set; }
        public int DetailsSL { get; set; }



    }

    public class ColumnDetails
    {
        public string COLUMN_NAME { get; set; }
    }
}
