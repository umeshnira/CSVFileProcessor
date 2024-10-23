using FileProcessing.Domain.DTO;
using FileProcessing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.Domain.Interfaces
{
    public interface IFileProcessingRecordRepository
    {
        Task<IEnumerable<FileProcessingRecordDTO>> GetAllProcessedFilesAsync();
        Task<FileProcessingRecord> GetFileByIdAsync(int id);
        Task AddFileProcessingRecordAsync(FileProcessingRecord fileRecord);
        Task UpdateFileProcessingStatusAsync(int status);
        Task<IEnumerable<FileProcessingRecord>> GetPendingFileAsync();
    }

}
