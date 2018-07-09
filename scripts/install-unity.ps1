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
Invoke-WebRequest -o $unitysetup $url 2>&1 > $downloadLog
if ($LastExitCode -ne 0)
{
    Write-Output "Download failed! (Code $($LastExitCode))"
    Write-Output "Log: \n $downloadLog"
    exit $LastExitCode
}

Write-Output "Installing $($unitysetup)"
$process = Start-Process $unitysetup $unitysetupargs -Wait -PassThru
if ($process.ExitCode -ne 0)
{
    Write-Error "Installation failed! (Code $($process.ExitCode))"
    exit $process.ExitCode
}

7z x .\scripts\license.7z -p"$($env:license_password)" 2>&1 > $zipLog
if ($LastExitCode  -ne 0)
{
    Write-Error "Unzip license failed! (Code $($LastExitCode))"
    Write-Output "Log: \n $zipLog"
    exit $LastExitCode
}

.\install-license.ps1 2>&1 > $installLicenseLog
if ($LastExitCode  -ne 0)
{
    Write-Error "Install license failed! (Code $($LastExitCode))"
    Write-Output "Log: \n $installLicenseLog"
    exit $LastExitCode
}