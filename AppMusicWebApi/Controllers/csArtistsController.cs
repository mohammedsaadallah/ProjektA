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
    public class csArtistsController : Controller
    {
        IMusicService _service;
        ILogger<csArtistsController> _logger;


        //GET: api/artists/read
        [HttpGet()]
        [ActionName("Read")]
        [ProducesResponseType(200, Type = typeof(List<csArtist>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Read(string flat = "true")
        {
            try
            {
                bool _flat = bool.Parse(flat);
                var _list = await _service.ReadArtistsAsync(_flat);

                return Ok(_list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        //GET: api/artists/readitem
        [HttpGet()]
        [ActionName("ReadItem")]
        [ProducesResponseType(200, Type = typeof(csArtist))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadItem(string id, string flat = "false")
        {
            try
            {
                var _id = Guid.Parse(id);
                bool _flat = bool.Parse(flat);
                var mg = await _service.ReadArtistsync(_id, _flat);

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

        //GET: api/artists/readitemdto
        [HttpGet()]
        [ActionName("ReadItemDto")]
        [ProducesResponseType(200, Type = typeof(csArtistCUdto))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadItemDto(string id)
        {
            try
            {
                var _id = Guid.Parse(id);
                var mg = await _service.ReadArtistsync(_id, false);

                if (mg == null)
                {
                    return BadRequest($"Item with id {id} does not exist");
                }

                var dto = new csArtistCUdto(mg);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //DELETE: api/artists/deleteitem/{id}
        [HttpDelete("{id}")]
        [ActionName("DeleteItem")]
        [ProducesResponseType(200, Type = typeof(csArtist))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> DeleteItem(string id)
        {
            try
            {
                var _id = Guid.Parse(id);
                var mg = await _service.DeleteArtistAsync(_id);
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

        //Put: api/artists/updateitem/{id}
        [HttpPut("{id}")]
        [ActionName("UpdateItem")]
        [ProducesResponseType(200, Type = typeof(csArtist))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> UpdateItem(string id, [FromBody] csArtistCUdto item)
        {
            try
            {
                var _id = Guid.Parse(id);

                if (item.ArtistId != _id)
                    throw new Exception("Id mismatch");

                var mg = await _service.UpdateArtistAsync(item);

                return Ok(mg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Upsert = Update or Insert
        //Post: api/artists/upsertitem
        [HttpPost()]
        [ActionName("UpsertItem")]
        [ProducesResponseType(200, Type = typeof(csArtist))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> UpsertItem([FromBody] csArtistCUdto item)
        {
            try
            {
                var mg = await _service.UpsertArtistAsync(item);

                return Ok(mg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public csArtistsController(IMusicService service , ILogger<csArtistsController> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}

