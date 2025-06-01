import { Component, OnInit } from '@angular/core';
import { User } from '../../models/user';
import { CustomRouterService } from '../../services/custom-router.service';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { SteamService } from '../../services/steam.service';
import { SteamProfile } from '../../models/steam-profile';
import { environment } from '../../../environments/environment';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { TranslocoModule } from '@jsverse/transloco';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PasswordValidatorService } from '../../services/password-validator.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [NavbarComponent, TranslocoModule, ReactiveFormsModule, CommonModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit {
  constructor(
    private userService: UserService,
    private authService: AuthService,
    private steamService: SteamService,
    private router: CustomRouterService,
    private formBuild: FormBuilder,
    private passwordValidator: PasswordValidatorService,
    private customRouter: CustomRouterService
  ) {
    this.userForm = this.formBuild.group({
      nickname: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]]
    });

    this.passwordForm = this.formBuild.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    },
      { validators: this.passwordValidator.passwordMatchValidator });
  }

  user: User | null = null;

  steamProfile: SteamProfile | null = null;

  STEAM_URL = '';

  userForm: FormGroup;
  passwordForm: FormGroup;
  isNewPasswordHidden = true; // Mostrar div de cambiar contraseña
  isEditing = false; //modo edición

  async ngOnInit() {
    if (!this.authService.isAuthenticated()) {
      this.router.navigateToUrl('login');
    }

    this.user = this.authService.getUser();
    console.log(this.user);

    await this.getCurrentUser();

    this.STEAM_URL = `${environment.apiUrl}SteamAuth/login/${this.user.id}/${this.user.verificationCode}`;
  }

  async getSteamDetails() {
    try {
      const data = await this.steamService.getUserDetailsById(
        this.user.steamId
      );
      console.log(data);

      this.steamProfile = {
        personaName: data.personaName,
        avatarFull: data.avatarFull,
        avatar: data.avatar,
        steamId: this.user.steamId,
      };
    } catch (error) {
      console.warn(error);
    }
  }

  async removeSteamAccount() {
    this.steamProfile = null;
    await this.steamService.removeAccount();
    await this.getCurrentUser();
  }

  async getCurrentUser() {
    this.user = await this.userService.getCurrentUser(this.user.id);
    this.authService.saveUser(this.user);
    this.userForm.reset(this.user);

    if (this.user.steamId != null && this.user.steamId != '') {
      await this.getSteamDetails();
    }
  }

  logOut() {
    this.authService.logout();
    this.customRouter.navigateToUrl('/');
  }

  async updateUser(): Promise<void> {
    const role = this.user?.role.toString();
    const formData = new FormData();
    formData.append("Nickname", this.userForm.value.nickname)
    formData.append("Email", this.userForm.value.email)
    const newPassword = this.passwordForm.get('newPassword')?.value
    
    if (newPassword) {
      if (!this.passwordForm.valid) {
        console.error("Error: La nueva contraseña no es válida.")
        return;
      }
      
      formData.append("Password", newPassword)
    }

    if (role) formData.append("Role", role)

    await this.userService.updateUser(formData, this.user.id)
    this.logOut() // Recargo siempre
  }

  edit() {
    this.isEditing = !this.isEditing;
    if (!this.isEditing) { // restaura los datos
      this.userForm.reset(this.user);
    }
  }

  editPassword() {
    const newPassword = this.passwordForm.get('newPassword')?.value;

    if (!newPassword) {
      console.error("Error: El campo de la contraseña está vacío.");
      return;
    }
  }

  showEditPassword() {
    this.isNewPasswordHidden = !this.isNewPasswordHidden;
  }
}
