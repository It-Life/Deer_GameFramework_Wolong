using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to get the change in position of all specified fingers.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanMultiUpdate")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Multi Update")]
	public class LeanMultiUpdate : MonoBehaviour
	{
		public enum CoordinateType
		{
			ScaledPixels,
			ScreenPixels,
			ScreenPercentage
		}

		[System.Serializable] public class LeanFingerListEvent : UnityEvent<List<LeanFinger>> {}
		[System.Serializable] public class FloatEvent : UnityEvent<float> {}
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}
		[System.Serializable] public class Vector3Vector3Event : UnityEvent<Vector3, Vector3> {}

		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>If the fingers didn't move, skip calling </summary>
		public bool IgnoreIfStatic { set { ignoreIfStatic = value; } get { return ignoreIfStatic; } } [FSA("IgnoreIfStatic")] [SerializeField] private bool ignoreIfStatic;

		/// <summary>This event is invoked when the requirements are met.
		/// List<LeanFinger> = The fingers that are touching the screen.</summary>
		public LeanFingerListEvent OnFingers { get { if (onFingers == null) onFingers = new LeanFingerListEvent(); return onFingers; } } [FSA("onSet")] [SerializeField] private LeanFingerListEvent onFingers;

		/// <summary>The coordinate space of the OnDelta values.</summary>
		public CoordinateType Coordinate { set { coordinate = value; } get { return coordinate; } } [FSA("Coordinate")] [SerializeField] private CoordinateType coordinate;

		/// <summary>The delta values will be multiplied by this when output.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [FSA("Multiplier")] [SerializeField] private float multiplier = 1.0f;

		/// <summary>This event is invoked when the requirements are met.
		/// Vector2 = Position Delta based on your Delta setting.</summary>
		public Vector2Event OnDelta { get { if (onDelta == null) onDelta = new Vector2Event(); return onDelta; } } [FSA("onDragDelta")] [SerializeField] private Vector2Event onDelta;

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
			// Get an initial list of fingers
			var fingers = Use.UpdateAndGetFingers();

			if (fingers.Count > 0)
			{
				// Get delta
				var screenFrom = LeanGesture.GetLastScreenCenter(fingers);
				var screenTo   = LeanGesture.GetScreenCenter(fingers);
				var finalDelta = screenTo - screenFrom;

				if (ignoreIfStatic == true && finalDelta.sqrMagnitude <= 0.0f)
				{
					return;
				}

				if (onFingers != null)
				{
					onFingers.Invoke(fingers);
				}

				switch (coordinate)
				{
					case CoordinateType.ScaledPixels:     finalDelta *= LeanTouch.ScalingFactor; break;
					case CoordinateType.ScreenPercentage: finalDelta *= LeanTouch.ScreenFactor;  break;
				}

				finalDelta *= multiplier;

				if (onDelta != null)
				{
					onDelta.Invoke(finalDelta);
				}

				if (onDistance != null)
				{
					onDistance.Invoke(finalDelta.magnitude);
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
					onWorldDelta.Invoke(worldTo - worldFrom);
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
	using TARGET = LeanMultiUpdate;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanMultiUpdate_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Use");
			Draw("ignoreIfStatic", "If the finger didn't move, ignore it?");

			Separator();

			var usedA = Any(tgts, t => t.OnFingers.GetPersistentEventCount() > 0);
			var usedB = Any(tgts, t => t.OnDelta.GetPersistentEventCount() > 0);
			var usedC = Any(tgts, t => t.OnDistance.GetPersistentEventCount() > 0);
			var usedD = Any(tgts, t => t.OnWorldFrom.GetPersistentEventCount() > 0);
			var usedE = Any(tgts, t => t.OnWorldTo.GetPersistentEventCount() > 0);
			var usedF = Any(tgts, t => t.OnWorldDelta.GetPersistentEventCount() > 0);
			var usedG = Any(tgts, t => t.OnWorldFromTo.GetPersistentEventCount() > 0);

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onFingers");
			}

			if (usedB == true || usedC == true || showUnusedEvents == true)
			{
				Draw("coordinate", "The coordinate space of the OnDelta values.");
				Draw("multiplier", "The delta values will be multiplied by this when output.");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onDelta");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onDistance");
			}

			if (usedD == true || usedE == true || usedF == true || usedG == true || showUnusedEvents == true)
			{
				Draw("ScreenDepth");
			}

			if (usedD == true || showUnusedEvents == true)
			{
				Draw("onWorldFrom");
			}

			if (usedE == true || showUnusedEvents == true)
			{
				Draw("onWorldTo");
			}

			if (usedF == true || showUnusedEvents == true)
			{
				Draw("onWorldDelta");
			}

			if (usedG == true || showUnusedEvents == true)
			{
				Draw("onWorldFromTo");
			}
		}
	}
}
#endif