<#
.Synopsis
   This cmdlet returns Octopus User Roles
.DESCRIPTION
   This cmdlet returns Octopus User Roles
.EXAMPLE
   Get-OctopusUserRole

   Gets all the user roles on the Octopus instance
.EXAMPLE
    Get-OctopusUserRole -name "Environment Manager"

    Gets the user role with the name "Environment Manager"
.EXAMPLE
    Get-OctopusUserRole -name "Project Lead","Environment Manager"

    Gets the user roles with the names "Project Lead" & "Environment Manager"
.EXAMPLE
    Get-OctopusTeam -name "Environment*"

    Gets all the roles whose name starts with "Environment"
.LINK
   Website: http://Octoposh.net
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusUserRole
{
    Param
    (
        # User role Name. Accepts wildcard.
        [Alias('Name')]        
        [string[]]$UserRoleName
    )

    Begin
    {
        $c = New-OctopusConnection
    }
    Process{
        If($UserRoleName){
            $UserRoles = $c.repository.UserRoles.FindMany({param($UserRole) if ((($UserRole.name -in $UserRoleName) -or ($UserRole.name -like $UserRoleName))) {$true}})

            foreach($ur in $UserRoleName){
                        If(($ur -notin $UserRoles.name) -and !($UserRoles.name -like $ur)){
                            write-error "No User Roles found with the name: $ur"                            
                        }
            }
        }
        Else{
            $UserRoles = $c.repository.UserRoles.FindAll()
        }        
    }

    End{
        return $UserRoles
    }

}