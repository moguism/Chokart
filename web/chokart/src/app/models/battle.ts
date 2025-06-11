import { UserBattle } from './user-battle';

export interface Battle {
  id: number;
  isAgainstBot: boolean;
  userBattles: UserBattle[];
  createdAt: Date;
  finishedAt: Date;
  trackId: number;
}
