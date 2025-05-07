import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { TranslocoModule, TranslocoService } from '@jsverse/transloco';
import { Router } from '@angular/router';

@Component({
  selector: 'app-download',
  standalone: true,
  imports: [NavbarComponent, TranslocoModule],
  templateUrl: './download.component.html',
  styleUrl: './download.component.css',
})
export class DownloadComponent {
  constructor(public router: Router) {}
}
