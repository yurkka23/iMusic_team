import {ConfirmationPasswordInterface} from './confirmation-password.interface';

export interface SignUpInterface extends ConfirmationPasswordInterface {
  email: string;
  userName: string;
  firstName: string;
  lastName: string;
  password: string;
}
