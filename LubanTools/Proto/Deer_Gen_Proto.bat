@echo off
cd %~dp0
set ROOT_PATH=%~dp0
set WORKSPACE=..
echo =================start gen proto code=================
set pb_path=pb_message
set out_path=../../Assets/Deer/Scripts/Hotfix/HotfixCommon/Proto
del /f /s /q %out_path%\*.*
for /f "delims=" %%i in ('dir /b %pb_path%') do (
echo ------------%%i start gen
protoc -I=pb_message --csharp_out=%out_path% pb_message\%%i
echo ------------%%i gen success
)
echo =================end gen proto code=================
set GEN_PROTOBUFRESOLVER=%WORKSPACE%\Tools\ProtobufResolver\ProtobufResolver.exe
set INPUT_DATA_DIR=%ROOT_PATH%pb_message
set OUTEVENTPATH=%WORKSPACE%/../Assets/Deer/Scripts/HotFix/HotFixCommon/Definition/Constant
%GEN_PROTOBUFRESOLVER% --input_data_dir %INPUT_DATA_DIR% --output_proto_dir %OUTEVENTPATH%
echo =================end gen proto event=================
pause

