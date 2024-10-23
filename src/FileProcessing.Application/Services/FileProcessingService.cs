using FileProcessing.Domain.Entities;
using FileProcessing.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using FileProcessing.Domain.DTO;

namespace FileProcessing.Application.Services
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IFileProcessingRecordRepository _fileProcessingRecordRepository;

        public FileProcessingService(IFileProcessingRecordRepository fileProcessingRecordRepository)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "file_processing_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            _fileProcessingRecordRepository = fileProcessingRecordRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task ScheduleFileProcessingAsync(string filePath)
        {
            var messageBody = Encoding.UTF8.GetBytes(filePath);
            _channel.BasicPublish(exchange: "",
                                 routingKey: "file_processing_queue",
                                 basicProperties: null,
                                 body: messageBody);
            if (messageBody.Length > 0)
            {
                var fileRecord = new FileProcessingRecord
                {
                    FileName = Path.GetFileName(filePath),                    
                    ProcessStartTime = DateTime.Now,
                    Status = (int)FileStatus.Pending
                };
                await _fileProcessingRecordRepository.AddFileProcessingRecordAsync(fileRecord);
            }
        }        

        public async Task<IEnumerable<FileProcessingRecord>> GetAllPendingFilesAsync()
        {
            return await _fileProcessingRecordRepository.GetPendingFileAsync();
        }

        public async Task<IEnumerable<FileProcessingRecordDTO>> GetAllProcessedFilesAsync()
        {
            return await _fileProcessingRecordRepository.GetAllProcessedFilesAsync();
        }
       
        public async Task UpdateFileStatus(int status)
        {
            await _fileProcessingRecordRepository.UpdateFileProcessingStatusAsync(status);
        }
    }
}
