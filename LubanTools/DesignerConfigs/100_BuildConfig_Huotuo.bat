@echo off  start huatuo100

cd /d %~dp0
set ROOT_PATH=%~dp0
set WORK_NUMBER=Huatuo_100
set PLATFORM=Windows64
set DATA_OUTPUT=%ROOT_PATH%..\GenerateDatas\LubanConfig
set GAME_COMMIT_PATH=%ROOT_PATH%..\..\CommitResources
set DATA_UPLOAD_OUTPUT=%GAME_COMMIT_PATH%\%WORK_NUMBER%\%PLATFORM%\LubanConfig

call gen_code_bin.bat

echo ======== 开始生成版本文件 ========
set configVersionOutPath=%ROOT_PATH%..\GenerateDatas
set configVersion_UPLOAD_OutPath=%GAME_COMMIT_PATH%\%WORK_NUMBER%\%PLATFORM%
set OUTPUT_DATA_PATH=%WORKSPACE%\GenerateDatas\LubanConfig
call ConfigVersion.exe %DATA_OUTPUT% %configVersionOutPath%
echo ======== 生成版本文件结束 ========

echo ======== 开始复制Data文件 ========
xcopy /s/y/i "%DATA_OUTPUT%\*.bytes" "%DATA_UPLOAD_OUTPUT%" 
echo ======== 复制Data文件结束 ========

echo ======== 开始复制Data文件 ========
xcopy /s/y/i "%configVersionOutPath%\*.xml" "%configVersion_UPLOAD_OutPath%" 
echo ======== 复制Data文件结束 ========

start %configVersion_UPLOAD_OutPath%

pause