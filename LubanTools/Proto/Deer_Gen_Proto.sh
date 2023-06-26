#!/bin/bash
CURRENT_DIR=$(cd `dirname $0`; pwd)
cd ${CURRENT_DIR}
WORKSPACE=..
echo =================start gen proto code=================
pb_path=pb_message
out_path=../../Assets/Deer/Scripts/Hotfix/HotfixCommon/Proto
rm -f $out_path/*
for file in $pb_path/*
do
  if [ -f "$file" ]; then
    echo "------------${file##*/} start gen"
    protoc -I=pb_message --csharp_out=$out_path $file
    echo "------------${file##*/} gen success"
  fi
done
echo =================end gen proto code=================
GEN_PROTOBUFRESOLVER=${WORKSPACE}/Tools/ProtobufResolver/ProtobufResolver.dll
INPUT_DATA_DIR=${CURRENT_DIR}/pb_message
OUTEVENTPATH=${WORKSPACE}/../Assets/Deer/Scripts/HotFix/HotFixCommon/Definition/Constant
dotnet ${GEN_PROTOBUFRESOLVER} --input_data_dir ${INPUT_DATA_DIR} --output_proto_dir ${OUTEVENTPATH}
echo =================end gen proto event=================
read -p "Press any key to continue..."

