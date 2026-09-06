import { test, expect } from '@playwright/test';
import * as http from 'node:http';
import * as fs from 'node:fs';
import * as path from 'node:path';

const DIST_DIR = path.resolve(__dirname, '../../src/frontend/dist/frontend/browser');

let server: http.Server | null = null;
const PORT = 4200;

test.beforeAll(async () => {
  // Check if port 4200 is already running
  const isPortAvailable = await new Promise<boolean>((resolve) => {
    const tester = http.createServer()
      .once('error', () => resolve(false))
      .once('listening', () => {
        tester.close(() => resolve(true));
      })
      .listen(PORT);
  });

  if (isPortAvailable) {
    server = http.createServer((req, res) => {
      const parsedUrl = new URL(req.url || '/', `http://localhost:${PORT}`);
      let pathname = parsedUrl.pathname;
      if (pathname === '/') pathname = '/index.html';

      const filePath = path.join(DIST_DIR, pathname);
      if (fs.existsSync(filePath) && fs.statSync(filePath).isFile()) {
        const ext = path.extname(filePath).toLowerCase();
        const contentTypes: Record<string, string> = {
          '.html': 'text/html',
          '.js': 'application/javascript',
          '.css': 'text/css',
          '.ico': 'image/x-icon',
          '.json': 'application/json'
        };
        res.writeHead(200, { 'Content-Type': contentTypes[ext] || 'application/octet-stream' });
        fs.createReadStream(filePath).pipe(res);
      } else {
        // SPA Fallback
        const indexPath = path.join(DIST_DIR, 'index.html');
        res.writeHead(200, { 'Content-Type': 'text/html' });
        fs.createReadStream(indexPath).pipe(res);
      }
    });

    await new Promise<void>((resolve) => {
      server!.listen(PORT, () => resolve());
    });
  }

  // Warm up real backend endpoints to avoid JIT cold-start on the first latency check
  try {
    const warmupGame = await fetch('http://localhost:5000/api/games', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ mode: 'TwoPlayer' })
    });
    if (warmupGame.ok) {
      const game = (await warmupGame.json()) as { id: string };
      await fetch(`http://localhost:5000/api/games/${game.id}/moves`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ player: 'X', row: 1, column: 1 })
      });
    }
    await fetch('http://localhost:5000/api/scoreboard/reset', { method: 'POST' });
  } catch {
    // webServer will ensure backend is up
  }
});

test.afterAll(async () => {
  if (server) {
    await new Promise<void>((resolve) => server!.close(() => resolve()));
  }
});

