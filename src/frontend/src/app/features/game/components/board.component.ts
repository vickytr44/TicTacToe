import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CellComponent } from './cell.component';
import { CellPositionDto, Player } from '../../../core/models/game.models';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [CommonModule, CellComponent],
  template: `
    <div class="board-container">
      <div class="board-grid" role="grid" aria-label="Tic-Tac-Toe Game Board">
        @for (row of [1, 2, 3]; track row; let rIdx = $index) {
          <div class="board-row" role="row">
            @for (col of [1, 2, 3]; track col; let cIdx = $index) {
              <div
                class="cell"
                [class.winning]="isCellWinning(row, col)"
                [attr.data-row]="row"
                [attr.data-col]="col"
                role="gridcell"
                (click)="onCellClick(row, col)"
              >
                <app-cell
                  [mark]="getCellMark(rIdx, cIdx)"
                  [row]="row"
                  [col]="col"
                  [isWinning]="isCellWinning(row, col)"
                  [disabled]="disabled()"
                />
              </div>
            }
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .board-container {
      display: flex;
      justify-content: center;
      padding: var(--space-md);
      background: var(--bg-glass);
      backdrop-filter: blur(16px);
      -webkit-backdrop-filter: blur(16px);
      border: 1px solid var(--border-subtle);
      border-radius: var(--radius-xl);
      box-shadow: var(--shadow-lg);
    }

    .board-grid {
      display: flex;
      flex-direction: column;
      gap: var(--space-sm);
      width: 100%;
      max-width: 400px;
    }

    .board-row {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: var(--space-sm);
    }

    .cell {
      width: 100%;
      display: flex;
      cursor: pointer;
    }
  `]
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
