import { StatusEnum } from "../../enums";

export interface PlaylistFormInterface {
    id: string;
    title: string;
    status: StatusEnum;
    playlistImgUrl: string;
    playlistImg?: File;
}
