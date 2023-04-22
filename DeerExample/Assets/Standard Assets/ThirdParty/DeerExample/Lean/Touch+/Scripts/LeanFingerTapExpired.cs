using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections.Generic;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This script calls the OnFingerTap event when a finger expires after it's tapped a specific amount of times.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanFingerTapExpired")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Finger Tap Expired")]
	public class LeanFingerTapExpired : MonoBehaviour
	{
		[System.Serializable] public class LeanFingerEvent : UnityEvent<LeanFinger> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}
		[System.Serializable] public class IntEvent : UnityEvent<int> {}

		/// <summary>Ignore fingers with StartedOverGui?</summary>
		public bool IgnoreStartedOverGui { set { ignoreStartedOverGui = value; } get { return ignoreStartedOverGui; } } [FSA("IgnoreStartedOverGui")] [SerializeField] private bool ignoreStartedOverGui = true;

		/// <summary>Ignore fingers with OverGui?</summary>
		public bool IgnoreIsOverGui { set { ignoreIsOverGui = value; } get { return ignoreIsOverGui; } } [FSA("IgnoreIsOverGui")] [SerializeField] private bool ignoreIsOverGui;

		/// <summary>If the specified object is set and isn't selected, then this component will do nothing.</summary>
		public LeanSelectable RequiredSelectable { set { requiredSelectable = value; } get { return requiredSelectable; } } [FSA("RequiredSelectable")] [SerializeField] private LeanSelectable requiredSelectable;

		/// <summary>How many times must this finger tap before OnTap gets called?
		/// 0 = Every time (keep in mind OnTap will only be called once if you use this).</summary>
		public int RequiredTapCount { set { requiredTapCount = value; } get { return requiredTapCount; } } [FSA("RequiredTapCount")] [SerializeField] private int requiredTapCount;

		/// <summary>How many times repeating must this finger tap before OnTap gets called?
		/// 0 = Every time (e.g. a setting of 2 means OnTap will get called when you tap 2 times, 4 times, 6, 8, 10, etc).</summary>
		public int RequiredTapInterval { set { requiredTapInterval = value; } get { return requiredTapInterval; } } [FSA("RequiredTapInterval")] [SerializeField] private int requiredTapInterval;

		/// <summary>Called on the first frame the conditions are met.</summary>
		public LeanFingerEvent OnFinger { get { if (onFinger == null) onFinger = new LeanFingerEvent(); return onFinger; } } [FormerlySerializedAs("onTap")] [FormerlySerializedAs("OnTap")] [SerializeField] private LeanFingerEvent onFinger;

		/// <summary>Called on the first frame the conditions are met.
		/// Int = The finger tap count.</summary>
		public IntEvent OnCount { get { if (onCount == null) onCount = new IntEvent(); return onCount; } } [SerializeField] private IntEvent onCount;

		/// <summary>The method used to find world coordinates from a finger. See LeanScreenDepth documentation for more information.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.DepthIntercept);

		/// <summary>Called on the first frame the conditions are met.
		/// Vector3 = Start point based on the ScreenDepth settings.</summary>
		public Vector3Event OnWorld { get { if (onWorld == null) onWorld = new Vector3Event(); return onWorld; } } [SerializeField] private Vector3Event onWorld;

		private List<LeanFinger> ignoreFingers = new List<LeanFinger>();

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			requiredSelectable = GetComponentInParent<LeanSelectable>();
		}
#endif

		protected virtual void Awake()
		{
			if (requiredSelectable == null)
			{
				requiredSelectable = GetComponentInParent<LeanSelectable>();
			}
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerTap     += HandleFingerTap;
			LeanTouch.OnFingerExpired += HandleFingerExpired;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerTap     -= HandleFingerTap;
			LeanTouch.OnFingerExpired -= HandleFingerExpired;
		}

		private void HandleFingerTap(LeanFinger finger)
		{
			if (ignoreIsOverGui == true && finger.IsOverGui == true && ignoreFingers.Contains(finger) == false)
			{
				ignoreFingers.Add(finger);
			}
		}

		private void HandleFingerExpired(LeanFinger finger)
		{
			// Ignore?
			if (ignoreFingers.Contains(finger) == true)
			{
				ignoreFingers.Remove(finger);

				return;
			}

			if (finger.TapCount == 0)
			{
				return;
			}

			if (ignoreStartedOverGui == true && finger.StartedOverGui == true)
			{
				return;
			}

			if (requiredTapCount > 0 && finger.TapCount != requiredTapCount)
			{
				return;
			}

			if (requiredTapInterval > 0 && (finger.TapCount % requiredTapInterval) != 0)
			{
				return;
			}

			if (requiredSelectable != null && requiredSelectable.IsSelected == false)
			{
				return;
			}

			if (onFinger != null)
			{
				onFinger.Invoke(finger);
			}

			if (onCount != null)
			{
				onCount.Invoke(finger.TapCount);
			}

			if (onWorld != null)
			{
				var position = ScreenDepth.Convert(finger.StartScreenPosition, gameObject);

				onWorld.Invoke(position);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanFingerTapExpired;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanFingerTapExpired_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("ignoreStartedOverGui", "Ignore fingers with StartedOverGui?");
			Draw("ignoreIsOverGui", "Ignore fingers with OverGui?");
			Draw("requiredSelectable", "If the specified object is set and isn't selected, then this component will do nothing.");
			Draw("requiredTapCount", "How many times must this finger tap before OnTap gets called?\n\n0 = Every time (keep in mind OnTap will only be called once if you use this).");
			Draw("requiredTapInterval", "How many times repeating must this finger tap before OnTap gets called?\n\n0 = Every time (e.g. a setting of 2 means OnTap will get called when you tap 2 times, 4 times, 6, 8, 10, etc).");

			Separator();

			var usedA = Any(tgts, t => t.OnFinger.GetPersistentEventCount() > 0);
			var usedB = Any(tgts, t => t.OnCount.GetPersistentEventCount() > 0);
			var usedC = Any(tgts, t => t.OnWorld.GetPersistentEventCount() > 0);

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onFinger");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onCount");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("ScreenDepth");
				Draw("onWorld");
			}
		}
	}
}
#endif