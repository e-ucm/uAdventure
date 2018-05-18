#! /bin/sh
################################################################################
# Copyright (C) 2016 Andrew Lord
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

company=$1
project=$2
package=$3
project_path=$(pwd)/src/$project
log_file=$(pwd)/build/unity-mac.log
export_directory=$(pwd)/current-package
if [ -z "$package" ]; then
  export_path=$export_directory/$project.unitypackage
  asset_path="Assets/$company/$project"
else
  export_path=$export_directory/$project.$package.unitypackage
  asset_path="Assets/$company/$project/$package"
fi
error_code=0

mkdir -p $export_directory

echo "Creating package."
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile "$log_file" \
  -projectPath "$project_path" \
  -exportPackage "$asset_path" "$export_path" \
  -quit
if [ $? = 0 ] ; then
  echo "Created package successfully."
  error_code=0
else
  echo "Creating package failed. Exited with $?."
  error_code=1
fi

echo 'Build logs:'
cat $log_file

echo "Finishing with code $error_code"
exit $error_code
