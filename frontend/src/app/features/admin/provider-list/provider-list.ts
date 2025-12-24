import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatSlideToggleModule } from '@angular/material/slide-toggle'; // Onay butonu

export interface ProviderElement {
  id: number;
  businessName: string;
  ownerName: string;
  category: string;
  rating: number;
  isApproved: boolean;
}

const PROVIDER_DATA: ProviderElement[] = [
  {id: 1, businessName: 'Altın Makas Berber', ownerName: 'Mehmet Yılmaz', category: 'Berber', rating: 4.5, isApproved: true},
  {id: 2, businessName: 'Diş Kliniği A+', ownerName: 'Dr. Zeynep', category: 'Diş Hekimi', rating: 5.0, isApproved: true},
  {id: 3, businessName: 'Güzellik Merkezi', ownerName: 'Ayşe Kaya', category: 'Güzellik', rating: 0, isApproved: false}, 
];

@Component({
  selector: 'app-provider-list',
  standalone: true,
  imports: [
    CommonModule, 
    MatTableModule, 
    MatButtonModule, 
    MatIconModule, 
    MatCardModule, 
    MatChipsModule,
    MatSlideToggleModule
  ],
  templateUrl: './provider-list.html',
  styleUrl: './provider-list.scss'
})
export class ProviderListComponent {
  displayedColumns: string[] = ['id', 'businessName', 'category', 'rating', 'status', 'actions'];
  dataSource = PROVIDER_DATA;

  toggleApproval(provider: ProviderElement) {
    provider.isApproved = !provider.isApproved;
    console.log(`${provider.businessName} onayı değişti: ${provider.isApproved}`);
  }
}