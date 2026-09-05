import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-error-banner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './error-banner.component.html',
  styleUrl: './error-banner.component.css'
})
export class ErrorBannerComponent {
  error = input<string | null>(null);
  dismissed = output<void>();

  onDismiss(): void {
    this.dismissed.emit();
  }
}
