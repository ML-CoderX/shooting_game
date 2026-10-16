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
