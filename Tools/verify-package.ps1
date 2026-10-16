$ErrorActionPreference = 'Stop'
$projectPath = Split-Path $PSScriptRoot -Parent
$archivePath = Join-Path $projectPath 'Builds\NeonSiege-Windows.zip'
$testDirectory = Join-Path $projectPath ('Builds\PackageCheck-' + [Guid]::NewGuid().ToString('N').Substring(0,8))
Expand-Archive -LiteralPath $archivePath -DestinationPath $testDirectory
$files = @(Get-ChildItem -LiteralPath $testDirectory -Recurse -File)
foreach ($file in $files) {
    $relative = $file.FullName.Substring($testDirectory.Length + 1)
    $source = Join-Path $projectPath ('Builds\Windows\' + $relative)
    if (!(Test-Path -LiteralPath $source) -or (Get-FileHash -LiteralPath $file.FullName).Hash -ne (Get-FileHash -LiteralPath $source).Hash) { throw "Archive mismatch: $relative" }
}
foreach ($required in @('NeonSiege.exe','UnityPlayer.dll','NeonSiege_Data\Managed\NeonSiege.Runtime.dll','PLAY.txt')) {
    if (!(Test-Path -LiteralPath (Join-Path $testDirectory $required))) { throw "Missing runtime file: $required" }
}
$playerPath = Join-Path $testDirectory 'NeonSiege.exe'
$bytes = [IO.File]::ReadAllBytes($playerPath)
$peOffset = [BitConverter]::ToInt32($bytes,60)
if ([BitConverter]::ToUInt16($bytes,$peOffset+4) -ne 0x8664) { throw 'Player is not Windows x86-64.' }
$logPath = Join-Path $projectPath 'player-package.log'
$process = Start-Process -FilePath $playerPath -ArgumentList @('-siegeQA','-logFile',('"'+$logPath+'"')) -WindowStyle Hidden -PassThru
if (!$process.WaitForExit(120000)) { $process.Kill(); throw 'Extracted player timed out.' }
if ($process.ExitCode -ne 0) { throw "Extracted player exited with code $($process.ExitCode)." }
$checks = @(Get-Content -LiteralPath (Join-Path $testDirectory 'combat-validation.txt'))
$passCount = @($checks | Where-Object { $_ -like 'PASS *' }).Count
if ($passCount -ne 30 -or @($checks | Where-Object { $_ -like 'FAIL *' }).Count -gt 0) { throw 'Extracted player did not pass all 30 checks.' }
$archiveHash = (Get-FileHash -LiteralPath $archivePath -Algorithm SHA256).Hash
$report = @(
    'PASS packaged Windows player',
    "Archive files verified against build: $($files.Count)",
    'PE machine: 0x8664 (Windows x86-64)',
    'Extracted executable launched and passed all 30 combat/input/save/restart checks.',
    "Archive SHA256: $archiveHash",
    "Archive bytes: $((Get-Item -LiteralPath $archivePath).Length)",
    'The extracted test folder is retained under Builds/PackageCheck-* (ignored by Git).'
)
$report | Set-Content -LiteralPath (Join-Path $projectPath 'Docs\package-validation.txt')
$report
