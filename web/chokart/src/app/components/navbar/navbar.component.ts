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
import { TranslatorService } from '../../services/translator.service';

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
  // Listado de idiomas
  LANGUAGES: { code: string; name: string; emoji: string; flag: string }[] = [];

  constructor(
    private authService: AuthService,
    private websocketService: WebsocketService,
    private translocoService: TranslocoService,
    private translatorService: TranslatorService
  ) {}

  ngOnInit(): void {
    // Obtiene el idioma activo actual.
    const activeLang = this.translocoService.getActiveLang();
    // Busca el índice del idioma activo en la lista LANGUAGES y lo guarda.
    this.languageSelected =
      this.translatorService.findLanguageIndex(activeLang);
    this.languageSelect = this.translatorService.getShortCode(activeLang);

    this.LANGUAGES = this.translatorService.LANGUAGES;
  }

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

  // Índice del idioma actualmente seleccionado.
  languageSelected: number;
  languageSelect: string;

  getFlagUrl(langCode: string): string {
    const map: Record<string, string> = {
      en: 'gb',
      es: 'es',
    };
    const countryCode = map[langCode] || langCode;
    return `https://flagcdn.com/w40/${countryCode}.png`;
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
}
