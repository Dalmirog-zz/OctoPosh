<#
.Synopsis
   Sets Octopus SMTP Configuration
.DESCRIPTION
   Sets Octopus SMTP Configuration
.EXAMPLE
   Set-OctopusSMTPConfig -SMTPHost "SMTP.VdlyInd.com" -Port 25 -SendEmailFrom "Octopus@VandelayIndustries.com"
.EXAMPLE
   Set-OctopusSMTPConfig -SMTPHost "SMTP.VdlyInd.com" -Port 25 -SendEmailFrom "Octopus@VandelayIndustries.com" -SMTPLogin "Email@VandelayIndustries.com" -SMTPPassword "SomePassword"
.LINK
   Github project: https://github.com/Dalmirog/OctopusDeploy-Powershell-module
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

        # User to login to the SMPT service.
        [Parameter(Mandatory=$true, parametersetname='setlogin')]
        [string]$SMTPLogin = $null,

        # Password to login to the SMPT service.
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
        $r = Invoke-WebRequest "$env:OctopusURL/api/smtpconfiguration" -Method Put -Headers $c.header -Body $body

    }
    End
    {
        If ($r.statuscode -eq 200) {Return $true}

        else {Return $false}
    }
}