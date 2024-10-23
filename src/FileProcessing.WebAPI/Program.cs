using FileProcessing.Infrastructure.DataAccess;
using FileProcessing.Domain.Interfaces;
using FileProcessing.Infrastructure.DataAccess.Repositories;
using FileProcessing.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog.Events;
using Serilog;
using Microsoft.AspNetCore.Hosting;
using FileProcessing.Application.Mapper;

namespace FileProcessing.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Serilog Configuration
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .Enrich.FromLogContext()                            
                            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                            .CreateLogger();            

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // JWT Authentication Configuration
            var key = builder.Configuration["Jwt:Key"];
            var issuer = builder.Configuration["Jwt:Issuer"];
            var audience = builder.Configuration["Jwt:Audience"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.Zero  // Enforce strict expiration
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            // Register services using built-in DI
            builder.Services.AddSingleton<DapperContext>();
            builder.Services.AddScoped<IFileProcessingService, FileProcessingService>();
            builder.Services.AddScoped<ICSVFileProcessor, CSVFileProcessingService>();
            builder.Services.AddScoped<IFileProcessingRecordRepository, FileProcessingRecordRepository>();
            builder.Services.AddScoped<ICustomerRecordRepository, CustomerRecordRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRecordRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
