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

# Core and Runner File
Copy-Item "$($build_folder)\\uAdventureScripts.dll" -Destination "$($assets_folder)\\Plugins\\uAdventureScripts.dll"
# Plugins Folder
Copy-Item -Path "$($project_path)\\Plugins" -Destination "$($assets_folder)\\" -Recurse -Force
# Animations Folder
Copy-Item -Path "$($project_path)\\Animations" -Destination "$($assets_folder)\\" -Recurse -Force
# Materials Folder
Copy-Item -Path "$($project_path)\\Materials" -Destination "$($assets_folder)\\" -Recurse -Force
# Shaders Folder
Copy-Item -Path "$($project_path)\\Shaders" -Destination "$($assets_folder)\\" -Recurse -Force
# Prefabs Folder
Copy-Item -Path "$($project_path)\\Prefabs" -Destination "$($assets_folder)\\" -Recurse -Force
# Scenes Folder
Copy-Item -Path "$($project_path)\\Scenes" -Destination "$($assets_folder)\\" -Recurse -Force
# Resources Folder
Copy-Item -Path "$($project_path)\\Resources" -Destination "$($assets_folder)\\" -Recurse -Force
# Editor File
Copy-Item "$($build_folder)\\uAdventureEditor.dll" -Destination "$($assets_folder)\\Editor\\Plugins\\uAdventureEditor.dll"
# Editor Plugins Folder
Copy-Item -Path "$($project_path)\\Editor\\Plugins" -Destination "$($assets_folder)\\Editor\\" -Recurse -Force
# Editor resources Folder
Copy-Item -Path "$($project_path)\\Editor\\Resources" -Destination "$($assets_folder)\\Editor\\" -Recurse -Force