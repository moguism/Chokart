import { Component, OnInit } from '@angular/core';
import { TranslocoModule } from '@jsverse/transloco';
import { GalleriaModule } from 'primeng/galleria';
import { characters } from '../../data/characters';

@Component({
  selector: 'app-character-galleria',
  standalone: true,
  imports: [GalleriaModule, TranslocoModule],
  templateUrl: './character-galleria.component.html',
  styleUrl: './character-galleria.component.css',
})
export class CharacterGalleriaComponent {
  characters = characters;

  responsiveOptions: any[] = [
    {
      breakpoint: '1300px',
      numVisible: 3,
    },
    {
      breakpoint: '575px',
      numVisible: 1,
    },
  ];
}
