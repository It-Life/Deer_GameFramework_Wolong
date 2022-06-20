@echo ProtocToScript Start ......

cd /d %~dp0

set ROOT_PATH=%~dp0
set PROTO_PAHT=%ROOT_PATH%proto
set CSHARP_OUTPUT=%ROOT_PATH%client\scripts
set SERVER_OUTPUT=%ROOT_PATH%server\class
set SERVER_LANGUGE=java

rem build script and data
python %ROOT_PATH%..\Excel2Protobuf\src\python\protobuf_interpreter.py -i %PROTO_PAHT% -c %CSHARP_OUTPUT% -s %SERVER_OUTPUT% -l %SERVER_LANGUGE%

@echo ======== ProtocToScript End ========
pause