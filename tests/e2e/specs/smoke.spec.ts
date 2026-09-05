import { test, expect } from '@playwright/test';

test.describe('Playwright Configuration Smoke Test', () => {
  test('should launch browser and evaluate page content', async ({ page }) => {
    await page.setContent('<html><head><title>Tic Tac Toe</title></head><body><div id="root">Ready</div></body></html>');
    await expect(page).toHaveTitle(/Tic Tac Toe/i);
    const content = await page.textContent('#root');
    expect(content).toBe('Ready');
  });
});
