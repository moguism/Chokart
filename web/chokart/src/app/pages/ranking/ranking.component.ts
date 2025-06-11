import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { RankingUser } from '../../models/rankingUser';
import { StadisticService } from '../../services/stadistic.service';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { TranslocoModule } from '@jsverse/transloco';
import { AuthService } from '../../services/auth.service';
import { SteamProfile } from '../../models/steam-profile';
import { SteamService } from '../../services/steam.service';

@Component({
  selector: 'app-ranking',
  standalone: true,
  imports: [NavbarComponent, TranslocoModule],
  templateUrl: './ranking.component.html',
  styleUrl: './ranking.component.css',
})
export class RankingComponent implements OnInit {
  constructor(
    private stadisticService: StadisticService,
    private authService: AuthService
  ) {}

  ranking: RankingUser[] = [];
  isLog: boolean = false;

  async ngOnInit(): Promise<void> {
    this.ranking = await this.stadisticService.getRanking();
    // console.log(this.ranking);

    this.isLog = this.authService.isAuthenticated();
  }
}
