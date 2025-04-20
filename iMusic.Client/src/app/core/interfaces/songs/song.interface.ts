import { StatusEnum } from "../../enums";
import { CategoryInterface } from "../category/category.interface";

export interface SongInterface {
    id: string;
    title: string;
    text: string;
    songUrl: string;
    duration: number;
    countRate: number;
    songImgUrl: string;
    status: StatusEnum;
    createdTime: string;
    singerId: string;
    singerFullName: string;
    singerUserName: string;
    category: CategoryInterface;
    albumId?: string;
    albumTitle?: string;
}
