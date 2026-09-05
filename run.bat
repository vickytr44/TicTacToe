@echo off
setlocal

title Tic-Tac-Toe Launcher

echo ========================================================
echo         Starting Tic-Tac-Toe Full-Stack App
echo ========================================================
echo.

set ROOT_DIR=%~dp0

echo [1/2] Launching Backend (.NET 10 Web API) on http://localhost:5000 ...
start "TicTacToe Backend (.NET 10)" cmd /k "cd /d "%ROOT_DIR%src\backend\Api" && dotnet run --launch-profile http"

echo [2/2] Launching Frontend (Angular Standalone) on http://localhost:4200 ...
start "TicTacToe Frontend (Angular)" cmd /k "cd /d "%ROOT_DIR%src\frontend" && npm start"

echo.
echo ========================================================
echo  Backend:  http://localhost:5000 (Health: /api/health)
echo  Frontend: http://localhost:4200 (Web Application)
echo ========================================================
echo.
echo Both applications have been launched in separate console windows.
echo Close those windows to stop the servers.
pause
