using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This script calculates the multi-tap event.
	/// A multi-tap is where you press and release at least one finger at the same time.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanMultiTap")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Multi Tap")]
	public class LeanMultiTap : MonoBehaviour
	{
		[System.Serializable] public class IntEvent : UnityEvent<int> {}
		[System.Serializable] public class IntIntEvent : UnityEvent<int, int> {}

		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>Called when a multi-tap occurs.</summary>
		public UnityEvent OnTap { get { if (onTap == null) onTap = new UnityEvent(); return onTap; } } [SerializeField] private UnityEvent onTap;

		/// <summary>Called when a multi-tap occurs.
		/// Int = The amount of times you've multi-tapped.</summary>
		public IntEvent OnCount { get { if (onCount == null) onCount = new IntEvent(); return onCount; } } [SerializeField] private IntEvent onCount;

		/// <summary>Called when a multi-tap occurs.
		/// Int = The maximum amount of fingers involved in this multi-tap.</summary>
		public IntEvent OnHighest { get { if (onHighest == null) onHighest = new IntEvent(); return onHighest; } } [SerializeField] private IntEvent onHighest;

		/// <summary>Called when a multi-tap occurs.
		/// Int = The amount of times you've multi-tapped.
		/// Int = The maximum amount of fingers involved in this multi-tap.</summary>
		public IntIntEvent OnCountHighest { get { if (onCountHighest == null) onCountHighest = new IntIntEvent(); return onCountHighest; } } [FSA("OnTap")] [SerializeField] private IntIntEvent onCountHighest;

		// Seconds at least one finger has been held down
		private float age;

		// Previous fingerCount
		private int lastFingerCount;

		/// <summary>This is set to the current multi-tap count.</summary>
		private int multiTapCount;

		/// <summary>Highest number of fingers held down during this multi-tap.</summary>
		private int highestFingerCount;

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
			// Get fingers and calculate how many are still touching the screen
			var fingers     = Use.UpdateAndGetFingers();
			var fingerCount = GetFingerCount(fingers);

			// At least one finger set?
			if (fingerCount > 0)
			{
				// Did this just begin?
				if (lastFingerCount == 0)
				{
					age                = 0.0f;
					highestFingerCount = fingerCount;
				}
				else if (fingerCount > highestFingerCount)
				{
					highestFingerCount = fingerCount;
				}
			}

			age += Time.unscaledDeltaTime;

			// Is a multi-tap still eligible?
			if (age <= LeanTouch.CurrentTapThreshold)
			{
				// All fingers released?
				if (fingerCount == 0 && lastFingerCount > 0)
				{
					multiTapCount += 1;

					if (onTap != null)
					{
						onTap.Invoke();
					}

					if (onCount != null)
					{
						onCount.Invoke(multiTapCount);
					}

					if (onHighest != null)
					{
						onHighest.Invoke(highestFingerCount);
					}

					if (onCountHighest != null)
					{
						onCountHighest.Invoke(multiTapCount, highestFingerCount);
					}
				}
			}
			// Reset
			else
			{
				multiTapCount      = 0;
				highestFingerCount = 0;
			}

			lastFingerCount = fingerCount;
		}

		private int GetFingerCount(List<LeanFinger> fingers)
		{
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				if (fingers[i].Up == false)
				{
					count += 1;
				}
			}

			return count;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanMultiTap;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanMultiTap_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Use");

			Separator();

			var usedA = Any(tgts, t => t.OnTap.GetPersistentEventCount() > 0);
			var usedB = Any(tgts, t => t.OnCount.GetPersistentEventCount() > 0);
			var usedC = Any(tgts, t => t.OnHighest.GetPersistentEventCount() > 0);
			var usedD = Any(tgts, t => t.OnCountHighest.GetPersistentEventCount() > 0);

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onTap");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onCount");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onHighest");
			}

			if (usedD == true || showUnusedEvents == true)
			{
				Draw("onCountHighest");
			}
		}
	}
}
#endif