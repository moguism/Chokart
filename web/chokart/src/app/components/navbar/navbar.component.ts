import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

import {
  RouterLink,
  RouterLinkActive,
  RouterLinkWithHref,
  RouterModule,
} from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { WebsocketService } from '../../services/websocket.service';
import { TranslocoService } from '@jsverse/transloco';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, RouterLink, RouterLinkWithHref, FormsModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent implements OnInit {
  constructor(
    private authService: AuthService,
    private websocketService: WebsocketService,
    private translocoService: TranslocoService
  ) {}
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

  // Listado de idiomas de ejemplo.
  readonly LANGUAGES = [
    { code: 'en', name: 'English', emoji: '🌭' },
    { code: 'es', name: 'Español', emoji: '🥘' },
  ];

  // Índice del idioma actualmente seleccionado.
  languageSelected: number;

  ngOnInit(): void {
    // Obtiene el idioma activo actual.
    const activeLang = this.translocoService.getActiveLang();
    // Busca el índice del idioma activo en la lista LANGUAGES y lo guarda.
    this.languageSelected = this.findLanguageIndex(activeLang);
  }

  // Devuelve el nombre del idioma detectado en el navegador del usuario.
  getBrowserLanguage(): string {
    return this.findLanguageName(window.navigator.language);
  }

  // Devuelve el nombre del idioma actualmente activo en Transloco.
  getActivatedLanguage() {
    return this.findLanguageName(this.translocoService.getActiveLang());
  }

  // Se llama cuando el usuario cambia el idioma desde la interfaz
  onLanguageChanged() {
    // Obtiene el idioma que el usuario ha seleccionado.
    const language = this.LANGUAGES[this.languageSelected];
    // Cambia el idioma activo en Transloco al seleccionado.
    this.translocoService.setActiveLang(language.code);
  }

  // Dado un código de idioma, devuelve el nombre del idioma.
  private findLanguageName(code: string) {
    // Extrae el código del idioma en 2 dígitos. Ej: "es-ES" -> "es".
    const shortCode = this.getShortCode(code);
    // Busca el idioma en la lista y devuelve su nombre. Si no lo encuentra, devuelve el código corto.
    return (
      this.LANGUAGES.find((language) => language.code == shortCode)?.name ??
      shortCode
    );
  }

  // Dado un código de idioma, devuelve su índice en la lista LANGUAGES.
  private findLanguageIndex(code: string) {
    // Extrae el código del idioma en 2 dígitos. Ej: "es-ES" -> "es".
    const shortCode = this.getShortCode(code);
    // Busca el índice del idioma correspondiente en la lista.
    return this.LANGUAGES.findIndex((language) => language.code == shortCode);
  }

  // Extrae el código corto de un lenguaje.
  private getShortCode(languageCode: string) {
    // Divide por "-" y devuelve la primera parte. Ej: "es-ES" -> "es".
    return languageCode.split('-')?.at(0);
  }
}
