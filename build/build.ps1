param (
	[string]$BuildVersionNumber=$(throw "-BuildVersionNumber is required.")
)
Get-ChildItem -Path $PSScriptRoot\..\src -Filter project.json -Recurse | ForEach-Object{ 
    $ProjectJsonPath =  $_.FullName
    (gc -Path $ProjectJsonPath) `
	    -replace "(?<=`"version`":\s`")[.\w-]*(?=`",)", "$BuildVersionNumber" |
	        sc -Path $ProjectJsonPath -Encoding UTF8
}
dotnet restore "src\DavisVantage.WeatherReader"
dotnet pack "src\DavisVantage.WeatherReader" --version-suffix $BuildVersionNumber