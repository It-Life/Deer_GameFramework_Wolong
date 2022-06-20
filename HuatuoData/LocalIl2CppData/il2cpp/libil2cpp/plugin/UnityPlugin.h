#pragma once

typedef enum
{
    MONO_GET_ASSEMBLY_CSHARP = 100,
    MONO_GET_METHOD,
    MONO_GET_STRING_HEAP,
    MONO_GET_OPCODE,
    MONO_METADATA_ENCRYPT,
    MONO_METHOD_TO_IR_CB,
    MONO_INTERNAL_HASH_TABLE_INSERT_CB,
    IL2CPP_GET_GLOBAL_METADATA,
    IL2CPP_GET_STRING
} callback_func_type;

extern const void * query_call_back(callback_func_type uID);

typedef const void * (* def_query_call_back)(callback_func_type uID);
typedef void * (* def_il2cpp_get_global_metadata) (const char* file_name);
typedef void(* def_il2cpp_get_string) (char * string, int index);
