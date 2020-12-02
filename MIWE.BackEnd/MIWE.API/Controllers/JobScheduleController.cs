using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIWE.Data.Dtos;
using MIWE.Data.Entities;
using MIWE.Data.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MIWE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobScheduleController : ControllerBase
    {

        private IJobScheduleRepository _jobScheduleRepository;

        public JobScheduleController(IJobScheduleRepository jobScheduleRepository)
        {
            _jobScheduleRepository = jobScheduleRepository;
        }

        // GET: api/<JobScheduleController>
        [HttpGet]
        public IEnumerable<JobSchedulePipelineDto> Get()
        {
            return _jobScheduleRepository.GetJobSchedulesWithPipeline().ToList();
        }

        // GET api/<JobScheduleController>/5
        [HttpGet("{id}")]
        public async Task<JobSchedule> Get(Guid id)
        {
            return await _jobScheduleRepository.GetById(id);
        }

        // POST api/<JobScheduleController>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(StatusCodeResult))]
        public async Task<IActionResult> Post([FromBody] JobSchedule jobSchedule)
        {
            try
            {
                var addedEntity = await _jobScheduleRepository.Create(jobSchedule);

                return Ok(addedEntity.Id);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // PUT api/<JobScheduleController>/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(StatusCodeResult))]
        public async Task<IActionResult> Put(Guid id, [FromBody] JobSchedule jobSchedule)
        {
            try
            {
                await _jobScheduleRepository.Update(jobSchedule);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // DELETE api/<JobScheduleController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
