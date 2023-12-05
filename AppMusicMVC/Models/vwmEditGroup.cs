using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

using AppMusicMVC.SeidoHelpers;
using Models;
using Services;

namespace AppMusicMVC.Models
{
	public class vwmEditGroup
    {
        [BindProperty]
        public csMusicGroupIM MusicGroupInput { get; set; }

        //I also use BindProperty to keep between several posts, bound to hidden <input> field
        [BindProperty]
        public string PageHeader { get; set; }

        //Used to populate the dropdown select
        //Notice how it will be populate every time the class is instansiated, i.e. before every get and post
        public List<SelectListItem> GenreItems { set; get; } = new List<SelectListItem>().PopulateSelectList<enMusicGenre>();

        //For Validation
        public reModelValidationResult ValidationResult { get; set; } = new reModelValidationResult(false, null, null);

        #region Input Model
        //InputModel (IM) is locally declared classes that contains ONLY the properties of the Model
        //that are bound to the <form> tag
        //EVERY property must be bound to an <input> tag in the <form>
        //These classes are in center of ModelBinding and Validation
        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }
        public class csArtistIM
        {
            public enStatusIM StatusIM { get; set; }

            public Guid ArtistId { get; set; }

            [Required(ErrorMessage = "You must provide a first name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "You must provide a last name")]
            public string LastName { get; set; }

            //This is because I want to confirm modifications in PostEditAlbum
            [Required(ErrorMessage = "You must provide a first name")]
            public string editFirstName { get; set; }

            [Required(ErrorMessage = "You must provide a last name")]
            public string editLastName { get; set; }

            public csArtist UpdateModel(csArtist model)
            {
                model.ArtistId = this.ArtistId;
                model.FirstName = this.FirstName;
                model.LastName = this.LastName;
                return model;
            }

            public csArtistIM() { }
            public csArtistIM(csArtistIM original)
            {
                StatusIM = original.StatusIM;
                ArtistId = original.ArtistId;
                FirstName = original.FirstName;
                LastName = original.LastName;

                editFirstName = original.editFirstName;
                editLastName = original.editLastName;
            }
            public csArtistIM(csArtist model)
            {
                StatusIM = enStatusIM.Unchanged;
                ArtistId = model.ArtistId;
                FirstName = editFirstName = model.FirstName;
                LastName = editLastName = model.LastName;
            }
        }
        public class csAlbumIM
        {
            public enStatusIM StatusIM { get; set; }

            public Guid AlbumId { get; set; }

            [Required(ErrorMessage = "You must enter an album name")]
            public string AlbumName { get; set; }

            [Range(1900, 2023, ErrorMessage = "You must provide a year between 1900 and 2023")]
            public int ReleaseYear { get; set; }


            [Required(ErrorMessage = "You must enter an album name")]
            public string editAlbumName { get; set; }

            [Range(1900, 2023, ErrorMessage = "You must provide a year between 1900 and 2023")]
            public int editReleaseYear { get; set; }

            public csAlbum UpdateModel(csAlbum model)
            {
                model.AlbumId = this.AlbumId;
                model.Name = this.AlbumName;
                model.ReleaseYear = this.ReleaseYear;
                return model;
            }

            public csAlbumIM() { }
            public csAlbumIM(csAlbumIM original)
            {
                StatusIM = original.StatusIM;
                AlbumId = original.AlbumId;
                AlbumName = original.AlbumName;
                ReleaseYear = original.ReleaseYear;


                editAlbumName = original.editAlbumName;
                editReleaseYear = original.editReleaseYear;
            }
            public csAlbumIM(csAlbum model)
            {
                StatusIM = enStatusIM.Unchanged;
                AlbumId = model.AlbumId;
                AlbumName = editAlbumName = model.Name;
                ReleaseYear = editReleaseYear = model.ReleaseYear;
            }
        }
        public class csMusicGroupIM
        {
            public enStatusIM StatusIM { get; set; }

            public Guid MusicGroupId { get; set; }

            [Required(ErrorMessage = "You must provide a group name")]
            public string Name { get; set; }

            [Range(1900, 2023, ErrorMessage = "You must provide a year between 1900 and 2023")]
            public int EstablishedYear { get; set; }

            //Made nullable and required to force user to make an active selection when creating new group
            [Required(ErrorMessage = "You must select a music genre")]
            public enMusicGenre? Genre { get; set; }

            public List<csAlbumIM> Albums { get; set; } = new List<csAlbumIM>();
            public List<csArtistIM> Artists { get; set; } = new List<csArtistIM>();

            public csMusicGroupIM() { }
            public csMusicGroupIM(csMusicGroup model)
            {
                StatusIM = enStatusIM.Unchanged;
                MusicGroupId = model.MusicGroupId;
                Name = model.Name;
                EstablishedYear = model.EstablishedYear;
                Genre = model.Genre;

                Albums = model.Albums?.Select(m => new csAlbumIM(m)).ToList();
                Artists = model.Artists?.Select(m => new csArtistIM(m)).ToList();
            }

            //to allow a new album being specified and bound in the form
            public csAlbumIM NewAlbum { get; set; } = new csAlbumIM();

            //to allow a new album being specified and bound in the form
            public csArtistIM NewArtist { get; set; } = new csArtistIM();
        }
        #endregion
    }
}

