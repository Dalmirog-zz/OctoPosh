<#
.Synopsis
   Enables or disables an Octopus User Account
.DESCRIPTION
   Enables or disables an Octopus User Account
.EXAMPLE
   Set-OctopusUserAccountStatus -Username Ian.Paullin -status Disabled

   Disable the account of the user Ian.Paullin
.EXAMPLE
   Get-OctopusUser -EmailAddress Ian.Paullin@VandalayIndustries.com | Set-OctopusUserAccountStatus -status Enabled

   Enable the account of the user with the email "Ian.Paullin@VandalayIndustries.com"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
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

        # User Name
        [parameter(Mandatory = $true, ValueFromPipelineByPropertyName=$true,ParameterSetName = 'Username')]
        [String[]]$Username,
        
        # Octopus user resource filter
        [parameter(Mandatory = $true, ValueFromPipelineByPropertyName=$true,ParameterSetName = 'Resource')]
        [Octopus.Client.Model.UserResource[]]$Resource
        

    )

    Begin
    {
        $c = New-OctopusConnection
        
        If($PSCmdlet.ParameterSetName -eq "Username"){
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting users with username: $Username"
            $users = $c.repository.Users.FindMany({param($u) if (($u.username -in $Username) -or ($u.username -like $Username)) {$true}})
        }
        Else{
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting by resource object"
            $users += $Resource
        }

        Write-Verbose "[$($MyInvocation.MyCommand)] Users found: $($users.Count)"

        If ($status -eq "Enabled"){$IsActive = $true}

        Else {$IsActive = $false}

    }

    Process
    {

        foreach ($user in $Users){
            Write-Verbose "[$($MyInvocation.MyCommand)] Setting user account [$($user.username) ; $($user.EmailAddress)] status to: $Status"

            $user.IsActive = $IsActive

            $ModifiedUser = $c.repository.Users.Modify($user)

            Write-Verbose "[$($MyInvocation.MyCommand)] Activity status of [$($ModifiedUser.username) ; $($Modifieduser.EmailAddress)] was set to: $($ModifiedUser.IsActive)"

        }

    }
    End
    {
        
    }
}