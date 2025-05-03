import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';

import {
  RouterLink,
  RouterLinkActive,
  RouterLinkWithHref,
  RouterModule,
} from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { WebsocketService } from '../../services/websocket.service';
import { TranslocoModule, TranslocoService } from '@jsverse/transloco';
import { SelectButtonModule } from 'primeng/selectbutton';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    RouterModule,
    RouterLink,
    RouterLinkWithHref,
    FormsModule,
    SelectButtonModule,
    TranslocoModule,
  ],
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
    {
      code: 'en',
      name: 'English',
      emoji: '🌭',
      flag: 'https://flagcdn.com/w320/gb.png',
    },
    {
      code: 'es',
      name: 'Español',
      emoji: '🥘',
      flag: 'https://flagcdn.com/w320/es.png',
    },
  ];

  // Índice del idioma actualmente seleccionado.
  languageSelected: number;

  languageSelect: string;

  ngOnInit(): void {
    // Obtiene el idioma activo actual.
    const activeLang = this.translocoService.getActiveLang();
    // Busca el índice del idioma activo en la lista LANGUAGES y lo guarda.
    this.languageSelected = this.findLanguageIndex(activeLang);
    this.languageSelect = this.getShortCode(activeLang);
  }

  getFlagUrl(langCode: string): string {
    const map: Record<string, string> = {
      en: 'gb',
      es: 'es',
    };
    const countryCode = map[langCode] || langCode;
    return `https://flagcdn.com/w40/${countryCode}.png`;
  }

  // Devuelve el nombre del idioma detectado en el navegador del usuario.
  getBrowserLanguage(): string {
    return this.findLanguageName(window.navigator.language);
  }

  // Devuelve el nombre del idioma actualmente activo en Transloco.
  getActivatedLanguage() {
    return this.findLanguageName(this.translocoService.getActiveLang());
  }

  @ViewChild('langSelector', { static: false })
  langSelector!: ElementRef<HTMLElement>;

  // Se llama cuando el usuario cambia el idioma desde la interfaz
  onLanguageChanged() {
    // Obtiene el idioma que el usuario ha seleccionado.
    const language = this.LANGUAGES[this.languageSelected];
    // Cambia el idioma activo en Transloco al seleccionado.
    this.translocoService.setActiveLang(language.code);

    this.langSelector.nativeElement.removeAttribute('open');
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
