using AutoMapper;
using iMusic.DAL.Constants;
using iMusic.DAL.DTOs.Album;
using iMusic.DAL.DTOs.Category;
using iMusic.DAL.DTOs.FavoriteList;
using iMusic.DAL.DTOs.Playlist;
using iMusic.DAL.DTOs.Song;
using iMusic.DAL.DTOs.User;
using iMusic.DAL.Entities;

namespace iMusic.BL.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDTO>()
             .ForMember(s => s.UserImgUrl, s => s.MapFrom(map => map.UserImgUrl == null ? null : HostConstant.CurrentHost + map.UserImgUrl));

        CreateMap<Category, CategoryDTO>()
             .ForMember(s => s.CategoryImgUrl, s => s.MapFrom(map => map.CategoryImgUrl == null ? null : HostConstant.CurrentHost + map.CategoryImgUrl));

        CreateMap<Song, SongDTO>()
            .ForMember(s => s.SingerFullName, s => s.MapFrom(map => map.Singer.FirstName + ' ' + map.Singer.LastName))
            .ForMember(s => s.SingerUserName, s => s.MapFrom(map => map.Singer.UserName))
            .ForMember(s => s.Category, s => s.MapFrom(map => map.Category))
            .ForMember(s => s.AlbumTitle, s => s.MapFrom(map => map.Album.Title))
            .ForMember(s => s.SongImgUrl, s => s.MapFrom(map => map.SongImgUrl == null ? null : HostConstant.CurrentHost + map.SongImgUrl))
            .ForMember(s => s.SongUrl, s => s.MapFrom(map => map.SongUrl == null ? null : HostConstant.CurrentHost + map.SongUrl));


        CreateMap<FavoriteList, FavoriteListDTO>()
            .ForMember(s => s.Id, s => s.MapFrom(map => map.UserId))
            .ForMember(s => s.Songs, s => s.MapFrom(map => map.Songs.Select(t => t.Song)))
            .ForMember(s => s.Albums, s => s.MapFrom(map => map.Albums.Select(t => t.Album)))
            .ForMember(s => s.Playlists, s => s.MapFrom(map => map.Playlists.Select(t => t.Playlist)));

        CreateMap<Album, AlbumDTO>()
            .ForMember(s => s.Singer, s => s.MapFrom(map => map.User))
            .ForMember(s => s.AlbumImgUrl, s => s.MapFrom(map => map.AlbumImgUrl == null ? null : HostConstant.CurrentHost + map.AlbumImgUrl));

        CreateMap<Playlist, PlaylistDTO>()
            .ForMember(s => s.Songs, s => s.MapFrom(map => map.SongPlaylists.Select(t => t.Song)))
            .ForMember(s => s.Author, s => s.MapFrom(map => map.User))
            .ForMember(s => s.PlaylistImgUrl, s => s.MapFrom(map => map.PlaylistImgUrl == null ? null : HostConstant.CurrentHost + map.PlaylistImgUrl));


    }
}
