import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { RankingUser } from '../models/rankingUser';
import { UserBattle } from '../models/user-battle';
import { Battle } from '../models/battle';

@Injectable({
  providedIn: 'root',
})
export class StadisticService {
  constructor(public api: ApiService) {}

  async getRanking(): Promise<any> {
    const result = await this.api.get(`Stadistic/ranking`);
    const dataRaw: any = result.data;

    const users: RankingUser[] = [];

    for (const u of dataRaw) {
      const user: RankingUser = {
        id: u.id,
        nickname: u.nickname,
        avatarPath: u.avatarPath,
        totalPoints: u.totalPoints,
        steamId: u.steamId,
      };
      users.push(user);
    }
    return users;
  }

  async getBattles(userId: number): Promise<any> {
    const result = await this.api.get(`Stadistic/${userId}`);
    const dataRaw: any = result.data;

    const battles: Battle[] = [];

    for (const battle of dataRaw) {
      const userBattles: UserBattle[] = battle.usersBattles.map((ub: any) => ({
        id: ub.id,
        punctuation: ub.punctuation,
        timePlayed: ub.timePlayed ?? 0,
        battleResultId: ub.battleResultId,
        isHost: ub.isHost ?? false,
        character: ub.character ?? null,
        characterId: ub.characterId,
        position: ub.position,
        totalKills: ub.totalKills,
      }));

      const b: Battle = {
        id: battle.id,
        isAgainstBot: battle.isAgainstBot,
        userBattles: userBattles,
        createdAt: battle.createdAt,
        finishedAt: battle.finishedAt,
        trackId: battle.trackId,
        gameModeId: battle.gameModeId,
      };
      battles.push(b);
    }
    return battles;
  }
}
