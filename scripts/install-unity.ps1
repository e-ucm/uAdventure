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
$HASH="21ae32b5a9cb"
$VERSION="2017.4.3f1"

$unitysetup = "UnitySetup64.exe"
$unitysetupargs = "/S"
$package = "Windows64EditorInstaller/UnitySetup64-$($VERSION).exe";

$url = "$BASE_URL/$HASH/$package"

Write-Output "Downloading from $($url): "
curl -o $unitysetup $url

Write-Output "Installing $($unitysetup)"
Start-Process $unitysetup $unitysetupargs -Wait 

Start-Process "C:\\Program Files\\Unity\\Editor\\Unity.exe" -batchmode -nographics -username $Evn:license_username -password $Evn:license_password -quit
