import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameMode } from '../../../../core/models/game.models';

@Component({
  selector: 'app-game-mode-selector',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './game-mode-selector.component.html',
  styleUrl: './game-mode-selector.component.css'
})
export class GameModeSelectorComponent {
  selectedMode = input.required<GameMode>();
  disabled = input<boolean>(false);
  modeChange = output<GameMode>();

  selectMode(mode: GameMode): void {
    if (this.disabled()) {
      return;
    }

    if (mode !== this.selectedMode()) {
      this.modeChange.emit(mode);
    }
  }
}
