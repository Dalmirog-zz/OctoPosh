<#
This file will get executed when a debug build executed using F5
#>

import-module .\Octoposh -verbose 

Set-OctopusConnectionInfo -apikey 'API-B3ZK7BTFAKSKRTCHQFKAZNPT5Y' -server 'http://devbox:82'

Set-OctopusToolsFolder -path C:\Tools\OctoTools