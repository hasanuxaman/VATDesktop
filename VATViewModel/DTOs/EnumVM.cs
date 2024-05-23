using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class EnumVM
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }


    public class ErrorMessage
    {
        public string ColumnName { get; set; }

        public string  Message { get; set; }
    }
}
