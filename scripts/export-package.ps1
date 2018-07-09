################################################################################
# Copyright (C) 2018 e-UCM Learning Group
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

$project = $($args[0])
$package = $($args[1])

$project_path = "$($pwd)"
$package_project_path = "$($project_path)\BuildProject\uAdventurePackage\uAdventurePackage"
$export_directory="$($pwd)\\Build"
$log_file = "$($export_directory)\\unity.log"

If (!$package) 
{
  $export_path="$($export_directory)/$($project).unitypackage"
  $asset_path="Assets/$($project)"
}
Else 
{
  $export_path="$($export_directory)/$($project).$($package).unitypackage"
  $asset_path="Assets/$($package)"
}

Write-Output "Creating Export dir"
New-Item -ItemType directory -Path $export_directory

$unity = "C:\\Program Files\\Unity\\Editor\\Unity.exe"
$arguments = "-batchmode -force-free -nographics -silent-crashes -logFile $($log_file) -projectPath $($package_project_path) -quit -exportPackage ""$($asset_path)"" ""$($export_path)"""

Write-Output "Creating package for $($asset_path)"
$process = Start-Process $unity $arguments -Wait -PassThru

$logContent = Get-Content $log_file
Write-Output "Logs from build: `r`n $($logContent)"
    
If ( $process.ExitCode -eq 0 ) 
{
    If ( $logContent -contains "*Scripts have compiler errors.*") 
    {
        Write-Error "Scripts have compiler errors!"
        exit 1
    }
    Else 
    {
        Write-Output "Created package successfully at $($export_path)."
    }
}
Else 
{
    Write-Error "Creating package failed. Exited with $($process.ExitCode)."
    exit $process.ExitCode
}

Write-Output 'Export dir:'
Get-ChildItem $export_directory

if (-not (Test-Path $export_path))
{
    Write-Error "Package not found at $($export_path)."
    exit 1
}

exit 0