import { StatusEnum } from "../../enums";

export interface EditPlaylistInterface {
    id: string;
    title: string;
    status: StatusEnum;
    playlistImg?: File;
 }