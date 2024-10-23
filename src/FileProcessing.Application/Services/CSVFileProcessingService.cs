using FileProcessing.Domain.Entities;
using FileProcessing.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace FileProcessing.Application.Services
{
    public class CSVFileProcessingService : ICSVFileProcessor
    {
        private readonly ICustomerRecordRepository _customerRecordRepository;

        public CSVFileProcessingService(ICustomerRecordRepository customerRecordRepository)
        {
            _customerRecordRepository = customerRecordRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task ReadFileAsync(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<CustomerRecord>();
                    foreach (var record in records)
                    {
                        await _customerRecordRepository.AddCustomerRecordAsync(record);
                    }
                }                
            }
            catch (Exception ex)
            {
                //_logger.Information($"Error processing file {filePath}: {ex.Message}");
            }
        }
    }
}
