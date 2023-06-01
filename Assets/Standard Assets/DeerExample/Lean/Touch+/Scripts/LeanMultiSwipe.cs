using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This script calculates the multi-swipe event.
	/// A multi-swipe is where you swipe multiple fingers at the same time, and OnSwipe gets called when the first finger is released from the screen.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanMultiSwipe")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Multi Swipe")]
	public class LeanMultiSwipe : MonoBehaviour
	{
		[System.Serializable] public class FingerListEvent : UnityEvent<List<LeanFinger>> {}
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}
		[System.Serializable] public class FloatEvent : UnityEvent<float> {}

		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>Each finger touching the screen must have moved at least this distance for a multi swipe to be considered. This prevents the scenario where multiple fingers are touching, but only one swipes.</summary>
		public float ScaledDistanceThreshold { set { scaledDistanceThreshold = value; } get { return scaledDistanceThreshold; } } [FSA("ScaledDistanceThreshold")] [SerializeField] private float scaledDistanceThreshold = 50.0f;

		/// <summary>This allows you to set the maximum angle between parallel swiping fingers for the OnSwipeParallel event to be fired.</summary>
		public float ParallelAngleThreshold { set { parallelAngleThreshold = value; } get { return parallelAngleThreshold; } } [FSA("ParallelAngleThreshold")] [SerializeField] private float parallelAngleThreshold = 20.0f;

		/// <summary>This allows you to set the minimum pinch distance for the OnSwipeIn and OnSwipeOut events to be fired.</summary>
		public float PinchScaledDistanceThreshold { set { pinchScaledDistanceThreshold = value; } get { return pinchScaledDistanceThreshold; } } [FSA("PinchScaledDistanceThreshold")] [SerializeField] private float pinchScaledDistanceThreshold = 100.0f;

		// Called when a multi-swipe occurs
		public FingerListEvent OnFingers { get { if (onFingers == null) onFingers = new FingerListEvent(); return onFingers; } } [FSA("onSwipe")] [FSA("OnSwipe")] [SerializeField] private FingerListEvent onFingers;

		// Called when a multi-swipe occurs where each finger moves parallel to each other (Vector2 = ScaledDirection)
		public Vector2Event OnSwipeParallel { get { if (onSwipeParallel == null) onSwipeParallel = new Vector2Event(); return onSwipeParallel; } } [FSA("OnSwipeParallel")] [SerializeField] private Vector2Event onSwipeParallel;

		// Called when a multi-swipe occurs where each finger pinches in (Float = ScaledDistance)
		public FloatEvent OnSwipeIn { get { if (onSwipeIn == null) onSwipeIn = new FloatEvent(); return onSwipeIn; } } [FSA("OnSwipeIn")] [SerializeField] private FloatEvent onSwipeIn;

		// Called when a multi-swipe occurs where each finger pinches out (Float = ScaledDistance)
		public FloatEvent OnSwipeOut { get { if (onSwipeOut == null) onSwipeOut = new FloatEvent(); return onSwipeOut; } } [FSA("OnSwipeOut")] [SerializeField] private FloatEvent onSwipeOut;

		// Set to prevent multiple invocation
		private bool swiped;

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
			// Get all valid fingers for swipe
			var fingers = Use.UpdateAndGetFingers();

			if (fingers.Count > 0)
			{
				if (swiped == false)
				{
					for (var i = fingers.Count - 1; i >= 0; i--)
					{
						var finger = fingers[i];

						if (finger.Swipe == true)
						{
							FingerSwipe(fingers, finger);

							break;
						}
					}
				}
			}
			else
			{
				swiped = false;
			}
		}

		private void FingerSwipe(List<LeanFinger> fingers, LeanFinger swipedFinger)
		{
			var scaledDelta = swipedFinger.SwipeScaledDelta;
			var isParallel  = true;

			swiped = true;

			// Go through all fingers
			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				// If it's too old to swipe, skip
				if (finger.Age > LeanTouch.CurrentTapThreshold)
				{
					return;
				}

				// If it didn't move far enough to swipe, skip
				if (finger.SwipeScaledDelta.magnitude < scaledDistanceThreshold)
				{
					return;
				}

				// If the finger didn't move parallel the others, make the OnSwipeParallel event ineligible
				if (finger != swipedFinger)
				{
					var angle = Vector2.Angle(scaledDelta, finger.SwipeScaledDelta);

					if (angle > parallelAngleThreshold)
					{
						isParallel = false;
					}
				}
			}

			if (onFingers != null)
			{
				onFingers.Invoke(fingers);
			}

			if (fingers.Count > 1)
			{
				var centerA = LeanGesture.GetStartScreenCenter(fingers);
				var centerB = LeanGesture.GetScreenCenter(fingers);

				if (onSwipeParallel != null && isParallel == true)
				{
					var delta = centerA - centerB;

					onSwipeParallel.Invoke(delta * LeanTouch.ScalingFactor);
				}
				else
				{
					var pinch = LeanGesture.GetScaledDistance(fingers, centerB) - LeanGesture.GetStartScaledDistance(fingers, centerA);

					if (onSwipeIn != null && pinch <= -pinchScaledDistanceThreshold)
					{
						onSwipeIn.Invoke(-pinch);
					}

					if (onSwipeOut != null && pinch >= pinchScaledDistanceThreshold)
					{
						onSwipeOut.Invoke(pinch);
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanMultiSwipe;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanMultiSwipe_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Use");

			Separator();

			Draw("scaledDistanceThreshold", "Each finger touching the screen must have moved at least this distance for a multi swipe to be considered. This prevents the scenario where multiple fingers are touching, but only one swipes.");
			Draw("parallelAngleThreshold", "This allows you to set the maximum angle between parallel swiping fingers for the OnSwipeParallel event to be fired.");
			Draw("pinchScaledDistanceThreshold", "This allows you to set the minimum pinch distance for the OnSwipeIn and OnSwipeOut events to be fired.");

			Separator();

			var usedA = Any(tgts, t => t.OnFingers.GetPersistentEventCount() > 0);
			var usedB = Any(tgts, t => t.OnSwipeParallel.GetPersistentEventCount() > 0);
			var usedC = Any(tgts, t => t.OnSwipeIn.GetPersistentEventCount() > 0);
			var usedD = Any(tgts, t => t.OnSwipeOut.GetPersistentEventCount() > 0);

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onFingers");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onSwipeParallel");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onSwipeIn");
			}

			if (usedD == true || showUnusedEvents == true)
			{
				Draw("onSwipeOut");
			}
		}
	}
}
#endif