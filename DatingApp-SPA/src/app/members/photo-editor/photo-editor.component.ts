import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
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
  @Output() getMemberPhotoChange = new EventEmitter<string>();
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

        /* if the response is Main photo, which mean there is no existing photo */
        if (photo.isMain) {
          this.authService.changeMemberPhoto(photo.url);
          /* update the user in local storage */
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
        }
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

        /* update the main photo on the left */
        /*this.getMemberPhotoChange.emit(photo.url);*/
        this.authService.changeMemberPhoto(photo.url);
        /* update the user in local storage */
        this.authService.currentUser.photoUrl = photo.url;
        localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
      },
      error => {
        this.alertify.error(error);
      }
    );
  }

  deletePhoto(id: number) {
    /* param: 1-string, 2-callback function */
    this.alertify.confirm('Are you sure you want to delete this photo?', () => {
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(
        () => {
          /* remove photo from photos array, (index, delete how many element) */
          this.photos.splice(
            this.photos.findIndex(p => p.id === id),
            1
          );
          this.alertify.success('Photo has been deleted');
        },
        error => {
          this.alertify.error('Failed to delete the photo');
        }
      );
    });
  }
}
