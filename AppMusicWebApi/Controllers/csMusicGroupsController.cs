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
    public class csMusicGroupsController : Controller
    {
        IMusicService _service;
        ILogger<csMusicGroupsController> _logger;

        //GET: api/musicgroups/read
        [HttpGet()]
        [ActionName("Read")]
        [ProducesResponseType(200, Type = typeof(List<csMusicGroup>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Read(string flat = "true")
        {
            try
            {
                bool _flat = bool.Parse(flat);
                var _list = await _service.ReadMusicGroupsAsync(_flat);

                return Ok(_list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        //GET: api/musicgroups/readitem
        [HttpGet()]
        [ActionName("ReadItem")]
        [ProducesResponseType(200, Type = typeof(csMusicGroup))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadItem(string id, string flat = "false")
        {
            try
            {
                var _id = Guid.Parse(id);
                bool _flat = bool.Parse(flat);
                var mg = await _service.ReadMusicGroupAsync(_id, _flat);

                if (mg == null)
                {
                    return BadRequest($"Item with id {id} does not exist");
                }

                return Ok(mg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //GET: api/musicgroups/readitemdto
        [HttpGet()]
        [ActionName("ReadItemDto")]
        [ProducesResponseType(200, Type = typeof(csMusicGroupCUdto))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadItemDto(string id)
        {
            try
            {
                var _id = Guid.Parse(id);
                var mg = await _service.ReadMusicGroupAsync(_id, false);

                if (mg == null)
                {
                    return BadRequest($"Item with id {id} does not exist");
                }

                var dto = new csMusicGroupCUdto(mg);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //DELETE: api/musicgroups/deleteitem/{id}
        [HttpDelete("{id}")]
        [ActionName("DeleteItem")]
        [ProducesResponseType(200, Type = typeof(csMusicGroup))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> DeleteItem(string id)
        {
            try
            {
                var _id = Guid.Parse(id);
                var mg = await _service.DeleteMusicGroupAsync(_id);
                if (mg == null)
                {
                    return BadRequest($"Item with id {id} does not exist");
                }
                return Ok(mg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Put: api/musicgroups/updateitem/{id}
        [HttpPut("{id}")]
        [ActionName("UpdateItem")]
        [ProducesResponseType(200, Type = typeof(csMusicGroup))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> UpdateItem(string id, [FromBody] csMusicGroupCUdto item)
        {
            try
            {
                var _id = Guid.Parse(id);

                if (item.MusicGroupId != _id)
                    throw new Exception("Id mismatch");

                var mg = await _service.UpdateMusicGroupAsync(item);

                return Ok(mg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Post: api/musicgroups/createitem
        [HttpPost()]
        [ActionName("CreateItem")]
        [ProducesResponseType(200, Type = typeof(csMusicGroup))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> CreateItem([FromBody] csMusicGroupCUdto item)
        {
            try
            {
                var mg = await _service.CreateMusicGroupAsync(item);

                return Ok(mg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public csMusicGroupsController(IMusicService service , ILogger<csMusicGroupsController> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}

