import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ErrorBannerComponent } from './error-banner.component';
import { By } from '@angular/platform-browser';

describe('ErrorBannerComponent', () => {
  let component: ErrorBannerComponent;
  let fixture: ComponentFixture<ErrorBannerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ErrorBannerComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(ErrorBannerComponent);
    component = fixture.componentInstance;
  });

  it('should render error message when error is provided', () => {
    fixture.componentRef.setInput('error', 'Something went wrong. Please try again.');
    fixture.detectChanges();

    const banner = fixture.debugElement.query(By.css('.error-banner'));
    expect(banner).toBeTruthy();
    expect(banner.nativeElement.textContent).toContain('Something went wrong. Please try again.');
  });

  it('should not render anything when error is null', () => {
    fixture.componentRef.setInput('error', null);
    fixture.detectChanges();

    const banner = fixture.debugElement.query(By.css('.error-banner'));
    expect(banner).toBeFalsy();
  });

  it('should emit dismissed event when dismiss button is clicked', () => {
    fixture.componentRef.setInput('error', 'Error text');
    fixture.detectChanges();

    let dismissed = false;
    component.dismissed.subscribe(() => {
      dismissed = true;
    });

    const closeButton = fixture.debugElement.query(By.css('.close-button'));
    closeButton.nativeElement.click();

    expect(dismissed).toBe(true);
  });
});
