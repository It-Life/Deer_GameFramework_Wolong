using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component fires events if a finger has been held for a certain amount of time without moving.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanFingerHeld")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Finger Held")]
	public class LeanFingerHeld : MonoBehaviour
	{
		[System.Serializable]
		public class FingerData : LeanFingerData
		{
			public bool    Eligible;
			public bool    Held;
			public Vector2 Movement;
		}

		[System.Serializable] public class LeanFingerEvent : UnityEvent<LeanFinger> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}

		/// <summary>Ignore fingers with StartedOverGui?</summary>
		public bool IgnoreStartedOverGui { set { ignoreStartedOverGui = value; } get { return ignoreStartedOverGui; } } [FSA("IgnoreStartedOverGui")] [SerializeField] private bool ignoreStartedOverGui = true;

		/// <summary>Ignore fingers with OverGui?</summary>
		public bool IgnoreIsOverGui { set { ignoreIsOverGui = value; } get { return ignoreIsOverGui; } } [FSA("IgnoreIsOverGui")] [SerializeField] private bool ignoreIsOverGui;

		/// <summary>If the specified object is set and isn't selected, then this component will do nothing.</summary>
		public LeanSelectable RequiredSelectable { set { requiredSelectable = value; } get { return requiredSelectable; } } [FSA("RequiredSelectable")] [SerializeField] private LeanSelectable requiredSelectable;

		/// <summary>The finger must be held for this many seconds.</summary>
		public float MinimumAge { set { minimumAge = value; } get { return minimumAge; } } [FSA("MinimumAge")] [SerializeField] private float minimumAge = 1.0f;

		/// <summary>The finger cannot move more than this many pixels relative to the reference DPI.</summary>
		public float MaximumMovement { set { maximumMovement = value; } get { return maximumMovement; } } [FSA("MaximumMovement")] [SerializeField] private float maximumMovement = 5.0f;

		/// <summary>Called on the first frame the conditions are met.</summary>
		public LeanFingerEvent OnFingerDown { get { if (onFingerDown == null) onFingerDown = new LeanFingerEvent(); return onFingerDown; } } [FSA("onHeldDown")] [FSA("OnHeldDown")] [SerializeField] private LeanFingerEvent onFingerDown;

		/// <summary>Called on every frame the conditions are met.</summary>
		public LeanFingerEvent OnFingerUpdate { get { if (onFingerUpdate == null) onFingerUpdate = new LeanFingerEvent(); return onFingerUpdate; } } [FSA("onFingerSet")] [FSA("onHeldSet")] [FSA("OnHeldSet")] [SerializeField] private LeanFingerEvent onFingerUpdate;

		/// <summary>Called on the last frame the conditions are met.</summary>
		public LeanFingerEvent OnFingerUp { get { if (onFingerUp == null) onFingerUp = new LeanFingerEvent(); return onFingerUp; } } [FSA("onHeldUp")] [FSA("OnHeldUp")] [SerializeField] private LeanFingerEvent onFingerUp;

		/// <summary>The method used to find world coordinates from a finger. See LeanScreenDepth documentation for more information.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.DepthIntercept);

		/// <summary>Called on the first frame the conditions are met.
		/// Vector3 = Start point based on the ScreenDepth settings.</summary>
		public Vector3Event OnWorldDown { get { if (onWorldDown == null) onWorldDown = new Vector3Event(); return onWorldDown; } } [FSA("onPositionDown")] [SerializeField] private Vector3Event onWorldDown;

		/// <summary>Called on the first frame the conditions are met.
		/// Vector3 = Current point based on the ScreenDepth settings.</summary>
		public Vector3Event OnWorldUpdate { get { if (onWorldUpdate == null) onWorldUpdate = new Vector3Event(); return onWorldUpdate; } } [FSA("onWorldSet")] [FSA("onPositionSet")] [SerializeField] private Vector3Event onWorldUpdate;

		/// <summary>Called on the first frame the conditions are met.
		/// Vector3 = End point based on the ScreenDepth settings.</summary>
		public Vector3Event OnWorldUp { get { if (onWorldUp == null) onWorldUp = new Vector3Event(); return onWorldUp; } } [FSA("onPositionUp")] [SerializeField] private Vector3Event onWorldUp;

		// Additional finger data
		[SerializeField]
		private List<FingerData> fingerDatas = new List<FingerData>();

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
			LeanTouch.OnFingerDown   += HandleFingerDown;
			LeanTouch.OnFingerUpdate += HandleFingerUpdate;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown   -= HandleFingerDown;
			LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
		}

		private void HandleFingerDown(LeanFinger finger)
		{
			if (ignoreStartedOverGui == true && finger.StartedOverGui == true)
			{
				return;
			}

			if (finger.Index == LeanTouch.HOVER_FINGER_INDEX)
			{
				return;
			}

			// Get link for this finger and reset
			var fingerData = LeanFingerData.FindOrCreate(ref fingerDatas, finger);

			fingerData.Eligible = true;
			fingerData.Held     = false;
			fingerData.Movement = Vector2.zero;
		}

		private void HandleFingerUpdate(LeanFinger finger)
		{
			// Try and find the link for this finger
			var fingerData = LeanFingerData.Find(fingerDatas, finger);

			if (fingerData != null)
			{
				fingerData.Movement += finger.ScaledDelta;

				if (fingerData.Movement.magnitude > maximumMovement)
				{
					fingerData.Eligible = false;
				}

				if (IsHeld(finger, fingerData) == true)
				{
					if (fingerData.Held == false)
					{
						fingerData.Held = true;

						InvokeDown(finger);
					}

					InvokeUpdate(finger);
				}
				else if (fingerData.Held == true)
				{
					InvokeUp(finger);

					fingerDatas.Remove(fingerData);
				}
				else if (finger.Set == false)
				{
					fingerDatas.Remove(fingerData);
				}
			}
		}

		private bool IsHeld(LeanFinger finger, FingerData fingerData)
		{
			if (ignoreIsOverGui == true && finger.IsOverGui == true)
			{
				return false;
			}

			if (requiredSelectable != null && requiredSelectable.IsSelected == false)
			{
				return false;
			}

			return fingerData.Eligible == true && finger.Age >= minimumAge && finger.Set == true;
		}

		private void InvokeDown(LeanFinger finger)
		{
			if (onFingerDown != null)
			{
				onFingerDown.Invoke(finger);
			}

			if (onWorldDown != null)
			{
				var position = ScreenDepth.Convert(finger.ScreenPosition, gameObject);

				onWorldDown.Invoke(position);
			}
		}

		private void InvokeUpdate(LeanFinger finger)
		{
			if (onFingerUpdate != null)
			{
				onFingerUpdate.Invoke(finger);
			}

			if (onWorldUpdate != null)
			{
				var position = ScreenDepth.Convert(finger.ScreenPosition, gameObject);

				onWorldUpdate.Invoke(position);
			}
		}

		private void InvokeUp(LeanFinger finger)
		{
			if (onFingerUp != null)
			{
				onFingerUp.Invoke(finger);
			}

			if (onWorldUp != null)
			{
				var position = ScreenDepth.Convert(finger.ScreenPosition, gameObject);

				onWorldUp.Invoke(position);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanFingerHeld;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanFingerHeld_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("ignoreStartedOverGui", "Ignore fingers with StartedOverGui?");
			Draw("ignoreIsOverGui", "Ignore fingers with OverGui?");
			Draw("requiredSelectable", "If the specified object is set and isn't selected, then this component will do nothing.");
			Draw("minimumAge", "The finger must be held for this many seconds.");
			Draw("maximumMovement", "The finger cannot move more than this many pixels relative to the reference DPI.");

			Separator();

			var usedA = Any(tgts, t => t.OnFingerDown.GetPersistentEventCount() > 0);
			var usedB = Any(tgts, t => t.OnFingerUpdate.GetPersistentEventCount() > 0);
			var usedC = Any(tgts, t => t.OnFingerUp.GetPersistentEventCount() > 0);
			var usedD = Any(tgts, t => t.OnWorldDown.GetPersistentEventCount() > 0);
			var usedE = Any(tgts, t => t.OnWorldUpdate.GetPersistentEventCount() > 0);
			var usedF = Any(tgts, t => t.OnWorldUp.GetPersistentEventCount() > 0);

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onFingerDown");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onFingerUpdate");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onFingerUp");
			}

			if (usedD == true || usedE == true || usedF == true || showUnusedEvents == true)
			{
				Draw("ScreenDepth");
			}

			if (usedD == true || showUnusedEvents == true)
			{
				Draw("onWorldDown");
			}

			if (usedE == true || showUnusedEvents == true)
			{
				Draw("onWorldUpdate");
			}

			if (usedF == true || showUnusedEvents == true)
			{
				Draw("onWorldUp");
			}
		}
	}
}
#endif