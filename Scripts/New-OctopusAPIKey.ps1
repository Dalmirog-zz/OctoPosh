<#
.Synopsis
   Creates an an API Key for a user using the Octopus Deploy credentials.

   The fact that this command has to be run manually from a console is as design. API Keys should be created only once, stored in a safe place and then reused.

   API keys can be used to authenticate with the Octopus Deploy REST API in place of a username and password. Using API keys lets you keep your username and password secret, but the API key itself is still sensitive information that needs to be protected
.DESCRIPTION
   Creates an an API Key for a user using the Octopus Deploy credentials.

   The fact that this command has to be run manually from a console is as design. API Keys should be created only once, stored in a safe place and then reused.

   API keys can be used to authenticate with the Octopus Deploy REST API in place of a username and password. Using API keys lets you keep your username and password secret, but the API key itself is still sensitive information that needs to be protected
.EXAMPLE
   New-OctopusAPIKey -Purpose "Scripting" -Username Dalmiro.Granas -Password "Pa$$w0rD" -OctopusURL "https://MyOctopusInstance.com"

   Create a new API Key for a user
.LINK
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function New-OctopusAPIKey
{
    [CmdletBinding()]
    Param
    (        
        # Purpose of the API Key
        [Parameter(Mandatory=$true)]
        [string]$Purpose,

        # Octopus login Username
        [Parameter(Mandatory=$true)]
        [string]$Username,
        
        # Octopus instance URL
        [Parameter(Mandatory=$true)]
        [alias("URL")]
        [string]$OctopusURL,

        # Octopus login Password
        [Parameter(Mandatory=$false)]
        $password,

        # Supresses warning when running command
        [switch]$NoWarning
    )

    Begin
    {
        $LoginObj = New-Object Octopus.Client.Model.LoginCommand
    }
    Process
    {        
        $LoginObj.Username = $Username
        
        if(!($password)){        
            $password = Read-Host "Password" -AsSecureString 

            $password = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))
        }

        $LoginObj.Password = $Password
        
        $endpoint = new-object Octopus.Client.OctopusServerEndpoint "$($OctopusURL)"    

        $repository = new-object Octopus.Client.OctopusRepository $endpoint

        Write-Verbose "[$($MyInvocation.MyCommand)] Logging in with user: $Username"

        $repository.Users.SignIn($LoginObj)

        $user = $repository.Users.GetCurrent()
        
        Write-Verbose "[$($MyInvocation.MyCommand)] Creating API key for user: $Username"

        Try{
            $APIKey = $repository.Users.CreateApiKey($user,$Purpose)
        }
        Catch{
            Throw $_
        }
        IF($APIKey -ne $null){
            Write-Verbose "[$($MyInvocation.MyCommand)] API Key Created"
        }
    }
    End
    {
        If(!($NoWarning)){
            Write-warning "API keys cannot be retrieved once they are created. Make sure you save this key in a safe place like a password management tool."
        }

        Return $APIKey

    }
}