param([ValidateSet('QA','Perf')][string]$Mode='QA',[switch]$Show)
$projectPath = Split-Path $PSScriptRoot -Parent
$playerPath = Join-Path $projectPath 'Builds\Windows\NeonSiege.exe'
if (!(Test-Path -LiteralPath $playerPath)) { throw 'Build the Windows player first.' }
$reportName = if ($Mode -eq 'QA') { 'combat-validation.txt' } else { 'performance.txt' }
$reportPath = Join-Path $projectPath ('Builds\Windows\' + $reportName)
$logPath = Join-Path $projectPath ('player-' + $Mode.ToLowerInvariant() + '.log')
$flag = if ($Mode -eq 'QA') { '-siegeQA' } else { '-siegePerf' }
$testWindowStyle = if ($Show -or $Mode -eq 'Perf') { 'Normal' } else { 'Hidden' }
$testProcess = Start-Process -FilePath $playerPath -ArgumentList @($flag,'-logFile',('"' + $logPath + '"')) -WindowStyle $testWindowStyle -PassThru
if (!$testProcess.WaitForExit(120000)) { $testProcess.Kill(); throw 'Player validation timed out. Inspect the player log.' }
if (!(Test-Path -LiteralPath $reportPath)) { throw 'Player exited without a report.' }
Copy-Item -LiteralPath $reportPath -Destination (Join-Path $projectPath ('Docs\' + $reportName))
Get-Content -LiteralPath $reportPath
if ($testProcess.ExitCode -ne 0) { throw "Player validation failed with exit code $($testProcess.ExitCode). Inspect $logPath" }
