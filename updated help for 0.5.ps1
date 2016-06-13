#region Listening Machine

#Create an instance of a Machine Object
$machine = Get-OctopusResourceModel -Resource Machine

#Get Environment where you'll be adding the machine
$environment = Get-OctopusEnvironment -EnvironmentName Development -ResourceOnly

#Add Mandatory properties to machine object        
$machine.name = "whatevermachine" #Display name of the machine
$machine.EnvironmentIds.Add($environment.id) #Adding ID(s) of the Environment(s).
$machine.Roles.Add("WebServer") #Adding Role(s)

$machine.CommunicationStyle = "TentaclePassive" #Communication style
$machine.Uri = "https://MachineURL:10933" #Machine URL and port
$machine.Thumbprint = "8A7E6157A34158EDA1B5127CB027B2A267760A4F" #Machine Thumbprint

#Creating the machine
New-OctopusResource -Resource $machine

#endregion 


