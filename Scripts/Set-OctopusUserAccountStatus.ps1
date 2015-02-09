<#
.Synopsis
   Short description
.DESCRIPTION
   Long description
.EXAMPLE
   Example of how to use this cmdlet
.EXAMPLE
   Another example of how to use this cmdlet
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
        [String[]]$Username,

        # Email address filter
        [String[]]$EmailAddress,
        
        # Octopus user resource filter
        [parameter(ValueFromPipeline=$True)]
        [Octopus.Client.Model.UserResource[]]$Resource
        #>

    )

    Begin
    {
        if ($Username -eq $null -and $EmailAddress -eq $null -and $Resource -eq $null){
            Throw "You must pass a value to at least one of the following parameters: Name, EmailAddress, Resource"
        }

        $c = New-OctopusConnection

        [Octopus.Client.Model.UserResource[]]$Users = $c.repository.Users.FindMany({param($u) if (($u.username -in $Username) -or ($u.username -like $Username) -or ($u.EmailAddress -in $EmailAddress) -or ($u.emailaddress -in $EmailAddress)) {$true}})

        $users += $Resource

        If ($status -eq "Enabled"){$IsActive = $true}

        Else {$IsActive = $false}

    }

    Process
    {

        foreach ($user in $Users){

            Write-warning "Setting user account [$($user.username) ; $($user.EmailAddress)] status to: $Status"

            $user.Isactive = $IsActive

            $c.repository.Users.Modify($user)

        }

    }
    End
    {
           
    }
}