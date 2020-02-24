import { Photo } from './photo';

export interface User {
  id: number;
  userName: string;
  knownAs: string;
  age: number;
  gender: string;
  created: Date;
  lastActive: any;
  photoUrl: string;
  city: string;
  country: string;
  interests?: string /*optional, must define after all required fields*/;
  introduction?: string;
  lookingFor?: string;
  photos?: Photo[];
  roles?: string[];
}
