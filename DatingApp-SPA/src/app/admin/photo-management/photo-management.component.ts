import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photos: any;

  constructor(private adminService: AdminService, private alertify: AlertifyService) {}

  ngOnInit() {
    this.getPhotosForApproval();
  }

  getPhotosForApproval() {
    this.adminService.getPhotosForApproval().subscribe(
      photos => {
        this.photos = photos;
      },
      error => {
        console.log(error);
        this.alertify.error(error);
      }
    );
  }

  /*remove photo from array for both approve and reject */
  approvePhoto(photoId) {
    this.adminService.approvePhoto(photoId).subscribe(
      () => {
        this.photos.splice(
          this.photos.findIndex(p => p.id === photoId),
          1
        );
      },
      error => {
        console.log(error);
        this.alertify.error(error);
      }
    );
  }

  rejectPhoto(photoId) {
    this.adminService.rejectPhoto(photoId).subscribe(
      () => {
        this.photos.splice(
          this.photos.findIndex(p => p.id === photoId),
          1
        );
      },
      error => {
        console.log(error);
        this.alertify.error(error);
      }
    );
  }
}
