<#
.Synopsis
   Creates a clone of an Octopus project an returns the [Octopus.Client.Model.ProjectResource] of the new project
.DESCRIPTION
   Creates a clone of an Octopus project an returns the [Octopus.Client.Model.ProjectResource] of the new project
.EXAMPLE
  New-OctopusProjectClone -BaseProjectName "MyExistingProject" -ProjectName "MyClonedProject"

  Creates a clone of project "MyExistingProject" named "MyClonedProject"
.EXAMPLE
  New-OctopusProjectClone -BaseProjectName "MyExistingProject" -ProjectName "MyClonedProject" -ProjectGroupName "ProjectGroup2"

  Creates a clone of project "MyExistingProject" named "MyClonedProject" under "ProjectGroup2"
.EXAMPLE
  New-OctopusProjectClone -BaseProjectName "MyExistingProject" -ProjectName "MyClonedProject" -LifeCycleName "LifeCycle2"

  Creates a clone of project "MyExistingProject" named "MyClonedProject" that will use the lifecycle name "Lifecycle2"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function New-OctopusProjectClone
{
    [CmdletBinding()]    
    [OutputType([Octopus.Client.Model.ProjectResource])]
    Param
    (
        # Name of the Project on which the clone project will be based on.
        [Parameter(Mandatory=$true)]
        [Alias("BaseProject")] 
        [string]$BaseProjectName,

        # The name of the clone project.
        [Parameter(Mandatory=$true)]
        [Alias("Name")] 
        [String]$ProjectName,

        # The name of the lifecycle that will be used by the clone project. If you don't pass a value to this parameter, the same lifecycle of the base project will be used by the cloned one.
        [Alias("LifeCycle")]
        [String]$LifecycleName,

        # The name of the Project Group that will be used by the clone project. If you don't pass a value to this parameter, the same Project Group of the base project will be used by the cloned one.
        [Alias("ProjectGroup")]
        [String]$ProjectGroupName,

        # The description of the cloned project        
        [String]$Description = ""
    )

    Begin
    {
        $c = New-OctopusConnection
    }
    Process
    {
        #region validate Project
        $allProjects = $c.repository.Projects.FindAll()

        If($projectname -in $allProjects.Name){
            Throw "A project with the name [$Projectname] already exists. Select a new name for the clone."
        }        

        $BaseProject = $allProjects | ?{$_.Name -eq $BaseProjectName}
        
        If($BaseProject.count -eq 0){
            Throw "Couldn't find base project: $BaseProjectName"
        }
        
        Write-Verbose "[$($MyInvocation.MyCommand)] Starting clone of [$($BaseProject.Name)] under name [$ProjectName]"
        #endregion

        #region validate Lifecycle
        If($LifecycleName){
            Write-Verbose "[$($MyInvocation.MyCommand)] Looking for custom lifecycle: [$LifecycleName]"
            $lifecycle = $c.repository.Lifecycles.FindByName($LifecycleName)
        }
        else{            
            Write-Verbose "[$($MyInvocation.MyCommand)] Using Lifecycle from project [$($BaseProject.name)]"
            $lifecycle = $c.repository.Lifecycles.Get($BaseProject.LifeCycleID)
        }
        If($lifecycle.count -eq 0){
            Throw "Lifecycle not found: $LifecycleName"
        }
        Write-Verbose "[$($MyInvocation.MyCommand)] Lifecycle Found: [$($lifecycle.name)]"
        #endregion

        #region validate ProjectGroup
        If($ProjectGroupName){
            Write-Verbose "[$($MyInvocation.MyCommand)] Looking for custom Project Group: [$ProjectGroupName]"
            $ProjectGroup = $c.repository.ProjectGroups.FindByName($ProjectGroupName)            
        }
        else{
            Write-Verbose "[$($MyInvocation.MyCommand)] Using Project Group from project [$($BaseProject.name)]"
            $ProjectGroup = $c.repository.ProjectGroups.Get($BaseProject.ProjectGroupID)
        }
        If($ProjectGroup.count -eq 0){
            Throw "ProjectGroup not found: $ProjectGroupName"
        }
        Write-Verbose "[$($MyInvocation.MyCommand)] Project Group Found: [$($ProjectGroup.name)]"
        #endregion

        #region Build clone object and POST
        $Clone = [PSCustomObject]@{
                    DefaultToSkipIfAlreadyInstalled = $BaseProject.DefaultToSkipIfAlreadyInstalled
                    Description = $description #If($Description){$Description}else{""}
                    LifecycleID = $lifecycle.id
                    Name = $ProjectName
                    ProjectGroupID = $ProjectGroup.Id
                    VersioningStrategy = $BaseProject.VersioningStrategy
                }

        Try{
            Write-Verbose "[$($MyInvocation.MyCommand)] Cloning [$($BaseProject.Name)]. Clone project name will be [$ProjectName]"
            $null = Post-OctopusResource -uri "api/projects?clone=$($BaseProject.id)" -resource $Clone
            $NewProject = $c.repository.Projects.FindByName($ProjectName)
        }
        catch{
            Throw $_
        }
        #endregion
    }
    End
    {
        return $NewProject
    }
}