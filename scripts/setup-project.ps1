################################################################################
# Copyright (C) 2018 The e-UCM Learning Group
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

$project_path = "$($pwd)\\Assets\\uAdventure"
$assets_folder = "$($pwd)\\BuildProject\\\uAdventurePackage\\\uAdventurePackage\\Assets\\uAdventure"
$build_folder = "$($pwd)\\BuildProject\\uAdventureEditor\\uAdventureEditor\\bin\\Release"

function Resolve-Error 
{
    Param ([int]$code, [string]$errorLog)

    if ($code -ne 0) 
    {
        Write-Error "Error ($($code)) setting up the project: `r`n $errorLog"
        exit $code
    }
}

New-Item -ItemType directory -Path "$($assets_folder)" -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
New-Item -ItemType directory -Path "$($assets_folder)\\Plugins" -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
New-Item -ItemType directory -Path "$($assets_folder)\\Editor" -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
New-Item -ItemType directory -Path "$($assets_folder)\\Editor\\Plugins" -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog

# Plugins Folder
Copy-Item -Path "$($project_path)\\Plugins" -Destination "$($assets_folder)\\" -Recurse -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
# Tracker Folder
Remove-Item -Path "$($assets_folder)\\Plugins\\unity-tracker" -Recurse -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
# Replace Tracker folder and files
New-Item -ItemType directory -Path "$($assets_folder)\\Plugins\\unity-tracker" -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
Copy-Item "$($build_folder)\\UnityTracker.dll" -Destination "$($assets_folder)\\Plugins\\unity-tracker\\" -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
Copy-Item "$($project_path)\\Plugins\\unity-tracker\\Tracker.prefab" -Destination "$($assets_folder)\\Plugins\\unity-tracker\\" -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
Copy-Item "$($project_path)\\Plugins\\unity-tracker\\Tracker.prefab.meta" -Destination "$($assets_folder)\\Plugins\\unity-tracker\\" -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
# Core and Runner File
Copy-Item "$($build_folder)\\uAdventureScripts.dll" -Destination "$($assets_folder)\\Plugins\\uAdventureScripts.dll" 2> $errorLog
Resolve-Error $LastExitCode $errorLog
# Animations Folder
Copy-Item -Path "$($project_path)\\Animations" -Destination "$($assets_folder)\\" -Recurse -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
# Materials Folder
Copy-Item -Path "$($project_path)\\Materials" -Destination "$($assets_folder)\\" -Recurse -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
# Shaders Folder
Copy-Item -Path "$($project_path)\\Shaders" -Destination "$($assets_folder)\\" -Recurse -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
# Prefabs Folder
Copy-Item -Path "$($project_path)\\Prefabs" -Destination "$($assets_folder)\\" -Recurse -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
# Scenes Folder
Copy-Item -Path "$($project_path)\\Scenes" -Destination "$($assets_folder)\\" -Recurse -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
# Resources Folder
Copy-Item -Path "$($project_path)\\Resources" -Destination "$($assets_folder)\\" -Recurse -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
# Editor Plugins Folder
Copy-Item -Path "$($project_path)\\Editor\\Plugins" -Destination "$($assets_folder)\\Editor\\" -Recurse -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
# Editor File
Copy-Item "$($build_folder)\\uAdventureEditor.dll" -Destination "$($assets_folder)\\Editor\\Plugins\\uAdventureEditor.dll" 2> $errorLog
Resolve-Error $LastExitCode $errorLog
# Editor resources Folder
Copy-Item -Path "$($project_path)\\Editor\\Resources" -Destination "$($assets_folder)\\Editor\\" -Recurse -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog
# Editor Layouts Folder
Copy-Item -Path "$($project_path)\\Editor\\Layouts" -Destination "$($assets_folder)\\Editor\\" -Recurse -Force 2> $errorLog
Resolve-Error $LastExitCode $errorLog

Write-Output "Setup project success."
exit 0
