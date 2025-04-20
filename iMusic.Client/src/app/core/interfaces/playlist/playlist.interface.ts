import { StatusEnum } from "../../enums";
import { UserInterface } from "../general";
import { SongInterface } from "../songs/song.interface";

export interface PlaylistInterface {
    id: string;
    title: string;
    playlistImgUrl: string;
    status: StatusEnum;
    createdTime: string;
    countRate: number;
    author: UserInterface;
    songs?: SongInterface[];
}
