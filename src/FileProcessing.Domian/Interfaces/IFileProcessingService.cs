using FileProcessing.Domain.DTO;
using FileProcessing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.Domain.Interfaces
{
    public interface IFileProcessingService
    {
        Task ScheduleFileProcessingAsync(string filePath);
        Task UpdateFileStatus(int status);
        Task<IEnumerable<FileProcessingRecord>> GetAllPendingFilesAsync();
        Task<IEnumerable<FileProcessingRecordDTO>> GetAllProcessedFilesAsync();
    }
}
