import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GamePageComponent } from './game-page.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { GameStore } from '../state/game.store';
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
});
