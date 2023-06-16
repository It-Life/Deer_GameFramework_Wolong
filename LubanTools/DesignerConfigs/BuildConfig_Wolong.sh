#!/bin/zsh
WORKSPACE=..
WORK_NUMBER=StreamingAssets
PLATFORM=IOS
DATA_OUTPUT=${WORKSPACE}/GenerateDatas/LubanConfig/Datas
GAME_COMMIT_PATH=${WORKSPACE}/../Assets
DATA_UPLOAD_OUTPUT=${GAME_COMMIT_PATH}/${WORK_NUMBER}/LubanConfig

sh gen_code_bin.sh

echo ======== 开始生成版本文件 ========
GEN_CONFIGVERSION=${WORKSPACE}/Tools/ConfigVersion/ConfigVersion.dll
configVersionOutPath=${WORKSPACE}/GenerateDatas/LubanConfig
configVersion_UPLOAD_OutPath=${GAME_COMMIT_PATH}/${WORK_NUMBER}
OUTPUT_DATA_PATH=${WORKSPACE}/GenerateDatas/LubanConfig
dotnet ${GEN_CONFIGVERSION} --input_data_dir ${DATA_OUTPUT} --output_version_dir ${configVersionOutPath}
echo ======== 生成版本文件结束 ========
