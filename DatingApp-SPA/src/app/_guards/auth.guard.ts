import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router,
    private alerity: AlertifyService
  ) {}

  /* only can access the page if logged in */
  canActivate(next: ActivatedRouteSnapshot): boolean {
    const roles = next.firstChild.data['roles'] as Array<string>; /*from routes.ts*/
    if (roles) {
      const match = this.authService.roleMatch(roles);
      if (match) {
        return true;
      } else {
        this.router.navigate(['members']);
        this.alerity.error('You are not authorized to access this area');
      }
    }

    if (this.authService.loggedIn()) {
      return true;
    }

    this.alerity.error('You shall not pass!!!');
    this.router.navigate(['/home']);
    return false;
  }
}
