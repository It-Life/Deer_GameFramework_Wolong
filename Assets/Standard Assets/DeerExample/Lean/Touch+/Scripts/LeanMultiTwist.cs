using UnityEngine;
using UnityEngine.Events;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to get the twist of all fingers.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanMultiTwist")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Multi Twist")]
	public class LeanMultiTwist : MonoBehaviour
	{
		public enum OneFingerType
		{
			None,
			ScreenCenter,
			FingerStart
		}

		// Event signature
		[System.Serializable] public class FloatEvent : UnityEvent<float> {}

		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>If there is no twisting, ignore the events?</summary>
		public bool IgnoreIfStatic { set { ignoreIfStatic = value; } get { return ignoreIfStatic; } } [FSA("IgnoreIfStatic")] [SerializeField] private bool ignoreIfStatic;

		/// <summary>Should this component allow one finger twisting?
		/// ScreenCenter = The twist pivot point will be the center of the screen.
		/// FingerStart = The twist pivot point will be the finger start position.</summary>
		public OneFingerType OneFinger { set { oneFinger = value; } get { return oneFinger; } } [FSA("OneFinger")] [SerializeField] private OneFingerType oneFinger;

		public FloatEvent OnTwistDegrees { get { if (onTwistDegrees == null) onTwistDegrees = new FloatEvent(); return onTwistDegrees; } } [UnityEngine.Serialization.FormerlySerializedAs("onTwist")] [SerializeField] private FloatEvent onTwistDegrees;

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
				// Get twist
				var degrees = 0.0f;

				if (fingers.Count > 1)
				{
					degrees = LeanGesture.GetTwistDegrees(fingers);
				}
				else if (oneFinger != OneFingerType.None)
				{
					var firstFinger    = fingers[0];
					var referencePoint = oneFinger == OneFingerType.FingerStart ? firstFinger.StartScreenPosition : new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

					degrees += firstFinger.GetDeltaDegrees(referencePoint, referencePoint);
				}

				// Ignore?
				if (ignoreIfStatic == true && degrees == 0.0f)
				{
					return;
				}

				// Call events
				if (onTwistDegrees != null)
				{
					onTwistDegrees.Invoke(degrees);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanMultiTwist;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanDestroy_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Use");
			Draw("ignoreIfStatic", "If there is no twisting, ignore the events?");
			Draw("oneFinger", "Should this component allow one finger twisting?\n\nScreenCenter = The twist pivot point will be the center of the screen.\n\nFingerStart = The twist pivot point will be the finger start position.");

			Separator();

			Draw("onTwistDegrees");
		}
	}
}
#endif