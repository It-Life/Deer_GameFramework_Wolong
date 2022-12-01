using UnityEngine;
using UnityEngine.Events;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to get the pinch of all fingers.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanMultiPinch")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Multi Pinch")]
	public class LeanMultiPinch : MonoBehaviour
	{
		public enum CoordinateType
		{
			OneBasedScale,
			OneBasedRatio,
			ZeroBasedScale,
			ZeroBasedRatio,
			ZeroBasedDistance
		}

		[System.Serializable] public class FloatEvent : UnityEvent<float> {}

		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>If there is no pinching, ignore it?</summary>
		public bool IgnoreIfStatic { set { ignoreIfStatic = value; } get { return ignoreIfStatic; } } [FSA("IgnoreIfStatic")] [SerializeField] private bool ignoreIfStatic;

		/// <summary>OneBasedScale = Scale (1 = no change, 2 = double size, 0.5 = half size).
		/// OneBasedReciprocal = 1 / Scale (1 = no change, 2 = half size, 0.5 = double size).
		/// ZeroBasedScale = Scale - 1 (0 = no change, 1 = double size, -0.5 = half size).
		/// ZeroBasedRatio = 1 / Scale - 1 (0 = no change, 1 = half size, -0.5 = double size).
		/// ZeroBasedDistance = Linear change in distance (0 = no change, 10 = 10 pixel larger radius, -10 = 10 pixel smaller radius).</summary>
		public CoordinateType Coordinate { set { coordinate = value; } get { return coordinate; } } [FSA("Coordinate")] [SerializeField] private CoordinateType coordinate;

		/// <summary>The swipe delta will be multiplied by this value.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [FSA("Multiplier")] [SerializeField] private float multiplier = 1.0f;

		/// <summary>This event is invoked when the requirements are met.
		/// Float = Pinch value based on your Scale setting.</summary>
		public FloatEvent OnPinch { get { if (onPinch == null) onPinch = new FloatEvent(); return onPinch; } } [SerializeField] private FloatEvent onPinch;

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

			if (fingers.Count > 1 && onPinch != null)
			{
				switch (coordinate)
				{
					case CoordinateType.OneBasedScale:
					{
						var scale = LeanGesture.GetPinchScale(fingers);

						scale = Mathf.Pow(scale, multiplier);

						onPinch.Invoke(scale);
					}
					break;

					case CoordinateType.OneBasedRatio:
					{
						var ratio = LeanGesture.GetPinchRatio(fingers);

						ratio = Mathf.Pow(ratio, multiplier);

						onPinch.Invoke(ratio);
					}
					break;

					case CoordinateType.ZeroBasedScale:
					{
						var scale = LeanGesture.GetPinchScale(fingers);

						scale = (scale - 1.0f) * multiplier;

						onPinch.Invoke(scale);
					}
					break;

					case CoordinateType.ZeroBasedRatio:
					{
						var ratio = LeanGesture.GetPinchRatio(fingers);

						ratio = (ratio - 1.0f) * multiplier;

						onPinch.Invoke(ratio);
					}
					break;

					case CoordinateType.ZeroBasedDistance:
					{
						var oldDistance = LeanGesture.GetLastScaledDistance(fingers, LeanGesture.GetLastScreenCenter(fingers));
						var newDistance = LeanGesture.GetScaledDistance(fingers, LeanGesture.GetScreenCenter(fingers));
						var movement    = (newDistance - oldDistance) * multiplier;

						onPinch.Invoke(movement);
					}
					break;
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanMultiPinch;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanMultiPinch_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Use");

			Separator();

			Draw("ignoreIfStatic", "If there is no pinching, ignore it?");
			Draw("coordinate", "OneBasedScale = Scale (1 = no change, 2 = double size, 0.5 = half size).\n\nOneBasedReciprocal = 1 / Scale (1 = no change, 2 = half size, 0.5 = double size).\n\nZeroBasedScale = Scale - 1 (0 = no change, 1 = double size, -0.5 = half size).\n\nZeroBasedRatio = 1 / Scale - 1 (0 = no change, 1 = half size, -0.5 = double size).\n\nZeroBasedDistance = Linear change in distance (0 = no change, 10 = 10 pixel larger radius, -10 = 10 pixel smaller radius).");
			Draw("multiplier", "The swipe delta will be multiplied by this value.");

			Separator();

			Draw("onPinch");
		}
	}
}
#endif