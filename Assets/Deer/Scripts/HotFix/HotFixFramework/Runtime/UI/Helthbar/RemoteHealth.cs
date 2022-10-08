using UnityEngine;
using System.Collections;

[System.Serializable]
public class RemoteField<T>
{
	public MonoBehaviour targetScript;
	public string fieldName;
	private System.Reflection.FieldInfo GetFieldInfo()
	{
		if (targetScript == null)
			return null;
		var type = targetScript.GetType();
		return type.GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
	}
	public T Value
	{
		get
		{
			var field = GetFieldInfo();
			if (field != null)
			{
				var val = field.GetValue(targetScript);
				return (T)val;
			}
			return default(T);
		}
		set
		{
			var field = GetFieldInfo();
			if (field != null)
			{
				field.SetValue(targetScript, value);
			}
		}
	}
}
[System.Serializable]
public class RemoteFloatHealth : RemoteField<float> { };

[System.Serializable]
public class RemoteIntHealth : RemoteField<int> { };
