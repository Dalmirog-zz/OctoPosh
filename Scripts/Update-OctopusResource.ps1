<#
.Synopsis
   Updates Octopus Resources on the database.   

   This is an advanced cmdlet and all its examples involve multiple lines of code. Please check the advanced examples for a better reference: https://github.com/Dalmirog/OctoPosh/wiki/Modifying-Resources
.DESCRIPTION
   Updates Octopus Resources on the database.   

   This is an advanced cmdlet and all its examples involve multiple lines of code. Please check the advanced examples for a better reference: https://github.com/Dalmirog/OctoPosh/wiki/Modifying-Resources
.EXAMPLE
    $pg = Get-OctopusProjectGroup -name SomeProjectName ; $pg.resource.name = "SomeOtherProjectName" ; $Pg | Update-OctopusResource

    Update the Name of a ProjectGroup   
.EXAMPLE
    $machine = Get-OctopusMachine -MachineName "SQL_Production" ; $machine.resource.isdisabled = $true ; $machine | Update-OctopusResource
    
    Update the [IsDisabled] property of a machine to disable it
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Modifying-Resources
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Update-OctopusResource
{
    [CmdletBinding(DefaultParameterSetName='Update')]
    Param
    (
        # Octopus resource object
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   ParameterSetName = 'Update',
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
            "Octopus.Client.Model.VariableSetResource")
            
            return $types
        }

        $c = New-OctopusConnection        
    }
    Process
    {
        Foreach ($r in $Resource){

            if(!($Force)){
                If (!(Get-UserConfirmation -message "Are you sure you want to update this resource? `n`n [$($R.GetType().tostring())] $($R.name)`n")){
                    Throw 'Canceled by user'
                }
            }
            switch ($r)
            {
                {$_.getType() -eq [Octopus.Client.Model.ProjectGroupResource]} {$ResourceType = 'ProjectGroups'}
                {$_.getType() -eq [Octopus.Client.Model.ProjectResource]} {$ResourceType = 'Projects'}
                {$_.getType() -eq [Octopus.Client.Model.EnvironmentResource]} {$ResourceType = 'Environments'}                
                {$_.getType() -eq [Octopus.Client.Model.MachineResource]} {$ResourceType = 'Machines'}          
                {$_.getType() -eq [Octopus.Client.Model.FeedResource]} {$ResourceType = 'Feeds'}                
                {$_.getType() -eq [Octopus.Client.Model.VariableSetResource]} {$ResourceType = 'VariableSets'}
                Default{Throw "Invalid object type: $($_.getType()) `nRun 'Update-OctopusResource -AcceptedTypes' to get a list of the object types accepted by this cmdlet"}                
            }

            Write-Verbose "[$($MyInvocation.MyCommand)] Updating $ResourceType`: $($r.name)"

            Try{
                $UpdatedObject = $c.repository.$ResourceType.Modify($r)
            }
            Catch{
                Write-Error $_    
            }
        }
    }
    End
    {
        If(!$UpdatedObject){
            $UpdatedObject = $null
        }

        return $UpdatedObject
    }
}