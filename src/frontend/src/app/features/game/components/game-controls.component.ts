import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-game-controls',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="game-controls-container" role="toolbar" aria-label="Game controls">
      <button
        id="reset-game-btn"
        type="button"
        class="control-btn reset-btn"
        [disabled]="disabled()"
        (click)="onReset()"
        aria-label="Reset current game"
      >
        <span class="btn-icon" aria-hidden="true">🔄</span>
        <span class="btn-text">Reset Game</span>
      </button>
    </div>
  `,
  styles: [`
    .game-controls-container {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: var(--space-md);
      width: 100%;
      padding: var(--space-xs) 0;
    }

    .control-btn {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: var(--space-sm);
      padding: var(--space-sm) var(--space-xl);
      background: var(--bg-surface);
      color: var(--text-primary);
      border: 1px solid var(--border-default);
      border-radius: var(--radius-full);
      font-family: var(--font-main);
      font-size: 0.95rem;
      font-weight: 600;
      letter-spacing: -0.01em;
      cursor: pointer;
      box-shadow: var(--shadow-sm);
      transition: all var(--transition-normal);
      user-select: none;
    }

    .control-btn:hover:not(:disabled) {
      background: var(--bg-elevated);
      border-color: var(--color-primary);
      box-shadow: 0 0 16px var(--color-primary-glow);
      transform: translateY(-2px);
    }

    .control-btn:active:not(:disabled) {
      transform: translateY(0) scale(0.98);
    }

    .control-btn:disabled {
      opacity: 0.45;
      cursor: not-allowed;
      transform: none;
      box-shadow: none;
    }

    .btn-icon {
      font-size: 1.05rem;
      display: inline-block;
      transition: transform var(--transition-fast);
    }

    .control-btn:hover:not(:disabled) .btn-icon {
      transform: rotate(45deg);
    }

    .btn-text {
      white-space: nowrap;
    }
  `]
})
export class GameControlsComponent {
  disabled = input<boolean>(false);
  reset = output<void>();

  onReset(): void {
    if (!this.disabled()) {
      this.reset.emit();
    }
  }
}
