import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MoveDto } from '../../../core/models/game.models';

@Component({
  selector: 'app-move-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './move-history.component.html',
  styleUrl: './move-history.component.css'
})
export class MoveHistoryComponent {
  moves = input<MoveDto[]>([]);
}
