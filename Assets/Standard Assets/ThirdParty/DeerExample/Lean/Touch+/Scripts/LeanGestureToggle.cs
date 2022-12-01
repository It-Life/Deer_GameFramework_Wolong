using UnityEngine;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component will enable/disable the target pinch and twist components based on total pinch and twist gestures, like mobile map applications.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanGestureToggle")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Gesture Toggle")]
	public class LeanGestureToggle : MonoBehaviour
	{
		public enum StateType
		{
			None,
			Drag,
			Pinch,
			Twist
		}

		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>If one specific gesture hasn't been isolated yet, keep them all enabled?</summary>
		public bool EnableWithoutIsolation { set { enableWithoutIsolation = value; } get { return enableWithoutIsolation; } } [FSA("EnableWithoutIsolation")] [SerializeField] private bool enableWithoutIsolation;

		/// <summary>The component that will be enabled/disabled when dragging.</summary>
		public MonoBehaviour DragComponent { set { dragComponent = value; } get { return dragComponent; } } [FSA("DragComponent")] [SerializeField] private MonoBehaviour dragComponent;

		/// <summary>The amount of drag required to enable dragging mode.</summary>
		public float DragThreshold { set { dragThreshold = value; } get { return dragThreshold; } } [FSA("DragThreshold")] [SerializeField] private float dragThreshold = 50.0f;

		/// <summary>The component that will be enabled/disabled when pinching.</summary>
		public MonoBehaviour PinchComponent { set { pinchComponent = value; } get { return pinchComponent; } } [FSA("PinchComponent")] [SerializeField] private MonoBehaviour pinchComponent;

		/// <summary>The amount of pinch required to enable twisting in scale (e.g. 0.1 = 0.9 to 1.1).</summary>
		public float PinchThreshold { set { pinchThreshold = value; } get { return pinchThreshold; } } [FSA("PinchThreshold")] [SerializeField] private float pinchThreshold = 0.1f;

		/// <summary>The component that will be enabled/disabled when twisting.</summary>
		public MonoBehaviour TwistComponent { set { twistComponent = value; } get { return twistComponent; } } [FSA("TwistComponent")] [SerializeField] private MonoBehaviour twistComponent;

		/// <summary>The amount of twist required to enable twisting in degrees.</summary>
		public float TwistThreshold { set { twistThreshold = value; } get { return twistThreshold; } } [FSA("TwistThreshold")] [SerializeField] private float twistThreshold = 5.0f;

		/// <summary>Enable twist component when pinch component is activated?</summary>
		public bool TwistWithPinch { set { twistWithPinch = value; } get { return twistWithPinch; } } [FSA("TwistWithPinch")] [SerializeField] private bool twistWithPinch;

		[System.NonSerialized]
		private StateType state;

		[System.NonSerialized]
		private Vector2 delta;

		[System.NonSerialized]
		private float scale = 1.0f;

		[System.NonSerialized]
		private float twist;

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
				delta += LeanGesture.GetScaledDelta(fingers);
				scale *= LeanGesture.GetPinchRatio(fingers);
				twist += LeanGesture.GetTwistDegrees(fingers);

				if (state == StateType.None)
				{
					if (dragComponent != null && delta.magnitude >= dragThreshold)
					{
						state = StateType.Drag;
					}
					else if (pinchComponent != null && Mathf.Abs(scale - 1.0f) >= pinchThreshold)
					{
						state = StateType.Pinch;
					}
					else if (twistComponent != null && Mathf.Abs(twist) >= twistThreshold)
					{
						state = StateType.Twist;
					}
				}
			}
			else
			{
				state = StateType.None;
				delta = Vector2.zero;
				scale = 1.0f;
				twist = 0.0f;
			}

			if (dragComponent != null)
			{
				dragComponent.enabled = state == StateType.Drag || (enableWithoutIsolation == true && state == StateType.None);
			}

			if (pinchComponent != null)
			{
				pinchComponent.enabled = state == StateType.Pinch || (enableWithoutIsolation == true && state == StateType.None);
			}

			if (twistComponent != null)
			{
				twistComponent.enabled = state == StateType.Twist || (enableWithoutIsolation == true && state == StateType.None) || (twistWithPinch == true && state == StateType.Pinch);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanGestureToggle;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanGestureToggle_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Use");
			Draw("enableWithoutIsolation", "If one specific gesture hasn't been isolated yet, keep them all enabled?");

			Separator();

			Draw("dragComponent", "The component that will be enabled/disabled when dragging.");
			Draw("dragThreshold", "The amount of drag required to enable dragging mode.");

			Separator();

			Draw("pinchComponent", "The component that will be enabled/disabled when pinching.");
			Draw("pinchThreshold", "The amount of pinch required to enable twisting in scale (e.g. 0.1 = 0.9 to 1.1).");

			Separator();

			Draw("twistComponent", "The component that will be enabled/disabled when twisting.");
			Draw("twistThreshold", "The amount of twist required to enable twisting in degrees.");
			Draw("twistWithPinch", "Enable twist component when pinch component is activated?");
		}
	}
}
#endif