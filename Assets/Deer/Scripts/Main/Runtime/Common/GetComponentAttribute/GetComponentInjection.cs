using System.Reflection;
using System;
using UnityEngine;
public static class GetComponentInjection
{
    public static InjectEvent<GetComponentAttribute, FieldInfo, MonoBehaviour> SingleObjectClassifier;
    public static InjectEvent<GetComponentAttribute, FieldInfo, MonoHolder<MonoBehaviour, MonoBehaviour[]>> MultipleObjectClassifier;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InjectScriptReferences()
    {
        SingleObjectClassifier += (attr, field, obj) =>
        {
            switch (attr.ComponentAddress)
            {
                case GetComponentFrom.Self:
                    field.SetValue(obj, obj.GetComponent(field.FieldType));
                    break;
                case GetComponentFrom.SceneObject:
                    field.SetValue(obj, MonoBehaviour.FindObjectOfType(field.FieldType));
                    break;
            }
        };
        Inject();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void InjectGameObjectReferences()
    {
        MultipleObjectClassifier += (attr, field, monoHolder) =>
        {
            if (attr.ComponentAddress != GetComponentFrom.TargetGameObject)
                return;
            GameObject targetObj = GameObject.Find(attr.TargetName);
            field.SetValue(monoHolder.t, targetObj.GetComponent(field.GetValue(monoHolder.t).GetType()));
        };
        Inject();
    }


    private static void Inject()
    {
        MonoBehaviour[] objs = MonoBehaviour.FindObjectsOfType<MonoBehaviour>();
        foreach (var obj in objs)
        {
            Type type = obj.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            GetComponentAttribute attr;
            foreach (var field in fields)
            {
                if (field.IsDefined(typeof(GetComponentAttribute), false))
                {
                    attr = field.GetCustomAttribute<GetComponentAttribute>();
                    SingleObjectClassifier?.Invoke(attr, field, obj);
                    MultipleObjectClassifier?.Invoke(attr, field, new MonoHolder<MonoBehaviour, MonoBehaviour[]>(obj, objs));
                }
            }
        }
    }

}