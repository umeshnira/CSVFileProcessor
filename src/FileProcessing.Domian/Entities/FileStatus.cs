using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.Domain.Entities
{
    public enum FileStatus
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Error = 4
    }
}
