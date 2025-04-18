import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { DownloadComponent } from './pages/download/download.component';
import { LoginComponent } from './pages/login/login.component';
import { ProfileComponent } from './pages/profile/profile.component';

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'download', component: DownloadComponent},
    {path: 'login', component: LoginComponent},
    {path: 'profile', component: ProfileComponent},
   // {path: 'play', component: PlayComponent}
   // {path: 'ranking', component: RankingComponent},
   // {path: 'dashboard', component: DashboardComponent}
];
