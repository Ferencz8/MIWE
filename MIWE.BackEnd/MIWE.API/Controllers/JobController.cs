using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIWE.Core.Interfaces;
using MIWE.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MIWE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        // GET: api/<JobsController>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(StatusCodeResult))]
        public IActionResult Get()
        {
            try
            {
                var results = _jobService.Get();
                return Ok(results);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // GET api/<JobsController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(StatusCodeResult))]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var scheduledJob = await _jobService.GetById(id);
                return Ok(scheduledJob);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("GetAssociatedJobs")]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(StatusCodeResult))]
        public IActionResult GetAssociatedJobs([FromBody]IEnumerable<Guid> ids)
        {
            try
            {
                var scheduledJobs = _jobService.GetByIds(ids);
                return Ok(scheduledJobs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // POST api/<JobsController>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(StatusCodeResult))]
        public async Task<IActionResult> Post([FromBody] Job scheduledJob)
        {
            try
            {
                var addedJobId = await _jobService.Add(scheduledJob);
                return Ok(addedJobId);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            } 
        }

        // PUT api/<JobsController>/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(StatusCodeResult))]
        public async Task<IActionResult> Put(Guid id, [FromBody] Job scheduledJob)
        {
            try
            {
                await _jobService.Update(scheduledJob);
                return Ok(scheduledJob);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("[action]/{jobId}")]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(StatusCodeResult))]
        public IActionResult Run(Guid jobId)
        {
            try
            {
                _jobService.StartJob(jobId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("[action]/{scheduledJobId}")]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(StatusCodeResult))]
        public IActionResult Stop(Guid scheduledJobId)
        {
            try
            {
                _jobService.StopJob(scheduledJobId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(StatusCodeResult))]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string name)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Content("file not selected");

                // TODO:: needs a check for other files w/ same name

                string finalPath = await _jobService.UploadFile(file, name);
                return new JsonResult(new
                {
                    fileUrl = $"{finalPath}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
