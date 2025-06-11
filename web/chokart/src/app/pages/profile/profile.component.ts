import { Component, OnInit } from '@angular/core';
import { User } from '../../models/user';
import { CustomRouterService } from '../../services/custom-router.service';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { SteamService } from '../../services/steam.service';
import { SteamProfile } from '../../models/steam-profile';
import { environment } from '../../../environments/environment';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { TranslocoModule, TranslocoService } from '@jsverse/transloco';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { PasswordValidatorService } from '../../services/password-validator.service';
import { CommonModule } from '@angular/common';
import { SweetalertService } from '../../services/sweetalert.service';
import { StadisticService } from '../../services/stadistic.service';
import { UserBattle } from '../../models/user-battle';
import { Battle } from '../../models/battle';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    NavbarComponent,
    TranslocoModule,
    ReactiveFormsModule,
    CommonModule,
  ],
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
    public customRouter: CustomRouterService,
    private sweetAlertService: SweetalertService,
    private translocoService: TranslocoService,
    private stadisticService: StadisticService
  ) {
    this.userForm = this.formBuild.group({
      nickname: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
    });

    this.passwordForm = this.formBuild.group(
      {
        newPassword: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
      },
      { validators: this.passwordValidator.passwordMatchValidator }
    );
  }

  user: User | null = null;

  steamProfile: SteamProfile | null = null;

  STEAM_URL = '';

  userForm: FormGroup;
  passwordForm: FormGroup;
  isNewPasswordHidden = true; // Mostrar div de cambiar contraseña
  isEditing = false; //modo edición

  battles: Battle[] = [];
  async ngOnInit() {
    if (!this.authService.isAuthenticated()) {
      this.router.navigateToUrl('login');
    }

    this.user = this.authService.getUser();
    //  console.log(this.user);

    if (this.user == null) {
      this.logOut();
      this.router.navigateToUrl('login');
    }

    await this.getCurrentUser();

    this.STEAM_URL = `${environment.apiUrl}SteamAuth/login/${this.user.id}/${this.user.verificationCode}`;

    this.battles = await this.stadisticService.getBattles(this.user.id);
    console.log(this.battles);
  }

  async getSteamDetails() {
    try {
      const data = await this.steamService.getUserDetailsById(
        this.user.steamId
      );
      //  console.log(data);

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

    let shouldReload = false;

    const nickname = this.userForm.value.nickname;
    const mail = this.userForm.value.email;

    if (this.user.nickname != nickname || this.user.email != mail) {
      shouldReload = true;
    }

    formData.append('Nickname', nickname);
    formData.append('Email', mail);
    const newPassword = this.passwordForm.get('newPassword')?.value;

    if (newPassword) {
      if (!this.passwordForm.valid) {
        console.error('Error: La nueva contraseña no es válida.');
        return;
      }

      formData.append('Password', newPassword);
      shouldReload = true;
    }

    if (role) formData.append('Role', role);

    await this.userService.updateUser(formData, this.user.id);
    if (shouldReload) {
      this.sweetAlertService.showAlert(
        'Info',
        this.translocoService.translate('closing-session'),
        'info'
      );
      this.logOut(); // Recargo siempre
    }

    this.edit();
  }

  edit() {
    this.isEditing = !this.isEditing;
    this.isNewPasswordHidden = !this.isNewPasswordHidden;
    if (!this.isEditing) {
      // restaura los datos
      this.userForm.reset(this.user);
      this.passwordForm.reset();
    }
  }

  editPassword() {
    const newPassword = this.passwordForm.get('newPassword')?.value;

    if (!newPassword) {
      console.error('Error: El campo de la contraseña está vacío.');
      return;
    }
  }
}
