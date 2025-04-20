import { StatusEnum } from "../../enums";

export interface AlbumFormInterface {
    id: string;
    title: string;
    status: StatusEnum;
    categoryId: string;
    albumImgUrl: string;
    albumImg?: File;
}
