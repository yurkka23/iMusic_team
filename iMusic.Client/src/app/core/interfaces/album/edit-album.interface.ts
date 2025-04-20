import { StatusEnum } from "../../enums";

export interface EditAlbumInterface {
    id: string;
    title: string;
    status: StatusEnum;
    categoryId: string;
    albumImg?: File;
 }

