// ================================================
//描 述:
//作 者:Xiaohei.Wang(Wenhao)
//创建时间:2023-05-14 15-02-05
//修改作者:Xiaohei.Wang(Wenhao)
//修改时间:2023-05-14 15-02-05
//版 本:0.1 
// ===============================================

using UnityEngine;

namespace Deer.Editor.TaskList
{
    [CreateAssetMenu(fileName = "TodoList", menuName = "Deer/Todo List", order = 300)]
    public class TaskListConfig : ScriptableObject
    {
#pragma warning disable 0414
        [SerializeField]
        int TaskCount = 0;
#pragma warning disable 0414
        [SerializeField]
        bool[] Mark = new bool[] { };
        [SerializeField]
        bool[] Enabled = new bool[] { };
        [SerializeField]
        int[] Progress = new int[] { };
        [SerializeField]
        string[] Title = new string[] { };
        [SerializeField]
        string[] Description = new string[] { };
    }
}
