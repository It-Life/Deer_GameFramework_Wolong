using UnityEngine;
using UnityEngine.Events;

namespace Lean.Common
{
	/// <summary>This component allows you to convert 1, 2, or 3 values from one range to another. For example, an angle in the range of -90..90 could be converted to 0..1. This is done by calling one of the <b>SetX/Y/Z</b> methods, and then sending it out using the <b>OnValueX/Y/Z</b> events.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanRemapValue")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Remap Value")]
	public class LeanRemapValue : MonoBehaviour
	{
		[System.Serializable] public class FloatEvent : UnityEvent<float> {}
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}

		/// <summary>The range of the input values.</summary>
		public Vector3 OldMin { set { oldMin = value; } get { return oldMin; } } [SerializeField] private Vector3 oldMin;

		/// <summary>The range of the input values.</summary>
		public Vector3 OldMax { set { oldMax = value; } get { return oldMax; } } [SerializeField] private Vector3 oldMax = Vector3.one;

		/// <summary>The range of the output values.</summary>
		public Vector3 NewMin { set { newMin = value; } get { return newMin; } } [SerializeField] private Vector3 newMin;

		/// <summary>The range of the output values.</summary>
		public Vector3 NewMax { set { newMax = value; } get { return newMax; } } [SerializeField] private Vector3 newMax = Vector3.one;

		/// <summary>Should the values be clamped to the specified ranges?</summary>
		//public bool Clamp { set { clamp = value; } get { return clamp; } } [SerializeField] private bool clamp;

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

		/// <summary>This method allows you to set the X axis.</summary>
		public void SetX(float x)
		{
			Remap(new Vector3(x, 0.0f, 0.0f));
		}

		/// <summary>This method allows you to set the Y axis.</summary>
		public void SetY(float y)
		{
			Remap(new Vector3(0.0f, y, 0.0f));
		}

		/// <summary>This method allows you to set the Z axis.</summary>
		public void SetZ(float z)
		{
			Remap(new Vector3(0.0f, 0.0f, z));
		}

		/// <summary>This method allows you to set the XY axis.</summary>
		public void SetXY(Vector2 xy)
		{
			Remap(new Vector3(xy.x, xy.y, 0.0f));
		}

		/// <summary>This method allows you to set the XYZ axis.</summary>
		public void SetXYZ(Vector3 xyz)
		{
			Remap(new Vector3(xyz.x, xyz.y, xyz.z));
		}

		private void Remap(Vector3 value)
		{
			value = value - oldMin;

			var oldRange = oldMax - oldMin;
			
			if (oldRange.x != 0.0f) value.x /= oldRange.x;
			if (oldRange.y != 0.0f) value.y /= oldRange.y;
			if (oldRange.z != 0.0f) value.z /= oldRange.z;

			var newRange = newMax - newMin;

			value.x *= newRange.x;
			value.y *= newRange.y;
			value.z *= newRange.z;

			value += newMin;

			if (onValueX != null)
			{
				onValueX.Invoke(value.x);
			}

			if (onValueY != null)
			{
				onValueY.Invoke(value.y);
			}

			if (onValueZ != null)
			{
				onValueZ.Invoke(value.z);
			}

			if (onValueXY != null)
			{
				onValueXY.Invoke(value);
			}

			if (onValueXYZ != null)
			{
				onValueXYZ.Invoke(value);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanRemapValue;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanRemapValue_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("oldMin", "The range of the input values.");
			Draw("oldMax", "The range of the input values.");

			Separator();

			Draw("newMin", "The range of the output values.");
			Draw("newMax", "The range of the output values.");

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