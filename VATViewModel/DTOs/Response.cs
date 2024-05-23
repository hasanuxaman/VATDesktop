using System.Collections.Generic;

namespace VATViewModel.DTOs
{
    public class Response
    {
        public string message { get; set; }
        public List<Item> items { get; set; }

        public bool Succeed
        {
            get
            {
                return message == "000";
            }
        }
    }
}