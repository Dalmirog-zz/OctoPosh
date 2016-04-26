<#
.Synopsis
   Deletes Octopus Deploy resources such as Projects, Releases, Environments, etc.

   This is an advanced cmdlet and all its examples involve multiple lines of code. Please check the advanced examples for a better reference: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
.DESCRIPTION
   Deletes Octopus Deploy resources such as Projects, Releases, Environments, etc

   This is an advanced cmdlet and all its examples involve multiple lines of code. Please check the advanced examples for a better reference: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
.EXAMPLE
   $ProjectResource = Get-OctopusProject -name "MyApp" ; Remove-OctopusResource -resource $ProjectResource
   
   Delete the project called "MyApp" from the Octopus Database
.EXAMPLE
   Get-OctopusProjectGroup -name "MyProjects" | select -ExpandProperty Projects | Remove-OctopusResource

   Remove all the projects inside the Project Group "MyProjects"
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Remove-OctopusResource
{
    [CmdletBinding(DefaultParameterSetName='Delete')]
    Param
    (
        # Octopus resource object
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   ParameterSetName = 'Delete',                   
                   Position=0)]
        [object[]]$Resource,

        # Prints a list of the resource types accepted by the parameter $Resource.
        [parameter(ParameterSetName = 'ListAcceptedTypes')]
        [switch]$AcceptedTypes,

        # Forces cmdlet to continue without prompting
        [switch]$Force
    )

    Begin
    {
        If($AcceptedTypes){
            $types = ("Octopus.Client.Model.ProjectGroupResource",
            "Octopus.Client.Model.ProjectResource",
            "Octopus.Client.Model.EnvironmentResource",
            "Octopus.Client.Model.MachineResource",
            "Octopus.Client.Model.FeedResource",
            "Octopus.Client.Model.VariableSetResource",
            "Octopus.Client.Model.LifecycleResource",
            'Octopus.Client.Model.TeamResource',
            'Octopus.Client.Model.UserResource')
            
            return $types
        }
        $c = New-OctopusConnection        
    }
    Process
    {
        Foreach ($r in $Resource){

            if(!($Force)){
                If (!(Get-UserConfirmation -message "Are you sure you want to delete this resource? `n`n [$($R.GetType().tostring())] $($R.name)`n")){
                    Throw 'Canceled by user'
                }
            }
            switch ($r)
            {
                {$_.getType() -eq [Octopus.Client.Model.ProjectGroupResource]} {$ResourceType = 'ProjectGroups'}
                {$_.getType() -eq [Octopus.Client.Model.ProjectResource]} {$ResourceType = 'Projects'}
                {$_.getType() -eq [Octopus.Client.Model.EnvironmentResource]} {$ResourceType = 'Environments'}
                {$_.getType() -eq [Octopus.Client.Model.DeploymentResource]} {$ResourceType = 'Deployments'}
                {$_.getType() -eq [Octopus.Client.Model.MachineResource]} {$ResourceType = 'Machines'}          
                {$_.getType() -eq [Octopus.Client.Model.FeedResource]} {$ResourceType = 'Feeds'}
                {$_.getType() -eq [Octopus.Client.Model.VariableSetResource]} {$ResourceType = 'LibraryVariableSets'}
                {$_.getType() -eq [Octopus.Client.Model.LifecycleResource]} {$ResourceType = 'Lifecycles'}
                {$_.getType() -eq [Octopus.Client.Model.TeamResource]} {$ResourceType = 'Teams'}
                {$_.getType() -eq [Octopus.Client.Model.UserResource]} {$ResourceType = 'Users'}
                Default{Throw "Invalid object type: $($_.getType()) `nRun 'Remove-OctopusResource -AcceptedTypes' to get a list of the object types accepted by this cmdlet"}                
            }

            Write-Verbose "[$($MyInvocation.MyCommand)] Deleting [$($R.GetType().tostring())] $($r.name)"
            Try{
                If(($ResourceType -eq 'Feeds') -or ($ResourceType -eq 'Machines')){
                    #Deleting using REST API cause client library delete method doesnt work
                $task = (Invoke-WebRequest ($env:OctopusURL + $r.Links.Self) -Method Delete -Headers $c.header -UseBasicParsing).content | ConvertFrom-Json
                }
                elseif($ResourceType -eq 'LibraryVariableSets'){
                    #Deleting using REST API cause client library delete method doesnt work
                    $task = Invoke-RestMethod ($env:OctopusURL + '/api/LibraryVariableSets/' +$r.OwnerId) -Method Delete -Headers $c.header
                }
                else{
                    $task = $c.repository.$ResourceType.Delete($r)
                }
            }
            Catch{
                #If task fails, it'll return the error
                return $_
            }
            
        }
    }
    End
    {
        #If task starts it'll return true.
        return $true
    }
}