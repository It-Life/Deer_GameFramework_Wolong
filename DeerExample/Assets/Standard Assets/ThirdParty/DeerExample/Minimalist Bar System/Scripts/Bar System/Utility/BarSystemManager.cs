using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimalist.Bar.Utility
{
    [ExecuteInEditMode]
    public class BarSystemManager : Singleton<BarSystemManager>
    {
        // Public fields
        public bool automaticObjectNaming = true;

        private void OnEnable()
        {
            if (automaticObjectNaming)
            {
                this.name = "Bar System Manager";
            }
        }
    }
}