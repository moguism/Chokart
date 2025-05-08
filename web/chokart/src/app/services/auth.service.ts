import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParams,
  HttpResponse,
} from '@angular/common/http';
import { Result } from '../models/result';
import { lastValueFrom, Observable } from 'rxjs';
import { ApiService } from './api.service';
import { LoginRequest } from '../models/loginRequest';
import { LoginResponse } from '../models/loginResponse';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly USER_KEY = 'user';
  private readonly TOKEN_KEY = 'jwtToken';
  rememberMe: boolean = false;

  constructor(private api: ApiService) {
    const token =
      localStorage.getItem(this.TOKEN_KEY) ||
      sessionStorage.getItem(this.TOKEN_KEY);
    if (token) {
      this.api.jwt = token;
    }
  }

  async login(
    authData: LoginRequest,
    rememberMe: boolean
  ): Promise<Result<LoginResponse>> {
    // Iniciar sesión
    const result = await this.api.post<LoginResponse>('Auth/login', authData);
    this.rememberMe = rememberMe;

    if (result.success && result.data) {
      const { accessToken, user } = result.data; // guardo info de la respuesta AuthResponse
      this.api.jwt = accessToken;

      if (rememberMe) {
        // Si se pulsó el botón recuérdame
        localStorage.setItem(this.TOKEN_KEY, accessToken);
        this.saveUser(user);
      } else {
        sessionStorage.setItem(this.TOKEN_KEY, accessToken);
        this.saveUser(user);
      }
    }

    return result;
  }

  public saveUser(user: User) {
    if (this.rememberMe) {
      localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    } else {
      sessionStorage.setItem(this.USER_KEY, JSON.stringify(user));
    }
  }

  // Comprobar si el usuario esta logeado
  isAuthenticated(): boolean {
    const token =
      localStorage.getItem(this.TOKEN_KEY) ||
      sessionStorage.getItem(this.TOKEN_KEY);
    return !!token;
  }

  // Cerrar sesión
  logout(): void {
    sessionStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.TOKEN_KEY);
    sessionStorage.removeItem(this.USER_KEY);
    localStorage.removeItem(this.USER_KEY);
  }

  getUser(): User | null {
    const user =
      localStorage.getItem(this.USER_KEY) ||
      sessionStorage.getItem(this.USER_KEY);
    if (user) {
      return JSON.parse(user);
    }
    return null;
  }

  // comprueba si es admin
  isAdmin(): boolean {
    const user = this.getUser();
    if (user?.role == 'Admin') {
      return true;
    } else {
      return false;
    }
  }

  // Registro
  async register(data: any): Promise<Result<any>> {
    return this.api.post<any>('Auth/register', data);
  }

  // Verificar el mail
  async verifyEmail(id: number, code: string): Promise<any> {
    const path = 'Auth/verify/' + id + '/' + code;
    const result = await this.api.get(path);
    const data: any = result.data;
    return data;
  }
}
