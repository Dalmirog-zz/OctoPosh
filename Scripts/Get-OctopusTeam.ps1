<#
.Synopsis
   This cmdlet returns Octopus Teams
.DESCRIPTION
   This cmdlet returns Octopus Teams
.EXAMPLE
   Get-OctopusTeam

   Gets all the teams on the Octopus instance
.EXAMPLE
    Get-OctopusTeam -name "ProjectA_Managers"

    Gets the team with the name "ProjectA_Managers"
.EXAMPLE
    Get-OctopusTeam -name "ProjectA_Managers","ProjectA_Developers"

    Gets the teams with the names "ProjectA_Managers" and "ProjectA_Developers"
.EXAMPLE
    Get-OctopusTeam -name "ProjectA*"

    Gets all the teams whose name starts with "ProjectA"
.LINK
   Website: http://Octoposh.net
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusTeam
{
    Param
    (
        # Team Name. Accepts wildcard.
        [Alias('Name')]        
        [string[]]$TeamName,
        

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
        If($TeamName){
            $Teams = $c.repository.Teams.FindMany({param($Team) if ((($Team.name -in $TeamName) -or ($Team.name -like $TeamName))) {$true}})

            foreach($t in $Teamname){
                        If(($t -notin $Teams.name) -and !($Teams.name -like $t)){
                            write-error "No Teams found with the name: $n"                            
                        }
            }
        }
        Else{
            $Teams = $c.repository.Teams.FindAll()
        }

        If($ResourceOnly){
            $list += $Teams
        }
        #Just for consistency I'm gonna keep both "ResourceOnly" and "NonResourceOnly" 
        #even though in this case they would be exactly the same because this cmdlet will return the team object exactly as it is
        else{

            $list += $Teams
        }
    }

    End{
        return $List
    }

}