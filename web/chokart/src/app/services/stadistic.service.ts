import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { RankingUser } from '../models/rankingUser';

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
}
