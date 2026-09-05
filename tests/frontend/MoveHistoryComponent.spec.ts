import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MoveHistoryComponent } from '../../src/frontend/src/app/features/game/components/move-history.component';
import { MoveDto } from '../../src/frontend/src/app/core/models/game.models';
import { describe, it, expect, beforeEach } from 'vitest';

describe('MoveHistoryComponent', () => {
  let fixture: ComponentFixture<MoveHistoryComponent>;
  let component: MoveHistoryComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MoveHistoryComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(MoveHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should display empty message when no moves are made', () => {
    fixture.componentRef.setInput('moves', []);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const emptyState = compiled.querySelector('.empty-history');
    expect(emptyState).toBeTruthy();
    expect(emptyState?.textContent).toContain('No moves yet');
  });

  it('should render chronological moves with move number, player, and coordinates', () => {
    const mockMoves: MoveDto[] = [
      { moveNumber: 1, player: 'X', row: 1, column: 1 },
      { moveNumber: 2, player: 'O', row: 2, column: 2 },
      { moveNumber: 3, player: 'X', row: 1, column: 3 }
    ];

    fixture.componentRef.setInput('moves', mockMoves);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const moveItems = compiled.querySelectorAll('.move-item');
    expect(moveItems.length).toBe(3);

    expect(moveItems[0].textContent).toContain('1');
    expect(moveItems[0].textContent).toContain('X');
    expect(moveItems[0].textContent).toContain('(1, 1)');

    expect(moveItems[1].textContent).toContain('2');
    expect(moveItems[1].textContent).toContain('O');
    expect(moveItems[1].textContent).toContain('(2, 2)');

    expect(moveItems[2].textContent).toContain('3');
    expect(moveItems[2].textContent).toContain('X');
    expect(moveItems[2].textContent).toContain('(1, 3)');
  });

  it('should apply player-specific styling classes', () => {
    const mockMoves: MoveDto[] = [
      { moveNumber: 1, player: 'X', row: 1, column: 1 },
      { moveNumber: 2, player: 'O', row: 2, column: 2 }
    ];

    fixture.componentRef.setInput('moves', mockMoves);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const moveItems = compiled.querySelectorAll('.move-item');
    expect(moveItems[0].classList.contains('move-x')).toBe(true);
    expect(moveItems[1].classList.contains('move-o')).toBe(true);
  });
});
