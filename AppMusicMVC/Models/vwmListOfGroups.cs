using System;
using Models;
using Services;

namespace AppMusicMVC.Models
{
	public class vwmListOfGroups
    {
        public List<csMusicGroup> MusicGroups { get; set; }
        public int NrOfGroups => MusicGroups.Count;
    }
}

