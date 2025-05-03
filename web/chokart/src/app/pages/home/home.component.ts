import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CustomRouterService } from '../../services/custom-router.service';
import { NavbarComponent } from '../../components/navbar/navbar.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [NavbarComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  constructor(public router: CustomRouterService) {}
}
