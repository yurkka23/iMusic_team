using AutoMapper;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.DAL.Abstractions;
using iMusic.DAL.DTOs.FavoriteList;
using iMusic.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace iMusic.BL.Services;

public class FavoriteListService : IFavoriteListService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FavoriteListService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<FavoriteListDTO> GetFavoriteListAsync(Guid userId)
    {
        var list = await _unitOfWork.FavoriteLists.GetFirstOrDefaultAsync(c => c.UserId == userId);

        var listSongs = await _unitOfWork.FavoriteSongs.GetAsync(c => c.FavoritelistId == userId,
            includes: x => x.Include(s => s.Song).ThenInclude(s => s.Singer)
            .Include(s => s.Song).ThenInclude(s => s.Category),
            orderBy: s => s.OrderByDescending(x => x.AddedTime));

        var listPlaylists = await _unitOfWork.FavoritePlaylists.GetAsync(c => c.FavoritelistId == userId,
            includes: x => x.Include(s => s.Playlist).ThenInclude(s => s.User),
             orderBy: s => s.OrderByDescending(x => x.AddedTime));

        var listAlbums = await _unitOfWork.FavoriteAlbums.GetAsync(c => c.FavoritelistId == userId,
           includes: x => x.Include(s => s.Album).ThenInclude(s => s.Category)
            .Include(s => s.Album).ThenInclude(s => s.User),
            orderBy: s => s.OrderByDescending(x => x.AddedTime));

        if(listSongs is not null && listSongs.Count() > 0)
        {
            list.Songs = (ICollection<FavoriteSongs>)listSongs;

        }

        if (listAlbums is not null && listAlbums.Count() > 0)
        {
            list.Albums = (ICollection<FavoriteAlbums>)listAlbums;

        }

        if (listPlaylists is not null && listPlaylists.Count() > 0)
        {
            list.Playlists = (ICollection<FavoritePlaylists>)listPlaylists;

        }


        if (list is null)
        {
            var newList = new FavoriteList()
            {
                UserId = userId
            };
            await _unitOfWork.FavoriteLists.InsertAsync(newList);

            await _unitOfWork.SaveAsync();

            return _mapper.Map<FavoriteListDTO>(newList);
        }


        return _mapper.Map<FavoriteListDTO>(list);
    }

    public async Task<bool> AddSongToFavoriteListAsync(Guid songId, Guid listId)
    {
        var song = await _unitOfWork.Songs.GetFirstOrDefaultAsync(s => s.Id == songId);

        if (song is null) throw new NotFoundException($"Song with Id - {songId} ");

        var list = await _unitOfWork.FavoriteLists.GetFirstOrDefaultAsync(c => c.UserId == listId, x => x.Include(s => s.Songs).Include(s => s.Playlists).Include(s => s.Albums));

        if (list is null)
        {
            var newList = new FavoriteList()
            {
                UserId = listId
            };

            await _unitOfWork.FavoriteLists.InsertAsync(newList);

            await _unitOfWork.SaveAsync();

        }

        var songfav = new FavoriteSongs()
        {
            SongId = songId,
            FavoritelistId = listId,
            AddedTime = DateTimeOffset.UtcNow
        };

        await _unitOfWork.FavoriteSongs.InsertAsync(songfav);

        song.CountRate += 1;
        _unitOfWork.Songs.Update(song);

        return await _unitOfWork.SaveAsync();
    }

    public async Task<bool> AddAlbumToFavoriteListAsync(Guid albumId, Guid listId)
    {
        var album = await _unitOfWork.Albums.GetFirstOrDefaultAsync(s => s.Id == albumId);

        if (album is null) throw new NotFoundException($"Album with Id - {albumId} ");

        var list = await _unitOfWork.FavoriteLists.GetFirstOrDefaultAsync(c => c.UserId == listId, x => x.Include(s => s.Songs).Include(s => s.Playlists).Include(s => s.Albums));

        if (list is null)
        {
            var newList = new FavoriteList()
            {
                UserId = listId
            };

            await _unitOfWork.FavoriteLists.InsertAsync(newList);
            await _unitOfWork.SaveAsync();
        }

        var albumfav = new FavoriteAlbums()
        {
            AlbumId = albumId,
            FavoritelistId = listId,
            AddedTime = DateTimeOffset.UtcNow

        };

        await _unitOfWork.FavoriteAlbums.InsertAsync(albumfav);

        album.CountRate += 1;
        _unitOfWork.Albums.Update(album);

        return await _unitOfWork.SaveAsync();
    }

    public async Task<bool> AddPlaylistToFavoriteListAsync(Guid playlistId, Guid listId)
    {
        var playlist = await _unitOfWork.Playlists.GetFirstOrDefaultAsync(s => s.Id == playlistId);

        if (playlist is null) throw new NotFoundException($"Playlist with Id - {playlistId} ");

        var list = await _unitOfWork.FavoriteLists.GetFirstOrDefaultAsync(c => c.UserId == listId, x => x.Include(s => s.Songs).Include(s => s.Playlists).Include(s => s.Albums));

        if (list is null)
        {
            var newList = new FavoriteList()
            {
                UserId = listId
            };

            await _unitOfWork.FavoriteLists.InsertAsync(newList);
            await _unitOfWork.SaveAsync();
        }

        var playfav = new FavoritePlaylists()
        {
            PlaylistId = playlistId,
            FavoritelistId = listId,
            AddedTime = DateTimeOffset.UtcNow
        };

        await _unitOfWork.FavoritePlaylists.InsertAsync(playfav);

        playlist.CountRate += 1;
        _unitOfWork.Playlists.Update(playlist);

        return await _unitOfWork.SaveAsync();
    }
    public async Task<bool> RemoveSongFromFavoriteListAsync(Guid songId, Guid listId)
    {
        var songlist = await _unitOfWork.FavoriteSongs.GetFirstOrDefaultAsync(s => s.SongId == songId && s.FavoritelistId == listId);

        if (songlist is null) throw new NotFoundException($"Favorite list - {listId} with Song with Id - {songId} ");

        var song = await _unitOfWork.Songs.GetFirstOrDefaultAsync(x => x.Id == songId);
        if (song is null) throw new NotFoundException($"Song with Id - {songId} ");

        song.CountRate -= 1;

        _unitOfWork.Songs.Update(song);

        _unitOfWork.FavoriteSongs.Delete(songlist);

        return await _unitOfWork.SaveAsync();
    }

    public async Task<bool> RemovePlaylistFromFavoriteListAsync(Guid playlistId, Guid listId)
    {
        
        var playFavoritelist = await _unitOfWork.FavoritePlaylists.GetFirstOrDefaultAsync(s => s.PlaylistId == playlistId && s.FavoritelistId == listId);

        if (playFavoritelist is null) throw new NotFoundException($"Favorite list - {listId} with Playlist with Id - {playlistId} ");

        var playlist = await _unitOfWork.Playlists.GetFirstOrDefaultAsync(x => x.Id == playlistId);
        if (playlist is null) throw new NotFoundException($"Playlist with Id - {playlistId} ");

        playlist.CountRate -= 1;

        _unitOfWork.Playlists.Update(playlist);

        _unitOfWork.FavoritePlaylists.Delete(playFavoritelist);

        return await _unitOfWork.SaveAsync();
    }

    public async Task<bool> RemoveAlbumFromFavoriteListAsync(Guid albumId, Guid listId)
    {
        var albumlist = await _unitOfWork.FavoriteAlbums.GetFirstOrDefaultAsync(s => s.AlbumId == albumId && s.FavoritelistId == listId);

        if (albumlist is null) throw new NotFoundException($"Favorite list - {listId} with Album with Id - {albumId} ");

        var album = await _unitOfWork.Albums.GetFirstOrDefaultAsync(x => x.Id == albumId);
        if (album is null) throw new NotFoundException($"Album with Id - {albumId} ");

        album.CountRate -= 1;

        _unitOfWork.Albums.Update(album);

        _unitOfWork.FavoriteAlbums.Delete(albumlist);

        return await _unitOfWork.SaveAsync();
    }
}
