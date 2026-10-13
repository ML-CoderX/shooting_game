param([string]$Method='NeonSiege.Editor.ProjectBuilder.Generate',[string]$Log='compile.log',[switch]$Play)
$editorPath = 'C:\Program Files\Unity\Hub\Editor\6000.6.5f1\Editor\Unity.exe'
$projectPath = Split-Path $PSScriptRoot -Parent
$unityArgs = @('-batchmode','-projectPath',('"'+$projectPath+'"'),'-executeMethod',$Method,'-logFile',('"'+(Join-Path $projectPath $Log)+'"'))
if (!$Play) { $unityArgs += '-quit' }
$unityProcess = Start-Process -FilePath $editorPath -ArgumentList $unityArgs -WindowStyle Hidden -PassThru
$unityProcess.WaitForExit()
exit $unityProcess.ExitCode
