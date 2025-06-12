import { Character } from './character';

export interface UserBattle {
  id: number;
  userId: number;
  punctuation: number;
  timePlayed: number;
  battleResultId: number;
  isHost: boolean;
  character: Character;
  characterId: number;
  position: number;
  totalKills: number;
}
