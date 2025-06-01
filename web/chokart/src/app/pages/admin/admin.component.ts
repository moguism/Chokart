import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from "../../components/navbar/navbar.component";
import { User } from '../../models/user';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../services/auth.service';
import { CustomRouterService } from '../../services/custom-router.service';


@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [FormsModule, NavbarComponent],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent implements OnInit {

  public readonly IMG_URL = environment.apiImg;
  
  users: User[] = [] // lista de usuarios

  constructor(
    private userService: UserService, 
    private authService: AuthService, 
    private customRouter: CustomRouterService
  ){}

  async ngOnInit() {
    if (!this.authService.isAuthenticated() || !this.authService.isAdmin()) {
      this.customRouter.navigateToUrl('/');
    }

    await this.loadUsers();
  }

  async loadUsers(){
    this.users = await this.userService.allUser()
  }

  // Editar el rol de un usuario
  async modifyUserRole(userId: number, newRole: string) {
    try {
      await this.userService.modifyRole(userId, newRole)
    } catch (error) {
      console.error("Error al modificar el rol", error)
    }
    await this.loadUsers()
  }

  // Banear a un usuario
  async banUser(userId: number) {
    try {

      const confirmation = confirm(`¿Estás seguro de que quieres cambiar el estado del baneo del usuario #${userId}?`)
      if (confirmation){
        await this.userService.banUserAsync(userId)
      }
      
    } catch (error) {
      console.error("Error al banear este usuario", error)
    }
    await this.loadUsers()
  }
}