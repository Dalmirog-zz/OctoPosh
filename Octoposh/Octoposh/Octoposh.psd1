@{
#
# Script module or binary module file associated with this manifest.
RootModule = 'OctoPosh.psm1'

# Version number of this module.
ModuleVersion = '0.0.0.0'

# Author of this module
Author = 'Dalmiro Granias ; http://about.me/dalmiro.granias'

# Description of the functionality provided by this module
Description = 'This module contains a set of cmdlets that talk to the Octopus REST API to perform basic Octopus Deploy administration tasks'

# Minimum version of the Windows PowerShell engine required by this module
PowerShellVersion = '3.0'

# Functions to export from this module
FunctionsToExport = '*'

# ID used to uniquely identify this module
GUID = 'd0a9150d-b6a4-4b17-a325-e3a24fed0aa9'

# Cmdlets to export from this module.
# This is being handled by on the psm1 file
# CmdletsToExport = '*'

# Assemblies that must be loaded prior to importing this module
# RequiredAssemblies = @()

# Private data to pass to the module specified in RootModule/ModuleToProcess. This may also contain a PSData hashtable with additional module metadata used by PowerShell.
PrivateData = @{

    PSData = @{

        # Tags applied to this module. These help with module discovery in online galleries.
         Tags = @('OctopusDeploy','Deployment')

        # A URL to the license for this module.
        # LicenseUri = ''

        # A URL to the main website for this project.
         ProjectURI = 'http://dalmirog.github.io/OctoPosh/'

        # A URL to an icon representing this module.
         IconUri = 'http://s6.postimg.org/x5auom5xd/librito_lg_white.png'

        # ReleaseNotes of this module
         ReleaseNotes = 'https://github.com/Dalmirog/OctoPosh/wiki/Release-Notes'

    } # End of PSData hashtable

} # End of PrivateData hashtable


}
