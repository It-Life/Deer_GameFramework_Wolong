using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

[assembly: Preserve]
enum IntEnum : int
{
    A,
    B,
}

public class RefTypes : MonoBehaviour
{

    void RefUnityEngine()
    {
        GameObject.Instantiate<GameObject>(null);
        Instantiate<GameObject>(null, null);
        Instantiate<GameObject>(null, null, false);
        Instantiate<GameObject>(null, new Vector3(), new Quaternion());
        Instantiate<GameObject>(null, new Vector3(), new Quaternion(), null);
        this.gameObject.AddComponent<RefTypes>();
        gameObject.AddComponent(typeof(RefTypes));
    }

    void RefNullable()
    {
        // nullable
        int? a = 5;
        object b = a;
    }

    void RefContainer()
    {
        new List<object>()
        {
            new Dictionary<int, int>(),
            new Dictionary<int, long>(),
            new Dictionary<int, object>(),
            new Dictionary<uint, object>(),
            new Dictionary<long, int>(),
            new Dictionary<long, long>(),
            new Dictionary<long, object>(),
            new Dictionary<object, long>(),
            new Dictionary<object, object>(),
            new SortedDictionary<int, long>(),
            new SortedDictionary<int, object>(),
            new SortedDictionary<long, int>(),
            new SortedDictionary<long, object>(),
            new HashSet<int>(),
            new HashSet<long>(),
            new HashSet<object>(),
            new List<int>(),
            new List<long>(),
            new List<float>(),
            new List<double>(),
            new List<object>(),
            new List<Assembly>(),
            new ValueTuple<int, int>(1, 1),
            new ValueTuple<long, long>(1, 1),
            new ValueTuple<object, object>(1, 1),
            new SingletonMono<MonoBehaviour>(),
        };
    }

    class RefStateMachine : IAsyncStateMachine
    {
        public void MoveNext()
        {
            throw new NotImplementedException();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            throw new NotImplementedException();
        }
    }
    void RefAsyncMethod()
    {
        var stateMachine = new RefStateMachine();

        TaskAwaiter aw = default;
        var c0 = new AsyncTaskMethodBuilder();
        c0.Start(ref stateMachine);
        c0.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c0.SetException(null);
        c0.SetResult();

        var c1 = new AsyncTaskMethodBuilder();
        c1.Start(ref stateMachine);
        c1.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c1.SetException(null);
        c1.SetResult();

        var c2 = new AsyncTaskMethodBuilder<bool>();
        c2.Start(ref stateMachine);
        c2.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c2.SetException(null);
        c2.SetResult(default);

        var c3 = new AsyncTaskMethodBuilder<int>();
        c3.Start(ref stateMachine);
        c3.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c3.SetException(null);
        c3.SetResult(default);

        var c4 = new AsyncTaskMethodBuilder<long>();
        c4.Start(ref stateMachine);
        c4.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c4.SetException(null);

        var c5 = new AsyncTaskMethodBuilder<float>();
        c5.Start(ref stateMachine);
        c5.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c5.SetException(null);
        c5.SetResult(default);

        var c6 = new AsyncTaskMethodBuilder<double>();
        c6.Start(ref stateMachine);
        c6.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c6.SetException(null);
        c6.SetResult(default);

        var c7 = new AsyncTaskMethodBuilder<object>();
        c7.Start(ref stateMachine);
        c7.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c7.SetException(null);
        c7.SetResult(default);

        var c8 = new AsyncTaskMethodBuilder<IntEnum>();
        c8.Start(ref stateMachine);
        c8.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c8.SetException(null);
        c8.SetResult(default);

        var c9 = new AsyncVoidMethodBuilder();
        var b = AsyncVoidMethodBuilder.Create();
        c9.Start(ref stateMachine);
        c9.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c9.SetException(null);
        c9.SetResult();
        Debug.Log(b);
    }

    class RefMessage : IMessage<RefMessage>
    {
        public MessageDescriptor Descriptor => throw new NotImplementedException();

        public UnknownFieldSet _unknownFields;

        public int CalculateSize()
        {
            throw new NotImplementedException();
        }

        public RefMessage Clone()
        {
            throw new NotImplementedException();
        }

        public bool Equals(RefMessage other)
        {
            throw new NotImplementedException();
        }

        public void MergeFrom(RefMessage message)
        {
            throw new NotImplementedException();
        }

        public void MergeFrom(CodedInputStream input)
        {
            throw new NotImplementedException();
        }

        public void WriteTo(CodedOutputStream output)
        {
            throw new NotImplementedException();
        }
    }

    void RefMessageMethod() 
    {
        IMessage message = new RefMessage();
        message.Equals((RefMessage)message);
        MessageParser<RefMessage> messageParser = new MessageParser<RefMessage>(() => new RefMessage());
        messageParser.ParseFrom(new byte[0]);
        messageParser.ParseFrom(new byte[0],0,0);
        messageParser.ParseFrom(ByteString.Empty);
        messageParser.ParseFrom(Stream.Null);
        messageParser.ParseDelimitedFrom(Stream.Null);
        messageParser.ParseJson(String.Empty);
        messageParser.WithDiscardUnknownFields(false);
        messageParser.WithExtensionRegistry(new ExtensionRegistry());
        Lists.Equals(new List<object>(),new List<object>());
        ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(0, 0);
        ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(0);
        RepeatedField<RefMessage> refMessages = new RepeatedField<RefMessage>();
    }
    public void RefGoogleProtobuf()
    {
        var o = new object[]
        {
            new MapField<int, int>(),
            new MapField<int,int>.Codec(default, default, default),
            new MapField<string, object>(),
            new MapField<string, object>.Codec(default, default, default),
        };
/*        IBsonSerializer<object> s = null;
        BsonSerializer.RegisterSerializer<object>(s);
        BsonSerializer.Serialize(null, typeof(void), null);
        BsonSerializer.Serialize<string>(null, "");*/
    }
}
