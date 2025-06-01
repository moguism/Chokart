import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { User } from '../models/user';


@Injectable({
  providedIn: 'root'
})

export class UserService {

  constructor(public api: ApiService) {}

  // buscar usuario -- en proceso --
  async searchUser(search: string): Promise<any> {
    const result = await this.api.get(`User/search?query=${search}`);
    const users: any = result.data;

    return users.users;

  }

  async getCurrentUser(id: number)
  {
    const user = await this.getUserById(id);
    return user
  }

  async getUserById(id : number): Promise<any> {
    const result = await this.api.get(`User/${id}`);
    const user: any = result.data;
    return user
  }

  async updateUser(formData: FormData, id: number): Promise<any> {
    return await this.api.putWithImage<any>(`User/${id}`, formData);
  }

  // devuelve todos los usuarios
  async allUser(): Promise<User[]> {
    const request = await this.api.get(`User/allUsers`)
    const dataRaw: any = request.data

    const users: User[] = []

    for (const u of dataRaw) {
      const user: User = {
        id: u.id,
        nickname: u.nickname,
        email: u.email,
        avatarPath: u.avatarPath,
        role: u.role,
        stateId: u.stateId,
        friendships: u.friendships,
        banned: u.banned,
        totalPoints: u.totalPoints,
        steamId: u.steamId,
        verificationCode: u.verificationCode
      }
      users.push(user);
    }
    return users;
  }

  // Modificar rol del usuario
  async modifyRole(id: number, newRole: string): Promise<any> {
    const body = {
        userId: id,
        newRole: newRole
    }
    return await this.api.put(`User/modifyUserRole`, body)
  }

  // Banear usuario
  async banUserAsync(userId: number){
    return await this.api.put(`User/banUser/${userId}`)
  }
}
