import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { TranslocoModule, TranslocoService } from '@jsverse/transloco';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { DownloadService } from '../../services/download.service';
import { FooterComponent } from '../../components/footer/footer.component';

@Component({
  selector: 'app-download',
  standalone: true,
  imports: [NavbarComponent, TranslocoModule, FooterComponent],
  templateUrl: './download.component.html',
  styleUrl: './download.component.css',
})
export class DownloadComponent {
  constructor(
    public router: Router,
    private downloadService: DownloadService
  ) {}

  downloadFile(platform: string) {
    this.downloadService.download(platform).catch((error) => {
      console.error('Error durante la descarga', error);
    });
  }

  goToGame() {
    window.open('https://moguism.itch.io/chokart');
  }
}
