import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Player } from '../../../../core/models/game.models';

@Component({
  selector: 'app-cell',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cell.component.html',
  styleUrl: './cell.component.css'
})
export class CellComponent {
  mark = input<Player | null>(null);
  row = input.required<number>();
  col = input.required<number>();
  isWinning = input<boolean>(false);
  disabled = input<boolean>(false);

  clicked = output<void>();

  ariaLabel(): string {
    const markVal = this.mark();
    if (markVal) {
      return `Row ${this.row()}, Column ${this.col()}, marked with ${markVal}`;
    }
    return `Empty cell at Row ${this.row()}, Column ${this.col()}`;
  }

  onClick(): void {
    if (!this.disabled() && !this.mark()) {
      this.clicked.emit();
    }
  }
}
