import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GameControlsComponent } from './game-controls.component';
import { describe, it, expect, beforeEach } from 'vitest';

describe('GameControlsComponent', () => {
  let fixture: ComponentFixture<GameControlsComponent>;
  let component: GameControlsComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GameControlsComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(GameControlsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should render the Reset Game button with id reset-game-btn', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const resetBtn = compiled.querySelector('#reset-game-btn') as HTMLButtonElement;
    expect(resetBtn).toBeTruthy();
    expect(resetBtn.textContent).toContain('Reset Game');
  });

  it('should emit reset event when Reset Game button is clicked', () => {
    let emitted = false;
    component.reset.subscribe(() => {
      emitted = true;
    });

    const resetBtn = fixture.nativeElement.querySelector('#reset-game-btn') as HTMLButtonElement;
    resetBtn.click();

    expect(emitted).toBe(true);
  });

  it('should disable Reset Game button when disabled input is true', () => {
    fixture.componentRef.setInput('disabled', true);
    fixture.detectChanges();

    const resetBtn = fixture.nativeElement.querySelector('#reset-game-btn') as HTMLButtonElement;
    expect(resetBtn.disabled).toBe(true);
  });

  it('should not emit reset when clicked while disabled', () => {
    fixture.componentRef.setInput('disabled', true);
    fixture.detectChanges();

    let emitted = false;
    component.reset.subscribe(() => {
      emitted = true;
    });

    const resetBtn = fixture.nativeElement.querySelector('#reset-game-btn') as HTMLButtonElement;
    resetBtn.click();

    expect(emitted).toBe(false);
  });
});
