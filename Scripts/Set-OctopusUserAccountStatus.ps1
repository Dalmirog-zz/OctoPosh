<#
.Synopsis
   Enables or disables an Octopus User Account
.DESCRIPTION
   Enables or disables an Octopus User Account
.EXAMPLE
   Set-OctopusUserAccountStatus -Username Ian.Paullin -status Enabled
.EXAMPLE
   Get-OctopusUser -EmailAddress Ian.Paullin@VandalayIndustries.com | Set-OctopusUserAccountStatus -status Disabled
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
#>
function Set-OctopusUserAccountStatus
{
    [CmdletBinding()]
    Param
    (
        
        # Sets Octopus maintenance mode on
        [Parameter(Mandatory=$true)]
        [ValidateSet("Enabled","Disabled")] 
        [string]$status,

        # User name filter

        [String[]]$Username
        
        # Octopus user resource filter
        #[parameter(ValueFromPipelineByPropertyName=$true)]
        #[Octopus.Client.Model.UserResource[]]$Resource
        #>

    )

    Begin
    {
        if (($Username -eq $null) -and ($Resource -eq $null)){
            Throw "You must pass a value to at least one of the following parameters: Name, Resource"
        }

        $c = New-OctopusConnection

        $users = $c.repository.Users.FindMany({param($u) if (($u.username -in $Username) -or ($u.username -like $Username)) {$true}})
        
        If($Resource){$users += $Resource}

        If ($status -eq "Enabled"){$IsActive = $true}

        Else {$IsActive = $false}

    }

    Process
    {

        foreach ($user in $Users){

            Write-Verbose "Setting user account [$($user.username) ; $($user.EmailAddress)] status to: $Status"

            $user.IsActive = $IsActive

            $c.repository.Users.Modify($user)

        }

    }
    End
    {
           
    }
}