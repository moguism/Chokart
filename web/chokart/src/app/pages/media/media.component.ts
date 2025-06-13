import { Component } from '@angular/core';
import { TranslocoModule, TranslocoService } from '@jsverse/transloco';
import { GalleriaModule } from 'primeng/galleria';
import { Router, RouterModule } from '@angular/router';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ImageModule } from 'primeng/image';

@Component({
  selector: 'app-media',
  standalone: true,
  imports: [RouterModule, NavbarComponent, TranslocoModule, ImageModule],
  templateUrl: './media.component.html',
  styleUrl: './media.component.css',
})
export class MediaComponent {}
