import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ScoreboardComponent } from './scoreboard.component';
import { By } from '@angular/platform-browser';

describe('ScoreboardComponent', () => {
  let component: ScoreboardComponent;
  let fixture: ComponentFixture<ScoreboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScoreboardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(ScoreboardComponent);
    component = fixture.componentInstance;
  });

  it('should create and display win and draw counts', () => {
    fixture.componentRef.setInput('scoreboard', { xWins: 5, oWins: 3, draws: 2 });
    fixture.detectChanges();

    const xScore = fixture.debugElement.query(By.css('.score-card.x .score-val'));
    const oScore = fixture.debugElement.query(By.css('.score-card.o .score-val'));
    const drawScore = fixture.debugElement.query(By.css('.score-card.draws .score-val'));

    expect(xScore.nativeElement.textContent.trim()).toBe('5');
    expect(oScore.nativeElement.textContent.trim()).toBe('3');
    expect(drawScore.nativeElement.textContent.trim()).toBe('2');
  });

  it('should emit resetScoreboard when reset button is clicked', () => {
    fixture.componentRef.setInput('scoreboard', { xWins: 1, oWins: 1, draws: 0 });
    fixture.detectChanges();

    const resetSpy = vi.spyOn(component.resetScoreboard, 'emit');
    const button = fixture.debugElement.query(By.css('.reset-scoreboard-btn'));

    button.nativeElement.click();
    expect(resetSpy).toHaveBeenCalled();
  });

  it('should disable reset button when disabled input is true', () => {
    fixture.componentRef.setInput('scoreboard', { xWins: 2, oWins: 1, draws: 1 });
    fixture.componentRef.setInput('disabled', true);
    fixture.detectChanges();

    const button = fixture.debugElement.query(By.css('.reset-scoreboard-btn'));
    expect(button.nativeElement.disabled).toBe(true);

    const resetSpy = vi.spyOn(component.resetScoreboard, 'emit');
    button.nativeElement.click();
    expect(resetSpy).not.toHaveBeenCalled();
  });
});
