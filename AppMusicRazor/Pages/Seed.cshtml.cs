using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AppMusicRazor.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace AppMusicRazor.Pages
{
    public class SeedModel : PageModel
    {
        //Just like for WebApi
        IMusicService _service = null;
        ILogger<SeedModel> _logger = null;

        public int NrOfGroups => _nrOfGroups().Result;
        private async Task<int> _nrOfGroups()
        {
            var list = await _service.ReadMusicGroupsAsync(true);
            return list.Count;
        }

        [BindProperty]
        [Required (ErrorMessage = "You must enter nr of items to seed")]
        public int NrOfItems { get; set; } = 5;

        [BindProperty]
        public bool RemoveSeeds { get; set; } = true;

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                if (RemoveSeeds)
                {
                    await _service.RemoveSeed();
                }
                await _service.Seed(NrOfItems);

                return Redirect($"~/ListOfGroups");
            }
            return Page();           
        }

        //Inject services just like in WebApi
        public SeedModel(IMusicService service, ILogger<SeedModel> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}
