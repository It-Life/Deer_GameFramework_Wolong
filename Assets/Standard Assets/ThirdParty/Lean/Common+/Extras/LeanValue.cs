using UnityEngine;
using UnityEngine.Events;

namespace Lean.Common
{
	/// <summary>This component allows you to store 1, 2, or 3 values. This is done by calling one of the <b>SetX/Y/Z</b> methods, and then sending it out using the <b>OnValueX/Y/Z</b> events.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanValue")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Value")]
	public class LeanValue : MonoBehaviour
	{
		[System.Serializable] public class FloatEvent : UnityEvent<float> {}
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}

		/// <summary>This event will send any previously set values after the specified delay.</summary>
		public FloatEvent OnValueX { get { if (onValueX == null) onValueX = new FloatEvent(); return onValueX; } } [SerializeField] private FloatEvent onValueX;

		/// <summary>This event will send any previously set values after the specified delay.</summary>
		public FloatEvent OnValueY { get { if (onValueY == null) onValueY = new FloatEvent(); return onValueY; } } [SerializeField] private FloatEvent onValueY;

		/// <summary>This event will send any previously set values after the specified delay.</summary>
		public FloatEvent OnValueZ { get { if (onValueZ == null) onValueZ = new FloatEvent(); return onValueZ; } } [SerializeField] private FloatEvent onValueZ;

		/// <summary>This event will send any previously set values after the specified delay.</summary>
		public Vector2Event OnValueXY { get { if (onValueXY == null) onValueXY = new Vector2Event(); return onValueXY; } } [SerializeField] private Vector2Event onValueXY;

		/// <summary>This event will send any previously set values after the specified delay.</summary>
		public Vector3Event OnValueXYZ { get { if (onValueXYZ == null) onValueXYZ = new Vector3Event(); return onValueXYZ; } } [SerializeField] private Vector3Event onValueXYZ;

		/// <summary>The current value.</summary>
		public Vector3 Current { set { if (current != value) { current = value; Submit(); } } get { return current; } } [SerializeField] private Vector3 current;

		/// <summary>This method allows you to set the X axis.</summary>
		public void SetX(float x)
		{
			current.x = x;
			
			Submit();
		}

		/// <summary>This method allows you to set the Y axis.</summary>
		public void SetY(float y)
		{
			current.y = y;
			
			Submit();
		}

		/// <summary>This method allows you to set the Z axis.</summary>
		public void SetZ(float z)
		{
			current.z = z;
			
			Submit();
		}

		/// <summary>This method allows you to set the XY axis.</summary>
		public void SetXY(Vector2 xy)
		{
			current.x = xy.x;
			current.y = xy.y;

			Submit();
		}

		/// <summary>This method allows you to set the XYZ axis.</summary>
		public void SetXYZ(Vector3 xyz)
		{
			current = xyz;

			Submit();
		}

		private void Submit()
		{
			if (onValueX != null)
			{
				onValueX.Invoke(current.x);
			}

			if (onValueY != null)
			{
				onValueY.Invoke(current.y);
			}

			if (onValueZ != null)
			{
				onValueZ.Invoke(current.z);
			}

			if (onValueXY != null)
			{
				onValueXY.Invoke(current);
			}

			if (onValueXYZ != null)
			{
				onValueXYZ.Invoke(current);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanValue;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanValue_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			if (Draw("current", "The current value.") == true)
			{
				Each(tgts, t => t.Current = serializedObject.FindProperty("current").vector3Value, true);
			}

			Separator();

			var usedA = Any(tgts, t => t.OnValueX.GetPersistentEventCount() > 0);
			var usedB = Any(tgts, t => t.OnValueY.GetPersistentEventCount() > 0);
			var usedC = Any(tgts, t => t.OnValueZ.GetPersistentEventCount() > 0);
			var usedD = Any(tgts, t => t.OnValueXY.GetPersistentEventCount() > 0);
			var usedE = Any(tgts, t => t.OnValueXYZ.GetPersistentEventCount() > 0);

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onValueX");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onValueY");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onValueZ");
			}

			if (usedD == true || showUnusedEvents == true)
			{
				Draw("onValueXY");
			}

			if (usedE == true || showUnusedEvents == true)
			{
				Draw("onValueXYZ");
			}
		}
	}
}
#endif