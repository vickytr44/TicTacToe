import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CellComponent } from '../cell/cell.component';
import { CellPositionDto, Player } from '../../../../core/models/game.models';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [CommonModule, CellComponent],
  templateUrl: './board.component.html',
  styleUrl: './board.component.css'
})
export class BoardComponent {
  board = input<(Player | null)[][]>([
    [null, null, null],
    [null, null, null],
    [null, null, null]
  ]);
  winningCells = input<CellPositionDto[]>([]);
  disabled = input<boolean>(false);

  cellClick = output<{ row: number; column: number }>();

  getCellMark(rIdx: number, cIdx: number): Player | null {
    const b = this.board();
    if (b && b[rIdx]) {
      return b[rIdx][cIdx] ?? null;
    }
    return null;
  }

  isCellWinning(row: number, col: number): boolean {
    const winning = this.winningCells();
    return winning.some((c) => c.row === row && c.column === col);
  }

  onCellClick(row: number, column: number): void {
    if (!this.disabled() && !this.getCellMark(row - 1, column - 1)) {
      this.cellClick.emit({ row, column });
    }
  }
}
