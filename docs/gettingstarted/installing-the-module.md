## From PowershellGallery (recommended)

1 - Open Powershell as administrator and run 
```powershell
Install-Module -Name Octoposh -force
```
The `Install-Module` cmdlet is part of the [Windows Management Framework 5.0 Preview](http://go.microsoft.com/fwlink/?LinkId=398175)

2 - Run 
```powershell
Import-Module Octoposh
```

3 - From the same console run ```Get-command -module OctoPosh``` to list all the module's cmdlets

![](http://s6.postimg.org/ssrvnf8jl/powershell.jpg)

## From Github

1 - Download the module from the [Github project page](https://github.com/Dalmirog/OctoPosh)

![](http://s6.postimg.org/3yuw6opoh/download.jpg)


2 - Right click on the downloaded zip -> **Properties -> **Unblock File**

![](http://s6.postimg.org/lllldt88x/unblock.jpg)

3 -  Create a folder called *OctoPosh* under your [PSModulePath](https://msdn.microsoft.com/en-us/library/dd878326%28v=vs.85%29.aspx). Use *C:\Program Files\WindowsPowerShell\Modules* if its on the list of Module Paths.

4 - Extract the contents of the zip on the new *OctoPosh* folder

![](http://s6.postimg.org/5q1co9729/folder.jpg)

5 - Open a Powershell console as administrator and run ```Import-Module Octoposh```

6 - From the same console run ```Get-command -module OctoPosh``` to list all the module's cmdlets

![](http://s6.postimg.org/ssrvnf8jl/powershell.jpg)