using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using DbContext;
using Models;
using System.Collections.Generic;

namespace Services;

public class csMusicServiceWaco : IMusicService
{
    HttpClient _httpClient;

    #region Seeding
    public async Task<int> Seed(int NrOfitems)
    {
        string uri = $"csadmin/seed?count={NrOfitems}";

        throw new NotImplementedException();
    }
    public async Task<int> RemoveSeed()
    {
        string uri = $"csadmin/removeseed";

        throw new NotImplementedException();
    }
    #endregion

    #region CRUD MusicGroup
    public async Task<List<csMusicGroup>> ReadMusicGroupsAsync(bool flat)
    {
        string uri = $"csmusicgroups/read?flat={flat}";

        throw new NotImplementedException();
    }

    public async Task<csMusicGroup> ReadMusicGroupAsync(Guid id, bool flat)
    {

        string uri = $"csmusicgroups/readitem?id={id}&flat={flat}";

        throw new NotImplementedException();
    }

    public async Task<csMusicGroup> UpdateMusicGroupAsync(csMusicGroupCUdto _src)
    {
        string uri = $"csmusicgroups/updateitem/{_src.MusicGroupId}";

        throw new NotImplementedException();
    }

    public async Task<csMusicGroup> CreateMusicGroupAsync(csMusicGroupCUdto _src)
    {
        string uri = $"csmusicgroups/createitem";

        throw new NotImplementedException();
    }

    public async Task<csMusicGroup> DeleteMusicGroupAsync(Guid id)
    {
        string uri = $"csmusicgroups/deleteitem/{id}";

        throw new NotImplementedException();
    }
    #endregion

    #region CRUD Albums
    public async Task<List<csAlbum>> ReadAlbumsAsync(bool flat)
    {
        string uri = $"csalbums/read?flat={flat}";

        throw new NotImplementedException();
    }

    public async Task<csAlbum> ReadAlbumAsync(Guid id, bool flat)
    {
        string uri = $"csalbums/readitem?id={id}&flat={flat}";

        throw new NotImplementedException();
    }

    public async Task<csAlbum> CreateAlbumAsync(csAlbumCUdto _src)
    {
        string uri = $"csalbums/createitem";

        throw new NotImplementedException();
    }

    public async Task<csAlbum> UpdateAlbumAsync(csAlbumCUdto _src)
    {
        string uri = $"csalbums/updateitem/{_src.AlbumId}";

        throw new NotImplementedException();
    }

    public async Task<csAlbum> DeleteAlbumAsync(Guid id)
    {
        string uri = $"csalbums/deleteitem/{id}";

        throw new NotImplementedException();
    }
    #endregion

    #region CRUD Artists
    public async Task<List<csArtist>> ReadArtistsAsync(bool flat)
    {
        string uri = $"csartists/read?flat={flat}";

        throw new NotImplementedException();
    }

    public async Task<csArtist> ReadArtistsync(Guid id, bool flat)
    {
        string uri = $"csartists/readitem?id={id}&flat={flat}";

        throw new NotImplementedException();
    }

    //Upsert = Update or Insert
    public async Task<csArtist> UpsertArtistAsync(csArtistCUdto _src)
    {
        string uri = $"csartists/upsertitem";

        throw new NotImplementedException();
    }

    public async Task<csArtist> UpdateArtistAsync(csArtistCUdto _src)
    {
        string uri = $"csartists/updateitem/{_src.ArtistId}";

        throw new NotImplementedException();
    }

    public async Task<csArtist> DeleteArtistAsync(Guid id)
    {
        string uri = $"csartists/deleteitem/{id}";

        throw new NotImplementedException();
    }
    #endregion

    public csMusicServiceWaco(IHttpClientFactory httpClientFactory)
    {
        //Using DI for best practise usage of HttpClient.
        //Create a client from the template MusicWebApi
        _httpClient = httpClientFactory.CreateClient(name: "MusicWebApi");
    }
}



