import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Player } from '../../../core/models/game.models';

@Component({
  selector: 'app-cell',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button
      type="button"
      class="cell-button"
      [class.cell-x]="mark() === 'X'"
      [class.cell-o]="mark() === 'O'"
      [class.cell-winning]="isWinning()"
      [disabled]="disabled() || mark() !== null"
      [attr.aria-label]="ariaLabel()"
      (click)="onClick()"
    >
      @if (mark(); as m) {
        <span class="mark" [class.mark-pop]="true">{{ m }}</span>
      }
    </button>
  `,
  styles: [`
    :host {
      display: block;
      width: 100%;
      aspect-ratio: 1 / 1;
    }

    .cell-button {
      width: 100%;
      height: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
      background: var(--bg-surface);
      border: 2px solid var(--border-subtle);
      border-radius: var(--radius-md);
      transition: all var(--transition-fast);
      position: relative;
      overflow: hidden;
    }

    .cell-button:hover:not(:disabled) {
      background: var(--bg-glass-hover);
      border-color: var(--border-default);
      transform: translateY(-2px);
      box-shadow: var(--shadow-md);
    }

    .cell-button:active:not(:disabled) {
      transform: translateY(0);
    }

    .cell-button:disabled {
      cursor: not-allowed;
    }

    .mark {
      font-size: 3rem;
      font-weight: 800;
      line-height: 1;
      font-family: var(--font-main);
      user-select: none;
    }

    .mark-pop {
      animation: popIn var(--transition-bounce) forwards;
    }

    @keyframes popIn {
      0% {
        transform: scale(0.3);
        opacity: 0;
      }
      70% {
        transform: scale(1.15);
      }
      100% {
        transform: scale(1);
        opacity: 1;
      }
    }

    .cell-x .mark {
      color: var(--color-player-x);
      text-shadow: var(--shadow-glow-x);
    }

    .cell-o .mark {
      color: var(--color-player-o);
      text-shadow: var(--shadow-glow-o);
    }

    .cell-winning {
      background: var(--color-win-bg) !important;
      border-color: var(--color-win-highlight) !important;
      box-shadow: var(--shadow-glow-win) !important;
      animation: pulseWin 1.5s infinite ease-in-out;
    }

    .cell-winning .mark {
      color: var(--color-win-highlight) !important;
      text-shadow: var(--shadow-glow-win) !important;
    }

    @keyframes pulseWin {
      0%, 100% {
        box-shadow: 0 0 15px var(--color-win-glow);
      }
      50% {
        box-shadow: 0 0 30px var(--color-win-glow);
      }
    }
  `]
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
