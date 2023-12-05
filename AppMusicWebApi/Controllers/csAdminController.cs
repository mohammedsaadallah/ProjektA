using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Models;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppMusicWApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class csAdminController : Controller
    {
        IMusicService _service;
        ILogger<csAdminController> _logger;

        //GET: api/musicgroups/seed?count={count}
        [HttpGet()]
        [ActionName("Seed")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Seed(string count)
        {
            try
            {
                int _count = int.Parse(count);

                int cnt = await _service.Seed(_count);
                return Ok(cnt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

        //GET: api/musicgroups/removeseed
        [HttpGet()]
        [ActionName("RemoveSeed")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> RemoveSeed()
        {         
            try
            {
                int _count = await _service.RemoveSeed();
                return Ok(_count);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }

        }

        public csAdminController(IMusicService service , ILogger<csAdminController> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}

