import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameStore } from '../../state/game.store';
import { BoardComponent } from '../../components/board/board.component';
import { ErrorBannerComponent } from '../../components/error-banner/error-banner.component';
import { GameControlsComponent } from '../../components/game-controls/game-controls.component';
import { GameModeSelectorComponent } from '../../components/game-mode-selector/game-mode-selector.component';
import { MoveHistoryComponent } from '../../components/move-history/move-history.component';
import { ScoreboardComponent } from '../../components/scoreboard/scoreboard.component';
import { GameMode } from '../../../../core/models/game.models';

@Component({
  selector: 'app-game-page',
  standalone: true,
  imports: [
    CommonModule,
    BoardComponent,
    ErrorBannerComponent,
    GameControlsComponent,
    GameModeSelectorComponent,
    MoveHistoryComponent,
    ScoreboardComponent
  ],
  templateUrl: './game-page.component.html',
  styleUrl: './game-page.component.css'
})
export class GamePageComponent implements OnInit {
  public readonly store = inject(GameStore);

  ngOnInit(): void {
    this.store.loadScoreboard();
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

  onResetScoreboard(): void {
    this.store.resetScoreboard();
  }
}
