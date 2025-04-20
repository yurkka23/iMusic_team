import { StatusEnum } from "../../enums";
import { SongFormInterface } from "./song-form.interface";

export interface EditSongInterface  {
   id: string;
   title: string;
   text?: string;
   status: StatusEnum;
   categoryName: string;
   songFile?: File;
   songImg?: File;
}