using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.Domain.DTO
{
    public class FileProcessingRecordDTO
    {
        
        public string? FileName { get; set; }
        public string StatusName { get; set; }
        public DateTime? ProcessStartTime { get; set; }
        public DateTime? ProcessEndTime { get; set; }
    }
}
