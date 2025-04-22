import { User } from './user';

export interface LoginResponse {
  accessToken: string;
  user: User;
}
