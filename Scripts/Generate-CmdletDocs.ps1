[CmdletBinding()]
Param
(
    # ModuleName
    [string]$Module = "Octoposh",

    # CmdletName = 
    [string]$CmdletName = "*",

    # Destionation directory
    [string]$Destination = "$PSscriptRoot\..\docs\cmdlets",

    # yaml index only
    [switch]$YamlIndexOnly
)

Begin
{    
    if(Get-Module $Module){
        Remove-Module $Module
    }
    if(!(Get-Module $Module -ListAvailable)){
        throw "Module $Module is not installed on the machine"
    }

    Import-Module $Module -Force

    function Replace-TypeName ($Type) {
        switch ($Type) {
            "System.String[]" {return "String[]"}
            "System.String" {return "String"}
            "System.Management.Automation.SwitchParameter" {return "Switch"}        
        }
    }
    function Get-YamlCmdletsIndex{
        #If $Destination is `..\docs\cmdlets`, get only "cmdlets"
        $baseFolder = $Destination.Split('\')[-1]

        $sb = New-Object System.Text.StringBuilder
        foreach($cmdlet in $cmdlets){
            <#
                the StringBuilder should end up with 1 line per cmdlet in the module in the following format: [tab]- [cmdletName]: cmdlets/[cmdletname].md            
                i.e: Get-OctopusChannel: cmdlets/get-octopuschannel.md
            #>
        
            $cmdletPath = [string]::Concat($baseFolder,"/",$cmdlet.ToLower(),".md")
            $null = $sb.AppendLine("  - $cmdlet`: $cmdletPath")
        }

        $sb.ToString() | clip
        "Index copied to clipboard. Please manually put it into [ProjectRoot]\mkdocs.yml under the [Cmdlets] category. Or feel free to automate this part ;)"
    }
}

Process
{
    $cmdlets = Get-Command -Module $Module | ?{$_.Name -like $CmdletName} | select -ExpandProperty Name    

    if($YamlIndexOnly){
        Get-YamlCmdletsIndex
    }

    else{
        foreach ($cmdlet in $cmdlets){
            Write-Output "Procesing $cmdlet"
            
            $OutFile = Join-Path (Resolve-Path $Destination) ($cmdlet.ToLower() + ".md")

            $OutFileParameters = @{FilePath = $OutFile; Append = $true; Force = $true; Encoding = "utf8"}

            Write-Output "Sending output to $outfile"        

            Write-Verbose "Processing: $outfile"
                
            If (Test-Path $OutFile){ 
                Clear-Content $OutFile -Force
            }                

            $help = Get-Help $cmdlet
            "" | Out-File @OutFileParameters
            "### Summary`n" | Out-File @OutFileParameters
            #$null = "$($help.Synopsis)`n" | Out-File @OutFileParameters
            $help.Synopsis | Out-File @OutFileParameters

            if(![string]::IsNullOrEmpty($help.parameters)){

                "### Parameters" | Out-File @OutFileParameters

                "| Name | DataType          | Description |" | Out-File @OutFileParameters
                "| ------------- | ----------- | ----------- |" | Out-File @OutFileParameters
            
                foreach($parameter in $help.parameters.parameter){
                    if(($parameter.description.text.count -gt 1) -and ($parameter.description.text[-1].Contains("This is an alias"))){
                        continue
                    }

                    $parameter.description| clip.exe ; Get-Clipboard -OutVariable description | Out-Null

                    $type = Replace-TypeName -type $parameter.type.name

                    "| $($parameter.Name) | $type | $description |" | Out-File @OutFileParameters
                }

                "" | Out-File @OutFileParameters
            }
            
            "### Syntax" | Out-File @OutFileParameters
            
            #$help.syntax | clip.exe ; Get-Clipboard -OutVariable Syntax | Out-Null

            $Syntax = $help.syntax
            "`````` powershell" | Out-File @OutFileParameters
            $syntax | Out-File @OutFileParameters
            "`````` `n" | Out-File @OutFileParameters
            
            "### Examples `n" | Out-File @OutFileParameters
            foreach ($example in $help.examples.example){
                $title = $example.title.Replace("-","").trim().insert(0,"**").insert(11,"**")

                "$title`n" | Out-File @OutFileParameters
                "$($example.remarks.text)`n" | Out-File @OutFileParameters

                $code = $example.code.Replace('PS C:\> ',"")

                "`````` powershell `n $code`n`````` `n" | Out-File @OutFileParameters
            }        
            #>
            Write-Verbose "Finished processing: $outfile"
        }
    }    
}

End
{
        
}