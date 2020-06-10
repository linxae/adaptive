function BuildFilePath([Parameter(Mandatory=$true)] $Resource)
{
    return "C:\github\linxae\adaptive\tests\.output\$($Resource.Type)_$($Resource.Id).json"
}

function BuildJobFilePath([Parameter(Mandatory=$true)] $Resource)
{
    return "C:\github\linxae\adaptive\tests\.output\$($Resource.Type)_$($Resource.Id)-job.json"
}

function Create([Parameter(Mandatory=$true)] $Resource)
{
    $path = (BuildFilePath $Resource)

    if($Resource.Data.name -and (Test-Path $path))
    {
        throw "Resource [$path] already exists. It cannot be created!"
    }

    $Resource.Data.name = $Resource.Data.zone.ToUpper().Substring(0,1) + [DateTime]::Now.Minute.ToString("d2") + [DateTime]::Now.Second.ToString("d2")
    $Resource.Id = $Resource.Data.name

    $job = $Resource.RequiredServices["AnsibleTower"].LaunchJobTemplate(7);
	
    Set-Content (BuildFilePath $Resource) -value (ConvertTo-Json $Resource)
    Set-Content (BuildJobFilePath $Resource) -value (ConvertTo-Json $job)

	return $Resource
}

function Read([Parameter(Mandatory=$true)] $Resource)
{
    $path = (BuildFilePath $Resource)

    if(!Test-Path $path)
    {
        return $null
    }

	return (Get-Content $path -Raw)
}

function Update([Parameter(Mandatory=$true)] $Resource)
{
    $path = (BuildFilePath $Resource)

    if(!Test-Path $path)
    {
        throw "Resource [$path] doen't exists. It cannot be updated!"
    }

	Set-Content $path -value (ConvertTo-Json $Resource)

	return $Resource
}

function Delete([Parameter(Mandatory=$true)] $Resource)
{
    $path = (BuildFilePath $Resource)

    if(!Test-Path $path)
    {
        throw "Resource [$path] doen't exists. It cannot be deleted!"
    }

	Remove-Item -Path (BuildFilePath $Resource)

	return $null
}
