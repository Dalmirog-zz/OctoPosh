<#
.Synopsis
   This cmdlet returns Octopus Users
.DESCRIPTION
   This cmdlet returns Octopus Users
.EXAMPLE
   Get-OctopusUser

   Gets all the Users on the Octopus instance
.EXAMPLE
    Get-OctopusUser -Username "Jotaro Joestar"

    Gets the user with the Username "Jotaro Joestar"
.EXAMPLE
    Get-OctopusUser -Username "Jotaro Joestar","Dio Brando"

    Gets the users with the Usernames "Jotaro Joestar" and "Dio Brando"
.EXAMPLE
    Get-OctopusUser - Username "*Joestar"

    Gets all the users whose username ends with "Joestar"
.LINK
   Website: http://Octoposh.net
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusUser
{
    [CmdletBinding()]
    Param
    (
        # User Name. Accepts wildcard.        
        [string[]]$UserName,
        

        # When used the cmdlet will only return the plain Octopus resource object
        [switch]$ResourceOnly
    )

    Begin
    {
        $c = New-OctopusConnection
        $List = @()
        $i = 1
    }
    Process{
        If($Username){
            $Users = $c.repository.Users.FindMany({param($User) if ((($User.Username -in $Username) -or ($User.Username-like $Username))) {$true}})

            foreach($u in $Username){
                        If(($u -notin $Users.username) -and !($Users.Username -like $u)){
                            write-error "No users found with the name: $u"                            
                        }
            }
        }
        Else{
            $Users = $c.repository.Users.FindAll()
        }

        If($ResourceOnly){
            $list += $Users
        }
        #Just for consistency I'm gonna keep both "ResourceOnly" and "NonResourceOnly" 
        #even though in this case they would be exactly the same because this cmdlet will return the team object exactly as it is
        else{

            $list += $Users
        }
    }

    End{
        return $List
    }

}