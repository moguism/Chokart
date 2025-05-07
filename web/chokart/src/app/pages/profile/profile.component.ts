import { Component, OnInit } from '@angular/core';
import { User } from '../../models/user';
import { CustomRouterService } from '../../services/custom-router.service';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { SteamService } from '../../services/steam.service';
import { ActivatedRoute } from '@angular/router';
import { SteamProfile } from '../../models/steam-profile';
import { environment } from '../../../environments/environment';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [NavbarComponent, TranslocoModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit {
  constructor(
    private userService: UserService,
    private authService: AuthService,
    private steamService: SteamService,
    private router: CustomRouterService,
    private activatedRoute: ActivatedRoute
  ) {}

  user: User | null = null;

  steamProfile: SteamProfile | null = null;

  STEAM_URL = '';

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

    if (this.user.steamId != null && this.user.steamId != '') {
      await this.getSteamDetails();
    }
  }

  logOut() {
    this.authService.logout();
    this.router.navigateToUrl('/');
  }
}
