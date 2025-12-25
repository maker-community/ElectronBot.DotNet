<#
.SYNOPSIS
  Publish `Verdure.Braincase` as a Windows self-contained single-file executable.

.DESCRIPTION
  Run this script on Windows (PowerShell) where .NET SDK, Windows 10/11 SDK and
  Windows App SDK / WinUI tooling are available. The script performs a `dotnet restore`
  and `dotnet publish` for `src/Verdure.Braincase/Verdure.Braincase.csproj`.

.EXAMPLE
  # In an elevated PowerShell prompt
  Set-ExecutionPolicy -Scope Process -ExecutionPolicy RemoteSigned
  .\scripts\publish-verdure-braincase.ps1

#>

param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$Platform = "x64",
    [string]$Output = ".\artifacts\publish\Verdure.Braincase-win-x64"
)

$proj = "src\Verdure.Braincase\Verdure.Braincase.csproj"
$solution = "ElectronBot.Braincase.sln"

Write-Host "Publishing project: $proj"
Write-Host "Configuration: $Configuration, Runtime: $Runtime, Platform: $Platform"
Write-Host "Output folder: $Output"

if (-not (Test-Path $proj)) {
    Write-Error "Project file not found: $proj"
    exit 2
}

Write-Host "Restoring solution..."
$restore = dotnet restore $solution
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet restore failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Publishing (this may take a while)..."
$publishCmd = @(
    'dotnet', 'publish', $proj,
    '-c', $Configuration,
    '-r', $Runtime,
    '--self-contained', 'true',
    '-p:PublishSingleFile=true',
    '-p:Platform=' + $Platform,
    '-o', $Output
) -join ' '

Write-Host $publishCmd
Invoke-Expression $publishCmd

if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet publish failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Publish complete. Output directory: $Output"
Write-Host "Tip: If native dependencies (HID/USB) are missing or incorrect, build on a Windows runner or in Visual Studio with the matching SDKs installed."