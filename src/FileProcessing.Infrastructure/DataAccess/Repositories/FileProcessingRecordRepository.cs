using FileProcessing.Domain.Entities;
using FileProcessing.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FileProcessing.Domain.DTO;
using AutoMapper;

namespace FileProcessing.Infrastructure.DataAccess.Repositories
{
    public class FileProcessingRecordRepository : IFileProcessingRecordRepository
    {
        private readonly DapperContext _context;
        private readonly IMapper _mapper;
        public FileProcessingRecordRepository(DapperContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FileProcessingRecordDTO>> GetAllProcessedFilesAsync()
        {
            var query = "SELECT dbo.FileProcessingRecords.FileName, dbo.Status.StatusName, dbo.FileProcessingRecords.ProcessStartTime, dbo.FileProcessingRecords.ProcessEndTime, dbo.FileProcessingRecords.Id FROM dbo.FileProcessingRecords INNER JOIN dbo.Status ON dbo.FileProcessingRecords.Status = dbo.Status.Id";
            using (var connection = _context.CreateConnection())
            {
                var _fileProcessingRecordDTO = await connection.QueryAsync<FileProcessingRecordDTO>(query);                
                //var _fileProcessingRecordDTO = _mapper.Map<IEnumerable<FileProcessingRecordDTO>>(fileRecords);
                return _fileProcessingRecordDTO;
            }
        }

        public async Task<IEnumerable<FileProcessingRecord>> GetPendingFileAsync()
        {
            var query = "SELECT * FROM FileProcessingRecords WHERE Status ="+FileStatus.Pending;
            using (var connection = _context.CreateConnection())
            {
                var fileRecords = await connection.QueryAsync<FileProcessingRecord>(query);
                return fileRecords.ToList();
            }
        }

        public async Task<FileProcessingRecord> GetFileByIdAsync(int id)
        {
            var query = "SELECT * FROM FileProcessingRecords WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                FileProcessingRecord? fileProcessingRecord = await connection.QuerySingleOrDefaultAsync<FileProcessingRecord>(query, new { Id = id });
                return fileProcessingRecord;
            }
        }

        public async Task AddFileProcessingRecordAsync(FileProcessingRecord fileRecord)
        {
            var query = "INSERT INTO FileProcessingRecords (FileName, Status, ProcessStartTime, ProcessEndTime) VALUES (@FileName, @Status, @ProcessStartTime, @ProcessEndTime)";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { fileRecord.FileName, fileRecord.Status, fileRecord.ProcessStartTime, fileRecord.ProcessEndTime });
            }
        }

        public async Task UpdateFileProcessingStatusAsync(int status)
        {
            var query = "UPDATE FileProcessingRecords SET Status = @Status";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Status = status });
            }
        }
    }

}
