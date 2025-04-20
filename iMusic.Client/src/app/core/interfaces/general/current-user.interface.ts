export interface CurrentUserInterface {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    aboutMe: string;
    userImgUrl: string;
    isBanned: boolean | null;
    bannedTime: string | null;
    registerTime: string;
    userRoles: string[];
  }