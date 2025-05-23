################################################################################
# Copyright (C) 2018 e-UCM e-Learning group
#
# Licensed under the Apache License, Version 2.0 (the "License"); you may not
# use this file except in compliance with the License.
#
# You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
# WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
#
# See the License for the specific language governing permissions and
# limitations under the License.
################################################################################
$BASE_URL="http://netstorage.unity3d.com/unity"
$HASH="c24f30193bac"
$VERSION="2017.4.6f1"

$unitysetup = "UnitySetup64.exe"
$unitysetupargs = "/S"
$package = "Windows64EditorInstaller/UnitySetup64-$($VERSION).exe";

$url = "$BASE_URL/$HASH/$package"

Write-Output "Downloading from $($url): "
try {
    $downloadLog = & Invoke-WebRequest -o $unitysetup $url 2>&1
} 
catch
{
    Write-Error "Download failed!"
    Write-Error "Message: `r`n $($_.Exception.Message)"
    exit 1
}

Write-Output "Installing $($unitysetup)"
$process = Start-Process $unitysetup $unitysetupargs -Wait -PassThru
if ($process.ExitCode -ne 0)
{
    Write-Error "Installation failed! (Code $($process.ExitCode))"
    exit 1
}

Write-Output "Unzipping Unity License"
$zipLog = & 7z x .\scripts\license.7z -p"$($env:license_password)" 2>&1
if ($LastExitCode -ne 0)
{
    Write-Error "Unzip license failed! (Code $($LastExitCode))"
    Write-Output "Log: `r`n $zipLog"
    exit 1
}

$installLicenseLog = & .\install-license.ps1 2>&1
if ($LastExitCode  -ne 0)
{
    Write-Error "Install license failed! (Code $($LastExitCode))"
    Write-Output "Log: `r`n $installLicenseLog"
    exit $LastExitCode
}