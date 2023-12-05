using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Linq;

using Configuration;

namespace Models
{
    public class csAlbum : ISeed<csAlbum>
    {
        [Key]
        public Guid AlbumId { get; set; }
        public bool Seeded { get; set; } = true;

        public string Name { get; set; }
        public int ReleaseYear { get; set; }
        public long CopiesSold { get; set; }

        //Navigation properties that EFC will use to build relations
        public csMusicGroup MusicGroup { get; set; } = null;

        #region Constructors
        public csAlbum()
        {
        }
        public csAlbum(csAlbumCUdto _dto)
        {
            AlbumId = Guid.NewGuid();
            UpdateFromDTO(_dto);
        }
        #endregion

        #region Update from DTO
        public csAlbum UpdateFromDTO(csAlbumCUdto _dto)
        {
            Name = _dto.Name;
            ReleaseYear = _dto.ReleaseYear;
            CopiesSold = _dto.CopiesSold;
            Seeded = false;

            return this;
        }
        #endregion

        public csAlbum Seed(csSeedGenerator sgen)
        {
            var al = new csAlbum
            {
                AlbumId = Guid.NewGuid(),
                Name = sgen.MusicAlbumName,
                CopiesSold = sgen.Next(1000, 1000000),
                ReleaseYear = sgen.Next(1970, 2023),
                Seeded = true
            };
            return al;
        }
    }
}

