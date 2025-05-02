import { Component, OnInit } from '@angular/core';
import {
  RouterLink,
  RouterLinkActive,
  RouterLinkWithHref,
  RouterModule,
} from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { WebsocketService } from '../../services/websocket.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, RouterLink, RouterLinkWithHref],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {

  constructor(private authService: AuthService, private websocketService: WebsocketService){} 
  isMenuOpen = false;

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMenu() {
    this.isMenuOpen = false;
  }

  /*async ngOnInit() {
      if(this.authService.isAuthenticated() && !this.websocketService.isConnectedNative())
      {
        this.websocketService.connectNative()
      }
  }*/
}
