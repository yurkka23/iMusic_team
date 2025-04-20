import { StatusEnum } from "../../enums";
import { CategoryInterface } from "../category/category.interface";
import { UserInterface } from "../general";
import { SongInterface } from "../songs/song.interface";

export interface AlbumInterface {
    id: string;
    title: string;
    createdTime: string;
    status: StatusEnum;
    countRate: number;
    albumImgUrl: string;
    songs?: SongInterface[];
    singer: UserInterface;
    category: CategoryInterface;
}
