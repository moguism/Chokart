import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
  Validator,
} from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule, NgIf } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { PasswordValidatorService } from '../../services/password-validator.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  menuSeleccion: 'login' | 'register' = 'login';

  emailOrNickname: string = '';
  password: string = '';
  rememberMe: boolean = false;
  jwt: string = '';

  registerForm: FormGroup;

  image: File | null = null;

  imgSeleccionada: Boolean = false;

  imagePreview!: string;

  pressedEnter: Boolean = false;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private passwordValidator: PasswordValidatorService
  ) {
    this.registerForm = this.formBuilder.group(
      {
        nickname: ['', Validators.required],
        email: [
          '',
          [
            Validators.required,
            Validators.email,
            Validators.pattern(
              /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/
            ),
          ],
        ],
        newPassword: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
      },
      { validators: this.passwordValidator.passwordMatchValidator }
    );
  }

  async login() {
    if (this.pressedEnter) return;
    this.pressedEnter = true;

    const authData = {
      emailOrNickname: this.emailOrNickname,
      password: this.password,
    };
    const result = await this.authService.login(authData, this.rememberMe);

    if (result.success) {
      if (result.data) {
        this.jwt = result.data.accessToken;
      } else {
        console.error('No se encontró información en result.data');
      }

      if (this.rememberMe) {
        localStorage.setItem('jwtToken', this.jwt);
      }

      alert('Inicio de sesión exitoso');

      this.router.navigateByUrl('');
    } else {
      alert('datos incorrectos');
      this.pressedEnter = false;
    }
  }

  // Registro
  async register() {
    if (this.pressedEnter) return;
    this.pressedEnter = true;

    if (this.registerForm.valid) {
      const formUser = this.registerForm.value;

      const registerPayload = {
        id: 0,
        nickname: formUser.nickname,
        email: formUser.email,
        password: formUser.newPassword,
        role: 'User',
        changeImage: 'true',
      };

      const registerResult = await this.authService.register(registerPayload);

      if (registerResult.success) {
        const authData = {
          emailOrNickname: formUser.email,
          password: formUser.newPassword,
        };
        const loginResult = await this.authService.login(authData, false);

        if (loginResult.success) {
          alert('Te has registrado con éxito');
          this.router.navigateByUrl('');
        } else {
          alert('Error en el login tras el registro');
          this.pressedEnter = false;
        }
      } else {
        alert('Error en el registro');
        this.pressedEnter = false;
      }
    } else {
      alert('Formulario no válido');
      this.pressedEnter = false;
    }
  }

  onFileSelected(event: any) {
    const image = event.target.files[0] as File;
    this.image = image;

    this.imgSeleccionada = true;

    if (event.target.files.length > 0) {
      const reader = new FileReader();
      reader.onload = (event: any) => {
        this.imagePreview = event.target.result;
      };
      reader.readAsDataURL(event.target.files[0]);
    }
  }
}
