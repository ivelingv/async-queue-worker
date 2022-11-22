using AsyncQueueWorker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncQueueWorker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackgroundWorkerController : ControllerBase
    {
        private readonly BackgroundWorkerService 
            _backgroundWorkerService;
        
        public BackgroundWorkerController(
            BackgroundWorkerService backgroundWorkerService)
        {
            _backgroundWorkerService = backgroundWorkerService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartAsync()
        {
            _backgroundWorkerService.Start();

            return Ok(new
            {
                Message = "Backgroung worker started",
                Timestamp =  DateTime.Now.Ticks
            });
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopAsync()
        {
            _backgroundWorkerService.Stop();

            return Ok(new
            {
                Message = "Backgroung worker stopped",
                Timestamp = DateTime.Now.Ticks
            });
        }

        [HttpPost("queue")]
        public async Task<IActionResult> QueueAsync()
        {
            var jobId = Guid
                .NewGuid()
                .ToString();

            _backgroundWorkerService.Queue(jobId);

            return Ok(new
            {
                Message = $"Backgroung worker job [{jobId}] queued",
                Timestamp = DateTime.Now.Ticks
            });
        }

        [HttpGet("status/{jobId}")]
        public async Task<IActionResult> StatusAsync(string jobId)
        {
            var job = _backgroundWorkerService.GetJobStatus(jobId);

            return Ok(new
            {
                Message = $"Backgroung worker job " +
                    $"[{job?.JobId}] status is [{job?.Status}]",

                Timestamp = DateTime.Now.Ticks
            });
        }

        [HttpGet("pending")]
        public async Task<IActionResult> PendingAsync()
        {
            var jobItems = _backgroundWorkerService.Pending();
            return Ok(jobItems);
        }

        [HttpGet("pause")]
        public async Task<IActionResult> PauseAsync()
        {
            _backgroundWorkerService.Pause();
            return Ok(new
            {
                Message = $"Backgroung worker pausing work",
                Timestamp = DateTime.Now.Ticks
            });
        }

        [HttpGet("resume")]
        public async Task<IActionResult> ResumeAsync()
        {
            _backgroundWorkerService.Resume();
            return Ok(new
            {
                Message = $"Backgroung worker resuming work",
                Timestamp = DateTime.Now.Ticks
            });
        }
    }
}
