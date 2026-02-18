# PROJECT_CONTEXT

## Overview
- This repository contains a C# WinForms horse-racing betting game.
- Primary specification source in the repo is `Spec.xlsx` (not `SPEC.md`).
- Current implementation includes title -> horse selection -> bet -> ticket -> race -> result -> clear/bankrupt flow.

## Repo structure
- `KeibaGame.sln`: solution file.
- `KeibaGame.App/`: WinForms application project.
- `KeibaGame.App/Forms/`: UI screens (`TitleForm`, `HorseSelectForm`, `BetForm`, `TicketForm`, `RaceForm`, `ResultForm`, `SettingsForm`, `ClearForm`, `BankruptForm`).
- `KeibaGame.App/Game/`: game logic (`GameFlow`, `GameSession`, `RaceFactory`, `RaceSimulator`, `GameSaveService`, `GameRules`, repositories/factories).
- `KeibaGame.App/Models/`: domain models (`Horse`, `TrapEvent`, `RaceResult`, `TrapType`).
- `KeibaGame.App/Ui/`: UI helper components and theme.
- `KeibaGame.App/Data/horse_names.csv`: horse name source data.
- `Spec.xlsx`: external specification document.

## Current status
- Implemented:
- Full screen flow per spec sections (title, selection, bet, ticket, race, result, clear, bankrupt).
- Race simulation with trap effects, odds calculation, ranking, auto progression.
- Persistent save/load in `%LocalAppData%/KeibaGame/save.json`.
- Settings screen for race speed (`1x/2x/4x`) from title screen.
- Horse names loaded from CSV (`Data/horse_names.csv`), not hardcoded array.
- UI redesign to turf-themed style and reduced flicker using double-buffered panel.
- XML comments added for methods and public members across current code.
- Not implemented / TBD:
- Automated tests are not present (`test` project is TBD).
- Installer/package pipeline is TBD.
- CI/CD workflow is TBD.
- Real horse photo assets are not integrated (currently generated sprite images).

## Decisions
- Source of truth:
- No `SPEC.md`, `TASKS.md`, or `CODING_GUIDE.md` found in repo at this time.
- Functional requirements are interpreted from `Spec.xlsx`.
- Architecture:
- Practical WinForms separation: Forms for view/UI, `Game/*` and `Models/*` for logic/data.
- Not strict MVC framework, but controller-like flow is centralized in `GameFlow`.
- Data and rules:
- Core constants centralized in `GameRules`.
- Horse names managed via CSV repository (`HorseNameRepository`).
- Save data managed by `GameSaveService`.
- Branch strategy:
- GitFlow intent is adopted: `main` -> `develop` -> `feature/*`.
- Current working branch: `feature/keiba-game-implementation`.

## How to run
- Prerequisite: .NET SDK (project targets `net8.0-windows`).
- Restore/build:
```powershell
dotnet restore KeibaGame.sln
dotnet build KeibaGame.sln
```
- Run:
```powershell
dotnet run --project KeibaGame.App/KeibaGame.App.csproj
```
- Test:
- TBD (no test project/commands currently defined).
- Preview:
- Same as local run for WinForms app (no web preview server).

## How to deploy
- This is a desktop WinForms app, not a web frontend.
- Current practical distribution is shipping build artifacts from `dotnet publish`.
- Example:
```powershell
dotnet publish KeibaGame.App/KeibaGame.App.csproj -c Release -r win-x64 --self-contained false
```
- Installer/signing/store distribution is TBD.

## Next steps
1. Validate every `Spec.xlsx` requirement against implemented behavior and list gaps explicitly.
2. Add automated tests for core logic (`RaceSimulator`, `RaceFactory`, odds, trap timing).
3. Add input/flow edge-case tests (bankrupt boundary, clear boundary, cancel/back transitions).
4. Add error-handling UX for CSV/save load failures with user-facing messages.
5. Decide and implement packaging strategy (`dotnet publish` profile, optional installer).
6. Add CI pipeline for build + tests on PR.
7. Add pull-request from `feature/keiba-game-implementation` into `develop` after review.
8. Consider replacing generated horse sprites with actual managed assets if required by stakeholders.
9. Add logging strategy for runtime errors (TBD).
10. Create `SPEC.md`/`TASKS.md` if team wants markdown-first workflow going forward (TBD).
