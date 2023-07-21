#!/bin/zsh
WORKSPACE=..

GEN_CLIENT=${WORKSPACE}/Tools/Luban.ClientServer/Luban.ClientServer.dll
CONF_ROOT=${WORKSPACE}/DesignerConfigs

OUTPUT_CODE_PATH=${WORKSPACE}/../Assets/Deer/Scripts/HotFix/HotFixCommon/LubanConfig
OUTPUT_DATA_PATH=${WORKSPACE}/GenerateDatas/LubanConfig/Datas

#OUTPUT_DATA_PATH=/Users/admin/Documents/elfsland/LubanTools/GenerateDatas/LubanConfig
#OUTPUT_CODE_PATH=/Users/admin/Documents/elfsland/Assets/Deer/Scripts/HotFix/HotFixCommon/LubanConfig
dotnet ${GEN_CLIENT} -h 127.0.0.1 -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_code_dir ${OUTPUT_CODE_PATH} \
 --output_data_dir ${OUTPUT_DATA_PATH} \
 --gen_types code_cs_unity_bin,data_bin \
 -s all 
