import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})

// MemberCardComponent is the child of MemberListComponent
export class MemberCardComponent implements OnInit {
  // pass down the user into card component
  @Input() user: User;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {}

  sendLike(id: number) {
    // id is reci
    this.userService.sendLike(this.authService.decodedToken.nameid, id).subscribe(
      data => {
        this.alertify.success('You have liked: ' + this.user.knownAs);
      },
      error => {
        this.alertify.error(error);
      }
    );
  }
}
