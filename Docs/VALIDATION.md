# NEON SIEGE validation record

Target: Windows x86-64, Unity 6000.6.5f1, URP 17.6.
Host GPU detected by Unity: NVIDIA GeForce RTX 3050 6GB Laptop GPU.

This file records actual checks, not assumed completion.

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
- Visual editor checks reached main menu, live gameplay and defeat. Follow-up asset fixes corrected the operator arm rotation and prop origins; final standalone inspection remains pending.

## M7 — baked arena and standalone
- GPU light bake completed successfully with saved lightmaps, shadowmask, 50 light probes, and navigation data.
- Windows x86-64 build and standalone checks are in progress.

## Remaining gates
- Standalone Windows build and gameplay smoke test.
- Representative performance capture (frame times, batches, triangles).
- Manual controller hardware verification if no gamepad is connected.

## Environment differences
- No Unity or Blender MCP tool is exposed. Installed applications are driven through command-line scripts; Computer Use is available for visual checks.
- Blender 5.2 is installed instead of Blender 4.x.
- RTK is not installed/on PATH, so direct shell commands are used.
