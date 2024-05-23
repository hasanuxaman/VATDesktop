using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATClient.ModelDTO
{
    public class VendorDTO
    {
        public string VendorID { get; set; }  //all
        public string VendorName { get; set; }  //all
        public string VendorGroupID { get; set; }  //all
        public string Address1 { get; set; }  //all
        public string Address2 { get; set; }  //all
        public string Address3 { get; set; }  //all
        public string City { get; set; }  //all
        public string TelephoneNo { get; set; }  //all
        public string FaxNo { get; set; }  //all
        public string Email { get; set; }  //all
        public string StartDateTime { get; set; }  //all
        public string ContactPerson { get; set; }  //all
        public string ContactPersonDesignation { get; set; }  //all
        public string ContactPersonTelephone { get; set; }  //all
        public string ContactPersonEmail { get; set; }  //all
        public string VATRegistrationNo { get; set; }  //all
        public string TINNo { get; set; }  //all
        public string Comments { get; set; }  //all
        public string ActiveStatus { get; set; }  //all
        public string CreatedBy { get; set; }  //all
        public string CreatedOn { get; set; }  //all
        public string LastModifiedBy { get; set; }  //all
        public string LastModifiedOn { get; set; }  //all
        public string Country { get; set; }  //all
        public string Info2 { get; set; }  //all
        public string Info3 { get; set; }  //all
        public string Info4 { get; set; }  //all
        public string Info5 { get; set; }  //all

    }
    public class VendorMiniDTO
    {
        public string VendorID { get; set; }  //all
        public string VendorCode { get; set; }  //all
        
        public string VendorName { get; set; }  //all
        public string VendorGroupID { get; set; }  //all
        public string VendorGroupName { get; set; }  //all
        public string Address1 { get; set; }  //all
        public string Address2 { get; set; }  //all
        public string Address3 { get; set; }  //all

        public string TelephoneNo { get; set; }  //all
        public string FaxNo { get; set; }  //all
        public string Email { get; set; }  //all
        public string VATRegistrationNo { get; set; }  //all
        public string TINNo { get; set; }  //all
        public string ActiveStatus { get; set; }  //all
        public string Country { get; set; }  //all
        public string GroupType { get; set; }  //all



    }
    public class VendorGroupDTO
    {
        public string VendorGroupID { get; set; }
        public string VendorGroupName { get; set; }
        public string VendorGroupDescription { get; set; }
        public string GroupType { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Info2 { get; set; }  
        public string Info3 { get; set; }  
        public string Info4 { get; set; }  
        public string Info5 { get; set; }  

    }
}
