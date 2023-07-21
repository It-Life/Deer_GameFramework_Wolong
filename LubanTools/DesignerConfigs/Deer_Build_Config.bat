@echo off

cd /d %~dp0
set ROOT_PATH=%~dp0
set WORKSPACE=..
set INPUT_DATA_DIR=%ROOT_PATH%..\GenerateDatas\LubanConfig\Datas
set GEN_CONFIGVERSION=%WORKSPACE%\Tools\ConfigVersion\ConfigVersion.exe
set OUTPUT_VERSION_DIR=%ROOT_PATH%..\GenerateDatas\LubanConfig
set LUBANCOM=LubanCom\
call %LubanCom%gen_code_bin.bat
echo ======== Start gen version file. ========
%GEN_CONFIGVERSION% --input_data_dir %INPUT_DATA_DIR% --output_version_dir %OUTPUT_VERSION_DIR%
echo ======== End gen version file. ========
pause