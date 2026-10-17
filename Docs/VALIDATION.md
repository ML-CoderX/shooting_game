# NEON SIEGE validation record

Target: Windows x86-64, Unity 6000.6.5f1, URP 17.6.
Host GPU detected by Unity: NVIDIA GeForce RTX 3050 6GB Laptop GPU.

This file records actual checks, not assumed completion.

## Definition of Done audit

| Requirement | Evidence / status |
| --- | --- |
| Launches to main menu | Standalone capture and controller Deploy check pass. |
| Full run: 10+ waves and boss | Normal-speed victory: 12 waves, 2 bosses, 11 upgrades, 378 kills. |
| 10–15 minute run | 610.8 seconds of gameplay; 645.7 seconds including upgrade selection (10:46). |
| Holds 60 FPS on target hardware | Full-run sample on RTX 3050 Laptop: 143.8 FPS average at a 144 cap, 6.98 ms p95; 2 of 88,643 sampled frames exceeded 16.67 ms. |
| No console errors | Full-run runtime errors: 0; standalone logs scanned for exceptions, warnings and unsupported shaders. |
| Windows x86-64 build | Successful DirectX 11 / Mono standalone build; PE machine 0x8664 verified. The ZIP was extracted, all 117 runtime files matched the build, and the extracted executable passed all 30 checks. |
| README | Controls, extension points, build and validation commands included. |
| Playable and fun | Actual combat/progression verified. Subjective enjoyment and human difficulty remain a human acceptance check. |

The objective technical checks pass. A scene-aware virtual controller cannot establish whether a human finds the game fun. This audit does not mark that subjective criterion as verified.

Release archive: `Builds/NeonSiege-Windows.zip` (34,481,869 bytes). SHA256: `C942FAEB40F64D7EB18F5A1EBEE6ED03C5AB33E40C00FCF4FAB1555D7AB93190`. See `package-validation.txt` and reproduce with `Tools/verify-package.ps1`.

## M1 — foundation
- URP template imported successfully.
- Initial C# compile succeeded; deprecated `FindFirstObjectByType` replaced with `FindAnyObjectByType`.
- Play-mode check passed: grounded operator and camera initialized.

## M2–M4 — combat and game loop
- Play-mode smoke checks passed for five weapon definitions, firing/ammo, reload, navigation, enemy death and pool reuse.
- Pause/resume, three distinct upgrade choices, next wave, defeat/restart, menu, and JSON serialization passed.
- Accelerated progression passed: 12 waves, 2 bosses, 11 upgrades, 102 kills. See `full-run-validation.txt`. This test directly damages enemies and does not measure normal combat difficulty.

## M5–M6 — presentation
- Blender generated 15 custom FBX models with a shared palette atlas. Triangle counts are in `asset-stats.json`.
- Operator imports as Humanoid; enemies use generic animation controllers with idle/run blends.
- Original synthesized SFX/music route through Master, SFX and Music mixer groups.
- Damage numbers, tracers, sparks, shell particles, hit markers, hit stop and URP post-processing are implemented.
- Visual checks reached main menu, live gameplay, settings and defeat. Standalone captures are in `Screenshots/`. Computer Use inspected the running game window.
- Corrected the operator's arm rotation and humanoid root-height import. The regression check measures head height 1.66 m and foot height 0.13 m relative to the character root. Prop origins were corrected before the final light bake.

