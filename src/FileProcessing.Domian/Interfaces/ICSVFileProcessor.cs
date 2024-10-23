using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.Domain.Interfaces
{
    public interface ICSVFileProcessor
    {
        Task ReadFileAsync(string filePath);
    }
}
