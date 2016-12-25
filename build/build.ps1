param (
	[string]$BuildVersionNumber=$(throw "-BuildVersionNumber is required."),
	[string]$TagVersionNumber
)

if ($TagVersionNumber){
	Write-Host "TagVersion" + $TagVersionNumber
}
else{
	Write-Host "Version: " + $BuildVersionNumber
}
Get-ChildItem -Path $PSScriptRoot\..\src -Filter project.json -Recurse | ForEach-Object{ 
    $ProjectJsonPath =  $_.FullName
    if ($TagVersionNumber){
        (gc -Path $ProjectJsonPath) `
	        -replace "(?<=`"version`":\s`")[.\w-]*(?=`",)", "$TagVersionNumber" |
	            sc -Path $ProjectJsonPath -Encoding UTF8
    }
    else{
        (gc -Path $ProjectJsonPath) `
	        -replace "(?<=`"version`":\s`")[.\w-]*(?=`",)", "$BuildVersionNumber" |
	            sc -Path $ProjectJsonPath -Encoding UTF8
    }
}
dotnet pack $PSScriptRoot\..\src\DavisVantage.WeatherReader