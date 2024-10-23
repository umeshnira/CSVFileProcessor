using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.Domain.Entities
{
    public class FileProcessingRecord
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public int Status { get; set; }
        public DateTime? ProcessStartTime { get; set; }
        public DateTime? ProcessEndTime { get; set; }
        public Status StatusDetails { get; set; }
    }

    public class Status
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
    }
}
