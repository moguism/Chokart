import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class CustomRouterService {

  constructor(private router: Router) { }

  public navigateToUrl(url: string)
  {
    this.router.navigateByUrl(url);
  }
}
