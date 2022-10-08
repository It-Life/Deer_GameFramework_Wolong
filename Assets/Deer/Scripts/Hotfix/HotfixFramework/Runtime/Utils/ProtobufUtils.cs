using System;
using System.ComponentModel;
using System.IO;
using Google.Protobuf;

public class ProtobufUtils 
{

		
    public static void ToStream(object message, MemoryStream stream)
    {
        ((IMessage) message).WriteTo(stream);
    }
		
    public static object FromBytes(Type type, byte[] bytes, int index, int count)
    {
        object message = Activator.CreateInstance(type);
        ((IMessage)message).MergeFrom(bytes, index, count);
        ISupportInitialize iSupportInitialize = message as ISupportInitialize;
        if (iSupportInitialize == null)
        {
            return message;
        }
        iSupportInitialize.EndInit();
        return message;
    }
		
    public static object FromBytes(object instance, byte[] bytes, int index, int count)
    {
        object message = instance;
        ((IMessage)message).MergeFrom(bytes, index, count);
        ISupportInitialize iSupportInitialize = message as ISupportInitialize;
        if (iSupportInitialize == null)
        {
            return message;
        }
        iSupportInitialize.EndInit();
        return message;
    }
		
    public static object FromStream(Type type, MemoryStream stream)
    {
        object message = Activator.CreateInstance(type);
        ((IMessage)message).MergeFrom(stream.GetBuffer(), (int)stream.Position, (int)stream.Length);
        ISupportInitialize iSupportInitialize = message as ISupportInitialize;
        if (iSupportInitialize == null)
        {
            return message;
        }
        iSupportInitialize.EndInit();
        return message;
    }
		
    public static object FromStream(object message, MemoryStream stream)
    {
        // 这个message可以从池中获取，减少gc
        ((IMessage)message).MergeFrom(stream.GetBuffer(), (int)stream.Position, (int)stream.Length);
        ISupportInitialize iSupportInitialize = message as ISupportInitialize;
        if (iSupportInitialize == null)
        {
            return message;
        }
        iSupportInitialize.EndInit();
        return message;
    }
    /// <summary>
    /// 序列化protobuf
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static byte[] Serialize(object message)
    {
        return ((IMessage)message).ToByteArray();
    }
    /// <summary>
    /// 反序列化protobuf
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataBytes"></param>
    /// <returns></returns>
    public static T Deserialize<T>(byte[] dataBytes) where T : IMessage, new()
    {
        T msg = new T();
        msg = (T)msg.Descriptor.Parser.ParseFrom(dataBytes);
        return msg;
    }
    public static int GetHighOrder(int cmdMerge)
    {
        return cmdMerge >> 16;
    }

    public static int GetLowOrder(int cmdMerge)
    {
        return cmdMerge & 65535;
    }
}