## M7 — baked arena and standalone
- GPU light bake completed successfully with saved lightmaps, shadowmask, 50 light probes, and navigation data.
- Windows x86-64 / DirectX 11 / Mono build succeeded. Project C# compilation has no errors or warnings.
- 30 standalone checks passed: five weapons and reloads, sniper piercing, cover blocking, virtual-gamepad movement/dash/menu navigation, persistent bindings, disk JSON save/load, humanoid rig/pose, mixer routing, baked lighting, and controller redeploy from both defeat and victory with fresh run state. See `combat-validation.txt`.
- The normal-speed run exposed ground navigation beneath solid pillars/cover. Explicit unwalkable volumes now exclude all eight footprints; spawn clearance and path-connectivity checks prevent invalid spawn positions, and melee requires line of sight. The saved navigation mesh was rebuilt without changing baked lighting. All valid perimeter routes and eight obstacle exclusions passed `navigation-validation.txt`; standalone combat checks passed again afterwards.
- 30 layout assertions passed at actual 1920×1080 and 2560×1080 render-target sizes: seven screens per size, all active controls inside the viewport, and a centered HUD crosshair. Screenshots are in `Screenshots/layout-*`. The fixture uses a camera-space canvas to render beyond this desktop's native window-size limit; it does not certify an attached ultrawide monitor. A fixture near-plane clipping issue was corrected before the final captures.
- Final standalone QA and performance logs contained no runtime errors, warnings, or unsupported-shader messages. Unity's editor package import can emit an optional Cinemachine HDRP sample assembly-reference message; HDRP is not installed or used.
- URP Forward rendering, one Medium shadow cascade, two High cascades, non-shadow-casting neon details/projectiles, static arena batching, and pooled gameplay objects reduce rendering cost. Runtime post-processing resources are retained explicitly.

## Measured performance

The complete normal-speed run is recorded in `normal-run-validation.txt`, with wave events in `normal-run-progress.txt` and the victory screen in `Screenshots/normal-victory.png`. It uses the normal Input Actions and UI, normal enemy/weapon/wave tuning, no invulnerability, no direct damage/healing, no teleports and no time acceleration. The operator's minimum recorded health was 33.5; it fired 1,966 shots. The controller uses scene-state targeting and waypoint navigation. Saved settings and high score were restored after the test.

| Full-run metric | Result |
| --- | ---: |
| Resolution / quality | 1920×1080 / Medium |
| Sampled gameplay frames | 88,643 |
| Average FPS (144 cap) | 143.8 |
| 95th-percentile frame time | 6.98 ms |
| Frames over 16.67 ms | 2 (0.0023%) |
| Peak draw calls | 259 |
| Peak triangles | 61,578 |
| Runtime errors | 0 |

This meets the performance and rendering-budget targets for the measured complete run. It is not a guarantee for every machine, random run or background workload. The test captures screenshots at two wave transitions; its frame sample includes those occasional capture costs.

The final `performance.txt` sample uses Medium quality at 1920×1080 on the RTX 3050 Laptop GPU, uncapped with VSync off, 24 enemies including a wave-10 boss, and an invulnerable stationary operator. It records 35 seconds after a five-second warmup.

| Metric | Result |
| --- | ---: |
| Samples | 13,707 |
| Average FPS | 391.6 |
| Median frame time | 2.58 ms |
| 95th-percentile frame time | 3.37 ms |
| Peak draw calls | 312 |
| Peak triangles | 88,920 |

The final fixture confirms `PERFORMANCE_FIXTURE_ALIVE=24` before sampling. It exceeds the 60 FPS target and stays below 500k triangles. Its 312 peak draw calls are 4% above the approximate 300-call goal; the normal full run peaked at 259. The legacy batch counter returned zero (unavailable); draw calls are reported separately. An intermediate fixture attempted 24 spawns but only 20 passed the new safety checks; that superseded report is retained as `performance-20-enemy-fixture.txt` and must not be treated as a 24-enemy measurement. Random placement and background workloads can change results.

## Remaining playtest and presentation limits
- A normal-speed 12-wave run now completes within the duration target. Human difficulty and fun are not established by the automated controller. The older `full-run-validation.txt` remains an accelerated progression test and must not be confused with `normal-run-validation.txt`.
- A physical Xbox controller has not been tested; controller tests inject a virtual Input System gamepad.
- 21:9 rendering is verified offscreen. This workstation has no physical ultrawide display for native monitor testing.
- Character animation, synthesized audio and the short adaptive music loop are prototype assets. Enemy death uses a burst and immediate pool return. See the README TODOs.
- Computer Use inspected the normal executable's main menu, clicked Settings, verified its layout, and returned through Save & Back on the final pass. The game was left at its main menu. A previous pass encountered two native activation failures; the later launch and interaction succeeded. Full-run and ultrawide evidence also includes the game's rendered captures.

## Environment differences
- No Unity or Blender MCP tool is exposed. Installed applications are driven through command-line scripts; Computer Use is available for visual checks.
- Blender 5.2 is installed instead of Blender 4.x.
- RTK is not installed/on PATH, so direct shell commands are used.
