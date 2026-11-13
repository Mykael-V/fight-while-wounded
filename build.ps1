param([string]$GamePath = 'D:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord')
$ErrorActionPreference = 'Stop'
$compilerPath = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$gameBin = Join-Path $GamePath 'bin\Win64_Shipping_Client'
$outputDir = Join-Path $PSScriptRoot 'Modules\FightWhileWounded\bin\Win64_Shipping_Client'
New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
$referenceNames = @('TaleWorlds.Core','TaleWorlds.Library','TaleWorlds.MountAndBlade','TaleWorlds.CampaignSystem','TaleWorlds.ObjectSystem','TaleWorlds.Localization','TaleWorlds.SaveSystem','TaleWorlds.Engine','TaleWorlds.DotNet')
$compilerArgs = @('/nologo','/target:library','/optimize+','/platform:x64',("/out:" + (Join-Path $outputDir 'FightWhileWounded.dll')))
$compilerArgs += '/reference:' + (Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\netstandard.dll')
foreach ($referenceName in $referenceNames) { $compilerArgs += '/reference:' + (Join-Path $gameBin ($referenceName + '.dll')) }
$compilerArgs += Join-Path $PSScriptRoot 'source\SubModule.cs'
& $compilerPath @compilerArgs
if ($LASTEXITCODE -ne 0) { throw 'Falha ao compilar.' }
Write-Output 'Compilado com sucesso.'
