import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GamePageComponent } from './game-page.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { GameStore } from '../../state/game.store';
import { By } from '@angular/platform-browser';

describe('GamePageComponent', () => {
  let component: GamePageComponent;
  let fixture: ComponentFixture<GamePageComponent>;
  let store: InstanceType<typeof GameStore>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GamePageComponent],
      providers: [provideHttpClient(), provideHttpClientTesting(), GameStore]
    }).compileComponents();

    fixture = TestBed.createComponent(GamePageComponent);
    component = fixture.componentInstance;
    store = TestBed.inject(GameStore);
    fixture.detectChanges();
  });

  it('should create and display turn status', () => {
    expect(component).toBeTruthy();
    const statusText = fixture.debugElement.query(By.css('.status-text'));
    expect(statusText.nativeElement.textContent).toContain("Player X's Turn");
  });

  it('should display winning badge when game is won', () => {
    store.updateGame({
      id: '123',
      board: [
        ['X', 'X', 'X'],
        [null, 'O', null],
        [null, null, 'O']
      ],
      currentPlayer: 'O',
      gameMode: 'TwoPlayer',
      status: 'Won',
      winner: 'X',
      winningCells: [
        { row: 1, column: 1 },
        { row: 1, column: 2 },
        { row: 1, column: 3 }
      ],
      moves: [],
      createdAt: new Date().toISOString()
    });
    fixture.detectChanges();

    const wonBadge = fixture.debugElement.query(By.css('.status-badge.won'));
    expect(wonBadge).toBeTruthy();
    expect(wonBadge.nativeElement.textContent).toContain('Player X Wins!');
  });

  it('should display draw badge and disable board when game is a draw', () => {
    store.updateGame({
      id: '123',
      board: [
        ['X', 'O', 'X'],
        ['X', 'O', 'O'],
        ['O', 'X', 'X']
      ],
      currentPlayer: 'O',
      gameMode: 'TwoPlayer',
      status: 'Draw',
      winner: null,
      winningCells: [],
      moves: [],
      createdAt: new Date().toISOString()
    });
    fixture.detectChanges();

    const drawBadge = fixture.debugElement.query(By.css('.status-badge.draw'));
    expect(drawBadge).toBeTruthy();
    expect(drawBadge.nativeElement.textContent).toContain("It's a Draw!");

    const board = fixture.debugElement.query(By.css('app-board'));
    expect(board.componentInstance.disabled()).toBe(true);
  });

  it('should render game controls and call onResetGame when reset is clicked', () => {
    const controls = fixture.debugElement.query(By.css('app-game-controls'));
    expect(controls).toBeTruthy();

    const resetSpy = vi.spyOn(component, 'onResetGame');
    const storeResetSpy = vi.spyOn(store, 'resetGame');

    controls.componentInstance.reset.emit();
    expect(resetSpy).toHaveBeenCalled();
    expect(storeResetSpy).toHaveBeenCalled();
  });

  it('should render mode selector and call onModeChange when mode is changed', () => {
    const selector = fixture.debugElement.query(By.css('app-game-mode-selector'));
    expect(selector).toBeTruthy();

    const modeChangeSpy = vi.spyOn(component, 'onModeChange');
    const storeSwitchSpy = vi.spyOn(store, 'switchMode');

    selector.componentInstance.modeChange.emit('Computer');
    expect(modeChangeSpy).toHaveBeenCalledWith('Computer');
    expect(storeSwitchSpy).toHaveBeenCalledWith('Computer');
  });

  it('should render move history component with store moves', () => {
    store.updateGame({
      id: '123',
      board: [[null, null, null], [null, null, null], [null, null, null]],
      currentPlayer: 'O',
      gameMode: 'TwoPlayer',
      status: 'InProgress',
      winner: null,
      winningCells: [],
      moves: [{ moveNumber: 1, player: 'X', row: 1, column: 1 }],
      createdAt: new Date().toISOString()
    });
    fixture.detectChanges();

    const history = fixture.debugElement.query(By.css('app-move-history'));
    expect(history).toBeTruthy();
    expect(history.componentInstance.moves().length).toBe(1);
    expect(history.nativeElement.textContent).toContain('(1, 1)');
  });

  it('should render game controls and call onUndoMove when undo is emitted', () => {
    const controls = fixture.debugElement.query(By.css('app-game-controls'));
    expect(controls).toBeTruthy();

    const undoSpy = vi.spyOn(component, 'onUndoMove');
    const storeUndoSpy = vi.spyOn(store, 'undoMove');

    controls.componentInstance.undo.emit();
    expect(undoSpy).toHaveBeenCalled();
    expect(storeUndoSpy).toHaveBeenCalled();
  });
});
