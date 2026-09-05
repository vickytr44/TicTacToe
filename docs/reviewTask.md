# `tasks.md` Review Against `spec.md`

## Verdict

**⚠️ Not ready for `/implement` yet.**

I performed a direct traceability review of the latest `tasks.md` against the latest `spec.md`, including FR-001–FR-025, acceptance scenarios, edge cases, success criteria, assumptions, and the API contract.

The task list is structurally good, but there are several concrete gaps and a few task/spec conflicts.

---

## 🔴 Must Fix

### RT-001 — `GET /api/games/{id}` is missing

`spec.md` requires the game-state retrieval endpoint.

`tasks.md` has:
- POST create game
- POST move
- POST reset
- POST undo

but **no task for GET `/api/games/{id}`**.

**Fix:** Add backend implementation, frontend usage where needed, and integration tests.

---

### RT-002 — FR-024 error handling is missing from the core task plan

`spec.md` requires:
- dismissible inline error banner
- network/server/unexpected-response handling
- preserve last valid board state
- no optimistic mutation

`tasks.md` only adds an `ErrorBannerComponent` in the final polish phase.

This should be treated as a real functional requirement, not merely polish.

**Fix:** Add explicit tests and implementation tasks for FR-024.

---

### RT-003 — FR-015 "thinking" indicator is not implemented/tested completely

`spec.md` requires during the 300–500ms delay:
- UI indicates computer is thinking
- board interaction disabled
- computer then moves
- turn returns to X

`tasks.md` explicitly covers UI locking, but does not explicitly require the **thinking indicator**.

**Fix:** Add implementation + component test for the thinking indicator.

---

### RT-004 — Computer strategy tests do not explicitly cover all five priorities

The spec defines:

1. Win
2. Block
3. Center
4. Corner
5. Any available cell

`T050` only says "Unit tests for Computer strategy."

**Fix:** Explicitly require all five scenarios in T050.

---

### RT-005 — Computer-mode Undo lacks integration coverage

Two-player Undo has an API integration test, but Computer-mode Undo only has a unit test.

The spec requires the complete behavior:
`X → O → Undo → both removed → X's turn`.

**Fix:** Add an integration test for POST `/api/games/{id}/undo` in Computer mode.

---

### RT-006 — Scoreboard exactly-once and reset behavior is under-tested

The spec requires:
- X wins increment
- O wins increment
- draw increment
- exactly once per completed game
- Reset Game preserves scoreboard
- Reset Scoreboard clears all counts
- incomplete games do not affect scoreboard

`T044/T045` are too broad to guarantee all of this.

**Fix:** Explicitly enumerate these scenarios in the test task(s).

---

### RT-007 — Mode switching is under-tested

The spec requires switching mode mid-game to:
- discard current game
- clear board
- clear history
- preserve scoreboard
- start a new session
- use selected mode

`T032` only says "component test for mode selector."

**Fix:** Add an integration/component test covering the complete behavior.

---

### RT-008 — Move history is treated as "verify serialization"

`T037` says:

> "Ensure backend MakeMove populates MoveDto objects ... just verify serialization."

The spec requires actual behavior: move number, player, row/column, chronological history, updated after every valid move.

**Fix:** Make T037 a concrete implementation/test task rather than a verification-only task.

---

## 🟠 Important

### RT-009 — Draw implementation lacks API/frontend integration coverage

`T024` tests draw logic, but there is no explicit integration test proving the API returns `Draw`, scoreboard updates, and frontend renders the draw state.

**Fix:** Add integration/component coverage.

---

### RT-010 — Win tests do not explicitly verify winning cells

The spec requires winning cells to be returned and highlighted.

`T015` says "win detection logic", but the task does not explicitly require row/column/diagonal winning-cell output.

**Fix:** Explicitly test winning-cell coordinates for all three win types.

---

### RT-011 — Reset Game does not explicitly test scoreboard preservation

The spec explicitly requires Reset Game to preserve the scoreboard.

`T027` should verify this.

**Fix:** Add scoreboard-before/after assertion.

---

### RT-012 — Move validation coverage is too vague

FR-019 requires rejection of:
- out-of-bounds
- occupied cell
- wrong player
- completed game

`T016` only says integration tests for game creation/moves.

**Fix:** Explicitly list these four validation scenarios.

---

### RT-013 — `POST /api/games` must support GameMode

`T033` handles this, but the initial game-creation integration test should also verify the selected mode is persisted/returned correctly.

**Fix:** Add mode assertions to API tests.

---

### RT-014 — API response contract is not explicitly tested

FR-021 requires:
- game ID
- board
- current player
- game mode
- status
- winner
- winning cells
- move history
- scoreboard or dedicated scoreboard mechanism

**Fix:** Add a response-contract integration test, including the scoreboard mechanism.

---

### RT-015 — Undo recalculation needs explicit tests

FR-008 requires recalculating win/draw status after Undo.

The tasks test undo restoration, but do not explicitly state tests for:
- undo from a pre-terminal state
- restoration of `InProgress`
- accurate history
- correct turn

**Fix:** Make these assertions explicit.

---

## 🟡 Task/Spec Consistency

### RT-016 — SQLite is a valid choice, but the spec allows in-memory storage

`tasks.md` makes SQLite + EF Core mandatory.

The spec says:

> In-memory is acceptable; SQLite may be used if preferred.

This is **not a violation**, but it is an implementation decision.

**Recommendation:** Keep SQLite if intentional, and document it as a design decision.

---

### RT-017 — Singleton `ScoreboardService` needs explicit rationale

`T046` specifies a singleton scoreboard service.

That is reasonable for a local single-session application, but it should be consistent with the chosen SQLite/in-memory persistence approach.

**Recommendation:** Document the lifetime/persistence decision during `/plan`.

---

### RT-018 — 300–500ms delay is NOT an invented requirement in the latest spec

Earlier this was a concern, but the latest `spec.md` explicitly defines the 300–500ms delay in FR-015.

Therefore:

**T054 is valid and should remain.** ✅

This corrects my earlier review.

---

## 🟢 Good

- Test-first structure is good.
- Domain logic is separated from API/UI.
- Computer strategy is isolated.
- Two-player and computer Undo are separated.
- Dependencies are understandable.
- MVP → enhancements progression is sensible.
- REST endpoint coverage is mostly strong.
- The task list is concrete enough once the above gaps are addressed.

---

## Final Checklist Before `/implement`

- [ ] Add `GET /api/games/{id}`
- [ ] Add complete FR-024 error handling tests/tasks
- [ ] Add computer "thinking" indicator
- [ ] Explicitly test all 5 computer priorities
- [ ] Add Computer Undo integration test
- [ ] Expand scoreboard tests
- [ ] Expand mode-switch test
- [ ] Make move-history implementation/test concrete
- [ ] Add draw integration/frontend coverage
- [ ] Add winning-cell assertions
- [ ] Verify Reset Game preserves scoreboard
- [ ] Explicitly test all invalid-move cases
- [ ] Add API response-contract test
- [ ] Explicitly test Undo status/history/turn restoration

## Recommendation

**Update `tasks.md` first. Then do one final `spec.md → tasks.md` traceability check before `/implement`.**

The current task plan is close, but I would not let an implementation agent start yet.
