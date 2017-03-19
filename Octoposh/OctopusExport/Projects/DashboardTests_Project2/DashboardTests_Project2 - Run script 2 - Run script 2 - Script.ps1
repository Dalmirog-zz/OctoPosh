if("Prod" -eq $OctopusParameters['Octopus.Environment.Name']){
    
    throw "Fail on purpose!"
}

$stepName = $OctopusParameters['Octopus.Step.Name']
$targetName = $OctopusParameters['Octopus.Action.TargetRoles']

"Hello from step [$StepName] on target [$target]"