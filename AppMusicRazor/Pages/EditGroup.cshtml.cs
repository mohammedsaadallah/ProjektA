using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using AppMusicRazor.SeidoHelpers;
using Models;
using Newtonsoft.Json;
using Services;

namespace AppMusicRazor.Pages
{
    public class EditGroupModel : PageModel
    {
        //Just like for WebApi
        IMusicService _service = null;
        ILogger<EditGroupModel> _logger = null;

        //InputModel (IM) is locally declared classes that contains ONLY the properties of the Model
        //that are bound to the <form> tag
        //EVERY property must be bound to an <input> tag in the <form>
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

        #region HTTP Requests
        public async Task<IActionResult> OnGet()
        {
            if (Guid.TryParse(Request.Query["id"], out Guid _groupId))
            {
                //Read a music group from 
                var mg = await _service.ReadMusicGroupAsync(_groupId, false);

                //Populate the InputModel from the music group
                MusicGroupInput = new csMusicGroupIM(mg);
                PageHeader = "Edit details of a music group";

            }
            else
            {
                //Create an empty music group
                MusicGroupInput = new csMusicGroupIM();
                MusicGroupInput.StatusIM = enStatusIM.Inserted;
                MusicGroupInput.Genre = null;

                PageHeader = "Create a new a music group";
            }

            return Page();
        }

        public IActionResult OnPostDeleteArtist(Guid artistId)
        {
            //Set the Artist as deleted, it will not be rendered
            MusicGroupInput.Artists.First(a => a.ArtistId == artistId).StatusIM = enStatusIM.Deleted;

            return Page();
        }

        public IActionResult OnPostAddArtist()
        {
            string[] keys = { "MusicGroupInput.NewArtist.FirstName",
                              "MusicGroupInput.NewArtist.LastName"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                ValidationResult = validationResult;
                return Page();
            }

            //Set the Artist as Inserted, it will later be inserted in the database
            MusicGroupInput.NewArtist.StatusIM = enStatusIM.Inserted;

            //Need to add a temp Guid so it can be deleted and editited in the form
            //A correct Guid will be created by the DTO when Inserted into the database
            MusicGroupInput.NewArtist.ArtistId = Guid.NewGuid();

            //Add it to the Input Models artists
            MusicGroupInput.Artists.Add(new csArtistIM(MusicGroupInput.NewArtist));

            //Clear the NewArtist so another album can be added
            MusicGroupInput.NewArtist = new csArtistIM();

            return Page();
        }

        public IActionResult OnPostEditArtist(Guid artistId)
        {
            int idx = MusicGroupInput.Artists.FindIndex(a => a.ArtistId == artistId);
            string[] keys = { $"MusicGroupInput.Artists[{idx}].editFirstName",
                            $"MusicGroupInput.Artists[{idx}].editLastName"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                ValidationResult = validationResult;
                return Page();
            }

            //Set the Album as Modified, it will later be updated in the database
            var a = MusicGroupInput.Artists.First(a => a.ArtistId == artistId);
            if (a.StatusIM != enStatusIM.Inserted)
            {
                a.StatusIM = enStatusIM.Modified;
            }

            //Implement the changes
            a.FirstName = a.editFirstName;
            a.LastName = a.editLastName;

            return Page();
        }


        public IActionResult OnPostDeleteAlbum(Guid albumId)
        {
            //Set the Album as deleted, it will not be rendered
            MusicGroupInput.Albums.First(a => a.AlbumId == albumId).StatusIM = enStatusIM.Deleted;

            return Page();
        }

