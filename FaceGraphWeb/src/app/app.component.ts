import { ImageFile } from './image-file';
import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  constructor(private http: HttpClient) {
    this.updateGalary();
  }
  title = 'FaceGraphWeb';

  images: ImageFile[];

  public updateGalary(): void {
    this.http.get<ImageFile[]>('http://localhost:51182/api/images')
      .subscribe(event => {
        this.images = event;
      });
  }
  public deleteImage(fileName: string): void {
      this.http.delete('http://localhost:51182/api/images/' + fileName)
      .subscribe(event => {
        this.updateGalary();
      });
  }

  /**
   * clearContainer
   */
  public clearContainer() {
    this.http.delete('http://localhost:51182/api/container/clear')
      .subscribe(event => {
        this.updateGalary();
      });
  }
}
