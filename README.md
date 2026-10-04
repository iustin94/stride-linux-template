# Stride Linux Template

Code-only [Stride](https://www.stride3d.net/) game template for Linux: no Game Studio editor, everything in C#.
Ships a small Space Invaders sample that exercises every piece of the scaffold (menu, scene switching with fade,
input map, HUD, pause and game-over screens).

## Requirements

| Requirement | Version |
|---|---|
| OS | Linux x64 |
| .NET SDK | 10.0 |
| GPU driver | OpenGL |

No system packages are needed: FreeImage and FreeType come from NuGet (see [Native libraries](#native-libraries)).

## Quick start

```bash
git clone <this repo> my-game
cd my-game
dotnet build
dotnet run --project StrideTemplate.csproj
dotnet test
```

If .NET is installed per-user (e.g. `~/.dotnet`), running the built binary directly needs `DOTNET_ROOT`:

```bash
DOTNET_ROOT=$HOME/.dotnet ./bin/Debug/net10.0/linux-x64/StrideTemplate
```

`dotnet run` and Rider do not need it.

## Controls

| Action | Keys |
|---|---|
| Move left | ← / A |
| Move right | → / D |
| Shoot | Space / W / ↑ |
| Pause / resume | Esc |

## Layout

```
StrideTemplate.csproj          game project (net10.0, linux-x64, OpenGL)
Program.cs                     entry point: game.Run(null, Start, Update)
Core/
  GameManager.cs               phase machine: MainMenu -> Gameplay <-> Paused -> GameOver
  SceneManager.cs              loads/unloads IGameScene, fade transition
  GameState.cs                 score, lives, game over, victory (engine-free, unit-tested)
Input/
  InputAction.cs               logical actions
  InputMap.cs                  action -> keys bindings, rebindable
Scenes/
  IGameScene.cs                Load / Update / Unload contract
  MainMenuScene.cs
  GameplayScene.cs             Space Invaders sample
UI/
  UIHelper.cs                  bundled font registration, UI element factories
  FadeOverlay.cs, HUD.cs, MenuScreen.cs
  Screens/                     MainMenu, Pause, GameOver
assets/m5x7.ttf                bundled pixel font
build/
  StrideAssetCompilerNativeLibraries.targets
StrideTemplate.Tests/          xUnit tests
docs/PLAN.md                   Stride-on-Linux code-only setup guide
```

## Starting a new game from the template

1. Clone, then rename `StrideTemplate` (csproj, sln, namespace, test project) to the game's name.
2. Replace `Scenes/GameplayScene.cs` with the game's own `IGameScene`.
3. Point `GameManager.StartGame` at the new scene.
4. Adjust `Input/InputAction.cs` and the default bindings in `Input/InputMap.cs`.

## Native libraries

Stride's asset compiler (`Stride.Core.Assets.CompilerApp`) runs at build time from the NuGet cache and needs
FreeImage and FreeType. Stride 4.3 ships neither for Linux.

| Library | Loaded by | Searched in |
|---|---|---|
| FreeImage | `NativeLibraryHelper.PreloadLibrary` | compiler folder, system `dlopen`, `<cwd>/runtimes/linux-x64/native`, `PATH` |
| FreeType | `DllImport("freetype")` | compiler folder, system `dlopen` (`LD_LIBRARY_PATH`, ld cache) |

`build/StrideAssetCompilerNativeLibraries.targets` runs before `StrideCompileAsset` and:

1. References `MonoGame.Library.FreeImage` and `MonoGame.Library.FreeType` (self-contained Linux builds).
2. Copies their `runtimes/linux-x64/native/*` into `obj/native/linux-x64/`.
3. Prepends that folder to `LD_LIBRARY_PATH` for the compiler process.
4. Sets `DOTNET_NUGET_SIGNATURE_VERIFICATION=false` for the compiler process: its internal NuGet resolver
   otherwise rejects expired Microsoft package signatures (`NotTimeValid`, `UntrustedRoot`).

Both variables are set in the MSBuild process for the rest of that build.
At runtime the game loads the same libraries from `bin/` through its own `deps.json`.

## Pinned versions

| Package | Version | Why pinned |
|---|---|---|
| `Stride.CommunityToolkit.Linux`, `Stride.CommunityToolkit.Bepu` | `1.0.0-preview.62` (Stride 4.3.0.2507) | `preview.65` pulls Stride 4.4.0-beta8, where `game.Run(null, Start, Update)` no longer compiles (`CS1501`) |
| `MonoGame.Library.FreeImage` | `3.18.0.3` | native FreeImage for the asset compiler |
| `MonoGame.Library.FreeType` | `2.13.2.5` | native FreeType for the asset compiler |

## Known issues

1. Pause does not stop gameplay: `GameManager.Update` ticks the scene in every phase, including `Paused`.
2. `InputAction.Quit` is bound to Q in `InputMap` but nothing handles it.

## Decision Literature

| Date | Decision | Source |
|---|---|---|
| 2026-10-04 | Asset compiler native libraries resolved through `LD_LIBRARY_PATH` + staged folder, not system packages or NuGet-cache symlinks | [Stride 4.3 `NativeLibraryHelper.cs`](https://github.com/stride3d/stride/blob/releases/4.3.0.2507/sources/core/Stride.Core/Native/NativeLibraryHelper.cs) |
| 2026-10-04 | FreeImage/FreeType binaries taken from MonoGame's native packages | [MonoGame.Library.FreeImage](https://www.nuget.org/packages/MonoGame.Library.FreeImage/3.18.0.3), [MonoGame.Library.FreeType](https://www.nuget.org/packages/MonoGame.Library.FreeType/2.13.2.5) |
| 2026-10-04 | Toolkit pinned to preview.62 / Stride 4.3 | [Stride releases](https://github.com/stride3d/stride/releases), [Stride Community Toolkit](https://github.com/stride3d/stride-community-toolkit) |
| 2026-10-04 | Code-only workflow on Linux (Game Studio is Windows-only) | [Stride download page](https://www.stride3d.net/download/), [stride discussion #1893](https://github.com/stride3d/stride/discussions/1893) |

## Licences

| Component | Licence |
|---|---|
| Stride | MIT |
| FreeImage (via MonoGame.Library.FreeImage) | FreeImage Public License (`license-fi.txt` in the package) |
| FreeType (via MonoGame.Library.FreeType) | see `LICENSE.txt` in the package |
| m5x7 font | Unverified: check the font author's terms before redistributing |
