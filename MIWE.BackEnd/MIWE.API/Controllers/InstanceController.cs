using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MIWE.Data;
using MIWE.Data.Services;
using MIWE.Data.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MIWE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstanceController : ControllerBase
    {
        private IInstanceService _instanceService;
        private IInstanceRepository _instanceRepository;

        public InstanceController(IInstanceService instanceService, IInstanceRepository instanceRepository)
        {
            _instanceService = instanceService;
            _instanceRepository = instanceRepository;
        }

        // GET: api/<InstanceController>
        [HttpGet]
        public IEnumerable<Instance> Get()
        {
            return _instanceRepository.GetAll();
        }
    }
}
