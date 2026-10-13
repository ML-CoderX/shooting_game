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
- Visual checks reached main menu, live gameplay, settings and defeat. Standalone captures are in `Screenshots/`. Computer Use inspected the running game window.
- Corrected the operator's arm rotation and humanoid root-height import. The regression check measures head height 1.66 m and foot height 0.13 m relative to the character root. Prop origins were corrected before the final light bake.

## M7 — baked arena and standalone
- GPU light bake completed successfully with saved lightmaps, shadowmask, 50 light probes, and navigation data.
- Windows x86-64 / DirectX 11 / Mono build succeeded. Project C# compilation has no errors or warnings.
- 28 standalone checks passed: five weapons and reloads, sniper piercing, cover blocking, virtual-gamepad movement/dash/menu navigation, persistent bindings, disk JSON save/load, humanoid rig/pose, mixer routing, and baked lighting. See `combat-validation.txt`.
- Final standalone QA and performance logs contained no runtime errors, warnings, or unsupported-shader messages. Unity's editor package import can emit an optional Cinemachine HDRP sample assembly-reference message; HDRP is not installed or used.
- URP Forward rendering, one Medium shadow cascade, two High cascades, non-shadow-casting neon details/projectiles, static arena batching, and pooled gameplay objects reduce rendering cost. Runtime post-processing resources are retained explicitly.

## Measured performance

The final `performance.txt` sample uses Medium quality at 1920×1080 on the RTX 3050 Laptop GPU, uncapped with VSync off, 24 enemies including a wave-10 boss, and an invulnerable stationary operator. It records 35 seconds after a five-second warmup.

| Metric | Result |
| --- | ---: |
| Samples | 13,713 |
| Average FPS | 391.8 |
| Median frame time | 2.64 ms |
| 95th-percentile frame time | 3.46 ms |
| Peak draw calls | 299 |
| Peak triangles | 86,018 |

These observations meet the requested targets in this scenario. They do not guarantee stable performance throughout every full run or on another machine. The legacy batch counter returned zero (unavailable); the draw-call counter is reported separately. Random enemy placement means repeat measurements can vary.

## Remaining playtest and presentation limits
- A normal 10–15 minute run has not been played end to end to establish difficulty, duration, or fun. The full-run test accelerates spawning and directly damages enemies.
- A physical Xbox controller has not been tested; controller tests inject a virtual Input System gamepad.
- 21:9 anchors and height-based UI scaling are implemented. The workstation clamped a requested 2560×1080 window to 1920×1080, so native ultrawide rendering remains unverified.
- Character animation, synthesized audio and the short adaptive music loop are prototype assets. Enemy death uses a burst and immediate pool return. See the README TODOs.

## Environment differences
- No Unity or Blender MCP tool is exposed. Installed applications are driven through command-line scripts; Computer Use is available for visual checks.
- Blender 5.2 is installed instead of Blender 4.x.
- RTK is not installed/on PATH, so direct shell commands are used.
