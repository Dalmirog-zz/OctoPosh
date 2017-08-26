$octopusServerExePath = "$($env:ProgramFiles)\Octopus Deploy\Octopus\Octopus.Server.exe"

function Get-TargetResource
{
  [Diagnostics.CodeAnalysis.SuppressMessageAttribute("PSDSCUseVerboseMessageInDSCResource", "")]
  [OutputType([Hashtable])]
  param (
    [Parameter(Mandatory)]
    [ValidateNotNullOrEmpty()]
    [string]$InstanceName,
    [Parameter(Mandatory)]
    [boolean]$Enabled
  )
  # check octopus installed
  if (-not (Test-Path -LiteralPath $octopusServerExePath)) {
    throw "Unable to find Octopus (checked for existance of file '$octopusServerExePath')."
  }
  # check octopus version >= 3.5.0
  if (-not (Test-OctopusVersionSupportsAuthenticationProviders)) {
    throw "This resource only supports Octopus Deploy 3.5.0+."
  }

  $config = Get-Configuration $InstanceName

  $result = @{
    InstanceName = $InstanceName
    Enabled = $config.Octopus.UsernamePassword.IsEnabled
  }

  return $result
}

function Set-TargetResource
{
  [Diagnostics.CodeAnalysis.SuppressMessageAttribute("PSDSCUseVerboseMessageInDSCResource", "")]
  param (
    [Parameter(Mandatory)]
    [ValidateNotNullOrEmpty()]
    [string]$InstanceName,
    [Parameter(Mandatory)]
    [boolean]$Enabled
  )
  $args = @(
    'configure',
    '--console',
    '--instance', $InstanceName,
    '--usernamePasswordIsEnabled', $Enabled
  )
  Invoke-OctopusServerCommand $args
}

function Test-TargetResource
{
  [OutputType([boolean])]
  param (
    [Parameter(Mandatory)]
    [ValidateNotNullOrEmpty()]
    [string]$InstanceName,
    [Parameter(Mandatory)]
    [boolean]$Enabled
  )
  $currentResource = (Get-TargetResource -InstanceName $InstanceName `
                                         -Enabled $Enabled)

  $params = Get-Parameters $MyInvocation.MyCommand.Parameters

  $currentConfigurationMatchesRequestedConfiguration = $true
  foreach($key in $currentResource.Keys)
  {
    $currentValue = $currentResource.Item($key)
    $requestedValue = $params.Item($key)
    if ($currentValue -ne $requestedValue)
    {
      Write-Verbose "(FOUND MISMATCH) Configuration parameter '$key' with value '$currentValue' mismatched the specified value '$requestedValue'"
      $currentConfigurationMatchesRequestedConfiguration = $false
    }
    else
    {
      Write-Verbose "Configuration parameter '$key' matches the requested value '$requestedValue'"
    }
  }

  return $currentConfigurationMatchesRequestedConfiguration
}

function Get-Configuration($instanceName)
{
  $rawConfig = & $octopusServerExePath show-configuration --format=json-hierarchical --noconsolelogging --console --instance $instanceName
  $config = $rawConfig | ConvertFrom-Json
  return $config
}

function Invoke-OctopusServerCommand ($arguments)
{
  Write-Log "Executing command '$octopusServerExePath $($arguments -join ' ')'"
  $output = .$octopusServerExePath $arguments

  Write-CommandOutput $output
  if (($null -ne $LASTEXITCODE) -and ($LASTEXITCODE -ne 0)) {
    Write-Error "Command returned exit code $LASTEXITCODE. Aborting."
    exit 1
  }
  Write-Log "done."
}

function Write-CommandOutput
{
  param (
    [string] $output
  )

  if ($output -eq "") { return }

  Write-Verbose ""
  #this isn't quite working
  foreach($line in $output.Trim().Split("`n"))
  {
    Write-Verbose $line
  }
  Write-Verbose ""
}

function Test-OctopusVersionSupportsAuthenticationProviders
{
  if (-not (Test-Path -LiteralPath $octopusServerExePath))
  {
    throw "Octopus.Server.exe path '$octopusServerExePath' does not exist."
  }

  $exeFile = Get-Item -LiteralPath $octopusServerExePath -ErrorAction Stop
  if ($exeFile -isnot [System.IO.FileInfo])
  {
    throw "Octopus.Server.exe path '$octopusServerExePath ' does not refer to a file."
  }

  $fileVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($octopusServerExePath).FileVersion
  $octopusServerVersion = New-Object System.Version $fileVersion
  $versionWhereAuthenticationProvidersWereIntroduced = New-Object System.Version 3, 5, 0

  return ($octopusServerVersion -ge $versionWhereAuthenticationProvidersWereIntroduced)
}

function Get-Parameters($parameters)
{
  # unfortunately $PSBoundParameters doesn't contain parameters that weren't supplied (because the default value was okay)
  # credit to https://www.briantist.com/how-to/splatting-psboundparameters-default-values-optional-parameters/
  $params = @{}
  foreach($h in $parameters.GetEnumerator()) {
    $key = $h.Key
    $var = Get-Variable -Name $key -ErrorAction SilentlyContinue
    if ($null -ne $var)
    {
      $val = Get-Variable -Name $key -ErrorAction Stop | Select-Object -ExpandProperty Value -ErrorAction Stop
      $params[$key] = $val
    }
  }
  return $params
}