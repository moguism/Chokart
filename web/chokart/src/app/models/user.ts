import { Friend } from "./friend";

export interface User {
  id: number;
  nickname: string;
  email: string;
  role: string;
  avatarPath: string;
  banned: boolean;
  stateId: number;
  totalPoints: number;
  friendships: Friend[];
  steamId: string;
  verificationCode: string
}
