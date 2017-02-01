if("Dev" -eq $OctopusParameters['Octopus.Environment.Name']){
    
    throw 'failing to create test data'
}