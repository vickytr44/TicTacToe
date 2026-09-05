import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameStore } from '../state/game.store';
import { BoardComponent } from '../components/board.component';
import { ErrorBannerComponent } from '../components/error-banner.component';
import { GameControlsComponent } from '../components/game-controls.component';
import { GameModeSelectorComponent } from '../components/game-mode-selector.component';
import { MoveHistoryComponent } from '../components/move-history.component';
import { GameMode } from '../../../core/models/game.models';

@Component({
  selector: 'app-game-page',
  standalone: true,
  imports: [
    CommonModule,
    BoardComponent,
    ErrorBannerComponent,
    GameControlsComponent,
    GameModeSelectorComponent,
    MoveHistoryComponent
  ],
  templateUrl: './game-page.component.html',
  styleUrl: './game-page.component.css'
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

  onResetGame(): void {
    this.store.resetGame();
  }

  onUndoMove(): void {
    this.store.undoMove();
  }

  onModeChange(mode: GameMode): void {
    this.store.switchMode(mode);
  }
}
