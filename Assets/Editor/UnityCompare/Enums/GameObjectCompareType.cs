using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCompare
{
    [Flags]
    public enum GameObjectCompareType
    {
        none = 0,
        activeEqual = 1 << 0, //GameObject的ActiveSelf是否相等
        tagEqual = 1 << 1, //GameObject的Tag是否相等
        layerEqual = 1 << 2, //GameObject的Layer是否相等
        childCountEqual = 1 << 3, //子对象数量是否相等
        childContentEqual = 1 << 4, //子对象内容是否相等
        componentCountEqual = 1 << 5, //GameObject的Component数量是否相等
        componentContentEqual = 1 << 6, //GameObject的内容是否相等

        allEqual = activeEqual + tagEqual + layerEqual + childCountEqual + childContentEqual + componentCountEqual + componentContentEqual,
    }
}
