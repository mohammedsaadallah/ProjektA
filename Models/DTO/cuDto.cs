using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
	public class csMusicGroupCUdto
    {
        public Guid MusicGroupId { get; set; }
        public bool Seeded { get; set; } = true;

        public string Name { get; set; }
        public int EstablishedYear { get; set; }

        public enMusicGenre Genre { get; set; }

        public List<Guid> AlbumsId { get; set; } = new List<Guid>();
        public List<Guid> ArtistsId { get; set; } = new List<Guid>();

        public csMusicGroupCUdto()
        {

        }
        public csMusicGroupCUdto(csMusicGroup model)
		{
            this.MusicGroupId = model.MusicGroupId;
            this.Seeded = model.Seeded;
            this.Name = model.Name;
            this.EstablishedYear = model.EstablishedYear;
            this.Genre = model.Genre;

            this.AlbumsId = model?.Albums.Select(a => a.AlbumId).ToList();
            this.ArtistsId = model?.Artists.Select(a => a.ArtistId).ToList();
        }
    }

    public class csAlbumCUdto
    {
        public Guid AlbumId { get; set; }
        public bool Seeded { get; set; } = true;

        public string Name { get; set; }
        public int ReleaseYear { get; set; }
        public long CopiesSold { get; set; }

        //Navigation properties that EFC will use to build relations
        public Guid MusicGroupId { get; set; }


        public csAlbumCUdto()
        {

        }
        public csAlbumCUdto(csAlbum model)
        {
            this.AlbumId = model.AlbumId;
            this.Seeded = model.Seeded;
            this.Name = model.Name;
            this.ReleaseYear = model.ReleaseYear;
            this.CopiesSold = model.CopiesSold;

            this.MusicGroupId = (model.MusicGroup != null) ? model.MusicGroup.MusicGroupId : Guid.Empty;
        }
    }

    public class csArtistCUdto
    {
        public Guid ArtistId { get; set; }
        public bool Seeded { get; set; } = true;

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime? BirthDay { get; set; }

        //Navigation properties that EFC will use to build relations
        public List<Guid> MusicGroupsId { get; set; } = null;


        public csArtistCUdto()
        {

        }
        public csArtistCUdto(csArtist model)
        {
            this.ArtistId = model.ArtistId;
            this.Seeded = model.Seeded;
            this.FirstName = model.FirstName;
            this.LastName = model.LastName;
            this.BirthDay = model.BirthDay;

            this.MusicGroupsId = model.MusicGroups?.Select(a => a.MusicGroupId).ToList();
        }
    }
}

