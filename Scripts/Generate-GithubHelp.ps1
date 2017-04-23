[CmdletBinding()]
Param
(
    # ModuleName    
    [string]$Module = "Octoposh",

    # CmdletName = 
    [string]$CmdletName = "*",

    # Destionation directory
    [string]$Destination = "..\docs\Cmdlets"
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
}

Process
{
   $cmdlets = Get-Command -Module $Module | ?{$_.Name -like $CmdletName} | select -ExpandProperty Name    

    foreach ($cmdlet in $cmdlets){
        Write-Output "Procesing $cmdlet"
        $OutFile = Join-Path (Resolve-Path $Destination) ($cmdlet + ".md")

        $OutFileParameters = @{FilePath = $OutFile; Append = $true; Force = $true; Encoding = "utf8"}

        Write-Output "Sending output to $outfile"        

        Write-Verbose "Processing: $outfile"
            
        If (Test-Path $OutFile){ 
            Clear-Content $OutFile -Force
        }                

        $help = Get-Help $cmdlet

        "### Summary" | Out-File @OutFileParameters
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
        
        "### Examples" | Out-File @OutFileParameters
        foreach ($example in $help.examples.example){
            #$title = $example.title.Replace("-","").trim().insert(0,"**").insert(11,"**")

            #"$title`n" | Out-File @OutFileParameters
            "$($example.remarks.text)`n" | Out-File @OutFileParameters

            $code = $example.code.Replace('PS C:\> ',"")

            "`````` powershell `n $code`n`````` `n" | Out-File @OutFileParameters
        }        
        #>
        Write-Verbose "Finished processing: $outfile"
    }
    
}

End
{
        
}