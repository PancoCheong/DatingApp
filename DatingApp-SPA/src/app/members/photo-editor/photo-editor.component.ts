import { Component, OnInit, Input } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  baseUrl = environment.apiUrl;
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  currentMainPhoto: Photo;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.initializeUploader();
  }

  /* if mouse hover drop zone, fire an event */
  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false /*user has to click a button to upload*/,
      maxFileSize: 10 * 1024 * 1024 /* 10 MB */
    });

    this.uploader.onAfterAddingFile = file => {
      file.withCredentials = false;
    };

    /* display photo right after uploaded successfully */
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };
        this.photos.push(photo);
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(
      () => {
        /*console.log('Successfully set to main');*/
        /* use array filter to find the main photo*/
        /* change the Main button to green */

        this.currentMainPhoto = this.photos.filter(p => p.isMain === true)[0];
        this.currentMainPhoto.isMain = false;
        photo.isMain = true;
      },
      error => {
        this.alertify.error(error);
      }
    );
  }
}
