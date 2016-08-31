﻿<#
.Synopsis
   Gets information about the NuGet feeds registered in Octopus. This include both external and built-in
.DESCRIPTION
   Gets information about the NuGet feeds registered in Octopus
.EXAMPLE
   Get-OctopusFeed -FeedName "MyGet"

   Get a feed with a specific name
.EXAMPLE
   Get-OctopusFeed -URL "*Mycompany*"

   Get a feed with a specific string inside the URL
.LINK
   WebSite: http://Octoposh.net
   Github project: https://github.com/Dalmirog/Octoposh
   Wiki: https://github.com/Dalmirog/OctoPosh/wiki
   QA and Cmdlet request: https://gitter.im/Dalmirog/OctoPosh#initial
#>
function Get-OctopusFeed
{
    [CmdletBinding()]
    Param
    (
        # Name of the feed
        [Alias("name")]
        [string[]]$FeedName,

        # URL of the feed
        [Alias("URI","FeedURI")]
        [string[]]$URL,
        
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
        If(([string]::IsNullOrEmpty($FeedName)) -and ([String]::IsNullOrEmpty($URL))){
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting all feeds"            
            $feeds = $c.repository.Feeds.FindAll()
        }
        else{
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting feeds by`nName: $feedname`nURL: $URL"  
            $feeds = $c.repository.Feeds.FindMany({param($f)if(($f.name -eq $FeedName) -or ($f.name -like $FeedName) -or ($f.feedURI -eq $URL) -or ($f.feedURI -like $URL))`
            
            {$true}})
        }

        Write-Verbose "[$($MyInvocation.MyCommand)] Feeds found: $($feeds.count)"

        If($ResourceOnly){
            $list += $Machines
        }

        foreach($feed in $feeds){

            Write-Progress -Activity "Getting info from feed: $($feed.name)" -status "$i of $($feeds.count)" -percentComplete ([Double]$i / $feeds.count*100)
            Write-Verbose "[$($MyInvocation.MyCommand)] Getting info from feed $($feed.name)"

            $obj = [PSCustomObject]@{
                Name = $feed.name
                FeedURI = $feed.FeedURI
                LoginUser= $feed.Username                
                ID = $Feed.id
                Resource = $feed
            }
            
            $list+=$obj
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