test.describe('Tic-Tac-Toe Full-Game E2E Flow (Real .NET Backend)', () => {
  test.beforeEach(async ({ page, request }) => {
    // Reset scoreboard on real backend so each test starts with a clean baseline
    await request.post('http://localhost:5000/api/scoreboard/reset');

    await page.goto('http://localhost:4200');

    // Wait for the game page to load and board to be interactive
    await expect(page.locator('.status-text')).toContainText("Player X's Turn");
    await expect(page.locator('[data-row="1"][data-col="1"] button')).toBeEnabled();
  });

  test('Flow 1: Two-Player game win detection and latency performance under 200ms', async ({ page }) => {
    // Verify initial layout
    await expect(page.locator('.status-text')).toContainText("Player X's Turn");

    // Ensure board is fully initialized and button is enabled
    const cellBtn = page.locator('[data-row="1"][data-col="1"] button');
    await expect(cellBtn).toBeEnabled();

    // SC-001 assertion: Measure click-to-render latency inside browser (< 200ms)
    const latencyMs = await page.evaluate(async () => {
      const btn = document.querySelector('[data-row="1"][data-col="1"] button') as HTMLButtonElement;
      return new Promise<number>((resolve) => {
        const start = performance.now();
        const observer = new MutationObserver(() => {
          const mark = btn.querySelector('.mark');
          if (mark && mark.textContent?.trim() === 'X') {
            observer.disconnect();
            resolve(performance.now() - start);
          }
        });
        observer.observe(btn, { childList: true, subtree: true, characterData: true });
        btn.click();
      });
    });

    expect(latencyMs).toBeLessThan(200);

    // Verify mark rendered and turn alternates to O
    await expect(page.locator('[data-row="1"][data-col="1"] .mark')).toHaveText('X');
    await expect(page.locator('.status-text')).toContainText("Player O's Turn");

    // Move 2: O plays (2,1)
    await page.locator('[data-row="2"][data-col="1"] button').click();
    await expect(page.locator('[data-row="2"][data-col="1"] .mark')).toHaveText('O');

    // Move 3: X plays (1,2)
    await page.locator('[data-row="1"][data-col="2"] button').click();
    await expect(page.locator('[data-row="1"][data-col="2"] .mark')).toHaveText('X');

    // Move 4: O plays (2,2)
    await page.locator('[data-row="2"][data-col="2"] button').click();
    await expect(page.locator('[data-row="2"][data-col="2"] .mark')).toHaveText('O');

    // Move 5: X plays (1,3) -> Row 1 Win!
    await page.locator('[data-row="1"][data-col="3"] button').click();

    // Verify win announcement and winning cell highlights
    await expect(page.locator('.status-badge.won')).toBeVisible();
    await expect(page.locator('.status-text')).toContainText('Player X Wins!');
    await expect(page.locator('[data-row="1"][data-col="1"]')).toHaveClass(/winning/);
    await expect(page.locator('[data-row="1"][data-col="2"]')).toHaveClass(/winning/);
    await expect(page.locator('[data-row="1"][data-col="3"]')).toHaveClass(/winning/);

    // Verify board is frozen (all cell buttons disabled)
    const remainingBtn = page.locator('[data-row="3"][data-col="3"] button');
    await expect(remainingBtn).toBeDisabled();

    // Verify Scoreboard updated for Player X
    const xScore = page.locator('.score-card.x .score-val');
    await expect(xScore).toHaveText('1');
  });

  test('Flow 2: Game Reset clears board, resets turn to X, and preserves scoreboard', async ({ page }) => {
    // Win game first
    await page.locator('[data-row="1"][data-col="1"] button').click();
    await page.locator('[data-row="2"][data-col="1"] button').click();
    await page.locator('[data-row="1"][data-col="2"] button').click();
    await page.locator('[data-row="2"][data-col="2"] button').click();
    await page.locator('[data-row="1"][data-col="3"] button').click();

    await expect(page.locator('.status-text')).toContainText('Player X Wins!');
    await expect(page.locator('.score-card.x .score-val')).toHaveText('1');

    // Click Reset Game
    await page.locator('#reset-game-btn').click();

    // Board is cleared
    await expect(page.locator('.mark')).toHaveCount(0);
    // Turn reset to Player X
    await expect(page.locator('.status-text')).toContainText("Player X's Turn");
    // Scoreboard is preserved!
    await expect(page.locator('.score-card.x .score-val')).toHaveText('1');
  });

  test('Flow 3: Undo move reverts move, updates board, turn, and move history', async ({ page }) => {
    // Move 1: X (1,1)
    await page.locator('[data-row="1"][data-col="1"] button').click();
    await expect(page.locator('.move-item')).toHaveCount(1);

    // Move 2: O (2,2)
    await page.locator('[data-row="2"][data-col="2"] button').click();
    await expect(page.locator('.move-item')).toHaveCount(2);

    // Undo Move 2
    await page.locator('#undo-btn').click();
    await expect(page.locator('[data-row="2"][data-col="2"] .mark')).toHaveCount(0);
    await expect(page.locator('.status-text')).toContainText("Player O's Turn");
    await expect(page.locator('.move-item')).toHaveCount(1);

    // Undo Move 1
    await page.locator('#undo-btn').click();
    await expect(page.locator('[data-row="1"][data-col="1"] .mark')).toHaveCount(0);
    await expect(page.locator('.status-text')).toContainText("Player X's Turn");
    await expect(page.locator('.move-item')).toHaveCount(0);

    // Undo button now disabled
    await expect(page.locator('#undo-btn')).toBeDisabled();
  });

  test('Flow 4: Scoreboard reset resets counts to zero independently', async ({ page }) => {
    // Win game to increment scoreboard
    await page.locator('[data-row="1"][data-col="1"] button').click();
    await page.locator('[data-row="2"][data-col="1"] button').click();
    await page.locator('[data-row="1"][data-col="2"] button').click();
    await page.locator('[data-row="2"][data-col="2"] button').click();
    await page.locator('[data-row="1"][data-col="3"] button').click();

    await expect(page.locator('.score-card.x .score-val')).toHaveText('1');

    // Click Reset Scoreboard
    await page.locator('.reset-scoreboard-btn').click();

    // Verify all counts are zero
    await expect(page.locator('.score-card.x .score-val')).toHaveText('0');
    await expect(page.locator('.score-card.o .score-val')).toHaveText('0');
    await expect(page.locator('.score-card.draws .score-val')).toHaveText('0');
  });

  test('Flow 5: Computer opponent mode automatically responds and move-pair undo works', async ({ page }) => {
    // Switch to vs Computer mode
    await page.locator('#mode-computer-btn').click();
    await expect(page.locator('#mode-computer-btn')).toHaveClass(/active/);

    // Human X plays (1,1)
    await page.locator('[data-row="1"][data-col="1"] button').click();

    // Verify human mark placed
    await expect(page.locator('[data-row="1"][data-col="1"] .mark')).toHaveText('X');

    // Wait for computer move to appear (center 2,2)
    await expect(page.locator('[data-row="2"][data-col="2"] .mark')).toHaveText('O');
    await expect(page.locator('.status-text')).toContainText("Player X's Turn");

    // Click Undo in Computer mode -> moves pair (both O and X) are rolled back
    await page.locator('#undo-btn').click();

    await expect(page.locator('[data-row="1"][data-col="1"] .mark')).toHaveCount(0);
    await expect(page.locator('[data-row="2"][data-col="2"] .mark')).toHaveCount(0);
    await expect(page.locator('.status-text')).toContainText("Player X's Turn");
  });

  test('Flow 5b: Real backend ComputerStrategy executes intelligent Block priority', async ({ page }) => {
    // Switch to vs Computer mode
    await page.locator('#mode-computer-btn').click();
    await expect(page.locator('#mode-computer-btn')).toHaveClass(/active/);

    // Move 1: Human X plays (1,1)
    await page.locator('[data-row="1"][data-col="1"] button').click();
    await expect(page.locator('[data-row="1"][data-col="1"] .mark')).toHaveText('X');

    // Computer takes Center (2,2) [Priority 3: Center]
    await expect(page.locator('[data-row="2"][data-col="2"] .mark')).toHaveText('O');

    // Move 2: Human X plays (1,2) -> threatens row 1 win at (1,3)!
    await page.locator('[data-row="1"][data-col="2"] button').click();
    await expect(page.locator('[data-row="1"][data-col="2"] .mark')).toHaveText('X');

    // Computer must execute Priority 2 (Block) and play (1,3) to block X!
    await expect(page.locator('[data-row="1"][data-col="3"] .mark')).toHaveText('O');
    await expect(page.locator('.status-text')).toContainText("Player X's Turn");
  });

  test('Flow 6: Draw detection triggers draw announcement, locks board, and increments scoreboard draws', async ({ page }) => {
    // 9-move sequence leading to a draw without 3-in-a-row:
    // Move 1: X plays (1,1)
    await page.locator('[data-row="1"][data-col="1"] button').click();
    // Move 2: O plays (1,2)
    await page.locator('[data-row="1"][data-col="2"] button').click();
    // Move 3: X plays (1,3)
    await page.locator('[data-row="1"][data-col="3"] button').click();
    // Move 4: O plays (2,2)
    await page.locator('[data-row="2"][data-col="2"] button').click();
    // Move 5: X plays (2,1)
    await page.locator('[data-row="2"][data-col="1"] button').click();
    // Move 6: O plays (3,1)
    await page.locator('[data-row="3"][data-col="1"] button').click();
    // Move 7: X plays (2,3)
    await page.locator('[data-row="2"][data-col="3"] button').click();
    // Move 8: O plays (3,3)
    await page.locator('[data-row="3"][data-col="3"] button').click();
    // Move 9: X plays (3,2)
    await page.locator('[data-row="3"][data-col="2"] button').click();

    // Verify Draw announcement
    await expect(page.locator('.status-badge.draw')).toBeVisible();
    await expect(page.locator('.status-text')).toContainText("It's a Draw!");

    // Verify all 9 cells are occupied
    await expect(page.locator('.mark')).toHaveCount(9);

    // Verify board is locked (all cell buttons are disabled)
    for (let r = 1; r <= 3; r++) {
      for (let c = 1; c <= 3; c++) {
        await expect(page.locator(`[data-row="${r}"][data-col="${c}"] button`)).toBeDisabled();
      }
    }

    // Verify Scoreboard draws count incremented to 1
    const drawsScore = page.locator('.score-card.draws .score-val');
    await expect(drawsScore).toHaveText('1');
  });

  test('Flow 7: Invalid move prevention and error banner resilience', async ({ page }) => {
    // Move 1: X plays (1,1)
    const cell11 = page.locator('[data-row="1"][data-col="1"] button');
    await cell11.click();
    await expect(page.locator('[data-row="1"][data-col="1"] .mark')).toHaveText('X');

    // Verify occupied cell button is immediately disabled to prevent invalid moves
    await expect(cell11).toBeDisabled();

    // Route override to simulate backend rejecting invalid move with RFC 7807 Problem Details
    await page.route('**/api/games/*/moves', async (route) => {
      await route.fulfill({
        status: 400,
        contentType: 'application/problem+json',
        json: {
          title: 'Bad Request',
          status: 400,
          detail: 'Cell (1, 2) is already occupied.'
        }
      });
    });

    // Attempt to make move on (1,2) which fails on backend
    await page.locator('[data-row="1"][data-col="2"] button').click();

    // Verify error banner appears with Problem Details error text
    const errorBanner = page.locator('.error-banner');
    await expect(errorBanner).toBeVisible();
    await expect(page.locator('.error-text')).toContainText('already occupied');

    // Dismiss error banner
    await page.locator('.close-button').click();
    await expect(errorBanner).toHaveCount(0);
  });
});
