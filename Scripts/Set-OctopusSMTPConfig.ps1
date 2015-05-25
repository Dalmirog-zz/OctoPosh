<#
.Synopsis
   Sets Octopus SMTP Configuration. This configuration will be used by the deployment process step "Email Message"
.DESCRIPTION
   Description.
.EXAMPLE
   Set-OctopusSMTPConfig -SMTPHost "SMTP.VdlyInd.com" -Port 25 -SendEmailFrom "Octopus@VandelayIndustries.com"

   Set the Octopus SMTP config
.EXAMPLE
   Set-OctopusSMTPConfig -SMTPHost "SMTP.VdlyInd.com" -Port 25 -SendEmailFrom "Octopus@VandelayIndustries.com" -SMTPLogin "Email@VandelayIndustries.com" -SMTPPassword "SomePassword"

   Set the Octopus SMTP config with a User and Passworrd
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Advanced Cmdlet Usage: https://github.com/Dalmirog/OctoPosh/wiki/Advanced-Examples
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Set-OctopusSMTPConfig
{
    [CmdletBinding(DefaultParameterSetName="All")] 
    
    Param
    (
        # SMTP server
        [Parameter(Mandatory=$true)]
        [string]$SMTPHost,

        # SMTP Port
        [Parameter(Mandatory=$true)]
        [int]$Port,

        # Address from which emails will be sent
        [Parameter(Mandatory=$true)]
        [string]$SendEmailFrom,

        # SSL Enabled/Disabled
        [switch]$SSL,        

        # User to login to the SMPT service
        [Parameter(Mandatory=$true, parametersetname='setlogin')]
        [string]$SMTPLogin = $null,

        # Password to login to the SMPT service
        [Parameter(Mandatory=$true,parametersetname='setlogin')]
        [string]$SMTPPassword = $null

    )

    Begin
    {
        $c = New-OctopusConnection

        If($SSL){$EnableSSL = "true"}
        else{$EnableSSL = "false"}

        $body = @{
            SMTPHost = $SMTPHost
            SMTPPort = $Port
            SendEmailFrom = $SendEmailFrom
            EnableSSL = $EnableSSL
            SMTPLogin = $SMTPLogin
            NewSMTPPassword = $SMTPPassword
        } | ConvertTo-Json

    }
    Process
    {
        
        Try{
            Write-Verbose "[$($MyInvocation.MyCommand)] Setting SMTP configuration for $env:OctopusURL"   
            $r = Invoke-WebRequest "$env:OctopusURL/api/smtpconfiguration" -Method Put -Headers $c.header -Body $body -UseBasicParsing
        }
        Catch{
            write-error $_
        }  

    }
    End
    {
        Write-Verbose "[$($MyInvocation.MyCommand)] HTTP request to set SMTP configuration returned code $($r.statuscode)"
        if($r.statuscode -eq 200){
            Return $True
        }
        Else{
            Return $false
        }
    }
}