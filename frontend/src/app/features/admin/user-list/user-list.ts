import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';

// Tablo için örnek veri modeli
export interface UserElement {
  id: number;
  name: string;
  email: string;
  role: 'Admin' | 'Customer' | 'Provider';
  status: 'Aktif' | 'Pasif';
}

const USER_DATA: UserElement[] = [
  {id: 1, name: 'Ahmet Yılmaz', email: 'ahmet@mail.com', role: 'Customer', status: 'Aktif'},
  {id: 2, name: 'Dr. Kemal', email: 'kemal@mail.com', role: 'Provider', status: 'Aktif'},
  {id: 3, name: 'Ayşe Demir', email: 'ayse@mail.com', role: 'Customer', status: 'Pasif'},
  {id: 4, name: 'Admin User', email: 'admin@randevu.com', role: 'Admin', status: 'Aktif'},
  {id: 5, name: 'Berber Mehmet', email: 'mehmet@mail.com', role: 'Provider', status: 'Aktif'},
];

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatCardModule, MatChipsModule],
  templateUrl: './user-list.html',
  styleUrl: './user-list.scss'
})
export class UserListComponent {
  displayedColumns: string[] = ['id', 'name', 'email', 'role', 'status', 'actions'];
  dataSource = USER_DATA;

  deleteUser(id: number) {
    if(confirm('Bu kullanıcıyı silmek istediğinize emin misiniz?')) {
      // Backend olmadığı için sadece listeden siliyormuş gibi yapalım
      this.dataSource = this.dataSource.filter(u => u.id !== id);
    }
  }
}