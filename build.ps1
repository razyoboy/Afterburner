# Variables
$projectPath = "Afterburner/Afterburner.csproj"
$runtime = "win-x64"
$framework = "net9.0"
$configuration = "Release"
$output = "Afterburner/publish"
$toolsDir = "C:\tools"

# Clean previous publish output (optional, remove if you want to keep other files)
if (Test-Path $output) {
    Remove-Item -Recurse -Force $output
}

# Publish directly into Afterburner/publish
dotnet publish $projectPath `
    --configuration $configuration `
    --runtime $runtime `
    --framework $framework `
    --self-contained false `
    /p:PublishSingleFile=true `
    --output $output

# Create tools directory if it doesn't exist
if (-not (Test-Path $toolsDir)) {
    New-Item -ItemType Directory -Path $toolsDir | Out-Null
}

# Copy published single-file executable to C:\tools
$exePath = Get-ChildItem "$output\*.exe" | Select-Object -First 1
if ($exePath) {
    Copy-Item $exePath.FullName -Destination $toolsDir -Force
    Write-Host "Copied $($exePath.Name) to $toolsDir"
} else {
    Write-Warning "No .exe file found in publish output!"
}
