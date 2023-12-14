using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using DbContext;
using Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Azure;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Services;

public class csMusicServiceWaco : IMusicService
{
    HttpClient _httpClient;

    #region Seeding
    public async Task<int> Seed(int NrOfitems)
    {
        string uri = $"csadmin/seed?count={NrOfitems}";

        HttpResponseMessage response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        string s = await response.Content.ReadAsStringAsync();
        var count = JsonConvert.DeserializeObject<int>(s);
        
        return count;
    }
    public async Task<int> RemoveSeed()
    {
        string uri = $"csadmin/removeseed";

        HttpResponseMessage response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        string s = await response.Content.ReadAsStringAsync();
        var count = JsonConvert.DeserializeObject<int>(s);

        return count;
    }
    #endregion

    #region CRUD MusicGroup
    public async Task<List<csMusicGroup>> ReadMusicGroupsAsync(bool flat)
    {
        string uri = $"csmusicgroups/read?flat={flat}";

        HttpResponseMessage response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        List<csMusicGroup>musicGroups = JsonConvert.DeserializeObject<List<csMusicGroup>>(content);
        
        return musicGroups;
    }

    public async Task<csMusicGroup> ReadMusicGroupAsync(Guid id, bool flat)
    {
        string uri = $"csmusicgroups/readitem?id={id}&flat={flat}";

        HttpResponseMessage response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();
     
        string content = await response.Content.ReadAsStringAsync();
        csMusicGroup musicGroup = JsonConvert.DeserializeObject<csMusicGroup>(content);

        return musicGroup;
    }

    public async Task<csMusicGroup> UpdateMusicGroupAsync(csMusicGroupCUdto _src)
    {
        string uri = $"csmusicgroups/updateitem/{_src.MusicGroupId}";

        string json = JsonConvert.SerializeObject(_src);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PutAsync(uri, content);
        response.EnsureSuccessStatusCode();

        string updatedMusicGroupJson = await response.Content.ReadAsStringAsync();
        csMusicGroup updatedMusicGroup = JsonConvert.DeserializeObject<csMusicGroup>(updatedMusicGroupJson);

        return updatedMusicGroup;
    }

    public async Task<csMusicGroup> CreateMusicGroupAsync(csMusicGroupCUdto _src)
    {
        string uri = $"csmusicgroups/createitem";

        string json = JsonConvert.SerializeObject(_src);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();

        string newMusicGroupJson = await response.Content.ReadAsStringAsync();
        csMusicGroup newMusicGroup = JsonConvert.DeserializeObject<csMusicGroup>(newMusicGroupJson);

        return newMusicGroup;
    }

    public async Task<csMusicGroup> DeleteMusicGroupAsync(Guid id)
    {
        string uri = $"csmusicgroups/deleteitem/{id}";

        HttpResponseMessage response = await _httpClient.DeleteAsync(uri);
        response.EnsureSuccessStatusCode();

        string deletingSkater = await response.Content.ReadAsStringAsync();
        csMusicGroup _deletingSkater = JsonConvert.DeserializeObject<csMusicGroup>(deletingSkater);

        return _deletingSkater;
    }
    #endregion

    #region CRUD Albums
    public async Task<List<csAlbum>> ReadAlbumsAsync(bool flat)
     {
        string uri = $"csalbums/read?flat={flat}";

        HttpResponseMessage response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        string albumsContent = await response.Content.ReadAsStringAsync();
        List<csAlbum> _albumsContent = JsonConvert.DeserializeObject<List<csAlbum>>(albumsContent);

        return _albumsContent;
    }

    public async Task<csAlbum> ReadAlbumAsync(Guid id, bool flat)
    {
        string uri = $"csalbums/readitem?id={id}&flat={flat}";

        throw new NotImplementedException();
    }

    public async Task<csAlbum> CreateAlbumAsync(csAlbumCUdto _src)
    {
        string uri = $"csalbums/createitem";

        string json = JsonConvert.SerializeObject(_src);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();

        string newAlbumGroupJson = await response.Content.ReadAsStringAsync();
        csAlbum newAlbum = JsonConvert.DeserializeObject<csAlbum>(newAlbumGroupJson);

        return newAlbum;
    }

    public async Task<csAlbum> UpdateAlbumAsync(csAlbumCUdto _src)
    {
        string uri = $"csalbums/updateitem/{_src.AlbumId}";

        string json = JsonConvert.SerializeObject(_src);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PutAsync(uri, content);
        response.EnsureSuccessStatusCode();

        string updateAlbumJson = await response.Content.ReadAsStringAsync();
        csAlbum updatedAlbum = JsonConvert.DeserializeObject<csAlbum>(updateAlbumJson);

        return updatedAlbum;
    }

    public async Task<csAlbum> DeleteAlbumAsync(Guid id)
    {
        string uri = $"csalbums/deleteitem/{id}";

        HttpResponseMessage response = await _httpClient.DeleteAsync(uri);
        response.EnsureSuccessStatusCode();

        string deletingAlbum = await response.Content.ReadAsStringAsync();
        csAlbum _deletingSkater = JsonConvert.DeserializeObject<csAlbum>(deletingAlbum);

        return _deletingSkater;
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

        string json = JsonConvert.SerializeObject(_src);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PutAsync(uri, content);
        response.EnsureSuccessStatusCode();

        string upsertArtistJson = await response.Content.ReadAsStringAsync();
        csArtist upsertArtist = JsonConvert.DeserializeObject<csArtist>(upsertArtistJson);

        return upsertArtist;
    }

    public async Task<csArtist> UpdateArtistAsync(csArtistCUdto _src)
    {
        string uri = $"csartists/updateitem/{_src.ArtistId}";

        throw new NotImplementedException();
    }

    public async Task<csArtist> DeleteArtistAsync(Guid id)
    {
        string uri = $"csartists/deleteitem/{id}";

        HttpResponseMessage response = await _httpClient.DeleteAsync(uri);
        response.EnsureSuccessStatusCode();

        string deletingArtist = await response.Content.ReadAsStringAsync();
        csArtist _deletingArtist = JsonConvert.DeserializeObject<csArtist>(deletingArtist);

        return _deletingArtist;
    }
    #endregion

    public csMusicServiceWaco(IHttpClientFactory httpClientFactory)
    {
        //Using DI for best practise usage of HttpClient.
        //Create a client from the template MusicWebApi
        _httpClient = httpClientFactory.CreateClient(name: "MusicWebApi");
    }
}



