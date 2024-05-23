using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VATViewModel.DTOs
{

    public class MPLZoneProfileVM
    {
        public int ZoneID { get; set; }
        public string ZoneCode { get; set; }
        public string ZoneName { get; set; }
        public string ZoneLegalName { get; set; }
        public string Address { get; set; }
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
        public string BanglaLegalName { get; set; }
        public string BanglaAddress { get; set; }

        public string IDs { get; set; }
        public string Operation { get; set; }

        public int BranchId { get; set; }
        public string BranchName { get; set; }

        public int MappingZoneId { get; set; }
        public int MappingBranchId { get; set; }
    }
    public class MPLZoneBranchMappingVM
    {
        public int Id { get; set; }
        public int ZoneId { get; set; }
        public int BranchId { get; set; }

        public string ZoneName { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string IDs { get; set; }
        public string Operation { get; set; }
        public string ModalOperation { get; set; }

        public int MappingZoneId { get; set; }
        public int MappingBranchId { get; set; }
    }
    

}

