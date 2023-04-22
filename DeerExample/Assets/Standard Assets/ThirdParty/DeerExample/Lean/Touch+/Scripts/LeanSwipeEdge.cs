using UnityEngine;
using UnityEngine.Events;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to detect when a finger swipes from the edge of the screen.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanSwipeEdge")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Swipe Edge")]
	public class LeanSwipeEdge : MonoBehaviour
	{
		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>Detect swipes coming from the left edge?</summary>
		public bool Left { set { left = value; } get { return left; } } [FSA("Left")] [SerializeField] private bool left = true;

		/// <summary>Detect swipes coming from the right edge?</summary>
		public bool Right { set { right = value; } get { return right; } } [FSA("Right")] [SerializeField] private bool right = true;

		/// <summary>Detect swipes coming from the bottom edge?</summary>
		public bool Bottom { set { bottom = value; } get { return bottom; } } [FSA("Bottom")] [SerializeField] private bool bottom = true;

		/// <summary>Detect swipes coming from the top edge?</summary>
		public bool Top { set { top = value; } get { return top; } } [FSA("Top")] [SerializeField] private bool top = true;

		/// <summary>If the swipe angle is off by this many degrees, it will be ignored.
		/// 0 = Must be exactly parallel.</summary>
		public float AngleThreshold { set { angleThreshold = value; } get { return angleThreshold; } } [FSA("AngleThreshold")] [SerializeField] [Range(1.0f, 90.0f)] private float angleThreshold = 10.0f;

		/// <summary>The swipe must begin within this many scaled pixels of the edge of the screen.</summary>
		public float EdgeThreshold { set { edgeThreshold = value; } get { return edgeThreshold; } } [FSA("EdgeThreshold")] [SerializeField] private float edgeThreshold = 10.0f;

		public UnityEvent OnEdge { get { if (onEdge == null) onEdge = new UnityEvent(); return onEdge; } } [SerializeField] private UnityEvent onEdge;

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually check for a swipe.</summary>
		public void CheckBetween(Vector2 from, Vector2 to)
		{
			var rect   = new Rect(0, 0, Screen.width, Screen.height);
			var vector = (to - from).normalized;

			if (left == true && CheckAngle(vector, Vector2.right) == true && CheckEdge(from.x - rect.xMin) == true)
			{
				InvokeEdge(); return;
			}
			else if (right == true && CheckAngle(vector, -Vector2.right) == true && CheckEdge(from.x - rect.xMax) == true)
			{
				InvokeEdge(); return;
			}
			else if (bottom == true && CheckAngle(vector, Vector2.up) == true && CheckEdge(from.y - rect.yMin) == true)
			{
				InvokeEdge(); return;
			}
			else if (top == true && CheckAngle(vector, -Vector2.up) == true && CheckEdge(from.y - rect.yMax) == true)
			{
				InvokeEdge(); return;
			}
		}

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
			// Get the fingers we want to use
			var fingers = Use.UpdateAndGetFingers();

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger.Swipe == true)
				{
					CheckBetween(finger.StartScreenPosition, finger.ScreenPosition);
				}
			}
		}

		private void InvokeEdge()
		{
			if (onEdge != null)
			{
				onEdge.Invoke();
			}
		}

		private bool CheckAngle(Vector2 a, Vector2 b)
		{
			return Vector2.Angle(a, b) <= angleThreshold;
		}

		private bool CheckEdge(float distance)
		{
			return Mathf.Abs(distance * LeanTouch.ScalingFactor) < edgeThreshold;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanSwipeEdge;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanSwipeEdge_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Use");

			Separator();

			Draw("left", "Detect swipes coming from the left edge?");
			Draw("right", "Detect swipes coming from the right edge?");
			Draw("bottom", "Detect swipes coming from the bottom edge?");
			Draw("top", "Detect swipes coming from the top edge?");

			Separator();

			Draw("angleThreshold", "If the swipe angle is off by this many degrees, it will be ignored.\n\n0 = Must be exactly parallel.");
			Draw("edgeThreshold", "The swipe must begin within this many scaled pixels of the edge of the screen.");

			Separator();

			Draw("onEdge");
		}
	}
}
#endif