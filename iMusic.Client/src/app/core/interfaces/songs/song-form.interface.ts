import { StatusEnum } from "../../enums";

export interface SongFormInterface {
    title: string;
    text?: string;
    status: StatusEnum;
    categoryName: string;
    songFile?: File;
    songImg?: File;
    songFileUrl: string;
    songImgUrl: string;
    albumId?: string; 
}
