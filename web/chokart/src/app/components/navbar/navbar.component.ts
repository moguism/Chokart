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
import { Language } from '../../models/language';

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
  languages: Language[] = [];
  selectedLangIndex: number;

  constructor(
    private authService: AuthService,
    private websocketService: WebsocketService,
    private translocoService: TranslocoService,
    private translatorService: TranslatorService
  ) {}

  // Índice del idioma actualmente seleccionado.
  languageSelected: number;
  languageSelect: string;

  ngOnInit(): void {
    this.languages = this.translatorService.LANGUAGES;

    const activeLang = this.translocoService.getActiveLang();
    this.languageSelected =
      this.translatorService.findLanguageIndex(activeLang);
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
    const language = this.languages[this.languageSelected];
    // Cambia el idioma activo en Transloco al seleccionado.
    this.translocoService.setActiveLang(language.code);

    this.langSelector.nativeElement.removeAttribute('open');
  }
}
