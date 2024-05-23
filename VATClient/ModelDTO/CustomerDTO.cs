using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATClient.ModelDTO
{
    public class CustomerDTO//2
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerGroupID { get; set; }
        public string CustomerGroupName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string StartDate { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactPersonTelephone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string TINNo { get; set; }
        public string VATRegistrationNo { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string GroupType { get; set; }



    }

    public class CustomerGroupDTO
    {
        public string CustomerGroupID { get; set; }
        public string CustomerGroupName { get; set; }
        public string CustomerGroupDescription { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string GroupType { get; set; }
    
    }

    public class CustomerSinmgleDTO//2
    {
        public string CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerGroupID { get; set; }
        public string CustomerGroupName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }

        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string TINNo { get; set; }
        public string VATRegistrationNo { get; set; }
        public string ActiveStatus { get; set; }
        public string Country { get; set; }
        public string GroupType { get; set; }



    }
}
