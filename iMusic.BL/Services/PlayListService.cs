using AutoMapper;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.DAL.Abstractions;
using iMusic.DAL.DTOs.Playlist;
using iMusic.DAL.DTOs.Search;
using iMusic.DAL.Entities;
using iMusic.DAL.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace iMusic.BL.Services;

public class PlayListService : IPlayListService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISongService _songService;
    private readonly IFileService _fileService;

    public PlayListService(IUnitOfWork unitOfWork, IMapper mapper, ISongService songService, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _songService = songService;
        _fileService = fileService;
    }

    public async Task<bool> CreatePlayListAsync(Guid currentUserId, CreatePlayListDTO createPlaylist)
    {
        if (createPlaylist.PlayListImg is null) throw new NoFileException($"Playlist Image File is required! Please add Photo");
        var imgPath = await _fileService.GetFilePathAsync(createPlaylist.PlayListImg);
      
        var newPlaylist = new Playlist()
        {
            Id = Guid.NewGuid(),
            Title = createPlaylist.Title,
            Status = createPlaylist.Status,
            CreatedTime = DateTimeOffset.Now,
            AuthorId = currentUserId,
            PlaylistImgUrl = imgPath,
            CountRate = 0
        };
        await _unitOfWork.Playlists.InsertAsync(newPlaylist);

        return await _unitOfWork.SaveAsync();
    }

    public async Task<bool> DeletePlaylistAsync(Guid playlistId)
    {
        var playlist = await _unitOfWork.Playlists.GetFirstOrDefaultAsync(p => p.Id == playlistId);
        if (playlist is null) throw new NotFoundException($"Playlist with id - {playlistId}");

        var favUserPlaylists = await _unitOfWork.FavoritePlaylists.GetAsync(x => x.PlaylistId == playlistId);

        var userSongPlaylist = await _unitOfWork.SongPlaylists.GetAsync(x => x.PlaylistId == playlistId);

        _unitOfWork.FavoritePlaylists.Delete(favUserPlaylists);

        _unitOfWork.SongPlaylists.Delete(userSongPlaylist);

        _unitOfWork.Playlists.Delete(playlist);

        return await _unitOfWork.SaveAsync();
      
    }

    public async Task<bool> EditPlayListAsync(EditPlayListDTO editPlaylist)
    {
        var playlist = await _unitOfWork.Playlists.GetFirstOrDefaultAsync(p => p.Id == editPlaylist.Id);

        if (playlist is null) throw new NotFoundException($"Playlist with id - {editPlaylist.Id}");

        if (editPlaylist.PlaylistImg is not null)
        {
            var img = await _fileService.GetFilePathAsync(editPlaylist.PlaylistImg);

            if (playlist.PlaylistImgUrl != null) _fileService.DeleteFile(playlist.PlaylistImgUrl);

            playlist.PlaylistImgUrl = img;
        }

        playlist.Title = editPlaylist.Title;
        playlist.Status = editPlaylist.Status;

        _unitOfWork.Playlists.Update(playlist);

        return await _unitOfWork.SaveAsync();
    }
  
    public async Task<bool> ChangeImageAsync(ChangePlaylistImageDTO changePlaylistImage)
    {
        var playlist = await _unitOfWork.Playlists.GetFirstOrDefaultAsync(p => p.Id == changePlaylistImage.Id);

        if (playlist is null) throw new NotFoundException($"Playlist with id - {changePlaylistImage.Id}");

        if (changePlaylistImage.PlayListImg is null) throw new NoFileException($"Playlist Image File is required! Please add Photo");

        if (playlist.PlaylistImgUrl is not null) _fileService.DeleteFile(playlist.PlaylistImgUrl);

        var imgPath = await _fileService.GetFilePathAsync(changePlaylistImage.PlayListImg);

        playlist.PlaylistImgUrl = imgPath;

        _unitOfWork.Playlists.Update(playlist);

        return await _unitOfWork.SaveAsync();
    }

    public async Task<bool> AddSongToPlaylistAsync(Guid playlistId, Guid songId)
    {
        var playlist = await _unitOfWork.Playlists.GetFirstOrDefaultAsync(p => p.Id == playlistId);

        if (playlist is null) throw new NotFoundException($"Playlist with id - {playlistId}");

        var song = await _unitOfWork.Songs.GetFirstOrDefaultAsync(s => s.Id == songId);

        if (song is null) throw new NotFoundException($"Song with id - {song}");

        var addSong = new SongPlaylist()
        {
            PlaylistId = playlistId,
            SongId = songId,
            AddedTime = DateTimeOffset.Now,
        };
        await _unitOfWork.SongPlaylists.InsertAsync(addSong);

        return await _unitOfWork.SaveAsync();
    }

    public async Task<bool> RemoveSongFromPlaylistAsync(Guid playlistId, Guid songId)
    {
        var songPlaylist = await _unitOfWork.SongPlaylists.GetFirstOrDefaultAsync(p => p.PlaylistId == playlistId && p.SongId == songId);

        if (songPlaylist is null) throw new NotFoundException($"SongPlaylist with playlist id - {playlistId} and song id - {songId}");
       
        _unitOfWork.SongPlaylists.Delete(songPlaylist);

        return await _unitOfWork.SaveAsync();
    }

    public async Task<PlaylistDTO> GetPlaylistAsync(Guid playlistId)
    {
        var playlist = await _unitOfWork.Playlists.GetFirstOrDefaultAsync(p => p.Id == playlistId,
            p => p.Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Singer)
            .Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Category)
            .Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Album)
            .Include(p => p.User));

        if (playlist is null) throw new NotFoundException($"Playlist with id - {playlistId} ");


        return _mapper.Map<PlaylistDTO>(playlist);
    }
    public async Task<IEnumerable<PlaylistDTO>> SearchPlaylistsAsync(SearchDTO search)
    {
        var playlist = (await _unitOfWork.Playlists.GetAsync(
            filter: p => p.Title.Contains(search.SearchTerm) && p.Status == Status.Public,
            includes: p => p.Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Singer)
            .Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Category)
            .Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Album)
            .Include(p => p.User))).Take(100);

        return _mapper.Map<IEnumerable<PlaylistDTO>>(playlist);
    }

    public async Task<IEnumerable<PlaylistDTO>> GetPlaylistsAsync()
    {
        var playlists = await _unitOfWork.Playlists.GetAsync(
            filter: p => p.Status == Status.Public, 
            includes: p => p.Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Singer)
            .Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Category)
            .Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Album)
            .Include(p => p.User));
      
        return _mapper.Map <IEnumerable<PlaylistDTO>> (playlists);
    }

    public async Task<IEnumerable<PlaylistDTO>> GetPlaylistsByUserAsync(Guid userId)
    {
        var playlists = await _unitOfWork.Playlists.GetAsync(x => x.AuthorId == userId,
            includes: p => p.Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Singer)
            .Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Category)
            .Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Album)
            .Include(p => p.User));
      
        return _mapper.Map<IEnumerable<PlaylistDTO>>(playlists);
    }
    public async Task<IEnumerable<PlaylistDTO>> GetTopPlaylistAsync()
    {
        var playlists = (await _unitOfWork.Playlists.GetAsync(filter: p => p.Status == Status.Public,
            orderBy: p => p.OrderByDescending(x => x.CountRate),
            includes: p => p.Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Singer)
            .Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Category)
            .Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Album)
            .Include(p => p.User))).Take(100);

        return _mapper.Map<IEnumerable<PlaylistDTO>>(playlists);
    }

}
