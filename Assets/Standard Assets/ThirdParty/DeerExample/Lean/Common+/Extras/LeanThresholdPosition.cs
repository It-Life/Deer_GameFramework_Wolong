using UnityEngine;
using UnityEngine.Events;

namespace Lean.Common
{
	/// <summary>This component allows you to store the <b>Current</b> position. Once this differs from the <b>Previous</b> position by more than the <b>Threshold</b>, the <b>Previous</b> value will change to match <b>Current</b>, and the <b>OnPosition</b> events will fire with the current position.
	/// This is useful for making more precise movements when using inaccurate touch inputs.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanThresholdPosition")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Threshold Position")]
	public class LeanThresholdPosition : MonoBehaviour
	{
		[System.Serializable] public class FloatEvent : UnityEvent<float> {}
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}

		/// <summary>The current position.</summary>
		public Vector3 Current;

		/// <summary>The previously sent position.</summary>
		public Vector3 Previous;

		/// <summary>When any dimension of <b>Current</b> exceeds this, <b>OnPosition___</b> will be called, and <b>Current</b> will be rolled back.</summary>
		public float Threshold = 1.0f;

		/// <summary>If you enable this then the position will step toward the <b>Current</b> value in increments based on the <b>Threshold</b> value. If you disable this then the position will immediately be set to the <b>Current</b> value.</summary>
		public bool Step;

		public FloatEvent OnPositionX { get { if (onPositionX == null) onPositionX = new FloatEvent(); return onPositionX; } } [SerializeField] private FloatEvent onPositionX;

		public FloatEvent OnPositionY { get { if (onPositionY == null) onPositionY = new FloatEvent(); return onPositionY; } } [SerializeField] private FloatEvent onPositionY;

		public FloatEvent OnPositionZ { get { if (onPositionZ == null) onPositionZ = new FloatEvent(); return onPositionZ; } } [SerializeField] private FloatEvent onPositionZ;

		public Vector2Event OnPositionXY { get { if (onPositionXY == null) onPositionXY = new Vector2Event(); return onPositionXY; } } [SerializeField] private Vector2Event onPositionXY;

		public Vector3Event OnPositionXYZ { get { if (onPositionXYZ == null) onPositionXYZ = new Vector3Event(); return onPositionXYZ; } } [SerializeField] private Vector3Event onPositionXYZ;

		/// <summary>This method allows you to increment <b>Current</b>.</summary>
		public void AddXY(Vector2 delta)
		{
			Current.x += delta.x;
			Current.y += delta.y;
		}

		/// <summary>This method allows you to increment <b>Current</b>.</summary>
		public void AddXYZ(Vector3 delta)
		{
			Current += delta;
		}

		/// <summary>This method allows you to increment <b>Current.x</b>.</summary>
		public void AddX(float delta)
		{
			Current.x += delta;
		}

		/// <summary>This method allows you to increment <b>Current.y</b>.</summary>
		public void AddY(float delta)
		{
			Current.y += delta;
		}

		/// <summary>This method allows you to increment <b>Current.z</b>.</summary>
		public void AddZ(float delta)
		{
			Current.z += delta;
		}

		/// <summary>This method allows you to set the <b>Current</b> position.</summary>
		public void SetXY(Vector2 position)
		{
			Current.x = position.x;
			Current.y = position.y;
		}

		/// <summary>This method allows you to set the <b>Current</b> position.</summary>
		public void SetXYZ(Vector3 position)
		{
			Current = position;
		}

		protected virtual void Update()
		{
			var delta = Current - Previous;

			if (Threshold > 0.0f)
			{
				var stepX = (int)(delta.x / Threshold);
				var stepY = (int)(delta.y / Threshold);
				var stepZ = (int)(delta.z / Threshold);

				if (stepX == 0 && stepY == 0 && stepZ == 0)
				{
					return;
				}

				if (Step == true)
				{
					Previous.x -= stepX * Threshold;
					Previous.y -= stepY * Threshold;
					Previous.z -= stepZ * Threshold;
				}
				else
				{
					Previous = Current;
				}
			}
			else
			{
				if (delta.x == 0.0f && delta.y == 0.0f && delta.z == 0.0f)
				{
					return;
				}

				Previous = Current;
			}

			if (onPositionX != null)
			{
				onPositionX.Invoke(Previous.x);
			}

			if (onPositionXY != null)
			{
				onPositionXY.Invoke(Previous);
			}

			if (onPositionXYZ != null)
			{
				onPositionXYZ.Invoke(Previous);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanThresholdPosition;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanThresholdPosition_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Current", "The current position.");
			Draw("Previous", "The previously sent position.");
			Draw("Threshold", "When any dimension of Value exceeds this, OnPosition___ will be called, and Value will be rolled back.");
			Draw("Step", "If you enable this then the position will step toward the Current value in increments based on the Threshold value. If you disable this then the position will immediately be set to the Current value.");

			Separator();

			var usedA = Any(tgts, t => t.OnPositionX.GetPersistentEventCount() > 0);
			var usedB = Any(tgts, t => t.OnPositionY.GetPersistentEventCount() > 0);
			var usedC = Any(tgts, t => t.OnPositionZ.GetPersistentEventCount() > 0);
			var usedD = Any(tgts, t => t.OnPositionXY.GetPersistentEventCount() > 0);
			var usedE = Any(tgts, t => t.OnPositionXYZ.GetPersistentEventCount() > 0);

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onPositionX");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onPositionY");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onPositionZ");
			}

			if (usedD == true || showUnusedEvents == true)
			{
				Draw("onPositionXY");
			}

			if (usedE == true || showUnusedEvents == true)
			{
				Draw("onPositionXYZ");
			}
		}
	}
}
#endif