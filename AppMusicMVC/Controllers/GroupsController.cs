using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AppMusicMVC.Models;
using AppMusicMVC.SeidoHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models;
using Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppMusicMVC.Controllers
{
    public class GroupsController : Controller
    {
        //Just like for WebApi
        IMusicService _service = null;
        ILogger<GroupsController> _logger = null;

        #region ListOfGroups handling
        public async Task<IActionResult> ListOfGroups()
        {
            //Use the Service
            var mg = await _service.ReadMusicGroupsAsync(false);

            //Create the viewModel
            var vm = new vwmListOfGroups() { MusicGroups = mg};

            //Render the View
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGroup(Guid groupId)
        {
            //Use the Service
            var mgDeleted = await _service.DeleteMusicGroupAsync(groupId);
            var mg = await _service.ReadMusicGroupsAsync(false);

            //Create the viewModel
            var vm = new vwmListOfGroups() { MusicGroups = mg };

            //Render the View
            //Note that View(vm) would try to render a View named DeleteGroup
            return View("ListOfGroups", vm);
        }
        #endregion

        #region ViewGroup handling
        public async Task<IActionResult> ViewGroup(Guid id) 
        {
            Guid _groupId = id;
            //Guid _groupId = Guid.Parse(Request.Query["id"]);   //an alternative

            //use the Service
            var mg = await _service.ReadMusicGroupAsync(_groupId, false);

            //Create the viewModel
            var vm = new vwmViewGroup() { MusicGroup = mg };

            //Render the View
            return View(vm);
        }
        #endregion

        #region EditGroup Handling
        public async Task<IActionResult> EditGroup(Guid? id)
        {
            Guid? _groupId = id;
            if (_groupId.HasValue)
            {
                //Read a music group from 
                var mg = await _service.ReadMusicGroupAsync(_groupId.Value, false);

                //Populate the InputModel from the music group
                //Create the viewModel
                var vm = new vwmEditGroup()
                {
                    MusicGroupInput = new vwmEditGroup.csMusicGroupIM(mg),
                    PageHeader = "Edit details of a music group"
                };


                return View(vm);
            }
            else
            {
                //Create an empty music group
                //Create the viewModel
                var vm = new vwmEditGroup()
                {
                    MusicGroupInput = new vwmEditGroup.csMusicGroupIM(),
                    PageHeader = "Create a new a music group"
                };
                vm.MusicGroupInput.StatusIM = vwmEditGroup.enStatusIM.Inserted;
                vm.MusicGroupInput.Genre = null;

                return View(vm);
            }


        }

        [HttpPost]
        public IActionResult DeleteArtist(Guid artistId, vwmEditGroup vm)
        {
            //Set the Artist as deleted, it will not be rendered
            vm.MusicGroupInput.Artists.First(a => a.ArtistId == artistId).StatusIM = vwmEditGroup.enStatusIM.Deleted;

            return View("EditGroup",vm);
        }

        [HttpPost]
        public IActionResult DeleteAlbum(Guid albumId, vwmEditGroup vm)
        {
            //Set the Album as deleted, it will not be rendered
            vm.MusicGroupInput.Albums.First(a => a.AlbumId == albumId).StatusIM = vwmEditGroup.enStatusIM.Deleted;

            return View("EditGroup", vm);
        }

        [HttpPost]
        public IActionResult AddArtist(vwmEditGroup vm)
        {
            string[] keys = { "MusicGroupInput.NewArtist.FirstName",
                              "MusicGroupInput.NewArtist.LastName"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                vm.ValidationResult = validationResult;
                return View("EditGroup", vm);
            }

            //Set the Artist as Inserted, it will later be inserted in the database
            vm.MusicGroupInput.NewArtist.StatusIM = vwmEditGroup.enStatusIM.Inserted;

            //Need to add a temp Guid so it can be deleted and editited in the form
            //A correct Guid will be created by the DTO when Inserted into the database
            vm.MusicGroupInput.NewArtist.ArtistId = Guid.NewGuid();

            //Add it to the Input Models artists
            vm.MusicGroupInput.Artists.Add(new vwmEditGroup.csArtistIM(vm.MusicGroupInput.NewArtist));

            //Clear the NewArtist so another album can be added
            vm.MusicGroupInput.NewArtist = new vwmEditGroup.csArtistIM();

            return View("EditGroup", vm);
        }

        [HttpPost]
        public IActionResult AddAlbum(vwmEditGroup vm)
        {
            string[] keys = { "MusicGroupInput.NewAlbum.ReleaseYear",
                              "MusicGroupInput.NewAlbum.AlbumName"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                vm.ValidationResult = validationResult;
                return View("EditGroup", vm);
            }

            //Set the Album as Inserted, it will later be inserted in the database
            vm.MusicGroupInput.NewAlbum.StatusIM = vwmEditGroup.enStatusIM.Inserted;

            //Need to add a temp Guid so it can be deleted and editited in the form
            //A correct Guid will be created by the DTO when Inserted into the database
            vm.MusicGroupInput.NewAlbum.AlbumId = Guid.NewGuid();

            //Add it to the Input Models albums
            vm.MusicGroupInput.Albums.Add(new vwmEditGroup.csAlbumIM(vm.MusicGroupInput.NewAlbum));

            //Clear the NewAlbum so another album can be added
            vm.MusicGroupInput.NewAlbum = new vwmEditGroup.csAlbumIM();

            return View("EditGroup", vm);
        }

        [HttpPost]
        public IActionResult EditArtist(Guid artistId, vwmEditGroup vm)
        {
            int idx = vm.MusicGroupInput.Artists.FindIndex(a => a.ArtistId == artistId);
            string[] keys = { $"MusicGroupInput.Artists[{idx}].editFirstName",
                            $"MusicGroupInput.Artists[{idx}].editLastName"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                vm.ValidationResult = validationResult;
                return View("EditGroup", vm);
            }

            //Set the Album as Modified, it will later be updated in the database
            var a = vm.MusicGroupInput.Artists.First(a => a.ArtistId == artistId);
            if (a.StatusIM != vwmEditGroup.enStatusIM.Inserted)
            {
                a.StatusIM = vwmEditGroup.enStatusIM.Modified;
            }

            //Implement the changes
            a.FirstName = a.editFirstName;
            a.LastName = a.editLastName;

            return View("EditGroup", vm);
        }
        public IActionResult EditAlbum(Guid albumId, vwmEditGroup vm)
        {
            int idx = vm.MusicGroupInput.Albums.FindIndex(a => a.AlbumId == albumId);
            string[] keys = { $"MusicGroupInput.Albums[{idx}].editAlbumName",
                            $"MusicGroupInput.Albums[{idx}].editReleaseYear"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                vm.ValidationResult = validationResult;
                return View("EditGroup", vm);
            }

            //Set the Album as Modified, it will later be updated in the database
            var a = vm.MusicGroupInput.Albums.First(a => a.AlbumId == albumId);
            if (a.StatusIM != vwmEditGroup.enStatusIM.Inserted)
            {
                a.StatusIM = vwmEditGroup.enStatusIM.Modified;
            }

            //Implement the changes
            a.AlbumName = a.editAlbumName;
            a.ReleaseYear = a.editReleaseYear;

            return View("EditGroup", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Undo(vwmEditGroup vm)
        {
            //Reload Music group from Database
            var mg = await _service.ReadMusicGroupAsync(vm.MusicGroupInput.MusicGroupId, false);

            //Repopulate the InputModel
            vm.MusicGroupInput = new vwmEditGroup.csMusicGroupIM(mg);

            return View("EditGroup", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Save(vwmEditGroup vm)
        {
            string[] keys = { "MusicGroupInput.Name",
                              "MusicGroupInput.EstablishedYear",
                              "MusicGroupInput.Genre"};

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult, keys))
            {
                vm.ValidationResult = validationResult;
                return View("EditGroup", vm);
            }

            //This is where the music plays
            //First, are we creating a new Music group or editing another
            if (vm.MusicGroupInput.StatusIM == vwmEditGroup.enStatusIM.Inserted)
            {
                var mgDto = new csMusicGroupCUdto();

                //create the music group in the database
                mgDto.Name = vm.MusicGroupInput.Name;
                mgDto.EstablishedYear = vm.MusicGroupInput.EstablishedYear;
                mgDto.Genre = vm.MusicGroupInput.Genre.Value;

                var newMg = await _service.CreateMusicGroupAsync(mgDto);
                //get the newly created MusicGroupId
                vm.MusicGroupInput.MusicGroupId = newMg.MusicGroupId;
            }

            //Do all updates for Albums
            csMusicGroup mg = await SaveAlbums(vm);

            // Do all updates for Artists
            mg = await SaveArtists(vm);

            //Finally, update the MusicGroup itself
            mg.Name = vm.MusicGroupInput.Name;
            mg.EstablishedYear = vm.MusicGroupInput.EstablishedYear;
            mg.Genre = vm.MusicGroupInput.Genre.Value;
            mg = await _service.UpdateMusicGroupAsync(new csMusicGroupCUdto(mg));

            if (vm.MusicGroupInput.StatusIM == vwmEditGroup.enStatusIM.Inserted)
            {
                return Redirect($"~/Groups/ListOfGroups");
            }

            return Redirect($"~/Groups/ViewGroup?id={vm.MusicGroupInput.MusicGroupId}");
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //Inject services just like in WebApi
        public GroupsController(IMusicService service, ILogger<GroupsController> logger)
        {
            _service = service;
            _logger = logger;
        }


        #region ViewModel InputModel Albums and Artists saved to database
        private async Task<csMusicGroup> SaveAlbums(vwmEditGroup vm)
        {
            //Check if there are deleted albums, if so simply remove them
            var deletedAlbums = vm.MusicGroupInput.Albums.FindAll(a => (a.StatusIM == vwmEditGroup.enStatusIM.Deleted));
            foreach (var item in deletedAlbums)
            {
                //Remove from the database
                await _service.DeleteAlbumAsync(item.AlbumId);
            }

            //Check if there are any new albums added, if so create them in the database
            var newAlbums = vm.MusicGroupInput.Albums.FindAll(a => (a.StatusIM == vwmEditGroup.enStatusIM.Inserted));
            foreach (var item in newAlbums)
            {
                //Create the corresposning model and CUdto objects
                var model = item.UpdateModel(new csAlbum());
                var cuDto = new csAlbumCUdto(model) { MusicGroupId = vm.MusicGroupInput.MusicGroupId };

                //Set the relationships of a newly created item and write to database
                cuDto.MusicGroupId = vm.MusicGroupInput.MusicGroupId;
                model = await _service.CreateAlbumAsync(cuDto);
            }

            //To update modified and deleted albums, lets first read the original
            //Note that now the deleted albums will be removed and created albums will be nicely included
            var mg = await _service.ReadMusicGroupAsync(vm.MusicGroupInput.MusicGroupId, false);


            //Check if there are any modified albums , if so update them in the database
            var modifiedAlbums = vm.MusicGroupInput.Albums.FindAll(a => (a.StatusIM == vwmEditGroup.enStatusIM.Modified));
            foreach (var item in modifiedAlbums)
            {
                var model = mg.Albums.First(a => a.AlbumId == item.AlbumId);

                //Update the model from the InputModel
                model = item.UpdateModel(model);

                //Updatet the model in the database
                model = await _service.UpdateAlbumAsync(new csAlbumCUdto(model) { MusicGroupId = vm.MusicGroupInput.MusicGroupId });
            }

            return mg;
        }
        private async Task<csMusicGroup> SaveArtists(vwmEditGroup vm)
        {
            //Check if there are deleted artists, if so simply remove them
            var deletedArtists = vm.MusicGroupInput.Artists.FindAll(a => (a.StatusIM == vwmEditGroup.enStatusIM.Deleted));
            foreach (var item in deletedArtists)
            {
                //Remove from the database
                await _service.DeleteArtistAsync(item.ArtistId);
            }

            //Check if there are any new artist added, if so create them in the database
            var newArtists = vm.MusicGroupInput.Artists.FindAll(a => (a.StatusIM == vwmEditGroup.enStatusIM.Inserted));
            foreach (var item in newArtists)
            {
                //Create the corresposning model and CUdto objects
                var model = item.UpdateModel(new csArtist());
                var cuDto = new csArtistCUdto(model);

                //Set the relationships of a newly created item and write to database
                cuDto.MusicGroupsId = new List<Guid>();
                cuDto.MusicGroupsId.Add(vm.MusicGroupInput.MusicGroupId);

                //Create if does not exists. 
                model = await _service.UpsertArtistAsync(cuDto);
            }

            //To update modified and deleted Artists, lets first read the original
            //Note that now the deleted artists will be removed and created artists will be nicely included
            var mg = await _service.ReadMusicGroupAsync(vm.MusicGroupInput.MusicGroupId, false);


            //Check if there are any modified artists , if so update them in the database
            var modifiedArtists = vm.MusicGroupInput.Artists.FindAll(a => (a.StatusIM == vwmEditGroup.enStatusIM.Modified));
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
    }
}

