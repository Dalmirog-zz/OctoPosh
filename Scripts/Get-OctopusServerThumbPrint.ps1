<#
.Synopsis
   Gets the Octopus Server thumbprint. Admin access in Octopus might be needed for this cmdlet to work.
.DESCRIPTION
   Gets the Octopus Server thumbprint. Admin access in Octopus might be needed for this cmdlet to work.
.EXAMPLE
   Get-OctopusServerThumbPrint

   Gets the thumbprint of the Octopus Server
.LINK
   WebSite: http://Octoposh.net
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusServerThumbPrint
{
    Begin
    {
        #$env:OctopusAPIKey = "whatever"
        $c = New-OctopusConnection
    }
    Process
    {
        Try{            
            $thumbprint = (get-octopusresource -uri "api/certificates/certificate-global").thumbprint
        }
        Catch{
            If($_.Exception.Response.StatusCode -eq "Unauthorized"){
                Throw "The remote server returned an error: (401) Unauthorized. This means that the API key used to authenticate against the Octopus server doesn't have enough permissions on the instance to get the Thumbprint. The API Key needs to belong to Administrator in Octopus to get this information. `n To check which API/Octopus URL you are currently using, use Get-OctopusConnectionInfo"
            }
            else{
                Throw $_
            }
        }
    }
    End
    {
       return $thumbprint
    }
}