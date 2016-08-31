<#
.Synopsis
   Gets information about Octopus Project Groups
.DESCRIPTION
   Gets information about Octopus Project Groups
.EXAMPLE
   Get-OctopusProjectGroup 

   Get all the Project Groups on the Octopus instance
.EXAMPLE
   Get-OctopusProjectGroup -name "MyProjects"

   Get a Project Group named "MyProjects"
.EXAMPLE
   Get-OctopusProjectGroup -name "*web*"

   Get all the Project Groups with the word "web" on their name
.EXAMPLE
   Get-OctopusProject "MyProject" | Get-OctopusProjectGroup | select -ExpandProperty projects | Get-OctopusProjects

   Get all the projects that are on the same group as the project "MyProject"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusProjectGroup
{
    [CmdletBinding()]    
    Param
    (
        # Name of the Project Group
        [alias("Name")]
        [Parameter(ValueFromPipelineByPropertyName=$true,Position=0)]
        [string[]]$ProjectGroupName,

        # When used the cmdlet will only return the plain Octopus resource object
        [switch]$ResourceOnly
    )

    Begin
    {
        $c = New-OctopusConnection
        $list = @()
        $i = 1
    }
    Process
    {
        If(!([string]::IsNullOrEmpty($ProjectGroupName))){
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting Project Groups by name: $ProjectGroupName"

            $ProjectGroups = $c.repository.ProjectGroups.FindMany({param($Pg) if (($Pg.name -in $ProjectGroupName) -or ($Pg.name -like $ProjectGroupName)) {$true}})
            foreach($N in $ProjectGroupName){
                If(($n -notin $ProjectGroups.name) -and !($ProjectGroups.name -like $n)){
                    Write-Error "Project group not found: $n"
                    #write-host "Project group not found: $n" -ForegroundColor Red
                }
            }
        }

        else{
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting all Project Groups"
            $ProjectGroups = $c.repository.ProjectGroups.FindAll()
        }

        Write-Verbose "[$($MyInvocation.MyCommand)] Project Groups found: $($ProjectGroups.count)"
        If($ResourceOnly){
            $list = $ProjectGroups
        }
        Else{
            foreach($ProjectGroup in $ProjectGroups){

            Write-Progress -Activity "Getting info from Project Group: $($ProjectGroup.name)" -status "$i of $($ProjectGroups.count)" -percentComplete ([Double]$i / $ProjectGroups.count*100)
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting info of Project Group: $($ProjectGroup.name)"

            $Plist = @()
            
            $Projects = Get-OctopusResource -uri $ProjectGroup.links.projects -header $c.header
            
            foreach($project in $Projects.items){

                $p = [PSCustomObject]@{
                        ProjectName = $project.name}

                $Plist += $p
            }

            $pg = [PSCustomObject]@{
                        Name = $ProjectGroup.name
                        Id = $ProjectGroup.id
                        Projects = $plist
                        resource = $ProjectGroup}

            $list += $pg

            $i++
        }
        }
    }
    End
    {
        If($list.count -eq 0){
            $list = $null
        }
        return $List
    }
}