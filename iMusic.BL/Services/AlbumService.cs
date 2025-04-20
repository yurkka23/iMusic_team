using AutoMapper;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.DAL.Abstractions;
using iMusic.DAL.DTOs.Album;
using iMusic.DAL.DTOs.Search;
using iMusic.DAL.DTOs.Song;
using iMusic.DAL.Entities;
using iMusic.DAL.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace iMusic.BL.Services;

public class AlbumService : IAlbumService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISongService _songService;
    private readonly IFileService _fileService;

    public AlbumService(IUnitOfWork unitOfWork, IMapper mapper, ISongService songService, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _songService = songService;
        _fileService = fileService; 
    }

    public async Task<Guid> CreateAlbumAsync(Guid currentUserId,CreateAlbumDTO createAlbumDTO)
    {
        var category = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Id == createAlbumDTO.CategoryId);
        if (category == null) throw new NotFoundException($"Category with id - {createAlbumDTO.CategoryId}");

        if (createAlbumDTO.AlbumImg is null) throw new NoFileException($"Image is required! Please add file");
        var img = await _fileService.GetFilePathAsync(createAlbumDTO.AlbumImg);

        var newAlbum = new Album()
        {
            Id = Guid.NewGuid(),
            Title = createAlbumDTO.Title,
            Status = createAlbumDTO.Status,
            CategoryId = createAlbumDTO.CategoryId,
            CreatedTime = DateTimeOffset.Now,
            SingerId = currentUserId,
            CountRate = 0,
            AlbumImgUrl = img
        };
        await _unitOfWork.Albums.InsertAsync(newAlbum);

         await _unitOfWork.SaveAsync();

        return newAlbum.Id;
    }

    public async Task<bool> EditAlbumAsync(EditAlbumDTO editAlbumDTO)
    {
        var category = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Id == editAlbumDTO.CategoryId);
        if (category == null) throw new NotFoundException($"Category with id - {editAlbumDTO.CategoryId}");

        var album = await _unitOfWork.Albums.GetFirstOrDefaultAsync(a => a.Id == editAlbumDTO.Id);
        if (album == null) throw new NotFoundException($"Album with id - {editAlbumDTO.Id}");

        if (editAlbumDTO.AlbumImg is not null)
        {
            var img = await _fileService.GetFilePathAsync(editAlbumDTO.AlbumImg);

            if (album.AlbumImgUrl != null) _fileService.DeleteFile(album.AlbumImgUrl);

            album.AlbumImgUrl = img;

        }

        album.Title = editAlbumDTO.Title;
        album.Status = editAlbumDTO.Status;
        album.CategoryId = editAlbumDTO.CategoryId;

        _unitOfWork.Albums.Update(album);

        return await _unitOfWork.SaveAsync();
    }

    public async Task<bool> DeleteAlbumAsync(Guid albumId)
    {
        var album = await _unitOfWork.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId, a => a.Include(a => a.Songs));
        if (album == null) throw new NotFoundException($"Album with id - {albumId}");

        if (album.AlbumImgUrl != null) _fileService.DeleteFile(album.AlbumImgUrl);

        if (album.Songs is not null)
        {
            var albumSongs = album!.Songs!.Select(s => s.Id);

            foreach (var id in albumSongs)
            {
                await _songService.DeleteSongAsync(id);
            }
        }

        var albumRes = await _unitOfWork.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId, a => a.Include(a => a.Songs));

        var favUserAlbums = await _unitOfWork.FavoriteAlbums.GetAsync(x => x.AlbumId == albumId);

        var userAlbums = await _unitOfWork.UserAlbums.GetAsync(x => x.AlbumId == albumId);

        _unitOfWork.FavoriteAlbums.Delete(favUserAlbums);

        _unitOfWork.UserAlbums.Delete(userAlbums);

        _unitOfWork.Albums.Delete(albumRes);

        return await _unitOfWork.SaveAsync();
    }
    public async Task<bool> RemoveAlbumFromUserAsync(Guid userId, Guid albumId)
    {
        var entity = await _unitOfWork.UserAlbums.GetFirstOrDefaultAsync(x => x.UserId == userId && x.AlbumId == albumId);
        if (entity is null) throw new NotFoundException($"User Album with userId - {userId} and albumId - {albumId} ");

        _unitOfWork.UserAlbums.Delete(entity);

        return await _unitOfWork.SaveAsync();

    }

    public async Task<bool> AddAlbumToUserAsync(Guid userId, Guid albumId)
    {

        var useralbum = new UserAlbum
        {
            UserId = userId,
            AlbumId = albumId,
            AddedTime = DateTimeOffset.Now
        };
        await _unitOfWork.UserAlbums.InsertAsync(useralbum);

        return await _unitOfWork.SaveAsync();
    }


    public async Task AddSongToAlbum(Guid currentUserId, UploadSongDTO uploadSongDTO)
    {
        var album = await _unitOfWork.Albums.GetFirstOrDefaultAsync(a => a.Id == uploadSongDTO.AlbumId);

        if (album is null) throw new NotFoundException($"Album with Id - {uploadSongDTO.AlbumId} ");

        await _songService.UploadSongAsync(currentUserId, uploadSongDTO);
    }

    public async Task RemoveSongFromAlbum(Guid albumId, Guid songId)
    {
        var album = await _unitOfWork.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId);

        if (album is null) throw new NotFoundException($"Album with Id - {albumId} ");

        await _songService.DeleteSongAsync(songId);

    }

    public async Task<AlbumDTO> GetAlbum(Guid id)
    {
        var album = await _unitOfWork.Albums.GetFirstOrDefaultAsync(a => a.Id == id,
            includes: a => a.Include(a => a.Category).Include(a => a.User)
            .Include(a => a.Songs).ThenInclude(s => s.Singer)
            .Include(s => s.Songs).ThenInclude(s => s.Category));

        return _mapper.Map<AlbumDTO>(album);
    }

    public async Task<IEnumerable<AlbumDTO>> GetAlbums()
    {
        var albums = await _unitOfWork.Albums.GetAsync(
            filter: s => s.Status == Status.Public,
            includes: a => a
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Songs).ThenInclude(s => s.Singer));

        return _mapper.Map<IEnumerable<AlbumDTO>>(albums);
    }

    public async Task<IEnumerable<AlbumDTO>> GetSingerAlbums(Guid singerId)
    {
        var albums = await _unitOfWork.Albums.GetAsync(a => a.SingerId == singerId,
            orderBy: x => x.OrderByDescending(x => x.CreatedTime),
            includes: a => a
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Songs).ThenInclude(s => s.Singer));

        return _mapper.Map<IEnumerable<AlbumDTO>>(albums);
    }

    public async Task<IEnumerable<AlbumDTO>> GetRecommendsAlbum()
    {
        var albums = (await _unitOfWork.Albums.GetAsync(
            filter: s => s.Status == Status.Public,
            orderBy: x => x.OrderByDescending(x => x.CountRate),
            includes: a => a
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Songs).ThenInclude(s => s.Singer))).Take(100);

        return _mapper.Map<IEnumerable<AlbumDTO>>(albums);
    }
    public async Task<IEnumerable<AlbumDTO>> SearchAlbumsAsync(SearchDTO search)
    {
        var albums = (await _unitOfWork.Albums.GetAsync(
            filter: x => x.Title.Contains(search.SearchTerm) && x.Status == Status.Public,
            orderBy: x => x.OrderByDescending(x => x.CountRate),
            includes: a => a
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Songs).ThenInclude(s => s.Singer))).Take(100);

        return _mapper.Map<IEnumerable<AlbumDTO>>(albums);
    }

    public async Task<IEnumerable<AlbumDTO>> GetUserAlbumsAsync(Guid id)
    {
        var albumsIds = (await _unitOfWork.UserAlbums.GetAsync(
            filter: x => x.UserId == id,
            includes: x => x.Include(x => x.Album))).Select(x => x.Album.Id);

        var albums = (await _unitOfWork.Albums.GetAsync(
            filter: x => albumsIds.Contains(x.Id),
            orderBy: x => x.OrderByDescending(x => x.CountRate),
            includes: a => a
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Songs).ThenInclude(s => s.Singer)));

        return _mapper.Map<IEnumerable<AlbumDTO>>(albums);
    }

    public async Task<IEnumerable<AlbumDTO>> GetUserRecentlyAlbumsAsync(Guid id)
    {
        var albumsIds = (await _unitOfWork.UserAlbums.GetAsync(
           filter: x => x.UserId == id,
           includes: x => x.Include(x => x.Album),
           orderBy: x => x.OrderByDescending(x => x.AddedTime))).Select(x => x.Album.Id);

        var albums = (await _unitOfWork.Albums.GetAsync(
            filter: x => albumsIds.Contains(x.Id),
            includes: a => a
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Songs).ThenInclude(s => s.Singer)));

        return _mapper.Map<IEnumerable<AlbumDTO>>(albums);
    }

}
