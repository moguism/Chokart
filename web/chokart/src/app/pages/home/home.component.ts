import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CustomRouterService } from '../../services/custom-router.service';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ButtonModule } from 'primeng/button';
import { TranslocoModule, TranslocoService } from '@jsverse/transloco';
import { TranslatorService } from '../../services/translator.service';
import { GalleriaModule } from 'primeng/galleria';
import { CharacterGalleriaComponent } from '../../components/character-galleria/character-galleria.component';
import { KartSwitchComponent } from '../../components/kart-switch/kart-switch.component';
import { trigger, transition, style, animate } from '@angular/animations';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    NavbarComponent,
    ButtonModule,
    TranslocoModule,
    GalleriaModule,
    CharacterGalleriaComponent,
    KartSwitchComponent,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  animations: [
    trigger('modeAnimation1', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateX(30px)' }),
        animate(
          '500ms ease-out',
          style({ opacity: 1, transform: 'translateX(0)' })
        ),
      ]),
    ]),
    trigger('modeAnimation2', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateX(-30px)' }),
        animate(
          '500ms ease-out',
          style({ opacity: 1, transform: 'translateX(0)' })
        ),
      ]),
    ]),
  ],
})
export class HomeComponent implements OnInit {
  constructor(
    public router: CustomRouterService,
    private translocoService: TranslocoService,
    private translatorService: TranslatorService
  ) {}

  languageSelected: number;
  playModeSwitch = false;

  ngOnInit(): void {
    // Obtiene el idioma activo actual.
    const activeLang = this.translocoService.getActiveLang();
    // Busca el índice del idioma activo en la lista LANGUAGES y lo guarda.
    this.languageSelected =
      this.translatorService.findLanguageIndex(activeLang);
  }

  onSwitchChange(checked: boolean) {
    this.playModeSwitch = checked;
  }
}
