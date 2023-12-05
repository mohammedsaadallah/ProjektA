using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Models;
using DbContext;
using Microsoft.EntityFrameworkCore;
using Services;


namespace AppMusicRazor.Pages
{
    public class ListOfGroupsModel : PageModel
    {
        //Just like for WebApi
        IMusicService _service = null;
        ILogger<ListOfGroupsModel> _logger = null;

        public List<csMusicGroup> MusicGroups { get; set; }

        public int NrOfGroups => _nrOfGroups().Result;
        private async Task<int> _nrOfGroups()
        {
            var list = await _service.ReadMusicGroupsAsync(true);
            return list.Count;
        }

        //will execute on a Get request
        public async Task<IActionResult> OnGet()
        {
            //Use the Service
            MusicGroups = await _service.ReadMusicGroupsAsync(false);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteGroup(Guid groupId)
        {
            var mgDeleted = await _service.DeleteMusicGroupAsync(groupId);
            MusicGroups = await _service.ReadMusicGroupsAsync(false);
            return Page();
        }

        /*
        public async Task<IActionResult> OnPostDeleteGroup([FromBody] csModalData modalData)
        {
            var id = Guid.Parse(modalData.postdata);

            var mgDeleted = await _service.DeleteItem(id);
            //Page is not rerendered as the Post is triggered from a butting click outside the form
            //Force the page to be rerendered
            //return Page();
            return RedirectToPage(Request.Path);
        }
        */

        //Inject services just like in WebApi
        public ListOfGroupsModel(IMusicService service, ILogger<ListOfGroupsModel> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}
