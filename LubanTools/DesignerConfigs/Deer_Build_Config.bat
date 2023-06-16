@echo off

cd /d %~dp0
set WORKSPACE=..
set ROOT_PATH=%~dp0
set WORK_NUMBER=StreamingAssets
set PLATFORM=Windows64
set DATA_OUTPUT=%ROOT_PATH%..\GenerateDatas\LubanConfig\Datas
set GAME_COMMIT_PATH=%ROOT_PATH%..\..\Assets
set DATA_UPLOAD_OUTPUT=%GAME_COMMIT_PATH%\%WORK_NUMBER%\LubanConfig

call gen_code_bin.bat

echo ======== 开始生成版本文件 ========
set GEN_CONFIGVERSION=%WORKSPACE%\Tools\ConfigVersion\ConfigVersion.exe
set configVersionOutPath=%ROOT_PATH%..\GenerateDatas\LubanConfig
set configVersion_UPLOAD_OutPath=%GAME_COMMIT_PATH%\%WORK_NUMBER%
set OUTPUT_DATA_PATH=%WORKSPACE%\GenerateDatas\LubanConfig
%GEN_CONFIGVERSION% --input_data_dir %DATA_OUTPUT% --output_version_dir %configVersionOutPath%
echo ======== 生成版本文件结束 ========

pause