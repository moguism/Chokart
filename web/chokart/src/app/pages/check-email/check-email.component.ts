import { Component } from '@angular/core';
import { Location } from '@angular/common';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-check-email',
  standalone: true,
  imports: [NavbarComponent, TranslocoModule],
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
