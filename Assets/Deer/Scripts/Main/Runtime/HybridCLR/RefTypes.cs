using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;
using System.IO;
using Bright.Serialization;
using Google.Protobuf.Collections;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using Google.Protobuf;

[assembly: Preserve]
enum IntEnum : int
{
    A,
    B,
}

enum ShortEnum : short
{
    A,
    B,
}

public class MyComparer<T> : Comparer<T>
{
    public override int Compare(T x, T y)
    {
        return 0;
    }
}

class MyStateMachine : IAsyncStateMachine
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

public class RefTypes : MonoBehaviour
{
    List<Type> GetTypes()
    {
        return new List<Type>
        {
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GetTypes());
        GameObject.Instantiate<GameObject>(null);
        Instantiate<GameObject>(null, null);
        Instantiate<GameObject>(null, null, false);
        Instantiate<GameObject>(null, new Vector3(), new Quaternion());
        Instantiate<GameObject>(null, new Vector3(), new Quaternion(), null);
        float a = 1;
        float b = a;
        Debug.Log(b);
        byte[] bytes = new byte[]{0,1};
        Debug.Log(bytes);
        ByteString bytestr = ByteString.CopyFrom(bytes);
        Debug.Log(bytestr.Length);
        ByteBuf buf = new ByteBuf(bytes);
        Queue<Int64> queueInt = new Queue<long>();
        queueInt.Enqueue(10);
        long aa = queueInt.Dequeue();
    }

    public void RefNumerics()
    {
        var a = new System.Numerics.BigInteger();
        a.ToString();
    }


    void RefMisc()
    {

    }

    void RefComparers()
    {
        var a = new object[]
        {
            new MyComparer<int>(),
            new MyComparer<long>(),
            new MyComparer<float>(),
            new MyComparer<double>(),
            new MyComparer<object>(),
        };

        new MyComparer<int>().Compare(default, default);
        new MyComparer<long>().Compare(default, default);
        new MyComparer<float>().Compare(default, default);
        new MyComparer<double>().Compare(default, default);
        new MyComparer<object>().Compare(default, default);

        object b = EqualityComparer<int>.Default;
        b = EqualityComparer<long>.Default;
        b = EqualityComparer<float>.Default;
        b = EqualityComparer<double>.Default;
        b = EqualityComparer<object>.Default;
    }


    void RefNullable()
    {
        // nullable
        object b = null;
        int? a = 5;
        b = a;
        int d = (int?)b ?? 7;
        int e = (int)b;
        a = d;
        b = a;
        b = Enumerable.Range(0, 1).Reverse().Take(1).TakeWhile(x => true).Skip(1).All(x => true);
        b = new WaitForSeconds(1f);
        b = new WaitForSecondsRealtime(1f);
        b = new WaitForFixedUpdate();
        b = new WaitForEndOfFrame();
        b = new WaitWhile(() => true);
        b = new WaitUntil(() => true);
    }

    void RefContainer()
    {
        //int, long,float,double, IntEnum,object
        List<object> b = new List<object>()
        {

        };
    }

    void RefAsyncMethod()
    {
        var stateMachine = new MyStateMachine();

        TaskAwaiter aw = default;
        var c0 = new AsyncUniTaskMethodBuilder();
        c0.Start(ref stateMachine);
        c0.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c0.SetException(null);
        c0.SetResult();

        var c1 = new AsyncUniTaskMethodBuilder();
        c1.Start(ref stateMachine);
        c1.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c1.SetException(null);
        c1.SetResult();

        var c2 = new AsyncUniTaskMethodBuilder<bool>();
        c2.Start(ref stateMachine);
        c2.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c2.SetException(null);
        c2.SetResult(default);

        var c3 = new AsyncUniTaskMethodBuilder<int>();
        c3.Start(ref stateMachine);
        c3.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c3.SetException(null);
        c3.SetResult(default);

        var c4 = new AsyncUniTaskMethodBuilder<long>();
        c4.Start(ref stateMachine);
        c4.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c4.SetException(null);

        var c5 = new AsyncUniTaskMethodBuilder<float>();
        c5.Start(ref stateMachine);
        c5.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c5.SetException(null);
        c5.SetResult(default);

        var c6 = new AsyncUniTaskMethodBuilder<double>();
        c6.Start(ref stateMachine);
        c6.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c6.SetException(null);
        c6.SetResult(default);

        var c7 = new AsyncUniTaskMethodBuilder<object>();
        c7.Start(ref stateMachine);
        c7.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c7.SetException(null);
        c7.SetResult(default);

        var c8 = new AsyncUniTaskMethodBuilder<IntEnum>();
        c8.Start(ref stateMachine);
        c8.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c8.SetException(null);
        c8.SetResult(default);

        var c9 = new AsyncUniTaskMethodBuilder();
        var b = AsyncVoidMethodBuilder.Create();
        c9.Start(ref stateMachine);
        c9.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c9.SetException(null);
        c9.SetResult();
        Debug.Log(b);
    }

    void RefNewtonsoftJson()
    {
        //AotHelper.EnsureList<int>();
        //AotHelper.EnsureList<long>();
        //AotHelper.EnsureList<float>();
        //AotHelper.EnsureList<double>();
        //AotHelper.EnsureList<string>();
        //AotHelper.EnsureDictionary<int, int>();
        //AotHelper.EnsureDictionary<int, string>();
    }

    public void RefProtobufNet()
    {
        
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
    }

    class TestTable
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public void RefSQLite()
    {
    }

    public static async void TestAsync3()
    {
        Debug.Log("async task 1");
        await UniTask.DelayFrame(10);
        Debug.Log("async task 2");
    }

/*    public static int Main_1()
    {
        Debug.Log("hello,hybridclr");
        var task = Task.Run(async () =>
        {
            await TestAsync2();
        });
        task.Wait();
        Debug.Log("async task end");
        Debug.Log("async task end2");
        return 0;
    }*/

    public static async UniTask TestAsync2()
    {
        Debug.Log("async task 1");
        await UniTask.Delay(3000);
        Debug.Log("async task 2");
        AsyncUniTaskMethodBuilder.Create();
        var _ = typeof(AsyncUniTaskMethodBuilder<>);
    }

// Update is called once per frame
/*    void Update()
    {
        TestAsync();
    }*/

/*    public static int TestAsync()
    {
        var t0 = Task.Run(async () =>
        {
            await Task.Delay(10);
        });
        t0.Wait();
        var task = Task.Run(async () =>
        {
            await Task.Delay(10);
            return 100;
        });
        Debug.Log(task.Result);
        return 0;
    }*/
	void RefXml()
    {
        System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
        System.Xml.XmlElement configRoot = xmlDocument.DocumentElement;
        System.Xml.XmlNode node = xmlDocument.SelectSingleNode("Root");
    }
    void RefUniTask()
    {
        UniTask uniTask = new UniTask();
        uniTask1();
    }

    UniTask<object> uniTask1() 
    {
        return new UniTask<object>();
    } 
}