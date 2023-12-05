using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Linq;

using Configuration;

namespace Models
{
    public class csArtist : ISeed<csArtist>
    {
        [Key]       // for EFC Code first
        public Guid ArtistId { get; set; }
        public bool Seeded { get; set; } = true;

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime? BirthDay { get; set; }
        public override string ToString() =>
            $"{FirstName} {LastName}";


        //Navigation properties that EFC will use to build relations
        public List<csMusicGroup> MusicGroups { get; set; } = null;

        #region Constructors
        public csArtist()
        {
        }
        public csArtist(csArtistCUdto _dto)
        {
            ArtistId = Guid.NewGuid();
            UpdateFromDTO(_dto);
        }
        #endregion

        #region Update from DTO
        public csArtist UpdateFromDTO(csArtistCUdto _dto)
        {
            this.Seeded = false;
            this.FirstName = _dto.FirstName;
            this.LastName = _dto.LastName;
            this.BirthDay = _dto.BirthDay;

            return this;
        }
        #endregion
        public csArtist Seed(csSeedGenerator sgen)
        {
            return new csArtist
            {
                ArtistId = Guid.NewGuid(),
                FirstName = sgen.FirstName,
                LastName = sgen.LastName,
                BirthDay = (sgen.Bool) ? sgen.DateAndTime(1940, 1990) : null,
                Seeded = true
            };
        }
    }
}

