import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class PasswordValidatorService {
  passwordMatchValidator(form: FormGroup) {
    const password = form.get('newPassword')?.value;
    const confirmPasswordControl = form.get('confirmPassword');
    const confirmPassword = confirmPasswordControl?.value;

    if (!password || !confirmPassword) {
      return null;
    }
    return password === confirmPassword ? null : { mismatch: true }; // Si no coinciden, devuelve error
  }
}
