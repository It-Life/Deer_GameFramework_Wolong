using UnityEngine;
using UnityEngine.Events;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to perform a continuous action based on the current position of the fingers relative to where they started.
	/// This allows you to perform actions like an invisible joystick.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanMultiPull")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Multi Pull")]
	public class LeanMultiPull : MonoBehaviour
	{
		public enum CoordinateType
		{
			ScaledPixels,
			ScreenPixels,
			ScreenPercentage
		}

		[System.Serializable] public class FloatEvent : UnityEvent<float> {}
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}
		[System.Serializable] public class Vector3Vector3Event : UnityEvent<Vector3, Vector3> {}

		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>The coordinate space of the OnDelta values.</summary>
		public CoordinateType Coordinate { set { coordinate = value; } get { return coordinate; } } [FSA("Coordinate")] [SerializeField] private CoordinateType coordinate;

		/// <summary>The delta values will be multiplied by this when output.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [FSA("Multiplier")] [SerializeField] private float multiplier = 1.0f;

		/// <summary>If you enable this then the delta values will be multiplied by Time.deltaTime. This allows you to maintain frame rate independent actions.</summary>
		public bool ScaleByTime { set { scaleByTime = value; } get { return scaleByTime; } } [FSA("ScaleByTime")] [SerializeField] private bool scaleByTime = true;

		/// <summary>This event is invoked when the requirements are met.
		/// Vector2 = Position Delta based on your Delta setting.</summary>
		public Vector2Event OnVector { get { if (onVector == null) onVector = new Vector2Event(); return onVector; } } [SerializeField] private Vector2Event onVector;

		/// <summary>Called on the first frame the conditions are met.
		/// Float = The distance/magnitude/length of the swipe delta vector.</summary>
		public FloatEvent OnDistance { get { if (onDistance == null) onDistance = new FloatEvent(); return onDistance; } } [SerializeField] private FloatEvent onDistance;

		/// <summary>The method used to find world coordinates from a finger. See LeanScreenDepth documentation for more information.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.DepthIntercept);

		/// <summary>Called on the first frame the conditions are met.
		/// Vector3 = Start point in world space.</summary>
		public Vector3Event OnWorldFrom { get { if (onWorldFrom == null) onWorldFrom = new Vector3Event(); return onWorldFrom; } } [SerializeField] private Vector3Event onWorldFrom;

		/// <summary>Called on the first frame the conditions are met.
		/// Vector3 = End point in world space.</summary>
		public Vector3Event OnWorldTo { get { if (onWorldTo == null) onWorldTo = new Vector3Event(); return onWorldTo; } } [SerializeField] private Vector3Event onWorldTo;

		/// <summary>Called on the first frame the conditions are met.
		/// Vector3 = The vector between the start and end points in world space.</summary>
		public Vector3Event OnWorldDelta { get { if (onWorldDelta == null) onWorldDelta = new Vector3Event(); return onWorldDelta; } } [SerializeField] private Vector3Event onWorldDelta;

		/// <summary>Called on the first frame the conditions are met.
		/// Vector3 = Start point in world space.
		/// Vector3 = End point in world space.</summary>
		public Vector3Vector3Event OnWorldFromTo { get { if (onWorldFromTo == null) onWorldFromTo = new Vector3Vector3Event(); return onWorldFromTo; } } [SerializeField] private Vector3Vector3Event onWorldFromTo;

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually add a finger.</summary>
		public void AddFinger(LeanFinger finger)
		{
			Use.AddFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove a finger.</summary>
		public void RemoveFinger(LeanFinger finger)
		{
			Use.RemoveFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove all fingers.</summary>
		public void RemoveAllFingers()
		{
			Use.RemoveAllFingers();
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}
#endif

		protected virtual void Awake()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}

		protected virtual void Update()
		{
			// Get fingers
			var fingers = Use.UpdateAndGetFingers();

			if (fingers.Count > 0)
			{
				var screenFrom = LeanGesture.GetStartScreenCenter(fingers);
				var screenTo   = LeanGesture.GetScreenCenter(fingers);
				var finalDelta = screenTo - screenFrom;
				var timeScale  = 1.0f;

				if (scaleByTime == true)
				{
					timeScale = Time.deltaTime;
				}

				switch (coordinate)
				{
					case CoordinateType.ScaledPixels:     finalDelta *= LeanTouch.ScalingFactor; break;
					case CoordinateType.ScreenPercentage: finalDelta *= LeanTouch.ScreenFactor;  break;
				}

				finalDelta *= multiplier;

				if (onVector != null)
				{
					onVector.Invoke(finalDelta * timeScale);
				}

				if (onDistance != null)
				{
					onDistance.Invoke(finalDelta.magnitude * timeScale);
				}

				var worldFrom = ScreenDepth.Convert(screenFrom, gameObject);
				var worldTo   = ScreenDepth.Convert(screenTo  , gameObject);

				if (onWorldFrom != null)
				{
					onWorldFrom.Invoke(worldFrom);
				}

				if (onWorldTo != null)
				{
					onWorldTo.Invoke(worldTo);
				}

				if (onWorldDelta != null)
				{
					onWorldDelta.Invoke((worldTo - worldFrom) * timeScale);
				}

				if (onWorldFromTo != null)
				{
					onWorldFromTo.Invoke(worldFrom, worldTo);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanMultiPull;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanMultiPull_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Use");

			Separator();

			var usedA = Any(tgts, t => t.OnVector.GetPersistentEventCount() > 0);
			var usedB = Any(tgts, t => t.OnDistance.GetPersistentEventCount() > 0);
			var usedC = Any(tgts, t => t.OnWorldFrom.GetPersistentEventCount() > 0);
			var usedD = Any(tgts, t => t.OnWorldTo.GetPersistentEventCount() > 0);
			var usedE = Any(tgts, t => t.OnWorldDelta.GetPersistentEventCount() > 0);
			var usedF = Any(tgts, t => t.OnWorldFromTo.GetPersistentEventCount() > 0);

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			if (usedA == true || usedB == true || showUnusedEvents == true)
			{
				Draw("coordinate", "The coordinate space of the OnDelta values.");
				Draw("multiplier", "The delta values will be multiplied by this when output.");
			}

			if (usedA == true || usedB == true || usedE == true || showUnusedEvents == true)
			{
				Draw("scaleByTime", "If you enable this then the delta values will be multiplied by Time.deltaTime. This allows you to maintain frame rate independent actions.");
			}

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onVector");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onDistance");
			}

			if (usedC == true || usedD == true || usedE == true || usedF == true || showUnusedEvents == true)
			{
				Draw("ScreenDepth");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onWorldFrom");
			}

			if (usedD == true || showUnusedEvents == true)
			{
				Draw("onWorldTo");
			}

			if (usedE == true || showUnusedEvents == true)
			{
				Draw("onWorldDelta");
			}

			if (usedF == true || showUnusedEvents == true)
			{
				Draw("onWorldFromTo");
			}
		}
	}
}
#endif