        public IActionResult OnPostAddAlbum()
        {
            string[] keys = { "MusicGroupInput.NewAlbum.ReleaseYear",
                              "MusicGroupInput.NewAlbum.AlbumName"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                ValidationResult = validationResult;
                return Page();
            }

            //Set the Album as Inserted, it will later be inserted in the database
            MusicGroupInput.NewAlbum.StatusIM = enStatusIM.Inserted;

            //Need to add a temp Guid so it can be deleted and editited in the form
            //A correct Guid will be created by the DTO when Inserted into the database
            MusicGroupInput.NewAlbum.AlbumId = Guid.NewGuid();

            //Add it to the Input Models albums
            MusicGroupInput.Albums.Add(new csAlbumIM(MusicGroupInput.NewAlbum));

            //Clear the NewAlbum so another album can be added
            MusicGroupInput.NewAlbum = new csAlbumIM();

            return Page();
        }

        public IActionResult OnPostEditAlbum(Guid albumId)
        {
            int idx = MusicGroupInput.Albums.FindIndex(a => a.AlbumId == albumId);
            string[] keys = { $"MusicGroupInput.Albums[{idx}].editAlbumName",
                            $"MusicGroupInput.Albums[{idx}].editReleaseYear"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                ValidationResult = validationResult;
                return Page();
            }

            //Set the Album as Modified, it will later be updated in the database
            var a = MusicGroupInput.Albums.First(a => a.AlbumId == albumId);
            if (a.StatusIM != enStatusIM.Inserted)
            {
                a.StatusIM = enStatusIM.Modified;
            }

            //Implement the changes
            a.AlbumName = a.editAlbumName;
            a.ReleaseYear = a.editReleaseYear;

            return Page();
        }

        public async Task<IActionResult> OnPostUndo()
        {
            //Reload Music group from Database
            var mg = await _service.ReadMusicGroupAsync(MusicGroupInput.MusicGroupId, false);

            //Repopulate the InputModel
            MusicGroupInput = new csMusicGroupIM(mg);
            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            string[] keys = { "MusicGroupInput.Name",
                              "MusicGroupInput.EstablishedYear",
                              "MusicGroupInput.Genre"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                ValidationResult = validationResult;
                return Page();
            }

            //This is where the music plays
            //First, are we creating a new Music group or editing another
            if (MusicGroupInput.StatusIM == enStatusIM.Inserted)
            {
                var mgDto = new csMusicGroupCUdto();

                //create the music group in the database
                mgDto.Name = MusicGroupInput.Name;
                mgDto.EstablishedYear = MusicGroupInput.EstablishedYear;
                mgDto.Genre = MusicGroupInput.Genre.Value;

                var newMg = await _service.CreateMusicGroupAsync(mgDto);
                //get the newly created MusicGroupId
                MusicGroupInput.MusicGroupId = newMg.MusicGroupId;
            }

            //Do all updates for Albums
            csMusicGroup mg = await SaveAlbums();

            // Do all updates for Artists
            mg = await SaveArtists();

            //Finally, update the MusicGroup itself
            mg.Name = MusicGroupInput.Name;
            mg.EstablishedYear = MusicGroupInput.EstablishedYear;
            mg.Genre = MusicGroupInput.Genre.Value;
            mg = await _service.UpdateMusicGroupAsync(new csMusicGroupCUdto(mg));

            if (MusicGroupInput.StatusIM == enStatusIM.Inserted)
            {
                return Redirect($"~/ListOfGroups");
            }

            return Redirect($"~/ViewGroup?id={MusicGroupInput.MusicGroupId}");
        }
        #endregion

