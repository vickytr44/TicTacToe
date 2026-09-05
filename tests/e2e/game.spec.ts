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
});

test.afterAll(async () => {
  if (server) {
    await new Promise<void>((resolve) => server!.close(() => resolve()));
  }
});

test.describe('Tic-Tac-Toe Full-Game E2E Flow', () => {
  let gameState: any = null;
  let scoreboard = { xWins: 0, oWins: 0, draws: 0 };

  test.beforeEach(async ({ page }) => {
    scoreboard = { xWins: 0, oWins: 0, draws: 0 };
    gameState = null;

    // Intercept backend API requests
    await page.route('**/api/**', async (route, request) => {
      const url = request.url();
      const method = request.method();

      if (url.includes('/api/scoreboard/reset') && method === 'POST') {
        scoreboard = { xWins: 0, oWins: 0, draws: 0 };
        return route.fulfill({ status: 200, json: scoreboard });
      }

      if (url.includes('/api/scoreboard') && method === 'GET') {
        return route.fulfill({ status: 200, json: scoreboard });
      }

      if (url.endsWith('/api/games') && method === 'POST') {
        const body = request.postDataJSON() || {};
        gameState = {
          id: 'test-game-id',
          board: [
            [null, null, null],
            [null, null, null],
            [null, null, null]
          ],
          currentPlayer: 'X',
          gameMode: body.mode || 'TwoPlayer',
          status: 'InProgress',
          winner: null,
          winningCells: [],
          moves: [],
          createdAt: new Date().toISOString()
        };
        return route.fulfill({ status: 201, json: gameState });
      }

      if (url.includes('/moves') && method === 'POST') {
        const moveReq = request.postDataJSON();
        const r = moveReq.row - 1;
        const c = moveReq.column - 1;

        gameState.board[r][c] = moveReq.player;
        gameState.moves.push({
          moveNumber: gameState.moves.length + 1,
          player: moveReq.player,
          row: moveReq.row,
          column: moveReq.column
        });

        // Check human win
        const lines = [
          [[0,0],[0,1],[0,2]],
          [[1,0],[1,1],[1,2]],
          [[2,0],[2,1],[2,2]],
          [[0,0],[1,0],[2,0]],
          [[0,1],[1,1],[2,1]],
          [[0,2],[1,2],[2,2]],
          [[0,0],[1,1],[2,2]],
          [[0,2],[1,1],[2,0]]
        ];

        let won = false;
        for (const line of lines) {
          if (line.every(([lr, lc]) => gameState.board[lr][lc] === moveReq.player)) {
            gameState.status = 'Won';
            gameState.winner = moveReq.player;
            gameState.winningCells = line.map(([lr, lc]) => ({ row: lr + 1, column: lc + 1 }));
            won = true;
            if (moveReq.player === 'X') scoreboard.xWins++;
            else scoreboard.oWins++;
            break;
          }
        }

        if (!won && gameState.moves.length === 9) {
          gameState.status = 'Draw';
          scoreboard.draws++;
        } else if (!won) {
          gameState.currentPlayer = moveReq.player === 'X' ? 'O' : 'X';

          // If computer mode, apply computer move
          if (gameState.gameMode === 'Computer' && gameState.currentPlayer === 'O') {
            let compR = 1;
            let compC = 1;
            if (gameState.board[1][1] !== null) {
              // Find first empty cell
              for (let i = 0; i < 3; i++) {
                for (let j = 0; j < 3; j++) {
                  if (gameState.board[i][j] === null) {
                    compR = i;
                    compC = j;
                    break;
                  }
                }
              }
            }

            gameState.board[compR][compC] = 'O';
            gameState.moves.push({
              moveNumber: gameState.moves.length + 1,
              player: 'O',
              row: compR + 1,
              column: compC + 1
            });
            gameState.currentPlayer = 'X';
          }
        }

        return route.fulfill({ status: 200, json: gameState });
      }

      if (url.includes('/reset') && method === 'POST') {
        gameState.board = [
          [null, null, null],
          [null, null, null],
          [null, null, null]
        ];
        gameState.currentPlayer = 'X';
        gameState.status = 'InProgress';
        gameState.winner = null;
        gameState.winningCells = [];
        gameState.moves = [];
        return route.fulfill({ status: 200, json: gameState });
      }

      if (url.includes('/undo') && method === 'POST') {
        if (gameState.gameMode === 'Computer') {
          // Pop O then X
          if (gameState.moves.length >= 2) {
            const m2 = gameState.moves.pop();
            gameState.board[m2.row - 1][m2.column - 1] = null;
            const m1 = gameState.moves.pop();
            gameState.board[m1.row - 1][m1.column - 1] = null;
          }
          gameState.currentPlayer = 'X';
        } else {
          if (gameState.moves.length > 0) {
            const m = gameState.moves.pop();
            gameState.board[m.row - 1][m.column - 1] = null;
            gameState.currentPlayer = m.player;
          }
        }
        return route.fulfill({ status: 200, json: gameState });
      }

      return route.continue();
    });

    await page.goto('http://localhost:4200');
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
});
