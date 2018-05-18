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
version=$2
package_path=$(pwd)/current-package/$project.unitypackage
release_directory=$(pwd)/release
release_path=$release_directory/$project.$version.unitypackage

mkdir -p $release_directory

echo "Preparing release version $version."
cp "$package_path" "$release_path"
