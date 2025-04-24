import { Component } from '@angular/core';
import { Location } from '@angular/common';

@Component({
  selector: 'app-check-email',
  standalone: true,
  imports: [],
  templateUrl: './check-email.component.html',
  styleUrl: './check-email.component.css',
})
export class CheckEmailComponent {
  email: string = '';

  constructor(private location: Location) {}

  ngOnInit(): void {
    const state = this.location.getState() as { email?: string };
    this.email = state?.email || '';
  }
}
