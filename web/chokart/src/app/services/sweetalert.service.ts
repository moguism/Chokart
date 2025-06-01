import { Injectable } from '@angular/core';
import Swal, { SweetAlertIcon } from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class SweetalertService {

  constructor() { }

  public showAlert(title: string, message: string, icon: SweetAlertIcon) {
    Swal.fire({
      title: title,
      text: message,
      showConfirmButton: false,
      icon: icon,
      timer: 2000
    })
  }
}
