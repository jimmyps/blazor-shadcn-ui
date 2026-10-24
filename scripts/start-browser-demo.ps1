[CmdletBinding()]
param(
    [string]$TunnelId = 'neoui-dev',
    [int]$Port = 5184
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$projectRoot = Join-Path $repoRoot 'demo\NeoUI.Demo.Server'
$assembly = Join-Path $projectRoot 'bin\Debug\net10.0\NeoUI.Demo.Server.dll'
if (!(Test-Path -LiteralPath $assembly)) {
    throw 'Build demo/NeoUI.Demo.Server before starting the preview.'
}

$logRoot = Join-Path $env:TEMP 'neoui-browser-demo'
New-Item -ItemType Directory -Path $logRoot -Force | Out-Null
$serverLog = Join-Path $logRoot 'server.log'
$serverError = Join-Path $logRoot 'server.error.log'
$tunnelLog = Join-Path $logRoot 'tunnel.log'
$tunnelError = Join-Path $logRoot 'tunnel.error.log'

$listener = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue | Select-Object -First 1
if ($listener) {
    $runningServer = Get-CimInstance Win32_Process -Filter "ProcessId=$($listener.OwningProcess)"
    if ($runningServer.CommandLine -notlike "*$assembly*") {
        throw "Port $Port is already used by a different process. Choose another port and tunnel port."
    }
    $serverId = $listener.OwningProcess
} else {
    $oldEnvironment = $env:ASPNETCORE_ENVIRONMENT
    $oldLogging = $env:Logging__LogLevel__Default
    try {
        $env:ASPNETCORE_ENVIRONMENT = 'Development'
        $env:Logging__LogLevel__Default = 'Warning'
        $server = Start-Process -FilePath (Get-Command dotnet).Source -ArgumentList @(('"{0}"' -f $assembly), '--urls', "http://localhost:$Port") -WorkingDirectory $projectRoot -WindowStyle Hidden -PassThru -RedirectStandardOutput $serverLog -RedirectStandardError $serverError
        $serverId = $server.Id
    } finally {
        $env:ASPNETCORE_ENVIRONMENT = $oldEnvironment
        $env:Logging__LogLevel__Default = $oldLogging
    }
}

$localUrl = "http://localhost:$Port/components/browser-interactions"
$healthy = $false
for ($attempt = 0; $attempt -lt 40; $attempt++) {
    try {
        $response = Invoke-WebRequest -Uri $localUrl -TimeoutSec 2
        if ($response.StatusCode -eq 200) { $healthy = $true; break }
    } catch { Start-Sleep -Milliseconds 500 }
}
if (!$healthy) { throw "The demo did not become ready. Inspect $serverLog and $serverError." }

# The named tunnel must be created separately so starting the demo never changes access policy.
$existingTunnel = Get-CimInstance Win32_Process | Where-Object {
    $_.Name -eq 'devtunnel.exe' -and $_.CommandLine -match ('\bhost\s+' + [regex]::Escape($TunnelId) + '(\.\w+)?(?:\s|$)')
} | Select-Object -First 1
if ($existingTunnel) {
    $tunnelIdProcess = $existingTunnel.ProcessId
} else {
    $tunnelProcess = Start-Process -FilePath (Get-Command devtunnel).Source -ArgumentList @('host', $TunnelId) -WorkingDirectory $repoRoot -WindowStyle Hidden -PassThru -RedirectStandardOutput $tunnelLog -RedirectStandardError $tunnelError
    $tunnelIdProcess = $tunnelProcess.Id
}

Write-Output "Demo server PID: $serverId"
Write-Output "Tunnel host PID: $tunnelIdProcess"
Write-Output "Local demo: $localUrl"
Write-Output "Logs: $logRoot"
for ($attempt = 0; $attempt -lt 80; $attempt++) {
    $log = if (Test-Path -LiteralPath $tunnelLog) { Get-Content -LiteralPath $tunnelLog -Raw } else { '' }
    if ($log -match 'Ready to accept connections') {
        $publicUrl = [regex]::Match($log, 'https://[a-zA-Z0-9.-]+\.devtunnels\.ms').Value
        if ($publicUrl) { Write-Output "Public demo: $publicUrl/components/browser-interactions" }
        return
    }
    if (!(Get-Process -Id $tunnelIdProcess -ErrorAction SilentlyContinue)) {
        throw "The tunnel host stopped. Inspect $tunnelError."
    }
    Start-Sleep -Milliseconds 500
}
throw "The tunnel is still starting. Inspect $tunnelLog and $tunnelError."
