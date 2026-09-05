import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-error-banner',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (error(); as err) {
      <div class="error-banner" role="alert" aria-live="assertive">
        <div class="error-content">
          <svg class="error-icon" viewBox="0 0 24 24" width="20" height="20" stroke="currentColor" stroke-width="2" fill="none">
            <circle cx="12" cy="12" r="10"></circle>
            <line x1="12" y1="8" x2="12" y2="12"></line>
            <line x1="12" y1="16" x2="12.01" y2="16"></line>
          </svg>
          <span class="error-text">{{ err }}</span>
        </div>
        <button
          type="button"
          class="close-button"
          aria-label="Dismiss error"
          (click)="onDismiss()"
        >
          &times;
        </button>
      </div>
    }
  `,
  styles: [`
    .error-banner {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: var(--space-md);
      padding: var(--space-sm) var(--space-md);
      background-color: var(--color-error-bg);
      border: 1px solid var(--color-error-border);
      border-radius: var(--radius-md);
      color: var(--color-error);
      font-size: 0.9rem;
      font-weight: 500;
      box-shadow: var(--shadow-sm);
      animation: fadeIn var(--transition-normal);
    }

    .error-content {
      display: flex;
      align-items: center;
      gap: var(--space-sm);
    }

    .error-icon {
      flex-shrink: 0;
    }

    .close-button {
      background: none;
      border: none;
      color: var(--color-error);
      font-size: 1.5rem;
      line-height: 1;
      padding: 0 var(--space-xs);
      cursor: pointer;
      opacity: 0.8;
      transition: opacity var(--transition-fast);
    }

    .close-button:hover {
      opacity: 1;
    }

    @keyframes fadeIn {
      from {
        opacity: 0;
        transform: translateY(-6px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }
  `]
})
export class ErrorBannerComponent {
  error = input<string | null>(null);
  dismissed = output<void>();

  onDismiss(): void {
    this.dismissed.emit();
  }
}
