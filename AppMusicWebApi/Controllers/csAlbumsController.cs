using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Models;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppMusicWApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class csAlbumsController : Controller
    {
        IMusicService _service;
        ILogger<csAlbumsController> _logger;

        //GET: api/albums/read
        [HttpGet()]
        [ActionName("Read")]
        [ProducesResponseType(200, Type = typeof(List<csAlbum>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Read(string flat = "true")
        {
            try
            {
                bool _flat = bool.Parse(flat);
                var _list = await _service.ReadAlbumsAsync(_flat);

                return Ok(_list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        //GET: api/albums/readitem
        [HttpGet()]
        [ActionName("ReadItem")]
        [ProducesResponseType(200, Type = typeof(csAlbum))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadItem(string id, string flat = "false")
        {
            try
            {
                var _id = Guid.Parse(id);
                bool _flat = bool.Parse(flat);
                var mg = await _service.ReadAlbumAsync(_id, _flat);

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

        //GET: api/albums/readitemdto
        [HttpGet()]
        [ActionName("ReadItemDto")]
        [ProducesResponseType(200, Type = typeof(csAlbumCUdto))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadItemDto(string id)
        {
            try
            {
                var _id = Guid.Parse(id);
                var mg = await _service.ReadAlbumAsync(_id, false);

                if (mg == null)
                {
                    return BadRequest($"Item with id {id} does not exist");
                }

                var dto = new csAlbumCUdto(mg);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //DELETE: api/albums/deleteitem/{id}
        [HttpDelete("{id}")]
        [ActionName("DeleteItem")]
        [ProducesResponseType(200, Type = typeof(csAlbum))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> DeleteItem(string id)
        {
            try
            {
                var _id = Guid.Parse(id);
                var mg = await _service.DeleteAlbumAsync(_id);
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

        //Put: api/albums/updateitem/{id}
        [HttpPut("{id}")]
        [ActionName("UpdateItem")]
        [ProducesResponseType(200, Type = typeof(csAlbum))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> UpdateItem(string id, [FromBody] csAlbumCUdto item)
        {
            try
            {
                var _id = Guid.Parse(id);

                if (item.AlbumId != _id)
                    throw new Exception("Id mismatch");

                var mg = await _service.UpdateAlbumAsync(item);

                return Ok(mg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Post: api/albums/createitem
        [HttpPost()]
        [ActionName("CreateItem")]
        [ProducesResponseType(200, Type = typeof(csAlbum))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> CreateItem([FromBody] csAlbumCUdto item)
        {
            try
            {
                var mg = await _service.CreateAlbumAsync(item);

                return Ok(mg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public csAlbumsController(IMusicService service , ILogger<csAlbumsController> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}

