using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CatJson
{
    /// <summary>
    /// 字典类型的Json格式化器
    /// </summary>
    public class DictionaryFormatter : BaseJsonFormatter<IDictionary>
    {

        /// <inheritdoc />
        public override void ToJson(JsonParser parser, IDictionary value, Type type, Type realType, int depth)
        {
            Type dictType = type;
            if (!type.IsGenericType)
            {
                //此处的处理原因类似ArrayFormatter
                dictType = realType;
            }
            Type valueType = TypeUtil.GetDictValueType(dictType);

            parser.AppendLine("{");

            if (value != null)
            {
                int index = 0;
                foreach (DictionaryEntry item in value)
                {
                    parser.Append("\"", depth);
                    parser.Append(item.Key.ToString());
                    parser.Append("\"");
                    parser.Append(":");
                    parser.InternalToJson(item.Value,valueType,null,depth + 1);

                    if (index < value.Count-1)
                    {
                        parser.AppendLine(",");
                    }
                    index++;
                }
            }

            parser.AppendLine(string.Empty);
            parser.Append("}", depth - 1);
        }

        /// <inheritdoc />
        public override IDictionary ParseJson(JsonParser parser, Type type, Type realType)
        {
            IDictionary dict = (IDictionary)TypeUtil.CreateInstance(realType);
            Type dictType = type;
            if (!type.IsGenericType)
            {
                dictType = realType;
            }
            Type keyType =  TypeUtil.GetDictKeyType(dictType);
            Type valueType = TypeUtil.GetDictValueType(dictType);
            ParserHelper.ParseJsonObjectProcedure(parser,dict,keyType,valueType, (userdata1,userdata2,userdata3, key) =>
            {
                IDictionary localDict = (IDictionary) userdata1;
                Type localKeyType = (Type) userdata2;
                Type localValueType = (Type) userdata3;
                
                object value = parser.InternalParseJson(localValueType);
                if (localKeyType == typeof(string))
                {
                    //处理字典key为string的情况
                    localDict.Add(key.ToString(), value);
                }
                else if (localKeyType == typeof(int))
                {
                    //处理字典key为int的情况
                    localDict.Add(key.AsInt(), value);
                }
                else if (localKeyType.IsEnum)
                {
                    //处理字典key为枚举的情况
                    object enumObj = Enum.Parse(localKeyType, key.ToString());
                    localDict.Add(enumObj,value);
                }
                else
                {
                    throw new Exception($"不支持的字典key类型:{localKeyType}");
                }
            });

            return dict;
        }
    }
}