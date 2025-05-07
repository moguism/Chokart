import { Component } from '@angular/core';
import { NavbarComponent } from "../../components/navbar/navbar.component";

@Component({
  selector: 'app-unity-game',
  standalone: true,
  imports: [NavbarComponent],
  templateUrl: './unity-game.component.html',
  styleUrl: './unity-game.component.css'
})
export class UnityGameComponent  {

}