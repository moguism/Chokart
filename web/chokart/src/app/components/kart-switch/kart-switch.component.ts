import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-kart-switch',
  standalone: true,
  imports: [],
  templateUrl: './kart-switch.component.html',
  styleUrl: './kart-switch.component.css',
})
export class KartSwitchComponent {
  @Output() switched = new EventEmitter<boolean>();

  toggle(event: Event) {
    const checked = (event.target as HTMLInputElement).checked;
    this.switched.emit(checked);
  }
}
