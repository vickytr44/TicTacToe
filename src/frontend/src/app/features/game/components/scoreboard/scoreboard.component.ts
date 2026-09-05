import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ScoreboardResponse } from '../../../../core/models/game.models';

@Component({
  selector: 'app-scoreboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './scoreboard.component.html',
  styleUrl: './scoreboard.component.css'
})
export class ScoreboardComponent {
  scoreboard = input.required<ScoreboardResponse>();
  disabled = input<boolean>(false);
  resetScoreboard = output<void>();

  onReset(): void {
    if (!this.disabled()) {
      this.resetScoreboard.emit();
    }
  }
}
