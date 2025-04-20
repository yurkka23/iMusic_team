import { AlbumInterface } from "../album/album.interface";
import { PlaylistInterface } from "../playlist/playlist.interface";
import { SongInterface } from "../songs/song.interface";

export interface FavoriteListInterface {
    id: string;
    songs?: SongInterface[];
    playlists?: PlaylistInterface[];
    albums?: AlbumInterface[];
}
