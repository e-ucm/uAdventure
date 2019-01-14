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

$project_path = "$($pwd)\BuildProject\uAdventurePackage\uAdventurePackage"
$exchange_folder = "$($pwd)\Exchange"
$test_file = "$($exchange_folder)\testresults.xml"
$log_file = "$($exchange_folder)\\unity.log"

$unity = "C:\\Program Files\\Unity\\Editor\\Unity.exe"
$arguments = "-batchmode -force-free -nographics -silent-crashes -logFile $($log_file) -projectPath $($project_path) -runEditorTests -editorTestsResultFile $($test_file)"

Write-Output "Running tests in $($project_path)"
$process = Start-Process $unity $arguments -Wait -PassThru

$logContent = Get-Content $log_file
Write-Output "Logs from tests: `r`n $($logContent)"

If ( $process.ExitCode -eq 0 ) 
{
    If ( $logContent -contains "*Scripts have compiler errors.*") 
    {
        Write-Error "Scripts have compiler errors!"
        exit 1
    }
    Else 
    {
        Write-Output "Test successfully executed."
    }
}
Else 
{
    Write-Error "Tests not passed. Exited with $($process.ExitCode)."
}

Write-Output 'Exhange dir:'
Get-ChildItem $exchange_folder

if (-not (Test-Path $test_file))
{
    Write-Error "Tests result not found at $($test_file)."
    exit 1
}

# upload results to AppVeyor
Write-Output "Uploading test results to AppVeyor... (APPVEYOR JOB ID: $($env:APPVEYOR_JOB_ID)"
try {

    $multipartContent = [System.Net.Http.MultipartFormDataContent]::new()
    $multipartFile = (Resolve-Path ".\Exchange\testresults.xml")
    $FileStream = [System.IO.FileStream]::new($multipartFile, [System.IO.FileMode]::Open)
    $fileHeader = [System.Net.Http.Headers.ContentDispositionHeaderValue]::new("form-data")
    $fileHeader.Name = "fileName"
    $fileHeader.FileName = 'testresults.xml'
    $fileContent = [System.Net.Http.StreamContent]::new($FileStream)
    $fileContent.Headers.ContentDisposition = $fileHeader
    $fileContent.Headers.ContentType = [System.Net.Http.Headers.MediaTypeHeaderValue]::Parse("text/xml")
    $multipartContent.Add($fileContent)
    
    $Response = Invoke-WebRequest -Uri "https://ci.appveyor.com/api/testresults/nunit3/$($env:APPVEYOR_JOB_ID)" -Body $multipartContent -Method 'POST'

    if ($?) {
        Write-Output 'Success!'
        Write-Output "Remote Response: $($response.Content)"
    } 
    else {
        Write-Error "Error! (Code $($LastExitCode)"
        exit 1
    }
}
catch {
    Write-Error $error.exception.message
    exit 1
}

exit 0