#!/bin/zsh

WORK_NUMBER=StreamingAssets
PLATFORM=IOS
DATA_OUTPUT=..\GenerateDatas\LubanConfig
GAME_COMMIT_PATH=..\..\Assets
DATA_UPLOAD_OUTPUT={GAME_COMMIT_PATH}\{WORK_NUMBER}\LubanConfig

sh gen_code_bin.sh

echo ======== 开始生成版本文件 ========
configVersionOutPath=%ROOT_PATH%..\GenerateDatas
configVersion_UPLOAD_OutPath=%GAME_COMMIT_PATH%\%WORK_NUMBER%
OUTPUT_DATA_PATH=%WORKSPACE%\GenerateDatas\LubanConfig
#call ConfigVersion.exe %DATA_OUTPUT% %configVersionOutPath%
echo ======== 生成版本文件结束 ========
