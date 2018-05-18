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

project=$1
project_path=$(pwd)/src/$project
guid_path=$(pwd)/src/exchange/guidmap.csv
log_file=$(pwd)/build/unity-mac.log

error_code=0

echo "Creating GUIDMap for $project"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile "$log_file" \
  -projectPath "$project_path" \
  -quit \
  -executeMethod "uAdventure.Editor.FileId.GenerateGUIDMap" guid_path
if [ $? = 0 ] ; then
  echo "GUIDMap created at $guid_path."
  error_code=0
else
  echo "GUIDMap creation failed. Exited with $?."
  error_code=1
fi

echo 'Build logs:'
cat $log_file

echo "Finishing with code $error_code"
exit $error_code
