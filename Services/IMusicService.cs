using System;
using Models;

namespace Services
{
	public interface IMusicService
	{
        public Task<int> Seed(int NrOfitems);
        public Task<int> RemoveSeed();

        public Task<List<csMusicGroup>> ReadMusicGroupsAsync(bool flat);
        public Task<csMusicGroup> ReadMusicGroupAsync(Guid id, bool flat);
        public Task<csMusicGroup> UpdateMusicGroupAsync(csMusicGroupCUdto _src);
        public Task<csMusicGroup> CreateMusicGroupAsync(csMusicGroupCUdto _src);
        public Task<csMusicGroup> DeleteMusicGroupAsync(Guid id);

        public Task<List<csAlbum>> ReadAlbumsAsync(bool flat);
        public Task<csAlbum> ReadAlbumAsync(Guid id, bool flat);
        public Task<csAlbum> CreateAlbumAsync(csAlbumCUdto _src);
        public Task<csAlbum> UpdateAlbumAsync(csAlbumCUdto _src);
        public Task<csAlbum> DeleteAlbumAsync(Guid id);

        public Task<List<csArtist>> ReadArtistsAsync(bool flat);
        public Task<csArtist> ReadArtistsync(Guid id, bool flat);
        public Task<csArtist> UpsertArtistAsync(csArtistCUdto _src);
        public Task<csArtist> UpdateArtistAsync(csArtistCUdto _src);
        public Task<csArtist> DeleteArtistAsync(Guid id);
    }
}

