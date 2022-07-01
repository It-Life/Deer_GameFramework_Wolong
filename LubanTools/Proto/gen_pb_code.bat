@echo off & setlocal enabledelayedexpansion
echo =================start gen proto code=================
set pb_path=pb_message
for /f "delims=" %%i in ('dir /b %pb_path%') do (
echo ------------%%i start gen
protoc -I=pb_message --csharp_out=scripts pb_message\%%i
echo ------------%%i gen success
)
echo =================end gen proto code=================

pause

