#!/bin/zsh
cd %~dp0
ROOT_PATH=%~dp0
WORKSPACE=..
echo =================start gen proto code=================
pb_path=pb_message
out_path=../../Assets/Deer/Scripts/Hotfix/HotfixCommon/Proto
del /f /s /q %out_path%\*.*
for /f "delims=" %%i in ('dir /b %pb_path%') do (
echo ------------%%i start gen
protoc -I=pb_message --csharp_out=%out_path% pb_message\%%i
echo ------------%%i gen success
)
echo =================end gen proto code=================
GEN_PROTOBUFRESOLVER=%WORKSPACE%\Tools\ProtobufResolver\ProtobufResolver.dll
INPUT_DATA_DIR=%ROOT_PATH%pb_message
OUTEVENTPATH=%WORKSPACE%/../Assets/Deer/Scripts/HotFix/HotFixCommon/Definition/Constant
%GEN_PROTOBUFRESOLVER% --input_data_dir %INPUT_DATA_DIR% --output_proto_dir %OUTEVENTPATH%
echo =================end gen proto event=================

