import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameMode } from '../../../core/models/game.models';

@Component({
  selector: 'app-game-mode-selector',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      class="mode-selector-container"
      role="radiogroup"
      aria-label="Game mode selection"
    >
      <button
        id="mode-twoplayer-btn"
        type="button"
        class="mode-btn"
        [class.active]="selectedMode() === 'TwoPlayer'"
        [disabled]="disabled()"
        (click)="selectMode('TwoPlayer')"
        role="radio"
        [attr.aria-checked]="selectedMode() === 'TwoPlayer'"
        aria-label="Two-Player mode"
      >
        <span class="mode-icon" aria-hidden="true">👥</span>
        <span class="mode-text">Two-Player</span>
      </button>

      <button
        id="mode-computer-btn"
        type="button"
        class="mode-btn"
        [class.active]="selectedMode() === 'Computer'"
        [disabled]="disabled()"
        (click)="selectMode('Computer')"
        role="radio"
        [attr.aria-checked]="selectedMode() === 'Computer'"
        aria-label="Play Against Computer mode"
      >
        <span class="mode-icon" aria-hidden="true">🤖</span>
        <span class="mode-text">vs Computer</span>
      </button>
    </div>
  `,
  styles: [`
    .mode-selector-container {
      display: inline-flex;
      align-items: center;
      background: var(--bg-surface);
      border: 1px solid var(--border-default);
      border-radius: var(--radius-full);
      padding: var(--space-xs);
      box-shadow: var(--shadow-sm);
      gap: var(--space-xs);
      margin: 0 auto;
    }

    .mode-btn {
      display: inline-flex;
      align-items: center;
      gap: var(--space-sm);
      padding: var(--space-xs) var(--space-lg);
      background: transparent;
      color: var(--text-secondary);
      border: none;
      border-radius: var(--radius-full);
      font-family: var(--font-main);
      font-size: 0.95rem;
      font-weight: 500;
      cursor: pointer;
      transition: all var(--transition-normal);
      user-select: none;
    }

    .mode-btn:hover:not(:disabled):not(.active) {
      color: var(--text-primary);
      background: var(--bg-glass-hover);
    }

    .mode-btn.active {
      background: var(--color-primary);
      color: #ffffff;
      font-weight: 600;
      box-shadow: 0 0 16px var(--color-primary-glow);
    }

    .mode-btn:disabled {
      opacity: 0.45;
      cursor: not-allowed;
    }

    .mode-icon {
      font-size: 1.1rem;
    }

    .mode-text {
      letter-spacing: -0.01em;
    }
  `]
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
