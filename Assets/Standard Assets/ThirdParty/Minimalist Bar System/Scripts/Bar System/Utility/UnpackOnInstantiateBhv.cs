using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Minimalist.Bar.Utility
{
    [ExecuteInEditMode]
    public class UnpackOnInstantiateBhv : MonoBehaviour
    {
        private void Awake()
        {

            if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
            {
                PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }

            DestroyImmediate(this);
        }
    }
}
#endif