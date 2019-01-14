################################################################################
# Copyright (C) 2018 The e-UCM Learning Group
#
# Original code from: https://gist.github.com/jrreed/f42938492dcf77fe1a4e43ecf2d62bc3
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

$XSLFileName = Resolve-Path ".\scripts\fix-unity-test-results.xslt"
$XMLFileName = Resolve-Path ".\Exchange\testresults.xml"
$OutPutFileName = "$(Resolve-Path ".\Exchange\")nunit3testresults.xml"

$XSLInputElement = New-Object System.Xml.Xsl.XslCompiledTransform;
$XSLInputElement.Load($XSLFileName)
$XSLInputElement.Transform($XMLFileName, $OutPutFileName)

# upload results to AppVeyor
Write-Output 'Uploading test results to AppVeyor...'
try {
    $wc = New-Object 'System.Net.WebClient'
    $wc.UploadFile("https://ci.appveyor.com/api/testresults/nunit3/$($env:APPVEYOR_JOB_ID)", $OutPutFileName)

    if ($?) {
        Write-Output 'Success!'
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