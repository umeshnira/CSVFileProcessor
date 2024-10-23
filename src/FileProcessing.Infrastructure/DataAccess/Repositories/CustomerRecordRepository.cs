using FileProcessing.Domain.Entities;
using FileProcessing.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace FileProcessing.Infrastructure.DataAccess.Repositories
{
    public class CustomerRecordRepository : ICustomerRecordRepository
    {
        private readonly DapperContext _context;

        public CustomerRecordRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task AddCustomerRecordAsync(CustomerRecord customer)
        {
            var query = "INSERT INTO CustomerRecord (SlNo, CustomerId, FirstName,LastName, Company,City,Country,Phone1,Phone2,Email,Website) VALUES (@SlNo, @CustomerId, @FirstName,@LastName, @Company,@City,@Country,@Phone1,@Phone2,@Email,@Website)";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { customer.SlNo, customer.CustomerId, customer.FirstName,customer.LastName, customer.Company,customer.City,customer.Country,customer.Phone1,customer.Phone2,customer.Email,customer.Website });
            }
        }
    }
}
