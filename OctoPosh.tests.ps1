#Generates a random test name that'll be used to name everything on the tests
Function New-TestName {    
    
    $length = 10 #length of random chars
    $characters = 'abcdefghkmnprstuvwxyzABCDEFGHKLMNPRSTUVWXYZ1234567890' #characters to use
    
    # select random characters
    $random = 1..$length | ForEach-Object { Get-Random -Maximum $characters.length }
        
    #Set ofs to "" to avoid having spaces between each char
    $private:ofs=''

    #output prefix (max 10 chars) + 5 random chars
    Return [String]($prefix + $characters[$random])

}

Describe 'Octopus Module Tests' {

    $TestName = new-testname

    $c = New-OctopusConnection
    
    It '[New-OctopusResource] adds NuGet feeds'{
        $Feedname = $testname
        $feedURL = "https://$testname.com"

        $feed = Get-OctopusResourceModel -Resource NugetFeed

        $feed.Name = $Feedname
        $feed.FeedUri = $feedURL

        $newfeed = New-OctopusResource -Resource $feed

        $newfeed.name | should be $testname 
        $newfeed.feeduri | should be $feedURL
    }
    It '[New-OctopusResource] creates Library Variable Sets'{
        $libraryName = $testname
        $library = Get-OctopusResourceModel -Resource LibraryVariableSet

        $library.Name = $libraryName

        $NewLibrary = New-OctopusResource -Resource $library

        $NewLibrary.name | should be $testname         
    }

    It '[Remove-OctopusResource] deletes NuGet feeds'{
        (Get-OctopusFeed -FeedName $TestName | Remove-OctopusResource -Force) | should be $true

        
    }
    It '[Remove-OctopusResource] deletes Library Variable Sets'{
        (Get-OctopusVariableSet -LibrarySetName $TestName | Remove-OctopusResource -Force) | should be $true
    }
}
