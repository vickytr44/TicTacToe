import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-game-controls',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './game-controls.component.html',
  styleUrl: './game-controls.component.css'
})
export class GameControlsComponent {
  disabled = input<boolean>(false);
  canUndo = input<boolean>(false);
  reset = output<void>();
  undo = output<void>();

  onReset(): void {
    if (!this.disabled()) {
      this.reset.emit();
    }
  }

  onUndo(): void {
    if (!this.disabled() && this.canUndo()) {
      this.undo.emit();
    }
  }
}
