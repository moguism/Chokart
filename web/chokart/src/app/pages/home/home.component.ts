import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CustomRouterService } from '../../services/custom-router.service';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ButtonModule } from 'primeng/button';
import { TranslocoModule, TranslocoService } from '@jsverse/transloco';
import { TranslatorService } from '../../services/translator.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [NavbarComponent, ButtonModule, TranslocoModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  constructor(
    public router: CustomRouterService,
    private translocoService: TranslocoService,
    private translatorService: TranslatorService
  ) {}

  languageSelected: number;

  ngOnInit(): void {
    // Obtiene el idioma activo actual.
    const activeLang = this.translocoService.getActiveLang();
    // Busca el índice del idioma activo en la lista LANGUAGES y lo guarda.
    this.languageSelected =
      this.translatorService.findLanguageIndex(activeLang);
  }
}
