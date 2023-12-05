using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Services;

namespace AppMusicRazor.Pages
{
	public class ViewGroupModel : PageModel
    {
        //Just like for WebApi
        IMusicService _service = null;
        ILogger<ViewGroupModel> _logger = null;

        public csMusicGroup MusicGroup  { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Guid _groupId = Guid.Parse(Request.Query["id"]);
            MusicGroup = await _service.ReadMusicGroupAsync(_groupId, false);

            return Page();
        }

        //Inject services just like in WebApi
        public ViewGroupModel(IMusicService service, ILogger<ViewGroupModel> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}
