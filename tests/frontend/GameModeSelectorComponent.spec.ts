import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GameModeSelectorComponent } from '../../src/frontend/src/app/features/game/components/game-mode-selector/game-mode-selector.component';
import { GameMode } from '../../src/frontend/src/app/core/models/game.models';
import { describe, it, expect, beforeEach } from 'vitest';

describe('GameModeSelectorComponent', () => {
  let fixture: ComponentFixture<GameModeSelectorComponent>;
  let component: GameModeSelectorComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GameModeSelectorComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(GameModeSelectorComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('selectedMode', 'TwoPlayer');
    fixture.detectChanges();
  });

  it('should render both mode options with unique IDs', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const twoPlayerBtn = compiled.querySelector('#mode-twoplayer-btn') as HTMLButtonElement;
    const computerBtn = compiled.querySelector('#mode-computer-btn') as HTMLButtonElement;

    expect(twoPlayerBtn).toBeTruthy();
    expect(computerBtn).toBeTruthy();
    expect(twoPlayerBtn.textContent).toContain('Two-Player');
    expect(computerBtn.textContent).toContain('vs Computer');
  });

  it('should highlight active mode based on selectedMode input', () => {
    const twoPlayerBtn = fixture.nativeElement.querySelector('#mode-twoplayer-btn') as HTMLButtonElement;
    const computerBtn = fixture.nativeElement.querySelector('#mode-computer-btn') as HTMLButtonElement;

    expect(twoPlayerBtn.classList.contains('active')).toBe(true);
    expect(computerBtn.classList.contains('active')).toBe(false);

    fixture.componentRef.setInput('selectedMode', 'Computer');
    fixture.detectChanges();

    expect(twoPlayerBtn.classList.contains('active')).toBe(false);
    expect(computerBtn.classList.contains('active')).toBe(true);
  });

  it('should emit modeChange when a different mode is clicked', () => {
    let emittedMode: GameMode | null = null;
    component.modeChange.subscribe((mode: GameMode) => {
      emittedMode = mode;
    });

    const computerBtn = fixture.nativeElement.querySelector('#mode-computer-btn') as HTMLButtonElement;
    computerBtn.click();

    expect(emittedMode).toBe('Computer');
  });

  it('should not emit modeChange when currently active mode is clicked', () => {
    let emitted = false;
    component.modeChange.subscribe(() => {
      emitted = true;
    });

    const twoPlayerBtn = fixture.nativeElement.querySelector('#mode-twoplayer-btn') as HTMLButtonElement;
    twoPlayerBtn.click();

    expect(emitted).toBe(false);
  });

  it('should disable buttons when disabled input is true', () => {
    fixture.componentRef.setInput('disabled', true);
    fixture.detectChanges();

    const twoPlayerBtn = fixture.nativeElement.querySelector('#mode-twoplayer-btn') as HTMLButtonElement;
    const computerBtn = fixture.nativeElement.querySelector('#mode-computer-btn') as HTMLButtonElement;

    expect(twoPlayerBtn.disabled).toBe(true);
    expect(computerBtn.disabled).toBe(true);
  });

  it('should not emit modeChange when clicked while disabled', () => {
    fixture.componentRef.setInput('disabled', true);
    fixture.detectChanges();

    let emitted = false;
    component.modeChange.subscribe(() => {
      emitted = true;
    });

    const computerBtn = fixture.nativeElement.querySelector('#mode-computer-btn') as HTMLButtonElement;
    computerBtn.click();

    expect(emitted).toBe(false);
  });
});
