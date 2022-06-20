@echo 复制到100

cd /d %~dp0
set WORK_NUMBER=HuaTuo_100
set ROOT_PATH=%~dp0
set DATA_OUTPUT=%ROOT_PATH%client\data
set GAME_COMMIT_PATH=%ROOT_PATH%..\..\CommitResources
set DATA_UPLOAD_OUTPUT=%GAME_COMMIT_PATH%\%WORK_NUMBER%\Windows\Config

call BuildConfig.bat
echo ======== 开始复制Data文件 ========
xcopy /s/y/i "%DATA_OUTPUT%\*.bin" "%DATA_UPLOAD_OUTPUT%" 
echo ======== 复制Data文件结束 ========

echo ======== 开始生成版本文件 ========
set configPath=%GAME_COMMIT_PATH%\%WORK_NUMBER%\Windows\Config
set configVersionOutPath=%GAME_COMMIT_PATH%\%WORK_NUMBER%\Windows
call ConfigVersion.exe %configPath% %configVersionOutPath%
echo ======== 生成版本文件结束 ========
pause