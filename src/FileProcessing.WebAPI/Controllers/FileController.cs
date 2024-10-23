using FileProcessing.Domain.Entities;
using FileProcessing.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace FileProcessing.WebAPI.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private readonly IFileProcessingRecordRepository _fileRepository;
        private readonly IFileProcessingService _fileProcessingService;
        private readonly ICSVFileProcessor _fileProcessor;
        private readonly ILogger<FileController> _logger;

        public FileController(IFileProcessingRecordRepository fileRepository, IFileProcessingService fileProcessingService, ICSVFileProcessor fileProcessor, ILogger<FileController> logger)
        {
            _fileRepository = fileRepository;
            _fileProcessingService = fileProcessingService;
            _fileProcessor = fileProcessor;
            _logger = logger;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetFileStatus()
        {
            try
            {
                var files = await _fileProcessingService.GetAllProcessedFilesAsync();
                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching file statuses.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching file statuses.");
            }
        }

        [HttpGet("pendingstatus")]
        public async Task<IActionResult> GetPendingStatus()
        {
            try
            {
                var files = await _fileProcessingService.GetAllPendingFilesAsync();
                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending file statuses.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching pending file statuses.");
            }
        }

        [Authorize]
        [HttpPost("process")]
        public async Task<IActionResult> ProcessFile([FromBody] FileProcessingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Processing file: {FilePath}", request.FilePath);
            await _fileProcessingService.ScheduleFileProcessingAsync(request.FilePath);
            return Accepted(new ProcessFileResponse
            {
                FilePath = request.FilePath,
                Message = "File processing scheduled",
                ScheduledAt = DateTime.UtcNow
            });
        }

        [HttpPost("read")]
        public async Task<IActionResult> ReadFile([FromBody] FileProcessingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _fileProcessor.ReadFileAsync(request.FilePath);
                return Ok("File read successfully");
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "File not found: {FilePath}", request.FilePath);
                return NotFound("File not found.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Access denied to file: {FilePath}", request.FilePath);
                return StatusCode(StatusCodes.Status403Forbidden, "Access denied.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading file: {FilePath}", request.FilePath);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error reading file.");
            }
        }

        [HttpPost("updatestatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateFileStatus request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _fileProcessingService.UpdateFileStatus(request.Status);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating file status.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating file status.");
            }
        }
    }

    public class FileProcessingRequest
    {
        [Required(ErrorMessage = "FilePath is required.")]
        public string FilePath { get; set; }
    }

    public class UpdateFileStatus
    {
        [Range(0, 3, ErrorMessage = "Status must be between 0 and 3.")]
        public int Status { get; set; }
    }

    public class ProcessFileResponse
    {
        public string FilePath { get; set; }
        public string Message { get; set; }
        public DateTime ScheduledAt { get; set; }
    }
}
