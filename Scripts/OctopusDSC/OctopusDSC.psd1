@{
  ModuleVersion = '2.0'
  GUID = 'cebaa69e-f83e-441b-a818-70c0e5c6cf46'
  Author = 'Paul Stovell'
  CompanyName = 'Octopus Deploy Pty. Ltd.'
  Copyright = '(c) 2016 Octopus Deploy Pty. Ltd. All rights reserved.'
  PowerShellVersion = '3.0'
  CLRVersion = '4.0'
  FunctionsToExport = @('Get-TargetResource','Set-TargetResource','Test-TargetResource')
  Description = 'Module with DSC resource to install and configure an Octopus Deploy Server and Tentacle agent.'
  PrivateData = @{
    PSData = @{
        Tags = @('DSC Resource', 'DSC', 'Octopus Deploy')
        ProjectUri = 'https://github.com/OctopusDeploy/OctopusDSC'
        IconUri = 'https://github.com/OctopusDeploy/OctopusDSC/blob/master/OctopusDSC/Octopus_blue_64px.png'
    }
  }
}
