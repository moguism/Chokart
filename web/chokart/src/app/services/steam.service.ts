import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class SteamService {

  constructor(private api: ApiService) { }

  async getUserDetailsById(id : string): Promise<any> {
    const result = await this.api.get(`SteamAuth/getId/${id}`);
    const user: any = result.data;
    return user
  }

  async removeAccount()
  {
    const result = await this.api.get(`SteamAuth/remove`);
    return result
  }
}
