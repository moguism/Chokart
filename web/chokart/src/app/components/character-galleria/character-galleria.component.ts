import { Component } from '@angular/core';
import { TranslocoModule } from '@jsverse/transloco';
import { GalleriaModule } from 'primeng/galleria';

@Component({
  selector: 'app-character-galleria',
  standalone: true,
  imports: [GalleriaModule, TranslocoModule],
  templateUrl: './character-galleria.component.html',
  styleUrl: './character-galleria.component.css',
})
export class CharacterGalleriaComponent {
  characters = [
    {
      name: 'Shrek',
      image: 'characters/shrek.webp',
      miniImage: 'characters/shrek-mini.png',
      quoteKey: 'shrek-quote',
    },
    {
      name: 'Alastor',
      image: 'characters/alastor.webp',
      miniImage: 'characters/alastor-mini.png',
      quoteKey: 'alastor-quote',
    },
    {
      name: 'Pingu',
      image: 'characters/pingu.webp',
      miniImage: 'characters/pingu-mini.png',

      quoteKey: 'pingu-quote',
    },
    {
      name: 'Jinx',
      image: 'characters/jinx.webp',
      miniImage: 'characters/jinx-mini.png',

      quoteKey: 'jinx-quote',
    },
    {
      name: 'Doraemon',
      image: 'characters/doraemon.webp',
      miniImage: 'characters/doraemon-mini.png',

      quoteKey: 'doraemon-quote',
    },
    {
      name: 'Spamton ',
      image: 'characters/spamton.png',
      miniImage: 'characters/spamton-mini.png',
      quoteKey: 'spamton-quote',
    },
  ];

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
