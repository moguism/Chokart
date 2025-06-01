import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { NavbarComponent } from "../../components/navbar/navbar.component";

@Component({
  selector: 'app-verification',
  standalone: true,
  imports: [NavbarComponent],
  templateUrl: './verification.component.html',
  styleUrl: './verification.component.css'
})
export class VerificationComponent implements OnInit
{
  constructor(private authService: AuthService, private activatedRoute: ActivatedRoute) {}

  couldVerify: boolean = false
  loading: boolean = true

  async ngOnInit()
  {
    const paramMap = this.activatedRoute.snapshot.paramMap;
    const id = paramMap.get('id') as unknown as number;
    const code = paramMap.get('code') as unknown as string;

    this.couldVerify = await this.authService.verifyEmail(id, code)
    this.loading = false
  }

}
