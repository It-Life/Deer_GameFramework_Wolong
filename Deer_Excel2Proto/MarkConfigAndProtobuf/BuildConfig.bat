@echo ConfData Pack Start ......

cd /d %~dp0
set ROOT_PATH=%~dp0

set NAMESPACE=ConfigData
set PACKAGE_NAME=com.deer.server.protobuf.proto

set EXCLE_PAHT=%ROOT_PATH%..\ConfigFiles(excel)
set DATA_OUTPUT=%ROOT_PATH%client\data
set CSHARP_OUTPUT=%ROOT_PATH%client\scripts
set SERVER_DATAPATH=%ROOT_PATH%server\data
set SERVER_OUTPUT=%ROOT_PATH%server\class


set TEMP_PROTOFILE=.\_temp\proto_files
set SERVER_PROTOFILE=%ROOT_PATH%proto\config
set SERVER_LANGUGE=java
rem build script and data
python %ROOT_PATH%..\Excel2Protobuf\src\python\excel_to_protobuf.py -i %EXCLE_PAHT% -n %NAMESPACE% -d %DATA_OUTPUT% -c %CSHARP_OUTPUT% -s %SERVER_OUTPUT% -l %SERVER_LANGUGE% -p %PACKAGE_NAME%
rem copy to server work folder

del %DATA_OUTPUT%\template.bin

xcopy /s/y/i "%DATA_OUTPUT%\*.bin" "%SERVER_DATAPATH%" 

xcopy /s/y/i "%TEMP_PROTOFILE%\*.proto" "%SERVER_PROTOFILE%"

@echo ======== ConfData Pack End ========
pause