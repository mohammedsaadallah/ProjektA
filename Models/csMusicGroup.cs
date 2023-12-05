using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Linq;

using Configuration;

namespace Models
{
    public enum enMusicGenre {Rock, Blues, Jazz, Metall }

    public class csMusicGroup : ISeed<csMusicGroup>
    {
        [Key]       
        public Guid MusicGroupId { get; set; }
        public bool Seeded { get; set; } = true;

        public string Name { get; set; }
        public int EstablishedYear { get; set; }

        [Required]
        public enMusicGenre Genre { get; set; }

        [Required]
        public string strGenre
        {
            get => Genre.ToString();
            set { }
        }
        public override string ToString() =>
             $"{Name} with {Artists.Count} members was esblished {EstablishedYear} and made {Albums.Count} great albums.";

        //Navigation properties that EFC will use to build relations
        public List<csAlbum> Albums { get; set; } = new List<csAlbum>();
        public List<csArtist> Artists { get; set; } = new List<csArtist>();

        #region Constructors
        public csMusicGroup()
        {
        }
        public csMusicGroup(csMusicGroupCUdto _dto)
        {
            MusicGroupId = Guid.NewGuid();
            UpdateFromDTO(_dto);
        }
        #endregion

        #region Update from DTO
        public csMusicGroup UpdateFromDTO(csMusicGroupCUdto _dto)
        {
            Name = _dto.Name;
            EstablishedYear = _dto.EstablishedYear;
            Genre = _dto.Genre;
            Seeded = false;

            return this;
        }
        #endregion
        public csMusicGroup Seed(csSeedGenerator sgen)
        {
            var mg = new csMusicGroup
            {
                MusicGroupId = Guid.NewGuid(),
                Name = sgen.MusicGroupName,
                EstablishedYear = sgen.Next(1970, 2023),
                Genre = sgen.FromEnum<enMusicGenre>(),
                Seeded = true
            };
            return mg;
        }
    }
}

