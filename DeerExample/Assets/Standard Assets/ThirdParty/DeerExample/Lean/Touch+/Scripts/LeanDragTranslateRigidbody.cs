using UnityEngine;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to drag this rigidbody.</summary>
	[RequireComponent(typeof(Rigidbody))]
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanDragTranslateRigidbody")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Translate Rigidbody")]
	public class LeanDragTranslateRigidbody : MonoBehaviour
	{
		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>The camera this component will calculate using.
		/// None/null = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [FSA("Camera")] [SerializeField] private Camera _camera;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping { set { damping = value; } get { return damping; } } [FSA("Damping")] [FSA("Dampening")] [SerializeField] private float damping = 10.0f;

		[System.NonSerialized]
		private Rigidbody cachedRigidbody;

		private Camera cachedCamera;

		private bool targetSet;

		private Vector3 targetScreenPoint;

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

		protected virtual void OnEnable()
		{
			cachedRigidbody = GetComponent<Rigidbody>();
		}

		protected virtual void FixedUpdate()
		{
			// Make sure the camera exists and the targetScreenPoint is set
			if (cachedCamera != null && targetSet == true)
			{
				// Calculate required velocity to arrive in one FixedUpdate
				var oldPosition = transform.position;
				var newPosition = cachedCamera.ScreenToWorldPoint(targetScreenPoint);
				var velocity    = (newPosition - oldPosition) / Time.fixedDeltaTime;

				var factor = LeanHelper.GetDampenFactor(damping, Time.fixedDeltaTime);

				// Apply the velocity
				cachedRigidbody.velocity = velocity * factor;
			}
		}

		protected virtual void Update()
		{
			// Get the fingers we want to use
			var fingers = Use.UpdateAndGetFingers();

			// Make sure the camera exists
			cachedCamera = LeanHelper.GetCamera(_camera, gameObject);

			if (cachedCamera != null)
			{
				if (fingers.Count > 0)
				{
					// If it's the first frame the fingers are down, grab the current screen point of this GameObject
					if (targetSet == false)
					{
						targetSet         = true;
						targetScreenPoint = cachedCamera.WorldToScreenPoint(transform.position);
					}

					// Shift target point based on finger deltas
					// NOTE: targetScreenPoint.z already stores the depth
					targetScreenPoint += (Vector3)LeanGesture.GetScreenDelta(fingers);
				}
				// Unset if no fingers are down
				else
				{
					targetSet = false;
				}
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanDragTranslateRigidbody;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanDragTranslateRigidbody_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);
			
			Draw("Use");
			Draw("_camera", "The camera this component will calculate using.\n\nNone/null = MainCamera.");
			Draw("damping", "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
		}
	}
}
#endif