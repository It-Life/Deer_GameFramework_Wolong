using UnityEngine;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to zoom a camera in and out based on the pinch gesture
	/// This supports both perspective and orthographic cameras</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanPinchCamera")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Pinch Camera")]
	public class LeanPinchCamera : MonoBehaviour
	{
		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>The camera this component will calculate using.
		/// None = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [FSA("Camera")] [SerializeField] private Camera _camera;

		/// <summary>The current FOV/Size.</summary>
		public float Zoom { set { zoom = value; } get { return zoom; } } [FSA("Zoom")] [SerializeField] private float zoom = 50.0f;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping { set { damping = value; } get { return damping; } } [FSA("Damping")] [FSA("Dampening")] [SerializeField] private float damping = -1.0f;

		/// <summary>Limit the FOV/Size?</summary>
		public bool Clamp { set { clamp = value; } get { return clamp; } } [FSA("ZoomClamp")] [FSA("Clamp")] [SerializeField] private bool clamp;

		/// <summary>The minimum FOV/Size we want to zoom to.</summary>
		public float ClampMin { set { clampMin = value; } get { return clampMin; } } [FSA("ZoomMin")] [FSA("ClampMin")] [SerializeField] private float clampMin = 10.0f;

		/// <summary>The maximum FOV/Size we want to zoom to.</summary>
		public float ClampMax { set { clampMax = value; } get { return clampMax; } } [FSA("ZoomMax")] [FSA("ClampMax")] [SerializeField] private float clampMax = 60.0f;

		/// <summary>Should the zoom be performed relative to the finger center?</summary>
		public bool Relative { set { relative = value; } get { return relative; } } [FSA("Relative")] [SerializeField] private bool relative;

		/// <summary>Ignore changes in Z translation for 2D?</summary>
		public bool IgnoreZ { set { ignoreZ = value; } get { return ignoreZ; } } [FSA("IgnoreZ")] [SerializeField] private bool ignoreZ;

		/// <summary>The method used to find world coordinates from a finger. See LeanScreenDepth documentation for more information.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.DepthIntercept);

		[SerializeField]
		private float currentZoom;

		[SerializeField]
		private Vector3 remainingTranslation;

		public void ContinuouslyZoom(float direction)
		{
			var factor = LeanHelper.GetDampenFactor(Mathf.Abs(direction), Time.deltaTime);

			if (direction > 0.0f)
			{
				zoom = Mathf.Lerp(zoom, clampMax, factor);
			}
			else if (direction <= 0.0f)
			{
				zoom = Mathf.Lerp(zoom, clampMin, factor);
			}
		}

		/// <summary>This method allows you to multiply the current <b>Zoom</b> value by the specified scale. This is useful for quickly changing the zoom from UI button clicks, or <b>LeanMouseWheel</b> scrolling.</summary>
		public void MultiplyZoom(float scale)
		{
			zoom *= scale;

			if (clamp == true)
			{
				zoom = Mathf.Clamp(zoom, clampMin, clampMax);
			}
		}

		/// <summary>This method allows you to multiply the current <b>Zoom</b> value by the specified delta. This works like <b>MultiplyZoom</b>, except a value of 0 will result in no change, -1 will halve the zoom, 2 will double the zoom, etc.</summary>
		public void IncrementZoom(float delta)
		{
			var scale = 1.0f + Mathf.Abs(delta);

			if (delta < 0.0f)
			{
				scale = 1.0f / scale;
			}

			MultiplyZoom(scale);
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

		protected virtual void Start()
		{
			currentZoom = zoom;
		}

		protected virtual void LateUpdate()
		{
			// Get the fingers we want to use
			var fingers = Use.UpdateAndGetFingers();

			// Get the pinch ratio of these fingers
			var pinchRatio = LeanGesture.GetPinchRatio(fingers);

			// Store
			var oldPosition = transform.localPosition;

			// Make sure the zoom value is valid
			zoom = TryClamp(zoom);

			if (pinchRatio != 1.0f)
			{
				// Store old zoom value and then modify zoom
				var oldZoom = zoom;

				zoom = TryClamp(zoom * pinchRatio);

				// Zoom relative to a point on screen?
				if (relative == true)
				{
					var screenPoint = default(Vector2);

					if (LeanGesture.TryGetScreenCenter(fingers, ref screenPoint) == true)
					{
						// Derive actual pinchRatio from the zoom delta (it may differ with clamping)
						pinchRatio = zoom / oldZoom;

						var worldPoint = ScreenDepth.Convert(screenPoint);

						transform.position = worldPoint + (transform.position - worldPoint) * pinchRatio;

						// Increment
						remainingTranslation += transform.localPosition - oldPosition;

						if (ignoreZ == true)
						{
							remainingTranslation.z = 0.0f;
						}
					}
				}
			}

			// Get t value
			var factor = LeanHelper.GetDampenFactor(damping, Time.deltaTime);

			// Lerp the current value to the target one
			currentZoom = Mathf.Lerp(currentZoom, zoom, factor);

			// Set the new zoom
			SetZoom(currentZoom);

			// Dampen remainingDelta
			var newRemainingTranslation = Vector3.Lerp(remainingTranslation, Vector3.zero, factor);

			// Shift this transform by the change in delta
			transform.localPosition = oldPosition + remainingTranslation - newRemainingTranslation;

			// Update remainingDelta with the dampened value
			remainingTranslation = newRemainingTranslation;
		}

		protected void SetZoom(float current)
		{
			// Make sure the camera exists
			var camera = LeanHelper.GetCamera(_camera, gameObject);

			if (camera != null)
			{
				if (camera.orthographic == true)
				{
					camera.orthographicSize = current;
				}
				else
				{
					camera.fieldOfView = current;
				}
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}
		}

		private float TryClamp(float z)
		{
			if (clamp == true)
			{
				z = Mathf.Clamp(z, clampMin, clampMax);
			}

			return z;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanPinchCamera;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanPinchCamera_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Use");
			Draw("_camera", "The camera that will be used during calculations.\n\nNone = MainCamera.");
			Draw("zoom", "The current FOV/Size.");
			Draw("damping", "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
			Draw("clamp", "Limit the FOV/Size?");

			if (Any(tgts, t => t.Clamp == true))
			{
				BeginIndent();
					Draw("clampMin", "The minimum FOV/Size we want to zoom to.", "Min");
					Draw("clampMax", "The maximum FOV/Size we want to zoom to.", "Max");
				EndIndent();
			}

			Draw("relative", "Should the zoom be performed relative to the finger center?");

			if (Any(tgts, t => t.Relative == true))
			{
				BeginIndent();
					Draw("ignoreZ", "Ignore changes in Z translation for 2D?");
					Draw("ScreenDepth");
				EndIndent();
			}
		}
	}
}
#endif