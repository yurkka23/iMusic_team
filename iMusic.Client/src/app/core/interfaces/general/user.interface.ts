export interface UserInterface {
    id: string;
    email: string;
    firstName: string;
    userName: string;
    lastName: string;
    registerTime: string;
    aboutMe?: string;
    userImgUrl?: string;
    isBanned?: boolean;
    wantToBeSinger?:boolean;
    userRoles?: string[];
    bannedTime?: string;
  }
  