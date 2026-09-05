import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BoardComponent } from './board.component';
import { By } from '@angular/platform-browser';

describe('BoardComponent', () => {
  let component: BoardComponent;
  let fixture: ComponentFixture<BoardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BoardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(BoardComponent);
    component = fixture.componentInstance;
  });

  it('should create and render 9 cells', () => {
    fixture.componentRef.setInput('board', [
      [null, null, null],
      [null, null, null],
      [null, null, null]
    ]);
    fixture.detectChanges();

    const cells = fixture.debugElement.queryAll(By.css('.cell'));
    expect(cells.length).toBe(9);
  });

  it('should emit cellClick with 1-based coordinates when an empty cell is clicked', () => {
    fixture.componentRef.setInput('board', [
      [null, null, null],
      [null, null, null],
      [null, null, null]
    ]);
    fixture.detectChanges();

    let clickedCoord: { row: number; column: number } | undefined;
    component.cellClick.subscribe((coord) => {
      clickedCoord = coord;
    });

    const firstCell = fixture.debugElement.query(By.css('[data-row="1"][data-col="1"]'));
    firstCell.nativeElement.click();

    expect(clickedCoord).toEqual({ row: 1, column: 1 });
  });

  it('should not emit cellClick when the board is disabled or pending', () => {
    fixture.componentRef.setInput('board', [
      [null, null, null],
      [null, null, null],
      [null, null, null]
    ]);
    fixture.componentRef.setInput('disabled', true);
    fixture.detectChanges();

    let emitted = false;
    component.cellClick.subscribe(() => {
      emitted = true;
    });

    const firstCell = fixture.debugElement.query(By.css('[data-row="1"][data-col="1"]'));
    firstCell.nativeElement.click();

    expect(emitted).toBe(false);
  });

  it('should highlight winning cells', () => {
    fixture.componentRef.setInput('board', [
      ['X', 'X', 'X'],
      [null, 'O', null],
      [null, null, 'O']
    ]);
    fixture.componentRef.setInput('winningCells', [
      { row: 1, column: 1 },
      { row: 1, column: 2 },
      { row: 1, column: 3 }
    ]);
    fixture.detectChanges();

    const winningCells = fixture.debugElement.queryAll(By.css('.cell.winning'));
    expect(winningCells.length).toBe(3);
  });
});
