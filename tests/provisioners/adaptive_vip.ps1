class extra_vars 
{
    [String]$user_name

    extra_vars([String]$user_name)
    {
        $this.user_name = $user_name
    }
}

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

    $jobInput = [extra_vars]::New("$($Resource.Data.name)")
    $job = $Resource.RequiredServices["AnsibleTower"].RunJobTemplate(8, $jobInput);
    Set-Content (BuildJobFilePath $Resource) -value (ConvertTo-Json $job)

    if(!$job -or $job.Failed)
    {
        throw "Failed creating resource $($Resource.Data.name). $job.Description" 
    }
	
    Set-Content (BuildFilePath $Resource) -value (ConvertTo-Json $Resource)    

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
