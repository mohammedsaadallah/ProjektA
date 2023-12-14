using Microsoft.EntityFrameworkCore;
using DbContext;
using Models;

namespace Services;

public class csMusicService : IMusicService
{
    #region Seeding
    public async Task<int> Seed(int NrOfitems)
    {
        #region Seeding
        var _seeder = new csSeedGenerator();

        //get a list of music groups
        var _musicGroups = _seeder.ItemsToList<csMusicGroup>(NrOfitems);

        //Set between 5 and 50 albums for each music groups
        _musicGroups.ForEach(mg => mg.Albums = _seeder.ItemsToList<csAlbum>(_seeder.Next(2, 5)));

        //get a list of artists
        var _artists = _seeder.ItemsToList<csArtist>(100);

        //Assign artists to Music groups
        _musicGroups.ForEach(mg => mg.Artists = _seeder.UniqueIndexPickedFromList<csArtist>(_seeder.Next(2, 5), _artists));
        #endregion

        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            foreach (var item in _musicGroups)
            {
                db.MusicGroups.Add(item);
            }

            await db.SaveChangesAsync();

            int cnt = await db.MusicGroups.CountAsync();
            return cnt;
        }
    }
    public async Task<int> RemoveSeed()
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            db.Albums.RemoveRange(db.Albums.Where(mg => mg.Seeded));
            db.MusicGroups.RemoveRange(db.MusicGroups.Where(mg => mg.Seeded));
            await db.SaveChangesAsync();

            int _count = await db.MusicGroups.CountAsync();
            return _count;
        }
    }
    #endregion

    #region CRUD MusicGroup
    public async Task<List<csMusicGroup>> ReadMusicGroupsAsync(bool flat)
    {
        flat = true;
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            if (flat)
            {
                var _list = await db.MusicGroups.ToListAsync();
                return _list;
            }
            else
            {
                var _list = await db.MusicGroups
                    .Include(mg => mg.Albums)
                    .Include(mg => mg.Artists).ToListAsync();
                return _list;
            }
        }
    }

    public async Task<csMusicGroup> ReadMusicGroupAsync(Guid id, bool flat)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            if (flat)
            {
                var mg = await db.MusicGroups
                    .FirstOrDefaultAsync(mg => mg.MusicGroupId == id);

                return mg;
            }
            else
            {
                var mg = await db.MusicGroups.Include(mg => mg.Albums)
                    .Include(mg => mg.Artists)
                    .FirstOrDefaultAsync(mg => mg.MusicGroupId == id);

                return mg;
            }
        }
    }

    public async Task<csMusicGroup> UpdateMusicGroupAsync(csMusicGroupCUdto _src)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            var _query1 = db.MusicGroups.Where(mg => mg.MusicGroupId == _src.MusicGroupId);
            var _item = await _query1.Include(mg => mg.Albums)
                .Include(mg => mg.Artists).FirstOrDefaultAsync();

            _item.UpdateFromDTO(_src);

            await csMusicGroupCUdto_To_csMusicGroup_NavProp(db, _src, _item);

            db.MusicGroups.Update(_item);
            await db.SaveChangesAsync();

            return (_item);
        }
    }

    public async Task<csMusicGroup> CreateMusicGroupAsync(csMusicGroupCUdto _src)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            var _item = new csMusicGroup(_src);

            await csMusicGroupCUdto_To_csMusicGroup_NavProp(db, _src, _item);

            db.MusicGroups.Add(_item);  //istf update
            await db.SaveChangesAsync();

            return (_item);
        }
    }

    public async Task<csMusicGroup> DeleteMusicGroupAsync(Guid id)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            //Find the instance with matching id
            var _query1 = db.MusicGroups
                .Where(i => i.MusicGroupId == id);
            var _item = await _query1.FirstOrDefaultAsync<csMusicGroup>();

            //If the item does not exists
            if (_item == null) throw new ArgumentException($"Item {id} is not existing");

            //delete in the database model
            db.MusicGroups.Remove(_item);

            //write to database in a UoW
            await db.SaveChangesAsync();
            return _item;
        }
    }


    async Task csMusicGroupCUdto_To_csMusicGroup_NavProp(csMainDbContext db,
        csMusicGroupCUdto _src, csMusicGroup _dst)
    {
        //Navigation prop Albums
        List<csAlbum> _albums = new List<csAlbum>();
        foreach (var id in _src.AlbumsId)
        {
            var _album = await db.Albums.FirstOrDefaultAsync(a => a.AlbumId == id);

            if (_album == null)
                throw new ArgumentException($"Item id {id} not existing");

            _albums.Add(_album);
        }

        _dst.Albums = _albums;

        //Navigation prop Artist
        List<csArtist> _artists = new List<csArtist>();
        foreach (var id in _src.ArtistsId)
        {
            var _artist = await db.Artists.FirstOrDefaultAsync(a => a.ArtistId == id);

            if (_artist == null)
                throw new ArgumentException($"Item id {id} not existing");

            _artists.Add(_artist);
        }

        _dst.Artists = _artists;

    }

    #endregion

    #region CRUD Albums
    public async Task<List<csAlbum>> ReadAlbumsAsync(bool flat)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            if (flat)
            {
                var _list = await db.Albums.ToListAsync();
                return _list;
            }
            else
            {
                var _list = await db.Albums
                    .Include(a => a.MusicGroup)
                    .ThenInclude(a => a.Artists).ToListAsync();
                return _list;
            }
        }
    }

    public async Task<csAlbum> ReadAlbumAsync(Guid id, bool flat)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            if (flat)
            {
                var mg = await db.Albums
                    .FirstOrDefaultAsync(a => a.AlbumId == id);

                return mg;
            }
            else
            {
                var mg = await db.Albums
                    .Include(a => a.MusicGroup)
                    .ThenInclude(a => a.Artists)
                    .FirstOrDefaultAsync(a => a.AlbumId == id);

                return mg;
            }
        }
    }

    public async Task<csAlbum> CreateAlbumAsync(csAlbumCUdto _src)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            var _item = new csAlbum(_src);

            await csAlbumCUdto_To_csAlbum_NavProp(db, _src, _item);

            db.Albums.Add(_item);  //istf update
            await db.SaveChangesAsync();

            return (_item);
        }
    }


    public async Task<csAlbum> UpdateAlbumAsync(csAlbumCUdto _src)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            var _query1 = db.Albums.Where(mg => mg.AlbumId == _src.AlbumId);
            var _item = await _query1.Include(a => a.MusicGroup).FirstOrDefaultAsync();

            _item.UpdateFromDTO(_src);

            await csAlbumCUdto_To_csAlbum_NavProp(db, _src, _item);

            db.Albums.Update(_item);
            await db.SaveChangesAsync();

            return (_item);
        }
    }
    public async Task<csAlbum> DeleteAlbumAsync(Guid id)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            //Find the instance with matching id
            var _query1 = db.Albums
                .Where(i => i.AlbumId == id);
            var _item = await _query1.FirstOrDefaultAsync<csAlbum>();

            //If the item does not exists
            if (_item == null) throw new ArgumentException($"Item {id} is not existing");

            //delete in the database model
            db.Albums.Remove(_item);

            //write to database in a UoW
            await db.SaveChangesAsync();
            return _item;
        }
    }

    async Task csAlbumCUdto_To_csAlbum_NavProp(csMainDbContext db,
    csAlbumCUdto _src, csAlbum _dst)
    {
        //Navigation prop Albums
        var _musicGroup = await db.MusicGroups.FirstOrDefaultAsync(a => a.MusicGroupId == _src.MusicGroupId);
        if (_musicGroup == null)
            throw new ArgumentException($"Item id {_src.MusicGroupId} not existing");

        _dst.MusicGroup = _musicGroup;
    }
    #endregion

    #region CRUD Artists

    public async Task<List<csArtist>> ReadArtistsAsync(bool flat)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            if (flat)
            {
                var _list = await db.Artists.ToListAsync();
                return _list;
            }
            else
            {
                var _list = await db.Artists
                    .Include(a => a.MusicGroups)
                    .ThenInclude(a => a.Albums).ToListAsync();
                return _list;
            }
        }
    }

    public async Task<csArtist> ReadArtistsync(Guid id, bool flat)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            if (flat)
            {
                var mg = await db.Artists
                    .FirstOrDefaultAsync(a => a.ArtistId == id);

                return mg;
            }
            else
            {
                var mg = await db.Artists
                    .Include(a => a.MusicGroups)
                    .ThenInclude(a => a.Albums)
                    .FirstOrDefaultAsync(a => a.ArtistId == id);

                return mg;
            }
        }
    }
    //Upsert = Update or Insert
    public async Task<csArtist> UpsertArtistAsync(csArtistCUdto _src)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            //Check if a Artist Exists
            var _query1 = db.Artists.Where(a => (a.FirstName == _src.FirstName) && (a.LastName == _src.LastName) && (a.BirthDay == _src.BirthDay));
            var _item = await _query1.Include(a => a.MusicGroups).FirstOrDefaultAsync();

            if (_item != null)
            {
                //Instead of Create the Artist should be updated
                _item.UpdateFromDTO(_src);

                await csArtistCUdto_To_csArtist_NavProp(db, _src, _item);
                db.Artists.Update(_item);
            }
            else
            {

                //Create and insert a new Artist
                _item = new csArtist(_src);
                await csArtistCUdto_To_csArtist_NavProp(db, _src, _item);
                db.Artists.Add(_item);
            }

            await db.SaveChangesAsync();
            return (_item);
        }
    }

    public async Task<csArtist> UpdateArtistAsync(csArtistCUdto _src)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            var _query1 = db.Artists.Where(mg => mg.ArtistId == _src.ArtistId);
            var _item = await _query1.Include(a => a.MusicGroups).FirstOrDefaultAsync();

            _item.UpdateFromDTO(_src);

            await csArtistCUdto_To_csArtist_NavProp(db, _src, _item);

            db.Artists.Update(_item);
            await db.SaveChangesAsync();

            return (_item);
        }
    }

    public async Task<csArtist> DeleteArtistAsync(Guid id)
    {
        using (var db = csMainDbContext.DbContext("sysadmin"))
        {
            //Find the instance with matching id
            var _query1 = db.Artists
                .Where(i => i.ArtistId == id);
            var _item = await _query1.FirstOrDefaultAsync<csArtist>();

            //If the item does not exists
            if (_item == null) throw new ArgumentException($"Item {id} is not existing");

            //delete in the database model
            db.Artists.Remove(_item);

            //write to database in a UoW
            await db.SaveChangesAsync();
            return _item;
        }
    }

    async Task csArtistCUdto_To_csArtist_NavProp(csMainDbContext db,
    csArtistCUdto _src, csArtist _dst)
    {
        //Navigation prop MusicGroups
        List<csMusicGroup> _mgs = new List<csMusicGroup>();
        foreach (var id in _src.MusicGroupsId)
        {
            var _mg = await db.MusicGroups.FirstOrDefaultAsync(a => a.MusicGroupId == id);

            if (_mg == null)
                throw new ArgumentException($"Item id {id} not existing");

            _mgs.Add(_mg);
        }

        _dst.MusicGroups = _mgs;
    }
    #endregion
}



