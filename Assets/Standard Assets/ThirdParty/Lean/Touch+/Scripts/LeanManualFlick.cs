using UnityEngine;
using System.Collections.Generic;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component works like LeanFingerSwipe, but you must manually add fingers from components like LeanFingerDown, LeanFingerDownCanvas, etc.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanManualFlick")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Manual Flick")]
	public class LeanManualFlick : LeanSwipeBase
	{
		[System.Serializable]
		public class FingerData : LeanFingerData
		{
			public bool Flicked;
		}

		public enum CheckType
		{
			Default,
			IgnoreAge,
			Multiple
		}

		/// <summary>Ignore fingers with OverGui?</summary>
		public bool IgnoreIsOverGui { set { ignoreIsOverGui = value; } get { return ignoreIsOverGui; } } [FSA("IgnoreIsOverGui")] [SerializeField] private bool ignoreIsOverGui;

		/// <summary>If the specified object is set and isn't selected, then this component will do nothing.</summary>
		public LeanSelectable RequiredSelectable { set { requiredSelectable = value; } get { return requiredSelectable; } } [FSA("RequiredSelectable")] [SerializeField] private LeanSelectable requiredSelectable;

		/// <summary>This allows you to choose how the flick will be detected.
		/// Default = Detects one flick within the current <b>TapThreshold</b> time.
		/// IgnoreAge = You can hold the finger for any duration before flicking.
		/// Multiple = You can stop moving the finger for <b>TapThreshold</b> seconds and perform additional flicks.</summary>
		public CheckType Check { set { check = value; } get { return check; } } [FSA("Check")] [SerializeField] private CheckType check;

		// Additional finger data
		[SerializeField]
		private List<FingerData> fingerDatas;

		/// <summary>This method allows you to manually add a finger.</summary>
		public void AddFinger(LeanFinger finger)
		{
			var fingerData = LeanFingerData.FindOrCreate(ref fingerDatas, finger);

			fingerData.Flicked = false;
		}

		/// <summary>This method allows you to manually remove a finger.</summary>
		public void RemoveFinger(LeanFinger finger)
		{
			LeanFingerData.Remove(fingerDatas, finger);
		}

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
			LeanTouch.OnFingerUp += HandleFingerUp;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerUp -= HandleFingerUp;
		}

		protected virtual void Update()
		{
			if (fingerDatas != null)
			{
				for (var i = fingerDatas.Count - 1; i >= 0; i--)
				{
					var fingerData = fingerDatas[i];
					var finger     = fingerData.Finger;
					var screenFrom = finger.GetSnapshotScreenPosition(finger.Age - LeanTouch.CurrentTapThreshold);
					var screenTo   = finger.ScreenPosition;

					if (Vector2.Distance(screenFrom, screenTo) > LeanTouch.CurrentSwipeThreshold / LeanTouch.ScalingFactor)
					{
						if (fingerData.Flicked == false && TestFinger(finger, screenFrom, screenTo) == true)
						{
							fingerData.Flicked = true;

							HandleFingerSwipe(finger, screenFrom, screenTo);

							// If multi-flicks aren't allowed, remove the finger
							if (check != CheckType.Multiple)
							{
								LeanFingerData.Remove(fingerDatas, finger);
							}
						}
					}
					else
					{
						fingerData.Flicked = false;
					}
				}
			}
		}

		private void HandleFingerUp(LeanFinger finger)
		{
			LeanFingerData.Remove(fingerDatas, finger);
		}

		private bool TestFinger(LeanFinger finger, Vector2 screenFrom, Vector2 screenTo)
		{
			if (ignoreIsOverGui == true && finger.IsOverGui == true)
			{
				return false;
			}

			if (requiredSelectable != null && requiredSelectable.IsSelected == false)
			{
				return false;
			}

			if (check == CheckType.Default && finger.Age >= LeanTouch.CurrentTapThreshold)
			{
				return false;
			}

			return AngleIsValid(screenTo - screenFrom);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanManualFlick;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanManualFlick_Editor : LeanSwipeBase_Editor
	{
		protected override void OnInspector()
		{
			Draw("ignoreIsOverGui", "Ignore fingers with OverGui?");
			Draw("requiredSelectable", "If the specified object is set and isn't selected, then this component will do nothing.");
			Draw("check", "This allows you to choose how the flick will be detected.\n\nDefault = Detects one flick within the current TapThreshold time.\n\nIgnoreAge = You can hold the finger for any duration before flicking.\n\nMultiple = You can stop moving the finger for TapThreshold seconds and perform additional flicks.");

			base.OnInspector();
		}
	}
}
#endif