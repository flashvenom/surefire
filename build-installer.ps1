[CmdletBinding()]
param(
    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release',

    [ValidateSet('win-x64')]
    [string]$RuntimeIdentifier = 'win-x64',

    [string]$DesktopTargetFramework = 'net10.0-windows10.0.19041.0',

    [switch]$SkipPrune,
    [switch]$SkipInstaller
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = $PSScriptRoot
$buildDir = Join-Path $repoRoot 'build'
$webDir = Join-Path $buildDir 'web'
$desktopDir = Join-Path $buildDir 'desktop'
$installerDir = Join-Path $buildDir 'installer'
$logDir = Join-Path $buildDir 'logs'
$logPath = Join-Path $logDir 'build-installer.log'

function Write-Log([string]$message) {
    $timestamped = "[{0}] {1}" -f (Get-Date -Format o), $message
    Write-Host $timestamped
    try { Add-Content -Path $logPath -Value $timestamped -Encoding UTF8 } catch { }
}

function Ensure-Directory([string]$path) {
    if (-not (Test-Path -LiteralPath $path)) {
        New-Item -ItemType Directory -Path $path | Out-Null
    }
}

function Clear-Directory([string]$path) {
    Ensure-Directory $path
    Get-ChildItem -LiteralPath $path -Force | Remove-Item -Recurse -Force
}

function Clear-DirectoryExceptChild([string]$path, [string]$childNameToKeep) {
    if (-not (Test-Path -LiteralPath $path)) {
        Write-Log "[SKIP] Directory not found: $path"
        return
    }

    Write-Log "Pruning directory: $path (keeping $childNameToKeep)"
    Get-ChildItem -LiteralPath $path -Force |
        Where-Object { $_.Name -ne $childNameToKeep } |
        Remove-Item -Recurse -Force
}

function Invoke-External([string]$filePath, [string[]]$arguments) {
    $line = $filePath + ' ' + ($arguments -join ' ')
    Write-Log "[RUN] $line"

    & $filePath @arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Command failed with exit code ${LASTEXITCODE}: $line"
    }
}

function Invoke-Robocopy([string]$source, [string]$destination) {
    Ensure-Directory $destination
    $args = @($source, $destination, '/MIR', '/NFL', '/NDL', '/NJH', '/NJS', '/NP')
    Write-Log "[RUN] robocopy $($args -join ' ')"
    & robocopy @args | Out-Null
    if ($LASTEXITCODE -ge 8) {
        throw "robocopy failed with exit code $LASTEXITCODE"
    }
}

function Remove-NonWindowsRuntimesDeep([string]$root) {
    if (-not (Test-Path -LiteralPath $root)) {
        return
    }

    $keep = @('win', 'win-x64')
    Get-ChildItem -LiteralPath $root -Directory -Recurse -Force |
        Where-Object { $_.Name -eq 'runtimes' } |
        ForEach-Object {
            Get-ChildItem -LiteralPath $_.FullName -Directory -Force | Where-Object {
                -not ($keep -contains $_.Name)
            } | ForEach-Object {
                Remove-Item -LiteralPath $_.FullName -Recurse -Force
                Write-Log "[OK] Removed runtime: $($_.FullName)"
            }
        }
}

function Remove-NonEnglishFolders([string]$root) {
	return
    if (-not (Test-Path -LiteralPath $root)) {
        return
    }

    $keep = @('en', 'en-us', 'en-gb')
    Get-ChildItem -LiteralPath $root -Directory -Recurse -Force | Where-Object {
        $name = $_.Name.ToLowerInvariant()
        $name -match '^[a-z]{2}(-[a-z0-9]+)*$' -and -not ($keep -contains $name)
    } | ForEach-Object {
        Remove-Item -LiteralPath $_.FullName -Recurse -Force
        Write-Log "[OK] Removed satellite folder: $($_.FullName)"
    }
}

function Resolve-IsccExe {
    $cmd = Get-Command iscc.exe -ErrorAction SilentlyContinue
    if ($cmd -and (Test-Path -LiteralPath $cmd.Path)) {
        return $cmd.Path
    }

    $candidates = @(
        (Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe'),
        (Join-Path $env:ProgramFiles 'Inno Setup 6\ISCC.exe')
    )

    foreach ($candidate in $candidates) {
        if (Test-Path -LiteralPath $candidate) {
            return $candidate
        }
    }

    throw 'Inno Setup compiler (iscc.exe) not found. Install Inno Setup 6 and ensure ISCC is available.'
}

try {
    Ensure-Directory $logDir
    "Quickfire installer build log" | Set-Content -Path $logPath -Encoding UTF8
    Write-Log "Repo: $repoRoot"
    Write-Log "Config: $Configuration; RID: $RuntimeIdentifier; Desktop TFM: $DesktopTargetFramework"

    Write-Log "Cleaning build outputs..."
    Clear-Directory $webDir
    Clear-Directory $desktopDir
    Clear-Directory $installerDir

    Write-Log "Publishing Blazor host -> $webDir"
    Invoke-External 'dotnet' @(
        'publish', (Join-Path $repoRoot 'src\Quickfire.Blazor\Quickfire.Blazor.csproj'),
        '-c', $Configuration,
        '-r', $RuntimeIdentifier,
        '-o', $webDir
    )

    if (-not $SkipPrune) {
        Remove-NonWindowsRuntimesDeep $webDir
        Remove-NonEnglishFolders $webDir
    }

    Write-Log "Staging host assets into MAUI package..."
    Invoke-External 'dotnet' @(
        'msbuild', (Join-Path $repoRoot 'src\Quickfire.Desktop\Quickfire.Desktop.csproj'),
        '/t:PrepareQuickfireHostPackage',
        "/p:Configuration=$Configuration",
        "/p:TargetFramework=$DesktopTargetFramework",
        "/p:QuickfirePublishDir=$webDir"
    )

    Write-Log "Publishing MAUI shell..."
    Invoke-External 'dotnet' @(
        'publish', (Join-Path $repoRoot 'src\Quickfire.Desktop\Quickfire.Desktop.csproj'),
        '-f', $DesktopTargetFramework,
        '-c', $Configuration,
        '-r', $RuntimeIdentifier,
        '-p:WindowsPackageType=None',
        "/p:QuickfirePublishDir=$webDir"
    )

    $desktopPublishSource = Join-Path $repoRoot ("src\Quickfire.Desktop\bin\{0}\{1}\{2}\publish" -f $Configuration, $DesktopTargetFramework, $RuntimeIdentifier)
    if (-not (Test-Path -LiteralPath $desktopPublishSource)) {
        throw "Desktop publish output not found at $desktopPublishSource"
    }

    Write-Log "Staging desktop publish -> $desktopDir"
    Invoke-Robocopy $desktopPublishSource $desktopDir

    if (-not $SkipPrune) {
        Remove-NonWindowsRuntimesDeep $desktopDir
        Remove-NonEnglishFolders $desktopDir
    }

    if (-not $SkipInstaller) {
        Write-Log "Compiling installer..."
        $isccExe = Resolve-IsccExe
        $iss = Join-Path $repoRoot 'loot\install\quickfire-installer.iss'

        Invoke-External $isccExe @($iss)

        Write-Log "Installer output: $(Join-Path $installerDir 'Install_Quickfire.exe')"
    }

    Write-Log 'DONE'
}
catch {
    Write-Log "FAILED: $($_.Exception.Message)"
    throw
}
