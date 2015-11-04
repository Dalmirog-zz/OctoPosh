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

    It '[New-OctopusResource] creates Project Groups'{
        $Pg = Get-OctopusResourceModel -Resource ProjectGroup
                                                
        $Pg.Name = $testname

        $Pgobj = New-OctopusResource -Resource $Pg

        $Pgobj.name | should be $testname
    }
    It '[New-OctopusResource] creates Projects'{
        $Proj = Get-OctopusResourceModel -Resource Project
                
        $Proj.Name = $testname
        $Proj.ProjectGroupId = (Get-OctopusProjectGroup -Name $TestName).id
        $Proj.LifecycleId = (Get-OctopusLifeCycle)[0].id

        $Projobj = New-OctopusResource -Resource $Proj

        $Projobj.Name | should be $testname
    }

    It 'Creating releases for tests'{

        for($i = 0 ;$i -lt 31 ; $i++){
            cd $PSScriptRoot
            & ".\tools\Octo.exe" create-release --server=$env:OctopusURL --apikey=$env:OctopusAPIKey --project=$testname
        }

        1 | should not be $null

    }

    It '[Get-OctopusRelease] Gets latest X releases of a project'{
        #This uses a hardcoded project with more than 30 releases
        $latest = Get-Random -Minimum 1 -Maximum 30
        $releases = Get-OctopusRelease -ProjectName $TestName -Latest $latest -resourceonly

        $releases.count | should be $latest
    }
    It '[Get-OctopusRelease] [latest] cant get amount of release out of range'{
        {Get-OctopusRelease -ProjectName $TestName -Latest -1} | should throw

        {Get-OctopusRelease -ProjectName $TestName -Latest 31} | should throw
    }

    It '[Get-OctopusRelease] Gets a single release by release version'{
        $release = Get-OctopusRelease -ProjectName $TestName -resourceonly -Latest 1

        (Get-OctopusRelease -ProjectName $TestName -ReleaseVersion $release.version).ReleaseVersion| should be $release.Version
    }
    It '[Get-OctopusRelease] Gets a releases by multiple release versions'{
        $max = 10
        $rel1 = Get-Random -Minimum 1 -Maximum $max
        do{
            $rel2 = Get-Random -Minimum 1 -Maximum $max
        }until($rel2 -ne $rel1)

        $AllReleases = Get-OctopusRelease -ProjectName $TestName -ResourceOnly -Latest $max

        $releases = Get-OctopusRelease -ProjectName $TestName -resourceonly -ReleaseVersion $AllReleases[$rel1].version,$AllReleases[$rel2].version

        $releases.count | should be 2
        
    }#>
    It '[Remove-OctopusResource] deletes Projects'{
        (Get-OctopusProject -Name $TestName | Remove-OctopusResource -Force) | should be $true

        Get-OctopusProject -Name $TestName -ErrorAction SilentlyContinue| should be $null
    }
    It '[Remove-OctopusResource] deletes Project Groups'{
        (Get-OctopusProjectGroup -Name $TestName |Remove-OctopusResource -Force) | should be $true

        Get-OctopusProjectGroup -Name $TestName -ErrorAction SilentlyContinue | should be $null
    }   

}