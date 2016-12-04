param(
 [string]$Action,
 [string]$InstanceName
)

If(Test-Path HKLM:\SOFTWARE\Octopus\OctopusServer\){
    $OctopusInstallPath = (Get-ItemProperty HKLM:\SOFTWARE\Octopus\OctopusServer\).InstallLocation
    $OctopusServerexe = join-path $OctopusInstallPath "Octopus.Server.exe"
    $OctopusMigratorexe = join-path $OctopusInstallPath "Octopus.Migrator.exe"
}
else{
    Throw "VM [$($env:computername)] doesn't have the Octopus Server installed. It needs to be installed to run tests against it"
}

If([string]::IsNullOrEmpty($InstanceName)){    
    $InstanceName = "OctoposhTest"
}

$BackupPassword = "Octoposhtest1!"
$OctopusBackupDir = "Databackup\OctopusExport" #Relative to the location of the cake script, NOT to the location of this ps1 script.

switch ($Action)
{
    "StopService" {& $OctopusServerexe service --instance $InstanceName --stop}
    "StartService" {& $OctopusServerexe service --instance $InstanceName --start}
    "ImportBackup" {& $OctopusMigratorexe import --instance $InstanceName --directory $OctopusBackupDir --password $BackupPassword --overwrite --include-tasklogs}
    "ExportBackup" {& $OctopusMigratorexe export --instance $InstanceName --directory $OctopusBackupDir --password $BackupPassword}
    Default {"You need to either pass STOPSERVICE or STARTSERVICE to the ACTION parameter"}
}

