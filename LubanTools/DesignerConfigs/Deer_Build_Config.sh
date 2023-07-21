#!/bin/bash
CURRENT_DIR=$(cd `dirname $0`; pwd)
cd ${CURRENT_DIR}
WORKSPACE=..
INPUT_DATA_DIR=${WORKSPACE}/GenerateDatas/LubanConfig/Datas
GEN_CONFIGVERSION=${WORKSPACE}/Tools/ConfigVersion/ConfigVersion.dll
OUTPUT_VERSION_DIR=${WORKSPACE}/GenerateDatas/LubanConfig
LUBANCOM=LubanCom/
sh ${LUBANCOM}gen_code_bin.sh
echo ======== Start gen version file. ========
dotnet ${GEN_CONFIGVERSION} --input_data_dir ${INPUT_DATA_DIR} --output_version_dir ${OUTPUT_VERSION_DIR}
echo ======== End gen version file. ========
