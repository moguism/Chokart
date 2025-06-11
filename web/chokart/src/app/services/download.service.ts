import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root',
})
export class DownloadService {
  constructor(private api: ApiService) {}

  async download(platform: string): Promise<void> {
    const url = `Download/${platform}`;

    try {
      const result = await this.api.get(url, {}, 'text');

      // console.log("RESULT:" , result)

      const file: any = result.data;

      if (file && file != '') {
        window.open(file, '_blank');
      }
    } catch (error) {
      console.error('Error al intentar descargar el archivo:', error);
    }
  }
}
