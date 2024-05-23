using System.Collections.Generic;

namespace VATViewModel.DTOs
{
    public class Body
    {
        public string api { get; set; }
        public string method { get; set; }
        public List<List> list { get; set; }
    }
}