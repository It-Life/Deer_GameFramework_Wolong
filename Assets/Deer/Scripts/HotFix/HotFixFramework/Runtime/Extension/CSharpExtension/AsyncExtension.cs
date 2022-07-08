// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-07-08 23-35-49
//修改作者:杜鑫
//修改时间:2022-07-08 23-35-49
//版 本:0.1 
// ===============================================

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Please modify the description.
/// </summary>
public static class AsyncExtension 
{
    public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
    {
        var tcs = new TaskCompletionSource<object>();
        asyncOp.completed += obj => { tcs.SetResult(null); };
        return ((Task)tcs.Task).GetAwaiter();
    }
}