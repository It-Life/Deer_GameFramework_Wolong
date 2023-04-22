using UnityEngine;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component fires events if a finger has swiped across the screen.
	/// A swipe is defined as a touch that began and ended within the LeanTouch.TapThreshold time, and moved more than the LeanTouch.SwipeThreshold distance.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanFingerSwipe")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Finger Swipe")]
	public class LeanFingerSwipe : LeanSwipeBase
	{
		/// <summary>Ignore fingers with StartedOverGui?</summary>
		public bool IgnoreStartedOverGui { set { ignoreStartedOverGui = value; } get { return ignoreStartedOverGui; } } [FSA("IgnoreStartedOverGui")] [SerializeField] private bool ignoreStartedOverGui = true;

		/// <summary>Ignore fingers with OverGui?</summary>
		public bool IgnoreIsOverGui { set { ignoreIsOverGui = value; } get { return ignoreIsOverGui; } } [FSA("IgnoreIsOverGui")] [SerializeField] private bool ignoreIsOverGui;

		/// <summary>If the specified object is set and isn't selected, then this component will do nothing.</summary>
		public LeanSelectable RequiredSelectable { set { requiredSelectable = value; } get { return requiredSelectable; } } [FSA("RequiredSelectable")] [SerializeField] private LeanSelectable requiredSelectable;

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			requiredSelectable = GetComponentInParent<LeanSelectable>();
		}
#endif

		protected virtual void Start()
		{
			if (requiredSelectable == null)
			{
				requiredSelectable = GetComponentInParent<LeanSelectable>();
			}
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerSwipe += HandleFingerSwipe;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerSwipe -= HandleFingerSwipe;
		}

		private void HandleFingerSwipe(LeanFinger finger)
		{
			if (ignoreStartedOverGui == true && finger.StartedOverGui == true)
			{
				return;
			}

			if (ignoreIsOverGui == true && finger.IsOverGui == true)
			{
				return;
			}

			if (requiredSelectable != null && requiredSelectable.IsSelected == false)
			{
				return;
			}

			HandleFingerSwipe(finger, finger.StartScreenPosition, finger.ScreenPosition);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanFingerSwipe;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanFingerSwipe_Editor : LeanSwipeBase_Editor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("ignoreStartedOverGui", "Ignore fingers with StartedOverGui?");
			Draw("ignoreIsOverGui", "Ignore fingers with OverGui?");
			Draw("requiredSelectable", "If the specified object is set and isn't selected, then this component will do nothing.");

			base.OnInspector();
		}
	}
}
#endif