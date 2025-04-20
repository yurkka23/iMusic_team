using AutoMapper;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.DAL.Abstractions;
using iMusic.DAL.DTOs.Search;
using iMusic.DAL.DTOs.Song;
using iMusic.DAL.Entities;
using iMusic.DAL.Entities.Enums;
using iMusic.DAL.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NAudio.Wave;

namespace iMusic.BL.Services;

public class SongService : ISongService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly HostSettings _hostSettings;

    public SongService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService,
         IOptionsSnapshot<HostSettings> hostSettings)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;
        _hostSettings = hostSettings.Value;
    }

    public async Task UploadSongAsync(Guid currentUserId, UploadSongDTO songDTO)
    {
        if(songDTO.AlbumId is not null){
            var album = await _unitOfWork.Albums.GetFirstOrDefaultAsync(x => x.Id == songDTO.AlbumId);
            if (album == null) throw new NotFoundException($"Album with id - {songDTO.AlbumId}");
        }

        var category = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Name == songDTO.CategoryName);

        if (category == null) throw new BadRequestException("Such category doesn't exists");
        if (songDTO.SongImg is null) throw new NoFileException($"Song Image File is required! Please add Photo");
        if (songDTO.SongFile is null) throw new NoFileException($"Song File is required! Please add Song File");

        var imgPath = await _fileService.GetFilePathAsync(songDTO.SongImg);
        var songPath = await _fileService.GetSongFilePathAsync(songDTO.SongFile);

        Mp3FileReader reader = new Mp3FileReader(songPath);
        TimeSpan duration = reader.TotalTime;
        string durationResult = duration.Minutes.ToString() + '.' + duration.Seconds.ToString();

        var newSong = new Song()
        {
            Id = Guid.NewGuid(),
            SongImgUrl = imgPath,
            SongUrl = songPath,
            Duration = double.Parse(durationResult, System.Globalization.CultureInfo.InvariantCulture),
            SingerId = currentUserId, 
            CategoryId = category.Id,
            Title = songDTO.Title,
            CreatedTime = DateTimeOffset.Now,
            Text = songDTO.Text,
            CountRate = 0,
            Status = songDTO.Status,
            AlbumId = songDTO.AlbumId
        };

        await _unitOfWork.Songs.InsertAsync(newSong);

        await _unitOfWork.SaveAsync();

    }

    public async Task EditSongAsync(EditSongDTO songDTO)
    {
        var category = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Name == songDTO.CategoryName);

        if (category == null) throw new BadRequestException("Such category doesn't exists");

        var song = await _unitOfWork.Songs.GetFirstOrDefaultAsync(c => c.Id == songDTO.Id);
        if (song is null) throw new NotFoundException($"Song with Id-{songDTO.Id} ");

        if (songDTO.SongImg is not null)
        {
            if (song.SongImgUrl != null) _fileService.DeleteFile(song.SongImgUrl);
            var imgPath = await _fileService.GetFilePathAsync(songDTO.SongImg);
            song.SongImgUrl = imgPath;
        }

        if (songDTO.SongFile is not null)
        {
            if (song.SongUrl != null) _fileService.DeleteFile(song.SongUrl);
            var songPath = await _fileService.GetSongFilePathAsync(songDTO.SongFile);
            Mp3FileReader reader = new Mp3FileReader(songPath);
            TimeSpan duration = reader.TotalTime;
            string durationResult = duration.Minutes.ToString() + '.' + duration.Seconds.ToString();
            song.SongUrl = songPath;
            song.Duration = double.Parse(durationResult, System.Globalization.CultureInfo.InvariantCulture);
        }

        song.CategoryId = category.Id;
        song.Title = songDTO.Title;
        song.Text = songDTO.Text;
        song.Status = songDTO.Status;
        
        _unitOfWork.Songs.Update(song);

        await _unitOfWork.SaveAsync();
    }

    public async Task<bool> DeleteSongAsync(Guid songId)
    {
        var song = await _unitOfWork.Songs.GetFirstOrDefaultAsync(c => c.Id == songId);
        if (song is null) throw new NotFoundException($"Song with Id-{songId} ");

        if (song.SongImgUrl != null) _fileService.DeleteFile(song.SongImgUrl);
        if (song.SongUrl != null) _fileService.DeleteFile(song.SongUrl);

        var addedUserSongs = await _unitOfWork.UserSongs.GetAsync(x => x.SongId == songId);

        var favUserSongs = await _unitOfWork.FavoriteSongs.GetAsync(x => x.SongId == songId);

        var userSongPlaylist = await _unitOfWork.SongPlaylists.GetAsync(x => x.SongId == songId);

        _unitOfWork.SongPlaylists.Delete(userSongPlaylist);

        _unitOfWork.FavoriteSongs.Delete(favUserSongs);

        _unitOfWork.UserSongs.Delete(addedUserSongs);

        _unitOfWork.Songs.Delete(song);

        return await _unitOfWork.SaveAsync(); ;
    }

    public async Task<SongDTO> GetSongAsync(Guid songId)
    {
        var song = await _unitOfWork.Songs.GetFirstOrDefaultAsync(c => c.Id == songId, 
            x => x.Include(s => s.Category).Include(s => s.Singer).Include(s => s.Album));
        if (song is null) throw new NotFoundException($"Song with Id-{songId} ");

        return _mapper.Map<SongDTO>(song);
    }

    public async Task<IEnumerable<SongDTO>> GetSongsAsync()
    {
        var songs = await _unitOfWork.Songs.GetAsync(filter: x => x.Status == Status.Public,
            includes: x => x.Include(s => s.Category).Include(s => s.Singer).Include(s => s.Album));

        return _mapper.Map<IEnumerable<SongDTO>>(songs);
    }
    public async Task<IEnumerable<SongDTO>> GetSongsBySinger(Guid id)
    {
        var songs = await _unitOfWork.Songs.GetAsync(filter: x => x.SingerId == id,
            orderBy: x => x.OrderByDescending(x => x.CreatedTime),
            includes: x => x.Include(s => s.Category).Include(s => s.Singer).Include(s => s.Album));

        return _mapper.Map<IEnumerable<SongDTO>>(songs);
    }

    public async Task<IEnumerable<SongDTO>> GetTopSongsAsync()
    {
        var songs = (await _unitOfWork.Songs.GetAsync(filter: x => x.Status == Status.Public,
            orderBy: x => x.OrderByDescending(x => x.CountRate),
            includes: x => x.Include(s => s.Category).Include(s => s.Singer).Include(s => s.Album))).Take(100);

        return _mapper.Map<IEnumerable<SongDTO>>(songs);
    }

    public async Task<IEnumerable<SongDTO>> GetSongsByCategoryAsync(Guid id)
    {
        var songs = (await _unitOfWork.Songs.GetAsync(filter: x => x.CategoryId == id && x.Status == Status.Public,
            orderBy: x => x.OrderByDescending(x => x.CountRate),
            includes: x => x.Include(s => s.Category).Include(s => s.Singer).Include(s => s.Album)));

        return _mapper.Map<IEnumerable<SongDTO>>(songs);
    }

    public async Task<IEnumerable<SongDTO>> GetNewSongsAsync()
    {
        var songs = (await _unitOfWork.Songs.GetAsync(filter: x => x.Status == Status.Public,
            orderBy: x => x.OrderByDescending(x => x.CreatedTime),
            includes: x => x.Include(s => s.Category).Include(s => s.Singer).Include(s => s.Album))).Take(100);

        return _mapper.Map<IEnumerable<SongDTO>>(songs);
    }

    public async Task<IEnumerable<SongDTO>> GetRecommendSongsToUserAsync(Guid id)
    {
        var userCategories = (await _unitOfWork.UserSongs.GetAsync(
            filter: x => x.UserId == id,
            orderBy: null,
            includes: x => x.Include(x=>x.Song))).Select(x => x.Song.CategoryId).Take(3).ToList();

        if(userCategories.Count <= 2)
        {
            var songstop = (await _unitOfWork.Songs.GetAsync(filter: x => x.Status == Status.Public,
           orderBy: x => x.OrderByDescending(x => x.CountRate),
           includes: x => x.Include(s => s.Category).Include(s => s.Singer).Include(s => s.Album))).Take(100);

            return _mapper.Map<IEnumerable<SongDTO>>(songstop);
        }

        var songs = (await _unitOfWork.Songs.GetAsync(filter: x => userCategories.Contains(x.CategoryId) ,
            orderBy: x => x.OrderByDescending(x => x.CountRate),
            includes: x => x.Include(s => s.Category)
            .Include(s => s.Singer)
            .Include(s => s.Album))).Take(100);

        return _mapper.Map<IEnumerable<SongDTO>>(songs);
    }

    public async Task<IEnumerable<SongDTO>> GetTopSingerSongsAsync(Guid id)
    {
        var songs = (await _unitOfWork.Songs.GetAsync(filter: x => x.SingerId == id && x.Status == Status.Public,
            orderBy: x => x.OrderByDescending(x => x.CountRate),
            includes: x => x.Include(s => s.Category).Include(s => s.Singer).Include(s => s.Album))).Take(100);

        return _mapper.Map<IEnumerable<SongDTO>>(songs);
    }
    public async Task<IEnumerable<SongDTO>> SearchSongsAsync(SearchDTO search)
    {
        var songs = (await _unitOfWork.Songs.GetAsync(
            filter: x => x.Title.Contains(search.SearchTerm) && x.Status == Status.Public,
            orderBy: x => x.OrderByDescending(x => x.CountRate),
            includes: x => x.Include(s => s.Category).Include(s => s.Singer).Include(s => s.Album))).Take(100);

        return _mapper.Map<IEnumerable<SongDTO>>(songs);
    }
    public async Task<IEnumerable<SongDTO>> GetUserSongsAsync(Guid id)
    {
        var songs = (await _unitOfWork.UserSongs.GetAsync(
            filter: x => x.UserId == id,
            orderBy: x => x.OrderBy(x => x.Song.Title),
            includes: x => x.Include(x=> x.Song).ThenInclude(s => s.Category)
            .Include(x => x.Song).ThenInclude(s => s.Singer)
            .Include(x=> x.Song).ThenInclude(s => s.Album))).Select(x => x.Song);

        return _mapper.Map<IEnumerable<SongDTO>>(songs);
    }
    public async Task<IEnumerable<SongDTO>> GetUserRecentlySongsAsync(Guid id)
    {
        var songs = (await _unitOfWork.UserSongs.GetAsync(
            filter: x => x.UserId == id,
            orderBy: x => x.OrderByDescending(x => x.AddedTime),
            includes: x => x.Include(x => x.Song).ThenInclude(s => s.Category)
            .Include(x => x.Song).ThenInclude(s => s.Singer)
            .Include(x => x.Song).ThenInclude(s => s.Album))).Select(x => x.Song);

        return _mapper.Map<IEnumerable<SongDTO>>(songs);
    }

    public async Task<bool> RemoveSongFromUserAsync(Guid userId, Guid songId)
    {
        var entity = await _unitOfWork.UserSongs.GetFirstOrDefaultAsync(x => x.UserId == userId && x.SongId == songId);
        if (entity is null) throw new NotFoundException($"User Song with userId - {userId} and songId - {songId} ");

        _unitOfWork.UserSongs.Delete(entity);

        return await _unitOfWork.SaveAsync();

    }

    public async Task<bool> AddSongToUserAsync(Guid userId, Guid songId)
    {
       // var entity = await _unitOfWork.UserSongs.GetFirstOrDefaultAsync(x => x.UserId == userId && x.SongId == songId);
       // if (entity is not null) throw new BadRequestException($"User Song with userId - {userId} and songId - {songId} already exists.");

        var useralbum = new UserSong
        {
            UserId = userId,
            SongId = songId,
            AddedTime = DateTimeOffset.Now
        };
        await _unitOfWork.UserSongs.InsertAsync(useralbum);

        return await _unitOfWork.SaveAsync();
    }

}
