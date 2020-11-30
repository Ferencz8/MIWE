using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MIWE.Core.Interfaces;
using MIWE.Data;
using MIWE.Data.Dtos;
using MIWE.Data.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MIWE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobSessionController : ControllerBase
    {
        private IJobSessionRepository _jobSessionRepository;
        private IJobService _jobService;
        public JobSessionController(IJobSessionRepository jobSessionRepository, IJobService jobService)
        {
            _jobSessionRepository = jobSessionRepository;
            _jobService = jobService;
        }

        // GET: api/<JobSessionController>
        [HttpGet]
        public IEnumerable<JobSessionDto> Get()
        {
            return _jobSessionRepository.GetJobSessionDtos();
        }

        [HttpGet("[action]/{sessionId}")]
        public async Task<ActionResult> DownloadResult(Guid sessionId)
        {
            try
            {
                var jobSessionResult = await _jobService.GetJobResultFile(sessionId);
                return File(jobSessionResult.FileAsBytes, jobSessionResult.ContentType);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
