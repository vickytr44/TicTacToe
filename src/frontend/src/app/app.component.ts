import { Component, signal } from '@angular/core';
import { GamePageComponent } from './features/game/containers/game-page.component';

@Component({
  imports: [GamePageComponent],
  selector: 'app-root',
  styleUrl: './app.component.css',
  templateUrl: './app.component.html',
})
export class App {
  protected readonly title = signal('Tic Tac Toe');
}
