import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class DownloadService {
  constructor(private api: ApiService) {}

  async download(platform: string): Promise<void> {
    const url = `Download/${platform}`;

    try {
      const result = await this.api.get(url, {}, 'blob');

      const file: any = result.data;

      if (file instanceof Blob) {
        const a = document.createElement('a');
        const objectUrl = URL.createObjectURL(file);

        a.href = objectUrl;
        a.download = `chokart.${platform}`;
        a.click();

        URL.revokeObjectURL(objectUrl);
      } else {
        console.error('La respuesta no es un Blob válido');
      }
    } catch (error) {
      console.error('Error al intentar descargar el archivo:', error);
    }
  }
}