        #region InputModel Albums and Artists saved to database
        private async Task<csMusicGroup> SaveAlbums()
        {
            //Check if there are deleted albums, if so simply remove them
            var deletedAlbums = MusicGroupInput.Albums.FindAll(a => (a.StatusIM == enStatusIM.Deleted));
            foreach (var item in deletedAlbums)
            {
                //Remove from the database
                await _service.DeleteAlbumAsync(item.AlbumId);
            }

            //Check if there are any new albums added, if so create them in the database
            var newAlbums = MusicGroupInput.Albums.FindAll(a => (a.StatusIM == enStatusIM.Inserted));
            foreach (var item in newAlbums)
            {
                //Create the corresposning model and CUdto objects
                var model = item.UpdateModel(new csAlbum());
                var cuDto = new csAlbumCUdto(model) { MusicGroupId = MusicGroupInput.MusicGroupId };

                //Set the relationships of a newly created item and write to database
                cuDto.MusicGroupId = MusicGroupInput.MusicGroupId;
                model = await _service.CreateAlbumAsync(cuDto);
            }

            //To update modified and deleted albums, lets first read the original
            //Note that now the deleted albums will be removed and created albums will be nicely included
            var mg = await _service.ReadMusicGroupAsync(MusicGroupInput.MusicGroupId, false);


            //Check if there are any modified albums , if so update them in the database
            var modifiedAlbums = MusicGroupInput.Albums.FindAll(a => (a.StatusIM == enStatusIM.Modified));
            foreach (var item in modifiedAlbums)
            {
                var model = mg.Albums.First(a => a.AlbumId == item.AlbumId);

                //Update the model from the InputModel
                model = item.UpdateModel(model);

                //Updatet the model in the database
                model = await _service.UpdateAlbumAsync(new csAlbumCUdto(model) { MusicGroupId = MusicGroupInput.MusicGroupId});
            }

            return mg;
        }
        private async Task<csMusicGroup> SaveArtists()
        {
            //Check if there are deleted artists, if so simply remove them
            var deletedArtists = MusicGroupInput.Artists.FindAll(a => (a.StatusIM == enStatusIM.Deleted));
            foreach (var item in deletedArtists)
            {
                //Remove from the database
                await _service.DeleteArtistAsync(item.ArtistId);
            }

            //Check if there are any new artist added, if so create them in the database
            var newArtists = MusicGroupInput.Artists.FindAll(a => (a.StatusIM == enStatusIM.Inserted));
            foreach (var item in newArtists)
            {
                //Create the corresposning model and CUdto objects
                var model = item.UpdateModel(new csArtist());
                var cuDto = new csArtistCUdto(model);

                //Set the relationships of a newly created item and write to database
                cuDto.MusicGroupsId = new List<Guid>();
                cuDto.MusicGroupsId.Add(MusicGroupInput.MusicGroupId);

                //Create if does not exists. 
                model = await _service.UpsertArtistAsync(cuDto);
            }

            //To update modified and deleted Artists, lets first read the original
            //Note that now the deleted artists will be removed and created artists will be nicely included
            var mg = await _service.ReadMusicGroupAsync(MusicGroupInput.MusicGroupId, false);


            //Check if there are any modified artists , if so update them in the database
            var modifiedArtists = MusicGroupInput.Artists.FindAll(a => (a.StatusIM == enStatusIM.Modified));
            foreach (var item in modifiedArtists)
            {
                var model = mg.Artists.First(a => a.ArtistId == item.ArtistId);

                //Update the model from the InputModel
                model = item.UpdateModel(model);

                //Updatet the model in the database
                model = await _service.UpdateArtistAsync(new csArtistCUdto(model));
            }

            return mg;
        }
        #endregion

        #region Constructors
        //Inject services just like in WebApi
        public EditGroupModel(IMusicService service, ILogger<EditGroupModel> logger)
        {
            _service = service;
            _logger = logger;
        }
        #endregion

        #region Input Model
        //InputModel (IM) is locally declared classes that contains ONLY the properties of the Model
        //that are bound to the <form> tag
        //EVERY property must be bound to an <input> tag in the <form>
        //These classes are in center of ModelBinding and Validation
        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted}
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

            [Range (1900, 2023, ErrorMessage = "You must provide a year between 1900 and 2023")]
            public int EstablishedYear { get; set; }

            //Made nullable and required to force user to make an active selection when creating new group
            [Required(ErrorMessage = "You must select a music genre")]
            public enMusicGenre? Genre { get; set; }

            public List<csAlbumIM> Albums { get; set; } = new List<csAlbumIM>();
            public List<csArtistIM> Artists { get; set; } = new List<csArtistIM>();

            public csMusicGroupIM() {}
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
