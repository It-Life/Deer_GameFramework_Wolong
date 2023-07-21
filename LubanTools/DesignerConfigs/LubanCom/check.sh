#!/bin/zsh
WORKSPACE=../..
GEN_CLIENT=${WORKSPACE}/Tools/Luban.ClientServer/Luban.ClientServer.dll

CONF_ROOT=${WORKSPACE}/DesignerConfigs

dotnet ${GEN_CLIENT} -h 127.0.0.1 -j cfg --generateonly --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_data_dir ../Projects/GenerateDatas/json \
 --gen_types data_bin \
 -s all 
