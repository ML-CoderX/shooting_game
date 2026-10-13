$projectPath = Split-Path $PSScriptRoot -Parent
$buildPath = Join-Path $projectPath 'Builds\Windows'
$playerPath = Join-Path $buildPath 'NeonSiege.exe'
if (!(Test-Path -LiteralPath $playerPath)) { throw 'Build the Windows player first.' }
$readmePath = Join-Path $buildPath 'PLAY.txt'
@'
NEON SIEGE — Windows x64

Extract the entire ZIP, then run NeonSiege.exe. Keep the runtime files and
folders together. Choose DEPLOY OPERATOR to begin a twelve-wave run.

WASD: move    Mouse: look    Left/Right mouse: fire/aim
Shift: sprint    Space: dash    R: reload    1–5/Q/wheel: weapons
Escape: pause. Settings includes graphics, audio and keyboard rebinding.

Xbox controller: sticks move/look, RT/LT fire/aim, A dash, X reload,
left-stick press sprint, RB next weapon, Menu pause, D-pad/A menus.

Choose an augment after each wave. Bosses arrive on waves 5 and 10.
Orange circles mark incoming boss strikes. Dash grants brief invulnerability.
Settings and best score are saved locally in:
%USERPROFILE%\AppData\LocalLow\Siege Studio\NEON SIEGE\profile.json

This is a playable prototype with procedural art and synthesized audio.
'@ | Set-Content -LiteralPath $readmePath
$runtimePaths = @(Get-ChildItem -LiteralPath $buildPath | Where-Object {
    $_.Name -notlike '*BackUpThisFolder*' -and
    ($_.PSIsContainer -or $_.Extension -in @('.exe','.dll') -or $_.Name -eq 'PLAY.txt')
} | Select-Object -ExpandProperty FullName)
$zipPath = Join-Path $projectPath 'Builds\NeonSiege-Windows.zip'
Compress-Archive -LiteralPath $runtimePaths -DestinationPath $zipPath -CompressionLevel Optimal -Force
Get-Item -LiteralPath $zipPath | Select-Object FullName,Length
