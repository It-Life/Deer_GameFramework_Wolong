using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCompare
{
    [Flags]
    public enum ComponentCompareType
    {
        none = 0,
        contentEqual = 1 << 1, //内容相等

        allEqual = contentEqual,
    }
}
