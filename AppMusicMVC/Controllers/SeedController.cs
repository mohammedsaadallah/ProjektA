using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppMusicMVC.Models;
using Services;

namespace AppMusicMVC.Controllers;

public class SeedController : Controller
{
    private readonly ILogger<SeedController> _logger;
    IMusicService _service = null;

    //Inject services just like in WebApi
    public SeedController(IMusicService service, ILogger<SeedController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        //Use the Service
        var mg = await _service.ReadMusicGroupsAsync(false);

        //Create the viewModel
        var vm = new vwmSeed() { NrOfGroups = mg.Count };

        //Render the View
        return View("Seed", vm);
    }


    [HttpPost]
    public async Task<IActionResult> Seed(vwmSeed vm)
    {
        if (ModelState.IsValid)
        {
            if (vm.RemoveSeeds)
            {
                await _service.RemoveSeed();
            }

            await _service.Seed(vm.NrOfItemsToSeed);
            return Redirect($"~/Groups/ListOfGroups");
        }

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

