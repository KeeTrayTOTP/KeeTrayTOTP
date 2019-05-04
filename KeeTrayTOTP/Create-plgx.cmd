@echo off
echo Creating KeePass plugin package
CHDIR /D %CD%
set PLUGIN_NAME=KeeTrayTOTP
set SourceFolder=%CD%
echo %SourceFolder%

set PublishFolder=%SourceFolder%\..
set OutputFile=%PublishFolder%\%PLUGIN_NAME%.plgx

:Cleanup
rd /s /q %SourceFolder%\obj %SourceFolder%\bin
del /s /q %OutputFile%

:Publish
"C:\Program Files (x86)\KeePass Password Safe 2\KeePass.exe" --plgx-create %SourceFolder%

echo KeePass Plugin package has been created: %PublishFolder%\%PLUGIN_NAME%.plgx
pause