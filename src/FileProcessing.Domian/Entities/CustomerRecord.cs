using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.Domain.Entities
{
    public class CustomerRecord
    {        
        public int SlNo { get; set; }
        public string? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Company { get; set; }
        public string?  City { get; set; }
        public string? Country { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? Email { get; set; }        
        public string? Website { get; set; }

    }
}
