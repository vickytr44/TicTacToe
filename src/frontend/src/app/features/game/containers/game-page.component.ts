import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameStore } from '../state/game.store';
import { BoardComponent } from '../components/board.component';
import { ErrorBannerComponent } from '../components/error-banner.component';

@Component({
  selector: 'app-game-page',
  standalone: true,
  imports: [CommonModule, BoardComponent, ErrorBannerComponent],
  template: `
    <div class="game-page-container">
      <app-error-banner
        [error]="store.error()"
        (dismissed)="store.clearError()"
      />

      <section class="game-status-card" aria-live="polite">
        @if (store.status() === 'Won') {
          <div class="status-badge won" [class.winner-x]="store.winner() === 'X'" [class.winner-o]="store.winner() === 'O'">
            <span class="status-icon">🎉</span>
            <span class="status-text">Player {{ store.winner() }} Wins!</span>
          </div>
        } @else if (store.status() === 'Draw') {
          <div class="status-badge draw">
            <span class="status-icon">🤝</span>
            <span class="status-text">It's a Draw!</span>
          </div>
        } @else {
          <div class="status-badge turn" [class.turn-x]="store.currentPlayer() === 'X'" [class.turn-o]="store.currentPlayer() === 'O'">
            <span class="turn-dot"></span>
            <span class="status-text">Player {{ store.currentPlayer() }}'s Turn</span>
          </div>
        }
      </section>

      <section class="game-board-section">
        <app-board
          [board]="store.board()"
          [winningCells]="store.winningCells()"
          [disabled]="store.isPending() || store.isGameOver() || store.isLoading()"
          (cellClick)="onCellClick($event)"
        />
      </section>
    </div>
  `,
  styles: [`
    .game-page-container {
      display: flex;
      flex-direction: column;
      gap: var(--space-lg);
      width: 100%;
    }

    .game-status-card {
      display: flex;
      justify-content: center;
      align-items: center;
    }

    .status-badge {
      display: inline-flex;
      align-items: center;
      gap: var(--space-sm);
      padding: var(--space-xs) var(--space-lg);
      border-radius: var(--radius-full);
      font-size: 1.15rem;
      font-weight: 600;
      letter-spacing: -0.01em;
      box-shadow: var(--shadow-sm);
      transition: all var(--transition-normal);
    }

    .turn-dot {
      width: 10px;
      height: 10px;
      border-radius: 50%;
    }

    .turn-x {
      background: var(--color-player-x-bg);
      color: var(--color-player-x);
      border: 1px solid var(--color-player-x);
    }
    .turn-x .turn-dot {
      background: var(--color-player-x);
      box-shadow: var(--shadow-glow-x);
    }

    .turn-o {
      background: var(--color-player-o-bg);
      color: var(--color-player-o);
      border: 1px solid var(--color-player-o);
    }
    .turn-o .turn-dot {
      background: var(--color-player-o);
      box-shadow: var(--shadow-glow-o);
    }

    .status-badge.won {
      background: var(--color-win-bg);
      border: 1px solid var(--color-win-highlight);
      animation: bounceWin 0.6s cubic-bezier(0.34, 1.56, 0.64, 1);
    }
    .winner-x {
      color: var(--color-player-x);
    }
    .winner-o {
      color: var(--color-player-o);
    }

    .status-badge.draw {
      background: var(--bg-surface);
      color: var(--text-secondary);
      border: 1px solid var(--border-default);
    }

    @keyframes bounceWin {
      0% { transform: scale(0.85); opacity: 0; }
      100% { transform: scale(1); opacity: 1; }
    }

    .game-board-section {
      display: flex;
      justify-content: center;
      width: 100%;
    }
  `]
})
export class GamePageComponent implements OnInit {
  public readonly store = inject(GameStore);

  ngOnInit(): void {
    if (!this.store.game()) {
      this.store.createGame('TwoPlayer');
    }
  }

  onCellClick(position: { row: number; column: number }): void {
    this.store.makeMove(position);
  }
}
