function BuildFilePath([Parameter(Mandatory=$true)] $Resource)
{
    return "C:\github\linxae\adaptive\tests\.output\$($Resource.Type)_$($Resource.Id).json"
}

function Create([Parameter(Mandatory=$true)] $Resource)
{
    $path = (BuildFilePath $Resource)

    if($Resource.Data.name -and (Test-Path $path))
    {
        throw "Resource [$path] already exists. It cannot be created!"
    }

	$Resource.Data.be_ip = "10.0.1.$([Int]::Parse($Resource.Data.instance) +  25)"
	$Resource.Data.fe_ip = "192.168.0.$([Int]::Parse($Resource.Data.instance) +  12)"

    $Resource.Data.name = "$($Resource.Data.domain.ToUpper().Substring(0,1))$($Resource.Data.app)$($Resource.Data.model.ToUpper().Substring(0,1))$([Int]::Parse($Resource.Data.instance).ToString("d2"))"

	$Resource.Id = $Resource.Data.name

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
