using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Models;
using Services;

namespace AppMusicMVC.Models
{
	public class vwmSeed
    {
        [BindProperty]
        public int NrOfGroups { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "You must enter nr of items to seed")]
        public int NrOfItemsToSeed { get; set; } = 5;

        [BindProperty]
        public bool RemoveSeeds { get; set; } = true;
    }
}

