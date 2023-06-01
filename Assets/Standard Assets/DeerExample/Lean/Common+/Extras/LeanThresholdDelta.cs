using UnityEngine;
using UnityEngine.Events;

namespace Lean.Common
{
	/// <summary>This component allows you to accumulate delta changes until they reach a threshold delta, and then output them.
	/// This is useful for making more precise movements when using inaccurate touch inputs.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanThresholdDelta")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Threshold Delta")]
	public class LeanThresholdDelta : MonoBehaviour
	{
		[System.Serializable] public class FloatEvent : UnityEvent<float> {}
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}

		/// <summary>The current accumulated delta.</summary>
		public Vector3 Current;

		/// <summary>When any dimension of <b>Current</b> exceeds this, <b>OnDelta___</b> will be called, and <b>Current</b> will be rolled back.</summary>
		public float Threshold = 1.0f;

		/// <summary>If you enable this then the delta will step in increments based on the <b>Threshold</b> value. If you disable this then the position will immediately be set to the <b>Current</b> value.</summary>
		public bool Step;

		public FloatEvent OnDeltaX { get { if (onDeltaX == null) onDeltaX = new FloatEvent(); return onDeltaX; } } [SerializeField] private FloatEvent onDeltaX;

		public FloatEvent OnDeltaY { get { if (onDeltaY == null) onDeltaY = new FloatEvent(); return onDeltaY; } } [SerializeField] private FloatEvent onDeltaY;

		public FloatEvent OnDeltaZ { get { if (onDeltaZ == null) onDeltaZ = new FloatEvent(); return onDeltaZ; } } [SerializeField] private FloatEvent onDeltaZ;

		public Vector2Event OnDeltaXY { get { if (onDeltaXY == null) onDeltaXY = new Vector2Event(); return onDeltaXY; } } [SerializeField] private Vector2Event onDeltaXY;

		public Vector3Event OnDeltaXYZ { get { if (onDeltaXYZ == null) onDeltaXYZ = new Vector3Event(); return onDeltaXYZ; } } [SerializeField] private Vector3Event onDeltaXYZ;

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

		protected virtual void Update()
		{
			var delta = Current;

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
					delta.x = stepX * Threshold;
					delta.y = stepY * Threshold;
					delta.z = stepZ * Threshold;

					Current -= delta;
				}
				else
				{
					Current = Vector3.zero;
				}
			}
			else
			{
				if (delta.x == 0.0f && delta.y == 0.0f && delta.z == 0.0f)
				{
					return;
				}

				Current = Vector3.zero;
			}

			if (onDeltaX != null)
			{
				onDeltaX.Invoke(delta.x);
			}

			if (onDeltaY != null)
			{
				onDeltaY.Invoke(delta.y);
			}

			if (onDeltaZ != null)
			{
				onDeltaZ.Invoke(delta.z);
			}

			if (onDeltaXY != null)
			{
				onDeltaXY.Invoke(delta);
			}

			if (onDeltaXYZ != null)
			{
				onDeltaXYZ.Invoke(delta);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanThresholdDelta;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanThresholdDelta_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Current", "The current accumulated delta.");
			Draw("Threshold", "When any dimension of Value exceeds this, OnDelta___ will be called, and Value will be rolled back.");
			Draw("Step", "If you enable this then the delta will step in increments based on the Threshold value. If you disable this then the position will immediately be set to the Current value.");

			Separator();

			var usedA = Any(tgts, t => t.OnDeltaX.GetPersistentEventCount() > 0);
			var usedB = Any(tgts, t => t.OnDeltaY.GetPersistentEventCount() > 0);
			var usedC = Any(tgts, t => t.OnDeltaZ.GetPersistentEventCount() > 0);
			var usedD = Any(tgts, t => t.OnDeltaXY.GetPersistentEventCount() > 0);
			var usedE = Any(tgts, t => t.OnDeltaXYZ.GetPersistentEventCount() > 0);

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onDeltaX");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onDeltaY");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onDeltaZ");
			}

			if (usedD == true || showUnusedEvents == true)
			{
				Draw("onDeltaXY");
			}

			if (usedE == true || showUnusedEvents == true)
			{
				Draw("onDeltaXYZ");
			}
		}
	}
}
#endif