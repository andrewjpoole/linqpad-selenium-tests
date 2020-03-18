# Create chromedriver folder if not exists
$chromeDriverDir = "c:\chromedriver\"
if(!(Test-Path -Path $chromeDriverDir )){
    New-Item -ItemType directory -Path $chromeDriverDir | Out-Null
}

# Get version of chrome...
$chromeVersion = (Get-Item (Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe').'(Default)').VersionInfo.ProductVersion
Write-Output "Currently installed chrome version: $chromeVersion"

# Get the major part...
$chromeMajor = $chromeVersion.Split(".")[0]

# Build path to look up correct chromedriver version for the chrome major version installed...
$baseUrl = "https://chromedriver.storage.googleapis.com/"
$latestVersionUrl = $baseUrl + "LATEST_RELEASE_" + $chromeMajor

# Get appropriate chromedriver version from ftp site...
$wc = New-Object System.Net.WebClient
Write-Output "Fetching latest chromedriver version for Chrome v$chromeMajor from: $latestVersionUrl"
$driverLatestVersion = $wc.DownloadString($latestVersionUrl)
Write-Output "The chromeDriver version we need is: $driverLatestVersion"

# Build path to appropriate chromedriver version and download...
$driverUrl = $baseUrl + $driverLatestVersion + "/chromedriver_win32.zip"
$outputZip = $chromeDriverDir + $driverLatestVersion + ".zip"
Write-Output "Fetching chromedriver.zip from: $driverUrl"
$wc.DownloadFile($driverUrl, $outputZip)

# remove the latest version directory
$chromeDriverLatestDir = "c:\chromedriver\latest"
if(Test-Path -Path $chromeDriverLatestDir ){
    Remove-Item –path $chromeDriverLatestDir –recurse
}

# Unzip the new chromedriver into latest...
Expand-Archive $outputZip -DestinationPath $chromeDriverLatestDir

# Remove the zip file...
Remove-Item -path $outputZip

Write-Output "Latest chromeDriver is now unzipped at: $chromeDriverLatestDir"
