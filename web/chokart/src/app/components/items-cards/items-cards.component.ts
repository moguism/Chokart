import { Component } from '@angular/core';
import { objects } from '../../data/objects';
import { TranslocoModule } from '@jsverse/transloco';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-items-cards',
  standalone: true,
  imports: [TranslocoModule, NgFor],
  templateUrl: './items-cards.component.html',
  styleUrl: './items-cards.component.css',
})
export class ItemsCardsComponent {
  objects = objects;
}
