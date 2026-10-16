param([ValidateSet('QA','Perf','Run','Layout')][string]$Mode='QA',[switch]$Show)
$projectPath = Split-Path $PSScriptRoot -Parent
$playerPath = Join-Path $projectPath 'Builds\Windows\NeonSiege.exe'
if (!(Test-Path -LiteralPath $playerPath)) { throw 'Build the Windows player first.' }
$reportName = switch ($Mode) { 'QA' {'combat-validation.txt'} 'Perf' {'performance.txt'} 'Run' {'normal-run-validation.txt'} 'Layout' {'layout-validation.txt'} }
$reportPath = Join-Path $projectPath ('Builds\Windows\' + $reportName)
$logPath = Join-Path $projectPath ('player-' + $Mode.ToLowerInvariant() + '.log')
$flag = '-siege' + $Mode
$testWindowStyle = if ($Show -or $Mode -ne 'QA') { 'Normal' } else { 'Hidden' }
$testStarted = Get-Date
$testProcess = Start-Process -FilePath $playerPath -ArgumentList @($flag,'-logFile',('"' + $logPath + '"')) -WindowStyle $testWindowStyle -PassThru
$timeout = if ($Mode -eq 'Run') { 1500000 } else { 120000 }
if (!$testProcess.WaitForExit($timeout)) { $testProcess.Kill(); throw 'Player validation timed out. Inspect the player log.' }
if (!(Test-Path -LiteralPath $reportPath)) { throw 'Player exited without a report.' }
if ((Get-Item -LiteralPath $reportPath).LastWriteTime -lt $testStarted) { throw 'Player did not write a fresh report.' }
Copy-Item -LiteralPath $reportPath -Destination (Join-Path $projectPath ('Docs\' + $reportName))
Get-Content -LiteralPath $reportPath
if ($testProcess.ExitCode -ne 0) { throw "Player validation failed with exit code $($testProcess.ExitCode). Inspect $logPath" }
if ($Mode -in @('Run','Perf')) {
    $runReport = Get-Content -LiteralPath $reportPath -Raw
    if ($Mode -eq 'Run' -and $runReport -notmatch 'Within 10.+15 minute target: True') { throw 'The completed run missed the 10-15 minute duration target.' }
    if ($Mode -eq 'Perf' -and (Get-Content -LiteralPath $logPath -Raw) -notmatch 'PERFORMANCE_FIXTURE_ALIVE=24') { throw 'The performance fixture did not confirm 24 live enemies.' }
    if ($runReport -notmatch '95th percentile ms: ([0-9.]+)') { throw 'Missing full-run frame-time measurement.' }
    $frameTime95 = [double]::Parse($Matches[1], [Globalization.CultureInfo]::InvariantCulture)
    if ($frameTime95 -gt 16.67) { throw 'The 95th-percentile frame time exceeded the 60 FPS budget.' }
}
