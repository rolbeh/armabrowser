param(
	[string]$MakensisPath = "BuildTools\NSIS\makensis.exe",
	[string]$nsisScript = ".\Setup-Script.nsi",
	[Parameter(Mandatory=$True)]
	[string]$outputPath

)

Write-Host "";
Write-Host "Create Setup with NSIS";
Write-Host "";
Write-Host "MakensisPath : " $MakensisPath;
Write-Host "nsisScript   : " $nsisScript;
Write-Host "outputPath   : " $outputPath;
Write-Host "";

$outputDir =  ([System.IO.Path]::GetDirectoryName($outputPath));
if(!(Test-Path $outputDir)) {
	New-Item -Force $outputDir -type directory | Out-Null;
}

&"$MakensisPath" $nsisScript "/XOutFile $outputPath";